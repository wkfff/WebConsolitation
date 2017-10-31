using Krista.FM.ServerLibrary;
using Krista.FM.Common.OfficeHelpers;

namespace Krista.FM.Common.ReportHelpers
{
    public class WordReportHelper : OfficeReportsHelper
    {
        public WordReportHelper(IScheme scheme)
            : base(scheme, OfficeHelper.CreateWordApplication())
        {
        }
    }
}
