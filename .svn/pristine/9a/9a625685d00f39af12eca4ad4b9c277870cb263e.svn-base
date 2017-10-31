using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls;
using Krista.FM.Client.Reports.UFK.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    [Description("ReportUFK006")]
    public class ReportUFK006Command : ExcelDirectCommand
    {
        public ReportUFK006Command()
        {
            key = "ReportUFK006";
            caption = "»Õ‘Œ–Ã¿÷»ﬂ Œ œŒ—“”œÀ≈Õ»ﬂ’ ¬€¡–¿ÕÕŒ√Œ Õ¿ÀŒ√ŒœÀ¿“≈À‹Ÿ» ¿ ¬ –¿«À»◊Õ€≈ ”–Œ¬Õ» ¡ﬁƒ∆≈“¿";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamOrgID, new ParamOrgPayerBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvlsFull());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport006SelectPayments(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK006Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}