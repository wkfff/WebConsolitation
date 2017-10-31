using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.Commands
{
    [Description("ReportUFNS005")]
    public class ReportUFNS005Command : ExcelDirectCommand
    {
        public ReportUFNS005Command()
        {
            key = "ReportUFNS005";
            caption = "ЕЖЕМЕС. АНАЛИЗ НАЧИСЛЕНИЙ ПО НАЛОГАМ ПО ФОРМЕ 1-НМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);

            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport005Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS005Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}