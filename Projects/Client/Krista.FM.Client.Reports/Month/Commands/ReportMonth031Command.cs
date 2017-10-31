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
    [Description("ReportMonth031")]
    public class ReportMonth031Command : ExcelDirectCommand
    {
        public ReportMonth031Command()
        {
            key = "ReportMonth031";
            caption = "031_ИСПОЛНЕНИЕ БЮДЖЕТА МУНИЦИПАЛЬНОГО ОБРАЗОВАНИЯ (ДЛЯ БЮДЖЕТНОГО КРЕДИТА)";
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
            return reportServer.GetMonthReport030Data(reportParams); // отчет отличается от 30-го только наличием дополнительных незаполняемых колонок
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new Report031Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}