using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS9Pump
{

    // ФНС - 0009 - Форма 5-ЕНВД
    public class FNS9PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 ЕНВД (d_Marks_FNS5ENVD)
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
        // ЕдИзмер.ОКЕИ (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 ЕНВД_Сводный (f_D_FNS5ENVDTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 ЕНВД_Районы (f_D_FNS5ENVDRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        // параметры обработки
        private int year = -1;
        private int month = -1;
        // итоговые суммы
        private decimal[] totalSums;
        // номер строки с кодами регионов (для ставрополя)
        private int regionsRow;
        // коэффициент перевода тысяч в рубли
        private decimal sumMultiplier = 1;
        private string regionName = string.Empty;
        private string regionCode = string.Empty;

        private bool noSvodReports = false;
        private bool isStavropolRegion2008 = false;
        private bool isVologaRegion2008 = false;
        private bool isTyvaRegion2008 = false;

        private bool hasTitleSheet = false;

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
        // код вид.лица "Индивидуальные предприниматели"
        private const int TYPE_INDIVID_ID = 4;

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

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

        private const string D_MARKS_FNS_5ENVD_GUID = "d1d92cd6-e6e9-48d7-b7e3-52d4cf5b82e1";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5ENVD_TOTAL_GUID = "91491ed3-3f20-4aae-87ce-a30dbd4adf29";
        private const string F_D_FNS_5ENVD_REGIONS_GUID = "0abdd984-5d0a-4098-9f9f-45cec7f4d3a6";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5ENVD_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID]};
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5ENVD_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5ENVD_REGIONS_GUID] };
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

        private int PumpRegion(string regionCode, string regionName)
        {
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
                object[] mapping = new object[] { "Value", factValue, "RefTypes", refTypes, "RefYearDayUNV", refDate, "RefMarks", refMarks };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "Value", factValue, "RefTypes", refTypes, "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions };
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
        Regex regExThousandRouble = new Regex(@"\(тыс\.(.*)руб\.\)", RegexOptions.IgnoreCase);
        private string DeleteThousandRouble(string value)
        {
            return regExThousandRouble.Replace(value, String.Empty).Trim();
        }

        private void SetSumMultiplier(string marksCode)
        {
            if ((marksCode == "010") || (marksCode == "020") || (marksCode == "030"))
                sumMultiplier = 1000;
            else
                sumMultiplier = 1;
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return (this.DataSource.Year * 10000 + this.DataSource.Month * 100);
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 ЕНВД' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isStavropolRegion2008 =
                (this.Region == RegionName.Stavropol) && (reportType == ReportType.Region) && (this.DataSource.Year >= 2008);
            isVologaRegion2008 =
                (this.Region == RegionName.Vologda) && (reportType == ReportType.Region) && (this.DataSource.Year == 2008);
            isTyvaRegion2008 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year <= 2008);
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
            noSvodReports =
                ((this.Region == RegionName.Kostroma) && (this.DataSource.Year >= 2008)) ||
                ((this.Region == RegionName.Samara) && (this.DataSource.Year == 2007)) ||
                ((this.Region == RegionName.Tula) && (this.DataSource.Year >= 2009)) ||
                (this.Region == RegionName.Tyva);
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                // Каталог "Сводный" должен присутствовать (кроме Костромы-2008 и Самары-2007)
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
                if (reportType == ReportType.Region)
                    comment = string.Format("для района '{0}' (код: {1})", regionName, regionCode);
                if (this.DataSource.Year >= 2007)
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

        // Регулярное выражение для выборки наименования и кода региона из строки,
        // представленной в формате "Налоговый орган ХХХХХХХХХХХ Наименование"
        Regex regExRegionsData = new Regex(@"[0-9]{11}", RegexOptions.IgnoreCase);
        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow, string fileName)
        {
            if (this.Region == RegionName.Altay)
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 1).Trim());
                regionName = excelDoc.GetValue(curRow - 2, 1).Trim();
            }
            else
            {
                string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                if (cellValue.ToUpper().StartsWith(MARK_TAX_DEP))
                {
                    regionCode = regExRegionsData.Match(cellValue).Value;
                    if ((this.Region == RegionName.Samara) && (this.DataSource.Year >= 2008))
                        regionName = fileName.Substring(0, fileName.LastIndexOf('.'));
                    else
                        regionName = regExRegionsData.Split(cellValue)[1].Trim();
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
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    regionName = excelDoc.GetValue(curRow, 5).Trim();
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            int code = nullMarks;
            string name = DeleteThousandRouble(excelDoc.GetValue(curRow, 1).Trim());
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if (value == string.Empty)
                return -1;
            code = Convert.ToInt32(value);
            SetSumMultiplier(value.PadLeft(3, '0'));
            object[] mapping = new object[] { "NAME", name, "CODE", code };
            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == -1)
                return;

            if (this.DataSource.Year >= 2007)
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                PumpFactRow(factValue, refDate, refMarks, refRegions, TYPE_URID_ID, 1);
                factValue = CleanFactValue(excelDoc.GetValue(curRow, 5));
                if ((this.Region == RegionName.Orenburg) || (this.Region == RegionName.Altay))
                    PumpFactRow(factValue, refDate, refMarks, refRegions, TYPE_INDIVID_ID, 2);
                else
                    PumpFactRow(factValue, refDate, refMarks, refRegions, TYPE_PHYS_ID, 2);
            }
            else
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                PumpFactRow(factValue, refDate, refMarks, refRegions, TYPE_ALL_ID, 0);
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

        private void PumpXlsRowStavropol(ExcelHelper excelDoc, int curRow, int refDate, int refTypes)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
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
                PumpFactRow(valueReport, refDate, refMarks, refRegions, refTypes, curCol - 3);
            }
        }

        #endregion Ставрополь

        #region Отчеты в разрезе строк

        private int PumpXlsRegionsStr(ExcelHelper excelDoc, int curRow)
        {
            regionName = excelDoc.GetValue(curRow, 1).Trim();
            regionCode = excelDoc.GetValue(curRow, 2).Trim();
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsMarksStr(ExcelHelper excelDoc, int curRow)
        {
            string value = excelDoc.GetValue(curRow, 1).Trim();
            string name = value.Substring(6);
            int code = Convert.ToInt32(value.Substring(0, 3));
            SetSumMultiplier(value.Substring(0, 3).PadLeft(3, '0'));
            object[] mapping = new object[] { "NAME", name, "CODE", code };
            if (noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpXlsRowStr(ExcelHelper excelDoc, int curRow, int refDate, int refMarks)
        {
            int refRegion = PumpXlsRegionsStr(excelDoc, curRow);

            decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
            PumpFactRow(factValue, refDate, refMarks, refRegion, TYPE_URID_ID, 1);
            factValue = CleanFactValue(excelDoc.GetValue(curRow, 5));
            PumpFactRow(factValue, refDate, refMarks, refRegion, TYPE_INDIVID_ID, 2);
        }

        // закачка отчётов в разрезе строк
        private const string STR_SECTION_MARK = "РАЗРЕЗ ПО СТРОКЕ";
        private const string STR_SECTION_END = "ВСЕГО";
        private void PumpXlsSheetDataStr(string fileName, ExcelHelper excelDoc, int refDate)
        {
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            int refMarks = nullMarks;

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

                    if (cellValue.ToUpper() == STR_SECTION_MARK)
                    {
                        refMarks = PumpXlsMarksStr(excelDoc, curRow + 1);
                        continue;
                    }

                    if (cellValue.ToUpper() == STR_SECTION_END)
                    {
                        CheckXlsTotalSum(excelDoc, curRow, -1);
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        PumpXlsRowStr(excelDoc, curRow, refDate, refMarks);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        totalSums = new decimal[3];
                        SetNullTotalSum();
                        toPumpRow = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
        }

        #endregion Отчеты в разрезе строк

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
            cellValue = cellValue.ToUpper();
            if ((this.Region == RegionName.Altay) ||
                ((this.Region == RegionName.Samara) && (this.DataSource.Year >= 2007)))
                    return cellValue.StartsWith(MARK_TAX_DEP);
            return cellValue.StartsWith(MARK_REGION);
        }

        private const string CUT_ROW = "РАЗРЕЗ ПО ГРАФЕ";
        private bool IsCutRow(string cellValue)
        {
            if (isStavropolRegion2008)
                return cellValue.ToUpper().StartsWith(CUT_ROW);
            return false;
        }

        private const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private const string REPORT_START_MARK = "ОТЧЕТНОСТЬ ФЕДЕРАЛЬНОЙ НАЛОГОВОЙ СЛУЖБЫ";
        private int GetFirstRow(ExcelHelper excelDoc)
        {
            if (isVologaRegion2008)
            {
                // для вологды при закачке районов начинаем качать со второго отчета
                // (в файле два отчета - первый фиктивный какой-то)
                for (int curRow = 10; ; curRow++)
                    if (excelDoc.GetValue(curRow, 1).Trim().ToUpper() == REPORT_START_MARK)
                        return curRow;
            }
            return 1;
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            if ((this.DataSource.Year >= 2008) && (reportType == ReportType.Region))
                return (cellValue.Trim().ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        // закачка сводных отчётов и отчётов в разрезе районов
        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            int refTypes = -1;
            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            
            int firstRow = GetFirstRow(excelDoc);
            int rowsCount = excelDoc.GetRowsCount();
            int indexStartSection = 0;

            for (int curRow = firstRow; curRow <= rowsCount; curRow++)
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

                    if (IsCutRow(cellValue))
                    {
                        string value = excelDoc.GetValue(curRow + 1, 1).Trim();
                        refTypes = Convert.ToInt32(value.Split('-')[0].Trim());
                        continue;
                    }

                    if (IsSectionEnd(cellValue))
                    {
                        if (refTypes != 1)
                            CheckXlsTotalSum(excelDoc, curRow, refTypes);
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        if (isStavropolRegion2008)
                            PumpXlsRowStavropol(excelDoc, curRow, refDate, refTypes);
                        else
                            PumpXlsRow(excelDoc, curRow, refDate, refRegions);
                    }

                    if (IsSectionStart(cellValue))
                    {
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1)))
                            continue;

                        //в сводных отчетах если нет титульного, то пропускаем первую секцию А
                        if (!hasTitleSheet && (indexStartSection < 1))
                        {
                            indexStartSection++;
                            continue;
                        }

                        int columnsCount = 3;
                        if (isStavropolRegion2008)
                        {
                            regionsRow = curRow;
                            columnsCount = PumpXlsRegionsStavropol(excelDoc, curRow);
                        }
                        totalSums = new decimal[columnsCount];
                        SetNullTotalSum();
                        // не закачиваем в разрезе по графе "1 - всего"
                        toPumpRow = refTypes != 1;
                    }
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
                hasTitleSheet = false;

                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    string worksheetName = excelDoc.GetWorksheetName().Trim().ToUpper(); ;

                    if (IsTitleSheet(worksheetName))//атк же проверяется на регион тува
                        refRegions = PumpXlsRegionsTyva(excelDoc);
                    else if (reportType == ReportType.Str)
                        PumpXlsSheetDataStr(file.Name, excelDoc, refDate);
                    else
                    {
                        if ((this.Region == RegionName.SamaraGO) && worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ") && (this.DataSource.Year >= 2008) && (this.reportType == ReportType.Svod))
                        {
                            hasTitleSheet = true;
                        }
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

        #region Работа с Txt

        private void CheckTxtTotalSum(string[] rowReport)
        {
            decimal controlSum = CleanFactValue(rowReport[3].Trim());
            CheckTotalSum(totalSums[0], controlSum, "по столбцу 1");
        }

        private int PumpTxtMarks(string[] rowReport, string marksName)
        {
            string value = rowReport[2].Trim();
            if (value == string.Empty)
                return -1;

            int code = Convert.ToInt32(value);
            SetSumMultiplier(value.PadLeft(3, '0'));
            marksName = DeleteThousandRouble(marksName);
            
            object[] mapping = new object[] { "NAME", marksName, "CODE", code };

            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpTxtRow(string[] rowReport, string marksName, int refDate)
        {
            int refMarks = PumpTxtMarks(rowReport, marksName);
            if (refMarks == -1)
                return;

            decimal factValue = CleanFactValue(rowReport[3].Trim());
            PumpFactRow(factValue, refDate, refMarks, nullRegions, TYPE_ALL_ID, 0);
        }

        // код разделителя "|"
        private const int DELIMETER_CODE = 9474;
        private char DELIMETER = Convert.ToChar(DELIMETER_CODE);
        private void PumpTxtReport(FileInfo file, int refDate)
        {
            totalSums = new decimal[1];
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

                    string strValue = txtReport[curRow].Trim();
                    if ((strValue == string.Empty) || (strValue[0] != DELIMETER))
                        continue;

                    string[] rowsReport = strValue.Split(DELIMETER);

                    if (toPumpRow)
                    {
                        if (rowsReport[1].ToUpper().Contains(TOTAL_ROW))
                        {
                            CheckTxtTotalSum(rowsReport);
                            toPumpRow = false;
                        }
                        else
                        {
                            marksName = String.Format("{0} {1}", marksName.Trim(), rowsReport[1].Trim());
                            if (rowsReport[2].Trim() != string.Empty)
                            {
                                PumpTxtRow(rowsReport, marksName, refDate);
                                marksName = string.Empty;
                            }
                        }
                    }

                    if (rowsReport[1].Trim().ToUpper() == "А")
                    {
                        SetNullTotalSum();
                        toPumpRow = true;
                    }
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
                int refDate = GetReportDate();
                PumpTxtReport(file, refDate);
            }
            finally
            {
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

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            switch (marksCode)
            {
                case 10:
                case 20:
                case 30:
                    return ROUBLE_UNIT_NAME;
                case 40:
                    return UNIT_UNIT_NAME;
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
            ProcessDataSourcesTemplate(year, month, "Выполняется установка ссылок с классификатора «Показатели.ФНС 5 УСН» на классификатор «ЕдИзмер.ОКЕИ»");
        }

        #endregion Обработка данных

    }
}
