using System;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common
{
    /// <summary>
    /// ����� �����
    /// </summary>
    public class GridHeaderLayout : GridHeaderCell
    {
        #region ����

        private UltraWebGrid grid;

        #endregion

        #region ��������

        /// <summary>
        /// ���� ����
        /// </summary>
        public UltraWebGrid Grid
        {
            get { return grid; }
        }

        /// <summary>
        /// ����� ������ ������
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
        /// ���������� ���������� RowLayoutColumnInfo ������� �����
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
        /// ���������� ���������� RowLayoutColumnInfo ������
        /// </summary>
        /// <param name="headerCell">������ ����</param>
        /// <param name="cellIndex">������ ������</param>
        /// <param name="level">���� ������</param>
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
        /// ���������� ������ �������
        /// </summary>
        /// <param name="caption">���������</param>
        /// <param name="hint">����</param>
        /// <param name="originX">������ �� X</param>
        /// <param name="originY">������ �� Y</param>
        /// <param name="spanX">������� �� X</param>
        /// <param name="spanY">������� �� Y</param>
        /// <returns>����� �������</returns>
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
        /// ��������� ���������� �������
        /// </summary>
        /// <param name="columnIndex">������ �������</param>
        /// <param name="caption">���������</param>
        /// <param name="hint">����</param> 
        /// <param name="originX">������ �� X</param>
        /// <param name="originY">������ �� Y</param>
        /// <param name="spanX">������� �� X</param>
        /// <param name="spanY">������� �� Y</param>
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
        /// ��������� ������� �� ������� ��� ����� �������
        /// </summary>
        /// <param name="columnIndex">������ �������</param>
        /// <returns>�������</returns>
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
        /// ��������� ������� �������� �������� � ������ ������ �������
        /// </summary>
        /// <param name="originX">������� �������</param>
        /// <returns>������� � ������ ������� �������</returns>
        private int GetFirstUnHiddenOriginX(int originX)
        {
            // ������� ������� �������
            int hiddenCount = 0;
            for (int i = 0; i <= originX; i++)
            {
                UltraGridColumn column = grid.Columns[i];
                if (column != null && column.Hidden)
                {
                    hiddenCount++;
                }
            }

            // �������� ������ � ������ ����� ������� �������
            int unHiddenOriginX = originX + hiddenCount;

            // ���������� ������ ���� ����� ��������� ��� ������� �������� - ���� ������ ���������
            while (unHiddenOriginX < grid.Columns.Count && grid.Columns[unHiddenOriginX].Hidden)
            {
                unHiddenOriginX++;
            }

            return unHiddenOriginX;
        }
		
		/// <summary>
		/// �������� ������ ������
		/// </summary>
		/// <param name="start">��������� ����� ���������, ��������� ���������� � 1 � ������ ������. 
		/// ���� �������� == 0, �� ��������� ���������� �� 2� ������, ���� -1, �� � 3� � �.�.</param>
		/// <returns></returns>
		public int AddNumericCells(int start)
		{
			return AddNumericCells(this, start);
		}

    	/// <summary>
		/// ���������� �������� ������ ������
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
