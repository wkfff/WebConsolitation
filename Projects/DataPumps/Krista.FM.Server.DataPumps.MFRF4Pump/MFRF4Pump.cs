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

namespace Krista.FM.Server.DataPumps.MFRF4Pump
{

    // МФРМ 4 - РАСХОДЫ ФБ_ГРБС
    public class MFRF4PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // МФ РФ.ГРБС ФБ (d_MFRF_GRBS)
        private IDbDataAdapter daGrbs;
        private DataSet dsGrbs;
        private IClassifier clsGrbs;
        private Dictionary<string, int> cacheGrbs = null;

        #endregion Классификаторы

        #region Факты

        // МФ РФ.Расходы ФБ (f_MFRF_ExpensFB)
        private IDbDataAdapter daMFRF4;
        private DataSet dsMFRF4;
        private IFactTable fctMFRF4;

        #endregion Факты

        private ExcelHelper excelHelper = null;
        private object excelObj = null;
        private List<int> deletedDateList = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daGrbs, ref dsGrbs, clsGrbs);
            InitFactDataSet(ref daMFRF4, ref dsMFRF4, fctMFRF4);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheGrbs, dsGrbs.Tables[0], "Code");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daGrbs, dsGrbs, clsGrbs);
            UpdateDataSet(daMFRF4, dsMFRF4, fctMFRF4);
        }

        private const string D_GRBS_GUID = "0c9e66d7-2570-4049-90a9-437bbc37c245";
        private const string F_MFRF4_GUID = "3d9f02d5-1836-48b5-b29c-cf3c3e84ebf5";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { clsGrbs = this.Scheme.Classifiers[D_GRBS_GUID] };
            this.UsedFacts = new IFactTable[] { fctMFRF4 = this.Scheme.FactTables[F_MFRF4_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMFRF4);
            ClearDataSet(ref dsGrbs);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        private void PumpXlsRow(decimal fact, decimal plan, int refDate, int refGrbs, int refCosgu)
        {
            object[] mapping = new object[] { "Fact", fact, "Plane", plan, "RefCOSGU", refCosgu, "RefGRBS", refGrbs, "RefYearDayUNV", refDate };
            PumpRow(dsMFRF4.Tables[0], mapping);
            if (dsMFRF4.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMFRF4, ref dsMFRF4);
            }
        }

        private const int PLAN_COLUMN_2008 = 4;
        private const int PLAN_COLUMN_2009 = 3;
        private int GetPlanColumn()
        {
            int planColumn = PLAN_COLUMN_2008;
            if (this.DataSource.Year >= 2009)
                planColumn = PLAN_COLUMN_2009;
            return planColumn;
        }

        private const int TOTAL_ROW_2008 = 7;
        private const int TOTAL_ROW_2009 = 6;
        private int GetTotalRow()
        {
            int totalRow = TOTAL_ROW_2008;
            if (this.DataSource.Year >= 2009)
                totalRow = TOTAL_ROW_2009;
            return totalRow;
        }

        private decimal GetPlan(object sheet, int curRow)
        {
            int planColumn = GetPlanColumn();
            return Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, planColumn).Value.Trim().PadLeft(1, '0'));
        }

        private int PumpGrbs(string code, string name)
        {
            code = code.TrimStart('0').PadLeft(1, '0');
            return PumpCachedRow(cacheGrbs, dsGrbs.Tables[0], clsGrbs,
                code, new object[] { "Code", code, "Name", name });
        }

        private void DeleteDataByDate(int refDate)
        {
            if (!deletedDateList.Contains(refDate))
            {
                if (!this.DeleteEarlierData)
                    DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }
        }

        private void PumpXlsSheet(object sheet, string fileName)
        {
            int curRow = GetTotalRow() + 1;
            int titleRow = GetTotalRow() - 1;
            int refGrbs = -1;
            for (; ; curRow++)
                try
                {
                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    if (cellValue == string.Empty)
                        return;
                    // в 2008 году закачиваем грбс если в столбце C нет значения, а в столбце B есть
                    // факты по этой строчке не закачиваем
                    if (this.DataSource.Year < 2009)
                    {
                        if ((excelHelper.GetCell(sheet, curRow, 2).Value.Trim() != string.Empty) &&
                            (excelHelper.GetCell(sheet, curRow, 3).Value.Trim() == string.Empty))
                        {
                            refGrbs = PumpGrbs(excelHelper.GetCell(sheet, curRow, 2).Value.Trim(),
                                               excelHelper.GetCell(sheet, curRow, 1).Value.Trim());
                            continue;
                        }
                    }
                    else
                    {
                        refGrbs = PumpGrbs(excelHelper.GetCell(sheet, curRow, 2).Value.Trim(),
                                           excelHelper.GetCell(sheet, curRow, 1).Value.Trim());
                    }
                    decimal plan = GetPlan(sheet, curRow) * 1000;
                    int refCosgu = 2;
                    if (this.DataSource.Year < 2009)
                        if (excelHelper.GetCell(sheet, curRow, 3).Value.Trim() == "211")
                            refCosgu = 1;
                    // обходим колонки и закачиваем суммы, если наименование колонки содержит дату (Кассовое исполнение на хх.хх.хх)
                    int curColumn = GetPlanColumn() + 1;
                    for (; ; curColumn++)
                    {
                        string columnName = excelHelper.GetCell(sheet, titleRow, curColumn).Value.Trim();
                        if (columnName == string.Empty)
                            break;
                        if (!columnName.ToUpper().StartsWith("КАССОВОЕ ИСПОЛНЕНИЕ"))
                            continue;
                        string date = CommonRoutines.TrimLetters(columnName);
                        int refDate = CommonRoutines.ShortDateToNewDate(date) / 100 * 100 + 1;
                        refDate = CommonRoutines.DecrementDate(refDate);
                        DeleteDataByDate(refDate);
                        decimal fact = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, curColumn).Value.Trim().PadLeft(1, '0')) * 1000;
                        if ((plan == 0) && (fact == 0))
                            continue;
                        PumpXlsRow(fact, plan, refDate, refGrbs, refCosgu);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, fileName, ex.Message), ex);
                }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, "январь");
                if (sheet == null)
                    return;
                PumpXlsSheet(sheet, file.FullName);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
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
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }

}
