using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS22Pump
{

    // ФНС - 0022 - Форма 5-МН
    public class FNS22PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 МН (d_Marks_FNS5MN)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<int, int> cacheMarks = null;
        private int nullMarks;
        // Районы.ФНС (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, DataRow> cacheRegions = null;
        private Dictionary<string, DataRow> cacheRegionsByCode = null;
        private Dictionary<string, string> cacheRegionsNames = null;
        private Dictionary<string, DataRow> cacheRegionsFirstRow = null;
        private int nullRegions;
        // ЕдИзмер.ОКЕИ (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 МН_Сводный (f_D_FNS5MNTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 МН_Районы (f_D_FNS5MNRegions)
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
        private int regionsRow;
        // номера столбцов, из которых не следует качать данные
        // (для файлов по строкам и файлов по районам для Тывы-2010)
        private List<int> excludedColumns = new List<int>();

        private bool noSvodReports = false;
        private bool isTyvaRegion2008 = false;
        private bool isTyvaRegion2010 = false;
        private bool isAltayKraiRegion2008 = false;
        private bool isBarnaulFile = false;
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

        // код вид.лица "Юридическое лицо"
        private const int TYPE_URID_ID = 2;

        // код вид.лица "Физическое лицо"
        private const int TYPE_PHYS_ID = 3;

        // количество разделов за 2005 год
        private const int SECTION_COUNT_2005 = 4;

        // количество разделов за 2006 год
        private const int SECTION_COUNT_2006 = 2;

        // количество разделов за 2007 год
        private const int SECTION_COUNT_2007 = 3;

        // наименования секций для отчетов
        private string[] sectionNames = new string[]
        {
           // для отчетов за 2005 год
           "Раздел I. Отчет о налоговой базе и структуре начислений по земельному налогу",
           "Раздел II. Отчет о налоговой базе и структуре начислений по земельному налогу",
           "Раздел III. Отчет о налоговой базе и структуре начислений по налогу на имущество физических лиц",
           "Раздел IV. Отчет о налоговой базе и структуре начислений по налогу с имущества, переходящего в порядке наследования или дарения",
           // для отчетов за 2006 год
           "Раздел I. Отчет о налоговой базе и структуре начислений по земельному налогу",
           "Раздел II. Отчет о налоговой базе и структуре начислений по налогу на имущество физических лиц",
           // для отчетов за 2007 год
           "Раздел I. Отчет о налоговой базе и структуре начислений по земельному налогу по юридическим лицам",
           "Раздел II. Отчет о налоговой базе и структуре начислений по земельному налогу по физическим лицам",
           "Раздел III. Отчет о налоговой базе и структуре начислений по налогу на имущество физических лиц"
        };

        private List<string> marksSectionNamesList = new List<string>(9);
        // список ай ди записей разделов в классификаторе Показатели.ФНС 5 МН
        private List<int> marksSectionRecordsIDList = new List<int>(9);

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        // получить ай ди записей разделов классификатора показатели мн
        private void GetMarksParentId()
        {
            marksSectionRecordsIDList.Clear();
            marksSectionNamesList.Clear();
            marksSectionNamesList.AddRange(sectionNames);
            marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            // у записей разделов код равен нулю
            // получаем ай ди существующих записей разделов, если каких то разделов нет - закачиваем
            DataRow[] marksSectionRecords = dsMarks.Tables[0].Select("CODE = 0");
            foreach (DataRow marksSectionRecord in marksSectionRecords)
            {
                string sectionName = marksSectionRecord["NAME"].ToString().ToUpper();
                sectionIndex = GetSectionIndex(sectionName);
                if (this.DataSource.Year > 2005)
                    sectionIndex += SECTION_COUNT_2005;
                if (this.DataSource.Year > 2006)
                    sectionIndex += SECTION_COUNT_2006;
                if (sectionIndex == -1)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = Convert.ToInt32(marksSectionRecord["ID"]);
            }

            int startIndex = 0;
            int endIndex = SECTION_COUNT_2005 - 1;
            if (this.DataSource.Year > 2005)
            {
                startIndex = endIndex + 1;
                endIndex = endIndex + SECTION_COUNT_2006;
            }
            if (this.DataSource.Year > 2006)
            {
                startIndex = endIndex + 1;
                endIndex = endIndex + SECTION_COUNT_2007;
            }

            for (sectionIndex = startIndex; sectionIndex <= endIndex; sectionIndex++)
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
            FillRowsCache(ref cacheRegionsByCode, dsRegions.Tables[0], new string[] { "CODE" });
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

        private const string D_MARKS_FNS_5MN_GUID = "bd362d6e-75ff-45d8-a5c3-ad98400ae6fc";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5MN_TOTAL_GUID = "909bda9d-cb4f-4763-9fb6-7cb28851ec7b";
        private const string F_D_FNS_5MN_REGIONS_GUID = "dfe4a1d4-67c3-41c8-8d6b-44e158f80464";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5MN_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5MN_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5MN_REGIONS_GUID] };
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

        #region Классификаторы и факты

        private int GetRefMarksByCode(int marksCode)
        {
            if (cacheMarks.ContainsKey(marksCode))
                return cacheMarks[marksCode];
            return nullMarks;
        }

        private int PumpMark(string markCodeStr, string markName, int markParentId)
        {
            markCodeStr = markCodeStr.Trim();
            if (markCodeStr == string.Empty)
                return nullMarks;

            int markCode = Convert.ToInt32(markCodeStr);
            SetSumMultiplier(markCode);
            markName = DeleteThousandRouble(markName);

            object[] mapping = new object[] { "NAME", markName, "CODE", markCode, "PARENTID", markParentId };
            if ((reportType != ReportType.Region) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, markCode, "CODE", mapping);
            return GetRefMarksByCode(markCode);
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            if (regionName == string.Empty)
                regionName = regionCode;
            if ((this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010)))
            {
                if (cacheRegionsByCode.ContainsKey(regionCode))
                    return Convert.ToInt32(cacheRegionsByCode[regionCode]["ID"]);
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
            cacheRegionsByCode.Add(regionCode, regionRow);
            // запоминаем регионы с уникальными наименованиями
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private void PumpFactRow(decimal factValue, int refDate, int refMarks, int refRegions, int refTypes, int sumIndex)
        {
            if (factValue == 0)
                return;

            if (sumIndex != -1)
                totalSums[sumIndex] += factValue;
            factValue *= sumMultiplier;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] { "ValueReport", factValue, "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefTypes", refTypes };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "ValueReport", factValue, "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions, "RefTypes", refTypes };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        #endregion Классификаторы и факты

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

        // Регулярное выражение для поиска сочетания (тыс. руб.) или (тыс.руб.)
        Regex regExThousandRobule = new Regex(@"\(тыс\.(.*)руб\.\)", RegexOptions.IgnoreCase);
        private string DeleteThousandRouble(string value)
        {
            return regExThousandRobule.Replace(value, String.Empty).Trim();
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private void SetSumMultiplier(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
            {
                switch (marksCode)
                {
                    case 1500:
                    case 1600:
                    case 1700:
                    case 1800:
                    case 1810:
                    case 1820:
                    case 2700:
                    case 2800:
                    case 2810:
                    case 2820:
                    case 2900:
                    case 3000:
                    case 3010:
                    case 3020:
                    case 3300:
                    case 3310:
                    case 3400:
                    case 3410:
                    case 3700:
                    case 3710:
                    case 3720:
                        sumMultiplier = 1000;
                        break;
                    case 2500:
                    case 2600:
                        if (this.DataSource.Year >= 2010)
                            sumMultiplier = 1;
                        else
                            sumMultiplier = 1000;
                        break;
                    default:
                        sumMultiplier = 1;
                        break;
                }
            }
            else
            {
                switch (marksCode)
                {
                    case 500:
                    case 600:
                    case 700:
                    case 800:
                    case 810:
                    case 820:
                    case 1300:
                    case 1310:
                    case 1400:
                    case 1410:
                    case 1700:
                        sumMultiplier = 1000;
                        break;
                    default:
                        sumMultiplier = 1;
                        break;
                }
            }
        }

        // получить номер раздела по значению ячейки (для всех, кроме тывы-2008)
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
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 МН' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isTyvaRegion2008 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year <= 2008);
            isTyvaRegion2010 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year >= 2010);
            isAltayKraiRegion2008 =
                (this.Region == RegionName.AltayKrai) && (reportType == ReportType.Region) && (this.DataSource.Year <= 2008);
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            SetFlags();
            CheckMarks();
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.txt", new ProcessFileDelegate(PumpTxtFile), false);
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
            noSvodReports = (this.Region == RegionName.OmskMO) ||
                ((this.Region == RegionName.AltayKrai) && (this.DataSource.Year <= 2008)) ||
                ((this.Region == RegionName.MoskvaObl) && (this.DataSource.Year <= 2008)) ||
                ((this.Region == RegionName.Tyva) && (this.DataSource.Year <= 2008 || this.DataSource.Year >= 2010));
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

        // проверка итоговой суммы в xls-отчетах
        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            if ((reportType == ReportType.Str) || isTyvaRegion2010 || isAltayKraiRegion2008)
            {
                int sumsCount = totalSums.GetLength(0);
                for (int i = 0; i < sumsCount; i++)
                {
                    decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, i + 3));
                    CheckTotalSum(totalSums[i], controlSum, string.Format("по стобцу {0} в разделе {1}", i + 3, sectionIndex + 1));
                }
            }
            else
            {
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, 3));
                if (sectionIndex == 0)
                {
                    CheckTotalSum(totalSums[0], controlSum, string.Format("по стобцу {0} в разделе {1}", 3, sectionIndex + 1));
                    if (this.DataSource.Year < 2007)
                    {
                        controlSum = CleanFactValue(excelDoc.GetValue(curRow, 4));
                        CheckTotalSum(totalSums[1], controlSum, string.Format("по стобцу {0} в разделе {1}", 4, sectionIndex + 1));
                    }
                }
                if ((sectionIndex == 1) || (sectionIndex == 2))
                {
                    CheckTotalSum(totalSums[0], controlSum, string.Format("по стобцу {0} в разделе {1}", 3, sectionIndex + 1));
                }
            }
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            int index = sectionIndex;
            if (this.DataSource.Year >= 2006)
                index += SECTION_COUNT_2005;
            if (this.DataSource.Year >= 2007)
                index += SECTION_COUNT_2006;

            int markParentId = marksSectionRecordsIDList[index];
            string markCode = excelDoc.GetValue(curRow, 2).Trim();
            string markName = excelDoc.GetValue(curRow, 1).Trim();

            return PumpMark(markCode, markName, markParentId);
        }

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            string regionName = string.Empty;
            string regionCode = string.Empty;
            string cellValue = excelDoc.GetValue(curRow, 1).Trim();
            if (this.Region == RegionName.Altay)
            {
                regionCode = CommonRoutines.TrimLetters(cellValue);
                regionName = excelDoc.GetValue(curRow - 2, 1).Trim();
            }
            else if (cellValue.ToUpper().StartsWith(MARK_TAX_DEP))
            {
                regionCode = CommonRoutines.TrimLetters(cellValue);
                regionName = regionCode;
            }
            else
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1));
                regionName = excelDoc.GetValue(curRow + 1, 1);
            }
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsRegionsTyva(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow < rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                if (cellValue == REGION_ROW)
                {
                    string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    string regionName = excelDoc.GetValue(curRow, 5).Trim();
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        // закачка строки xls-отчета
        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == nullMarks)
                return;

            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
            if (sectionIndex == 0)
            {
                PumpFactRow(factValue, refDate, refMarks, refRegions, TYPE_URID_ID, 0);
                if (this.DataSource.Year < 2007)
                {
                    factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                    PumpFactRow(factValue, refDate, refMarks, refRegions, TYPE_PHYS_ID, 1);
                }
            }
            if ((sectionIndex == 1) || (sectionIndex == 2))
            {
                PumpFactRow(factValue, refDate, refMarks, refRegions, TYPE_PHYS_ID, 0);
            }
        }

        #region Алтайский край

        private int GetFactColumn(int marksCode)
        {
            if (marksCode == 10)
                return 6;
            if (marksCode == 20)
                return 9;
            if (marksCode == 30)
                return 12;
            if (marksCode == 40)
                return 15;
            if (marksCode == 50)
                return 18;
            return 1;
        }

        private int GetRefTypesByMarksCode(int marksCode)
        {
            if (marksCode < 2000)
                return 2;
            return 3;
        }

        private void PumpXlsRowAltayKrai(ExcelHelper excelDoc, int curRow, int startRow, int refDate)
        {
            string regionCode = excelDoc.GetValue(curRow, 2).Trim();
            string regionName = excelDoc.GetValue(curRow, 4).Trim();
            int refRegions = PumpRegion(regionCode, regionName);

            for (int curCol = 5; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(startRow, curCol).Trim();
                if (cellValue == string.Empty)
                {
                    cellValue = excelDoc.GetValue(startRow, curCol + 1).Trim();
                    if (cellValue == string.Empty)
                        return;
                    continue;
                }

                int marksCode = Convert.ToInt32(cellValue.Split(new char[] { '-' })[0].Trim());
                int refMarks = GetRefMarksByCode(marksCode);
                SetSumMultiplier(marksCode);

                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                PumpFactRow(factValue, refDate, refMarks, refRegions, GetRefTypesByMarksCode(marksCode), -1);
            }
        }

        private void PumpXlsSheetDataAltayKrai(string fileName, ExcelHelper excelDoc, int refDate)
        {
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();
            int startRow = 1;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 2).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (toPumpRow)
                    {
                        PumpXlsRowAltayKrai(excelDoc, curRow, startRow, refDate);
                        continue;
                    }

                    if (cellValue.ToUpper() == "А")
                    {
                        toPumpRow = true;
                        startRow = curRow;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        private void PumpXlsRowAltayKraiStr(ExcelHelper excelDoc, int curRow, int refDate, int refMarks, int refTypes)
        {
            //в районы.фнс поле код целое. для карелии в отчете по строкам уникальность районам проверяем по коду. поэтому нужно убрать нули с лева
            string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 2)).Trim().TrimStart('0').Trim();
            string regionName = excelDoc.GetValue(curRow, 1).Trim();
            int refRegions = PumpRegion(regionCode, regionName);

            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
            PumpFactRow(factValue, refDate, refMarks, refRegions, refTypes, 0);
        }

        private void PumpXlsSheetDataAltayKraiStr(string fileName, ExcelHelper excelDoc, int refDate)
        {
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();
            int refTypes = -1;
            int refMarks = nullMarks;
            string cutRow = string.Empty;
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
                        if (cutRow.ToUpper().Contains("КОНТРОЛЬНАЯ"))
                        {
                            refMarks = nullMarks;
                        }
                        else
                        {
                            int marksCode = Convert.ToInt32(cutRow.Split(new char[] { '-' })[0].Trim());
                            if (this.Region == RegionName.Kalmykya)
                                SetSumMultiplier(marksCode);
                            refMarks = GetRefMarksByCode(marksCode);
                            refTypes = GetRefTypesByMarksCode(marksCode);
                        }
                        continue;
                    }

                    if (cellValue.ToUpper().StartsWith("ВСЕГО"))
                    {
                        decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, 3));
                        CheckTotalSum(totalSums[0], controlSum, string.Format("в разрезе по строке '{0}'", cutRow));
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        PumpXlsRowAltayKraiStr(excelDoc, curRow, refDate, refMarks, refTypes);
                        continue;
                    }

                    if ((cellValue.ToUpper() == "А") && (refMarks != nullMarks))
                    {
                        toPumpRow = true;
                        totalSums = new decimal[1];
                        SetNullTotalSum();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        #endregion Алтайский край

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
        private int PumpXlsRegionsStr(ExcelHelper excelDoc)
        {
            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(regionsRow, curCol).Trim();
                if ((cellValue == string.Empty) || (cellValue.ToUpper() == SUF))
                {
                    return (curCol - 3);
                }
                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(regionsRow - 1, curCol).Trim();
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

        private void PumpXlsRowStr(ExcelHelper excelDoc, int curRow, int refDate, int refTypes)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == nullMarks)
                return;

            for (int curCol = 3; ; curCol++)
            {
                if (excludedColumns.Contains(curCol))
                    continue;
                string cellValue = excelDoc.GetValue(regionsRow, curCol).Trim();
                if ((cellValue == string.Empty) || (cellValue.ToUpper() == SUF))
                    return;

                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(regionsRow - 1, curCol).Trim();
                int refRegions = FindRegion(regionCode, regionName);

                decimal valueReport = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                PumpFactRow(valueReport, refDate, refMarks, refRegions, refTypes, curCol - 3);
            }
        }

        #endregion Отчеты в разрезе строк

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            if (reportType == ReportType.Region)
                return (cellValue.ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        private bool IsSectionStart(string cellValue)
        {
            return ((sectionIndex != -1) && (cellValue.ToUpper() == "А"));
        }

        private const string CUT_ROW = "РАЗРЕЗ ПО ГРАФЕ";
        private bool IsCutRow(string cellValue)
        {
            if ((reportType == ReportType.Str) || isTyvaRegion2010)
                return cellValue.Trim().ToUpper().StartsWith(CUT_ROW);
            return false;
        }

        private const string REGION_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private const string MARK_REGION = "КОД ОКАТО";
        private const string MARK_TAX_DEP = "НАЛОГОВЫЙ ОРГАН";
        private bool IsRegionRow(ExcelHelper excelDoc, int curRow)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion2008 || isTyvaRegion2010 || isAltayKraiRegion2008)
                return false;
            string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            if (cellValue.StartsWith(REGION_ROW))
                return excelDoc.GetValue(curRow + 2, 1).Trim().ToUpper().StartsWith(MARK_REGION);
            return cellValue.StartsWith(MARK_TAX_DEP);
        }

        private const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private int GetFirstColumn(ExcelHelper excelDoc, int curRow)
        {
            for (int curCol = 1; curCol <= 5; curCol++)
                if (excelDoc.GetValue(curRow, curCol).Trim() != string.Empty)
                    return curCol;
            return 6;
        }

        private int GetRefTypes(string cellValue)
        {
            cellValue = cellValue.Trim().ToUpper();
            if (this.DataSource.Year >= 2007)
            {
                if (sectionIndex == 0)
                    return TYPE_URID_ID;
                if ((sectionIndex == 1) || (sectionIndex == 2))
                    return TYPE_PHYS_ID;
            }
            else
            {
                if (cellValue.Contains("1 - ПО ОРГАНИЗАЦИЯМ И ИНДИВИДУАЛЬНЫМ ПРЕДПРИНИМАТЕЛЯМ"))
                    return TYPE_URID_ID;
                if (cellValue.Contains("2 - ПО ФИЗИЧЕСКИМ ЛИЦАМ") || cellValue.Contains("1 - ЗНАЧЕНИЕ ПОКАЗАТЕЛЕЙ"))
                    return TYPE_PHYS_ID;
            }
            return -1;
        }

        private const string CONST_SECTION = "РАЗДЕЛ";
        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            SetSectionIndex(excelDoc.GetWorksheetName());
            bool toPumpRow = false;
            int refTypes = -1;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();

            //для самары го
            int indexStartSection = 1;
            int startSection = 1;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    int curCol = GetFirstColumn(excelDoc, curRow);
                    if (curCol > 5)
                        continue;
                    string cellValue = excelDoc.GetValue(curRow, curCol).Trim();
                    
                    if (cellValue.ToUpper().StartsWith(CONST_SECTION) && (!isTyvaRegion2008))
                    #region начало раздела
                    {
                        sectionIndex = GetSectionIndex(cellValue);
                        if (sectionIndex == 0)
                            refTypes = 2;
                        else
                            refTypes = 3;
                        continue;
                    }
                    #endregion

                    if ((!hasTitle) && (this.Region == RegionName.SamaraGO))
                    {
                        if ((curRow >= 10) && (curRow <= 20))
                        {
                            if (cellValue.ToUpper().Contains("МУНИЦИПАЛЬНОЕ") && cellValue.ToUpper().Contains("ОБРАЗОВАНИЕ"))
                            {
                                startSection = 2;
                            }
                        }
                    }

                    if (IsRegionRow(excelDoc, curRow))
                    #region строка с районом
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        curRow += 2;
                        continue;
                    }
                    #endregion

                    if (IsCutRow(cellValue))
                    #region разрез по графе
                    {
                        refTypes = GetRefTypes(excelDoc.GetValue(curRow + 1, curCol));
                        continue;
                    }
                    #endregion

                    if (IsSectionEnd(cellValue))
                    #region конец раздела - контрольная сумма
                    {
                        CheckXlsTotalSum(excelDoc, curRow);
                        toPumpRow = false;
                        continue;
                    }
                    #endregion

                    if (toPumpRow)
                    #region закачка строки
                    {
                        if ((reportType == ReportType.Str) || isTyvaRegion2010 || isAltayKraiRegion2008)
                            PumpXlsRowStr(excelDoc, curRow, refDate, refTypes);
                        else
                            PumpXlsRow(excelDoc, curRow, refDate, refRegions);
                        continue;
                    }
                    #endregion

                    if (IsSectionStart(cellValue))
                    #region строка с записью "A" - старт закачки
                    {
                        // в некоторых отчетах райнов перед основными разделами добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1).Trim()))
                            continue;

                        //в сводных отчетах если нет титульного, то пропускаем первую секцию А
                        if ((this.Region == RegionName.SamaraGO) && (indexStartSection < startSection))
                        {
                            indexStartSection++;
                            continue;
                        }

                        int columnCount = 2;
                        if ((reportType == ReportType.Str) || isTyvaRegion2010 || isAltayKraiRegion2008)
                        {
                            regionsRow = curRow;
                            excludedColumns.Clear();
                            columnCount = PumpXlsRegionsStr(excelDoc);
                        }
                        totalSums = new decimal[columnCount];
                        SetNullTotalSum();

                        toPumpRow = true;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            if (file.Name.ToUpper().Contains("БАРНАУЛ"))
            {
                isBarnaulFile = (this.Region == RegionName.AltayKrai) && (this.DataSource.Year <= 2008);
                isAltayKraiRegion2008 = false;
            }
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int refRegions = nullRegions;
                int wsCount = excelDoc.GetWorksheetsCount();
                hasTitle = false;
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    string worksheetName = excelDoc.GetWorksheetName().Trim().ToUpper();

                    if (isTyvaRegion2008)
                    {
                        if (worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ"))
                            refRegions = PumpXlsRegionsTyva(excelDoc);
                    }
                    else if ((this.Region == RegionName.AltayKrai) && (this.DataSource.Year == 2009))
                    {
                        if ((worksheetName == "5MN") && (reportType == ReportType.Svod))
                            PumpXlsSheetData(file.Name, excelDoc, refDate, nullRegions);
                        if ((worksheetName == "МО") && (reportType == ReportType.Region))
                            PumpXlsSheetDataAltayKrai(file.Name, excelDoc, refDate);
                    }
                    else if ((this.Region == RegionName.AltayKrai) && (reportType == ReportType.Str))
                    {
                        PumpXlsSheetDataAltayKraiStr(file.Name, excelDoc, refDate);
                    }
                    else if ((this.Region == RegionName.SamaraGO) && (worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ")))
                    {
                        hasTitle = true;
                        continue;
                    }
                    else if ((this.Region == RegionName.Kalmykya) && (this.reportType == ReportType.Str) && ((this.DataSource.Year == 2009) || (this.DataSource.Year == 2010)))
                    {
                        PumpXlsSheetDataAltayKraiStr(file.Name, excelDoc, refDate);
                    }
                    else
                    {
                        PumpXlsSheetData(file.Name, excelDoc, refDate, refRegions);
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

        #region Работа с Txt

        // проверка итоговой суммы в текстовых файлах
        private void CheckTxtTotalSum(string[] rowReport)
        {
            decimal controlSum;
            if (sectionIndex == 0)
            {
                controlSum = Convert.ToDecimal(rowReport[3].Trim());
                CheckTotalSum(totalSums[0], controlSum, "по организациям и индивидуальным предпринимателям");
                controlSum = Convert.ToDecimal(rowReport[4].Trim());
                CheckTotalSum(totalSums[1], controlSum, "по физическим лицам");
            }
            if (sectionIndex == 1)
            {
                controlSum = Convert.ToDecimal(rowReport[3].Trim());
                CheckTotalSum(totalSums[0], controlSum, "по физическим лицам");
            }
        }

        private int PumpTxtMarks(string[] rowReport, string markName)
        {
            int index = sectionIndex;
            if (this.DataSource.Year > 2005)
                index += SECTION_COUNT_2005;
            if (this.DataSource.Year > 2006)
                index += SECTION_COUNT_2006;

            int markParentID = marksSectionRecordsIDList[index];
            string markCode = rowReport[2].Trim();

            return PumpMark(markCode, markName, markParentID);
        }

        private void PumpTxtRow(string[] rowReport, string marksName, int refDate)
        {
            int refMarks = PumpTxtMarks(rowReport, marksName);
            if (refMarks == nullMarks)
                return;

            decimal factValue;
            if (sectionIndex == 0)
            {
                factValue = Convert.ToDecimal(rowReport[3].Trim());
                PumpFactRow(factValue, refDate, refMarks, nullRegions, TYPE_URID_ID, 0);
                factValue = Convert.ToDecimal(rowReport[4].Trim());
                PumpFactRow(factValue, refDate, refMarks, nullRegions, TYPE_PHYS_ID, 1);
            }
            if (sectionIndex == 1)
            {
                factValue = Convert.ToDecimal(rowReport[3].Trim());
                PumpFactRow(factValue, refDate, refMarks, nullRegions, TYPE_PHYS_ID, 0);
            }
        }

        // код разделителя "|"
        private const int DELIMETER_CODE = 9474;
        private char DELIMETER = Convert.ToChar(DELIMETER_CODE);
        private void PumpTxtReport(FileInfo file, int refDate)
        {
            string[] txtReport = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtDosCodePage());
            string marksName = string.Empty;
            bool toPumpRow = false;
            int rowsCount = txtReport.Length;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 0; curRow < rowsCount - 1; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = txtReport[curRow].Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (cellValue.ToUpper().StartsWith("РАЗДЕЛ"))
                    {
                        sectionIndex = GetSectionIndex(cellValue.ToUpper());
                        continue;
                    }

                    if (cellValue[0] != DELIMETER)
                        continue;

                    string[] rowsReport = cellValue.Split(DELIMETER);
                    if (IsSectionEnd(rowsReport[1]))
                    {
                        CheckTxtTotalSum(rowsReport);
                        toPumpRow = false;
                    }

                    if (toPumpRow)
                    {
                        marksName = string.Format("{0} {1}", marksName.Trim(), rowsReport[1].Trim());
                        if (rowsReport[2].Trim() != string.Empty)
                        {
                            PumpTxtRow(rowsReport, marksName, refDate);
                            marksName = string.Empty;
                        }
                    }

                    if (rowsReport[1].Trim().ToUpper() == "А")
                        toPumpRow = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpTxtFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            try
            {
                cacheRegionsNames = new Dictionary<string, string>();
                int refDate = GetReportDate();
                PumpTxtReport(file, refDate);
            }
            finally
            {
                cacheRegionsNames.Clear();
                GC.GetTotalMemory(true);
            }
        }

        #endregion Работа с Txt

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

        private const string MAN_UNIT_NAME = "Человек";
        private const string PART_UNIT_NAME = "Доля";
        private const string ROUBLE_UNIT_NAME = "Рубль";
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
            {
                switch (marksCode)
                {
                    case 2500:
                    case 2600:
                        if (this.DataSource.Year >= 2010)
                            return UNIT_UNIT_NAME;
                        else
                            return ROUBLE_UNIT_NAME;
                    case 1100:
                    case 1200:
                    case 1300:
                    case 1400:
                    case 2100:
                    case 2200:
                    case 2210:
                    case 2220:
                    case 2300:
                    case 2400:
                    case 3100:
                    case 3110:
                    case 3200:
                    case 3210:
                    case 3500:
                    case 3510:
                    case 3600:
                    case 3610:
                    case 3620:
                        return UNIT_UNIT_NAME;
                    case 1500:
                    case 1600:
                    case 1700:
                    case 1800:
                    case 1810:
                    case 1820:
                    case 2700:
                    case 2800:
                    case 2810:
                    case 2820:
                    case 2900:
                    case 3000:
                    case 3010:
                    case 3020:
                    case 3300:
                    case 3310:
                    case 3400:
                    case 3410:
                    case 3700:
                    case 3710:
                    case 3720:
                        return ROUBLE_UNIT_NAME;
                }
            }
            else
            {
                switch (marksCode)
                {
                    case 1500:
                    case 1510:
                    case 1600:
                        return MAN_UNIT_NAME;
                    case 300:
                    case 400:
                        return PART_UNIT_NAME;
                    case 500:
                    case 600:
                    case 700:
                    case 800:
                    case 810:
                    case 820:
                    case 1300:
                    case 1310:
                    case 1400:
                    case 1410:
                    case 1700:
                        return ROUBLE_UNIT_NAME;
                    case 100:
                    case 200:
                    case 1100:
                    case 1110:
                    case 1200:
                    case 1210:
                        return UNIT_UNIT_NAME;
                }
            }
            return string.Empty;
        }

        private int GetRefUnits(int marksCode)
        {
            if (this.DataSource.Year >= 2006)
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
            string d_Marks_FNS22_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2010)
                d_Marks_FNS22_HierarchyFileName = const_d_Marks_FNS22_HierarchyFile2010;
            else if (this.DataSource.Year >= 2007)
                d_Marks_FNS22_HierarchyFileName = const_d_Marks_FNS22_HierarchyFile2007;
            else
                d_Marks_FNS22_HierarchyFileName = const_d_Marks_FNS22_HierarchyFile2006;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS22_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy()
        {
            F1NMSumCorrectionConfig f1nmSumCorrectionConfig = new F1NMSumCorrectionConfig();
            f1nmSumCorrectionConfig.EarnedField = "Value";
            f1nmSumCorrectionConfig.EarnedReportField = "ValueReport";
            f1nmSumCorrectionConfig.InpaymentsField = string.Empty;
            f1nmSumCorrectionConfig.InpaymentsReportField = string.Empty;
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTypes" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTypes" }, "RefRegions", string.Empty, true);
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
