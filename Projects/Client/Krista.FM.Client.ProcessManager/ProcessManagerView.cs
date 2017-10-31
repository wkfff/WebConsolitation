using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Components;

using Krista.FM.ServerLibrary;
using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Client.ProcessManager
{
    /// <summary>
    /// �������� ��������.
    /// </summary>
    public partial class ProcessManagerView : UserControl
    {
        private IProcessor processor;
        private Operation operation;
        private DataTable batchTable;
        private DataTable objectsTable;
        private DataTable emptyObjects;
        private ImageList il;
        private IScheme _scheme;

        /// <summary>
        /// ���� ��� ���������� �� ����� ������ ��� �������� �� ���������� �������� ��� ���������
        /// </summary>
        private string packageFilter;

        /// <summary>
        /// ������������� ����������.
        /// </summary>
        public ProcessManagerView()
        {
            InitializeComponent();

            this.components = new System.ComponentModel.Container();

            il = new ImageList(this.components);
            il.Images.Add("ProcessorServicePause", Properties.Resources.ProcessorServicePause);
            il.Images.Add("ProcessorServiceRun", Properties.Resources.ProcessorServiceRun);
            il.Images.Add("ProcessorServiceDelete", Properties.Resources.Delete);
            il.Images.Add("Empty", Properties.Resources.Empty);
            il.TransparentColor = Color.Magenta;

            batchGrid.AllowAddNewRecords = false;
            batchGrid.AllowDeleteRows = false;
            batchGrid.StateRowEnable = false;
            batchGrid.ugData.DisplayLayout.GroupByBox.Hidden = true;

            batchGrid.OnGridInitializeLayout += new GridInitializeLayout(batchGrid_OnGridInitializeLayout);
            batchGrid.OnRefreshData += new RefreshData(batchGrid_OnRefreshData);
            batchGrid.OnAfterRowActivate += new AfterRowActivate(batchGrid_OnAfterRowActivate);
            batchGrid.OnGetGridColumnsState += new GetGridColumnsState(batchGrid_OnGetGridColumnsState);
            batchGrid.OnClickCellButton += new ClickCellButton(batchGrid_OnClickCellButton);
            batchGrid.ugData.AfterRowFilterChanged += new AfterRowFilterChangedEventHandler(ugData_AfterRowFilterChanged);
            batchGrid.ugData.InitializeRow += new InitializeRowEventHandler(ugData_InitializeRow);

            partitionsGrid.OnGridInitializeLayout += new GridInitializeLayout(objectsGrid_OnGridInitializeLayout);
            partitionsGrid.ugData.InitializeRow += new InitializeRowEventHandler(objectsGrid_ugData_InitializeRow);

            dimensionsGrid.OnGridInitializeLayout += new GridInitializeLayout(dimensionsGrid_OnGridInitializeLayout);
            dimensionsGrid._ugData.InitializeRow += new InitializeRowEventHandler(objectsGrid_ugData_InitializeRow);

            // ������� �������������� ������ �� �������
            UltraToolbar processorToolbar = new UltraToolbar("processorToolbar");
            processorToolbar.DockedColumn = 1;
            processorToolbar.DockedRow = 0;
            processorToolbar.Text = "�������� ��������";

            // ������ ��� ������������ ���������� ��������           
            ButtonTool processServiceRunTool = new ButtonTool("ProcessServiceRun");
            processServiceRunTool.SharedProps.Caption = "��������� ��������� ������� �������";
            processServiceRunTool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.ProcessorServiceRun;

            // ������ "���������� ��������� ������� �������"
            ButtonTool processServicePauseTool = new ButtonTool("ProcessServicePause");
            processServicePauseTool.SharedProps.Caption = "���������� ��������� ������� �������";
            processServicePauseTool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.ProcessorServicePause;

            // ������ "�������� ��� ������"
            StateButtonTool defaultFiltersTool = new StateButtonTool("DefaultFilters");
            defaultFiltersTool.SharedProps.Caption = "�������� ������, ��������� �������";
            defaultFiltersTool.Checked = false;
            defaultFiltersTool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.Filter;

            #region ������ "�������� ��� ��������"

            // ������ "�������� ��� ��������"
            PopupMenuTool changeBatchStateTool = new PopupMenuTool("ChangeBatchState");
            changeBatchStateTool.SharedProps.Caption = "�������� ��� ��������";
            changeBatchStateTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Actions_Process;
            changeBatchStateTool.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageOnlyOnToolbars;

            ButtonTool suspendProcessTool = new ButtonTool("SuspendProcess");
            suspendProcessTool.SharedProps.Caption = "�������� ��������� ���������� �������";
            suspendProcessTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Pause_Process;

            ButtonTool resumeProcessTool = new ButtonTool("ResumeProcess");
            resumeProcessTool.SharedProps.Caption = "����������� ��������� ���������� �������";
            resumeProcessTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Run_Process;

            ButtonTool deleteBatchTool = new ButtonTool("DeleteBatch");
            deleteBatchTool.SharedProps.Caption = "������� ���������� ������";
            deleteBatchTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Delete_Process;

            ButtonTool canselBatchTool = new ButtonTool("CancelBatch");
            canselBatchTool.SharedProps.Caption = "�������� ���������� ��������� �������� ������";
            canselBatchTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.cancel;

            #endregion ������ "�������� ��� ��������"

            processorToolbar.Tools.AddRange(new ToolBase[] { 
                changeBatchStateTool, 
                processServiceRunTool, 
                processServicePauseTool, 
                defaultFiltersTool
            });

            batchGrid.utmMain.Toolbars.Add(processorToolbar);

            changeBatchStateTool.ToolbarsManager.Tools.AddRange(new ToolBase[] { 
                suspendProcessTool,
                resumeProcessTool,
                deleteBatchTool,
                canselBatchTool
            });

            changeBatchStateTool.Tools.AddTool(suspendProcessTool.Key);
            changeBatchStateTool.Tools.AddTool(resumeProcessTool.Key);
            changeBatchStateTool.Tools.AddTool(deleteBatchTool.Key);
            changeBatchStateTool.Tools.AddTool(canselBatchTool.Key);

            batchGrid.utmMain.ToolClick += new ToolClickEventHandler(Toolbar_ToolClick);

            batchGrid._ugData.AfterRowFilterChanged += new AfterRowFilterChangedEventHandler(_ugData_AfterRowFilterChanged);
            utcDetails.ActiveTabChanged += new Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventHandler(utcDataCls_ActiveTabChanged);
            //InfragisticComponentsCustomize.CustomizeUltraGridParams(dimensionsGrid._ugData);
        }

        private static void batchGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // ������� ������� � ������� ��� ������������ ������ ������������
            UltraGridColumn stateControlColumn = e.Layout.Bands[0].Columns.Add("StateControl");
            UltraGridHelper.SetLikelyImageColumnsStyle(stateControlColumn, -1);
            stateControlColumn.Header.VisiblePosition = 0;
            stateControlColumn.CellButtonAppearance.Image = Properties.Resources.ProcessorServicePause;

            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn;

            band.Columns["ID"].Hidden = true;
            band.Columns["RefUser"].Hidden = true;
            band.Columns["BatchState"].Hidden = true;
            band.Columns["Priority"].Hidden = true;

            clmn = band.Columns["BatchId"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 1;
            clmn.Hidden = false;
            clmn.Header.Caption = "ID ������";
            clmn.Width = 240;

            clmn = band.Columns["UserName"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 2;
            clmn.Hidden = false;
            clmn.Header.Caption = "������������";
            clmn.Width = 150;

            clmn = band.Columns["AdditionTime"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 3;
            clmn.Hidden = false;
            clmn.Header.Caption = "������";
            clmn.Width = 100;

            clmn = band.Columns["BatchStateName"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 4;
            clmn.Hidden = false;
            clmn.Header.Caption = "���������";
            clmn.Width = 122;

            clmn = band.Columns["SessionId"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 5;
            clmn.Hidden = false;
            clmn.Header.Caption = "������";
            clmn.Width = 165;

            clmn = band.Columns["PriorityName"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 6;
            clmn.Hidden = false;
            clmn.Header.Caption = "���������";
            clmn.Width = 113;
        }

        private GridColumnsStates cashedColumnsStates;
        private GridColumnsStates batchGrid_OnGetGridColumnsState(object sender)
        {
            if (cashedColumnsStates == null)
            {
                cashedColumnsStates = new GridColumnsStates();

                GridColumnState state = new GridColumnState();
                state.ColumnCaption = "������";
                state.ColumnName = "AdditionTime";
                state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
                cashedColumnsStates.Add("AdditionTime", state);
            }
            return cashedColumnsStates;
        }

        private static void objectsGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            OlapObjectsView.OlapObjectsGridInitializeLayout(e, OlapObjectType.Partition);
            e.Layout.Bands[0].Columns["RefBatchId"].Hidden = true;
        }

        private static void objectsGrid_ugData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            #region ��������� ����� �����

            if (e.Row.IsDataRow)
            {
                e.Row.Appearance.BackColor = Color.Gainsboro;
            }

            #endregion ��������� ����� �����
        }

        void dimensionsGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            OlapObjectsView.OlapObjectsGridInitializeLayout(e, OlapObjectType.Dimension);
            e.Layout.Bands[0].Columns["RefBatchId"].Hidden = true;
        }

        private void ugData_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            // ������ �������� ������ ������
            if (batchGrid.ugData.Rows.FilteredInRowCount > 0)
                batchGrid.ugData.Rows.GetFilteredInNonGroupByRows()[0].Activate();
        }

        private void ugData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;

            #region ������������� ������ ���������� �������

            switch ((BatchState)Convert.ToInt32(row.Cells["BatchState"].Value))
            {
                case BatchState.Waiting:
                    row.Cells["StateControl"].ToolTipText = "����� � ������� ����������";
                    row.Cells["StateControl"].Appearance.Image = this.il.Images["ProcessorServiceRun"];
                    break;
                case BatchState.Canceled:
                    row.Cells["StateControl"].ToolTipText = "���������� ������ ��������";
                    row.Cells["StateControl"].Appearance.Image = this.il.Images["ProcessorServicePause"];
                    break;
                case BatchState.Deleted:
                    row.Cells["StateControl"].ToolTipText = "����� ������� �� ��������";
                    row.Cells["StateControl"].Appearance.Image = this.il.Images["ProcessorServiceDelete"];
                    break;
                default:
                    row.Cells["StateControl"].ToolTipText = String.Empty;
                    row.Cells["StateControl"].Appearance.Image = this.il.Images["Empty"];
                    break;
            }

            #endregion ������������� ������ ���������� �������

            #region ��������� ����� �����

            if (row.IsDataRow)
            {
                Color rowColor = Color.Empty;
                switch ((BatchState)Convert.ToInt32(row.Cells["BatchState"].Value))
                {
                    case BatchState.Created:
                        rowColor = Color.White;
                        break;
                    case BatchState.Waiting:
                        rowColor = Color.PaleGoldenrod;
                        break;
                    case BatchState.Running:
                        rowColor = Color.Red;
                        break;
                    case BatchState.Complited:
                        rowColor = Color.PaleGreen;
                        break;
                    case BatchState.Canceled:
                        rowColor = Color.LightGray;
                        break;
                    case BatchState.ComplitedWithError:
                        rowColor = Color.LightSalmon;
                        break;
                }
                row.Appearance.BackColor = rowColor;
            }

            #endregion ��������� ����� �����
        }

        private void batchGrid_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "StateControl")
            {
                UltraGridRow row = e.Cell.Row;

                BatchState state = (BatchState)Convert.ToInt32(row.Cells["BatchState"].Value);
                BatchState newState = state;
                switch (state)
                {
                    case BatchState.Waiting: newState = BatchState.Canceled; break;
                    case BatchState.Canceled: newState = BatchState.Waiting; break;
                }
                if (newState != state)
                {
                    DataRow dataRow = batchTable.Rows.Find(row.Cells["BatchId"].Value);
                    if (dataRow != null)
                    {
                        try
                        {
                            dataRow["BatchState"] = newState;
                            dataRow["BatchStateName"] = ProcessorEnumsConverter.GetBatchStateName(newState);
                            DataTable changes = batchTable.GetChanges();
                            processor.OlapDBWrapper.UpdateBatchValues(ref changes);
                        }
                        catch
                        {
                            batchTable.RejectChanges();
                        }
                    }
                }
            }
        }

        private void Toolbar_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "ProcessServiceRun":
                    RunPauseProcessService(false);
                    break;
                case "ProcessServicePause":
                    RunPauseProcessService(true);
                    break;
                case "DefaultFilters":
                    StateButtonTool tool = (StateButtonTool)e.Tool;
                    if (tool.Checked)
                    {
                        RestoreDefaultFilters();
                        tool.SharedProps.Caption = "�������� ��� ������";
                    }
                    else
                    {
                        ResetFilters();
                        tool.SharedProps.Caption = "�������� ������, ��������� �������";
                    }
                    ColumnFilter columnFilter = batchGrid.ugData.DisplayLayout.Bands[0].ColumnFilters["BatchStateName"];
                    UltraGridColumn column = batchGrid.ugData.DisplayLayout.Bands[0].Columns["BatchStateName"];
                    _ugData_AfterRowFilterChanged(batchGrid._ugData, new AfterRowFilterChangedEventArgs(column, columnFilter));
                    break;
                    // ������������� 
                case "SuspendProcess":
                    SetBatchState(BatchState.Canceled);
                    break;
                    // ����������
                case "ResumeProcess":
                    SetBatchState(BatchState.Waiting);
                    break;
                    // �������
                case "DeleteBatch":
                    SetBatchState(BatchState.Deleted);
                    break;
                case "CancelBatch":
                    try
                    {
                        operation.StartOperation();
                        operation.Text = "������ �������� ������";
                        foreach (UltraGridRow selectedRow in batchGrid._ugData.Rows)
                        {
                            if ((BatchState) Convert.ToInt32(selectedRow.Cells["BatchState"].Value) ==
                                BatchState.Running)
                                processor.ProcessManager.StopBatch(
                                    new Guid(selectedRow.Cells["BatchId"].Value.ToString()));
                        }
                        Thread.Sleep(3000);
                    }
                    finally
                    {
                        operation.StopOperation();
                    }
                    
                    break;
            }
        }


        /// <summary>
        /// ��������� ��������� ������ � ���� �� ���������.
        /// </summary>
        /// <param name="newState"></param>
        private void SetBatchState(BatchState newState)
        {
            if (batchGrid._ugData.Selected.Rows.Count == 0 && batchGrid._ugData.ActiveRow != null)
            {
                batchGrid._ugData.ActiveRow.Selected = true;
            }
            
            foreach (UltraGridRow selectedRow in batchGrid._ugData.Selected.Rows)
            {
                BatchState state = (BatchState)Convert.ToInt32(selectedRow.Cells["BatchState"].Value);
                bool canSetState = false;
                switch (state)
                {
                    // ����� ���������� � ��� ���������: �������� � ��������
                    case BatchState.Canceled:
                        if (newState == BatchState.Waiting || newState == BatchState.Deleted)
                            canSetState = true;
                        break;
                    // ����� ���������� � ���� ���������: ������� ��� ������
                    case BatchState.Waiting:
                        if (newState == BatchState.Canceled || newState == BatchState.Deleted)
                            canSetState = true;
                        break;
                }
                if (canSetState)
                {
                    DataRow dataRow = batchTable.Rows.Find(selectedRow.Cells["BatchId"].Value);
                    if (dataRow != null)
                    {
                        dataRow["BatchState"] = newState;
                        dataRow["BatchStateName"] = ProcessorEnumsConverter.GetBatchStateName(newState);
                    }
                }
            }
            
            try
            {
                DataTable changes = batchTable.GetChanges();
                processor.OlapDBWrapper.UpdateBatchValues(ref changes);
            }
            catch (Exception e)
            {
                FormException.ShowErrorForm(e, ErrorFormButtons.Continue | ErrorFormButtons.DetailInfo | ErrorFormButtons.Report);
                batchTable.RejectChanges();
            }
        }

        private void ResetFilters()
        {
            foreach (ColumnFilter filter in batchGrid.ugData.DisplayLayout.Bands[0].ColumnFilters)
            {
                filter.ClearFilterConditions();
            }
        }

        private void RestoreDefaultFilters()
        {
            ResetFilters();
            // ���������� ������ �� ��������� �� ������ ������������
            batchGrid.ugData.DisplayLayout.Bands[0].ColumnFilters["BatchStateName"].
                FilterConditions.Add(FilterComparisionOperator.NotEquals, ProcessorEnumsConverter.GetBatchStateName(BatchState.Complited));
            batchGrid.ugData.DisplayLayout.Bands[0].ColumnFilters["BatchStateName"].
                FilterConditions.Add(FilterComparisionOperator.NotEquals, ProcessorEnumsConverter.GetBatchStateName(BatchState.ComplitedWithError));
            batchGrid.ugData.DisplayLayout.Bands[0].ColumnFilters["BatchStateName"].
                FilterConditions.Add(FilterComparisionOperator.NotEquals, ProcessorEnumsConverter.GetBatchStateName(BatchState.Deleted));
        }

        private void RunPauseProcessService(bool pause)
        {
            processor.ProcessManager.Paused = pause;
            RefreshRunPauseProcessServiceTools();
        }

        private void RefreshRunPauseProcessServiceTools()
        {
            bool paused = processor.ProcessManager.Paused;
            batchGrid.utmMain.Toolbars["processorToolbar"].Tools["ProcessServiceRun"].SharedProps.Enabled = paused;
            batchGrid.utmMain.Toolbars["processorToolbar"].Tools["ProcessServicePause"].SharedProps.Enabled = !paused;
        }

        /// <summary>
        /// ���������� ��������� �� ��������� � ������� �������.
        /// </summary>
        public bool Connected
        {
            get { return processor != null; }
        }

        /// <summary>
        /// ����������� ���������� � ������� �������.
        /// </summary>
        /// <param name="_processor">�������� ������� �����.</param>
        /// <param name="_operation">��������� ��������.</param>
        public void Connect(IProcessor _processor, Operation _operation)
        {
            this.processor = _processor;
            this.operation = _operation;
            this.Dock = DockStyle.Fill;
            this.Visible = false;

            InfragisticComponentsCustomize.CustomizeUltraGridParams(batchGrid._ugData);
            InfragisticComponentsCustomize.CustomizeUltraGridParams(partitionsGrid._ugData);
            InfragisticComponentsCustomize.CustomizeUltraGridParams(dimensionsGrid._ugData);

            RefreshData();

            // ������������� ���������� �� ���� ID
            batchGrid.sortColumnName = "ID";
            SetSortDirection(batchGrid);

            // RestoreDefaultFilters();
            // ������ �������� ������ ������
            if (batchGrid.ugData.Rows.FilteredInRowCount > 0)
                batchGrid.ugData.Rows.GetFilteredInNonGroupByRows()[0].Activate();

            RefreshRunPauseProcessServiceTools();
        }


        /// <summary>
        /// ����������� ���������� � ������� �������.
        /// </summary>
        /// <param name="_processor">�������� ������� �����.</param>
        /// <param name="_operation">��������� ��������.</param>
        public void Connect(IScheme scheme, Operation _operation)
        {
            this.processor = scheme.Processor;
            this._scheme = scheme;
            this.operation = _operation;
            this.Dock = DockStyle.Fill;
            this.Visible = false;

            InfragisticComponentsCustomize.CustomizeUltraGridParams(batchGrid._ugData);
            InfragisticComponentsCustomize.CustomizeUltraGridParams(partitionsGrid._ugData);
            InfragisticComponentsCustomize.CustomizeUltraGridParams(dimensionsGrid._ugData);
            InfragisticComponentsCustomize.CustomizeUltraTabControl(this.utcDetails);

            emptyObjects = processor.OlapDBWrapper.GetPartitions("1 <> 1");

            RefreshData();

            // ������������� ���������� �� ���� ID
            batchGrid.sortColumnName = "ID";
            SetSortDirection(batchGrid);

            // RestoreDefaultFilters();
            // ������ �������� ������ ������
            if (batchGrid.ugData.Rows.FilteredInRowCount > 0)
                batchGrid.ugData.Rows.GetFilteredInNonGroupByRows()[0].Activate();

            RefreshRunPauseProcessServiceTools();

            ((PopupMenuTool)batchGrid.utmMain.Toolbars["processorToolbar"].Tools["ChangeBatchState"]).Tools["CancelBatch"].SharedProps.Visible =
                scheme.SchemeMDStore.IsAS2005();
        }


        private void batchGrid_OnAfterRowActivate(object sender, EventArgs e)
        {
            UltraGridRow tmpRow = UltraGridHelper.GetRowCells(batchGrid.ugData.ActiveRow);
            LoadDetailData(tmpRow);
        }

        private bool batchGrid_OnRefreshData(object sender)
        {
            RefreshRunPauseProcessServiceTools();

            UltraGridStateSettings settings = UltraGridStateSettings.SaveUltraGridFilterSettings(batchGrid.ugData);
            RefreshData();
            settings.RestoreUltraGridFilterSettings(batchGrid.ugData);

            // ������������� ���������� �� ���� ID
            batchGrid.sortColumnName = "ID";
            SetSortDirection(batchGrid);
            
            // ������ �������� ������ ������
            if (batchGrid.ugData.Rows.FilteredInRowCount > 0)
                batchGrid.ugData.Rows.GetFilteredInNonGroupByRows()[0].Activate();

            // ��������� ������
            UltraGridRow tmpRow = UltraGridHelper.GetRowCells(batchGrid.ugData.ActiveRow);
            if (tmpRow != null)
            {
                LoadDetailData(tmpRow);
            }
            else
            {
                partitionsGrid.DataSource = emptyObjects;
                dimensionsGrid.DataSource = emptyObjects;
                ShowProtocolForRow(null);
            }

            return true;
        }

        /// <summary>
        /// ���������� ������ �����. ������ ������������� � �������.
        /// </summary>
        private void RefreshData()
        {
            try
            {
                operation.StartOperation();
                operation.Text = "��������� ������";
                batchTable = processor.OlapDBWrapper.BatchesView;
                batchGrid.DataSource = batchTable;
            }
            finally
            {
                operation.StopOperation();
            }
        }

        /// <summary>
        /// ������� ������ ������.
        /// </summary>
        private void ClearDetailData()
        {
            partitionsGrid.DataSource = emptyObjects;
            dimensionsGrid.DataSource = emptyObjects;
        }

        /// <summary>
        /// �������� ������ ������.
        /// </summary>
        /// <param name="masterRow"></param>
        private void LoadDetailData(UltraGridRow masterRow)
        {
            if (utcDetails.ActiveTab == null)
                utcDetails.ActiveTab = utcDetails.Tabs[0];

            int objectType = (int)OlapObjectType.Partition;
            UltraGridEx detailGrid = partitionsGrid;

            switch (utcDetails.ActiveTab.Index)
            {
                case 0:
                case 1:
                    // �������� ������ ������
                    if (utcDetails.ActiveTab.Index == 1)
                    {
                        objectType = (int)OlapObjectType.Dimension;
                        detailGrid = dimensionsGrid;
                    }

                    try
                    {
                        if (masterRow == null || (BatchState)masterRow.Cells["BatchState"].Value == BatchState.Complited ||
                            (BatchState)masterRow.Cells["BatchState"].Value == BatchState.ComplitedWithError)
                        {
                            detailGrid.DataSource = emptyObjects;
                        }
                        else
                        {
                            operation.Text = "��������� ������ ������";
                            operation.StartOperation();
                            objectsTable = processor.OlapDBWrapper.GetPartitions(
                                String.Format("RefBatchId = '{0}' and ObjectType = {1}", masterRow.Cells["BatchId"].Value, objectType));
                            DataTableHelper.RemoveEmptyData(ref objectsTable);
                            detailGrid.DataSource = objectsTable;
                            detailGrid.ugData.DisplayLayout.GroupByBox.Hidden = true;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message, e);
                    }
                    finally
                    {
                        operation.StopOperation();
                    }
                    break;
                case 2:
                    ShowProtocolForRow(UltraGridHelper.GetRowCells(masterRow));
                    break;
            }
        }

        /// <summary>
        /// �������� ������ ��� �������� �� �������� ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void utcDataCls_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            LoadDetailData(UltraGridHelper.GetRowCells(batchGrid._ugData.ActiveRow));
        }


        void _ugData_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            if (utcDetails.ActiveTab != null)
                if (utcDetails.ActiveTab.Index == 2)
                {
                    ShowProtocolForRow(batchGrid._ugData.ActiveRow);
                }
        }

        /// <summary>
        /// ����������� ���������� �� ��������� ������
        /// </summary>
        /// <param name="row"></param>
        private void ShowProtocolForRow(UltraGridRow row)
        {
            if (row != null && row.VisibleIndex >= 0)
            {
                string batchID = row.Cells["BatchId"].Value.ToString();
                IDbDataParameter param = new System.Data.OleDb.OleDbParameter("BatchId", batchID);//db.CreateParameter("BatchId", batchID);
                protocols.ShowProtocol(ModulesTypes.MDProcessingModule, _scheme, "BatchId = ?", param);
                protocols.GridEx.SaveLoadFileName = string.Format("������ ����������� ����_[{0}]", batchID);
                protocols.GridEx.sortColumnName = "ID";
                SetSortDirection(protocols.GridEx);
            }
            else
            {
                protocols.ShowProtocol(ModulesTypes.MDProcessingModule, _scheme, "1 <> 1");
            }
        }

        private void SetSortDirection(UltraGridEx uGridEx)
        {
            foreach (UltraGridBand band in uGridEx._ugData.DisplayLayout.Bands)
            {
                band.Columns[uGridEx.sortColumnName].SortIndicator = SortIndicator.Descending;
            }
        }

        /// <summary>
        /// ���� ��� ���������� �� ����� ������ ��� �������� �� ���������� �������� ����� ��� ���������
        /// </summary>
        public string PackageFilter
        {
            get { return packageFilter; }
            set 
            {
                packageFilter = value;
                ResetFilters();
                RefreshData();
                batchGrid.ugData.DisplayLayout.Bands[0].ColumnFilters["BatchId"].
                    FilterConditions.Add(FilterComparisionOperator.Equals, packageFilter);

                if (batchGrid.ugData.Rows.FilteredInRowCount > 0)
                    batchGrid.ugData.Rows.GetFilteredInNonGroupByRows()[0].Activate();
            }
        }
    }
}
