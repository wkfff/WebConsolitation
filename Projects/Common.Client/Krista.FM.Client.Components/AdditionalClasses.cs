using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using System.Data;

namespace Krista.FM.Client.Components
{
    #region GridColumnState
    /// <summary>
    /// �������� �������� ��������� ������� � �����
    /// </summary>
    public class GridColumnState
    {
        public GridColumnState()
        {
            
        }

        public GridColumnState(string columnName)
            : this()
        {
            ColumnName = columnName;
        }
        // �������� ������� � �����
        public string ColumnName = string.Empty;
        // ������� �������� �������
        public string ColumnCaption = string.Empty;
        // ����������, ������� ������� ��� ���
        public bool IsHiden = false;
        // ����������, ����� ������������� ������� � ����� ��� ���
        public bool IsReadOnly = false;
        // ����������, �������� �� ������� ���������
        public bool IsSystem = false;
        // ����������, �������� �� ������� 
        public bool IsNullable = false;
        // ����������, ������� ����� �������� ��� ������ ����������� � ������ �����
        public bool IsLookUp = false;
        // ����������, �������� �� ������� ���� BLOB � �������� � ���� ������ ������, ������ ����� ���� ����������
        public bool IsBLOB = false;
        // ����������, ������� ����� �������� ��� ����������� �����������
        public bool CalendarColumn = false;
        // ����� ��� �������
        public string Mask = string.Empty;
        // ����� ��� �������
        public bool UseMaskEditor = false;
        // ������ �������
        public int ColumnWidth = 100;
        // �����
        public Infragistics.Win.UltraWinGrid.ColumnStyle ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
        // ���
        public UltraGridEx.ColumnType ColumnType = UltraGridEx.ColumnType.unknown;
        // ������� �������
        public int ColumnPosition = 0;
        // �������� ��-���������
        public object DefaultValue = DBNull.Value;
        // ����������, �������� �� ������� ������� �� ��� �� ������ (������ �������������)
        public bool IsReference = false;
        // ����������, ����� �� � ������� ������� ������� �� ������ �� ��������� ������
        public bool isWrapWord = true;
        // ������������ �� ��� ����������
        //public bool UseInFilter = false;
        // �������������� ���������� ��� ��������� � ���� TAG �������
        public object Tag = null;
        // ����������, ���������� �� ������� ��� ������ ��� ��� ������ �����
        public bool isTextColumn = false;
        // ����������, �������� �� ������� ������ � ������
        public bool FirstInGroup = false;
        // ��������� �������� ����������� �������. ���� ������, � ����������� �� ���������
        public string GroupName = string.Empty;
        // ��������� �� ��, ������ �� ������� � ����� ������ ������
        public bool InGroup
        {
            get { return !string.IsNullOrEmpty(GroupName); }
        }
    }

    // ������������������������ ���������
    public class GridColumnsStates : Dictionary<string, GridColumnState>
    {
        new public void Add(string key, GridColumnState value)
        {
            base.Add(key.ToUpper(), value);
        }

        public void Add(GridColumnState value)
        {
            base.Add(value.ColumnName.ToUpper(), value);
        }

        new public bool ContainsKey(string key)
        {
            return base.ContainsKey(key.ToUpper());
        }

        new public GridColumnState this[string key]
        {
            get { return base[key.ToUpper()]; }
            set { base[key.ToUpper()] = value; }
        }
    }
    #endregion

    // ���� ��������
    public enum HierarchyType { Unknown, Regular, ParentChild };
    public enum LoadMode { AllRows, OnParentExpand, OnFlatScroll };
    public enum ViewState { NotDefined, Hierarchy, Flat };

    // ����������� ������
    public enum SearchDirectionEnum
    {
        Down = 0, Up = 1, All = 2
    }
    // ��� ������ ������
    public enum SearchContentEnum
    {
        AnyPartOfField = 0,
        WholeField = 1,
        StartOfField = 2
    }

    #region HierarchyInfo
    public class HierarchyInfo
    {
        public const int DefaultHierarchyLevelsCount = 2;
        public const int MaxRowsCount = 5000;
        public int MaxHierarchyLevelsCount = 20;
        // ��� ��������
        public HierarchyType ObjectHierarchyType = HierarchyType.Unknown;
        // ������� �� ������� �� ��������
        public string ParentRefClmnName = string.Empty;
        // �������-��������
        public string ParentClmnName = string.Empty;
        // ���������� ������� (������ ������ ���� >= 1)
        public int LevelsCount = DefaultHierarchyLevelsCount;
        // ����� �������
        public string[] LevelsNames = null;
        // ������� ������� ����� ����������� (?)
        public bool isDivideCode = false;
        // ����� �������� 
        public LoadMode loadMode = LoadMode.AllRows;
        // ������� ���
        public ViewState CurViewState = ViewState.NotDefined;
        // ��� ������� ������ � ������� ����
        public string FlatLevelName = string.Empty;
    }
    #endregion


    public class SearchInfo
    {
        public string searchString;
        public List<string> lookIn;
        public SearchDirectionEnum searchDirection;
        public SearchContentEnum searchContent;
        public bool matchCase = false;
        public string searchColumn;
        public bool inHierarchy;

        public SearchInfo()
        {
            searchContent = SearchContentEnum.AnyPartOfField;
            lookIn = new List<string>();
            matchCase = false;
            searchDirection = SearchDirectionEnum.Down;
            searchString = string.Empty;
            searchColumn = string.Empty;
            inHierarchy = false;
        }
    }

    public class UltraGridTextSearcher
    {
        protected UltraGrid _grid;
        protected SearchInfo _searchInfo;

        public UltraGridTextSearcher(UltraGrid grid, SearchInfo searchInfo)
        {
            _grid = grid;
            _searchInfo = searchInfo;
        }

        internal UltraGridColumn m_oColumn;

        public void Search()
        {
            Search(true);
        }

        internal virtual bool Search(bool showMessage)
        {
            return true;
        }

        #region ����� ��������

        protected bool CheckRow(UltraGridRow oRow)
        {
            if (MatchText(oRow))
            {
                _grid.ActiveRow = oRow;
                if (m_oColumn != null)
                {
                    _grid.ActiveCell = oRow.Cells[m_oColumn.Key];
                    _grid.PerformAction(UltraGridAction.EnterEditMode, false, false);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// ����� ������ � ������
        /// </summary>
        /// <param name="oRow"></param>
        /// <returns></returns>
        protected bool MatchText(UltraGridRow oRow)
        {
            if (oRow == null)
                return false;
            // �������� �� ���� �����, � ������� ����� ������ �����
            foreach (string columnName in _searchInfo.lookIn)
            {
                UltraGridColumn oCol = _grid.DisplayLayout.Bands[0].Columns[columnName];
                if (oRow.Cells[oCol.Key].Value != null)
                {
                    if (Match(_searchInfo.searchString, oRow.Cells[oCol.Key].Value.ToString()))
                    {
                        m_oColumn = oCol;
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool Match(string userString, string cellValue)
        {
            //   If our search is case insensitive, make both strings uppercase
            if (!_searchInfo.matchCase)
            {
                userString = userString.ToUpper();
                cellValue = cellValue.ToUpper();
            }

            //   If we are searching any part of the cell value...
            if (_searchInfo.searchContent == SearchContentEnum.AnyPartOfField)
            {
                //   If the user string is larger than the cell value, it is by definition
                //   a mismatch, so return false
                if (userString.Length > cellValue.Length)
                    return false;
                else if (userString.Length == cellValue.Length)
                {
                    //   If the lengths are equal, the strings must be equal as well
                    return userString == cellValue;
                }
                else
                {
                    //   There is probably an easier way to do this
                    for (int i = 0; i <= (cellValue.Length - userString.Length); i++)
                    {
                        if (userString == cellValue.Substring(i, userString.Length))
                            return true;
                    }
                    return false;
                }
            }
            else if (_searchInfo.searchContent == SearchContentEnum.WholeField)
            {
                return userString == cellValue;
            }
            else if (_searchInfo.searchContent == SearchContentEnum.StartOfField)
            {
                if (userString.Length >= cellValue.Length)
                {
                    return userString.Substring(0, cellValue.Length) == cellValue;
                }
                else
                {
                    return cellValue.Substring(0, userString.Length) == userString;
                }
            }
            return false;
        }

        #endregion
    }

    internal class HierarchyUltraGridTextSearcher : UltraGridTextSearcher
    {
        private HierarchyUltraGridTextSearcher(UltraGrid grid, SearchInfo searchInfo)
            : base(grid, searchInfo)
        {
            
        }

        private bool Search(SiblingRow searchDirection, UltraGridRow oRow)
        {
            if (oRow == null)
                oRow = _grid.ActiveRow;
            if (oRow == null)
            {
                oRow = _grid.GetRow(ChildRow.First);
                oRow.Activate();
            }
            while (oRow != null)
            {
                if (Search(oRow))
                    return true;
                oRow = oRow.GetSibling(searchDirection);
            }
            return false;
        }

        /// <summary>
        /// ����������� ����� �������� � ��������
        /// </summary>
        /// <param name="parentRow"></param>
        private bool Search(UltraGridRow parentRow)
        {
            foreach (UltraGridChildBand childBand in parentRow.ChildBands)
            {
                foreach (UltraGridRow childRow in childBand.Rows)
                {
                    UltraGridRow oRow = childRow;
                    while (oRow != null)
                    {
                        if (MatchText(oRow))
                        {
                            _grid.ActiveRow = oRow;
                            if (m_oColumn != null)
                            {
                                _grid.ActiveCell = oRow.Cells[m_oColumn.Key];
                                _grid.PerformAction(UltraGridAction.EnterEditMode, false, false);
                            }
                            return true;
                        }
                        if (Search(oRow))
                            return true;
                        oRow = oRow.GetSibling(SiblingRow.Next);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ����� ������ 
        /// </summary>
        internal override bool Search(bool showMessage)
        {
            UltraGridRow oRow = UltraGridHelper.GetRowCells(_grid.ActiveRow);
            bool isFindRow = false;
            if (oRow == null)
            {
                oRow = _grid.GetRow(ChildRow.First);
                oRow.Activate();
            }
            try
            {
                // ����� ���� �� �����
                if (_searchInfo.searchDirection == SearchDirectionEnum.Down)
                {
                    if (Search(SiblingRow.Next, oRow))
                        return true;
                }
                //   ����� ����� �� �����
                else if (_searchInfo.searchDirection == SearchDirectionEnum.Up)
                {
                    if (Search(SiblingRow.Previous, oRow))
                        return true;
                }
                //   Search all rows. First, we start with the active row. If we don//t find
                //   it by the time we hit the  last row, try again starting from the first row
                else if (_searchInfo.searchDirection == SearchDirectionEnum.All)
                {
                    if (Search(SiblingRow.Next, oRow))
                        return true;
                    //   We didn't find it the first time around, so start again from the first row
                    oRow = _grid.GetRow(ChildRow.First);
                    if (Search(SiblingRow.Next, oRow))
                        return true;
                }
                return false;
            }
            finally
            {
                if (showMessage)
                    MessageBox.Show("����������� ��� ������. ������ '" + _searchInfo.searchString + "' �� ���� �������.", "����� ������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    /*
    internal class FlatUltraGridTextSearcher : UltraGridTextSearcher
    {
        internal FlatUltraGridTextSearcher(UltraGrid grid, SearchInfo searchInfo)
            :base(grid, searchInfo)
        {
            
        }

        /// <summary>
        /// ����� ������ 
        /// </summary>
        internal override bool Search(bool showMessage)
        {
            UltraGridRow oRow = UltraGridHelper.GetRowCells(_grid.ActiveRow);
            bool isFindRow = false;
            if (oRow == null)
            {
                oRow = _grid.GetRow(ChildRow.First);
                oRow.Activate();
            }
            try
            {
                // ����� ���� �� �����
                if (_searchInfo.searchDirection == SearchDirectionEnum.Down)
                {
                    isFindRow = Search(SiblingRow.Next, oRow, false);
                }
                //   ����� ����� �� �����
                else if (_searchInfo.searchDirection == SearchDirectionEnum.Up)
                {
                    isFindRow = Search(SiblingRow.Previous, oRow, false);
                }
                //   Search all rows. First, we start with the active row. If we don//t find
                //   it by the time we hit the  last row, try again starting from the first row
                else if (_searchInfo.searchDirection == SearchDirectionEnum.All)
                {
                    isFindRow = Search(SiblingRow.Next, oRow, false);
                    //   We didn't find it the first time around, so start again from the first row
                    if (!isFindRow)
                    {
                        oRow = _grid.GetRow(ChildRow.First);
                        isFindRow = Search(SiblingRow.Next, oRow, true);
                    }
                }

                return isFindRow;
            }
            finally
            {
                if (!isFindRow && showMessage)
                    MessageBox.Show("����������� ��� ������. ������ '" + _searchInfo.searchString + "' �� ���� �������.", "����� ������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool Search(SiblingRow searchDirection, UltraGridRow oRow, bool checkFirstRow)
        {
            if (oRow == null)
                oRow = _grid.ActiveRow;
            if (oRow == null)
            {
                oRow = _grid.GetRow(ChildRow.First);
                oRow.Activate();
            }
            while (oRow != null)
            {
                if (checkFirstRow)
                    if (CheckRow(oRow))
                        return true;
                oRow = oRow.GetSibling(searchDirection);
                if (CheckRow(oRow))
                    return true;
            }
            return false;
        }
    }
    */
    internal class GroupUltraGridTextSearcher : UltraGridTextSearcher
    {
        internal GroupUltraGridTextSearcher(UltraGrid grid, SearchInfo searchInfo)
            :base(grid, searchInfo)
        {
            
        }

        internal override bool Search(bool showMessage)
        {
            if (_grid.Rows.Count == 0)
                return false;
            UltraGridRow oRow = _grid.ActiveRow;
            bool isFindRow = false;
            if (oRow == null)
            {
                oRow = _grid.GetRow(ChildRow.First);
                oRow.Activate();
            }
            try
            {
                // ����� ���� �� �����
                if (_searchInfo.searchDirection == SearchDirectionEnum.Down)
                {
                    if (Search(SiblingRow.Next, oRow, false))
                    {
                        isFindRow = true;
                        return isFindRow;
                    }
                }
                //   ����� ����� �� �����
                else if (_searchInfo.searchDirection == SearchDirectionEnum.Up)
                {
                    if (Search(SiblingRow.Previous, oRow, false))
                    {
                        isFindRow = true;
                        return isFindRow;
                    }
                }
                //   Search all rows. First, we start with the active row. If we don//t find
                //   it by the time we hit the  last row, try again starting from the first row
                else if (_searchInfo.searchDirection == SearchDirectionEnum.All)
                {
                    if (Search(SiblingRow.Next, oRow, false))
                    {
                        isFindRow = true;
                        return isFindRow;
                    }
                    //   We didn't find it the first time around, so start again from the first row
                    oRow = _grid.GetRow(ChildRow.First);
                    if (Search(SiblingRow.Next, oRow, true))
                    {
                        isFindRow = true;
                        return isFindRow;
                    }
                }
                return isFindRow;
            }
            finally
            {
                if (!isFindRow && showMessage)
                    MessageBox.Show("����������� ��� ������. ������ '" + _searchInfo.searchString + "' �� ���� �������.", "����� ������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool Search(SiblingRow searchDirection, UltraGridRow oRow)
        {
            oRow = UltraGridHelper.GetRowCells(oRow);
            while (oRow != null)
            {
                if (CheckRow(oRow))
                    return true;
                oRow = oRow.GetSibling(searchDirection);
            }
            return false;
        }

        private bool Search(SiblingRow searchDirection, UltraGridRow oRow, bool isFirstCheck)
        {
            if (oRow == null)
                oRow = _grid.ActiveRow;
            if (oRow == null)
            {
                oRow = _grid.GetRow(ChildRow.First);
                oRow.Activate();
            }

            while (oRow != null)
            {
                // ���� ���� �� ���� ������������, �� �������� �� ������, �� ������� �����
                if (isFirstCheck)
                {
                    if (oRow.IsDataRow)
                    {
                        if (CheckRow(oRow))
                            return true;
                    }
                    else if (Search(searchDirection, oRow))
                        return true;
                }

                // ����� �������� ��� �������� ������������ ������ ��� ������
                if (oRow.GetSibling(searchDirection) == null)
                {
                    if (oRow.ParentRow != null)
                    {
                        while (oRow.ParentRow != null)
                        {
                            oRow = oRow.ParentRow;
                            if (oRow.GetSibling(searchDirection) != null || oRow.ParentRow == null)
                            {
                                oRow = oRow.GetSibling(searchDirection);
                                break;
                            }
                        }
                    }
                    else
                        oRow = oRow.GetSibling(searchDirection);
                }
                else
                    oRow = oRow.GetSibling(searchDirection);

                if (oRow == null)
                    break;
                // �������� ��������� ������ ����� �������
                if (oRow.IsDataRow)
                {
                    if (CheckRow(oRow))
                        return true;
                }
                else if (Search(searchDirection, oRow))
                    return true;
            }
            return false;
        }
    }

    internal class UltraGridRowSeacher
    {
        private UltraGrid _grid;
        private SearchInfo _searchInfo;

        internal UltraGridRowSeacher(UltraGrid grid, SearchInfo searchInfo)
        {
            _grid = grid;
            _searchInfo = searchInfo;
        }

        internal UltraGridRow SearchRow()
        {
            return Search(null);
        }

        internal UltraGridRow[] SearchRows()
        {
            return null;
        }

        private UltraGridRow Search(UltraGridRow oRow)
        {
            if (oRow == null)
            {
                oRow = _grid.GetRow(ChildRow.First);
            }

            while (oRow != null)
            {
                if (oRow.IsDataRow)
                {
                    if (CheckRow(oRow))
                        return oRow;
                }
                else if (Search(SiblingRow.Next, ref oRow))
                    return oRow;

                // ����� �������� ��� �������� ������������ ������ ��� ������
                if (oRow.GetSibling(SiblingRow.Next) == null)
                {
                    if (oRow.ParentRow != null)
                    {
                        while (oRow.ParentRow != null)
                        {
                            oRow = oRow.ParentRow;
                            if (oRow.GetSibling(SiblingRow.Next) != null || oRow.ParentRow == null)
                            {
                                oRow = oRow.GetSibling(SiblingRow.Next);
                                break;
                            }
                        }
                    }
                    else
                        oRow = oRow.GetSibling(SiblingRow.Next);
                }
                else
                    oRow = oRow.GetSibling(SiblingRow.Next);

                if (oRow == null)
                    break;
                // �������� ��������� ������ ����� �������
                if (oRow.IsDataRow)
                {
                    if (CheckRow(oRow))
                        return oRow;
                }
                else if (Search(SiblingRow.Next, ref oRow))
                    return oRow;
            }
            return null;
        }

        private bool Search(SiblingRow searchDirection, ref UltraGridRow oRow)
        {
            UltraGridRow _oRow = UltraGridHelper.GetRowCells(oRow);
            while (_oRow != null)
            {
                oRow = _oRow;
                if (CheckRow(_oRow))
                {
                    return true;
                }
                _oRow = _oRow.GetSibling(searchDirection);
            }
            return false;
        }
        
        #region ����� ��������

        private UltraGridColumn m_oColumn;

        private bool CheckRow(UltraGridRow oRow)
        {
            if (MatchText(oRow))
            {
                //_grid.ActiveRow = oRow;
                if (m_oColumn != null)
                {
                    //_grid.ActiveCell = oRow.Cells[m_oColumn.Key];
                    //_grid.PerformAction(UltraGridAction.EnterEditMode, false, false);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// ����� ������ � ������
        /// </summary>
        /// <param name="oRow"></param>
        /// <returns></returns>
        private bool MatchText(UltraGridRow oRow)
        {
            if (oRow == null)
                return false;
            // �������� �� ���� �����, � ������� ����� ������ �����
            foreach (string columnName in _searchInfo.lookIn)
            {
                UltraGridColumn oCol = _grid.DisplayLayout.Bands[0].Columns[columnName];
                if (oRow.Cells[oCol.Key].Value != null)
                {
                    if (Match(_searchInfo.searchString, oRow.Cells[oCol.Key].Value.ToString()))
                    {
                        m_oColumn = oCol;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Match(string userString, string cellValue)
        {
            //   If our search is case insensitive, make both strings uppercase
            if (!_searchInfo.matchCase)
            {
                userString = userString.ToUpper();
                cellValue = cellValue.ToUpper();
            }

            //   If we are searching any part of the cell value...
            if (_searchInfo.searchContent == SearchContentEnum.AnyPartOfField)
            {
                //   If the user string is larger than the cell value, it is by definition
                //   a mismatch, so return false
                if (userString.Length > cellValue.Length)
                    return false;
                else if (userString.Length == cellValue.Length)
                {
                    //   If the lengths are equal, the strings must be equal as well
                    return userString == cellValue;
                }
                else
                {
                    //   There is probably an easier way to do this
                    for (int i = 0; i <= (cellValue.Length - userString.Length); i++)
                    {
                        if (userString == cellValue.Substring(i, userString.Length))
                            return true;
                    }
                    return false;
                }
            }
            else if (_searchInfo.searchContent == SearchContentEnum.WholeField)
            {
                return userString == cellValue;
            }
            else if (_searchInfo.searchContent == SearchContentEnum.StartOfField)
            {
                if (userString.Length >= cellValue.Length)
                {
                    return userString.Substring(0, cellValue.Length) == cellValue;
                }
                else
                {
                    return cellValue.Substring(0, userString.Length) == userString;
                }
            }
            return false;
        }

        #endregion
    }

    internal class ChangedUltraGridRow
    {
        private int rowID = -1;
        private int parentID = -1;
        private DataRowState rowState;

        internal ChangedUltraGridRow(UltraGridRow gridRow)
        {
            rowID = Convert.ToInt32(gridRow.Cells["ID"].Value);
            UltraGridRow parentRow = gridRow.ParentRow;
            while (parentRow != null)
            {
                if (parentRow.Cells != null)
                    parentID = Convert.ToInt32(parentRow.Cells["ID"].Value);
                parentRow = parentRow.ParentRow;
            }
            rowState = DataRowState.Unchanged;
            if (gridRow.Tag != null)
                rowState = (DataRowState) gridRow.Tag;
        }
        /// <summary>
        /// ID ������
        /// </summary>
        internal int RowID
        {
            get {
                return rowID; }
        }

        /// <summary>
        /// ID ������������ ������ ������ �������� ������
        /// </summary>
        internal int ParentID
        {
            get
            {
                return parentID;
            }
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        internal DataRowState RowState
        {
            get
            {
                return rowState;
            }
        }

        internal UltraGridRow FindRow(UltraGridEx gridEx, UltraGridRow parentRow)
        {
            if (parentID == -1)
                return UltraGridHelper.FindGridRow(gridEx.ugData, "ID", RowID.ToString());

            List<UltraGridRow> list =
                UltraGridFindRowsHelper.FindRows(gridEx.ugData, parentRow, "ID", RowID.ToString());
            if (list.Count > 0)
                return list[0];

            return null;
        }
    }
}