using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0022.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0022.Commands
{
    [Description("ReportMOFO006")]
    public class ReportMOFO0022_006Command : ExcelDirectCommand
    {
        public ReportMOFO0022_006Command()
        {
            key = "ReportMOFO006";
            caption = "006_СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О ЗАДОЛЖЕННОСТИ ПО АРЕНДНОЙ ПЛАТЕ НА ЗЕМЛЮ И ИМУЩЕСТВО";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamQuarter, typeof(QuarterEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0022Report006Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0022_006Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}