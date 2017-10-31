using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS7Pump
{

    // ФНС - 0007 - Форма 5-ТН
    public class FNS7PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 ТН (d.Marks.FNS5TN)
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
        private Dictionary<string, int> cacheRegionsByCode = null;
        private int nullRegions;
        // ЕдИзмер.ОКЕИ (d.Units.OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 ТН_Сводный (f.D.FNS5TNTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 ТН_Районы (f.D.FNS5TNRegions)
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
        private decimal[] totalSums;
        // коэффициент перевода тысяч в рубли
        private decimal sumMultiplier = 1;
        // номера столбцов, из которых не следует качать данные
        // (для файлов по строкам и файлов по районам для Тывы-2010)
        private List<int> excludedColumns = new List<int>();

        private int regionsRow;
        private bool noSvodReports = false;
        private bool isTyvaRegion2008 = false;
        private bool isTyvaRegion2010 = false;

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

        // код вид.лица "Всего"
        private const int TYPE_ALL_ID = 1;
        // код вид.лица "Юридическое лицо"
        private const int TYPE_URID_ID = 2;
        // код вид.лица "Физическое лицо"
        private const int TYPE_PHYS_ID = 3;

        // наименования секций для отчетов
        private string[] sectionNames = new string[]
        {
           "Раздел I. Отчет о структуре начислений по транспортному налогу по юридическим лицам",
           "Раздел II. Отчет о структуре начислений по транспортному налогу по физическим лицам",
        };

        private List<string> marksSectionNamesList = new List<string>(2);
        // список ай ди записей разделов в классификаторе Показатели.ФНС 5 ТН
        private List<int> marksSectionRecordsIDList = new List<int>(2);

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        // получить ай ди записей разделов классификатора показатели мн
        private void GetMarksParentId()
        {
            marksSectionNamesList.AddRange(sectionNames);
            marksSectionRecordsIDList.AddRange(new int[] { 0, 0 });
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
            for (sectionIndex = 0; sectionIndex <= 1; sectionIndex++)
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
            FillRowsCache(ref cacheRegionsByCode, dsRegions.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5TN_GUID = "ae8ca8de-34f0-479c-885e-533089b535f8";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5TN_TOTAL_GUID = "3077c6fe-535c-4898-98f3-e984ac1e24be";
        private const string F_D_FNS_5TN_REGIONS_GUID = "344c3962-9a80-46f4-bfef-6ab41d5c6d5d";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5TN_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID]};
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5TN_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5TN_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsUnits);
            marksSectionRecordsIDList.Clear();
            marksSectionNamesList.Clear();
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        // обнуление итоговой суммы
        private void SetNullTotalSum()
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                totalSums[i] = 0;
            }
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

        private void SetSumMultiplier(int marksCode)
        {
            if (this.DataSource.Year >= 2010)
            {
                if (((marksCode >= 1120) && (marksCode <= 1190)) || ((marksCode >= 2150) && (marksCode <= 2220)))
                    sumMultiplier = 1000;
                else
                    sumMultiplier = 1;
            }
            else if (this.DataSource.Year >= 2009)
            {
                if (((marksCode >= 1120) && (marksCode <= 1180)) || ((marksCode >= 2120) && (marksCode <= 2180)))
                    sumMultiplier = 1000;
                else
                    sumMultiplier = 1;
            }
            else
            {
                if (((marksCode >= 1120) && (marksCode <= 1150)) || ((marksCode >= 1200) && (marksCode <= 1220)) ||
                    ((marksCode >= 2120) && (marksCode <= 2150)) || ((marksCode >= 2200) && (marksCode <= 2220)))
                    sumMultiplier = 1000;
                else
                    sumMultiplier = 1;
            }
        }

        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            if (cellValue.Contains("РАЗДЕЛ II"))
                return 1;
            else if (cellValue.Contains("РАЗДЕЛ I"))
                return 0;
            return -1;
        }

        private int GetRefTypes()
        {
            if (sectionIndex == 1)
                return TYPE_PHYS_ID;
            return TYPE_URID_ID;
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return this.DataSource.Year * 10000 + 1;
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 ТН' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isTyvaRegion2008 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year <= 2008);
            isTyvaRegion2010 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year >= 2010);
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
            reportType = ReportType.Str;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        private void CheckDirectories(DirectoryInfo dir)
        {
            noSvodReports = (this.Region == RegionName.Tyva) && (this.DataSource.Year <= 2008);
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

        #endregion Общие функции закачки

        #region Работа с Excel

        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, i + 3));
                string comment = string.Format("по столбцу {0} в разделе {1}", i + 3, sectionIndex + 1);
                CheckTotalSum(totalSums[i], controlSum, comment);
            }
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            if ((this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010)))
            {
                if (cacheRegionsByCode.ContainsKey(regionCode))
                    return cacheRegionsByCode[regionCode];
            }

            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            regionKey = string.Format("{0}|{1} ({0})", regionCode, regionName);
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
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
            regionKey = string.Format("{0}|{1}", regionCode, regionName);
            DataRow regionRow = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regionKey, mapping, false);
            cacheRegionsByCode.Add(regionCode, Convert.ToInt32(regionRow["ID"]));
            // запоминаем регионы с уникальными наименованиями
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private void PumpFactRow(decimal factValue, int refDate, int refMarks, int refRegions, int refTypes, int sumIndex)
        {
            if (factValue == 0)
                return;

            totalSums[sumIndex] += factValue;
            factValue *= sumMultiplier;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] { "ValueReport", factValue, "RefTypes", refTypes,
                    "RefYearDayUNV", refDate, "RefMarks", refMarks };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "ValueReport", factValue, "RefTypes", refTypes,
                    "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            string regionCode = string.Empty;
            string regionName = string.Empty;
            if (this.Region == RegionName.Altay)
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 1).Trim());
                regionName = excelDoc.GetValue(curRow - 2, 1).Trim();
            }
            else
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
                regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
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
            int code = nullMarks;
            string name = excelDoc.GetValue(curRow, 1).Trim();
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if (value == string.Empty)
                return -1;
            code = Convert.ToInt32(value);
            SetSumMultiplier(code);
            int parentId = marksSectionRecordsIDList[sectionIndex];
            object[] mapping = new object[] { "NAME", name, "CODE", code, "PARENTID", parentId };
            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private int GetRefMarksByCode(int marksCode)
        {
            if (cacheMarks.ContainsKey(marksCode.ToString()))
                return cacheMarks[marksCode.ToString()];
            return nullMarks;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == -1)
                return;
            int refTypes = GetRefTypes();
            decimal valueReport = CleanFactValue(excelDoc.GetValue(curRow, 3));
            PumpFactRow(valueReport, refDate, refMarks, refRegions, refTypes, 0);
        }

        #region Отчеты в разрезе строк

        private int FindRegion(string regionCode, string regionName)
        {
            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            regionKey = string.Format("{0}|{1} ({0})", regionCode, regionName);;
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            return nullRegions;
        }

        private const string SUF = "[SUF]";
        private int PumpXlsRegionsStr(ExcelHelper excelDoc, int curRow)
        {
            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(curRow, curCol).Trim();
                if (cellValue.ToUpper() == SUF)
                {
                    return (curCol - 3);
                }
                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(curRow - 1, curCol).Trim();
                long code = 0;
                if (!Int64.TryParse(regionCode, out code))
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "В столбце {0} обнаружен некорректный код района '{1}'. Данные по нему будут пропущены.",
                        curCol, regionCode));
                    excludedColumns.Add(curCol);
                }
                else
                {
                    PumpRegion(regionCode, regionName);
                }
            }
        }

        private void PumpXlsRowStr(ExcelHelper excelDoc, int curRow, int refDate)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == -1)
                return;
            int refTypes = GetRefTypes();
            for (int curCol = 3; ; curCol++)
            {
                if (excludedColumns.Contains(curCol))
                    continue;
                string cellValue = excelDoc.GetValue(regionsRow, curCol).Trim();
                if (cellValue.ToUpper() == SUF)
                    return;

                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(regionsRow - 1, curCol).Trim();
                int refRegions = FindRegion(regionCode, regionName);

                decimal valueReport = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                PumpFactRow(valueReport, refDate, refMarks, refRegions, refTypes, curCol - 3);
            }
        }

        private void PumpXlsRowStrKalmykya(ExcelHelper excelDoc, int curRow, int refDate, int refMarks)
        {
            //в районы.фнс поле код целое. для карелии в отчете по строкам уникальность районам проверяем по коду. поэтому нужно убрать нули с лева
            string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 2)).Trim().TrimStart('0').Trim();
            string regionName = excelDoc.GetValue(curRow, 1).Trim();
            int refRegions = PumpRegion(regionCode, regionName);
            int refTypes = GetRefTypes();

            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
            PumpFactRow(factValue, refDate, refMarks, refRegions, refTypes, 0);
        }

        #endregion Отчеты в разрезе строк

        private bool IsSectionStart(string cellValue)
        {
            return ((sectionIndex != -1) && (cellValue.ToUpper() == "А"));
        }

        public const string MARK_REGION = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        public const string MARK_REGION_ALTAY = "НАЛОГОВЫЙ ОРГАН";
        private bool IsRegionRow(string cellValue)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion2008 || isTyvaRegion2010)
                return false;
            cellValue = cellValue.ToUpper();
            if (this.Region == RegionName.Altay)
                return cellValue.StartsWith(MARK_REGION_ALTAY);
            return (cellValue == MARK_REGION);
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        public const string TOTAL = "ВСЕГО";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW) || (cellValue.ToUpper().Contains(TOTAL) && (this.reportType == ReportType.Str));
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            sectionIndex = -1;
            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            string cutRow = string.Empty;
            int refMarks = nullMarks;
            int oldSectionIndex = -1;

            bool skipSection = false;

            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (cellValue.ToUpper().StartsWith("РАЗРЕЗ"))
                    {
                        cutRow = excelDoc.GetValue(curRow + 1, 1).Trim();
                        if (cutRow.ToUpper().Contains("КОНТРОЛЬНАЯ") && cutRow.ToUpper().Contains("СУММА"))
                        {
                            skipSection = true;
                            continue;
                        }
                        int marksCode = Convert.ToInt32(cutRow.Split(new char[] { '-' })[0].Trim());
                        refMarks = GetRefMarksByCode(marksCode);
                        SetSumMultiplier(marksCode);
                        continue;
                    }


                    if (IsSectionEnd(cellValue) && toPumpRow)
                    {
                        CheckXlsTotalSum(excelDoc, curRow);
                        if (((reportType != ReportType.Region) || ((this.Region != RegionName.Orenburg) && (this.DataSource.Year != 2007))) && (!((reportType == ReportType.Str) && (this.Region == RegionName.Kalmykya) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010)))))
                            sectionIndex = -1;
                        toPumpRow = false;
                        continue;
                    }

                    if (IsRegionRow(cellValue))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        continue;
                    }

                    if (toPumpRow)
                    {
                        if ((reportType == ReportType.Str) && (this.Region == RegionName.Kalmykya) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010)))
                            PumpXlsRowStrKalmykya(excelDoc, curRow, refDate, refMarks);
                        else if ((reportType == ReportType.Str) || isTyvaRegion2010)
                            PumpXlsRowStr(excelDoc, curRow, refDate);
                        else
                            PumpXlsRow(excelDoc, curRow, refDate, refRegions);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        if (skipSection)
                        {
                            skipSection = false;
                            for (; curRow <= rowsCount; curRow++)
                            {
                                cellValue = excelDoc.GetValue(curRow, 1).Trim();
                                if (IsSectionEnd(cellValue))
                                    break;
                            }

                            continue;
                        }
                        int columnsCount = 1;
                        if ((!((reportType == ReportType.Str) && (this.Region == RegionName.Kalmykya) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010))))&& ((reportType == ReportType.Str) || isTyvaRegion2010))
                        {
                            regionsRow = curRow;
                            excludedColumns.Clear();
                            columnsCount = PumpXlsRegionsStr(excelDoc, curRow);
                        }
                        totalSums = new decimal[columnsCount];
                        SetNullTotalSum();
                        toPumpRow = true;
                        continue;
                    }

                    if (cellValue.ToUpper().Contains("РАЗДЕЛ"))
                    {
                        if ((reportType == ReportType.Str) && (this.Region == RegionName.Kalmykya) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010)))
                            oldSectionIndex = sectionIndex;
                        sectionIndex = GetSectionIndex(cellValue);
                        if ((reportType == ReportType.Str) && (this.Region == RegionName.Kalmykya) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010)) && (sectionIndex < 0))
                            sectionIndex = oldSectionIndex;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
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
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            if (marksCode == 0)
                return string.Empty;
            if (this.DataSource.Year >= 2010)
            {
                if (((marksCode >= 1120) && (marksCode <= 1190)) || ((marksCode >= 2150) && (marksCode <= 2220)))
                    return ROUBLE_UNIT_NAME;
                return UNIT_UNIT_NAME;
            }
            else if (this.DataSource.Year >= 2009)
            {
                if (((marksCode >= 1120) && (marksCode <= 1180)) || ((marksCode >= 2120) && (marksCode <= 2180)))
                    return ROUBLE_UNIT_NAME;
                return UNIT_UNIT_NAME;
            }
            else
            {
                if (((marksCode >= 1120) && (marksCode <= 1150)) || ((marksCode >= 1200) && (marksCode <= 1220)) ||
                    ((marksCode >= 2120) && (marksCode <= 2150)) || ((marksCode >= 2200) && (marksCode <= 2220)))
                    return ROUBLE_UNIT_NAME;
                return UNIT_UNIT_NAME;
            }
        }

        private int GetRefUnits(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
            {
                string unitName = GetUnitName(marksCode);
                return FindCachedRow(cacheUnits, unitName, nullUnits);
            }
            return -1;
        }

        protected void SetRefUnits()
        {
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
                row["RefUnits"] = refUnits;
            }
        }

        private void SetClsHierarchy()
        {
            string d_Marks_FNS7_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2010)
                d_Marks_FNS7_HierarchyFileName = const_d_Marks_FNS7_HierarchyFile2010;
            else if (this.DataSource.Year >= 2009)
                d_Marks_FNS7_HierarchyFileName = const_d_Marks_FNS7_HierarchyFile2009;
            else
                d_Marks_FNS7_HierarchyFileName = const_d_Marks_FNS7_HierarchyFile2007;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS7_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy()
        {
            CommonSumCorrectionConfig commonCorrectionConfig = new CommonSumCorrectionConfig();
            commonCorrectionConfig.Sum1 = "Value";
            commonCorrectionConfig.Sum1Report = "ValueReport";
            GroupTable(fctIncomesTotal, new string[] { "RefMarks", "RefYearDayUNV", "RefTypes" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefYearDayUNV", "RefTypes" }, string.Empty, string.Empty, true);
            GroupTable(fctIncomesRegion, new string[] { "RefMarks", "RefYearDayUNV", "RefTypes", "RefRegions" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefYearDayUNV", "RefTypes" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByHierarchy();
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Выполняется корректировка сумм фактов по иерархии классификаторов");
        }

        #endregion Обработка данных

    }

}
