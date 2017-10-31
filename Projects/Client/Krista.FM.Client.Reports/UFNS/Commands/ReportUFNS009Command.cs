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
    [Description("ReportUFNS009")]
    public class ReportUFNS009Command : ExcelDirectCommand
    {
        public ReportUFNS009Command()
        {
            key = "ReportUFNS009";
            caption = "АНАЛИЗ ЗАДОЛЖЕННОСТИ И ПЕРЕПЛАТЫ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.FourLastYears);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamArrearsFNS, new ParamArrearsFNSBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport009Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS009Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}