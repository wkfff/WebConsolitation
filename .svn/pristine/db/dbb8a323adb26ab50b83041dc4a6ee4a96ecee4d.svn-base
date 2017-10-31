using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal partial class TemplatestUIBase
	{
		private bool OnSaveToXML(object sender)
		{
			bool exportAllRows = true;
			bool exportHiaerarchy = true;
			if (FormImportParameters.ShowImportParams((IWin32Window)Workplace, ref exportAllRows, ref exportHiaerarchy))
			{
			    string tmpFileName = string.Empty;
                if (!exportAllRows && ViewObject.ugeTemplates.ugData.Selected.Rows.Count == 1)
                {
                    tmpFileName = ViewObject.ugeTemplates.ugData.Selected.Rows[0].Cells["Name"].Value.ToString();
                }
                else
                    tmpFileName = ViewObject.ugeTemplates.SaveLoadFileName;
				if (ExportImportHelper.GetFileName(tmpFileName, ExportImportHelper.fileExtensions.xml, true, ref tmpFileName))
				{
					Workplace.OperationObj.Text = "Сохранение данных";
					Workplace.OperationObj.StartOperation();
					var stream = new FileStream(tmpFileName, FileMode.Create);
					try
					{
						if (!exportAllRows)
						{
							HierarchyInfo hi = new HierarchyInfo();
							hi.loadMode = LoadMode.AllRows;
							List<int> selectedIds;
							UltraGridHelper.GetSelectedIds(ViewObject.ugeTemplates, hi, out selectedIds, exportHiaerarchy);
							repository.RepositoryExport(stream, selectedIds, templateType);
						}
						else
							repository.RepositoryExport(stream, templateType);
						return true;
					}
					finally
					{
						stream.Close();
						Workplace.OperationObj.StopOperation();
					}
				}
			}
			return false;
		}

		private bool OnLoadFromXML(object sender)
		{
			string tmpFileName = ViewObject.ugeTemplates.SaveLoadFileName;
			bool addToTopLevel = !(Grid.Rows.Count > 0 && Grid.ActiveRow != null);
			if (ImportParams.ShowForm(ref addToTopLevel))
			{
				if (ExportImportHelper.GetFileName(tmpFileName, ExportImportHelper.fileExtensions.xml, false, ref tmpFileName))
				{
					Workplace.OperationObj.Text = "Загрузка данных";
					Workplace.OperationObj.StartOperation();
					FileStream stream = new FileStream(tmpFileName, FileMode.Open, FileAccess.Read);
					try
					{
						if (addToTopLevel)
							repository.RepositoryImport(stream, templateType);
						else
							repository.RepositoryImport(stream, UltraGridHelper.GetActiveID(Grid), templateType);
					}
					finally
					{
						stream.Close();
						Workplace.OperationObj.StopOperation();
					}

					ReloadData();
				}
			}
			return false;
		}
	}
}
