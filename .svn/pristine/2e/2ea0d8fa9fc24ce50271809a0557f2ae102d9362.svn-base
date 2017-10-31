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

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // ФО 2 мес отч скиф - формат XLS
    // только у новосиба, 2005 год

    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {

        #region поля

        private bool isMainForm;
        private int refRegionXlsNovosib = -1;
        private int refDateXlsNovosib;
        private decimal sumFactor = 1;

        #endregion поля

        #region закачка основной таблицы

        private void PumpMainFactRowXlsNovosib(DataTable dt, DataRow row, int budgetLevel, object[] clsMapping, int fieldIndex)
        {
            decimal yearPlanReport = Convert.ToDecimal(row[fieldIndex].ToString().Trim().PadLeft(1, '0')) * sumFactor;
            decimal monthPlanReport = Convert.ToDecimal(row[fieldIndex + 3].ToString().Trim().PadLeft(1, '0')) * sumFactor;
            decimal factReport = Convert.ToDecimal(row[fieldIndex + 6].ToString().Trim().PadLeft(1, '0')) * sumFactor;
            object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping, 
                new object[] { "YearPlanReport", yearPlanReport, "MonthPlanReport", monthPlanReport, 
                               "FactReport", factReport, "RefBdgtLevels", budgetLevel });
            PumpRow(dt, mapping);
        }

        private void PumpXlsNovosibIncomes(DataTable sheetData)
        {
            foreach (DataRow row in sheetData.Rows)
            {
                string code = row["Code"].ToString().Replace(" ", string.Empty);
                int refKD = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                    new object[] { "CodeStr", code, "Name", row["Name"].ToString(), "Kl", 0, "Kst", 0 });
                object[] clsMapping = new object[] { "RefKD", refKD, "RefYearDayUNV", refDateXlsNovosib,
                    "RefYearDayUNV", refDateXlsNovosib, "RefRegions", refRegionXlsNovosib, "RefMeansType", 1 };
                PumpMainFactRowXlsNovosib(dsMonthRepIncomes.Tables[0], row, 2, clsMapping, 2);
                PumpMainFactRowXlsNovosib(dsMonthRepIncomes.Tables[0], row, 3, clsMapping, 3);
                PumpMainFactRowXlsNovosib(dsMonthRepIncomes.Tables[0], row, 7, clsMapping, 4);
                if (code == "00089000000000000000")
                    return;
            }
        }

        private void PumpXlsNovosibOutcomesJan(DataTable sheetData)
        {
            int refEkr = PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, "0",
                new object[] { "Code", "0", "Name", constDefaultClsName});
            bool toPumpRow = false;
            foreach (DataRow row in sheetData.Rows)
            {
                string code = row["Code"].ToString().Replace(" ", string.Empty).TrimStart('0').PadLeft(1, '0'); 
                if (code == "100")
                    toPumpRow = true;
                if (code == "7900")
                    return;
                if (!toPumpRow)
                    continue;
                int refFkr = PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                    new object[] { "Code", code, "Name", row["Name"].ToString() });
                object[] clsMapping = new object[] { "RefFKR", refFkr, "RefEKR", refEkr, 
                    "RefYearDayUNV", refDateXlsNovosib, "RefRegions", refRegionXlsNovosib, "RefMeansType", 1 };
                PumpMainFactRowXlsNovosib(dsMonthRepOutcomes.Tables[0], row, 2, clsMapping, 2);
                PumpMainFactRowXlsNovosib(dsMonthRepOutcomes.Tables[0], row, 3, clsMapping, 3);
                PumpMainFactRowXlsNovosib(dsMonthRepOutcomes.Tables[0], row, 7, clsMapping, 4);
            }
        }

        private void PumpXlsNovosibOutcomesOct(DataTable sheetData)
        {
            PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, "0",
                new object[] { "Code", "0", "Name", constDefaultClsName });
            bool toPumpRow = false;
            foreach (DataRow row in sheetData.Rows)
            {
                string code = row["Code"].ToString().Replace(" ", string.Empty);
                if (code == "00096000000000000000")
                    toPumpRow = true;
                if (!toPumpRow)
                    continue;
                string fkrCode = code.Substring(3, 4).TrimStart('0').PadLeft(1, '0'); 
                int refFkr = PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, fkrCode,
                    new object[] { "Code", fkrCode, "Name", row["Name"].ToString() });

                string ekrCode = code.Substring(17, 3).TrimStart('0').PadLeft(1, '0');
                int refEkr = PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, ekrCode,
                    new object[] { "Code", ekrCode, "Name", row["Name"].ToString() });

                object[] clsMapping = new object[] { "RefFKR", refFkr, "RefEKR", refEkr, 
                    "RefYearDayUNV", refDateXlsNovosib, "RefRegions", refRegionXlsNovosib, "RefMeansType", 1 };
                PumpMainFactRowXlsNovosib(dsMonthRepOutcomes.Tables[0], row, 2, clsMapping, 2);
                PumpMainFactRowXlsNovosib(dsMonthRepOutcomes.Tables[0], row, 3, clsMapping, 3);
                PumpMainFactRowXlsNovosib(dsMonthRepOutcomes.Tables[0], row, 7, clsMapping, 4);
                if (code == "00098000000000000000")
                    return;
            }
        }

        private void PumpXlsNovosibOutcomes(DataTable sheetData)
        {
            if (this.DataSource.Month <= 9)
                PumpXlsNovosibOutcomesJan(sheetData);
            else
                PumpXlsNovosibOutcomesOct(sheetData);
        }

        private void PumpXlsNovosibDefProf(DataTable sheetData)
        {
            foreach (DataRow row in sheetData.Rows)
            {
                string code = row["Code"].ToString().Replace(" ", string.Empty);
                if (this.DataSource.Month <= 9)
                {
                    if (code != "7900")
                        continue;
                }
                else
                {
                    if (code != "00079000000000000000")
                        continue;
                }
                object[] clsMapping = new object[] { "RefYearDayUNV", refDateXlsNovosib, "RefRegions", 
                    refRegionXlsNovosib, "RefMeansType", 1 };
                PumpMainFactRowXlsNovosib(dsMonthRepDefProf.Tables[0], row, 2, clsMapping, 2);
                PumpMainFactRowXlsNovosib(dsMonthRepDefProf.Tables[0], row, 3, clsMapping, 3);
                PumpMainFactRowXlsNovosib(dsMonthRepDefProf.Tables[0], row, 7, clsMapping, 4);
                return;
            }
        }

        private void PumpXlsNovosibInnerFinSources(DataTable sheetData)
        {
            // блять ебота -  в 11 и 12 месяце нужно качать код "00050000000000000000" отдельно, 
            // так как порядок сбит совершенно и сортировке не поддается (заебало писать такую еботу, новосибирск - пидарасы)
            if (this.DataSource.Month >= 11)
                foreach (DataRow row in sheetData.Rows)
                {
                    string code = row["Code"].ToString().Replace(" ", string.Empty);
                    if (code != "00050000000000000000")
                        continue;
                    int refKif = PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                        new object[] { "CodeStr", code, "Name", row["Name"].ToString().Trim(), "Kl", 0, "Kst", 0 });
                    object[] clsMapping = new object[] { "RefSIF", refKif, "RefYearDayUNV", refDateXlsNovosib, 
                        "RefRegions", refRegionXlsNovosib, "RefMeansType", 1 };
                    PumpMainFactRowXlsNovosib(dsMonthRepInFin.Tables[0], row, 2, clsMapping, 2);
                    PumpMainFactRowXlsNovosib(dsMonthRepInFin.Tables[0], row, 3, clsMapping, 3);
                    PumpMainFactRowXlsNovosib(dsMonthRepInFin.Tables[0], row, 7, clsMapping, 4);
                }
            
            bool toPumpRow = false;
            foreach (DataRow row in sheetData.Rows)
            {
                string code = row["Code"].ToString().Replace(" ", string.Empty);
                string name = row["Name"].ToString().Trim();
                if (code == "00001010000000000000")
                    toPumpRow = true;
                if (!toPumpRow)
                    continue;
                if (name == string.Empty)
                    return;
                int refKif = PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                    new object[] { "CodeStr", code, "Name", name, "Kl", 0, "Kst", 0 });
                object[] clsMapping = new object[] { "RefSIF", refKif, "RefYearDayUNV", refDateXlsNovosib, 
                    "RefRegions", refRegionXlsNovosib, "RefMeansType", 1 };
                PumpMainFactRowXlsNovosib(dsMonthRepInFin.Tables[0], row, 2, clsMapping, 2);
                PumpMainFactRowXlsNovosib(dsMonthRepInFin.Tables[0], row, 3, clsMapping, 3);
                PumpMainFactRowXlsNovosib(dsMonthRepInFin.Tables[0], row, 7, clsMapping, 4);
                if (code == "00050000000000000000")
                    return;
            }
        }

        private void PumpXlsNovosibOuterFinSources(DataTable sheetData)
        {
            foreach (DataRow row in sheetData.Rows)
            {
                string code = row["Code"].ToString().Replace(" ", string.Empty);
                if (code != "00090000000000000000")
                    continue;
                int refKif = PumpCachedRow(srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, code,
                    new object[] { "CodeStr", code, "Name", row["Name"].ToString(), "Kl", 0, "Kst", 0 });
                object[] clsMapping = new object[] { "RefSOF", refKif, "RefYearDayUNV", refDateXlsNovosib, 
                    "RefRegions", refRegionXlsNovosib, "RefMeansType", 1 };
                PumpMainFactRowXlsNovosib(dsMonthRepOutFin.Tables[0], row, 2, clsMapping, 2);
                PumpMainFactRowXlsNovosib(dsMonthRepOutFin.Tables[0], row, 3, clsMapping, 3);
                PumpMainFactRowXlsNovosib(dsMonthRepOutFin.Tables[0], row, 7, clsMapping, 4);
                return;
            }
        }

        private int GetLastRow(object sheet, int firstRow)
        {
            for (int curRow = firstRow; ; curRow++)
            {
                string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value;
                if (cellValue.Trim() == string.Empty)
                    return curRow - 1;
            }
        }

        private const int MAIN_FIRST_ROW = 14;
        private object[] MAIN_XLS_MAPPING = new object[] { "Code", 1, "Name", 2, "Sum1", 3, "Sum2", 4, "Sum3", 5, "Sum4", 6, 
                                                           "Sum5", 7, "Sum6", 8, "Sum7", 9, "Sum8", 10, "Sum9", 11 };
        private void PumpMainForm(object sheet)
        {
            int lastRow = GetLastRow(sheet, MAIN_FIRST_ROW);
            DataTable sheetData = excelHelper.GetSheetDataTable(sheet, MAIN_FIRST_ROW, lastRow, MAIN_XLS_MAPPING);
            try
            {
                if (ToPumpBlock(Block.bIncomes))
                    PumpXlsNovosibIncomes(sheetData);
                if (ToPumpBlock(Block.bOutcomes))
                    PumpXlsNovosibOutcomes(sheetData);
                if (ToPumpBlock(Block.bDefProf))
                    PumpXlsNovosibDefProf(sheetData);
                if (ToPumpBlock(Block.bInnerFinSources))
                    PumpXlsNovosibInnerFinSources(sheetData);
                if (ToPumpBlock(Block.bOuterFinSources))
                    PumpXlsNovosibOuterFinSources(sheetData);
            }
            finally
            {
                sheetData.Clear();
            }
        }

        #endregion закачка основной таблицы

        #region закачка справочной таблицы

        private void PumpRefsFactRowXlsNovosib(DataTable dt, DataRow row, int budgetLevel, object[] clsMapping, int fieldIndex)
        {
            decimal factReport = Convert.ToDecimal(row[fieldIndex].ToString().Trim().PadLeft(1, '0')) * sumFactor;
            object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                new object[] { "FactReport", factReport, "RefBdgtLevels", budgetLevel });
            PumpRow(dt, mapping);
        }

        private void PumpXlsNovosibIncomesRefs(DataTable sheetData)
        {
            foreach (DataRow row in sheetData.Rows)
            {
                int kst = Convert.ToInt32(row["Kst"].ToString().PadLeft(1, '0'));
                if (kst != 1)
                    continue;
                string kl = row["Kl"].ToString().Trim();
                int refKvsr = PumpCachedRow(kvsrCache, dsKVSR.Tables[0], clsKVSR, kl,
                    new object[] { "Code", kl, "Name", row["Name"].ToString(), "Kl", 0, "Kst", kst });
                object[] clsMapping = new object[] { "RefKVSR", refKvsr, "RefYearDayUNV", refDateXlsNovosib, 
                    "RefRegions", refRegionXlsNovosib };
                PumpRefsFactRowXlsNovosib(dsMonthRepIncomesBooks.Tables[0], row, 2, clsMapping, 4);
                PumpRefsFactRowXlsNovosib(dsMonthRepIncomesBooks.Tables[0], row, 3, clsMapping, 5);
                PumpRefsFactRowXlsNovosib(dsMonthRepIncomesBooks.Tables[0], row, 7, clsMapping, 6);
            }
        }

        private void PumpXlsNovosibOutcomesRefsAdd(DataTable sheetData)
        {
            foreach (DataRow row in sheetData.Rows)
            {
                int kst = Convert.ToInt32(row["Kst"].ToString().PadLeft(1, '0'));
                if ((kst < 2) || (kst > 75))
                    continue;
                int kl = Convert.ToInt32(row["Kl"].ToString().PadLeft(1, '0'));
                string code = row["Code"].ToString();
                string longCode = string.Format("{0}{1}{2}", code, kl, kst);
                string name = row["Name"].ToString();
                if ((kl == 0) && (kst == 0))
                    name = constDefaultClsName;
                int refMarksOutcomes = PumpCachedRow(marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes, longCode,
                    new object[] { "LongCode", longCode, "Name", name, "Kl", 0, "Kst", kst, 
                    "FKR", code, "EKR", kl, "KVR", 0, "KCSR", 0 });
                object[] clsMapping = new object[] { "RefMarksOutcomes", refMarksOutcomes, "RefYearDayUNV", refDateXlsNovosib, 
                    "RefRegions", refRegionXlsNovosib };
                PumpRefsFactRowXlsNovosib(dsMonthRepOutcomesBooksEx.Tables[0], row, 2, clsMapping, 4);
                PumpRefsFactRowXlsNovosib(dsMonthRepOutcomesBooksEx.Tables[0], row, 3, clsMapping, 5);
                PumpRefsFactRowXlsNovosib(dsMonthRepOutcomesBooksEx.Tables[0], row, 7, clsMapping, 6);
            }
        }

        private void PumpXlsNovosibInnerFinSourcesRefs(DataTable sheetData)
        {
            foreach (DataRow row in sheetData.Rows)
            {
                int kst = Convert.ToInt32(row["Kst"].ToString().PadLeft(1, '0'));
                if ((kst < 76) || (kst > 124))
                    continue;
                int kl = Convert.ToInt32(row["Kl"].ToString().PadLeft(1, '0'));
                string code = row["Code"].ToString().Trim().Replace(" ", string.Empty);
                string longCode = string.Format("{0}{1}{2}", code, kl, kst);
                int refMarksInDebt = PumpCachedRow(scrInFinSourcesRefCache, dsMarksInDebt.Tables[0], clsMarksInDebt, longCode,
                    new object[] { "LongCode", longCode, "SrcInFin", code, "GvrmInDebt", 0, 
                        "Name", row["Name"].ToString(), "Kl", 0, "Kst", kst });
                object[] clsMapping = new object[] { "RefMarksInDebt", refMarksInDebt, "RefYearDayUNV", refDateXlsNovosib, 
                    "RefRegions", refRegionXlsNovosib };
                PumpRefsFactRowXlsNovosib(dsMonthRepInDebtBooks.Tables[0], row, 2, clsMapping, 4);
                PumpRefsFactRowXlsNovosib(dsMonthRepInDebtBooks.Tables[0], row, 3, clsMapping, 5);
                PumpRefsFactRowXlsNovosib(dsMonthRepInDebtBooks.Tables[0], row, 7, clsMapping, 6);
            }
        }

        private void PumpXlsNovosibArearsRefs(DataTable sheetData)
        {
            foreach (DataRow row in sheetData.Rows)
            {
                int kst = Convert.ToInt32(row["Kst"].ToString().PadLeft(1, '0'));
                if ((kst < 136) || (kst > 170))
                    continue;
                int kl = Convert.ToInt32(row["Kl"].ToString().PadLeft(1, '0'));
                string code = row["Code"].ToString();
                string longCode = string.Format("{0}{1}{2}", code, kl, kst);
                int refMarksArrears = PumpCachedRow(arrearsCache, dsMarksArrears.Tables[0], clsMarksArrears, longCode,
                    new object[] { "LongCode", longCode, "Name", row["Name"].ToString(), "Kl", 0, "Kst", kst, 
                    "FKR", code, "EKR", kl, "KVR", 0, "KCSR", 0 });
                object[] clsMapping = new object[] { "RefMarksArrears", refMarksArrears, "RefYearDayUNV", refDateXlsNovosib, 
                    "RefRegions", refRegionXlsNovosib };
                PumpRefsFactRowXlsNovosib(dsMonthRepArrearsBooks.Tables[0], row, 2, clsMapping, 4);
                PumpRefsFactRowXlsNovosib(dsMonthRepArrearsBooks.Tables[0], row, 3, clsMapping, 5);
                PumpRefsFactRowXlsNovosib(dsMonthRepArrearsBooks.Tables[0], row, 7, clsMapping, 6);
            }
        }

        private const int REFS_FIRST_ROW = 13;
        private object[] REFS_XLS_MAPPING = new object[] { "Name", 1, "Code", 2, "Kl", 3, "Kst", 4, "Sum1", 5, "Sum2", 6, "Sum3", 7 };
        private void PumpRefsForm(object sheet)
        {
            int lastRow = GetLastRow(sheet, REFS_FIRST_ROW);
            DataTable sheetData = excelHelper.GetSheetDataTable(sheet, REFS_FIRST_ROW, lastRow, REFS_XLS_MAPPING);
            try
            {
                if (ToPumpBlock(Block.bIncomesRefs))
                    PumpXlsNovosibIncomesRefs(sheetData);
                if (ToPumpBlock(Block.bOutcomesRefsAdd))
                    PumpXlsNovosibOutcomesRefsAdd(sheetData);
                if (ToPumpBlock(Block.bInnerFinSourcesRefs))
                    PumpXlsNovosibInnerFinSourcesRefs(sheetData);
                if (ToPumpBlock(Block.bArrearsRefs))
                    PumpXlsNovosibArearsRefs(sheetData);
            }
            finally
            {
                sheetData.Clear();
            }
        }

        #endregion закачка справочной таблицы

        #region общая организация закачки

        private void PumpXlsNovosibRegions()
        {
            string code = "51".PadLeft(10, '0');
            string name = "Новосибирская область";
            string key = string.Format("{0}|{1}", code, name);
            object[] mapping = new object[] { "CodeStr", code, "Name", name, "BudgetKind", "КБС", "BudgetName", "Консолидированный бюджет субъекта" };
            refRegionXlsNovosib = PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, key, mapping);
        }

        private void PumpXlsNovosibFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            excelObj = excelHelper.OpenExcel(false);
            object workbook = excelHelper.GetWorkbook(excelObj, file.FullName, true);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                if (isMainForm)
                    PumpMainForm(sheet);
                else
                    PumpRefsForm(sheet);
            }
            finally
            {
                if (excelHelper != null)
                {
                    excelHelper.SetDisplayAlert(excelObj, false);
                    excelHelper.CloseWorkBooks(excelObj);
                    excelHelper.CloseExcel(ref excelObj);
                    excelHelper.Close();
                }
                GC.GetTotalMemory(true);
            }
        }

        protected override void PumpXlsNovosibReports(DirectoryInfo dir)
        {
            if (this.DataSource.Year != 2005)
                throw new PumpDataFailedException("Закачка месячной отчетности СКИФ формата XLS предназначена только для 2005 года.");
            if (this.DataSource.Month <= 9)
                sumFactor = 1000;
            else
                sumFactor = 1;
            PumpXlsNovosibRegions();
            refDateXlsNovosib = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            isMainForm = true;
            ProcessFilesTemplate(dir, "*отчет*.xls", new ProcessFileDelegate(PumpXlsNovosibFile), false);
            isMainForm = false;
            ProcessFilesTemplate(dir, "*справочн*.xls", new ProcessFileDelegate(PumpXlsNovosibFile), false);
            UpdateData();
        }

        #endregion общая организация закачки

    }

}
