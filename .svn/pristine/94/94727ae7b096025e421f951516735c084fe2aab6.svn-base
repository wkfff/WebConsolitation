using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS29Pump
{

    // ФНС - 0029 - 1 Патент
    public class FNS29PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС_1 ПАТЕНТ (d_Marks_FNS1PATENT)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        private int nullMarks = -1;
        // Районы.ФНС (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, DataRow> cacheRegions = null;
        private Dictionary<string, string> cacheRegionsNames = null;
        private Dictionary<string, DataRow> cacheRegionsFirstRow = null;
        private int nullRegions = -1;
        // ЕдИзмер.ОКЕИ (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_1 ПАТЕНТ_Сводный (f_D_FNS1PATENTTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_1 ПАТЕНТ_Районы (f_D_FNS1PATENTRegions)
        private IDbDataAdapter daIncomesRegions;
        private DataSet dsIncomesRegions;
        private IFactTable fctIncomesRegions;

        #endregion Факты

        private ReportType reportType;
        private decimal totalSum = 0;
        private string regionName = string.Empty;
        private string regionCode = string.Empty;

        // параметры обработки
        private int year = -1;
        private int month = -1;

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

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "Code", "Name" }, "|");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegions, ref dsIncomesRegions, fctIncomesRegions);
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegions, dsIncomesRegions, fctIncomesRegions);
        }

        private const string D_MARKS_FNS_1PATENT_GUID = "85e59f93-9d28-4a31-b98b-6eb3c76def8b";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_1PATENT_TOTAL_GUID = "92cad1d0-04bb-49e9-b1f2-afe9c30c63ce";
        private const string F_D_FNS_1PATENT_REGIONS_GUID = "32053ac9-e2ff-4acb-8f91-d9cd3e43ebfa";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_1PATENT_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_1PATENT_TOTAL_GUID],
                fctIncomesRegions = this.Scheme.FactTables[F_D_FNS_1PATENT_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegions);
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

        private int GetReportDate()
        {
            // получаем из параметров источника
            return (this.DataSource.Year * 10000 + this.DataSource.Month * 100);
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0))
                throw new Exception("Не заполнен Классификатор 'Показатели.ФНС_1 ПАТЕНТ' - закачайте сводные отчеты");
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
            // reportType = ReportType.Str;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            // ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
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
            decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, 3));
            CheckTotalSum(totalSum, controlSum, string.Empty);
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
            regionName = excelDoc.GetValue(curRow + 1, 1);
            regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1));
            return PumpRegion(regionCode, regionName);
        }

        private void PumpFactRow(decimal factValue, int refDate, int refMarks, int refRegions)
        {
            if (factValue == 0)
                return;

            totalSum += factValue;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] {
                    "Value", factValue, "RefYearDayUNV", refDate, "RefMarks", refMarks };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "Value", factValue,
                    "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions };
                PumpRow(dsIncomesRegions.Tables[0], mapping);
                if (dsIncomesRegions.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegions, ref dsIncomesRegions);
                }
            }
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            int code = Convert.ToInt32(excelDoc.GetValue(curRow, 2).Trim());
            string name = excelDoc.GetValue(curRow, 1).Trim();
            if (name.Length > 255)
                name = name.Substring(0, 255);
            object[] mapping = new object[] { "Code", code, "Name", name };

            if (reportType != ReportType.Svod)
                return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
            return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, code.ToString(), mapping);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions, bool isFirstPumpedRow)
        {
            if (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
                return;

            int refMarks = PumpXlsMarks(excelDoc, curRow);
            // из первой строки формируем только показатель, а факт не качаем
            if (!isFirstPumpedRow)
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                PumpFactRow(factValue, refDate, refMarks, refRegions);
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

        private const string MARK_REGION = "КОД ОКАТО";
        private const string REGION_ROW = "МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ";
        private bool IsRegionRow(ExcelHelper excelDoc, int curRow)
        {
            if (reportType != ReportType.Region)
                return false;
            string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            string cellValue2 = excelDoc.GetValue(curRow + 2, 1).Trim().ToUpper();
            return cellValue.StartsWith(REGION_ROW) && cellValue2.StartsWith(MARK_REGION);
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            if (reportType == ReportType.Region)
                return (cellValue.Trim().ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        private void PumpXlsSheetData(string filename, ExcelHelper excelDoc, int refDate)
        {
            bool isFirstPumpedRow = false;
            bool toPump = false;
            int rowsCount = excelDoc.GetRowsCount();
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int refRegions = -1;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, filename),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsRegionRow(excelDoc, curRow))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        curRow += 2;
                        continue;
                    }

                    if (IsSectionEnd(cellValue))
                    {
                        //проверка контрольной суммы убрна, т.к. не читаем первую строку в таблице.
                        //CheckXlsTotalSum(excelDoc, curRow);
                        toPump = false;
                    }

                    if (toPump)
                    {
                        PumpXlsRow(excelDoc, curRow, refDate, refRegions, isFirstPumpedRow);
                        isFirstPumpedRow = false;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        // в отчетах райнов иногда сверху добавляется одна вспомогательная таблица - ее не качаем
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1)))
                            continue;
                        SetNullTotalSum();
                        isFirstPumpedRow = true;
                        toPump = true;
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
                excelDoc.OpenDocument(file.FullName);
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
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

        #region Перекрытые методы

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

        #endregion Перекрытые методы

        #endregion Закачка данных

        #region Обработка данных

        private void SetClsHierarchy()
        {
            SetClsHierarchy(clsMarks, ref dsMarks, "Code", const_d_Marks_FNS29_HierarchyFile2010, ClsHierarchyMode.Special);
        }

        private const string UNIT_UNIT_NAME = "Единица";
        private void SetRefUnits()
        {
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                row["RefUnits"] = FindCachedRow(cacheUnits, UNIT_UNIT_NAME, nullUnits);
            }
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Выполняется установка иерархии для классификатора 'Показатели.ФНС_1 ПАТЕНТ'");
        }   

        #endregion Обработка данных

    }

}
