using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class GuaranteeVologdaReport : DebtBookVologdaReport
    {
        public GuaranteeVologdaReport(ReportsDataService reportsDataService)
            : base(reportsDataService)
        {
        }

        protected override void DeleteSheets(HSSFWorkbook wb)
        {
            wb.SetSheetHidden(3, true);
            wb.SetSheetHidden(1, true);
            wb.SetSheetHidden(0, true);
        }
    }
}
