using System;
using System.Data;
using Microsoft.Practices.ServiceLocation;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class DebtBookDataService
    {
        private IScheme scheme;

        public DebtBookDataService()
            : this(ServiceLocator.Current.GetInstance<IScheme>())
        {
        }

        public DebtBookDataService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public DataTable GetBudgetSettlementsCreditData(int refRegion, int refVariant)
        {
            const string creditQuery = "select credit.ID, region.Name, credit.Attract, " +
                "credit.Discharge, credit.PlanService, credit.FactService, credit.CapitalDebt, " +
                "credit.ServiceDebt, credit.StaleDebt from {0} credit, {1} region " +
                "where credit.RefRegion = region.ID and credit.RefTypeCredit = 1 and credit.RefVariant = {2} and RefRegion IN ({3})";
            return GetCreditSettlementsData(creditQuery, refRegion, refVariant);
        }

        public DataTable GetOrganizationSettlementsCreditData(int refRegion, int refVariant)
        {
            const string creditQuery = "select credit.ID, region.Name, credit.Attract, " +
                "credit.Discharge, credit.PlanService, credit.FactService, credit.CapitalDebt, " +
                "credit.ServiceDebt, credit.StaleDebt from {0} credit, {1} region " +
                "where credit.RefRegion = region.ID and credit.RefTypeCredit = 0 and credit.RefVariant = {2} and RefRegion IN ({3})";
            return GetCreditSettlementsData(creditQuery, refRegion, refVariant);
        }

        private DataTable GetCreditSettlementsData(string creditQuery, int refRegion, int refVariant)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            IEntity regionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            string regionsQuery = string.Format("select id from {0} where ParentID IN (select id from {0} where ParentID = {1})", regionEntity.FullDBName, refRegion);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable dtSettlements =
                    (DataTable)db.ExecQuery(string.Format(creditQuery, entity.FullDBName, regionEntity.FullDBName, refVariant, regionsQuery), QueryResultTypes.DataTable);
                if (dtSettlements.Rows.Count == 0)
                    return dtSettlements;
                decimal attract = 0;
                decimal discharge = 0;
                decimal planService = 0;
                decimal factService = 0;
                decimal capitalDebt = 0;
                decimal serviceDebt = 0;
                decimal staleDebt = 0;
                int index = 1;
                foreach (DataColumn column in dtSettlements.Columns)
                {
                    column.AllowDBNull = true;
                    column.ReadOnly = false;
                }
                foreach (DataRow row in dtSettlements.Rows)
                {
                    row[0] = index++;
                    attract += row["Attract"] is DBNull ? 0 : Convert.ToDecimal(row["Attract"]);
                    discharge += row["Discharge"] is DBNull ? 0 : Convert.ToDecimal(row["Discharge"]);
                    planService += row["PlanService"] is DBNull ? 0 : Convert.ToDecimal(row["PlanService"]);
                    factService += row["FactService"] is DBNull ? 0 : Convert.ToDecimal(row["FactService"]);
                    capitalDebt += row["CapitalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    serviceDebt += row["ServiceDebt"] is DBNull ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                    staleDebt += row["StaleDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StaleDebt"]);
                }
                DataRow newRow = dtSettlements.NewRow();
                newRow.ItemArray = dtSettlements.Rows[0].ItemArray;
                newRow["Attract"] = attract;
                newRow["Discharge"] = discharge;
                newRow["PlanService"] = planService;
                newRow["FactService"] = factService;
                newRow["CapitalDebt"] = capitalDebt;
                newRow["ServiceDebt"] = serviceDebt;
                newRow["StaleDebt"] = staleDebt;
                newRow[0] = index;
                dtSettlements.Rows.Add(newRow);
                return dtSettlements;
            }
        }

        public DataTable GetGuaranteeSettlementsData(int refRegion, int refVariant)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissuedPos);
            IEntity regionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            string regionQuery = string.Format("select id from {0} where ParentID IN (select id from {0} where ParentID = {1})", regionEntity.FullDBName, refRegion);

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                const string guaranteeSettlementsQuery =
                    "select guarantee.ID, region.Name, guarantee.UpDebt, guarantee.UpService, " +
                    "guarantee.DownDebt, guarantee.DownService, guarantee.DownGarant, guarantee.TotalDebt, guarantee.CapitalDebt, " +
                    "guarantee.ServiceDebt, guarantee.StalePrincipalDebt, guarantee.StaleGarantDebt from {0} guarantee, {1} region " +
                    "where guarantee.RefRegion = region.ID and guarantee.RefVariant = {2} and RefRegion IN ({3})";
                DataTable dtSettlements = (DataTable)db.ExecQuery(string.Format(guaranteeSettlementsQuery, entity.FullDBName, regionEntity.FullDBName, refVariant, regionQuery), QueryResultTypes.DataTable);
                if (dtSettlements.Rows.Count == 0)
                    return dtSettlements;
                dtSettlements.BeginLoadData();
                decimal upDebt = 0;
                decimal upService = 0;
                decimal downDebt = 0;
                decimal downService = 0;
                decimal capitalDebt = 0;
                decimal stalePrincipalDebt = 0;
                decimal staleGarantDebt = 0;
                decimal downGarant = 0;
                decimal totalDebt = 0;
                decimal serviceDebt = 0;
                int index = 1;
                foreach (DataColumn column in dtSettlements.Columns)
                {
                    column.AllowDBNull = true;
                    column.ReadOnly = false;
                }
                foreach (DataRow row in dtSettlements.Rows)
                {
                    row[0] = index++;
                    upDebt += row["UpDebt"] is DBNull ? 0 : Convert.ToDecimal(row["UpDebt"]);
                    upService += row["UpService"] is DBNull ? 0 : Convert.ToDecimal(row["UpService"]);
                    downDebt += row["DownDebt"] is DBNull ? 0 : Convert.ToDecimal(row["DownDebt"]);
                    downService += row["DownService"] is DBNull ? 0 : Convert.ToDecimal(row["DownService"]);
                    capitalDebt += row["CapitalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    stalePrincipalDebt += row["StalePrincipalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StalePrincipalDebt"]);
                    staleGarantDebt += row["StaleGarantDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StaleGarantDebt"]);
                    downGarant += row["DownGarant"] is DBNull ? 0 : Convert.ToDecimal(row["DownGarant"]);
                    totalDebt += row["TotalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["TotalDebt"]);
                    serviceDebt += row["ServiceDebt"] is DBNull ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                }

                DataRow newRow = dtSettlements.NewRow();
                newRow.ItemArray = dtSettlements.Rows[0].ItemArray;
                newRow["UpDebt"] = upDebt;
                newRow["UpService"] = upService;
                newRow["DownDebt"] = downDebt;
                newRow["DownService"] = downService;
                newRow["CapitalDebt"] = capitalDebt;
                newRow["StalePrincipalDebt"] = stalePrincipalDebt;
                newRow["StaleGarantDebt"] = staleGarantDebt;
                newRow["DownGarant"] = downGarant;
                newRow["TotalDebt"] = totalDebt;
                newRow["ServiceDebt"] = serviceDebt;
                newRow[0] = index;
                dtSettlements.Rows.Add(newRow);
                return dtSettlements;
            }
        }
    }
}
