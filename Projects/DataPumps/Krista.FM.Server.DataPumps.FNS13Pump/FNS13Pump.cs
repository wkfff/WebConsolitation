using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS13Pump
{

    // ФНС - 0013 - Форма 5-УСН
    public class FNS13PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 УСН (d_Marks_FNS5YSN)
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
        private Dictionary<string, int> cacheRegionsByCode = null;
        private int nullRegions;
        // ЕдИзмер.ОКЕИ (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 УСН_Сводный (f_D_FNSYSNTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 УСН_Районы (f_D_FNS5YSNRegions)
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
        // номер строки с кодами регионов (для ставрополя)
        private int regionsRow;
        private string regionName = string.Empty;
        private string regionCode = string.Empty;
        // коэффициент перевода тысяч в рубли
        private decimal sumMultiplier = 1;

        private bool noSvodReports = false;
        private bool isStavropolRegion2008 = false;
        private bool isTyvaRegion2008 = false;

        private bool hasTitle = false;

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
        // код вид.лица "Индивидуальные предприниматели"
        private const int TYPE_INDIVID_ID = 3;

        // наименования секций для отчетов
        private string[] sectionNames = new string[]
        {
           "Раздел I. Отчет о налоговой базе и структуре начислений по налогу, уплачиваемому в связи с применением упрощенной системы налогообложения", 
           "Раздел I.I. Справочно к отчету",
        };

        private List<string> marksSectionNamesList = new List<string>(2);
        // список ай ди записей разделов в классификаторе Показатели.ФНС 5 УСН
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

        private const string D_MARKS_FNS_5YSN_GUID = "f37498f5-4c93-46dc-8563-5b2d7789437b";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5YSN_TOTAL_GUID = "2498f869-58c3-4972-9796-7bd3399b2698";
        private const string F_D_FNS_5YSN_REGIONS_GUID = "0afcadc6-4ff9-4b1c-a300-244931230d95";
        private const string FX_TYPES_PERSON_GUID = "40103871-5c06-4d22-aa45-55d7bcd6b942";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5YSN_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5YSN_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5YSN_REGIONS_GUID] };
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


        private int GetSectionIndex(string cellValue)
        {
            if (cellValue.Contains("РАЗДЕЛ I.I"))
                return 1;
            else if (cellValue.Contains("РАЗДЕЛ I"))
                return 0;
            return -1;
        }

        private void SetSumMultiplier(string marksCode)
        {
            switch (marksCode)
            {
                case "010":
                case "020":
                case "030":
                case "040":
                case "050":
                case "060":
                case "070":
                case "080":
                case "090":
                case "150":
                case "160":
                case "170":
                case "180":
                case "190":
                case "200":
                    sumMultiplier = 1000;
                    break;
                default:
                    sumMultiplier = 1;
                    break;
            }
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Trim().ToUpper().Trim('X').Trim('Х').PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 УСН' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isStavropolRegion2008 =
                (this.Region == RegionName.Stavropol) && (reportType == ReportType.Region) && (this.DataSource.Year >= 2008);
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
            noSvodReports = (this.Region == RegionName.Tyva);
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

        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow, int refTypes)
        {
            if (isStavropolRegion2008)
            {
                string comment = string.Format("в разрезе по графе {0}", refTypes);
                int sumsCount = totalSums.GetLength(0);
                for (int i = 0; i < sumsCount; i++)
                {
                    decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, i + 3));
                    CheckTotalSum(totalSums[i], controlSum, string.Format("по столбцу {0} {1}", i + 3, comment));
                }
            }
            else
            {
                string comment = string.Empty;
                if ((reportType == ReportType.Region))
                    comment = string.Format("для района '{0}' (код: {1})", regionName, regionCode);
                if (sectionIndex == 0)
                {
                    decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, 4));
                    CheckTotalSum(totalSums[1], controlSum, string.Format("по столбцу 4 {0}", comment));
                    controlSum = CleanFactValue(excelDoc.GetValue(curRow, 5));
                    CheckTotalSum(totalSums[2], controlSum, string.Format("по столбцу 5 {0}", comment));
                }
                else
                {
                    decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, 3));
                    CheckTotalSum(totalSums[0], controlSum, string.Format("по столбцу 3 {0}", comment));
                }
            }
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            if ((this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str))
            {
                if (cacheRegionsByCode.ContainsKey(regionCode))
                    return cacheRegionsByCode[regionCode];
            }

            if (regionName == string.Empty)
                regionName = regionCode;
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
            cacheRegionsByCode.Add(regionCode, Convert.ToInt32(regionRow["ID"]));
            // запоминаем регионы с уникальными наименованиями
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private void PumpFactRow(decimal factValue, int refDate, int refRegions, int refMarks, int refPerson, int sumIndex)
        {
            if (factValue == 0)
                return;

            totalSums[sumIndex] += factValue;
            factValue *= sumMultiplier;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] {
                    "RefYear", this.DataSource.Year, "RefMarks", refMarks, "RefTypes", refPerson,
                    "RefYearDayUNV", refDate, "ValueReport", factValue };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] {
                    "RefYear", this.DataSource.Year, "RefMarks", refMarks, "RefTypes", refPerson,
                    "RefYearDayUNV", refDate, "ValueReport", factValue, "RefRegions", refRegions };
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
            regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
            regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
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
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    regionName = excelDoc.GetValue(curRow, 5).Trim();
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        private int GetSectionByCode(int code)
        {
            if (code >= 150)
            {
                return 1;
            }
            else return 0;
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow, string name)
        {
            int code = nullMarks;
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if (value == string.Empty)
                return -1;
            code = Convert.ToInt32(value);
            if (this.Region == RegionName.SamaraGO)
            {
                sectionIndex = GetSectionByCode(code);
            }

            SetSumMultiplier(value.PadLeft(3, '0'));
            int parentId = marksSectionRecordsIDList[sectionIndex];

            object[] mapping = new object[] { "NAME", name, "CODE", code, "PARENTID", parentId };
            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private int GetPersonRef(int curColumn)
        {
            if (curColumn == 1)
                return TYPE_URID_ID;
            if (curColumn == 2)
                return TYPE_INDIVID_ID;
            return TYPE_ALL_ID;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, string markName, int refRegions, int refDate)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, markName);
            if (refMarks == -1)
                return;

            if (sectionIndex == 0)
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                PumpFactRow(factValue, refDate, refRegions, refMarks, TYPE_URID_ID, 1);
                factValue = CleanFactValue(excelDoc.GetValue(curRow, 5));
                PumpFactRow(factValue, refDate, refRegions, refMarks, TYPE_INDIVID_ID, 2);
            }
            else
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                PumpFactRow(factValue, refDate, refRegions, refMarks, TYPE_ALL_ID, 0);
            }
        }

        #region Ставрополь

        private int FindRegion(string regionCode, string regionName)
        {
            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            regionKey = string.Format("{0}|{1} ({0})", regionCode, regionName); ;
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            return nullRegions;
        }

        private const string SUF = "[SUF]";
        private int PumpXlsRegionsStavropol(ExcelHelper excelDoc, int curRow)
        {
            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(curRow, curCol).Trim();
                if (cellValue.ToUpper() == SUF)
                {
                    return (curCol - 3);
                }
                regionCode = CommonRoutines.TrimLetters(cellValue);
                regionName = excelDoc.GetValue(curRow - 1, curCol).Trim();
                PumpRegion(regionCode, regionName);
            }
        }

        private void PumpXlsRowStavropol(ExcelHelper excelDoc, int curRow, string markName, int refDate, int refTypes)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, markName);
            if (refMarks == -1)
                return;

            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(regionsRow, curCol).Trim();
                if (cellValue.ToUpper() == SUF)
                    return;

                regionCode = CommonRoutines.TrimLetters(cellValue);
                regionName = excelDoc.GetValue(regionsRow - 1, curCol).Trim();
                int refRegions = FindRegion(regionCode, regionName);

                decimal valueReport = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                PumpFactRow(valueReport, refDate, refRegions, refMarks, refTypes, curCol - 3);
            }
        }

        #endregion Ставрополь

        #region Калмыкия

        private void PumpXlsRowStrKalmykya(ExcelHelper excelDoc, int curRow, int refDate, int refMarks)
        {
            string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 2)).Trim().TrimStart('0').Trim().PadLeft(3,'0');
            string regionName = excelDoc.GetValue(curRow, 1).Trim();
            int refRegions = PumpRegion(regionCode, regionName);

            if (sectionIndex == 0)
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                PumpFactRow(factValue, refDate, refRegions, refMarks, TYPE_URID_ID, 1);
                factValue = CleanFactValue(excelDoc.GetValue(curRow, 5));
                PumpFactRow(factValue, refDate, refRegions, refMarks, TYPE_INDIVID_ID, 2);
            }
            else
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                PumpFactRow(factValue, refDate, refRegions, refMarks, TYPE_ALL_ID, 0);
            }
        }

        #endregion

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        // Муниципальное образование
        private const string MARK_REGION = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private bool IsRegionRow(string cellValue)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion2008)
                return false;
            return (cellValue.ToUpper().StartsWith(MARK_REGION));
        }

        private const string CUT_ROW = "РАЗРЕЗ ПО ГРАФЕ";
        private bool IsCutRow(string cellValue)
        {
            if (isStavropolRegion2008)
                return cellValue.ToUpper().StartsWith(CUT_ROW);
            return false;
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        public const string TOTAL_ROW1 = "ВСЕГО";
        private bool IsSectionEnd(string cellValue)
        {
            if ((this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str))
                return cellValue.ToUpper().Contains(TOTAL_ROW1);
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            return (cellValue.Trim().ToUpper() == AUX_TABLE_MARK_REGION);
        }

        // получаем полное наименование показателя,
        // т.к. оно может находиться на нескольких строках
        private string GetXlsMarksName(ExcelHelper excelDoc, ref int curRow)
        {
            List<string> marksName = new List<string>();
            while (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
            {
                marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
                curRow++;
            }
            marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
            return string.Join(" ", marksName.ToArray());
        }

        private int GetRefMarksByCode(int marksCode)
        {
            if (cacheMarks.ContainsKey(marksCode.ToString()))
                return cacheMarks[marksCode.ToString()];
            return nullMarks;
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            int refTypes = -1;
            bool toPumpRow = false;
            sectionIndex = -1;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            string cutRow;
            bool skipSection = false;

            int refMarks = -1;

            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                    {
                        if (sectionIndex == 1)
                            toPumpRow = false;
                        continue;
                    }

                    if (IsRegionRow(cellValue))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        continue;
                    }

                    if (IsCutRow(cellValue))
                    {
                        string value = excelDoc.GetValue(curRow + 1, 1).Trim();
                        refTypes = Convert.ToInt32(value.Split('-')[0].Trim());
                        continue;
                    }

                    if (cellValue.ToUpper().StartsWith("РАЗРЕЗ") && (!toPumpRow))
                    {
                        cutRow = excelDoc.GetValue(curRow + 1, 1).Trim();
                        if (cutRow.ToUpper().Contains("КОНТРОЛЬНАЯ") && cutRow.ToUpper().Contains("СУММА"))
                        {
                            skipSection = true;
                            continue;
                        }
                        int marksCode = Convert.ToInt32(cutRow.Split(new char[] { '-' })[0].Trim().PadLeft(3, '0'));
                        refMarks = GetRefMarksByCode(marksCode);
                        SetSumMultiplier(marksCode.ToString().PadLeft(3,'0'));
                        continue;
                    }

                    if ((IsSectionEnd(cellValue)) && toPumpRow)
                    {
                        CheckXlsTotalSum(excelDoc, curRow, refTypes);
                        if (!hasTitle)
                            toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        if ((reportType == ReportType.Str) && (this.Region == RegionName.Kalmykya))
                            PumpXlsRowStrKalmykya(excelDoc, curRow, refDate, refMarks);
                        else
                        {
                            string markName = GetXlsMarksName(excelDoc, ref curRow);
                            if (isStavropolRegion2008)
                                PumpXlsRowStavropol(excelDoc, curRow, markName, refDate, refTypes);
                            else
                                PumpXlsRow(excelDoc, curRow, markName, refRegions, refDate);
                        }
                        continue;
                    }

                    if ((IsSectionStart(cellValue)) && (!toPumpRow))
                    {
                        if (skipSection)
                        {
                            skipSection = false;
                            continue;
                        }

                        // в сводных отчетах и райнов иногда сверху добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1)))
                            continue;

                        if (isTyvaRegion2008)
                            sectionIndex = 0;

                        int columnsCount = 3;
                        if (isStavropolRegion2008)
                        {
                            regionsRow = curRow;
                            columnsCount = PumpXlsRegionsStavropol(excelDoc, curRow);
                        }
                        totalSums = new decimal[columnsCount];
                        SetNullTotalSum();
                        toPumpRow = true;
                        continue;
                    }

                    if ((cellValue.ToUpper().Contains("РАЗДЕЛ")) && (!toPumpRow))
                        sectionIndex = GetSectionIndex(cellValue.ToUpper());
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
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
                hasTitle = false;

                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    string worksheetName = excelDoc.GetWorksheetName().Trim().ToUpper();

                    if (IsTitleSheet(worksheetName))
                        refRegions = PumpXlsRegionsTyva(excelDoc);
                    else
                    {
                        if ((this.Region == RegionName.SamaraGO) && worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ") && (this.DataSource.Year >= 2008) && (this.reportType == ReportType.Svod))
                            hasTitle = true;
                        else PumpXlsSheetData(file.Name, excelDoc, refDate, refRegions);
                    }
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
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
            {
                switch (marksCode)
                {
                    case 100:
                    case 110:
                    case 120:
                    case 130:
                        return UNIT_UNIT_NAME;
                    case 10:
                    case 20:
                    case 30:
                    case 40:
                    case 50:
                    case 60:
                    case 70:
                    case 80:
                    case 90:
                    case 150:
                    case 160:
                    case 170:
                    case 180:
                    case 190:
                    case 200:
                        return ROUBLE_UNIT_NAME;
                }
            }
            return string.Empty;
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
            if (cacheUnits.Count <= 1)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, "Классификатор «ЕдИзмер.ОКЕИ» не заполнен.");
            }
            else
            {
                foreach (DataRow row in dsMarks.Tables[0].Rows)
                {
                    int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
                    row["RefUnits"] = refUnits;
                }
            }
        }

        private void SetClsHierarchy()
        {
            string d_Marks_FNS13_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2007)
                d_Marks_FNS13_HierarchyFileName = const_d_Marks_FNS13_HierarchyFile2007;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS13_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy()
        {
            CommonSumCorrectionConfig commonCorrectionConfig = new CommonSumCorrectionConfig();
            commonCorrectionConfig.Sum1 = "Value";
            commonCorrectionConfig.Sum1Report = "ValueReport";
            GroupTable(fctIncomesTotal, new string[] { "RefMarks", "RefTypes", "RefYearDayUNV" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTypes" }, string.Empty, string.Empty, true);
            GroupTable(fctIncomesRegion, new string[] { "RefMarks", "RefTypes", "RefYearDayUNV", "RefRegions" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTypes" }, "RefRegions", string.Empty, true);
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
            ProcessDataSourcesTemplate(year, month, "Выполняется установка иерархии классификатора «Показатели.ФНС 5 УСН», корректировка сумм фактов по иерархии классификаторов и установка ссылок с классификатора «Показатели.ФНС 5 УСН» на классификатор «ЕдИзмер.ОКЕИ»");
        }
        
        #endregion Обработка данных
        
    }

}