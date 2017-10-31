using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Класс для расщепления сумм фактов по нормативам отчислений доходов

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region Поля

        protected IDbDataAdapter daDisintegratedRulesKD;
        protected IDbDataAdapter daDisintegratedRulesEX;
        protected IDbDataAdapter daDisintegratedRulesALTKD;
        protected DataSet dsDisintRules;
        protected IDbDataAdapter daSourceData;
        protected DataSet dsSourceData;
        protected IDbDataAdapter daDisintegratedData;
        protected DataSet dsDisintegratedData;
        // ОКАТО
        protected IDbDataAdapter daOKATO;
        protected DataSet dsOKATO;
        // ОКАТО.Сопоставимый
        protected IDbDataAdapter daOKATOBridge;
        protected DataSet dsOKATOBridge;

        protected IFactTable fctSourceTable;
        protected IFactTable fctDisintegratedData;
        protected IClassifier clsKD;
        protected IClassifier clsOKATO;

        protected IClassifier brdOKATO;

        protected string refDateFieldName;
        protected string refKDFieldName;
        protected string refOkatoFieldName;

        // Счетчик расщепленных записей
        protected int disintCount = 0;
        protected int totalRecsForSourceID = 0;
        protected int currentOkatoID;
        protected string currentOkatoCode;

        // Кэш правил расщепления: ключ - год, значение - список правил (ключ - код КД, значение - строка правила)
        protected Dictionary<int, Dictionary<string, DataRow>> disintRulesCache =
            new Dictionary<int, Dictionary<string, DataRow>>(5);
        protected bool disintRulesIsEmpty = false;

        protected DataSet dsMessages;
        protected Dictionary<string, DataRow> messagesCache = new Dictionary<string, DataRow>(1000);

        // имя поля - признак для закачки
        protected string disintFlagFieldName = "REFISDISINT";
        protected string refBudgetLevelFieldName = "REFBUDGETLEVELS";
        protected string disintDateConstraint = string.Empty;
        #endregion Поля


        #region Константы

        /// <summary>
        /// Количество записей, с которым будем работать при расщеплении
        /// </summary>
        protected const int constMaxQueryRecordsForDisint = 50000;

        #endregion Константы


        #region Функции для работы с кэшем правил расщепления

        /// <summary>
        /// Добавляет запись в кэш правил расщепления
        /// </summary>
        /// <param name="kd">КД</param>
        /// <param name="year">Год</param>
        /// <param name="row">Строка правил расщепления</param>
        private void AddRuleToCache(Dictionary<int, Dictionary<string, DataRow>> cache, string kd, int year, 
            DataRow row)
        {
            if (!cache[year].ContainsKey(kd))
            {
                cache[year].Add(kd, row);
            }
        }

        /// <summary>
        /// Заполняет кэш правил расщепления
        /// </summary>
        protected void FillDisintRulesCache()
        {
            QueryDisintRules();

            disintRulesCache.Clear();

            DataTable dt = dsDisintRules.Tables["disintrules_kd"];
            DataRow[] rows = dt.Select(string.Empty, "YEAR ASC");
            int year = -1;

            int count = rows.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (year != Convert.ToInt32(rows[i]["YEAR"]))
                {
                    year = Convert.ToInt32(rows[i]["YEAR"]);
                    disintRulesCache.Add(year, new Dictionary<string, DataRow>(1000));
                }

                AddRuleToCache(disintRulesCache, Convert.ToString(rows[i]["KD"]), year, rows[i]);

                // В этот же кэш пишем и альтернативные коды
                DataRow[] altKdRows = rows[i].GetChildRows("KD_ALTKD");
                int kdRowsCount = altKdRows.GetLength(0);
                for (int j = 0; j < kdRowsCount; j++)
                {
                    AddRuleToCache(disintRulesCache, Convert.ToString(altKdRows[j]["KD"]), year, rows[i]);
                }
            }
        }

        /// <summary>
        /// Ищет правило расщепления
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="kd">Код КД</param>
        /// <returns>Строка правила расщепления</returns>
        protected DataRow FindDisintRule(Dictionary<int, Dictionary<string, DataRow>> cache, int year, string kd)
        {
            if (cache == null || !cache.ContainsKey(year)) return null;

            // Если КД длинный (=новый), то выкидываем первые три цифры - администратора бюджетных средств
            // Код программы не учитывается, потому выбрасываем его
            string restrictedKD = kd;
            if (kd.Length == 20)
            {
                restrictedKD = kd.Remove(0, 3);
            }

            if (cache.ContainsKey(year))
            {
                // Сначала ищем с учетом кода программ
                if (cache[year].ContainsKey(restrictedKD))
                {
                    return cache[year][restrictedKD];
                }
                // Если не нашли, то ищем с нулевым кодом программ
                else
                {
                    if (kd.Length == 20)
                    {
                        restrictedKD = restrictedKD.Remove(10, 4).Insert(10, "0000");
                        if (cache[year].ContainsKey(restrictedKD))
                        {
                            return cache[year][restrictedKD];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Инициализация датасета правил расщепления
        /// </summary>
        private void QueryDisintRules()
        {
            DataRelation rel;

            this.ClearDataSet(ref dsDisintRules);

            // Инициализация адаптеров
            // Основные КД
            InitLocalDataAdapter(this.DB, ref daDisintegratedRulesKD,
                "select k.ID, k.KD, k.YEAR, k.BYBUDGET, k.FED_PERCENT, k.CONS_PERCENT, k.SUBJ_PERCENT, " +
                "       k.CONSMR_PERCENT, k.MR_PERCENT, k.STAD_PERCENT, k.OUTOFBUDGETFOND_PERCENT, " +
                "       k.SMOLENSKACCOUNT_PERCENT, k.TUMENACCOUNT_PERCENT, k.CONSMO_PERCENT, k.GO_PERCENT " +
                "from DISINTRULES_KD k " +
                "order by k.KD asc");
            daDisintegratedRulesKD.Fill(dsDisintRules);
            dsDisintRules.Tables["Table"].TableName = "disintrules_kd";

            if (dsDisintRules.Tables["disintrules_kd"].Rows.Count == 0)
            {
                //throw new Exception("Отсутствуют правила расщепления");
            }

            // Уточнения
            InitLocalDataAdapter(this.DB, ref daDisintegratedRulesEX,
                "select e.INIT_DATE, e.REGION, e.FED_PERCENT, e.CONS_PERCENT, e.SUBJ_PERCENT, e.CONSMR_PERCENT, " +
                "       e.MR_PERCENT, e.STAD_PERCENT, e.OUTOFBUDGETFOND_PERCENT, e.SMOLENSKACCOUNT_PERCENT, " +
                "       e.TUMENACCOUNT_PERCENT, e.CONSMO_PERCENT, e.GO_PERCENT, e.REFDISINTRULES_KD " +
                "from DISINTRULES_EX e " +
                "order by e.INIT_DATE asc");
            daDisintegratedRulesEX.Fill(dsDisintRules);
            dsDisintRules.Tables["Table"].TableName = "disintrules_ex";

            // Дополнительные КД
            InitLocalDataAdapter(this.DB, ref daDisintegratedRulesALTKD,
                "select a.KD, a.REFDISINTRULES_KD from DISINTRULES_ALTKD a");
            daDisintegratedRulesALTKD.Fill(dsDisintRules);
            dsDisintRules.Tables["Table"].TableName = "disintrules_altkd";

            // Создаем отношения между таблицами датасета
            rel = new DataRelation(
                "KD_EX",
                dsDisintRules.Tables["disintrules_kd"].Columns["ID"],
                dsDisintRules.Tables["disintrules_ex"].Columns["refdisintrules_kd"]);
            dsDisintRules.Relations.Add(rel);

            rel = new DataRelation(
                "KD_ALTKD",
                dsDisintRules.Tables["disintrules_kd"].Columns["ID"],
                dsDisintRules.Tables["disintrules_altkd"].Columns["refdisintrules_kd"]);
            dsDisintRules.Relations.Add(rel);
        }

        /// <summary>
        /// Инициализация датасета для хранения записей лога (чтобы не писать в него повторяющиеся записи)
        /// </summary>
        protected void PrepareMessagesDS()
        {
            this.ClearDataSet(ref dsMessages);
            if (messagesCache != null)
                messagesCache.Clear();

            dsMessages.Tables.Add("Messages");
            dsMessages.Tables[0].Columns.Add("PumpID", typeof(string));
            dsMessages.Tables[0].Columns.Add("SOURCEID", typeof(string));
            dsMessages.Tables[0].Columns.Add("KD", typeof(string));
            dsMessages.Tables[0].Columns.Add("YEAR", typeof(int));
            dsMessages.Tables[0].Columns.Add("DATE", typeof(int));
            dsMessages.Tables[0].Columns.Add("COUNTER", typeof(int));
        }

        /// <summary>
        /// Вносит запись в датасет с данными о пропущенных записях
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="year">Год</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="fullKD">Код КД</param>
        protected void WriteRecInMessagesDS(string date, int year, int sourceID, string fullKD)
        {
            if (disintRulesIsEmpty) return;

            string key = GetComplexCacheKey(new object[] { this.PumpID, sourceID, fullKD, year, date });

            if (!messagesCache.ContainsKey(key))
            {
                DataRow row = dsMessages.Tables[0].NewRow();
                row["PUMPID"] = this.PumpID;
                row["SOURCEID"] = sourceID;
                row["KD"] = fullKD;
                row["YEAR"] = year;
                row["DATE"] = date;
                row["COUNTER"] = 1;
                dsMessages.Tables[0].Rows.Add(row);

                messagesCache.Add(key, row);
            }
            else
            {
                messagesCache[key]["COUNTER"] = Convert.ToInt32(messagesCache[key]["COUNTER"]) + 1;
            }
        }

        /// <summary>
        /// Сбрасывает сообщения о ненайденных расщеплениях в лог
        /// </summary>
        protected void UpdateMessagesDS()
        {
            if (disintRulesIsEmpty) return;

            if (dsMessages != null)
            {
                for (int i = 0; i < dsMessages.Tables[0].Rows.Count; i++)
                {
                    string msg = string.Format(
                        "Не найдено ни одного расщепления для КД = {0}, Год = {1}, Дата = {2} ({3} записей).",
                        dsMessages.Tables[0].Rows[i]["KD"],
                        dsMessages.Tables[0].Rows[i]["YEAR"],
                        dsMessages.Tables[0].Rows[i]["DATE"],
                        dsMessages.Tables[0].Rows[i]["COUNTER"]);

                    WriteEventIntoProcessDataProtocol(
                        ProcessDataEventKind.pdeWarning, 
                        Convert.ToInt32(dsMessages.Tables[0].Rows[i]["PUMPID"]),
                        Convert.ToInt32(dsMessages.Tables[0].Rows[i]["SOURCEID"]), 
                        msg, null);
                }
            }
        }

        #endregion Функции для работы с кэшем правил расщепления


        #region Функции расщепления данных

        /// <summary>
        /// Возвращает наиболее подходящую запись уточнения для базовой записи
        /// </summary>
        /// <param name="row">Базовая запись</param>
        /// <param name="date">Дата</param>
        /// <param name="okato">Код ОКАТО</param>
        /// <returns>Запись уточнения</returns>
        protected DataRow GetDisintExRow(DataRow row, int date, string okato)
        {
            DataRow realOkato = null;
            DataRow zeroOkato = null;

            DataRow[] rows = row.GetChildRows("KD_EX");

            // окато района - если нет норматива по окато поселения, то применяем норматив по окато района 
            // (первые 5 цифр окато поселения, остальные нули)
            string okatoRegion = string.Concat(okato.Substring(0, 5), "000000");

            for (int i = rows.GetLength(0) - 1; i >= 0; i--)
            {
                // Берем только те, где поле «дата, с которого действует уточнение» меньше или равна нашей дате
                if ((Convert.ToInt32(rows[i]["INIT_DATE"]) / 100) > (date / 100))
                    continue;

                string region = Convert.ToString(rows[i]["REGION"]);
                if (region == okato)
                {
                    // нашли норматив по заданному окато - выходим
                    realOkato = rows[i];
                    break;
                }
                if (region == okatoRegion)
                    // нашли норматив по окато района - цикл не завершаем, так как возможно есть норматив по заданному окато
                    realOkato = rows[i];
                // Запоминаем нулевое ОКАТО на случай, если не найдем заданного ОКАТО - тогда вернем нулевое
                else if (region.Trim('0') == string.Empty)
                    zeroOkato = rows[i];
            }
            if (realOkato != null)
                return realOkato;
            else
                return zeroOkato;
        }

        /// <summary>
        /// Определяет, применим ли указанный процент к определенной территории
        /// </summary>
        /// <param name="percentIndex">Порядковый номер процента расщепления</param>
        /// <param name="terrType">Тип территории</param>
        /// <returns>Применять или нет</returns>
        protected bool CheckPercentByTerrType(int percentIndex, int terrType)
        {
            // Для ОКАТО, у которых тип района = городской округ должны применяться нормативы из поля 
            // "% бюджет ГО" и не должно применяться расщепление "% конс.бюджет МР", "% бюджет района", 
            // "% бюджет поселения"
            if (terrType == 7 && (percentIndex >= 4 && percentIndex <= 6))
            {
                return false;
            }

            // Для ОКАТО, у которых тип района = муниципальный район должны применяться нормативы из поля 
            // "% конс.бюджет МР" и не должно применяться расщепление "% бюджет ГО"
            if (terrType == 4 && percentIndex == 15)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Расщепляет строку и добавляет ее в таблицу расщепленных
        /// </summary>
        /// <param name="sourceRow">Строка для расщепления</param>
        /// <param name="disintRow">Правило расщепления</param>
        /// <param name="fieldsForDisint">Массив полей с данными для расщепления</param>
        private void DisintegrateRow(DataRow sourceRow, DataRow disintRow, string[] fieldsForDisint)
        {
            DataRow row = null;
            DataRow okatoRow = GetOkatoRow(currentOkatoID);
            if (okatoRow == null)
                return;
            int terrType = Convert.ToInt32(okatoRow["REFTERRTYPE"]);
            // у неуказанного типа территории ничего не делаем
            if (terrType == 0)
                return;

            bool isUFK14Pump = (this.PumpProgramID == PumpProgramID.UFK14Pump);
            // закачка уфк - 14, у типов территорий 1, 2, 8 (фикс.типы территорий) - ничего не делаем
            if (isUFK14Pump)
                if ((terrType == 1) || (terrType == 2) || (terrType == 8))
                    return;

            // Обрабатываем все проценты расщепления
            for (int j = 1; j <= 15; j++)
            {
                if (j == 8)
                {
                    j = 12;
                }

                bool zeroSums = true;
                bool skipRow = false;

                int count = fieldsForDisint.GetLength(0);
                for (int i = 0; i < count; i++)
                {
                    if (!CheckPercentByTerrType(j, terrType))
                    {
                        skipRow = true;
                        break;
                    }

                    if (row == null)
                    {
                        row = dsDisintegratedData.Tables[0].NewRow();
                        CopyRowToRow(sourceRow, row);
                    }

                    double d = Convert.ToDouble(sourceRow[fieldsForDisint[i]]);

                    switch (j)
                    {
                        case 1:
                            // уфк 14 - НЕ считаем у типа терр - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["FED_PERCENT"]) / 100;
                            break;

                        case 2:
                            if (isUFK14Pump)
                                if (this.Region == RegionName.MoskvaObl)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["CONS_PERCENT"]) / 100;
                            break;

                        case 3:
                            // уфк 14 - НЕ считаем у типа терр - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["SUBJ_PERCENT"]) / 100;
                            break;

                        case 4:
                            // уфк 14 - НЕ считаем у типа терр - 3, 7
                            if (isUFK14Pump)
                                if ((terrType == 3) || (terrType == 7) || (this.Region == RegionName.MoskvaObl))
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["CONSMR_PERCENT"]) / 100;
                            break;

                        case 5:
                            // уфк 14 - НЕ считаем у типа терр - 3, 7
                            if (isUFK14Pump)
                                if ((terrType == 3) || (terrType == 7))
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["MR_PERCENT"]) / 100;
                            break;

                        case 6:
                            // уфк 14 - НЕ считаем у типа терр - 3, 7, 9
                            if (isUFK14Pump)
                                if ((terrType == 3) || (terrType == 7) || (terrType == 9))
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["STAD_PERCENT"]) / 100;
                            break;

                        case 7:
                            d *= Convert.ToDouble(disintRow["OUTOFBUDGETFOND_PERCENT"]) / 100;
                            break;

                        case 12:
                            // уфк 14 - НЕ считаем только у типа терр - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["SMOLENSKACCOUNT_PERCENT"]) / 100;
                            break;

                        case 13:
                            // уфк 14 - НЕ считаем только у типа терр - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["TUMENACCOUNT_PERCENT"]) / 100;
                            break;

                        case 14:
                            if (isUFK14Pump)
                                 if (this.Region == RegionName.MoskvaObl)
                                 {
                                     d = 0;
                                     break;
                                 }
                            d *= Convert.ToDouble(disintRow["CONSMO_PERCENT"]) / 100;
                            break;

                        case 15: 
                            // уфк 14 - считаем только у типа терр - 7
                            if (isUFK14Pump)
                            {
                                if (terrType != 7)
                                    d = 0;
                                else
                                    d *= Convert.ToDouble(disintRow["GO_PERCENT"]) / 100;
                            }
                            else
                            {
                                // для типа территорий ГП, СП, Районный центр -  сумма по этому уровню бюджета отсутствует
                                if ((terrType == 5) || (terrType == 6) || (terrType == 10))
                                    d = 0;
                                else
                                    d *= Convert.ToDouble(disintRow["GO_PERCENT"]) / 100;
                            }
                            break;
                    }

                    if (d != 0)
                        zeroSums = false;

                    row[fieldsForDisint[i]] = d;
                }

                if (!zeroSums && !skipRow)
                {
                    row["SOURCEKEY"] = sourceRow["ID"];
                    row[refBudgetLevelFieldName] = j;

                    // временное решение - пока расщепление не перевели на новый алгоритм
                    if (this.PumpProgramID == PumpProgramID.FNS28nDataPump)
                        row["RefNormDeduct"] = 6;

                    dsDisintegratedData.Tables[0].Rows.Add(row);

                    // Если накопилось много расщепленных записей, то сбрасываем их в базу
                    if (dsDisintegratedData.Tables[0].Rows.Count >= constMaxQueryRecordsForDisint)
                    {
                        UpdateOkatoData();
                        UpdateDataSet(daDisintegratedData, dsDisintegratedData, fctDisintegratedData);
                        ClearDataSet(daDisintegratedData, ref dsDisintegratedData);
                    }
                }

                row = null;
            }

            this.DB.ExecQuery(
                string.Format("update {0} set {1} = 1 where ID = ?", fctSourceTable.FullDBName, disintFlagFieldName),
                QueryResultTypes.NonQuery,
                this.DB.CreateParameter("ID", sourceRow["ID"], DbType.Int64));
        }

        /// </summary>
        /// Запрос порции данных из базы для расщепления
        /// </summary>
        /// <param name="constr">Ограничение запроса</param>
        /// <param name="refKDFieldName">Имя поля - ссылки на КД</param>
        /// <param name="refOkatoFieldName">Имя поля - ссылки на OKATO</param>
        private void QuerySourceData(string constr, string refKDFieldName, string refOkatoFieldName)
        {
            this.SetProgress(-1, -1, "Запрос данных для расщепления...", string.Empty, true);
            WriteToTrace("Запрос данных для расщепления...", TraceMessageKind.Information);

            // Инициализация адаптеров
            // Адаптер для просмотра данных для расщепления (с подтягиванием КД)
            string str = string.Format(
                "select {0}.*, k.CODESTR KDCODE " +
                "from {0} left join {1} k on (k.id = {0}.{2})",
                fctSourceTable.FullDBName, clsKD.FullDBName, refKDFieldName);

            if (constr != string.Empty)
            {
                str += string.Format(" where {0}", constr);
            }
            str += string.Format(
                " order by {0}.{1} asc, {0}.sourceid asc", fctSourceTable.FullDBName, refKDFieldName);

            InitLocalDataAdapter(this.DB, ref daSourceData, str);
            ClearDataSet(ref dsSourceData);
            daSourceData.Fill(dsSourceData);

            WriteToTrace("Запрос данных для расщепления окончен.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Функция расщепления данных
        /// </summary>
        /// <param name="fieldsForDisint">Массив полей с данными для расщепления</param>
        private void DisintegrateData(string[] fieldsForDisint)
        {
            // Счетчик записей
            int recCount = 0;
            DataRow disintRowEx = null;

            disintCount = 0;
            totalRecsForSourceID = 0;

            // Запрашиваем данные ОКАТО по текущему источнику и заполняем кэш ОКАТО
            PrepareOkatoForSumDisint(clsOKATO);
            // добавляем записи окато в районы.служебный для закачки, и берем из служебного класс-ра тип территории для окато
            AddOkatoToRegionsForPump();

            string constr = string.Format("{0}.SOURCEID = {1} and {0}.{2} = 0",
                fctSourceTable.FullDBName, this.SourceID, disintFlagFieldName);
            if (disintDateConstraint != string.Empty)
                constr += string.Format(" and {0}", disintDateConstraint);

            // Узнаем, есть ли данные для расщепления и сколько их
            // Всего записей
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            if (totalRecs == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeCriticalError, "Нет данных для расщепления.");
                return;
            }

            WriteToTrace(string.Format("Записей для расщепления: {0}.", totalRecs), TraceMessageKind.Information);

            // Узнаем значение первого ИД
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(ID) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            // Верхнее ограничение ИД для выборки
            int lastID = firstID + constMaxQueryRecordsForDisint - 1;

            // Таблица расщепленных записей
            InitFactDataSet(ref daDisintegratedData, ref dsDisintegratedData, fctDisintegratedData);

            do
            {
                // Ограничение запроса для выборки порции данных
                string restrictID = string.Format(
                    "{0}.ID >= {1} and {0}.ID <= {2} and {3}", fctSourceTable.FullDBName, firstID, lastID, constr);
                firstID = lastID + 1;
                lastID += constMaxQueryRecordsForDisint;

                QuerySourceData(restrictID, refKDFieldName, refOkatoFieldName);

                if (dsSourceData.Tables[0].Rows.Count == 0)
                    continue;

                // Расщепляем полученные данные
                for (int i = 0; i < dsSourceData.Tables[0].Rows.Count; i++)
                {
                    recCount++;

                    DataRow sourceRow = dsSourceData.Tables[0].Rows[i];

                    this.SetProgress(totalRecs, recCount, 
                        string.Format("Обработка данных (ID источника {0})...", this.SourceID),
                        string.Format("Запись {0} из {1}", recCount, totalRecs));

                    // По записи 28н берем ОКАТО района, КД, год и месяц. Формируем дату - первое число этого года 
                    // и этого месяца (для поиска уточнений)
                    currentOkatoID = Convert.ToInt32(sourceRow[refOkatoFieldName]);
                    currentOkatoCode = FindCachedRow(okatoCodesCache, currentOkatoID, string.Empty);
                    if (currentOkatoCode == string.Empty)
                    {
                        throw new Exception(string.Format("Не найдена строка ОКАТО по ссылке {0}", currentOkatoID));
                    }
                    
                    string kd = Convert.ToString(sourceRow["KDCODE"]);
                    string date = Convert.ToString(sourceRow[refDateFieldName]);
                    int year = Convert.ToInt32(date.Substring(0, 4));

                    totalRecsForSourceID++;

                    // Ищем правила расщепления, удовлетворяющие условиям: КД, OКАТО района такое как у нашего района, Год,
                    // Если запись уточнения, то берем только те, где поле "дата, с которого действует уточнение" 
                    // меньше или равна нашей дате.
                    DataRow disintRow = FindDisintRule(disintRulesCache, year, kd);

                    if (disintRow == null)
                    {
                        // Не найдено не одного расщепления - пишем в протокол некритическую ошибку
                        WriteRecInMessagesDS(date, year, this.SourceID, kd);

                        GetOkatoRow(currentOkatoID);

                        continue;
                    }
                    else
                    {
                        // Ищем уточнение
                        disintRowEx = GetDisintExRow(disintRow, Convert.ToInt32(date), currentOkatoCode);
                        // Если нашли уточнение, то его и используем
                        if (disintRowEx != null)
                            disintRow = disintRowEx;
                    }

                    // Расщепляем строку
                    DisintegrateRow(sourceRow, disintRow, fieldsForDisint);
                    disintCount++;
                }
            }
            while (recCount < totalRecs);

            UpdateOkatoData();
            UpdateDataSet(daDisintegratedData, dsDisintegratedData, fctDisintegratedData);
        }

        /// <summary>
        /// Возвращает номер режима расщепления: 0 - только нерасщепленные, 1 - все
        /// </summary>
        private int GetDisintMode()
        {
            string str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "rbtnDisintegratedOnly", "false");
            if (Convert.ToBoolean(str))
            {
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// Возвращает параметры этапа расщепления данных
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="disintAll">Расщеплять все или нет</param>
        protected void GetDisintParams(ref int year, ref int month, ref bool disintAll)
        {
            if (GetDisintMode() == 0)
            {
                disintAll = false;
            }
            else
            {
                disintAll = true;
            }

            string str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "umeYears", string.Empty);
            if (str == string.Empty)
            {
                year = -1;
            }
            else
            {
                year = Convert.ToInt32(str);
            }

            str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucMonths", string.Empty);
            if (str == string.Empty)
            {
                month = -1;
            }
            else
            {
                month = Convert.ToInt32(str);
            }
        }

        /// <summary>
        /// Удалить расщепленные данные
        /// </summary>
        /// <returns>Ошибка</returns>
        protected void PrepareDisintData(bool disintAll)
        {
            string query = string.Format("update {0} set {1} = 0 where SOURCEID = {2}",
                fctSourceTable.FullDBName, disintFlagFieldName, this.SourceID);
            if (disintDateConstraint != string.Empty)
                query += " and " + disintDateConstraint;
            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted || disintAll)
            {
                DeleteTableData(fctDisintegratedData, -1, this.SourceID, disintDateConstraint);
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
            }
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted && disintAll)
            {

                this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
            }
        }

        /// <summary>
        /// Расщепляет данные по нормативам отчислений доходов
        /// </summary>
        /// <param name="dpm">Модуль закачки, вызывающий данный метод</param>
        /// <param name="fctSourceTable">Таблица исходных данных</param>
        /// <param name="fctDisintegratedData">Таблица для расщепленных данных</param>
        /// <param name="clsKD">Объект классификатора КД</param>
        /// <param name="clsOKATO">Объект классификатора ОКАТО</param>
        /// <param name="fieldsForDisint">Массив полей с данными для расщепления</param>
        /// <param name="refDateFieldName">Имя поля - ссылки на дату</param>
        /// <param name="refKDFieldName">Имя поля - ссылки на КД</param>
        /// <param name="refOkatoFieldName">Имя поля - ссылки на OKATO</param>
        protected void DisintegrateData(IFactTable fctSourceTable, IFactTable fctDisintegratedData, IClassifier clsKD,
            IClassifier clsOKATO, string[] fieldsForDisint, string refDateFieldName, string refKDFieldName,
            string refOkatoFieldName, bool disintAll)
        {
            this.fctSourceTable = fctSourceTable;
            this.fctDisintegratedData = fctDisintegratedData;
            this.clsKD = clsKD;
            this.clsOKATO = clsOKATO;
            this.refDateFieldName = refDateFieldName;
            this.refKDFieldName = refKDFieldName;
            this.refOkatoFieldName = refOkatoFieldName;

            PrepareDisintData(disintAll);

            DisintegrateData(fieldsForDisint);
        }

        /// <summary>
        /// Проверяет наличие правил расщепления для текущего года
        /// </summary>
        protected void CheckDisintRulesCache()
        {
            // Проверяем наличие правил расщепления для текущего года
            if (!disintRulesCache.ContainsKey(this.DataSource.Year))
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                    string.Format("Отсутствуют правила расщепления для {0} года - данные не будут расщеплены.", this.DataSource.Year));
                disintRulesIsEmpty = true;
            }
            else
            {
                disintRulesIsEmpty = false;
            }
        }

        #endregion Функции расщепления данных
    }
}