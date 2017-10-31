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
    [Description("ReportUFK012")]
    public class ReportUFK012Command : ExcelDirectCommand
    {
        public ReportUFK012Command()
        {
            key = "ReportUFK012";
            caption = "ПОСТУПЛЕНИЕ КД ПО ИНН И АТЕ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate).SetValue(DefaultParamValues.CurrentYearStart);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamOrgID, new ParamOrgPayerBridge());
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
            return reportServer.GetUFKReport012(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK012Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}