using Krista.FM.Common.Constants;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.DebtBook.Params;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class RegionOrganizationCreditViewService : CreditViewService
    {
        public RegionOrganizationCreditViewService(IDebtBookExtension extension) 
            : base(extension, 2)
        {
            if (new Params.OKTMOValueProvider(SchemeAccessor.GetScheme()).GetValue() == OKTMO.Vologda)
            {
                var reportDescriptor = new ReportDescriptor();
                reportDescriptor.Title = "Отчет по кредитам от кредитных организаций";
                reportDescriptor.Handler =
@"function(){
    #{ReportParamsWindow}.autoLoad.url='/BebtBookReports/ShowParams?report=Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.OrganizationCreditsVologdaReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports';
    #{ReportParamsWindow}.show();
}";
                Actions.Add(reportDescriptor);
            }

            if (new Params.OKTMOValueProvider(SchemeAccessor.GetScheme()).GetValue() == OKTMO.Samara)
            {
                var reportDescriptor = new ReportDescriptor();
                reportDescriptor.Title = "Отчет по муниципальным коммерческим кредитам";
                reportDescriptor.Handler =
@"function(){
    #{ReportParamsWindow}.autoLoad.url='/BebtBookReports/ShowParams?report=Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtOrgBudCreditSamaraReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports';
    #{ReportParamsWindow}.show();
}";
                Actions.Add(reportDescriptor);
            }
        }

        public override int CreditTypeId
        {
            get { return 0; }
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
