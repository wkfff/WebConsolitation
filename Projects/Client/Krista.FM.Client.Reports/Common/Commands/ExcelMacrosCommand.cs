using Krista.FM.Common.ReportHelpers;

namespace Krista.FM.Client.Reports.Common.Commands
{
    public class ExcelMacrosCommand : CommonReportsCommand
    {
        protected override OfficeReportsHelper CreateOfficeObject()
        {
            return new ExcelReportHelper(scheme);
        }

        public override int GetImageIndex()
        {
            return 1;
        }
    }
}
