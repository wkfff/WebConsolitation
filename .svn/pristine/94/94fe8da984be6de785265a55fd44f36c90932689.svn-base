using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal partial class TemplatestUIBase
	{
		internal class DragDropRowsStrategy : IDragDropStrategy
		{
			private readonly TemplatestUIBase content;
			private UltraGridRowDrawFilter drawFilter;

			internal DragDropRowsStrategy(TemplatestUIBase content)
			{
				this.content = content;
			}

			private void FinishDrag()
			{
				drawFilter.ClearDropHighlight();
				drawFilter.Invalidate -= OnDrawFilterInvalidate;
				drawFilter.QueryStateAllowedForRow -= OnQueryStateAllowedForRow;

				content.Grid.DrawFilter = drawFilter.WrappedDrawFilter;
			}

			public void DragEnter(object sender, DragEventArgs e)
			{
				drawFilter = new UltraGridRowDrawFilter(content.Grid.DrawFilter);
				drawFilter.Invalidate += OnDrawFilterInvalidate;
				drawFilter.QueryStateAllowedForRow += OnQueryStateAllowedForRow;

				content.Grid.DrawFilter = drawFilter;

				e.Effect = DragDropEffects.Move;
			}

			public void DragLeave(object sender, EventArgs e)
			{
				FinishDrag();
			}

			public void DragOver(object sender, DragEventArgs e)
			{
				string[] formats = e.Data.GetFormats();
				if (formats.GetLength(0) == 0)
					return;

				SelectedRowsCollection selectedRows = (SelectedRowsCollection)e.Data.GetData(formats[0]);

				UltraGridRow row = content.GetRowFromPos(e.X, e.Y);
				if (row != null && selectedRows != null)
				{
					// запрещаем, если строка является перетаскиваемой
					if (selectedRows.Contains(row))
					{
						drawFilter.ClearDropHighlight();
						e.Effect = DragDropEffects.None;
						return;
					}
					// перетаскивать можно только в пределах одного уровня ветки
					if (selectedRows[0].Band.Index != row.Band.Index)
					{
						drawFilter.ClearDropHighlight();
						e.Effect = DragDropEffects.None;
						return;
					}
					// перетаскивать можно только в пределах одного уровня ветки
					if (!selectedRows[0].Cells[TemplateFields.ParentID].Value.Equals(row.Cells[TemplateFields.ParentID].Value))
					{
						drawFilter.ClearDropHighlight();
						e.Effect = DragDropEffects.None;
						return;
					}
					if (row.IsDataRow)
					{
						e.Effect = DragDropEffects.Move;
						drawFilter.SetDropHighlightNode(row, content.Grid.PointToClient(new Point(e.X, e.Y)));
						return;
					}
				}
				drawFilter.ClearDropHighlight();
				e.Effect = DragDropEffects.None;
			}

			public void DragDrop(object sender, DragEventArgs e)
			{
				UltraGridRowDrawFilter filter = drawFilter;
				try
				{
					// определяем целевую позицию перетаскивания (куда вставлять)
					int insertPosition;
					if (filter.DropHightLightRow.Cells[TemplateFields.SortIndex].Value is DBNull)
						insertPosition = 1;
					else
						insertPosition = Convert.ToInt32(filter.DropHightLightRow.Cells[TemplateFields.SortIndex].Value);

					if (drawFilter.DropLinePosition == DropLinePositionEnum.BelowNode)
					{
						insertPosition++;
					}

					//-----------------------------------------------
					// Сдвигаем вниз позицию у всех строк, которые ниже целевой позиции перетаскивания
					
					//заначение на которое нужно изменить позицию у других строк 
					int incrementValue = content.Grid.Selected.Rows.Count;

					DataRow[] branchRows;
					if (filter.DropHightLightRow.Cells[TemplateFields.ParentID].Value is DBNull)
					{
						branchRows = content.dtTemplates.Select("ParentID is null", "SortIndex asc");
					}
					else
					{
						branchRows = content.dtTemplates.Select(
							String.Format("ParentID = {0}", Convert.ToInt32(filter.DropHightLightRow.Cells[TemplateFields.ParentID].Value)),
							"SortIndex asc");
					}
					foreach (DataRow branchRow in branchRows)
					{
						if (!(branchRow[TemplateFields.SortIndex] is DBNull) && 
							insertPosition > Convert.ToInt32(branchRow[TemplateFields.SortIndex]))
							continue;

						bool skipRow = false;
						foreach (UltraGridRow row in content.Grid.Selected.Rows)
						{
							if (Convert.ToInt32(branchRow[TemplateFields.ID]) == Convert.ToInt32(row.Cells[TemplateFields.ID].Value))
							{
								skipRow = true;
								incrementValue--;
								break;
							}
						}
						if (skipRow)
							continue;

						UltraGridRow gridRow = GetUltraGridRowById(content.Grid, Convert.ToInt32(branchRow[TemplateFields.ID]));

						int rowPositionIndex;
						if (gridRow.Cells[TemplateFields.SortIndex].Value is DBNull)
							rowPositionIndex = 1;
						else
							rowPositionIndex = Convert.ToInt32(gridRow.Cells[TemplateFields.SortIndex].Value);

						int originalValue = Convert.ToInt32(gridRow.Cells[TemplateFields.SortIndex].Value);
						int newValue = rowPositionIndex + incrementValue;
						if (originalValue != newValue)
						{
							gridRow.Cells[TemplateFields.SortIndex].Value = newValue;
							gridRow.Update();
						}
					}

					// устанавливаем новую позицию для перетаскиваемых строк
					foreach (UltraGridRow row in content.Grid.Selected.Rows)
					{
						row.Cells[TemplateFields.SortIndex].Value = insertPosition++;
						row.RefreshSortPosition();
						row.Update();
					}
				}
				finally
				{
					FinishDrag();
				}
			}

			private void OnQueryStateAllowedForRow(UltraGridRowDrawFilter sender, UltraGridRowDrawFilter.QueryStateAllowedForNodeEventArgs e)
			{
				if (content.Grid.Selected.Rows.Contains(e.Row) ||
					content.Grid.Selected.Rows[0].Band.Index != e.Row.Band.Index ||
					!content.Grid.Selected.Rows[0].Cells[TemplateFields.ParentID].Value.Equals(sender.DropHightLightRow.Cells[TemplateFields.ParentID].Value))
				{
					e.StatesAllowed = DropLinePositionEnum.None;
					return;
				}
				e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode;
			}

			private void OnDrawFilterInvalidate(object sender, EventArgs e)
			{
				content.Grid.Invalidate();
			}
		}
	}
}
