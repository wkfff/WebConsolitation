namespace Krista.FM.Client.Reports.Common.Commands
{
    public class ExcelDirectCommand : CommonReportsCommand
    {
        protected override bool UseNPOI()
        {
            return true;
        }
    }
}
