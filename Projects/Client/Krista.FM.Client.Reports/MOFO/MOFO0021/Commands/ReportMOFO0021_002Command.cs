using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.MOFO0021.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0021.Commands
{
    [Description("ReportMOFO0021_002")]
    public class ReportMOFO0021_002Command : ExcelDirectCommand
    {
        public ReportMOFO0021_002Command()
        {
            key = "ReportMOFO0021_002";
            caption = "002_СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О СУММАХ ПРОГНОЗА ОМСУ ПО ДОХОДНЫМ ИСТОЧНИКАМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantMOFOMarks());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0021Report002Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0021_002Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}