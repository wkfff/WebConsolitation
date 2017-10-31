using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.Commands
{
    [Description("ReportUFNS013")]
    public class ReportUFNS013Command : ExcelDirectCommand
    {
        public ReportUFNS013Command()
        {
            key = "ReportUFNS013";
            caption = "013_СВОД ПОКАЗАТЕЛЕЙ ФОРМЫ 5-НДФЛ В РАЗРЕЗЕ ОМСУ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamHalfYear, new ParamHalfYear());
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksFNS5NDFLBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr)
                .SetValue(DefaultParamValues.HideEmptyStr);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            var undercut = new UndercutMarkYear(new ParamMarksFNS5NDFLBridge().BookInfo,
                                                new ParamMarksFNS5NDFL().BookInfo);
            paramBuilder.AddParamLink(ReportConsts.ParamMark, ReportConsts.ParamYear, undercut);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport013Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS013Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}