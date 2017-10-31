using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Получение данных - Вологда
    /// </summary>
    public partial class ReportsDataService
    {
        public DataTable GetBudgetSettlementsCreditData(int refRegion, int refVariant)
        {
            string creditQuery = "select credit.ID, region.Name, credit.Attract, " +
                "credit.Discharge, credit.PlanService, credit.FactService, credit.CapitalDebt, " +
                "credit.ServiceDebt, credit.StaleDebt from {0} credit, {1} region " +
                "where credit.RefRegion = region.ID and credit.RefTypeCredit = 1 and credit.RefVariant = {2} and RefRegion IN ({3}) order by region.Name";
            return GetCreditSettlementsData(creditQuery, refRegion, refVariant);
        }

        public DataTable GetOrganizationSettlementsCreditData(int refRegion, int refVariant)
        {
            string creditQuery = "select credit.ID, region.Name, credit.Attract, " +
                "credit.Discharge, credit.PlanService, credit.FactService, credit.CapitalDebt, " +
                "credit.ServiceDebt, credit.StaleDebt from {0} credit, {1} region " +
                "where credit.RefRegion = region.ID and credit.RefTypeCredit = 0 and credit.RefVariant = {2} and RefRegion IN ({3}) order by region.Name";
            return GetCreditSettlementsData(creditQuery, refRegion, refVariant);
        }

        public DataTable GetGuaranteeSettlementsData(int refRegion, int refVariant)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissuedPos);
            IEntity regionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            string regionQuery = string.Format("select id from {0} where ParentID IN (select id from {0} where ParentID = {1})", regionEntity.FullDBName, refRegion);

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string guaranteeSettlementsQuery =
                    "select guarantee.ID, region.Name, guarantee.UpDebt, guarantee.UpService, " +
                    "guarantee.DownDebt, guarantee.DownService, guarantee.DownGarant, guarantee.TotalDebt, guarantee.CapitalDebt, " +
                    "guarantee.ServiceDebt, guarantee.StalePrincipalDebt, guarantee.StaleGarantDebt from {0} guarantee, {1} region " +
                    "where guarantee.RefRegion = region.ID and guarantee.RefVariant = {2} and RefRegion IN ({3}) order by region.Name";
                DataTable tblSettlements = (DataTable)db.ExecQuery(string.Format(guaranteeSettlementsQuery, entity.FullDBName, regionEntity.FullDBName, refVariant, regionQuery), QueryResultTypes.DataTable);
                if (tblSettlements.Rows.Count == 0)
                {
                    return tblSettlements;
                }

                tblSettlements.BeginLoadData();
                decimal attrDebt = 0;
                decimal attrService = 0;
                decimal downDebt = 0;
                decimal downService = 0;
                decimal capitalDebt = 0;
                decimal stalePrincipalDebt = 0;
                decimal staleGarantDebt = 0;
                decimal downGarant = 0;
                decimal totalDebt = 0;
                decimal serviceDebt = 0;
                int index = 1;
                foreach (DataColumn column in tblSettlements.Columns)
                {
                    column.AllowDBNull = true;
                    column.ReadOnly = false;
                }

                foreach (DataRow row in tblSettlements.Rows)
                {
                    row[0] = index++;
                    attrDebt += row["UpDebt"] is DBNull ? 0 : Convert.ToDecimal(row["UpDebt"]);
                    attrService += row["UpService"] is DBNull ? 0 : Convert.ToDecimal(row["UpService"]);
                    downDebt += row["DownDebt"] is DBNull ? 0 : Convert.ToDecimal(row["DownDebt"]);
                    downService += row["DownService"] is DBNull ? 0 : Convert.ToDecimal(row["DownService"]);
                    capitalDebt += row["CapitalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    stalePrincipalDebt += row["StalePrincipalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StalePrincipalDebt"]);
                    staleGarantDebt += row["StaleGarantDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StaleGarantDebt"]);
                    downGarant += row["DownGarant"] is DBNull ? 0 : Convert.ToDecimal(row["DownGarant"]);
                    totalDebt += row["TotalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["TotalDebt"]);
                    serviceDebt += row["ServiceDebt"] is DBNull ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                }

                DataRow newRow = tblSettlements.NewRow();
                newRow.ItemArray = tblSettlements.Rows[0].ItemArray;
                newRow["UpDebt"] = attrDebt;
                newRow["UpService"] = attrService;
                newRow["DownDebt"] = downDebt;
                newRow["DownService"] = downService;
                newRow["CapitalDebt"] = capitalDebt;
                newRow["StalePrincipalDebt"] = stalePrincipalDebt;
                newRow["StaleGarantDebt"] = staleGarantDebt;
                newRow["DownGarant"] = downGarant;
                newRow["TotalDebt"] = totalDebt;
                newRow["ServiceDebt"] = serviceDebt;
                newRow[0] = index;
                tblSettlements.Rows.Add(newRow);
                return tblSettlements;
            }
        }

        public void GetBudgetCreditsData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            GetCreditData(refVariant, refRegion, 1, ref tables, calculateDate);
            tables[1] = GetBudgetSettlementsCreditData(refRegion, refVariant);
        }

        public void GetOrganizationCreditsData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            GetCreditData(refVariant, refRegion, 0, ref tables, calculateDate);
            tables[1] = GetOrganizationSettlementsCreditData(refRegion, refVariant);
        }

        public void GetGuaranteeData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            IEntity guaranteeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            IEntity regionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query =
                    "select guarantee.ID, guarantee.RegNum, guaranteeType.Name, guarantee.Num, guarantee.DateDoc, guarantee.FurtherConvention, guarantee.Occasion, " +
                    "organization.Name, orzanizationPlan.Name, guarantee.Sum, okv.Name, guarantee.Occasion, guarantee.DateDoc, guarantee.Occasion, guarantee.EndDate, " +
                    "guarantee.Collateral, guarantee.Regress, guarantee.UpDebt, guarantee.UpService, guarantee.DownDebt, guarantee.DownService, " +
                    "guarantee.DownGarant, guarantee.TotalDebt, guarantee.CapitalDebt, guarantee.ServiceDebt, guarantee.StalePrincipalDebt, " +
                    "guarantee.StaleGarantDebt from {0} guarantee, d_S_TypeContract guaranteeType, d_Organizations_Plan organization, " +
                    "d_Organizations_Plan orzanizationPlan, d_OKV_Currency okv where guaranteeType.ID = guarantee.RefTypeContract and " +
                    "organization.ID = guarantee.RefOrganizations and orzanizationPlan.ID = guarantee.RefOrganizationsPlan3 and okv.ID = guarantee.RefOKV and " +
                    "guarantee.RefRegion = {1} and guarantee.RefVariant = {2}";

                DataTable guaranteeTable = (DataTable)db.ExecQuery(string.Format(query, guaranteeEntity.FullDBName, refRegion, refVariant), QueryResultTypes.DataTable);

                decimal attrDebt = 0;
                decimal attrService = 0;
                decimal downDebt = 0;
                decimal downService = 0;
                decimal capitalDebt = 0;
                decimal stalePrincipalDebt = 0;
                decimal staleGarantDebt = 0;
                decimal downGarant = 0;
                decimal totalDebt = 0;
                decimal serviceDebt = 0;
                int index = 1;

                foreach (DataColumn column in guaranteeTable.Columns)
                {
                    column.AllowDBNull = true;
                    column.ReadOnly = false;
                }

                foreach (DataRow row in guaranteeTable.Rows)
                {
                    row[0] = index++;
                    row[2] = row["Name"] + " " + row["Num"] + " " + row["DateDoc"] + (row.IsNull("FurtherConvention") ? string.Empty : " " + row["FurtherConvention"]);
                    attrDebt += row["UpDebt"] is DBNull ? 0 : Convert.ToDecimal(row["UpDebt"]);
                    attrService += row["UpService"] is DBNull ? 0 : Convert.ToDecimal(row["UpService"]);
                    downDebt += row["DownDebt"] is DBNull ? 0 : Convert.ToDecimal(row["DownDebt"]);
                    downService += row["DownService"] is DBNull ? 0 : Convert.ToDecimal(row["DownService"]);
                    capitalDebt += row["CapitalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    stalePrincipalDebt += row["StalePrincipalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StalePrincipalDebt"]);
                    staleGarantDebt += row["StaleGarantDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StaleGarantDebt"]);
                    downGarant += row["DownGarant"] is DBNull ? 0 : Convert.ToDecimal(row["DownGarant"]);
                    totalDebt += row["TotalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["TotalDebt"]);
                    serviceDebt += row["ServiceDebt"] is DBNull ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                    row["Occasion1"] = row.IsNull("DateDoc") ? string.Empty : row["DateDoc"];
                    row["Occasion2"] = row["EndDate"];
                }

                if (guaranteeTable.Rows.Count > 0)
                {
                    DataRow newRow = guaranteeTable.NewRow();
                    newRow.ItemArray = guaranteeTable.Rows[0].ItemArray;
                    newRow["UpDebt"] = attrDebt;
                    newRow["UpService"] = attrService;
                    newRow["DownDebt"] = downDebt;
                    newRow["DownService"] = downService;
                    newRow["CapitalDebt"] = capitalDebt;
                    newRow["StalePrincipalDebt"] = stalePrincipalDebt;
                    newRow["StaleGarantDebt"] = staleGarantDebt;
                    newRow["DownGarant"] = downGarant;
                    newRow["TotalDebt"] = totalDebt;
                    newRow["ServiceDebt"] = serviceDebt;
                    guaranteeTable.Rows.Add(newRow);
                }

                guaranteeTable.Columns.Remove(guaranteeTable.Columns["Num"]);
                guaranteeTable.Columns.Remove(guaranteeTable.Columns["FurtherConvention"]);
                guaranteeTable.Columns.Remove(guaranteeTable.Columns["DateDoc"]);
                guaranteeTable.Columns.Remove(guaranteeTable.Columns["DateDoc1"]);
                guaranteeTable.Columns.Remove(guaranteeTable.Columns["EndDate"]);
                guaranteeTable.AcceptChanges();

                DataTable settlementsGuaranteeTable = GetGuaranteeSettlementsData(refRegion, refVariant);
                DataTable regionData = (DataTable)db.ExecQuery(string.Format("select name, RefTerr from {0} where id = {1}", regionEntity.FullDBName, refRegion), QueryResultTypes.DataTable);
                regionData.Columns.Add("Date", typeof(string));
                regionData.Rows[0][2] = calculateDate.AddDays(1).ToShortDateString();
                tables[0] = guaranteeTable;
                tables[1] = settlementsGuaranteeTable;
                tables[2] = regionData;
            }
        }

        public DataTable GetStocksData(int region)
        {
            IEntity regionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable regionData = (DataTable)db.ExecQuery(string.Format("select name from {0} where id = {1}", regionEntity.FullDBName, region), QueryResultTypes.DataTable);
                regionData.Columns.Add("Date", typeof(string));
                regionData.Rows[0][1] = DateTime.Today.ToShortDateString();
                return regionData;
            }
        }

        public DataTable GetOrgCreditsFullData(int refVariant)
        {
            IEntity creditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            IEntity settlementsCreditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            IEntity regionsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);

            int flagTotalSummary = -10;
            
            // запрос по районам
            string orgRegionsCreditsQuery =
                "select region.RefTerr, region.ID, region.Name, Sum(credit.Attract) as Attract, Sum(credit.PlanService) as PlanService, Sum(credit.Discharge) as Discharge, Sum(credit.FactService) as FactService, " +
                "Sum(credit.CapitalDebt) as CapitalDebt, Sum(credit.ServiceDebt) as ServiceDebt, Sum(credit.StaleDebt) as StaleDebt from {0} credit, {1} region " +
                "where region.ID = credit.RefRegion and (region.refterr = 4 or region.refterr = 7) " +
                "and credit.RefVariant = {2} and credit.RefTypeCredit = 0 GROUP BY region.RefTerr, region.ID, region.Name order by region.Name";

            // запрос по поселениям
            string orgSettlementsCreditsQuery =
                "select region2.RefTerr, region2.id, region2.Name, Sum(settlement.Attract) as Attract, Sum(settlement.PlanService) as PlanService, " +
                "Sum(settlement.Discharge) as Discharge, Sum(settlement.FactService) as FactService, " +
                "Sum(settlement.CapitalDebt) as CapitalDebt, Sum(settlement.ServiceDebt) as ServiceDebt, " +
                "Sum(settlement.StaleDebt) as StaleDebt " +
                "from {0} settlement, {1} region, {1} region2 " +
                "where settlement.RefVariant = {2} and settlement.RefTypeCredit = 0 and settlement.refregion = region.id " +
                " and region2.refterr = 4 and " +
                "region.parentid in (select region3.id from {1} region3 where region3.parentid IN " +
                "(select region4.ID from {1} region4 where region4.id = region2.id)) " +
                "group by region2.RefTerr, region2.id, region2.Name order by region2.Name";

            // запрос по отдельным поселениям
            string orgSettlementCreditsQuery =
                "select region.RefTerr, region2.id, region.id, region.Name, Sum(settlement.Attract) as Attract, Sum(settlement.PlanService) as PlanService, " +
                "Sum(settlement.Discharge) as Discharge, Sum(settlement.FactService) as FactService, " +
                "Sum(settlement.CapitalDebt) as CapitalDebt, Sum(settlement.ServiceDebt) as ServiceDebt, " +
                "Sum(settlement.StaleDebt) as StaleDebt " +
                "from {0} settlement, {1} region, {1} region2 " +
                "where settlement.RefVariant = {2} and settlement.RefTypeCredit = 0 and settlement.refregion = region.id " +
                " and (region2.refterr = 4 or region2.refterr = 7) and " +
                "region.parentid in (select region3.id from {1} region3 where region3.parentid IN " +
                "(select region4.ID from {1} region4 where region4.id = region2.id)) " +
                "group by region2.id, region.RefTerr, region.id, region.Name order by region.Name";

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable[] dts = new DataTable[2];
                dts[0] = (DataTable)db.ExecQuery(
                    string.Format(
                        orgRegionsCreditsQuery,
                        creditsEntity.FullDBName,
                        regionsEntity.FullDBName,
                        refVariant),
                    QueryResultTypes.DataTable);
                dts[1] = (DataTable)db.ExecQuery(
                    string.Format(
                        orgSettlementsCreditsQuery,
                        settlementsCreditsEntity.FullDBName,
                        regionsEntity.FullDBName,
                        refVariant),
                    QueryResultTypes.DataTable);
                DataTable tblSettlement = (DataTable)
                    db.ExecQuery(
                        string.Format(
                            orgSettlementCreditsQuery,
                            settlementsCreditsEntity.FullDBName,
                            regionsEntity.FullDBName,
                            refVariant),
                        QueryResultTypes.DataTable);

                DataTable tblRegions =
                    (DataTable)db.ExecQuery(string.Format("select id, refterr from {0} where refterr = 4 or refterr = 7 order by code ASC", regionsEntity.FullDBName), QueryResultTypes.DataTable);
                DataTable tblRegionData = dts[0].Clone();
                int i = 1;

                foreach (DataRow region in tblRegions.Rows)
                {
                    DataRow newRow = null;
                    DataRow[] orgRegionRows = dts[0].Select(string.Format("ID = {0}", region[0]));
                    DataRow[] orgSelltementsRows = dts[1].Select(string.Format("ID = {0}", region[0]));
                    DataRow[] orgSelltementRows = tblSettlement.Select(string.Format("ID = {0}", region[0]));
                    FillEmptyCells(orgRegionRows);
                    FillEmptyCells(orgSelltementsRows);

                    if (orgRegionRows.Length > 0)
                    {
                        if (orgSelltementsRows.Length > 0)
                        {
                            orgSelltementsRows[0]["Attract"] = Convert.ToDecimal(orgSelltementsRows[0]["Attract"]) +
                                                      Convert.ToDecimal(orgRegionRows[0]["Attract"]);
                            orgSelltementsRows[0]["Discharge"] = Convert.ToDecimal(orgSelltementsRows[0]["Discharge"]) +
                                                      Convert.ToDecimal(orgRegionRows[0]["Discharge"]);
                            orgSelltementsRows[0]["PlanService"] = Convert.ToDecimal(orgSelltementsRows[0]["PlanService"]) +
                                                      Convert.ToDecimal(orgRegionRows[0]["PlanService"]);
                            orgSelltementsRows[0]["FactService"] = Convert.ToDecimal(orgSelltementsRows[0]["FactService"]) +
                                                      Convert.ToDecimal(orgRegionRows[0]["FactService"]);
                            orgSelltementsRows[0]["CapitalDebt"] = Convert.ToDecimal(orgSelltementsRows[0]["CapitalDebt"]) +
                                                      Convert.ToDecimal(orgRegionRows[0]["CapitalDebt"]);
                            orgSelltementsRows[0]["ServiceDebt"] = Convert.ToDecimal(orgSelltementsRows[0]["ServiceDebt"]) +
                                                      Convert.ToDecimal(orgRegionRows[0]["ServiceDebt"]);
                            orgSelltementsRows[0]["StaleDebt"] = Convert.ToDecimal(orgSelltementsRows[0]["StaleDebt"]) +
                                                      Convert.ToDecimal(orgRegionRows[0]["StaleDebt"]);
                            newRow = tblRegionData.Rows.Add(orgSelltementsRows[0].ItemArray);
                            newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                            tblRegionData.Rows.Add(orgRegionRows[0].ItemArray);
                        }
                        else
                        {
                            newRow = tblRegionData.Rows.Add(orgRegionRows[0].ItemArray);
                            newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                            if (Convert.ToInt32(orgRegionRows[0][0]) == 4)
                            {
                                tblRegionData.Rows.Add(orgRegionRows[0].ItemArray);
                            }
                        }
                    }
                    else if (orgRegionRows.Length == 0 && orgSelltementsRows.Length > 0)
                    {
                        newRow = tblRegionData.Rows.Add(orgSelltementsRows[0].ItemArray);
                        newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                        tblRegionData.Rows.Add(orgSelltementsRows[0][0], orgSelltementsRows[0][1], orgSelltementsRows[0][2], 0, 0, 0, 0, 0, 0, 0);
                    }

                    if (newRow != null)
                    {
                        newRow[1] = i++;
                    }

                    if (orgSelltementRows.Length > 0)
                    {
                        DataRow settlementsResult = tblRegionData.NewRow();
                        GetSettlementsResult(settlementsResult, orgSelltementRows);
                        foreach (DataRow row in orgSelltementRows)
                        {
                            tblRegionData.Rows.Add(
                                row[0], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10]);
                        }

                        tblRegionData.Rows.Add(settlementsResult);
                    }
                }

                DataRow[] rowsSummary = tblRegionData.Select(String.Format("{0} <= {1}", tblRegionData.Columns[0].ColumnName, flagTotalSummary));
                DataRow rowSummary = tblRegionData.Rows.Add(3, 0, 0, 0, 0, 0, 0, 0, 0, 0);

                foreach (DataRow rowData in rowsSummary)
                {
                    for (int c = 3; c < tblRegionData.Columns.Count; c++)
                    {
                        rowSummary[c] = Convert.ToDouble(rowSummary[c]) + Convert.ToDouble(rowData[c]);
                    }
                }

                return tblRegionData;
            }
        }

        /// <summary>
        /// получение данных по кредитам
        /// </summary>
        public DataTable GetBudCreditsFullData(int refVariant)
        {
            IEntity creditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            IEntity settlementsCreditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            IEntity regionsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);

            // данные по районам
            const string BudRegionsCreditsQuery =
                "select region.RefTerr, region.ID, region.Name, Sum(credit.Attract) as Attract, Sum(credit.PlanService) as PlanService, Sum(credit.Discharge) as Discharge, Sum(credit.FactService) as FactService, " +
                "Sum(credit.CapitalDebt) as CapitalDebt, Sum(credit.ServiceDebt) as ServiceDebt, Sum(credit.StaleDebt) as StaleDebt from {0} credit, {1} region " +
                "where region.ID = credit.RefRegion and (region.refterr = 4 or region.refterr = 7) " +
                "and credit.RefVariant = {2} and credit.RefTypeCredit = 1 GROUP BY region.RefTerr, region.ID, region.Name order by region.Name";

            const string BudSettlementsCreditsQuery =
                "select region2.RefTerr, region2.id, region2.Name, Sum(settlement.Attract) as Attract, Sum(settlement.PlanService) as PlanService, " +
                "Sum(settlement.Discharge) as Discharge, Sum(settlement.FactService) as FactService, " +
                "Sum(settlement.CapitalDebt) as CapitalDebt, Sum(settlement.ServiceDebt) as ServiceDebt, " +
                "Sum(settlement.StaleDebt) as StaleDebt " +
                "from {0} settlement, {1} region, {1} region2 " +
                "where settlement.RefVariant = {2} and settlement.RefTypeCredit = 1 and" +
                " settlement.refregion = region.id and region2.refterr = 4 and " +
                "region.parentid in (select region3.id from {1} region3 where region3.parentid IN " +
                "(select region4.ID from {1} region4 where region4.id = region2.id)) " +
                "group by region2.RefTerr, region2.id, region2.Name order by region2.Name";

            const string BudSettlementCreditsQuery =
                "select region.RefTerr, region2.id, region.id, region.Name, Sum(settlement.Attract) as Attract, Sum(settlement.PlanService) as PlanService, " +
                "Sum(settlement.Discharge) as Discharge, Sum(settlement.FactService) as FactService, " +
                "Sum(settlement.CapitalDebt) as CapitalDebt, Sum(settlement.ServiceDebt) as ServiceDebt, " +
                "Sum(settlement.StaleDebt) as StaleDebt " +
                "from {0} settlement, {1} region, {1} region2 " +
                "where settlement.RefVariant = {2} and settlement.RefTypeCredit = 1 and settlement.refregion = region.id " +
                " and (region2.refterr = 4 or region2.refterr = 7) and " +
                "region.parentid in (select region3.id from {1} region3 where region3.parentid IN " +
                "(select region4.ID from {1} region4 where region4.id = region2.id)) " +
                "group by region.RefTerr, region2.id, region.id, region.Name order by region.Name";

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable[] dts = new DataTable[2];
                dts[0] = (DataTable)db.ExecQuery(
                    string.Format(
                        BudRegionsCreditsQuery,
                        creditsEntity.FullDBName,
                        regionsEntity.FullDBName,
                        refVariant),
                    QueryResultTypes.DataTable);
                dts[1] = (DataTable)db.ExecQuery(
                    string.Format(
                        BudSettlementsCreditsQuery,
                        settlementsCreditsEntity.FullDBName,
                        regionsEntity.FullDBName,
                        refVariant),
                    QueryResultTypes.DataTable);
                DataTable tblSettlement = (DataTable)
                    db.ExecQuery(
                        string.Format(
                            BudSettlementCreditsQuery,
                            settlementsCreditsEntity.FullDBName,
                            regionsEntity.FullDBName,
                            refVariant),
                        QueryResultTypes.DataTable);
                DataTable tblRegions =
                    (DataTable)db.ExecQuery(string.Format("select id from {0} where refterr = 4 or refterr = 7 order by code ASC", regionsEntity.FullDBName), QueryResultTypes.DataTable);
                DataTable tblRegionData = dts[0].Clone();
                int i = 1;
                int flagTotalSummary = -10;
                foreach (DataRow region in tblRegions.Rows)
                {
                    DataRow newRow = null;
                    DataRow[] budRegionRows = dts[0].Select(string.Format("ID = {0}", region[0]));
                    DataRow[] budSelltementsRows = dts[1].Select(string.Format("ID = {0}", region[0]));
                    DataRow[] budSelltementRows = tblSettlement.Select(string.Format("ID = {0}", region[0]));

                    FillEmptyCells(budRegionRows);
                    FillEmptyCells(budSelltementsRows);

                    if (budRegionRows.Length > 0)
                    {
                        if (budSelltementsRows.Length > 0)
                        {
                            budSelltementsRows[0]["Attract"] = Convert.ToDecimal(budSelltementsRows[0]["Attract"]) +
                                                      Convert.ToDecimal(budRegionRows[0]["Attract"]);
                            budSelltementsRows[0]["Discharge"] = Convert.ToDecimal(budSelltementsRows[0]["Discharge"]) +
                                                      Convert.ToDecimal(budRegionRows[0]["Discharge"]);
                            budSelltementsRows[0]["PlanService"] = Convert.ToDecimal(budSelltementsRows[0]["PlanService"]) +
                                                      Convert.ToDecimal(budRegionRows[0]["PlanService"]);
                            budSelltementsRows[0]["FactService"] = Convert.ToDecimal(budSelltementsRows[0]["FactService"]) +
                                                      Convert.ToDecimal(budRegionRows[0]["FactService"]);
                            budSelltementsRows[0]["CapitalDebt"] = Convert.ToDecimal(budSelltementsRows[0]["CapitalDebt"]) +
                                                      Convert.ToDecimal(budRegionRows[0]["CapitalDebt"]);
                            budSelltementsRows[0]["ServiceDebt"] = Convert.ToDecimal(budSelltementsRows[0]["ServiceDebt"]) +
                                                      Convert.ToDecimal(budRegionRows[0]["ServiceDebt"]);
                            budSelltementsRows[0]["StaleDebt"] = Convert.ToDecimal(budSelltementsRows[0]["StaleDebt"]) +
                                                      Convert.ToDecimal(budRegionRows[0]["StaleDebt"]);
                            newRow = tblRegionData.Rows.Add(budSelltementsRows[0].ItemArray);
                            newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                            tblRegionData.Rows.Add(budRegionRows[0].ItemArray);
                        }
                        else
                        {
                            newRow = tblRegionData.Rows.Add(budRegionRows[0].ItemArray);
                            newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                            if (Convert.ToInt32(budRegionRows[0][0]) == 4)
                            {
                                tblRegionData.Rows.Add(budRegionRows[0].ItemArray);
                            }
                        }
                    }
                    else if (budRegionRows.Length == 0 && budSelltementsRows.Length > 0)
                    {
                        newRow = tblRegionData.Rows.Add(budSelltementsRows[0].ItemArray);
                        newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                        tblRegionData.Rows.Add(budSelltementsRows[0][0], budSelltementsRows[0][1], budSelltementsRows[0][2], 0, 0, 0, 0, 0, 0, 0);
                    }

                    if (newRow != null)
                    {
                        newRow[1] = i++;
                    }

                    if (budSelltementRows.Length > 0)
                    {
                        DataRow settlementsResult = tblRegionData.NewRow();
                        GetSettlementsResult(settlementsResult, budSelltementRows);
                        foreach (DataRow row in budSelltementRows)
                        {
                            tblRegionData.Rows.Add(
                                row[0], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10]);
                        }

                        tblRegionData.Rows.Add(settlementsResult);
                    }
                }

                DataRow[] rowsSummary = tblRegionData.Select(String.Format("{0} <= {1}", tblRegionData.Columns[0].ColumnName, flagTotalSummary));
                DataRow rowSummary = tblRegionData.Rows.Add(3, 0, 0, 0, 0, 0, 0, 0, 0, 0);

                foreach (DataRow rowData in rowsSummary)
                {
                    for (int c = 3; c < tblRegionData.Columns.Count; c++)
                    {
                        rowSummary[c] = Convert.ToDouble(rowSummary[c]) + Convert.ToDouble(rowData[c]);
                    }
                }

                return tblRegionData;
            }
        }

        /// <summary>
        /// получение данных по гарантиям
        /// </summary>
        public DataTable GetGuaranteeTotalData(int refVariant)
        {
            IEntity guaranteeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            IEntity settlementsGuaranteeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissuedPos);
            IEntity regionsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            int flagTotalSummary = -10;

            // данные по районам
            string regionsGuaranteeQuery =
                "select region.RefTerr, region.ID, region.Name, Sum(guarantee.UpDebt), Sum(guarantee.UpService), Sum(guarantee.DownDebt), Sum(guarantee.DownService), " +
                "Sum(guarantee.TotalDebt), Sum(guarantee.CapitalDebt), Sum(guarantee.ServiceDebt), Sum(guarantee.StalePrincipalDebt), Sum(guarantee.StaleGarantDebt) " +
                "from {0} guarantee, {1} region " +
                "where region.ID = guarantee.RefRegion and (region.refterr = 4 or region.refterr = 7) " +
                "and guarantee.RefVariant = {2} GROUP BY region.RefTerr, region.ID, region.Name  order by region.Name";

            // по поселениям
            string settlementsGuaranteesQuery =
                "select region2.RefTerr, region2.ID, region2.Name, Sum(settlement.UpDebt), Sum(settlement.UpService), Sum(settlement.DownDebt), Sum(settlement.DownService), " +
                "Sum(settlement.TotalDebt), Sum(settlement.CapitalDebt), Sum(settlement.ServiceDebt), Sum(settlement.StalePrincipalDebt), Sum(settlement.StaleGarantDebt) " +
                "from {0} settlement, {1} region, {1} region2 " +
                "where settlement.RefVariant = {2} and settlement.refregion = region.id " +
                " and region2.refterr = 4 and " +
                "region.parentid in (select region3.id from {1} region3 where region3.parentid IN " +
                "(select region4.ID from {1} region4 where region4.id = region2.id)) " +
                "group by region2.RefTerr, region2.id, region2.Name  order by region2.Name";

            string settlementsGuaranteeQuery =
                "select region.RefTerr, region2.ID, region.ID, region.Name, Sum(settlement.UpDebt), Sum(settlement.UpService), Sum(settlement.DownDebt), Sum(settlement.DownService), " +
                "Sum(settlement.TotalDebt), Sum(settlement.CapitalDebt), Sum(settlement.ServiceDebt), Sum(settlement.StalePrincipalDebt), Sum(settlement.StaleGarantDebt) " +
                "from {0} settlement, {1} region, {1} region2 " +
                "where settlement.RefVariant = {2} and settlement.refregion = region.id " +
                " and (region2.refterr = 4 or region2.refterr = 7) and " +
                "region.parentid in (select region3.id from {1} region3 where region3.parentid IN " +
                "(select region4.ID from {1} region4 where region4.id = region2.id)) " +
                "group by region.RefTerr, region2.id, region.id, region.Name order by region.Name";

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable[] tables = new DataTable[2];
                tables[0] =
                    (DataTable)db.ExecQuery(string.Format(regionsGuaranteeQuery, guaranteeEntity.FullDBName, regionsEntity.FullDBName, refVariant), QueryResultTypes.DataTable);
                tables[1] =
                    (DataTable)db.ExecQuery(string.Format(settlementsGuaranteesQuery, settlementsGuaranteeEntity.FullDBName, regionsEntity.FullDBName, refVariant), QueryResultTypes.DataTable);
                DataTable tblSettlement = (DataTable)db.ExecQuery(string.Format(settlementsGuaranteeQuery, settlementsGuaranteeEntity.FullDBName, regionsEntity.FullDBName, refVariant), QueryResultTypes.DataTable);

                DataTable tblRegions =
                    (DataTable)db.ExecQuery(string.Format("select id from {0} where refterr = 4 or refterr = 7 order by code ASC", regionsEntity.FullDBName), QueryResultTypes.DataTable);
                DataTable resultTable = tables[0].Clone();
                int i = 1;

                foreach (DataRow region in tblRegions.Rows)
                {
                    DataRow newRow = null;
                    DataRow[] regionRows = tables[0].Select(string.Format("ID = {0}", region[0]));
                    DataRow[] settlementsRows = tables[1].Select(string.Format("ID = {0}", region[0]));
                    DataRow[] settlementRows = tblSettlement.Select(string.Format("ID = {0}", region[0]));

                    FillEmptyCells(regionRows);
                    FillEmptyCells(settlementsRows);
                    if (regionRows.Length > 0)
                    {
                        if (settlementsRows.Length > 0)
                        {
                            settlementsRows[0][3] = Convert.ToDecimal(settlementsRows[0][3]) +
                                                   Convert.ToDecimal(regionRows[0][3]);
                            settlementsRows[0][4] = Convert.ToDecimal(settlementsRows[0][4]) +
                                                   Convert.ToDecimal(regionRows[0][4]);
                            settlementsRows[0][5] = Convert.ToDecimal(settlementsRows[0][5]) +
                                                   Convert.ToDecimal(regionRows[0][5]);
                            settlementsRows[0][6] = Convert.ToDecimal(settlementsRows[0][6]) +
                                                   Convert.ToDecimal(regionRows[0][6]);
                            settlementsRows[0][7] = Convert.ToDecimal(settlementsRows[0][7]) +
                                                   Convert.ToDecimal(regionRows[0][7]);
                            settlementsRows[0][8] = Convert.ToDecimal(settlementsRows[0][8]) +
                                                   Convert.ToDecimal(regionRows[0][8]);
                            settlementsRows[0][9] = Convert.ToDecimal(settlementsRows[0][9]) +
                                                   Convert.ToDecimal(regionRows[0][9]);
                            settlementsRows[0][10] = Convert.ToDecimal(settlementsRows[0][10]) +
                                                   Convert.ToDecimal(regionRows[0][10]);
                            settlementsRows[0][11] = Convert.ToDecimal(settlementsRows[0][11]) +
                                                    Convert.ToDecimal(regionRows[0][11]);
                            newRow = resultTable.Rows.Add(settlementsRows[0].ItemArray);
                            newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                            resultTable.Rows.Add(regionRows[0].ItemArray);
                        }
                        else
                        {
                            newRow = resultTable.Rows.Add(regionRows[0].ItemArray);
                            newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                            if (Convert.ToInt32(regionRows[0][0]) == 4)
                            {
                                resultTable.Rows.Add(regionRows[0].ItemArray);
                            }
                        }
                    }
                    else if (regionRows.Length == 0 && settlementsRows.Length > 0)
                    {
                        newRow = resultTable.Rows.Add(settlementsRows[0].ItemArray);
                        newRow[0] = flagTotalSummary - Convert.ToInt32(newRow[0]);
                        resultTable.Rows.Add(settlementsRows[0][0], settlementsRows[0][1], settlementsRows[0][2], 0, 0, 0, 0, 0, 0, 0);
                    }

                    if (newRow != null)
                    {
                        newRow[1] = i++;
                    }

                    if (settlementRows.Length > 0)
                    {
                        DataRow settlementsResult = resultTable.NewRow();
                        GetSettlementsResult(settlementsResult, settlementRows);
                        foreach (DataRow row in settlementRows)
                        {
                            resultTable.Rows.Add(
                                row[0], row[1], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10], row[11], row[12]);
                        }

                        resultTable.Rows.Add(settlementsResult);
                    }
                }

                DataRow[] rowsSummary = resultTable.Select(String.Format("{0} <= {1}", resultTable.Columns[0].ColumnName, flagTotalSummary));
                DataRow rowSummary = resultTable.Rows.Add(3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

                foreach (DataRow rowData in rowsSummary)
                {
                    for (int c = 3; c < resultTable.Columns.Count; c++)
                    {
                        rowSummary[c] = Convert.ToDouble(rowSummary[c]) + Convert.ToDouble(rowData[c]);
                    }
                }

                return resultTable;
            }
        }

        /// <summary>
        /// все данные по субъекту
        /// </summary>
        public DataTable GetTotalSubjectData(int refVariant)
        {
            IEntity creditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            IEntity guaranteeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            IEntity regionsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);

            // получаем данные по субъекту
            const string SubjectGuaranteeQuery =
                "select Sum(UpDebt), Sum(UpService), Sum(DownDebt), Sum(DownService), Sum(CapitalDebt), " +
                "Sum(ServiceDebt), Sum(StalePrincipalDebt + StaleGarantDebt) from {0} where RefVariant = {1} and " +
                "RefRegion in (select id from {2} where refterr in (3))";

            // получаем данные по субъекту
            const string OrgSubjectCreditsQuery =
                "select Sum(Attract), Sum(PlanService), Sum(Discharge), Sum(FactService), Sum(CapitalDebt), Sum(ServiceDebt), Sum(StaleDebt) from {0} where RefVariant = {1} and RefTypeCredit = 0 and " +
                "RefRegion in (select id from {2} where refterr in (3))";

            const string BudSubjectCreditsQuery =
                "select Sum(Attract), Sum(PlanService), Sum(Discharge), Sum(FactService), Sum(CapitalDebt), Sum(ServiceDebt), Sum(StaleDebt) from {0} where RefVariant = {1} and RefTypeCredit = 1 and " +
                "RefRegion in (select id from {2} where refterr in (3))";

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable dt1 =
                    (DataTable)db.ExecQuery(string.Format(OrgSubjectCreditsQuery, creditsEntity.FullDBName, refVariant, regionsEntity.FullDBName), QueryResultTypes.DataTable);

                DataTable dt2 =
                    (DataTable)db.ExecQuery(string.Format(BudSubjectCreditsQuery, creditsEntity.FullDBName, refVariant, regionsEntity.FullDBName), QueryResultTypes.DataTable);

                DataTable dt3 =
                    (DataTable)db.ExecQuery(string.Format(SubjectGuaranteeQuery, guaranteeEntity.FullDBName, refVariant, regionsEntity.FullDBName), QueryResultTypes.DataTable);

                // добавим итоговую запись
                if (dt1.Rows.Count == 0)
                {
                    dt1.Rows.Add(0, 0, 0, 0, 0, 0, 0);
                }
                else if (dt1.Rows[0].IsNull(0))
                {
                    dt1.Rows[0].Delete();
                    dt1.AcceptChanges();
                    dt1.Rows.Add(0, 0, 0, 0, 0, 0, 0);
                }

                if (dt2.Rows.Count > 0 && !dt2.Rows[0].IsNull(0))
                {
                    dt1.Rows.Add(dt2.Rows[0].ItemArray);
                }
                else
                {
                    dt1.Rows.Add(0, 0, 0, 0, 0, 0, 0);
                }

                if (dt3.Rows.Count > 0 && !dt3.Rows[0].IsNull(0))
                {
                    dt1.Rows.Add(dt3.Rows[0].ItemArray);
                }
                else
                {
                    dt1.Rows.Add(0, 0, 0, 0, 0, 0, 0);
                }

                return dt1;
            }
        }

        public DataTable[] GetSubjectReportData(int regionID, int variantID, string reportDate)
        {
            // шаблон запроса на получение данных по гарантиям
            const string GuaranteeSubjectQuery = "select grnt.ID, grnt.RegNum, contract.Name, grnt.Num, grnt.StartDate, grnt.FurtherConvention, " +
                "grnt.Occasion, org.Name, org3.Name, grnt.Collateral, grnt.Sum, okv.Name, grnt.DateDoc, grnt.EndDate, grnt.Collateral, grnt.Regress, " +
                "grnt.UpDebt, grnt.UpService, grnt.DownDebt, grnt.DownService, grnt.DownGarant, grnt.TotalDebt, " +
                "grnt.CapitalDebt, grnt.ServiceDebt, grnt.StalePrincipalDebt, grnt.StaleGarantDebt " +
                "from {0} grnt, {1} contract, {2} org, {2} org3, {3} okv " +
                "where grnt.RefVariant = {4} and grnt.RefRegion = {5} and grnt.RefOrganizations = org.ID and grnt.RefOrganizationsPlan3 = org3.ID " +
                "and grnt.RefOKV = okv.ID and grnt.RefTypeContract = contract.ID order by grnt.RegNum";

            // шаблон запроса на получение данных по кредитам полученным
            const string BudgetSubjectQuery = "select credit.ID, credit.RegNum, contract.Name, credit.Num, credit.ContractDate, credit.FurtherConvention, " +
                "credit.Occasion, org.Name, credit.Sum, credit.CreditPercent, credit.EndDate, credit.Collateral, credit.StartDate, credit.FactDate, " +
                "credit.Attract, credit.Discharge, credit.PlanService, credit.FactService, credit.CapitalDebt, " +
                "credit.ServiceDebt, credit.StaleDebt " +
                "from {0} credit, {1} contract, {2} org " +
                "where credit.RefVariant = {3} and credit.RefRegion = {4} and credit.RefTypeCredit = {5} and credit.RefTypeContract = contract.ID " +
                "and org.id = credit.reforganizations order by credit.RegNum";

            // шаблон запроса по ценным бумагам
            DataTable[] tables = new DataTable[4];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntity guaranteeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
                IEntity creditEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
                IEntity contractEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_TypeContract);
                IEntity orgEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Organizations_Plan);
                IEntity okvEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_OKV_Currency);
                tables[0] = (DataTable)db.ExecQuery(
                    string.Format(
                        BudgetSubjectQuery,
                        creditEntity.FullDBName,
                        contractEntity.FullDBName,
                        orgEntity.FullDBName,
                        variantID,
                        regionID,
                        0),
                    QueryResultTypes.DataTable);
                tables[1] = (DataTable)db.ExecQuery(
                    string.Format(
                        BudgetSubjectQuery,
                        creditEntity.FullDBName,
                        contractEntity.FullDBName,
                        orgEntity.FullDBName,
                        variantID,
                        regionID,
                        1),
                    QueryResultTypes.DataTable);
                tables[2] = (DataTable)db.ExecQuery(
                    string.Format(
                        GuaranteeSubjectQuery,
                        guaranteeEntity.FullDBName,
                        contractEntity.FullDBName,
                        orgEntity.FullDBName,
                        okvEntity.FullDBName,
                        variantID,
                        regionID),
                    QueryResultTypes.DataTable);
                int index = 1;
                foreach (DataRow row in tables[0].Rows)
                {
                    row["ID"] = index++;
                    string contractDate = row.IsNull("ContractDate")
                        ? string.Empty
                        : Convert.ToDateTime(row["ContractDate"]).ToShortDateString();
                    row["Name"] = row["Name"] + " " + row["Num"] + " от " + contractDate +
                        (row.IsNull("FurtherConvention") ? string.Empty : string.Format(" с дополнительными соглашениями  {0}", row["FurtherConvention"]));
                    row["Name"] = row["Name"].ToString().Replace("\r", string.Empty);
                }

                tables[0].Columns.Remove("Num");
                tables[0].Columns.Remove("ContractDate");
                tables[0].Columns.Remove("FurtherConvention");
                index = 1;
                foreach (DataColumn column in tables[1].Columns)
                {
                    column.AllowDBNull = true;
                    column.ReadOnly = false;
                }

                foreach (DataRow row in tables[1].Rows)
                {
                    row["ID"] = index++;
                    string contractDate = row.IsNull("ContractDate")
                        ? string.Empty
                        : Convert.ToDateTime(row["ContractDate"]).ToShortDateString();
                    row["Name"] = row["Name"] + " " + row["Num"] + " от " + contractDate +
                        (row.IsNull("FurtherConvention") ? string.Empty : string.Format(" с дополнительными соглашениями  {0}", row["FurtherConvention"]));
                    row["Name"] = row["Name"].ToString().Replace("\r", string.Empty);
                }

                tables[1].Columns.Remove("Num");
                tables[1].Columns.Remove("ContractDate");
                tables[1].Columns.Remove("FurtherConvention");
                index = 1;
                foreach (DataRow row in tables[2].Rows)
                {
                    row["ID"] = index++;
                    string startDate = row.IsNull("StartDate")
                        ? string.Empty : Convert.ToDateTime(row["StartDate"]).ToShortDateString();
                    row["Name"] = row["Name"] + " " + row["Num"] + " от " + startDate +
                        (row.IsNull("FurtherConvention") ? string.Empty : string.Format(" с дополнительными соглашениями  {0}", row["FurtherConvention"]));
                    row["Name"] = row["Name"].ToString().Replace("\r", string.Empty);
                    row["Collateral"] = DBNull.Value;
                }

                tables[2].Columns.Remove("Num");
                tables[2].Columns.Remove("StartDate");
                tables[2].Columns.Remove("FurtherConvention");

                tables[3] = new DataTable();
                tables[3].Columns.Add("AllFactDebt", typeof(decimal));
                tables[3].Columns.Add("MainFactDebt", typeof(decimal));
                tables[3].Columns.Add("CerviceFactDebt", typeof(decimal));
                tables[3].Columns.Add("ReportDate", typeof(string));
                DataRow totalRow = tables[3].NewRow();
                decimal allFactDebt = 0;
                decimal mainFactDebt = 0;
                decimal cerviceFactDebt = 0;
                foreach (DataRow row in tables[0].Rows)
                {
                    allFactDebt += row.IsNull("CapitalDebt") ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    allFactDebt += row.IsNull("ServiceDebt") ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                    mainFactDebt += row.IsNull("CapitalDebt") ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                }

                foreach (DataRow row in tables[1].Rows)
                {
                    allFactDebt += row.IsNull("CapitalDebt") ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    allFactDebt += row.IsNull("ServiceDebt") ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                    mainFactDebt += row.IsNull("CapitalDebt") ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                }

                foreach (DataRow row in tables[2].Rows)
                {
                    allFactDebt += row.IsNull("TotalDebt") ? 0 : Convert.ToDecimal(row["TotalDebt"]);
                    mainFactDebt += row.IsNull("CapitalDebt") ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    cerviceFactDebt += row.IsNull("ServiceDebt") ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                }

                totalRow[0] = mainFactDebt + cerviceFactDebt;
                totalRow[1] = mainFactDebt;
                totalRow[2] = cerviceFactDebt;
                totalRow[3] = reportDate;

                tables[3].Rows.Add(totalRow);
            }

            return tables;
        }

        private void FillEmptyCells(DataRow[] rows)
        {
            foreach (DataRow row in rows)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (column.DataType == typeof(decimal) && row.IsNull(column))
                    {
                        row[column] = 0;
                    }
                }
            }
        }

        private DataTable GetCreditSettlementsData(string creditQuery, int refRegion, int refVariant)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            IEntity regionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            string regionsQuery = string.Format("select id from {0} where ParentID IN (select id from {0} where ParentID = {1})", regionEntity.FullDBName, refRegion);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable tblSettlements =
                    (DataTable)db.ExecQuery(string.Format(creditQuery, entity.FullDBName, regionEntity.FullDBName, refVariant, regionsQuery), QueryResultTypes.DataTable);
                if (tblSettlements.Rows.Count == 0)
                {
                    return tblSettlements;
                }

                decimal attract = 0;
                decimal discharge = 0;
                decimal planService = 0;
                decimal factService = 0;
                decimal capitalDebt = 0;
                decimal serviceDebt = 0;
                decimal staleDebt = 0;
                int index = 1;
                foreach (DataColumn column in tblSettlements.Columns)
                {
                    column.AllowDBNull = true;
                    column.ReadOnly = false;
                }

                foreach (DataRow row in tblSettlements.Rows)
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

                DataRow newRow = tblSettlements.NewRow();
                newRow.ItemArray = tblSettlements.Rows[0].ItemArray;
                newRow["Attract"] = attract;
                newRow["Discharge"] = discharge;
                newRow["PlanService"] = planService;
                newRow["FactService"] = factService;
                newRow["CapitalDebt"] = capitalDebt;
                newRow["ServiceDebt"] = serviceDebt;
                newRow["StaleDebt"] = staleDebt;
                newRow[0] = index;
                tblSettlements.Rows.Add(newRow);
                return tblSettlements;
            }
        }
        
        private void GetCreditData(int refVariant, int refRegion, int creditType, ref DataTable[] tables, DateTime calculateDate)
        {
            string creditQuery = "select credit.ID, credit.RegNum, credittype.Name, credit.Num, credit.ContractDate, credit.FurtherConvention, " +
                "credit.Occasion, organization.Name, credit.Sum, credit.ValueDebt, credit.CreditPercent, credittype.Name, credit.EndDate, credit.Collateral, " +
                "credittype.Name, credit.StartDate, credittype.Name, credit.FactDate, credit.Attract, credit.Discharge, credit.PlanService, credit.FactService, " +
                "credit.CapitalDebt, credit.ServiceDebt, credit.StaleDebt from {0} credit, d_S_TypeContract credittype, " +
                "d_Organizations_Plan organization where credit.RefTypeContract = credittype.ID and credit.RefOrganizations = organization.ID " +
                " and credit.RefTypeCredit = {1} and credit.RefRegion = {2} and credit.RefVariant = {3}";

            IEntity creditEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            IEntity regionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable creditTable = (DataTable)db.ExecQuery(string.Format(creditQuery, creditEntity.FullDBName, creditType, refRegion, refVariant), QueryResultTypes.DataTable);
                decimal attract = 0;
                decimal discharge = 0;
                decimal planService = 0;
                decimal factService = 0;
                decimal capitalDebt = 0;
                decimal serviceDebt = 0;
                decimal staleDebt = 0;
                int index = 1;

                foreach (DataColumn column in creditTable.Columns)
                {
                    column.AllowDBNull = true;
                    column.ReadOnly = false;
                }

                foreach (DataRow row in creditTable.Rows)
                {
                    row[0] = index++;
                    row[2] = row[2] + " " + row[3] + " от " + (row.IsNull(4) ? string.Empty : Convert.ToDateTime(row[4]).ToShortDateString()) + (row.IsNull(5) ? string.Empty : " " + row[5]);
                    row["Name2"] = row["EndDate"];
                    row["Name3"] = row["StartDate"];
                    row["Name4"] = row["FactDate"];
                    row["ValueDebt"] = row["Sum"] + (row.IsNull("ValueDebt") ? string.Empty : Environment.NewLine + row["ValueDebt"]);
                    row["ValueDebt"] = row["ValueDebt"].ToString().Replace("\r", string.Empty);
                    attract += row["Attract"] is DBNull ? 0 : Convert.ToDecimal(row["Attract"]);
                    discharge += row["Discharge"] is DBNull ? 0 : Convert.ToDecimal(row["Discharge"]);
                    planService += row["PlanService"] is DBNull ? 0 : Convert.ToDecimal(row["PlanService"]);
                    factService += row["FactService"] is DBNull ? 0 : Convert.ToDecimal(row["FactService"]);
                    capitalDebt += row["CapitalDebt"] is DBNull ? 0 : Convert.ToDecimal(row["CapitalDebt"]);
                    serviceDebt += row["ServiceDebt"] is DBNull ? 0 : Convert.ToDecimal(row["ServiceDebt"]);
                    staleDebt += row["StaleDebt"] is DBNull ? 0 : Convert.ToDecimal(row["StaleDebt"]);
                }

                if (creditTable.Rows.Count > 0)
                {
                    DataRow newRow = creditTable.NewRow();
                    newRow.ItemArray = creditTable.Rows[0].ItemArray;
                    newRow["Attract"] = attract;
                    newRow["Discharge"] = discharge;
                    newRow["PlanService"] = planService;
                    newRow["FactService"] = factService;
                    newRow["CapitalDebt"] = capitalDebt;
                    newRow["ServiceDebt"] = serviceDebt;
                    newRow["StaleDebt"] = staleDebt;
                    newRow[0] = index;
                    creditTable.Rows.Add(newRow);
                }

                creditTable.Columns.Remove(creditTable.Columns["Num"]);
                creditTable.Columns.Remove(creditTable.Columns["Sum"]);
                creditTable.Columns.Remove(creditTable.Columns["FurtherConvention"]);
                creditTable.Columns.Remove(creditTable.Columns["ContractDate"]);
                creditTable.Columns.Remove(creditTable.Columns["EndDate"]);
                creditTable.Columns.Remove(creditTable.Columns["StartDate"]);
                creditTable.Columns.Remove(creditTable.Columns["FactDate"]);
                creditTable.AcceptChanges();

                DataTable regionData = (DataTable)db.ExecQuery(string.Format("select name, RefTerr from {0} where id = {1}", regionEntity.FullDBName, refRegion), QueryResultTypes.DataTable);
                regionData.Columns.Add("Date", typeof(string));
                regionData.Rows[0][2] = calculateDate.AddDays(1).ToShortDateString();
                tables[0] = creditTable;
                tables[2] = regionData;
            }
        }

        /// <summary>
        /// Результат по всем поселениям одного из районов
        /// </summary>
        /// <param name="resultRow">Строка для вывода результата</param>
        /// <param name="settlementsRows">Строки данных поселений</param>
        private void GetSettlementsResult(DataRow resultRow, DataRow[] settlementsRows)
        {
            if (settlementsRows.Length == 0)
            {
                return;
            }

            resultRow[0] = settlementsRows[0][0];
            resultRow[1] = settlementsRows[0][1];
            resultRow[2] = "Итого по поселениям";
            foreach (DataRow row in settlementsRows)
            {
                for (int i = 3; i <= resultRow.Table.Columns.Count - 1; i++)
                {
                    resultRow[i] = (resultRow.IsNull(i) ? 0 : Convert.ToDecimal(resultRow[i])) + Convert.ToDecimal(row[i + 1]);
                }
            }
        }
    }
}
