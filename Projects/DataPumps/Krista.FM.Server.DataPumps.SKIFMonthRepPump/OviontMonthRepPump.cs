using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Data.Common;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // закачка из базы овионта
    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {

        #region поля

        private Database oviontDb = null;
        private string oviontDateRep = string.Empty;
        private int sourceDateOviont = 0;

        #endregion поля

        #region закачка овионт клс

        private void PumpOviontTable(string query, ProcessOviontClsRowDelegate pocr)
        {
            DataTable dt = (DataTable)oviontDb.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            try
            {
                foreach (DataRow row in dt.Rows)
                    pocr(row);
            }
            finally
            {
                dt.Clear();
                dt = null;
            }
            UpdateData();
        }

        private delegate void ProcessOviontClsRowDelegate(DataRow dataRow);

        private void PumpRegionClsOviont(DataRow row)
        {
            string regionCode = row["ter"].ToString().Trim().PadLeft(10, '0');
            string regionName = row["cu_name"].ToString().Trim();
            string key = string.Format("{0}|{1}", regionCode, regionName);
            string type = row["type"].ToString().Trim();
            string budgetName = "Неуказанный бюджет";
            int refDocType = 1;
            if (type == "4")
            {
                budgetName = "Бюджет городского округа";
                refDocType = 7;
            }
            else if (type == "5")
            {
                budgetName = "Бюджет муниципального района";
                refDocType = 6;
            }
            else if (type == "2")
            {
                budgetName = "Бюджет субъекта РФ";
                refDocType = 5;
            }
            else if (type == "10")
            {
                budgetName = "Бюджет поселения";
                refDocType = 8;
            }
            if (type == string.Empty)
                type = "0";
            PumpCachedRow(region4PumpCache, dsRegions4Pump.Tables[0], clsRegions4Pump, key,
                new object[] { "CodeStr", regionCode, "Name", regionName, "RefDocType", refDocType, "SOURCEID", regForPumpSourceID });
            PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, key,
                new object[] { "CodeStr", regionCode, "Name", regionName, "BudgetKind", type, "BudgetName", budgetName });
        }

        private void PumpIncomesClsOviont(DataRow row)
        {
            string codeStr = row["KBK"].ToString().Trim();
            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, codeStr,
                new object[] { "CodeStr", codeStr, "Name", name, 
                               "KL", row["KL"].ToString().Trim(), "KST", row["KST"].ToString().Trim() });
        }

        private void PumpEkrClsOviont(DataRow row)
        {
            string code = row["KBK"].ToString().Trim().Substring(14, 3).TrimStart('0').PadLeft(1, '0');
            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, code,
                new object[] { "Code", code, "Name", name });
        }

        private void PumpFkrClsOviont1(DataRow row)
        {
            string code = row["KBK"].ToString().Trim().Substring(0, 14).TrimStart('0').PadLeft(1, '0');
            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", name });
        }

        private void PumpFkrClsOviont2(DataRow row)
        {
            string code = row["code"].ToString().Trim().TrimStart('0').PadLeft(1, '0') + "0000000000";
            string name = row["Name"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", name });
        }

        private void PumpKvsrClsOviont(DataRow row)
        {
            string code = row["Code_mask"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
            string name = row["Name"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            PumpCachedRow(kvsrCache, dsKVSR.Tables[0], clsKVSR, code,
                new object[] { "Code", code, "Name", name, "Kl", 0, "Kst", "0" });
        }

        private void PumpSrcInFinClsOviont(DataRow row)
        {
            string codeStr = row["KBK"].ToString().Trim();
            string name = row["Name1"].ToString().Trim();
            string kst = row["KST"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
            if (name == string.Empty)
                name = constDefaultClsName;
            string key = string.Format("{0}|{1}", codeStr, kst);
            PumpCachedRow(srcInFinWithKstCache, dsSrcInFin.Tables[0], clsSrcInFin, key,
                new object[] { "CodeStr", codeStr, "Name", name, "KL", row["KL"].ToString().Trim(), "KST", kst });
        }

        private void PumpSrcOutFinClsOviont(DataRow row)
        {
            string codeStr = row["KBK"].ToString().Trim();
            string name = row["Name1"].ToString().Trim();
            string kst = row["KST"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
            if (name == string.Empty)
                name = constDefaultClsName;
            string key = string.Format("{0}|{1}", codeStr, kst);
            PumpCachedRow(srcOutFinWithKstCache, dsSrcOutFin.Tables[0], clsSrcOutFin, key,
                new object[] { "CodeStr", codeStr, "Name", name, "KL", row["KL"].ToString().Trim(), "KST", kst });
        }

        private void PumpClsOviont()
        {
            PumpOviontTable("select ter, cu_name, type from budget_ter where status < 3",
                new ProcessOviontClsRowDelegate(PumpRegionClsOviont));
            if (toPumpIncomes)
                PumpOviontTable("select KBK, Name1, KL, KST from BUDGET_REPORT_Mo where TYPE = 128 and PART = 1 and date_rep = " + oviontDateRep,
                    new ProcessOviontClsRowDelegate(PumpIncomesClsOviont));
            if (toPumpOutcomes)
            {
                PumpOviontTable("select KBK, Name1 from BUDGET_REPORT_Mo where type = 128 and (part > 9 and part <> 97) and date_rep = " + oviontDateRep,
                    new ProcessOviontClsRowDelegate(PumpEkrClsOviont));
                PumpOviontTable("select KBK, Name1 from BUDGET_REPORT_Mo where type = 128 and (part > 9 and part <> 97) and KBK > '11050000000000' and date_rep = " + oviontDateRep,
                    new ProcessOviontClsRowDelegate(PumpFkrClsOviont1));
                PumpOviontTable("select Code, Name from BUDGET_KFSR where code <= 1105 and code >= 100",
                    new ProcessOviontClsRowDelegate(PumpFkrClsOviont2));
                PumpOviontTable("select Code_mask, Name from BUDGET_KVSR",
                    new ProcessOviontClsRowDelegate(PumpKvsrClsOviont));
            }
            if (toPumpInnerFinSources)
                PumpOviontTable("select KBK, Name1, KL, KST from BUDGET_REPORT_Mo where TYPE = 128 and PART = 3 and Kst <> 620 and date_rep = " + oviontDateRep,
                    new ProcessOviontClsRowDelegate(PumpSrcInFinClsOviont));
            if (toPumpOuterFinSources)
                PumpOviontTable("select KBK, Name1, KL, KST from BUDGET_REPORT_Mo where TYPE = 128 and PART = 3 and Kst = 620 and date_rep = " + oviontDateRep,
                    new ProcessOviontClsRowDelegate(PumpSrcOutFinClsOviont));
        }

        #endregion закачка овионт клс

        #region закачка овионт факт

        #region общие методы 

        private bool GetSumMappingOviont(DataRow row, ref object[] sumMapping)
        {
            bool zeroSums = true;
            for (int i = 0; i <= sumMapping.GetLength(0) - 1; i += 2)
            {
                object sum = row[sumMapping[i + 1].ToString()];
                if (sum == DBNull.Value)
                    sum = string.Empty;
                decimal sumDec = Convert.ToDecimal(sum.ToString().PadLeft(1, '0'));
                sumMapping[i + 1] = sumDec;
                if (sumDec != 0)
                    zeroSums = false;
            }
            return (!zeroSums);
        }

        private void PumpFactRowOviontBudLevel(DataRow row, object[] refMapping, object[] sumMapping, int budLevel,
            ref DataSet ds, IDbDataAdapter da)
        {
            string regKey = string.Format("{0}|{1}", row["Ter"].ToString().Trim().PadLeft(10, '0'), row["cu_name"].ToString().Trim());
            int refRegions = FindCachedRow(regionCache, regKey, -1);
            if (refRegions == -1)
                return;
            if (!GetSumMappingOviont(row, ref sumMapping))
                return;
            int meansType = 1;
            if ((row["Type"].ToString().Trim() == "228") || (row["Type"].ToString().Trim() == "227"))
                meansType = 2;
            int dateRep = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            object[] mapping = (object[])CommonRoutines.ConcatArrays(sumMapping, refMapping,
                new object[] { "RefMeansType", meansType, "RefYearDayUNV", dateRep, "RefBdgtLevels", budLevel, "RefRegions", refRegions });
            PumpRow(ds.Tables[0], mapping);
            if (ds.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT * 2)
            {
                UpdateData();
                ClearDataSet(da, ref ds);
            }
        }

        private void PumpFactRowOviont128(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S1", "FactReport", "S9" }, 1, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S2", "FactReport", "S10" }, 2, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S3", "FactReport", "S11" }, 3, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S4", "FactReport", "S12" }, 11, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S5", "FactReport", "S13" }, 4, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S6", "FactReport", "S14" }, 5, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S7", "FactReport", "S15" }, 6, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S8", "FactReport", "S16" }, 8, ref ds, da);
        }

        private void PumpFactRowOviont127(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S1", "FactReport", "S5", "SpreadFactYearPlanReport", "S6" }, 3, ref ds, da);
        }

        private void PumpFactRowOviont228(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S1", "FactReport", "S4", "SpreadFactYearPlanReport", "S7" }, 2, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S2", "FactReport", "S5", "SpreadFactYearPlanReport", "S8" }, 3, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S3", "FactReport", "S6", "SpreadFactYearPlanReport", "S9" }, 7, ref ds, da);
        }

        private void PumpFactRowOviont227(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "YearPlanReport", "S1", "FactReport", "S5", "SpreadFactYearPlanReport", "S6" }, 3, ref ds, da);
        }

        private void PumpFactRowOviont387(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S1", "FactReport", "S7" }, 2, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S2", "FactReport", "S8" }, 3, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S3", "FactReport", "S9" }, 11, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S4", "FactReport", "S10" }, 4, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S5", "FactReport", "S11" }, 5, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S6", "FactReport", "S12" }, 6, ref ds, da);
        }

        private void PumpFactRowOviont487(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S1", "FactReport", "S7" }, 12, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S2", "FactReport", "S8" }, 13, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S3", "FactReport", "S9" }, 17, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S4", "FactReport", "S10" }, 14, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S5", "FactReport", "S11" }, 15, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S6", "FactReport", "S12" }, 16, ref ds, da);
        }

        private void PumpFactRowOviont(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            string type = row["Type"].ToString();
            if (type == "128")
                PumpFactRowOviont128(row, refMapping, ref ds, da);
            else if (type == "127")
                PumpFactRowOviont127(row, refMapping, ref ds, da);
            else if (type == "228")
                PumpFactRowOviont228(row, refMapping, ref ds, da);
            else if (type == "227")
                PumpFactRowOviont227(row, refMapping, ref ds, da);
            else if (type == "387")
                PumpFactRowOviont387(row, refMapping, ref ds, da);
            else if (type == "487")
                PumpFactRowOviont487(row, refMapping, ref ds, da);
        }

        #endregion общие методы



        #region дефицит профицит

        private void PumpDefProfFactOviontRow(DataRow row)
        {
            PumpFactRowOviont(row, new object[] { }, ref dsMonthRepDefProf, daMonthRepDefProf);
        }

        private void PumpDefProfFactOviont()
        {
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.s13, m.s14, m.s15, m.s16, m.type, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 128 or m.type = 228) and m.part = 97 and m.date_rep = " + oviontDateRep,
                new ProcessOviontClsRowDelegate(PumpDefProfFactOviontRow));
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.s13, m.s14, m.s15, m.s16, m.type, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 127 or m.type = 227) and m.part = 2 and Kbk = '00079000000000000000' and m.date_rep = " + oviontDateRep,
                new ProcessOviontClsRowDelegate(PumpDefProfFactOviontRow));
        }

        #endregion дефицит профицит

        #region доходы

        private void PumpIncomesFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            if (kbk.StartsWith("998") || kbk.StartsWith("999"))
                return;
            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            int refKd = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kbk,
                new object[] { "CodeStr", kbk, "Name", name, 
                                "KL", row["KL"].ToString().Trim(), "KST", row["KST"].ToString().Trim() });
            PumpFactRowOviont(row, new object[] { "RefKD", refKd }, ref dsMonthRepIncomes, daMonthRepIncomes);
        }

        private void PumpIncomesFactOviont()
        {
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 128 or m.type = 127 or m.type = 228 or m.type = 227) and m.part = 1 and m.date_rep = " + oviontDateRep,
                new ProcessOviontClsRowDelegate(PumpIncomesFactOviontRow));
        }

        #endregion доходы

        #region расходы

        private void PumpOutcomesFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string fkrCode = string.Empty;
            string ekrCode = string.Empty;
            string kvsrCode = string.Empty;

            string type = row["Type"].ToString();
            if ((type == "128") || (type == "228"))
            {
                fkrCode = kbk.Substring(0, 14).TrimStart('0').PadLeft(1, '0');
                ekrCode = kbk.Substring(14, 3).TrimStart('0').PadLeft(1, '0');
                kvsrCode = "0";
            }
            else if ((type == "127") || (type == "227"))
            {
                fkrCode = kbk.Substring(3, 14).TrimStart('0').PadLeft(1, '0');
                ekrCode = kbk.Substring(17, 3).TrimStart('0').PadLeft(1, '0');
                kvsrCode = kbk.Substring(0, 3).TrimStart('0').PadLeft(1, '0');
            }

            int refFkr = PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, fkrCode,
                new object[] { "Code", fkrCode, "Name", constDefaultClsName });
            int refEkr = PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, ekrCode,
                new object[] { "Code", ekrCode, "Name", constDefaultClsName });
            int refKvsr = PumpCachedRow(kvsrCache, dsKVSR.Tables[0], clsKVSR, kvsrCode,
                new object[] { "Code", kvsrCode, "Name", constDefaultClsName, "Kl", 0, "Kst", 0 });

            PumpFactRowOviont(row, new object[] { "RefFKR", refFkr, "RefEKR", refEkr, "RefKVSR", refKvsr }, ref dsMonthRepOutcomes, daMonthRepOutcomes);
        }

        private void PumpOutcomesFactOviont()
        {
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 128 or m.type = 228) and (m.part > 9 and m.part <> 97) and m.date_rep = " + oviontDateRep,
                new ProcessOviontClsRowDelegate(PumpOutcomesFactOviontRow));
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 127 or m.type = 227) and (m.part = 2 and m.kbk <> '00079000000000000000') and m.date_rep = " + oviontDateRep,
                new ProcessOviontClsRowDelegate(PumpOutcomesFactOviontRow));
        }

        #endregion расходы

        #region источники внутр

        private void PumpInnerFinSourcesFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string name = row["Name1"].ToString().Trim();
            string kst = row["KST"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
            if (name == string.Empty)
                name = constDefaultClsName;
            string key = string.Format("{0}|{1}", kbk, kst);
            int refSif = PumpCachedRow(srcInFinWithKstCache, dsSrcInFin.Tables[0], clsSrcInFin, key,
                new object[] { "CodeStr", kbk, "Name", name, "KL", row["KL"].ToString().Trim(), "KST", kst });
            PumpFactRowOviont(row, new object[] { "RefSIF", refSif }, ref dsMonthRepInFin, daMonthRepInFin);
        }

        private void PumpInnerFinSourcesFactOviont()
        {
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 128 or m.type = 127 or m.type = 228 or m.type = 227) and m.part = 3 and m.kst <> 620 and m.date_rep = " + oviontDateRep,
                new ProcessOviontClsRowDelegate(PumpInnerFinSourcesFactOviontRow));
        }

        #endregion источники внутр

        #region источники внешн

        private void PumpOuterFinSourcesFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string name = row["Name1"].ToString().Trim();
            string kst = row["KST"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
            if (name == string.Empty)
                name = constDefaultClsName;
            string key = string.Format("{0}|{1}", kbk, kst);
            int refSof = PumpCachedRow(srcOutFinWithKstCache, dsSrcOutFin.Tables[0], clsSrcOutFin, key,
                new object[] { "CodeStr", kbk, "Name", name, "KL", row["KL"].ToString().Trim(), "KST", kst });
            PumpFactRowOviont(row, new object[] { "RefSOF", refSof }, ref dsMonthRepOutFin, daMonthRepOutFin);
        }

        private void PumpOuterFinSourcesFactOviont()
        {
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 128 or m.type = 127 or m.type = 228 or m.type = 227) and m.part = 3 and m.kst = 620 and m.date_rep = " + oviontDateRep,
                new ProcessOviontClsRowDelegate(PumpOuterFinSourcesFactOviontRow));
        }

        #endregion источники внешн



        #region источники внутр справ

        private void PumpInnerFinSourcesRefsFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string longCode = row["Kst"].ToString().PadLeft(22, '0');
            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            int refCls = PumpCachedRow(scrInFinSourcesRefCache, dsMarksInDebt.Tables[0], clsMarksInDebt, longCode,
                    new object[] { "LongCode", longCode, "Name", name, "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", row["Kl"].ToString(), "Kst", row["Kst"].ToString() });
            PumpFactRowOviont(row, new object[] { "RefMarksInDebt", refCls }, ref dsMonthRepInDebtBooks, daMonthRepInDebtBooks);
        }

        private void PumpInnerFinSourcesRefsFactOviont()
        {
            string kstConstr = string.Empty;
            if (sourceDateOviont >= 20100100)
                kstConstr = " and ((m.kst >= 9600 and m.kst < 9700) or (m.kst >= 9800 and m.kst < 9900) or (m.kst = 11600)) ";
            else if (sourceDateOviont >= 20090100)
                kstConstr = " and ((m.kst >= 9700 and m.kst < 9800) or (m.kst >= 9900 and m.kst < 10000)) ";
            else if (sourceDateOviont >= 20080100)
                kstConstr = " and ((m.kst >= 8200 and m.kst < 8300) or (m.kst >= 8400 and m.kst < 8500)) ";

            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 387 or m.type = 487) and m.date_rep = " + oviontDateRep + kstConstr,
                new ProcessOviontClsRowDelegate(PumpInnerFinSourcesRefsFactOviontRow));
        }

        #endregion источники внутр справ

        #region источники внешн справ

        private void PumpOuterFinSourcesRefsFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string longCode = row["Kst"].ToString().PadLeft(22, '0');
            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            int refCls = PumpCachedRow(scrOutFinSourcesRefCache, dsMarksOutDebt.Tables[0], clsMarksOutDebt, longCode,
                    new object[] { "LongCode", longCode, "Name", name, "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", row["Kl"].ToString(), "Kst", row["Kst"].ToString() });
            PumpFactRowOviont(row, new object[] { "RefMarksOutDebt", refCls }, ref dsMonthRepOutDebtBooks, daMonthRepOutDebtBooks);
        }

        private void PumpOuterFinSourcesRefsFactOviont()
        {
            string kstConstr = string.Empty;
            if (sourceDateOviont >= 20100100)
                kstConstr = " and (m.kst >= 9700 and m.kst < 9800) ";
            else if (sourceDateOviont >= 20090100)
                kstConstr = " and (m.kst >= 9800 and m.kst < 9900) ";
            else if (sourceDateOviont >= 20080100)
                kstConstr = " and (m.kst >= 8300 and m.kst < 8400) ";
            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 387 or m.type = 487) and m.date_rep = " + oviontDateRep + kstConstr,
                new ProcessOviontClsRowDelegate(PumpOuterFinSourcesRefsFactOviontRow));
        }

        #endregion источники внешн справ

        #region справ расходы доп

        private void PumpOutcomesRefsAddFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string kst = row["kst"].ToString().Trim();
            string fkr = kbk.Substring(3, 4);
            string kcsr = kbk.Substring(7, 7);
            string kvr = kbk.Substring(14, 3);
            string ekr = kbk.Substring(17, 3);
            string longCode = kst + fkr + kcsr + kvr + ekr;

            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            int refCls = PumpCachedRow(marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes, longCode,
                    new object[] { "LongCode", longCode, "Name", name, "FKR", fkr, "KCSR", kcsr, 
                                   "KVR", kvr, "EKR", ekr, "Kl", row["Kl"].ToString(), "Kst", kst });
            PumpFactRowOviont(row, new object[] { "RefMarksOutcomes", refCls }, ref dsMonthRepOutcomesBooksEx, daMonthRepOutcomesBooksEx);
        }

        private void PumpOutcomesRefsAddFactOviont()
        {
            string kstConstr = string.Empty;
            if (sourceDateOviont >= 20100100)
                kstConstr = " and ((m.kst >= 100 and m.kst < 9600) or (m.kst >= 11300 and m.kst < 11600)) ";
            else if (sourceDateOviont >= 20090400)
                kstConstr = " and ((m.kst >= 100 and m.kst < 9700) or (m.kst >= 11400 and m.kst < 19900)) ";
            else if (sourceDateOviont >= 20090100)
                kstConstr = " and (m.kst >= 100 and m.kst < 9700) ";
            else if (sourceDateOviont >= 20080100)
                kstConstr = " and (m.kst >= 100 and m.kst < 8200) ";

            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 387 or m.type = 487) and m.date_rep = " + oviontDateRep + kstConstr,
                new ProcessOviontClsRowDelegate(PumpOutcomesRefsAddFactOviontRow));
        }

        #endregion справ расходы доп

        #region справ задолженность

        private void PumpArrearsRefsFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string kst = row["kst"].ToString().Trim();
            string fkr = kbk.Substring(3, 4);
            string kcsr = kbk.Substring(7, 7);
            string kvr = kbk.Substring(14, 3);
            string ekr = kbk.Substring(17, 3);
            string longCode = kst + fkr + kcsr + kvr + ekr;

            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            int refCls = PumpCachedRow(arrearsCache, dsMarksArrears.Tables[0], clsMarksArrears, longCode,
                    new object[] { "LongCode", longCode, "Name", name, "FKR", fkr, "KCSR", kcsr, 
                                   "KVR", kvr, "EKR", ekr, "Kl", row["Kl"].ToString(), "Kst", kst });
            PumpFactRowOviont(row, new object[] { "RefMarksArrears", refCls }, ref dsMonthRepArrearsBooks, daMonthRepArrearsBooks);
        }

        private void PumpArrearsRefsFactOviont()
        {
            string kstConstr = string.Empty;
            if (sourceDateOviont >= 20100100)
                kstConstr = " and (m.kst >= 10000 and m.kst < 11300) ";
            else if (sourceDateOviont >= 20090400)
                kstConstr = " and (m.kst >= 10100 and m.kst < 11400) ";
            else if (sourceDateOviont >= 20090100)
                kstConstr = " and (m.kst >= 10100) ";
            else if (sourceDateOviont >= 20080100)
                kstConstr = " and (m.kst >= 8600) ";

            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 387 or m.type = 487) and m.date_rep = " + oviontDateRep + kstConstr,
                new ProcessOviontClsRowDelegate(PumpArrearsRefsFactOviontRow));
        }

        #endregion справ задолженность

        #region справ остатки

        private void PumpExcessRefsFactOviontRow(DataRow row)
        {
            string kbk = row["kbk"].ToString().Trim();
            string kst = row["kst"].ToString().Trim();
            string fkr = kbk.Substring(3, 4);
            string kcsr = kbk.Substring(7, 7);
            string kvr = kbk.Substring(14, 3);
            string ekr = kbk.Substring(17, 3);
            string longCode = kst + fkr + kcsr + kvr + ekr;

            string name = row["Name1"].ToString().Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            int refCls = PumpCachedRow(excessCache, dsMarksExcess.Tables[0], clsMarksExcess, longCode,
                    new object[] { "LongCode", longCode, "Name", name, "FKR", fkr, "KCSR", kcsr, 
                                   "KVR", kvr, "EKR", ekr, "Kl", row["Kl"].ToString(), "Kst", kst });
            PumpFactRowOviont(row, new object[] { "RefMarks", refCls }, ref dsMonthRepExcessBooks, daMonthRepExcessBooks);
        }

        private void PumpExcessRefsFactOviont()
        {
            string kstConstr = string.Empty;
            if (sourceDateOviont >= 20100100)
                kstConstr = " and (m.kst >= 9900 and m.kst < 10000) ";
            else if (sourceDateOviont >= 20090100)
                kstConstr = " and (m.kst >= 10000 and m.kst < 10100) ";
            else if (sourceDateOviont >= 20080100)
                kstConstr = " and (m.kst >= 8500 and m.kst < 8600) ";

            PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                            "m.type, m.Kbk, m.Name1, m.kl, m.kst, t.Ter, t.cu_name " +
                            " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                            "where (m.type = 387 or m.type = 487) and m.date_rep = " + oviontDateRep + kstConstr,
                new ProcessOviontClsRowDelegate(PumpExcessRefsFactOviontRow));
        }

        #endregion справ остатки

        private void PumpFactOviont()
        {
            if (toPumpDefProf)
                PumpDefProfFactOviont();
            if (toPumpIncomes)
                PumpIncomesFactOviont();
            if (toPumpOutcomes)
                PumpOutcomesFactOviont();
            if (toPumpInnerFinSources)
                PumpInnerFinSourcesFactOviont();
            if (toPumpOuterFinSources)
                PumpOuterFinSourcesFactOviont();

            if (toPumpInnerFinSourcesRefs)
                PumpInnerFinSourcesRefsFactOviont();
            if (toPumpOuterFinSourcesRefs)
                PumpOuterFinSourcesRefsFactOviont();
            if (toPumpOutcomesRefsAdd)
                PumpOutcomesRefsAddFactOviont();
            if (toPumpArrearsRefs)
                PumpArrearsRefsFactOviont();
            if (toPumpExcessRefs)
                PumpExcessRefsFactOviont();
        }

        #endregion закачка овионт факт

        #region Функции общей организации закачки блоков

        private void InitConnectionOviont(DirectoryInfo dir)
        {
            DbProviderFactory factory = new DbProviderFactoryWrapper(DbProviderFactories.GetFactory(ProviderFactoryConstants.SqlClient));
            ConnectionString connectionString = new ConnectionString();
            connectionString.ReadConnectionString(dir.FullName + "\\" + this.DataSource.Year.ToString() + ".UDL");
            DbConnection oviontConnection = factory.CreateConnection();
            oviontConnection.ConnectionString = connectionString.ToString();
            oviontDb = new Database(oviontConnection, factory, false, constCommandTimeout);
        }

        protected override void PumpOviont(DirectoryInfo dir)
        {
            if (dir.GetFiles("*.udl", SearchOption.AllDirectories).GetLength(0) == 0)
                return;

            toPumpIncomes = (ToPumpBlock(Block.bIncomes));
            toPumpOutcomes = (ToPumpBlock(Block.bOutcomes));
            toPumpDefProf = (ToPumpBlock(Block.bDefProf));
            toPumpInnerFinSources = (ToPumpBlock(Block.bInnerFinSources));
            toPumpOuterFinSources = (ToPumpBlock(Block.bOuterFinSources));
            toPumpAccount = (ToPumpBlock(Block.bAccount));
            toPumpIncomesRefs = (ToPumpBlock(Block.bIncomesRefs));
            toPumpOutcomesRefs = (ToPumpBlock(Block.bOutcomesRefs));
            toPumpOutcomesRefsAdd = (ToPumpBlock(Block.bOutcomesRefsAdd));
            toPumpInnerFinSourcesRefs = (ToPumpBlock(Block.bInnerFinSourcesRefs));
            toPumpOuterFinSourcesRefs = (ToPumpBlock(Block.bOuterFinSourcesRefs));
            toPumpArrearsRefs = (ToPumpBlock(Block.bArrearsRefs));
            toPumpExcessRefs = (ToPumpBlock(Block.bExcessRefs));

            int repYear = this.DataSource.Year;
            int repMonth = this.DataSource.Month;
            if (this.DataSource.Month == 12)
            {
                repYear += 1;
                repMonth = 0;
            }
            repMonth += 1;
            oviontDateRep = string.Format("'{0}'", (repYear * 10000 + repMonth * 100 + 01));
            sourceDateOviont = this.DataSource.Year * 10000 + this.DataSource.Month * 100;

            InitConnectionOviont(dir);
            try
            {
                WriteToTrace(string.Format("подключение к базе: {0}", oviontDb.Connection.ConnectionString), TraceMessageKind.Information);
                PumpClsOviont();
                PumpFactOviont();
            }
            finally
            {
                if (oviontDb != null)
                {
                    oviontDb.Dispose();
                    oviontDb = null;
                }

            }
        }

        #endregion Функции общей организации закачки блоков

    }
}
