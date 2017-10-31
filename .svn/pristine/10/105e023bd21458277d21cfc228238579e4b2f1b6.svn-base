using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class RegionOtherCreditViewService : CreditViewService
    {
        public RegionOtherCreditViewService(IDebtBookExtension extension) 
            : base(extension, 2)
        {
        }

        public override int CreditTypeId
        {
            get { return 5; }
        }

        public override string GetDataFilter()
        {
            switch (Extension.UserRegionType)
            {
                case UserRegionType.Region:
                case UserRegionType.Town:
                    return "(RefRegion = {0}) and (RefTypeCredit = {1}) and (ParentID is null)"
                        .FormatWith(Extension.UserRegionId, CreditTypeId);
                case UserRegionType.Subject:
                    return "(RefTypeCredit = {1}) and (RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0}))) and (ParentID is null)"
                        .FormatWith(Extension.SubjectRegionId, CreditTypeId);
                default:
                    return "(RefTypeCredit = {0}) and (ParentID is null)"
                        .FormatWith(CreditTypeId);
            }
        }
    }
}
