using System;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Infragistics.Excel;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
	public class TasksNavigation : BaseViewObject.BaseNavigationCtrl
	{
        public const int MAX_TASKS_HIERARCHY_LEVEL = 100;

        #region Форматы для CLIPBOARD
        public static string CLPB_COPY_TASKS_XML = "CLPB_COPY_TASKS_XML";
        public static string CLPB_CUT_TASKS_XML = "CLPB_CUT_TASKS_XML";
        public static string CLPB_TASKS_OPERATION_COMPLETED = "CLPB_TASKS_OPERATION_COMPLETED";

        public static string CLPB_CUT_DOCUMENTS_XML = "CLPB_CUT_DOCUMENTS_XML";
        public static string CLPB_COPY_DOCUMENTS_XML = "CLPB_COPY_DOCUMENTS_XML";
        public static string CLPB_DOCUMENTS_OPERATION_COMPLETED = "CLPB_DOCUMENTS_OPERATION_COMPLETED";

        public static string CLPB_OPERATION_CANCELED = "CLPB_OPERATION_CANCELED";
        #endregion

        #region Права пользователя на определенные действия

        internal bool AllowAddNewTask { get; set; }

        internal bool AllowEditTask { get; set; }

        internal bool AllowDeleteTask { get; set; }

        #endregion

        private static TasksNavigation instance;

        internal static TasksNavigation Instance
        {
            get
            {
                if (instance == null)
                { 
                    instance = new TasksNavigation();
                }
                return instance;
            }
        }

        private TasksViewObj taskViewObject;

        private Dictionary<string, TasksViewObj> taskViewObjects = new Dictionary<string, TasksViewObj>(); 

        internal TaskStubManager returnedTasks = new TaskStubManager();

        private DataSet ds = new DataSet();

        public TasksNavigation()
        {
            instance = this;
            Caption = "Задачи";
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.Task_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.Task_24; }
        }

        private TasksSearch searchForm;
        private IInplaceTasksPermissionsView taskPermissions;
        private IUsersModal usersModalForm;

        internal ClipboardChangeNotifier clipboardChangeNotifier = null;

	    private bool isFirstTask;

        public override void Initialize()
        {
            InitializeComponent();
            ugTasks.DataSource = null;
            ugTasks.ResetLayouts();
            ugTasks.BeforeRowActivate += new Infragistics.Win.UltraWinGrid.RowEventHandler(ugTasks_BeforeRowActivate);
            ugTasks.AfterRowActivate += new System.EventHandler(ugTasks_AfterRowActivate);
            ugTasks.BeforeRowDeactivate += new CancelEventHandler(ugTasks_BeforeRowDeactivate);
            ugTasks.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(ugTasks_InitializeRow);
            ugTasks.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(ugTasks_InitializeLayout);
            ugTasks.KeyDown += new KeyEventHandler(ugTasks_KeyDown);
            ugTasks.MouseClick += new MouseEventHandler(ugTasks_MouseClick);
            ugTasks.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(ugTasks_MouseEnterElement);
            ugTasks.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

            Workplace.ViewClosed += new Krista.FM.Client.Common.Gui.ViewContentEventHandler(Workplace_ViewClosed);
            Workplace.ActiveWorkplaceWindowChanged += new EventHandler(Workplace_ActiveWorkplaceWindowChanged);

            clipboardChangeNotifier = new ClipboardChangeNotifier();
            clipboardChangeNotifier.ClipboardChanged += new EventHandler(OnClipboardChanged);
            clipboardChangeNotifier.AssignHandle(Instance.Handle);
            clipboardChangeNotifier.Install();

            utbTasksActions.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(utbTasksActions_ToolClick);
            cmsTaskActions.ItemClicked += new ToolStripItemClickedEventHandler(cmsTaskActions_ItemClicked);

            base.Initialize();

            searchForm = new TasksSearch();
            taskPermissions = Workplace.GetTasksPermissions();
            if (taskPermissions == null)
                taskPermissions = GetNewTaskPermission();
            if (taskPermissions != null)
                usersModalForm = taskPermissions.IUserModalForm;

            returnedTasks.ParentScheme = Workplace.ActiveScheme;
            returnedTasks.TasksGrid = ugTasks;

            ReloadData();
            isFirstTask = true;

            CheckBoxOnHeader checkBoxOnHeader = new CheckBoxOnHeader(typeof(string), CheckState.Checked, ugTasks);
            ugTasks.CreationFilter = checkBoxOnHeader;
            if (ugTasks.ActiveRow != null)
            {
                ClearCurrentTask(taskViewObject.Key, taskViewObject);
                SetToolsState(false);
            }
            isFirstTask = false;
        }

	    internal IInplaceTasksPermissionsView GetNewTaskPermission()
	    {
            AdministrationUI.AdministrationUI newPermissions = 
                new AdministrationUI.AdministrationUI("E3FE8CF7-C803-41e1-A9FE-C595A1CF6A15");

	        newPermissions.Workplace = Workplace;
	        newPermissions.Initialize();
	        return newPermissions;
	    }

        /// <summary>
        /// удаляем все подчиненные задачи
        /// </summary>
        internal List<string> GetDeletedTasks()
        {
            List<int> deletedTasks = UltraGridHelper.GetChildsIDs(ugTasks.ActiveRow);
            deletedTasks.Add(Convert.ToInt32(ugTasks.ActiveRow.Cells["ID"].Value));

            List<string> deletedTasksKeys = new List<string>();
            foreach (int id in deletedTasks)
            {
                foreach(string key in taskViewObjects.Keys)
                {
                    if (Convert.ToInt32(key.Split('_')[1]) == id)
                    {
                        deletedTasksKeys.Add(key);
                    }
                }
            }
            return deletedTasksKeys;
        }

        /// <summary>
        /// удаляем родительскую задачу
        /// </summary>
        internal void DeleteTasks(List<string> taskKeys)
        {
            List<TasksViewObj> deletedTasks = new List<TasksViewObj>();
            foreach (string key in taskKeys)
            {
                deletedTasks.Add(taskViewObjects[key]);
                taskViewObjects.Remove(key);
            }
            foreach (TasksViewObj taskObj in deletedTasks)
            {
                taskObj.WorkplaceWindow.CloseWindow(false); 
            }
        }

        void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (e == null)
                return;
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                if (taskViewObjects.ContainsKey(key))
                {
                    int taskID = Convert.ToInt32(key.Split('_')[1]);
                    DataRow[] rows = ds.Tables[0].Select(string.Format("ID = {0}", taskID));
                    if (rows.Length == 0)
                    {
                        taskViewObjects[key].WorkplaceWindow.CloseWindow(false);
                        taskViewObjects.Remove(key);
                        return;
                    }
                    Workplace.SwitchTo("Задачи");

                    taskViewObject = taskViewObjects[key];
                    
                    UltraGridRow taskRow = returnedTasks.GetNavigationRowByID(taskID);
                    ugTasks.ActiveRow = taskRow;
                    //ugTasks.Selected.Rows.Clear();
                    //taskRow.Selected = true;
                }

            }
        }

        void Workplace_ViewClosed(object sender, Krista.FM.Client.Common.Gui.ViewContentEventArgs e)
        {
            if (taskViewObjects.ContainsKey(e.Content.Key))
            {
                // делаем запись неактивной
                ClearCurrentTask(e.Content.Key, taskViewObjects[e.Content.Key]);
                // делаем неактивными те кнопки на тулбаре, которые выполняют действия над задачами
                SetToolsState(false);
            }
        }

        private void SetToolsState(bool enableTool)
        {
            utbTasksActions.Tools["btnAddChild"].SharedProps.Enabled = enableTool;
            utbTasksActions.Tools["btnDelete"].SharedProps.Enabled = enableTool;
            utbTasksActions.Tools["btnDelete"].SharedProps.Enabled = enableTool;
            utbTasksActions.Tools["btnExport"].SharedProps.Enabled = enableTool;
            utbTasksActions.Tools["btnCutTasks"].SharedProps.Enabled = enableTool;
            utbTasksActions.Tools["btnCopyTasks"].SharedProps.Enabled = enableTool;
            utbTasksActions.Tools["btnChangeParams"].SharedProps.Enabled = enableTool;
            utbTasksActions.Tools["pmWordReports"].SharedProps.Enabled = enableTool;
        }


        private void ClearCurrentTask(string key, TasksViewObj currentRaskViewObject)
        {
            taskViewObjects.Remove(key);
            ugTasks.ActiveRow = null;
            if (currentRaskViewObject != null)
            {
                if (currentRaskViewObject.ActiveTask != null)
                {
                    if (!isFirstTask && currentRaskViewObject.ActiveTask.LockByCurrentUser())
                    {
                        currentRaskViewObject.SaveActiveTaskStateIntoDataBase(false);
                    }
                    ClearTaskData();
                    currentRaskViewObject.ActiveTask = null;
                }
                currentRaskViewObject.Dispose();
                activeTaskRow = null;
            }
        }

        void cmsTaskActions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ((ContextMenuStrip)sender).Hide();
            string itemName = e.ClickedItem.Name;
            switch (itemName)
            {
                case "expandTastTree":
                    if (ugTasks.ActiveRow != null)
                        ugTasks.ActiveRow.ExpandAll();
                    break;
                case "showAudit":
                    FormAudit.ShowAudit(Workplace, Caption, string.Empty,
                        UltraGridHelper.GetActiveID(ugTasks), AuditShowObjects.TaskObject);
                    break;
            }
        }

        void utbTasksActions_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                // добавить новую задачу верхнего уровня
                case "btnAddNew":
                    AddNewTask();
                    break;
                // добавить подчиненную задачу
                case "btnAddChild":
                    taskViewObject.AddNewTask(true);
                    break;
                // удалить активную задачу
                case "btnDelete":
                    DeleteActiveTask(true);
                    break;
                // обновить список задач
                case "btnRefresh":
                    ReloadData();
                    break;
                case "btnSearch":
                    int? taskID;
                    int? docID;
                    searchForm.PerformSearch(Workplace.ActiveScheme, usersModalForm, out taskID, out docID);
                    if (taskID != null)
                    {
                        // переход на найденную задачу
                        UltraGridRow taskRow = returnedTasks.GetNavigationRowByID(taskID);
                        ugTasks.ActiveRow = taskRow;
                        ugTasks.Selected.Rows.Clear();
                        taskRow.Selected = true;
                        taskViewObject.ClearTaskData();
                        taskViewObject.LoadTaskData();
                        // переход на найденный документ
                        if (docID != null)
                        {
                            taskViewObject.TasksView.utcPages.SelectedTab = taskViewObject.TasksView.utpDocuments.Tab;
                            UltraGridRow docRow = UltraGridHelper.FindGridRow(taskViewObject.TasksView.ugDocuments, "ID", docID.ToString());
                            docRow.Selected = true;
                        }
                    }
                    break;
                case "btnExport":
                    TasksExportHelper.ExportTasksNew(Workplace, ugTasks);
                    break;
                case "btnImport":
                    //if (TasksExportHelper.ImportTasks(Workplace, ugTasks))
                    TasksExportHelper.ImportTasks(Workplace, ugTasks);
                    ReloadData();
                    break;
                case "btnExportToMSProject":
                    MSProjectReportHelper.CreateTasksReport(Workplace, ugTasks);
                    break;
                case "btnFullWordReport":
                    MSWordReportHelper.CreateTaskReport(Workplace, ugTasks, MSWordReportHelper.MSWordReportsKind.Full);
                    break;
                case "btnLiteWordReport":
                    MSWordReportHelper.CreateTaskReport(Workplace, ugTasks, MSWordReportHelper.MSWordReportsKind.Lite);
                    break;
                case "btnCutTasks":
                    //taskViewObject.CutTasksIntoClipboard(ugTasks);
                    PutTasksIntoClipboard(ugTasks, true);
                    break;
                case "btnCopyTasks":
                    //taskViewObject.CopyTasksIntoClipboard(ugTasks);
                    PutTasksIntoClipboard(ugTasks, false);
                    break;
                case "btnPasteTasks":
                    //taskViewObject.InsertTasksFromClipboard(ugTasks);
                    InsertTasksFromClipboard(ugTasks);
                    ReloadData();
                    break;
                case "btnChangeParams":
                    taskViewObject.ChangeChildsTaskParams();
                    break;
                case "btnRenaming":
                    // переименование
                    bool isRenameAllTasks = false;
                    bool childTaskRename = false;
                    string renameFileName = string.Empty;
                    if (FormTasksRenaming.ShowTasksRenamingParams(Workplace.WindowHandle, ugTasks, ref isRenameAllTasks, ref childTaskRename, ref renameFileName))
                    {
                        // собственно само переименование
                        
                        DataSet dsRenamingReport = null;
                        bool hasError = false;
                        using (TasksDocumentsRenaming renaming = new TasksDocumentsRenaming(Workplace, ugTasks, isRenameAllTasks, childTaskRename, renameFileName))
                        {
                            using (IUsersOperationProtocol protocol = (IUsersOperationProtocol)Workplace.ActiveScheme.GetProtocol("Krista.FM.Client.ViewObjects.TasksUI.dll"))
                            {
                                string connectedState = string.Format("Запущен процесс переименования документов в задачах c ID ({0})",
                                    string.Join(", ", renaming.GetRenamingTablesID()));
                                UsersOperationEventKind kind = UsersOperationEventKind.uoeChangePermissionsTable;
                                protocol.WriteEventIntoUsersOperationProtocol(kind, connectedState, SystemInformation.ComputerName);
                            }
                            hasError = !renaming.RenameDocuments(out dsRenamingReport);
                            if (taskViewObject.TasksView.utcPages.ActiveTab.Key == "utpParameters")
                                taskViewObject.LoadParametersPage();
                        }                        

                        ReportForm repForm = new ReportForm();
                        repForm.FormCaption = "Результаты переименования";
                        repForm.DataGrid.DataSource = dsRenamingReport;
                        repForm.DataGrid.DisplayLayout.Bands[0].Columns[1].Width = 300;
                        repForm.DataGrid.DisplayLayout.Bands[1].Columns[0].Width = 600;
                        repForm.DataGrid.DisplayLayout.Bands[1].Columns[1].Hidden = true;
                        repForm.DataGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
                        repForm.ReportFileName = "Протокол переименования задач";
                        repForm.ShowDialog(Workplace.WindowHandle);                                               
                        }

                    break;
            }
        }

        #region дествия на тулбаре

        internal void AddNewTask()
        {
            UltraGridRow taskRow = null;
            try
            {
                TaskStub newTask = returnedTasks.AddNew(null);
                // если родительская запись не существует - добавить подчиненную нельзя
                taskRow = returnedTasks.GetNavigationRowByID(newTask.ID);
            }
            finally
            {
                if (taskRow != null)
                {
                    ugTasks.Selected.Rows.Clear();
                    ugTasks.ActiveRow = taskRow;
                    taskRow.Selected = true;
                    if (taskViewObject != null && taskViewObject.ActiveTask.LockByUser != -1)
                        taskViewObject.SaveActiveTaskStateIntoDataBase(false);
                }
            }
        }

        #endregion

        private bool showContMenu;
        private UIElement element;

        void ugTasks_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            showContMenu = e.Element is RowSelectorUIElement;
            if (showContMenu)
            {
                element = e.Element;
            }
        }

        void ugTasks_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && showContMenu)
            {
                Point point = ((UltraGrid)sender).PointToScreen(new Point(element.Rect.X, element.Rect.Bottom));
                cmsTaskActions.Show(point);
            }
        }

        void ugTasks_KeyDown(object sender, KeyEventArgs e)
        {
            if (taskViewObject == null)
                return;
            taskViewObject.GridsKeyDown(sender, e);
        }

        void ugTasks_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (e.Layout.Tag != null)
                return;
            e.Layout.Tag = "INITIALIZED";

            //e.Layout.LoadStyle = LoadStyle.PreloadRows;
            e.Layout.LoadStyle = LoadStyle.LoadOnDemand;
            e.Layout.Override.AllowAddNew = AllowAddNew.Yes;
            e.Layout.Override.AllowDelete = DefaultableBoolean.False;
            e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
            e.Layout.Scrollbars = Scrollbars.Both;
            e.Layout.ScrollStyle = ScrollStyle.Immediate;
            e.Layout.Override.RowSizing = RowSizing.AutoFixed;

            #region Первый банд
            UltraGridBand firstBand = e.Layout.Bands[0];

            firstBand.CardSettings.AllowLabelSizing = false;
            firstBand.CardSettings.AllowSizing = false;
            firstBand.CardSettings.AutoFit = false;
            firstBand.CardView = false;

            UltraGridColumn clmn = firstBand.Columns["ID"];
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.False;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 1;
            clmn.Width = 38;

            clmn = firstBand.Columns["HeadLine"];
            clmn.Header.Caption = "Наименование";
            clmn.Header.VisiblePosition = 2;
            clmn.Width = 146;

            clmn = firstBand.Columns["State"];
            clmn.Header.Caption = "Состояние";
            clmn.Header.VisiblePosition = 3;

            clmn = firstBand.Columns["FromDate"];
            clmn.Header.Caption = "Дата начала";
            clmn.Header.VisiblePosition = 4;
            clmn.Width = 89;

            clmn = firstBand.Columns["ToDate"];
            clmn.Header.Caption = "Дата окончания";
            clmn.Header.VisiblePosition = 5;
            clmn.Width = 93;

            clmn = firstBand.Columns["Owner"];
            clmn.Header.Caption = "Владелец";
            clmn.Hidden = true;

            clmn = firstBand.Columns["Doer"];
            clmn.Header.Caption = "Исполнитель";
            clmn.Width = 63;
            clmn.Hidden = true;

            clmn = firstBand.Columns["Curator"];
            clmn.Width = 63;
            clmn.Hidden = true;

            clmn = firstBand.Columns["LockByUser"];
            clmn.Width = 63;
            clmn.Hidden = true;

            clmn = firstBand.Columns["visible"];
            clmn.Hidden = true;

            clmn = firstBand.Columns["RefTasksTypes"];
            clmn.Hidden = true;

            if (firstBand.Columns.IndexOf("clmnStatePic") < 0)
            {
                clmn = firstBand.Columns.Add("clmnStatePic");
                clmn.AllowGroupBy = DefaultableBoolean.False;
                clmn.AllowRowFiltering = DefaultableBoolean.False;
                clmn.AllowRowSummaries = AllowRowSummaries.False;
                clmn.CellActivation = Activation.NoEdit;
                clmn.Header.Caption = "";
                clmn.Header.VisiblePosition = 0;
                clmn.LockedWidth = true;
                clmn.Width = 20;
            }

            clmn = firstBand.Columns["RefTasks"];
            clmn.Hidden = true;

            // OwnerName
            clmn = firstBand.Columns["OwnerName"];
            clmn.Header.Caption = "Владелец";
            clmn.Header.VisiblePosition = 7;

            // DoerName
            clmn = firstBand.Columns["DoerName"];
            clmn.Header.Caption = "Исполнитель";
            clmn.Header.VisiblePosition = 8;

            // CuratorName
            clmn = firstBand.Columns["CuratorName"];
            clmn.Header.Caption = "Куратор";
            clmn.Header.VisiblePosition = 9;

            // эти  надо спрятать
            firstBand.Columns["Job"].Hidden = true;
            firstBand.Columns["Description"].Hidden = true;
            firstBand.Columns["LockedUserName"].Hidden = true;
            firstBand.Columns["StateCode"].Hidden = true;

            firstBand.ColumnFilters["RefTasks"].FilterConditions.Clear();
            firstBand.ColumnFilters["RefTasks"].FilterConditions.Add(FilterComparisionOperator.Equals, null);
            firstBand.ColumnFilters["RefTasks"].FilterConditions.Add(FilterComparisionOperator.Equals, -1);
            firstBand.ColumnFilters["RefTasks"].LogicalOperator = FilterLogicalOperator.Or;

            #endregion

            #region Остальные банды инициализируем по первому, методом грязного хака
            for (int i = 1; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand gb = e.Layout.Bands[i];
                gb.GetType().InvokeMember("InitializeFrom",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, gb, new object[] { firstBand, PropertyCategories.All });
            }
            #endregion
        }

        /// <summary>
        /// получение статуса задачи в списке задач
        /// </summary>
        /// <param name="taskRow"></param>
        /// <returns></returns>
        private static TaskVisibleInNavigation GetTaskVisible(UltraGridRow taskRow)
        {
            TaskVisibleInNavigation vsbl = TaskVisibleInNavigation.tvVisible;
            taskRow = UltraGridHelper.GetRowCells(taskRow);
            try
            {
                vsbl = (TaskVisibleInNavigation)Convert.ToInt32(taskRow.Cells["visible"].Value);
            }
            catch { }
            return vsbl;
        }

        void ugTasks_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            TaskVisibleInNavigation vsbl = GetTaskVisible(e.Row);
            if (vsbl == TaskVisibleInNavigation.tvFantom)
            {
                e.Row.CellAppearance.ForeColor = SystemColors.ButtonFace;
            }
            // заблокированность задачи
            object val = e.Row.Cells["LockByUser"].Value;
            int lockedUser = -1;
            if (DBNull.Value != val)
                lockedUser = Convert.ToInt32(val);
            // устанавливаем нужную картинку
            UltraGridCell picCell = e.Row.Cells["clmnStatePic"];
            // по умолчанию - активна и свободна
            picCell.Appearance.Image = 4;
            if (lockedUser == -1)
            {
                // проверим не закрыта ли задача
                // состояние задачи
                TaskStates ts = (TaskStates)Convert.ToInt32(e.Row.Cells["StateCode"].Value);
                if (ts == TaskStates.tsClosed)
                {
                    picCell.Appearance.Image = 5;
                    picCell.ToolTipText = "Задача закрыта";
                }
                else
                {
                    picCell.Appearance.Image = 4;
                    picCell.ToolTipText = "Задача свободна";
                }
            }
            else
            {
                // заблокировано текущим пользователем
				if (lockedUser == ClientAuthentication.UserID)
                {
                    picCell.Appearance.Image = 7;
                    picCell.ToolTipText = "Задача заблокирована текущим пользователем";
                }
                // заблокировано кем-то другим
                else
                {
                    picCell.Appearance.Image = 6;
                    picCell.ToolTipText =
                        String.Format(
                        "Задача заблокирована пользователем '{0}'",
                        Convert.ToString(e.Row.Cells["LockedUserName"].Value)
                    );
                }
            }
        }

	    private UltraGridRow activeTaskRow = null;

        void ugTasks_BeforeRowDeactivate(object sender, CancelEventArgs e)
        {
            TaskVisibleInNavigation vsbl = GetTaskVisible(ugTasks.ActiveRow);
            if (vsbl == TaskVisibleInNavigation.tvFantom || vsbl == TaskVisibleInNavigation.tvInvsible)
                return;
            taskViewObject.TaskRowDeactivate();
            e.Cancel = !taskViewObject.CanDeactivate;
        }

        void ugTasks_AfterRowActivate(object sender, System.EventArgs e)
        {
            string taskKey = "task_" + ugTasks.ActiveRow.Cells["ID"].Value.ToString();

            TaskVisibleInNavigation vsbl = GetTaskVisible(ugTasks.ActiveRow);
            if (vsbl == TaskVisibleInNavigation.tvFantom)
            {
                // если переходим на задачу, которую нельзя смотреть, выдаем сообщение об этом
                PermissionException perm =
                    new PermissionException(Workplace.ActiveScheme.UsersManager.GetCurrentUserName(), string.Format("Задача(ID = {0})", ugTasks.ActiveRow.Cells["ID"].Value),
                        "Просмотр задачи", string.Empty);
                FormPermissionException.ShowErrorForm(perm);
                // переходим на ту задачу, которая была активна до этого
                ugTasks.ActiveRow = null;
                if (activeTaskRow != null)
                {
                    activeTaskRow.Activate();
                }
                return;
            }
            activeTaskRow = ugTasks.ActiveRow;

            if (!taskViewObjects.ContainsKey(taskKey))
            {
                taskViewObject = new TasksViewObj(taskKey);
                taskViewObject.Workplace = Workplace;
                taskViewObject.Initialize();
                taskViewObjects.Add(taskKey, taskViewObject);
            }
            else
            {
                taskViewObject = taskViewObjects[taskKey];
            }
            SetPermissions(true);
            taskViewObject.TaskRowActivete();
            OnActiveItemChanged(this, taskViewObject);
        }

        void ugTasks_BeforeRowActivate(object sender, RowEventArgs e)
        {
            TaskVisibleInNavigation vsbl = GetTaskVisible(e.Row);
            if (vsbl == TaskVisibleInNavigation.tvFantom)
            {
                if (activeTaskRow != null)
                {
                    //activeTaskRow.Activate();
                }
            }
        }

        private const DataViewRowState RowStateFilter =
            DataViewRowState.Added | DataViewRowState.CurrentRows |
            DataViewRowState.ModifiedCurrent | DataViewRowState.Unchanged;

        public void ClearTaskData()
        {
            taskViewObject.ClearTaskInfoPage();
            taskViewObject.ClearDocumentsPage();
            taskViewObject.ClearHistoryPage();
            taskViewObject.ClearParamsConstsPages();
            taskViewObject.ClearGroupsPermissionsPage();
            taskViewObject.ClearUsersPermissionsPage();
        }

        /// <summary>
        /// Перегрузка списка задач (в гриде навигации)
        /// </summary>
        public void ReloadData(bool saveActiveTaskState)
        {
            Workplace.OperationObj.Text = "Построение списка задач";
            Workplace.OperationObj.StartOperation();
            object curTaskID = null;
            try
            {
                // запоминаем текущую задачу
                if (taskViewObject != null)
                {
                    if (taskViewObject.ActiveTask != null)
                    {
                        curTaskID = taskViewObject.ActiveTask.ID;
                        if (saveActiveTaskState)
                            taskViewObject.ActiveTask = null;
                    }
                }

                ITaskManager taskManager = Workplace.ActiveScheme.TaskManager;
                UltraGrid ug = ugTasks;

                DataTable dt = taskManager.Tasks.GetTasksInfo();
                if (dt == null)
                    return;

                returnedTasks.TasksTable = dt;

                ds = new DataSet("Tasks");
                ds.BeginInit();

                ds.Tables.Add(dt);

                DataColumn clmnParent = dt.Columns[TasksNavigationTableColumns.ID_COLUMN];
                // для обратной совместимости
                if ((dt.PrimaryKey == null) || (dt.PrimaryKey.Length == 0))
                    dt.PrimaryKey = new DataColumn[1] { clmnParent };
                DataColumn clmnChild = dt.Columns[TasksNavigationTableColumns.REFTASKS_COLUMN];
                DataRelation dr = new DataRelation("ID_RefTasks_Relation", clmnParent, clmnChild, false);
                ds.Relations.Add(dr);

                ds.EndInit();
                ds.EnforceConstraints = false;

                DataViewManager dvManager = new DataViewManager(ds);
                foreach (DataViewSetting dvs in dvManager.DataViewSettings)
                    dvs.RowStateFilter = RowStateFilter;
                DataView dv = dvManager.CreateDataView(ds.Tables[0]);
                // ****
                // Внимание! Без вспомогательного объекта BindingSource
                // Грид аццки тормозит при работе с иерархическими данными
                // подробнее см.
                // http://devcenter.infragistics.com/Support/KnowledgeBaseArticle.aspx?ArticleID=9280
                // Как я понял, этот новый биндингсоурс должен заменить какой-то левый, который
                // создается по-умолчанию
                // ***
                BindingSource bs = new BindingSource();
                bs.DataSource = dv;
                ug.SyncWithCurrencyManager = true;
                // назначаем гриду Datasource
                ug.BeginUpdate();
                ug.DisplayLayout.MaxBandDepth = MAX_TASKS_HIERARCHY_LEVEL;
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AfterEvents, false);
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.BeforeEvents, false);
                // сбрасываем группировку, она не восстанавливается при перегрузке грида
                ug.DisplayLayout.ClearGroupByColumns();

                ug.DataSource = bs;//ds;

                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AfterEvents, true);
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.BeforeEvents, true);
                // есть ли данные по задачам
                bool dataPresent = dt.Rows.Count > 0;
                // устанавливаем доступность кнопок редактирования в соответствии с правами пользователя
                SetPermissions(dataPresent);


            }
            finally
            {
                ugTasks.EndUpdate(true);
                Workplace.OperationObj.StopOperation();
                // если была активная задача - устанавливаем фокус на нее
                if (curTaskID != null && ds.Tables[0].Select(string.Format("ID = {0}", curTaskID)).Length != 0)
                {
                    UltraGridRow row = returnedTasks.GetNavigationRowByID(curTaskID);
                    if (row != null)
                    {
                        ugTasks.ActiveRow = row;
                        row.Selected = true;
                    }
                }
                // если активной задачи нет (только загружено) - делаем активной первую запись
                else
                {
                    UltraGridHelper.SetActiveRowToFirst(ugTasks);
                }
            }
        }

        /// <summary>
        /// Перегрузка списка задач (в гриде навигации)
        /// </summary>
        public void ReloadData()
        {
            ReloadData(true);
        }

        private void SetPermissions(bool dataPresent)
        {
            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            // добавление
            AllowAddNewTask = um.CurrentUserCanCreateTasks();
            ToolBase tool = utbTasksActions.Tools["btnAddNew"];
            tool.SharedProps.Enabled = AllowAddNewTask;
            tool.SharedProps.Caption = "Добавить новую";
            if (!AllowAddNewTask)
                tool.SharedProps.Caption = tool.SharedProps.Caption + "<недостаточно прав>";
            tool = utbTasksActions.Tools["btnAddChild"];
            tool.SharedProps.Enabled = AllowAddNewTask && dataPresent;
            tool.SharedProps.Caption = "Добавить подчиненную";
            if (!AllowAddNewTask)
                tool.SharedProps.Caption = tool.SharedProps.Caption + "<недостаточно прав>";
            // экспорт в MSProject в соостветствии с его доступностью
            tool = utbTasksActions.Tools["btnExportToMSProject"];
            bool allowed = MSProjectReportHelper.MSProjectInstalled();
            tool.SharedProps.Caption = "Сохранить в MS Project 2003";
            tool.SharedProps.Enabled = allowed && dataPresent;
            if (!allowed)
                tool.SharedProps.Caption = tool.SharedProps.Caption + " <приложение не установлено>";
            // права на изменение параметров подчиненных задач
			AllowEditTask = Workplace.ActiveScheme.UsersManager.CheckAllTasksPermissionForTask(ClientAuthentication.UserID, AllTasksOperations.EditTaskAction);
            tool = utbTasksActions.Tools["btnChangeParams"];
            tool.SharedProps.Caption = "Установить параметры подчиненных задач";
            tool.SharedProps.Enabled = AllowEditTask && dataPresent;
            if (!AllowEditTask)
                tool.SharedProps.Caption = tool.SharedProps.Caption + "<недостаточно прав>";
            // права на переименование параметров и констант в задачах
            tool = utbTasksActions.Tools["btnRenaming"];
            tool.SharedProps.Enabled = AllowEditTask && dataPresent;
            tool.SharedProps.Caption = "Переименователь";
            if (!AllowEditTask)
                tool.SharedProps.Caption = tool.SharedProps.Caption + "<недостаточно прав>";
            
            // права на экспорт и импорт
			allowed = Workplace.ActiveScheme.UsersManager.CheckAllTasksPermissionForTask(ClientAuthentication.UserID, AllTasksOperations.ExportTask);
            tool = utbTasksActions.Tools["btnExport"];
            tool.SharedProps.Enabled = allowed && dataPresent;
            tool.SharedProps.Caption = "Сохранить в XML";
            if (!allowed)
                tool.SharedProps.Caption = tool.SharedProps.Caption + "<недостаточно прав>";

			allowed = Workplace.ActiveScheme.UsersManager.CheckAllTasksPermissionForTask(ClientAuthentication.UserID, AllTasksOperations.ImportTask);
            tool = utbTasksActions.Tools["btnImport"];
            tool.SharedProps.Enabled = allowed;
            tool.SharedProps.Caption = "Загрузить из XML";
            if (!allowed)
                tool.SharedProps.Caption = tool.SharedProps.Caption + "<недостаточно прав>";

            utbTasksActions.Tools["pmWordReports"].SharedProps.Enabled = dataPresent;
            // копирование в буфер
            utbTasksActions.Tools["btnCopyTasks"].SharedProps.Enabled = AllowAddNewTask && dataPresent;
            // копирование из буфера
            string data;
            tool = utbTasksActions.Tools["btnPasteTasks"];
            tool.SharedProps.Enabled = AllowAddNewTask &&
                TasksViewObj.TasksDataPresentInClipboard(out data) && dataPresent;
            //SetToolsState(dataPresent);
        }

		public Infragistics.Win.UltraWinToolbars.UltraToolbarsManager utbTasksActions;
		private Panel BaseNavigationCtrl_Fill_Panel;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Left;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Right;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Top;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom;
        private UltraDataSource udsTasks;
        internal UltraGrid ugTasks;
		private ImageList ilImages16;
        internal ContextMenuStrip cmsTaskActions;
        internal ToolStripMenuItem showAudit;
        private ToolStripMenuItem expandTastTree;
		private IContainer components;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}


		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TasksNavigation));
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("HeadLine");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("State");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FromDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ToDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Owner");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Doer");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Curator");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Year");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("clmnStatePic", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DoerName", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OwnerName", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CuratorName", 3);
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("HeadLine");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("State");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("FromDate");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ToDate");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Owner");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Doer");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Curator");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Year");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("tbTasksActions");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRefresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnAddNew");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnAddChild");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnDelete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCutTasks");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCopyTasks");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnPasteTasks");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnChangeParams");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRenaming");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Перемещение");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToUp");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToDown");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToLeft");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToRight");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar3 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("utbExport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSearch");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnImport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnExport");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmWordReports");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnAddNew");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnAddChild");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnDelete");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRefresh");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSearch");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnExport");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnImport");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnExportToMSProject");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToUp");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToDown");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToLeft");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnMoveToRight");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnLiteWordReport");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCutTasks");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCopyTasks");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnPasteTasks");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmWordReports");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnExportToMSProject");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnLiteWordReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnFullWordReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnFullWordReport");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnShowAudit");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnShowTaskStruct");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnChangeParams");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRenaming");
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            this.ilImages16 = new System.Windows.Forms.ImageList(this.components);
            this.BaseNavigationCtrl_Fill_Panel = new System.Windows.Forms.Panel();
            this.ugTasks = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.udsTasks = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.cmsTaskActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showAudit = new System.Windows.Forms.ToolStripMenuItem();
            this.expandTastTree = new System.Windows.Forms.ToolStripMenuItem();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utbTasksActions = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.BaseNavigationCtrl_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugTasks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udsTasks)).BeginInit();
            this.cmsTaskActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utbTasksActions)).BeginInit();
            this.SuspendLayout();
            // 
            // ilImages16
            // 
            this.ilImages16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages16.ImageStream")));
            this.ilImages16.TransparentColor = System.Drawing.Color.Fuchsia;
            this.ilImages16.Images.SetKeyName(0, "task_navigation_refresh.bmp");
            this.ilImages16.Images.SetKeyName(1, "task_navigation_newtask.bmp");
            this.ilImages16.Images.SetKeyName(2, "task_navigation_newchildtask.bmp");
            this.ilImages16.Images.SetKeyName(3, "task_navigation_deltask.bmp");
            this.ilImages16.Images.SetKeyName(4, "task_navigation_active.bmp");
            this.ilImages16.Images.SetKeyName(5, "task_navigation_disabled.bmp");
            this.ilImages16.Images.SetKeyName(6, "task_navigation_locked_by_another.bmp");
            this.ilImages16.Images.SetKeyName(7, "task_navigation_locked_by_me.bmp");
            this.ilImages16.Images.SetKeyName(8, "task_navigation_search.bmp");
            this.ilImages16.Images.SetKeyName(9, "task_export.bmp");
            this.ilImages16.Images.SetKeyName(10, "task_import.bmp");
            this.ilImages16.Images.SetKeyName(11, "task_ExportToMSProject.bmp");
            this.ilImages16.Images.SetKeyName(12, "task_navigation_up.bmp");
            this.ilImages16.Images.SetKeyName(13, "task_navigation_down.bmp");
            this.ilImages16.Images.SetKeyName(14, "task_navigation_left.bmp");
            this.ilImages16.Images.SetKeyName(15, "task_navigation_right.bmp");
            this.ilImages16.Images.SetKeyName(16, "Word.bmp");
            this.ilImages16.Images.SetKeyName(17, "Paste.bmp");
            this.ilImages16.Images.SetKeyName(18, "Cut.bmp");
            this.ilImages16.Images.SetKeyName(19, "Copy.bmp");
            this.ilImages16.Images.SetKeyName(20, "Book_StackOfReports.bmp");
            this.ilImages16.Images.SetKeyName(21, "Audit.bmp");
            this.ilImages16.Images.SetKeyName(22, "ExpandCurrentTask.bmp");
            this.ilImages16.Images.SetKeyName(23, "childTasksParams.bmp");
            this.ilImages16.Images.SetKeyName(24, "Renaming.bmp");
            // 
            // BaseNavigationCtrl_Fill_Panel
            // 
            this.BaseNavigationCtrl_Fill_Panel.Controls.Add(this.ugTasks);
            this.BaseNavigationCtrl_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseNavigationCtrl_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseNavigationCtrl_Fill_Panel.Location = new System.Drawing.Point(0, 81);
            this.BaseNavigationCtrl_Fill_Panel.Name = "BaseNavigationCtrl_Fill_Panel";
            this.BaseNavigationCtrl_Fill_Panel.Size = new System.Drawing.Size(884, 391);
            this.BaseNavigationCtrl_Fill_Panel.TabIndex = 0;
            // 
            // ugTasks
            // 
            this.ugTasks.AllowDrop = true;
            this.ugTasks.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugTasks.DataSource = this.udsTasks;
            ultraGridColumn1.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn1.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn1.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Width = 38;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn2.Header.Caption = "Наименование";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 146;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn3.Header.Caption = "Состояние";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn4.Header.Caption = "Дата начала";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
            ultraGridColumn4.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
            ultraGridColumn4.MaskInput = "{LOC}dd/mm/yyyy hh:mm:ss";
            ultraGridColumn4.Width = 96;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn5.Header.Caption = "Дата окончания";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
            ultraGridColumn5.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
            ultraGridColumn5.MaskInput = "{LOC}dd/mm/yyyy hh:mm:ss";
            ultraGridColumn5.Width = 95;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn6.Header.Caption = "Владелец";
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn7.Header.Caption = "Исполнитель";
            ultraGridColumn7.Header.VisiblePosition = 10;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn7.Width = 63;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn8.Header.VisiblePosition = 8;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn9.Header.Caption = "Год планирования";
            ultraGridColumn9.Header.VisiblePosition = 11;
            ultraGridColumn9.Width = 70;
            ultraGridColumn10.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn10.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn10.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn10.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn10.Header.Caption = "";
            ultraGridColumn10.Header.VisiblePosition = 0;
            ultraGridColumn10.LockedWidth = true;
            ultraGridColumn10.Width = 20;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn11.Header.Caption = "Исполнитель";
            ultraGridColumn11.Header.VisiblePosition = 9;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn12.Header.Caption = "Владелец";
            ultraGridColumn12.Header.VisiblePosition = 7;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn13.Header.Caption = "Куратор";
            ultraGridColumn13.Header.VisiblePosition = 12;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13});
            this.ugTasks.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ugTasks.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.ugTasks.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugTasks.ImageList = this.ilImages16;
            this.ugTasks.Location = new System.Drawing.Point(0, 0);
            this.ugTasks.Name = "ugTasks";
            this.ugTasks.Size = new System.Drawing.Size(884, 391);
            this.ugTasks.SyncWithCurrencyManager = false;
            this.ugTasks.TabIndex = 7;
            // 
            // udsTasks
            // 
            this.udsTasks.AllowDelete = false;
            ultraDataColumn1.DataType = typeof(int);
            ultraDataColumn4.DataType = typeof(System.DateTime);
            ultraDataColumn5.DataType = typeof(System.DateTime);
            ultraDataColumn6.DataType = typeof(int);
            ultraDataColumn7.DataType = typeof(int);
            ultraDataColumn8.DataType = typeof(int);
            ultraDataColumn9.DataType = typeof(int);
            this.udsTasks.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9});
            this.udsTasks.ReadOnly = true;
            // 
            // cmsTaskActions
            // 
            this.cmsTaskActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAudit,
            this.expandTastTree});
            this.cmsTaskActions.Name = "cmsAudit";
            this.cmsTaskActions.Size = new System.Drawing.Size(279, 48);
            // 
            // showAudit
            // 
            this.showAudit.Image = ((System.Drawing.Image)(resources.GetObject("showAudit.Image")));
            this.showAudit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showAudit.Name = "showAudit";
            this.showAudit.Size = new System.Drawing.Size(278, 22);
            this.showAudit.Text = "Аудит";
            // 
            // expandTastTree
            // 
            this.expandTastTree.Image = global::Krista.FM.Client.ViewObjects.TasksUI.Properties.Resources.ExpandCurrentTask;
            this.expandTastTree.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.expandTastTree.Name = "expandTastTree";
            this.expandTastTree.Size = new System.Drawing.Size(278, 22);
            this.expandTastTree.Text = "Развернуть дерево подчиненных задач";
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Left
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 81);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Left";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 391);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.ToolbarsManager = this.utbTasksActions;
            // 
            // utbTasksActions
            // 
            this.utbTasksActions.DesignerFlags = 1;
            this.utbTasksActions.DockWithinContainer = this;
            this.utbTasksActions.ImageListLarge = this.ilImages16;
            this.utbTasksActions.ImageListSmall = this.ilImages16;
            this.utbTasksActions.RightAlignedMenus = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.utbTasksActions.ShowFullMenusDelay = 500;
            this.utbTasksActions.ShowQuickCustomizeButton = false;
            this.utbTasksActions.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.VisualStudio2005;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool40});
            ultraToolbar1.Settings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.FillEntireRow = Infragistics.Win.DefaultableBoolean.True;
            ultraToolbar1.Text = "Действия";
            ultraToolbar2.DockedColumn = 0;
            ultraToolbar2.DockedRow = 2;
            ultraToolbar2.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12});
            ultraToolbar2.Settings.FillEntireRow = Infragistics.Win.DefaultableBoolean.True;
            ultraToolbar2.Text = "Перемещение";
            ultraToolbar2.Visible = false;
            ultraToolbar3.DockedColumn = 0;
            ultraToolbar3.DockedRow = 1;
            ultraToolbar3.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13,
            buttonTool14,
            buttonTool15,
            popupMenuTool1});
            ultraToolbar3.Settings.FillEntireRow = Infragistics.Win.DefaultableBoolean.True;
            ultraToolbar3.Text = "utbExport";
            this.utbTasksActions.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2,
            ultraToolbar3});
            this.utbTasksActions.ToolbarSettings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.ToolbarSettings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.ToolbarSettings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.ToolbarSettings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.ToolbarSettings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.ToolbarSettings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.ToolbarSettings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            this.utbTasksActions.ToolbarSettings.GrabHandleStyle = Infragistics.Win.UltraWinToolbars.GrabHandleStyle.None;
            appearance1.Image = "task_navigation_newtask.bmp";
            buttonTool16.SharedPropsInternal.AppearancesSmall.Appearance = appearance1;
            buttonTool16.SharedPropsInternal.Caption = "Добавить";
            appearance2.Image = "task_navigation_newchildtask.bmp";
            buttonTool17.SharedPropsInternal.AppearancesSmall.Appearance = appearance2;
            buttonTool17.SharedPropsInternal.Caption = "Добавить подчиненную";
            appearance3.Image = "task_navigation_deltask.bmp";
            buttonTool18.SharedPropsInternal.AppearancesSmall.Appearance = appearance3;
            buttonTool18.SharedPropsInternal.Caption = "Удалить";
            appearance4.Image = "task_navigation_refresh.bmp";
            buttonTool19.SharedPropsInternal.AppearancesSmall.Appearance = appearance4;
            buttonTool19.SharedPropsInternal.Caption = "Обновить";
            appearance5.Image = "task_navigation_search.bmp";
            buttonTool20.SharedPropsInternal.AppearancesSmall.Appearance = appearance5;
            buttonTool20.SharedPropsInternal.Caption = "Поиск";
            appearance6.Image = "task_export.bmp";
            buttonTool21.SharedPropsInternal.AppearancesSmall.Appearance = appearance6;
            buttonTool21.SharedPropsInternal.Caption = "Сохранить в XML";
            appearance7.Image = "task_import.bmp";
            buttonTool22.SharedPropsInternal.AppearancesSmall.Appearance = appearance7;
            buttonTool22.SharedPropsInternal.Caption = "Загрузить из XML";
            appearance8.Image = "task_ExportToMSProject.bmp";
            buttonTool23.SharedPropsInternal.AppearancesSmall.Appearance = appearance8;
            buttonTool23.SharedPropsInternal.Caption = "Сохранить в MS Project 2003";
            appearance9.Image = "task_navigation_up.bmp";
            buttonTool24.SharedPropsInternal.AppearancesSmall.Appearance = appearance9;
            buttonTool24.SharedPropsInternal.Caption = "Переместить вверх";
            appearance10.Image = "task_navigation_down.bmp";
            buttonTool25.SharedPropsInternal.AppearancesSmall.Appearance = appearance10;
            buttonTool25.SharedPropsInternal.Caption = "Переместить вниз";
            appearance11.Image = "task_navigation_left.bmp";
            buttonTool26.SharedPropsInternal.AppearancesSmall.Appearance = appearance11;
            buttonTool26.SharedPropsInternal.Caption = "Переместить влево";
            appearance12.Image = "task_navigation_right.bmp";
            buttonTool27.SharedPropsInternal.AppearancesSmall.Appearance = appearance12;
            buttonTool27.SharedPropsInternal.Caption = "Переместить вправо";
            appearance13.Image = "Word.bmp";
            buttonTool28.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool28.SharedPropsInternal.Caption = "Сохранить в MS Word. Список задач";
            appearance14.Image = "Cut.bmp";
            buttonTool29.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool29.SharedPropsInternal.Caption = "Вырезать";
            buttonTool29.SharedPropsInternal.ToolTipText = "Вырезать";
            appearance15.Image = "Copy.bmp";
            buttonTool30.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool30.SharedPropsInternal.Caption = "Копировать";
            buttonTool30.SharedPropsInternal.ToolTipText = "Копировать";
            appearance16.Image = "Paste.bmp";
            buttonTool31.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool31.SharedPropsInternal.Caption = "Вставить";
            buttonTool31.SharedPropsInternal.Enabled = false;
            buttonTool31.SharedPropsInternal.ToolTipText = "Вставить";
            appearance17.Image = 20;
            popupMenuTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            popupMenuTool2.SharedPropsInternal.Caption = "Oтчет";
            popupMenuTool2.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageOnlyOnToolbars;
            popupMenuTool2.SharedPropsInternal.ToolTipText = "Oтчет";
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool32,
            buttonTool33,
            buttonTool34});
            appearance18.Image = "Word.bmp";
            buttonTool35.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool35.SharedPropsInternal.Caption = "Сохранить в MS Word. Задачи с описаниями";
            appearance19.Image = 21;
            buttonTool36.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool36.SharedPropsInternal.Caption = "Аудит";
            appearance20.Image = "ExpandCurrentTask.bmp";
            buttonTool37.SharedPropsInternal.AppearancesSmall.Appearance = appearance20;
            buttonTool37.SharedPropsInternal.Caption = "Развернуть дерево подчиненных задач";
            appearance21.Image = "childTasksParams.bmp";
            buttonTool38.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool38.SharedPropsInternal.Caption = "Установить параметры подчиненных задач";
            appearance32.Image = "Renaming.bmp";
            buttonTool39.SharedPropsInternal.AppearancesSmall.Appearance = appearance32;
            buttonTool39.SharedPropsInternal.Caption = "Переименователь";
            this.utbTasksActions.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool16,
            buttonTool17,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22,
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28,
            buttonTool29,
            buttonTool30,
            buttonTool31,
            popupMenuTool2,
            buttonTool35,
            buttonTool36,
            buttonTool37,
            buttonTool38,
            buttonTool39});
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Right
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(884, 81);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Right";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 391);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ToolbarsManager = this.utbTasksActions;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Top
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Top";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(884, 81);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ToolbarsManager = this.utbTasksActions;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 472);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Bottom";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(884, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.utbTasksActions;
            // 
            // TasksNavigation
            // 
            this.Controls.Add(this.BaseNavigationCtrl_Fill_Panel);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom);
            this.Name = "TasksNavigation";
            this.Size = new System.Drawing.Size(884, 472);
            this.BaseNavigationCtrl_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugTasks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udsTasks)).EndInit();
            this.cmsTaskActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utbTasksActions)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		public override void Customize()
		{
			base.Customize();
			ComponentCustomizer.CustomizeInfragisticsComponents(components);
		}

        public override bool CanUnload
        {
            get
            {
                bool retValue = false;
                DataTable dt = Workplace.ActiveScheme.TaskManager.Tasks.GetCurrentUserLockedTasks();
                if ((dt == null) || (dt.Rows.Count == 0))
                {
                    Clipboard.SetData(CLPB_TASKS_OPERATION_COMPLETED, String.Empty);
                    Workplace.ActiveScheme.TaskManager.Tasks.DeleteTempFile();
                    Application.DoEvents();
                    return true;
                }
                    

                // добавляем столбец для галочки "Применить изменения"
                dt.BeginLoadData();
                dt.Columns.Add("ApplayChanges", typeof(bool));
                foreach (DataRow row in dt.Rows)
                {
                    row["ApplayChanges"] = false;
                }
                dt.EndLoadData();
                dt.AcceptChanges();

                FormLockedTasksAction.LockedTasksAction act = FormLockedTasksAction.SelectLockedTasksAction(ref dt);
                switch (act)
                {
                    case FormLockedTasksAction.LockedTasksAction.NoAction:
                        retValue = true;
                        break;
                    case FormLockedTasksAction.LockedTasksAction.ContinueWork:
                        retValue = false;
                        break;
                    case FormLockedTasksAction.LockedTasksAction.ApplayChanges:
                        // инициируем сохранение изменений текущей задачи
                        if (taskViewObject != null)
                            taskViewObject.ActiveTask = null;
                        // применяем измения для всех заблокированных
                        Workplace.OperationObj.Text = "Применение изменений для заблокированных задач";
                        Workplace.OperationObj.StartOperation();
                        try
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                bool applay = Convert.ToBoolean(row["ApplayChanges"]);
                                if (!applay)
                                    continue;
                                int id = Convert.ToInt32(row["ID"]);
                                ITask tmpTask = Workplace.ActiveScheme.TaskManager.Tasks[id];
                                try
                                {
                                    tmpTask.EndUpdate();
                                }
                                finally
                                {
                                    if (tmpTask != null)
                                        tmpTask.Dispose();
                                }
                            }
                        }
                        finally
                        {
                            Workplace.OperationObj.StopOperation();
                        }
                        retValue = true;
                        break;
                    default:
                        retValue = true;
                        break;
                }
                if (retValue)
                {
                    Clipboard.SetData(CLPB_TASKS_OPERATION_COMPLETED, String.Empty);
                    Application.DoEvents();
                    Workplace.ActiveScheme.TaskManager.Tasks.DeleteTempFile();
                }

                return retValue;
            }
        }

        /// <summary>
        /// Удалить активную задачу
        /// </summary>
        internal void DeleteActiveTask(bool inInteractiveMode)
        {
            if (taskViewObject.ActiveTask != null)
            {
                // спрашиваем подтверждение
                if ((inInteractiveMode) &&
                    (MessageBox.Show("Будут удалены текущая и все подчиненные задачи. Продолжить?",
                    "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes))
                {
                    taskViewObject.SetTaskActionCaption(String.Empty);
                    return;
                }
                // Перемещаемся на предыдущую строку грида
                List<string> deletedTasks = GetDeletedTasks();
                UltraGridRow previousRow = UltraGridHelper.GetRow(ugTasks, SiblingRow.Previous);

                if (returnedTasks.DeleteTask(taskViewObject.ActiveTask))
                {
                    DataRow delRow = ds.Tables[0].Rows.Find(taskViewObject.ActiveTask.ID);
                    delRow.Delete();
                    delRow.AcceptChanges();
                    //taskViewObject.ActiveTask = null;
                    DeleteTasks(deletedTasks);
                    // инициируем перегрузку полей задачи
                    if (previousRow != null)
                    {
                        ugTasks.ActiveRow = previousRow;
                        previousRow.Selected = true;
                    }
                    else
                    {
                        if (ugTasks.Rows.Count > 0)
                        {
                            ugTasks.ActiveRow = ugTasks.Rows[0];
                        }
                        else
                        {
                            taskViewObject.TasksView.utcPages.Visible = false;
                        }
                    }
                }
            }
        }

        #region Различные текстовые сообщения

        public static string COPY_TASKS_OPERATION = "Копирование задач в буфер обмена";
        public static string COPY_DOCUMENTS_OPERATION = "Копирование документов в буфер обмена";
        public static string PASTE_TASKS_OPERATION = "Вставка задач из буфера обмена";
        public static string PASTE_DOCUMЕNТS_OPERATION = "Вставка документов из буфера обмена";
        public static string DELETE_TASKS_OPERATION = "Удаление вырезанных задач";
        public static string DELETE_DOCUMENTS_OPERATION = "Удаление вырезанных документов";

        public static string ERROR_ON_TASKS_CUTTING = "Невозможно скопировать задачи";
        public static string ERROR_ON_DOCUMENTS_CUTTING = "Невозможно скопировать документы";

        #endregion

        #region копировать вставить

        private bool inClipboardOperation;

        private void PutTasksIntoClipboard(UltraGrid tasksGrid, bool markAsCutted)
        {
            //CheckChanges();
            ClipbpardHelper.ResetCuttedTasksIDs(tasksGrid);
            inClipboardOperation = true;

            List<int> selectedID;
            UltraGridHelper.GetSelectedIDs(tasksGrid, out selectedID);
            if (selectedID.Count == 0)
                return;

            Workplace.OperationObj.Text = COPY_TASKS_OPERATION;
            Workplace.OperationObj.StartOperation();

            try
            {
                // если нужно - помечаем строки грида как выделенные
                if (markAsCutted)
                {
                    if (!CanCutTasks(tasksGrid))
                    {
                        Workplace.OperationObj.StopOperation();
                        return;
                    }
                    // Если выбрали опцию вырезать, делать копию задачи не будем, просто в момент вставки меняем у задачи ссылку на родительскую
                    Clipboard.SetData(CLPB_CUT_TASKS_XML, string.Empty);
                    UltraGridHelper.SetRowsTransparent(tasksGrid, ClipbpardHelper.CuttedTasksIDs, true);
                }
                else
                {
                    int[] exportTaskList = GetTasksList(TaskExportType.teIncludeChild, selectedID, tasksGrid);
                    Clipboard.SetData(CLPB_COPY_TASKS_XML, string.Empty);
                    Workplace.ActiveScheme.TaskManager.Tasks.CopyTask(exportTaskList);
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        internal void InsertTasksFromClipboard(UltraGrid tasksGrid)
        {
            string data;
            if (!TasksDataPresentInClipboard(out data))
                return;

            List<string> formats = FormatsPresentInClipboard();
            bool isCopy = formats.Contains(CLPB_COPY_TASKS_XML);

            inClipboardOperation = true;

            TaskImportType importType;
            int? parentID;

            if (!isCopy && ClipbpardHelper.CuttedTasksIDs.Contains(UltraGridHelper.GetActiveID(tasksGrid)))
            {
                importType = TaskImportType.tiAsRootTasks;
                parentID = null;
            }
            else
                if (!TasksExportHelper.ResolveImportType(Workplace, tasksGrid, out importType, out parentID))
                    return;

            Workplace.OperationObj.Text = PASTE_TASKS_OPERATION;
            Workplace.OperationObj.StartOperation();
            try
            {
                // если это копирование, подставляем к имени всех задач "(копия)"
                if (isCopy)
                {
                    Workplace.ActiveScheme.TaskManager.Tasks.PasteTask(parentID);
                }
                else
                {
                    // вставка вырезанных таблиц осуществляется путем смены ссылки на родительскую задачу
                    foreach (int taskID in ClipbpardHelper.CuttedTasksIDs)
                    {
                        ITask task = returnedTasks[taskID];
                        task.SetParentTask(parentID);
                    }
                    // помещаем метку о том что операция копирования завершилась
                    Clipboard.SetData(CLPB_TASKS_OPERATION_COMPLETED, String.Empty);
                    Application.DoEvents();
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
                inClipboardOperation = false;
            }
        }

        private static List<string> FormatsPresentInClipboard()
        {
            List<string> result = new List<string>();
            IDataObject dto = Clipboard.GetDataObject();
            foreach (string fmt in dto.GetFormats())
            {
                result.Add(fmt);
            }
            return result;
        }

        public static bool TasksDataPresentInClipboard(out string data)
        {
            return DataPresentInClipboard(CLPB_COPY_TASKS_XML, out data) ||
                   DataPresentInClipboard(CLPB_CUT_TASKS_XML, out data);
        }

        private static bool DataPresentInClipboard(string formatName, out string data)
        {
            data = String.Empty;
            IDataObject dto = Clipboard.GetDataObject();
            foreach (string fmt in dto.GetFormats())
            {
                if (fmt == formatName)
                {
                    data = dto.GetData(formatName).ToString();
                    return true;
                }
            }
            return false;
        }

        private bool CanCutTasks(UltraGrid tasksGrid)
        {
            try
            {
                // формируем список выделенных ID
                if (CanCutTasks(tasksGrid, ds.Tables[0]))
                    return true;

                ClipbpardHelper.ResetCuttedTasksIDs(tasksGrid);

                ShowClipboardOperationError(true,
                                            "Одна из выделенных или подчиненных задач находится в состоянии редактирования. Операция 'Вырезать' не может быть выполнена",
                                            ERROR_ON_TASKS_CUTTING);
                return false;
            }
            catch (Exception e)
            {
                // по каким-то причинам одна из задач не может быть вырезана
                ShowClipboardOperationError(true, e.Message, ERROR_ON_TASKS_CUTTING);
                return false;
            }
        }

        private static int[] GetTasksList(TaskExportType taskExportType, List<int> selectedID, UltraGrid grid)
        {
            if (taskExportType == TaskExportType.teSelectedOnly)
                return selectedID.ToArray();
            return UltraGridHelper.GetSelectedAndChildsIDs(grid);
        }

        private void OnClipboardChanged(object sender, EventArgs e)
        {
            List<string> formats = FormatsPresentInClipboard();
            // задачи
            ToolBase tool = utbTasksActions.Tools["btnPasteTasks"];
            tool.SharedProps.Enabled =
                ((formats.Contains(CLPB_COPY_TASKS_XML)) ||
                (formats.Contains(CLPB_CUT_TASKS_XML)));
        }

        private void ShowClipboardOperationError(bool isTasks, string errorInfo, string caption)
        {
            ClipbpardHelper.ResetCuttedTasksIDs(ugTasks);
            inClipboardOperation = false;
            Workplace.OperationObj.StopOperation();
            if (isTasks)
                MessageBox.Show(errorInfo, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool CanCutTasks(UltraGrid grid, DataTable dtTasks)
        {
            List<int> selectedID;
            UltraGridHelper.GetSelectedIDs(grid, out selectedID);
            foreach (int taskId in selectedID)
            {
                DataRow taskRow = dtTasks.Select(string.Format("ID = {0}", taskId))[0];
                if (!taskRow.IsNull("LockByUser") && Convert.ToInt32(taskRow["LockByUser"]) != -1)
                    return false;

                if (CheckChildRowsCanCut(taskRow, dtTasks) == false)
                    return false;
                // помечаем как вырезанные задачи только те, которые выделили. Без подчиненных
                ClipbpardHelper.CuttedTasksIDs.Add(Convert.ToInt32(taskRow["ID"]));
            }
            return true;
        }

        private bool CheckChildRowsCanCut(DataRow parentRow, DataTable dtTasks)
        {
            foreach (DataRow taskRow in dtTasks.Select(string.Format("RefTasks = {0}", parentRow["ID"])))
            {
                if (!taskRow.IsNull("LockByUser") && Convert.ToInt32(taskRow["LockByUser"]) != -1)
                    return false;
                if (CheckChildRowsCanCut(taskRow, dtTasks) == false)
                    return false;
            }
            return true;
        }

        #endregion
    }
}

