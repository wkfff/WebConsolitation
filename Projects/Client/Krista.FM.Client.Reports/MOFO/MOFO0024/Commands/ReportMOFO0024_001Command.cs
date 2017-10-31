using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0024.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0024.Commands
{
    [Description("ReportMOFO0024_001")]
    public class ReportMOFO0024_001Command : ExcelDirectCommand
    {
        public ReportMOFO0024_001Command()
        {
            key = "ReportMOFO0024_001";
            caption = "001_НАЛОГ НА ПРИБЫЛЬ К ДОПЛАТЕ (УМЕНЬШЕНИЮ) ИТОГОВЫЙ СВОД";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamQuarter, typeof(QuarterEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0024Report001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0024_001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}