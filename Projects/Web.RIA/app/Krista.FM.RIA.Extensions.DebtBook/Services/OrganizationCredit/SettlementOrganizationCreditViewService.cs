using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class SettlementOrganizationCreditViewService : CreditViewService
    {
        public SettlementOrganizationCreditViewService(IDebtBookExtension extension) 
            : base(extension, 3)
        {
        }

        public override int CreditTypeId
        {
            get { return 0; }
        }

        public override string GetDataFilter()
        {
            switch (Extension.UserRegionType)
            {
                case UserRegionType.Subject:
                    return "(RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0}))))) and (RefTypeCredit = {1}) and (ParentID is null)"
                        .FormatWith(Extension.UserRegionId, CreditTypeId);
                case UserRegionType.Region:
                case UserRegionType.Town:
                    return "(RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0}))) and (RefTypeCredit = {1}) and (ParentID is null)"
                        .FormatWith(Extension.UserRegionId, CreditTypeId);
                case UserRegionType.Settlement:
                    return "(RefRegion = {0}) and (RefTypeCredit = {1}) and (ParentID is null)"
                        .FormatWith(Extension.UserRegionId, CreditTypeId);
                default:
                    return "(RefTypeCredit = {0}) and (ParentID is null)".FormatWith(CreditTypeId);
            }
        }
    }
}
