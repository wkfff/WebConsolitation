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

namespace Krista.FM.Server.DataPumps.FO36Pump
{
    // фо 36 - бюджет кс
    public class FO36PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.ФО_БюджетКС (d_KD_FOBudKS)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;

        // КОСГУ.ФО_БюджетКС (d_KOSGY_FOBudKS)
        private IDbDataAdapter daKosgu;
        private DataSet dsKosgu;
        private IClassifier clsKosgu;
        private Dictionary<string, int> cacheKosgu = null;
        private int nullKosgu = -1;

        // Расходы.ФО_БюджетКС (d_R_FOBudKS)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IClassifier clsOutcomes;
        private Dictionary<string, int> cacheOutcomes = null;
        private int nullOutcomes = -1;

        // Тип средств.ФО_БюджетКС (d_MeansType_FOBudKS)
        private IDbDataAdapter daMeansType;
        private DataSet dsMeansType;
        private IClassifier clsMeansType;
        private Dictionary<string, int> cacheMeansType = null;
        private int nullMeansType = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФО_БюджетКС (f_D_FOBudKS)
        private IDbDataAdapter daFactFO36Incomes;
        private DataSet dsFactFO36Incomes;
        private IFactTable fctFactFO36Incomes;

        // Расходы.ФО_БюджетКС (f_R_FOBudKS)
        private IDbDataAdapter daFactFO36Outcomes;
        private DataSet dsFactFO36Outcomes;
        private IFactTable fctFactFO36Outcomes;

        #endregion Факты

        private ReportType reportType;
        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int yearSourceId = -1;
        private int year = -1;
        private int month = -1;

        #endregion Поля

        #region Структуры, перечисления

        // Тип отчета
        private enum ReportType
        {
            Incomes,
            Outcomes
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            yearSourceId = AddDataSource("ФО", "0036", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daKd, ref dsKd, clsKd, false, string.Format("SOURCEID = {0}", yearSourceId), string.Empty);
            InitDataSet(ref daKosgu, ref dsKosgu, clsKosgu, false, string.Format("SOURCEID = {0}", yearSourceId), string.Empty);
            InitDataSet(ref daOutcomes, ref dsOutcomes, clsOutcomes, false, string.Format("SOURCEID = {0}", yearSourceId), string.Empty);
            InitDataSet(ref daMeansType, ref dsMeansType, clsMeansType, false, string.Format("SOURCEID = {0}", yearSourceId), string.Empty);
            nullMeansType = clsMeansType.UpdateFixedRows(this.DB, yearSourceId);
            InitFactDataSet(ref daFactFO36Incomes, ref dsFactFO36Incomes, fctFactFO36Incomes);
            InitFactDataSet(ref daFactFO36Outcomes, ref dsFactFO36Outcomes, fctFactFO36Outcomes);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CODESTR", "ID");
            FillRowsCache(ref cacheKosgu, dsKosgu.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheOutcomes, dsOutcomes.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheMeansType, dsMeansType.Tables[0], "CODE", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daKosgu, dsKosgu, clsKosgu);
            UpdateDataSet(daOutcomes, dsOutcomes, clsOutcomes);
            UpdateDataSet(daMeansType, dsMeansType, clsMeansType);
            UpdateDataSet(daFactFO36Incomes, dsFactFO36Incomes, fctFactFO36Incomes);
            UpdateDataSet(daFactFO36Outcomes, dsFactFO36Outcomes, fctFactFO36Outcomes);
        }

        private const string D_KD_GUID = "5f7dd917-b963-48ac-afe2-b334bc30dc9b";
        private const string D_KOSGU_GUID = "b8f76304-e0db-40f8-abec-f957f67d4a8c";
        private const string D_OUTCOMES_GUID = "0a067788-2e39-4928-baba-9d9ea5c00deb";
        private const string D_MEANS_TYPE_GUID = "9d6530b1-ecab-42b0-ba41-6c539fb60af2";
        private const string F_FO36_INCOMES_GUID = "d070a6eb-41c3-4823-aeb0-ce999b7b5163";
        private const string F_FO36_OUTCOMES_GUID = "cf6656f1-6c0b-4fed-bed0-d9792826047f";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKd = this.Scheme.Classifiers[D_KD_GUID],
                clsKosgu = this.Scheme.Classifiers[D_KOSGU_GUID],
                clsOutcomes = this.Scheme.Classifiers[D_OUTCOMES_GUID],
                clsMeansType = this.Scheme.Classifiers[D_MEANS_TYPE_GUID] };

            this.UsedFacts = new IFactTable[] { 
                fctFactFO36Incomes = this.Scheme.FactTables[F_FO36_INCOMES_GUID], 
                fctFactFO36Outcomes = this.Scheme.FactTables[F_FO36_OUTCOMES_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsKosgu);
            ClearDataSet(ref dsOutcomes);
            ClearDataSet(ref dsMeansType);
            ClearDataSet(ref dsFactFO36Incomes);
            ClearDataSet(ref dsFactFO36Outcomes);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Excel

        private const string REPORT_TYPE_INCOMES = "ДОХОД";
        private ReportType GetReportType(string fileName)
        {
            if (fileName.ToUpper().Contains(REPORT_TYPE_INCOMES))
                return ReportType.Incomes;
            else
                return ReportType.Outcomes;
        }

        private int PumpKd(DataRow row)
        {
            string code = row["KdCode"].ToString().Trim();
            string name = row["Name"].ToString().Trim();
            if (name.Length > 255)
                name = name.Substring(0, 255);

            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code,
                new object[] { "CodeStr", code, "Name", name, "SourceId", yearSourceId });
        }

        private int PumpMeansType(DataRow row)
        {
            string code = row["MeansTypeCode"].ToString().Trim();
            string name = row["Name"].ToString().Trim();
            if (code == string.Empty)
                return nullMeansType;
            if (name.Length > 255)
                name = name.Substring(0, 255);

            return PumpCachedRow(cacheMeansType, dsMeansType.Tables[0], clsMeansType, code,
                new object[] { "Code", code, "Name", name, "SourceId", yearSourceId });
        }

        private int PumpKosgu(DataRow row)
        {
            int code = Convert.ToInt32(row["KosguCode"].ToString().Trim());
            string name = row["Name"].ToString().Trim();
            if (name.Length > 255)
                name = name.Substring(0, 255);

            return PumpCachedRow(cacheKosgu, dsKosgu.Tables[0], clsKosgu, code.ToString(),
                new object[] { "Code", code, "Name", name, "SourceId", yearSourceId });
        }

        private int PumpOutcomes(DataRow row)
        {
            long code =  Convert.ToInt64(string.Format("{0}{1}{2}{3}", row["OucomeCode1"].ToString().Trim(),
                  row["OucomeCode2"].ToString().Trim(), row["OucomeCode3"].ToString().Trim(), row["OucomeCode4"].ToString().Trim()));
            string name = row["Name"].ToString().Trim();
            if (name.Length > 255)
                name = name.Substring(0, 255);

            return PumpCachedRow(cacheOutcomes, dsOutcomes.Tables[0], clsOutcomes, code.ToString(), "Code",
                new object[] { "Code", code, "Name", name, "SourceId", yearSourceId });
        }

        private void PumpFactRowIncomes(object[] mapping)
        {
            PumpRow(dsFactFO36Incomes.Tables[0], mapping);
            if (dsFactFO36Incomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactFO36Incomes, ref dsFactFO36Incomes);
            }
        }

        private void PumpFactRowOutcomes(object[] mapping)
        {
            PumpRow(dsFactFO36Outcomes.Tables[0], mapping);
            if (dsFactFO36Outcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactFO36Outcomes, ref dsFactFO36Outcomes);
            }
        }

        private void PumpXlsRow(DataRow row, int refDate)
        {
            object[] mapping = null;
            if (reportType == ReportType.Incomes)
            {
                int refKd = PumpKd(row);
                int refMeansType = PumpMeansType(row);

                decimal balance = Convert.ToDecimal(row["Balance"].ToString().PadLeft(1, '0'));
                decimal planY = Convert.ToDecimal(row["PlanY"].ToString().PadLeft(1, '0'));
                decimal fact = Convert.ToDecimal(row["Fact"].ToString().PadLeft(1, '0'));
                decimal factPeriod = Convert.ToDecimal(row["FactPeriod"].ToString().PadLeft(1, '0'));

                mapping = new object[] { "Balance", balance, "PlanY", planY, "Fact", fact, "FactPeriod", factPeriod,
                    "RefKD", refKd, "RefMeansType", refMeansType, "RefYearDayUNV", refDate };
                PumpFactRowIncomes(mapping);
            }
            else
            {
                int refKosgu = PumpKosgu(row);
                int refOutcomes = PumpOutcomes(row);
                int refMeansType = PumpMeansType(row);

                decimal planYRep = Convert.ToDecimal(row["PlanYRep"].ToString().PadLeft(1, '0'));
                decimal financRep = Convert.ToDecimal(row["FinancRep"].ToString().PadLeft(1, '0'));
                decimal chargesRep = Convert.ToDecimal(row["ChargesRep"].ToString().PadLeft(1, '0'));
                decimal balanceRep = Convert.ToDecimal(row["BalanceRep"].ToString().PadLeft(1, '0'));

                mapping = new object[] { "PlanYRep", planYRep, "FinancRep", financRep, "ChargesRep", chargesRep, "BalanceRep", balanceRep,
                    "RefKOSGU", refKosgu, "RefR", refOutcomes, "RefMeansType", refMeansType, "RefYearDayUNV", refDate };
                PumpFactRowOutcomes(mapping);
            }
        }

        private int GetDateRef(object sheet)
        {
            string value = string.Empty;

            if (reportType == ReportType.Incomes)
                value = excelHelper.GetCell(sheet, 5, 1).Value.Trim();
            else
                value = excelHelper.GetCell(sheet, 3, 1).Value.Trim();

            string[] dateRow = value.ToUpper().Split(new string[] { "ПО" }, StringSplitOptions.None);
            dateRow[1] = CommonRoutines.TrimLetters(dateRow[1]);
            return CommonRoutines.ShortDateToNewDate(dateRow[1]);
        }

        private int GetFirstRow()
        {
            if (reportType == ReportType.Incomes)
                return 9;
            return 5;
        }

        private int GetLastRow(object sheet)
        {
            for (int curRow = GetFirstRow(); ; curRow++)
                if (excelHelper.GetCell(sheet, curRow, 2).Value.Trim() == string.Empty)
                    return (curRow - 1);
        }

        private object[] XLS_MAPPING_INCOMES = new object[] {
            "KdCode", 1, "Name", 2, "MeansTypeCode", 4, "Balance", 9, "PlanY", 12, "Fact", 21, "FactPeriod", 24};
        private object[] XLS_MAPPING_OUTCOMES = new object[] {
            "Name", 1, "OucomeCode1", 2, "OucomeCode2", 3, "OucomeCode3", 4, "OucomeCode4", 5, "KosguCode", 6,
            "MeansTypeCode", 7, "PlanYRep", 9, "FinancRep", 22, "ChargesRep", 23, "BalanceRep", 24};
        private object[] GetXlsMapping()
        {
            if (reportType == ReportType.Incomes)
                return XLS_MAPPING_INCOMES;
            return XLS_MAPPING_OUTCOMES;
        }

        private void PumpXLSSheetData(FileInfo file, object sheet)
        {
            int refDate = GetDateRef(sheet);
            int lastRow = GetLastRow(sheet);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, GetFirstRow(), lastRow, GetXlsMapping());
            for (int curRow = 0; curRow < dt.Rows.Count; curRow++)
            {
                try
                {
                    SetProgress(lastRow, curRow, string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", curRow, lastRow));
                    PumpXlsRow(dt.Rows[curRow], refDate);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                reportType = GetReportType(file.Name);
                object sheet = excelHelper.GetSheet(workbook, 1);
                PumpXLSSheetData(file, sheet);
            }
            finally
            {
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
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
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

        private void CorrectSumByHierarchy(string field, string fieldReport)
        {
            F4NMSumCorrectionConfig f4nomSumCorrectionConfig = new F4NMSumCorrectionConfig();
            f4nomSumCorrectionConfig.ValueField = field;
            f4nomSumCorrectionConfig.ValueReportField = fieldReport;
            CorrectFactTableSums(fctFactFO36Outcomes, dsOutcomes.Tables[0], clsOutcomes, "RefR",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { }, string.Empty, string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy(ref dsKd, clsKd, null, "CodeStr", ClsHierarchyMode.Standard);
            SetClsHierarchy(ref dsOutcomes, clsOutcomes, null, "Code", ClsHierarchyMode.Standard);
            UpdateData();
            CorrectSumByHierarchy("PlanY", "PlanYRep");
            CorrectSumByHierarchy("Financing", "FinancRep");
            CorrectSumByHierarchy("Charges", "ChargesRep");
            CorrectSumByHierarchy("Balance", "BalanceRep");
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
