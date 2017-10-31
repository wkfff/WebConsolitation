using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0022.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0022.Commands
{
    [Description("ReportMOFO001")]
    public class ReportMOFO0022_001Command : ExcelDirectCommand
    {
        public ReportMOFO0022_001Command()
        {
            key = "ReportMOFO001";
            caption = "001_ОТЧЕТ О ЗАДОЛЖЕННОСТИ ПО АРЕНДНОЙ ПЛАТЕ НА ЗЕМЛЮ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamMark, typeof(MOFOContractTypeEnum))
                .SetValue(DefaultParamValues.ContractType);
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr).SetValue(DefaultParamValues.HideEmptyStr);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0022Report001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0022_001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}