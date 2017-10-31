using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.Commands
{
    [Description("ReportUFNS015")]
    public class ReportUFNS015Command : ExcelDirectCommand
    {
        public ReportUFNS015Command()
        {
            key = "ReportUFNS015";
            caption = "015_ПОКАЗАТЕЛИ ФОРМЫ 5-ДДК";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksFNS5DDKBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamIncomes, new ParamTypesIncomes());
            var paramPerson = new ParamTypesPersons().SetItemFilter(fx_Types_Persons.ParentID, fx_Types_Persons.People);
            paramBuilder.AddBookParam(ReportConsts.ParamPersons, paramPerson);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            var undercut = new UndercutMarkYear(new ParamMarksFNS5DDKBridge().BookInfo,
                                                new ParamMarksFNS5DDK().BookInfo);
            paramBuilder.AddParamLink(ReportConsts.ParamMark, ReportConsts.ParamYear, undercut);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport015Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS015Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}