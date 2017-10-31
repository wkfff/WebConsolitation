using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Модуль с функциями коррекции сумм

    /// <summary>
    /// Базовый класс для закачек, включающих индивидуальную установку иерархии классификаторов и 
    /// коррекцию сумм по иерархии классификаторов на этапе обработки данных.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {

        #region Структуры, перечисления

        /// <summary>
        /// Модификатор обработки блока. Нужен для выполнения индивидуальных операций для какого-либо из блоков.
        /// </summary>
        protected enum BlockProcessModifier
        {
            /// <summary>
            /// МесОтч. Стандартный блок (несправочный)
            /// </summary>
            MRStandard,

            /// <summary>
            /// МесОтч. Блок "ДефицитПрофицит"
            /// </summary>
            MRDefProf,

            /// <summary>
            /// МесОтч. Блок "Доходы"
            /// </summary>
            MRIncomes,

            /// <summary>
            /// МесОтч. Блок "Источники внешнего финансирования"
            /// </summary>
            MRSrcOutFin,

            /// <summary>
            /// МесОтч. Блок "Источники внутреннего финансирования"
            /// </summary>
            MRSrcInFin,

            /// <summary>
            /// МесОтч. Блок "Расходы"
            /// </summary>
            MROutcomes,

            /// <summary>
            /// МесОтч. Справочный блок (без конкретизации)
            /// </summary>
            MRCommonBooks,

            // справ остатки
            MRExcessBooks,

            /// <summary>
            /// МесОтч. Блок "СправДоходы"
            /// </summary>
            MRIncomesBooks,

            /// <summary>
            /// МесОтч. Блок "СправРасходы"
            /// </summary>
            MROutcomesBooks,

            /// <summary>
            /// МесОтч. Блок "СправРасходыДоп"
            /// </summary>
            MROutcomesBooksEx,

            /// <summary>
            /// МесОтч. Блок "КонсРасходы"
            /// </summary>
            MRAccount,

            /// <summary>
            /// МесОтч. Блок "Задолженность"
            /// </summary>
            MRArrears,

            /// <summary>
            /// ГодОтч. Стандартный блок (несправочный)
            /// </summary>
            YRStandard,

            /// <summary>
            /// ГодОтч. Блок "Недостачи Хищения"
            /// </summary>
            YREmbezzles,

            /// <summary>
            /// ГодОтч. Блок "Расходы"
            /// </summary>
            YROutcomes,

            /// <summary>
            /// ГодОтч. Блок "Сеть Штаты Контингент"
            /// </summary>
            YRNet,

            /// <summary>
            /// ГодОтч. Блок "ДефицитПрофицит"
            /// </summary>
            YRDefProf,

            /// <summary>
            /// ГодОтч. Блок "Доходы"
            /// </summary>
            YRIncomes,

            /// <summary>
            /// ГодОтч. Блок "Источники финансирования"
            /// </summary>
            YRSrcFin,

            /// <summary>
            /// ГодОтч. Блок "Баланс"
            /// </summary>
            YRBalanc
        }

        #endregion Структуры, перечисления

        
        #region Функции коррекции сумм по иерархии классификаторов

        /// <summary>
        /// Возвращает сумму значений поля в указанных строках или null, если все суммы == 0
        /// </summary>
        /// <param name="rows">Строки</param>
        /// <param name="fieldName">Название поля</param>
        /// <returns>Сумма</returns>
        private double[] GetRowsSum(DataRow[] rows, string[] fieldName)
        {
            if (rows.GetLength(0) == 0)
                return null;

            bool zeroSums = true;

            double[] result = new double[fieldName.GetLength(0)];
            int count = fieldName.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result[i] = 0;
                int rowsCount = rows.GetLength(0);
                for (int j = 0; j < rowsCount; j++)
                {
                    result[i] += Convert.ToDouble(rows[j][fieldName[i]]);
                    if (result[i] != 0)
                        zeroSums = false;
                }
            }
            if (zeroSums)
                return null;

            return result;
        }

        /// <summary>
        /// Возвращает сумму значений поля в указанных строках или null, если все суммы == 0.
        /// Суммируются только те записи фактов, у которых значение поля равно указанному
        /// </summary>
        /// <param name="factRows">Строки</param>
        /// <param name="clsIDs">Список ИД классификатора, по которым считать суммы</param>
        /// <param name="fields4CorrectedSums">Название полей сумм</param>
        /// <param name="fieldValuesMapping">Массив пар имя_поля-значение_поля для проверки строки</param>
        /// <returns>Сумма</returns>
        /// <param name="factRefToCls">Название ссылки на классификатор</param>
        private double[] GetRowsSum(Dictionary<int, List<DataRow>> cache, List<int> clsIDs,
            string[] fields4CorrectedSums, object[] fieldValuesMapping, string factRefToCls)
        {
            if (clsIDs.Count == 0)
                return null;

            bool zeroSums = true;
            bool skipRow = false;

            double[] result = new double[fields4CorrectedSums.GetLength(0)];

            for (int i = 0; i < clsIDs.Count; i++)
            {
                List<DataRow> factRows;
                if (cache.ContainsKey(clsIDs[i]))
                {
                    factRows = cache[clsIDs[i]];
                }
                else
                {
                    continue;
                }

                for (int j = 0; j < factRows.Count; j++)
                {
                    skipRow = false;

                    int count = fieldValuesMapping.GetLength(0);
                    for (int k = 0; k < count; k += 2)
                    {
                        if (factRows[j][Convert.ToString(fieldValuesMapping[k])].ToString() !=
                            fieldValuesMapping[k + 1].ToString())
                        {
                            skipRow = true;
                            break;
                        }
                    }

                    if (!skipRow)
                    {
                        count = fields4CorrectedSums.GetLength(0);
                        for (int k = 0; k < count; k++)
                        {
                            result[k] += GetDoubleCellValue(factRows[j], fields4CorrectedSums[k], 0);
                            if (result[k] != 0)
                                zeroSums = false;
                        }
                    }
                }
            }

            if (zeroSums)
                return null;

            return result;
        }

        /// <summary>
        /// Корректирует суммы таблицы фактов сограсно иерархии классификатора. Вспомогательная функция.
        /// </summary>
        /// <param name="parent2ChildCls">Кэш иерархии классификатора</param>
        /// <param name="sumFieldForCorrect">Массив полей с исходными суммами для коррекции в таблице фактов</param>
        /// <param name="fields4CorrectedSums">Массив полей для откорректированных сумм</param>
        /// <param name="cache">Структура данных фактов по району: 
        /// Ключ - ИД классификатора, Значение - список строк по этому классификатору.</param>
        /// <param name="bdgtLevelsRefFieldName">Название поля ссылки на классификатор уровней бюджета</param>
        /// <param name="multiClsCorrFields">Массив ссылок на классификаторы, которые будут учитываться при
        /// коррекции</param>
        /// <param name="factRefToCls">Название ссылки на классификатор</param>
        private void CorrectFactSums(SortedDictionary<int, List<int>> parent2ChildCls, string[] sumFieldForCorrect,
            string[] fields4CorrectedSums, Dictionary<int, List<DataRow>> cache, string bdgtLevelsRefFieldName,
            string[] multiClsCorrFields, string factRefToCls, DataTable factTable, bool transferSourceSums)
        {
            // Идем по записям таблицы классификатора и корректируем все суммы фактов в строках, ссылающихся
            // на текущую строку классификатора с учетом того, что у родительской и подчиненных записей 
            // должны совпадать ссылки из multiClsCorrFields
            foreach (KeyValuePair<int, List<int>> parentCls in parent2ChildCls)
            {
                int parentID = parentCls.Key;
                // Ищем строку с родительским классификатором
                List<DataRow> parentRows;
                if (cache.ContainsKey(parentID))
                {
                    parentRows = cache[parentID];
                }
                else
                {
                    continue;
                }
                List<string> budgetLevels = new List<string>(20);

                for (int j = 0; j < parentRows.Count; j++)
                {
                    int bl = 0;
                    if (bdgtLevelsRefFieldName != string.Empty)
                        bl = Convert.ToInt32(parentRows[j][bdgtLevelsRefFieldName]);
                    // Получаем ИД классификаторов, учитываемых при коррекции
                    object[] clsFieldsValues = (object[])CommonRoutines.ConcatArrays(
                        GetFieldValuesMappingFromRow(parentRows[j], multiClsCorrFields) );
                    if (bdgtLevelsRefFieldName != string.Empty)
                        clsFieldsValues = (object[])CommonRoutines.ConcatArrays(clsFieldsValues, new object[] { bdgtLevelsRefFieldName, bl });
                    if (bdgtLevelsRefFieldName != string.Empty)
                    {
                        string blKey = GetComplexCacheKey(GetFieldValuesFromValuesMapping(clsFieldsValues));
                        if (!budgetLevels.Contains(blKey))
                        {
                            budgetLevels.Add(blKey);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    // Получаем сумму всех подчиненных строк
                    double[] childSum;
                    // непонятное и кривое условие
                    // по моему вычитать нужно ВСЕГДА суммы ОТЧЕТА, а не уже скорректированные суммы при первой корректировке
                    // (по первому классификатору), в случае второго вызова корректировки над уже скорректированными данными
                    // уже оперируем откорректированными суммами
                    // в принципе можно ориентироваться на признак переноса исходных сумм (transferSourceSums) - feanor
                    if (transferSourceSums)
                        childSum = GetRowsSum(cache, parentCls.Value, sumFieldForCorrect, clsFieldsValues, factRefToCls);
                    else
                        childSum = GetRowsSum(cache, parentCls.Value, fields4CorrectedSums, clsFieldsValues, factRefToCls);
                   /* if (this.PumpProgramID == PumpProgramID.Form1NMPump)
                    {
                        childSum = GetRowsSum(cache, parentCls.Value, sumFieldForCorrect, clsFieldsValues,
                            factRefToCls);
                    }
                    else
                    {
                        childSum = GetRowsSum(cache, parentCls.Value, fields4CorrectedSums, clsFieldsValues, 
                            factRefToCls);
                    }*/

                    // Если все суммы подчиненных записей == 0, то нехрен и вычитать
                    if (childSum == null)
                        continue;

                    // Корректируем суммы
                    int count = fields4CorrectedSums.GetLength(0);
                    for (int k = 0; k < count; k++)
                        if (parentRows[j][fields4CorrectedSums[k]] != DBNull.Value)
                            parentRows[j][fields4CorrectedSums[k]] = 
                                GetDoubleCellValue(parentRows[j], fields4CorrectedSums[k], 0) - childSum[k];
                }
            }
        }

        /// <summary>
        /// Переносит суммы отчетов в поля для коррекции
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="sumCorrectionConfig">Структура с настройками для коррекции сумм</param>
        protected void TransferSourceSums(IFactTable fct, SumCorrectionConfig sumCorrectionConfig)
        {
            if (sumCorrectionConfig == null)
                return;

            string updateStr = string.Empty;

            string[] sumFieldForCorrect = sumCorrectionConfig.SumFieldForCorrect();
            string[] fields4CorrectedSums = sumCorrectionConfig.Fields4CorrectedSums();

            int count = sumFieldForCorrect.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                updateStr += string.Format("{0} = {1}, ", fields4CorrectedSums[i], sumFieldForCorrect[i]);
            }

            if (updateStr != string.Empty)
            {
                updateStr = updateStr.Remove(updateStr.Length - 2);
            }
            else
            {
                return;
            }
            string queryText = string.Format("update {0} set {1} where SOURCEID = {2}", fct.FullDBName, updateStr, this.SourceID);
            this.DB.ExecQuery(queryText,QueryResultTypes.NonQuery);
        }

        /// <summary>
        /// Заполнаяет поля отклонений сумм
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="mrSumCorrectionConfig">Структура с настройками для коррекции сумм (МесОтч)</param>
        protected void FillSpreadSums(IFactTable fct, MRSumCorrectionConfig mrSumCorrectionConfig)
        {
            if (mrSumCorrectionConfig == null || mrSumCorrectionConfig.MonthPlanField == string.Empty ||
                mrSumCorrectionConfig.YearPlanField == string.Empty)
                return;

            this.DB.ExecQuery(string.Format(
                "update {0} set {1} = {2} - {3}, {4} = {5} - {6} where SOURCEID = {7}",
                fct.FullDBName, mrSumCorrectionConfig.SpreadMonthPlanField, mrSumCorrectionConfig.MonthPlanField,
                mrSumCorrectionConfig.FactField, mrSumCorrectionConfig.SpreadYearPlanField,
                mrSumCorrectionConfig.YearPlanField, mrSumCorrectionConfig.FactField, this.SourceID),
                QueryResultTypes.NonQuery);
        }

        /// <summary>
        /// Заполняет кэш корректируемых данных
        /// </summary>
        /// <param name="dt">Таблица с данными для коррекции</param>
        /// <param name="regionRefFieldName">Имя поля ссылки на район</param>
        /// <param name="clsRefFieldName">Имя поля ссылки на классификатор</param>
        /// <param name="cache">Кэш</param>
        private void FillCorrectedSumsCache(DataTable dt, string regionRefFieldName, string clsRefFieldName,
            Dictionary<int, Dictionary<int, List<DataRow>>> cache)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int regionID = GetIntCellValue(dt.Rows[i], regionRefFieldName, 0);
                if (!cache.ContainsKey(regionID))
                {
                    cache.Add(regionID, new Dictionary<int, List<DataRow>>(1000));
                }
                Dictionary<int, List<DataRow>> regionData = cache[regionID];

                int clsID = Convert.ToInt32(dt.Rows[i][clsRefFieldName]);
                if (!regionData.ContainsKey(clsID))
                {
                    regionData.Add(clsID, new List<DataRow>(1000));
                }
                List<DataRow> rows = regionData[clsID];

                rows.Add(dt.Rows[i]);
            }
        }

        /// <summary>
        /// Заполянет кэш иерархии классификатора
        /// </summary>
        /// <param name="clsTable">Таблица классификатора</param>
        /// <param name="parent2ChildCls">Кэш иерархии</param>
        /// <param name="sumCorrectionConfig">Структура с настройками для коррекции сумм</param>
        private void FillParent2ChildClsCache(DataTable clsTable, ref SortedDictionary<int, List<int>> parent2ChildCls,
            IClassifier cls, SumCorrectionConfig sumCorrectionConfig)
        {
            F1NMSumCorrectionConfig f1nmSumCorrectionConfig = sumCorrectionConfig as F1NMSumCorrectionConfig;
            
            parent2ChildCls = new SortedDictionary<int, List<int>>();
            string codeField = GetClsCodeField(cls);
            if (codeField == string.Empty)
                codeField = "ID";
            DataRow[] rows = clsTable.Select(string.Empty, string.Format("{0} ASC", codeField));

            foreach (DataRow row in rows)
            {
                int id = Convert.ToInt32(row["ID"]);
                if (id < 0)
                    continue;

                int parentID = GetIntCellValue(row, "PARENTID", 0);

                if (parentID > 0)
                {
                    // В 1НМ для показателя "Начислено" при корректировке сумм не нужно учитывать раздел 3
                    if (f1nmSumCorrectionConfig != null)
                    {
                        if (this.PumpProgramID == PumpProgramID.Form1NMPump &&
                            !string.IsNullOrEmpty(f1nmSumCorrectionConfig.EarnedField))
                        {
                            int rowCode = GetIntCellValue(row, "ROWCODE", 0);
                            if (rowCode == 3300 || rowCode == 3400 || rowCode == 3500 || rowCode == 3600)
                                continue;
                        }
                    }

                    if (!parent2ChildCls.ContainsKey(parentID))
                    {
                        parent2ChildCls.Add(parentID, new List<int>(200));
                    }

                    List<int> childs = parent2ChildCls[parentID];
                    if (!childs.Contains(id))
                    {
                        childs.Add(id);
                    }
                }
            }
        }

        /// <summary>
        /// Заполняет кэш данных классификации и данных фактов по этой классификации
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="clsTable">Таблица классификатора</param>
        /// <param name="factRefToCls">Ссылка на классификатор</param>
        /// <param name="cls2FactCache">Кэш</param>
        private void FillCls2FactCache(DataTable factTable, string factRefToCls, 
            Dictionary<int, Dictionary<string, DataRow>> cls2FactCache)
        {
            foreach (DataRow row in factTable.Rows)
            {
                int clsID = Convert.ToInt32(row[factRefToCls]);
                if (!cls2FactCache.ContainsKey(clsID))
                    cls2FactCache.Add(clsID, new Dictionary<string, DataRow>(500));

                Dictionary<string, DataRow> factRow = cls2FactCache[clsID];
                string key = GetComplexCacheKey(row, 
                    new string[] { "REFFKR", "REFKCSR", "REFKVR", "REFREGIONS", "REFMEANSTYPE", "REFBDGTLEVELS" });
                if (!factRow.ContainsKey(key))
                    factRow.Add(key, row);
            }
        }

        /// <summary>
        /// Добавляет родительские записи ЭКР
        /// </summary>
        /// <param name="fct">Таблица фактов</param>
        /// <param name="clsTable">Классификатор</param>
        /// <param name="blockProcessModifier">Идентификатор блока</param>
        private void AddParentEKR(DataTable factTable, IFactTable fct, DataTable clsTable, IClassifier cls, 
            string factRefToCls, BlockProcessModifier blockProcessModifier, SumCorrectionConfig sumCorrectionConfig)
        {
            if (blockProcessModifier != BlockProcessModifier.YROutcomes ||
               (string.Compare(cls.FullName, "d.EKR.FOYR2004", true) != 0 &&
                string.Compare(cls.FullName, "d.EKR.FOYR2005", true) != 0))
                return;

            // Кэш данных классификации и данных фактов по этой классификации
            // Ключ - ИД классификатора, Значение - данные фактов.
            // Данные фактов: ключ - синтетическое значение из разреза классификации строки фактов,
            // значение - строка фактов.
            Dictionary<int, Dictionary<string, DataRow>> cls2FactCache = 
                new Dictionary<int, Dictionary<string, DataRow>>(500);

            try
            {
                FillCls2FactCache(factTable, factRefToCls, cls2FactCache);
                DataRow[] clsRows = clsTable.Select(string.Empty, "CODE DESC");

                for (int i = 0; i < clsRows.Length; i++)
                {
                    DataRow row = clsRows[i];

                    int id = Convert.ToInt32(row["ID"]);
                    int parentID = GetIntCellValue(row, "PARENTID", -1);

                    if (id < 0 || parentID < 0)
                        continue;

                    if (cls2FactCache.ContainsKey(id))
                    {
                        Dictionary<string, DataRow> childClsFactRows = cls2FactCache[id];

                        if (!cls2FactCache.ContainsKey(parentID))
                            cls2FactCache.Add(parentID, new Dictionary<string, DataRow>(20));
                        Dictionary<string, DataRow> parentClsFactRows = cls2FactCache[parentID];

                        foreach (KeyValuePair<string, DataRow> kvp in childClsFactRows)
                        {
                            if (!parentClsFactRows.ContainsKey(kvp.Key))
                            {
                                DataRow factRow = factTable.NewRow();
                                CopyRowToRow(kvp.Value, factRow);
                                factRow["ID"] = DBNull.Value;
                                factRow[factRefToCls] = parentID;

                                string[] fields = (string[])CommonRoutines.ConcatArrays(
                                    sumCorrectionConfig.Fields4CorrectedSums(),
                                    sumCorrectionConfig.SumFieldForCorrect());
                                for (int j = 0; j < fields.Length; j++)
                                {
                                    factRow[fields[j]] = 0;
                                }

                                factTable.Rows.Add(factRow);
                                parentClsFactRows.Add(kvp.Key, factRow);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (cls2FactCache != null)
                    cls2FactCache.Clear();
            }
        }

        /// <summary>
        /// Корректирует суммы в таблице фактов согласно иерархии классификатора
        /// </summary>
        /// <param name="fct">IFactTable</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="sumCorrectionConfig">Структура с настройками для коррекции сумм</param>
        /// <param name="hierarchyMapping">Коллекция соответствия ИД подчиненной и родительской записей 
        /// для каждого классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="multiClsCorrFields">Массив ссылок на классификаторы, которые будут учитываться при
        /// коррекции</param>
        /// <param name="regionRefFieldName">Название поля ссылки на классификатор районов.
        /// Пустая строка - не учитывать район</param>
        /// <param name="bdgtLevelsRefFieldName">Название поля ссылки на классификатор уровней бюджета</param>
        /// <param name="transferSourceSums">Переносить исходные суммы в поля для коррекции или нет</param>
        protected void CorrectFactTableSums(IFactTable fct, DataTable clsTable, IClassifier cls, string factRefToCls, 
            SumCorrectionConfig sumCorrectionConfig, BlockProcessModifier blockProcessModifier, 
            string[] multiClsCorrFields, string regionRefFieldName, string bdgtLevelsRefFieldName, 
            bool transferSourceSums, string factConstr)
        {
            string factSemantic = fct.FullCaption;
            string clsSemantic = cls.FullCaption;

            WriteToTrace(string.Format(
                "Коррекция сумм {0} по классификатору {1}...", factSemantic, clsSemantic), TraceMessageKind.Information);

            // Запрос данных фактов
            SetProgress(0, 0, string.Format("Запрос данных {0}...", factSemantic), string.Empty);

            if (transferSourceSums)
            {
                TransferSourceSums(fct, sumCorrectionConfig);
            }

            if (!clsTable.Columns.Contains("PARENTID") || clsTable.Rows.Count == 0)
            {
                WriteToTrace("Классификатор пуст или не имеет иерархии.", TraceMessageKind.Warning);
                return;
            }

            // Запрашиваем данные фактов
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, factConstr, string.Empty);
            DataTable factTable = ds.Tables[0];

            if (factTable.Rows.Count == 0)
            {
                WriteToTrace("Отсутствуют данные фактов.", TraceMessageKind.Warning);
                return;
            }

            // Добавляем родительские записи ЭКР
            AddParentEKR(ds.Tables[0], fct, clsTable, cls, factRefToCls, blockProcessModifier, sumCorrectionConfig);

            string[] sumFieldForCorrect = sumCorrectionConfig.SumFieldForCorrect();
            string[] fields4CorrectedSums = sumCorrectionConfig.Fields4CorrectedSums();

            // Заполняем кэш корректируемых данных
            // Структура кэша:
            // Ключ - ИД района, Значение - данные фактов по району.
            // Структура данных фактов по району:
            // Ключ - ИД классификатора, Значение - список строк по этому классификатору.
            Dictionary<int, Dictionary<int, List<DataRow>>> factCache = 
                new Dictionary<int, Dictionary<int, List<DataRow>>>(50);
            FillCorrectedSumsCache(ds.Tables[0], regionRefFieldName, factRefToCls, factCache);

            // Кэш иерархии классификатора
            // Ключ - ИД родительской записи, Значение - список ИД подчиненных записей
            SortedDictionary<int, List<int>> parent2ChildClsCache = null;
            FillParent2ChildClsCache(clsTable, ref parent2ChildClsCache, cls, sumCorrectionConfig);

            if (parent2ChildClsCache == null || parent2ChildClsCache.Count == 0)
            {
                WriteToTrace("Классификатор пуст или не имеет иерархии.", TraceMessageKind.Warning);
                return;
            }

            try
            {
                SetProgress(0, 0, string.Format(
                    "Коррекция сумм {0} по классификатору {1}...", factSemantic, clsSemantic), string.Empty);

                DataRow[] clsRows = null;
                // Для стандартного варианта берем только подчиненные записи
                clsRows = clsTable.Select("(PARENTID is not null) and (PARENTID > 0)", "PARENTID ASC");

                // Идем по датасету классификатора районов
                int i = 0;
                foreach (KeyValuePair<int, Dictionary<int, List<DataRow>>> kvp in factCache)
                {
                    i++;
                    SetProgress(factCache.Count, i,
                        string.Format("Коррекция сумм {0} по классификатору {1}...", factSemantic, clsSemantic),
                        string.Format("Район {0} из {1}", i, factCache.Count), true);

                    CorrectFactSums(parent2ChildClsCache, sumFieldForCorrect, fields4CorrectedSums, kvp.Value,
                        bdgtLevelsRefFieldName, multiClsCorrFields, factRefToCls, ds.Tables[0], transferSourceSums);
                }

                UpdateDataSet(da, ds, fct);
                //ClearDataSet(ref ds);

                // Заполянем поля отклонений сумм
                if (this.PumpProgramID == PumpProgramID.SKIFMonthRepPump)
                    FillSpreadSums(fct, sumCorrectionConfig as MRSumCorrectionConfig);

                SetProgress(-1, -1,
                    string.Format("Коррекция сумм {0} по классификатору {1} закончена.",
                    factSemantic, clsSemantic), string.Empty, true);
                WriteToTrace(string.Format("Коррекция сумм {0} по классификатору {1} закончена.",
                    factSemantic, clsSemantic), TraceMessageKind.Information);
            }
            finally
            {
                if (factCache != null)
                {
                    factCache.Clear();
                    factCache = null;
                }

                if (parent2ChildClsCache != null)
                {
                    parent2ChildClsCache.Clear();
                    parent2ChildClsCache = null;
                }
            }
        }

        protected void CorrectFactTableSums(IFactTable fct, DataTable clsTable, IClassifier cls, string factRefToCls,
            SumCorrectionConfig sumCorrectionConfig, BlockProcessModifier blockProcessModifier,
            string[] multiClsCorrFields, string regionRefFieldName, string bdgtLevelsRefFieldName,
            bool transferSourceSums)
        {
            string factConstr = string.Format("SOURCEID = {0}", this.SourceID);
            CorrectFactTableSums(fct, clsTable, cls, factRefToCls, sumCorrectionConfig, blockProcessModifier, 
                multiClsCorrFields, regionRefFieldName, bdgtLevelsRefFieldName, transferSourceSums, factConstr);
        }

        /// <summary>
        /// Корректирует суммы в таблице фактов согласно иерархии классификатора
        /// </summary>
        /// <param name="fct">IFactTable</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="sumCorrectionConfig">Структура с настройками для коррекции сумм</param>
        /// <param name="hierarchyMapping">Коллекция соответствия ИД подчиненной и родительской записей 
        /// для каждого классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        protected void CorrectFactTableSums(IFactTable fct,
            DataTable clsTable, IClassifier cls, string factRefToCls, SumCorrectionConfig sumCorrectionConfig,
            BlockProcessModifier blockProcessModifier)
        {
            CorrectFactTableSums(fct, clsTable, cls, factRefToCls, sumCorrectionConfig,
                blockProcessModifier, null, "REFREGIONS", "REFBDGTLEVELS", true);
        }

        /// <summary>
        /// Корректирует суммы в таблице фактов согласно иерархии классификатора
        /// </summary>
        /// <param name="fct">IFactTable</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="sumCorrectionConfig">Структура с настройками для коррекции сумм</param>
        /// <param name="hierarchyMapping">Коллекция соответствия ИД подчиненной и родительской записей 
        /// для каждого классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="transferSourceSums">Переносить исходные суммы в поля для коррекции или нет</param>
        protected void CorrectFactTableSums(IFactTable fct,
            DataTable clsTable, IClassifier cls, string factRefToCls, SumCorrectionConfig sumCorrectionConfig,
            BlockProcessModifier blockProcessModifier, bool transferSourceSums)
        {
            CorrectFactTableSums(fct, clsTable, cls, factRefToCls, sumCorrectionConfig,
                blockProcessModifier, null, "REFREGIONS", "REFBDGTLEVELS", transferSourceSums);
        }

        /// <summary>
        /// Корректирует суммы в таблице фактов согласно иерархии классификатора
        /// </summary>
        /// <param name="fct">IFactTable</param>
        /// <param name="clsTable">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="factRefsToCls">Ссылки из таблицы фактов на классификаторы clsTable</param>
        /// <param name="sumCorrectionConfig">Структура с настройками для коррекции сумм</param>
        /// <param name="hierarchyMapping">Коллекция соответствия ИД подчиненной и родительской записей 
        /// для каждого классификатора</param>
        /// <param name="blockProcessModifier">Модификатор обработки блока. Нужен для выполнения индивидуальных 
        /// операций для какого-либо из блоков.</param>
        /// <param name="multiClsCorrFields">Массив ссылок на классификаторы, которые будут учитываться при
        /// коррекции</param>
        /// <param name="regionRefFieldName">Название поля ссылки на классификатор районов</param>
        /// <param name="bdgtLevelsRefFieldName">Название поля ссылки на классификатор уровней бюджета</param>
        /// <param name="transferSourceSums">Переносить исходные суммы в поля для коррекции или нет</param>
        protected void CorrectFactTableSums(IFactTable fct,
            DataTable clsTable, IClassifier cls, string factRefToCls, SumCorrectionConfig sumCorrectionConfig,
            BlockProcessModifier blockProcessModifier, string[] multiClsCorrFields, string regionRefFieldName,
            string bdgtLevelsRefFieldName)
        {
            CorrectFactTableSums(fct, clsTable, cls, factRefToCls, sumCorrectionConfig,
                blockProcessModifier, multiClsCorrFields, regionRefFieldName, bdgtLevelsRefFieldName, true);
        }

        #endregion Функции коррекции сумм по иерархии классификаторов

        #region добавление родительских записей (фнс 23)

        private string GetCacheKey(string cls1ID, string cls2ID, string cls3ID)
        {
            return string.Format("{0}|{1}|{2}", cls1ID, cls2ID, cls3ID);
        }

        // вычисление номера раздела по коду классификатора
        private int GetClsSection(int code)
        {
            if ((code >= 100000000 && code < 105000000) || (code > 200000000) || (code >= 1000 && code < 2000))
                return 1;
            if (code >= 2000 && code < 3000)
                return 2;
            if ((code >= 105000000 && code < 106000000) || (code >= 4000 && code < 5000))
                return 3;
            if ((code >= 106000000 && code < 107000000) || (code >= 5000 && code < 6000))
                return 4;
            if ((code >= 107000000 && code < 108000000) || (code >= 6000 && code < 7000))
                return 5;
            // код = 0 у классификатора доходы.группы_фнс может относиться к нескольким разделам
            return 0;
        }

        /// <summary>
        /// добавление записей на родительском уровне (если нет)
        /// пока нужно только для блока фнс 23 - 4 ном 
        /// добавлять записи нужно в разрезности задолженностей фнс, потом доходы.группы фнс
        /// задача в квесте 7425
        /// </summary>
        /// <param name="fct"> факт </param>
        /// <param name="clsTable1"> первая ключевая таблица классификатора (суммы считаются в зависимости от его иерархии) </param>
        /// <param name="refsCls1"> ссылка на классификатор 1 </param>
        /// <param name="clsTable2"> вторая ключевая таблица классификатора (иерархия не учитывается)</param>
        /// <param name="refsCls2"> ссылка на классификатор 2 </param>
        /// <param name="clsTable3"> третья ключевая таблица классификатора (иерархия не учитывается)</param>
        /// <param name="refsCls3"> ссылка на классификатор 3 </param>
        /// <param name="sumFieldName"> имя поля с данными факта (сумма) </param>
        protected void AddParentRecords(IFactTable fct, DataTable clsTable1, string refCls1,
            DataTable clsTable2, string refCls2, DataTable clsTable3, string refCls3, string sumFieldName)
        {
            // получаем данные факта и заполняем кэш: составной ключ из двух ссылок на классификаторы - строка факта
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
            DataTable factTable = ds.Tables[0];
            Dictionary<string, DataRow> cacheFact = null;
            string[] allRefsCls = new string[] { refCls1, refCls2, refCls3 };
            FillRowsCache(ref cacheFact, factTable, allRefsCls, "|");
            DataRow[] clsRows1 = clsTable1.Select(string.Empty, "Code DESC");
            DataRow[] clsRows2 = clsTable2.Select(string.Empty, "Code DESC");
            DataRow[] clsRows3 = clsTable3.Select(string.Empty, "CodeStr DESC");
            // идем по всем записям третьего классификатора
            foreach (DataRow clsRow3 in clsRows3)
            {
                int cls3ID = Convert.ToInt32(clsRow3["ID"]);
                // идем по всем записям второго классификатора
                foreach (DataRow clsRow2 in clsRows2)
                {
                    int cls2ID = Convert.ToInt32(clsRow2["ID"]);

                    int sectionCls2 = GetClsSection(Convert.ToInt32(clsRow2["CODE"]));

                    foreach (DataRow clsRow1 in clsRows1)
                    {
                        int cls1ID = Convert.ToInt32(clsRow1["ID"]);

                        int sectionCls3 = GetClsSection(Convert.ToInt32(clsRow1["CODE"]));

                        if ((sectionCls2 == 0 || sectionCls3 == 0) ^ (sectionCls2 == sectionCls3))
                        {
                            string cls1Code = clsRow1["Code"].ToString();
                            // пропускаем нулевой код (фнс 23 - 4 ном)
                            if (Convert.ToInt32(cls1Code) == 0)
                                continue;
                            // если есть такая запись факта, пропускаем
                            string cacheKey = GetCacheKey(cls1ID.ToString(), cls2ID.ToString(), cls3ID.ToString());
                            if (cacheFact.ContainsKey(cacheKey))
                                continue;
                            // если нет дочерних записей в классификаторе, пропускаем
                            string clsConstraint = string.Format("ParentID = {0}", cls1ID);
                            DataRow[] clsChildRows = clsTable1.Select(clsConstraint);
                            if (clsChildRows.GetLength(0) == 0)
                                continue;
                            double childsSum = 0;
                            DataRow childFactRow = null;
                            bool toAddParentFactRow = false;
                            foreach (DataRow clsChildRow in clsChildRows)
                            {
                                string cls1ChildID = clsChildRow["ID"].ToString();
                                // если нет записей факта - с ссылкой на дочернюю запись классификатора, пропускаем
                                cacheKey = GetCacheKey(cls1ChildID, cls2ID.ToString(), cls3ID.ToString());
                                if (!cacheFact.ContainsKey(cacheKey))
                                    continue;
                                toAddParentFactRow = true;
                                childFactRow = cacheFact[cacheKey];
                                childsSum += Convert.ToDouble(childFactRow[sumFieldName]);
                            }
                            if (!toAddParentFactRow)
                                continue;
                            // добавляем запись факта - с суммой = сумма всех подчиненных по этому классификатору
                            DataRow parentFactRow = factTable.NewRow();
                            CopyRowToRow(childFactRow, parentFactRow);
                            parentFactRow["ID"] = DBNull.Value;
                            parentFactRow[refCls1] = cls1ID;
                            parentFactRow[sumFieldName] = childsSum;
                            factTable.Rows.Add(parentFactRow);
                        }
                    }
                }
            }
            UpdateDataSet(da, ds, fct);
        }

        #endregion добавление родительских записей (фнс 23)

        #region добавление родительских записей

        private void ClearFactRowSums(DataRow row, SumCorrectionConfig sumCorrectionConfig)
        {
            string[] fields = (string[])CommonRoutines.ConcatArrays(
                sumCorrectionConfig.Fields4CorrectedSums(), sumCorrectionConfig.SumFieldForCorrect());
            for (int i = 0; i < fields.Length; i++)
                row[fields[i]] = 0;
        }

        private void FillFactCache(DataTable factTable, string refCls, string[] refsCls,
            Dictionary<int, Dictionary<string, DataRow>> factCache)
        {
            foreach (DataRow row in factTable.Rows)
            {
                int clsID = Convert.ToInt32(row[refCls]);
                if (!factCache.ContainsKey(clsID))
                    factCache.Add(clsID, new Dictionary<string, DataRow>());
                Dictionary<string, DataRow> factRow = factCache[clsID];
                string key = GetGroupCacheKey(row, refsCls);
                if (!factRow.ContainsKey(key))
                    factRow.Add(key, row);
            }
        }

        // добавляем родительские (в контексте классификатора) записи факта, если отсутствуют - нужно для корректировки сумм
        protected void AddParentRecords(IFactTable fct, DataTable clsTable, string clsCodeField, string refCls, 
            string[] refsCls, SumCorrectionConfig sumCorrectionConfig)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
            DataTable factTable = ds.Tables[0];
            // Ключ - ИД классификатора, Значение - данные фактов. 
            // Данные фактов: ключ - синтетическое значение из разреза классификации строки фактов, значение - строка фактов.
            Dictionary<int, Dictionary<string, DataRow>> factCache = new Dictionary<int, Dictionary<string, DataRow>>();
            FillFactCache(factTable, refCls, refsCls, factCache);
            try
            {
                DataRow[] clsRows = clsTable.Select(string.Empty, string.Format("{0} DESC", clsCodeField));
                foreach (DataRow row in clsRows)
                {
                    int id = Convert.ToInt32(row["ID"]);
                    int parentID = GetIntCellValue(row, "PARENTID", -1);
                    if (id < 0 || parentID < 0)
                        continue;
                    if (!factCache.ContainsKey(id))
                        continue;
                    if (!factCache.ContainsKey(parentID))
                        factCache.Add(parentID, new Dictionary<string, DataRow>());
                    Dictionary<string, DataRow> parentClsFactRows = factCache[parentID];
                    Dictionary<string, DataRow> childClsFactRows = factCache[id];
                    foreach (KeyValuePair<string, DataRow> item in childClsFactRows)
                    {
                        if (parentClsFactRows.ContainsKey(item.Key))
                            continue;
                        DataRow factRow = factTable.NewRow();
                        CopyRowToRow(item.Value, factRow);
                        factRow["ID"] = DBNull.Value;
                        factRow[refCls] = parentID;
                        ClearFactRowSums(factRow, sumCorrectionConfig);
                        factTable.Rows.Add(factRow);
                        parentClsFactRows.Add(item.Key, factRow);
                    }
                }
                UpdateDataSet(da, ds, fct);
            }
            finally
            {
                if (factCache != null)
                    factCache.Clear();
            }
        }

        #endregion добавление родительских записей

        #region группировка записей по набору ключей - пока чиста суммирование 

        protected string GetGroupCacheKey(DataRow row, string[] refsCls)
        {
            string key = string.Empty;
            foreach (string clsRef in refsCls)
            {
                string refValue = row[clsRef].ToString();
                key += string.Format("{0}|", refValue);
            }
            key = key.Remove(key.Length - 1);
            return key;
        }

        // если есть записи ваще в одинаковой разрезности - циферки суммируем, оставляем только одну запись
        protected void GroupTable(IFactTable fct, string[] refsCls, SumCorrectionConfig sumCorrectionConfig)
        {
            string constr = string.Format("SOURCEID = {0}", this.SourceID);
            string[] sumFields = new string[] { };
            foreach (string sumField in sumCorrectionConfig.SumFieldForCorrect())
                sumFields = (string[])CommonRoutines.ConcatArrays(sumFields, new string[] { sumField});
            GroupTable(fct, refsCls, sumFields, constr);
        }

        // если есть записи ваще в одинаковой разрезности - циферки суммируем, оставляем только одну запись
        protected void GroupTable(IFactTable fct, string[] refsCls, string[] sumFields, string factConstr)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, factConstr, string.Empty);
            // Ключ - ИД классификатора, Значение - данные фактов. 
            // Данные фактов: ключ - синтетическое значение из разреза классификации строки фактов, значение - строка фактов.
            Dictionary<string, DataRow> factCache = new Dictionary<string, DataRow>();
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string key = GetGroupCacheKey(row, refsCls);
                    if (!factCache.ContainsKey(key))
                    {
                        factCache.Add(key, row);
                        continue;
                    }
                    DataRow cacheRow = factCache[key];
                    foreach (string sumField in sumFields)
                        if (row[sumField] != DBNull.Value)
                            cacheRow[sumField] = Convert.ToDecimal(cacheRow[sumField].ToString().PadLeft(1, '0')) +
                                                 Convert.ToDecimal(row[sumField]);
                    row.Delete();
                }
                UpdateDataSet(da, ds, fct);
            }
            finally
            {
                factCache.Clear();
            }
        }

        #endregion группировка записей по набору ключей - пока чиста суммирование

    }
}