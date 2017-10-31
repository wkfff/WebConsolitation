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
    [Description("ReportUFK003")]
    public class ReportUFK003Command : ExcelDirectCommand
    {
        public ReportUFK003Command()
        {
            key = "ReportUFK003";
            caption = "ƒ»Õ¿Ã» ¿ œŒ—“”œÀ≈Õ»ﬂ ƒŒ’ŒƒŒ¬";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate).SetValue(DateTime.Today);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvlsFull())
                .SetValue(0);
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr)
                .SetValue(DefaultParamValues.HideEmptyStr);

            ((ParamSingleBdgtLvlsFull)paramBuilder[ReportConsts.ParamBdgtLevels]).ValuesFilter = "1,2,3,4,5";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport003DynamicIncome(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK003Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}