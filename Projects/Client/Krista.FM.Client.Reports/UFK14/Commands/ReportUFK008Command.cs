using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls;
using Krista.FM.Client.Reports.UFK.ReportFillers;
using NPOI.HSSF.UserModel;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    [Description("ReportUFK008")]
    public class ReportUFK008Command : ExcelDirectCommand
    {
        public ReportUFK008Command()
        {
            key = "ReportUFK008";
            caption = "008_ПОСТУПЛЕНИЯ ОТ ОРГАНИЗАЦИЙ ПО УРОВНЯМ БЮДЖЕТА";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(ReportDataServer.GetYearStart(DateTime.Now.Year));
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate)
                .SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamOrgID, new ParamOrgPayerBridge());
            paramBuilder.AddNumParam(ReportConsts.ParamSum)
                .SetValue(DefaultParamValues.LimitSum)
                .SetCaption("Платежи свыше (млн. руб.)")
                .SetMask(DefaultParamValues.LimitSumMask);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvlsFull());
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport008(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK008Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}