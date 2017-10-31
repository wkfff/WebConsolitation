using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.MOFO0027.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0027.Commands
{
    [Description("ReportMOFO0027_001")]
    public class ReportMOFO0027_001Command : ExcelDirectCommand
    {
        public ReportMOFO0027_001Command()
        {
            key = "ReportMOFO0027_001";
            caption = "001_ОТЧЕТ ПО ПОКАЗАТЕЛЯМ ДЕЯТЕЛЬНОСТИ МУНИЦИПАЛЬНЫХ УНИТАРНЫХ ПРЕДПРИЯТИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamActivityStatus());
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddBoolParam(ReportConsts.ParamQuarter)
                .SetValue(DefaultParamValues.ShowQuarters)
                .SetCaption(DefaultParamValues.ShowQuartersTitle);
            paramBuilder.AddBoolParam(ReportConsts.ParamOutputMode)
                .SetValue(DefaultParamValues.ShowOrg)
                .SetCaption(DefaultParamValues.ShowOrgTitle);
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr)
                .SetValue(DefaultParamValues.HideEmptyStr);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0027Report001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0027_001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}