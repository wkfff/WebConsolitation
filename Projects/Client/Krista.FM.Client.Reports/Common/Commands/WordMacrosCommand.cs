using Krista.FM.Common.ReportHelpers;

namespace Krista.FM.Client.Reports.Common.Commands
{
    public class WordMacrosCommand : CommonReportsCommand
    {
        protected override OfficeReportsHelper CreateOfficeObject()
        {
            return new WordReportHelper(scheme);
        }

        public override int GetImageIndex()
        {
            return 3;
        }
    }
}
