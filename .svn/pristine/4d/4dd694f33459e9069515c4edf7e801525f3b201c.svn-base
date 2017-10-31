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
    [Description("ReportMonth020")]
    public class ReportMonth020Command : ExcelDirectCommand
    {
        public ReportMonth020Command()
        {
            key = "ReportMonth020";
            caption = "020_УТОЧНЕНИЕ ПЛАНОВЫХ ПОКАЗАТЕЛЕЙ ПО НАЛОГОВЫМ И НЕ НАЛОГОВЫМ ДОХОДАМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamMonth, new ParamMonth())
                .SetMultiSelect(true);
            paramBuilder.AddBookParam(ReportConsts.ParamGroupKD, new ParamGroupKD()).BookInfo.DeepSelect = false;
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvlsFull())
                .SetValue(0);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            ((ParamSingleBdgtLvlsFull)paramBuilder[ReportConsts.ParamBdgtLevels]).ValuesFilter = "1,2,4";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport020Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new Report020Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}