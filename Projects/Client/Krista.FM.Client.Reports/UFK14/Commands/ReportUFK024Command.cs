using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.UFK.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    [Description("ReportUFK024")]
    public class ReportUFK024Command : ExcelDirectCommand
    {
        public ReportUFK024Command()
        {
            key = "ReportUFK024";
            caption = "024_ИСПОЛНЕНИЕ БЮДЖЕТА МО ПО ДОХОДНОМУ ИСТОЧНИКУ ЗА ОТЧЕТНЫЙ ГОД";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge())
                .SetDeepSelect(false)
                .BookInfo.WriteFullText = true;
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport024(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK024Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}