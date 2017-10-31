using System;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.Common
{
    /// <summary>
    /// Ячейка хидера грида
    /// </summary>
    public class GridHeaderCell
    {
        #region Поля

        public Collection<GridHeaderCell> childCells;
        private string caption = String.Empty;
        private string hint = String.Empty;
        private int spanY = 1;

        #endregion

        #region Свойства
        
        /// <summary>
        /// Заголовок ячейки
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        /// <summary>
        /// Хинт ячейки
        /// </summary>
        public string Hint
        {
            get { return hint; }
            set { hint = value; }
        }

        /// <summary>
        /// Спан ячейки по вертикали
        /// </summary>
        public int SpanY
        {
            get { return spanY; }
            set { spanY = value; }
        }

		/// <summary>
		/// Число уровней потомков ячейки
		/// </summary>
		public int LevelCount
		{
			get { return GetLevelCount(this)-1; }
		}

        /// <summary>
        /// Число потомков (первого уровня)
        /// </summary>
        public int ChildCount
        {
            get { return childCells.Count; }
        }

        /// <summary>
        /// Число потомков (всех уровней)
        /// </summary>
        public int AllChildCount
        {
            get { return GetChildCount(this); }
        }

        #endregion

        public GridHeaderCell()
        {
            childCells = new Collection<GridHeaderCell>();
        }

        /// <summary>
        /// Добавление новой ячейки
        /// </summary>
        /// <param name="cellCaption">имя</param>
        /// <returns>ячейка</returns>
        public GridHeaderCell AddCell(string cellCaption)
        {
            return  AddCell(cellCaption, String.Empty, 1);
        }

        /// <summary>
        /// Добавление новой ячейки
        /// </summary>
        /// <param name="cellCaption">имя</param>
        /// <param name="cellSpanY">спан ячейки по вертикали</param>
        /// <returns>ячейка</returns>
        public GridHeaderCell AddCell(string cellCaption, int cellSpanY)
        {
            return AddCell(cellCaption, String.Empty, cellSpanY);
        }

        /// <summary>
        /// Добавление новой ячейки
        /// </summary>
        /// <param name="cellCaption">имя</param>
        /// <param name="cellHint">хинт</param>
        /// <returns>ячейка</returns>
        public GridHeaderCell AddCell(string cellCaption, string cellHint)
        {
            return AddCell(cellCaption, cellHint, 1);
        }

        /// <summary>
        /// Добавление новой ячейки
        /// </summary>
        /// <param name="cellCaption">имя</param>
        /// <param name="cellHint">хинт</param>
        /// <param name="cellSpanY">спан ячейки по вертикали</param>
        /// <returns>ячейка</returns>
        public GridHeaderCell AddCell(string cellCaption, string cellHint, int cellSpanY)
        {
            GridHeaderCell cell = new GridHeaderCell();
            cell.Caption = cellCaption;
            cell.Hint = cellHint;
            cell.SpanY = cellSpanY;
            childCells.Add(cell);
            return cell;
        }

        /// <summary>
        /// Получение ячейки по имени
        /// </summary>
        /// <param name="cellCaption">имя</param>
        /// <returns>ячейка</returns>
        public GridHeaderCell GetChildCellByCaption(string cellCaption)
        {
            foreach (GridHeaderCell cell in childCells)
            {
                if (cell.Caption.Equals(cellCaption))
                {
                    return cell;
                }
            }
            
            return this;
        }

        /// <summary>
        /// Высота дерева потомков ячейки
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <returns>уровень</returns>
        protected int GetLevelCount(GridHeaderCell cell)
        {
            int maxLevel = 0;
            foreach (GridHeaderCell c in cell.childCells)
            {
                int level = GetLevelCount(c);
                if (level > maxLevel)
                {
                    maxLevel = level;
                }
            }
            return maxLevel + 1;
        }
        
        /// <summary>
        /// Получение числа потомков ячейки
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <returns>число потомков</returns>
        protected int GetChildCount(GridHeaderCell cell)
        {
            if (cell.childCells.Count == 0)
            {
                return 1;
            }

            int childCount = 0;
            foreach (GridHeaderCell c in cell.childCells)
            {
				childCount += GetChildCount(c);
            }
            return childCount;
        }
    }
}
