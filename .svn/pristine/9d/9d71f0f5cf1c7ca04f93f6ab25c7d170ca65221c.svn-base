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

namespace Krista.FM.Server.DataPumps.FO37Pump
{
    // фо 37 - исполнение бюджета
    public class FO37PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФО_ИсполнениеБюджета (d_Marks_FOBudImp)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.ФО_Исполнение бюджета (f_Marks_FOBudImp)
        private IDbDataAdapter daMarksFOBudImp;
        private DataSet dsMarksFOBudImp;
        private IFactTable fctMarksFOBudImp;

        // Показатели.ФО_Исполнение бюджета_Доходы (f_Marks_FOBudImpM)
        private IDbDataAdapter daMarksFOBudImpM;
        private DataSet dsMarksFOBudImpM;
        private IFactTable fctMarksFOBudImpM;

        #endregion Факты

        private int parentMarksId = -1;
        private ReportType reportType;
        private ReportDataType reportDataType;
        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int year = -1;
        private int month = -1;
        private List<int> deletedDateList = null;

        #endregion Поля

        #region Структуры, перечисления

        // тип данных
        private enum ReportDataType
        {
            Weekly,
            Monthly
        }

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
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            InitFactDataSet(ref daMarksFOBudImp, ref dsMarksFOBudImp, fctMarksFOBudImp);
            InitFactDataSet(ref daMarksFOBudImpM, ref dsMarksFOBudImpM, fctMarksFOBudImpM);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "NAME", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daMarksFOBudImp, dsMarksFOBudImp, fctMarksFOBudImp);
            UpdateDataSet(daMarksFOBudImpM, dsMarksFOBudImpM, fctMarksFOBudImpM);
        }

        private const string D_MARKS_GUID = "f41310da-d11a-4562-9e31-b9a91a359a9d";
        private const string F_MARKS_FOBUDIMP = "9444193a-898b-4fdb-ac32-6aaaf4337781";
        private const string F_MARKS_FOBUDIMP_M = "c4769058-2374-496e-8407-9baf1eb1ff9f";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { };
            clsMarks = this.Scheme.Classifiers[D_MARKS_GUID];

            this.UsedFacts = new IFactTable[] { };
            fctMarksFOBudImp = this.Scheme.FactTables[F_MARKS_FOBUDIMP];
            fctMarksFOBudImpM = this.Scheme.FactTables[F_MARKS_FOBUDIMP_M];
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsMarksFOBudImp);
            ClearDataSet(ref dsMarksFOBudImpM);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Excel

        private decimal CleanFactValue(string factValue)
        {
            factValue = CommonRoutines.TrimLetters(factValue.Trim()).Replace(".", ",").Trim();
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        private const string REPORT_TYPE_INCOMES = "ДОХОД";
        private ReportType GetReportType(string fileName)
        {
            if (fileName.ToUpper().Contains(REPORT_TYPE_INCOMES))
                return ReportType.Incomes;
            return ReportType.Outcomes;
        }

        private const string PARENT_MARKS_INCOMES = "НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ";
        private const string PARENT_MARKS_OUTCOMES = "РАСХОДЫ БЮДЖЕТА-ИТОГО";
        private int PumpMarks(DataRow row)
        {
            string name = row["Name"].ToString().Trim();
            if (name.Length > 255)
                name = name.Substring(0, 255);

            if ((name.ToUpper() == PARENT_MARKS_INCOMES) || (name.ToUpper() == PARENT_MARKS_OUTCOMES))
            {
                parentMarksId = PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, new object[] { "Name", name }, name, "ID");
                return parentMarksId;
            }
            object[] mapping = new object[] { "Name", name, "ParentId", parentMarksId };
            return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, name, "ID");
        }

        private void PumpFactsMonthly(object[] mapping)
        {
            PumpRow(dsMarksFOBudImp.Tables[0], mapping);
            if (dsMarksFOBudImp.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMarksFOBudImp, ref dsMarksFOBudImp);
            }
        }

        private void PumpFactsWeekly(object[] mapping)
        {
            PumpRow(dsMarksFOBudImpM.Tables[0], mapping);
            if (dsMarksFOBudImpM.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMarksFOBudImpM, ref dsMarksFOBudImpM);
            }
        }

        private void PumpXlsRow(DataRow row, int refDate)
        {
            int refMarks = PumpMarks(row);

            decimal planRep = CleanFactValue(row["PlanRep"].ToString()) * 1000;
            decimal factRep = CleanFactValue(row["FactRep"].ToString()) * 1000;
            decimal pace = CleanFactValue(row["Pace"].ToString());
            decimal cent = CleanFactValue(row["Cent"].ToString());
            decimal lastFactR = CleanFactValue(row["LastFactR"].ToString()) * 1000;
            decimal lastYFactR = CleanFactValue(row["LastYFactR"].ToString()) * 1000;

            object[] mapping = new object[] { "PlanRep", planRep, "FactRep", factRep, "Pace", pace, "Cent", cent,
                "LastFactR", lastFactR, "LastYFactR", lastYFactR, "RefMarks", refMarks, "RefYearDayUNV", refDate };

            if (reportDataType == ReportDataType.Monthly)
                PumpFactsMonthly(mapping);
            else
                PumpFactsWeekly(mapping);
        }

        private int GetDateRef(object sheet)
        {
            string value = excelHelper.GetCell(sheet, 3, 1).Value.Trim();
            int refDate =  CommonRoutines.ShortDateToNewDate(CommonRoutines.TrimLetters(value));
            // если 1 число, то ссылка на 00 число предыдущего месяца (только для ежемесячных отчетов)
            if ((reportDataType == ReportDataType.Monthly) && (refDate % 100 == 1))
            {
                refDate = CommonRoutines.DecrementDateWithLastDay(refDate);
                // убираем день из даты
                refDate = refDate - refDate % 100;
            }
            if (!deletedDateList.Contains(refDate) && !this.DeleteEarlierData)
            {
                string constr = string.Format("RefYearDayUNV = {0}", refDate);
                DirectDeleteFactData(new IFactTable[] { fctMarksFOBudImp, fctMarksFOBudImpM }, -1, this.SourceID, constr);
                deletedDateList.Add(refDate);
            }
            return refDate;
        }

        private int GetFirstRow()
        {
            if ((reportType == ReportType.Outcomes) && (this.DataSource.Year >= 2011))
                return 8;
            return 9;
        }

        private const string LAST_ROW_MARK_OUTCOMES = "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ";
        private int GetLastRow(object sheet)
        {
            if (reportType == ReportType.Incomes)
                return 26;
            for (int curRow = GetFirstRow(); ; curRow++)
            {
                string value = excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper();
                if (value.StartsWith(LAST_ROW_MARK_OUTCOMES))
                    return curRow;
            }
        }

        private object[] XLS_MAPPING_INCOMES_2010 = new object[] {
            "Name", 1, "PlanRep", 9, "FactRep", 11, "Pace", 12, "Cent", 13, "LastFactR", 7, "LastYFactR", 5 };
        private object[] XLS_MAPPING_INCOMES_2011 = new object[] {
            "Name", 1, "PlanRep", 4, "FactRep", 5, "Pace", 6, "Cent", 7, "LastFactR", 2, "LastYFactR", 3 };
        private object[] XLS_MAPPING_OUTCOMES = new object[] {
            "Name", 1, "PlanRep", 4, "FactRep", 5, "Pace", 6, "Cent", 7, "LastFactR", 3, "LastYFactR", 2 };
        private object[] GetXlsMapping()
        {
            if (reportType == ReportType.Incomes)
            {
                if (this.DataSource.Year >= 2011)
                    return XLS_MAPPING_INCOMES_2011;
                return XLS_MAPPING_INCOMES_2010;
            }
            return XLS_MAPPING_OUTCOMES;
        }

        private void PumpXLSSheetData(FileInfo file, object sheet)
        {
            parentMarksId = -1;
            int refDate = GetDateRef(sheet);
            int firstRow = GetFirstRow();
            int lastRow = GetLastRow(sheet);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, firstRow, lastRow, GetXlsMapping());
            for (int curRow = 0; curRow < dt.Rows.Count; curRow++)
            {
                try
                {
                    SetProgress(lastRow, curRow,
                        string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", curRow + firstRow, lastRow));

                    DataRow row = dt.Rows[curRow];
                    if (row["Name"].ToString() == string.Empty)
                        continue;

                    PumpXlsRow(row, refDate);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})",
                        curRow + firstRow, ex.Message), ex);
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

        private const string constMonthlyDirName = "Месячные данные по УБУиО";
        private const string constWeeklyDirName = "Еженедельные данные по УБУиО";
        private void PumpFiles(DirectoryInfo dir)
        {
            reportDataType = ReportDataType.Monthly;
            DirectoryInfo[] dirs = dir.GetDirectories(constMonthlyDirName);
            if (dirs.GetLength(0) > 0)
            {
                if (this.DeleteEarlierData)
                    DirectDeleteFactData(new IFactTable[] { fctMarksFOBudImp }, -1, this.SourceID, string.Empty);
                ProcessFilesTemplate(dirs[0], "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
            }

            reportDataType = ReportDataType.Weekly;
            dirs = dir.GetDirectories(constWeeklyDirName);
            if (dirs.GetLength(0) > 0)
            {
                if (this.DeleteEarlierData)
                    DirectDeleteFactData(new IFactTable[] { fctMarksFOBudImpM }, -1, this.SourceID, string.Empty);
                ProcessFilesTemplate(dirs[0], "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
                deletedDateList.Clear();
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
            CorrectFactTableSums(fctMarksFOBudImp, dsMarks.Tables[0], clsMarks, "RefMarks",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctMarksFOBudImpM, dsMarks.Tables[0], clsMarks, "RefMarks",
                f4nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            CorrectSumByHierarchy("YPlan", "PlanRep");
            CorrectSumByHierarchy("Fact", "FactRep");
            CorrectSumByHierarchy("LastFact", "LastFactR");
            CorrectSumByHierarchy("LastYFact", "LastYFactR");
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
