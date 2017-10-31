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
    [Description("ReportUFK009")]
    public class ReportUFK009Command : ExcelDirectCommand
    {
        public ReportUFK009Command()
        {
            key = "ReportUFK009";
            caption = "œŒ—“”œÀ≈Õ»≈ ƒŒ’ŒƒŒ¬ — œŒ ¬¿–“¿À‹ÕŒ… –¿«¡»¬ Œ… œŒ  ¡  ¬ –¿«–≈«≈ “≈––»“Œ–»… » Õ¿ÀŒ√ŒœÀ¿“≈À‹Ÿ» Œ¬";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamQuarter, new ParamQuarter());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvlsFull())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddBoolParam(ReportConsts.ParamOutputMode)
                .SetValue(DefaultParamValues.ShowOrg).SetCaption(DefaultParamValues.ShowOrgTitle);
            paramBuilder.AddNumParam(ReportConsts.ParamSum)
                .SetValue(DefaultParamValues.LimitSum)
                .SetCaption("œÎ‡ÚÂÊË Ò‚˚¯Â (ÏÎÌ. Û·.)")
                .SetMask(DefaultParamValues.LimitSumMask);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            ((ParamSingleBdgtLvlsFull)paramBuilder[ReportConsts.ParamBdgtLevels]).ValuesFilter = "1,2,3";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport009QuarterPayments(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK009Filler();
            reportFiller.FillUFKReport(wb, tableList);
        }
    }
}