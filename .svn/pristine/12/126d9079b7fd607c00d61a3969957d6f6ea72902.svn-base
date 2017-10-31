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

namespace Krista.FM.Server.DataPumps.FNSRF3Pump
{

    // ФНС РФ 0003 - Форма 1 НОМ
    public class FNSRF3PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ОКВЭД.ФНС (d.OKVED.FNS)
        private IDbDataAdapter daOkved;
        private DataSet dsOkved;
        private IClassifier clsOkved;
        private Dictionary<string, int> okvedCache = null;
        private int nullOkved;
        // Доходы.Группы ФНС (d.D.GroupFNS)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IClassifier clsIncomes;
        private Dictionary<int, int> incomesCache = null;
        private int nullIncomes;
        // Территории.ФНС РФ (d.Territory.FNSRF)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> territoryCache = null;
        private int nullTerritory;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС РФ_1 НОМ (f.D.FNSRF1NOM)
        private IDbDataAdapter daIncomesFact;
        private DataSet dsIncomesFact;
        private IFactTable fctIncomesFact;

        #endregion Факты

        // объект доступа к экселу
        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int sectionIndex = -1;
        private decimal[] totalSums = new decimal[17];
        // параметры обработки
        private int year;
        private int month;
        
        #endregion Поля

        #region Константы

        private int[] incomesCodesList;
        // список кодов в классификаторе Доходы.Группы ФНС по разделам
        private int[] incomesCodeSection1List = new int[] { 0, 0, 100000000, 101000000,
            101010000, 102000000, 108000000, 103000000, 103010000, 104000000,
            200000000, 300000000, 400000000 };
        private int[] incomesCodeSection2List = new int[] { 109000000, 109000000, 109010000,
            109010100, 109010200, 109010300, 109010400, 109020000, 109020100, 109020200,
            109020300, 109020400, 109030000, 109030100, 109030200, 109030300, 109030400 };

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daOkved, ref dsOkved, clsOkved);
            nullOkved = clsOkved.UpdateFixedRows(this.DB, this.SourceID);
            int incomesSourceId = AddDataSource("ФНСРФ", "0003", ParamKindTypes.YearMonth,
                string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daIncomes, ref dsIncomes, clsIncomes, false,
                string.Format("SOURCEID = {0}", incomesSourceId), string.Empty);
            nullIncomes = clsIncomes.UpdateFixedRows(this.DB, incomesSourceId);
            InitClsDataSet(ref daTerritory, ref dsTerritory, clsTerritory);
            nullTerritory = clsTerritory.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesFact, ref dsIncomesFact, fctIncomesFact);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref okvedCache, dsOkved.Tables[0], "ROWCODE", "ID");
            FillRowsCache(ref incomesCache, dsIncomes.Tables[0], "CODE", "ID");
            FillRowsCache(ref territoryCache, dsTerritory.Tables[0], "CODE", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOkved, dsOkved, clsOkved);
            UpdateDataSet(daTerritory, dsTerritory, clsTerritory);
            UpdateDataSet(daIncomesFact, dsIncomesFact, fctIncomesFact);
        }

        private const string D_OKVED_FNS_GUID = "9f549d45-9e27-4c0a-948e-b99294de79bf";
        private const string D_D_GROUP_FNS_GUID = "b9169eb6-de81-420b-8a2b-05ffa2fd35c1";
        private const string D_TERRITORY_FNSRF_GUID = "965c78a9-b857-44ce-9e26-4a14ba740be6";
        private const string F_D_FNS4RFNM_FACT_GUID = "fc9b4263-44e7-40ef-bd4b-d96432189e41";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsOkved = this.Scheme.Classifiers[D_OKVED_FNS_GUID],
                clsTerritory = this.Scheme.Classifiers[D_TERRITORY_FNSRF_GUID],
                clsIncomes = this.Scheme.Classifiers[D_D_GROUP_FNS_GUID] };
            this.UsedFacts = new IFactTable[] { 
                fctIncomesFact = this.Scheme.FactTables[F_D_FNS4RFNM_FACT_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesFact);
            ClearDataSet(ref dsOkved);
            ClearDataSet(ref dsIncomes);
            ClearDataSet(ref dsTerritory);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        private void CheckIncomes()
        {
            // если не заполнен классификатор Доходы.Группы ФНС - предупреждение
            if (incomesCache.Count <= 1)
                throw new Exception("Не заполнен классификатор «Доходы.Группы ФНС». Данные по этому источнику закачаны не будут.");
        }

        private int GetReportDate()
        {
            return (this.DataSource.Year * 10000 + this.DataSource.Month * 100);
        }

        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            if (cellValue.StartsWith("РАЗДЕЛ II"))
            {
                incomesCodesList = incomesCodeSection2List;
                return 1;
            }
            else if (cellValue.StartsWith("РАЗДЕЛ I"))
            {
                incomesCodesList = incomesCodeSection1List;
                return 0;
            }
            incomesCodesList = incomesCodeSection1List;
            return -1;
        }

        private void ClearTotalSums()
        {
            for (int i = 0; i < totalSums.GetLength(0); i++)
                totalSums[i] = 0.0M;
        }

        private string GetSectionName()
        {
            if (sectionIndex == 0)
                return "РАЗДЕЛ I";
            else if (sectionIndex == 1)
                return "РАЗДЕЛ II";
            return string.Empty;
        }

        #endregion Общие функции закачки

        #region Работа с Excel

        private void CheckTotalSums(object sheet, int curRow)
        {
            int columnsCount = GetColumnsCount();
            string sectionName = GetSectionName();
            for (int curColumn = 4; curColumn <= columnsCount; curColumn++)
            {
                string cellValue = excelHelper.GetCell(sheet, curRow, curColumn).Value.Trim();
                decimal control = Convert.ToDecimal(cellValue.PadLeft(1, '0'));
                if (control != totalSums[curColumn - 4])
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Контрольная сумма {0:F} по столбцу {1} {2} не сходится с итоговой {3:F}",
                        control, curColumn, sectionName, totalSums[curColumn - 4]));
            }
        }

        private int GetRowsCount(object sheet)
        {
            int curRow = 0;
            int emptyStrCount = 0;
            for (curRow = 1; emptyStrCount < 10; curRow++)
            {
                string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                if (cellValue == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
            }
            return (curRow - 10);
        }

        private int GetColumnsCount()
        {
            if ((this.DataSource.Year >= 2007) && (sectionIndex > 0))
                return 20;
            return 16;
        }

        private int PumpTerritory(object sheet)
        {
            int code = 0;
            string name = "Неуказанное наименование";
            // пытаемся найти территорию в диапазоне ячеек A5..A25
            for (int curRow = 5; curRow <= 25; curRow++)
            {
                string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                if (cellValue.ToUpper().StartsWith("НАЛОГОВЫЙ ОРГАН"))
                    code = Convert.ToInt32(CommonRoutines.TrimLetters(cellValue));
                if (cellValue.ToUpper().Contains("ОБРАЗОВАНИЕ"))
                    name = excelHelper.GetCell(sheet, curRow + 1, 1).Value;
            }
            object[] mapping = new object[] { "CODE", code, "NAME", name };
            return PumpCachedRow(territoryCache, dsTerritory.Tables[0], clsTerritory, mapping, code.ToString(), "ID");
        }

        private int PumpOkved(object sheet, int curRow)
        {
            string name = excelHelper.GetCell(sheet, curRow, 1).Value;
            string codeStr = excelHelper.GetCell(sheet, curRow, 2).Value;
            int rowCode = Convert.ToInt32(excelHelper.GetCell(sheet, curRow, 3).Value);
            object[] mapping = new object[] { "NAME", name, "CODESTR", codeStr, "ROWCODE", rowCode };
            int refOkved = PumpCachedRow(okvedCache, dsOkved.Tables[0], clsOkved, mapping, rowCode.ToString(), "ID");
            return refOkved;
        }

        private int GetRefIncomes(int curColumn)
        {
            int code = incomesCodesList[curColumn - 4];
            return FindCachedRow(incomesCache, code, nullIncomes);
        }

        private void PumpXlsRow(object sheet, int curRow, int refDate, int refTerritory)
        {
            if (excelHelper.GetCell(sheet, curRow, 3).Value.Trim() == string.Empty)
                return;
            object[] mapping = null;
            int refOkved = PumpOkved(sheet, curRow);
            int columnsCount = GetColumnsCount();
            for (int curColumn = 4; curColumn <= columnsCount; curColumn++)
            {
                int refIncomes = GetRefIncomes(curColumn);
                string cellValue = excelHelper.GetCell(sheet, curRow, curColumn).Value.Trim();
                decimal value = Convert.ToDecimal(cellValue.PadLeft(1, '0'));
                totalSums[curColumn - 4] += value;
                value *= 1000;

                if (curColumn == 4)
                    mapping = new object[] { "EarnedReport", value, "RefTerritory", refTerritory,
                        "RefD", refIncomes, "RefOKVED", refOkved, "RefYearDayUNV", refDate };
                else
                    mapping = new object[] { "InpaymentsReport", value, "RefTerritory", refTerritory,
                        "RefD", refIncomes, "RefOKVED", refOkved, "RefYearDayUNV", refDate };
                
                PumpRow(dsIncomesFact.Tables[0], mapping);
                if (dsIncomesFact.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesFact, ref dsIncomesFact);
                }
            }
        }

        private bool IsSectionStartMark(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        private bool IsSectionEndMark(string cellValue)
        {
            return (cellValue.ToUpper().StartsWith("КОНТРОЛЬНАЯ СУММА"));
        }

        private bool IsSectionRow(string cellValue)
        {
            return (cellValue.ToUpper().StartsWith("РАЗДЕЛ"));
        }

        private void PumpXlsSheetData(string fileName, object sheet, int refDate)
        {
            int refTerritory = PumpTerritory(sheet);
            incomesCodesList = incomesCodeSection1List;
            sectionIndex = -1;
            bool toPump = false;
            int rowsCount = GetRowsCount(sheet);
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow, string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsSectionEndMark(cellValue))
                    {
                        toPump = false;
                        CheckTotalSums(sheet, curRow);
                        continue;
                    }

                    if (IsSectionRow(cellValue))
                    {
                        sectionIndex = GetSectionIndex(cellValue);
                        continue;
                    }

                    if (IsSectionStartMark(cellValue))
                    {
                        toPump = true;
                        ClearTotalSums();
                        continue;
                    }

                    if (toPump)
                    {
                        PumpXlsRow(sheet, curRow, refDate, refTerritory);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} листа {1} возникла ошибка ({2})",
                        curRow, excelHelper.GetSheetName(sheet), ex.Message), ex);
                }
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int refDate = GetReportDate();
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int curSheet = 1; curSheet <= sheetCount; curSheet++)
                {
                    object sheet = excelHelper.GetSheet(workbook, curSheet);
                    PumpXlsSheetData(file.Name, sheet, refDate);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        private void PumpXlsFiles(DirectoryInfo dir)
        {
            CheckIncomes();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных из отчетов.");
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                PumpXlsFiles(dir);
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
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private void SetClsHierarchy()
        {
            string d_OKVED_FNSRF3_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2007)
                d_OKVED_FNSRF3_HierarchyFileName = const_d_OKVED_FNSRF3_HierarchyFile2007;
            else if (this.DataSource.Year >= 2006)
                d_OKVED_FNSRF3_HierarchyFileName = const_d_OKVED_FNSRF3_HierarchyFile2006;
            else
                d_OKVED_FNSRF3_HierarchyFileName = const_d_OKVED_FNSRF3_HierarchyFile2005;
            SetClsHierarchy(clsOkved, ref dsOkved, "ROWCODE", d_OKVED_FNSRF3_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByDataSource()
        {
            F4NMSumCorrectionConfig f4nmSumCorrectionConfig = new F4NMSumCorrectionConfig();
            f4nmSumCorrectionConfig.ValueField = "Earned";
            f4nmSumCorrectionConfig.ValueReportField = "EarnedReport";
            // GroupTable(fctIncomesFact, new string[] { "RefTerritory", "RefYearDayUNV", "RefOKVED", "RefD" }, f4nmSumCorrectionConfig);
            CorrectFactTableSums(fctIncomesFact, dsOkved.Tables[0], clsOkved, "RefOKVED",
                f4nmSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefTerritory", "RefYearDayUNV", "RefD" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesFact, dsIncomes.Tables[0], clsIncomes, "RefD",
                f4nmSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefTerritory", "RefYearDayUNV", "RefOKVED" }, string.Empty, string.Empty, false);
            f4nmSumCorrectionConfig.ValueField = "Inpayments";
            f4nmSumCorrectionConfig.ValueReportField = "InpaymentsReport";
            CorrectFactTableSums(fctIncomesFact, dsOkved.Tables[0], clsOkved, "RefOKVED",
                f4nmSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefTerritory", "RefYearDayUNV", "RefD" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesFact, dsIncomes.Tables[0], clsIncomes, "RefD",
                f4nmSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefTerritory", "RefYearDayUNV", "RefOKVED" }, string.Empty, string.Empty, false);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByDataSource();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month,
                "Выполняется установка иерархии в классификаторе «ОКВЭД.ФНС» и коррекция сумм фактов по данным источника");
        }

        #endregion Обработка данных

    }

}
