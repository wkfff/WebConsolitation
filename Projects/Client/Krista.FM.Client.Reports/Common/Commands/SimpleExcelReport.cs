using System;
using System.Data;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.Reports.Common.Commands
{
    public class SimpleExcelReport
    {
        private readonly IScheme scheme;
        private readonly string key;
        private readonly string caption;

        public SimpleExcelReport(IScheme activeScheme, string reportKey, string reportCaption)
        {
            scheme = activeScheme;
            key = reportKey;
            caption = reportCaption;
        }

        public void SetReportFolder(string reportFolder)
        {
        }

        public void CreateReport(DataTable[] dtData)
        {
            var dtTemplatesData = scheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
            var drsSelect = dtTemplatesData.Select(String.Format("{0} = '{1}'", TemplateFields.Code, key));

            if (drsSelect.Length <= 0)
            {
                return;
            }

            var templateRow = drsSelect[0];
            var documentsHelper = new TemplatesDocumentsHelper(scheme.TemplatesService.Repository);
            var templateId = Convert.ToInt32(templateRow[TemplateFields.ID]);
            var templateName = templateRow[TemplateFields.DocumentFileName].ToString();
            var docName = documentsHelper.SaveDocument(templateId, caption, templateName);

            using (OfficeReportsHelper helper = new ExcelReportHelper(scheme))
            {
                helper.CreateReport(docName, dtData);
                helper.ShowReport();
            }
        }
    }
}
