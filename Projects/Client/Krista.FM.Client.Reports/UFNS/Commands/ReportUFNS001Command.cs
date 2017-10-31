using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.Commands
{
    [Description("ReportUFNS001")]
    public class ReportUFNS001Command : ExcelDirectCommand
    {
        public ReportUFNS001Command()
        {
            key = "ReportUFNS001";
            caption = "ЕЖЕМЕСЯЧНЫЙ АНАЛИЗ ЗАДОЛЖЕННОСТИ И НЕДОИМКИ ПО КБК_65Н";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.FourLastYears);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamOKVED, new ParamOKVED());
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof (SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof (PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}