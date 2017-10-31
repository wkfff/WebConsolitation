using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class StocksVologdaReport : DebtBookVologdaReport
    {
        public StocksVologdaReport(ReportsDataService reportsDataService)
            : base(reportsDataService)
        {
        }

        protected override void DeleteSheets(HSSFWorkbook wb)
        {
            wb.SetSheetHidden(2, true);
            wb.SetSheetHidden(1, true);
            wb.SetSheetHidden(0, true);
        }
    }
}
