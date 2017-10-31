using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls;
using Krista.FM.Client.Reports.Month.Fillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Commands
{
    [Description("ReportMonth023")]
    public class ReportMonth023Command : ExcelDirectCommand
    {
        public ReportMonth023Command()
        {
            key = "ReportMonth023";
            caption = "023_ДИНАМИКА ПОСТУПЛЕНИЯ НАЛОГОВЫХ И НЕНАЛОГОВЫХ ДОХОДОВ В БС ПО СОСТОЯНИЮ НА ОПРЕДЕЛЕННУЮ ДАТУ И ЗА УКАЗАННЫЕ ГОДЫ С УДЕЛЬНЫМИ ВЕСАМИ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamGroupKD, new ParamGroupKD()).BookInfo.DeepSelect = false;
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvlsFull())
                .SetValue(0);
            paramBuilder.AddBookParam(ReportConsts.ParamMeans, new ParamMeansTypeSKIF())
                .SetValue(1);
            paramBuilder.AddBookParam(ReportConsts.ParamDocType, new ParamDocTypeSKIF())
                .SetValue(3);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            ((ParamSingleBdgtLvlsFull)paramBuilder[ReportConsts.ParamBdgtLevels]).ValuesFilter = "1,2,4";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport023Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new Report023Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}