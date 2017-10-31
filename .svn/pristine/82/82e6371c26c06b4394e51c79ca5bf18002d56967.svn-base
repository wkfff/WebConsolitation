using System;
using System.Data;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtGarantSamaraReport : CommonSamaraReport
    {
        public DebtGarantSamaraReport(ReportsDataService reportsDataService)
        {
            this.DataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "ReportDebtBookGarantSamara"; }
        }

        public override void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
            ColumnCount = 10;
            DataService.GetGarantSamaraData(
                currentVariantId, regionId, ref tables, calculateDate);
        }
    }
}
