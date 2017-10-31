using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.UFK.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    [Description("ReportUFK016")]
    public class ReportUFK016Command : ExcelDirectCommand
    {
        public ReportUFK016Command()
        {
            key = "ReportUFK016";
            caption = "016_ПОСТУПЛЕНИЯ В КБС, БС, КМБ И ВОЗВРАТЫ ПО ПЛАТЕЛЬЩИКАМ БЕЗ РАЗБИВКИ ПО КБК";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate).SetValue(DateTime.Today);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamOrgID, new ParamOrgPayerBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddNumParam(ReportConsts.ParamSum)
                .SetValue(DefaultParamValues.LimitSum)
                .SetCaption("Платежи свыше (млн. руб.)")
                .SetMask(DefaultParamValues.LimitSumMask);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport016(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK016Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}