using System;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.Common;
using Krista.FM.Common.Templates;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Commands
{
	/// <summary>
	/// Сохранить документ на диск.
	/// </summary>
	internal class SaveTemplateCommand : TemplatesCommand
	{
		internal SaveTemplateCommand()
		{
			key = GetType().Name;
			caption = "Сохранить документ на диск";
			iconKey = "SaveAs";
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

				int templateID = Convert.ToInt32(activeRow.Cells["ID"].Value);

				byte[] document = content.GetDocument(templateID);
				if (document != null)
				{
					string fileName = content.GetDocumentFileName(templateID);
					if (ShowSaveDialog(ref fileName))
						TemplatesDocumentsHelper.SaveDocument(fileName, document);
				}
				else
					MessageBox.Show("У шаблона нет прикрепленного документа.", 
						"Информация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					
			}
		}

        public static bool ShowSaveDialog(ref string documentName)
        {
            string ext = Path.GetExtension(documentName);
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = string.Format("*{0}|*{0}", ext);
            fd.FileName = documentName;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                documentName = fd.FileName;
                return true;
            }
            return false;
        }
	}
}
