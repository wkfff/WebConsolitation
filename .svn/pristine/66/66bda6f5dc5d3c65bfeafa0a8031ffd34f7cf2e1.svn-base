using System.Collections.Generic;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtorBookOrenburgSettleReport : DebtorBookOrenburgReport
    {
        public DebtorBookOrenburgSettleReport(ReportsDataService reportsDataService)
            : base(reportsDataService)
        {
        }

        protected override List<int> ListLimitDeleteRows()
        {
            return new List<int> { 9, 10, 12, 13, 15, 16, 18, 20, 21, 23, 24, 26, 27, 29, 31, 32, 34, 35, 37, 38, 40 };
        }

        protected override List<int> ListLimitSummaryDeleteRows()
        {
            return new List<int> { 5, 6, 8, 9, 11, 12, 14, 16, 17, 19, 20, 22, 23, 25, 27, 28, 30, 31, 33, 34, 36 };
        }

        protected override ReportType GetReportType()
        {
            return ReportType.Settles;
        }
    }
}
