using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.Remains;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.Workplace.Gui;
using System.Data;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class CalculateRemainsCommand : AbstractCommand
    {
        public CalculateRemainsCommand()
        {
            key = "CalculateRemains";
            caption = "Расчет остатков средств на счетах";
        }

        public override void Run()
        {
            int incomeVariant = -1;
            int outcomeVariant = -1;
            int ifVariant = FinSourcePlanningNavigation.Instance.CurrentVariantID > 0 ? FinSourcePlanningNavigation.Instance.CurrentVariantID : -1;
            string ifVariantCaption = ifVariant > 0 ? FinSourcePlanningNavigation.Instance.CurrentVariantCaption : string.Empty;
            if (VariantSelectForm.ShowVariantForm(WorkplaceSingleton.Workplace.WindowHandle, ifVariantCaption, ref incomeVariant, ref outcomeVariant, ref ifVariant))
            {
                WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет остатков средств на счетах";
                WorkplaceSingleton.Workplace.OperationObj.StartOperation();
                try
                {
                    RemainsUI remainsUI = (RemainsUI)WorkplaceSingleton.Workplace.ActiveContent;
                    remainsUI.ClearData();
                    RemainsServer server = new RemainsServer(WorkplaceSingleton.Workplace.ActiveScheme);
                    DataRow[] rows = server.CalculateRemains(FinSourcePlanningNavigation.Instance.CurrentSourceID,
                        ifVariant, incomeVariant, outcomeVariant);

                    foreach (DataRow row in rows)
                        remainsUI.AddData(row);
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Расчет остатков средств на счетах успешно завершен",
                        "Остатки средств на счетах", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    throw;
                }
            }
        }
    }

    public class RemainsReportCommand : AbstractCommand
    {
        public RemainsReportCommand()
        {
            key = "RemainsReport";
            caption = "Отчет по остаткам средств на счетах";
        }

        public override void Run()
        {
            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Получение и обработка данных";
            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
            try
            {
                RemainsUI remainsUI = (RemainsUI)WorkplaceSingleton.Workplace.ActiveContent;
                DataTable dtRemains = remainsUI.GetData();
                if (dtRemains.Rows.Count == 0) return;


                DataTable dtTemplates = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
                DataRow[] templates = dtTemplates.Select(String.Format("Code = '{0}'", "RemainsDesign"));
                if (templates.Length == 0)
                    return;
                DataRow templateRow = templates[0];

                string reportFullName = templateRow["DocumentFileName"].ToString();
                int templateID = Convert.ToInt32(templateRow["ID"]);

                RemainsReport reportServer = new RemainsReport(WorkplaceSingleton.Workplace.ActiveScheme);
                DataTable[] reportData = reportServer.GetReportData(dtRemains);

                using (OfficeReportsHelper helper = new ExcelReportHelper(FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme))
                {
                    TemplatesDocumentsHelper documentsHelper =
                        new TemplatesDocumentsHelper(
                            FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.TemplatesService.Repository);
                    string newCaption =
                        string.Format("{0}{1}{2}_", DateTime.Today.Year, DateTime.Today.Month.ToString().PadLeft(2, '0'), DateTime.Today.Day) +
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
