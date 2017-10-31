using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;


namespace Krista.FM.Client.Components
{
    enum FindRowsType { SingleRow, MultiRows }

    public struct UltraGridColumnFilter
    {
        private int bandIndex;
        private ColumnFilter columnFilter;

        public UltraGridColumnFilter(int bandIndex, ColumnFilter columnFilter)
        {
            this.bandIndex = bandIndex;
            this.columnFilter = columnFilter;
        }

        public int BandIndex
        {
            get { return bandIndex; }
        }

        public ColumnFilter ColumnFilter
        {
            get { return columnFilter; }
        }
    }

    public class UltraGridColumnFilterCollection
    {
        
    }

    /// <summary>
    /// простейшее создание чекбокса в заголовке поля грида.
    /// </summary>
    public class CheckBoxOnHeader : IUIElementCreationFilter
    {
        // This event will fire when the CheckBox is clicked. 
        public delegate void HeaderCheckBoxClickedHandler(Object sender, Infragistics.Win.UIElementEventArgs e);

        public delegate void HeaderLinesChange(Object sender, int bandIndex, int colLines);

        private HeaderCheckBoxClickedHandler _headerCheckBoxClickedHandler;
        public event HeaderCheckBoxClickedHandler OnHeaderCheckBoxClick
        {
            add { _headerCheckBoxClickedHandler += value; }
            remove { _headerCheckBoxClickedHandler -= value; }
        }

        private HeaderLinesChange _HeaderLinesChange;
        public event HeaderLinesChange OnHeaderLinesChange
        {
            add { _HeaderLinesChange += value; }
            remove { _HeaderLinesChange -= value; }
        }

        private Type _columnDataType;
        private CheckState _checkState;
        private UltraGrid _grid;

        private int[] maxHeight;

		public CheckBoxOnHeader(Type columnDataType, CheckState checkState, UltraGrid grid)
		{
            _columnDataType = columnDataType;
            _checkState = checkState;
            _grid = grid;
            maxHeight = new int[grid.DisplayLayout.Bands.Count];

            grid.AfterColPosChanged += new AfterColPosChangedEventHandler(_grid_AfterColPosChanged);

            grid.MouseEnterElement += new UIElementEventHandler(grid_MouseEnterElement);
            grid.MouseLeaveElement += new UIElementEventHandler(grid_MouseLeaveElement);

            grid.DisplayLayout.Override.WrapHeaderText = DefaultableBoolean.Default;

            OnHeaderCheckBoxClick += new HeaderCheckBoxClickedHandler(CheckBoxOnHeader_OnHeaderCheckBoxClick);


		}


        void CheckBoxOnHeader_OnHeaderCheckBoxClick(object sender, UIElementEventArgs e)
        {
            CheckBoxUIElement checkBox = (CheckBoxUIElement)e.Element;

            Infragistics.Win.UltraWinGrid.ColumnHeader header = (Infragistics.Win.UltraWinGrid.ColumnHeader)checkBox.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));
            header.Tag = checkBox.CheckState;
            bool checkState = Convert.ToBoolean(checkBox.CheckState);

            UltraGridColumn column = header.Column;

            string columnName = column.Key;

            if (column.DataType == typeof(string))
            {
                if (checkBox.CheckState == CheckState.Checked)
                {
                    column.CellMultiLine = DefaultableBoolean.True;
                }
                else
                {
                    column.CellMultiLine = DefaultableBoolean.False;
                }

                int columnWidth = column.Width;
                column.PerformAutoResize(PerformAutoSizeType.None);
                column.Width = columnWidth;
            }
            else if (column.DataType == typeof(bool))
            {
                bool check = checkBox.CheckState == CheckState.Checked;
                foreach (UltraGridRow row in _grid.Rows)
                {
                    row.Cells[columnName].Value = check;
                    row.Update();
                }
            }
        }


        void _grid_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            if (e.PosChanged == PosChanged.Sized)
            {
                maxHeight[e.ColumnHeaders[0].Band.Index] = 0;
            }
        }

        /// <summary>
        /// событие клика на созданом элементе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aCheckBoxUIElement_ElementClick(Object sender, Infragistics.Win.UIElementEventArgs e)
        {
            // Get the CheckBoxUIElement that was clicked
            CheckBoxUIElement aCheckBoxUIElement = (CheckBoxUIElement)e.Element;

            // Get the Header associated with this particular element
            Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader = (Infragistics.Win.UltraWinGrid.ColumnHeader)aCheckBoxUIElement.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

            // Set the Tag on the Header to the new CheckState
            aColumnHeader.Tag = aCheckBoxUIElement.CheckState;
            bool checkState = Convert.ToBoolean(aCheckBoxUIElement.CheckState);
            // So that we can apply various changes only to the relevant Rows collection that the header belongs to
            HeaderUIElement aHeaderUIElement = aCheckBoxUIElement.GetAncestor(typeof(HeaderUIElement)) as HeaderUIElement;
            RowsCollection hRows = aHeaderUIElement.GetContext(typeof(RowsCollection)) as RowsCollection;

            // Raise an event so the programmer can do something when the CheckState changes
            if (_headerCheckBoxClickedHandler != null)
                _headerCheckBoxClickedHandler(sender, e);
        }

        public bool BeforeCreateChildElements(Infragistics.Win.UIElement parent)
        {
            // тут ничего не делаем
            return false;
        }

        public void AfterCreateChildElements(Infragistics.Win.UIElement parent) // Implements Infragistics.Win.IUIElementCreationFilter.AfterCreateChildElements
        {
            // все действия по созданию каких то эелементов для заоголовка выполняем тут
            if (parent is HeaderUIElement)
            {
                Infragistics.Win.UltraWinGrid.HeaderBase aHeader = ((HeaderUIElement)parent).Header;

                if (aHeader.Column == null)
                    return;
                string caption = aHeader.Column.Header.Caption;
                if (caption != string.Empty)
                {
                    Infragistics.Win.CheckBoxUIElement cbWrapWordsUIElement = null;
                    if (aHeader.Column.DataType == _columnDataType)
                    {
                        cbWrapWordsUIElement = (CheckBoxUIElement)parent.GetDescendant(typeof(ButtonUIElement));

                        if (cbWrapWordsUIElement == null)
                        {
                            // создаем чекбокс
                            cbWrapWordsUIElement = new CheckBoxUIElement(parent);
                        }

                        Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader =
                            (Infragistics.Win.UltraWinGrid.ColumnHeader)cbWrapWordsUIElement.GetAncestor(typeof(HeaderUIElement))
                            .GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                        if (aColumnHeader.Tag == null)
                            //If the tag was nothing, this is probably the first time this 
                            //Header is being displayed, so default to Indeterminate
                            aColumnHeader.Tag = _checkState;
                        else
                            cbWrapWordsUIElement.CheckState = (CheckState)aColumnHeader.Tag;

                        cbWrapWordsUIElement.ElementClick += new UIElementEventHandler(aCheckBoxUIElement_ElementClick);

                        parent.ChildElements.Add(cbWrapWordsUIElement);

                        // передвигаем все элементы в элементе заголовка на ширину чекбокса
                        cbWrapWordsUIElement.Rect = new Rectangle(parent.Rect.X + 3, parent.Rect.Y + ((parent.Rect.Height - cbWrapWordsUIElement.CheckSize.Height) / 2), cbWrapWordsUIElement.CheckSize.Width, cbWrapWordsUIElement.CheckSize.Height);
                    }
                    TextUIElement aTextUIElement = (TextUIElement)parent.GetDescendant(typeof(TextUIElement));

                    // если нету текстового заголовка, то выходим
                    if (aTextUIElement == null)
                        return;
                    // получаем элемент для отображения сортировки
                    SortIndicatorUIElement aSortIndicatorElement = (SortIndicatorUIElement)parent.GetDescendant(typeof(SortIndicatorUIElement));
                    // получаем элемент кнопки фильтра
                    FilterDropDownButtonUIElement aFilterUIElement = (FilterDropDownButtonUIElement)parent.GetDescendant(typeof(FilterDropDownButtonUIElement));

                    int textLengh = 0;
                    if (aSortIndicatorElement != null && cbWrapWordsUIElement != null)
                        textLengh = 36;
                    else if (aSortIndicatorElement == null && cbWrapWordsUIElement != null)
                        textLengh = 20;
                    else if (aSortIndicatorElement == null && cbWrapWordsUIElement == null)
                        textLengh = 6;
                    else if (aSortIndicatorElement != null && cbWrapWordsUIElement == null)
                        textLengh = 19;
                    if (cbWrapWordsUIElement != null && aFilterUIElement == null)
                        textLengh = textLengh - 13;

                    // Push the TextUIElement to the right a little to make 
                    // room for the CheckBox. 3 pixels of padding are used again. 
                    if (cbWrapWordsUIElement != null)
                        aTextUIElement.Rect = new Rectangle(cbWrapWordsUIElement.Rect.Right + 3, aTextUIElement.Rect.Y, parent.Rect.Width - (cbWrapWordsUIElement.Rect.Right - parent.Rect.X) - textLengh, aTextUIElement.Rect.Height);
                    else
                        aTextUIElement.Rect = new Rectangle(parent.Rect.X + 3, aTextUIElement.Rect.Y, parent.Rect.Width - textLengh, aTextUIElement.Rect.Height);

                    #region херня по нормализации надписей в заголовках колонок грида

                    int fullHeight = GetStringHeight(caption, _grid.Font, aTextUIElement.Rect.Width) + 10;
                    float fontHeight = _grid.Font.GetHeight();
                    int linesCount = (int)(fullHeight / fontHeight);

                    UltraGridBand band = aHeader.Column.Band;
                    int bandIndex = band.Index;
                    band = _grid.DisplayLayout.Bands[bandIndex];

                    if ((maxHeight[bandIndex] < linesCount))
                    {
                        maxHeight[bandIndex] = linesCount;

                        if (maxHeight[bandIndex] > 10)
                            maxHeight[bandIndex] = 10;
                        if (band.ColHeaderLines != maxHeight[bandIndex])
                        {
                            band.ColHeaderLines = maxHeight[bandIndex];
                            band.NotifyPropChange(Infragistics.Win.UltraWinGrid.PropertyIds.ColHeaderLines);
                        }
                    }
                    int minWidth = GetMinimalColWidth(caption, aHeader.Column.Width, aTextUIElement.Rect.Width, _grid.Font);
                    if (aHeader.Column.MinWidth != minWidth)
                        aHeader.Column.MinWidth = minWidth;

                    aTextUIElement.WrapText = true;

                    #endregion

                    if (aSortIndicatorElement != null)
                        aSortIndicatorElement.Rect = new Rectangle(aTextUIElement.Rect.Right + 3, aSortIndicatorElement.Rect.Y, 13, aSortIndicatorElement.Rect.Height);

                    if (aFilterUIElement != null)
                        if (aSortIndicatorElement != null)
                            aFilterUIElement.Rect = new Rectangle(aSortIndicatorElement.Rect.Right + 3, aFilterUIElement.Rect.Y, 13, aFilterUIElement.Rect.Height);
                        else
                            aFilterUIElement.Rect = new Rectangle(aTextUIElement.Rect.Right + 3, aFilterUIElement.Rect.Y, 13, aFilterUIElement.Rect.Height);
                }
            }
        }

        /// <summary>
        /// получение минимальной ширины колонки исходя из расчета 10 строк заголовка
        /// </summary>
        /// <returns></returns>
        private int GetMinimalColWidth(string columnCaption, int columnWidth, int captionWidth, Font font)
        {
            int lineSymbolsCount = columnCaption.Length / 10;
            if ((columnCaption.Length % 10) != 0)
                lineSymbolsCount++;
            string tmpStr = string.Empty.PadLeft(lineSymbolsCount, 'm');
            int lineWidth = this.GetStringWidth(tmpStr, font);
            return columnWidth - captionWidth + lineWidth;
        }

        public int GetStringHeight(string measuredString, Font font, int rectangleWidth)
        {
            Graphics g = Graphics.FromHwnd(_grid.Handle);
            SizeF sizeF = g.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);

            return rect.Height;
        }


        public int GetStringWidth(string measuredString, Font font)
        {
            Graphics g = Graphics.FromHwnd(_grid.Handle);
            SizeF sizeF = g.MeasureString(measuredString, font);
            Size rect = Size.Round(sizeF);
            return rect.Width;
        }

        private Infragistics.Win.ToolTip toolTipValue = null;

        private Infragistics.Win.ToolTip pToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new Infragistics.Win.ToolTip(_grid);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        void grid_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is CheckBoxUIElement)
                pToolTip.Hide();
        }

        void grid_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is CheckBoxUIElement)
            {
                CheckBoxUIElement checkBox = (CheckBoxUIElement)e.Element;
                if (checkBox != null)
                    if (checkBox.Parent is HeaderUIElement)
                    {

                        Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader = (Infragistics.Win.UltraWinGrid.ColumnHeader)checkBox.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                        if (aColumnHeader.Column.DataType == typeof(bool))
                        {
                            if (checkBox.CheckState == CheckState.Unchecked)
                                pToolTip.ToolTipText = "Применить для всех";
                            else
                                pToolTip.ToolTipText = "Отменить для всех";
                        }
                        else if (aColumnHeader.Column.DataType == typeof(string))
                        {
                            pToolTip.ToolTipText = "Перенос по словам";
                        }

                        Point tooltipPos = new Point(e.Element.ClipRect.Left, e.Element.ClipRect.Bottom);
                        tooltipPos.Y = tooltipPos.Y + checkBox.Rect.Height + 2;
                        pToolTip.Show(this._grid.PointToScreen(tooltipPos));
                    }
            }
        }

    }

    public struct UltraGridHelper
    {
        #region

        /// <summary>
        /// поиск записи в гриде по значениям нескольких полей
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="columnTypes"></param>
        /// <returns></returns>
        public static UltraGridRow FindRow(UltraGrid grid, string[] fields, object[] values, Type[] columnTypes)
        {
            for (int i = 0; i <= fields.Length -1; i++)
            {
                values[i] = Convert.ChangeType(values[i], columnTypes[i]);
            }

            foreach (UltraGridRow gridRow in grid.Rows)
            {
                bool findRow = true;
                for (int i = 0; i <= fields.Length -1; i++)
                {
                    if (gridRow.Cells[fields[i]].Value == DBNull.Value)
                    {
                        findRow = false;
                        break;
                    }
                        
                    object cellValue = Convert.ChangeType(gridRow.Cells[fields[i]].Value, columnTypes[i]);
                    IComparable cmp = values[i] as IComparable;
                    findRow = cmp.CompareTo(cellValue) == 0;
                    if (!findRow)
                        break;
                }
                if (findRow)
                {
                    return gridRow;
                }
            }
            return null;
        }

        #endregion


        #region различные методы работы с гридом

        public static void SaveUltragridFilters(UltraGrid grid)
        {
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
            {
                foreach (ColumnFilter columnFilter in band.ColumnFilters)
                {
                    ColumnFilter newFilter = new ColumnFilter(columnFilter.Column, columnFilter.LogicalOperator);
                }
            }
        }


        /// <summary>
        /// проверяем, находится ли грид в режиме группировки
        /// </summary>
        /// <returns></returns>
        public static bool GridInGroupByMode(UltraGrid grid)
        {
            bool inGroupByMode = false;
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
            {
                foreach (UltraGridColumn clmn in band.SortedColumns)
                {
                    if (clmn.IsGroupByColumn)
                    {
                        inGroupByMode = true;
                        break;
                    }
                }
                if (inGroupByMode)
                    break;
            }
            return inGroupByMode;
        }

		public static void SetActiveRowToFirst(UltraGrid ug)
		{
			UltraGridRow row = ug.GetRow(ChildRow.First);
			if (row == null) 
				return;
			while ((row.IsFilteredOut) || (row == null))
				row = row.GetSibling(SiblingRow.Next);
			if (row != null)
				ug.ActiveRow = row;
		}

		public static UltraGridRow GetActiveRowCells(UltraGrid ug)
		{
			if (ug.ActiveRow == null) return null;
			UltraGridRow row = ug.ActiveRow;
            return GetRowCells(row);
		}

        public static UltraGridRow GetRowCells(UltraGridRow row)
        {
            if (row == null) return null;
            // идем по потомкам до тех пор, пока не получим ячейки
            while (row.Cells == null)
                row = row.ChildBands[0].Rows[0];
            if (!row.IsDataRow)
                throw new InvalidOperationException("Ошибка определения конечного элемента");
            return row;
        }

        public static int GetRowID(UltraGridRow row)
        {
            UltraGridRow actualRow = GetRowCells(row);
            int activeID = -1;
            if (actualRow != null)
            {
                
                if (Int32.TryParse(actualRow.Cells["ID"].Value.ToString(), out activeID))
                    return activeID;
            }
            return activeID;
        }

        public static int GetFirstParentRowID(UltraGridRow row)
        {
            UltraGridRow actualRow = GetRowCells(row);
            while (actualRow.ParentRow != null && actualRow.ParentRow.IsDataRow)
            {
                actualRow = actualRow.ParentRow;
            }
            return GetRowID(actualRow);
        }

        /// <summary>
        /// получаем ID всех записей предков записи, включая эту запись
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static List<int> GetParentsIds(UltraGridRow row)
        {
            List<int> parentRowsIds = new List<int>();
            UltraGridRow actualRow = GetRowCells(row);
            parentRowsIds.Add(GetRowID(actualRow));
            while (actualRow.ParentRow != null && actualRow.ParentRow.IsDataRow)
            {
                actualRow = actualRow.ParentRow;
                parentRowsIds.Add(GetRowID(actualRow));
            }
            return parentRowsIds;
        }

        public static List<int> GetChildsIDs(UltraGridRow parentRow)
        {
            List<int> childRowsIDs = new List<int>();

            foreach (UltraGridChildBand band in parentRow.ChildBands)
            {
                GetChildRowsID(childRowsIDs, band.Rows);
            }
            return childRowsIDs;
        }

        #region списки id записей в гриде по уровням

        /// <summary>
        /// получение списков id записей в гриде согласно их уровням иерархии
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static int[] GetSelectedAndChildsIDs(UltraGrid grid)
        {
            Dictionary<int, List<int>> idDict = new Dictionary<int, List<int>>();
            if (grid.Selected.Rows.Count == 0 && grid.ActiveRow != null)
                grid.ActiveRow.Selected = true;
            foreach (UltraGridRow row in grid.Selected.Rows)
            {
                if (idDict.ContainsKey(row.Band.Index))
                    idDict[row.Band.Index].Add(GetRowID(row));
                else
                {
                    List<int> list = new List<int>();
                    list.Add(GetRowID(row));
                    idDict.Add(row.Band.Index, list);
                }
                GetChildsIDs(row, ref idDict);
            }
            // преобразуем результат в массив
            List<int> idList = new List<int>();
            foreach (int key in idDict.Keys)
            {
                idList.AddRange(idDict[key]);
            }
            return idList.ToArray();
        }

        private static void GetChildsIDs(UltraGridRow parentRow, ref Dictionary<int, List<int>> idDict)
        {
            foreach (UltraGridChildBand band in parentRow.ChildBands)
            {
                GetChildRowsID(band.Rows, ref idDict);
            }
        }

        private static void GetChildRowsID(RowsCollection rowsCollection, ref Dictionary<int, List<int>> idDict)
        {
            foreach (UltraGridRow row in rowsCollection)
            {
                // только для не удаленных записей просматриваем подчиненные
                if (idDict.ContainsKey(row.Band.Index))
                    idDict[row.Band.Index].Add(GetRowID(row));
                else
                {
                    List<int> list = new List<int>();
                    list.Add(GetRowID(row));
                    idDict.Add(row.Band.Index, list);
                }

                if (row.ChildBands != null)
                    foreach (UltraGridChildBand childBand in row.ChildBands)
                    {
                        if (childBand.Rows != null)
                            GetChildRowsID(childBand.Rows, ref idDict);
                    }
            }
        }

        #endregion

        /// <summary>
		/// Возвращает выбранное пользователем Значение классификатора (ID)
		/// </summary>
		/// <returns>ID</returns>
		public static int GetActiveID(UltraGrid ug)
		{
			if (ug.ActiveRow == null) return -10;
			UltraGridRow row = ug.ActiveRow;
            return GetRowID(GetRowCells(row));
		}

        public static string GetActiveName(UltraGrid ug)
        {
            string cellName = "Name";
            if (ug.ActiveRow == null) return string.Empty;
            UltraGridRow row = ug.ActiveRow;
            if (row.Cells.Exists(cellName))
                return row.Cells[cellName].Value.ToString();
            return string.Empty;
        }


        public static UltraGridRow GetRow(UltraGrid grid, SiblingRow direction)
        {
            UltraGridRow returnRow = null;
            UltraGridRow gridRow = null;
            switch (direction)
            {
                case SiblingRow.First:
                    returnRow = grid.Rows[0];
                    break;
                case SiblingRow.Last:
                    returnRow = grid.Rows[grid.Rows.Count - 1];
                    break;
                case SiblingRow.Next:
                    returnRow = grid.ActiveRow.GetChild(ChildRow.First);
                    if (returnRow == null)
                        returnRow = grid.ActiveRow.GetSibling(SiblingRow.Next);
                    break;
                case SiblingRow.Previous:
                    gridRow = grid.ActiveRow.GetSibling(SiblingRow.Previous);
                    if (gridRow != null)
                        while (gridRow.HasPrevSibling())
                        {                      
                            if (!gridRow.IsFilteredOut)
                            {
                                returnRow = gridRow;
                                break;
                            }
                            gridRow = gridRow.GetSibling(SiblingRow.Previous);
                        }
                    if (returnRow == null)
                        returnRow = grid.ActiveRow.ParentRow;
                    break;
            }
            return returnRow;
        }

        /// <summary>
        /// визуальное перемещение по гриду
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="direction"></param>
        public static void MoveTo(UltraGrid grid, SiblingRow direction)
        {
            switch (direction)
            {
                case SiblingRow.First:
                    grid.PerformAction(UltraGridAction.FirstRowInBand);
                    break;
                case SiblingRow.Previous:
                    grid.PerformAction(UltraGridAction.PrevRow);
                    break;
                case SiblingRow.Next:
                    grid.PerformAction(UltraGridAction.NextRow);
                    break;
                case SiblingRow.Last:
                    grid.PerformAction(UltraGridAction.LastRowInGrid);
                    break;
            }
        }

        /// <summary>
        /// Формирует списко ID выбранных строк
        /// </summary>
        /// <param name="ug">Грид</param>
        /// <param name="ids">Список ID</param>
        public static void GetSelectedIDs(UltraGrid ug, out List<int> ids)
        {
            ids = new List<int>();
            foreach (UltraGridRow row in ug.Selected.Rows)
            {
                int curID = UltraGridHelper.GetRowID(row);
                ids.Add(curID);
            }
            if (ids.Count == 0 && ug.ActiveRow != null)
            {
                ids.Add(UltraGridHelper.GetActiveID(ug));
            }
        }

        /// <summary>
        /// возвращает ID выделенных записей в гриде
        /// </summary>
        /// <param name="ug">грид</param>
        /// <param name="ids">список ID</param>
        /// <param name="withChildRows">флаг, указывает на то, добавлять в список ID подчиненных записей</param>
        public static void GetSelectedIds(UltraGridEx ugEx, HierarchyInfo hi, out List<int> ids, bool withChildRows)
        {
            UltraGrid ug = ugEx.ugData;
            if (ug.Selected.Rows.Count == 0 && ug.ActiveRow != null)
                ug.ActiveRow.Selected = true;
            bool isParentExpand = hi.loadMode == LoadMode.OnParentExpand;
            if (withChildRows)
            {
                ids = new List<int>();
                if (isParentExpand)
                    ug.BeginUpdate();
                foreach (UltraGridRow row in ug.Selected.Rows)
                {
                    if (isParentExpand)
                        row.ExpandAll();

                    int curID = UltraGridHelper.GetRowID(row);
                    ids.Add(curID);
                    if (row.ChildBands != null)
                        foreach (UltraGridChildBand childBand in row.ChildBands)
                        {
                            if (childBand.Rows != null)
                                GetChildRowsID(ids, childBand.Rows);
                        }
                }
                if (isParentExpand)
                    ug.EndUpdate();
            }
            else
                GetSelectedIDs(ug, out ids);
        }

        private static void GetChildRowsID(List<int> ids, RowsCollection rowsCollection)
        {
            foreach (UltraGridRow row in rowsCollection)
            {
                // только для не удаленных записей просматриваем подчиненные
                int idValue = Convert.ToInt32(row.Cells["ID"].Value);
                ids.Add(idValue);

                if (row.ChildBands != null)
                    foreach (UltraGridChildBand childBand in row.ChildBands)
                    {
                        if (childBand.Rows != null)
                            GetChildRowsID(ids, childBand.Rows);
                    }
            }
        }

		public static object GetActiveRowColumnValue(UltraGrid ug, string columnName)
		{
			UltraGridRow row = GetActiveRowCells(ug);
			if (row != null)
			{
				return row.Cells[columnName].Value;
			}
			else
			{
				return null;
			}
        }


        public static List<int> GetVisibleRowsIds(UltraGrid grid)
        {
            List<int> visibleIds = new List<int>();
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.IsDataRow && row.VisibleIndex >= 0)
                    visibleIds.Add(Convert.ToInt32(row.Cells["ID"].Value));
            }
            return visibleIds;
        }

        #endregion

        #region поиск записей в гриде

        // делегат на поиск записей
        public delegate bool CheckRowDelegate(UltraGridRow row);
        // название колонки, по которой ищем значение
        private static string findColumnName;
        // значение, которое ищем
        private static string findValue;
        // возвращаемые записи, которые соответствуют критерию поиска
        private static List<UltraGridRow> rows;

        private enum FindRowsType { SingleRow, MultiRows }
        private static FindRowsType findRowsType;

        /// <summary>
        /// проверяет, подходит ли запись параметрам поиска
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static bool CheckRow(UltraGridRow row)
        {  
            if (row.Cells[findColumnName].Value.ToString() == findValue)
            {
                // если запись подходит
                if (findRowsType == FindRowsType.SingleRow)
                {
                    // то, если ищем одну запись, выходим
                    rows.Add(row);
                    return true;
                }
                else
                {
                    // продолжаем искать такие же записи
                    rows.Add(row);
                    return false;
                }
            }

            return false;
        }

        public static void EnumGridRows(UltraGrid grid, UltraGridRow row, CheckRowDelegate checkDelegate)
        {
            EnumGridRows(grid, row, false, checkDelegate);
        }


        public static void EnumGridRows(UltraGrid grid, UltraGridRow row, bool checkChildRowsOnly, CheckRowDelegate checkDelegate)
        {
            int timeCount = Environment.TickCount;
            grid.BeginUpdate();
            try
            {
                UltraGridRow gridRow = null;
                if (row != null)
                    gridRow = row;
                else
                    gridRow = grid.GetRow(Infragistics.Win.UltraWinGrid.ChildRow.First);

                UltraGridRow siblingRow = gridRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);
                                
                UltraGridRow parentRow = gridRow.ParentRow;
                // если нету записей соседних, то тут или всего одна или поднимаемся выше и ищем записи там
                if (parentRow != null && siblingRow == null)
                {
                    UltraGridRow tmpParentRow = parentRow;
                    while (siblingRow == null && tmpParentRow != null)
                    {
                        siblingRow = tmpParentRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);
                        tmpParentRow = tmpParentRow.ParentRow;
                    }
                }

                while (gridRow != null)
                {
                    // если проверка только для подчиненных записей 
                    //выходим при переходе на родительскую или соседнюю записи
                    if (checkChildRowsOnly)
                    {
                        if (object.Equals(gridRow, siblingRow) || object.Equals(gridRow, parentRow))
                            break;
                    }
                    if (gridRow.Cells != null)
                        if ((!gridRow.IsFilteredOut) && checkDelegate(gridRow))
                        {
                            break;
                        }
                    
                    if (gridRow.HasChild())
                        gridRow = gridRow.GetChild(Infragistics.Win.UltraWinGrid.ChildRow.First);
                    else
                        if (gridRow.HasNextSibling())
                            gridRow = gridRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);
                        else
                            if (gridRow.ParentRow != null)
                            {
                                while (!gridRow.HasNextSibling())
                                {
                                    if (gridRow.ParentRow != null)
                                        gridRow = gridRow.ParentRow;
                                    else
                                    {
                                        gridRow = null;
                                        break;
                                    }
                                }
                                if (!(gridRow == null))
                                    gridRow = gridRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);
                            }
                            else
                                gridRow = null;
                }
            }
            finally
            {
                grid.EndUpdate();
                timeCount = Environment.TickCount - timeCount;
            }
        }


        /// <summary>
        ///  Ищет строку в плоском гриде по указанному полю и значению этого поля
        /// </summary>
        /// <param name="grid">грид</param>
        /// <param name="ColumnName">наименование колонки</param>
        /// <param name="Value">значение</param>
        /// <returns>найденная строка</returns>
        public static List<UltraGridRow> FindRowsFromFlatUltraGrid(UltraGrid grid, string columnName, string value)
        {
            FindGridRows(grid, columnName, value, FindRowsType.SingleRow);
            return rows;
        }


        /// <summary>
        /// поиск записи в гриде любого уровня
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UltraGridRow FindRow(UltraGrid grid, string columnName, string value)
        {
            List<UltraGridRow> listRows = UltraGridFindRowsHelper.FindRows(grid, null, columnName, value);
            if (listRows.Count > 0)
            {
                if (listRows[0].IsFilteredOut)
                {
                    foreach (UltraGridBand band in grid.DisplayLayout.Bands)
                    {
                        band.ColumnFilters.ClearAllFilters();
                    }
                }
                return listRows[0];
            }
            return null;
        }

        /// <summary>
        /// общий метод для поиска в гриде без указания дополнительных критериев поиска
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <param name="findType"></param>
        /// <returns></returns>
        private static bool FindGridRows(UltraGrid grid, string columnName, string value, FindRowsType findType)
        {
            // убираем фильтр во время поиска только для плоского грида
            if (grid.DisplayLayout.Bands.Count == 1)
            {
                foreach (UltraGridBand band in grid.DisplayLayout.Bands)
                {
                    band.ColumnFilters.ClearAllFilters();
                }
            }
            findColumnName = columnName;
            findValue = value;
            rows = new List<UltraGridRow>();
            UltraGridRow gridRow = grid.GetRow(ChildRow.First);
            findRowsType = findType;
            EnumGridRows(grid, null, false, CheckRow);
            // обнуляем некоторые параметры
            findColumnName = string.Empty;
            findValue = string.Empty;

			return rows.Count > 0;
        }

		/// <summary>
		/// Поиск в плоском гриде.
		/// </summary>
		/// <param name="grid">Грид в котором искать.</param>
		/// <param name="findingField">Поле в котором искать.</param>
		/// <param name="findingValue">Что искать.</param>
		/// <returns></returns>
		public static UltraGridRow FindGridRow(UltraGrid grid, string findingField, object findingValue)
		{
			return FindGridRow(grid, findingField, findingValue, true);
		}

		/// <summary>
		/// Поиск в плоском гриде
		/// </summary>
		/// <param name="grid">Грид в котором искать.</param>
		/// <param name="findingField">Поле в котором искать.</param>
		/// <param name="findingValue">Что искать.</param>
		/// <param name="clearFilters">Снимать ли фильтры с грида, 
		/// если найденная запись отфильтрована по фильтру.</param>
		/// <returns></returns>
		public static UltraGridRow FindGridRow(UltraGrid grid, string findingField, object findingValue, bool clearFilters)
        {
			// параметры поиска
			SearchInfo sInf = new SearchInfo();
			sInf.inHierarchy = false;
			sInf.lookIn = new List<string>();
			sInf.lookIn.Add(findingField);
			sInf.matchCase = false;
			sInf.searchContent = SearchContentEnum.AnyPartOfField;
			sInf.searchDirection = SearchDirectionEnum.All;
			sInf.searchString = findingValue.ToString();
			UltraGridRowSeacher searcher = new UltraGridRowSeacher(grid, sInf);
			UltraGridRow searchRow = searcher.SearchRow();
			// если найденная запись отфильтрована по фильтру, снимаем фильтры с грида
			if (clearFilters && searchRow != null && searchRow.IsFilteredOut)
			{
				foreach (UltraGridBand band in grid.DisplayLayout.Bands)
				{
					band.ColumnFilters.ClearAllFilters();
				}
			}

			return searchRow;
		}

        public static List<UltraGridRow> FindFlatGridRows(UltraGrid grid, string columnName, Type columnType, object findValue)
        {
            findValue = Convert.ChangeType(findValue, columnType);
            IComparable cmp = findValue as IComparable;

            List<UltraGridRow> rows = new List<UltraGridRow>();

            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells[columnName].Value != DBNull.Value)
                {
                    object cellValue = Convert.ChangeType(row.Cells[columnName].Value, columnType);
                    if (cmp.CompareTo(cellValue) == 0)
                        rows.Add(row);
                }
            }
            return rows;
        }

        /// <summary>
        /// поиск подчиненной записи по заданному полю и значению 
        /// </summary>
        public static UltraGridRow FindChildRow(UltraGridRow parentRow, string findingField, object findingValue)
        {
            // получаем запись с колонками в случае группировки записей
            parentRow = GetRowCells(parentRow);
            // изменяем тип значения, который ищем на тип поля, в котором ищем значение
            Type columnType = parentRow.Band.Columns[findingField].DataType;
            findingValue = Convert.ChangeType(findingValue, columnType);
            IComparable cmp = findingValue as IComparable;
            if (parentRow.ChildBands != null)
                foreach (UltraGridChildBand childBand in parentRow.ChildBands)
                {
                    foreach (UltraGridRow row in childBand.Rows)
                    {
                        // учитываем возможность группировки 
                        UltraGridRow tmpRow = UltraGridHelper.GetRowCells(row);
                        if (cmp.CompareTo(tmpRow.Cells[findingField].Value) == 0)
                            // если запись отфильтрована, то возвращаем null
                            if (tmpRow.IsFilteredOut)
                                return null;
                            else
                                return tmpRow;
                    }
                }
            // если ничего не нашли, возвращаем null
            return null;
        }

        /// <summary>
        /// поиск подчиненной записи по заданному полю и значению 
        /// </summary>
        public static UltraGridRow FindChildRow(UltraGrid grid, UltraGridRow parentRow, string findingField, object findingValue)
        {
            if (parentRow == null)
            {
                Type columnType = grid.DisplayLayout.Bands[0].Columns[findingField].DataType;
                findingValue = Convert.ChangeType(findingValue, columnType);
                IComparable cmp = findingValue as IComparable;
                foreach (UltraGridRow row in grid.Rows)
                {
                    if (row.VisibleIndex != -1)
                    {
                        if (cmp.CompareTo(GetRowCells(row).Cells[findingField].Value) == 0)
                            // если запись отфильтрована, то возвращаем null
                            if (row.IsFilteredOut)
                                return null;
                            else
                                return row;
                    }
                }
                return null;
            }
            else
            {
                // получаем запись с колонками в случае группировки записей
                parentRow = GetRowCells(parentRow);
                // изменяем тип значения, который ищем на тип поля, в котором ищем значение
                Type columnType = parentRow.Band.Columns[findingField].DataType;
                findingValue = Convert.ChangeType(findingValue, columnType);
                IComparable cmp = findingValue as IComparable;
                if (parentRow.ChildBands != null)
                    foreach (UltraGridChildBand childBand in parentRow.ChildBands)
                    {
                        foreach (UltraGridRow row in childBand.Rows)
                        {
                            // учитываем возможность группировки 
                            UltraGridRow tmpRow = GetRowCells(row);
                            if (cmp.CompareTo(tmpRow.Cells[findingField].Value) == 0)
                                // если запись отфильтрована, то возвращаем null
                                if (tmpRow.IsFilteredOut)
                                    return null;
                                else
                                    return tmpRow;
                        }
                    }
            }
            // если ничего не нашли, возвращаем null
            return null;
        }

        #endregion

        public static UltraGridColumn CreateDummyColumn(UltraGridColumn sourceColumn, string newColumnName)
        {
            UltraGridColumn dummyColumn = null;
            // смотрим создана ли фиктивная колонка в гриде
            if (!sourceColumn.Band.Columns.Exists(newColumnName))
                // если нет - добавляем
                dummyColumn = sourceColumn.Band.Columns.Add(newColumnName);
            else
                // если да - запоминаем существующую
                dummyColumn = sourceColumn.Band.Columns[newColumnName];

            
            // копируем нужные параметры родительской колонки
            dummyColumn.CellAppearance.BackColor = sourceColumn.CellAppearance.BackColor;
            dummyColumn.CellActivation = sourceColumn.CellActivation;
            dummyColumn.Style = sourceColumn.Style;
            dummyColumn.Hidden = sourceColumn.Hidden;
            dummyColumn.Header.VisiblePosition = sourceColumn.Header.VisiblePosition;
            dummyColumn.Header.Caption = sourceColumn.Header.Caption;
            dummyColumn.CellButtonAppearance = sourceColumn.CellButtonAppearance;
            //
            dummyColumn.Width = sourceColumn.Width;
            dummyColumn.AllowGroupBy = sourceColumn.AllowGroupBy;
            dummyColumn.AllowRowFiltering = sourceColumn.AllowRowFiltering;
           

            // прячем исходную колонку
            sourceColumn.Hidden = true;
            sourceColumn.Header.Caption = "";

            return dummyColumn;
        }

        public static void SetRowTransparentColor(UltraGridRow row, bool transparent)
        {
            if (row == null) return;

            if (transparent)
            {
                row.Appearance.AlphaLevel = 100;
                row.Appearance.BackColorAlpha = Alpha.UseAlphaLevel;
                row.Appearance.BorderAlpha = Alpha.UseAlphaLevel;
                row.Appearance.ForegroundAlpha = Alpha.UseAlphaLevel;
                row.RowSelectorAppearance.AlphaLevel = 100;
                row.RowSelectorAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
                row.RowSelectorAppearance.BorderAlpha = Alpha.UseAlphaLevel;
                row.RowSelectorAppearance.ForegroundAlpha = Alpha.UseAlphaLevel;
            }
            else
            {
                row.Appearance.AlphaLevel = 0;
                row.Appearance.BackColorAlpha = Alpha.Default;
                row.Appearance.BorderAlpha = Alpha.Default;
                row.Appearance.ForegroundAlpha = Alpha.Default;
                row.RowSelectorAppearance.AlphaLevel = 0;
                row.RowSelectorAppearance.BackColorAlpha = Alpha.Default;
                row.RowSelectorAppearance.BorderAlpha = Alpha.Default;
                row.RowSelectorAppearance.ForegroundAlpha = Alpha.Default;
            }
            //row.Refresh(RefreshRow.RefreshDisplay, false);
        }

        public static void SetRowsTransparent(UltraGrid ug, List<int> rowIDs, bool transparent)
        {
            foreach (int id in rowIDs)
            {
                UltraGridRow row = FindRow(ug, "ID", id.ToString());//UltraGridHelper.FindGridRow(ug, "ID", id);
                SetRowTransparent(row, transparent);
            }
        }

        private static void SetRowTransparent(UltraGridRow row, bool transparent)
        {
            SetRowTransparentColor(row, transparent);
            if (row.ChildBands != null)
                foreach (UltraGridChildBand childBand in row.ChildBands)
                {
                    foreach (UltraGridRow childRow in childBand.Rows)
                    {
                        SetRowTransparent(childRow, transparent);
                    }
                }
        }

        public static void SetLikelyButtonColumnsStyle(UltraGridColumn clmn, int imageIndex)
        {
            clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            clmn.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            clmn.AutoEdit = false;
            clmn.AutoSizeEdit = Infragistics.Win.DefaultableBoolean.False;
            clmn.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            clmn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            // cellappearance
            clmn.CellAppearance.ForeColor = System.Drawing.Color.Transparent;
            clmn.CellAppearance.ForeColorDisabled = System.Drawing.Color.Transparent;
            clmn.CellAppearance.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            clmn.CellAppearance.Image = imageIndex;
            clmn.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            clmn.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            // cellbuttonappearance
            clmn.CellButtonAppearance.BackColor = System.Drawing.SystemColors.ButtonFace;
            clmn.CellButtonAppearance.ForeColor = System.Drawing.Color.Transparent;
            clmn.CellButtonAppearance.ForeColorDisabled = System.Drawing.Color.Transparent;
            clmn.CellButtonAppearance.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            clmn.CellButtonAppearance.Image = imageIndex;
            clmn.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            clmn.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            //
            clmn.CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.FullEditorDisplay;
            clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            clmn.Header.Caption = String.Empty;
            clmn.LockedWidth = true;
            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            clmn.Width = 21;
        }

        public static void SetLikelyImageColumnsStyle(UltraGridColumn clmn, int imageIndex)
        {
            clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            clmn.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            clmn.AutoEdit = false;
            clmn.AutoSizeEdit = Infragistics.Win.DefaultableBoolean.False;
            clmn.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            clmn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            // cellappearance
            clmn.CellAppearance.ForeColor = System.Drawing.Color.Transparent;
            clmn.CellAppearance.ForeColorDisabled = System.Drawing.Color.Transparent;
            clmn.CellAppearance.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            clmn.CellAppearance.Image = imageIndex;
            clmn.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            clmn.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            //
            clmn.CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.FullEditorDisplay;
            clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            clmn.Header.Caption = String.Empty;
            clmn.LockedWidth = true;
            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
            //clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            clmn.Width = 21;
        }

        public static void SetLikelyCheckBoxColumnsStyle(UltraGridColumn clmn, int imageIndex)
        {
            clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            clmn.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            clmn.AutoEdit = false;
            clmn.AutoSizeEdit = Infragistics.Win.DefaultableBoolean.False;
            clmn.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            clmn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            clmn.CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.FullEditorDisplay;
            clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            clmn.Header.Caption = String.Empty;
            clmn.LockedWidth = true;
            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            //clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            clmn.Width = 21;
        }

        public static void SetLikelyEditButtonStyleAppearance(Infragistics.Win.AppearanceBase app)
        {
            app.BackColor = SystemColors.ButtonFace;
            app.ForeColor = System.Drawing.Color.Black;
            app.ForeColorDisabled = System.Drawing.Color.Transparent;
            app.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            app.ImageHAlign = Infragistics.Win.HAlign.Center;
            app.ImageVAlign = Infragistics.Win.VAlign.Middle;
        }

        public static void SetLikelyEditButtonColumnsStyle(UltraGridColumn clmn, int imageIndex)
        {
            clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.True;
            clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            clmn.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            clmn.AutoEdit = false;
            clmn.AutoSizeEdit = Infragistics.Win.DefaultableBoolean.False;
            clmn.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            //clmn.CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            // cellappearance
            clmn.CellAppearance.ForeColor = System.Drawing.Color.Black;
            clmn.CellAppearance.ForeColorDisabled = System.Drawing.Color.Black;
            //clmn.CellAppearance.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            //clmn.CellAppearance.Image = imageIndex;
            clmn.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            clmn.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            // cellbuttonappearance
            SetLikelyEditButtonStyleAppearance(clmn.CellButtonAppearance);
            clmn.CellButtonAppearance.Image = imageIndex;
            //
            clmn.CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.FullEditorDisplay;
            clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            //clmn.LockedWidth = true;
            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
        }

        public static void CustomizeRowFilerDialog(CustomRowFiltersDialog dlg, string columnCaption)
        {
            dlg.Text = String.Format("Введите параметры фильтра для поля \"{0}\"", columnCaption);
            // колонку состояния записи в редакторе сложных фильтров 
            // стилизуем под ячейку с номером фильтра
            UltraGridOverride settings = dlg.Grid.DisplayLayout.Override;
            settings.RowSelectorAppearance.Image = null;
            settings.RowSelectorAppearance.TextHAlign = HAlign.Left;
            settings.RowSelectorAppearance.TextVAlign = VAlign.Middle;
            settings.BorderStyleRowSelector = UIElementBorderStyle.InsetSoft;
            settings.RowSelectorWidth = 17;
            settings.RowSelectorNumberStyle = RowSelectorNumberStyle.RowIndex;
        }

        /// <summary>
        /// Получить UltraGridRow от UIElement. Опрашиваются и потомки.
        /// </summary>
        /// <param name="elem">элемент</param>
        /// <returns>строка</returns>
        private static UltraGridRow GetRowFromElement(UIElement elem)
        {
            UltraGridRow row = null;
            try
            {
                row = (UltraGridRow)elem.GetContext(typeof(UltraGridRow), true);
            }
            catch { }
            return row;
        }

        /// <summary>
        /// Получить UltraGridRow по экранным координатам. Опрашиваются и потомки.
        /// </summary>
        /// <param name="X">Координата X (экранная)</param>
        /// <param name="Y">Координата Y (экранная)</param>
        /// <returns></returns>
        public static UltraGridRow GetRowFromPos(int X, int Y, UltraGrid grid)
        {
            Point pt = new Point(X, Y);
            pt = grid.PointToClient(pt);
            UIElement elem = grid.DisplayLayout.UIElement.ElementFromPoint(pt);
            return GetRowFromElement(elem);
        }
	}

    internal static class UltraGridFindRowsHelper
    {
        private delegate bool FindRowDelegate(UltraGridRow row, string columnName, string value, ref List<UltraGridRow> rows);

        private static void EnumGridRows(UltraGrid grid, UltraGridRow row, bool checkChildRowsOnly,
            FindRowDelegate checkDelegate, string columnName, string value, ref List<UltraGridRow> rows)
        {
            //grid.BeginUpdate();
            try
            {
                UltraGridRow gridRow = null;
                if (row != null)
                    gridRow = row;
                else
                    gridRow = grid.GetRow(Infragistics.Win.UltraWinGrid.ChildRow.First);

                UltraGridRow siblingRow = gridRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);

                UltraGridRow parentRow = gridRow.ParentRow;
                // если нету записей соседних, то тут или всего одна или поднимаемся выше и ищем записи там
                if (parentRow != null && siblingRow == null)
                {
                    UltraGridRow tmpParentRow = parentRow;
                    while (siblingRow == null && tmpParentRow != null)
                    {
                        siblingRow = tmpParentRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);
                        tmpParentRow = tmpParentRow.ParentRow;
                    }
                }

                while (gridRow != null)
                {
                    // если проверка только для подчиненных записей 
                    //выходим при переходе на родительскую или соседнюю записи
                    if (checkChildRowsOnly)
                    {
                        if (object.Equals(gridRow, siblingRow) || object.Equals(gridRow, parentRow))
                            break;
                    }
                    if (gridRow.Cells != null)
                        if ((!gridRow.IsFilteredOut) && checkDelegate(gridRow, columnName, value, ref rows))
                        {
                            // проверим, что бы все записи верхнего уровня были отображены в гриде
                            if (gridRow.ParentRow == null)
                                break;
                            parentRow = gridRow.ParentRow;
                            bool isFilteredOut = false;
                            while (parentRow != null)
                            {
                                isFilteredOut = parentRow.IsFilteredOut;
                                if (isFilteredOut)
                                    break;
                                parentRow = parentRow.ParentRow;
                            }

                            if (!isFilteredOut)
                                break;

                            rows.Remove(gridRow);
                        }

                    if (gridRow.HasChild())
                        gridRow = gridRow.GetChild(Infragistics.Win.UltraWinGrid.ChildRow.First);
                    else
                        if (gridRow.HasNextSibling())
                            gridRow = gridRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);
                        else
                            if (gridRow.ParentRow != null)
                            {
                                while (!gridRow.HasNextSibling())
                                {
                                    if (gridRow.ParentRow != null)
                                        gridRow = gridRow.ParentRow;
                                    else
                                    {
                                        gridRow = null;
                                        break;
                                    }
                                }
                                if (!(gridRow == null))
                                    gridRow = gridRow.GetSibling(Infragistics.Win.UltraWinGrid.SiblingRow.Next);
                            }
                            else
                                gridRow = null;
                }
            }
            finally
            {
                //grid.EndUpdate();
            }
        }

        private static bool CheckRow(UltraGridRow row, string columnName, string findValue, ref List<UltraGridRow> rows)
        {
            if (row.Cells[columnName].Value.ToString() == findValue)
            {
                rows.Add(row);
                return true;
            }
            return false;
        }

        /// <summary>
        /// поиск записи в гриде любого уровня
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static List<UltraGridRow> FindRows(UltraGrid grid, UltraGridRow parentRow, string columnName, string value)
        {
            FindRowDelegate dlg = new FindRowDelegate(CheckRow);
            UltraGridRow gridRow = grid.GetRow(Infragistics.Win.UltraWinGrid.ChildRow.First);
            FindRowsType findRowType = FindRowsType.MultiRows;

            List<UltraGridRow> listRow = new List<UltraGridRow>();

            FindRowDelegate findRow = new FindRowDelegate(CheckRow);

            bool checkChildsRow = parentRow != null;

            EnumGridRows(grid, parentRow, checkChildsRow, findRow, columnName, value, ref listRow);

            return listRow;
        }

    }
}