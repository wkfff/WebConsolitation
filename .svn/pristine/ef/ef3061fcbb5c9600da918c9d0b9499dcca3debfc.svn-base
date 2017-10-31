using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.BorrowingVolume;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.VolumeHoldings;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE.Commands
{
    public class DdeReportCommand : AbstractCommand
    {
        public DdeReportCommand()
        {
            key = "DdeReport";
            caption = "Построить отчет";
        }

        public override void Run()
        {
            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Получение и обработка данных";
            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
            try
            {
                DDEIndicatorsUI clsUI = (DDEIndicatorsUI)WorkplaceSingleton.Workplace.ActiveContent;
                DataTable dtDdeData = clsUI.GetReportData();
                if (dtDdeData.Rows.Count == 0) return;

                DataTable dtTemplates = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
                DataRow[] templates = dtTemplates.Select(String.Format("Code = '{0}'", "DdeReport"));
                if (templates.Length == 0)
                    return;
                DataRow templateRow = templates[0];

                string reportFullName = templateRow["DocumentFileName"].ToString();
                int templateID = Convert.ToInt32(templateRow["ID"]);

                DataTable[] reportData = new DataTable[] { dtDdeData };

                using (OfficeReportsHelper helper = new ExcelReportHelper(FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme))
                {
                    TemplatesDocumentsHelper documentsHelper =
                        new TemplatesDocumentsHelper(
                            FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.TemplatesService.Repository);
                    string newCaption =
                        string.Format("{0}{1}{2}_", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day) +
                        Path.GetFileNameWithoutExtension(caption) + Path.GetExtension(caption);
                    string templateDocumentName = documentsHelper.SaveDocument(templateID, newCaption, reportFullName);
                    helper.CreateReport(templateDocumentName, reportData);
                    helper.ShowReport();
                }
            }
            finally
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
            }
        }
    }
}
