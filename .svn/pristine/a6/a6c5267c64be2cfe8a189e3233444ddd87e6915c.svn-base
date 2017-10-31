using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.MOFO0023.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0023.Commands
{
    [Description("ReportMOFO0023_001")]
    public class ReportMOFO0023_001Command : ExcelDirectCommand
    {
        public ReportMOFO0023_001Command()
        {
            key = "ReportMOFO0023_001";
            caption = "001_ПОСТУПЛЕНИЯ ДОХОДОВ ОТ ПРОДАЖИ ПРАВА НА ЗАКЛЮЧЕНИЕ ДОГОВОРОВ АРЕНДЫ ЗА ЗЕМЕЛЬНЫЕ УЧАСТКИ И ПОСТУПЛЕНИЙ ОТ РЕАЛИЗАЦИИ ИНВЕСТИЦИОННЫХ КОНТРАКТОВ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksReceipt());
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr).SetValue(DefaultParamValues.HideEmptyStr);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOFO0023Report001Data(reportParams);

        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportMOFO0023_001Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}