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
    [Description("ReportUFNS006")]
    public class ReportUFNS006Command : ExcelDirectCommand
    {
        public ReportUFNS006Command()
        {
            key = "ReportUFNS006";
            caption = "АНАЛИЗ ПЕРЕПЛАТЫ ПО НАЛОГАМ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ ПО ФОРМАМ 1-НМ, 4-НМ И 65Н";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.ThreeLastYears);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamArrearsFNS, new ParamArrearsFNSBridge())
                .SetValue(DefaultParamValues.AssetTaxArrears);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport006Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS006Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }

    [Description("ReportUFNS006_1")]
    public class ReportUFNS006_1Command : ExcelDirectCommand
    {
        public ReportUFNS006_1Command()
        {
            key = "ReportUFNS006_1";
            caption = "АНАЛИЗ ПЕРЕПЛАТЫ ПО НАЛОГУ НА ПРИБЫЛЬ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ СУБЪЕКТА";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.ThreeLastYears);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge())
                .SetValue(DefaultParamValues.OrgProfitTax);
            paramBuilder.AddBookParam(ReportConsts.ParamArrearsFNS, new ParamArrearsFNSBridge())
                .SetValue(DefaultParamValues.Arrears);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport006_1Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS006Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }

}