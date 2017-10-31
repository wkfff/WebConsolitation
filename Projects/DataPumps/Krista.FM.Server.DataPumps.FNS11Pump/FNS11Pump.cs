using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS11Pump
{

    // ФНС - 0011 - Форма 5-ЕСХН
    public class FNS11PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 ЕСХН (d_Marks_FNS5ESXN)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<int, int> cacheMarks = null;
        private int nullMarks;
        // районы.фнс (d_Regions_FNS)
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

        // Доходы.ФНС_5 ЕСХН_Сводный (f_D_FNS5ESXNTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 ЕСХН_Районы (f_D_FNS5ESXNRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        private decimal sumMultiplier = 1;
        // номер строки с кодами регионов (для ставрополя)
        private int regionsRow;
        // итоговая сумма
        private decimal[] totalSums;

        private bool noSvodReports = false;
        private string regionName = string.Empty;
        private string regionCode = string.Empty;
        private bool isStavropolRegion2008 = false;
        private bool isTyvaRegion2008 = false;
        private bool isAltayKrai2008 = false;
        private bool isAltayKraiRegion2009 = false;

        private bool hasTitleSheet = false;

        #endregion Поля

        #region Структуры, перечисления

        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

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

        private const string D_MARKS_FNS_5ESXN_GUID = "85a2e17f-57a3-439d-9462-8e0de68539c8";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string F_D_FNS_5ESXN_TOTAL_GUID = "301f1167-8b8f-4b0b-830f-d5051d6627b0";
        private const string F_D_FNS_5ESXN_REGIONS_GUID = "bb925366-816b-415c-b82a-6327dc66bb04";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5ESXN_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5ESXN_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5ESXN_REGIONS_GUID] };
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

        private int GetRefMarksByCode(int code)
        {
            if (cacheMarks.ContainsKey(code))
                return cacheMarks[code];
            return nullMarks;
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return (this.DataSource.Year * 10000 + this.DataSource.Month * 100);
        }

        private void SetSumMultiplier(int marksCode)
        {
            if (this.DataSource.Year >= 2010)
            {
                if (marksCode <= 50)
                    sumMultiplier = 1000;
                else
                    sumMultiplier = 1;
            }
            else
            {
                if (marksCode <= 40)
                    sumMultiplier = 1000;
                else
                    sumMultiplier = 1;
            }
        }

        private void SetFlags()
        {
            isStavropolRegion2008 =
                (this.Region == RegionName.Stavropol) && (reportType == ReportType.Region) && (this.DataSource.Year >= 2008);
            isTyvaRegion2008 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year <= 2008);
            isAltayKrai2008 =
                (this.Region == RegionName.AltayKrai) && (this.DataSource.Year <= 2008);
            isAltayKraiRegion2009 =
                (this.Region == RegionName.AltayKrai) && (reportType == ReportType.Region) && (this.DataSource.Year == 2009);
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 ЕСХН' - закачайте сводные отчеты");
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
            if (isStavropolRegion2008 || isAltayKraiRegion2009)
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

        private void PumpFactRow(decimal factValue, int refDate, int refMarks, int refRegions, int refTypes, int sumIndex, int typeData)
        {
            if (((factValue == 0) && (this.Region != RegionName.MoskvaObl)) || ((this.Region == RegionName.MoskvaObl) && (typeData == 0) && (factValue == 0)))
                return;

            totalSums[sumIndex] += factValue;
            factValue *= sumMultiplier;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] { "Value", (typeData==0)?factValue:0, "RefTypes", refTypes, "RefYearDayUNV", refDate, "RefMarks", refMarks, "TypeData", typeData};
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "Value", (typeData == 0) ? factValue : 0, "RefTypes", refTypes, "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions, "TypeData", typeData };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private int PumpXlsRegionsTyva(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow < rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                if (cellValue == REGION_ROW)
                {
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    regionName = excelDoc.GetValue(curRow, 5).Trim();
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow, string fileName)
        {
            if (this.Region == RegionName.Altay)
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 1).Trim());
                regionName = excelDoc.GetValue(curRow - 2, 1).Trim();
            }
            else if ((this.Region == RegionName.Samara) &&
                (this.DataSource.Year >= 2009))
            {
                regionName = fileName.Substring(0, fileName.Length - 4);
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 1));
            }
            else
            {
                regionName = excelDoc.GetValue(curRow + 1, 1);
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1));
            }
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            string name = DeleteThousandRouble(excelDoc.GetValue(curRow, 1).Trim());
            string codeStr = excelDoc.GetValue(curRow, 2).Trim();
            if (codeStr == string.Empty)
                return -1;
            int code = Convert.ToInt32(codeStr);
            SetSumMultiplier(code);

            object[] mapping = new object[] { "NAME", name, "CODE", code };
            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, code, "CODE", mapping);
            return GetRefMarksByCode(code);
        }

        int GetInformationType(string value)
        {
            if ((value.ToUpper().Contains("X") || value.ToUpper().Contains("Х")) && (this.Region == RegionName.MoskvaObl))
                return 1;
            else return 0;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == -1)
                return;

            if (this.DataSource.Year >= 2007)
            {
                string value = excelDoc.GetValue(curRow, 4);
                decimal factValue = CleanFactValue(value);
                
                PumpFactRow(factValue, refDate, refMarks, refRegions, 2, 1, GetInformationType(value));
                value = excelDoc.GetValue(curRow, 5);
                factValue = CleanFactValue(value);
                PumpFactRow(factValue, refDate, refMarks, refRegions, 3, 2, GetInformationType(value));
            }
            else
            {
                string value = excelDoc.GetValue(curRow, 3);
                decimal factValue = CleanFactValue(value);
                PumpFactRow(factValue, refDate, refMarks, refRegions, 1, 0, GetInformationType(value));
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

        private void PumpXlsRowAltayKrai(ExcelHelper excelDoc, int curRow, int refDate)
        {
            regionCode = excelDoc.GetValue(curRow, 2).Trim();
            regionName = excelDoc.GetValue(curRow, 4).Trim();
            int refRegions = PumpRegion(regionCode, regionName);

            for (int marksCode = 10; marksCode <= 50; marksCode += 10)
            {
                int refMarks = GetRefMarksByCode(marksCode);
                SetSumMultiplier(marksCode);

                int marksColumn = GetFactColumn(marksCode);
                string value = excelDoc.GetValue(excelDoc.GetValue(curRow, marksColumn));
                decimal factValue = CleanFactValue(value);

                PumpFactRow(factValue, refDate, refMarks, refRegions, 2, 1, GetInformationType(value));
                value = excelDoc.GetValue(curRow, marksColumn + 1);
                factValue = CleanFactValue(value);
                PumpFactRow(factValue, refDate, refMarks, refRegions, 3, 2, GetInformationType(value));
            }
        }

        private void PumpXlsSheetDataAltayKrai(string fileName, ExcelHelper excelDoc, int refDate)
        {
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();
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
                        PumpXlsRowAltayKrai(excelDoc, curRow, refDate);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
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

        #endregion Алтайский край

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

                string value = excelDoc.GetValue(excelDoc.GetValue(curRow, curCol));
                decimal valueReport = CleanFactValue(value);

                PumpFactRow(valueReport, refDate, refMarks, refRegions, refTypes, curCol - 3, GetInformationType(value));
            }
        }

        #endregion Ставрополь

        #region Отчеты в разрезе строк

        private int PumpXlsRegionStr(ExcelHelper excelDoc, int curRow)
        {
            regionName = excelDoc.GetValue(curRow, 1).Trim();
            regionCode = excelDoc.GetValue(curRow, 2).Trim();
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsMarksStr(ExcelHelper excelDoc, int curRow)
        {
            int code = -1;
            string codeStr = excelDoc.GetValue(curRow, 1).Trim();
            if (codeStr != string.Empty)
                code = Convert.ToInt32(codeStr.Substring(0, 3));
            SetSumMultiplier(code);
            return GetRefMarksByCode(code);
        }

        private void PumpXlsRowStr(ExcelHelper excelDoc, int curRow, int refDate, int refMarks)
        {
            int refRegions = PumpXlsRegionStr(excelDoc, curRow);

            string value = excelDoc.GetValue(excelDoc.GetValue(curRow, 4));
            decimal factValue = CleanFactValue(value);

            PumpFactRow(factValue, refDate, refMarks, refRegions, 2, 1, GetInformationType(value));
            
            value = excelDoc.GetValue(excelDoc.GetValue(curRow, 5));
            factValue = CleanFactValue(value);
            PumpFactRow(factValue, refDate, refMarks, refRegions, 3, 2, GetInformationType(value));
        }

        // закачка данных их xls-отчёта по строкам
        private const string CUT_ROW_STR = "РАЗРЕЗ ПО СТРОКЕ";
        private const string TOTAL_ROW_STR = "ВСЕГО";
        private void PumpXlsSheetDataStr(string fileName, ExcelHelper excelDoc, int refDate)
        {
            int refMarks = -1;
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
                        if (this.Region != RegionName.AltayKrai)
                            continue;
                        // в отчетах алтайского края встречаются строки, на которых нет имени района (1-й столбец),
                        // поэтому такие строки пропускаются, что некорректно, т.к. не закачиваются все нужные данные
                        // в связи с этим добавляем ещё одну проверку на пустую строку, но уже во 2-м столбце
                        if (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
                        {
                            toPumpRow = false;
                            continue;
                        }
                    }

                    if (cellValue.ToUpper().StartsWith(CUT_ROW_STR))
                    {
                        refMarks = PumpXlsMarksStr(excelDoc, curRow + 1);
                        continue;
                    }

                    if (cellValue.ToUpper().Contains(TOTAL_ROW_STR))
                    {
                        CheckXlsTotalSum(excelDoc, curRow, -1);
                        toPumpRow = false;
                        refMarks = -1;
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
                        if (refMarks != nullMarks)
                            toPumpRow = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        #endregion Отчеты в разрезе строк

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        private const string MARK_REGION = "КОД ОКАТО";
        private const string REGION_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private const string MARK_TAX_DEP = "НАЛОГОВЫЙ ОРГАН";
        private bool IsRegionRow(ExcelHelper excelDoc, int curRow)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion2008 || isStavropolRegion2008 || isAltayKraiRegion2009)
                return false;
            string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            if (this.Region == RegionName.Altay)
                return cellValue.StartsWith(MARK_TAX_DEP);
            if ((this.Region == RegionName.Samara) && (this.DataSource.Year >= 2009))
                return cellValue.StartsWith(MARK_TAX_DEP);
            string cellValue2 = excelDoc.GetValue(curRow + 2, 1).Trim().ToUpper();
            return cellValue.StartsWith(REGION_ROW) && cellValue2.StartsWith(MARK_REGION);
        }

        private const string CUT_ROW = "РАЗРЕЗ ПО ГРАФЕ";
        private bool IsCutRow(string cellValue)
        {
            if (isStavropolRegion2008 || isAltayKraiRegion2009)
                return cellValue.ToUpper().StartsWith(CUT_ROW);
            return false;
        }

        private const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            if ((reportType == ReportType.Region) && (this.DataSource.Year >= 2008))
                return (cellValue.Trim().ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            int refTypes = -1;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();
            int indexStartSection = 0;

            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsRegionRow(excelDoc, curRow))
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
                        if (isStavropolRegion2008 || isAltayKraiRegion2009)
                            PumpXlsRowStavropol(excelDoc, curRow, refDate, refTypes);
                        else
                            PumpXlsRow(excelDoc, curRow, refDate, refRegions);
                    }

                    if (IsSectionStart(cellValue))
                    {
                        // в отчетах райнов иногда сверху добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1)))
                            continue;

                        //для самары го в сводных отчетах если нет титульного, то пропускаем первую секцию А
                        if ((this.Region == RegionName.SamaraGO) &&(!hasTitleSheet) && (indexStartSection < 1))
                        {
                            indexStartSection++;
                            continue;
                        }

                        int columnsCount = 3;
                        if (isStavropolRegion2008 || isAltayKraiRegion2009)
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
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int wsCount = excelDoc.GetWorksheetsCount();
                int refRegions = nullRegions;

                hasTitleSheet = false;

                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    string worksheetName = excelDoc.GetWorksheetName().Trim().ToUpper();

                    if (isTyvaRegion2008)
                    {
                        if (worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ"))
                            refRegions = PumpXlsRegionsTyva(excelDoc);
                    }
                    else if (isAltayKrai2008)
                    {
                        if ((worksheetName == "5ESHN") && (reportType == ReportType.Svod))
                            PumpXlsSheetData(file.Name, excelDoc, refDate, nullRegions);
                        if ((worksheetName == "МО") && (reportType == ReportType.Region))
                            PumpXlsSheetDataAltayKrai(file.Name, excelDoc, refDate);
                    }
                    else if (reportType == ReportType.Str)
                    {
                        PumpXlsSheetDataStr(file.Name, excelDoc, refDate);
                    }
                    else
                    {
                        if ((this.Region == RegionName.SamaraGO) && worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ") && (this.DataSource.Year >= 2009) && (this.reportType == ReportType.Svod))
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
            if (this.DataSource.Year >= 2010)
            {
                if (marksCode <= 50)
                    return ROUBLE_UNIT_NAME;
                return UNIT_UNIT_NAME;
            }
            else
            {
                if (marksCode <= 40)
                    return ROUBLE_UNIT_NAME;
                return UNIT_UNIT_NAME;
            }
        }

        private int GetRefUnits(int marksCode)
        {
            if (this.DataSource.Year <= 2006)
                return FindCachedRow(cacheUnits, ROUBLE_UNIT_NAME, nullUnits);
            else
            {
                string unitName = GetUnitName(marksCode);
                return FindCachedRow(cacheUnits, unitName, nullUnits);
            }
        }

        protected override void ProcessDataSource()
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
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Установка ссылок с классификатора «Показатели.ФНС 5 ЕСХН» на классификатор «ЕдИзмер.ОКЕИ»");
        }

        #endregion Обработка данных

    }
}