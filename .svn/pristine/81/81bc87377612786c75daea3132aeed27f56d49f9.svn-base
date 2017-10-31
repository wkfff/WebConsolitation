using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.EGRUL;

namespace Krista.FM.Client.Reports.EGRUL.Commands
{
    [Description("ReportEGRUL0004")]
    public class ReportEGRUL0004Command : ExcelMacrosCommand
    {
        public ReportEGRUL0004Command()
        {
            key = "ReportEGRUL0004";
            caption = "ВЫБОР ОРГАНИЗАЦИЙ ПО КОДУ КЛАДР СУБЪЕКТА И ОКОПФ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKladrInfo, new ParamKLADR());
            paramBuilder.AddBookParam(ReportConsts.ParamOKOPF, new ParamOKOPF());
            paramBuilder.AddBookParam(ReportConsts.ParamKOPF, new ParamKOPF());
            paramBuilder.AddBookParam(ReportConsts.ParamOKATO, new ParamOKATO());
            paramBuilder.AddBookParam(ReportConsts.ParamINN, new ParamINN());
            paramBuilder.AddBookParam(ReportConsts.ParamOrgName, new ParamOrgFoundation());
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetEGRUL0004ReportData(reportParams);
        }
    }
}
