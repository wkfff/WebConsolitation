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
    [Description("ReportUFK021")]
    public class ReportUFK021Command : ExcelDirectCommand
    {
        public ReportUFK021Command()
        {
            key = "ReportUFK021";
            caption = "021_ДИНАМИКА ПОСТУПЛЕНИЙ ПО КД";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvlsFull());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof (SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof (PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            ((ParamSingleBdgtLvlsFull)paramBuilder[ReportConsts.ParamBdgtLevels]).ValuesFilter = "1,2,4";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport021(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK021Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}