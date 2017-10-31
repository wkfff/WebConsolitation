using System;
using System.Data;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtOrgBudCreditSamaraReport : CommonSamaraReport
    {
        public DebtOrgBudCreditSamaraReport(ReportsDataService reportsDataService)
        {
            this.DataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "ReportDebtBookOrgCreditSamara"; }
        }

        public override void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
            ColumnCount = 8;
            DataService.GetOrgCreditSamaraData(
                currentVariantId, regionId, ref tables, calculateDate);
        }
    }
}
