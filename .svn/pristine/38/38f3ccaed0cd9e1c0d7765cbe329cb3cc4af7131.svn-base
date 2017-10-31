using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0023.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0023.Commands
{
    [Description("ReportMOFO0023_002")]
    public class ReportMOFO0023_002Command : ExcelDirectCommand
    {
        public ReportMOFO0023_002Command()
        {
            key = "ReportMOFO0023_002";
            caption = "002_СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ ПО ПОСТУПЛЕНИЯМ ПО АРЕНДНОЙ ПЛАТЕ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0023Report002Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0023_002Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}