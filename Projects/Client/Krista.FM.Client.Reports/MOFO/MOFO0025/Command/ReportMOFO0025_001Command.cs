using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0025.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0025.Commands
{
    [Description("ReportMOFO0025_001")]
    public class ReportMOFO0025_001Command : ExcelDirectCommand
    {
        public ReportMOFO0025_001Command()
        {
            key = "ReportMOFO0025_001";
            caption = "001_ОТЧЕТ О ПРОГНОЗИРУЕМЫХ НАЧИСЛЕНИЯХ ЗЕМЕЛЬНОГО НАЛОГА НА ОЧЕРЕДНОЙ ФИНАНСОВЫЙ ГОД И ПЛАНОВЫЙ ПЕРИОД";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr)
                .SetValue(DefaultParamValues.HideEmptyStr);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0025Report001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0025_001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}