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

namespace Krista.FM.Server.DataPumps.FNS12Pump
{
    // фнс 0012 - форма 5 вбр
    public class FNS12PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 ВБР (d.Marks.FNS5VBR)
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

        // Доходы.ФНС_5 ВБР_Сводный (f.D.FNSVBRTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 ВБР_Районы (f.D.FNS5VBRRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        private ExcelHelper excelHelper;
        private object excelObj = null;
        // итоговая сумма
        private decimal totalSum;
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

        private const string D_MARKS_FNS_5VBR_GUID = "5d16a2b8-b432-40c4-a0c9-c9b7f973c9d0";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5VBR_TOTAL_GUID = "4e804a0f-8b1c-418f-9ebc-bb18fc124e02";
        private const string F_D_FNS_5VBR_REGIONS_GUID = "b6b7029c-fbf0-4d0d-828f-7c322e50bc2d";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5VBR_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID]};
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5VBR_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5VBR_REGIONS_GUID] };
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

        private void GetSumMultiplier(int marksCode)
        {
            switch (marksCode)
            {
                case 100:
                case 200:
                    sumMultiplier = 1;
                    break;
                default:
                    sumMultiplier = 1000;
                    break;
            }
        }

        // обнуление итоговой суммы
        private void SetNullTotalSum()
        {
            totalSum = 0.0M;
        }

        private void CheckTotalSum(decimal totalSum, decimal controlSum)
        {
            if (totalSum != controlSum)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F}", controlSum, totalSum));
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

        private void WriteStartMsgToProtocol()
        {
            switch (reportType)
            {
                case ReportType.Region:
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
                    break;
                case ReportType.Str:
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
                    break;
                case ReportType.Svod:
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
                    break;
            }
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0))
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС 5 ВБР' - закачайте сводные отчеты");
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
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            reportType = ReportType.Str;
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
        private string CleanFactValue(string value)
        {
            return value.ToUpper().Trim(LITERAL_X.ToCharArray());
        }

        private void CheckXLSTotalSum(object sheet, int curRow)
        {
            string value = value = CleanFactValue(excelHelper.GetCell(sheet, curRow, 3).Value.Trim());
            if (value != string.Empty)
                CheckTotalSum(totalSum, Convert.ToDecimal(value));
        }

        private int PumpRegion(object sheet, int regionTitleRow)
        {
            string regionName = excelHelper.GetCell(sheet, regionTitleRow + 1, 1).Value;
            string regionCode = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, regionTitleRow + 2, 1).Value);

            if (cacheRegionsNames.ContainsValue(regionName))
                regionName = string.Format("{0} ({1})", regionName, regionCode);

            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regKey = string.Format("{0}|{1}", regionCode, regionName);
            int refRegion = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, mapping, regKey, "ID");
            cacheRegionsNames.Add(refRegion, regionName);

            return refRegion;
        }

        private int PumpMarks(object sheet, int curRow)
        {
            int code = nullMarks;
            string name = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
            string value = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
            if (value == string.Empty)
                return -1;
            code = Convert.ToInt32(value);
            GetSumMultiplier(code);

            object[] mapping = new object[] { "NAME", name, "CODE", code };

            if (reportType == ReportType.Svod)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            else
                return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpFactRow(int refMarks, int refRegions, int refDate, decimal valueReport)
        {
            object[] mapping = new object[] { "RefMarks", refMarks, "RefYearDayUNV", refDate, "Value", valueReport };
            if (reportType != ReportType.Svod)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegions", refRegions });

            PumpRow(GetFactTable(), mapping);
            if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
            }
        }

        private void PumpXLSRow(object sheet, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpMarks(sheet, curRow);
            if (refMarks == -1)
                return;

            string value = CleanFactValue(excelHelper.GetCell(sheet, curRow, 3).Value.Trim());
            if (value == string.Empty)
                return;

            decimal valueReport = Convert.ToDecimal(value);
            if (valueReport != 0)
            {
                totalSum += valueReport;
                valueReport *= sumMultiplier;
                PumpFactRow(refMarks, refRegions, refDate, valueReport);
            }
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(object sheet, int curRow)
        {
            string cellValue = excelHelper.GetCell(sheet, curRow + 1, 1).Value.Trim();
            if (reportType == ReportType.Region)
                return (cellValue.ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        public const string REGION_START_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private bool IsRegionStart(string cellValue)
        {
            if (reportType == ReportType.Region)
                return (cellValue.ToUpper() == REGION_START_ROW);
            return false;
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private void PumpXLSSheetData(FileInfo file, object sheet, int refDate)
        {
            int refRegions = nullRegions;
            int rowsCount = GetRowsCount(sheet);
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);

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

                    if (toPumpRow)
                    {
                        PumpXLSRow(sheet, curRow, refDate, refRegions);
                        continue;
                    }

                    if (IsRegionStart(cellValue))
                    {
                        refRegions = PumpRegion(sheet, curRow);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        // в некоторых отчетах райнов перед основными разделами добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(sheet, curRow))
                            continue;

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
            WriteStartMsgToProtocol();
            CheckMarks();
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
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            switch (marksCode)
            {
                case 100:
                case 200:
                    return UNIT_UNIT_NAME;
            }
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
            ProcessDataSourcesTemplate(year, month, "Выполняется проставление ссылки с классификатора показателей на классификатор единиц измерения.");
        }   

        #endregion Обработка данных

    }
}
