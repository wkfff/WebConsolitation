using System;
using System.IO;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.Common.Templates;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
	public class CreateReportCommand : AbstractCommand
	{
		private int reportID;
		private string reportFullName;

		public CreateReportCommand(int reportID, string name, string documentFileName)
		{
			this.reportID = reportID;
			key = String.Format("ReportID_{0}", reportID);
			caption = name;
			reportFullName = documentFileName;
		}

		public override void Run()
		{
			FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;

			// получаем ID активной записи для построения отчета
			int activeId = UltraGridHelper.GetActiveID(((BaseClsView)content.ViewCtrl).ugeCls.ugData);
            if (activeId < 0)
            {
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle, "Не выбрано ни одного договора",
                    "Источники финансирования", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

		    TemplatesDocumentsHelper documentsHelper = new TemplatesDocumentsHelper(WorkplaceSingleton.Workplace.ActiveScheme.TemplatesService.Repository);
			string templateDocumentName = documentsHelper.SaveDocument(reportID, caption, reportFullName);
			ReportsHelper reportHelper = null;
			switch (Path.GetExtension(templateDocumentName).ToLower())
			{
				case ".xls":
				case ".xlt":
					reportHelper = new ExcelReportHelper(WorkplaceSingleton.Workplace.ActiveScheme);
					break;
				case ".doc":
				case ".dot":
					reportHelper = new WordReportHelper(WorkplaceSingleton.Workplace.ActiveScheme);
					break;
			}
		    if (reportHelper != null)
		    {
                try
                {
                    reportHelper.CreateReport(activeId, templateDocumentName);
                }
                finally
                {
                    reportHelper.Dispose();
                }
		    }
		}
	}
}
