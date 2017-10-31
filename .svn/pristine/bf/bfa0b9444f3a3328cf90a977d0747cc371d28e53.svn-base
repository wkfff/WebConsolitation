using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class SubjectBudgetCreditViewService : CreditViewService
    {
        public SubjectBudgetCreditViewService(IDebtBookExtension extension)
            : base(extension, 0)
        {
        }

        public override int CreditTypeId
        {
            get { return 1; }
        }

        public override string GetDataFilter()
        {
            return "(RefTypeCredit = {0} and RefRegion = {1}) and (ParentID is null)"
                .FormatWith(CreditTypeId, Extension.SubjectRegionId);
        }
    }
}
