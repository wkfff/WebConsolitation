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

namespace Krista.FM.Server.DataPumps.SKIFYearRepPump
{
    // закачка базы овионта
    public partial class SKIFYearRepPumpModule : SKIFRepPumpModuleBase
    {

        #region поля 

        Database oviontDb = null;

        #endregion поля

        #region закачка овионт клс

        private delegate void ProcessOviontClsRowDelegate(DataRow dataRow);

        private void PumpIncomesClsOviont(DataRow row)
        {
            string codeStr = row["KBK"].ToString().Trim();
            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, codeStr,
                new object[] { "CodeStr", codeStr, "Name", row["Name1"].ToString().Trim(), 
                               "KL", row["KL"].ToString().Trim(), "KST", row["KST"].ToString().Trim() });
        }

        private void PumpSrcFinClsOviont(DataRow row)
        {
            string codeStr = row["KBK"].ToString().Trim();
            PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, codeStr,
                new object[] { "CodeStr", codeStr, "Name", row["Name1"].ToString().Trim(), 
                               "KL", row["KL"].ToString().Trim(), "KST", row["KST"].ToString().Trim() });
        }

        private void PumpEkrClsOviont(DataRow row)
        {
            string code = row["KBK"].ToString().Trim().Substring(14, 3).TrimStart('0').PadLeft(1, '0');
            PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, code,
                new object[] { "Code", code, "Name", row["Name1"].ToString().Trim() });
        }

        private void PumpFkrClsOviont1(DataRow row)
        {
            string code = row["KBK"].ToString().Trim().Substring(0, 4).TrimStart('0').PadLeft(1, '0');
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", row["Name1"].ToString().Trim() });
        }

        private void PumpFkrClsOviont2(DataRow row)
        {
            string code = row["code"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", row["Name"].ToString().Trim() });
        }

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

        private void PumpClsOviont()
        {
            PumpOviontTable("select ter, cu_name, type from budget_ter where status < 3", 
                new ProcessOviontClsRowDelegate(PumpRegionClsOviont));
            if (toPumpIncomes)
                PumpOviontTable("select KBK, Name1, KL, KST from BUDGET_REPORT_Mo where TYPE = 428 and PART = 1", 
                    new ProcessOviontClsRowDelegate(PumpIncomesClsOviont));
            if (toPumpFinSources)
                PumpOviontTable("select KBK, Name1, KL, KST from BUDGET_REPORT_Mo where TYPE = 428 and PART = 3", 
                    new ProcessOviontClsRowDelegate(PumpSrcFinClsOviont));
            if (toPumpOutcomes)
            {
                PumpOviontTable("select KBK, Name1 from BUDGET_REPORT_Mo where type = 428 and (part > 9 and part <> 97)",
                    new ProcessOviontClsRowDelegate(PumpEkrClsOviont));
                PumpOviontTable("select KBK, Name1 from BUDGET_REPORT_Mo where type = 428 and (part > 9 and part <> 97) and KBK > '11050000000000'",
                    new ProcessOviontClsRowDelegate(PumpFkrClsOviont1));
                PumpOviontTable("select Code, Name from BUDGET_KFSR where code <= 1105 and code >= 100",
                    new ProcessOviontClsRowDelegate(PumpFkrClsOviont2));
            }
        }

        #endregion закачка овионт клс

        #region закачка овионт факт

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

        private int GetRegionRefOviont(string code)
        {
            foreach (KeyValuePair<string, int> item in regionCache)
                if (item.Key.Split('|')[0] == code)
                    return item.Value;
            return -1;
        }

        private void PumpFactRowOviontBudLevel(DataRow row, object[] refMapping, object[] sumMapping, int budLevel, 
            ref DataSet ds, IDbDataAdapter da)
        {
            if (!GetSumMappingOviont(row, ref sumMapping))
                return;
            int meansType = 1;
            if (row["Type"].ToString().Trim() == "528")
                meansType = 2;
            int dateRep = this.DataSource.Year * 10000 + 1;
            int refRegions = GetRegionRefOviont(row["Ter"].ToString().PadLeft(10, '0'));
            object[] mapping = (object[])CommonRoutines.ConcatArrays(sumMapping, refMapping,
                new object[] { "RefMeansType", meansType, "RefYearDayUNV", dateRep, "RefBdgtLevels", budLevel, "RefRegions", refRegions });
            PumpRow(ds.Tables[0], mapping);
            if (ds.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT * 2)
            {
                UpdateData();
                ClearDataSet(da, ref ds);
            }
        }

        private void PumpFactRowOviont(DataRow row, object[] refMapping, ref DataSet ds, IDbDataAdapter da)
        {
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S1", "PerformedReport", "S9" }, 1, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S2", "PerformedReport", "S10" }, 2, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S3", "PerformedReport", "S11" }, 3, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S4", "PerformedReport", "S12" }, 11, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S5", "PerformedReport", "S13" }, 4, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S6", "PerformedReport", "S14" }, 5, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S7", "PerformedReport", "S15" }, 6, ref ds, da);
            PumpFactRowOviontBudLevel(row, refMapping, new object[] { "AssignedReport", "S8", "PerformedReport", "S16" }, 8, ref ds, da);
        }

        private void PumpYearRepDefProfOviont(DataRow row)
        {
            PumpFactRowOviont(row, new object[] { }, ref dsFOYRDefProf, daFOYRDefProf);
        }

        private void PumpYearRepIncomesOviont(DataRow row)
        {
            string kbk = row["kbk"].ToString();
            if (kbk.StartsWith("998") || kbk.StartsWith("999"))
                return;
            int refKd = FindCachedRow(kdCache, kbk, -1);
            PumpFactRowOviont(row, new object[] { "RefKD", refKd }, ref dsFOYRIncomes, daFOYRIncomes);
        }

        private void PumpYearRepSrcFinOviont(DataRow row)
        {
            string kbk = row["kbk"].ToString();
            int refKif = FindCachedRow(kifCache, kbk, -1);
            PumpFactRowOviont(row, new object[] { "RefKIF2005", refKif, "RefKIF2004", nullKIF2004 }, ref dsFOYRSrcFin, daFOYRSrcFin);
        }

        private void PumpYearRepOutcomesOviont(DataRow row)
        {
            string kbk = row["kbk"].ToString();
            int refEkr = FindCachedRow(ekrCache, kbk.Substring(14, 3).TrimStart('0').PadLeft(1, '0'), -1);
            int refFkr = FindCachedRow(fkrCache, kbk.Substring(0, 4).TrimStart('0').PadLeft(1, '0'), -1);
            int refKvsr = PumpCachedRow(kvsrCache, dsKvsr.Tables[0], clsKvsr, "0", 
                new object[] {"Code", "0", "Name", constDefaultClsName });
            int refKcsr = PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, "0",
                new object[] { "Code", "0", "Name", constDefaultClsName });
            int refKvr = PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, "0",
                new object[] { "Code", "0", "Name", constDefaultClsName });
            PumpFactRowOviont(row, new object[] { "RefEKRFOYR", refEkr, "RefFKR", refFkr, 
                "RefKVSRYR", refKvsr, "RefKCSR", refKcsr, "refKvr", refKvr }, ref dsFOYROutcomes, daFOYROutcomes);
        }

        private void PumpFactOviont()
        {
            string dateRep = string.Format("'{0}0101'", this.DataSource.Year + 1);
            if (toPumpDefProf)
                PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                                "m.s13, m.s14, m.s15, m.s16, m.type, t.Ter " +
                                " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                                "where (m.type = 428 or m.type = 528) and m.part = 97 and m.date_rep = " + dateRep,
                    new ProcessOviontClsRowDelegate(PumpYearRepDefProfOviont));
            if (toPumpIncomes)
                PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                                "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, t.Ter " +
                                " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                                "where (m.type = 428 or m.type = 528) and m.part = 1 and m.date_rep = " + dateRep,
                    new ProcessOviontClsRowDelegate(PumpYearRepIncomesOviont));
            if (toPumpFinSources)
                PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                                "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, t.Ter " +
                                " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                                "where (m.type = 428 or m.type = 528) and m.part = 3 and m.date_rep = " + dateRep,
                    new ProcessOviontClsRowDelegate(PumpYearRepSrcFinOviont));
            if (toPumpOutcomes)
                PumpOviontTable("select m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.s10, m.s11, m.s12, " +
                                "m.s13, m.s14, m.s15, m.s16, m.type, m.Kbk, t.Ter " +
                                " from BUDGET_REPORT_Mo m left join BUDGET_TER t on (t.TER_ID = m.Terri) " +
                                "where (m.type = 428 or m.type = 528) and (m.part > 9 and m.part <> 97) and m.date_rep = " + dateRep,
                    new ProcessOviontClsRowDelegate(PumpYearRepOutcomesOviont));
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

            toPumpIncomes = ToPumpBlock(Block.bIncomes);
            toPumpOutcomes = ToPumpBlock(Block.bOutcomes);
            toPumpDefProf = ToPumpBlock(Block.bDefProf);
            toPumpFinSources = ToPumpBlock(Block.bFinSources);
            toPumpNet = ToPumpBlock(Block.bNet);

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
