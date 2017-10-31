using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.EGRUL;

namespace Krista.FM.Client.Reports.EGRUL.Commands
{
    [Description("ReportEGRUL0005")]
    public class ReportEGRUL0005Command : ExcelMacrosCommand
    {
        public ReportEGRUL0005Command()
        {
            key = "ReportEGRUL0005";
            caption = "ВЫБОР ОРГАНИЗАЦИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamOKOPF, new ParamOKOPF());
            paramBuilder.AddBookParam(ReportConsts.ParamKOPF, new ParamKOPF());
            paramBuilder.AddBookParam(ReportConsts.ParamOKATO, new ParamOKATO());
            paramBuilder.AddBookParam(ReportConsts.ParamINN, new ParamINN());
            paramBuilder.AddBookParam(ReportConsts.ParamOrgName, new ParamOrgFoundation());
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetEGRUL0005ReportData(reportParams);
        }
    }
}
