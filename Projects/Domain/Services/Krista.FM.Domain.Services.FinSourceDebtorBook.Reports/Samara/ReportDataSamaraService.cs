using System;
using System.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Получение данных - Самара
    /// </summary>
    public partial class ReportsDataService
    {
        public void GetCreditSamaraData(
            int refVariant, 
            int refRegion, 
            ref DataTable[] tables,
            DateTime calculateDate, 
            int creditType)
        {
            tables = new DataTable[3];
            const string ContactQuery =
                "select {0} from {1} c where c.RefVariant = {2} and c.RefRegion in ({3}) and " +
                " c.RefTypeCredit = {4} order by c.ContractDate";

            // Основные таблицы
            var crdEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            using (var db = scheme.SchemeDWH.DB)
            {
                var tblContracts = (DataTable)db.ExecQuery(
                    String.Format(
                        ContactQuery,
                        GetFieldNames(crdEntity, "c"), 
                        crdEntity.FullDBName,
                        refVariant, 
                        refRegion, 
                        creditType), 
                    QueryResultTypes.DataTable);

                tables[0] = CreateReportCaptionTable(8);
                tables[1] = CreateReportCaptionTable(8);
                var i = 1;
                var totalSum = new double[2];

                foreach (DataRow dr in tblContracts.Rows)
                {
                    var tableIndex = 0;

                    if (Convert.ToInt32(dr["RefOKV"]) != -1)
                    {
                        tableIndex = 1;
                    }

                    var rowResult = tables[tableIndex].Rows.Add();
                    rowResult[0] = i++;
                    rowResult[1] = String.Format("{0} от {1}", dr["Num"], GetDateValue(dr["ContractDate"]));
                    rowResult[2] = dr["Creditor"];
                    rowResult[3] = GetBookValue(scheme, DomainObjectsKeys.d_OKV_Currency, dr["RefOKV"].ToString());
                    rowResult[4] = dr["StartDate"];
                    rowResult[5] = dr["CreditPercent"];
                    rowResult[6] = GetDateValue(dr["FactDate"]);
                    var sum = GetDoubleValue(dr["CapitalDebt"]) + GetDoubleValue(dr["ServiceDebt"]);
                    rowResult[7] = sum;
                    totalSum[tableIndex] += sum;
                }

                tables[2] = CreateReportCaptionTable(3);
                var rowSummary = tables[2].Rows.Add();
                rowSummary[0] = totalSum[0];
                rowSummary[1] = totalSum[1];
                rowSummary[2] = totalSum[0] + totalSum[1];
            }
        }

        public void GetOrgCreditSamaraData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            GetCreditSamaraData(refVariant, refRegion, ref tables, calculateDate, 0);
        }

        public void GetBudCreditSamaraData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            GetCreditSamaraData(refVariant, refRegion, ref tables, calculateDate, 1);
        }

        public void GetCapitalSamaraData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            tables = new DataTable[3];
            const string ContactQuery = "select {0} from {1} c where c.RefVariant = {2} and c.RefRegion in ({3})";

            // Основные таблицы
            var crdEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCapital);
            using (var db = scheme.SchemeDWH.DB)
            {
                var tblContracts = (DataTable)db.ExecQuery(
                    String.Format(
                        ContactQuery,
                        GetFieldNames(crdEntity, "c"), 
                        crdEntity.FullDBName, 
                        refVariant, 
                        refRegion),
                    QueryResultTypes.DataTable);

                tables[0] = CreateReportCaptionTable(24);
                tables[1] = CreateReportCaptionTable(24);
                var i = 1;
                var totalSum = new double[2];

                foreach (DataRow dr in tblContracts.Rows)
                {
                    var tableIndex = 0;

                    if (Convert.ToInt32(dr["RefOKV"]) != -1)
                    {
                        tableIndex = 1;
                    }

                    var rowResult = tables[tableIndex].Rows.Add();
                    rowResult[00] = i++;
                    rowResult[01] = dr["OfficialNumber"];
                    rowResult[02] = String.Format(
                        "{0} от {1}", 
                        dr["NameNPA"],
                        GetBookValue(scheme, DomainObjectsKeys.d_S_TypeContract, dr["RefSCap"].ToString()));
                    rowResult[03] = dr["FormCap"];
                    rowResult[04] = GetBookValue(scheme, DomainObjectsKeys.d_OKV_Currency, dr["RefOKV"].ToString());
                    rowResult[05] = String.Format("{1}, {0}", dr["RegNumber"], GetDateValue(dr["RegEmissionDate"]));
                    rowResult[06] = String.Format("{0} от {1}", dr["NumberNPA"], GetDateValue(dr["DateNPA"]));
                    rowResult[07] = dr["Owner"];
                    rowResult[08] = dr["Nominal"];
                    rowResult[09] = dr["Sum"];
                    rowResult[10] = GetDateValue(dr["StartDate"]);
                    rowResult[11] = GetDateValue(dr["DateDischarge"]);
                    rowResult[12] = dr["IssueSum"];
                    rowResult[13] = dr["Coupon"];
                    rowResult[14] = dr["Income"];
                    rowResult[15] = dr["Discount"];
                    rowResult[16] = dr["GenAgent"];
                    rowResult[17] = dr["Depository"];
                    rowResult[18] = dr["Trade"];
                    rowResult[19] = dr["PeriodIncome"];
                    rowResult[20] = dr["FactServiceSum"];
                    rowResult[21] = dr["FactDiscountSum"];
                    rowResult[22] = dr["TotalSum"];
                    var sum = GetDoubleValue(dr["IssueSum"]) + GetDoubleValue(dr["TotalSum"]);
                    rowResult[23] = sum;
                    totalSum[tableIndex] += sum;
                }

                tables[2] = CreateReportCaptionTable(3);
                var rowSummary = tables[2].Rows.Add();
                rowSummary[0] = totalSum[0];
                rowSummary[1] = totalSum[1];
                rowSummary[2] = totalSum[0] + totalSum[1];
            }
        }

        public void GetGarantSamaraData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            tables = new DataTable[3];
            const string ContactQuery = "select {0} from {1} c where c.RefVariant = {2} and c.RefRegion in ({3})";

            // Основные таблицы
            var crdEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            using (var db = scheme.SchemeDWH.DB)
            {
                var tblContracts = (DataTable)db.ExecQuery(
                    String.Format(
                        ContactQuery,
                        GetFieldNames(crdEntity, "c"), 
                        crdEntity.FullDBName, 
                        refVariant, 
                        refRegion),
                    QueryResultTypes.DataTable);
                tables[0] = CreateReportCaptionTable(10);
                tables[1] = CreateReportCaptionTable(10);
                var i = 1;
                var totalSum = new double[2];

                foreach (DataRow dr in tblContracts.Rows)
                {
                    var tableIndex = 0;

                    if (Convert.ToInt32(dr["RefOKV"]) != -1)
                    {
                        tableIndex = 1;
                    }

                    var rowResult = tables[tableIndex].Rows.Add();
                    rowResult[00] = i++;
                    rowResult[01] = String.Format("{0} от {1}", dr["Num"], GetDateValue(dr["StartDate"]));
                    rowResult[02] = dr["Creditor"];
                    rowResult[03] = dr["Principal"];
                    rowResult[04] = GetBookValue(scheme, DomainObjectsKeys.d_OKV_Currency, dr["RefOKV"].ToString());
                    rowResult[05] = dr["DateDoc"];
                    rowResult[06] = dr["EndDate"];
                    rowResult[07] = dr["DateDemand"];
                    rowResult[08] = dr["DatePerformance"];
                    var sum = GetDoubleValue(dr["TotalDebt"]);
                    rowResult[09] = sum;
                    totalSum[tableIndex] += sum;
                }

                tables[2] = CreateReportCaptionTable(3);
                var rowSummary = tables[2].Rows.Add();
                rowSummary[0] = totalSum[0];
                rowSummary[1] = totalSum[1];
                rowSummary[2] = totalSum[0] + totalSum[1];
            }
        }
    }
}
