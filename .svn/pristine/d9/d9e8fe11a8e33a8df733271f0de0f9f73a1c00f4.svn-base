using System;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Commands
{
	/// <summary>
	/// Открыть для изменения.
	/// </summary>
	internal class EditTemplateCommand : TemplatesCommand
	{
		/// <summary>
		/// Открыть для изменения.
		/// </summary>
		internal EditTemplateCommand()
		{
			key = GetType().Name;
			caption = "Открыть для изменения";
			iconKey = "FileEdit";
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

				int templateID = Convert.ToInt32(activeRow.Cells[TemplateFields.ID].Value);

				if (content.IsNewTemplate(templateID))
				{
					MessageBox.Show(
						"Перед редактированием документа необходимо сначала сохранить изменения.", 
						"Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				
				string filePath = content.GetDocumentFullFilePath(templateID);

				// Блокируем шаблон
				content.LockTemplate(templateID);

				if (activeRow.Cells[TemplateFields.LastEditData].Value == DBNull.Value)
				{
					// Сохраняем документ во временный каталог с документами
					content.SaveDocumentToTempFolder(templateID);
				}

				// Открываем документ
				System.Diagnostics.Process.Start(filePath);

				// Запоминаем время сохранения, при закрытии всего воркплейса проверяем,
				// отличается ли дата последнего изменения от даты создания
				// если да, сохраняем документ в базу
				FileInfo fi = new FileInfo(filePath);
				activeRow.Cells[TemplateFields.LastEditData].Value = fi.LastWriteTimeUtc;
				activeRow.Cells[TemplateFields.DocumentFileName].Value = filePath;
				activeRow.Update();
			}
		}
	}
}
