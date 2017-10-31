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
    [Description("ReportUFK001")]
    public class ReportUFK001Command : ExcelDirectCommand
    {
        public ReportUFK001Command()
        {
            key = "ReportUFK001";
            caption = "онярсокемхе днунднб б йнмянкхдхпнбюммши ачдфер лняйнбяйни накюярх";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(DefaultParamValues.CurrentYearStart);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvlsFull())
                .SetValue(0);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
            paramBuilder.AddBoolParam(ReportConsts.ParamHideEmptyStr).SetValue(DefaultParamValues.HideEmptyStr);

            ((ParamSingleBdgtLvlsFull)paramBuilder[ReportConsts.ParamBdgtLevels]).ValuesFilter = "1,2,3,4,5";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport001IncomeConsBudgetData(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK001Filler();
            reportFiller.FillUFKReport(wb, tableList);
        }
    }
}