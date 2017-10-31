using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS8Pump
{

    // ФНС - 0008 - Форма 5-ПМ
    public class FNS8PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 П (d.Marks.FNS5P)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        private int nullMarks;
        // Районы.ФНС (d.Regions.FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, DataRow> cacheRegions = null;
        private Dictionary<string, string> cacheRegionsNames = null;
        private Dictionary<string, DataRow> cacheRegionsFirstRow = null;
        private int nullRegions;
        // ЕдИзмер.ОКЕИ (d.Units.OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 П_Сводный (f.D.FNS5PTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 П_Районы (f.D.FNS5PRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        private int sectionIndex = -1;
        // параметры обработки
        private int year = -1;
        private int month = -1;
        // итоговая сумма
        private decimal totalSum = 0;
        private decimal totalTaxSum = 0;

        private bool noSvodReports = false;
        private bool isTyvaRegion2008 = false;

        #endregion Поля

        #region Структуры, перечисления

        // Тип отчета
        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

        #region Константы

        // наименования разделов для отчетов
        private string[] sectionNames = new string[] {
            "Раздел 1. Данные по организациям, не имеющим обособленных подразделений, и по организациям без входящих в них обособленных подразделений",
            "Раздел 2. Данные по обособленным подразделениям организаций",
            "Раздел 3. Данные по обособленным подразделениям, ликвидированным в течение текущего налогового периода",
            "Раздел 4. Данные по группе обособленных подразделений, находящихся на территории одного субъекта Российской Федерации" };

        private List<string> marksSectionNamesList = new List<string>(4);
        // список ай ди записей разделов в классификаторе Показатели.ФНС 5 П
        private List<int> marksSectionRecordsIDList = new List<int>(4);

        #endregion

        #region Закачка данных

        #region Работа с базой и кэшами

        // получить ай ди записей разделов классификатора показатели мн
        private void GetMarksParentId()
        {
            marksSectionRecordsIDList.Clear();
            marksSectionNamesList.Clear();
            marksSectionNamesList.AddRange(sectionNames);
            marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0 });
            // у записей разделов код равен нулю
            // получаем ай ди существующих записей разделов, если каких то разделов нет - закачиваем
            DataRow[] marksSectionRecords = dsMarks.Tables[0].Select("CODE = 0");
            foreach (DataRow marksSectionRecord in marksSectionRecords)
            {
                string sectionName = marksSectionRecord["NAME"].ToString().ToUpper();
                sectionIndex = GetSectionIndex(sectionName);
                if (sectionIndex == -1)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = Convert.ToInt32(marksSectionRecord["ID"]);
            }
            for (sectionIndex = 0; sectionIndex <= 3; sectionIndex++)
            {
                if (marksSectionRecordsIDList[sectionIndex] > 0)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = PumpRow(dsMarks.Tables[0], clsMarks,
                    new object[] { "CODE", 0, "NAME", marksSectionNamesList[sectionIndex] });
            }
            sectionIndex = -1;
        }

        // получить ай ди записей разделов классификатора задолженности
        // заполнить список последних кодов секций
        private void InitAuxStructures()
        {
            GetMarksParentId();
        }

        protected override void QueryData()
        {
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
            InitAuxStructures();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5P_GUID = "68871697-9b78-4ac5-a0e3-f7d7d3f4a472";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5P_TOTAL_GUID = "da24d67c-30c7-4336-8a41-8002e94f17d8";
        private const string F_D_FNS_5P_REGIONS_GUID = "0a829d58-0c8c-4691-bb48-5ee80bbcca73";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5P_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID]};
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5P_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5P_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsUnits);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        // обнуление итоговой суммы
        private void SetNullTotalSum()
        {
            totalSum = 0;
            totalTaxSum = 0;
        }

        // проверка контрольной суммы
        private void CheckTotalSum(decimal totalSum, decimal controlSum, string comment)
        {
            if (totalSum != controlSum)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F} {2}",
                    controlSum, totalSum, comment));
            }
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Trim().ToUpper().Trim('X').Trim('Х').PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        // получить номер раздела по значению ячейки (для всех, кроме тывы-2008)
        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            if (cellValue.Contains("РАЗДЕЛ 1"))
                return 0;
            else if (cellValue.Contains("РАЗДЕЛ 2"))
                return 1;
            else if (cellValue.Contains("РАЗДЕЛ 3"))
                return 2;
            else if (cellValue.Contains("РАЗДЕЛ 4"))
                return 3;
            return -1;
        }

        // установить номер раздела по наименованию листа книги (только для тывы-2008)
        private void SetSectionIndex(string worksheetName)
        {
            worksheetName = worksheetName.Trim().ToUpper();
            if (worksheetName == "ЛИСТ1")
                sectionIndex = 0;
            else if (worksheetName == "ЛИСТ2")
                sectionIndex = 1;
            else if (worksheetName == "ЛИСТ3")
                sectionIndex = 2;
            else if (worksheetName == "ЛИСТ4")
                sectionIndex = 3;
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        private void CheckDirectories(DirectoryInfo dir)
        {
            noSvodReports =
                ((this.Region == RegionName.Samara) && (this.DataSource.Year >= 2009)) ||
                ((this.Region == RegionName.Tyva) && (this.DataSource.Year <= 2008));
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                if (!noSvodReports)
                    throw new Exception(string.Format("Отсутствует каталог \"{0}\"", constSvodDirName));
            }
            if (str.GetLength(0) == 0)
                dir.CreateSubdirectory(constStrDirName);
            if (reg.GetLength(0) == 0)
                dir.CreateSubdirectory(constRegDirName);
            // Каталоги Строки и Районы для одного месяца не могут быть заполнены одновременно
            if ((str.GetLength(0) > 0 && str[0].GetFiles().GetLength(0) > 0) &&
                (reg.GetLength(0) > 0 && reg[0].GetFiles().GetLength(0) > 0))
                throw new Exception("Каталоги \"Строки\" и \"Районы\" для одного месяца не могут быть заполнены одновременно");
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 П' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isTyvaRegion2008 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year <= 2008);
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            SetFlags();
            CheckMarks();
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.rar", new ProcessFileDelegate(PumpRarFile), false);
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            // reportType = ReportType.Str;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            // ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        #endregion Общие функции закачки

        #region Работа с Excel

        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, 3));
            CheckTotalSum(totalSum, controlSum, string.Format("по столбцу {0}", 3));

            decimal controlTaxSum = CleanFactValue(excelDoc.GetValue(curRow, 4));
            CheckTotalSum(totalTaxSum, controlTaxSum, string.Format("по столбцу {0}", 4));
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            // если у регионов наименования одинаковые, а коды разные,
            // то к наименованию необходимо приписывать код в скобках
            if (!cacheRegionsNames.ContainsKey(regionCode))
            {
                // проверка: встречалось ли такое наименование, но с другим кодом
                if (cacheRegionsNames.ContainsValue(regionName))
                {
                    // если да, то необходимо изменить наименование у первой попавшейся записи с таким же наименованием
                    if (cacheRegionsFirstRow.ContainsKey(regionName))
                    {
                        DataRow firstRow = cacheRegionsFirstRow[regionName];
                        firstRow["Name"] = string.Format("{0} ({1})", firstRow["Name"], firstRow["Code"]);
                        cacheRegionsFirstRow.Remove(regionName);
                    }
                    regionName = string.Format("{0} ({1})", regionName, regionCode);
                }
                cacheRegionsNames.Add(regionCode, regionName);
            }
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            DataRow regionRow = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regionKey, mapping, false);
            // запоминаем регионы с уникальными наименованиями
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private void PumpFactRow(decimal factValue, decimal taxSum, int refDate, int refMarks, int refRegions)
        {
            totalSum += factValue;
            factValue *= 1000;

            totalTaxSum += taxSum;
            taxSum *= 1000;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] { "Value", factValue, "TaxUnderfundsSum", taxSum,
                    "RefYearDayUNV", refDate, "RefMarks", refMarks };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "Value", factValue, "TaxUnderfundsSum", taxSum,
                    "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow, string fileName)
        {
            string regionName;
            string regionCode;
            if (this.Region == RegionName.Orenburg && this.DataSource.Year == 2007)
            {
                regionName = excelDoc.GetValue(curRow + 3, 3).Trim();
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 1, 3).Trim());
            }
            else if (this.Region == RegionName.Altay)
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 1).Trim());
                regionName = excelDoc.GetValue(curRow - 2, 1).Trim();
            }
            else
            {
                string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                if (cellValue.Trim().ToUpper().StartsWith(MARK_TAX_DEP))
                {
                    regionCode = CommonRoutines.TrimLetters(cellValue);
                    if ((this.Region == RegionName.Samara) && (this.DataSource.Year >= 2009))
                        regionName = fileName.Substring(0, fileName.Length - 4);
                    else
                        regionName = regionCode.ToString();
                }
                else
                {
                    regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
                }
            }
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsRegionsTyva(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow < rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                if (cellValue == MARK_REGION)
                {
                    string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    string regionName = excelDoc.GetValue(curRow, 5).Trim();
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            int marksParentID = marksSectionRecordsIDList[sectionIndex];
            string code = excelDoc.GetValue(curRow, 2).Trim().PadLeft(1, '0');
            string name = excelDoc.GetValue(curRow, 1).Trim();

            object[] mapping = new object[] { "NAME", name, "CODE", code, "PARENTID", marksParentID };
            
            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code, "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);

            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
            decimal taxSum = CleanFactValue(excelDoc.GetValue(curRow, 4));
            if (factValue == 0 && taxSum == 0)
                return;

            PumpFactRow(factValue, taxSum, refDate, refMarks, refRegions);
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            if (reportType == ReportType.Region)
                return (cellValue.ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        private const string MARK_REGION = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private const string MARK_TAX_DEP = "НАЛОГОВЫЙ ОРГАН";
        private bool IsRegionRow(string cellValue)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion2008)
                return false;
            cellValue = cellValue.Trim().ToUpper();
            return (cellValue.StartsWith(MARK_TAX_DEP) || cellValue.StartsWith(MARK_REGION));
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            sectionIndex = -1;
            if (isTyvaRegion2008)
                SetSectionIndex(excelDoc.GetWorksheetName());
            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsRegionRow(cellValue))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow, fileName);
                        curRow += 2;
                        continue;
                    }

                    if (cellValue.ToUpper().StartsWith("РАЗДЕЛ") && !isTyvaRegion2008)
                    {
                        sectionIndex = GetSectionIndex(cellValue);
                        continue;
                    }

                    if (IsSectionEnd(cellValue))
                    {
                        CheckXlsTotalSum(excelDoc, curRow);
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        PumpXlsRow(excelDoc, curRow, refDate, refRegions);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        // в некоторых отчетах районов перед основными разделами добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1).Trim()))
                        {
                            if ((this.Region == RegionName.Orenburg) && (this.DataSource.Year == 2007))
                                refRegions = PumpXlsRegions(excelDoc, curRow, string.Empty);
                            continue;
                        }
                        SetNullTotalSum();
                        toPumpRow = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        private bool IsSkipWorksheet(string worksheetName)
        {
            if ((this.Region != RegionName.Omsk) || (reportType != ReportType.Svod))
                return false;
            return (!worksheetName.Trim().ToUpper().Contains("СВОД"));
        }

        private bool IsTitleSheet(string worksheetName)
        {
            worksheetName = worksheetName.ToUpper();
            return (isTyvaRegion2008 && worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ"));
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int refRegions = nullRegions;
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (IsSkipWorksheet(excelDoc.GetWorksheetName()))
                        continue;
                    else if (IsTitleSheet(excelDoc.GetWorksheetName()))
                        refRegions = PumpXlsRegionsTyva(excelDoc);
                    else
                        PumpXlsSheetData(file.Name, excelDoc, refDate, refRegions);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Работа с Rar

        private void PumpRarFile(FileInfo file)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
            try
            {
                ProcessAllFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        #endregion Работа с Rar

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            cacheRegionsNames = new Dictionary<string, string>();
            cacheRegionsFirstRow = new Dictionary<string, DataRow>();
            try
            {
                CheckDirectories(dir);
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
                cacheRegionsFirstRow.Clear();
                cacheRegionsNames.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private int GetRefUnits(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
                return FindCachedRow(cacheUnits, ROUBLE_UNIT_NAME, nullUnits);
            return -1;
        }

        protected void SetRefUnits()
        {
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                if (row["Code"].ToString() != "0")
                {
                    int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
                    row["RefUnits"] = refUnits;
                }
            }
        }

        protected override void ProcessDataSource()
        {
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Выполняется проставление ссылок с классификатора данных «Показатели.ФНС 5 П» на классификатор «ЕдИзмер.ОКЕИ».");
        }

        #endregion Обработка данных

    }
}