using System;
using System.IO;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.BorrowingVolume;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.VolumeHoldings;
using System.Data;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.Workplace.Gui;
using System.Windows.Forms;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class CalculateBorrowingValumeCommand : AbstractCommand
    {
        public CalculateBorrowingValumeCommand()
        {
            key = "CalculateBorrowingValume";
            caption = "Расчет необходимого объема заимствований";
        }

        public override void Run()
        {
            int incomeVariant = -1;
            int outcomeVariant = -1;
            int ifVariant = FinSourcePlanningNavigation.Instance.CurrentVariantID > 0 ? FinSourcePlanningNavigation.Instance.CurrentVariantID : -1;
            string ifVariantCaption = ifVariant > 0 ? FinSourcePlanningNavigation.Instance.CurrentVariantCaption : string.Empty;
            BorrowingVolumeBudgetType budgetDataType = BorrowingVolumeBudgetType.BudgetList;
            decimal euroRate = 0;
            decimal dollarRate = 0;
            if (BorrowingVolumeForm.ShowVariantForm(WorkplaceSingleton.Workplace.WindowHandle, true,
                ifVariantCaption, ref incomeVariant, ref outcomeVariant, ref ifVariant, ref budgetDataType, ref euroRate, ref dollarRate))
            {
                WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет необходимого объема заимствований";
                WorkplaceSingleton.Workplace.OperationObj.StartOperation();
                try
                {
                    BorrowingVolumeUI clsUI = (BorrowingVolumeUI) WorkplaceSingleton.Workplace.ActiveContent;
                    clsUI.ClearData();
                    BorrowingVolumeServer server = new BorrowingVolumeServer(WorkplaceSingleton.Workplace.ActiveScheme);
                    DataRow[] rows = server.VolumeHoldingResults(FinSourcePlanningNavigation.Instance.CurrentSourceID,
                        ifVariant, incomeVariant, outcomeVariant, euroRate, dollarRate, budgetDataType);
                    foreach (DataRow row in rows)
                        clsUI.AddData(row);
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Расчет необходимого объема заимствований успешно завершен",
                        "Определение необходимых заимствований", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    throw;
                }
            }
        }
    }

    public class BorrowingValumeReportCommand : AbstractCommand
    {
        public BorrowingValumeReportCommand()
        {
            key = "BorrowingValumeReport";
            caption = "Отчет по необходимым заимствованиям";
        }

        public override void Run()
        {
            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Получение и обработка данных";
            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
            try
            {
                BorrowingVolumeUI clsUI = (BorrowingVolumeUI)WorkplaceSingleton.Workplace.ActiveContent;
                DataTable dtBorrowing = clsUI.GetBorrowingData();
                if (dtBorrowing.Rows.Count == 0) return;
                

                DataTable dtTemplates = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
                DataRow[] templates = dtTemplates.Select(String.Format("Code = '{0}'", "VolumeHoldings"));
                if (templates.Length == 0)
                    return;
                DataRow templateRow = templates[0];

                string reportFullName = templateRow["DocumentFileName"].ToString();
                int templateID = Convert.ToInt32(templateRow["ID"]);

                VolumeHoldingsReport reportServer = new VolumeHoldingsReport(WorkplaceSingleton.Workplace.ActiveScheme);
                DataTable[] reportData = reportServer.GetReportData(dtBorrowing);

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
