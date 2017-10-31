using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.UFK22.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK22.Commands
{
    [Description("ReportUFK22_024")]
    public class ReportUFK22_024Command : ExcelDirectCommand
    {
        public ReportUFK22_024Command()
        {
            key = "ReportUFK22_024";
            caption = "024_ДИНАМИКА ПОСТУПЛЕНИЯ НАЛОГОВЫХ И НЕНАЛОГОВЫХ ДОХОДОВ В БC – ОПЕРАТИВНАЯ ИНФОРМАЦИЯ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(DefaultParamValues.CurrentYearStart);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamGroupKD, new ParamGroupKD()).BookInfo.DeepSelect = false;
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksInpayments())
                .SetValue(DefaultParamValues.MarkInpaymentsTransfer);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFK22Report024Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK22_024Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}