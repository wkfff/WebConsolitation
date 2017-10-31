using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.UFK.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    [Description("ReportUFK004")]
    public class ReportUFK004Command : ExcelDirectCommand
    {
        public ReportUFK004Command()
        {
            key = "ReportUFK004";
            caption = "004_ПОСТУПЛЕНИЕ НАЛОГА НА ПРИБЫЛЬ И НДФЛ С ТЕРРИТОРИИ СУБЪЕКТА РФ ОТ СТРУКТУРНЫХ ПОДРАЗДЕЛЕНИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(ReportDataServer.GetYearStart(DateTime.Now.Year));
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetValue(DateTime.Today);
            paramBuilder.AddBookParam(ReportConsts.ParamStructure, new ParamStructureCharacter());
            paramBuilder.AddBoolParam(ReportConsts.ParamOutputMode)
                .SetValue(DefaultParamValues.ShowOrg).SetCaption(DefaultParamValues.ShowOrgTitle);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetUFKReport004(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new ReportUFK004Filler();
            reportFiller.FillUFKReport(wb, tableList);
        }
    }
}