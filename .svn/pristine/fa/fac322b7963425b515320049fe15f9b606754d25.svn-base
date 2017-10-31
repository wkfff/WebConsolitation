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
            Caption = "Протоколы";
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
            // настройки для грида аудита
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
            state.ColumnCaption = "ID записи протокола";
            state.ColumnName = "ID";
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "RefUsersOperations";
            state.IsHiden = true;
            states.Add("RefUsersOperations", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Операция закачки";
            state.ColumnName = "PumpHistoryID";
            states.Add("PumpHistoryID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "ID источника данных";
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
            state.ColumnCaption = "Дата/Время";
            state.ColumnName = "EventDateTime";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
            states.Add("EventDateTime", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Сообщение";
            state.ColumnName = "InfoMessage";
            state.ColumnWidth = 500;
            states.Add("InfoMessage", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Пользователь";
            state.ColumnName = "UserName";
            states.Add("UserName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = String.Empty;
            state.ColumnName = "KindsOfEvents";
            state.IsHiden = true;
            states.Add("KindsOfEvents", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Модуль";
            state.IsHiden = false;

            state.ColumnName = "MODULE";
            state.ColumnWidth = 150;
            states.Add("Module", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Имя объекта";
            state.ColumnName = "MDObjectName";
            states.Add("MDObjectName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Объект БД";
            state.ColumnName = "Classifier";
            states.Add("Classifier", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Сопоставляемый";
            state.ColumnName = "BridgeRoleA";
            states.Add("BridgeRoleA", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Сопоставимый";
            state.ColumnName = "BridgeRoleB";
            states.Add("BridgeRoleB", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Машина пользователя";
            state.ColumnName = "USERHOST";
            states.Add("USERHOST", state);

            //SessionID
            state = new CC.GridColumnState();
            state.ColumnCaption = "ID сессии";
            state.ColumnName = "SessionID";
            state.IsHiden = true;
            states.Add("SessionID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "ObjectFullName";
            state.IsHiden = true;
            states.Add("ObjectFullName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Имя объекта";
            state.ColumnName = "ObjectFullCaption";
            states.Add("ObjectFullCaption", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип модификации";
            state.ColumnName = "ModificationType";
            states.Add("ModificationType", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип объекта БД";
            state.ColumnName = "OBJECTTYPE";
            states.Add("OBJECTTYPE", state);

            //

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип объекта";
            state.ColumnName = "OlapObjectType";
            states.Add("OlapObjectType", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Идентификатор объекта";
            state.ColumnName = "ObjectIdentifier";
            states.Add("ObjectIdentifier", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Идентификатор пакета";
            state.ColumnName = "BatchId";
            states.Add("BatchId", state);

            cashedColumnsSettings.Add(currentProtocol.ToString(), states);

            return states;
        }

        // переменные для хранения предыдущего состояния фильтров
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
        ///  Определяет, активный или нет фильтр по дате
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
        ///  Получение используемого по умолчанию имени файла  
        /// </summary>
        void GetSaveLoadFileName()
        {
            // если фильтр по дате активен, то даты прописываем в имя файла
            if (pView.cbUsePeriod.Checked)
                pView.ugex1.SaveLoadFileName = String.Format("{1} {2} {0}", EnumHelper.GetEnumItemDescription(typeof(ModulesTypes), currentProtocol),
                pView.udteBeginPeriod.DateTime.ToString("yyyyMMdd"), pView.udteEndPeriod.DateTime.ToString("yyyyMMdd"));
            
            else
                pView.ugex1.SaveLoadFileName = EnumHelper.GetEnumItemDescription(typeof(ModulesTypes), currentProtocol);

        }
        /// <summary>
        ///  Определяет, активный или нет фильтр по событиям
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
        /// Обработчик перехода на лог по нажатию кнопки у пользователей	
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
            this.Workplace.OperationObj.Text = "Запрос данных";
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
        /// Отображение сообщений по конкретному модулю в гриде
        /// </summary>
        private void ShowLogForCurentModule(ModulesTypes ModuleNumber, string Filter, params IDbDataParameter[] FilterParams)
        {
            IBaseProtocol ViewProtocol = this.Workplace.ActiveScheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);

            // Получение данных
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
        ///  Заполнение событиями выпадающий список к фильтру
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

                    eventsFilterTable.Rows.Add("Начало операции закачки", 101);
                    eventsFilterTable.Rows.Add("Информация в процессе", 102);
                    eventsFilterTable.Rows.Add("Предупреждение", 103);
                    eventsFilterTable.Rows.Add("Успешное окончание операции закачки", 104);
                    eventsFilterTable.Rows.Add("Окончание операции закачки с ошибкой", 105);
                    eventsFilterTable.Rows.Add("Ошибка в процессе закачки", 106);
                    eventsFilterTable.Rows.Add("Критическая ошибка в процессе закачки", 107);
                    eventsFilterTable.Rows.Add("Начало закачки файла", 108);
                    eventsFilterTable.Rows.Add("Завершение закачки файла с ошибкой", 109);
                    eventsFilterTable.Rows.Add("Успешное завершение закачки файла", 110);
                    eventsFilterTable.Rows.Add("Начало обработки источника данных", 111);
                    eventsFilterTable.Rows.Add("Завершение обработки источника данных с ошибкой", 112);
                    eventsFilterTable.Rows.Add("Успешное завершение обработки источника данных", 113);

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

                    eventsFilterTable.Rows.Add("Начало операции сопоставления", 201);
                    eventsFilterTable.Rows.Add("Информация в процессе", 202);
                    eventsFilterTable.Rows.Add("Предупреждение", 203);
                    eventsFilterTable.Rows.Add("Успешное окончание операции сопоставления", 204);
                    eventsFilterTable.Rows.Add("Окончание операции сопоставления с ошибкой", 205);
                    eventsFilterTable.Rows.Add("Ошибка в процессе сопоставления", 206);
                    eventsFilterTable.Rows.Add("Критическая ошибка в процессе сопоставления", 207);
                    eventsFilterTable.Rows.Add("Начало обработки источника данных", 211);
                    eventsFilterTable.Rows.Add("Завершение обработки источника данных с ошибкой", 212);
                    eventsFilterTable.Rows.Add("Успешное завершение обработки источника данных", 213);

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
                    eventsFilterTable.Rows.Add("Пользователь подключился к схеме", 401);
                    
                    eventsFilterTable.Rows.Add("Пользователь отключился от схемы", 402);

                    eventsFilterTable.Rows.Add("Изменение таблицы пользователей", 40301);
                    
                    eventsFilterTable.Rows.Add("Изменение таблицы групп пользователей", 40302);

                    eventsFilterTable.Rows.Add("Изменение таблицы отделов", 40303);

                    eventsFilterTable.Rows.Add("Изменение таблицы организаций", 40304);

                    eventsFilterTable.Rows.Add("Изменение таблицы типов задач", 40305);
                    
                    eventsFilterTable.Rows.Add("Изменение вхождения пользователя в группу", 40306);

                    eventsFilterTable.Rows.Add("Необработанная ошибка", 40666);
                    // обновление схемы
                    eventsFilterTable.Rows.Add("Обновление схемы", 40501);
                    // архивирование протоколов
                    eventsFilterTable.Rows.Add("Архивирование протоколов", 40701);

                    //int i = pView.ucEvents.Rows.Count;

                    //Пользователь подключился к схеме
                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 6;
                   // pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    //Пользователь отключился от схемы
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 7;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    //Изменение таблицы пользователей
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 12;
                   // pView.ucEvents.Rows[2].Cells[0].ToolTipText = pView.ucEvents.Rows[2].Cells[0].Text;
                    //Изменение таблицы групп
                    pView.ucEvents.Rows[3].Cells[0].Appearance.Image = 13;
                   // pView.ucEvents.Rows[3].Cells[0].ToolTipText = pView.ucEvents.Rows[3].Cells[0].Text;
                    //Изменение таблицы отделов
                    pView.ucEvents.Rows[4].Cells[0].Appearance.Image = 15;
                   // pView.ucEvents.Rows[4].Cells[0].ToolTipText = pView.ucEvents.Rows[4].Cells[0].Text;
                    //Изменение таблицы организаций
                    pView.ucEvents.Rows[5].Cells[0].Appearance.Image = 15;
                   // pView.ucEvents.Rows[5].Cells[0].ToolTipText = pView.ucEvents.Rows[5].Cells[0].Text;
                    //Изменение таблицы типов задач
                    pView.ucEvents.Rows[6].Cells[0].Appearance.Image = 15;
                    //pView.ucEvents.Rows[6].Cells[0].ToolTipText = pView.ucEvents.Rows[6].Cells[0].Text;
                    //Изменение вхождения пользователя в группу
                    pView.ucEvents.Rows[7].Cells[0].Appearance.Image = 16;
                    //pView.ucEvents.Rows[7].Cells[0].ToolTipText = pView.ucEvents.Rows[7].Cells[0].Text;
                    //Необработанная ошибка
                    pView.ucEvents.Rows[8].Cells[0].Appearance.Image = 4;
                    //pView.ucEvents.Rows[8].Cells[0].ToolTipText = pView.ucEvents.Rows[8].Cells[0].Text;
                    //Обновление схемы
                    pView.ucEvents.Rows[9].Cells[0].Appearance.Image = 21;
                    //pView.ucEvents.Rows[9].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    //Архивирование протоколов
                    pView.ucEvents.Rows[10].Cells[0].Appearance.Image = 3;
                   // pView.ucEvents.Rows[10].Cells[0].ToolTipText = pView.ucEvents.Rows[9].Cells[0].Text;
                    #endregion
                    break;
                case ModulesTypes.SystemEventsModule:
                    #region SystemEventsModule
                    //eventsFilterTable.Rows.Clear();

                    eventsFilterTable.Rows.Add("Информация", 502);
                    eventsFilterTable.Rows.Add("Ошибка", 506);
                    eventsFilterTable.Rows.Add("Критическая ошибка", 507);
                    eventsFilterTable.Rows.Add("Предупреждение", 503);

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

                    eventsFilterTable.Rows.Add("Начало операции сверки данных", 601);
                    eventsFilterTable.Rows.Add("Успешное окончание операции сверки данных", 604);
                    eventsFilterTable.Rows.Add("Окончание операции сверки данных с ошибкой", 605);
                    eventsFilterTable.Rows.Add("Критическая ошибка в процессе сверки данных", 607);
                    eventsFilterTable.Rows.Add("Ошибка в процессе сверки данных", 606);
                    eventsFilterTable.Rows.Add("Информация в процессе", 602);
                    eventsFilterTable.Rows.Add("Предупреждение", 603);
                    eventsFilterTable.Rows.Add("Начало обработки источника данных", 611);
                    eventsFilterTable.Rows.Add("Завершение обработки источника данных с ошибкой", 612);
                    eventsFilterTable.Rows.Add("Успешное завершение обработки источника данных", 613);

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

                    eventsFilterTable.Rows.Add("Начало операции обработки данных", 701);
                    eventsFilterTable.Rows.Add("Успешное окончание операции обработки данных", 704);
                    eventsFilterTable.Rows.Add("Окончание операции обработки данных с ошибкой", 705);
                    eventsFilterTable.Rows.Add("Критическая ошибка в процессе обработки данных", 707);
                    eventsFilterTable.Rows.Add("Ошибка в процессе обработки данных", 706);
                    eventsFilterTable.Rows.Add("Информация в процессе обработки данных", 702);
                    eventsFilterTable.Rows.Add("Предупреждение", 703);
                    eventsFilterTable.Rows.Add("Начало обработки источника данных", 711);
                    eventsFilterTable.Rows.Add("Завершение обработки источника данных с ошибкой", 712);
                    eventsFilterTable.Rows.Add("Успешное завершение обработки источника данных", 713);

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

                    eventsFilterTable.Rows.Add("Начало операции удаления данных", 801);
                    eventsFilterTable.Rows.Add("Успешное окончание операции удаления данных", 804);
                    eventsFilterTable.Rows.Add("Окончание операции удаления данных с ошибкой", 805);
                    eventsFilterTable.Rows.Add("Критическая ошибка в процессе удаления данных", 807);
                    eventsFilterTable.Rows.Add("Ошибка в процессе удаления данных", 806);
                    eventsFilterTable.Rows.Add("Информация в процессе удаления данных", 802);
                    eventsFilterTable.Rows.Add("Предупреждение", 803);
                    eventsFilterTable.Rows.Add("Начало обработки источника данных", 811);
                    eventsFilterTable.Rows.Add("Завершение обработки источника данных с ошибкой", 812);
                    eventsFilterTable.Rows.Add("Успешное завершение обработки источника данных", 813);

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

                    eventsFilterTable.Rows.Add("Начало операции предпросмотра данных", 901);
                    eventsFilterTable.Rows.Add("Информация в процессе", 902);
                    eventsFilterTable.Rows.Add("Предупреждение", 903);
                    eventsFilterTable.Rows.Add("Успешное окончание операции предпросмотра данных", 904);
                    eventsFilterTable.Rows.Add("Окончание операции предпросмотра данных с ошибкой", 905);
                    eventsFilterTable.Rows.Add("Ошибка в процессе предпросмотра данных", 906);
                    eventsFilterTable.Rows.Add("Критическая ошибка в процессе предпросмотра данных", 907);
                    eventsFilterTable.Rows.Add("Начало закачки файла", 908);
                    eventsFilterTable.Rows.Add("Завершение закачки файла с ошибкой", 909);
                    eventsFilterTable.Rows.Add("Успешное завершение закачки файла", 910);
                    eventsFilterTable.Rows.Add("Начало обработки источника данных", 911);
                    eventsFilterTable.Rows.Add("Завершение обработки источника данных с ошибкой", 912);
                    eventsFilterTable.Rows.Add("Успешное завершение обработки источника данных", 913);


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
                    #region Классификаторы
                    //eventsFilterTable.Rows.Clear();
                    
                    eventsFilterTable.Rows.Add("Информация в процессе", 1001);
                    eventsFilterTable.Rows.Add("Предупреждение", 1002);
                    eventsFilterTable.Rows.Add("Ошибка", 1003);
                    eventsFilterTable.Rows.Add("Критическая ошибка", 1004);
                    eventsFilterTable.Rows.Add("Начало операции установки иерархии", 1005);
                    eventsFilterTable.Rows.Add("Успешное окончание операции установки иерархии", 1006);
                    eventsFilterTable.Rows.Add("Окончание операции установки иерархии с ошибкой", 1007);
                    eventsFilterTable.Rows.Add("Начало операции очистки данных", 1008);
                    eventsFilterTable.Rows.Add("Начало операции импорта", 1009);
                    eventsFilterTable.Rows.Add("Начало операции Формирования сопоставимого классификатора", 1010);
                    eventsFilterTable.Rows.Add("Успешное окончание операции", 1011);
                    // варианты
                    eventsFilterTable.Rows.Add("Вариант скопирован", 100401);
                    eventsFilterTable.Rows.Add("Вариант открыт для изменений", 100403);
                    eventsFilterTable.Rows.Add("Вариант закрыт от изменений", 100402);
                    eventsFilterTable.Rows.Add("Вариант удален", 100404);

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
                    // варианты 
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
                    #region обновление схемы
                    eventsFilterTable.Rows.Add("Начало операции обновления схемы", 50001);
                    eventsFilterTable.Rows.Add("Информация в процессе", 50000);
                    eventsFilterTable.Rows.Add("Успешное окончание операции обновления схемы", 50002);
                    eventsFilterTable.Rows.Add("Окончание операции обновления схемы с ошибкой", 50003);

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
                    #region блокировка источников
                    eventsFilterTable.Rows.Add("Источник открыт для изменений", 1017);
                    eventsFilterTable.Rows.Add("Источник закрыт от изменений", 1016);
                    eventsFilterTable.Rows.Add("Источник удален", 1018);
                    pView.ucEvents.Rows[0].Cells[0].Appearance.Image = 18;
                    //pView.ucEvents.Rows[0].Cells[0].ToolTipText = pView.ucEvents.Rows[0].Cells[0].Text;
                    pView.ucEvents.Rows[1].Cells[0].Appearance.Image = 19;
                    //pView.ucEvents.Rows[1].Cells[0].ToolTipText = pView.ucEvents.Rows[1].Cells[0].Text;
                    pView.ucEvents.Rows[2].Cells[0].Appearance.Image = 20;
                    #endregion
                    break;
                case ModulesTypes.TransferDBToNewYearModule:
                    #region Функция перехода на новый год

                    eventsFilterTable.Rows.Add("Создание источника", 1101);
                    eventsFilterTable.Rows.Add("Экспорт классификатора", 1102);
                    eventsFilterTable.Rows.Add("Импорт классификатора", 1103);
                    eventsFilterTable.Rows.Add("Требование на расчет измерения", 1104);
                    eventsFilterTable.Rows.Add("Требование на расчет куба", 1105);
                    eventsFilterTable.Rows.Add("Предупреждение", 1106);
                    eventsFilterTable.Rows.Add("Ошибка", 1107);
                    eventsFilterTable.Rows.Add("Начало работы функции перевода базы на новый год", 1108);
                    eventsFilterTable.Rows.Add("Окончание работы функции перевода базы на новый год", 1109);

                    #endregion
                    break;
                case ModulesTypes.MessagesExchangeModule:
                    eventsFilterTable.Rows.Add("Создание нового сообщения от администратора", 1201);
                    eventsFilterTable.Rows.Add("Удаление сообщения", 1202);
                    eventsFilterTable.Rows.Add("Очистка неактуальных сообщений", 1203);
                    eventsFilterTable.Rows.Add("Создание сообщения от интерфейса расчета кубов", 1204);
                    eventsFilterTable.Rows.Add("Создание сообщения (прочее)", 1210);
                    eventsFilterTable.Rows.Add("Ошибка при отправке сообщения", 1211);
                    break;
            }
        }

        /// <summary>
        /// Заполняет список событий по информации из определения типа.
        /// </summary>
        /// <param name="type">Тип перечисления.</param>
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
        ///  Получает тип события, которое было выбрано в фильтре
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
        /// Обработчик перемещения по модулям
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
        /// Сохранение данных в XML
        /// </summary>
        private bool ugex1_OnSaveToXML(object sender)
        {
            //IXmlExportImporter exporter = this.Workplace.ActiveScheme.GetXMLExportImporter();
            try
            {
                // проверка необходимости обновления грида
                if (String.Compare(pView.ugex1.utmMain.Tools["Refresh"].SharedProps.AppearancesSmall.Appearance.BackColor.Name,"0") != 0)
                    ugex1_OnRefreshData(sender); 
              
                DataTable dt = new DataTable();
                
                if (InInplaceMode)
                    dt = AttachLogDataTable.Copy();
                else   
                    dt = LogDataTable.Copy();
                 
                // проверяем есть ли данные для сохранения
                if (dt.Rows.Count != 0)
                     {
                         // сохраняем данные в DataSet, который пишем в файл
                         DataSet ds = new DataSet(currentProtocol.ToString());
                         ds.Tables.Add(dt);
                         ExportImportHelper.SaveToXML(ds, pView.ugex1.SaveLoadFileName);
                     }
                else 
                    MessageBox.Show("Нет данных для записи.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            finally
            {
                //exporter.Dispose();
            }
            return true;
        }


        /// <summary>
        /// Изменение текущего фильтра по дате и событиям в зависимости от настроек фильтра
        /// </summary>
        private void ChangeDateTimeFilterCondition()
        {
            // если начальная дата больше конечной поменяем их местами
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
            // Если выбран только фильтр по дате
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
            // Если выбран только фильтр по событиям, формируем фильтр по коду этого события
            // если был выбран еще и фильтр по дате, то добавляем условие в общий фильтр
            if (pView.cbUseEventType.Checked)
            {
                KindsOfEvents = GetEventType();
                // Если какое то событие в списке фильтра выбрано
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
        /// Прорисовка иконок и хинтов в гриде
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
                // Расшифровка сообщений для протокола закачки данных
                case (int)DataPumpEventKind.dpeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; // Индкс картинки в листе
                    iniCell.Value = "Начало операции закачки";
                    iniCell.ToolTipText = "Начало операции закачки";
                    break;
                case (int)DataPumpEventKind.dpeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)DataPumpEventKind.dpeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)DataPumpEventKind.dpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции закачки";
                    iniCell.ToolTipText = "Успешное окончание операции закачки";
                    break;
                case (int)DataPumpEventKind.dpeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции закачки с ошибкой";
                    iniCell.ToolTipText = "Окончание операции закачки с ошибкой";
                    break;
                case (int)DataPumpEventKind.dpeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе закачки";
                    iniCell.ToolTipText = "Ошибка в процессе закачки";
                    break;
                case (int)DataPumpEventKind.dpeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе закачки";
                    iniCell.ToolTipText = "Критическая ошибка в процессе закачки";
                    break;
                case (int)DataPumpEventKind.dpeStartFilePumping:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало закачки файла";
                    iniCell.ToolTipText = "Начало закачки файла";
                    break;
                case (int)DataPumpEventKind.dpeSuccessfullFinishFilePump:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение закачки файла";
                    iniCell.ToolTipText = "Успешное завершение закачки файла";
                    break;
                case (int)DataPumpEventKind.dpeFinishFilePumpWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение закачки файла с ошибкой";
                    iniCell.ToolTipText = "Завершение закачки файла с ошибкой";
                    break;
                case (int)DataPumpEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)DataPumpEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)DataPumpEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region BridgeOperationsEventKind
                // Расшифровка сообщений для протокола сопоставления данных
                case (int)BridgeOperationsEventKind.boeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции сопоставления";
                    iniCell.ToolTipText = "Начало операции сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.boeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)BridgeOperationsEventKind.boeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)BridgeOperationsEventKind.boeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции сопоставления";
                    iniCell.ToolTipText = "Успешное окончание операции сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.boeFinishedWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции сопоставления с ошибкой";
                    iniCell.ToolTipText = "Окончание операции сопоставления с ошибкой";
                    break;
                case (int)BridgeOperationsEventKind.boeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе сопоставления";
                    iniCell.ToolTipText = "Ошибка в процессе сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.boeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе сопоставления";
                    iniCell.ToolTipText = "Критическая ошибка в процессе сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)BridgeOperationsEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки источника данных";
                    iniCell.ToolTipText = "Успешное завершение обработки источника данных";
                    break;
                case (int)BridgeOperationsEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region UsersOperationEventKind
                // Расшифровка сообщений для протокола действий пользователей
                case (int)UsersOperationEventKind.uoeUserConnectToScheme:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[6]; //6;
                    iniCell.Value = "Пользователь подключился к схеме";
                    iniCell.ToolTipText = "Пользователь подключился к схеме";
                    break;
                case (int)UsersOperationEventKind.uoeUserDisconnectFromScheme:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[7]; //7;
                    iniCell.Value = "Пользователь отключился от схемы";
                    iniCell.ToolTipText = "Пользователь отключился от схемы";
                    break;
                case (int)UsersOperationEventKind.uoeChangeUsersTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[12]; //7;
                    iniCell.Value = "Изменение таблицы пользователей";
                    iniCell.ToolTipText = "Изменение таблицы пользователей";
                    break;
                case (int)UsersOperationEventKind.uoeChangeGroupsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[13]; //7;
                    iniCell.Value = "Изменение таблицы групп пользователей";
                    iniCell.ToolTipText = "Изменение таблицы групп пользователей";
                    break;
                case (int)UsersOperationEventKind.uoeChangeDepartmentsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[15]; //7;
                    iniCell.Value = "Изменение таблицы отделов";
                    iniCell.ToolTipText = "Изменение таблицы отделов";
                    break;
                case (int)UsersOperationEventKind.uoeChangeOrganizationsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[15]; //7;
                    iniCell.Value = "Изменение таблицы организаций";
                    iniCell.ToolTipText = "Изменение таблицы организаций";
                    break;
                case (int)UsersOperationEventKind.uoeChangeTasksTypes:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[15]; //7;
                    iniCell.Value = "Изменение таблицы типов задач";
                    iniCell.ToolTipText = "Изменение таблицы типов задач";
                    break;
                case (int)UsersOperationEventKind.uoeChangeMembershipsTable:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[16]; //7;
                    iniCell.Value = "Изменение вхождения пользователя в группу";
                    iniCell.ToolTipText = "Изменение вхождения пользователя в группу";
                    break;
                case (int)UsersOperationEventKind.uoeUntilledExceptionsEvent:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //7;
                    iniCell.Value = "Необработанная ошибка";
                    iniCell.ToolTipText = "Необработанная ошибка";
                    break;
                case (int)UsersOperationEventKind.uoeSchemeUpdate:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[21]; //7;
                    iniCell.Value = "Обновление схемы";
                    iniCell.ToolTipText = "Обновление схемы";
                    break;
                case (int)UsersOperationEventKind.uoeProtocolsToArchive:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //7;
                    iniCell.Value = "Протоколы заархивированы";
                    iniCell.ToolTipText = "Протоколы заархивированы";
                    break;
                #endregion

                #region SystemEventKind
                case (int)SystemEventKind.seInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация";
                    iniCell.ToolTipText = "Информация";
                    break;
                case (int)SystemEventKind.seError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Ошибка";
                    iniCell.ToolTipText = "Ошибка";
                    break;
                case (int)SystemEventKind.seCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка";
                    iniCell.ToolTipText = "Критическая ошибка";
                    break;
                case (int)SystemEventKind.seWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                #endregion

                #region ProcessDataEventKind
                case (int)ProcessDataEventKind.pdeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции обработки данных";
                    iniCell.ToolTipText = "Начало операции обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции обработки данных";
                    iniCell.ToolTipText = "Успешное окончание операции обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции обработки данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции обработки данных с ошибкой";
                    break;
                case (int)ProcessDataEventKind.pdeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе обработки данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе обработки данных";
                    iniCell.ToolTipText = "Ошибка в процессе обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе обработки данных";
                    iniCell.ToolTipText = "Информация в процессе обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)ProcessDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)ProcessDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)ProcessDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region DeleteDataEventKind
                case (int)DeleteDataEventKind.ddeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции удаления данных";
                    iniCell.ToolTipText = "Начало операции удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции удаления данных";
                    iniCell.ToolTipText = "Успешное окончание операции удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции удаления данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции удаления данных с ошибкой";
                    break;
                case (int)DeleteDataEventKind.ddeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе удаления данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе удаления данных";
                    iniCell.ToolTipText = "Ошибка в процессе удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе удаления данных";
                    iniCell.ToolTipText = "Информация в процессе удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)DeleteDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)DeleteDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)DeleteDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region MDProcessingEventKind
                case (int)MDProcessingEventKind.mdpeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Начало операции обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Успешное окончание операции обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeFinishedWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции обработки многомерных хранилищ с ошибкой";
                    iniCell.ToolTipText = "Окончание операции обработки многомерных хранилищ с ошибкой";
                    break;
                case (int)MDProcessingEventKind.mdpeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Критическая ошибка в процессе обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Ошибка в процессе обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Информация в процессе обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)MDProcessingEventKind.InvalidateObject:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //8;
                    iniCell.Value = "Требование на расчет";
                    iniCell.ToolTipText = "Требование на расчет";
                    break;
                /*case (int)MDProcessingEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)MDProcessingEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;*/
                #endregion

                #region ReviseDataEventKind
                case (int)ReviseDataEventKind.pdeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции проверки данных";
                    iniCell.ToolTipText = "Начало операции проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции проверки данных";
                    iniCell.ToolTipText = "Успешное окончание операции проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции проверки данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции проверки данных с ошибкой";
                    break;
                case (int)ReviseDataEventKind.pdeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе проверки данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе проверки данныхщ";
                    iniCell.ToolTipText = "Ошибка в процессе проверки данныхщ";
                    break;
                case (int)ReviseDataEventKind.pdeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе проверки данных";
                    iniCell.ToolTipText = "Информация в процессе проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)ReviseDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)ReviseDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)ReviseDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;

                #endregion

                #region предпросмотр данных
                case (int)PreviewDataEventKind.dpeStart:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; // Индкс картинки в листе
                    iniCell.Value = "Начало операции предпросмотра данных";
                    iniCell.ToolTipText = "Начало операции предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)PreviewDataEventKind.dpeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)PreviewDataEventKind.dpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции предпросмотра данных";
                    iniCell.ToolTipText = "Успешное окончание операции предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции предпросмотра данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции предпросмотра данных с ошибкой";
                    break;
                case (int)PreviewDataEventKind.dpeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе предпросмотра данных";
                    iniCell.ToolTipText = "Ошибка в процессе предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе предпросмотра данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeStartFilePumping:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки файла";
                    iniCell.ToolTipText = "Начало обработки файла";
                    break;
                case (int)PreviewDataEventKind.dpeSuccessfullFinishFilePump:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)PreviewDataEventKind.dpeFinishFilePumpWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки файла с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки файла с ошибкой";
                    break;
                case (int)PreviewDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)PreviewDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)PreviewDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region BridgeOperationsEventKind
                // Расшифровка сообщений для протокола сопоставления данных
                case (int)ClassifiersEventKind.ceInformation:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)ClassifiersEventKind.ceWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)ClassifiersEventKind.ceSuccefullFinished:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции";
                    iniCell.ToolTipText = "Успешное окончание операции";
                    break;
                case (int)ClassifiersEventKind.ceStartHierarchySet:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //10;
                    iniCell.Value = "Начало операции установки иерархии";
                    iniCell.ToolTipText = "Начало операции установки иерархии";
                    break;
                case (int)ClassifiersEventKind.ceSuccessfullFinishHierarchySet:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //4;
                    iniCell.Value = "Успешное окончание операции установки иерархии";
                    iniCell.ToolTipText = "Успешное окончание операции установки иерархии";
                    break;
                case (int)ClassifiersEventKind.ceFinishHierarchySetWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции установки иерархии с ошибкой";
                    iniCell.ToolTipText = "Окончание операции установки иерархии с ошибкой";
                    break;
                case (int)ClassifiersEventKind.ceError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка";
                    iniCell.ToolTipText = "Ошибка";
                    break;
                case (int)ClassifiersEventKind.ceCriticalError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка";
                    iniCell.ToolTipText = "Критическая ошибка";
                    break;
                case (int)ClassifiersEventKind.ceClearClassifierData:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //8;
                    iniCell.Value = "Начало операции очистки данных";
                    iniCell.ToolTipText = "Начало операции очистки данных";
                    break;
                case (int)ClassifiersEventKind.ceImportClassifierData:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //9;
                    iniCell.Value = "Начало операции импорта";
                    iniCell.ToolTipText = "Начало операции импорта";
                    break;
                case (int)ClassifiersEventKind.ceCreateBridge:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции формирования сопоставимого классификатора";
                    iniCell.ToolTipText = "Начало операции формирования сопоставимого классификатора";
                    break;
                case (int)ClassifiersEventKind.ceVariantCopy:
                    message = "Вариант скопирован";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[17];
                    break;
                case (int)ClassifiersEventKind.ceVariantDelete:
                    message = "Вариант удален";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[20];
                    break;
                case (int)ClassifiersEventKind.ceVariantLock:
                    message = "Вариант закрыт от изменений";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[19];
                    break;
                case (int)ClassifiersEventKind.ceVariantUnlok:
                    message = "Вариант открыт для изменений";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[18];
                    break;
                
                #endregion

                #region обновления схемы
                case (int)UpdateSchemeEventKind.BeginUpdate:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0]; //10;
                    iniCell.Value = "Начало операции обновления схемы";
                    iniCell.ToolTipText = "Начало операции обновления схемы";
                    break;
                case (int)UpdateSchemeEventKind.Information:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)UpdateSchemeEventKind.EndUpdateSuccess:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции обновления схемы";
                    iniCell.ToolTipText = "Успешное окончание операции обновления схемы";
                    break;
                case (int)UpdateSchemeEventKind.EndUpdateWithError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции обновления схемы с ошибкой";
                    iniCell.ToolTipText = "Окончание операции обновления схемы с ошибкой";
                    break;
                #endregion

                #region Блокировка источников.
                case (int)DataSourceEventKind.ceSourceLock:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[19];
                    iniCell.Value = "Источник закрыт от изменений";
                    iniCell.ToolTipText = "Источник закрыт от изменений";
                    break;
                case (int)DataSourceEventKind.ceSourceUnlock:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[18];
                    iniCell.Value = "Источник открыт для изменений";
                    iniCell.ToolTipText = "Источник открыт для изменений";
                    break;
                case (int)DataSourceEventKind.ceSourceDelete:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[20];
                    iniCell.Value = "Источник удален";
                    iniCell.ToolTipText = "Источник удален";
                    break;

                #endregion

                #region Перевод базы на новый год
                case (int)TransferDBToNewYearEventKind.tnyeError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка";
                    iniCell.ToolTipText = "Ошибка";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeWarning:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[2];
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeBegin:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[0];
                    iniCell.Value = "Начало работы функции перевода базы на новый год";
                    iniCell.ToolTipText = "Начало работы функции перевода базы на новый год";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeEnd:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[3];
                    iniCell.Value = "Окончание работы функции перевода базы на новый год";
                    iniCell.ToolTipText = "Окончание работы функции перевода базы на новый год";
                    break;
                case (int)TransferDBToNewYearEventKind.tnyeCreateSource:
                case (int)TransferDBToNewYearEventKind.tnyeExportClassifierData:
                case (int)TransferDBToNewYearEventKind.tnyeImportClassifierData:
                case (int)TransferDBToNewYearEventKind.tnyeInvalidateCube:
                case (int)TransferDBToNewYearEventKind.tnyeInvalidateDimension:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[1];
                    iniCell.Value = "Информационное сообщение";
                    iniCell.ToolTipText = "Информационное сообщение";
                    break;
                #endregion

                #region Подсистема обмена сообщениями

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
                    iniCell.Value = "Информационное сообщение"; 
                    iniCell.ToolTipText = "Информационное сообщение";
                    break;
                case (int)MessagesEventKind.mekSendError:
                    iniCell.Appearance.ImageBackground = pView.ilColumns.Images[4];
                    iniCell.Value = "Ошибка при отправке сообщения";
                    iniCell.ToolTipText = "Ошибка при отправке сообщения";
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
                        cell.ToolTipText = "Создание нового объекта";
                        break;
                    case 1:
                        cell.Appearance.Image = this.pView.ugeAudit.il.Images[0];
                        cell.ToolTipText = "Изменение структуры";
                        break;
                    case 2:
                        cell.Appearance.Image = this.pView.ugeAudit.il.Images[1];
                        cell.ToolTipText = "Удаление существующего объекта";
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
                        cell.ToolTipText = "Тип объекта: 'Сопоставимый классификатор'";
                        break;
                    case 1:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[1];
                        cell.ToolTipText = "Тип объекта: 'Классификатор данных'";
                        break;
                    case 2:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[2];
                        cell.ToolTipText = "Тип объекта: 'Фиксированный классификатор'";
                        break;
                    case 3:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[3];
                        cell.ToolTipText = "Тип объекта: 'Таблица фактов'";
                        break;
                    case 4:
                        cell.Appearance.Image = this.pView.ilObjectType.Images[4];
                        cell.ToolTipText = "Тип объекта: 'Системная таблица'";
                        break;
                }
            }
        }

    }
}
