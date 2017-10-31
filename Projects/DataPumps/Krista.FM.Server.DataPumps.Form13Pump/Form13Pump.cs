using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.Form13Pump
{
    /// <summary>
    /// УФК_0006_Форма 13
    /// Сводная ведомость поступлений, подлежащих перечислению в бюджеты (.txt)
    /// Приложение № 13 к Порядку учета Федеральным казначейством поступлений в бюджетную систему РФ и их распределения между бюджетами 
    /// бюджетной системы РФ, утвержденному приказом Минфина РФ от 16.12.2004 г. № 116н
    /// </summary>
    public class Form13PumpModule : TextRepPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> kdList = null;
        // Документы.Ведомость Операция (d_Doc_List)
        private IDbDataAdapter daDocs;
        private DataSet dsDocs;
        private IClassifier clsDocs;
        private Dictionary<string, int> docsList = null;
        // Районы.УФК (d_Regions_UFK)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionsList = null;
        private int nullRegions = -1;
        // Местные бюджеты.УФК (d_LocBdgt_UFK)
        private IDbDataAdapter daLocBdgt;
        private DataSet dsLocBdgt;
        private IClassifier clsLocBdgt;
        private Dictionary<string, int> locBdgtList = null;
        // Период.Соответствие операционных дней (d_Date_ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<int, int> cachePeriod = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Свод поступлений подлежащих перечислению (f_D_UFK13)
        private IDbDataAdapter daUFK13;
        private DataSet dsUFK13;
        private IFactTable fctUFK13;
        // Доходы.УФК_Справочно (f_D_Information)
        private IDbDataAdapter daInformation;
        private DataSet dsInformation;
        private IFactTable fctInformation;

        #endregion Факты

        string processedFiles = "<Нет данных>";
        private List<int> deletedDate = new List<int>();
        private decimal[, ] totalSum = new decimal[3, 8];

        private bool isVologda2011 = false;

        #endregion Поля

        #region Константы

        // Количество записей для занесения в базу
        private const int constMaxQueryRecords = 10000;

        #endregion Константы
        
        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public Form13PumpModule()
            : base()
        {

        }

        #endregion Инициализация

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref kdList, dsKd.Tables[0], "CODESTR");
            FillRowsCache(ref docsList, dsDocs.Tables[0], "NAME");
            FillRowsCache(ref locBdgtList, dsLocBdgt.Tables[0], "OKATO");
            FillRowsCache(ref regionsList, dsRegions.Tables[0], "NAME");
            FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFKDate", "RefFODate");
        }

        /// <summary>
        /// Запрос данных из базы
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daKd, ref dsKd, clsKd, false, string.Empty);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            InitClsDataSet(ref daLocBdgt, ref dsLocBdgt, clsLocBdgt, false, string.Empty);
            InitDataSet(ref daDocs, ref dsDocs, clsDocs, false, string.Empty, string.Empty);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            InitFactDataSet(ref daUFK13, ref dsUFK13, fctUFK13);
            InitFactDataSet(ref daInformation, ref dsInformation, fctInformation);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            FillCaches();
        }

        /// <summary>
        /// Внести изменения в базу
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daDocs, dsDocs, clsDocs);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daLocBdgt, dsLocBdgt, clsLocBdgt);
            UpdateDataSet(daUFK13, dsUFK13, fctUFK13);
            UpdateDataSet(daInformation, dsInformation, fctInformation);
        }

        #region GUIDs

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_REGIONS_UFK_GUID = "90375d17-5145-43b9-81f1-2145aba86b7c";
        private const string D_DOC_LIST_GUID = "f31e70b3-3ce2-49bc-809c-96002469a216";
        private const string D_LOC_BDGT_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string D_DATE_CONVERSION_FK_GUID = "414c27e7-393c-4516-8b47-cf6df384569d";
        private const string F_D_UFK13_GUID = "2ebefa61-d979-4e57-a234-a433992eac03";
        private const string F_D_INFORMATION_GUID = "c69847ad-0ac3-4fe8-ba46-ff9b4c43ff36";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsPeriod = this.Scheme.Classifiers[D_DATE_CONVERSION_FK_GUID];
            clsDocs = this.Scheme.Classifiers[D_DOC_LIST_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsKd = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_UFK_GUID],
                clsLocBdgt = this.Scheme.Classifiers[D_LOC_BDGT_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctUFK13 = this.Scheme.FactTables[F_D_UFK13_GUID],
                fctInformation = this.Scheme.FactTables[F_D_INFORMATION_GUID] };
        }

        /// <summary>
        /// Функция выполнения завершающих действий этап
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsDocs);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsLocBdgt);
            ClearDataSet(ref dsPeriod);
            ClearDataSet(ref dsUFK13);
            ClearDataSet(ref dsInformation);
            deletedDate.Clear();
        }

        #endregion Работа с базой и кэшами

        #region Общие функции

        private void SetNullTotalSum()
        {
            for (int i = 0; i < totalSum.GetLength(0); i++)
                for (int j = 0; j < totalSum.GetLength(1); j++)
                    totalSum[i, j] = 0.0M;
        }

        private decimal CleanFactValue(string factValue)
        {
            return Convert.ToDecimal(factValue.Trim().PadLeft(1, '0'));
        }

        private void SetFlags(int refDate)
        {
            isVologda2011 = (this.Region == RegionName.Vologda) && (refDate >= 20110500);
        }

        private int[] COLUMNS_NUM = new int[] { 5, 6, 7 };
        private int[] COLUMNS_NUM_2008 = new int[] { 4, 5, 6 };
        private int[] COLUMNS_NUM_KRASNODAR = new int[] { 5, 6, 7, 8, 9, 10, 11, 12 };
        private int[] COLUMNS_NUM_KRASNODAR_2008 = new int[] { 4, 5, 6, 7, 8, 9, 10, 11 };
        private int[] COLUMNS_NUM_TULA = new int[] { 17, 20, 23 };
        private int[] COLUMNS_NUM_VOLOGDA_2011 = new int[] { 17, 20, 23 };
        private int[] GetColumnsNum()
        {
            if (this.Region == RegionName.Krasnodar)
            {
                if (this.DataSource.Year == 2008)
                    return COLUMNS_NUM_KRASNODAR_2008;
                return COLUMNS_NUM_KRASNODAR;
            }
            if (this.Region == RegionName.Tula)
            {
                return COLUMNS_NUM_TULA;
            }
            if (isVologda2011)
            {
                return COLUMNS_NUM_VOLOGDA_2011;
            }
            if (this.DataSource.Year == 2008)
                return COLUMNS_NUM_2008;
            return COLUMNS_NUM;
        }

        private int[] BUDGET_LEVELS = new int[] { 3, 1, 14, 9, 8, 11, 10, 12 };
        private int[] BUDGET_LEVELS_TULA = new int[] { 1, 3, 14 };
        private int[] BUDGET_LEVELS_VOLOGDA = new int[] { 1, 3, 14, 8, 9, 10, 11, 12 };
        private int[] BUDGET_LEVELS_VOLOGDA_2011 = new int[] { 1, 3, 14 };
        private int[] GetBudgetLevels()
        {
            if (this.Region == RegionName.Tula)
                return BUDGET_LEVELS_TULA;
            if (isVologda2011)
                return BUDGET_LEVELS_VOLOGDA_2011;
            if (this.Region == RegionName.Vologda)
                return BUDGET_LEVELS_VOLOGDA;
            return BUDGET_LEVELS;
        }

        private int GetRefFODate(int refFKDate)
        {
            if ((this.Region != RegionName.Vologda) &&
                (this.Region != RegionName.Krasnodar) &&
                (this.Region != RegionName.Tula))
                return -1;
            if (cachePeriod.ContainsKey(refFKDate))
                return cachePeriod[refFKDate];
            return -1;
        }

        #endregion Общие функции

        #region Работа с Txt

        /// <summary>
        /// Формирует и закачивает строку фактов
        /// </summary>
        /// <param name="row">Строка отчета</param>
        /// <param name="regionID"></param>
        /// <param name="date"></param>
        private void PumpFactRow(DataRow row, int regionID, int date)
        {
            if (Convert.ToDouble(row["OTHERRECIPIENTSSUM"]) != 0)
            {
                throw new Exception(string.Format(
                    "Значение в поле \"Иным получателям\" файла {0} отлично от нуля", processedFiles));
            }

            string kd = Convert.ToString(row["KD"]);
            if (String.Compare(kd, "ВСЕГО", true) == 0) return;

            // Сумма бюджетов всех уровней
            AddFactRow(row["TOTAL"], date, kd, regionID, 0);

            // Федеральный бюджет
            AddFactRow(row["FEDBUDGETSUM"], date, kd, regionID, 1);

            // Консолидированный бюджет субъекта РФ/города федерального значения
            AddFactRow(row["SUBJBUDGETSUM"], date, kd, regionID, 3);

            // Консолидированный бюджет муниципального района/городского округа
            AddFactRow(row["REGBUDGETSUM"], date, kd, regionID, 4);

            // Пенсионный фонд РФ
            AddFactRow(row["PENSFONDSUM"], date, kd, regionID, 8);

            // Фонд социального страхования РФ
            AddFactRow(row["SOCFONDSUM"], date, kd, regionID, 9);

            // Федеральный фонд ОМС
            AddFactRow(row["FEDMEDFONDSUM"], date, kd, regionID, 10);

            // Территориальные фонды ОМС
            AddFactRow(row["TERRMEDFONDSUM"], date, kd, regionID, 11);
        }

        /// <summary>
        /// Закачивает строку из датасета текстовых отчетов
        /// </summary>
        /// <param name="sum">Сумма</param>
        /// <param name="date">Дата справки</param>
        /// <param name="kd">Код КД</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="budgetLevelID">ИД уровня бюджета</param>
        private void AddFactRow(object sum, int date, string kd, int regionID, int budgetLevelID)
        {
            if (sum == null || Convert.ToDouble(sum) == 0) return;

            int kdID = PumpCachedRow(kdList, dsKd.Tables[0], clsKd, kd, new object[] { "CODESTR", kd });

            DataRow row = dsUFK13.Tables[0].NewRow();
            //row["ID"] = fctUFK13.GetGeneratorNextValue;
            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["SUMMA"] = sum;
            row["REFKD"] = kdID;
            row["REFYEARDAYUNV"] = date;
            row["REFBUDGETLEVELS"] = budgetLevelID;
            row["REFREGIONS"] = regionID;
            dsUFK13.Tables[0].Rows.Add(row);

            if (dsUFK13.Tables[0].Rows.Count >= constMaxQueryRecords)
            {
                UpdateData();
                ClearDataSet(daUFK13, ref dsUFK13);
            }
        }

        /// <summary>
        /// Формирует и закачивает строку классификатора Районы
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="fixedParams">Фиксированные параметры файла</param>
        /// <returns>ИД строки классификатора</returns>
        private int PumpRegionRow(string fileName, Dictionary<string, FixedParameter> fixedParams)
        {
            // Для файлов вида F13ХХМДД.txt:
            // Поле «Код» формируется из имени файла. Символы «__» в коде района заменять на «0».
            // Поле «Наименование» формируется из левого верхнего угла файла. Это поле может быть 
            // не заполнено (если данные предоставлены по республиканскому бюджету).
            if (fileName.ToUpper().StartsWith("F13"))
            {
                string str = fileName.Replace('_', '0');
                if (fixedParams["TaxOrgan"].Value == string.Empty)
                {
                    return PumpOriginalRow(dsRegions, clsRegions,
                        new object[] { "CODE", str.Substring(3, 2), "NAME", "Бюджет субъекта" });
                }
                else
                {
                    return PumpOriginalRow(dsRegions, clsRegions,
                        new object[] { "CODE", str.Substring(3, 2), "NAME", fixedParams["TaxOrgan"].Value });
                }
            }
            // Для файлов вида Н___ХХ поле «Код» не формируется. Поле «Наименование» формируется 
            // из строки заголовка «Наименование бюджета»
            else
            {
                return PumpOriginalRow(dsRegions, clsRegions,
                    new object[] { "CODE", 0, "NAME", fixedParams["Budget"].Value });
            }
        }

        private void PumpTxtFile(FileInfo file)
        {
            int totalRecs;
            int rowsCount = 0;
            int skippedReports = 0;
            int skippedRows = 0;
            int processedReports = 0;
            int date = 0;
            string err = string.Empty;


            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных текстовых отчетов.");

            try
            {
                CommonRoutines.ExtractArchiveFile(file.FullName, file.Directory.FullName, ArchivatorName.Zip,
                    FilesExtractingOption.SeparateSubDirs);

                try
                {
                    this.CallTXTSorcerer(xmlSettingsForm13, file.Directory.FullName);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Закачка данных текстовых отчетов закончена: " + ex.Message);
                    return;
                }

                totalRecs = GetTotalRecs();

                // Закачиваем полученные данные
                // Первая таблица датасета - служебная, ее не берем
                for (int i = 1; i < this.ResultDataSet.Tables.Count; i++)
                {
                    DataTable dt = this.ResultDataSet.Tables[i];
                    if (dt.Rows.Count == 0) continue;

                    int fileIndex = Convert.ToInt32(dt.Rows[0][this.FileIndexFieldName]);
                    processedFiles = GetStringCellValue(this.ResultDataSet.Tables[0].Rows[fileIndex], "FILES", "<Нет данных>");

                    try
                    {
                        // Дата справки
                        string str = this.FixedParameters[fileIndex]["ReportDate"].Value;

                        if (str != string.Empty)
                        {
                            date = Convert.ToInt32(str);
                            if (date > 0 && date / 10000 != this.DataSource.Year)
                            {
                                skippedReports++;
                                skippedRows += this.ResultDataSet.Tables[i].Rows.Count;

                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                                    "Дата файла {0} не соответствует параметрам источника.", processedFiles));

                                continue;
                            }
                        }

                        DeleteData(string.Format("RefYearDayUNV = {0}", date), string.Format("Дата отчета: {0}.", date));

                        // Формируем классификатор Районы
                        int regionID = PumpRegionRow(processedFiles, this.FixedParameters[fileIndex]);

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            PumpFactRow(dt.Rows[j], regionID, date);

                            rowsCount++;
                            SetProgress(totalRecs, rowsCount,
                                string.Format("Источник {0}. Обработка данных...", file.Directory.FullName),
                                string.Format("Строка {0} из {1}", rowsCount, totalRecs));
                        }
                    }
                    catch (Exception ex)
                    {
                        skippedReports++;
                        skippedRows += this.ResultDataSet.Tables[i].Rows.Count;

                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                                "Закачка данных {0} закончена с ошибками.", processedFiles), ex);
                    }

                    processedReports++;
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                        "Закачка данных текстовых отчетов закончена. Обработано отчетов: {0} ({1} строк), " +
                        "из них пропущено: {2} отчетов ({3} строк).",
                        processedReports, rowsCount, skippedReports, skippedRows));
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                        "Закачка данных текстовых отчетов закончена с ошибками. \n" +
                        "На момент возникновения ошибки достигнуты следующие результаты. " +
                        "Обработано отчетов: {0} ({1} строк), из них пропущено: {2} отчетов ({3} строк). " +
                        "Ошибка возникла при обрабатке файлов {4}.",
                        processedReports, rowsCount, skippedReports, skippedRows, processedFiles), ex);
                throw;
            }
            finally
            {
                CommonRoutines.DeleteExtractedDirectories(file.Directory);
            }
        }

        #endregion Работа с Txt

        #region Работа с Excel

        private int GetXlsReportDate(ExcelHelper excelDoc)
        {
            string value;
            if (this.Region == RegionName.Tula)
            {
                value = excelDoc.GetValue(5, 37).Trim();
                // в некоторых отчётах вместе с датой указывается ещё и время,
                // его нужно убрать, чтобы далее корректно преобразовать дату
                if (value.Length > 10)
                    value = value.Substring(0, 10);
            }
            else if (this.DataSource.Year == 2008)
            {
                value = excelDoc.GetValue(4, 11).Trim();
            }
            else
            {
                value = excelDoc.GetValue(4, 12).Trim();
            }
            if ((this.Region == RegionName.Vologda) && (this.DataSource.Year >= 2011) && (value == string.Empty))
            {
                value = excelDoc.GetValue(5, 37).Trim();
                // в некоторых отчётах вместе с датой указывается ещё и время,
                // его нужно убрать, чтобы далее корректно преобразовать дату
                if (value.Length > 10)
                    value = value.Substring(0, 10);
            }
            int refDate = CommonRoutines.ShortDateToNewDate(value);
            if (!deletedDate.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDate.Add(refDate);
            }
            return refDate;
        }

        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            if ((this.Region == RegionName.Tula)|| isVologda2011)
                return;

            int[] columnsNum = GetColumnsNum();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < columnsNum.GetLength(0); j++)
                {
                    decimal controlSum = Convert.ToDecimal(
                        excelDoc.GetValue(curRow + i, columnsNum[j]).Trim().PadLeft(1, '0'));
                    if (totalSum[i, j] != controlSum)
                    {
                        string str = CONST_SHEET_DISTRIB_INCOME;
                        if (i == 1)
                            str = CONST_SHEET_EXTRA_BANK_OPS;
                        if (i == 2)
                            str = CONST_SHEET_BACKSPACE;

                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                            string.Format("Контрольная сумма {0} по строке '{1}' в графе {2} не сходится с итоговой {3}.",
                            controlSum, str, columnsNum[j], totalSum[i, j]));
                    }
                }
        }

        private int PumpXlsKd(ExcelHelper excelDoc, int curRow)
        {
            string code;
            if ((this.Region == RegionName.Tula) || isVologda2011)
                code = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 6));
            else
                code = excelDoc.GetValue(curRow, 2);

            return PumpCachedRow(kdList, dsKd.Tables[0], clsKd, code,
                new object[] { "CODESTR", code, "NAME", "Неуказанное наименование" });
        }

        private int PumpXlsDocs(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 1);
            return PumpCachedRow(docsList, dsDocs.Tables[0], clsDocs, name, new object[] { "NAME", name });
        }

        private int PumpXlsLocBudget(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 1).Trim();
            string okato = string.Empty;
            if (isVologda2011)
                okato = excelDoc.GetValue(curRow, 26).Trim();
            else
                okato = excelDoc.GetValue(curRow, 5).Trim();

            return PumpCachedRow(locBdgtList, dsLocBdgt.Tables[0], clsLocBdgt, okato,
                 new object[] { "OKATO", okato, "NAME", name, "ACCOUNT", "Неуказанный счет" });
        }

        // Регулярное выражение для поиска названия региона (то, что в круглых скобках и без скобок)
        Regex regExRegionName = new Regex(@"(?<=\().*(?=\))", RegexOptions.IgnoreCase);
        private int PumpXlsRegions(ExcelHelper excelDoc)
        {
            if (this.Region == RegionName.Krasnodar)
            {
                string name = excelDoc.GetValue(5, 2).Trim();
                name = regExRegionName.Match(name).Value;
                return PumpCachedRow(regionsList, dsRegions.Tables[0], clsRegions, name, new object[] { "NAME", name });
            }
            return nullRegions;
        }

        private const string CONST_SHEET_DISTRIB_INCOME = "ВЕДОМОСТЬ РАСПРЕДЕЛЕНИЯ ПОСТУПЛЕНИЙ";
        private const string CONST_SHEET_EXTRA_BANK_OPS = "ВЕДОМОСТЬ УЧЕТА ВНЕБАНКОВСКИХ ОПЕРАЦИЙ";
        private const string CONST_SHEET_BACKSPACE = "ВЕДОМОСТЬ УЧЕТА ВОЗВРАТОВ (ВОЗМЕЩЕНИЙ)";
        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refRegions, int refDate)
        {
            int refKD = PumpXlsKd(excelDoc, curRow);
            int refDoc = PumpXlsDocs(excelDoc, curRow);
            int[] columnsNum = GetColumnsNum();
            int[] budgetLevels = GetBudgetLevels();
            for (int curCol = 0; curCol < columnsNum.GetLength(0); curCol++)
            {
                string value = excelDoc.GetValue(curRow, columnsNum[curCol]);
                if (value.Trim() == string.Empty)
                    continue;

                decimal summa = CleanFactValue(value);
                if (summa == 0)
                    continue;
                int refBudgetLevels = budgetLevels[curCol];

                value = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
                if (value == CONST_SHEET_DISTRIB_INCOME)
                    totalSum[0, curCol] += summa;
                if (value == CONST_SHEET_EXTRA_BANK_OPS)
                    totalSum[1, curCol] += summa;
                if (value == CONST_SHEET_BACKSPACE)
                {
                    totalSum[2, curCol] += summa;
                    // суммы для строк «Ведомость учета возвратов (возмещений)» закачиваются с противоположным знаком
                    if ((this.Region != RegionName.Tula) && !isVologda2011)
                        summa = -summa;
                }

                object[] mapping = new object[] {
                    "SUMMA", summa, "RefYearDayUNV", refDate, "REFREGIONS", refRegions,
                    "REFBUDGETLEVELS", refBudgetLevels, "REFKD", refKD, "REFDOC", refDoc
                };

                int refFODate = GetRefFODate(refDate);
                if (refFODate != -1)
                {
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefFODayUNV", refFODate });
                }

                PumpRow(dsUFK13.Tables[0], mapping);
                if (dsUFK13.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daUFK13, ref dsUFK13);
                }
            }
        }

        private void PumpXlsInfoRow(ExcelHelper excelDoc, int curRow, int refDate)
        {
            int refLocBudget = PumpXlsLocBudget(excelDoc, curRow);
            decimal factValue = 0;
            if (isVologda2011)
                factValue = CleanFactValue(excelDoc.GetValue(curRow, 31));
            else
                factValue = CleanFactValue(excelDoc.GetValue(curRow, 7));
            object[] mapping = new object[] { "SUMMA", factValue, "RefYearDayUNV", refDate, "REFLOCBDGT", refLocBudget };
            PumpRow(dsInformation.Tables[0], mapping);
            if (dsInformation.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daInformation, ref dsInformation);
            }
        }

        private bool IsStartReport(ExcelHelper excelDoc, int curRow)
        {
            string cellValue = excelDoc.GetValue(curRow, 1).Trim();
            if (cellValue != "1")
                return false;
            if ((this.Region == RegionName.Tula) || isVologda2011)
                cellValue = excelDoc.GetValue(curRow, 6).Trim();
            else
                cellValue = excelDoc.GetValue(curRow, 2).Trim();
            return ((cellValue != string.Empty) && (curRow > 1));
        }

        private const string MARK_INFO = "СПРАВОЧНО";
        private bool IsStartInfo(string cellValue)
        {
            if ((this.DataSource.Year >= 2009) && (this.Region == RegionName.Vologda))
                return (cellValue.ToUpper() == MARK_INFO);
            return false;
        }

        private const string MARK_TOTAL_ROW = "ВСЕГО";
        private const string MARK_VEDOMOST = "ВЕДОМОСТЬ";
        private bool IsEndReport(ExcelHelper excelDoc, int curRow)
        {
            string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            if ((this.Region == RegionName.Tula) || isVologda2011)
                return cellValue.StartsWith(MARK_TOTAL_ROW);
            if (cellValue.StartsWith(MARK_VEDOMOST))
                return (excelDoc.GetValue(curRow, 2).Trim() == string.Empty);
            return cellValue.StartsWith(MARK_TOTAL_ROW);
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate)
        {
            SetNullTotalSum();
            bool toPumpRow = false;
            bool toPumpInfoRow = false;
            int refRegions = PumpXlsRegions(excelDoc);
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if ((cellValue == string.Empty) || cellValue.ToUpper().StartsWith("ИТОГО"))
                        continue;

                    if (IsEndReport(excelDoc, curRow))
                    {
                        if (toPumpRow)
                            CheckXlsTotalSum(excelDoc, curRow);
                        toPumpRow = false;
                        toPumpInfoRow = false;
                        continue;
                    }

                    if (toPumpRow)
                        PumpXlsRow(excelDoc, curRow, refRegions, refDate);

                    if (toPumpInfoRow)
                        PumpXlsInfoRow(excelDoc, curRow, refDate);

                    if (IsStartInfo(cellValue))
                    {
                        curRow += 2;
                        toPumpInfoRow = true;
                        continue;
                    }

                    if (IsStartReport(excelDoc, curRow))
                        toPumpRow = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.OpenDocument(file.FullName);
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    int refDate = GetXlsReportDate(excelDoc);
                    SetFlags(refDate);
                    PumpXlsSheetData(file.Name, excelDoc, refDate);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        /// <summary>
        /// Закачивает текстовые файлы
        /// </summary>
        /// <param name="dir">Каталог с файлами для закачки</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.zip", new ProcessFileDelegate(PumpTxtFile), false);
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            UpdateData();
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            deletedDate.Clear();
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных
    }
}