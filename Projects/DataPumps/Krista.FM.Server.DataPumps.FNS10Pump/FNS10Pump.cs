using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS10Pump
{
    // ФНС - 0010 - Форма 5-НДФЛ
    public class FNS10PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 НДФЛ (d_Marks_FNS5NDFL)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        private int nullMarks;
        // Районы.ФНС (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, DataRow> cacheRegions = null;
        private Dictionary<string, string> cacheRegionsNames = null;
        private Dictionary<string, DataRow> cacheRegionsFirstRow = null;
        private int nullRegions;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 НДФЛ_Сводный (f_D_FNS5NDFLTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 НДФЛ_Районы (f_D_FNS5NDFLRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        private int sectionIndex = -1;
        private int sectionsCount;
        private int formatYear;
        private bool noSvodReports = false;
        // параметры обработки
        private int year = -1;
        private int month = -1;
        private int marksParentCode = -1;
        private int marksParentID = -1;

        #endregion Поля

        #region Структуры, перечисления

        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

        #region Константы

        // начало текста ячейки с датой
        private const string DATE_CELL_TEXT = "по состоянию на";
        // наименования секций
        private string[] sectionNames2005 = new string[] {
            "Раздел I. Налоговая база, подлежащая налогообложению по ставке 13%, общая сумма исчисленного, удержанного и перечисленного налога",
            "Раздел II. Налоговая база, подлежащая налогообложению по ставке 30%, общая сумма исчисленного, удержанного и перечисленного налога",
            "Раздел III. Налоговая база, подлежащая налогообложению по ставке 9%, общая сумма исчисленного удержанного и перечисленного налога",
            "Раздел IV. Налоговая база, подлежащая налогообложению по ставке 35%, общая сумма исчисленного, удержанного и перечисленного налога",
            "Раздел V. Налоговая база, подлежащая налогообложению по налоговым ставкам, установленных в Соглашениях об избежании двойного налогообложения, общая сумма исчисленного, удержанного и перечисленного налога (5%, 10%, 15%)",
            "Раздел VI. Налоговые вычеты" };
        private string[] sectionNames2007 = new string[] {
            "Раздел I. Налоговая база, подлежащая налогообложению по ставке 13%, и сумма налога",
            "Раздел II. Налоговая база, подлежащая налогообложению по ставке 30%, и сумма налога",
            "Раздел III. Налоговая база, подлежащая налогообложению по ставке 9%, и сумма налога",
            "Раздел IV. Налоговая база, подлежащая налогообложению по ставке 35%, и сумма налога",
            "Раздел V. Налоговая база, подлежащая налогообложению по налоговым ставкам, установленным в Соглашениях об избежании двойного налогообложения, и сумма налога",
            "Раздел VI. Налоговые вычеты" };
        private string[] sectionNames2008 = new string[] {
            "Раздел I. Налоговая база, подлежащая налогообложению по ставке 13%, и сумма налога",
            "Раздел II. Налоговая база, подлежащая налогообложению по ставке 30%, и сумма налога",
            "Раздел III. Налоговая база, подлежащая налогообложению по ставке 9%, и сумма налога",
            "Раздел IV. Налоговая база, подлежащая налогообложению по ставке 35%, и сумма налога",
            "Раздел V. Налоговая база, подлежащая налогообложению по иным налоговым ставкам и сумма налога",
            "Раздел VI. Сведения о имущественных и стандартных налоговых вычетах",
            "Раздел VII. Сведения о налоговых вычетах, предоставленных по отдельным видам доходов" };
        private string[] sectionNames2009 = new string[] {
            "Раздел I. Налоговая база, подлежащая налогообложению по ставке 13%, и сумма налога",
            "Раздел II. Налоговая база, подлежащая налогообложению по ставке 30%, и сумма налога",
            "Раздел III. Налоговая база, подлежащая налогообложению по ставке 9%, и сумма налога",
            "Раздел IV. Налоговая база, подлежащая налогообложению по ставке 35%, и сумма налога",
            "Раздел V. Налоговая база, подлежащая налогообложению по ставке 15%, и сумма налога",
            "Раздел VI. Налоговая база, подлежащая налогообложению по иным налоговым ставкам и сумма налога",
            "Раздел VII. Сведения о имущественных и стандартных налоговых вычетах",
            "Раздел VIII. Сведения о налоговых вычетах, предоставленных по отдельным видам доходов" };
        private List<string> marksSectionNamesList = new List<string>(8);
        // список ай ди записей разделов в классификаторе Показатели.ФНС 5 НДФЛ
        private List<int> marksSectionRecordsIDList = new List<int>(8);

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        // получить ай ди записей разделов классификатора показатели ндфл
        private void GetMarksParentId()
        {
            if (formatYear >= 2009)
            {
                sectionsCount = 8;
                marksSectionNamesList.AddRange(sectionNames2009);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            }
            else if (formatYear >= 2008)
            {
                sectionsCount = 7;
                marksSectionNamesList.AddRange(sectionNames2008);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0 });
            }
            else if (formatYear >= 2007)
            {
                sectionsCount = 6;
                marksSectionNamesList.AddRange(sectionNames2007);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0 });
            }
            else
            {
                sectionsCount = 6;
                marksSectionNamesList.AddRange(sectionNames2005);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0 });
            }
            
            // у записей разделов код равен нулю
            // получаем ай ди существующих записей разделов, если каких-то разделов нет - закачиваем
            DataRow[] marksSectionRecords = dsMarks.Tables[0].Select("CODE = 0");
            foreach (DataRow marksSectionRecord in marksSectionRecords)
            {
                string sectionName = marksSectionRecord["NAME"].ToString().ToUpper();
                sectionIndex = GetSectionIndex(sectionName);
                if (sectionIndex == -1)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = Convert.ToInt32(marksSectionRecord["ID"]);
            }

            for (sectionIndex = 0; sectionIndex < sectionsCount; sectionIndex++)
            {
                if (marksSectionRecordsIDList[sectionIndex] > 0)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = PumpRow(dsMarks.Tables[0], clsMarks,
                    new object[] { "CODE", 0, "NAME", marksSectionNamesList[sectionIndex] });
            }
        }

        // получить ай ди записей разделов классификатора задолженности
        // заполнить список последних кодов секций
        private void InitAuxStructures()
        {
            GetMarksParentId();
        }

        protected override void QueryData()
        {
            SetFormatYear();
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
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5NDFL_GUID = "b1b4fb58-7b8c-44d5-8da5-92edb40185aa";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string F_D_FNS_5NDFL_TOTAL_GUID = "99f5b0bc-b4ec-4b76-a424-fb9509c94a6d";
        private const string F_D_FNS_5NDFL_REGIONS_GUID = "6405ebb0-4bb4-44ca-a300-34a5f620cc57";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5NDFL_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5NDFL_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5NDFL_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
            marksSectionRecordsIDList.Clear();
            marksSectionNamesList.Clear();
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        private int GetReportDate()
        {
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private DataTable GetFactTable()
        {
            if (reportType == ReportType.Svod)
                return dsIncomesTotal.Tables[0];
            return dsIncomesRegion.Tables[0];
        }

        private void SetFormatYear()
        {
            switch (this.Region)
            {
                case RegionName.Tula:
                case RegionName.Kostroma:
                case RegionName.Stavropol:
                case RegionName.Vologda:
                case RegionName.Samara:
                    if (this.DataSource.Year >= 2007 && this.DataSource.Year <= 2008)
                        formatYear = 2008;
                    else
                        formatYear = this.DataSource.Year;
                    break;
                default:
                    formatYear = this.DataSource.Year;
                    break;
            }
        }

        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            if (cellValue.Contains("РАЗДЕЛ III"))
                return 2;
            else if (cellValue.Contains("РАЗДЕЛ II"))
                return 1;
            else if (cellValue.Contains("РАЗДЕЛ IV"))
                return 3;
            else if (cellValue.Contains("РАЗДЕЛ I"))
                return 0;
            else if (cellValue.Contains("РАЗДЕЛ VIII"))
                return 7;
            else if (cellValue.Contains("РАЗДЕЛ VII"))
                return 6;
            else if (cellValue.Contains("РАЗДЕЛ VI"))
                return 5;
            else if (cellValue.Contains("РАЗДЕЛ V"))
                return 4;
            return -1;
        }

        private void CheckMarks()
        {
            if (!noSvodReports && (cacheMarks.Count == 0))
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 НДФЛ' - закачайте сводные отчеты");
        }

        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        private void CheckDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            noSvodReports =
                ((this.Region == RegionName.Tyva) && (this.DataSource.Year <= 2008)) ||
                (this.Region == RegionName.EAO) || (this.Region == RegionName.Omsk) ||
                (this.Region == RegionName.OmskMO) || (this.Region == RegionName.Orenburg);
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

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.rar", new ProcessFileDelegate(PumpRarFile), false);
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            CheckMarks();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            // reportType = ReportType.Str;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            // ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        #endregion Общие функции закачки

        #region Работа с Excel

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

        // регулярное выражение для выбора кода региона из строки вида "ОКАТОМ: 52223000000, Дата формирования: 03.07.2009"
        Regex regExRegionCode = new Regex(@"(?<=ОКАТОМ: )[0-9]*(?=\,)", RegexOptions.IgnoreCase);
        private string GetRegionCode(string cellValue)
        {
            return regExRegionCode.Match(cellValue).Value.Trim();
        }

        private int PumpXlsRegions(ExcelHelper excelDoc)
        {
            string regionName = excelDoc.GetValue(4, 1).Trim();
            string regionCode = GetRegionCode(excelDoc.GetValue(5, 1).Trim());
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            int code = -1;
            string name = excelDoc.GetValue(curRow, 2).Trim();
            string value = excelDoc.GetValue(curRow, 3).Trim();
            if (value != string.Empty)
            {
                marksParentCode = Convert.ToInt32(value);
                code = marksParentCode * 10000;
                if (sectionIndex >= sectionsCount)
                    return nullMarks;
                marksParentID = marksSectionRecordsIDList[sectionIndex];
            }
            else
            {
                code = marksParentCode * 10000 + Convert.ToInt32(name);
            }
            object[] mapping = new object[] { "NAME", name, "CODE", code, "PARENTID", marksParentID };
            int refMarks;
            if ((reportType == ReportType.Svod) || noSvodReports)
                refMarks = PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            else
                refMarks = FindCachedRow(cacheMarks, code.ToString(), nullMarks);
            if (value != string.Empty)
                marksParentID = refMarks;
            return refMarks;
        }

        private bool IsBigSection()
        {
            if (this.DataSource.Year >= 2009)
                return sectionIndex >= 6;
            return sectionIndex >= 5;
        }

        private const string AUX_ROW = "В ТОМ ЧИСЛЕ ПО КОДАМ ДОХОДА:";
        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if ((value.ToUpper() == AUX_ROW) || (value == "1"))
                return;
            value = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            if ((sectionIndex < 6) && (value == "1."))
                return;

            int refMarks = PumpXlsMarks(excelDoc, curRow);

            if (IsBigSection())
                value = excelDoc.GetValue(curRow, 5).Trim();
            else
                value = excelDoc.GetValue(curRow, 4).Trim();
            decimal factValue = Convert.ToDecimal(value.PadLeft(1, '0'));
            if (factValue == 0)
                return;

            object[] mapping = new object[] { "ValueReport", factValue, "Value", 0, "RefYearDayUNV", refDate, "RefMarks", refMarks };
            if (IsBigSection())
            {
                string taxPayers = excelDoc.GetValue(curRow, 4).Trim();
                if (taxPayers != string.Empty)
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "TaxpayersNumberReport", Convert.ToDecimal(taxPayers), "TaxpayersNumber", 0 });
            }
            if (reportType == ReportType.Region)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegions", refRegions });
            PumpRow(GetFactTable(), mapping);
            if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
            }
        }

        private bool IsSectionEndMark(string cellValue)
        {
            return (cellValue.Trim().ToUpper().StartsWith("*"));
        }

        private const string TABLE_TITLE_MARK = "№ П/П";
        private const string TABLE_TITLE_MARK_2008 = "НАИМЕНОВАНИЕ ПОКАЗАТЕЛЕЙ";
        private bool IsTableTitle(ExcelHelper excelDoc, int curRow)
        {
            if (formatYear < 2008)
                return (string.Compare(excelDoc.GetValue(curRow, 1).Trim().ToUpper(), TABLE_TITLE_MARK, true) == 0);
            return (string.Compare(excelDoc.GetValue(curRow, 2).Trim().ToUpper(), TABLE_TITLE_MARK_2008, true) == 0);
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate)
        {
            int refRegions = nullRegions;
            if (reportType == ReportType.Region)
                refRegions = PumpXlsRegions(excelDoc);
            sectionIndex = -1;
            bool toPumpRow = false;
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
                    if (cellValue == string.Empty)
                    {
                        cellValue = excelDoc.GetValue(curRow, 2).Trim();
                        if (cellValue == string.Empty)
                            continue;
                    }

                    if (cellValue.ToUpper().StartsWith("РАЗДЕЛ"))
                    {
                        sectionIndex = GetSectionIndex(cellValue);
                        toPumpRow = false;
                    }

                    if (IsSectionEndMark(cellValue))
                        toPumpRow = false;

                    if (toPumpRow)
                        PumpXlsRow(excelDoc, curRow, refDate, refRegions);

                    if (IsTableTitle(excelDoc, curRow))
                        toPumpRow = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private bool IsSkipFile(FileInfo file)
        {
            if (file.Name.Contains("OPR_"))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Отчет {0} закачан не будет.", file.FullName));
                return true;
            }
            return false;
        }

        private void PumpXlsFile(FileInfo file)
        {
            if (IsSkipFile(file))
                return;
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
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
                // иерархию устанавливаем вручную
                toSetHierarchy = false;
                SetClsHierarchy();
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

        private void SetClsHierarchy()
        {
            // иерархию устанавливаем вручную
            toSetHierarchy = false;
            string d_Marks_FNS10_HierarchyFileName = string.Empty;
            if (formatYear >= 2010)
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2010;
            else if (formatYear >= 2009)
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2009;
            else if (formatYear >= 2008)
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2008;
            else
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2006;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS10_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy()
        {
            F1NMSumCorrectionConfig f1nmSumCorrectionConfig = new F1NMSumCorrectionConfig();
            f1nmSumCorrectionConfig.EarnedField = "Value";
            f1nmSumCorrectionConfig.EarnedReportField = "ValueReport";
            f1nmSumCorrectionConfig.InpaymentsField = "TaxpayersNumber";
            f1nmSumCorrectionConfig.InpaymentsReportField = "TaxpayersNumberReport";
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByHierarchy();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Коррекции сумм фактов по данным источника");
        }

        #endregion Обработка данных

    }
}
