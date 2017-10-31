using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS27Pump
{

    // ФНС - 0027 - Форма 5-АЛ
    public class FNS27PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 АЛ (d_Marks_FNS5AL)
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

        // Доходы.ФНС_5 АЛ_Сводный (f_D_FNS5ALTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 АЛ_Районы (f_D_FNS5ALRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        private int sectionIndex = -1;
        // итоговая сумма
        private decimal[] totalSums = new decimal[2];
        // коэффициент перевода тысяч в рубли
        private decimal sumMultiplier = 1;
        private int year;
        private int month;

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

        private const string D_MARKS_FNS_5AL_GUID = "ee1152ec-6dc8-441a-9fa9-e00d92e0a1a6";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5AL_TOTAL_GUID = "93e6e349-7d3c-4172-91f4-4a53a90aaccf";
        private const string F_D_FNS_5AL_REGIONS_GUID = "b2f6ad33-d4f1-4408-85c6-5184fb5087be";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5AL_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5AL_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5AL_REGIONS_GUID] };
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

        private int GetSectionIndex(string cellValue)
        {
            if (cellValue.ToUpper().StartsWith("СПРАВОЧНО"))
                return 1;
            return 0;
        }

        private void SetSumMultiplier(int marksCode)
        {
            if (marksCode == 350)
                sumMultiplier = 1;
            else
                sumMultiplier = 1000;
        }

        private int GetReportDate()
        {
            return (this.DataSource.Year * 10000 + 1);
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0))
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 АЛ' - закачайте сводные отчеты");
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
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
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
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
            if (sectionIndex > 0)
                return;
            decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, 4).Trim());
            CheckTotalSum(totalSums[0], controlSum, "по столбцу 'Значение показателей'");
            controlSum = CleanFactValue(excelDoc.GetValue(curRow, 5).Trim());
            CheckTotalSum(totalSums[1], controlSum, "по столбцу 'Сумма акциза'");
        }

        private void PumpFactRow(decimal factValue, decimal sumValue, int refDate, int refMarks, int refRegions)
        {
            if ((factValue == 0) && (sumValue == 0))
                return;

            totalSums[0] += factValue;
            totalSums[1] += sumValue;

            factValue *= sumMultiplier;
            sumValue *= sumMultiplier;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] {
                    "ValueReport", factValue, "SummReport", sumValue,
                    "RefFNS5AL", refMarks, "RefYearDayUNV", refDate };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] {
                    "ValueReport", factValue, "SummReport", sumValue,
                    "RefFNS5AL", refMarks, "RefYearDayUNV", refDate, "RefRegions", refRegions };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
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

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            string regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
            string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 1).Trim();
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if (value == string.Empty)
                return -1;
            
            int code = Convert.ToInt32(value);
            SetSumMultiplier(code);

            object[] mapping = new object[] { "NAME", name, "CODE", code };
            if (sectionIndex == 0)
            {
                string kindCode = excelDoc.GetValue(curRow, 3).Trim();
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "KINDCODE", kindCode });
            }

            if (reportType == ReportType.Svod)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == -1)
                return;

            if (sectionIndex == 0)
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                decimal sumValue = CleanFactValue(excelDoc.GetValue(curRow, 5));
                PumpFactRow(factValue, sumValue, refDate, refMarks, refRegions);
            }
            else
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                PumpFactRow(factValue, 0, refDate, refMarks, refRegions);
            }
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        public const string REGION_START_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private bool IsRegionStart(string cellValue)
        {
            if (reportType != ReportType.Region)
                return false;
            return (cellValue.ToUpper() == REGION_START_ROW);
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private void PumpXlsSheetData(FileInfo file, ExcelHelper excelDoc, int refDate)
        {
            sectionIndex = -1;
            int refRegions = nullRegions;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

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

                    if (IsRegionStart(cellValue))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        sectionIndex = GetSectionIndex(excelDoc.GetValue(curRow - 2, 1).Trim());
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
                excelDoc.SetWorksheet(1);
                PumpXlsSheetData(file, excelDoc, refDate);
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

        #endregion  Закачка данных

        #region Обработка данных

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            if (marksCode == 350)
                return UNIT_UNIT_NAME;
            return ROUBLE_UNIT_NAME;
        }

        private int GetRefUnits(int marksCode)
        {
            string unitName = GetUnitName(marksCode);
            return FindCachedRow(cacheUnits, unitName, nullUnits);
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
            string d_Marks_FNS27_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2009)
                d_Marks_FNS27_HierarchyFileName = const_d_Marks_FNS27_HierarchyFile2009;
            else if (this.DataSource.Year >= 2008)
                d_Marks_FNS27_HierarchyFileName = const_d_Marks_FNS27_HierarchyFile2008;
            else
                d_Marks_FNS27_HierarchyFileName = const_d_Marks_FNS27_HierarchyFile2007;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS27_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy(string field, string fieldReport)
        {
            CommonSumCorrectionConfig commonCorrectionConfig = new CommonSumCorrectionConfig();
            commonCorrectionConfig.Sum1 = field;
            commonCorrectionConfig.Sum1Report = fieldReport;
            GroupTable(fctIncomesTotal, new string[] { "RefFNS5AL", "RefYearDayUNV" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefFNS5AL",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
            GroupTable(fctIncomesRegion, new string[] { "RefFNS5AL", "RefYearDayUNV", "RefRegions" }, commonCorrectionConfig);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefFNS5AL",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByHierarchy("Value", "ValueReport");
            CorrectSumByHierarchy("Summ", "SummReport");
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
