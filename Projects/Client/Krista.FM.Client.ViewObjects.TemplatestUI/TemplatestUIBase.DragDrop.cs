using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal partial class TemplatestUIBase
	{
		internal interface IDragDropStrategy
		{
			void DragEnter(object sender, DragEventArgs e);
			void DragLeave(object sender, EventArgs e);
			void DragOver(object sender, DragEventArgs e);
			void DragDrop(object sender, DragEventArgs e);
		}

		private IDragDropStrategy currentDragDropStrategy;

		private IDragDropStrategy DragDropStrategyFactory(DragEventArgs e)
		{
			if (e.Data.GetDataPresent("FileDrop"))
				return new DragDropFilesStrategy(this);

			if (e.Data.GetDataPresent("Infragistics.Win.UltraWinGrid.SelectedRowsCollection"))
				return new DragDropRowsStrategy(this);
			
			return null;
		}

		private void OnGridDragEnter(object sender, DragEventArgs e)
		{
			currentDragDropStrategy = DragDropStrategyFactory(e);
			if (currentDragDropStrategy != null)
			{
				ViewObject.ugeTemplates.inDragDrop = true;
				currentDragDropStrategy.DragEnter(sender, e);
			}
		}

		private void OnGridDragLeave(object sender, EventArgs e)
		{
			if (currentDragDropStrategy != null)
				currentDragDropStrategy.DragLeave(sender, e);
			ViewObject.ugeTemplates.inDragDrop = false;
			currentDragDropStrategy = null;
		}

		private void OnGridDragOver(object sender, DragEventArgs e)
		{
			if (currentDragDropStrategy == null)
				return;

			SetScrollDirection(e);

			currentDragDropStrategy.DragOver(sender, e);
		}

		private void OnGridDragDrop(object sender, DragEventArgs e)
		{
			if (currentDragDropStrategy != null)
				currentDragDropStrategy.DragDrop(sender, e);

			ViewObject.ugeTemplates.inDragDrop = false;
		}

		private void OnGridSelectionDrag(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Grid.DoDragDrop(Grid.Selected.Rows, DragDropEffects.Move);
		}

		private void SetScrollDirection(DragEventArgs e)
		{
			// Получаем строку над которой в данный момент находимся
			UltraGridRow tmpRow = GetRowFromPos(e.X, e.Y);
			if (tmpRow == null)
				return;

			int rowsCount = Grid.ActiveRowScrollRegion.VisibleRows.Count;
			if (tmpRow == Grid.ActiveRowScrollRegion.VisibleRows[rowsCount - 3].Row ||
				tmpRow == Grid.ActiveRowScrollRegion.VisibleRows[rowsCount - 2].Row ||
				tmpRow == Grid.ActiveRowScrollRegion.VisibleRows[rowsCount - 1].Row)
				ViewObject.ugeTemplates.scrollDirection = UltraGridEx.ScrollDirections.down;
			else
				if (tmpRow == Grid.ActiveRowScrollRegion.VisibleRows[0].Row ||
					tmpRow == Grid.ActiveRowScrollRegion.VisibleRows[1].Row ||
					tmpRow == Grid.ActiveRowScrollRegion.VisibleRows[2].Row)
					ViewObject.ugeTemplates.scrollDirection = UltraGridEx.ScrollDirections.up;
				else
					ViewObject.ugeTemplates.scrollDirection = UltraGridEx.ScrollDirections.unknown;
		}

		/// <summary>
		/// Получить UltraGridRow по экранным координатам. Опрашиваются и потомки.
		/// </summary>
		/// <param name="X">Координата X (экранная)</param>
		/// <param name="Y">Координата Y (экранная)</param>
		/// <returns></returns>
		private UltraGridRow GetRowFromPos(int X, int Y)
		{
			Point pt = new Point(X, Y);
			pt = Grid.PointToClient(pt);
			UIElement elem = Grid.DisplayLayout.UIElement.ElementFromPoint(pt);
			return GetRowFromElement(elem);
		}

		/// <summary>
		/// Получить UltraGridRow от UIElement. Опрашиваются и потомки.
		/// </summary>
		/// <param name="elem">элемент</param>
		/// <returns>строка</returns>
		private static UltraGridRow GetRowFromElement(UIElement elem)
		{
			UltraGridRow row;
			try
			{
				row = (UltraGridRow)elem.GetContext(typeof(UltraGridRow), true);
			}
			catch
			{
				row = null;
			}
			return row;
		}
	}
}