using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0024.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0024.Commands
{
    [Description("ReportMOFO0024_005")]
    public class ReportMOFO0024_005Command : ExcelDirectCommand
    {
        public ReportMOFO0024_005Command()
        {
            key = "ReportMOFO0024_005";
            caption = "005_ПОКВАРТАЛЬНЫЙ АНАЛИЗ ПОСТУПЛЕНИЙ НАЛОГА НА ПРИБЫЛЬ ОРГАНИЗАЦИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge())
                .SetValue(DefaultParamValues.ProfitTax);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0024Report005Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0024_005Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}