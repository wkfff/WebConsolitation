using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;
using Krista.FM.Client.Common.Configuration;


namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    //public enum NavigationNodeKind { ndUnknown, ndAllUsers, ndAllGroups, ndAllObjects, ndAllDirectoryes, ndOrganizations, ndDivisions, ndTasksTypes, ndSessions };

    public partial class AdministrationUI : BaseViewObj, IInplaceTasksPermissionsView
    {
        #region Константы и переменные
        private AdministrationView vo;

        const string AdminGroupCaption = "Администраторы";
        const string WebAdminGroupCaption = "Web-администраторы";
        
        public ArrayList CreatableTaskTypesIds = null;

        private DataSet dsAllList;

        private string adminCaption;

        private TreeNode lastSelectedNode = null;

        private IUsersModal userModal = null;

        private NavigationNodeKind _currentNavigationNodeKind;

        internal NavigationNodeKind CurrentNavigationNode
        {
            get { return _currentNavigationNodeKind; }
            set { _currentNavigationNodeKind = value;}
        }

        public void SetNavigationNodeKind(NavigationNodeKind navigationNodeKind )
        {
            CurrentNavigationNode = navigationNodeKind;
        }

        #endregion

        #region Создание и инициализация объекта
        public AdministrationUI(string key)
            : base(key)
        {
            CurrentNavigationNode = GetNavigationNodeKindFromName(key);
            Caption = "Администрирование";
            dsAllList = new DataSet();
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new AdministrationView();
            vo = (AdministrationView)fViewCtrl;

            ViewSettings settings = new ViewSettings(fViewCtrl, String.Format("{0}.{1}", this.GetType().FullName, Key));
            settings.Settings.Add(new UltraGridExSettingsPartial(String.Format("{0}.{1}", this.GetType().FullName, Key), ((AdministrationView)fViewCtrl).ugeGroupsPermissions.ugData));
        }

        public override void Initialize()
        {
            base.Initialize();

            vo.ugeAllList.StateRowEnable = true;
            vo.ugeAllList.OnBeforeCellDeactivate += new Krista.FM.Client.Components.BeforeCellDeactivate(ugeAllList_OnBeforeCellDeactivate);
            vo.ugeAllList.OnAfterRowActivate += new Krista.FM.Client.Components.AfterRowActivate(ugeAllList_OnAfterRowActivate);
            vo.ugeAllList.OnBeforeRowDeactivate += new Krista.FM.Client.Components.BeforeRowDeactivate(ugeAllList_OnBeforeRowDeactivate);
            vo.ugeAllList.OnAfterRowInsert += new Krista.FM.Client.Components.AfterRowInsert(ugeAllList_OnAfterRowInsert);
            vo.ugeAllList.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeAllList_OnGetGridColumnsState);
            vo.ugeAllList.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeAllList_OnCancelChanges);
            vo.ugeAllList.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeAllList_OnSaveChanges);
            vo.ugeAllList.OnClickCellButton += new Krista.FM.Client.Components.ClickCellButton(ugeAllList_OnClickCellButton);
            vo.ugeAllList.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugeAllList_OnGridInitializeLayout);
            vo.ugeAllList.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ugeAllList_OnInitializeRow);
            vo.ugeAllList.OnBeforeRowsDelete += new Krista.FM.Client.Components.BeforeRowsDelete(this.ugeAllList_OnBeforeRowsDelete);
            vo.ugeAllList.OnClearCurrentTable += new Krista.FM.Client.Components.DataWorking(ugeAllList_OnClearCurrentTable);
            vo.ugeAllList.ToolClick += new Krista.FM.Client.Components.ToolBarToolsClick(ugeAllList_ToolClick);
            vo.ugeAllList.ugData.AfterCellUpdate += new CellEventHandler(ugData_AfterCellUpdate);
            vo.ugeAllList.OnRefreshData += new Krista.FM.Client.Components.RefreshData(ugeAllList_OnRefreshData);
            vo.ugeAllList.ugData.CellChange += new CellEventHandler(ugData_CellChange);
            vo.ugeAllList.ugData.BeforeEnterEditMode += new CancelEventHandler(ugData_BeforeEnterEditMode);
            vo.ugeAllList.OnMouseEnterGridElement += ugeAllList_OnMouseEnterGridElement;
            vo.ugeAllList.OnMouseLeaveGridElement += ugeAllList_OnMouseLeaveGridElement;
            vo.ugeAllList.OnGetLookupValue += new GetLookupValueDelegate(newGrid_OnGetLookupValue);
            vo.ugeAllList.OnCheckLookupValue += new CheckLookupValueDelegate(newGrid_OnCheckLookupValue);

            vo.ugeAssignedPermissions.IsReadOnly = true;
            vo.ugeAssignedPermissions.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugeAssignedPermissions_OnGridInitializeLayout);
            vo.ugeAssignedPermissions.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeAssignedPermissions_OnGetGridColumnsState);
            vo.ugeAssignedPermissions.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeAssignedPermissions_OnSaveChanges);
            vo.ugeAssignedPermissions.OnRefreshData += new RefreshData(ugeAssignedPermissions_OnRefreshData);

            vo.ugeGroupsPermissions.StateRowEnable = true;
            vo.ugeGroupsPermissions.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugeUsersGroupsPermissions_OnGridInitializeLayout);
            vo.ugeGroupsPermissions.AllowAddNewRecords = false;
            vo.ugeGroupsPermissions.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeGroupsPermissions_OnGetGridColumnsState);
            vo.ugeGroupsPermissions.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeGroupsPermissions_OnCancelChanges);
            vo.ugeGroupsPermissions.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeGroupsPermissions_OnSaveChanges);
            vo.ugeGroupsPermissions.OnRefreshData += new RefreshData(ugeGroupsPermissions_OnRefreshData);
            vo.ugeGroupsPermissions.ugData.DisplayLayout.GroupByBox.Hidden = true;
            vo.ugeGroupsPermissions.utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;

            vo.ugeMembership.StateRowEnable = true;
            vo.ugeMembership.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugeMembership_OnGridInitializeLayout);
            vo.ugeMembership.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ugeMembership_OnInitializeRow);
            vo.ugeMembership.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeMembership_OnGetGridColumnsState);
            vo.ugeMembership.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeMembership_OnCancelChanges);
            vo.ugeMembership.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeMembership_OnSaveChanges);
            vo.ugeMembership.OnRefreshData += new RefreshData(ugeMembership_OnRefreshData);

            vo.ugeUsersPermissions.StateRowEnable = true;
            vo.ugeUsersPermissions.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ugeUsersGroupsPermissions_OnGridInitializeLayout);
            vo.ugeUsersPermissions.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeUsersPermissions_OnGetGridColumnsState);
            vo.ugeUsersPermissions.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeUsersPermissions_OnCancelChanges);
            vo.ugeUsersPermissions.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeUsersPermissions_OnSaveChanges);
            vo.ugeUsersPermissions.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ugeUsersPermissions_OnInitializeRow);
            vo.ugeUsersPermissions.OnRefreshData += new RefreshData(ugeUsersPermissions_OnRefreshData);
            vo.ugeUsersPermissions.ugData.DisplayLayout.GroupByBox.Hidden = true;
            vo.ugeUsersPermissions.utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;

            vo.ugeAssignedPermissions.ugData.InitializeGroupByRow += new InitializeGroupByRowEventHandler(ugData_InitializeGroupByRow);

            // старые
            vo.utcPages.SelectedTabChanging += new Infragistics.Win.UltraWinTabControl.SelectedTabChangingEventHandler(this.utcPages_SelectedTabChanging);

        }

        bool ugeUsersPermissions_OnRefreshData(object sender)
        {
            utcPages_SelectedTabChanging(null, new SelectedTabChangingEventArgs(vo.utcPages.ActiveTab));
            return true;
        }

        bool ugeMembership_OnRefreshData(object sender)
        {
            utcPages_SelectedTabChanging(null, new SelectedTabChangingEventArgs(vo.utcPages.ActiveTab));
            return true;
        }

        bool ugeGroupsPermissions_OnRefreshData(object sender)
        {
            utcPages_SelectedTabChanging(null, new SelectedTabChangingEventArgs(vo.utcPages.ActiveTab));
            return true;
        }

        bool ugeAssignedPermissions_OnRefreshData(object sender)
        {
            utcPages_SelectedTabChanging(null, new SelectedTabChangingEventArgs(vo.utcPages.ActiveTab));
            return true;
        }
        
        void ugData_BeforeEnterEditMode(object sender, CancelEventArgs e)
        {
            UltraGridCell cell = vo.ugeAllList.ugData.ActiveCell;
            string str = "AllowDomainAuth";
            if (cell.Column.Key.ToUpper() == str.ToUpper())
            { 
                if (cell.Row.Cells["DNSNAME"].Value.ToString() == String.Empty)
                    e.Cancel = true;
            }
        }

        void ugData_CellChange(object sender, CellEventArgs e)
        {
            
        }

        bool ugeAllList_OnRefreshData(object sender)
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            if (nk == NavigationNodeKind.ndSessions)
            {
                dsAllList.Tables.Clear();
                DataTable dt = new DataTable(); 
                GetSessionsFromScheme(ref dt);
                dsAllList.Tables.Add(dt);
                vo.ugeAllList.DataSource = dsAllList;

                ColumnFilter filter = vo.ugeAllList.ugData.DisplayLayout.Bands[0].ColumnFilters["SessionType"];
                filter.FilterConditions.Add(FilterComparisionOperator.NotEquals, SessionClientType.Server);
                filter.FilterConditions.Add(FilterComparisionOperator.NotEquals, SessionClientType.DataPump);
                return true;
            }
            else
            {
                if (dsAllList.GetChanges() != null)
                {
                    if (MessageBox.Show(Workplace.WindowHandle, "Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SaveChanges();
                    }
                }
                LoadData();
                return true;
            }
        }

        public override Icon Icon
        {
            get
            {
                return Icon.FromHandle(Properties.Resources.Admin_16.GetHicon());
            }
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.Admin_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.Admin_32; }
        }

        public override void InternalFinalize()
        {
            SaveChangesWhenExit();
            base.InternalFinalize();
        }

        #endregion

        #region Добавление пользователей из домена

        void AddUsersFromDomain()
        {
            DomainItemProperties[] addUsers;
            if (AddNewUser.GetUsersFromDomain(this.Workplace, out addUsers))
            {
                IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB;
                try
                {
                    vo.ugeAllList.ugData.BeginUpdate();
                    // нужно получить выбранного пользователя или группу пользователей и добавить в интерфей                        
                    vo.ugeAllList.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                    bool idAddUsers = false;
                    foreach (DomainItemProperties user in addUsers)
                    {
                        string userLogin = user.Login;
                        if (CheckDomainUser(userLogin))
                        {
                            DataRow dRow = dsAllList.Tables[0].Rows.Add(null, null);
                            dRow["FIRSTNAME"] = user.firstName;
                            dRow["LASTNAME"] = user.surname;
                            dRow["PATRONYMIC"] = user.patronymicName;
                            dRow["NAME"] = userLogin;
                            dRow["Description"] = user.description;
                            dRow["DNSNAME"] = userLogin;
                            dRow["AllowDomainAuth"] = false;
                            dRow["AllowPwdAuth"] = false;
                            // добавляем ID и другие необходимые параметры для добавленного пользователя
                            SetAditionalParams(dRow, db);
                            vo.ugeAllList.SetRowToStateByID(Convert.ToInt32(dRow["ID"]), vo.ugeAllList.ugData.Rows, Krista.FM.Client.Components.UltraGridEx.LocalRowState.Added);
                            idAddUsers = true;
                        }
                    }
                    if (idAddUsers)
                    {
                        vo.ugeAllList.BurnChangesDataButtons(true);
                    }
                }
                finally
                {
                    db.Dispose();
                    vo.ugeAllList.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    vo.ugeAllList.ugData.EndUpdate();
                }
            }
        }

        #endregion

        #region Обработки (проверки) значений в ячейках, а так же события, связанные с этими обработками

        /// <summary>
        /// обработчик нажатия кнопок на тулбаре (добавление пользователей из домена)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeAllList_ToolClick(object sender, ToolClickEventArgs e)
        {
            AddUsersFromDomain();
        }

        /// <summary>
        /// при добавлении записей записываем некоторые необходимые данные
        /// </summary>
        /// <param name="dRow"></param>
        /// <param name="db"></param>
        private void SetAditionalParams(DataRow dRow, IDatabase db)
        {
            string generatorName = "g_Users";
            int newID = db.GetGenerator(generatorName);
            dRow["ID"] = newID;
            dRow["BLOCKED"] = 0;
            dRow["USERTYPE"] = 0;
        }

        /// <summary>
        /// проверка пользователей при добавлении через грид
        /// </summary>
        /// <param name="clmnName"></param>
        /// <param name="clmnValue"></param>
        /// <param name="currentRowID"></param>
        /// <returns></returns>
        private bool CheckCurrentUser(string clmnName, string clmnValue, int currentRowID)
        {
            // проверим всех пользователей, если находим такого, сообщаем
            bool returnValue = true;
            if (clmnValue == string.Empty)
                return returnValue;
            foreach (DataRow row in dsAllList.Tables[0].Rows)
            {
                if (Convert.ToInt32(row["ID"]) != currentRowID)
                {
                    // сравниваем по верхнему регистру, имена с разным регистром считаются одинаковыми
                    if (row[clmnName].ToString().ToUpper() == clmnValue.ToUpper())
                    {
                        returnValue = false;
                        break;
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// проверка пользователей при добавлении через домен
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        private bool CheckDomainUser(string userLogin)
        {
            string query = string.Format("NAME = '{0}' AND DNSNAME = '{0}'", userLogin);
            if (dsAllList.Tables[0].Select(query).Length > 0)
                return false;
            return true;
        }

        private bool CheckAllUsers(ref StringBuilder errors)
        {
            foreach (DataRow userRow in dsAllList.Tables[0].Rows)
            {
                // проверяем только нормальных добавляемых пользователей
                if ((userRow.RowState == DataRowState.Added) || (userRow.RowState == DataRowState.Modified))
                {
                    int userID = Convert.ToInt32(userRow["ID"]);
                    if (userID >= 100)
                    {
                        foreach (DataRow row in dsAllList.Tables[0].Rows)
                        {
                            int rowID = Convert.ToInt32(row["ID"]);
                            if (rowID >= 100 && rowID != userID)
                            {
                                if (userRow["NAME"].ToString().ToUpper() == row["NAME"].ToString().ToUpper() && userRow["NAME"].ToString() != string.Empty)
                                {
                                    errors.AppendLine(String.Format("Пользователь c именем '{0}' уже добавлен. Измените имя пользователя", userRow["NAME"].ToString()));
                                }
                                if (userRow["DNSNAME"].ToString().ToUpper() == row["DNSNAME"].ToString().ToUpper() && userRow["DNSNAME"].ToString() != string.Empty)
                                {
                                    errors.AppendLine(String.Format("Пользователь c именем DNS '{0}' уже добавлен. Измените имя DNS пользователя", userRow["DNSNAME"].ToString()));
                                }
                            }
                        }
                    }
                }
            }
            if (errors.Length > 0)
                return false;
            return true;
        }

        /// <summary>
        /// действия, выполняемые перед сменой активной ячейки в гриде
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeAllList_OnBeforeCellDeactivate(object sender, CancelEventArgs e)
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            UltraGridCell cell = vo.ugeAllList.ugData.ActiveCell;
            switch (nk)
            {
                case NavigationNodeKind.ndAllGroups:
                    if (cell.Column.Key == "NAME" && vo.ugeAllList.ugData.ActiveRow.Index > 0)
                    {
                        if (cell.Value.ToString() == AdminGroupCaption || cell.Value.ToString() == WebAdminGroupCaption)
                        {
                            MessageBox.Show(
                                string.Format("Группа '{0}' уже существует. Выберите для группы другое наименование.",
                                              cell.Value), "Ввод данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = true;
                            cell.Value = string.Empty;
                        }
                    }
                    break;
                case NavigationNodeKind.ndAllUsers:
                    if (cell.Column.Key == "NAME" && vo.ugeAllList.ugData.ActiveRow.Index > 0)
                    {
                        if (!CheckCurrentUser("NAME", cell.Value.ToString(), Convert.ToInt32(cell.Row.Cells["ID"].Value)))
                        {
                            string message = string.Format("Пользователь c именем '{0}' уже добавлен. Измените имя пользователя", cell.Value);
                            MessageBox.Show(message, "Добавление пользователей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = false;
                        }
                    }
                    if (cell.Column.Key == "DNSNAME" && vo.ugeAllList.ugData.ActiveRow.Index > 0)
                    {
                        if (!CheckCurrentUser("DNSNAME", cell.Value.ToString(), Convert.ToInt32(cell.Row.Cells["ID"].Value)))
                        {
                            string message = string.Format("Пользователь c именем DNS '{0}' уже добавлен. Измените имя DNS пользователя", cell.Value);
                            MessageBox.Show(message, "Добавление пользователей", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = false;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// действия, выполняемые перед удалением записей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ugeAllList_OnBeforeRowsDelete(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            bool finded = false;
            switch (CurrentNavigationNode)
            {
                case NavigationNodeKind.ndAllGroups:
                    string name = String.Empty;
                    foreach (UltraGridRow row in e.Rows)
                    {
                        name = Convert.ToString(row.Cells["NAME"].Value);
                        finded = (name == AdminGroupCaption || name == WebAdminGroupCaption);
                        if (finded)
                            break;
                    }
                    if (finded)
                    {
                        e.Cancel = true;
                        e.DisplayPromptMsg = false;
                        MessageBox.Show(string.Format("Нельзя удалить группу '{0}'",  name), "Невозможно удалить запись",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
                case NavigationNodeKind.ndAllUsers:
                    foreach (UltraGridRow row in e.Rows)
                    {
                        int id = CC.UltraGridHelper.GetRowID(row);
                        finded = FixedUsers.UserIsFixed(id);
                        if (finded)
                            break;
                    }
                    if (finded)
                    {
                        e.Cancel = true;
                        e.DisplayPromptMsg = false;
                        MessageBox.Show("Нельзя удалить группу фиксированного пользователя", "Невозможно удалить запись",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
            }
        }

        private const string d_Regions_Analysis = "383f887a-3ebb-4dba-8abb-560b5777436f";

        private int GetAnalysisDataSource()
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object queryResult = db.ExecQuery("select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    new System.Data.OleDb.OleDbParameter("SupplierCode", "ФО"),
                    new System.Data.OleDb.OleDbParameter("DataCode", 6),
                    new System.Data.OleDb.OleDbParameter("Year", DateTime.Now.Year));
                if (!(queryResult is DBNull))
                    return Convert.ToInt32(queryResult);
                return -1;
            }
        }

        /// <summary>
        /// обработчик нажатия кнопок в ячейках грида
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeAllList_OnClickCellButton(object sender, CellEventArgs e)
        {
            string columnName = e.Cell.Column.Key;
            UltraGridCell visibleCell = e.Cell;
            bool isLookup = columnName.Contains("Lookup");
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(vo.ugeAllList.ugData);

            // выбор значения по лукапу
            if (isLookup)
            {
                UsersModalForm modal = new UsersModalForm(Workplace);
                int ID = 0;
                string Name = string.Empty;
                bool success = false;
                UltraGridCell valueCell = null;
                switch (columnName)
                {
                    case "LookupDep":
                        success = modal.ShowModal(NavigationNodeKind.ndDivisions, ref ID, ref Name);
                        valueCell = visibleCell.Row.Cells["REFDEPARTMENTS"];
                        break;
                    case "LookupOrg":
                        success = modal.ShowModal(NavigationNodeKind.ndOrganizations, ref ID, ref Name);
                        valueCell = visibleCell.Row.Cells["REFORGANIZATIONS"];
                        break;
                    case "REFREGION_Lookup":
                        object refRegion = -1;
                        if (Workplace.ClsManager.ShowClsModal(d_Regions_Analysis, -1, GetAnalysisDataSource(), DateTime.Today.Year, ref refRegion))
                            activeRow.Cells["RefRegion"].Value = refRegion;
                        break;
                }
                if (success)
                {
                    valueCell.Value = ID;
                    ((UltraGrid)sender).EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                    visibleCell.Value = Name;
                    ((UltraGrid)sender).EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    visibleCell.Row.Update();
                    ((UltraGrid)sender).PerformAction(UltraGridAction.ExitEditMode);
                }
            }
            else
            {
                if (columnName == "ChangePassword")
                {
                    // проверить существует ли пользователь в базе данных
                    // ....

                    //для добавленных и удаленных записей вызывать форму смены пароля не надо
                    int rowID = Convert.ToInt32(e.Cell.Row.Cells["ID"].Value);
                    // выбираем запись в таблице, на которой стоим
                    DataRow[] rows = dsAllList.Tables[0].Select(string.Format("ID = {0}", rowID));
                    // если такая запись есть
                    if (rows.Length > 0)
                        if (rows[0].RowState == DataRowState.Added)
                            MessageBox.Show("Для смены пароля необходимо сохранить учетную запись пользователя", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            Workplace.ChangePasswordAdm(CC.UltraGridHelper.GetRowID(e.Cell.Row));
                    //если нет, то значит запись помечена на удаление
                    else
                        MessageBox.Show("Учетная запись пользователя помечена как удаленная, для изменения пароля отмените удаление.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (columnName == "dropSession")
                {
                    string sessionID = e.Cell.Row.Cells["ID"].Value.ToString();
                    ISession session = null;
                    try
                    {
                        session = this.Workplace.ActiveScheme.SessionManager.Sessions[sessionID];
                    }
                    catch
                    {
                        MessageBox.Show("Данная сессия отсутствует в подключениях", "Сессии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ugeAllList_OnRefreshData(vo.ugeAllList);
                        return;
                    }
                    if (!session.IsBlocked)
                    {
                        if (MessageBox.Show("Заблокировать сессию", "Сессиия пользователя", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            session.IsBlocked = true;
                            e.Cell.Row.Appearance.BackColor = Color.DarkOrange;
                            e.Cell.ToolTipText = "Разблокировать сессию";
                            e.Cell.ButtonAppearance.Image = vo.il.Images[3];
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Разблокировать сессию", "Сессиия пользователя", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            session.IsBlocked = false;
                            e.Cell.Row.Appearance.ResetBackColor();
                            e.Cell.ToolTipText = "Заблокировать сессию";
                            e.Cell.ButtonAppearance.Image = vo.il.Images[4];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// устанавливает значения для лукапных колонок
        /// </summary>
        /// <param name="um"></param>
        /// <param name="cell"></param>
        /// <param name="visibleColumnName"></param>
        /// <param name="valueColumnName"></param>
        private void SetLookupParams(IUsersManager um, UltraGridCell cell, string visibleColumnName, string valueColumnName)
        {
            if (cell.Value == null || cell.Value.ToString() == "\r\n")
            {
                cell.Row.Cells[valueColumnName].Value = DBNull.Value;
                cell.Row.Update();
            }
            else
            {
                DataTable dt = um.GetOrganizations();
                DataRow[] rows = dt.Select(string.Format("NAME = '{0}'", cell.Value));
                if (rows.Length != 0)
                {
                    cell.Row.Cells[valueColumnName].Value = rows[0]["ID"];
                    cell.Row.Update();
                }
                else
                {
                    cell.Row.Cells[valueColumnName].Value = DBNull.Value;
                    cell.Value = string.Empty;
                }
                dt.Clear();
            }
        }

        /// <summary>
        /// обработчик события при котором данные ячейки записываются в компонент хранения данных (DataSet, DataTable и т.п.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            UltraGridRow row = e.Cell.Row;
            UltraGridCell cell = e.Cell;
            if (cell.Column.Key != "LookupOrg" && cell.Column.Key != "LookupDep") return;
            
            if (row.Cells.Exists("REFORGANIZATIONS") && row.Cells.Exists("REFDEPARTMENTS"))
            {
                ((UltraGrid)sender).EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                IUsersManager um = Workplace.ActiveScheme.UsersManager;
                if (cell.Column.Key == "LookupOrg")
                    SetLookupParams(um, cell, "LookupOrg", "REFORGANIZATIONS");


                if (cell.Column.Key == "LookupDep")
                    SetLookupParams(um, cell, "LookupDep", "REFDEPARTMENTS");
                ((UltraGrid)sender).EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        /// <summary>
        /// инициализация строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeAllList_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            if (row.Cells["ID"].Value == DBNull.Value)
                return;
            object curID = row.Cells["ID"].Value;
            switch (CurrentNavigationNode)
            {
                #region Группы
                case NavigationNodeKind.ndAllGroups:
                    // группу администроторов помечаем отдельным цветом
                    string groupName = Convert.ToString(row.Cells["NAME"].Value);
                    if (groupName == AdminGroupCaption || groupName == WebAdminGroupCaption)
                    {
                        row.Activation = Activation.NoEdit;
                        row.Appearance.ForeColor = System.Drawing.Color.Blue;
                    }
                    break;
                #endregion

                #region Пользователи
                case NavigationNodeKind.ndAllUsers:
                    // текущего пользователя выделяем цветом
					if (Convert.ToInt32(curID) == ClientAuthentication.UserID)
                    {
                        row.Appearance.ForeColor = System.Drawing.Color.Red;
                        row.ToolTipText = "Текущий пользователь";
                    }
                    // фиксированных - синим цветом и удалять нельзя
                    if (FixedUsers.UserIsFixed(Convert.ToInt32(curID)))
                    {
                        row.Appearance.ForeColor = System.Drawing.Color.Blue;
                        row.Activation = Activation.NoEdit;
                    }

                    if (row.Cells.Exists("ChangePassword"))
                        row.Cells["ChangePassword"].ToolTipText = "Сменить пароль";

                    // проставляем строковое значение в лукапных колонках
                    IUsersManager um = Workplace.ActiveScheme.UsersManager;
                    ((UltraGrid)sender).EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                    SetLookupColumnsValues(row, um);
                    ((UltraGrid)sender).EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    break;
                #endregion

                #region Типы задач
                case NavigationNodeKind.ndTasksTypes:
                    // в модальном режиме делаем доступными только те типы задач которые пользователь может создать
                    if ((InInplaceMode) && (CreatableTaskTypesIds != null) && (CreatableTaskTypesIds.Count > 0))
                    {
                        curID = row.Cells["ID"].Value;
                        if (!CreatableTaskTypesIds.Contains(curID))
                        {
                            e.Row.Activation = Activation.Disabled;
                        }

                    }
                    break;
                #endregion

                #region сессии
                case NavigationNodeKind.ndSessions:
                    bool sessionBlocked = Convert.ToBoolean(row.Cells["isBlocked"].Value);

                    if (sessionBlocked)
                    {
                        row.Appearance.BackColor = Color.DarkOrange;
                        row.Cells["dropSession"].ToolTipText = "Разблокировать сессию";
                        row.Cells["dropSession"].ButtonAppearance.Image = vo.il.Images[3];
                    }
                    else
                    {
                        row.Appearance.ResetBackColor();
                        row.Cells["dropSession"].ToolTipText = "Заблокировать сессию";
                        row.Cells["dropSession"].ButtonAppearance.Image = vo.il.Images[4];
                    }
                    break;
                #endregion
            }
        }

        /// <summary>
        /// Установка параметров для колонок "Подразделение" и "Организация" пользователей
        /// </summary>
        /// <param name="row"></param>
        /// <param name="userManager"></param>
        private void SetLookupColumnsValues(UltraGridRow row, IUsersManager userManager)
        {
            row = CC.UltraGridHelper.GetRowCells(row); 
            if (row.Cells.Exists("REFORGANIZATIONS") && row.Cells.Exists("REFDEPARTMENTS"))
            {
                if (row.Cells["REFORGANIZATIONS"].Value != DBNull.Value)
                    row.Cells["LookupOrg"].Value = userManager.GetNameFromDirectoryByID(
                        DirectoryKind.dkOrganizations,
                        Convert.ToInt32(row.Cells["REFORGANIZATIONS"].Value));
                else
                    row.Cells["LookupOrg"].Value = null;

                if (row.Cells["REFDEPARTMENTS"].Value != DBNull.Value)
                    row.Cells["LookupDep"].Value = userManager.GetNameFromDirectoryByID(
                        DirectoryKind.dkDepartments,
                        Convert.ToInt32(row.Cells["REFDEPARTMENTS"].Value));
                else
                    row.Cells["LookupDep"].Value = null;
            }
        }

        void ugeMembership_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            int userID = 0;
            switch (CurrentNavigationNode)
            {
                case NavigationNodeKind.ndAllGroups:
                    userID = CC.UltraGridHelper.GetRowID(e.Row);
                    break;
                // для фиксированных пользователях изменения в этих таблицах недоступны
                case NavigationNodeKind.ndAllUsers:
                    userID = CC.UltraGridHelper.GetActiveID(vo.ugeAllList._ugData);
                    break;
            }
            if (FixedUsers.UserIsFixed(userID))
                e.Row.Activation = Activation.Disabled;
        }


        void ugeUsersPermissions_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            int i = 3;
            // не является ли пользователь фиксированным?
            int id = CC.UltraGridHelper.GetRowID(e.Row);
            if (FixedUsers.UserIsFixed(id))
                e.Row.Activation = Activation.Disabled;
            // если унаследовано разрешение от группы - 
            // можно запретить пользовательский ввод
            while (i < e.Row.Cells.Count)
            {
                if (e.Row.Cells[i - 1].Value is bool)
                    if (Convert.ToBoolean(e.Row.Cells[i - 1].Value))
                    {
                        e.Row.Cells[i].Activation = Activation.Disabled;
                    }
                e.Row.Cells[i - 1].Activation = Activation.Disabled;
                i = i + 2;
            }
        }

        void ugeAllList_OnBeforeRowDeactivate(object sender, CancelEventArgs e)
        {
            //CheckUsers(this.vo.ugeAllList.ugData.ActiveRow);
            CheckMinorChanges();
            if (ActiveUge == null) return;
            if (ActiveUge.Name != vo.ugeAssignedPermissions.Name)
                ActiveUge.ugData.DisplayLayout.Bands[0].SortedColumns.Clear();
        }

        void  ugeAllList_OnAfterRowActivate(object sender, EventArgs e)
        {
            UltraGridRow activeRow = CC.UltraGridHelper.GetActiveRowCells((UltraGrid)sender);
            LoadObjectsPages(activeRow);

            if (ActiveUge == null || ActiveUge == vo.ugeAssignedPermissions) return;
            if (activeRow.Cells["ID"].Value is string) return;
            if (activeRow.Cells["ID"].Value != DBNull.Value)
                SetParamsForAddedRows(Convert.ToInt32(activeRow.Cells["ID"].Value));
        }

        private void SetParamsForAddedRows(int activeRowID)
        {
            DataRow[] rows = dsAllList.Tables[0].Select(string.Format("ID = {0}", activeRowID));
            if (ActiveUge == null) return;
            if (rows.Length == 0 || rows[0].RowState == DataRowState.Added || rows[0].RowState == DataRowState.Deleted)
                ActiveUge.IsReadOnly = true;//SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.readonlyState);
            else
                ActiveUge.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.editableState);
        }

        void ugeAllList_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            if (/*(!row.IsAddRow) || */(row.Cells["ID"].Value.ToString() != String.Empty)) return;
            UltraGrid ug = (UltraGrid)sender;
            IDatabase db = null;
            ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                // устанавалием новое ID
                string generatorName = String.Empty;
                db = Workplace.ActiveScheme.SchemeDWH.DB;
                switch (CurrentNavigationNode)
                {
                    case NavigationNodeKind.ndAllUsers:
                        generatorName = "g_Users";
                        break;
                    case NavigationNodeKind.ndAllGroups:
                        generatorName = "g_Groups";
                        break;
                    case NavigationNodeKind.ndDivisions:
                        generatorName = "g_departments";
                        break;
                    case NavigationNodeKind.ndOrganizations:
                        generatorName = "g_organizations";
                        break;
                    case NavigationNodeKind.ndTasksTypes:
                        generatorName = "g_TasksTypes";
                        break;
                    default:
                        throw new Exception("Для данного типа объектов добавление недопустимо");
                }
                int newID = db.GetGenerator(generatorName);
                row.Cells["ID"].Value = newID;
                if (row.Cells.Exists("BLOCKED"))
                    row.Cells["BLOCKED"].Value = 0;
                if (row.Cells.Exists("USERTYPE"))
                    row.Cells["USERTYPE"].Value = 0;
                
                if (row.Cells.Exists("ALLOWDOMAINAUTH"))
                    row.Cells["ALLOWDOMAINAUTH"].Value = 0;
                if (row.Cells.Exists("ALLOWPWDAUTH"))
                    row.Cells["ALLOWPWDAUTH"].Value = 0;

                if (row.Cells["ID"].Value != DBNull.Value)
                    SetParamsForAddedRows(Convert.ToInt32(row.Cells["ID"].Value));
            }
            finally
            {
                row.Update();
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                LoadObjectsPages(row);
                if (db != null)
                    db.Dispose();
            }
        }

        private bool CheckNotNullValue(DataTable table, ref string errorString)
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            if (nk == NavigationNodeKind.ndAllObjects)
                return true;
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                if (table.Rows[i].RowState != DataRowState.Deleted)
                {
                    switch (nk)
                    {
                        case NavigationNodeKind.ndAllGroups:
                            if (table.Rows[i]["Name"] == null || table.Rows[i]["Name"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Наименование' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }
                            break;
                        case NavigationNodeKind.ndDivisions:
                            if (table.Rows[i]["Name"] == null || table.Rows[i]["Name"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Наименование' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }
                            break;
                        case NavigationNodeKind.ndAllUsers:
                            if (table.Rows[i]["Name"] == null || table.Rows[i]["Name"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Логин' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }
                            break;
                        case NavigationNodeKind.ndOrganizations:
                            if (table.Rows[i]["Name"] == null || table.Rows[i]["Name"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Наименование' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }
                            break;
                        case NavigationNodeKind.ndTasksTypes:
                            if (table.Rows[i]["Name"] == null || table.Rows[i]["Name"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Наименование' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }

                            if (table.Rows[i]["Code"] == null || table.Rows[i]["Code"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Код' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }

                            if (table.Rows[i]["Name"] == null || table.Rows[i]["Name"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Наименование' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }

                            if (table.Rows[i]["TaskType"] == null || table.Rows[i]["TaskType"].ToString() == string.Empty)
                            {
                                errorString = string.Format("Обязательное поле 'Тип задачи' записи с ID = {0} не заполнено", table.Rows[i]["ID"]);
                                return false;
                            }
                            break;
                    }
                }
            }
            return true;
        }

        #endregion

        #region обработка всех данных грида (сохранение, отмена изменений, очистка данных)

        /// <summary>
        /// очистка данных
        /// </summary>
        void  ugeAllList_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show("Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                NavigationNodeKind nk = CurrentNavigationNode;
                dsAllList.Tables[0].RejectChanges();
                foreach (DataRow row in dsAllList.Tables[0].Rows)
                {
                    switch (nk)
                    {
                        case NavigationNodeKind.ndAllGroups:
                            // удаляем все кроме группы администраторов
                            if (row["NAME"].ToString() != AdminGroupCaption && row["NAME"].ToString() != WebAdminGroupCaption)
                                row.Delete();
                            break;
                        case NavigationNodeKind.ndAllUsers:
                            // удаляем все кроме фиксированных пользователей
                            if (!FixedUsers.UserIsFixed(Convert.ToInt32(row["ID"])))
                                row.Delete();
                            break;
                        default:
                            row.Delete();
                            break;
                    }
                }
                //if (dsAllList.GetChanges() != null)
                ugeAllList_OnSaveChanges(vo.ugeAllList);
                this.CanDeactivate = true;
            }
        }

        /// <summary>
        /// запрос на сохранение данных при выходе 
        /// </summary>
        void SaveChangesWhenExit()
        {
            if (GetChanges())
            {
                if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveChanges();
                    
                }
                else
                {
                    ugeAllList_OnCancelChanges(vo.ugeAllList);
                    vo.ugeAllList.ClearAllRowsImages();

                    if (vo.ugeGroupsPermissions.DataSource != null)
                    {
                        ugeGroupsPermissions_OnCancelChanges(vo.ugeAllList);
                        vo.ugeGroupsPermissions.ClearAllRowsImages();
                    }
                    if (vo.ugeMembership.DataSource != null)
                    {
                        ugeMembership_OnCancelChanges(vo.ugeAllList);
                        vo.ugeMembership.ClearAllRowsImages();
                    }
                    if (vo.ugeUsersPermissions.DataSource != null)
                    {
                        ugeUsersPermissions_OnCancelChanges(vo.ugeAllList);
                        vo.ugeUsersPermissions.ClearAllRowsImages();
                    }
                }
                //vo.ugeAllList.BurnChangesDataButtons(false);
                if (ActiveUge != null)
                    ActiveUge.BurnChangesDataButtons(false);
            }
        }

        bool ugeUsersPermissions_OnSaveChanges(object sender)
        {
            SaveChanges();
            return true;
        }

        bool ugeMembership_OnSaveChanges(object sender)
        {
            SaveChanges();
            return true;
        }

        bool ugeGroupsPermissions_OnSaveChanges(object sender)
        {
            SaveChanges();
            return true;
        }

        bool ugeAssignedPermissions_OnSaveChanges(object sender)
        {
            SaveChanges();
            return true;
        }

        /// <summary>
        /// сохраняем все изменения
        /// </summary>
        /// <returns></returns>
        bool ugeAllList_OnSaveChanges(object sender)
        {
            if (CurrentNavigationNode == NavigationNodeKind.ndAllUsers)
            {
                StringBuilder errors = new StringBuilder();
                if (!CheckAllUsers(ref errors))
                {
                    MessageBox.Show(errors.ToString(), "Сохранение данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            bool successSave = false;

            base.SaveChanges();
            
            DataTable allListChanges = null;
            DataTable membershipsChanges = null;
            DataTable usersPermissionsChanges = null;
            DataTable groupsPermissionsChanges = null;

            if (!GetChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges))
                return true;

            if (SaveChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges))
                successSave = true;
            
            UltraGridRow activeRow = vo.ugeAllList.ugData.ActiveRow;
            vo.ugeAllList.ugData.ActiveRow = null;
            if (activeRow != null)
                vo.ugeAllList.ugData.ActiveRow = activeRow;
            else
                if (vo.ugeAllList.ugData.Rows.Count > 0)
                    vo.ugeAllList.ugData.ActiveRow = vo.ugeAllList.ugData.Rows[0];

            return successSave;
        }

        void ugeGroupsPermissions_OnCancelChanges(object sender)
        {
            if (vo.ugeGroupsPermissions._ugData.ActiveRow != null)
                vo.ugeGroupsPermissions._ugData.ActiveRow.Update();
            DataTable membershipChanges = ((DataTable)vo.ugeGroupsPermissions.DataSource).GetChanges();
            if ((membershipChanges != null) && (membershipChanges.Rows.Count > 0))
                ((DataTable)vo.ugeGroupsPermissions.DataSource).RejectChanges();
        }

        void ugeUsersPermissions_OnCancelChanges(object sender)
        {
            if (vo.ugeUsersPermissions._ugData.ActiveRow != null)
                vo.ugeUsersPermissions._ugData.ActiveRow.Update();
            DataTable usersPermissionsChanges = ((DataTable)vo.ugeUsersPermissions.DataSource).GetChanges();
            if ((usersPermissionsChanges != null) && (usersPermissionsChanges.Rows.Count > 0))
                ((DataTable)vo.ugeUsersPermissions.DataSource).RejectChanges();
        }

        void ugeMembership_OnCancelChanges(object sender)
        {
            if (vo.ugeMembership._ugData.ActiveRow != null)
                vo.ugeMembership._ugData.ActiveRow.Update();
            DataTable membershipChanges = ((DataTable)vo.ugeMembership.DataSource).GetChanges();
            if ((membershipChanges != null) && (membershipChanges.Rows.Count > 0))
                ((DataTable)vo.ugeMembership.DataSource).RejectChanges();
        }

        void ugeAllList_OnCancelChanges(object sender)
        {
            ((DataSet)vo.ugeAllList.DataSource).Tables[0].RejectChanges();
            //object sender = vo.ugeAllList.ugData;
            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            foreach (UltraGridRow row in vo.ugeAllList.ugData.Rows)
            {
                SetLookupColumnsValues(row, um);
            }
        }

        /// <summary>
        /// проверяет изменения для подчиненных гридов
        /// </summary>
        private void CheckMinorChanges()
        {
            DataTable membershipsChanges = null;
            DataTable usersPermissionsChanges = null;
            DataTable groupsPermissionsChanges = null;

            if (!GetMinorChanges(ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges))
                return;

            if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveMinorChanges(ref membershipsChanges, ref usersPermissionsChanges, ref groupsPermissionsChanges);
            }
            else
            {
                CancelMinorChanges(ref membershipsChanges, ref usersPermissionsChanges, ref groupsPermissionsChanges);
            }
        }

        /// <summary>
        /// проверяет изменения для подчиненных гридов
        /// </summary>
        private bool GetMinorChanges(ref DataTable membershipChanges,
            ref DataTable usersPermissionsChanges, ref DataTable groupsPermissionsChanges)
        {
            if (vo.ugeMembership.DataSource != null)
                membershipChanges = ((DataTable)vo.ugeMembership.DataSource).GetChanges();
            if (vo.ugeUsersPermissions.DataSource != null)
                usersPermissionsChanges = ((DataTable)vo.ugeUsersPermissions.DataSource).GetChanges();
            if (vo.ugeGroupsPermissions.DataSource != null)
                groupsPermissionsChanges = ((DataTable)vo.ugeGroupsPermissions.DataSource).GetChanges();

            bool changesPresent =
                (membershipChanges != null) ||
                (usersPermissionsChanges != null) ||
                (groupsPermissionsChanges != null);

            return changesPresent;
        }

        private void CancelMinorChanges(ref DataTable membershipChanges,
            ref DataTable usersPermissionsChanges, ref DataTable groupsPermissionsChanges)
        {
            if ((membershipChanges != null) && (membershipChanges.Rows.Count > 0))
                ((DataTable)vo.ugeMembership.DataSource).RejectChanges();
            if ((usersPermissionsChanges != null) && (usersPermissionsChanges.Rows.Count > 0))
                ((DataTable)vo.ugeUsersPermissions.DataSource).RejectChanges();
            if ((groupsPermissionsChanges != null) && (groupsPermissionsChanges.Rows.Count > 0))
                ((DataTable)vo.ugeGroupsPermissions.DataSource).RejectChanges();
        }

        private void SaveMinorChanges(ref DataTable membershipChanges,
            ref DataTable usersPermissionsChanges, ref DataTable groupsPermissionsChanges)
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            // для некоторых методов необходим ID главного объекта (из грида общего списка)
            int mainID = Convert.ToInt32(vo.ugeAllList.ugData.ActiveRow.Cells["ID"].Value);

            IUsersManager um = this.Workplace.ActiveScheme.UsersManager;

            if ((membershipChanges != null) && (membershipChanges.Rows.Count > 0))
            {
                try
                {
                    switch (nk)
                    {
                        case NavigationNodeKind.ndAllGroups:
                            um.ApplayMembershipChanges(mainID, membershipChanges, false);
                            break;
                        case NavigationNodeKind.ndAllUsers:
                            um.ApplayMembershipChanges(mainID, membershipChanges, true);
                            break;
                    }
                    ((DataTable)vo.ugeMembership.DataSource).AcceptChanges();
                }
                catch
                {
                    //((DataTable)vo.ugeMembership.DataSource).RejectChanges();
                    //MessageBox.Show("Ошибка при сохранении данных. Возможно запись верхнего уровня не сохранена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if ((usersPermissionsChanges != null) && (usersPermissionsChanges.Rows.Count > 0))
            {
                // здесь нужен еще тип объекта
                int objectType = Convert.ToInt32(vo.ugeAllList.ugData.ActiveRow.Cells["OBJECTTYPE"].Value);
                try
                {
                    um.ApplayUsersPermissionsChanges(mainID, objectType, usersPermissionsChanges);
                    ((DataTable)vo.ugeUsersPermissions.DataSource).AcceptChanges();
                }
                catch
                {
                    //((DataTable)vo.ugeUsersPermissions.DataSource).RejectChanges();
                    //MessageBox.Show("Ошибка при сохранении данных. Возможно запись верхнего уровня не сохранена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if ((groupsPermissionsChanges != null) && (groupsPermissionsChanges.Rows.Count > 0))
            {
                // здесь нужен еще тип объекта
                int objectType = Convert.ToInt32(vo.ugeAllList.ugData.ActiveRow.Cells["OBJECTTYPE"].Value);
                try
                {
                    um.ApplayGroupsPermissionsChanges(mainID, objectType, groupsPermissionsChanges);
                    ((DataTable)vo.ugeGroupsPermissions.DataSource).AcceptChanges();
                }
                catch
                {
                    //((DataTable)vo.ugeGroupsPermissions.DataSource).RejectChanges();
                    //MessageBox.Show("Ошибка при сохранении данных. Возможно запись верхнего уровня не сохранена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CheckChanges()
        {
            DataTable allListChanges = null;
            DataTable membershipsChanges = null;
            DataTable usersPermissionsChanges = null;
            DataTable groupsPermissionsChanges = null;

            if (!GetChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges))
                return;

            if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveChanges(ref allListChanges, ref membershipsChanges,
                    ref usersPermissionsChanges, ref groupsPermissionsChanges);
            }
            else
            {
                CancelChanges(ref allListChanges, ref membershipsChanges,
                    ref usersPermissionsChanges, ref groupsPermissionsChanges);
            }
        }

        private bool GetChanges(ref DataTable allListChanges, ref DataTable membershipChanges,
            ref DataTable usersPermissionsChanges, ref DataTable groupsPermissionsChanges)
        {
            if (vo.ugeAllList.DataSource != null)
                if (((DataSet)vo.ugeAllList.DataSource).Tables.Count > 0)
                    allListChanges = ((DataSet)vo.ugeAllList.DataSource).Tables[0].GetChanges();
            if (vo.ugeMembership.DataSource != null)
                membershipChanges = ((DataTable)vo.ugeMembership.DataSource).GetChanges();
            if (vo.ugeUsersPermissions.DataSource != null)
                usersPermissionsChanges = ((DataTable)vo.ugeUsersPermissions.DataSource).GetChanges();
            if (vo.ugeGroupsPermissions.DataSource != null)
                groupsPermissionsChanges = ((DataTable)vo.ugeGroupsPermissions.DataSource).GetChanges();

            bool changesPresent =
                (allListChanges != null) ||
                (membershipChanges != null) ||
                (usersPermissionsChanges != null) ||
                (groupsPermissionsChanges != null);

            return changesPresent;
        }

        /// <summary>
        /// Получаем, были ли изменения вообще
        /// </summary>
        /// <returns></returns>
        private bool GetChanges()
        {
            DataTable allListChanges = null;
            DataTable membershipsChanges = null;
            DataTable usersPermissionsChanges = null;
            DataTable groupsPermissionsChanges = null;
            if (vo.ugeAllList.ugData.ActiveRow != null)
                vo.ugeAllList.ugData.ActiveRow.Update();
            if (vo.ugeAssignedPermissions.ugData.ActiveRow != null)
                vo.ugeAssignedPermissions.ugData.ActiveRow.Update();
            if (vo.ugeGroupsPermissions.ugData.ActiveRow != null)
                vo.ugeGroupsPermissions.ugData.ActiveRow.Update();
            if (vo.ugeMembership.ugData.ActiveRow != null)
                vo.ugeMembership.ugData.ActiveRow.Update();
            if (vo.ugeUsersPermissions.ugData.ActiveRow != null)
                vo.ugeUsersPermissions.ugData.ActiveRow.Update();
            return GetChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges);
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            DataTable allListChanges = null;
            DataTable membershipsChanges = null;
            DataTable usersPermissionsChanges = null;
            DataTable groupsPermissionsChanges = null;

            if (!GetChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges))
                return;

            if (SaveChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges))
            {
                ClearAllRowsImages();
                vo.ugeAllList.BurnChangesDataButtons(false);
            }
        }

        private void ClearAllRowsImages()
        {
            vo.ugeAllList.ClearAllRowsImages();
            vo.ugeAssignedPermissions.ClearAllRowsImages();
            vo.ugeGroupsPermissions.ClearAllRowsImages();
            vo.ugeMembership.ClearAllRowsImages();
            vo.ugeUsersPermissions.ClearAllRowsImages();
        }

        // сохранение всех данных

        public bool SaveChanges(ref DataTable allListChanges, ref DataTable membershipChanges,
            ref DataTable usersPermissionsChanges, ref DataTable groupsPermissionsChanges)
        {
            bool returnValue = false;
            NavigationNodeKind nk = CurrentNavigationNode;
            // для некоторых методов необходим ID главного объекта (из грида общего списка)
            int mainID = 0;
            int objectType = 0;

            if (_inInplaceMode)
                mainID = _inplacedMainID;
            else
                mainID = CC.UltraGridHelper.GetActiveID(vo.ugeAllList.ugData);

            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            if ((allListChanges != null) && (allListChanges.Rows.Count > 0))
            {
                string errString = string.Empty;
                if (CheckNotNullValue(allListChanges, ref errString))
                {
                    switch (nk)
                    {
                        case NavigationNodeKind.ndAllGroups:
                            um.ApplayGroupsChanges(allListChanges);
                            break;
                        case NavigationNodeKind.ndAllUsers:
                            um.ApplayUsersChanges(allListChanges);
                            break;
                        case NavigationNodeKind.ndOrganizations:
                            um.ApplayOrganizationsChanges(allListChanges);
                            break;
                        case NavigationNodeKind.ndTasksTypes:
                            um.ApplayTasksTypesChanges(allListChanges);
                            break;
                        case NavigationNodeKind.ndDivisions:
                            um.ApplayDepartmentsChanges(allListChanges);
                            break;
                        case NavigationNodeKind.ndAllObjects:
                            um.ApplayObjectsChanges(allListChanges);
                            break;
                    }
                    ((DataSet)vo.ugeAllList.DataSource).AcceptChanges();
                    this.CanDeactivate = true;
                    returnValue = true;
                   
                    return returnValue;
                }
                else
                {
                    MessageBox.Show(errString, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //this.CanDeactivate = false;
                    return false;
                }
            }

            if ((membershipChanges != null) && (membershipChanges.Rows.Count > 0))
            {
                try
                {
                    switch (nk)
                    {
                        case NavigationNodeKind.ndAllGroups:
                            um.ApplayMembershipChanges(mainID, membershipChanges, false);
                            break;
                        case NavigationNodeKind.ndAllUsers:
                            um.ApplayMembershipChanges(mainID, membershipChanges, true);
                            break;
                    }
                    ((DataTable)vo.ugeMembership.DataSource).AcceptChanges();
                }
                catch
                {
                    //((DataTable)vo.ugeMembership.DataSource).RejectChanges();
                    throw;
                }
            }

            if ((usersPermissionsChanges != null) && (usersPermissionsChanges.Rows.Count > 0))
            {
                // здесь нужен еще тип объекта
                if (_inInplaceMode)
                    objectType = (int)_inplacedObjectType;
                else
                    objectType = Convert.ToInt32(vo.ugeAllList.ugData.ActiveRow.Cells["OBJECTTYPE"].Value);
                try
                {
                    um.ApplayUsersPermissionsChanges(mainID, objectType, usersPermissionsChanges);
                    ((DataTable)vo.ugeUsersPermissions.DataSource).AcceptChanges();
                }
                catch
                {
                    throw;
                }
            }

            if ((groupsPermissionsChanges != null) && (groupsPermissionsChanges.Rows.Count > 0))
            {
                // здесь нужен еще тип объекта
                if (_inInplaceMode)
                    objectType = (int)_inplacedObjectType;
                else
                    objectType = Convert.ToInt32(vo.ugeAllList.ugData.ActiveRow.Cells["OBJECTTYPE"].Value);
                try
                {
                    um.ApplayGroupsPermissionsChanges(mainID, objectType, groupsPermissionsChanges);
                    ((DataTable)vo.ugeGroupsPermissions.DataSource).AcceptChanges();
                }
                catch
                {
                    throw;
                }
            }
            return true;
        }

        // отмена всех данных

        public override void CancelChanges()
        {
            base.CancelChanges();
            DataTable allListChanges = null;
            DataTable membershipsChanges = null;
            DataTable usersPermissionsChanges = null;
            DataTable groupsPermissionsChanges = null;

            if (!GetChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges))
                return;
            CancelChanges(ref allListChanges, ref membershipsChanges,
                ref usersPermissionsChanges, ref groupsPermissionsChanges);
        }

        public void CancelChanges(ref DataTable allListChanges, ref DataTable membershipChanges,
            ref DataTable usersPermissionsChanges, ref DataTable groupsPermissionsChanges)
        {
            if ((allListChanges != null) && (allListChanges.Rows.Count > 0))
                ((DataSet)vo.ugeAllList.DataSource).RejectChanges();
            if ((membershipChanges != null) && (membershipChanges.Rows.Count > 0))
                ((DataTable)vo.ugeMembership.DataSource).RejectChanges();
            if ((usersPermissionsChanges != null) && (usersPermissionsChanges.Rows.Count > 0))
                ((DataTable)vo.ugeUsersPermissions.DataSource).RejectChanges();
            if ((groupsPermissionsChanges != null) && (groupsPermissionsChanges.Rows.Count > 0))
                ((DataTable)vo.ugeGroupsPermissions.DataSource).RejectChanges();
        }

        #endregion

        #region Инициализация отображения всех данных для всех гридов

        /// <summary>
        /// инициализация каждой строки в гриде, добавление кнопок, хинтов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeAllList_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // добавляем колонки для лукапов в нужные места, из других их удаляем, т.к. грид один на все

            NavigationNodeKind nk = CurrentNavigationNode;
            for (int i = 0; i <= vo.ugeAllList.ugData.DisplayLayout.Bands.Count - 1; i++)
            {
                UltraGridBand band = vo.ugeAllList.ugData.DisplayLayout.Bands[i];
                // удаления вспомогательных колонок
                if (band.Columns.Exists("ChangePassword"))
                    band.Columns.Remove("ChangePassword");

                if (band.Columns.Exists("LookupOrg"))
                    band.Columns.Remove("LookupOrg");

                if (band.Columns.Exists("LookupDep"))
                    band.Columns.Remove("LookupDep");

                if (band.Columns.Exists("LookupTypeTask"))
                    band.Columns.Remove("LookupTypeTask");

                if (band.Columns.Exists("ObjectType"))
                    band.Columns["ObjectType"].SortIndicator = SortIndicator.Ascending;

                band.ColumnFilters.ClearAllFilters();
                UltraGridColumn clmn = null;
                switch (nk)
                {
                    case NavigationNodeKind.ndAllUsers:
                        if (!band.Columns.Exists("ChangePassword"))
                        {
                            clmn = band.Columns.Add("ChangePassword", "Сменить пароль");
                            clmn.Header.VisiblePosition = 7;
                            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
                            clmn.CellActivation = Activation.NoEdit;
                            clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                            clmn.Width = 20;
                            CC.UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, -1);
                            //*************************************************************//
                            // ставим картинку с замком
                            clmn.CellButtonAppearance.Image = vo.ilLittle.Images[0];
                        }
                        if (!band.Columns.Exists("LookupOrg"))
                        {
                            clmn = band.Columns.Add("LookupOrg", "Организация");
                            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
                            clmn.CellActivation = Activation.NoEdit;
                            clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                            clmn.Width = 170;
                            CC.UltraGridHelper.SetLikelyEditButtonColumnsStyle(clmn, -1);
                            clmn.CellButtonAppearance.Image = vo.ilLittle.Images[1];
                            clmn.CellButtonAppearance.ImageVAlign = VAlign.Bottom;
                        }

                        if (!band.Columns.Exists("LookupDep"))
                        {
                            clmn = band.Columns.Add("LookupDep", "Отдел");
                            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
                            clmn.CellActivation = Activation.NoEdit;
                            clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                            clmn.Width = 170;
                            CC.UltraGridHelper.SetLikelyEditButtonColumnsStyle(clmn, -1);
                            clmn.CellButtonAppearance.Image = vo.ilLittle.Images[1];
                            clmn.CellButtonAppearance.ImageVAlign = VAlign.Bottom;
                        }

                        if (band.Columns.Exists("LookupTypeTask"))
                            band.Columns.Remove("LookupTypeTask");

                        break;
                    case NavigationNodeKind.ndTasksTypes:

                        ValueList list = null;
                        if (!vo.ugeAllList.ugData.DisplayLayout.ValueLists.Exists("TaskTypes"))
                        {
                            list = vo.ugeAllList.ugData.DisplayLayout.ValueLists.Add("TaskTypes");
                            ValueListItem item = list.ValueListItems.Add("item0");
                            item.DisplayText = "Общий";
                            item.DataValue = 0;
                        }
                        else
                            list = vo.ugeAllList.ugData.DisplayLayout.ValueLists["TaskTypes"];
                        vo.ugeAllList.ugData.DisplayLayout.Bands[0].Columns["TASKTYPE"].ValueList = list;
                        vo.ugeAllList.ugData.DisplayLayout.Bands[0].Columns["TASKTYPE"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

                        if (band.Columns.Exists("LookupOrg"))
                            band.Columns.Remove("LookupOrg");

                        if (band.Columns.Exists("LookupDep"))
                            band.Columns.Remove("LookupDep");

                        break;
                    case NavigationNodeKind.ndAllObjects:
                        // для объектов настриваем фильтр для иерехического представления
                        ColumnFilter filter = band.ColumnFilters["RefObjectType"];
                        filter.FilterConditions.Add(FilterComparisionOperator.Equals, null);
                        break;

                    #region Сессии пользователей
                    case NavigationNodeKind.ndSessions:
                        if (!band.Columns.Exists("dropSession"))
                        {
                            clmn = band.Columns.Add("dropSession", "Отключить сессию от базы данных");
                            CC.UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, -1);
                            clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                            clmn.Header.VisiblePosition = 0;
                            //clmn.CellButtonAppearance.Image = vo.il.Images[4];
                        }
                        break;
                    #endregion
                }
            }
        }

        void ugeAllList_OnGetHierarchyColumns(ref string parentColumnName, ref string refParentColumnName)
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            if (nk == NavigationNodeKind.ndAllObjects)
            {
                parentColumnName = "OBJECTTYPE";
                refParentColumnName = "REFOBJECTTYPE";
            }
            else
            {
                parentColumnName = string.Empty;
                refParentColumnName = string.Empty;
            }

        }

        void ugData_InitializeGroupByRow(object sender, InitializeGroupByRowEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
        }

        int ugeAllList_OnGetHierarchyLevelsCount()
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            if (nk == NavigationNodeKind.ndAllObjects)
                return 2;
            return 1;
        }

        /// <summary>
        /// названия кнопок для добавления записей
        /// </summary>
        /// <returns></returns>
        string[] ugeAllList_OnGetHierarchyLevelsNames()
        {
            string[] LevelName = new string[1];
            NavigationNodeKind nk = CurrentNavigationNode;
            switch (nk)
            {
                case NavigationNodeKind.ndAllGroups:
                    LevelName[0] = "Группа";
                    break;
                case NavigationNodeKind.ndAllUsers:
                    LevelName[0] = "Пользователь";
                    break;
                case NavigationNodeKind.ndDivisions:
                    LevelName[0] = "Отдел";
                    break;
                case NavigationNodeKind.ndOrganizations:
                    LevelName[0] = "Организация";
                    break;
                case NavigationNodeKind.ndTasksTypes:
                    LevelName[0] = "Вид задачи";
                    break;
            }
            return LevelName;
        }

        void ugeUsersGroupsPermissions_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand band = e.Layout.Bands[i];
                UltraGridColumn clmn = band.Columns["ID"];
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                //clmn.Header.VisiblePosition = 1;
                clmn.Width = 40;

                clmn = band.Columns["NAME"];
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.CellActivation = Activation.ActivateOnly;
                //clmn.Header.VisiblePosition = 2;
                clmn.Header.Caption = "Наименование";
                clmn.Width = 150;

                // для колонок начиная с третьей (в гриде пользователей) нужно произвести разыменовку 
                // (код операции -> Caption операции)
                // через одну, предыдущая - унаследованные разрешения
                int j = 2;
                BindingSource bs = (BindingSource)((UltraGrid)sender).DataSource;
                DataView dv = (DataView)bs.DataSource;
                bool isUsers = dv.Table.TableName != "GroupsPermissions";
                /*DataTable dt = (DataTable)((UltraGrid)sender).DataSource;
                //DataView dv = (DataView)((UltraGrid)sender).DataSource;
                //DataTable dt = dv.Table;
                bool isUsers = dt.TableName != "GroupsPermissions";*/
                if (isUsers)
                    j = 3;
                while (j < band.Columns.Count - 1)
                {
                    clmn = band.Columns[j];
                    clmn.AllowGroupBy = DefaultableBoolean.False;
                    clmn.AllowRowFiltering = DefaultableBoolean.False;
                    clmn.AllowRowSummaries = AllowRowSummaries.False;
                    string curCaption = um.GetCaptionForOperation(Convert.ToInt32(clmn.Key));
                    clmn.Header.Caption = curCaption;
                    clmn.Width = 80;

                    if (isUsers)
                    {
                        // теперь настраиваем предыдущую (это унаследованные права)
                        clmn = band.Columns[j - 1];
                        clmn.AllowGroupBy = DefaultableBoolean.False;
                        clmn.AllowRowFiltering = DefaultableBoolean.False;
                        clmn.AllowRowSummaries = AllowRowSummaries.False;
                        //clmn.CellActivation = Activation.Disabled;
                        clmn.CellActivation = Activation.Disabled;
                        clmn.Header.Caption = curCaption + " (от групп)";
                        clmn.Width = 80;
                        clmn.CellAppearance.BackColor = System.Drawing.SystemColors.ButtonFace;

                        j = j + 2;
                    }
                    else
                        j = j + 1;
                }
            }
        }

        void ugeMembership_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            // users
            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand band = e.Layout.Bands[i];
                UltraGridColumn clmn = band.Columns["ID"];
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowFiltering = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.Header.VisiblePosition = 1;
                clmn.Width = 40;

                clmn = band.Columns["NAME"];
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.Header.VisiblePosition = 2;
                clmn.Header.Caption = "Наименование";
                clmn.Width = 150;

                clmn = band.Columns["ISMEMBER"];
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.Header.VisiblePosition = 3;
                clmn.Header.Caption = "Вхождение";
                clmn.Width = 80;
            }
        }

        void ugeAssignedPermissions_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            NavigationNodeKind nk = CurrentNavigationNode;
            UltraGrid grid = (UltraGrid)sender;
            UltraGridColumn cl = grid.DisplayLayout.Bands[0].Columns["ObjectCaption"];

            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand band = e.Layout.Bands[i];
                UltraGridColumn clmn = band.Columns["ObjectID"];
                clmn.Hidden = true;
                clmn = band.Columns["AllowedAction"];
                clmn.Hidden = true;

                clmn = band.Columns["ObjectCaption"];
                clmn.AllowRowFiltering = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.Header.Caption = "Объект";
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.Width = 300;

                clmn = band.Columns["AllowedActionCaption"];
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowFiltering = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.Header.Caption = "Разрешенные операции";
                clmn.CellActivation = Activation.ActivateOnly;
                clmn.Width = 400;
            }
        }

        #endregion

        #region Обработчики объекта навигации

        internal static NavigationNodeKind GetNavigationNodeKindFromName(string nodeName)
        {
            switch (nodeName)
            {
                case AdministrationNavigationObjectKeys.Users:
                    return NavigationNodeKind.ndAllUsers;
                case AdministrationNavigationObjectKeys.Groups:
                    return NavigationNodeKind.ndAllGroups;
                case AdministrationNavigationObjectKeys.Objects:
                    return NavigationNodeKind.ndAllObjects;
                case AdministrationNavigationObjectKeys.Organizations:
                    return NavigationNodeKind.ndOrganizations;
                case AdministrationNavigationObjectKeys.Divisions:
                    return NavigationNodeKind.ndDivisions;
                case AdministrationNavigationObjectKeys.TaskTypes:
                    return NavigationNodeKind.ndTasksTypes;
                case AdministrationNavigationObjectKeys.Sessions:
                    return NavigationNodeKind.ndSessions;
                default:
                    return NavigationNodeKind.ndUnknown;
            }
        }

        internal static string GetCaptionFromNavigationNodeKind(NavigationNodeKind nodeKind)
        {
            switch (nodeKind)
            {
                case NavigationNodeKind.ndAllUsers:
                    return "Пользователи";
                case NavigationNodeKind.ndAllGroups:
                    return "Группы";
                case NavigationNodeKind.ndAllObjects:
                    return "Объекты системы";
                case NavigationNodeKind.ndAllDirectoryes:
                    return "Справочники";
                case NavigationNodeKind.ndOrganizations:
                    return "Организации";
                case NavigationNodeKind.ndDivisions:
                    return "Отделы";
                case NavigationNodeKind.ndTasksTypes:
                    return "Виды задач";
                case NavigationNodeKind.ndSessions:
                    return "Сессии";
                case NavigationNodeKind.ndUnknown:
                    return "???";
                default:
                    throw new ArgumentException(String.Format("Неизвестный тип перечисленяи: {0}", nodeKind));
            }
        }

        internal void LoadData()
        {
            vo.ugeAllList._ugData.DataSource = null;

            if (!_inInplaceMode)
            {
                adminCaption = GetCaptionFromNavigationNodeKind(CurrentNavigationNode);
                //Workplace.ViewObjectCaption = adminCaption;
                ViewCtrl.Text = adminCaption;
            }
            vo.spcTemplate.Visible = true;
            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            vo.spcTemplate.Panel2Collapsed = false;

            vo.ugeAllList.Visible = true;
            NavigationNodeKind selectedNodeKind = CurrentNavigationNode;
            UltraToolbar tb = vo.ugeAllList.utmMain.Toolbars["userToolBar"];
            tb.Visible = false;

            DataTable dt = new DataTable();
            switch (selectedNodeKind)
            {
                case NavigationNodeKind.ndAllUsers:
                    dt = um.GetUsers();
                    // мочим странные проявления дат, которые вставляются сами по себе
                    // во вновь добавленные записи
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!row.IsNull("LASTLOGIN") && Convert.ToDateTime(row["LASTLOGIN"]).Year == 1)
                        {
                            row["LASTLOGIN"] = DBNull.Value;
                            row.AcceptChanges();
                        }
                    }
                    // показываем кнопку добавления пользователей
                    // если кнопки нет, то добавляем
                    if (!vo.ugeAllList.utmMain.Tools.Exists("AddUser"))
                    {
                        ButtonTool addUser = new ButtonTool("AddUser");
                        addUser.SharedProps.ToolTipText = "Импортировать пользователя из домена";
                        addUser.SharedProps.AppearancesSmall.Appearance.Image = vo.il.Images[0];
                        vo.ugeAllList.utmMain.Tools.Add(addUser);
                        tb.Tools.AddTool("AddUser");
                    }
                    tb.Visible = true;
                    vo.ugeAllList.SingleBandLevelName = "Пользователь";
                    break;
                case NavigationNodeKind.ndAllGroups:
                    dt = um.GetGroups();
                    vo.ugeAllList.SingleBandLevelName = "Группа";
                    break;
                case NavigationNodeKind.ndAllObjects:
                    dt = um.GetObjects();
                    break;
                case NavigationNodeKind.ndTasksTypes:
                    dt = um.GetTasksTypes();
                    vo.ugeAllList.SingleBandLevelName = "Вид задачи";
                    break;
                case NavigationNodeKind.ndOrganizations:
                    dt = um.GetOrganizations();
                    vo.ugeAllList.SingleBandLevelName = "Организация";
                    break;
                case NavigationNodeKind.ndDivisions:
                    dt = um.GetDepartments();
                    vo.ugeAllList.SingleBandLevelName = "Департамент";
                    break;
                case NavigationNodeKind.ndSessions:
                    GetSessionsFromScheme(ref dt);
                    break;
            }

            dsAllList = new DataSet();
            //dsAllList.Relations.Clear();
            //dsAllList.Tables.Clear();

            if (dt != null)
                dsAllList.Tables.Add(dt);

            if (selectedNodeKind == NavigationNodeKind.ndAllObjects)
            {
                DataColumn ParentColumn = dsAllList.Tables[0].Columns["OBJECTTYPE"];
                DataColumn ChildColumn = dsAllList.Tables[0].Columns["REFOBJECTTYPE"];
                DataRelation hr = new DataRelation("HierarchyRelation", ParentColumn, ChildColumn, false);
                dsAllList.Relations.Add(hr);
            }

            // загружаем данные
            LoadAllList(dsAllList);

            // выставляем параметры основного грида
            //vo.ugeAllList.StateRowEnable = (selectedNodeKind != NavigationNodeKind.ndAllObjects) && (selectedNodeKind != NavigationNodeKind.ndSessions);

            switch (selectedNodeKind)
            {
                case NavigationNodeKind.ndAllUsers:
                    vo.ugeAllList.AllowAddNewRecords = true;
                    vo.ugeAllList.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.allowAddState);
                    break;
                case NavigationNodeKind.ndAllGroups:
                    vo.ugeAllList.AllowAddNewRecords = true;
                    vo.ugeAllList.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.allowAddState);
                    break;
                case NavigationNodeKind.ndAllObjects:
                    vo.ugeAllList.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.editableState);
                    vo.ugeAllList.utmMain.EventManager.SetEnabled(Infragistics.Win.UltraWinToolbars.EventGroups.AllEvents, false);
                    ((StateButtonTool)vo.ugeAllList.utmMain.Tools["ShowHierarchy"]).Checked = true;
                    vo.ugeAllList.utmMain.EventManager.SetEnabled(Infragistics.Win.UltraWinToolbars.EventGroups.AllEvents, true);
                    vo.ugeAllList.ugData.DisplayLayout.GroupByBox.Hidden = true;
                    vo.ugeAllList.utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;
                    break;
                case NavigationNodeKind.ndAllDirectoryes:
                    vo.ugeAllList.Visible = false;
                    break;
                case NavigationNodeKind.ndTasksTypes:
                    vo.ugeAllList.AllowAddNewRecords = true;
                    vo.spcTemplate.Panel2Collapsed = true;
                    vo.ugeAllList.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.allowAddState);
                    break;
                case NavigationNodeKind.ndOrganizations:
                    vo.ugeAllList.AllowAddNewRecords = true;
                    vo.spcTemplate.Panel2Collapsed = true;
                    vo.ugeAllList.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.allowAddState);
                    break;
                case NavigationNodeKind.ndDivisions:
                    vo.ugeAllList.AllowAddNewRecords = true;
                    vo.spcTemplate.Panel2Collapsed = true;
                    vo.ugeAllList.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.allowAddState);
                    break;
                case NavigationNodeKind.ndSessions:
                    vo.ugeAllList.AllowAddNewRecords = false;
                    vo.spcTemplate.Panel2Collapsed = true;
                    vo.ugeAllList.AllowEditRows = false;
                    vo.ugeAllList.IsReadOnly = true;
                    vo.ugeAllList.AllowClearTable = false;
                    vo.ugeAllList.AllowDeleteRows = false;
                    vo.ugeAllList.AllowImportFromXML = false;
                    vo.ugeAllList.StateRowEnable = false;
                    
                    break;
            }

            if ((dt != null) && (dt.Rows.Count > 0))
            {
                vo.utcPages.Visible = true;
                vo.ugeAllList.ugData.ActiveRow = vo.ugeAllList.ugData.Rows[0];
            }
            else
            {
                vo.utcPages.Visible = false;
            }
            // фильтр на сессии
            if (selectedNodeKind == NavigationNodeKind.ndSessions)
            {
                ColumnFilter filter = vo.ugeAllList.ugData.DisplayLayout.Bands[0].ColumnFilters["SessionType"];
                filter.FilterConditions.Add(FilterComparisionOperator.NotEquals, SessionClientType.Server);
                filter.FilterConditions.Add(FilterComparisionOperator.NotEquals, SessionClientType.DataPump);
            }
        }

        void GetSessionsFromScheme(ref DataTable dt)
        {
            DataRow row = null;
            if (dt == null)
                dt = new DataTable();
            dt.BeginLoadData();
            dt.Rows.Clear();
            dt.Columns.Clear();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("SessionName", typeof(string));
            dt.Columns.Add("LogonTime", typeof(string));
            dt.Columns.Add("Host", typeof(string));
            dt.Columns.Add("Application", typeof(string));
            dt.Columns.Add("ResourcesCount", typeof(string));
            dt.Columns.Add("SessionType", typeof(string));
            dt.Columns.Add("isBlocked", typeof(bool));
            vo.ugeAllList.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            foreach (ISession session in this.Workplace.ActiveScheme.SessionManager.Sessions.Values)
            {
                row = dt.Rows.Add(null, null);
                row.BeginEdit();
                row["ID"] = session.SessionId;
                row["SessionName"] = session.Principal.Identity.Name;
                row["LogonTime"] = session.LogonTime;
                row["Host"] = session.Host;
                row["Application"] = session.Application;
                row["ResourcesCount"] = session.ResourcesCount;
                row["SessionType"] = session.ClientType;
                row["isBlocked"] = session.IsBlocked;
                row.EndEdit();
            }
            dt.EndLoadData();
            vo.ugeAllList.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            dt.AcceptChanges(); 
        }

        private void tvNavigation_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            CheckChanges();

            foreach (UltraGridBand band in vo.ugeAllList.ugData.DisplayLayout.Bands)
            {
                band.SortedColumns.Clear();
            }
        }

        private void LoadAllList(DataSet ds)
        {
            vo.ugeAllList.ugData.BeginUpdate();
            try
            {
                //vo.ugeAllList.DataSource = null;
                if (ds.Tables.Count > 0)
                {
                    vo.ugeAllList.DataSource = ds;
                    //vo.ugeAllList.ugData.Rows.ExpandAll(true);
                }
            }
            finally
            {
                vo.ugeAllList.ugData.EndUpdate();
            }
        }
        #endregion

        #region Обработчики пэйдж-контрола

        Krista.FM.Client.Components.UltraGridEx ActiveUge;

        private void utcPages_SelectedTabChanging(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangingEventArgs e)
        {

            if (vo.ugeAllList.ugData.ActiveRow == null)
                return;
            if (!vo.spcTemplate.Visible)
                return;
            // если кто то не сохранил главную запись, то ничего не сохраняем вообще в подчиненных
            
            CheckMinorChanges();

            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            UltraGridRow activeRow = CC.UltraGridHelper.GetActiveRowCells(vo.ugeAllList.ugData);
            object id = activeRow.Cells["ID"].Value;
            DataTable dt = null;
            int tp = 0;
            ActiveUge = null;
            switch (e.Tab.Key)
            {
                case "tbMembership":
                    switch (CurrentNavigationNode)
                    {
                        case NavigationNodeKind.ndAllGroups:
                            dt = um.GetUsersForGroup(Convert.ToInt32(id));
                            vo.ugeMembership.DataSource = null;
                            vo.ugeMembership.DataSource = dt;
                            break;
                        case NavigationNodeKind.ndAllUsers:
                            dt = um.GetGroupsForUser(Convert.ToInt32(id));
                            vo.ugeMembership.DataSource = null;
                            vo.ugeMembership.DataSource = dt;
                            break;
                    }
                    ActiveUge = vo.ugeMembership;
                    break;
                case "tbAssignedPermissions":
                    switch (CurrentNavigationNode)
                    {
                        case NavigationNodeKind.ndAllGroups:
                            dt = um.GetAssignedPermissions(Convert.ToInt32(id), false);
                            vo.ugeAssignedPermissions.DataSource = null;
                            vo.ugeAssignedPermissions.DataSource = dt;
                            break;
                        case NavigationNodeKind.ndAllUsers:
                            dt = um.GetAssignedPermissions(Convert.ToInt32(id), true);
                            vo.ugeAssignedPermissions.DataSource = null;
                            vo.ugeAssignedPermissions.DataSource = dt;
                            break;
                    }
                    ActiveUge = vo.ugeAssignedPermissions;
                    vo.ugeAssignedPermissions.ugData.DisplayLayout.Bands[0].SortedColumns.Add("ObjectCaption", false, true);
                    vo.ugeAssignedPermissions.ugData.Rows.ExpandAll(true);
                    ActiveUge.AllowAddNewRecords = false;
                    ActiveUge.AllowClearTable = false;
                    ActiveUge.AllowDeleteRows = false;
                    ActiveUge.AllowEditRows = false;
                    ActiveUge.AllowImportFromXML = false;
                    ActiveUge.StateRowEnable = false;
                    ActiveUge.IsReadOnly = true;
//SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.readonlyState);
                    break;
                case "tbGroupsPermissions":
                    tp = Convert.ToInt32(activeRow.Cells["OBJECTTYPE"].Value);
                    dt = um.GetGroupsPermissionsForObject(Convert.ToInt32(id), tp);
                    vo.ugeGroupsPermissions.DataSource = null;
                    vo.ugeGroupsPermissions.DataSource = dt;
                    ActiveUge = vo.ugeGroupsPermissions;
                    SetParentSettings();
                    break;
                case "tbUsersPermissions":
                    tp = Convert.ToInt32(activeRow.Cells["OBJECTTYPE"].Value);
                    dt = um.GetUsersPermissionsForObject(Convert.ToInt32(id), tp);
                    vo.ugeUsersPermissions.DataSource = null;
                    vo.ugeUsersPermissions.DataSource = dt;
                    ActiveUge = vo.ugeUsersPermissions;
                    ActiveUge.AllowAddNewRecords = false;
                    ActiveUge.AllowClearTable = false;
                    ActiveUge.AllowDeleteRows = false;
                    SetParentSettings();
                    foreach (UltraGridRow row in ActiveUge.ugData.Rows)
                        row.Refresh(RefreshRow.FireInitializeRow);
                    break;
            }
        }


        private void SetParentSettings()
        {
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(vo.ugeAllList.ugData);
            if (CurrentNavigationNode == NavigationNodeKind.ndAllObjects)
            {
                // если у текущей записи есть родительский элемент
                UltraGridRow parentRow = activeRow.ParentRow;
                while (parentRow != null)
                {
                    parentRow = UltraGridHelper.GetRowCells(parentRow);
                    IUsersManager um = Workplace.ActiveScheme.UsersManager;
                    // получаем права родительского элемента
                    int id = Convert.ToInt32(parentRow.Cells["ID"].Value);
                    int objectType = Convert.ToInt32(parentRow.Cells["OBJECTTYPE"].Value);
                    DataTable table = null;
                    if (ActiveUge == vo.ugeGroupsPermissions)
                        table = um.GetGroupsPermissionsForObject(id, objectType);
                    else if (ActiveUge == vo.ugeUsersPermissions)
                        table = um.GetUsersPermissionsForObject(id, objectType);
                    DataTable currentPermissionsTable = (DataTable)ActiveUge.DataSource;
                    ActiveUge.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                    try
                    {
                        // для всех записей в гриде с правами
                        foreach (UltraGridRow row in ActiveUge.ugData.Rows)
                        {
                            // берем соответствующую ей родительскую запись с правами
                            DataRow permissionRow = table.Select(string.Format("ID = {0}", row.Cells["ID"].Value))[0];
                            for (int i = 2; i <= ActiveUge.ugData.DisplayLayout.Bands[0].Columns.Count - 1; i++)
                            {
                                if (ActiveUge.ugData.DisplayLayout.Bands[0].Columns[i].Key == "29002")
                                {
                                    foreach (DataColumn column in permissionRow.Table.Columns)
                                    {
                                        if (column.ColumnName.Contains("07"))
                                        {
                                            if (Convert.ToBoolean(permissionRow[column]))
                                            {
                                                row.Cells[i].Activation = Activation.Disabled;
                                                row.Cells[i].Value = permissionRow[i];
                                            }
                                            break;
                                        }
                                    }
                                }
                                else
                                if (ActiveUge.ugData.DisplayLayout.Bands[0].Columns[i].Key != "pic")
                                    if (Convert.ToBoolean(permissionRow[i]))
                                    {
                                        row.Cells[i].Activation = Activation.Disabled;
                                        row.Cells[i].Value = permissionRow[i];
                                    }
                            }
                            currentPermissionsTable.Select(string.Format("ID = {0}", row.Cells["ID"].Value))[0].AcceptChanges();
                            row.Update();
                        }
                    }
                    finally
                    {
                        ActiveUge.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    }
                    parentRow = parentRow.ParentRow;
                }
            }
        }


        private void LoadObjectsPages(UltraGridRow row)
        {
            // если это новая строка - прячем
            if ((row == null) || (row.IsAddRow))
            {
                vo.utcPages.Visible = false;
                return;
            }
            else
            {
                if (!vo.utcPages.Visible)
                    vo.utcPages.Visible = true;
            }

            vo.utcPages.BeginUpdate();
            vo.utcPages.EventManager.SetEnabled(UltraTabControlEventGroup.AllEvents, false);
            // запоминаем активную вкладку 
            UltraTab activeTab = vo.utcPages.ActiveTab;
            try
            {
                //foreach (UltraTab tab in vo.utcPages.Tabs)
                //    tab.Visible = false;

                UltraTab tb1 = null;
                UltraTab tb2 = null;
                switch (CurrentNavigationNode)
                {
                    case NavigationNodeKind.ndAllUsers:
                        tb1 = vo.utcPages.Tabs["tbMembership"];
                        tb1.Text = "Членство в группах";
                        tb1.Visible = true;

                        tb2 = vo.utcPages.Tabs["tbAssignedPermissions"];
                        tb2.Text = "Назначенные права";
                        tb2.Visible = true;
                        break;
                    case NavigationNodeKind.ndAllGroups:
                        tb1 = vo.utcPages.Tabs["tbMembership"];
                        tb1.Text = "Члены группы";
                        tb1.Visible = true;

                        tb2 = vo.utcPages.Tabs["tbAssignedPermissions"];
                        tb2.Text = "Назначенные права";
                        tb2.Visible = true;
                        break;
                    case NavigationNodeKind.ndAllObjects:
                        tb1 = vo.utcPages.Tabs["tbGroupsPermissions"];
                        tb1.Text = "Права групп";
                        tb1.Visible = true;

                        tb2 = vo.utcPages.Tabs["tbUsersPermissions"];
                        tb2.Text = "Права пользователей";
                        tb2.Visible = true;

                        break;
                    case NavigationNodeKind.ndAllDirectoryes:
                        break;
                }

                if (tb1 != null)
                {
                    tb1.Selected = true;
                }
                    

                foreach (UltraTab tab in vo.utcPages.Tabs)
                {
                    if (tab != tb1 && tab != tb2)
                        tab.Visible = false;
                }
                // если стоим на добавленной записи, то запрещаем какое либо редактирование подчиненных записей
            }
            finally
            {
                vo.utcPages.EventManager.SetEnabled(UltraTabControlEventGroup.AllEvents, true);
                vo.utcPages.EndUpdate();
                //if 
                // инициируем загрузку активной страницы PageControl'а если он виден
                if (vo.utcPages.Visible)
                    if (vo.utcPages.ActiveTab != activeTab && activeTab != null)
                    {
                        if (activeTab.Visible)
                            vo.utcPages.SelectedTab = activeTab;
                        else
                        {
                            SelectedTabChangingEventArgs e = new SelectedTabChangingEventArgs(vo.utcPages.ActiveTab);
                            this.utcPages_SelectedTabChanging(vo.utcPages, e);
                        }
                    }
                    else
                    {
                        SelectedTabChangingEventArgs e = new SelectedTabChangingEventArgs(vo.utcPages.ActiveTab);
                        this.utcPages_SelectedTabChanging(vo.utcPages, e);
                    }
            }
        }

        /// <summary>
        /// проверяет активную запись верхнего грида, если она добавлена или удалена
        /// </summary>
        /// <param name="activeUltraGridRow"></param>
        /// <param name="activeNode"></param>
        /// <returns>да, если запись нормальная, нет, если удалена или добавлена</returns>
        private bool CheckActiveRow(UltraGridRow activeUltraGridRow, NavigationNodeKind activeNode)
        {
            if (ActiveUge == null)
                return true;

            string selectFilter = string.Format("ID = {0}", activeUltraGridRow.Cells["ID"].Value);
            DataRow[] rows = dsAllList.Tables[0].Select(selectFilter);
            DataRow activeDataRow = rows[0];
            if (activeDataRow.RowState == DataRowState.Added || activeDataRow.RowState == DataRowState.Deleted)
            {
                if (ActiveUge.DataSource != null)
                    ActiveUge.IsReadOnly = true;
                return false;
            }
            if (ActiveUge != null)
                if (ActiveUge.DataSource != null)
                    ActiveUge.IsReadOnly = false;
            return true;
        }

        #endregion

        #region Обработчики грида членства
        

        #endregion

        #region Обработчики грида назначенных прав для пользователей/групп
        

        #endregion

        #region Обработчики для гридов прав на объект для пользователей и групп
        

        private void ugUsersPermissions_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            int i = 3;
            while (i < e.Row.Cells.Count)
            {
                // если унаследовано разрешение от группы - 
                // можно запретить пользовательский ввод
                if (Convert.ToBoolean(e.Row.Cells[i - 1].Value))
                {
                    //e.Row.Cells[i].Appearance.BackColor = System.Drawing.SystemColors.ButtonFace;
                    e.Row.Cells[i].Activation = Activation.Disabled;
                }
                i = i + 2;
            }
        }

        #endregion

        #region реализация интерфейса IInplaceTasksPermissionsView
        private bool _inInplaceMode = false;
        public bool InInplaceMode
        {
            get { return _inInplaceMode; }
            set { _inInplaceMode = value; }
        }

        private int _inplacedMainID = 0;
        private SysObjectsTypes _inplacedObjectType = SysObjectsTypes.Unknown;

        public void AttachViewObject(Control groupsPermissionsArea, Control usersPermissionsArea)
        {
            _inInplaceMode = true;
            vo.ugeGroupsPermissions.Parent = groupsPermissionsArea;
            vo.ugeGroupsPermissions.Dock = DockStyle.Fill;
            vo.ugeUsersPermissions.Parent = usersPermissionsArea;
            vo.ugeUsersPermissions.Dock = DockStyle.Fill;

			// Настраиваем отображение в режиме InplaceMode
        	vo.ugeGroupsPermissions.ugData.DisplayLayout.GroupByBox.Hidden = true;
			vo.ugeGroupsPermissions.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			vo.ugeGroupsPermissions.utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;
			
			vo.ugeUsersPermissions.ugData.DisplayLayout.GroupByBox.Hidden = true;
			vo.ugeUsersPermissions.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			vo.ugeUsersPermissions.utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;
			vo.ugeUsersPermissions.AllowAddNewRecords = false;
		}

        public void DetachViewObject()
        {
            if (!_inInplaceMode) return;
            vo.ugeGroupsPermissions.Parent = null;
            vo.ugeUsersPermissions.Parent = null;
        }

        public void Close()
        {
            if (!_inInplaceMode) return;
            DetachViewObject();
            this.InternalFinalize();
        }

        public void RefreshAttachedData(int mainID, SysObjectsTypes tp, bool isUsers)
        {
            if (!_inInplaceMode) return;

            _inplacedMainID = mainID;
            _inplacedObjectType = tp;

            DataTable dt = null;
            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            if (!isUsers)
            {
                dt = um.GetGroupsPermissionsForObject(_inplacedMainID, (int)_inplacedObjectType);
                vo.ugeGroupsPermissions.DataSource = null;
                vo.ugeGroupsPermissions.DataSource = dt;
            }
            else
            {
                dt = um.GetUsersPermissionsForObject(_inplacedMainID, (int)_inplacedObjectType);
                vo.ugeUsersPermissions.DataSource = null;
                vo.ugeUsersPermissions.DataSource = dt;
            }
        }

        public void ClearAttachedData(bool isUsers)
        {
            if (!_inInplaceMode) return;

            DataTable changes = null;
            DataTable source = null;
            if (isUsers)
                source = ((DataTable)vo.ugeUsersPermissions.DataSource);
            else
                source = ((DataTable)vo.ugeGroupsPermissions.DataSource);

            if (source != null)
                changes = source.GetChanges();

            if ((changes != null) && (changes.Rows.Count > 0) &&
                MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                IUsersManager um = Workplace.ActiveScheme.UsersManager;
                if (isUsers)
                    um.ApplayUsersPermissionsChanges(_inplacedMainID, (int)_inplacedObjectType, changes);
                else
                    um.ApplayGroupsPermissionsChanges(_inplacedMainID, (int)_inplacedObjectType, changes);
                source.AcceptChanges();
            }
            else
            {
                if (source != null)
                    source.RejectChanges();
            }
        }

        public IUsersModal IUserModalForm
        {
            get 
            {
                if (userModal != null)
                    return userModal;
                else
                {
                    UsersModalForm frm = new UsersModalForm(this.Workplace);
                    return (IUsersModal)frm;
                }
            }
        }


        #endregion
    }
}
