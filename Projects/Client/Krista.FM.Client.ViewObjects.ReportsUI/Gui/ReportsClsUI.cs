using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Excel;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.ReportsUI.Commands;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Reports;
using Krista.FM.Client.Reports.Common.Commands;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public class ReportsClsUI : AssociatedClsUI
    {
        protected IFReportMenu reportMenu;

        public ReportsClsUI(IEntity dataObject)
            : this(dataObject, dataObject.ObjectKey)
		{
		}

        public ReportsClsUI(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Index = 2;
            Caption = "Kлассификаторы";
            clsClassType = ClassTypes.clsBridgeClassifier;
        }

        #region внутренние переменные и свойства

        internal DataSet MasterHistory
        {
            get; set;
        }

        internal DataSet DetailHistory
        {
            get; set;
        }

        private const string fileFilterCaption = "Выбрать из файла";

        private const string BufferFilterCaption = "Вставить из буфера";

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            MasterHistory = new DataSet();
            DetailHistory = new DataSet();

            UltraToolbar utbMain = vo.ugeCls.utmMain.Toolbars["utbMain"];
            ButtonTool delRow = (ButtonTool)vo.ugeCls.utmMain.Tools["DeleteSelectedRows"];
            delRow.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Document_Delete_icon;
            utbMain.Tools.Remove(delRow);
            utbMain.Tools.Add(delRow);
            vo.ugeCls.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = true;

            AbstractCommand command = new ShowHistoryCommand();
            StateButtonTool buttonTool = CommandService.AttachToolbarTool<StateButtonTool>
                (command, vo.ugeCls.utmMain.Toolbars["utbColumns"]);
            command.Owner = buttonTool;
            buttonTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.history_icon;

            vo.ugeCls.utmMain.Tools["SaveToExcel"].SharedProps.Visible = false;
            vo.ugeCls.ugData.BeforeCustomRowFilterDialog += ugData_BeforeCustomRowFilterDialog;
            vo.ugeCls.OnBeforeRowFilterDropDownPopulateEventHandler += ugFilter_BeforeRowFilterDropDownPopulate;
            vo.ugeCls.ugFilter.AfterCellListCloseUp += ugFilter_AfterCellListCloseUp;

            // создание меню отчетов
            ImageList il = new ImageList();
            il.Images.Add(Properties.Resources.Document_Microsoft_Word_icon);
            il.Images.Add(Properties.Resources.Document_Microsoft_Excel_icon);

            ReportMenuParams reportMenuParams = new ReportMenuParams();
            reportMenuParams.tb = vo.ugeCls.utmMain.Toolbars[1];
            reportMenuParams.tbManager = vo.ugeCls.utmMain;
            reportMenuParams.il = il;
            reportMenu = new IFReportMenu(reportMenuParams);
            reportMenu.scheme = Workplace.ActiveScheme;
            reportMenu.window = Workplace.WindowHandle;
            reportMenu.operationObj = Workplace.OperationObj;
        }

        void ugFilter_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            if (e.Cell.Text == fileFilterCaption)
            {
                string fileName = e.Cell.Column.Key;
                // показываем диалог с выбором файла, читаем данные, строим фильтр
                if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.txt, false, ref fileName))
                {
                    object [] filterParams = null;
                    GetTextFilterData(fileName, ref filterParams);
                    vo.ugeCls.ugFilter.DisplayLayout.Bands[0].ColumnFilters[e.Cell.Column.Key].LogicalOperator =
                        FilterLogicalOperator.Or;
                    foreach (object param in filterParams)
                    {
                        vo.ugeCls.ugFilter.DisplayLayout.Bands[0].ColumnFilters[e.Cell.Column.Key].FilterConditions.Add(
                            FilterComparisionOperator.Equals, param);
                    }
                    vo.ugeCls.BurnRefreshDataButton(filterParams.Length > 0);
                }
            }
            if (string.Compare(e.Cell.Text, BufferFilterCaption, true) == 0)
            {
                SetBufferFilter(vo.ugeCls, e.Cell.Column.Key);
                Refresh();
            }
        }

        private void SetBufferFilter(UltraGridEx gridEx, string columnName)
        {
            string filterString = Clipboard.GetText();
            if (!string.IsNullOrEmpty(filterString))
            {
                string[] filterConditions = null;
                if (filterString.Contains(";"))
                {
                    filterConditions = filterString.Split(';');
                }
                else if (filterString.Contains(","))
                {
                    filterConditions = filterString.Split(',');
                }
                else
                    filterConditions = new string[] { filterString };
                if (filterConditions != null)
                {
                    gridEx.ugFilter.DisplayLayout.Bands[0].ColumnFilters[columnName].LogicalOperator =
                        FilterLogicalOperator.Or;
                    foreach (var filterCondition in filterConditions)
                    {
                        gridEx.ugFilter.DisplayLayout.Bands[0].ColumnFilters[columnName].FilterConditions.Add(
                            FilterComparisionOperator.Equals, filterCondition.Trim());
                    }
                    vo.ugeCls.BurnRefreshDataButton(filterConditions.Length > 0);
                }
            }
        }

        void ugData_BeforeCustomRowFilterDialog(object sender, BeforeCustomRowFilterDialogEventArgs e)
        {
            
        }

        private void ugFilter_BeforeRowFilterDropDownPopulate(object sender, BeforeRowFilterDropDownPopulateEventArgs e)
        {
            if (!(e.Column.Key == "OGRN" || e.Column.Key == "INN" || e.Column.Key == "INN20" ||
                e.Column.Key == "OGRNIP"))
                return;

            GridColumnsStates states = this.ugeCls_OnGetGridColumnsState(vo.ugeCls);
            if (!states.ContainsKey(e.Column.Key))
                return;

            GridColumnState gcs = states[e.Column.Key];
            if (!gcs.CalendarColumn)
                e.ValueList.ValueListItems.Add(null, fileFilterCaption);
            if (!gcs.CalendarColumn)
                e.ValueList.ValueListItems.Add(null, BufferFilterCaption);
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);
            SetupMasterToolbar();
            CollapceGroups();
            if (vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns.Exists("Posl"))
            {
                vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["Posl"].Hidden = true;
            }

            UltraStatusPanel panel = Workplace.MainStatusBar.Panels["Rows"];
            panel.Text = string.Format("{0} записей из {1}", CurrentRecordsCount, AllRecordsCount);
        }

        protected override void AfterLoadDetailData(ref DataSet detail)
        {
            base.AfterLoadDetailData(ref detail);
            SetupDetailToolbar();
            AddStatusColumns(activeDetailGrid);
            activeDetailGrid.ugData.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
        }

        protected override void DetailGridSetup(UltraGridEx ugeDetail)
        {
            base.DetailGridSetup(ugeDetail);
            ugeDetail.ugData.DisplayLayout.Override.CellMultiLine = DefaultableBoolean.False;
            ugeDetail.StateRowEnable = false;
        }

        protected void SetupMasterToolbar()
        {
            vo.ugeCls.ugData.DisplayLayout.AddNewBox.Hidden = true;
            vo.ugeCls.utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;
            ButtonTool delRow = (ButtonTool)vo.ugeCls.utmMain.Tools["DeleteSelectedRows"];
            delRow.SharedProps.ToolTipText = "Удалить запись";
            vo.ugeCls.utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;
        }

        protected void SetupDetailToolbar()
        {
            activeDetailGrid.ugData.DisplayLayout.AddNewBox.Hidden = true;
            activeDetailGrid.utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;
            UltraToolbar utbMain = activeDetailGrid.utmMain.Toolbars["utbMain"];
            ButtonTool delRow = (ButtonTool)activeDetailGrid.utmMain.Tools["DeleteSelectedRows"];
            delRow.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Document_Delete_icon;
            utbMain.Tools.Remove(delRow);
            utbMain.Tools.Add(delRow);

            ButtonTool clearData = (ButtonTool)activeDetailGrid.utmMain.Tools["ClearCurrentTable"];
            utbMain.Tools.Remove(clearData);
            utbMain.Tools.Add(clearData);
            clearData.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Table_delete_icon;
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);

            e.Layout.Override.WrapHeaderText = DefaultableBoolean.True;

            foreach (string group in vo.ugeCls.Groups.Keys)
            {
                e.Layout.Bands[0].Groups.Add(group, group);
            }

            foreach (GridColumnState state in vo.ugeCls.CurrentStates.Values)
            {
                if (e.Layout.Bands[0].Groups.Exists(state.GroupName))
                {
                    string columnName = vo.ugeCls.GetGridColumnName(state.ColumnName);
                    e.Layout.Bands[0].Columns[columnName].Group = e.Layout.Bands[0].Groups[state.GroupName];
                    e.Layout.Bands[0].Columns[columnName].CellMultiLine = DefaultableBoolean.True;
                }
            }
            AddStatusGroup(vo.ugeCls);

            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                if (column.DataType == typeof(String))
                {
                    column.Header.Tag = CheckState.Checked;
                }
            }
        }

        public override void ugeDetail_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeDetail_OnGridInitializeLayout(sender, e);

            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                if (column.DataType == typeof(String))
                {
                    column.Header.Tag = CheckState.Checked;
                }
            }
        }

        private List<string> collapsedGroups = new List<string>();

        protected override void ugeCls_OnCreateUIElement(object sender, UIElement parent)
        {
            base.ugeCls_OnCreateUIElement(sender, parent);

            if (parent is HeaderUIElement)
            {
                HeaderUIElement headerUIElement = (HeaderUIElement)parent;
                HeaderBase aHeader = ((HeaderUIElement)parent).Header;
                if (aHeader.Group != null && aHeader.Column == null)
                {
                    CreateGroupButton(headerUIElement);
                }
            }
        }

        private void ButtonUIElementElementClick(object sender, UIElementEventArgs e)
        {
            ImageAndTextButtonUIElement button = e.Element as ImageAndTextButtonUIElement;
            if (button == null)
                return;
            GroupHeader groupHeader = (GroupHeader)button.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(GroupHeader));
            HeaderUIElement header = (HeaderUIElement)groupHeader.GetUIElement();
            bool collapseGroup = !collapsedGroups.Contains(groupHeader.Group.Key);
            vo.ugeCls.CollapseGroup(groupHeader.Group.Key, collapseGroup);
            if (collapseGroup)
            {
                collapsedGroups.Add(groupHeader.Group.Key);
                CreateGroupButton(header);
            }
            else
            {
                collapsedGroups.Remove(groupHeader.Group.Key);
                CreateGroupButton(header);
            }
        }

        private void CreateGroupButton(HeaderUIElement groupHeader)
        {
            ImageAndTextButtonUIElement buttonElement = (ImageAndTextButtonUIElement)groupHeader.GetDescendant(typeof(ImageAndTextButtonUIElement));
            if (buttonElement != null)
            {
                groupHeader.ChildElements.Remove(buttonElement);
                buttonElement.Dispose();
            }

            if (string.IsNullOrEmpty(groupHeader.Header.Group.Header.Caption))
                return;

            string groupKey = groupHeader.Header.Group.Key;
            TextUIElement aTextUIElement = (TextUIElement)groupHeader.GetDescendant(typeof(TextUIElement));
            ImageAndTextButtonUIElement collapceButtonUIElement = new ImageAndTextButtonUIElement(groupHeader);
            groupHeader.ChildElements.Add(collapceButtonUIElement);
            collapceButtonUIElement.Rect = new Rectangle(groupHeader.Rect.X, groupHeader.Rect.Y, groupHeader.Rect.Height, groupHeader.Rect.Height);
            collapceButtonUIElement.Image = collapsedGroups.Contains(groupKey) ?
                Properties.Resources.toggle_small_expand_icon :
                Properties.Resources.minus_small_white_icon;
            collapceButtonUIElement.ElementClick += ButtonUIElementElementClick;
            aTextUIElement.Rect = new Rectangle(collapceButtonUIElement.Rect.Right + 3,
                aTextUIElement.Rect.Y, groupHeader.Rect.Width - collapceButtonUIElement.Rect.Width, aTextUIElement.Rect.Height);
        }

        protected override void AddFilter()
        {
            dataQuery += " and Last = 1";
        }

        #region заполнение истории

        private long activeMasterRowId;

        private bool showDetailHistory = false;

        internal void ShowMasterHistory(DataRow masterRow, bool showhistory)
        {
            showDetailHistory = showhistory;
            vo.ugeCls.ServerFilterEnabled = !showhistory;
            if (showhistory)
            {
                using (IDataUpdater du = ActiveDataObj.GetDataUpdater("ID = ? or ParentID = ?", null,
                    new DbParameterDescriptor("p0", masterRow["ID"]),
                    new DbParameterDescriptor("p1", masterRow["ID"])))
                {
                    activeMasterRowId = Convert.ToInt64(masterRow["ID"]);
                    DataSet ds = new DataSet();
                    du.Fill(ref ds);
                    MasterHistory = ds.Copy();
                    vo.ugeCls.SetGridDataSource(MasterHistory, false);
                    vo.ugeCls.ugData.Rows[0].Activate();
                }
            }
            else
            {
                vo.ugeCls.SetGridDataSource(dsObjData, false);
                UltraGridRow activeRow = UltraGridHelper.FindGridRow(vo.ugeCls.ugData, "ID", activeMasterRowId);
                if (activeRow != null)
                {
                    activeRow.Activate();
                    vo.ugeCls.ugData.ActiveRowScrollRegion.FirstRow = activeRow;
                }
            }
            ShowDetailHistory(showhistory);
            vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["Posl"].Hidden = !showhistory;
            SetupMasterToolbar();
            CollapceGroups();
        }

        private void AddStatusGroup(UltraGridEx gridEx)
        {
            UltraGridGroup statusGroup = null;
            if (!gridEx.ugData.DisplayLayout.Bands[0].Groups.Exists("Status"))
            {
                statusGroup = gridEx.ugData.DisplayLayout.Bands[0].Groups.Add("Status", string.Empty);
                statusGroup.Header.VisiblePosition = 0;
                UltraGridColumn column = gridEx.ugData.DisplayLayout.Bands[0].Columns.Add("Status", "Статус");
                UltraGridHelper.SetLikelyImageColumnsStyle(column, -1);
                statusGroup.Columns.Add(column);

                column = gridEx.ugData.DisplayLayout.Bands[0].Columns.Add("Posl");
                column.Hidden = true;
                UltraGridHelper.SetLikelyCheckBoxColumnsStyle(column, -1);
                statusGroup.Columns.Add(column);
            }

            if (gridEx.ServerFilterEnabled)
            {
                if (!gridEx.ugFilter.DisplayLayout.Bands[0].Groups.Exists("Status"))
                {
                    statusGroup = gridEx.ugFilter.DisplayLayout.Bands[0].Groups.Add("Status");
                    UltraGridColumn column = gridEx.ugFilter.DisplayLayout.Bands[0].Columns.Add("Status", "Статус");
                    UltraGridHelper.SetLikelyImageColumnsStyle(column, -1);
                    statusGroup.Columns.Add(column);

                    column = gridEx.ugFilter.DisplayLayout.Bands[0].Columns.Add("Posl");
                    UltraGridHelper.SetLikelyCheckBoxColumnsStyle(column, -1);
                    column.Hidden = true;
                    statusGroup.Columns.Add(column);
                    statusGroup.Header.VisiblePosition = 0;
                }
            }
        }

        private void AddStatusColumns(UltraGridEx gridEx)
        {
            gridEx.StateRowEnable = false;
            if (!gridEx.ugData.DisplayLayout.Bands[0].Columns.Exists("Status"))
            {
                UltraGridColumn column = gridEx.ugData.DisplayLayout.Bands[0].Columns.Add("Status", "Статус");
                UltraGridHelper.SetLikelyImageColumnsStyle(column, -1);
                column.Header.VisiblePosition = 0;
            }

            if (!gridEx.ugData.DisplayLayout.Bands[0].Columns.Exists("Posl"))
            {
                UltraGridColumn column = gridEx.ugData.DisplayLayout.Bands[0].Columns.Add("Posl");
                UltraGridHelper.SetLikelyCheckBoxColumnsStyle(column, -1);
                column.Header.VisiblePosition = 1;
                column.Hidden = !showDetailHistory;
            }
        }

        internal void ShowDetailHistory(bool showhistory)
        {
            activeDetailGrid.ugData.DisplayLayout.Bands[0].Columns["Posl"].Hidden = !showhistory;
            SetupDetailToolbar();
        }

        #endregion

        public override void UpdateToolbar()
        {
            base.UpdateToolbar();
            vo.ugeCls.utmMain.Tools["SetHierarchy"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["ShowDependedData"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["btnMergingDuplicates"].SharedProps.Visible = false;
        }

        private void CollapceGroups()
        {
            foreach (UltraGridGroup group in vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups)
            {
                vo.ugeCls.CollapseGroup(group.Key, true);
                collapsedGroups.Add(group.Key);
            }
        }

        public override void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            base.ugeCls_OnInitializeRow(sender, e);

            UltraGridRow row = e.Row;
            bool last = Convert.ToInt32(row.Cells["Last"].Value) == 1;
            int status = Convert.ToInt32(row.Cells["RefStatus"].Value);
            if (row.Cells.Exists("Status"))
            {
                switch (status)
                {
                    case 1:
                        row.Cells["Status"].Appearance.Image = Properties.Resources.no_edit_icon;
                        row.Cells["Status"].ToolTipText = "Значение не изменилось";
                        break;
                    case 2:
                        row.Cells["Status"].Appearance.Image = Properties.Resources.add_icon;
                        row.Cells["Status"].ToolTipText = "Добавлено значение";
                        break;
                    case 3:
                        row.Cells["Status"].Appearance.Image = Properties.Resources.edit_icon;
                        row.Cells["Status"].ToolTipText = "Изменено значение";
                        break;
                }
            }
            if (row.Cells.Exists("Posl"))
            {
                row.Cells["Posl"].Value = last;
            }
        }

        protected override void InitializeDetailRow(object sender, InitializeRowEventArgs e)
        {
            base.InitializeDetailRow(sender, e);

            UltraGridRow row = e.Row;
            bool last = Convert.ToInt32(row.Cells["Last"].Value) == 1;
            int status = Convert.ToInt32(row.Cells["RefStatus"].Value);
            if (row.Cells.Exists("Status"))
            {
                switch (status)
                {
                    case 1:
                        row.Cells["Status"].Appearance.Image = Properties.Resources.no_edit_icon;
                        row.Cells["Status"].ToolTipText = "Значение не изменилось";
                        break;
                    case 2:
                        row.Cells["Status"].Appearance.Image = Properties.Resources.add_icon;
                        row.Cells["Status"].ToolTipText = "Добавлено значение";
                        break;
                    case 3:
                        row.Cells["Status"].Appearance.Image = Properties.Resources.edit_icon;
                        row.Cells["Status"].ToolTipText = "Изменено значение";
                        break;
                }
            }
            if (row.Cells.Exists("Posl"))
            {
                row.Cells["Posl"].Value = last;
            }
        }

        #region загрузка фильтров

        private void GetTextFilterData(string fileName, ref object[] values)
        {
            List<object> valuesList = new List<object>();
            string[] lines = System.IO.File.ReadAllLines(fileName);
            foreach (string line in lines)
            {
                string num = GetNum(line);
                long value;
                if (long.TryParse(num, out value))
                    valuesList.Add(value);
            }
            values = valuesList.ToArray();
        }

        private string GetNum(string line)
        {
            string value = string.Empty;
            foreach (char ch in line)
            {
                if (ch >= '0' && ch <= '9')
                {
                    value += ch.ToString();
                }
                else
                    return value;
            }
            return value;
        }

        private void GetFilterData(string fileName, string columnName, ref object[] values)
        {
            List<object> valuesList = new List<object>();
            Workbook book = Workbook.Load(fileName);
            int rowIndex = 2;
            string cellAdress = string.Format("A{0}", rowIndex);
            object cellValue = book.Worksheets[0].GetCell(cellAdress).Value;
            while (cellValue != null && !string.IsNullOrEmpty(cellValue.ToString()))
            {
                valuesList.Add(cellValue);
                rowIndex++;
                cellAdress = string.Format("A{0}", rowIndex);
                cellValue = book.Worksheets[0].GetCell(cellAdress).Value;
            }
            values = valuesList.ToArray();
        }

        #endregion

        protected override void ugeCls_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool != null)
            {
                var btn = e.Tool as ButtonTool;
                if (btn != null)
                {
                    if (btn.SharedProps.Tag is CommonReportsCommand)
                    {
                        var cmdReport = (CommonReportsCommand)btn.SharedProps.Tag;
                        var activeRow = GetActiveDataRow();
                        object paramValue = ReportConsts.UndefinedKey;

                        if (activeRow != null)
                        {
                            paramValue = activeRow["ID"];
                        }

                        var filterParts = dataQuery.Split('?');
                        var sqlBuilder = new StringBuilder();
                        int paramCounter = 0;
                        var filterParams = GetServerFilterParameters();

                        if (filterParams != null)
                        {
                            foreach (var filterParam in filterParams)
                            {
                                sqlBuilder.Append(filterParts[paramCounter]);
                                sqlBuilder.Append(filterParam.ParamValue);
                                paramCounter++;
                            }
                        }

                        sqlBuilder.Append(filterParts[paramCounter]);
                        cmdReport.SetReportParamValue(ReportConsts.ParamOrgID, paramValue);
                        cmdReport.SetReportParamValue(ReportConsts.ParamMasterFilter, sqlBuilder.ToString());
                    }
                }
            }

            base.ugeCls_ToolClick(sender, e);
        }
    }
}
