using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinToolTip;
using Infragistics.Win.UltraWinTree;
using ButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle;
using ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds;
using ToolTipDisplayStyle = Infragistics.Win.ToolTipDisplayStyle;

namespace Krista.FM.Client.Reports.Common.CommonParamForm
{
    public partial class ReportParamForm : Form
    {
        private const string LblColumn = "Name";
        private const string ValColumn = "Value";
        private const string CHK_FLG = "CHK_FLG";
        private const string CELL_ID = "ID";
        private const string CELL_NAME = "NAME";
        private ParamContainer containerParam = new ParamContainer();

        public int CaptionWidth { get; set; }
        public int FormWidth { get; set; }

        public ReportParamForm()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;

            CaptionWidth = 120;
            FormWidth = 450;

            SetTreeSettings();
        }

        private void UpdateConditionalVisibility(string paramName, object paramValue)
        {
            var relParams = containerParam.lstParams.Where(f => f.Filter != null && f.Filter.FieldName == paramName);

            foreach (var paramInfo in relParams)
            {
                var filterInfo = paramInfo.Filter;
                var nodeCurrent = FindNode(tvParams.Nodes, paramInfo.Name);
                nodeCurrent.Visible = filterInfo.FieldValue.Contains(paramValue);
            }
        }

        public bool ShowForm(IWin32Window parent, ParamContainer container)
        {
            containerParam = container;

            foreach (var param in containerParam.lstParams)
            {
                AddPropertyNode(param, param.ParentName);
            }

            UpdateParams(tvParams.Nodes);

            foreach (var link in containerParam.links)
            {
                var rec = link.Value.FirstOrDefault();
                var nodeParent = FindNode(tvParams.Nodes, rec.Key);
                var value = nodeParent.Cells[ValColumn].Value;
                UpdateLink(link, value);
            }

            foreach (var param in containerParam.GetParams())
            {
                UpdateConditionalVisibility(param.Key, param.Value);
            }

            RefreshHeight();
            var height = tvParams.Height + btnCreate.Height + 40;

            MinimumSize = new Size(FormWidth, height);
            MaximumSize = new Size(FormWidth, height);

            if (ShowDialog(null) == DialogResult.OK)
            {
                UpdateParams(tvParams.Nodes);
                return true;
            }

            return false;
        }

        private void SetTreeSettings()
        {
            formManager.FormStyleSettings.FormDisplayStyle = FormDisplayStyle.RoundedFixed;
            tvParams.Override.UseEditor = DefaultableBoolean.True;
            tvParams.Override.ShowEditorButtons = ShowEditorButtons.Always;
            tvParams.Override.LabelEdit = DefaultableBoolean.True;
            tvParams.Override.CellClickAction = Infragistics.Win.UltraWinTree.CellClickAction.EditCell;
            tvParams.Nodes.Clear();
            tvParams.ColumnSettings.BorderStyleCell = UIElementBorderStyle.Solid;
            tvParams.ColumnSettings.CellAppearance.BorderColor = SystemColors.GradientActiveCaption;
            tvParams.ColumnSettings.LabelPosition = NodeLayoutLabelPosition.None;
            tvParams.ColumnSettings.AutoFitColumns = AutoFitColumns.ResizeAllColumns;
            var rootColumns = tvParams.ColumnSettings.RootColumnSet;
            var colName = rootColumns.Columns.Add(LblColumn);
            colName.DataType = typeof(string);
            colName.LayoutInfo.PreferredCellSize = new Size(CaptionWidth, 0);
            var colValue = rootColumns.Columns.Add(ValColumn);
            colValue.DataType = typeof(object);
            colValue.LayoutInfo.PreferredCellSize = new Size(Width - colName.LayoutInfo.PreferredCellSize.Width, 0);
            tvParams.DisplayStyle = UltraTreeDisplayStyle.Standard;
            BackColor = GetBackColor1();
            btnCreate.ButtonStyle = UIElementButtonStyle.WindowsVistaButton;
            btnCreate.Appearance.BackColor = GetBackColor1();
            btnCreate.Appearance.BackColor2 = GetBackColor2();
            tvParams.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.OutlookExpress;

            tvParams.MouseEnterElement += tvParams_MouseEnterElement;
        }

        private void tvParams_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            var element = e.Element as UltraTreeNodeCellUIElement;

            if (element == null)
            {
                return;
            }

            tooltipManager.HideToolTip();

            if (element.Cell.Key == LblColumn)
            {
                return;
            }

            var pt = new Point(e.Element.Rect.X, e.Element.Rect.Y);
            var node = tvParams.GetNodeFromPoint(pt);

            if (node == null)
            {
                return;
            }

            var settings = containerParam[node.Key];

            if (settings.ParamType != ReportParamType.Book && settings.ParamType != ReportParamType.List)
            {
                return;
            }

            var cell = node.Cells[ValColumn];
            var text = cell.Text;

            if (cell.EditorComponent != null)
            {
                var combo = cell.EditorComponent as UltraCombo;

                if (combo != null && combo.Text.Length > 0)
                {
                    text = combo.Text;
                }
            }

            if (cell.Editor != null)
            {
                var editor = cell.Editor as EditorWithText;

                if (editor != null && editor.TextBox.Text.Length > 0)
                {
                    text = editor.TextBox.Text;
                }
            }

            tooltipManager.AutoPopDelay = 10000;
            tooltipManager.InitialDelay = 0;
            tooltipManager.DisplayStyle = ToolTipDisplayStyle.Office2007;
            var tipInfo = new UltraToolTipInfo(text, ToolTipImage.Default, String.Empty, DefaultableBoolean.True);
            tooltipManager.SetUltraToolTip(tvParams, tipInfo);

            if (text.Length > 0)
            {
                tooltipManager.ShowToolTip(tvParams);
            }
        }

        private static Color GetBackColor1()
        {
            return Color.FromArgb(218, 235, 255);
        }

        private static Color GetBackColor2()
        {
            return Color.FromArgb(129, 168, 225);
        }

        private void RefreshHeight()
        {
            var height = tvParams.GetNodeCount(true) * tvParams.Nodes[0].Bounds.Height + 10;
            tvParams.Scrollable = Infragistics.Win.UltraWinTree.Scrollbar.Hide;
            tvParams.Height = height;
            tvParams.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
        }

        private static void AddHeaderCell(UltraTreeColumnSet columnSet, UltraTreeNode node, string paramCaption)
        {
            var colName = columnSet.Columns[LblColumn];
            node.SetCellValue(colName, paramCaption);
            var cellName = node.Cells[colName];
            cellName.Appearance.BackColor = GetBackColor1();
            cellName.Appearance.BackColor2 = GetBackColor2();
            cellName.Appearance.BackGradientStyle = GradientStyle.Vertical;
            cellName.Appearance.ForeColor = SystemColors.ControlDarkDark;
            cellName.Appearance.FontData.Bold = DefaultableBoolean.True;
            cellName.Appearance.BorderColor = Color.Black;
        }

        private static UltraTreeNodeCell AddValueCell(UltraTreeColumnSet columnSet, UltraTreeNode node, object paramValue)
        {
            var colValue = columnSet.Columns[ValColumn];
            colValue.AllowCellEdit = AllowCellEdit.Full;
            colValue.DataType = typeof(string);
            node.SetCellValue(colValue, paramValue);
            var cellValue = node.Cells[colValue];
            cellValue.Appearance.BackColor = Color.White;
            cellValue.Appearance.BorderColor = Color.Black;
            cellValue.AllowEdit = AllowCellEdit.Full;
            return cellValue;
        }

        private void AddPropertyNode(ParamInfo settings, string parentNode)
        {
            var columnSet = tvParams.ColumnSettings.RootColumnSet;
            var node = String.IsNullOrEmpty(parentNode)
                           ? tvParams.Nodes.Add(settings.Name)
                           : FindNode(tvParams.Nodes, parentNode).Nodes.Add(settings.Name);
            node.Override.UseEditor = DefaultableBoolean.True;
            node.Override.TipStyleNode = TipStyleNode.Hide;

            if (String.IsNullOrEmpty(settings.Description))
            {
                settings.Description = ReportParamCaptions.GetParamDefaultCaption(settings.Name, settings.EnumObj);
            }

            AddHeaderCell(columnSet, node, settings.Description);
            var cellValue = AddValueCell(columnSet, node,
                settings.ParamType != ReportParamType.List ?
                settings.DefaultValue :
                String.Empty);

            if (settings.ReadOnly)
            {
                cellValue.AllowEdit = AllowCellEdit.Disabled;
            }

            switch (settings.ParamType)
            {
                case ReportParamType.Flag:
                    var checkEditor = new UltraCheckEditor();
                    cellValue.EditorComponent = checkEditor;
                    checkEditor.GlyphInfo = UIElementDrawParams.Office2010CheckBoxGlyphInfo;
                    break;
                case ReportParamType.Number:
                    var numEditor = new UltraNumericEditor();
                    cellValue.EditorComponent = numEditor;
                    numEditor.NumericType = NumericType.Decimal;
                    numEditor.DisplayStyle = EmbeddableElementDisplayStyle.Office2010;
                    numEditor.MaskInput = settings.MaskInput;
                    numEditor.MaskDisplayMode = MaskMode.IncludeLiterals;
                    numEditor.MaskClipMode = MaskMode.IncludeLiterals;
                    numEditor.PromptChar = ' ';
                    numEditor.SpinButtonDisplayStyle = ButtonDisplayStyle.Always;
                    numEditor.SpinButtonAlignment = ButtonAlignment.Right;
                    break;
                case ReportParamType.DateValue:
                    var editor = new UltraDateTimeEditor
                    {
                        DropDownButtonAlignment = ButtonAlignment.Right,
                        DropDownButtonDisplayStyle = ButtonDisplayStyle.Always,
                        DisplayStyle = EmbeddableElementDisplayStyle.Office2010,
                        SpinIncrement = { Days = 1 },
                        SpinButtonDisplayStyle = ButtonDisplayStyle.Always,
                        SpinButtonAlignment = ButtonAlignment.Right,
                    };

                    cellValue.EditorComponent = editor;
                    break;
                case ReportParamType.List:
                    var combo = new UltraCombo
                    {
                        DropDownButtonDisplayStyle = ButtonDisplayStyle.Always,
                        DisplayStyle = EmbeddableElementDisplayStyle.Office2010,
                        AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest,
                        AutoSuggestFilterMode = AutoSuggestFilterMode.Contains,
                        Tag = settings.Name,
                        MaxDropDownItems = 20
                    };

                    if (settings.EnumObj != null)
                    {
                        combo.DropDownStyle = UltraComboStyle.DropDownList;
                    }

                    if (settings.MultiSelect)
                    {
                        combo.CheckedListSettings.CheckStateMember = CHK_FLG;
                        combo.CheckedListSettings.EditorValueSource = EditorWithComboValueSource.CheckedItems;
                        combo.CheckedListSettings.ListSeparator = ", ";
                        combo.CheckedListSettings.ItemCheckArea = ItemCheckArea.Item;
                    }

                    var dt = GetListData(settings);
                    settings.Table = dt;
                    combo.InitializeLayout += cb_InitializeLayout;
                    combo.DataSource = dt;
                    combo.DisplayMember = String.IsNullOrEmpty(settings.DisplayFieldName) ? 
                        CELL_NAME : 
                        settings.DisplayFieldName;
                    combo.Appearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    combo.BeforeDropDown += combo_BeforeDropDown;
                    combo.AfterCloseUp += cb_AfterCloseUp;
                    SetListValue(cellValue, combo, settings);
                    cellValue.EditorComponent = combo;
                    break;
                case ReportParamType.Book:
                    var book = new EditorWithText();
                    cellValue.Editor = book;

                    var button = new DropDownEditorButton
                    {
                        ButtonStyle = UIElementButtonStyle.WindowsVistaButton,
                        Appearance =
                        {
                            BackColor = GetBackColor1(),
                            BackColor2 = GetBackColor2()
                        }
                    };

                    book.ButtonsRight.Add(button);
                    book.Tag = settings;

                    if (settings.DefaultValue != null)
                    {
                        cellValue.Value = settings.BookInfo.GetText(settings.DefaultValue);
                        containerParam.SetParamValue(settings.Name, settings.DefaultValue);
                    }

                    book.KeyDown += editor_KeyDown;
                    book.BeforeEnterEditMode += editor_BeforeEnterEditMode;
                    book.BeforeExitEditMode += editor_BeforeExitEditMode;
                    book.EditorButtonClick += editor_EditorButtonClick;
                    break;
            }
        }

        void combo_BeforeDropDown(object sender, CancelEventArgs args)
        {
            var combo = (UltraCombo)sender;

            if (combo == null)
            {
                return;
            }

            var settings = containerParam[Convert.ToString(combo.Tag)];

            if (settings.NotUncheckBeforeSelect)
            {
                return;
            }

            var itemList = (ICheckedItemList)combo;
            for (var i = 0; i < combo.Rows.Count; i++)
                itemList.SetCheckState(i, CheckState.Unchecked);
        }

        private void cb_AfterCloseUp(object sender, EventArgs e)
        {
        }

        private void cb_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            var combo = (UltraCombo)sender;
            var band = e.Layout.Bands[0];
            band.ColHeadersVisible = false;
            band.Columns[CELL_ID].Hidden = true;

            band.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            var width = 0;
            var settings = containerParam[Convert.ToString(combo.Tag)];

            band.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

            band.Override.RowSizing = RowSizing.AutoFree;
            band.Layout.Override.CellMultiLine = DefaultableBoolean.True;

            foreach (var column in band.Columns)
            {
                var visible = column.Key == CHK_FLG || column.Key == CELL_NAME || 
                    (settings.Columns != null && settings.Columns.Contains(column.Key));
                column.Hidden = !visible;
                column.CellMultiLine = DefaultableBoolean.True;
                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, false);
                
                column.Width = Math.Min(column.Width, Screen.PrimaryScreen.Bounds.Width / 2);

                if (!column.Hidden)
                {
                    width += column.Width;
                }
            }

            width += 20;

            combo.DropDownWidth = width;
            combo.PreferredDropDownSize = new Size(width, 0);

            if (band.Columns.Exists(CHK_FLG))
            {
                band.Columns[CHK_FLG].Width = 20;
                band.Columns[CHK_FLG].Header.VisiblePosition = 0;
            }
        }

        void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            var editor = (EditorWithText)sender;
            var paramSettings = (ParamInfo)editor.Tag;
            var bookInfo = paramSettings.BookInfo;  
              
            if (bookInfo == null || String.IsNullOrEmpty(bookInfo.EditorSearchField))
            {
                return;
            }

            if (bookInfo.CheckSearchMask(editor.Value))
            {
                var info = bookInfo.GetSearchInfo(editor.Value);
                editor.Value = info.Caption;
                containerParam.SetParamValue(paramSettings.Name, info.Values);
            }
        }

        void editor_BeforeExitEditMode(object sender, Infragistics.Win.BeforeExitEditModeEventArgs e)
        {
            var editor = sender as EmbeddableEditorBase;
            OnEditModeChanged(editor, false);
        }

        void editor_BeforeEnterEditMode(object sender, CancelEventArgs e)
        {
            var editor = sender as EmbeddableEditorBase;
            OnEditModeChanged(editor, true);
        }

        private void OnEditModeChanged(EmbeddableEditorBase editor, bool entering)
        {
            var embeddableElement = editor != null ? editor.ElementBeingEdited : null;

            if (embeddableElement == null)
            {
                return;
            }

            var ancestor = embeddableElement.GetAncestor(typeof(UltraTreeNodeCellUIElement));
            var cellElement = ancestor as UltraTreeNodeCellUIElement;

            if (cellElement != null)
            {
                cellElement.Column.AllowCellEdit = entering ? AllowCellEdit.ReadOnly : AllowCellEdit.Full;
            }
        }

        private void editor_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            var editor = (EditorWithText) sender;
            var paramSettings = (ParamInfo)editor.Tag;
            var bookInfo = paramSettings.BookInfo;

            var initialValue = paramSettings.NotUncheckBeforeSelect ? 
                containerParam.GetParamValue(paramSettings.Name) :
                String.Empty;

            var formParam = new MultiCheckForm
                                {
                                    Settings = bookInfo,
                                    SourceList = bookInfo.CreateSourceList(),
                                    DataList = bookInfo.CreateDataList(),
                                    SelectedValues = initialValue,
                                };

            formParam.LoadBookData();

            if (formParam.ShowDialog() == DialogResult.OK)
            {
                editor.Value = bookInfo.GetText(formParam.SelectedValues);
                var value = String.IsNullOrEmpty(formParam.SelectedValues)
                                ? ReportConsts.UndefinedKey
                                : formParam.SelectedValues;
                containerParam.SetParamValue(paramSettings.Name, value);
            }
        }

        private static UltraTreeNode FindNode(TreeNodesCollection nodes, string key)
        {
            return nodes.Cast<UltraTreeNode>()
                .Aggregate<UltraTreeNode, UltraTreeNode>(null, (current, curNode) => 
                    nodes.IndexOf(key) >= 0 ? nodes[key] : current);
        }

        private void UpdateParams(TreeNodesCollection nodes)
        {
            foreach (var curNode in nodes)
            {
                var setting = containerParam[curNode.Key];
                var value = curNode.Cells[ValColumn].Value;

                if (setting.ParamType == ReportParamType.Number)
                {
                    var separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                    var strValue = Convert.ToString(value);
                    if (strValue.IndexOf(separator) >= 0)
                    {
                        value = strValue.TrimEnd('0').TrimEnd(separator.ToCharArray());
                    }

                    if (Convert.ToString(value).Length == 0)
                    {
                        containerParam.SetParamValue(curNode.Key, 0);
                        continue;
                    }
                }

                if (setting.ParamType == ReportParamType.Book)
                {
                    if (Convert.ToString(value).Length == 0)
                    {
                        containerParam.SetParamValue(curNode.Key, ReportConsts.UndefinedKey);
                    }

                    continue;
                }

                if (setting.ParamType == ReportParamType.DateValue)
                {
                    if (value == null)
                    {
                        value = DateTime.Now;
                    }

                    value = Convert.ToDateTime(value).ToShortDateString();
                }

                if (setting.ParamType != ReportParamType.List)
                {
                    containerParam.SetParamValue(curNode.Key, value);
                }
                else
                {
                    var combo = (UltraCombo)curNode.Cells[ValColumn].EditorComponent;

                    if (combo == null)
                    {
                        return;
                    }

                    if (setting.MultiSelect)
                    {
                        var lstVals = new List<string>();

                        for (var i = 0; i < combo.CheckedRows.Count; i++)
                        {
                            var row = combo.CheckedRows[i];
                            lstVals.Add(Convert.ToString(row.Cells[CELL_ID].Value));
                        }

                        containerParam.SetParamValue(curNode.Key, String.Join(",", lstVals.ToArray()));
                    }
                    else
                    {
                        var text = Convert.ToString(curNode.Cells[ValColumn].Value);

                        if (!String.IsNullOrEmpty(text))
                        {
                            var settings = containerParam[Convert.ToString(combo.Tag)];
                            var fieldFind = String.IsNullOrEmpty(settings.DisplayFieldName) ?
                                CELL_NAME :
                                settings.DisplayFieldName;

                            var row = combo.Rows
                                .Where(f => f.Cells[fieldFind].Value.ToString() == text)
                                .FirstOrDefault();
                            containerParam.SetParamValue(curNode.Key, row.Cells[CELL_ID].Value);
                        }
                        else
                        {
                            containerParam.SetParamValue(curNode.Key, ReportConsts.UndefinedKey);
                        }
                    }
                }

                UpdateParams(curNode.Nodes);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void tvParams_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!e.IsValid)
            {
                return;
            }

            var paramName = e.Node.Key;
            var relatedParams = containerParam.links.Where(f => f.Value.ContainsKey(paramName));
            var paramInfo = containerParam[paramName];

            foreach (var linkParam in relatedParams)
            {
                var newValue = e.CurrentValue;
 
                if (!String.IsNullOrEmpty(paramInfo.DisplayFieldName))
                {
                    var combo = (UltraCombo)e.Cell.EditorComponent;
                    var row = combo.Rows
                        .Where(f => f.Cells[paramInfo.DisplayFieldName].Value.ToString() == Convert.ToString(newValue))
                        .FirstOrDefault();

                    if (row != null)
                    {
                        newValue = row.Cells[CELL_ID].Value;
                    }
                }

                UpdateLink(linkParam, newValue);
            }

            UpdateConditionalVisibility(paramName, e.CurrentValue);
        }

        private void UpdateLink(KeyValuePair<string, Dictionary<string, UndercutParamBase>> linkParam, object value)
        {
            var obj = linkParam.Value.FirstOrDefault().Value;
            obj.ParentValue = value;
            var newValue = obj.NewValue;
            containerParam.SetParamValue(linkParam.Key, newValue);
            var node = FindNode(tvParams.Nodes, linkParam.Key);

            if (newValue == null)
            {
                newValue = String.Empty;
            }

            node.Cells[ValColumn].Value = newValue;
            var settings = containerParam[linkParam.Key];

            if (settings.ParamType == ReportParamType.List)
            {
                var combo = (UltraCombo) node.Cells[ValColumn].EditorComponent;
                combo.DataSource = settings.Table;
            }

            if (settings.ParamType == ReportParamType.Book)
            {
                node.Cells[ValColumn].Value = settings.BookInfo.GetText(newValue);
            }
        }

        private DataTable GetListData(ParamInfo settings)
        {
            return settings.Table == null ?
                CreateList(settings.Values, settings.MultiSelect, Convert.ToString(settings.DefaultValue)) :
                CreateList(settings.Table, settings.MultiSelect, Convert.ToString(settings.DefaultValue));
        }

        private static void SetListValue(UltraTreeNodeCell cellValue, UltraCombo combo, ParamInfo settings)
        {
            var dt = settings.Table;

            if (!settings.MultiSelect)
            {
                var rowData = dt.Rows.Count > 0 ? dt.Rows[0] : null;

                if (settings.DefaultValue != null)
                {
                    var value = Convert.ToString(settings.DefaultValue);

                    if (value.Length > 0)
                    {
                        var quote = dt.Columns[CELL_ID].DataType != typeof(string) ? String.Empty : "'";

                        var rowsData = dt.Select(String.Format("{0} = {2}{1}{2}",
                            CELL_ID,
                            settings.DefaultValue,
                            quote));

                        if (rowsData.Length > 0)
                        {
                            rowData = rowsData[0];
                        }                        
                    }
                }

                if (rowData != null)
                {
                    if (settings.AutoFirstValue || settings.DefaultValue != null)
                    {
                        combo.SetInitialValue(rowData[CELL_ID], Convert.ToString(rowData[CELL_NAME]));
                        cellValue.Node.SetCellValue(cellValue.Node.Cells[ValColumn].Column, rowData[CELL_NAME]);
                    }
                }
            }
            else
            {
                var text = combo.Text;

                if (settings.DefaultValue != null)
                {
                    var rowsData = dt.Select(String.Format("{0} = true", CHK_FLG));

                    if (rowsData.Length > 0)
                    {
                        var values = new string[rowsData.Length];

                        for (var i = 0; i < rowsData.Length; i++)
                        {
                            values[i] = Convert.ToString(rowsData[i][CELL_NAME]);
                        }

                        text = String.Join(",", values);
                    }
                }

                cellValue.Node.SetCellValue(cellValue.Node.Cells[ValColumn].Column, text);
                
                var itemList = (ICheckedItemList)combo;

                for (var i = 0; i < combo.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(combo.Rows[i].Cells[CHK_FLG].Value))
                    {
                        itemList.SetCheckState(i, CheckState.Checked);
                    }
                }
            }
        }

        private static List<string> SplitDefValues(string values)
        {
            return String.IsNullOrEmpty(values)
                       ? new List<string>()
                       : new List<string>(values.Split(','));
        }

        private DataTable CreateList(List<object> values, bool multiSelect, string defValues)
        {
            var tbl = new DataTable("List");

            if (multiSelect)
            {
                tbl.Columns.Add(CHK_FLG, typeof(bool));
            }

            tbl.Columns.Add(CELL_ID, typeof(string));
            tbl.Columns.Add(CELL_NAME, typeof(string));

            var defList = SplitDefValues(defValues);
            var counter = 0;

            foreach (var value in values)
            {
                var row = tbl.Rows.Add();
                var key = Convert.ToString(counter);
                row[CELL_ID] = key;
                row[CELL_NAME] = value;

                if (multiSelect)
                {
                    row[CHK_FLG] = defList.Contains(key);
                }

                counter++;
            }

            return tbl;
        }

        private static DataTable CreateList(DataTable tbl, bool multiSelect, string defValues)
        {
            if (multiSelect && !tbl.Columns.Contains(CHK_FLG))
            {
                tbl.Columns.Add(CHK_FLG, typeof(bool));
            }

            var defList = SplitDefValues(defValues);

            foreach (DataRow row in tbl.Rows)
            {
                var key = Convert.ToString(row[CELL_ID]);

                if (multiSelect)
                {
                    row[CHK_FLG] = defList.Contains(key);
                }
            }

            return tbl;
        }
    }
}
