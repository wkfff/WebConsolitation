using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.MOFO0029.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0029.Commands
{
    [Description("ReportMOFO0029_001")]
    public class ReportMOFO0029_001Command : ExcelDirectCommand
    {
        public ReportMOFO0029_001Command()
        {
            key = "ReportMOFO0029_001";
            caption = "001_ОТЧЕТ О НАЧИСЛЕННЫХ СУММАХ НАЛОГА НА ИМУЩЕСТВО ФИЗИЧЕСКИХ ЛИЦ В ОТЧЕТНОМ ГОДУ И НАЧИСЛЕНИИ НА ТЕКУЩИЙ ГОД";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantPropertyTax());
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
            return reportServer.GetMOFO0029Report001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0029_001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}