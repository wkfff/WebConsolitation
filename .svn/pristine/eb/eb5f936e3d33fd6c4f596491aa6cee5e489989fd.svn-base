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
    [Description("ReportUFNS008")]
    public class ReportUFNS008Command : ExcelDirectCommand
    {
        public ReportUFNS008Command()
        {
            key = "ReportUFNS008";
            caption = "АНАЛИЗ СОБИРАЕМОСТИ И ЗАДОЛЖЕННОСТИ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ";
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
            return reportServer.GetUFNSReport008Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS008Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}