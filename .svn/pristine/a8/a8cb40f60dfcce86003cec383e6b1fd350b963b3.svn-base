using System;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Commands
{
	/// <summary>
	/// Открыть для посмотра.
	/// </summary>
	internal class OpenTemplateCommand : TemplatesCommand
	{
		/// <summary>
		/// Открыть для посмотра.
		/// </summary>
		internal OpenTemplateCommand()
		{
			key = GetType().Name;
			caption = "Открыть для просмотра";
			iconKey = "OpedDocument";
			IsRowCommand = true;
		}

		public override void Run()
		{
			TemplatestUIBase content = (TemplatestUIBase)WorkplaceSingleton.Workplace.ActiveContent;

			UltraGridRow activeRow = content.Grid.ActiveRow;
			if (activeRow != null)
			{
				if (activeRow.DataChanged)
					activeRow.Update();
				int templateId = Convert.ToInt32(activeRow.Cells[TemplateFields.ID].Value);
				string filePath = content.GetDocumentFullFilePath(templateId);

				if (activeRow.Cells[TemplateFields.LastEditData].Value == DBNull.Value)
				{
					// Сохраняем документ во временный каталог с документами
					content.SaveDocumentToTempFolder(templateId);
				}

				// Открываем документ
				System.Diagnostics.Process.Start(filePath);
			}
		}
	}
}
