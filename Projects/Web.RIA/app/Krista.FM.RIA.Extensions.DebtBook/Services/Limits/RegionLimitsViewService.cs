using System;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class RegionLimitsViewService : DebtBookViewService
    {
        public RegionLimitsViewService(IDebtBookExtension extension) 
            : base(extension, 2)
        {
        }

        public override string GetDataFilter()
        {
            switch (Extension.UserRegionType)
            {
                case UserRegionType.Subject:
                    return "(RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0})))"
                        .FormatWith(Extension.SubjectRegionId);
                case UserRegionType.Region:
                case UserRegionType.Town:
                    return "(RefRegion = {0})"
                        .FormatWith(Extension.UserRegionId);
                default:
                    return String.Empty;
            }
        }
    }
}
