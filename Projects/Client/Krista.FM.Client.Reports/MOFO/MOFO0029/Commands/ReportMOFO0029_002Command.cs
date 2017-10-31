using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.MOFO0029.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0029.Commands
{
    [Description("ReportMOFO0029_002")]
    public class ReportMOFO0029_002Command : ExcelDirectCommand
    {
        public ReportMOFO0029_002Command()
        {
            key = "ReportMOFO0029_002";
            caption = "002_СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О НАЧИСЛЕННЫХ СУММАХ НАЛОГА НА ИМУЩЕСТВО ФИЗИЧЕСКИХ ЛИЦ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantPropertyTax());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0029Report002Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0029_002Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}