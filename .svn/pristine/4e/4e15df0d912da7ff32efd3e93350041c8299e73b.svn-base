using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;

namespace Krista.FM.Client.Reports.EGRUL.Commands
{
    [Description("ReportUFKMO0008")]
    public class ReportEGRUL0002Command : ExcelMacrosCommand
    {
        public ReportEGRUL0002Command()
        {
            key = "ReportUFKMO0008";
            caption = "Выбор организаций муниципального образования";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridge());
            paramBuilder.AddBoolParam(ReportConsts.ParamOutputMode)
                .SetValue(ReportConsts.strFalse)
                .SetCaption("Выгрузить в один документ");
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetEGRUL0002ReportData(reportParams);
        }
    }
}
