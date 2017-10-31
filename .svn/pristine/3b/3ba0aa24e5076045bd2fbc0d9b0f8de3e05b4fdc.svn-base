using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0028.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0028.Commands
{
    [Description("ReportMOFO0028_002")]
    public class ReportMOFO0028_002Command : ExcelDirectCommand
    {
        public ReportMOFO0028_002Command()
        {
            key = "ReportMOFO0028_002";
            caption = "002_СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О НАЧИСЛЕННЫХ СУММАХ АРЕНДНОЙ ПЛАТЫ ЗА ЗЕМЛЮ И ИМУЩЕСТВО";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0028Report002Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0028_002Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}