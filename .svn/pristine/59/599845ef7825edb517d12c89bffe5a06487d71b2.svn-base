using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Variants;
using Krista.FM.Client.Reports.Month.Fillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Commands
{
    [Description("ReportMonth030")]
    public class ReportMonth030Command : ExcelDirectCommand
    {
        public ReportMonth030Command()
        {
            key = "ReportMonth030";
            caption = "030_ИСПОЛНЕНИЕ БЮДЖЕТА МО ПО НАЛОГОВЫМ И НЕ НАЛОГОВЫМ ДОХОДАМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamGroupKD, new ParamGroupKD()).BookInfo.DeepSelect = false;
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantIncomePlan());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvlsFull())
                .SetValue(0);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            ((ParamSingleBdgtLvlsFull)paramBuilder[ReportConsts.ParamBdgtLevels]).ValuesFilter = "3,4,5";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport030Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new Report030Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}