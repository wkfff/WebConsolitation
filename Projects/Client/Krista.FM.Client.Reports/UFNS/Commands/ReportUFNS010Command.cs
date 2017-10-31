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
    [Description("ReportUFNS010")]
    public class ReportUFNS010Command : ExcelDirectCommand
    {
        public ReportUFNS010Command()
        {
            key = "ReportUFNS010";
            caption = "ПОКАЗАТЕЛИ Ф.5-МН В РАЗРЕЗЕ ОМСУ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksFNS5MN());
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            var undercut = new UndercutMarkYear(null, new ParamMarksFNS5MN().BookInfo);
            paramBuilder.AddParamLink(ReportConsts.ParamMark, ReportConsts.ParamYear, undercut);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport010Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS010Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}