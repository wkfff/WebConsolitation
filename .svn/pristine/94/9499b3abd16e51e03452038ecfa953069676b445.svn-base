using System;
using Krista.FM.Common.Constants;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class RegionGuaranteeViewService : DebtBookViewService
    {
        public RegionGuaranteeViewService(IDebtBookExtension extension) 
            : base(extension, 2)
        {
            if (new Params.OKTMOValueProvider(SchemeAccessor.GetScheme()).GetValue() == OKTMO.Vologda)
            {
                var reportDescriptor = new ReportDescriptor();
                reportDescriptor.Title = "Отчет по гарантиям";
                reportDescriptor.Handler =
@"function(){
    #{ReportParamsWindow}.autoLoad.url='/BebtBookReports/ShowParams?report=Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.GuaranteeVologdaReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports';
    #{ReportParamsWindow}.show();
}";
                Actions.Add(reportDescriptor);
            }

            if (new Params.OKTMOValueProvider(SchemeAccessor.GetScheme()).GetValue() == OKTMO.Samara)
            {
                var reportDescriptor = new ReportDescriptor();
                reportDescriptor.Title = "Отчет по муниципальным гарантиям";
                reportDescriptor.Handler =
@"function(){
    #{ReportParamsWindow}.autoLoad.url='/BebtBookReports/ShowParams?report=Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtGarantSamaraReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports';
    #{ReportParamsWindow}.show();
}";
                Actions.Add(reportDescriptor);
            }
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
