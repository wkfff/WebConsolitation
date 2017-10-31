using System;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using System.Data;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Balance
{
    public class BalanceServer
    {
        public static DataTable GetBalanceTable(int sourceID, int year, int currentVariant, int incomesVariant, int outcomesVariant)
        {
            DataTable dt = GetBalanceTable();
            DataRow newRow = dt.NewRow();
            newRow["SourceID"] = sourceID;
            newRow["CurrentVariant"] = currentVariant;
            newRow["IncomesVariant"] = incomesVariant;
            newRow["OutcomesVariant"] = outcomesVariant;
            newRow["Incomes"] = GetIncomingSum(sourceID, year, incomesVariant);
            newRow["Outcomes"] = GetOutcomingSum(sourceID, year, outcomesVariant);
            newRow["Accretion"] = GetCreditIssuedSum(sourceID, currentVariant, year);
            newRow["Recession"] = GetCreditIncomingSum(sourceID, currentVariant, year);
            newRow["RemainsAccretion"] = Convert.ToDecimal(newRow["Accretion"]) + Convert.ToDecimal(newRow["Incomes"]);
            newRow["RemainsRecession"] = Convert.ToDecimal(newRow["Recession"]) + Convert.ToDecimal(newRow["Outcomes"]);
            newRow["RemainsChange"] = Convert.ToDecimal(newRow["RemainsRecession"]) - Convert.ToDecimal(newRow["RemainsAccretion"]);
            dt.Rows.Add(newRow);
            dt.AcceptChanges();
            return dt;
        }

        private static DataTable GetBalanceTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("SourceID", typeof(Int32));
            table.Columns.Add("Incomes", typeof(Decimal));
            table.Columns.Add("Outcomes", typeof(Decimal));
            table.Columns.Add("Accretion", typeof(Decimal));
            table.Columns.Add("Recession", typeof(Decimal));
            table.Columns.Add("RemainsAccretion", typeof(Decimal));
            table.Columns.Add("RemainsRecession", typeof(Decimal));
            table.Columns.Add("RemainsChange", typeof(Decimal));
            table.Columns.Add("CurrentVariant", typeof(Int32));
            table.Columns.Add("IncomesVariant", typeof(Int32));
            table.Columns.Add("OutcomesVariant", typeof(Int32));
            return table;
        }

        public static void CalculateBalance(ref DataTable dtBalance)
        {
            dtBalance.Rows[0]["RemainsAccretion"] = Convert.ToDecimal(dtBalance.Rows[0]["Accretion"]) +
                Convert.ToDecimal(dtBalance.Rows[0]["Incomes"]);
            dtBalance.Rows[0]["RemainsRecession"] = Convert.ToDecimal(dtBalance.Rows[0]["Recession"]) +
                Convert.ToDecimal(dtBalance.Rows[0]["Outcomes"]);
            dtBalance.Rows[0]["RemainsChange"] = Convert.ToDecimal(dtBalance.Rows[0]["RemainsRecession"]) -
                Convert.ToDecimal(dtBalance.Rows[0]["RemainsAccretion"]);
        }

        /// <summary>
        /// доходы
        /// </summary>
        public static Decimal GetIncomingSum(int sourceID, int year, int variant)
        {
            decimal result = 0;
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string kdSelect = string.Format("select id from d_KD_PlanIncomes where (Code2 = 1 or Code2 = 2 or Code2 = 3) and SourceID = {0}", sourceID);
                string regionsSelect = string.Format("select id from d_Regions_Plan where name LIKE 'Бюджет субъекта%' and SourceID = {0}", sourceID);
                string query =
                    string.Format("select Forecast from f_D_FOPlanIncDivide where RefVariant = {0} and RefYear = {1} and RefKD IN ({2}) and RefRegions = ({3})",
                    variant, year, kdSelect, regionsSelect);

                DataTable dtResult = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                foreach (DataRow row in dtResult.Rows)
                {
                    result += Convert.ToDecimal(row[0]);
                }
            }
            return result;
        }

        /// <summary>
        /// расходы
        /// </summary>
        public static Decimal GetOutcomingSum(int sourceID, int year, int variant)
        {
            decimal result = 0;
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string outcomesPlaningQuery =
                    string.Format(
                        "select id from d_R_Plan where (code2 BETWEEN 1 and 11) and code3 = 0 and SourceID = {0}",
                        sourceID);
                string regionsSelect = string.Format("select id from d_Regions_Plan where name LIKE 'Бюджет субъекта%' and SourceID = {0}", sourceID);
                string ekrSelect = string.Format("select id from d_EKR_PlanOutcomes where (code1 = 2) and SourceID = {0}", sourceID);
                const string budgetLevelSelect = "select Id from fx_FX_BudgetLevels where Code = 21000";
                string select =
                    string.Format(
                        "select Summa from f_R_FO26R where RefVariant = {0} and RefBdgtLvls = ({1}) and RefYear = {2} and RefEKR IN ({3}) and RefRegions = ({4}) and RefR IN ({5})",
                        variant, budgetLevelSelect, year, ekrSelect, regionsSelect, outcomesPlaningQuery);
                DataTable dtResult = (DataTable)db.ExecQuery(select, QueryResultTypes.DataTable);
                foreach (DataRow row in dtResult.Rows)
                {
                    result += Convert.ToDecimal(row[0]);
                }
            }
            return result;
        }

        /// <summary>
        /// доходная часть
        /// </summary>
        public static Decimal GetCreditIssuedSum(int sourceID, int variant, int year)
        {
            decimal result = 0;

            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string creditIncomingSelect = string.Format("select Sum from f_S_Creditincome where RefSStatusPlan = 0 and (StartDate BETWEEN ? and ?) and RefVariant = {0}", variant);
                DateTime startOfYear = new DateTime(year, 1, 1);
                IDbDataParameter[] prms = new IDbDataParameter[2];
                prms[0] = new System.Data.OleDb.OleDbParameter("p0", startOfYear);
                DateTime endOfYear = new DateTime(year, 12, 31);
                prms[1] = new System.Data.OleDb.OleDbParameter("p1", endOfYear);

                DataTable dtCreditIncomingSum = (DataTable)db.ExecQuery(creditIncomingSelect, QueryResultTypes.DataTable, prms);

                string kifSelect = string.Format("select id from d_KIF_Plan where (CodeStr = '00001060501020000640' or CodeStr = '00001060502020000640') and SourceID = {0}", sourceID);
                prms[0] = new System.Data.OleDb.OleDbParameter("p0", startOfYear);
                prms[1] = new System.Data.OleDb.OleDbParameter("p1", endOfYear);
                string creditIssuedFactDedtSelect =
                    string.Format(
                        "select sum from t_S_FactDebtCO where (FactDate BETWEEN ? and ?) and (RefKIF IN ({0}))",
                        kifSelect);
                DataTable dtCreditIssuedFactDedtSum = (DataTable)db.ExecQuery(creditIssuedFactDedtSelect,
                                                                   QueryResultTypes.DataTable, prms);

                foreach (DataRow row in dtCreditIncomingSum.Rows)
                {
                    result += Convert.ToDecimal(row[0]);
                }
                foreach (DataRow row in dtCreditIssuedFactDedtSum.Rows)
                {
                    result += Convert.ToDecimal(row[0]);
                }
                result += GetADMPlanSrcFin(sourceID);
            }

            return result;
        }

        /// <summary>
        /// расходная часть
        /// </summary>
        public static Decimal GetCreditIncomingSum(int sourceID, int variant, int year)
        {
            decimal result = 0;

            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string creditIssuedSelect = string.Format("select Sum from f_S_Creditissued where RefSStatusPlan = 0 and (StartDate BETWEEN ? and ?) and RefVariant = {0}", variant);
                DateTime startOfYear = new DateTime(year, 1, 1);
                IDbDataParameter[] prms = new IDbDataParameter[2];
                prms[0] = new System.Data.OleDb.OleDbParameter("p0", startOfYear);
                DateTime endOfYear = new DateTime(year, 12, 31);
                prms[1] = new System.Data.OleDb.OleDbParameter("p1", endOfYear);
                DataTable dtCreditIssuedSum = (DataTable)db.ExecQuery(creditIssuedSelect, QueryResultTypes.DataTable, prms);

                string kifSelect = string.Format("select id from d_KIF_Plan where (CodeStr = '00001010000020000810' or CodeStr = '00001020000020000810' or CodeStr = '00001030000020000810') and SourceID = {0}", sourceID);
                prms[0] = new System.Data.OleDb.OleDbParameter("p0", startOfYear);
                prms[1] = new System.Data.OleDb.OleDbParameter("p1", endOfYear);
                string creditIncomingFactDedtSelect =
                    string.Format(
                        "select sum from t_S_FactDebtCI where (FactDate BETWEEN ? and ?) and (RefKIF IN ({0}))",
                        kifSelect);
                DataTable dtCreditIncomingFactDedtSum = (DataTable)db.ExecQuery(creditIncomingFactDedtSelect,
                                                                   QueryResultTypes.DataTable, prms);

                foreach (DataRow row in dtCreditIssuedSum.Rows)
                {
                    result += Convert.ToDecimal(row[0]);
                }
                foreach (DataRow row in dtCreditIncomingFactDedtSum.Rows)
                {
                    result += Convert.ToDecimal(row[0]);
                }
            }

            return result;
        }

        /// <summary>
        /// Уменьшение стоимости акций и иных форм участия в капитале
        /// </summary>
        private static Decimal GetADMPlanSrcFin(int sourceID)
        {
            decimal result = 0;

            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                const string budgetLevelSelect = "select Id from fx_FX_BudgetLevels where Code = 21000";
                string regionsSelect = string.Format("select id from d_Regions_Plan where name LIKE 'Бюджет субъекта%' and SourceID = {0}", sourceID);
                string kifSelect = string.Format("select id from d_KIF_Plan where CodeStr = '00001060100020000630' and SourceID = {0}", sourceID);
                string admSelect = string.Format("select Forecast from f_S_ADMPlanSrcFin where SourceID = {0} and RefBudgetLevels = ({1}) and RefRegion = ({2}) and RefKIF = ({3})",
                    sourceID, budgetLevelSelect, regionsSelect, kifSelect);
                DataTable dtADM = (DataTable)db.ExecQuery(admSelect, QueryResultTypes.DataTable);

                foreach (DataRow row in dtADM.Rows)
                {
                    result += Convert.ToDecimal(row[0]);
                }
            }

            return result;
        }

        /// <summary>
        /// сохранение данных в базе
        /// </summary>
        public static void SaveData(DataRow balanceRow, int year)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_RemainsDesign);

            using (IDataUpdater du = entity.GetDataUpdater(string.Format("RefVBorrow = {0} and RefVIncomes = {1} and RefVOutcomes = {2}",
                balanceRow["CurrentVariant"], balanceRow["IncomesVariant"], balanceRow["OutcomesVariant"]), null))
            {
                DataTable dtRemains = new DataTable();
                du.Fill(ref dtRemains);
                // заполняем необходимые поля
                DataRow newRow = dtRemains.Rows.Count == 0 ? dtRemains.NewRow() : dtRemains.Rows[0];
                newRow.BeginEdit();
                newRow["ID"] = newRow.IsNull("ID") ? entity.GetGeneratorNextValue : newRow["ID"];
                newRow["SourceID"] = balanceRow["SourceID"];
                newRow["TaskID"] = -1;
                newRow["RefVBorrow"] = balanceRow["CurrentVariant"];
                newRow["Income"] = balanceRow["Incomes"];
                newRow["Charge"] = balanceRow["Outcomes"];
                newRow["RemainsAccretion"] = balanceRow["RemainsAccretion"];
                newRow["RemainsRecession"] = balanceRow["RemainsRecession"];
                newRow["RemainsChange"] = balanceRow["RemainsChange"];
                newRow["RefVIncomes"] = balanceRow["IncomesVariant"];
                newRow["RefVOutcomes"] = balanceRow["OutcomesVariant"];
                newRow.EndEdit();
                if (newRow.RowState == DataRowState.Added || newRow.RowState == DataRowState.Detached)
                    dtRemains.Rows.Add(newRow);

                SavePlanIF(balanceRow, year);

                du.Update(ref dtRemains);
            }
        }

        public static void SavePlanIF(DataRow balanceRow, int year)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_PlanResult);

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                object change = db.ExecQuery(string.Format("select id from d_KIF_Plan where CodeStr = '00001050000000000000' and SourceID = {0}", balanceRow["SourceID"]), QueryResultTypes.Scalar);

                object inc = db.ExecQuery(string.Format("select id from d_KIF_Plan where CodeStr = '00001050000000000500' and SourceID = {0}", balanceRow["SourceID"]), QueryResultTypes.Scalar);

                object dec = db.ExecQuery(string.Format("select id from d_KIF_Plan where CodeStr = '00001050000000000600' and SourceID = {0}", balanceRow["SourceID"]), QueryResultTypes.Scalar);

                object budgetLevel =
                    Utils.GetBudgetLevel(Convert.ToInt32(scheme.GlobalConstsManager.Consts["TerrPartType"].Value));

                object region = db.ExecQuery(string.Format(
                        "select id from d_Regions_Plan where name LIKE 'Бюджет субъекта%' and SourceID = {0}", balanceRow["SourceID"]),
                    QueryResultTypes.Scalar);

                using (IDataUpdater du = entity.GetDataUpdater(string.Format("RefVariant = {0}", balanceRow["CurrentVariant"]), null))
                {
                    DataTable dtPlanIF = new DataTable();
                    du.Fill(ref dtPlanIF);
                    if (dtPlanIF.Rows.Count > 0)
                    {
                        DataRow[] rows = dtPlanIF.Select(string.Format("RefKIFPlan = {0}", change));
                        rows[0]["Sum"] = balanceRow["RemainsChange"];
                        rows = dtPlanIF.Select(string.Format("RefKIFPlan = {0}", inc));
                        rows[0]["Sum"] = balanceRow["RemainsAccretion"];
                        rows = dtPlanIF.Select(string.Format("RefKIFPlan = {0}", dec));
                        rows[0]["Sum"] = balanceRow["RemainsRecession"];
                        du.Update(ref dtPlanIF);
                        return;
                    }

                    DataRow newRow = GetNewPlanIFRow(dtPlanIF, balanceRow["SourceID"], balanceRow["CurrentVariant"],
                        year, budgetLevel, region, change, balanceRow["RemainsChange"]);
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    dtPlanIF.Rows.Add(newRow);
                    newRow = GetNewPlanIFRow(dtPlanIF, balanceRow["SourceID"], balanceRow["CurrentVariant"],
                        year, budgetLevel, region, inc, balanceRow["RemainsAccretion"]);
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    dtPlanIF.Rows.Add(newRow);
                    newRow = GetNewPlanIFRow(dtPlanIF, balanceRow["SourceID"], balanceRow["CurrentVariant"],
                        year, budgetLevel, region, dec, balanceRow["RemainsRecession"]);
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    dtPlanIF.Rows.Add(newRow);
                    du.Update(ref dtPlanIF);
                }
            }
        }

        private static DataRow GetNewPlanIFRow(DataTable dtPlanIF, object sourceID, object variant, int year, object budgetLevel, object region, object kif, object sum)
        {
            DataRow newRow = dtPlanIF.NewRow();
            newRow.BeginEdit();
            newRow["SourceID"] = sourceID;
            newRow["PumpID"] = -1;
            newRow["TaskID"] = -1;
            newRow["RefVariant"] = variant;
            newRow["RefYear"] = year;
            newRow["RefLevelBudget"] = budgetLevel;
            newRow["RefRegions"] = region;
            newRow["RefKIFPlan"] = kif;
            newRow["RefAdminPlan"] = -1;
            newRow["Sum"] = sum;
            newRow.EndEdit();
            return newRow;
        }
    }
}
