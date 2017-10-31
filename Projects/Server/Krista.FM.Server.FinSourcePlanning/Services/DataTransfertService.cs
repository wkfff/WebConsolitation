using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.FinSourcePlanning.Services
{
    public class DataTransfertService
    {
        public void TransfertData(IScheme scheme, object pumpId)
        {
            using (var db = scheme.SchemeDWH.DB)
            {
                object region = GetRegion(db);
                if (region == null)
                    return;
                object currentVariant = null;
                object reportDate = null;
                GetCurrentVariant(db, ref currentVariant, ref reportDate);
                if (currentVariant == null)
                    return;
                object sourceId = GetDataSource(scheme);
                var date = Convert.ToDateTime(reportDate);
                TransfertCredits(scheme, db, region, pumpId, currentVariant, sourceId, date);
                TransfertGuaranties(scheme, db, region, pumpId, currentVariant, sourceId);
            }
        }

        #region общие методы

        private object GetRegion(IDatabase db)
        {
            var regions = (DataTable)db.ExecQuery(
                @"select id from d_Regions_Analysis where ParentID is null and RefTerr = 3 and SourceId in
                (select id from DataSources where Year = ?)",
                QueryResultTypes.DataTable,
                new DbParameterDescriptor("p0", DateTime.Today.Year));
            if (regions.Rows.Count > 0)
                return regions.Rows[0][0];
            return null;
        }

        private DataRow GetDataRow(DataTable source, string columnKey, object rowKey)
        {
            foreach (var row in source.Select(string.Format("{0} = {1}", columnKey, rowKey)))
            {
                return row;
            }
            var newRow = source.NewRow();
            newRow[columnKey] = rowKey;
            source.Rows.Add(newRow);
            return newRow;
        }

        private void GetCurrentVariant(IDatabase db, ref object  variantId, ref object reportDate)
        {
            var table = (DataTable)db.ExecQuery("select id, ReportDate from d_Variant_Schuldbuch where CurrentVariant = 1", QueryResultTypes.DataTable);
            variantId = table != null && table.Rows.Count > 0 ? table.Rows[0][0] : null;
            reportDate = table != null && table.Rows.Count > 0 ? table.Rows[0][1] : null;
        }

        /// <summary>
        /// Ищет источник данных с указанными параметрами.
        /// </summary>
        /// <returns>ID источника данных.</returns>
        internal static int GetDataSource(IScheme scheme)
        {
            using (var db = scheme.SchemeDWH.DB)
            {
                object sourceID = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    new DbParameterDescriptor("SupplierCode", "ФО"),
                    new DbParameterDescriptor("DataCode", 30),
                    new DbParameterDescriptor("Year", DateTime.Today.Year));

                if (sourceID == null || sourceID == DBNull.Value)
                {
                    IDataSource ds =
                        scheme.DataSourceManager.DataSources.
                            CreateElement();
                    ds.SupplierCode = "ФО";
                    ds.DataCode = "30";
                    ds.DataName = "Проект бюджета";
                    ds.Year = DateTime.Today.Year;
                    ds.ParametersType = ParamKindTypes.Year;
                    return ds.Save();
                }
                return Convert.ToInt32(sourceID);
            }
        }

        #endregion

        #region перенос кредитов

        private void TransfertCredits(IScheme scheme, IDatabase db, object region, object pumpId, object variant, object sourceId, DateTime reportDate)
        {
            var masterData = (DataTable)db.ExecQuery(
                    @"select credit.ID, credit.Num, credit.IDDoc, credit.RefSTypeCredit, credit.RegNum, credit.Num, credit.ContractDate, credit.Note, credit.Sum,
                        credit.RenewalDate, credit.EndDate, credit.RefOKV, credit.CreditPercent, credit.RefOKV, credit.RefSCreditPeriod, credit.RefOrganizations, credit.RefSTypeCredit,
                        org.Name, period.Name as FactDate, period.ID as FactId
                        from f_S_Creditincome credit, d_Organizations_Plan org, fx_S_Periodicity period
                        where (credit.RefVariant = -2 or credit.RefVariant = 0) 
                        and credit.RefOrganizations = org.ID and credit.RefPeriodRate = period.ID and credit.IDDoc is not null",
                    QueryResultTypes.DataTable);
            var collateralData = (DataTable)db.ExecQuery(
                "select Name, RefCreditInc from t_S_CollateralCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);
            var contractData = (DataTable)db.ExecQuery(
                "select FactDate, RefCreditInc from t_S_FactAttractCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);
            var offsetData = (DataTable)db.ExecQuery(
                "select FactDate, RefCreditInc from t_S_FactDebtCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);
            var attractData = (DataTable)db.ExecQuery(
                "select FactDate, Sum, RefCreditInc from t_S_FactAttractCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);
            var dischargeData = (DataTable)db.ExecQuery(
                "select FactDate, Sum, RefCreditInc from t_S_FactDebtCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);
            var staleDebtData = (DataTable)db.ExecQuery(
                "select EndDate, Sum, RefCreditInc from t_S_PlanDebtCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);
            var serviceData = (DataTable)db.ExecQuery(
                "select FactDate, Sum, ChargeSum, RefCreditInc from t_S_FactPercentCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);
            var penaltyData = (DataTable)db.ExecQuery(
                "select FactDate, Sum, RefCreditInc from t_S_FactPenaltyDebtCI where RefCreditInc in (select id from f_S_Creditincome where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);

            var creditEntity = scheme.RootPackage.FindEntityByName(DebtBookKeys.f_S_SchBCreditincome);
            using (var upd = creditEntity.GetDataUpdater("RefVariant = ? and SourceID = ?", null,
                new DbParameterDescriptor("RefVariant", variant), new DbParameterDescriptor("SourceID", sourceId)))
            {
                var dtData = new DataTable();
                upd.Fill(ref dtData);
                foreach (DataRow row in masterData.Rows)
                {
                    int id = Convert.ToInt32(row["ID"]);
                    var newRow = GetDataRow(dtData, "IDDoc", row["IDDoc"]);
                    newRow["RefTypeCredit"] = row["RefSTypeCredit"];
                    newRow["PaymentDate"] = row.IsNull("RenewalDate") ? row["EndDate"] : row["RenewalDate"];
                    var rows = offsetData.Select(string.Format("RefCreditInc = {0}", row["ID"]), "FactDate desc");
                    DateTime offsetDate = rows.Length > 0 ? Convert.ToDateTime(rows[0]["FactDate"]) : DateTime.MinValue;
                    if (offsetDate > DateTime.MinValue)
                        newRow["OffsetDate"] = offsetDate;
                    newRow["CreditPercentNum"] = row["CreditPercent"];

                    var attract = attractData.Rows.Cast<DataRow>().
                        Where(w => Convert.ToInt32(w["RefCreditInc"]) == id && !w.IsNull("FactDate") && !w.IsNull("Sum") && Convert.ToDateTime(w["FactDate"]) < reportDate).Sum(
                        s => Convert.ToDecimal(s["Sum"]));
                    var discharge = dischargeData.Rows.Cast<DataRow>().
                        Where(w => Convert.ToInt32(w["RefCreditInc"]) == id && !w.IsNull("FactDate") && !w.IsNull("Sum") && Convert.ToDateTime(w["FactDate"]) < reportDate).Sum(
                        s => Convert.ToDecimal(s["Sum"]));
                    var capitalDebt = attract - discharge;
                    newRow["Attract"] = attract;
                    newRow["Discharge"] = discharge;
                    newRow["CapitalDebt"] = capitalDebt;
                    newRow["Sum"] = row["Sum"];

                    if (staleDebtData.Rows.Count > 0)
                    {
                        var staleDebt = staleDebtData.Rows.Cast<DataRow>().
                            Where(w => Convert.ToInt32(w["RefCreditInc"]) == id && !w.IsNull("EndDate") && !w.IsNull("Sum") && Convert.ToDateTime(w["EndDate"]) < reportDate).Sum(
                            s => Convert.ToDecimal(s["Sum"]));
                        newRow["StaleDebt"] = staleDebt > discharge ? staleDebt - discharge : 0;
                    }
                    else
                        newRow["StaleDebt"] = 0;

                    var planService =
                            serviceData.Rows.Cast<DataRow>().
                            Where(w => Convert.ToInt32(w["RefCreditInc"]) == id && !w.IsNull("FactDate") && !w.IsNull("ChargeSum") && Convert.ToDateTime(w["FactDate"]) < reportDate).
                            Sum(s => Convert.ToDecimal(s["ChargeSum"]));
                    var factService =
                        serviceData.Rows.Cast<DataRow>().
                        Where(w => Convert.ToInt32(w["RefCreditInc"]) == id && !w.IsNull("FactDate") && !w.IsNull("Sum") && Convert.ToDateTime(w["FactDate"]) < reportDate).
                            Sum(s => Convert.ToDecimal(s["Sum"]));
                    var serviceDebt = planService - factService;
                    newRow["PlanService"] = planService;
                    newRow["FactService"] = factService;
                    newRow["ServiceDebt"] = serviceDebt;

                    var chargePenlt =
                            penaltyData.Rows.Cast<DataRow>().
                            Where(w => Convert.ToInt32(w["RefCreditInc"]) == id && !w.IsNull("FactDate") && !w.IsNull("Sum") && Convert.ToDateTime(w["FactDate"]) < reportDate).
                                Sum(s => Convert.ToDecimal(s["Sum"]));
                    var factPenlt = chargePenlt;
                    var remnsEndMnthPenlt = chargePenlt - factPenlt;
                    newRow["ChargePenlt"] = chargePenlt;
                    newRow["FactPenlt"] = factPenlt;
                    newRow["RemnsEndMnthPenlt"] = remnsEndMnthPenlt;

                    newRow["Note"] = row["Note"];

                    if (newRow.RowState == DataRowState.Added)
                    {
                        newRow["PumpID"] = pumpId;
                        newRow["SourceID"] = sourceId;
                        newRow["TaskID"] = -1;
                        newRow["RefVariant"] = variant;
                        newRow["RefRegion"] = region;
                        newRow["Num"] = row["RegNum"];
                        newRow["Creditor"] = row["Name"];
                        newRow["RefSCreditPeriod"] = row["RefSCreditPeriod"];
                        newRow["RefOKV"] = row["RefOKV"];
                        rows = contractData.Select(string.Format("RefCreditInc = {0}", row["ID"]), "FactDate asc");
                        DateTime contractDate = rows.Length > 0 ? Convert.ToDateTime(rows[0]["FactDate"]) : DateTime.MinValue;
                        if (contractDate > DateTime.MinValue)
                            newRow["ContractDate"] = contractDate;
                        var collateralRows = collateralData.Select(string.Format("RefCreditInc = {0}", row["ID"])).
                            Select(s => s["Name"].ToString()).ToArray();
                        newRow["Collateral"] = string.Join(", ", collateralRows);
                        var contrDate = row.IsNull("ContractDate")
                                            ? DateTime.Today
                                            : Convert.ToDateTime(row["ContractDate"]);

                        var crediType = Convert.ToInt32(row["RefSTypeCredit"]);
                        var occasion = crediType == 1
                                           ? string.Format("Соглашение от {0} № {1}", contrDate.ToShortDateString(), row["Num"])
                                           : string.Format("Государственный контракт от {0} № {1}", contrDate.ToShortDateString(), row["Num"]);
                        newRow["Occasion"] = occasion;
                        if (Convert.ToInt32(row["FactId"]) != -1)
                            newRow["FactDate"] = row["FactDate"];
                        newRow["CreditPercent"] = 0;
                        newRow["FromFinSource"] = 1;
                        newRow["RemnsBgnMnthDbt"] = 0;
                        newRow["RemnsEndMnthDbt"] = 0;
                        newRow["DiffrncRatesInterest"] = 0;
                        newRow["DiffrncRatesPenlt"] = 0;
                        newRow["RemnsBgnMnthPenlt"] = 0;
                        newRow["RemnsEndMnthPenlt"] = 0;
                        newRow["ChargePenlt"] = 0;
                        newRow["FactPenlt"] = 0;
                        newRow["BgnYearDbt"] = 0;

                        newRow["BgnYearInterest"] = 0;
                        newRow["BgnYearPenlt"] = 0;
                        newRow["StalePenlt"] = 0;
                        newRow["StaleInterest"] = 0;
                        newRow["DiffrncRatesDbt"] = 0;
                        newRow["RemnsEndMnthInterest"] = 0;

                        newRow["IsBlocked"] = 0;
                        newRow["RefTypeContract"] = -1;
                        newRow["RefOrganizations"] = row["RefOrganizations"];
                    }
                }
                var changes = dtData.GetChanges();
                if (changes != null)
                    upd.Update(ref changes);
            }
        }

        #endregion

        #region перенос гарантий

        private void TransfertGuaranties(IScheme scheme, IDatabase db, object region, object pumpId, object variant, object sourceId)
        {
            var masterData = (DataTable)db.ExecQuery(
                    @"select guaranty.ID, guaranty.IDDoc, guaranty.RegNum, guaranty.Num, guaranty.StartDate, guaranty.Note,
                        guaranty.Sum, guaranty.RenewalDate, guaranty.EndDate, guaranty.RefOKV, guaranty.RefOrganizationsPlan3, guaranty.RefOrganizations,
                        creditor.Name as Creditor, principal.Name as Principal
                        from f_S_Guarantissued guaranty, d_Organizations_Plan creditor, d_Organizations_Plan principal, fx_S_Periodicity period
                        where (guaranty.RefVariant = -2 or guaranty.RefVariant = 0) 
                        and guaranty.RefOrganizationsPlan3 = creditor.ID and guaranty.RefOrganizations = principal.ID and guaranty.IDDoc is not null",
                    QueryResultTypes.DataTable);

            var collateralData = (DataTable)db.ExecQuery(
                "select Name, RefGrnt from t_S_CollateralGrnt where RefGrnt in (select id from f_S_Guarantissued where RefVariant = -2 or RefVariant = 0)",
                QueryResultTypes.DataTable);

            var guarantyEntity = scheme.RootPackage.FindEntityByName(DebtBookKeys.f_S_SchBGuarantissued);
            using (var upd = guarantyEntity.GetDataUpdater("RefVariant = ? and SourceID = ?", null,
                new DbParameterDescriptor("RefVariant", variant), new DbParameterDescriptor("SourceID", sourceId)))
            {
                var guarantyData = new DataTable();
                upd.Fill(ref guarantyData);
                foreach (DataRow row in masterData.Rows)
                {
                    var newRow = GetDataRow(guarantyData, "IDDoc", row["IDDoc"]);
                    newRow["Sum"] = row["Sum"];
                    newRow["Note"] = row["Note"];
                    if (newRow.RowState == DataRowState.Added)
                    {
                        newRow["PumpID"] = pumpId;
                        newRow["SourceID"] = sourceId;
                        newRow["TaskID"] = -1;
                        newRow["RefVariant"] = variant;
                        newRow["RefRegion"] = region;
                        newRow["Num"] = row["RegNum"];
                        newRow["PrincipalNum"] = string.Format("{0} от {1}", row["Num"], Convert.ToDateTime(row["StartDate"]).ToShortDateString());
                        newRow["Creditor"] = row["Creditor"];
                        newRow["Principal"] = row["Principal"];
                        var collateralRows = collateralData.Select(string.Format("RefGrnt = {0}", row["ID"])).Select(s => s["Name"].ToString()).ToArray();
                        newRow["Collateral"] = string.Join(",", collateralRows);
                        newRow["StartDate"] = row["StartDate"];
                        newRow["RefOKV"] = row["RefOKV"];
                        newRow["PrincipalEndDate"] = row.IsNull("RenewalDate") ? row["EndDate"] : row["RenewalDate"];
                        newRow["EndCreditDate"] = row.IsNull("RenewalDate") ? row["EndDate"] : row["RenewalDate"];
                        newRow["RenewalDate"] = row.IsNull("RenewalDate") ? row["EndDate"] : row["RenewalDate"];

                        newRow["RefOrganizations"] = row["RefOrganizations"];
                        newRow["RefOrganizationsPlan3"] = row["RefOrganizationsPlan3"];

                        newRow["Regress"] = "Нет";
                        newRow["UpDebt"] = 0;
                        newRow["UpService"] = 0;
                        newRow["DownDebt"] = 0;
                        newRow["DownService"] = 0;
                        newRow["CapitalDebt"] = 0;
                        newRow["StalePrincipalDebt"] = 0;
                        newRow["StaleGarantDebt"] = 0;
                        newRow["TotalDebt"] = 0;
                        newRow["ServiceDebt"] = 0;
                        newRow["FromFinSource"] = 1;
                        newRow["BgnYearDebt"] = 0;
                        newRow["RemnsBgnMnthDbt"] = 0;
                        newRow["RemnsEndMnthDbt"] = 0;
                        newRow["IsBlocked"] = 0;
                        newRow["DownPenaltyGarant"] = 0;
                        newRow["RefTypeContract"] = -1;
                        newRow["RemnsBgnMnthDbt"] = 0;
                    }
                }
                var changes = guarantyData.GetChanges();
                if (changes != null)
                    upd.Update(ref changes);
            }
        }

        #endregion
    }
}
