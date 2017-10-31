using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;
using AutoCompleteMode = Infragistics.Win.AutoCompleteMode;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports.Common.CommonParamForm
{
    public partial class MultiCheckForm : Form
    {
        private const string ID = "ID";
        private const string NAME = "NAME";
        private const string PARENTID = "PARENTID";
        private const string COL_CHECK = "COL_CHECK";

        private string findText = String.Empty;

        public ParamBookInfo Settings { set; get; }
        public DataTable SourceList { set; get; }
        public DataTable DataList { set; get; }
        public string SelectedValues { set; get; }
        private readonly List<string> selectedValues = new List<string>();
        private readonly Collection<string> columnList = new Collection<string>();
        readonly ReportDBHelper dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
        private readonly Dictionary<string, DataTable> clsData = new Dictionary<string, DataTable>();
        private readonly Dictionary<string, string> lookupColumns = new Dictionary<string, string>();
        private DataSet dsData = new DataSet();

        public MultiCheckForm()
        {
            InitializeComponent();

            miCheck.Image = Properties.Resources.Check;
            miUncheck.Image = Properties.Resources.Uncheck;
            miTree.Image = Properties.Resources.Tree;
            miFind.Image = Properties.Resources.Find;
            miExpand.Image = Properties.Resources.Expand;
            miCollapse.Image = Properties.Resources.Collapse;
            miFilter.Image = Properties.Resources.Filter;
        }

        public void LoadBookData()
        {
            GetVisibleColumns();
            LoadSourceData();
            LoadGridData();
            SetVisualSettings();
        }

        private void SetVisualSettings()
        {
            SetGridVisualSettings();

            btnCancel.Left = Width - btnCancel.Width - 10;
            btnOk.Left = btnCancel.Left - btnOk.Width - 10;
            cbSource.Left = 5;
            cbSource.Width = btnOk.Left - 20;

            miTree.Checked = Settings.HasHierarchy;
            miFilter.Visible = !String.IsNullOrEmpty(Settings.EditorSearchField);
        }

        private void SetGridVisualSettings()
        {
            grid.DisplayLayout.GroupByBox.Prompt = "Группировка";
            grid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            grid.DisplayLayout.Override.ExpansionIndicator = ShowExpansionIndicator.CheckOnDisplay;
            grid.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.True;
            grid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            grid.DisplayLayout.Override.CellClickAction = CellClickAction.Edit;
            grid.DisplayLayout.MaxBandDepth = 10;
            grid.DisplayLayout.Override.RowFilterMode = RowFilterMode.SiblingRowsOnly;

            grid.BeforeRowFilterDropDownPopulate += BeforeRowFilterDropDownPopulate;
        }

        private void BeforeRowFilterDropDownPopulate(object sender, BeforeRowFilterDropDownPopulateEventArgs args)
        {
            args.Handled = true;
        }

        private void LoadGridData()
        {
            grid.BeginUpdate();
            
            dsData = new DataSet();
            bs = new BindingSource();

            if (Settings.HasSource && cbSource.SelectedRow != null)
            {
                var sourceId = cbSource.SelectedRow.Cells[ID].Value;
                var sourceFilter = String.Format("SourceID = '{0}'", sourceId);
                dsData.Tables.Add(DataTableUtils.FilterDataSet(DataList, sourceFilter));
            }
            else
            {
                dsData.Tables.Add(DataList);
            }

            if (clsData.Count == 0 && Settings.EntityKey.Length > 0)
            {
                var dataEntity = ConvertorSchemeLink.GetEntity(Settings.EntityKey);
                lookupColumns.Clear();

                foreach (var ass in dataEntity.Associations)
                {
                    // если ссылка не на себя
                    if (ass.Value.RoleBridge.ObjectKey == Settings.EntityKey) continue;

                    var clsKey = ass.Value.RoleBridge.ObjectKey;
                    var refName = ass.Value.RoleDataAttribute.Name;
                    var fieldName = String.Format("lookup{0}", refName);

                    if (Settings.ItemTemplate.Length == 0 || Settings.ItemTemplate.Contains(refName))
                    {
                        clsData.Add(refName, dbHelper.GetEntityData(clsKey));
                    }

                    lookupColumns.Add(refName, fieldName);
                }
            }

            if (Settings.HasHierarchy)
            {
                dsData.Relations.Add(
                    dsData.Tables[0].TableName,
                    dsData.Tables[0].Columns[ID],
                    dsData.Tables[0].Columns[PARENTID],
                    false);
            }

            grid.Layouts.Clear();
            bs.DataSource = dsData;
            grid.DataSource = bs;
            grid.EndUpdate();
        }

        private object ResolveLookupValue(string fieldName, object value)
        {
            if (clsData.ContainsKey(fieldName))
            {
                return Convert.ToInt32(value) > 0
                           ? clsData[fieldName].Select(String.Format("{0} = {1}", ID, value))[0][NAME]
                           : String.Empty;
            }

            return String.Empty;
        }

        #region Список источников

        protected virtual object GetActiveSource()
        {
            return Settings.ActiveSource;
        }

        private int FindActiveSourceIndex()
        {
            var activeSource = Convert.ToInt32(GetActiveSource());
            var rowIndex = 0;

            foreach (DataRow rowSource in SourceList.Rows)
            {
                var sourceId = Convert.ToInt32(rowSource[ID]);

                if (activeSource == sourceId)
                {
                    return rowIndex;
                }

                rowIndex++;
            }

            rowIndex = 0;

            foreach (DataRow rowSource in SourceList.Rows)
            {
                var sourceYear = Convert.ToInt32(rowSource[HUB_Datasources.YEAR]);
                if (sourceYear == DateTime.Now.Year)
                {
                    return rowIndex;
                }

                rowIndex++;
            }

            return 0;
        }

        private void LoadSourceData()
        {
            if (Settings.HasSource)
            {
                cbSource.DataSource = SourceList;
                cbSource.RowSelected -= cbSource_RowSelected;

                if (cbSource.Rows.Count > 0)
                {
                    var activeSourceIndex = FindActiveSourceIndex();
                    cbSource.SelectedRow = cbSource.Rows[activeSourceIndex];
                }

                cbSource.RowSelected += cbSource_RowSelected;
            }

            cbSource.Visible = Settings.HasSource;
        }

        private static string GetSourceCaption(UltraGridRow row)
        {
            if (row != null)
            {
                return String.Format("{0}/{1} {2:0000} - {3}",
                                     row.Cells[HUB_Datasources.SUPPLIERCODE].Value,
                                     row.Cells[HUB_Datasources.DATANAME].Value,
                                     row.Cells[HUB_Datasources.DATACODE].Value,
                                     row.Cells[HUB_Datasources.YEAR].Value);
            }

            return String.Empty;
        }

        private void cbSource_RowSelected(object sender, RowSelectedEventArgs e)
        {
            if (e.Row != null)
            {
                cbSource.RowSelected -= cbSource_RowSelected;
                cbSource.SelectedRow = e.Row;
                cbSource.RowSelected += cbSource_RowSelected;
                LoadGridData();
            }
        }

        private static void SetSourceColumnHeader(UltraGridColumn column, Dictionary<string, string> columnCaptions)
        {
            if (columnCaptions.ContainsKey(column.Header.Caption))
            {
                column.CellMultiLine = DefaultableBoolean.True;
                column.Header.Caption = columnCaptions[column.Header.Caption];
            }
            else
            {
                column.Hidden = true;
            }
        }

        private void cbSource_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            cbSource.MaxDropDownItems = 20;
            cbSource.AutoCompleteMode = AutoCompleteMode.None;

            var textColumn = e.Layout.Bands[0].Columns.Add("SOURCETEXT");
            cbSource.DisplayMember = textColumn.Key;

            var captions = new Dictionary<string, string>
                               {
                                   {HUB_Datasources.id.ToUpper(), "Номер"},
                                   {HUB_Datasources.DATACODE, "Код"},
                                   {HUB_Datasources.DATANAME, "Источник"},
                                   {HUB_Datasources.YEAR, "Год"},
                                   {HUB_Datasources.MONTH, "Месяц"},
                                   {HUB_Datasources.VARIANT, "Вариант"},
                                   {HUB_Datasources.SUPPLIERCODE, "Поставщик данных"}
                               };

            foreach (var column in e.Layout.Bands[0].Columns)
            {
                SetSourceColumnHeader(column, captions);
            }
        }

        private void cbSource_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["SOURCETEXT"].Value = GetSourceCaption(e.Row);
        }

        private void cbSource_Resize(object sender, EventArgs e)
        {
            cbSource.DropDownWidth = cbSource.Width;
        }

        #endregion

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            var dataEntity = ConvertorSchemeLink.GetEntity(Settings.EntityKey);
            var mainBand = e.Layout.Bands[0];
            mainBand.Override.RowSizing = RowSizing.AutoFree;

            foreach (var currentBand in e.Layout.Bands)
            {
                currentBand.Override.RowSizing = RowSizing.AutoFree;

                if (Settings.MultiSelect && !currentBand.Columns.Exists(COL_CHECK))
                {
                    var checkColumn = currentBand.Columns.Add(COL_CHECK);
                    checkColumn.CellActivation = Activation.AllowEdit;
                    checkColumn.CellClickAction = CellClickAction.Edit;
                    checkColumn.Style = ColumnStyle.CheckBox;
                    checkColumn.AllowRowFiltering = DefaultableBoolean.False;
                }

                foreach (var lookupColumn in lookupColumns)
                {
                    if (!currentBand.Columns.Exists(lookupColumn.Value))
                    {
                        var lookupGridColumn = currentBand.Columns.Add(lookupColumn.Value);
                        lookupGridColumn.DataType = typeof (string);
                    }
                }

                var startVisibleIndex = Settings.MultiSelect ? 1 : 0;

                if (currentBand.ParentBand == null)
                {
                    foreach (var column in currentBand.Columns)
                    {
                        column.CellMultiLine = DefaultableBoolean.True;
                        var attrName = column.Key.ToUpper();

                        if (dataEntity != null)
                        {
                            foreach (var attr in dataEntity.Attributes.Values)
                            {
                                if (attr.Name.ToUpper() == attrName)
                                {
                                    column.Header.Caption = attr.Caption;
                                    column.Hidden = !attr.Visible || attr.Caption.Length == 0;

                                    if (columnList.Count > 0)
                                    {
                                        column.Hidden = !columnList.Contains(attrName);

                                        if (!column.Hidden)
                                        {
                                            column.Header.VisiblePosition = startVisibleIndex +
                                                                            columnList.IndexOf(attrName);

                                            if (attr.Type != DataAttributeTypes.dtString || attr.Size < 30)
                                            {
                                                SetColumnWidth(column, attr.Size * Convert.ToInt32(grid.Font.Size + 2));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (attrName == ID || attrName == PARENTID)
                        {
                            column.Hidden = true;
                        }
                    }

                    if (Settings.MultiSelect)
                    {
                        currentBand.Columns[COL_CHECK].Header.Caption = String.Empty;
                    }

                    foreach (var lookupColumn in lookupColumns)
                    {
                        var refColumn = currentBand.Columns[lookupColumn.Key];
                        var valColumn = currentBand.Columns[lookupColumn.Value];

                        valColumn.Header.Caption = refColumn.Header.Caption;
                        valColumn.Hidden = refColumn.Hidden;
                        refColumn.Hidden = true;
                    }
                }
                else
                {
                    currentBand.ColHeadersVisible = false;

                    foreach (var column in currentBand.Columns)
                    {
                        var parentColumn = mainBand.Columns[column.Key];
                        column.Header.Caption = parentColumn.Header.Caption;
                        column.Hidden = parentColumn.Hidden;
                        column.Header.VisiblePosition = parentColumn.Header.VisiblePosition;
                        column.MinWidth = parentColumn.MinWidth;
                        column.MaxWidth = parentColumn.MaxWidth;
                        column.Width = parentColumn.Width;
                        column.CellMultiLine = DefaultableBoolean.True;
                    }
                }

                if (Settings.MultiSelect)
                {
                    currentBand.Columns[COL_CHECK].Header.VisiblePosition = 0;
                    SetColumnWidth(currentBand.Columns[COL_CHECK], 20);
                }
            }

            if (dataEntity != null)
            {
                Text = String.Format("Справочник {0}", dataEntity.FullCaption);
            }

            if (Settings.HasHierarchy)
            {
                mainBand.ColumnFilters[PARENTID].FilterConditions.Add(FilterComparisionOperator.Equals, DBNull.Value);
            }
        }

        private void SetColumnWidth(UltraGridColumn column, int width)
        {
            column.MinWidth = width;
            column.MaxWidth = width;
            column.Width = width;            
        }

        private void MultiCheckForm_Resize(object sender, EventArgs e)
        {
            SetVisualSettings();
        }

        private void UpdateResultList(object key, object value)
        {
            var keyValue = Convert.ToString(key);

            if (Convert.ToBoolean(value))
            {
                if (!selectedValues.Contains(keyValue))
                {
                    selectedValues.Add(keyValue);
                }
            }
            else
            {
                if (selectedValues.Contains(keyValue))
                {
                    selectedValues.Remove(keyValue);
                }
            }
        }

        private void SetCheckState(UltraGridRow row, bool value)
        {
            foreach (UltraGridChildBand childBand in row.ChildBands)
            {
                foreach (var childRow in childBand.Rows)
                {
                    SetRowCheck(childRow, value);
                    SetCheckState(childRow, value);
                }
            }
        }

        private void grid_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key != COL_CHECK) return;
            
            grid.BeginUpdate();
            
            e.Cell.Row.Update();
            UpdateResultList(e.Cell.Row.Cells[ID].Value, e.Cell.Value);

            if (Settings.HasHierarchy && Settings.DeepSelect)
            {
                SetCheckState(e.Cell.Row, Convert.ToBoolean(e.Cell.Value));
            }

            grid.EndUpdate();
        }

        private void GetVisibleColumns()
        {
            columnList.Clear();
            if (Settings.ItemTemplate.Length <= 0) return;
            var templateParts = Settings.ItemTemplate.Split(' ');

            foreach (var columnName in templateParts)
            {
                if (DataList.Columns.Contains(columnName))
                {
                    columnList.Add(columnName.ToUpper());
                }
            }
        }

        private void GetResultValue()
        {
            var strBuilder = new StringBuilder();

            foreach (var selectedValue in selectedValues)
            {
                strBuilder.Append(selectedValue);
                strBuilder.Append(',');
            }

            SelectedValues = strBuilder.ToString().Trim(',');
            FinalizeGridSource();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GetResultValue();
        }

        private void grid_AfterCellActivate(object sender, EventArgs e)
        {
            if (Settings.MultiSelect) return;
            UpdateResultList(grid.ActiveRow.Cells[ID].Value, true);
            GetResultValue();
            DialogResult = DialogResult.OK;
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (Settings.MultiSelect && e.Row.Cells[COL_CHECK].Value == null)
            {
                e.Row.Cells[COL_CHECK].Value = false;
            }

            foreach (var lookupColumn in lookupColumns)
            {
                var cellValue = e.Row.Cells[lookupColumn.Key].Value;

                if (cellValue != null)
                {
                    e.Row.Cells[lookupColumn.Value].Value = ResolveLookupValue(lookupColumn.Key, cellValue);
                }
            }
        }

        private void FinalizeGridSource()
        {
            grid.DataSource = null;
            bs.DataSource = null;
            dsData.Relations.Clear();

            if (dsData.Tables.Contains(DataList.TableName))
            {
                dsData.Tables.Remove(DataList.TableName);
            }
        }

        private void MultiCheckForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FinalizeGridSource();
        }

        private void grid_AfterSortChange(object sender, BandEventArgs e)
        {
            grid.AfterSortChange -= grid_AfterSortChange;

            var rootBand = e.Band.Layout.Bands[0];

            foreach (var band in e.Band.Layout.Bands)
            {
                if (band == rootBand) continue;

                foreach (UltraGridColumn sortedColumn in rootBand.SortedColumns)
                {
                    band.SortedColumns.Add(sortedColumn.Key, sortedColumn.SortIndicator == SortIndicator.Descending);
                }
            }

            grid.AfterSortChange += grid_AfterSortChange;
        }

        private void SetRowCheck(UltraGridRow row, bool value)
        {
            if (row.Cells == null) return;
            row.Cells[COL_CHECK].Value = value;
            UpdateResultList(row.Cells[ID].Value, value);
        }

        private void SetChildRowsState(UltraGridRow oRow, bool value)
        {
            oRow = oRow.GetChild(ChildRow.First);

            while (oRow != null)
            {
                SetRowCheck(oRow, value);
                SetChildRowsState(oRow, value);
                oRow = oRow.GetSibling(SiblingRow.Next);
            }
        }

        private void SetRowsState(bool value)
        {
            grid.CellChange -= grid_CellChange;
            var oRow = grid.GetRow(ChildRow.First);

            while (oRow != null)
            {
                SetRowCheck(oRow, value);
                SetChildRowsState(oRow, value);
                oRow = oRow.GetSibling(SiblingRow.Next);
            }

            grid.CellChange += grid_CellChange;            
        }

        private void miCheck_Click(object sender, EventArgs e)
        {
            SetRowsState(true);
        }

        private void miTree_Click(object sender, EventArgs e)
        {
            Settings.HasHierarchy = miTree.Checked;
            FinalizeGridSource();
            LoadGridData();
        }

        private void miUncheck_Click(object sender, EventArgs e)
        {
            SetRowsState(false);
        }

        private bool Match(object cellValue)
        {
            return Convert.ToString(cellValue).ToUpper().Contains(findText);
        }

        private bool MatchText(UltraGridRow oRow, UltraGridColumn oColumn)
        {
            if (oRow == null) return false;

            if (oColumn == null)
            {
                foreach (var oCol in grid.DisplayLayout.Bands[0].Columns)
                {
                    if (oRow.Cells[oCol.Key].Value == null) continue;

                    if (Match(oRow.Cells[oCol.Key].Value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (Match(oRow.Cells[oColumn.Key].Value))
                {
                    return true;
                }                
            }

            return false;
        }

        private bool FindChildRow(UltraGridRow oRow, UltraGridColumn oColumn)
        {
            oRow = oRow.GetChild(ChildRow.First);

            while (oRow != null)
            {
                if (!oRow.HiddenResolved)
                {
                    if (MatchText(oRow, oColumn))
                    {
                        grid.ActiveRow = oRow;
                        oRow.Selected = true;

                        if (oColumn != null)
                        {
                            grid.ActiveCell = oRow.Cells[oColumn.Key];
                            grid.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        }

                        return true;
                    }
                }

                oRow = oRow.GetSibling(SiblingRow.Next);
            }

            return false;
        }

        private void miFind_Click(object sender, EventArgs e)
        {
            findText = textFind.Text.ToUpper();
            var oRow = grid.GetRow(ChildRow.First);

            UltraGridColumn oCol = null;

            if (grid.ActiveCell != null)
            {
                oCol = grid.ActiveCell.Column;
            }

            while (oRow != null)
            {
                if (!oRow.HiddenResolved)
                {
                    if (MatchText(oRow, oCol))
                    {
                        grid.ActiveRow = oRow;

                        if (oCol != null)
                        {
                            grid.ActiveCell = oRow.Cells[oCol.Key];
                            grid.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        }

                        return;
                    }

                    if (FindChildRow(oRow, oCol))
                    {
                        return;
                    }
                }

                oRow = oRow.GetSibling(SiblingRow.Next);
            }
        }

        private void grid_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            grid.AfterRowFilterChanged -= grid_AfterRowFilterChanged;
            
            var rootBand = grid.DisplayLayout.Bands[0];

            foreach (var band in grid.DisplayLayout.Bands)
            {
                if (band == rootBand) continue;

                band.ColumnFilters[e.Column.Key].FilterConditions.Clear();

                foreach (FilterCondition filterCondition in rootBand.ColumnFilters[e.Column.Key].FilterConditions)
                {
                    band.ColumnFilters[e.Column.Key].FilterConditions.Add(
                        filterCondition.ComparisionOperator, filterCondition.CompareValue);
                }
            }

            grid.AfterRowFilterChanged += grid_AfterRowFilterChanged;
        }

        private void NodeAction(bool collapse)
        {
            grid.BeginUpdate();

            var gridRow = grid.GetRow(ChildRow.First);

            while (gridRow != null)
            {
                if (!gridRow.HiddenResolved)
                {
                    if (collapse)
                    {
                        gridRow.CollapseAll();
                    }
                    else
                    {
                        gridRow.ExpandAll();                        
                    }
                }

                gridRow = gridRow.GetSibling(SiblingRow.Next);
            }

            grid.EndUpdate();            
        }


        private void miExpand_Click(object sender, EventArgs e)
        {
            NodeAction(false);
        }

        private void miCollapse_Click(object sender, EventArgs e)
        {
            NodeAction(true);
        }

        private void miFilter_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Settings.EditorSearchField))
            {
                return;
            }

            foreach (var band in grid.DisplayLayout.Bands)
            {
                band.ColumnFilters[Settings.EditorSearchField].FilterConditions.Add(
                    FilterComparisionOperator.Contains, 
                    textFind);                
            }
        }
    }
}
