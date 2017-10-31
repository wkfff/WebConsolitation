using System;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class RegionContrDebtViewService : DebtBookViewService
    {
        public RegionContrDebtViewService(IDebtBookExtension extension)
            : base(extension, 2)
        {
        }

        public override string GetDataFilter()
        {
            switch (Extension.UserRegionType)
            {
                case UserRegionType.Region:
                case UserRegionType.Town:
                    return "(RefRegion = {0}) and (ParentID is null)"
                        .FormatWith(Extension.UserRegionId);
                case UserRegionType.Subject:
                    return "(RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0}))) and (ParentID is null)"
                        .FormatWith(Extension.SubjectRegionId);
                default:
                    return String.Empty;
            }
        }
    }
}
