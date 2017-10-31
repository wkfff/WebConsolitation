using System.Collections.Generic;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtorBookOrenburgRegionReport : DebtorBookOrenburgReport
    {
        public DebtorBookOrenburgRegionReport(ReportsDataService reportsDataService)
            : base(reportsDataService)
        {
        }

        protected override List<int> ListLimitDeleteRows()
        {
            return new List<int> { 9, 11, 12, 14, 15, 17, 20, 22, 23, 25, 26, 28, 31, 33, 34, 36, 37, 39 };
        }

        protected override List<int> ListLimitSummaryDeleteRows()
        {
            return new List<int> { 5, 7, 8, 10, 11, 13, 16, 18, 19, 21, 22, 24, 27, 29, 30, 32, 33, 35 };
        }

        protected override ReportType GetReportType()
        {
            return ReportType.Region;
        }
    }
}
