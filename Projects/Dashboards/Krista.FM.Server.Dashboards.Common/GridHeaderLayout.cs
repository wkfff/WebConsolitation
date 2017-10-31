using System;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common
{
    /// <summary>
    /// Хидер грида
    /// </summary>
    public class GridHeaderLayout : GridHeaderCell
    {
        #region Поля

        private UltraWebGrid grid;

        #endregion

        #region Свойства

        /// <summary>
        /// Грид слоя
        /// </summary>
        public UltraWebGrid Grid
        {
            get { return grid; }
        }

        /// <summary>
        /// Число этажей хидера
        /// </summary>
        public int HeaderHeight
        {
			get { return LevelCount; }
        }

        #endregion

        public GridHeaderLayout()
        {
            grid = new UltraWebGrid();
        }

        public GridHeaderLayout(UltraWebGrid ultraWebGrid)
        {
            grid = ultraWebGrid;
        }

        /// <summary>
        /// Применение параметров RowLayoutColumnInfo колонок грида
        /// </summary>
        public void ApplyHeaderInfo()
        {
            int cellIndex = 0;
            foreach (GridHeaderCell cell in childCells)
            {
                if (cell.ChildCount == 0)
                {
                    SetColumnInfo(cellIndex, cell.Caption, cell.Hint, cellIndex, 0, 1, HeaderHeight);
                    cellIndex++;
                }
                else
                {
                    cellIndex += ApplyCellsInfo(cell, cellIndex, 0);
                }
            }
        }

        /// <summary>
        /// Применение параметров RowLayoutColumnInfo ячейки
        /// </summary>
        /// <param name="headerCell">ячейка слоя</param>
        /// <param name="cellIndex">индекс ячейки</param>
        /// <param name="level">этаж хидера</param>
        private int ApplyCellsInfo(GridHeaderCell headerCell, int cellIndex, int level)
        {
            int childIndex = cellIndex;
            int childCount = 0;

			for (int i = 0; i < headerCell.ChildCount; i++)
            {
                GridHeaderCell cell = headerCell.childCells[i];
                if (cell.ChildCount == 0)
                {
					SetColumnInfo(childIndex, cell.Caption, cell.Hint, childIndex, level + headerCell.SpanY, 1, HeaderHeight - level - headerCell.SpanY);
                    childCount++;
                    childIndex++;
                }
                else
                {
					int count = ApplyCellsInfo(cell, childIndex, level + headerCell.SpanY);
                    childIndex += count;
                    childCount += count;
                }
            }

			AddColumnHeader(headerCell.Caption, headerCell.Hint, cellIndex, level, childCount, headerCell.SpanY);

            return childCount;
        }

        /// <summary>
        /// Добавление хидера колонки
        /// </summary>
        /// <param name="caption">заголовок</param>
        /// <param name="hint">хинт</param>
        /// <param name="originX">индекс по X</param>
        /// <param name="originY">индекс по Y</param>
        /// <param name="spanX">слияние по X</param>
        /// <param name="spanY">слияние по Y</param>
        /// <returns>хидер колонки</returns>
        private ColumnHeader AddColumnHeader(string caption, string hint, int originX, int originY, int spanX, int spanY)
        {
            ColumnHeader header = new ColumnHeader(true);
            header.Caption = caption;
            header.Title = hint;
            //header.RowLayoutColumnInfo.OriginX = originY == HeaderHeight - 1 ? originX : GetFirstUnHiddenOriginX(originX);
            header.RowLayoutColumnInfo.OriginX = originY != 0 ? originX : GetFirstUnHiddenOriginX(originX);
            header.RowLayoutColumnInfo.OriginY = originY;
            header.RowLayoutColumnInfo.SpanX = spanX;
            header.RowLayoutColumnInfo.SpanY = spanY;
            grid.Bands[0].HeaderLayout.Add(header);
            
            //CRHelper.SaveToErrorLog(String.Format("AddColumnHeader('{4}' {0}, {1}, {2}, {3})", header.RowLayoutColumnInfo.OriginX, header.RowLayoutColumnInfo.OriginY, header.RowLayoutColumnInfo.SpanX, header.RowLayoutColumnInfo.SpanY, header.Caption));
            
            return header;
        }

        /// <summary>
        /// Установка параметров колонки
        /// </summary>
        /// <param name="columnIndex">индекс колонки</param>
        /// <param name="caption">заголовок</param>
        /// <param name="hint">хинт</param> 
        /// <param name="originX">индекс по X</param>
        /// <param name="originY">индекс по Y</param>
        /// <param name="spanX">слияние по X</param>
        /// <param name="spanY">слияние по Y</param>
        private void SetColumnInfo(int columnIndex, string caption, string hint, int originX, int originY, int spanX, int spanY)
        {
            UltraGridColumn column = GetNonHiddenColumn(columnIndex);
            if (column != null)
            {
                column.Header.Caption = caption;
                column.Header.Title = hint;
                column.Header.RowLayoutColumnInfo.OriginX = originX;
                column.Header.RowLayoutColumnInfo.OriginY = originY;
                column.Header.RowLayoutColumnInfo.SpanX = spanX;
                column.Header.RowLayoutColumnInfo.SpanY = spanY;
				
                //CRHelper.SaveToErrorLog(String.Format("SetColumnInfo('{4}', {0}, {1}, {2}, {3})", column.Header.RowLayoutColumnInfo.OriginX, column.Header.RowLayoutColumnInfo.OriginY, column.Header.RowLayoutColumnInfo.SpanX, column.Header.RowLayoutColumnInfo.SpanY, column.Header.Caption));
            }
        }

        /// <summary>
        /// Получение колонки по индексу без учета скрытых
        /// </summary>
        /// <param name="columnIndex">индекс колонки</param>
        /// <returns>колонка</returns>
        public UltraGridColumn GetNonHiddenColumn(int columnIndex)
        {
            int index = 0;
            foreach (UltraGridColumn column in grid.Columns)
            {
                if (!column.Hidden)
                {
                    if (index == columnIndex)
                    {
                        return column;
                    }
                    index++;
                }
            }

            return null;
        }

        /// <summary>
        /// Получение первого видимого ориджина с учетом скртых колонок
        /// </summary>
        /// <param name="originX">текущий ориджин</param>
        /// <returns>ориджин с учетом скрытых колонок</returns>
        private int GetFirstUnHiddenOriginX(int originX)
        {
            // считаем скрытые колонки
            int hiddenCount = 0;
            for (int i = 0; i <= originX; i++)
            {
                UltraGridColumn column = grid.Columns[i];
                if (column != null && column.Hidden)
                {
                    hiddenCount++;
                }
            }

            // сдвигаем оригин с учетом числа скрытых колонок
            int unHiddenOriginX = originX + hiddenCount;

            // полученный оригин тоже может оказаться под скрытой колонкой - ищем первый нескрытый
            while (unHiddenOriginX < grid.Columns.Count && grid.Columns[unHiddenOriginX].Hidden)
            {
                unHiddenOriginX++;
            }

            return unHiddenOriginX;
        }
		
		/// <summary>
		/// Нумерует ячейки хидера
		/// </summary>
		/// <param name="start">Стартовый номер нумерации, нумерация начинается с 1 с первой ячейки. 
		/// Если параметр == 0, то нумерация начинается со 2й ячейки, если -1, то с 3й и т.д.</param>
		/// <returns></returns>
		public int AddNumericCells(int start)
		{
			return AddNumericCells(this, start);
		}

    	/// <summary>
		/// Рекурсивно нумерует ячейки хидера
		/// </summary>
		private static int AddNumericCells(GridHeaderCell cell, int start)
		{
			int i = 0;
			foreach (GridHeaderCell child in cell.childCells)
			{
				if (child.ChildCount == 0)
				{
					child.AddCell((start + i) <= 0 ? " " : String.Format("{0}", start + i));
					i++;
				}
				else
				{
					i += AddNumericCells(child, start + i);
				}
			}
			return i;
		}

    }
}
