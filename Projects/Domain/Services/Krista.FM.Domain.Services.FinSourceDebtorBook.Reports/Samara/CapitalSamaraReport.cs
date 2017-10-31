using System;
using System.Data;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtCapitalSamaraReport : CommonSamaraReport
    {
        public DebtCapitalSamaraReport(ReportsDataService reportsDataService)
        {
            this.DataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "ReportDebtBookCapitalSamara"; }
        }

        public override void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
            ColumnCount = 24;
            DataService.GetCapitalSamaraData(
                currentVariantId, regionId, ref tables, calculateDate);
        }
    }
}
