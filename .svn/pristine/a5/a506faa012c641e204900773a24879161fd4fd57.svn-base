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
    [Description("ReportUFK011")]
    public class ReportUFK011Command : ExcelDirectCommand
    {
        public ReportUFK011Command()
        {
            key = "ReportUFK011";
            caption = "011_ПОСТУПЛЕНИЕ ДОХОДОВ В КБС (БС, МБ) В РАЗРЕЗЕ КД И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ ПО НАЛОГОПЛАТЕЛЬЩИКАМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(ReportDataServer.GetYearStart(DateTime.Now.Year));
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvlsFull())
                .SetValue(2);
            paramBuilder.AddBoolParam(ReportConsts.ParamOutputMode)
                .SetValue(DefaultParamValues.ShowOrg).SetCaption(DefaultParamValues.ShowOrgTitle);
            paramBuilder.AddNumParam(ReportConsts.ParamSum)
                .SetValue(DefaultParamValues.LimitSum)
                .SetCaption("Платежи свыше (млн. руб.)")
                .SetMask(DefaultParamValues.LimitSumMask);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport011(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK011Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}