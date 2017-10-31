using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.UFK.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    [Description("ReportUFK007")]
    public class ReportUFK007Command : ExcelDirectCommand
    {
        public ReportUFK007Command()
        {
            key = "ReportUFK007";
            caption = "007_АНАЛИЗ ПОСТУПЛЕНИЯ ПО НДФЛ И НП ПО СТРУКТУРАМ ЗА ДВА ПРОШЛЫХ ГОДА И ТЕКУЩИЙ ГОД";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
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
            return reportServer.GetUFKReport007(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK007Filler();
            reportFiller.FillUFKReport(wb, tableList);
        }
    }
}