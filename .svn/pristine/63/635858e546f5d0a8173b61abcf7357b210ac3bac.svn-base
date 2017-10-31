using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0025.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0025.Commands
{
    [Description("ReportMOFO0025_002")]
    public class ReportMOFO0025_002Command : ExcelDirectCommand
    {
        public ReportMOFO0025_002Command()
        {
            key = "ReportMOFO0025_002";
            caption = "002_СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О ПРОГНОЗИРУЕМЫХ НАЧИСЛЕНИЯХ ЗЕМЕЛЬНОГО НАЛОГА НА ОФГ И ПП";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0025Report002Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0025_002Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}