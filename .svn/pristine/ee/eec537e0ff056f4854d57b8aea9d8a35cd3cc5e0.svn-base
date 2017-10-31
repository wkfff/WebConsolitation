using System;
using System.Windows.Forms;
using System.IO;

using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common;
using Krista.FM.Common.FileUtils;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Commands
{
	/// <summary>
	/// Вставить документ с диска.
	/// </summary>
	internal class AddDocumentTemplateCommand : TemplatesCommand
	{
		internal AddDocumentTemplateCommand()
		{
			key = GetType().Name;
			caption = "Вставить документ с диска";
			iconKey = "AddFile";
			IsRowCommand = true;
		}

		public override void Run()
		{
			AddFileToTemplate(String.Empty, (TemplatestUIBase)WorkplaceSingleton.Workplace.ActiveContent);
		}

		internal static void AddFileToTemplate(string filePath, TemplatestUIBase content)
		{
			UltraGridRow activeRow = content.Grid.ActiveRow;
			if (activeRow == null)
				return;

			if (activeRow.DataChanged)
				activeRow.Update();

			int templateId = Convert.ToInt32(activeRow.Cells[TemplateFields.ID].Value);

			if (!content.CheckCanEditTemplate(templateId))
			{
				MessageBox.Show(WorkplaceSingleton.MainForm, 
					"Недостаточно прав для изменения отчета.", "Система прав", 
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			// если документ в базе есть, то спросим пользователя о замене
			string documentExists;
			if (content.IsNewTemplate(templateId) && !(activeRow.Cells[TemplateFields.Document].Value is DBNull))
				documentExists = activeRow.Cells[TemplateFields.DocumentFileName].Value.ToString();
			else
				documentExists = content.GetDocument(templateId) == null ? String.Empty : content.GetDocumentFileName(templateId);

			if (!String.IsNullOrEmpty(documentExists) &&
				MessageBox.Show(WorkplaceSingleton.MainForm, 
					String.Format("К шаблону уже прикреплен документ \"{0}\". Заменить?", documentExists),
					"Информация", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				return;

			// Запрашиваем файл у пользователя
			if (String.IsNullOrEmpty(filePath))
			{
				OpenFileDialog openDialog = new OpenFileDialog();
				openDialog.Multiselect = false;
				openDialog.Filter = TemplatesDocumentsHelper.GetFilters();
				openDialog.FilterIndex = TemplatesDocumentsHelper.GetFilterIndexForType((TemplateDocumentTypes) Convert.ToInt32(activeRow.Cells[TemplateFields.Type].Value)) + 1;
				if (openDialog.ShowDialog() == DialogResult.OK)
				{
					filePath = openDialog.FileName;
				}
			}

			if (!String.IsNullOrEmpty(filePath))
			{
				// Блокируем шаблон
				content.LockTemplate(templateId);

				// Прикрепляем файл к шаблону
				FileInfo fi = new FileInfo(filePath);

				fi = fi.CopyTo(Path.Combine(TemplatesDocumentsHelper.GetDocsFolder(), fi.Name), true);
				
				activeRow.Cells[TemplateFields.LastEditData].Value = fi.LastWriteTimeUtc;
				activeRow.Cells[TemplateFields.DocumentFileName].Value = fi.Name;
				activeRow.Cells[TemplateFields.Type].Value = (int)content.GetTemplateType(fi.Extension);
				activeRow.Cells[TemplateFields.Name].Value = Path.GetFileNameWithoutExtension(fi.Name);

				activeRow.Cells[TemplateFields.Document].Value 
					= DocumentsHelper.CompressFile(FileHelper.ReadFileData(fi.FullName));

				content.SetActiveTemplatePermissions(templateId, (TemplateDocumentTypes)Convert.ToInt32(activeRow.Cells[TemplateFields.Type].Value));
				content.SetActiveTemplateAvailableCommands(activeRow);
			}
		}
	}
}
