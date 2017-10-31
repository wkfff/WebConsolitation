using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.MOFO0021.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0021.Commands
{
    [Description("ReportMOFO0021_001")]
    public class ReportMOFO0021_001Command : ExcelDirectCommand
    {
        public ReportMOFO0021_001Command()
        {
            key = "ReportMOFO0021_001";
            caption = "001_СУММЫ ПРОГНОЗА ОМСУ ПО ДОХОДНЫМ ИСТОЧНИКАМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantMOFOMarks());
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksForecast());
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr).SetValue(DefaultParamValues.HideEmptyStr);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0021Report001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0021_001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}