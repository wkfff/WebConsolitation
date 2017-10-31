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

namespace Krista.FM.Server.DataPumps.BudgetCashReceiptsPump
{
    // УФК_0013_1Н_6_ ВЕДОМОСТЬ ПО КАССОВЫМ ПОСТУПЛЕНИЯМ В БЮДЖЕТ
    public class BudgetCashReceiptsPumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d.KD.UFK)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        // Районы.УФК (d.regions.UFK)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionsCache = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_1Н_Ведомость по кассовым поступлениям в бюджет (f.D.UFK61N)
        private IDbDataAdapter daUFK1N;
        private DataSet dsUFK1N;
        private IFactTable fctUFK1N;

        #endregion Факты

        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;
        private double[] controlSums = null;
        private int yearSourceID;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void GetYearSourceID()
        {
            yearSourceID = AddDataSource("УФК", "0013", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        protected override void QueryData()
        {
            GetYearSourceID();
            InitDataSet(ref daKD, ref dsKD, clsKD, false, string.Format("SOURCEID = {0}", yearSourceID), string.Empty);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Format("SOURCEID = {0}", yearSourceID), string.Empty);
            InitFactDataSet(ref daUFK1N, ref dsUFK1N, fctUFK1N);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR", "ID");
            FillRowsCache(ref regionsCache, dsRegions.Tables[0], "CODE", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daUFK1N, dsUFK1N, fctUFK1N);
        }

        private const string F_D_UFK61N_GUID = "3c0983c7-f830-4048-9142-5479601a44ad";
        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_REGIONS_UFK_GUID = "90375d17-5145-43b9-81f1-2145aba86b7c";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_UFK_GUID] };

            this.AssociateClassifiersEx = this.UsedClassifiers;

            this.UsedFacts = new IFactTable[] { fctUFK1N = this.Scheme.FactTables[F_D_UFK61N_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK1N);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

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

        private void CheckControlSums(double[] reportControlSums)
        {
            if (Math.Abs(reportControlSums[0] - controlSums[0]) > 0.01) 
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, 
                    string.Format("Контрольная сумма {0} ({1}) не совпадает с суммой в отчете ({2})", 
                        "'За период'", controlSums[0], reportControlSums[0]));
            if (Math.Abs(reportControlSums[0] - controlSums[0]) > 0.01) 
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Контрольная сумма {0} ({1}) не совпадает с суммой в отчете ({2})",
                        "'С начала года'", controlSums[1], reportControlSums[1]));
            controlSums[0] = 0;
            controlSums[1] = 0;
        }

        private bool IsReportEnd(string value)
        {
            return value.ToUpper().StartsWith("РУКОВОДИТЕЛЬ УФК");
        }

        private const int constMaxQueryRecords = 10000;
        private void PumpXLSRow(object sheet, int curRow, int refDate, int refRegion)
        {
            string kdName = excelHelper.GetCell(sheet, curRow, 1).Value;
            if (kdName.Length > 255)
                kdName = kdName.Remove(255);
            double forPeriod = Convert.ToDouble(excelHelper.GetCell(sheet, curRow, 3).Value.Replace(" ", "").PadLeft(1, '0'));
            double fromBeginYear = Convert.ToDouble(excelHelper.GetCell(sheet, curRow, 4).Value.Replace(" ", "").PadLeft(1, '0'));
            if (kdName == string.Empty)
            {
                CheckControlSums(new double[] { forPeriod, fromBeginYear });
                return;
            }
            string kdCode = excelHelper.GetCell(sheet, curRow, 2).Value.Trim(' ');
            int refKD = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, new object[] { "CODESTR", kdCode, "NAME", kdName, 
                "Code1", kdCode.Substring(0, 3), "Code2", kdCode.Substring(3, 8), "Code3", kdCode.Substring(11, 2), 
                "Code4", kdCode.Substring(13, 4), "Code5", kdCode.Substring(17, 3), "SOURCEID", yearSourceID }, kdCode, "ID");
            controlSums[0] += forPeriod;
            controlSums[1] += fromBeginYear;
            PumpRow(dsUFK1N.Tables[0], new object[] { "ForPeriod", forPeriod, "FromBeginYearReport", fromBeginYear,
                    "RefYearDayUNV", refDate, "REFKD", refKD, "RefRegions", refRegion });
            if (dsUFK1N.Tables[0].Rows.Count >= constMaxQueryRecords)
            {
                UpdateData();
                ClearDataSet(daUFK1N, ref dsUFK1N);
            }
        }

        private int GetRefDate()
        {
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private const string regionNameStartSubstring = "доходов перечисленных ";
        private int PumpRegion(object sheet, string sheetName)
        {
            string regionCode = CommonRoutines.TrimLetters(sheetName);
            // имя идет после "доходов перечисленных"
            string regionName = excelHelper.GetCell(sheet, 2, 1).Value.TrimStart(' ').Remove(0, regionNameStartSubstring.Length);
            return PumpCachedRow(regionsCache, dsRegions.Tables[0], clsRegions,
                new object[] { "Code", regionCode, "Name", regionName, "SOURCEID", yearSourceID }, regionCode, "ID");
        }

        private void PumpXLSSheet(object sheet, FileInfo file)
        {
            string sheetName = excelHelper.GetSheetName(sheet);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, string.Format("Обработка страницы {0} отчета.", sheetName));
            controlSums = new double[] { 0, 0 };
            int refDate = GetRefDate();
            int refRegion = PumpRegion(sheet, sheetName);
            for (int curRow = 6; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow, string.Format("Обработка файла {0}...", file.FullName),
                            string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value;
                    if (cellValue.ToUpper().StartsWith("РУКОВОДИТЕЛЬ УФК"))
                        break;

                    PumpXLSRow(sheet, curRow, refDate, refRegion);
                    
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, file.FullName, ex.Message), ex);
                }
            }
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, string.Format("Страница {0} отчета успешно обработана.", sheetName));
        }

        private void PumpXLSFile(FileInfo file)
        {
            WriteToTrace(String.Format("Открытие документа {0}", file.Name), TraceMessageKind.Information);
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                rowsCount = GetRowsCount(sheet);
                PumpXLSSheet(sheet, file);
                UpdateData();
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        private bool GetArchFiles(DirectoryInfo dir, ref FileInfo[] acrhFiles)
        {
            acrhFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            return (acrhFiles.GetLength(0) != 0);
        }

        private DirectoryInfo GetTempDir(string fileName)
        {
            string tempDirPath = String.Concat(CommonRoutines.ExtractArchiveFileToTempDir(fileName,
                ArchivatorName.Rar, FilesExtractingOption.SingleDirectory), "\\In_n_n");
            return new DirectoryInfo(tempDirPath);
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            FileInfo[] archFiles = null;
            DirectoryInfo tempDir = dir;
            if (GetArchFiles(dir, ref archFiles))
                tempDir = GetTempDir(archFiles[0].FullName);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                ProcessFilesTemplate(tempDir, "*.xls", new ProcessFileDelegate(PumpXLSFile));
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
            try
            {
                if (tempDir != dir)
                    tempDir.Delete(true);
            }
            catch
            {
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region сопоставление

        protected override int GetClsSourceID(int sourceID)
        {
            if (sourceID > 0)
            {
                IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
                IDataSource clsDs = null;
                clsDs = FindDataSource(ParamKindTypes.Year, ds.SupplierCode, ds.DataCode, string.Empty, ds.Year, 0, string.Empty, 0, string.Empty);
                return clsDs.ID;
            }
            return -1;
        }

        #endregion сопоставление

    }
}
