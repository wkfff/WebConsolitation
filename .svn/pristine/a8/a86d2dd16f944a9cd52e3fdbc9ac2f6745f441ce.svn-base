using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0026.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0026.Commands
{
    [Description("ReportMOFO0026_002")]
    public class ReportMOFO0026_002Command : ExcelDirectCommand
    {
        public ReportMOFO0026_002Command()
        {
            key = "ReportMOFO0026_002";
            caption = "002_СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ ПО ПОКАЗАТЕЛЯМ ДЕЯТЕЛЬНОСТИ АКЦИОНЕРНЫХ ОБЩЕСТВ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0026Report002Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0026_002Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}