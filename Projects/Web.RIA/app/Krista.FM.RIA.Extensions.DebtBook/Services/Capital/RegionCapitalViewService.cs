using System;
using Krista.FM.Common.Constants;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.DebtBook.Params;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class RegionCapitalViewService : DebtBookViewService
    {
        public RegionCapitalViewService(IDebtBookExtension extension) 
            : base(extension, 2)
        {
            if (new OKTMOValueProvider(SchemeAccessor.GetScheme()).GetValue() == OKTMO.Samara)
            {
                var reportDescriptor = new ReportDescriptor();
                reportDescriptor.Title = "Отчет по муниципальным ценным бумагам";
                reportDescriptor.Handler =
@"function(){
    #{ReportParamsWindow}.autoLoad.url='/BebtBookReports/ShowParams?report=Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtCapitalSamaraReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports';
    #{ReportParamsWindow}.show();
}";
                Actions.Add(reportDescriptor);
            }
        }

        public override string GetDataFilter()
        {
            switch (Extension.UserRegionType)
            {
                case UserRegionType.Subject:
                    return "(RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0}))) and (ParentID is null)"
                        .FormatWith(Extension.SubjectRegionId);
                case UserRegionType.Region:
                case UserRegionType.Town:
                    return "(RefRegion = {0}) and (ParentID is null)"
                        .FormatWith(Extension.UserRegionId);
                default:
                    return String.Empty;
            }
        }
    }
}
