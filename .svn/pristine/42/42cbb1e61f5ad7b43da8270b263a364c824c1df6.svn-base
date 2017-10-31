using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls;
using Krista.FM.Client.Reports.UFK.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    [Description("ReportUFK015")]
    public class ReportUFK015Command : ExcelDirectCommand
    {
        public ReportUFK015Command()
        {
            key = "ReportUFK015";
            caption = "015_ПОСТУПЛЕНИЯ В БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ И ВОЗВРАТЫ ИЗ БЮДЖЕТА МОСКОВСКОЙ ОБЛАСТИ ПО ПЛАТЕЛЬЩИКАМ И ВИДУ ДОХОДОВ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate).SetValue(DateTime.Today);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddNumParam(ReportConsts.ParamSum)
                .SetValue(DefaultParamValues.LimitSum)
                .SetCaption("Платежи свыше (млн. руб.)")
                .SetMask(DefaultParamValues.LimitSumMask);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvlsFull())
                .SetValue(2);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport015(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK015Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}