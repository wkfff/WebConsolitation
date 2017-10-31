using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class BudgetCreditsVologdaReport : DebtBookVologdaReport
    {
        public BudgetCreditsVologdaReport(ReportsDataService reportsDataService)
            : base(reportsDataService)
        {
        }

        protected override void DeleteSheets(HSSFWorkbook wb)
        {
            wb.SetSheetHidden(3, true);
            wb.SetSheetHidden(2, true);
            wb.SetSheetHidden(0, true);
        }
    }
}
