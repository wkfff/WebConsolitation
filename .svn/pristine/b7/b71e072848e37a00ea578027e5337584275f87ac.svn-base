using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class D_S_TitleReportServive : ViewService
    {
        private readonly IDebtBookExtension extension;

        public D_S_TitleReportServive(IDebtBookExtension extension) 
            : base(extension)
        {
            this.extension = extension;
        }

        public override string GetDataFilter()
        {
            return "REFREGION = {0}".FormatWith(
                new Params.UserRegionIdValueProvider(extension).GetValue());
        }
    }
}
