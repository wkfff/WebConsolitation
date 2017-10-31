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
    [Description("ReportUFK019")]
    public class ReportUFK019Command : ExcelDirectCommand
    {
        public ReportUFK019Command()
        {
            key = "ReportUFK019";
            caption = "019_ЕЖЕМЕСЯЧНЫЕ СУММЫ ВОЗВРАТОВ НАЛОГА В БС В ТЕКУЩЕМ ГОДУ В РАЗРЕЗЕ ТЕРРИТОРИЙ И НАЛОГОПЛАТЕЛЬЩИКОВ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBoolParam(ReportConsts.ParamOutputMode)
                .SetValue(DefaultParamValues.ShowOrg).SetCaption(DefaultParamValues.ShowOrgTitle);
            paramBuilder.AddNumParam(ReportConsts.ParamSum)
                .SetValue(DefaultParamValues.LimitSum)
                .SetCaption("Платежи свыше (млн. руб.)")
                .SetMask(DefaultParamValues.LimitSumMask);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvlsFull())
                .SetValue(DefaultParamValues.SubjectBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport019(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK019Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}