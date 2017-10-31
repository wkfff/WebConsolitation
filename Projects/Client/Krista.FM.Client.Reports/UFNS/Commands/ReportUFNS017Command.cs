using System;
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
    [Description("ReportUFNS017")]
    public class ReportUFNS017Command : ExcelDirectCommand
    {
        public ReportUFNS017Command()
        {
            key = "ReportUFNS017";
            caption = "017_ПОКАЗАТЕЛИ ФОРМЫ 5-УСН";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.CurrentYear);
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksFNS5YSNBridge());
            var value = String.Format("{0},{1},{2}", fx_Types_Persons.All, fx_Types_Persons.Org, fx_Types_Persons.People);
            var paramPerson = new ParamTypesPersons().SetItemFilter(fx_Types_Persons.ID, value);
            paramBuilder.AddBookParam(ReportConsts.ParamPersons, paramPerson);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            var undercut = new UndercutMarkYear(new ParamMarksFNS5YSNBridge().BookInfo,
                                                new ParamMarksFNS5YSN().BookInfo);
            paramBuilder.AddParamLink(ReportConsts.ParamMark, ReportConsts.ParamYear, undercut);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFNSReport017Data(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFNS017Filler();
            reportFiller.FillReport(wb, tableList);
        }
    }
}