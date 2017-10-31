using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.TemplatesUI.Commands;
using Krista.FM.Client.Workplace.Gui;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
using EventGroups = Infragistics.Win.UltraWinGrid.EventGroups;

using Krista.FM.Client.Components;
using Krista.FM.Common;
using Krista.FM.Common.Services;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal partial class TemplatestUIBase
	{
		private const string imageColumn = "templateTypeImg";

		private void OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			UltraGrid grid = (UltraGrid)sender;

			ValueList list;
			if (!ViewObject.ugeTemplates.ugData.DisplayLayout.ValueLists.Exists("TemplatesTypes"))
			{
				list = ViewObject.ugeTemplates.ugData.DisplayLayout.ValueLists.Add("TemplatesTypes");
				ValueListItem item = list.ValueListItems.Add("item0");
				item.DisplayText = "Группа отчетов (0)";
				item.DataValue = 0;

				item = list.ValueListItems.Add("item1");
				item.DisplayText = "Документ MS Word (1)";
				item.DataValue = 1;

				item = list.ValueListItems.Add("item2");
				item.DisplayText = "Документ MS Excel (2)";
				item.DataValue = 2;

				item = list.ValueListItems.Add("item3");
				item.DisplayText = "Документ MDX Эксперт (3)";
				item.DataValue = 3;

				item = list.ValueListItems.Add("item4");
				item.DisplayText = "Документ MDX Эксперт 3 (4)";
				item.DataValue = 4;

				item = list.ValueListItems.Add("item5");
				item.DisplayText = "Документ надстройки MS Word (5)";
				item.DataValue = 5;

				item = list.ValueListItems.Add("item6");
				item.DisplayText = "Документ надстройки MS Excel (6)";
				item.DataValue = 6;

				item = list.ValueListItems.Add("item7");
				item.DisplayText = "Произвольный документ (7)";
				item.DataValue = 7;

				item = list.ValueListItems.Add("item8");
				item.DisplayText = "Шаблон документа MS Word (8)";
				item.DataValue = 8;

				item = list.ValueListItems.Add("item9");
				item.DisplayText = "Шаблон документа MS Excel (9)";
				item.DataValue = 9;

				item = list.ValueListItems.Add("item10");
				item.DisplayText = "Веб-отчет (10)";
				item.DataValue = 10;
			}
			else
				list = ViewObject.ugeTemplates.ugData.DisplayLayout.ValueLists["TemplatesTypes"];

			foreach (UltraGridBand band in grid.DisplayLayout.Bands)
			{
				band.Columns["Type"].ValueList = list;
				band.Columns["Type"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

				UltraGridColumn uImageColumn = band.Columns.Add(imageColumn);
				UltraGridHelper.SetLikelyImageColumnsStyle(uImageColumn, -1);
				uImageColumn.Header.VisiblePosition = 1;
			}
		}

		protected virtual GridColumnsStates OnGetGridColumnsState(object sender)
		{
			GridColumnsStates states = new GridColumnsStates();

			GridColumnState state = new GridColumnState();
			state.ColumnName = UltraGridEx.StateColumnName;
			state.ColumnPosition = 0;
			states.Add(UltraGridEx.StateColumnName, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.ID;
			state.ColumnCaption = TemplateFields.ID;
			state.IsReadOnly = true;
			state.IsHiden = true;
			state.ColumnPosition = 2;
			states.Add(TemplateFields.ID, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.Flags;
			state.ColumnCaption = "Флаги";
			state.IsHiden = true;
			state.ColumnWidth = 60;
			state.ColumnPosition = 3;
			states.Add(TemplateFields.Flags, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.Name;
			state.ColumnCaption = "Наименование";
			state.ColumnWidth = 200;
			state.ColumnPosition = 4;
			states.Add(TemplateFields.Name, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.Description;
			state.ColumnCaption = "Описание";
			state.ColumnWidth = 500;
			state.ColumnPosition = 5;
			states.Add(TemplateFields.Description, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.Code;
			state.ColumnCaption = "Код";
			state.ColumnWidth = 100;
			state.IsHiden = true;
			state.ColumnPosition = 6;
			states.Add(TemplateFields.Code, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.Type;
			state.ColumnCaption = "Тип отчета репозитория";
			state.ColumnStyle = ColumnStyle.DropDownList;
			state.ColumnWidth = 200;
			state.ColumnPosition = 7;
			states.Add(TemplateFields.Type, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.DocumentFileName;
			state.ColumnCaption = "Файл";
			state.ColumnWidth = 300;
			state.IsHiden = true;
			state.ColumnPosition = 20;
			states.Add(TemplateFields.DocumentFileName, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.ParentID;
			state.ColumnCaption = "Родитель";
			state.IsHiden = true;
			state.ColumnPosition = 20;
			states.Add(TemplateFields.ParentID, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.Document;
			state.ColumnCaption = "Документ";
			state.IsHiden = true;
			state.ColumnPosition = 21;
			states.Add(TemplateFields.Document, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.Editor;
			state.ColumnCaption = "Пользователь";
		    state.IsReadOnly = true;
			state.IsHiden = true;
			state.ColumnPosition = 22;
			states.Add(TemplateFields.Editor, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.RefTemplatesTypes;
			state.ColumnCaption = TemplateFields.RefTemplatesTypes;
			state.IsHiden = true;
			state.ColumnPosition = 23;
			states.Add(TemplateFields.RefTemplatesTypes, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.IsVisible;
			state.ColumnCaption = TemplateFields.IsVisible;
			state.IsHiden = true;
			state.ColumnPosition = 24;
			states.Add(TemplateFields.IsVisible, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.SortIndex;
			state.ColumnCaption = TemplateFields.SortIndex;
			state.IsHiden = true;
			state.ColumnPosition = 25;
			states.Add(TemplateFields.SortIndex, state);

			state = new GridColumnState();
			state.ColumnName = TemplateFields.LastEditData;
			state.ColumnCaption = TemplateFields.LastEditData;
			state.IsHiden = true;
			state.ColumnPosition = 26;
			states.Add(TemplateFields.LastEditData, state);

			return states;
		}

		private void OnInitializeRow(object sender, InitializeRowEventArgs e)
		{
			UltraGridRow row = e.Row;

			// Поле Flags
			UltraGridCell flagsCell = row.Cells[TemplateFields.Flags];
			flagsCell.Style = ColumnStyle.Image;
			flagsCell.Activation = Activation.Disabled;

			// Тип шаблона
			if (row.Cells[TemplateFields.Type].Value == null || 
				row.Cells[TemplateFields.Type].Value == DBNull.Value || 
				String.IsNullOrEmpty(row.Cells[TemplateFields.Type].Value.ToString()))
				return;

			TemplateDocumentTypes templateDocumentType = (TemplateDocumentTypes)Convert.ToInt32(row.Cells[TemplateFields.Type].Value);
			SetTemplateImage(templateDocumentType, row.Cells[imageColumn]);

			// Блокировка строки шаблона
			int templateUserEditor = Convert.ToInt32(row.Cells[TemplateFields.Editor].Value);

			if (templateUserEditor != -1 && templateUserEditor != ClientAuthentication.UserID)
			{
				row.Cells[UltraGridEx.StateColumnName].Appearance.Image = ResourceService.GetIcon("Lock").ToBitmap();
				row.Cells[UltraGridEx.StateColumnName].ToolTipText =
					String.Format("Заблокированно пользователем \"{0}\"", 
					WorkplaceSingleton.Workplace.ActiveScheme.UsersManager.GetUserNameByID(templateUserEditor));
				row.Activation = Activation.NoEdit;
			}

			if (row.IsDeleted)
				row.Activation = Activation.NoEdit;

			if (!Convert.ToBoolean(row.Cells[TemplateFields.IsVisible].Value))
				row.Activation = Activation.Disabled;
		}

		private void SetTemplateImage(TemplateDocumentTypes templateDocumentType, UltraGridCell imageCell)
		{
			switch (templateDocumentType)
			{
				case TemplateDocumentTypes.Arbitrary:
					imageCell.Appearance.Image = ViewObject.ilTemplatesType.Images[3];
					return;
				case TemplateDocumentTypes.Group:
					imageCell.Appearance.Image = ViewObject.ilTemplatesType.Images[0];
					return;
				case TemplateDocumentTypes.MDXExpert:
				case TemplateDocumentTypes.MDXExpert3:
					imageCell.Appearance.Image = ViewObject.ilTemplatesType.Images[4];
					return;
				case TemplateDocumentTypes.MSExcel:
				case TemplateDocumentTypes.MSExcelTemplate:
				case TemplateDocumentTypes.MSExcelPlaning:
					imageCell.Appearance.Image = ViewObject.ilTemplatesType.Images[2];
					return;
				case TemplateDocumentTypes.MSWord:
				case TemplateDocumentTypes.MSWordTemplate:
				case TemplateDocumentTypes.MSWordPlaning:
					imageCell.Appearance.Image = ViewObject.ilTemplatesType.Images[1];
					return;
				case TemplateDocumentTypes.WebReport:
					imageCell.Appearance.Image = ViewObject.ilTemplatesType.Images[5];
					return;
			}
		}

		private void OnAfterRowActivate(object sender, EventArgs e)
		{
			if (Grid.ActiveRow == null)
				return;

			if (Grid.ActiveRow.Cells[TemplateFields.ID].Value is DBNull)
			{
				return;
			}

			if (Convert.ToBoolean(Grid.ActiveRow.Cells[TemplateFields.IsVisible].Value))
			{

				int activeRowId = UltraGridHelper.GetActiveID(Grid);

				SetActiveTemplatePermissions(activeRowId, (TemplateDocumentTypes)Convert.ToInt32(Grid.ActiveRow.Cells[TemplateFields.Type].Value));

				SetActiveTemplateAvailableCommands(Grid.ActiveRow);

				SetDetailVisible(true);
			}
			else
			{
				foreach (TemplatesCommand command in CommandList.Values)
				{
					if (command.IsRowCommand)
						command.IsEnabled = false;
				}
				SetDetailVisible(false);
			}
		}

		internal void SetActiveTemplateAvailableCommands(UltraGridRow row)
		{
			// Определяем доступность шаблона
			bool docAvailable = DocAvailable(row);

			// Определяем заблокирован ли шиблон
			int templateUserEditor = Convert.ToInt32(row.Cells[TemplateFields.Editor].Value);
			bool docLocked = templateUserEditor != -1 && templateUserEditor != ClientAuthentication.UserID;

			CommandList[typeof(OpenTemplateCommand).Name].IsEnabled &= docAvailable;
			CommandList[typeof(SaveTemplateCommand).Name].IsEnabled &= docAvailable;
			CommandList[typeof(EditTemplateCommand).Name].IsEnabled &= docAvailable & !docLocked;
			CommandList[typeof(AddDocumentTemplateCommand).Name].IsEnabled &= !docLocked;
		}

		private void OnCellChange(object sender, CellEventArgs e)
		{
            /*
			if (e.Cell.Column.Key.ToUpper() == TemplateFields.Type.ToUpper())
			{
				// Если к шаблону прикреплен документ, то запрещаем менять тип на "Группа отчетов"
				if (Convert.ToInt32(e.Cell.Value) != 0 && e.Cell.ValueListResolved.SelectedItemIndex == 0 && 
					DocAvailable(e.Cell.Row))
				{
					MessageBox.Show((IWin32Window) Workplace,
						"Невозможно изменить текущий тип шаблона на тип \"Группа отчетов\", так как к шаблону прикреплен документ, который будет утерян.",
						"Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					e.Cell.CancelUpdate();
					return;
				}
			}

			e.Cell.Row.Refresh(RefreshRow.ReloadData);

			try
			{
				LockTemplate(Convert.ToInt32(e.Cell.Row.Cells[TemplateFields.ID].Value));
				
				if (e.Cell.Column.Key.ToUpper() == TemplateFields.Type.ToUpper())
				{
					SetActiveTemplatePermissions(Convert.ToInt32(e.Cell.Row.Cells[TemplateFields.ID].Value),
					                             (TemplateDocumentTypes)
					                             Convert.ToInt32(
					                             	e.Cell.Row.Cells[TemplateFields.Type].ValueListResolved.SelectedItemIndex));
				}
				SetActiveTemplateAvailableCommands(e.Cell.Row);
			}
			catch (TemplateLokedException ex)
			{
				MessageBox.Show((IWin32Window)Workplace, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Cell.CancelUpdate();
			}*/
		}

        void ugData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            if (e.Cell.Column.Key.ToUpper() == TemplateFields.Type.ToUpper())
            {
                // Если к шаблону прикреплен документ, то запрещаем менять тип на "Группа отчетов"
                if (Convert.ToInt32(e.NewValue) == 0 && e.Cell.ValueListResolved.SelectedItemIndex == 0 &&
                    DocAvailable(e.Cell.Row))
                {
                    MessageBox.Show((IWin32Window)Workplace,
                        "Невозможно изменить текущий тип шаблона на тип \"Группа отчетов\", так как к шаблону прикреплен документ, который будет утерян.",
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }
        }

		protected virtual void OnAfterCellUpdate(object sender, CellEventArgs e)
		{
            e.Cell.Row.Refresh(RefreshRow.ReloadData);

            try
            {
                LockTemplate(Convert.ToInt32(e.Cell.Row.Cells[TemplateFields.ID].Value));

                if (e.Cell.Column.Key.ToUpper() == TemplateFields.Type.ToUpper())
                {
                    SetActiveTemplatePermissions(Convert.ToInt32(e.Cell.Row.Cells[TemplateFields.ID].Value),
                                                 (TemplateDocumentTypes)
                                                 Convert.ToInt32(
                                                    e.Cell.Row.Cells[TemplateFields.Type].ValueListResolved.SelectedItemIndex));
                }
                SetActiveTemplateAvailableCommands(e.Cell.Row);
            }
            catch (TemplateLokedException ex)
            {
                MessageBox.Show((IWin32Window)Workplace, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cell.CancelUpdate();
            }
		}

		private void OnAfterRowInsert(object sender, UltraGridRow row)
		{
			UltraGrid ug = (UltraGrid)sender;

			ug.EventManager.SetEnabled(EventGroups.AllEvents, false);
			try
			{
				int newTemplateId = Repository.NewTemplateID();

				row.Cells[TemplateFields.ID].Value = newTemplateId;
				row.Cells[TemplateFields.Code].Value = String.Empty;
				row.Cells[TemplateFields.Editor].Value = -1;
				row.Cells[TemplateFields.RefTemplatesTypes].Value = templateType;
				row.Cells[TemplateFields.Flags].Value = 0;

				// Устанавливаем SortIndex
				if (row.Cells[TemplateFields.ParentID].Value is DBNull)
				{
					DataRow[] rows = dtTemplates.Select("ParentID is null and SortIndex is not null", "SortIndex desc");
					row.Cells[TemplateFields.SortIndex].Value = rows.GetLength(0) == 0 ? 1 : Convert.ToInt32(rows[0][TemplateFields.SortIndex]) + 1;
				}
				else
				{
					DataRow[] rows = dtTemplates.Select(String.Format("ParentID = {0} and SortIndex is not null", Convert.ToInt32(row.Cells[TemplateFields.ParentID].Value)), "SortIndex desc");
					row.Cells[TemplateFields.SortIndex].Value = rows.GetLength(0) == 0 ? 1 : Convert.ToInt32(rows[0][TemplateFields.SortIndex]) + 1;
				}

				SetActiveTemplateAvailableCommands(row);
			}
			finally
			{
				ug.EventManager.SetEnabled(EventGroups.AllEvents, true);
			}
		}

		private readonly List<int> childDeleteRows = new List<int>();

		private bool DeleteChildRows(UltraGridRow delRow)
		{
			if (delRow.ChildBands == null)
				return true;

			foreach (UltraGridChildBand band in delRow.ChildBands)
			{
				foreach (UltraGridRow row in band.Rows)
				{
					int templateId = Convert.ToInt32(row.Cells[TemplateFields.ID].Value);
					if (!CheckCanEditTemplate(templateId))
					{
						return false;
					}

					if (!DeleteChildRows(row))
						return false;
					
					LockTemplate(templateId);
					if (!childDeleteRows.Contains(templateId))
						childDeleteRows.Add(templateId);
					row.Delete();
					row.Activation = Activation.Disabled;
				}
			}
			return true;
		}

		private void OnBeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
		{
			Workplace.OperationObj.Text = "Удаление...";
			Workplace.OperationObj.StartOperation();
			try
			{
				if (!CheckCanEditTemplate(Convert.ToInt32(Grid.ActiveRow.Cells[TemplateFields.ID].Value)))
				{
					MessageBox.Show((IWin32Window) Workplace, "Недостаточно прав для удаления.", "Система прав", MessageBoxButtons.OK,
					                MessageBoxIcon.Exclamation);
					e.Cancel = true;
					return;
				}

				foreach (UltraGridRow delRow in e.Rows)
				{
					if (!DeleteChildRows(delRow))
					{
						MessageBox.Show((IWin32Window) Workplace, "Недостаточно прав для удаления одной из подчиненных записей.",
						                "Система прав", MessageBoxButtons.OK,
						                MessageBoxIcon.Exclamation);
						e.Cancel = true;
						return;
					}

					if (!IsNewTemplate(Convert.ToInt32(delRow.Cells[TemplateFields.ID].Value)))
						Repository.LockTemplate(Convert.ToInt32(delRow.Cells[TemplateFields.ID].Value));

					delRow.Activation = Activation.Disabled;
				}
			}
			finally
			{
				Workplace.OperationObj.StopOperation();
			}
		}

		#region проверки на заполнение всех обязательных полей

		/// <summary>
		/// проверяем, все ли основные поля в записях заполнены
		/// </summary>
		/// <returns></returns>
		private bool CheckNotNullFields()
		{
			List<string> errorsList = new List<string>();
			foreach (DataRow row in dtTemplates.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				object id = row[TemplateFields.ID];
				if (row.IsNull(TemplateFields.Type))
				{
					errorsList.Add(string.Format("Запись с ID = {0}. Поле '{1}' не заполнено", id, "Тип элемента репозитория"));
				}
				if (row.IsNull(TemplateFields.Name) || String.IsNullOrEmpty(row[TemplateFields.Name].ToString()))
				{
					errorsList.Add(string.Format("Запись с ID = {0}. Поле '{1}' не заполнено", id, "Наименование"));
				}
			}

			if (errorsList.Count == 0)
			{
				return true;
			}

			StringBuilder sb = new StringBuilder();
			foreach (string str in errorsList)
			{
				sb.AppendLine(str);
			}
			MessageBox.Show(sb.ToString(), "Предупреждение");
			return false;
		}

		#endregion

		#region Утилиты

		internal UltraGrid Grid
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return ViewObject.ugeTemplates.ugData; }
		}

		private static void ActiveateFirstGridRow(UltraGridBase grid)
		{
			if (grid.Rows.Count > 0)
			{
				foreach (UltraGridRow row in grid.Rows)
				{
					if (row.Band.Index == 0 && 
						row.VisibleIndex >= 0 && 
						Convert.ToBoolean(row.Cells[TemplateFields.IsVisible].Value))
					{
						row.Activate();
						break;
					}
				}
			}
		}

		private static UltraGridRow GetUltraGridRowById(UltraGridBase grid, int templateId)
		{
			if (grid.ActiveRow != null)
			{
				if (Convert.ToInt32(grid.ActiveRow.Cells[TemplateFields.ID].Value) == templateId)
				{
					return grid.ActiveRow;
				}
			}

			foreach (object item in grid.Rows.All)
			{
				UltraGridRow row = (UltraGridRow)item;
				if (row.IsDataRow)
				{
					if (Convert.ToInt32(row.Cells[TemplateFields.ID].Value) == templateId)
					{
						return row;
					}
				}
			}
			
			return null;
		}

		private bool DocAvailable(UltraGridRow row)
		{
			bool docAvailable;
			if (row.IsAddRow)
			{
				docAvailable = row.Cells[TemplateFields.Document].Value != DBNull.Value;
			}
			else
			{
				docAvailable = repository.ExistDocument(Convert.ToInt32(row.Cells[TemplateFields.ID].Value)) || row.Cells[TemplateFields.Document].Value != DBNull.Value;
			}
			return docAvailable;
		}

		#endregion Утилиты
	}
}
