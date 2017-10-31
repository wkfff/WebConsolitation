using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

using Krista.FM.Client.Common;
using Krista.FM.Common;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;

using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.AdministrationUI;


namespace Krista.FM.Client.ViewObjects.ProtocolsUI
{
    public partial class ProtocolsViewObject : BaseViewObj, IInplaceProtocolView
    {
        private ProtocolsView pView;
        private DataTable LogDataTable = new DataTable();

        //private int UserFilterCondition = -1;
        private string DateFilterCondition = string.Empty;
        private IDbDataParameter[] DateTimeFilterParams = null;

        private DataTable eventsFilterTable = new DataTable();

        private const string DisplayLogKindColumnName = "pic";
        private const string DisplayModuleKindColumnName = "DisplayModuleKind";

        private ModulesTypes currentProtocol;

        public ProtocolsViewObject(ModulesTypes protocolType)
            : base(protocolType.ToString())
        {
            currentProtocol = protocolType;
            Caption = "���������";
        }

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.log_Main_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.log_Main_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.log_Main_24; }
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new ProtocolsView();
            pView = (ProtocolsView)fViewCtrl;
        }

        public override void Initialize()
        {
            base.Initialize();

            pView.udteBeginPeriod.Value = DateTime.Today;
            pView.udteEndPeriod.Value = DateTime.Today;
          
            pView.cbUseEventType.CheckedChanged += new EventHandler(cbUseEventType_CheckedChanged);
            pView.cbUsePeriod.CheckedChanged += new EventHandler(cbUsePeriod_CheckedChanged);

            pView.udteBeginPeriod.AfterCloseUp += new EventHandler(udteBeginPeriod_AfterCloseUp);
            pView.udteEndPeriod.AfterCloseUp += new EventHandler(udteBeginPeriod_AfterCloseUp);

            pView.udteBeginPeriod.TextChanged += new EventHandler(udteBeginPeriod_TextChanged);
            pView.udteEndPeriod.TextChanged += new EventHandler(udteBeginPeriod_TextChanged);

            pView.udteBeginPeriod.BeforeDropDown += new System.ComponentModel.CancelEventHandler(udteBeginPeriod_BeforeDropDown);
            pView.udteEndPeriod.BeforeDropDown += new System.ComponentModel.CancelEventHandler(udteBeginPeriod_BeforeDropDown);

            pView.ucEvents.AfterCloseUp += new EventHandler(ucEvents_AfterCloseUp);

            pView.ucEvents.AfterDropDown += new EventHandler(ucEvents_AfterDropDown);
            pView.ucEvents.AfterCloseUp += new EventHandler(ucEvents_AfterCloseUp);
            pView.ucEvents.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(ucEvents_MouseEnterElement);
            pView.ucEvents.MouseLeaveElement += new Infragistics.Win.UIElementEventHandler(ucEvents_MouseLeaveElement);
            pView.ucEvents.MouseEnter += new EventHandler(ucEvents_MouseEnter);
            pView.ucEvents.MouseLeave += new EventHandler(ucEvents_MouseLeave);

            pView.ugex1.OnInitializeRow += new CC.InitializeRow(ProtocolView_InitializeRow);
            pView.ugex1.OnClickCellButton += new CC.ClickCellButton(ProtocolUserView_ButtonClick);
            pView.ugex1.OnRefreshData += new CC.RefreshData(ugex1_OnRefreshData);
            pView.ugex1.OnSaveToXML += new CC.SaveLoadXML(ugex1_OnSaveToXML);
            pView.ugex1.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugex1_OnGetGridColumnsState);
            pView.ugex1.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugex1_OnGridInitializeLayout);

            pView.ugeAudit.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeAudit_OnGetGridColumnsState);
            pView.ugeAudit.OnRefreshData += new Krista.FM.Client.Components.RefreshData(ugex1_OnRefreshData);
            pView.ugeAudit.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ugeAudit_OnInitializeRow);
            pView.ugeAudit.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugeAudit_OnGridInitializeLayout);
            pView.ugeAudit.OnGetHandbookValue += new Krista.FM.Client.Components.GetHandbookValue(ugeAudit_OnGetHandbookValue);
            pView.ugeAudit.OnGetServerFilterCustomDialogColumnsList += new Krista.FM.Client.Components.GetServerFilterCustomDialogColumnsList(ugeAudit_OnGetServerFilterCustomDialogColumnsList);
            pView.ugeAudit.OnBeforeRowFilterDropDownPopulateEventHandler += new BeforeRowFilterDropDownPopulateEventHandler(ugeAudit_OnBeforeRowFilterDropDownPopulateEventHandler);
            // ��������� ��� ����� ������
            pView.ugeAudit.ServerFilterEnabled = true;
            pView.ugeAudit.StateRowEnable = false;
            pView.ugeAudit.AllowImportFromXML = false;
            pView.ugeAudit.AllowDeleteRows = false;
            pView.ugeAudit.AllowAddNewRecords = false;
            pView.ugeAudit.AllowEditRows = false;
            //pView.ugeAudit.StateRowEnable = false;

            pView.ugeAudit.ugFilter.BeforeRowFilterDropDown += new BeforeRowFilterDropDownEventHandler(ugData_BeforeRowFilterDropDown);

            taskPermissions = this.Workplace.GetTasksPermissions();
            if (taskPermissions == null)
                taskPermissions = GetNewTaskPermission();
            usersModalForm = taskPermissions.IUserModalForm;
            pView.ugex1.StateRowEnable = true;

            pView.ugex1._ugData.ImageList = pView.ilColumns;

            eventsFilterTable.Columns.Add("Caption");
            eventsFilterTable.Columns.Add("EventCode");
            pView.ucEvents.DataSource = eventsFilterTable;
            ChangeDateTimeFilterCondition();
            //ProtocolsNavigation.Instance.uebNavi.Groups[0].Items[0].Checked = true;

            SetCurrentFilterState();

            pView.utcProtocols.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;

            pView.ugex1.utmMain.Tools["excelExport"].SharedProps.Visible = false;
           
            pView.ugex1.SaveMenuVisible = true;
            pView.ugex1.LoadMenuVisible = false;

            pView.ugeAudit.SaveMenuVisible = false;
            pView.ugeAudit.LoadMenuVisible = false;
        }

        internal IInplaceTasksPermissionsView GetNewTaskPermission()
        {
            AdministrationUI.AdministrationUI newPermissions =
                new AdministrationUI.AdministrationUI("E3FE8CF7-C803-41e1-A9FE-C595A1CF6A15");

            newPermissions.Workplace = Workplace;
            newPermissions.Initialize();
            return newPermissions;
        }

        void ugex1_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];
            
            UltraGridColumn clmn = band.Columns["EventDateTime"];
            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
            clmn.Width = 125;
            clmn.Format = "dd.MM.yyyy HH:mm:ss";
        }

        private Dictionary<string, CC.GridColumnsStates> cashedColumnsSettings = new Dictionary<string, CC.GridColumnsStates>();

        CC.GridColumnsStates ugex1_OnGetGridColumnsState(object sender)
        {
            if (cashedColumnsSettings.ContainsKey(currentProtocol.ToString()))
                return cashedColumnsSettings[currentProtocol.ToString()];

            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID ������ ���������";
            state.ColumnName = "ID";
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "RefUsersOperations";
            state.IsHiden = true;
            states.Add("RefUsersOperations", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "�������� �������";
            state.ColumnName = "PumpHistoryID";
            states.Add("PumpHistoryID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "ID ��������� ������";
            state.ColumnName = "DataSourceID";
            states.Add("DataSourceID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = string.Empty;
            state.ColumnName = "ObjectName";
            state.IsHiden = true;
            states.Add("ObjectName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = String.Empty;
            state.ColumnName = "ActionName";
            state.IsHiden = true;
            states.Add("ActionName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "����/�����";
            state.ColumnName = "EventDateTime";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
            states.Add("EventDateTime", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "���������";
            state.ColumnName = "InfoMessage";
            state.ColumnWidth = 500;
            states.Add("InfoMessage", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "������������";
            state.ColumnName = "UserName";
            states.Add("UserName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = String.Empty;
            state.ColumnName = "KindsOfEvents";
            state.IsHiden = true;
            states.Add("KindsOfEvents", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "������";
            state.IsHiden = false;

            state.ColumnName = "MODULE";
            state.ColumnWidth = 150;
            states.Add("Module", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "��� �������";
            state.ColumnName = "MDObjectName";
            states.Add("MDObjectName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "������ ��";
            state.ColumnName = "Classifier";
            states.Add("Classifier", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "��������������";
            state.ColumnName = "BridgeRoleA";
            states.Add("BridgeRoleA", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "������������";
            state.ColumnName = "BridgeRoleB";
            states.Add("BridgeRoleB", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "������ ������������";
            state.ColumnName = "USERHOST";
            states.Add("USERHOST", state);

            //SessionID
            state = new CC.GridColumnState();
            state.ColumnCaption = "ID ������";
            state.ColumnName = "SessionID";
            state.IsHiden = true;
            states.Add("SessionID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "ObjectFullName";
            state.IsHiden = true;
            states.Add("ObjectFullName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "��� �������";
            state.ColumnName = "ObjectFullCaption";
            states.Add("ObjectFullCaption", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "��� �����������";
            state.ColumnName = "ModificationType";
            states.Add("ModificationType", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "��� ������� ��";
            state.ColumnName = "OBJECTTYPE";
            states.Add("OBJECTTYPE", state);

            //

            state = new CC.GridColumnState();
            state.ColumnCaption = "��� �������";
            state.ColumnName = "OlapObjectType";
            states.Add("OlapObjectType", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "������������� �������";
            state.ColumnName = "ObjectIdentifier";
            states.Add("ObjectIdentifier", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "������������� ������";
            state.ColumnName = "BatchId";
            states.Add("BatchId", state);

            cashedColumnsSettings.Add(currentProtocol.ToString(), states);

            return states;
        }

        // ���������� ��� �������� ����������� ��������� ��������
        private bool UsePeriod;
        private DateTime beginPeriod;
        private DateTime endPeriod;
        private bool UseEventType;
        private string Events;

        private void SetCurrentFilterState()
        {
            UsePeriod = pView.cbUsePeriod.Checked;
            beginPeriod = pView.udteBeginPeriod.DateTime;
            endPeriod = pView.udteEndPeriod.DateTime;
            UseEventType = pView.cbUseEventType.Checked;
            Events = pView.ucEvents.Text;
        }

        private bool IsFilterChange()
        {
            if ((UsePeriod != pView.cbUsePeriod.Checked) ||
            (beginPeriod != pView.udteBeginPeriod.DateTime) ||
            (endPeriod != pView.udteEndPeriod.DateTime) ||
            (UseEventType != pView.cbUseEventType.Checked) ||
            (Events != pView.ucEvents.Text))
                return true;
            return false;
        }

        void udteBeginPeriod_TextChanged(object sender, EventArgs e)
        {
            if (currentProtocol == ModulesTypes.AuditModule)
                pView.ugeAudit.BurnRefreshDataButton(true);
            else
                pView.ugex1.BurnRefreshDataButton(true);
        }

        void udteBeginPeriod_AfterCloseUp(object sender, EventArgs e)
        {
            if (currentProtocol == ModulesTypes.AuditModule)
                pView.ugeAudit.BurnRefreshDataButton(IsFilterChange());
            else
            {
                pView.ugex1.BurnRefreshDataButton(IsFilterChange());
                GetSaveLoadFileName();
            }
        }

        private DateTime lastDateTime;
        void udteBeginPeriod_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lastDateTime = ((UltraDateTimeEditor)sender).DateTime;
        }

        void cbUserIdent_CheckedChanged(object sender, EventArgs e)
        {
            if (currentProtocol == ModulesTypes.AuditModule)
                pView.ugeAudit.BurnRefreshDataButton(IsFilterChange());
            else
                pView.ugex1.BurnRefreshDataButton(IsFilterChange());
        }

        void ucEvents_MouseLeave(object sender, EventArgs e)
        {
            pView.pToolTip.Hide();
        }

        void ucEvents_MouseEnter(object sender, EventArgs e)
        {
            Point tooltipPos = pView.ucEvents.Location;
            tooltipPos.Y = tooltipPos.Y + pView.ucEvents.Height;
            string toolTipText = pView.ucEvents.Text;
            pView.pToolTip.ToolTipText = toolTipText;
            pView.pToolTip.Show(pView.pnControls.PointToScreen(tooltipPos));
        }

        void ucEvents_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (e.Element is CellUIElement)
                pView.pToolTip.Hide();
        }

        void ucEvents_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (e.Element is CellUIElement)
            {
                pView.pToolTip.Hide();
                UltraGridCell cell = (UltraGridCell)e.Element.GetContext(typeof(UltraGridCell));
                if (cell == null) return;

                string toolTipText = cell.Text;

                pView.pToolTip.ToolTipText = toolTipText;
                Point tooltipPos = new Point(e.Element.ClipRect.Left, e.Element.ClipRect.Bottom);
                tooltipPos.Y = tooltipPos.Y + cell.Height + 2;
                pView.pToolTip.Show(pView.ucEvents.PointToScreen(tooltipPos));
            }
        }

        void ucEvents_AfterCloseUp(object sender, EventArgs e)
        {
            int ImageIndex = Convert.ToInt32(pView.ucEvents.ActiveRow.Cells[0].Appearance.Image);
            pView.pbEventImage.Image = pView.ilColumns.Images[ImageIndex];
            if (currentProtocol == ModulesTypes.AuditModule)
                pView.ugeAudit.BurnRefreshDataButton(IsFilterChange());
            else
                pView.ugex1.BurnRefreshDataButton(IsFilterChange());
        }

        void ucEvents_AfterDropDown(object sender, EventArgs e)
        {
            //pView.ucEvents.
        }

        /// <summary>
        ///  ����������, �������� ��� ��� ������ �� ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbUsePeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (pView.cbUsePeriod.Checked)
            {
                pView.udteBeginPeriod.Enabled = true;
                pView.udteEndPeriod.Enabled = true;
            }
            else
            {
                pView.udteBeginPeriod.Enabled = false;
                pView.udteEndPeriod.Enabled = false;
            }
            if (currentProtocol == ModulesTypes.AuditModule)
                pView.ugeAudit.BurnRefreshDataButton(IsFilterChange());
            else
            {
                pView.ugex1.BurnRefreshDataButton(IsFilterChange());
                GetSaveLoadFileName();
            }
        }


        /// <summary>
        ///  ��������� ������������� �� ��������� ����� �����  
        /// </summary>
        void GetSaveLoadFileName()
        {
            // ���� ������ �� ���� �������, �� ���� ����������� � ��� �����
            if (pView.cbUsePeriod.Checked)
                pView.ugex1.SaveLoadFileName = String.Format("{1} {2} {0}", EnumHelper.GetEnumItemDescription(typeof(ModulesTypes), currentProtocol),
                pView.udteBeginPeriod.DateTime.ToString("yyyyMMdd"), pView.udteEndPeriod.DateTime.ToString("yyyyMMdd"));
            
            else
                pView.ugex1.SaveLoadFileName = EnumHelper.GetEnumItemDescription(typeof(ModulesTypes), currentProtocol);

        }
        /// <summary>
        ///  ����������, �������� ��� ��� ������ �� ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbUseEventType_CheckedChanged(object sender, EventArgs e)
        {
            if (pView.cbUseEventType.Checked)
            {
                pView.ucEvents.Enabled = true;
                pView.ucEvents.Rows[0].Activate();
                pView.ucEvents.Rows[0].Selected = true;
                pView.ucEvents.Text = pView.ucEvents.Rows[0].Cells[0].Text;

                int ImageIndex = Convert.ToInt32(pView.ucEvents.ActiveRow.Cells[0].Appearance.Image);
                pView.pbEventImage.Image = pView.ilColumns.Images[ImageIndex];
                pView.pbEventImage.Visible = true;
            }
            else
            {
                pView.ucEvents.Enabled = false;
                pView.ucEvents.Text = string.Empty;
                pView.ucEvents.Update();
                pView.pbEventImage.Visible = false;
            }
            if (currentProtocol == ModulesTypes.AuditModule)
                pView.ugeAudit.BurnRefreshDataButton(IsFilterChange());
            else
                pView.ugex1.BurnRefreshDataButton(IsFilterChange());
                
        }

        public override void InternalFinalize()
        {
            //UltraGridHelper.Save(this.GetType().FullName, pView.ugex1.ugData);
            base.InternalFinalize();
        }

        /// <summary>
        /// ���������� �������� �� ��� �� ������� ������ � �������������	
        /// </summary>
        private void ProtocolUserView_ButtonClick(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {

        }

        private bool CheckColumn(string columnName, ref UltraGridColumn clmn)
        {
            try
            {
                clmn = pView.ugex1._ugData.DisplayLayout.Bands[0].Columns[columnName];
            }
            catch
            {
                clmn = null;
            }
            return (clmn != null);
        }

        private void SetColumnsCaptions()
        {
            UltraGrid grid = pView.ugex1._ugData;
            foreach (UltraGridColumn clmn in grid.DisplayLayout.Bands[0].Columns)
            {

            }
        }

        private void CustomizeColumns()
        {
            try
            {
                UltraGridColumn clmn = null;

                if (CheckColumn("PumpHistoryID", ref clmn))
                {

                    clmn.Width = 100;

                }
                if (CheckColumn("DataSourceID", ref clmn))
                {

                    clmn.Width = 100;
                }

                if (CheckColumn("ObjectName", ref clmn))
                {
                    clmn.Hidden = true;
                }
                if (CheckColumn("ID", ref clmn))
                {
                    clmn.Width = 120;
                    clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                }
                if (CheckColumn("EventDateTime", ref clmn))
                {
                    clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    clmn.Format = "dd.MM.yyyy HH:mm:ss";
                    clmn.Width = 125;
                }
                if (CheckColumn("InfoMessage", ref clmn))
                {
                    clmn.Width = 400;
                    clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                }
                if (CheckColumn("UserName", ref clmn))
                {
                    clmn.Width = 150;
                    clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                    clmn.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    CC.UltraGridHelper.SetLikelyEditButtonColumnsStyle(clmn, -1);
                    clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                    clmn.CellButtonAppearance.Image = pView.ilFilter.Images[2];
                    clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                }
                if (CheckColumn("MDObjectName", ref clmn))
                {
                    clmn.Width = 120;
                }
                if (CheckColumn("BridgeRoleA", ref clmn))
                {
                    clmn.Width = 120;
                }
                if (CheckColumn("BridgeRoleB", ref clmn))
                {
                    clmn.Width = 120;
                }
            }
            finally
            {
                //pView.ugProtocol.EndUpdate();
            }
        }

        private void SetSortDirection(UltraGrid uGrid)
        {
            foreach (UltraGridBand band in uGrid.DisplayLayout.Bands)
            {
                band.Columns["ID"].SortIndicator = SortIndicator.Descending;
            }
        }

        private void ShowProtocolData()
        {
            this.Workplace.OperationObj.Text = "������ ������";
            this.Workplace.OperationObj.StartOperation();
            try
            {
                LogDataTable.Clear();
                if (currentProtocol != ModulesTypes.AuditModule)
                {
                    ShowLogForCurentModule(currentProtocol, DateFilterCondition, DateTimeFilterParams);
                    if (pView.ugex1.ugData.Rows.Count > 0)
                    {
                        pView.ugex1.ugData.Rows.GetRowAtVisibleIndex(0).Activate();
                    }
                }
                else
                {
                    ShowAuditData();
                    if (pView.ugeAudit.ugData.Rows.Count > 0)
                    {
                        pView.ugeAudit.ugData.Rows.GetRowAtVisibleIndex(0).Activate();
                    }
                }
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }

        }

        /// <summary>
        /// ����������� ��������� �� ����������� ������ � �����
        /// </summary>
        private void ShowLogForCurentModule(ModulesTypes ModuleNumber, string Filter, params IDbDataParameter[] FilterParams)
        {
            IBaseProtocol ViewProtocol = this.Workplace.ActiveScheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);

            // ��������� ������
            try
            {
                UltraGrid grid = null;
                ViewProtocol.GetProtocolData(ModuleNumber, ref LogDataTable, Filter, FilterParams);
                pView.ugex1.DataSource = LogDataTable;
                SetSortDirection(pView.ugex1.ugData);
                pView.ugex1.IsReadOnly = true;
                pView.ugex1.IsReadOnly = true;
                grid = pView.ugex1._ugData;

                pView.ugex1.StateRowEnable = true;

                if (LogDataTable.Rows.Count == 0)
                    this.Workplace.OperationObj.StopOperation();
            }
            finally
            {
                if (ViewProtocol != null)
                {
                    ViewProtocol.Dispose();
                    ViewProtocol = null;
                }
            }
        }

        /// <summary>
        ///  ���������� ��������� ���������� ������ � �������
        /// </summary>
        /// <param name="mt"></param>
        private void AddModuleEvents(ModulesTypes mt)
        {
            eventsFilterTable.Rows.Clear();
            switch (mt)
            {
                case ModulesTypes.DataPumpModule:
                    #region DataPumpModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("������ �������� �������", 101);
                    eventsFilterTable.Rows.Add("���������� � ��������", 102);
                    eventsFilterTable.Rows.Add("��������������", 103);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� �������", 104);
                    eventsFilterTable.Rows.Add("��������� �������� ������� � �������", 105);
                    eventsFilterTable.Rows.Add("������ � �������� �������", 106);
                    eventsFilterTable.Rows.Add("����������� ������ � �������� �������", 107);
                    eventsFilterTable.Rows.Add("������ ������� �����", 108);
                    eventsFilterTable.Rows.Add("���������� ������� ����� � �������", 109);
                    eventsFilterTable.Rows.Add("�������� ���������� ������� �����", 110);
                    eventsFilterTable.Rows.Add("������ ��������� ��������� ������", 111);
                    eventsFilterTable.Rows.Add("���������� ��������� ��������� ������ � �������", 112);
                    eventsFilterTable.Rows.Add("�������� ���������� ��������� ��������� ������", 113);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    pView.ucEvents.Rows[10].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[10].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[11].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[11].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[12].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[12].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.BridgeOperationsModule:
                    #region BridgeOperationsModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("������ �������� �������������", 201);
                    eventsFilterTable.Rows.Add("���������� � ��������", 202);
                    eventsFilterTable.Rows.Add("��������������", 203);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� �������������", 204);
                    eventsFilterTable.Rows.Add("��������� �������� ������������� � �������", 205);
                    eventsFilterTable.Rows.Add("������ � �������� �������������", 206);
                    eventsFilterTable.Rows.Add("����������� ������ � �������� �������������", 207);
                    eventsFilterTable.Rows.Add("������ ��������� ��������� ������", 211);
                    eventsFilterTable.Rows.Add("���������� ��������� ��������� ������ � �������", 212);
                    eventsFilterTable.Rows.Add("�������� ���������� ��������� ��������� ������", 213);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.MDProcessingModule:
                    #region MDProcessingModule
                    //eventsFilterTable.Rows.Clear();

                    FillTypeEvents(typeof (MDProcessingEventKind));

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.UsersOperationsModule:
                    #region UsersOperationsModule
                   // eventsFilterTable.Rows.Clear();
                    eventsFilterTable.Rows.Add("������������ ����������� � �����", 401);
                    
                    eventsFilterTable.Rows.Add("������������ ���������� �� �����", 402);

                    eventsFilterTable.Rows.Add("��������� ������� �������������", 40301);
                    
                    eventsFilterTable.Rows.Add("��������� ������� ����� �������������", 40302);

                    eventsFilterTable.Rows.Add("��������� ������� �������", 40303);

                    eventsFilterTable.Rows.Add("��������� ������� �����������", 40304);

                    eventsFilterTable.Rows.Add("��������� ������� ����� �����", 40305);
                    
                    eventsFilterTable.Rows.Add("��������� ��������� ������������ � ������", 40306);

                    eventsFilterTable.Rows.Add("�������������� ������", 40666);
                    // ���������� �����
                    eventsFilterTable.Rows.Add("���������� �����", 40501);
                    // ������������� ����������
                    eventsFilterTable.Rows.Add("������������� ����������", 40701);

                    //int i = pView.ucEvents.Rows.Count;

                    //������������ ����������� � �����
                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 6;
                   // pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    //������������ ���������� �� �����
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 7;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    //��������� ������� �������������
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 12;
                   // pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    //��������� ������� �����
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 13;
                   // pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    //��������� ������� �������
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 15;
                   // pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    //��������� ������� �����������
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 15;
                   // pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    //��������� ������� ����� �����
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 15;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    //��������� ��������� ������������ � ������
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 16;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    //�������������� ������
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    //���������� �����
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 21;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    //������������� ����������
                    pView.ucEvents.Rows[10].Cells[0].Appearance.Image = 3;
                   // pView.ucEvents.Rows[10].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.SystemEventsModule:
                    #region SystemEventsModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("����������", 502);
                    eventsFilterTable.Rows.Add("������", 506);
                    eventsFilterTable.Rows.Add("����������� ������", 507);
                    eventsFilterTable.Rows.Add("��������������", 503);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.ReviseDataModule:
                    #region ReviseDataModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("������ �������� ������ ������", 601);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� ������ ������", 604);
                    eventsFilterTable.Rows.Add("��������� �������� ������ ������ � �������", 605);
                    eventsFilterTable.Rows.Add("����������� ������ � �������� ������ ������", 607);
                    eventsFilterTable.Rows.Add("������ � �������� ������ ������", 606);
                    eventsFilterTable.Rows.Add("���������� � ��������", 602);
                    eventsFilterTable.Rows.Add("��������������", 603);
                    eventsFilterTable.Rows.Add("������ ��������� ��������� ������", 611);
                    eventsFilterTable.Rows.Add("���������� ��������� ��������� ������ � �������", 612);
                    eventsFilterTable.Rows.Add("�������� ���������� ��������� ��������� ������", 613);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.ProcessDataModule:
                    #region ProcessDataModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("������ �������� ��������� ������", 701);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� ��������� ������", 704);
                    eventsFilterTable.Rows.Add("��������� �������� ��������� ������ � �������", 705);
                    eventsFilterTable.Rows.Add("����������� ������ � �������� ��������� ������", 707);
                    eventsFilterTable.Rows.Add("������ � �������� ��������� ������", 706);
                    eventsFilterTable.Rows.Add("���������� � �������� ��������� ������", 702);
                    eventsFilterTable.Rows.Add("��������������", 703);
                    eventsFilterTable.Rows.Add("������ ��������� ��������� ������", 711);
                    eventsFilterTable.Rows.Add("���������� ��������� ��������� ������ � �������", 712);
                    eventsFilterTable.Rows.Add("�������� ���������� ��������� ��������� ������", 713);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.DeleteDataModule:
                    #region DeleteDataModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("������ �������� �������� ������", 801);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� �������� ������", 804);
                    eventsFilterTable.Rows.Add("��������� �������� �������� ������ � �������", 805);
                    eventsFilterTable.Rows.Add("����������� ������ � �������� �������� ������", 807);
                    eventsFilterTable.Rows.Add("������ � �������� �������� ������", 806);
                    eventsFilterTable.Rows.Add("���������� � �������� �������� ������", 802);
                    eventsFilterTable.Rows.Add("��������������", 803);
                    eventsFilterTable.Rows.Add("������ ��������� ��������� ������", 811);
                    eventsFilterTable.Rows.Add("���������� ��������� ��������� ������ � �������", 812);
                    eventsFilterTable.Rows.Add("�������� ���������� ��������� ��������� ������", 813);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.PreviewDataModule:
                    #region PreviewDataModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("������ �������� ������������� ������", 901);
                    eventsFilterTable.Rows.Add("���������� � ��������", 902);
                    eventsFilterTable.Rows.Add("��������������", 903);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� ������������� ������", 904);
                    eventsFilterTable.Rows.Add("��������� �������� ������������� ������ � �������", 905);
                    eventsFilterTable.Rows.Add("������ � �������� ������������� ������", 906);
                    eventsFilterTable.Rows.Add("����������� ������ � �������� ������������� ������", 907);
                    eventsFilterTable.Rows.Add("������ ������� �����", 908);
                    eventsFilterTable.Rows.Add("���������� ������� ����� � �������", 909);
                    eventsFilterTable.Rows.Add("�������� ���������� ������� �����", 910);
                    eventsFilterTable.Rows.Add("������ ��������� ��������� ������", 911);
                    eventsFilterTable.Rows.Add("���������� ��������� ��������� ������ � �������", 912);
                    eventsFilterTable.Rows.Add("�������� ���������� ��������� ��������� ������", 913);


                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    pView.ucEvents.Rows[10].Cells[0].Appearance.Image = 8;
                    //pView.ucEvents.Rows[10].Cells[0].ToolTipText = pView.ucEvents.Rows[10].Cells[0].Text;
                    pView.ucEvents.Rows[11].Cells[0].Appearance.Image = 10;
                    //pView.ucEvents.Rows[11].Cells[0].ToolTipText = pView.ucEvents.Rows[11].Cells[0].Text;
                    pView.ucEvents.Rows[12].Cells[0].Appearance.Image = 9;
                    //pView.ucEvents.Rows[12].Cells[0].ToolTipText = pView.ucEvents.Rows[12].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.ClassifiersModule:
                    #region ��������������
                    //eventsFilterTable.Rows.Clear();
                    
                    eventsFilterTable.Rows.Add("���������� � ��������", 1001);
                    eventsFilterTable.Rows.Add("��������������", 1002);
                    eventsFilterTable.Rows.Add("������", 1003);
                    eventsFilterTable.Rows.Add("����������� ������", 1004);
                    eventsFilterTable.Rows.Add("������ �������� ��������� ��������", 1005);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� ��������� ��������", 1006);
                    eventsFilterTable.Rows.Add("��������� �������� ��������� �������� � �������", 1007);
                    eventsFilterTable.Rows.Add("������ �������� ������� ������", 1008);
                    eventsFilterTable.Rows.Add("������ �������� �������", 1009);
                    eventsFilterTable.Rows.Add("������ �������� ������������ ������������� ��������������", 1010);
                    eventsFilterTable.Rows.Add("�������� ��������� ��������", 1011);
                    // ��������
                    eventsFilterTable.Rows.Add("������� ����������", 100401);
                    eventsFilterTable.Rows.Add("������� ������ ��� ���������", 100403);
                    eventsFilterTable.Rows.Add("������� ������ �� ���������", 100402);
                    eventsFilterTable.Rows.Add("������� ������", 100404);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 2;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 5;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    pView.ucEvents.Rows[10].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[10].Cells[0].ToolTipText = pView.ucEvents.Rows[10].Cells[0].Text;
                    // �������� 
                    pView.ucEvents.Rows[11].Cells[0].Appearance.Image = 17;
                    //pView.ucEvents.Rows[11].Cells[0].ToolTipText = pView.ucEvents.Rows[11].Cells[0].Text;
                    pView.ucEvents.Rows[12].Cells[0].Appearance.Image = 18;
                    //pView.ucEvents.Rows[12].Cells[0].ToolTipText = pView.ucEvents.Rows[12].Cells[0].Text;
                    pView.ucEvents.Rows[13].Cells[0].Appearance.Image = 19;
                    //pView.ucEvents.Rows[13].Cells[0].ToolTipText = pView.ucEvents.Rows[13].Cells[0].Text;
                    pView.ucEvents.Rows[14].Cells[0].Appearance.Image = 20;
                    //pView.ucEvents.Rows[14].Cells[0].ToolTipText = pView.ucEvents.Rows[14].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.UpdateModule:
                    #region ���������� �����
                    eventsFilterTable.Rows.Add("������ �������� ���������� �����", 50001);
                    eventsFilterTable.Rows.Add("���������� � ��������", 50000);
                    eventsFilterTable.Rows.Add("�������� ��������� �������� ���������� �����", 50002);
                    eventsFilterTable.Rows.Add("��������� �������� ���������� ����� � �������", 50003);

                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 0;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 1;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 3;
                    //pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.DataSourceModule:
                    #region ���������� ����������
                    eventsFilterTable.Rows.Add("�������� ������ ��� ���������", 1017);
                    eventsFilterTable.Rows.Add("�������� ������ �� ���������", 1016);
                    eventsFilterTable.Rows.Add("�������� ������", 1018);
                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 18;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 19;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 20;
                    #endregion
                    break;
                case ModulesTypes.TransferDBToNewYearModule:
                    #region ������� �������� �� ����� ���

                    eventsFilterTable.Rows.Add("�������� ���������", 1101);
                    eventsFilterTable.Rows.Add("������� ��������������", 1102);
                    eventsFilterTable.Rows.Add("������ ��������������", 1103);
                    eventsFilterTable.Rows.Add("���������� �� ������ ���������", 1104);
                    eventsFilterTable.Rows.Add("���������� �� ������ ����", 1105);
                    eventsFilterTable.Rows.Add("��������������", 1106);
                    eventsFilterTable.Rows.Add("������", 1107);
                    eventsFilterTable.Rows.Add("������ ������ ������� �������� ���� �� ����� ���", 1108);
                    eventsFilterTable.Rows.Add("��������� ������ ������� �������� ���� �� ����� ���", 1109);

                    #endregion
                    break;
                case ModulesTypes.MessagesExchangeModule:
                    eventsFilterTable.Rows.Add("�������� ������ ��������� �� ��������������", 1201);
                    eventsFilterTable.Rows.Add("�������� ���������", 1202);
                    eventsFilterTable.Rows.Add("������� ������������ ���������", 1203);
                    eventsFilterTable.Rows.Add("�������� ��������� �� ���������� ������� �����", 1204);
                    eventsFilterTable.Rows.Add("�������� ��������� (������)", 1210);
                    eventsFilterTable.Rows.Add("������ ��� �������� ���������", 1211);
                    break;
            }
        }

        /// <summary>
        /// ��������� ������ ������� �� ���������� �� ����������� ����.
        /// </summary>
        /// <param name="type">��� ������������.</param>
        private void FillTypeEvents(Type type)
        {
            foreach (int value in Enum.GetValues(type))
            {
                string name = Enum.GetName(type, value);
                FieldInfo fi = type.GetField(name);
                DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                                                                    fi, typeof(DescriptionAttribute));
                if (da != null)
                {
                    eventsFilterTable.Rows.Add(da.Description, value);
                }
                else
                {
                    eventsFilterTable.Rows.Add(name, value);
                }
            }
        }

        /// <summary>
        ///  �������� ��� �������, ������� ���� ������� � �������
        /// </summary>
        private int GetEventType()
        {
            if (pView.ucEvents.ActiveRow != null)

                return Convert.ToInt32(pView.ucEvents.ActiveRow.Cells[1].Value);
            else
                return -1;
        }

        private string LogCaption = string.Empty;

        public override void Activate(bool Activated)
        {
            if (Activated)
            {
                //UltraGridHelper.Load(this.GetType().FullName, pView.ugex1.ugData);
                Workplace.ViewObjectCaption = LogCaption;
            }
            else
            {
                //UltraGridHelper.Save(this.GetType().FullName, pView.ugex1.ugData);
            }
        }


        /// <summary>
        /// ���������� ����������� �� �������
        /// </summary>
        internal void LoadData()
        {
            pView.cbUseEventType.Checked = false;
            pView.utcProtocols.SelectedTab = pView.utcProtocols.Tabs[0];

            LogCaption = EnumHelper.GetEnumItemDescription(typeof(ModulesTypes), currentProtocol);
            ViewCtrl.Text = LogCaption;

            if (currentProtocol == ModulesTypes.AuditModule)
            {
                pView.utcProtocols.SelectedTab = pView.utcProtocols.Tabs[1];
                pView.ugeAudit.SaveLoadFileName = LogCaption;
                if (pView.ugeAudit.DataSource != null)
                    return;
            }

            if (!inInplaceMode)
            {
                Workplace.ViewObjectCaption = LogCaption;
                ViewCtrl.Text = LogCaption;
                AddModuleEvents(currentProtocol);
                ChangeDateTimeFilterCondition();
                ShowProtocolData();
                GetSaveLoadFileName();
            }
        }

        public override void ReloadData()
        {
        }

        private bool ugex1_OnRefreshData(object sender)
        {
            if (!inInplaceMode)
            {
                ChangeDateTimeFilterCondition();
                ShowProtocolData();
                SetCurrentFilterState();
            }
            else
            {
                if (AttachModuleType == ModulesTypes.AuditModule)
                    RefreshAttachAuditData();
                else
                    RefreshAttachData();
            }
            return true;
        }

        /// <summary>
        /// ���������� ������ � XML
        /// </summary>
        private bool ugex1_OnSaveToXML(object sender)
        {
            //IXmlExportImporter exporter = this.Workplace.ActiveScheme.GetXMLExportImporter();
            try
            {
                // �������� ������������� ���������� �����
                if (String.Compare(pView.ugex1.utmMain.Tools["Refresh"].SharedProps.AppearancesSmall.Appearance.BackColor.Name,"0") != 0)
                    ugex1_OnRefreshData(sender); 
              
                DataTable dt = new DataTable();
                
                if (InInplaceMode)
                    dt = AttachLogDataTable.Copy();
                else   
                    dt = LogDataTable.Copy();
                 
                // ��������� ���� �� ������ ��� ����������
                if (dt.Rows.Count != 0)
                     {
                         // ��������� ������ � DataSet, ������� ����� � ����
                         DataSet ds = new DataSet(currentProtocol.ToString());
                         ds.Tables.Add(dt);
                         ExportImportHelper.SaveToXML(ds, pView.ugex1.SaveLoadFileName);
                     }
                else 
                    MessageBox.Show("��� ������ ��� ������.", "��������������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            finally
            {
                //exporter.Dispose();
            }
            return true;
        }


        /// <summary>
        /// ��������� �������� ������� �� ���� � �������� � ����������� �� �������� �������
        /// </summary>
        private void ChangeDateTimeFilterCondition()
        {
            // ���� ��������� ���� ������ �������� �������� �� �������
            if (pView.udteBeginPeriod.DateTime > pView.udteEndPeriod.DateTime)
            {
                DateTime tmpTime = pView.udteBeginPeriod.DateTime;
                pView.udteBeginPeriod.Value = pView.udteEndPeriod.Value;
                pView.udteEndPeriod.Value = tmpTime;
            }

            DateFilterCondition = string.Empty;
            DateTime EndDate;
            int KindsOfEvents = 0;
            DateTimeFilterParams = null;

            //IDatabase tmpDB = Workplace.ActiveScheme.SchemeDWH.DB;
            // ���� ������ ������ ������ �� ����
            if (pView.cbUsePeriod.Checked)
            {
                EndDate = pView.udteEndPeriod.DateTime.AddDays(1);
                DateFilterCondition = "(dpp.EventDateTime between ? and ?)";
                try
                {
                    DateTimeFilterParams = new IDbDataParameter[2];
                    DateTimeFilterParams[0] = new System.Data.OleDb.OleDbParameter("p0", pView.udteBeginPeriod.DateTime);
                    DateTimeFilterParams[1] = new System.Data.OleDb.OleDbParameter("p1", EndDate);
                    //DateTimeFilterParams[0] = tmpDB.CreateParameter("p0", pView.udteBeginPeriod.DateTime, DbType.DateTime);
                    //DateTimeFilterParams[1] = tmpDB.CreateParameter("p1", EndDate, DbType.DateTime);
                }
                finally
                {
                    //tmpDB.Dispose();
                }
            }
            // ���� ������ ������ ������ �� ��������, ��������� ������ �� ���� ����� �������
            // ���� ��� ������ ��� � ������ �� ����, �� ��������� ������� � ����� ������
            if (pView.cbUseEventType.Checked)
            {
                KindsOfEvents = GetEventType();
                // ���� ����� �� ������� � ������ ������� �������
                if (KindsOfEvents >= 0)
                    if (pView.cbUsePeriod.Checked)
                        DateFilterCondition = DateFilterCondition + string.Format(" and dpp.KindsOfEvents = {0}", KindsOfEvents);
                    else
                    {
                        DateFilterCondition = string.Format("dpp.KindsOfEvents = {0}", KindsOfEvents);
                    }
                //if (tmpDB != null)
                //    tmpDB.Dispose();
            }
        }

        /// <summary>
        /// ���������� ������ � ������ � �����
        /// </summary>
        private void ProtocolView_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            UltraGridCell iniCell = e.Row.Cells["pic"];
            iniCell.Column.AutoSizeMode = ColumnAutoSizeMode.None;
            iniCell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            string message = string.Empty;
            switch (System.Convert.ToInt32(e.Row.Cells["KindsOfEvents"].Value))
            {
                #region DataPumpEventKind
                // ����������� ��������� ��� ��������� ������� ������
                case (int)DataPumpEventKind.dpeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; // ����� �������� � �����
                    iniCell.Value = "������ �������� �������";
                    iniCell.ToolTipText = "������ �������� �������";
                    break;
                case (int)DataPumpEventKind.dpeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � ��������";
                    iniCell.ToolTipText = "���������� � ��������";
                    break;
                case (int)DataPumpEventKind.dpeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)DataPumpEventKind.dpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� �������";
                    iniCell.ToolTipText = "�������� ��������� �������� �������";
                    break;
                case (int)DataPumpEventKind.dpeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� ������� � �������";
                    iniCell.ToolTipText = "��������� �������� ������� � �������";
                    break;
                case (int)DataPumpEventKind.dpeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������ � �������� �������";
                    iniCell.ToolTipText = "������ � �������� �������";
                    break;
                case (int)DataPumpEventKind.dpeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������ � �������� �������";
                    iniCell.ToolTipText = "����������� ������ � �������� �������";
                    break;
                case (int)DataPumpEventKind.dpeStartFilePumping:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ������� �����";
                    iniCell.ToolTipText = "������ ������� �����";
                    break;
                case (int)DataPumpEventKind.dpeSuccessfullFinishFilePump:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ������� �����";
                    iniCell.ToolTipText = "�������� ���������� ������� �����";
                    break;
                case (int)DataPumpEventKind.dpeFinishFilePumpWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ������� ����� � �������";
                    iniCell.ToolTipText = "���������� ������� ����� � �������";
                    break;
                case (int)DataPumpEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ��������� ��������� ������";
                    iniCell.ToolTipText = "������ ��������� ��������� ������";
                    break;
                case (int)DataPumpEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� �����";
                    iniCell.ToolTipText = "�������� ���������� ��������� �����";
                    break;
                case (int)DataPumpEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ��������� ������ � �������";
                    iniCell.ToolTipText = "���������� ��������� ��������� ������ � �������";
                    break;
                #endregion

                #region BridgeOperationsEventKind
                // ����������� ��������� ��� ��������� ������������� ������
                case (int)BridgeOperationsEventKind.boeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "������ �������� �������������";
                    iniCell.ToolTipText = "������ �������� �������������";
                    break;
                case (int)BridgeOperationsEventKind.boeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � ��������";
                    iniCell.ToolTipText = "���������� � ��������";
                    break;
                case (int)BridgeOperationsEventKind.boeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)BridgeOperationsEventKind.boeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� �������������";
                    iniCell.ToolTipText = "�������� ��������� �������� �������������";
                    break;
                case (int)BridgeOperationsEventKind.boeFinishedWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� ������������� � �������";
                    iniCell.ToolTipText = "��������� �������� ������������� � �������";
                    break;
                case (int)BridgeOperationsEventKind.boeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������ � �������� �������������";
                    iniCell.ToolTipText = "������ � �������� �������������";
                    break;
                case (int)BridgeOperationsEventKind.boeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������ � �������� �������������";
                    iniCell.ToolTipText = "����������� ������ � �������� �������������";
                    break;
                case (int)BridgeOperationsEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ��������� ��������� ������";
                    iniCell.ToolTipText = "������ ��������� ��������� ������";
                    break;
                case (int)BridgeOperationsEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� ��������� ������";
                    iniCell.ToolTipText = "�������� ���������� ��������� ��������� ������";
                    break;
                case (int)BridgeOperationsEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ��������� ������ � �������";
                    iniCell.ToolTipText = "���������� ��������� ��������� ������ � �������";
                    break;
                #endregion

                #region UsersOperationEventKind
                // ����������� ��������� ��� ��������� �������� �������������
                case (int)UsersOperationEventKind.uoeUserConnectToScheme:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[6]; //6;
                    iniCell.Value = "������������ ����������� � �����";
                    iniCell.ToolTipText = "������������ ����������� � �����";
                    break;
                case (int)UsersOperationEventKind.uoeUserDisconnectFromScheme:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[7]; //7;
                    iniCell.Value = "������������ ���������� �� �����";
                    iniCell.ToolTipText = "������������ ���������� �� �����";
                    break;
                case (int)UsersOperationEventKind.uoeChangeUsersTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[12]; //7;
                    iniCell.Value = "��������� ������� �������������";
                    iniCell.ToolTipText = "��������� ������� �������������";
                    break;
                case (int)UsersOperationEventKind.uoeChangeGroupsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[13]; //7;
                    iniCell.Value = "��������� ������� ����� �������������";
                    iniCell.ToolTipText = "��������� ������� ����� �������������";
                    break;
                case (int)UsersOperationEventKind.uoeChangeDepartmentsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[15]; //7;
                    iniCell.Value = "��������� ������� �������";
                    iniCell.ToolTipText = "��������� ������� �������";
                    break;
                case (int)UsersOperationEventKind.uoeChangeOrganizationsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[15]; //7;
                    iniCell.Value = "��������� ������� �����������";
                    iniCell.ToolTipText = "��������� ������� �����������";
                    break;
                case (int)UsersOperationEventKind.uoeChangeTasksTypes:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[15]; //7;
                    iniCell.Value = "��������� ������� ����� �����";
                    iniCell.ToolTipText = "��������� ������� ����� �����";
                    break;
                case (int)UsersOperationEventKind.uoeChangeMembershipsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[16]; //7;
                    iniCell.Value = "��������� ��������� ������������ � ������";
                    iniCell.ToolTipText = "��������� ��������� ������������ � ������";
                    break;
                case (int)UsersOperationEventKind.uoeUntilledExceptionsEvent:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //7;
                    iniCell.Value = "�������������� ������";
                    iniCell.ToolTipText = "�������������� ������";
                    break;
                case (int)UsersOperationEventKind.uoeSchemeUpdate:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[21]; //7;
                    iniCell.Value = "���������� �����";
                    iniCell.ToolTipText = "���������� �����";
                    break;
                case (int)UsersOperationEventKind.uoeProtocolsToArchive:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //7;
                    iniCell.Value = "��������� ��������������";
                    iniCell.ToolTipText = "��������� ��������������";
                    break;
                #endregion

                #region SystemEventKind
                case (int)SystemEventKind.seInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "����������";
                    iniCell.ToolTipText = "����������";
                    break;
                case (int)SystemEventKind.seError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "������";
                    iniCell.ToolTipText = "������";
                    break;
                case (int)SystemEventKind.seCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������";
                    iniCell.ToolTipText = "����������� ������";
                    break;
                case (int)SystemEventKind.seWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                #endregion

                #region ProcessDataEventKind
                case (int)ProcessDataEventKind.pdeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "������ �������� ��������� ������";
                    iniCell.ToolTipText = "������ �������� ��������� ������";
                    break;
                case (int)ProcessDataEventKind.pdeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� ��������� ������";
                    iniCell.ToolTipText = "�������� ��������� �������� ��������� ������";
                    break;
                case (int)ProcessDataEventKind.pdeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� ��������� ������ � �������";
                    iniCell.ToolTipText = "��������� �������� ��������� ������ � �������";
                    break;
                case (int)ProcessDataEventKind.pdeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������ � �������� ��������� ������";
                    iniCell.ToolTipText = "����������� ������ � �������� ��������� ������";
                    break;
                case (int)ProcessDataEventKind.pdeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������ � �������� ��������� ������";
                    iniCell.ToolTipText = "������ � �������� ��������� ������";
                    break;
                case (int)ProcessDataEventKind.pdeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � �������� ��������� ������";
                    iniCell.ToolTipText = "���������� � �������� ��������� ������";
                    break;
                case (int)ProcessDataEventKind.pdeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)ProcessDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ��������� ��������� ������";
                    iniCell.ToolTipText = "������ ��������� ��������� ������";
                    break;
                case (int)ProcessDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� �����";
                    iniCell.ToolTipText = "�������� ���������� ��������� �����";
                    break;
                case (int)ProcessDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ��������� ������ � �������";
                    iniCell.ToolTipText = "���������� ��������� ��������� ������ � �������";
                    break;
                #endregion

                #region DeleteDataEventKind
                case (int)DeleteDataEventKind.ddeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "������ �������� �������� ������";
                    iniCell.ToolTipText = "������ �������� �������� ������";
                    break;
                case (int)DeleteDataEventKind.ddeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� �������� ������";
                    iniCell.ToolTipText = "�������� ��������� �������� �������� ������";
                    break;
                case (int)DeleteDataEventKind.ddeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� �������� ������ � �������";
                    iniCell.ToolTipText = "��������� �������� �������� ������ � �������";
                    break;
                case (int)DeleteDataEventKind.ddeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������ � �������� �������� ������";
                    iniCell.ToolTipText = "����������� ������ � �������� �������� ������";
                    break;
                case (int)DeleteDataEventKind.ddeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������ � �������� �������� ������";
                    iniCell.ToolTipText = "������ � �������� �������� ������";
                    break;
                case (int)DeleteDataEventKind.ddeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � �������� �������� ������";
                    iniCell.ToolTipText = "���������� � �������� �������� ������";
                    break;
                case (int)DeleteDataEventKind.ddeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)DeleteDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ��������� ��������� ������";
                    iniCell.ToolTipText = "������ ��������� ��������� ������";
                    break;
                case (int)DeleteDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� �����";
                    iniCell.ToolTipText = "�������� ���������� ��������� �����";
                    break;
                case (int)DeleteDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ��������� ������ � �������";
                    iniCell.ToolTipText = "���������� ��������� ��������� ������ � �������";
                    break;
                #endregion

                #region MDProcessingEventKind
                case (int)MDProcessingEventKind.mdpeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "������ �������� ��������� ����������� ��������";
                    iniCell.ToolTipText = "������ �������� ��������� ����������� ��������";
                    break;
                case (int)MDProcessingEventKind.mdpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� ��������� ����������� ��������";
                    iniCell.ToolTipText = "�������� ��������� �������� ��������� ����������� ��������";
                    break;
                case (int)MDProcessingEventKind.mdpeFinishedWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� ��������� ����������� �������� � �������";
                    iniCell.ToolTipText = "��������� �������� ��������� ����������� �������� � �������";
                    break;
                case (int)MDProcessingEventKind.mdpeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������ � �������� ��������� ����������� ��������";
                    iniCell.ToolTipText = "����������� ������ � �������� ��������� ����������� ��������";
                    break;
                case (int)MDProcessingEventKind.mdpeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������ � �������� ��������� ����������� ��������";
                    iniCell.ToolTipText = "������ � �������� ��������� ����������� ��������";
                    break;
                case (int)MDProcessingEventKind.mdpeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � �������� ��������� ����������� ��������";
                    iniCell.ToolTipText = "���������� � �������� ��������� ����������� ��������";
                    break;
                case (int)MDProcessingEventKind.mdpeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)MDProcessingEventKind.InvalidateObject:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //8;
                    iniCell.Value = "���������� �� ������";
                    iniCell.ToolTipText = "���������� �� ������";
                    break;
                /*case (int)MDProcessingEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� �����";
                    iniCell.ToolTipText = "�������� ���������� ��������� �����";
                    break;
                case (int)MDProcessingEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ��������� ������ � �������";
                    iniCell.ToolTipText = "���������� ��������� ��������� ������ � �������";
                    break;*/
                #endregion

                #region ReviseDataEventKind
                case (int)ReviseDataEventKind.pdeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "������ �������� �������� ������";
                    iniCell.ToolTipText = "������ �������� �������� ������";
                    break;
                case (int)ReviseDataEventKind.pdeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� �������� ������";
                    iniCell.ToolTipText = "�������� ��������� �������� �������� ������";
                    break;
                case (int)ReviseDataEventKind.pdeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� �������� ������ � �������";
                    iniCell.ToolTipText = "��������� �������� �������� ������ � �������";
                    break;
                case (int)ReviseDataEventKind.pdeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������ � �������� �������� ������";
                    iniCell.ToolTipText = "����������� ������ � �������� �������� ������";
                    break;
                case (int)ReviseDataEventKind.pdeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������ � �������� �������� �������";
                    iniCell.ToolTipText = "������ � �������� �������� �������";
                    break;
                case (int)ReviseDataEventKind.pdeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � �������� �������� ������";
                    iniCell.ToolTipText = "���������� � �������� �������� ������";
                    break;
                case (int)ReviseDataEventKind.pdeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)ReviseDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ��������� ��������� ������";
                    iniCell.ToolTipText = "������ ��������� ��������� ������";
                    break;
                case (int)ReviseDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� �����";
                    iniCell.ToolTipText = "�������� ���������� ��������� �����";
                    break;
                case (int)ReviseDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ��������� ������ � �������";
                    iniCell.ToolTipText = "���������� ��������� ��������� ������ � �������";
                    break;

                #endregion

                #region ������������ ������
                case (int)PreviewDataEventKind.dpeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; // ����� �������� � �����
                    iniCell.Value = "������ �������� ������������� ������";
                    iniCell.ToolTipText = "������ �������� ������������� ������";
                    break;
                case (int)PreviewDataEventKind.dpeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � ��������";
                    iniCell.ToolTipText = "���������� � ��������";
                    break;
                case (int)PreviewDataEventKind.dpeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)PreviewDataEventKind.dpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� ������������� ������";
                    iniCell.ToolTipText = "�������� ��������� �������� ������������� ������";
                    break;
                case (int)PreviewDataEventKind.dpeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� ������������� ������ � �������";
                    iniCell.ToolTipText = "��������� �������� ������������� ������ � �������";
                    break;
                case (int)PreviewDataEventKind.dpeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������ � �������� ������������� ������";
                    iniCell.ToolTipText = "������ � �������� ������������� ������";
                    break;
                case (int)PreviewDataEventKind.dpeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������ � �������� ������������� ������";
                    iniCell.ToolTipText = "����������� ������ � �������� ������������� ������";
                    break;
                case (int)PreviewDataEventKind.dpeStartFilePumping:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ��������� �����";
                    iniCell.ToolTipText = "������ ��������� �����";
                    break;
                case (int)PreviewDataEventKind.dpeSuccessfullFinishFilePump:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� �����";
                    iniCell.ToolTipText = "�������� ���������� ��������� �����";
                    break;
                case (int)PreviewDataEventKind.dpeFinishFilePumpWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ����� � �������";
                    iniCell.ToolTipText = "���������� ��������� ����� � �������";
                    break;
                case (int)PreviewDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "������ ��������� ��������� ������";
                    iniCell.ToolTipText = "������ ��������� ��������� ������";
                    break;
                case (int)PreviewDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "�������� ���������� ��������� �����";
                    iniCell.ToolTipText = "�������� ���������� ��������� �����";
                    break;
                case (int)PreviewDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "���������� ��������� ��������� ������ � �������";
                    iniCell.ToolTipText = "���������� ��������� ��������� ������ � �������";
                    break;
                #endregion

                #region BridgeOperationsEventKind
                // ����������� ��������� ��� ��������� ������������� ������
                case (int)ClassifiersEventKind.ceInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � ��������";
                    iniCell.ToolTipText = "���������� � ��������";
                    break;
                case (int)ClassifiersEventKind.ceWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)ClassifiersEventKind.ceSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� ��������";
                    iniCell.ToolTipText = "�������� ��������� ��������";
                    break;
                case (int)ClassifiersEventKind.ceStartHierarchySet:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //10;
                    iniCell.Value = "������ �������� ��������� ��������";
                    iniCell.ToolTipText = "������ �������� ��������� ��������";
                    break;
                case (int)ClassifiersEventKind.ceSuccessfullFinishHierarchySet:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //4;
                    iniCell.Value = "�������� ��������� �������� ��������� ��������";
                    iniCell.ToolTipText = "�������� ��������� �������� ��������� ��������";
                    break;
                case (int)ClassifiersEventKind.ceFinishHierarchySetWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� ��������� �������� � �������";
                    iniCell.ToolTipText = "��������� �������� ��������� �������� � �������";
                    break;
                case (int)ClassifiersEventKind.ceError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������";
                    iniCell.ToolTipText = "������";
                    break;
                case (int)ClassifiersEventKind.ceCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "����������� ������";
                    iniCell.ToolTipText = "����������� ������";
                    break;
                case (int)ClassifiersEventKind.ceClearClassifierData:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //8;
                    iniCell.Value = "������ �������� ������� ������";
                    iniCell.ToolTipText = "������ �������� ������� ������";
                    break;
                case (int)ClassifiersEventKind.ceImportClassifierData:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //9;
                    iniCell.Value = "������ �������� �������";
                    iniCell.ToolTipText = "������ �������� �������";
                    break;
                case (int)ClassifiersEventKind.ceCreateBridge:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "������ �������� ������������ ������������� ��������������";
                    iniCell.ToolTipText = "������ �������� ������������ ������������� ��������������";
                    break;
                case (int)ClassifiersEventKind.ceVariantCopy:
                    message = "������� ����������";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[17];
                    break;
                case (int)ClassifiersEventKind.ceVariantDelete:
                    message = "������� ������";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[20];
                    break;
                case (int)ClassifiersEventKind.ceVariantLock:
                    message = "������� ������ �� ���������";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[19];
                    break;
                case (int)ClassifiersEventKind.ceVariantUnlok:
                    message = "������� ������ ��� ���������";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[18];
                    break;
                
                #endregion

                #region ���������� �����
                case (int)UpdateSchemeEventKind.BeginUpdate:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //10;
                    iniCell.Value = "������ �������� ���������� �����";
                    iniCell.ToolTipText = "������ �������� ���������� �����";
                    break;
                case (int)UpdateSchemeEventKind.Information:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "���������� � ��������";
                    iniCell.ToolTipText = "���������� � ��������";
                    break;
                case (int)UpdateSchemeEventKind.EndUpdateSuccess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "�������� ��������� �������� ���������� �����";
                    iniCell.ToolTipText = "�������� ��������� �������� ���������� �����";
                    break;
                case (int)UpdateSchemeEventKind.EndUpdateWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "��������� �������� ���������� ����� � �������";
                    iniCell.ToolTipText = "��������� �������� ���������� ����� � �������";
                    break;
                #endregion

                #region ���������� ����������.
                case (int)DataSourceEventKind.ceSourceLock:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[19];
                    iniCell.Value = "�������� ������ �� ���������";
                    iniCell.ToolTipText = "�������� ������ �� ���������";
                    break;
                case (int)DataSourceEventKind.ceSourceUnlock:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[18];
                    iniCell.Value = "�������� ������ ��� ���������";
                    iniCell.ToolTipText = "�������� ������ ��� ���������";
                    break;
                case (int)DataSourceEventKind.ceSourceDelete:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[20];
                    iniCell.Value = "�������� ������";
                    iniCell.ToolTipText = "�������� ������";
                    break;

                #endregion

                #region ������� ���� �� ����� ���
                case (int)TransferDBToNewYearEventKind.tnyeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "������";
                    iniCell.ToolTipText = "������";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2];
                    iniCell.Value = "��������������";
                    iniCell.ToolTipText = "��������������";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeBegin:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0];
                    iniCell.Value = "������ ������ ������� �������� ���� �� ����� ���";
                    iniCell.ToolTipText = "������ ������ ������� �������� ���� �� ����� ���";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeEnd:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3];
                    iniCell.Value = "��������� ������ ������� �������� ���� �� ����� ���";
                    iniCell.ToolTipText = "��������� ������ ������� �������� ���� �� ����� ���";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeCreateSource:
                case (int)TransferDBToNewYearEventKind.tnyeExportClassifierData:
                case (int)TransferDBToNewYearEventKind.tnyeImportClassifierData:
                case (int)TransferDBToNewYearEventKind.tnyeInvalidateCube:
                case (int)TransferDBToNewYearEventKind.tnyeInvalidateDimension:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1];
                    iniCell.Value = "�������������� ���������";
                    iniCell.ToolTipText = "�������������� ���������";
                    break;
                #endregion

                #region ���������� ������ �����������

                case (int)MessagesEventKind.mekCreateAdmMessage:
                    e.Row.Appearance.BackColor = ColorTranslator.FromHtml("#CCFFFF");
                    break;
                case (int)MessagesEventKind.mekCreateCubeMessage:
                    e.Row.Appearance.BackColor = ColorTranslator.FromHtml("#FFFFCC");
                    break;
                case (int)MessagesEventKind.mekCreateOther:
                    e.Row.Appearance.BackColor = Color.FromArgb(
                       Int32.Parse("#CCCCCC".Replace("#", ""), NumberStyles.HexNumber));
                    break;
                case (int)MessagesEventKind.mekDeleteMessage:
                case (int)MessagesEventKind.mekRemoveObsoleteMessages:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1];
                    iniCell.Value = "�������������� ���������"; 
                    iniCell.ToolTipText = "�������������� ���������";
                    break;
                case (int)MessagesEventKind.mekSendError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4];
                    iniCell.Value = "������ ��� �������� ���������";
                    iniCell.ToolTipText = "������ ��� �������� ���������";
                    break;

                #endregion
            }

            UltraGridRow row = CC.UltraGridHelper.GetRowCells(e.Row);
            UltraGridCell cell = null;
            int? val = -1;

            if (currentProtocol == ModulesTypes.UpdateModule)
            {
                cell = row.Cells["ModificationType"];
                val = Convert.ToInt32(cell.Value);

                switch (val)
                {
                    case 0:
                        cell.Appearance.Image = this.pView.ugeAudit.il.Images[2];
                        cell.ToolTipText = "�������� ������ �������";
                        break;
                    case 1:
                        cell.Appearance.Image = this.pView.ugeAudit.il.Images[0];
                        cell.ToolTipText = "��������� ���������";
                        break;
                    case 2:
                        cell.Appearance.Image = this.pView.ugeAudit.il.Images[1];
                        cell.ToolTipText = "�������� ������������� �������";
                        break;
                }
            }

            if (currentProtocol == ModulesTypes.ClassifiersModule)
            {
                cell = row.Cells["OBJECTTYPE"];
                if (cell.Value != DBNull.Value && cell.Value != null)
                    val = Convert.ToInt32(cell.Value);
                switch (val)
                {
                    case 0:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[0];
                        cell.ToolTipText = "��� �������: '������������ �������������'";
                        break;
                    case 1:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[1];
                        cell.ToolTipText = "��� �������: '������������� ������'";
                        break;
                    case 2:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[2];
                        cell.ToolTipText = "��� �������: '������������� �������������'";
                        break;
                    case 3:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[3];
                        cell.ToolTipText = "��� �������: '������� ������'";
                        break;
                    case 4:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[4];
                        cell.ToolTipText = "��� �������: '��������� �������'";
                        break;
                }
            }
        }

    }
}
