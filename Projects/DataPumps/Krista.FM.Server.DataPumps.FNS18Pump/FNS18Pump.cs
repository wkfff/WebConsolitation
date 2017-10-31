using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS18Pump
{
    // фнс 0018 - форма 5 пв
    public class FNS18PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 ПВ (d.Marks.FNS5PV)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        private int nullMarks;
        // Районы.ФНС (d.Regions.FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        private Dictionary<int, string> cacheRegionsNames = null;
        private int nullRegions;
        // ЕдИзмер.ОКЕИ (d.Units.OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 ПВ_Сводный (f.D.FNS5PVTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 ПВ_Районы (f.D.FNS5PVRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        private ExcelHelper excelHelper;
        private object excelObj = null;
        // итоговая сумма
        private decimal[] totalSums = new decimal[2];
        private int tableNumber;
        private int year;
        private int month;

        // коэффициент перевода тысяч в рубли
        private decimal sumMultiplier = 1;

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

            if (this.State == PumpProcessStates.PumpData)
                foreach (DataRow row in dsRegions.Tables[0].Rows)
                    row["Name"] = row["Name"].ToString().ToLower();
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CODE", "Name" }, "|", "ID");

            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5PV_GUID = "d47b7c3c-a532-4069-b059-a8af007b61b7";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5PV_TOTAL_GUID = "32ca0e6f-8160-4f5b-a3bd-3123a92b4ccf";
        private const string F_D_FNS_5PV_REGIONS_GUID = "9ad7737d-d040-46a0-a416-9009c143ef9e";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5PV_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5PV_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5PV_REGIONS_GUID] };
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
            for (int i = 0; i < totalSums.Length; i++)
                totalSums[i] = 0.0M;
        }

        private void CheckTotalSum(decimal controlSum, decimal totalSum, string column)
        {
            if (totalSum != controlSum)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F} по столбцу {2}", controlSum, totalSum, column));
        }

        private DataTable GetFactTable()
        {
            if (reportType == ReportType.Svod)
                return dsIncomesTotal.Tables[0];
            else
                return dsIncomesRegion.Tables[0];
        }

        private int GetReportDate()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            return (this.DataSource.Year * 10000 + 1);
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
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                if (this.DataSource.Year == 2007)
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
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);

            // если есть распаковываем архивы rar
            FileInfo[] rarFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo rarFile in rarFiles)
            {
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(rarFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    ProcessAllFiles(tempDir);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
            }
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

        #endregion Общие функции закачки

        #region Работа с Excel

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 1;
            while (emptyStrCount < 10)
            {
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim() == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
                curRow++;
            }
            return (curRow - 10);
        }

        private const string LITERAL_X = "XХ";
        private string CleanValue(string value)
        {
            return value.ToUpper().Trim(LITERAL_X.ToCharArray());
        }

        private void CheckXLSTotalSum(object sheet, int curRow)
        {
            string value = CleanValue(excelHelper.GetCell(sheet, curRow, 4).Value.Trim());
            if (value != string.Empty)
                CheckTotalSum(Convert.ToDecimal(value), totalSums[0], "Значение показателей");
            value = CleanValue(excelHelper.GetCell(sheet, curRow, 5).Value.Trim());
            if (value != string.Empty)
                CheckTotalSum(Convert.ToDecimal(value), totalSums[1], "Сумма акциза");
        }

        private int PumpRegion(object sheet, int codeRow)
        {
            string regionCode = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, codeRow, 1).Value.Trim());
            string regionName = string.Empty;
            if ((this.DataSource.Year >= 2008) && (this.Region == RegionName.Orenburg))
                regionName = regionCode;
            else
                regionName = excelHelper.GetCell(sheet, codeRow - 1, 1).Value.Trim();
            string regionNameLower = regionName.ToLower();
            if (cacheRegionsNames.ContainsValue(regionNameLower))
            {
                regionName = string.Format("{0} ({1})", regionName, regionCode);
                regionNameLower = string.Format("{0} ({1})", regionNameLower, regionCode);
            }
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regKey = string.Format("{0}|{1}", regionCode, regionNameLower);
            int refRegion = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, mapping, regKey, "ID");
            cacheRegionsNames.Add(refRegion, regionNameLower);
            return refRegion;
        }

        private int PumpMarks(object sheet, int curRow)
        {
            string name = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
            string value = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
            if (value == string.Empty)
                return -1;

            int code = Convert.ToInt32(value);
            object[] mapping = new object[] { "NAME", name, "CODE", code };

            value = CleanValue(excelHelper.GetCell(sheet, curRow, 3).Value.Trim());
            if ((value != string.Empty) && (tableNumber == 1))
                mapping = (object [])CommonRoutines.ConcatArrays(mapping, new object[] { "CODEPRODUCTION", Convert.ToInt32(value) });

            if (((this.DataSource.Year == 2007) || (this.DataSource.Year == 2009)) && (reportType == ReportType.Region))
                return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
            return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
        }

        private void PumpFactRow(object[] mapping, int refRegions)
        {
            if (reportType != ReportType.Svod)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegions", refRegions });
            PumpRow(GetFactTable(), mapping);
            if ((dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT) || (dsIncomesTotal.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT))
            {
                UpdateData();
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                else ClearDataSet(daIncomesTotal, ref dsIncomesRegion);
            }
        }

        // установить коэффициент sumMultiplier
        private void SetSumMultiplier(int marksCode)
        {
            if (marksCode == 200)
                sumMultiplier = 1;
            else
                sumMultiplier = 1000;
        }

        private void PumpXLSRow(object sheet, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpMarks(sheet, curRow);
            if (refMarks == -1)
                return;

            string v = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
            int code = Convert.ToInt32(v);

            if (tableNumber == 1)
            {
                string value = CleanValue(excelHelper.GetCell(sheet, curRow, 4).Value.Trim()).PadLeft(1, '0');
                decimal valueReport = Convert.ToDecimal(value);
                value = CleanValue(excelHelper.GetCell(sheet, curRow, 5).Value.Trim()).PadLeft(1, '0');
                decimal sumReport = Convert.ToDecimal(value);
                if ((valueReport == 0) && (sumReport == 0))
                    return;
                totalSums[0] += valueReport;
                totalSums[1] += sumReport;
                object[] mapping = new object[] { "RefMarks", refMarks, "RefYearDayUNV", refDate,
                    "ValueReport", valueReport , "SumReport", (code==200)?sumReport: sumReport * 1000 };
                PumpFactRow(mapping, refRegions);
            }
            else
            {
                string value = CleanValue(excelHelper.GetCell(sheet, curRow, 3).Value.Trim()).PadLeft(1, '0');
                decimal valueReport = Convert.ToDecimal(value);
                if (valueReport == 0)
                    return;
                object[] mapping = new object[] { "RefMarks", refMarks,
                    "RefYearDayUNV", refDate, "ValueReport", valueReport };
                PumpFactRow(mapping, refRegions);
            }
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        public const string REGION_START_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private bool IsRegionStart(object sheet, int curRow)
        {
            if (/*(this.DataSource.Year >= 2008) || */(reportType != ReportType.Region))
                return false;
            return (excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper() == REGION_START_ROW);
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            if ((reportType == ReportType.Region))
                return (cellValue.Trim().ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        private void PumpXLSSheetData(FileInfo file, object sheet, int refDate)
        {
            int refRegions = nullRegions;
            if (((this.DataSource.Year >= 2008) && (reportType == ReportType.Region) && (this.Region == RegionName.Orenburg)))
                refRegions = PumpRegion(sheet, 19);
            int rowsCount = GetRowsCount(sheet);
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);

            tableNumber = 0;
            bool toPumpRow = false;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow, string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsSectionEnd(cellValue))
                    {
                        CheckXLSTotalSum(sheet, curRow);
                        toPumpRow = false;
                        continue;
                    }

                    if (IsRegionStart(sheet, curRow))
                    {
                        refRegions = PumpRegion(sheet, curRow + 2);
                        continue;
                    }

                    if (toPumpRow)
                    {
                        PumpXLSRow(sheet, curRow, refDate, refRegions);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        // в отчетах райнов иногда сверху добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(excelHelper.GetCell(sheet, curRow + 1, 1).Value))
                            continue;
                        tableNumber++;
                        SetNullTotalSum();
                        toPumpRow = true;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXLSFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);

            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                cacheRegionsNames = new Dictionary<int, string>();
                int refDate = GetReportDate();
                object sheet = excelHelper.GetSheet(workbook, 1);
                PumpXLSSheetData(file, sheet, refDate);
            }
            finally
            {
                cacheRegionsNames.Clear();
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                CheckDirectories(dir);
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
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
        private const string MAN_UNIT_NAME = "Человек";
        private int GetRefUnits(int marksCode)
        {
            if (marksCode == 200)
                return FindCachedRow(cacheUnits, MAN_UNIT_NAME, nullUnits);
            return FindCachedRow(cacheUnits, ROUBLE_UNIT_NAME, nullUnits);
        }

        protected void SetRefUnits()
        {
            if (cacheUnits.Count <= 1)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, "Классификатор «ЕдИзмер.ОКЕИ» не заполнен.");
                return;
            }
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
                row["RefUnits"] = refUnits;
            }
        }

        private void SetClsHierarchy()
        {
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", const_d_Marks_FNS18_HierarchyFile2008, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy(string field, string fieldReport)
        {
            CommonSumCorrectionConfig commonCorrectionConfig = new CommonSumCorrectionConfig();
            commonCorrectionConfig.Sum1 = field;
            commonCorrectionConfig.Sum1Report = fieldReport;
            GroupTable(fctIncomesTotal, new string[] { "RefMarks", "RefYearDayUNV" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
            GroupTable(fctIncomesRegion, new string[] { "RefMarks", "RefYearDayUNV", "RefRegions" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByHierarchy("Value", "ValueReport");
            CorrectSumByHierarchy("SumExcise", "SumReport");
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
