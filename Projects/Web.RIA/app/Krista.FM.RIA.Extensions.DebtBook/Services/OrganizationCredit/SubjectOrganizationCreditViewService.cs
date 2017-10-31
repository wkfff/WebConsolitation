using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class SubjectOrganizationCreditViewService : CreditViewService
    {
        public SubjectOrganizationCreditViewService(IDebtBookExtension extension) 
            : base(extension, 0)
        {
        }

        public override int CreditTypeId
        {
            get { return 0; }
        }

        public override string GetDataFilter()
        {
            return "(RefRegion = {0}) and (RefTypeCredit = {1}) and (ParentID is null)"
                .FormatWith(Extension.SubjectRegionId, CreditTypeId);
        }
    }
}
