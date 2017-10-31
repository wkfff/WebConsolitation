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
    // Класс для установки дат по классификатору «Соответствие операционных дней»

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region Поля

        // Кжш соответствия операционных дней ФО по ФК
        private Dictionary<int, int> corrFK2FO = null;
        // Кжш соответствия операционных дней ФК по ФО
        private Dictionary<int, int> corrFO2FK = null;
        private List<string> nullAccountsCache = new List<string>(100);
        private string ghostField;
        private string fkDateField;
        private string foDateField;

        #endregion Поля


        #region Константы

        /// <summary>
        /// Количество записей, которое будем запрашивать при обработке
        /// </summary>
        private const int constMaxQueryRecordsForProcess = 50000;

        #endregion Константы


        /// <summary>
        /// Добавляет запись в кэш незаполненных р/счетов
        /// </summary>
        /// <param name="code">Код ОКАТО</param>
        private void WriteToNullAccountsCache(string code)
        {
            if (!nullAccountsCache.Contains(code))
            {
                nullAccountsCache.Add(code);
            }
        }

        /// <summary>
        /// Записывает в лог данные о не найденных р/счетах
        /// </summary>
        protected void WriteNullAccountsCacheToBD()
        {
            if (nullAccountsCache.Count > 0)
            {
                string msg = string.Format(
                    "Для некоторых кодов ОКАТО не найден расчетный счет в классификаторе Районы.Служебный. " +
                    "Заполните в этом классификаторе поле \"Расчетный счет\" и запустите этап обработки еще раз. " +
                    "Всего кодов: {0}. Список кодов ОКАТО: \n{1}.",
                    nullAccountsCache.Count, string.Join(", ", nullAccountsCache.ToArray()));

                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, msg);
            }
        }

        /// <summary>
        /// Заполняет кэши соответствия операционных дней
        /// </summary>
        protected void FillOperationDaysCorrCache()
        {
            if (corrFK2FO != null)
                corrFK2FO.Clear();
            if (corrFO2FK != null)
                corrFO2FK.Clear();

            IDbDataAdapter da = null;
            DataSet ds = null;

            InitDataSet(this.DB, ref da, ref ds, "select REFFKDATE, REFFODATE from D_DATE_CONVERSIONFK");
            DataTable dt = ds.Tables[0];

            corrFK2FO = new Dictionary<int, int>(dt.Rows.Count);
            corrFO2FK = new Dictionary<int, int>(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int fkDate = Convert.ToInt32(dt.Rows[i]["REFFKDATE"]);
                int foDate = Convert.ToInt32(dt.Rows[i]["REFFODATE"]);

                if (!corrFK2FO.ContainsKey(fkDate))
                    corrFK2FO.Add(fkDate, foDate);
                if (!corrFO2FK.ContainsKey(foDate))
                    corrFO2FK.Add(foDate, fkDate);
            }

            if (corrFK2FO.Count == 0 && corrFO2FK.Count == 0)
            {
                throw new Exception("Классификатор соответствия операционных дней пуст");
            }
        }

        /// <summary>
        /// Функция обработки полученных данных
        /// </summary>
        /// <param name="dt">Таблица с данными для обработки</param>
        /// <param name="totalRecs">Общее количество записей</param>
        /// <param name="processedRecCount">Общее количество обработанных записей</param>
        private void SetOperationDays(DataRow row)
        {
            string ghost = Convert.ToString(row[ghostField]);

            // Если стоит признак для закачки 1 (закачка СводаФУ), то в соответствии с классификатором 
            // «Соответствие операционных дней» заполняется поле «Дата ФО» по существующей «Дата ФК» 
            // или это закачка УФК_0009_Результаты регулировки налогов между уровнями бюджетов
            if (ghost == "1" || this.PumpProgramID == PumpProgramID.TaxesRegulationDataPump)
            {
                int fkDate = Convert.ToInt32(row[fkDateField]);
                if (corrFK2FO.ContainsKey(fkDate))
                {
                    row[foDateField] = corrFK2FO[fkDate];
                }
            }
            // Если стоит признак для закачки 2, 3 или 4 (закачка 1н), то в соответствии с классификатором 
            // «Соответствие операционных дней» заполняется поле «Дата ФК» по существующей «Дата ФО».
            else if (ghost == "2" || ghost == "3" || ghost == "4")
            {
                int foDate = Convert.ToInt32(row[foDateField]);
                if (corrFO2FK.ContainsKey(foDate))
                {
                    row[fkDateField] = corrFO2FK[foDate];
                }
            }

            if (this.PumpProgramID == PumpProgramID.TaxesRegulationDataPump)
            {
                currentOkatoID = Convert.ToInt32(row["REFOKATO"]);
                currentOkatoCode = FindCachedRow(okatoCodesCache, currentOkatoID, string.Empty);

                // Заполняется поле «Тип территории» классификатора ОКАТО.УФК в соответствии с 
                // классификатором «Районы.Служебный для закачки»
                DataRow okatoRow = GetOkatoRow(currentOkatoID);
                if (okatoRow == null)
                    return;
                int terrType = GetIntCellValue(okatoRow, "REFTERRTYPE", -1);
                string account = GetStringCellValue(okatoRow, "ACCOUNT", 0);
                string dutyAccount = GetStringCellValue(okatoRow, "DUTYACCOUNT", -1);

                // В соответствии со значениями в поле «Тип территории» классификатора «ОКАТО.УФК» 
                // корректируется поле «Уровни бюджета». Корректировка осуществляется только для строк, 
                // у которых призрак для закачки имеет значение DMB. При заполнении значение из поля 
                // Уровни бюджетов перезаписывается.
                if (string.Compare(ghost, "DMB", true) == 0)
                {
                    switch (terrType)
                    {
                        case 0:
                            row[refBudgetLevelFieldName] = 14;
                            break;

                        case 4:
                            row[refBudgetLevelFieldName] = 5;
                            break;

                        case 5:
                        case 6:
                            if (this.DataSource.Year < 2006 || account == dutyAccount)
                            {
                                row[refBudgetLevelFieldName] = 6;
                            }
                            else
                            {
                                row[refBudgetLevelFieldName] = 5;
                            }
                            break;

                        case 7:
                            row[refBudgetLevelFieldName] = 15;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Функция установки соответствия операционных дней для таблицы фактов
        /// </summary>
        /// <param name="fct">Таблица фактов</param>
        /// <param name="ghostField">Поле признака для закачки</param>
        /// <param name="fkDateField">Поле даты ФК</param>
        /// <param name="foDateField">Поле даты ФО</param>
        /// <param name="month">Месяц из параметров закачки</param>
        protected void SetOperationDaysForFact(IFactTable fct, string ghostField, string fkDateField,
            string foDateField, string selectFactDataByMonthConstraint)
        {
            this.ghostField = ghostField;
            this.fkDateField = fkDateField;
            this.foDateField = foDateField;

            string semantic = fct.FullCaption;
            string constr = string.Empty;

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                constr = string.Format("PUMPID = {0}", this.PumpID);
            }

            // Если указано ограничение по месяцу, то учитываем его
            if (selectFactDataByMonthConstraint != string.Empty)
            {
                if (!string.IsNullOrEmpty(constr))
                    constr += " and ";
                constr = string.Format("{0} {1}", constr, selectFactDataByMonthConstraint);
            }

            PartialDataProcessingTemplate(fct, constr, constMaxQueryRecordsForProcess,
                new DataPartRowProcessing(SetOperationDays), "Установка соответствия операционных дней");
        }

        /// <summary>
        /// Возвращает параметры Год и Месяц
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        protected void GetPumpParams(ref int year, ref int month)
        {
            year = -1;
            month = -1;

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                return;

            // Получаем значение параметра "Год"
            string str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "umeYears", string.Empty);
            if (str != string.Empty)
            {
                year = Convert.ToInt32(str);
            }

            // Получаем значение параметра "Месяц"
            str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucMonths", string.Empty);
            if (str != string.Empty)
            {
                month = Convert.ToInt32(str);
            }
        }
    }
}