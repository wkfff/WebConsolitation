using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.Commands
{
    [Description("ReportUFNS014")]
    public class ReportUFNS014Command : ExcelDirectCommand
    {
        public ReportUFNS014Command()
        {
            key = "ReportUFNS014";
            caption = "014_ПОКАЗАТЕЛИ ФОРМЫ 5-ДДК В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksFNS5DDKBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamIncomes, new ParamTypesIncomes());
            var paramPerson = new ParamTypesPersons().SetItemFilter(fx_Types_Persons.ParentID, fx_Types_Persons.People);
            paramBuilder.AddBookParam(ReportConsts.ParamPersons, paramPerson);
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr)
                .SetValue(DefaultParamValues.HideEmptyStr);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            var undercut = new UndercutMarkYear(new ParamMarksFNS5DDKBridge().BookInfo,
                                                new ParamMarksFNS5DDK().BookInfo);
            paramBuilder.AddParamLink(ReportConsts.ParamMark, ReportConsts.ParamYear, undercut);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport014Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS014Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}