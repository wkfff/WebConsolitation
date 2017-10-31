using System;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal partial class TemplatestUIBase
	{
		internal class DragDropFilesStrategy : IDragDropStrategy
		{
			private readonly TemplatestUIBase content;
			
			internal DragDropFilesStrategy(TemplatestUIBase content)
			{
				this.content = content;
			}

			public void DragEnter(object sender, DragEventArgs e)
			{
				e.Effect = DragDropEffects.Copy;
			}

			public void DragLeave(object sender, EventArgs e)
			{
			}

			public void DragOver(object sender, DragEventArgs e)
			{
				e.Effect = DragDropEffects.None;
				UltraGridRow tmpRow = UltraGridHelper.GetRowFromPos(e.X, e.Y, content.Grid);
				if (e.Data.GetDataPresent("FileDrop"))
				{
					string[] files = (string[])e.Data.GetData("FileDrop");

					if (files.GetLength(0) == 0)
						return;

					if (tmpRow == null)
						e.Effect = DragDropEffects.Copy;
					else
					{
						tmpRow.Activate();
						TemplateDocumentTypes tmpRowType = (TemplateDocumentTypes)Convert.ToInt32(tmpRow.Cells[TemplateFields.Type].Value);

						if (tmpRowType == TemplateDocumentTypes.Group)
						{
							e.Effect = DragDropEffects.Copy;
						}
						else
						{
							if (files.GetLength(0) == 1)
							{
								// если это не директория
								FileAttributes attr = File.GetAttributes(files[0]);
								if (!((attr & FileAttributes.Directory) == FileAttributes.Directory))
									e.Effect = DragDropEffects.Move;
							}
						}
					}
				}
			}

			public void DragDrop(object sender, DragEventArgs e)
			{
				if (!e.Data.GetDataPresent("FileDrop"))
					return;

				UltraGridRow rootRow = UltraGridHelper.GetRowFromPos(e.X, e.Y, content.Grid);
				string[] files = (string[])e.Data.GetData("FileDrop");
				if (e.Effect == DragDropEffects.Move)
				{
					// Добавляем файл в шаблон
					rootRow.Activate();
					Commands.AddDocumentTemplateCommand.AddFileToTemplate(files[0], content);
				}
				else if (e.Effect == DragDropEffects.Copy)
				{
					if (!content.CheckCanCreateTemplates())
					{
						MessageBox.Show(Client.Workplace.Gui.WorkplaceSingleton.MainForm,
							"Недостаточно прав для создания отчета.", "Система прав",
							MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}

					// Добавляем файлы и папки с вложенными в них файлами в репозиторий
					foreach (string path in files)
					{
						AddNewFiles(rootRow, path);
					}
				}
			}

			private void AddNewFiles(UltraGridRow rootRow, string path)
			{
				FileAttributes attr = File.GetAttributes(path);
				if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
				{
					// Добавляем шаблон верхнего уровня
					if (rootRow == null)
					{
						new Commands.CreateRootTemplateCommand().Run();
					}
					else
					{
						rootRow.Activate();
						new Commands.CreateChildTemplateCommand().Run();
					}

					rootRow = content.Grid.ActiveRow;

					rootRow.Cells[TemplateFields.Name].Value = Path.GetFileName(path);
					rootRow.Cells[TemplateFields.Type].Value = TemplateDocumentTypes.Group;

					string[] subItems = Directory.GetDirectories(path);
					foreach (string subPath in subItems)
					{
						AddNewFiles(rootRow, subPath);
					}

					subItems = Directory.GetFiles(path);
					foreach (string subPath in subItems)
					{
						AddNewFiles(rootRow, subPath);
					}
				}
				else
				{
					if (rootRow == null)
					{
						// Добавляем шаблон верхнего уровня
						new Commands.CreateRootTemplateCommand().Run();
						Commands.AddDocumentTemplateCommand.AddFileToTemplate(path, content);
					}
					else
					{
						// Добавляем подчиненный шаблон
						rootRow.Activate();
						new Commands.CreateChildTemplateCommand().Run();
						Commands.AddDocumentTemplateCommand.AddFileToTemplate(path, content);
					}
				}
			}
		}
	}
}
