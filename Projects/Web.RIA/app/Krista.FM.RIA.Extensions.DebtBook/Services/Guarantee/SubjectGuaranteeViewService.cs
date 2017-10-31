using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class SubjectGuaranteeViewService : DebtBookViewService
    {
        public SubjectGuaranteeViewService(IDebtBookExtension extension) 
            : base(extension, 0)
        {
        }

        public override string GetDataFilter()
        {
            return "(RefRegion = {0}) and (ParentID is null)".FormatWith(Extension.SubjectRegionId);
        }
    }
}
