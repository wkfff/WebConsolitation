using System;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Common;
using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.BaseViewObject;

using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    /// <summary>
    /// Класс для манипуляций с задачами
    /// </summary>
    public partial class TasksViewObj : BaseViewObj
    {
        #region Базовые методы предка
        public override Icon Icon
        {
            get { return Icon.FromHandle(Krista.FM.Client.ViewObjects.TasksUI.Properties.Resource.Task_16.GetHicon()); }
        }

        public override Image TypeImage16
        {
            get { return Krista.FM.Client.ViewObjects.TasksUI.Properties.Resource.Task_16; }
        }

        public override Image TypeImage24
        {
            get { return Krista.FM.Client.ViewObjects.TasksUI.Properties.Resource.Task_24; }
        }

        private TasksNavigation _tasksNavigation;

        private TasksView _tasksView;
        internal TasksView TasksView
        {
            get { return _tasksView; }
        }

        /// <summary>
        /// Создание объекта просмотра
        /// </summary>
        protected override void SetViewCtrl()
        {
            fViewCtrl = new TasksView();
            _tasksView = (TasksView)fViewCtrl;
        }

        /// <summary>
        /// Обработчик активации рабочего места (из воркплэйса)
        /// </summary>
        /// <param name="Activated">Активировано/Деактивировано</param>
        public override void Activate(bool Activated)
        {
            base.Activate(Activated);
            SetViewObjectCaption();
        }

        /*
        /// <summary>
        /// Вызыывается при закрытии воркплэйса пользователем.
        /// Проверяем есть ли заблокированные задачи
        /// </summary>
        public override bool CanUnload
        {
            get
            {
                DataTable dt = Workplace.ActiveScheme.TaskManager.Tasks.GetCurrentUserLockedTasks();
                if ((dt == null) || (dt.Rows.Count == 0))
                    return true;

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
                        Clipboard.SetData(CLPB_DOCUMENTS_OPERATION_COMPLETED, String.Empty);
                        return true;
                    case FormLockedTasksAction.LockedTasksAction.ContinueWork:
                        return false;
                    case FormLockedTasksAction.LockedTasksAction.ApplayChanges:
                        // инициируем сохранение изменений текущей задачи
                        ActiveTask = null;
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
                            Clipboard.SetData(CLPB_DOCUMENTS_OPERATION_COMPLETED, String.Empty);
                        }
                        finally
                        {
                            Workplace.OperationObj.StopOperation();
                        }
                        return true;
                    default:
                        return true;
                }
            }
        }
         * */
        #endregion

        #region Вспомогательные внутренние объекты
        private DocumentsHelper documentsHelper;
        private SOAPDimEditorHelper soapDimEditorHelper;
        private SOAPDimChooserHelper soapDimChooserHelper;
        
        private IInplaceTasksPermissionsView taskPermissions;
        internal IUsersModal usersModalForm;
        // отслеживатель состояния буфера обмена (прицеплять будем к объекту навигации)
        //private ClipboardChangeNotifier clipboardChangeNotifier = null;
        #endregion

        #region Конструктор
        public TasksViewObj(string taskKey)
            : base(taskKey)
        {
            Caption = "Задачи";
        }
        #endregion

        #region Перекрытые методы предка
        /// <summary>
        /// Инициализация внутренних объектов и свойств
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _tasksNavigation = TasksNavigation.Instance;
            _tasksNavigation.clipboardChangeNotifier.ClipboardChanged += new EventHandler(OnClipboardChanged);
            /*clipboardChangeNotifier = new ClipboardChangeNotifier();
            clipboardChangeNotifier.ClipboardChanged += new EventHandler(OnClipboardChanged);
            clipboardChangeNotifier.AssignHandle(TasksNavigation.Instance.Handle);
            clipboardChangeNotifier.Install();*/

            // объект для работы с документами
            documentsHelper = new DocumentsHelper();
            soapDimEditorHelper = new SOAPDimEditorHelper();
            soapDimChooserHelper = new SOAPDimChooserHelper();

            //форма поиска
            taskPermissions = Workplace.GetTasksPermissions();
            if (taskPermissions == null)
                taskPermissions = _tasksNavigation.GetNewTaskPermission();
            if (taskPermissions != null)
            {
                taskPermissions.AttachViewObject(_tasksView.utpcGroupsPermissions, _tasksView.utpcUsersPermissions);
                usersModalForm = taskPermissions.IUserModalForm;
            }
            // объект навигации
            _tasksView.btnCancel.Click += new EventHandler(btnCancelApply_Click);
            _tasksView.btnApply.Click += new EventHandler(btnCancelApply_Click);
            _tasksView.utcPages.VisibleChanged += new EventHandler(utcPages_VisibleChanged);
            _tasksView.utcPages.Visible = false;
            _tasksView.uddbDoAction.ClosedUp += new EventHandler(uddbDoAction_ClosedUp);
            _tasksView.uddbDoAction.DroppingDown += new CancelEventHandler(uddbDoAction_DroppingDown);
            _tasksView.utcPages.SelectedTabChanged += new SelectedTabChangedEventHandler(utcPages_SelectedTabChanged);
            _tasksView.utcPages.SelectedTabChanging += new SelectedTabChangingEventHandler(utcPages_SelectedTabChanging);
            _tasksView.utbDocuments.ToolClick += new ToolClickEventHandler(utbDocuments_ToolClick);
            _tasksView.ugDocuments.InitializeLayout += new InitializeLayoutEventHandler(ugDocuments_InitializeLayout);
            _tasksView.ugDocuments.ClickCellButton += new CellEventHandler(ugDocuments_ClickCellButton);
            _tasksView.ugDocuments.InitializeRow += new InitializeRowEventHandler(ugDocuments_InitializeRow);
            _tasksView.ugDocuments.AfterCellUpdate += new CellEventHandler(ugDocuments_AfterCellUpdate);
            _tasksView.ugDocuments.BeforeCellListDropDown += new CancelableCellEventHandler(ugDocuments_BeforeCellListDropDown);
            _tasksView.ugDocuments.CellListSelect += new CellEventHandler(ugDocuments_CellListSelect);
            _tasksView.ugDocuments.KeyDown += new KeyEventHandler(GridsKeyDown);
            _tasksView.ugDocuments.DragDrop += new DragEventHandler(ugDocuments_DragDrop);
            _tasksView.ugDocuments.DragEnter += new DragEventHandler(ugDocuments_DragEnter);


            _tasksView.ugeParams.OnGetGridColumnsState += new GetGridColumnsState(ugeParams_OnGetGridColumnsState);
            _tasksView.ugeParams._ugData.InitializeLayout += new InitializeLayoutEventHandler(ugeConstsParams_InitializeLayout);
            _tasksView.ugeParams._ugData.InitializeRow += new InitializeRowEventHandler(ugeConstsParams_InitializeRow);
            _tasksView.ugeParams._ugData.ClickCellButton += new CellEventHandler(ugeParams_ClickCellButton);

            _tasksView.ugeParams.OnRefreshData += new RefreshData(ugeConstsParams_OnRefreshData);
            _tasksView.ugeParams.OnSaveChanges += new SaveChanges(ugeConstsParams_OnSaveChanges);
            _tasksView.ugeParams.OnCancelChanges += new DataWorking(ugeConstsParams_OnCancelChanges);
            _tasksView.ugeParams.OnAfterRowInsert += new AfterRowInsert(ugeConstsParams_OnAfterRowInsert);
            _tasksView.ugeParams.utmMain.Toolbars["utbImportExport"].Visible = false;
            _tasksView.ugeParams.AllowImportFromXML = false;
            //_tasksView.ugeParams.AllowClearTable = false;
            _tasksView.ugeParams.SingleBandLevelName = "Добавить параметр";
            _tasksView.ugeParams.StateRowEnable = true;
            _tasksView.ugeParams.OnClearCurrentTable += new DataWorking(ugeConstsParams_OnClearCurrentTable);
            _tasksView.ugeParams._utmMain.Tools["ClearCurrentTable"].SharedProps.ToolTipText = "Удалить все парметры";

            _tasksView.ugeConsts.OnGetGridColumnsState += new GetGridColumnsState(ugeConsts_OnGetGridColumnsState);
            _tasksView.ugeConsts._ugData.InitializeLayout += new InitializeLayoutEventHandler(ugeConstsParams_InitializeLayout);
            _tasksView.ugeConsts._ugData.InitializeRow += new InitializeRowEventHandler(ugeConstsParams_InitializeRow);
            _tasksView.ugeConsts._ugData.ClickCellButton += new CellEventHandler(ugeParams_ClickCellButton);
            _tasksView.ugeConsts.utmMain.Toolbars["utbImportExport"].Visible = false;

            _tasksView.ugeConsts.OnRefreshData += new RefreshData(ugeConstsParams_OnRefreshData);
            _tasksView.ugeConsts.OnSaveChanges += new SaveChanges(ugeConstsParams_OnSaveChanges);
            _tasksView.ugeConsts.OnCancelChanges += new DataWorking(ugeConstsParams_OnCancelChanges);
            _tasksView.ugeConsts.OnAfterRowInsert += new AfterRowInsert(ugeConstsParams_OnAfterRowInsert);
            _tasksView.ugeConsts.AllowImportFromXML = false;
            //_tasksView.ugeConsts.AllowClearTable = false;
            _tasksView.ugeConsts.SingleBandLevelName = "Добавить константу";
            _tasksView.ugeConsts.OnClearCurrentTable += new DataWorking(ugeConstsParams_OnClearCurrentTable);
            _tasksView.ugeConsts._utmMain.Tools["ClearCurrentTable"].SharedProps.ToolTipText = "Удалить все  константы";
            _tasksView.ugeParams.StateRowEnable = true;


            _tasksView.btnSelectOwner.Click += new EventHandler(SelectUserBtnClick);
            _tasksView.btnSelectDoer.Click += new EventHandler(SelectUserBtnClick);
            _tasksView.btnSelectCurator.Click += new EventHandler(SelectUserBtnClick);
            _tasksView.btnSelectTaskType.Click += new EventHandler(SelectTaskTypeBtnClick);

            //returnedTasks.ParentScheme = Workplace.ActiveScheme;
            //returnedTasks.TasksGrid = _tasksNavigation.ugTasks;

            CheckBoxOnHeader checkBoxOnHeaderDocuments = new CheckBoxOnHeader(typeof(string), CheckState.Checked, _tasksView.ugDocuments);
            _tasksView.ugDocuments.CreationFilter = checkBoxOnHeaderDocuments;
        }

        #region меню с аудитом и развертыванием структуры задачи

        private bool showContMenu;
        private UIElement element;

        void ugTasks_MouseEnterElement(object sender, UIElementEventArgs e)
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
                _tasksNavigation.cmsTaskActions.Show(point);
            }
        }

        void cmsTaskActions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ((ContextMenuStrip)sender).Hide();
            string itemName = e.ClickedItem.Name;
            switch (itemName)
            {
                case "expandTastTree":
                    if (_tasksNavigation.ugTasks.ActiveRow != null)
                        _tasksNavigation.ugTasks.ActiveRow.ExpandAll();
                    break;
                case "showAudit":
                    FormAudit.ShowAudit(Workplace, Caption, string.Empty,
                        UltraGridHelper.GetActiveID(_tasksNavigation.ugTasks), AuditShowObjects.TaskObject);
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public override void InternalFinalize()
        {
            ActiveTask = null;

            ClearTaskData();

            //returnedTasks.Clear();
            soapDimEditorHelper.Dispose();
            soapDimChooserHelper.Dispose();

            //taskPermissions.Close();
            //searchForm.Dispose();

            //clipboardChangeNotifier.Dispose();

            base.InternalFinalize();
        }

        /// <summary>
        /// Сохранение изменений активной задачи при выходе из режима редактирований
        /// </summary>
        public override void SaveChanges()
        {
            base.SaveChanges();
            if (ActiveTask != null)
            {
                // обновляем элементы интерфейса
                RefreshActiveTaskFields();
                ActiveTask.RefreshNavigationRow();
                // проверяем диапазоны дат, оставляем возможность остаться в режиме редактирования
                if (!CheckTaskDates(ActiveTask.ID))
                    return;
                SaveActiveTaskStateIntoDataBase(true);
                // завершаем обновление информации о задаче в базе
                ActiveTask.EndUpdate();
                // сохраняем документы задачи
                // !!! Вроде как этот вызов лишний
                //SaveActiveTaskDocuments();
                SetTaskActionCaption(String.Empty);
                //SetTaskActionCaption(ActiveTask.CashedAction);
                SetTaskStateCaption(ActiveTask.State);
                // выставляем флаг необходимости перезагрузки страницы истории
                _historyPageLoaded = false;
                
                ActiveTask.RefreshNavigationRow();
            }
            // выключаем режим редактирования
            EnableTaskEditMode(false);
            // выставляем флаги необходимости перегрузки страниц
            ClearHistoryPage();
            ClearDocumentsPage();
            ClearGroupsPermissionsPage();
            ClearUsersPermissionsPage();
            // обновляем текст в заголовке
            ChangeViewObjectCaption();
        }

        private static string CANCEL_EDIT_PROMT_MSG =
            "Внимание!" + Environment.NewLine + Environment.NewLine +
            "Все изменения, сделанные пользователем {0}, будут потеряны." + Environment.NewLine + Environment.NewLine +
            "Продолжить?";

        /// <summary>
        /// Отмена изменений в активной задаче при выходе из режима редактирования
        /// </summary>
        public override void CancelChanges()
        {
            base.CancelChanges();
            if (ActiveTask != null)
            {
                //Проверяем - не отмена редактирования другого пользователя
				if (ActiveTask.LockByUser != ClientAuthentication.UserID)
                {
                    if (MessageBox.Show(
                        String.Format(CANCEL_EDIT_PROMT_MSG, Workplace.ActiveScheme.UsersManager.GetUserNameByID(ActiveTask.LockByUser)),
                        "Предупреждение",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        // если передумали отменять - ничего не делаем
                        return;
                    }
                }
                ActiveTask.CancelUpdate();
                // при отмене действий над задачей никаких действий с документом делать не будем
                // если задача новая или находится только в кэше - удаляем из дерева навигации
                if ((ActiveTask.IsNew) || (ActiveTask.PlacedInCacheOnly))
                {
                    _tasksNavigation.DeleteActiveTask(false);
                    return;
                    //DeleteTaskFromNavigation(ActiveTask.ID);
                }
            }
            // перегружаем информацию о задаче
            ClearTaskData();
            LoadTaskData();
            ActiveTask.RefreshNavigationRow();

            SetTaskActionCaption(String.Empty);
            EnableTaskEditMode(false);
        }

        #endregion

        #region Вспомогательные

        /// <summary>
        /// сохранение параметров и констант задачи при переходе на другую
        /// </summary>
        public void TaskRowDeactivate()
        {
            //if (_tasksView.utcPages.ActiveTab == null)
            //    return;
            /* при переходе между задачами сохранять константы и параметры не будем
            if (_tasksView.utcPages.ActiveTab.Key == "utpParameters")
            {
                ugeConstsParams_OnSaveChanges(_tasksView.ugeParams);
            }
            if (_tasksView.utcPages.ActiveTab.Key == "utpConsts")
            {
                ugeConstsParams_OnSaveChanges(_tasksView.ugeConsts);
            }
             * */
        }

        /// <summary>
        /// Инициализация (создание) записи в гриде навигации
        /// </summary>
        public void InitNavigationRow(/*TaskStub task*/)
        {
            if (!_tasksView.utcPages.Visible)
            {
                _tasksView.utcPages.EventManager.SetEnabled(UltraTabControlEventGroup.AllEvents, false);
                _tasksView.utcPages.Visible = true;
                _tasksView.utcPages.EventManager.SetEnabled(UltraTabControlEventGroup.AllEvents, true);
            }
        }

        /// <summary>
        /// Выйти из режима редактирования с запросом сохранить/отменить изменения
        /// </summary>
        public void CheckChanges()
        {
            if ((ActiveTask != null) && (ActiveTask.InEdit))
            {
                SaveActiveTaskStateIntoDataBase(false);
                LoadTaskData();
            }
        }

        internal void SaveActiveTaskDocuments(bool showProgress)
        {
            // таблица со сведениями о документах
            DataTable dt;
            // таблица с изменениями
            DataTable changes;
            // датаадаптер для обновлений
            IDataUpdater du = null;
            // обновляем информацию о документах из интерфейса (если они уже были загружены)
            if (_tasksView.ugDocuments.DataSource is DataTable)
            {
                UltraGrid ug = _tasksView.ugDocuments;
                ug.PerformAction(UltraGridAction.ExitEditMode);
                if ((ug.ActiveRow != null) && (ug.ActiveRow.DataChanged))
                    ug.ActiveRow.Update();

                dt = (DataTable)ug.DataSource;
                changes = dt.GetChanges();
                if ((changes != null) && (changes.Rows.Count > 0))
                {
                    try
                    {
                        du = ActiveTask.GetTaskDocumentsAdapter();
                        du.Update(ref changes);
                        dt.AcceptChanges();
                    }
                    finally
                    {
                        if (du != null)
                            du.Dispose();
                    }
                }
            }
            // теперь проверяем содержимое документов
            dt = new DataTable();
            try
            {
                du = ActiveTask.GetTaskDocumentsAdapter();
                du.Fill(ref dt);
                // если документы есть
                if (dt.Rows.Count > 0)
                {
                    // теперь проверяем содержимое документов и, если нужно, обновляем
                    if (showProgress)
                    {
                        Workplace.ProgressObj.Caption = "Сохранение документов задачи";
                        Workplace.ProgressObj.MaxProgress = dt.Rows.Count;
                        Workplace.ProgressObj.Position = 0;
                        Workplace.ProgressObj.StartProgress();
                    }
                    try
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            documentsHelper.CheckDocumentTypeAndCRC(Workplace.ProgressObj, ActiveTask, row);
                            if (showProgress)
                                Workplace.ProgressObj.Position++;
                        }
                    }
                    finally
                    {
                        if (showProgress)
                            Workplace.ProgressObj.StopProgress();
                    }
                    // проверяем были-ли изменения (некоторые документы могли сменить типы)
                    changes = dt.GetChanges();
                    // если таки да - обновляем значения в базе
                    if ((changes != null) && (changes.Rows.Count > 0))
                        du.Update(ref changes);
                }
            }
            finally
            {
                if (du != null)
                    du.Dispose();
            }
        }

        internal void SyncLocalDocumentsWithServer()
        {
            // таблица со сведениями о документах
            DataTable dt;
            // датаадаптер для обновлений
            IDataUpdater du = null;
            // теперь проверяем содержимое документов
            dt = new DataTable();
            try
            {
                du = ActiveTask.GetTaskDocumentsAdapter();
                du.Fill(ref dt);
            }
            finally
            {
                if (du != null)
                    du.Dispose();
            }

            // если документы есть
            if (dt.Rows.Count == 0)
                return;

            // теперь проверяем содержимое документов и, если нужно, обновляем
            Workplace.ProgressObj.Caption = "Обновление документов задачи";
            Workplace.ProgressObj.Text = string.Empty;
            Workplace.ProgressObj.MaxProgress = dt.Rows.Count;
            Workplace.ProgressObj.Position = 0;
            Workplace.ProgressObj.StartProgress();
            try
            {
                int taskID = ActiveTask.ID;
                foreach (DataRow row in dt.Rows)
                {
                    int documentID = Convert.ToInt32(row["ID"]);
                    string documentName = Convert.ToString(row["Name"]);
                    string sourceFileName = Convert.ToString(row["SourceFileName"]);
                    documentsHelper.CheckLocalTaskDocument(null, ActiveTask, taskID,
                        documentID, documentName, sourceFileName);
                    Workplace.ProgressObj.Position++;
                }
            }
            finally
            {
                Workplace.ProgressObj.StopProgress();
            }
        }

        internal void CancelTaskDocumentsChanges()
        {
            SyncLocalDocumentsWithServer();
        }

        private void RefreshActiveTaskFields()
        {
            if ((ActiveTask != null) && (ActiveTask.InEdit))
            {
                // обновляем поля задачи
                // всяких пользователей-кураторов надо обновлять в первую очередь, т.к. от них 
                // зависит возможность обновления полей
                int userID = (int)_tasksView.tbOwner.Tag;
                ActiveTask.Owner = userID;
                userID = (int)_tasksView.tbDoer.Tag;
                ActiveTask.Doer = userID;
                userID = (int)_tasksView.tbCurator.Tag;
                ActiveTask.Curator = userID;
                // теперь другие поля
                ActiveTask.FromDate = _tasksView.dtpFromDate.Value;
                ActiveTask.ToDate = _tasksView.dtpDateTo.Value;
                ActiveTask.Headline = _tasksView.tbDescription.Text.Replace(Environment.NewLine, " ");
                ActiveTask.Job = _tasksView.tbJob.Text;
                ActiveTask.Description = _tasksView.tbComment.Text;

                ActiveTask.RefTasksTypes = (int)_tasksView.tbTaskKind.Tag;
            }
        }

        public void SaveActiveTaskStateIntoDataBase(bool showPregress)
        {
            if (ActiveTask != null)
            {
                RefreshActiveTaskFields();
                ActiveTask.SaveStateIntoDatabase();
                SaveActiveTaskDocuments(showPregress);
                ActiveTask.RefreshNavigationRow();
                ugeConstsParams_OnSaveChanges(_tasksView.ugeConsts);
                ugeConstsParams_OnSaveChanges(_tasksView.ugeParams);
                // выставляем флаги необходимости перегрузки страниц
                ClearHistoryPage();
                ClearDocumentsPage();
                ClearGroupsPermissionsPage();
                ClearUsersPermissionsPage();
            }
        }

        private const string DATE_FORMAT = "dd.MM.yyyy";

        /// <summary>
        /// Проверка диапазонов дат задачи
        /// </summary>
        /// <param name="taskID">ID задачи</param>
        private bool CheckTaskDates(int taskID)
        {
            bool result = true;
            // поскольку ни на что не влияет будем полностью выполнять на клиенте
            //UltraGridRow row = UltraGridHelper.FindGridRow(_tasksNavigation.ugTasks, "ID", taskID.ToString());
            UltraGridRow row = _tasksNavigation.returnedTasks.GetNavigationRowByID(taskID);
            // если не нашли - выходим
            if (row == null)
                return result;
            StringBuilder sb = new StringBuilder();
            // Проверяем свой диапазон дат
            DateTime fromDate = Convert.ToDateTime(UltraGridHelper.GetRowCells(row).Cells["FromDate"].Value).Date;
            DateTime toDate = Convert.ToDateTime(UltraGridHelper.GetRowCells(row).Cells["ToDate"].Value).Date;
            if (fromDate > toDate)
            {
                sb.AppendLine(
                    String.Format(
                        "Неправильный диапазон дат: дата начала {0} больше даты окончания {1}",
                        fromDate.ToString(DATE_FORMAT),
                        toDate.ToString(DATE_FORMAT)
                    )
                );
                sb.AppendLine();
            }
            // Проверяем диапазон дат родительской задачи
            if (row.HasParent())
            {
                DateTime parentFromDate = Convert.ToDateTime(UltraGridHelper.GetRowCells(row.ParentRow).Cells["FromDate"].Value).Date;
                DateTime parentToDate = Convert.ToDateTime(UltraGridHelper.GetRowCells(row.ParentRow).Cells["ToDate"].Value).Date;
                if ((fromDate < parentFromDate) || (toDate > parentToDate))
                {
                    sb.AppendLine(
                        String.Format(
                            "Диапазон дат задачи ({0}..{1}) не входит в диапазон дат родительской задачи ID={2} ({3}..{4})",
                            fromDate.ToString(DATE_FORMAT), toDate.ToString(DATE_FORMAT),
                            UltraGridHelper.GetRowID(row.ParentRow),
                            parentFromDate.ToString(DATE_FORMAT), parentToDate.ToString(DATE_FORMAT)
                        )
                    );
                    sb.AppendLine();
                }
            }
            // Проверяем диапазон дат дочерних задач
            if (row.HasChild())
            {
                UltraGridRow childRow = row.GetChild(ChildRow.First);
                while (childRow != null)
                {
                    DateTime childFromDate = Convert.ToDateTime(UltraGridHelper.GetRowCells(childRow).Cells["FromDate"].Value).Date;
                    DateTime childToDate = Convert.ToDateTime(UltraGridHelper.GetRowCells(childRow).Cells["ToDate"].Value).Date;
                    int childID = UltraGridHelper.GetRowID(childRow);
                    if ((childFromDate < fromDate) || (childToDate > toDate))
                    {
                        sb.AppendLine(
                            String.Format(
                                "Диапазон дат дочерней задачи ID={0} ({1}..{2}) не входит в диапазон дат текущей ({3}..{4})",
                                childID,
                                childFromDate.ToString(DATE_FORMAT), childToDate.ToString(DATE_FORMAT),
                                fromDate.ToString(DATE_FORMAT), toDate.ToString(DATE_FORMAT)
                            )
                        );
                        sb.AppendLine();
                    }
                    if (childRow.HasNextSibling())
                        childRow = childRow.GetSibling(SiblingRow.Next);
                    else
                        childRow = null;
                }
            }
            // если найдены ошибки - показываем предупреждение
            if (sb.Length > 0)
            {
                sb.Insert(0, String.Format("При проверке диапазона дат текущей задачи (ID = {0}) обнаружены ошибки:{1}{2}", taskID, Environment.NewLine, Environment.NewLine));
                sb.Append(String.Empty);
                sb.Append("Применить изменения?");
                result = MessageBox.Show(sb.ToString(), "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }
            return result;
        }

        /// <summary>
        /// Установка надписи на объекте просмотра
        /// </summary>
        public void SetViewObjectCaption()
        {
            string tmpStr = String.Empty;
            if (ActiveTask != null)
            {
                tmpStr = "Задача{0}: {1} <{2}> {3}";
                if (ActiveTask.InEdit)
                    tmpStr = String.Format(tmpStr, '*', ActiveTask.ID, ActiveTask.State, ActiveTask.Headline);
                else
                    tmpStr = String.Format(tmpStr, String.Empty, ActiveTask.ID, ActiveTask.State, ActiveTask.Headline);
            }
            ViewCtrl.Text = tmpStr;
        }

        private void ChangeViewObjectCaption()
        {
            string tmpStr = String.Empty;
            if (ActiveTask != null)
            {
                tmpStr = "Задача{0}: {1} <{2}> {3}";
                if (ActiveTask.InEdit)
                    tmpStr = String.Format(tmpStr, '*', ActiveTask.ID, ActiveTask.State, ActiveTask.Headline);
                else
                    tmpStr = String.Format(tmpStr, String.Empty, ActiveTask.ID, ActiveTask.State, ActiveTask.Headline);
            }
            if (Workplace.ActiveContent is IViewContent)
            {
                if (((IViewContent)Workplace.ActiveContent).WorkplaceWindow != null)
                    ((IViewContent)Workplace.ActiveContent).WorkplaceWindow.Title = tmpStr;
            }
        }

        /// <summary>
        /// Установка названия состояния задачи
        /// </summary>
        /// <param name="caption">состояние</param>
        public void SetTaskStateCaption(string caption)
        {
            _tasksView.tbState.Text = string.Format("Текущее состояние: {0}", caption);
        }

        /// <summary>
        /// Установка названия действия над задачей
        /// </summary>
        /// <param name="caption">действие</param>
        public void SetTaskActionCaption(string caption)
        {
            if (caption == String.Empty)
                _tasksView.tbAction.Text = String.Empty;
            else
                _tasksView.tbAction.Text = string.Format("Текущее действие: {0}", caption);
        }

        /// <summary>
        /// Разрешение/запрещение режима редактирования для элементов пользовательского интерфейса
        /// </summary>
        /// <param name="enable">разрешить/запретить</param>
        internal void EnableTaskEditMode(bool enable)
        {
            _tasksView.SuspendLayout();
            try
            {
				int curUser = ClientAuthentication.UserID;
                // страница информации
                _tasksView.btnApply.Enabled = enable;
                // *** Дополнительные проверки

                bool isEditAction = string.Compare(ActiveTask.CashedAction, "Редактировать", true) == 0 ||
                    string.Compare(ActiveTask.CashedAction, "Назначить", true) == 0;
                bool curUserIsOwner = (curUser == ActiveTask.Owner);
                if (enable)
                {
                    _tasksView.btnCancel.Enabled = true;
                    // эти параметры можно менять только при выполнении действия "редактировать"
                    // и если текущий пользователь является владельцем
                    _tasksView.tbDescription.Enabled = curUserIsOwner || isEditAction;
                    _tasksView.tbJob.Enabled = curUserIsOwner || isEditAction;
                    _tasksView.dtpFromDate.Enabled = curUserIsOwner || isEditAction;
                    _tasksView.dtpDateTo.Enabled = curUserIsOwner || isEditAction;

                    // для кэшированных задач пишем надпись действия
                    SetTaskActionCaption(ActiveTask.CashedAction);
                    // изменить можно только при выполнении системного действия "Редактировать"
                    // на сервере проверку делать не будем - слишком уж мутное условие
                    _tasksView.btnSelectOwner.Enabled = isEditAction && taskPermissions != null;
                    // эти три параметра может менять только владелец задачи
                    _tasksView.btnSelectDoer.Enabled = isEditAction && taskPermissions != null;
                    _tasksView.btnSelectCurator.Enabled = isEditAction && taskPermissions != null;
                    _tasksView.btnSelectTaskType.Enabled = isEditAction && taskPermissions != null;
                }
                else
                {
                    // ***
                    // если задача заблокирована другим пользователем, то пользователь,
                    // обладающий нужными правами может отменить редактирование
                    int lockedUser = ActiveTask.LockByUser;
                    if (((lockedUser != -1) && (lockedUser != curUser)) &&
                        (Workplace.ActiveScheme.UsersManager.CheckPermissionForTask(
                            ActiveTask.ID, ActiveTask.RefTasksTypes,
                            (int)TaskTypeOperations.CanCancelEdit, false)))
                    {
                        _tasksView.btnCancel.Enabled = true;
                    }
                    else
                    {
                        _tasksView.btnCancel.Enabled = false;
                    }
                    // ***
                    _tasksView.tbDescription.Enabled = false;
                    _tasksView.tbJob.Enabled = false;
                    _tasksView.btnSelectOwner.Enabled = false;
                    _tasksView.btnSelectDoer.Enabled = false;
                    _tasksView.btnSelectCurator.Enabled = false;
                    _tasksView.btnSelectTaskType.Enabled = false;
                    _tasksView.dtpFromDate.Enabled = false;
                    _tasksView.dtpDateTo.Enabled = false;
                }

                _tasksView.tbComment.Enabled = enable;

                // *** Параметры
                _tasksView.ugeParams.AllowAddNewRecords = enable;
                _tasksView.ugeParams.AllowEditRows = enable;
                _tasksView.ugeParams.AllowDeleteRows = enable;
                _tasksView.ugeParams.AllowClearTable = enable;

                _tasksView.ugeConsts.AllowAddNewRecords = enable;
                _tasksView.ugeConsts.AllowEditRows = enable;
                _tasksView.ugeConsts.AllowDeleteRows = enable;
                _tasksView.ugeConsts.AllowClearTable = enable;
                // ***

                //_tasksView.btnApply.Enabled	= enable;
                //_tasksView.btnCancel.Enabled = enable;
                _tasksView.uddbDoAction.Enabled = !enable;

                _tasksView.tbDoer.Enabled = enable;
                _tasksView.tbOwner.Enabled = enable;
                _tasksView.tbCurator.Enabled = enable;
                _tasksView.tbTaskKind.Enabled = enable;
                // страница документов
                // добавить документ
                _tasksView.utbDocuments.Tools["pmAddDocument"].SharedProps.Enabled = enable;
                // удалить документ
                _tasksView.utbDocuments.Tools["btnDelDocument"].SharedProps.Enabled = enable;
                // обновить все листы
                _tasksView.utbDocuments.Tools["pmRefresh"].SharedProps.Enabled = enable;
                // передать данные всех листов на сервер
                _tasksView.utbDocuments.Tools["btnWriteBackAll"].SharedProps.Enabled = enable;
                // вырезать документы
                _tasksView.utbDocuments.Tools["btnCutDocuments"].SharedProps.Enabled = enable;
                // копировать документы (доступна всегда)
                //_tasksView.utbDocuments.Tools["0"].SharedProps.Enabled = true;
                // вставить документы
                string data;
                _tasksView.utbDocuments.Tools["btnPasteDocuments"].SharedProps.Enabled = enable && DocumentsDataPresentInClipboard(out data);

                Activation act = Activation.ActivateOnly;
                if (enable) act = Activation.AllowEdit;

                UltraGridColumn clmn = _tasksView.ugDocuments.DisplayLayout.Bands[0].Columns["Name"];
                clmn.CellActivation = act;
                clmn = _tasksView.ugDocuments.DisplayLayout.Bands[0].Columns["Description"];
                clmn.CellActivation = act;
                clmn = _tasksView.ugDocuments.DisplayLayout.Bands[0].Columns["clmnOwnershipName"];
                clmn.CellActivation = act;

                // если задача не в режиме редактирования - проверяем ее заблокированность
                if (!enable)
                {
                    _tasksView.uddbDoAction.Enabled = (ActiveTask.LockByUser == -1);
                }

                // обновляем заголовок объекта просмотра
                SetViewObjectCaption();
            }
            finally
            {
                _tasksView.ResumeLayout();
            }
        }

        #endregion

        #region Загрузка данных в разные страницы объекта просмотра
        //признак загруженности страницы с информацией о задаче
        //private bool _taskInfoPageLoaded = false;

        /// <summary>
        /// Очистить данные объектов страницы информации
        /// </summary>
        public void ClearTaskInfoPage()
        {
            //_taskInfoPageLoaded = false;
        }

        /// <summary>
        /// Загрузить страницу с информацией о задаче
        /// </summary>
        public void LoadTaskInfoPage()
        {
            if (ActiveTask == null)
                return;

            _tasksView.utpTaskInfo.SuspendLayout();
            try
            {
                SetTaskStateCaption(ActiveTask.State);
                SetTaskActionCaption(ActiveTask.CashedAction);
                try
                {
                    _tasksView.dtpFromDate.Value = ActiveTask.FromDate;
                }
                catch { }
                try
                {
                    _tasksView.dtpDateTo.Value = ActiveTask.ToDate;
                }
                catch { }
                _tasksView.tbOwner.Text = ActiveTask.OwnerName;
                _tasksView.tbOwner.Tag = ActiveTask.Owner;

                _tasksView.tbDoer.Text = ActiveTask.DoerName;
                _tasksView.tbDoer.Tag = ActiveTask.Doer;

                _tasksView.tbCurator.Text = ActiveTask.CuratorName;
                _tasksView.tbCurator.Tag = ActiveTask.Curator;

                IUsersManager um = Workplace.ActiveScheme.UsersManager;
                _tasksView.tbTaskKind.Text = um.GetNameFromDirectoryByID(DirectoryKind.dkTasksTypes, ActiveTask.RefTasksTypes);
                _tasksView.tbTaskKind.Tag = ActiveTask.RefTasksTypes;

                _tasksView.tbDescription.Text = ActiveTask.Headline;
                _tasksView.tbJob.Text = ActiveTask.Job;
                _tasksView.tbComment.Text = ActiveTask.Description;
            }
            finally
            {
                _tasksView.utpTaskInfo.ResumeLayout();
                //				_taskInfoPageLoaded = true;
            }
        }

        // признак загруженности страницы документов
        private bool _documentsPageLoaded = false;

        /// <summary>
        /// Очистить данные объектов страницы документов
        /// </summary>
        public void ClearDocumentsPage()
        {
            _documentsPageLoaded = false;
            if (_tasksView.ugDocuments.DataSource is DataTable)
                ((DataTable)_tasksView.ugDocuments.DataSource).Rows.Clear();
            //_tasksView.udsDocuments.Rows.Clear();
        }

        /// <summary>
        /// Загрузить страницу документов
        /// </summary>
        public void LoadDocumentsPage()
        {
            if (ActiveTask == null)
                return;
            IDataUpdater du = null;
            try
            {
                du = ActiveTask.GetTaskDocumentsAdapter();
                DataTable dt = new DataTable("Documents");
                du.Fill(ref dt);

                if (dt == null) return;
                dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };

                _tasksView.ugDocuments.DataSource = null;
                _tasksView.ugDocuments.ResetLayouts();
                _tasksView.ugDocuments.SyncWithCurrencyManager = false;
                _tasksView.ugDocuments.DisplayLayout.MaxBandDepth = 1;
                _tasksView.ugDocuments.DataSource = dt;
            }
            finally
            {
                if (du != null)
                    du.Dispose();
            }

            _documentsPageLoaded = true;
        }

        // признак загруженности страницы истории
        private bool _historyPageLoaded = false;

        /// <summary>
        /// Очистить данные объектов страницы истории
        /// </summary>
        public void ClearHistoryPage()
        {
            //_tasksView.udsHistory.Rows.Clear();
            if (_tasksView.ugHistory.DataSource != null)
                _tasksView.ugHistory.DataSource = null;
            _historyPageLoaded = false;
        }

        /// <summary>
        /// Загрузить страницу истории
        /// </summary>
        public void LoadHistoryPage()
        {
            if (ActiveTask == null) return;
            ClearHistoryPage();
            DataTable dt = ActiveTask.GetTaskHistory();

            _tasksView.ugHistory.DataSource = null;
            _tasksView.ugHistory.DataSource = dt;
            _historyPageLoaded = true;
        }

        private bool _groupsPermissionsLoaded = false;

        public void ClearGroupsPermissionsPage()
        {
            if (taskPermissions != null)
            {
                _groupsPermissionsLoaded = false;
                if (ActiveTask == null) return;
                taskPermissions.ClearAttachedData(false);
            }
        }

        public void LoadGroupsPermissionsPage()
        {
            if (ActiveTask == null) return;
            if (taskPermissions != null)
            {
                taskPermissions.RefreshAttachedData(ActiveTask.ID, SysObjectsTypes.Task, false);
                _groupsPermissionsLoaded = true;
            }
        }

        private bool _usersPermissionsLoaded = false;

        public void ClearUsersPermissionsPage()
        {
            if (taskPermissions != null)
            {
                _usersPermissionsLoaded = false;
                if (ActiveTask == null) return;
                taskPermissions.ClearAttachedData(true);
            }
        }

        public void LoadUsersPermissionsPage()
        {
            if (ActiveTask == null) return;
            if (taskPermissions == null)
                return;
            taskPermissions.RefreshAttachedData(ActiveTask.ID, SysObjectsTypes.Task, true);
            _usersPermissionsLoaded = true;
        }

        public void LoadParametersPage()
        {
            if (ActiveTask == null)
                return;
            ugeConstsParams_OnRefreshData(_tasksView.ugeParams);
        }

        public void LoadConstsPage()
        {
            if (ActiveTask == null)
                return;
            ugeConstsParams_OnRefreshData(_tasksView.ugeConsts);
        }

        public void ClearParamsConstsPages()
        {
            _tasksView.ugeParams.DataSource = null;
            _tasksView.ugeConsts.DataSource = null;
        }


        /// <summary>
        /// Основной метод очистки данных по задаче
        /// </summary>
        public void ClearTaskData()
        {
            ClearTaskInfoPage();
            ClearDocumentsPage();
            ClearHistoryPage();
            ClearParamsConstsPages();
            ClearGroupsPermissionsPage();
            ClearUsersPermissionsPage();
        }

        /// <summary>
        /// Основной метод загрузки данных задачи
        /// </summary>
        public void LoadTaskData()
        {
            if (_tasksView.utcPages.SelectedTab == null)
                _tasksView.utcPages.SelectedTab = _tasksView.utcPages.Tabs[0];
            // информацию о задаче перегружаем всегда
            LoadTaskInfoPage();
            switch (_tasksView.utcPages.SelectedTab.Key)
            {
                case "utpDocuments":
                    if (!_documentsPageLoaded)
                        LoadDocumentsPage();
                    break;
                case "utpHistory":
                    if (!_historyPageLoaded)
                        LoadHistoryPage();
                    break;
                case "utpGroupsPermissions":
                    if (!_groupsPermissionsLoaded)
                        LoadGroupsPermissionsPage();
                    break;
                case "utpUsersPermissions":
                    if (!_usersPermissionsLoaded)
                        LoadUsersPermissionsPage();
                    break;
                case "utpParameters":
                    LoadParametersPage();
                    break;
                case "utpConsts":
                    LoadConstsPage();
                    break;
            }
        }
        #endregion

        #region Активная задача и список загруженных задач
        //internal TaskStubManager returnedTasks = new TaskStubManager();

        private TaskStub _activeTask = null;

        public TaskStub ActiveTask
        {
            get { return _activeTask; }
            set
            {
                if (_activeTask != null)
                {
                    CheckChanges();
                }
                _activeTask = value;
            }
        }

        /// <summary>
        /// Метод для получения активной задачи
        /// </summary>
        /// <returns></returns>
        public TaskStub GetActiveTask()
        {
            CellsCollection cells = UltraGridHelper.GetActiveRowCells(_tasksNavigation.ugTasks).Cells;
            int taskID = Convert.ToInt32(cells["ID"].Value);
            return _tasksNavigation.returnedTasks[taskID];
        }

        #endregion

        #region ugTasks и методы работы с ним

        public void TaskRowActivete()
        {
            if (ActiveTask == null)
                ActiveTask = GetActiveTask();

            if (ActiveTask == null)
            {
                // пользователь не обрадает правами на просмотр задачи
                _tasksView.laContainerInfo.Text = "Недостаточно прав для просмотра задачи";
                _tasksView.utcPages.Visible = false;
            }
            else
            {
                // проверяем может ли пользователь удалять задачу
                bool allowed = Workplace.ActiveScheme.UsersManager.CheckPermissionForTask(ActiveTask.ID, ActiveTask.RefTasksTypes, (int)TaskTypeOperations.DelTaskAction, false);
                ToolBase tool = _tasksNavigation.utbTasksActions.Tools["btnDelete"];

                tool.SharedProps.Enabled = allowed;
                tool.SharedProps.Caption = "Удалить";
                if (!allowed)
                    tool.SharedProps.Caption = tool.SharedProps.Caption + "<недостаточно прав>";

                tool = _tasksNavigation.utbTasksActions.Tools["btnCutTasks"];
                tool.SharedProps.ToolTipText = "Вырезать";
                tool.SharedProps.Enabled = allowed;
                if (!allowed)
                    tool.SharedProps.ToolTipText = tool.SharedProps.Caption + "<недостаточно прав>";

                // проверяем может ли пользователь назначать права на задачу
                bool allowAssignPermission = Workplace.ActiveScheme.UsersManager.CheckPermissionForTask(ActiveTask.ID, ActiveTask.RefTasksTypes, (int)TaskTypeOperations.AssignTaskViewPermission, false);
                // в зависимости от этого прячем/показываем страницы с правами
                _tasksView.utcPages.Tabs["utpGroupsPermissions"].Visible = allowAssignPermission && taskPermissions != null;
                _tasksView.utcPages.Tabs["utpUsersPermissions"].Visible = allowAssignPermission && taskPermissions != null;
                SetEnableChildTaskParamsBtn(!ActiveTask.InEdit);
                LoadTaskData();
                _tasksView.utcPages.Visible = true;
                EnableTaskEditMode(ActiveTask.InEdit);
            }
        }

        #endregion

        #region Обработчики

        internal void AddNewTask(bool asChild)
        {
            UltraGridRow taskRow = null;
            int? parentTaskID = null;
            if (asChild)
            {
                if (ActiveTask != null)
                    parentTaskID = ActiveTask.ID;
                else
                {
                    MessageBox.Show("Родительская задача не задана", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // максимально доступный уровень для добавления задачи 6
            if (asChild && _tasksNavigation.ugTasks.ActiveRow != null &&
                _tasksNavigation.ugTasks.ActiveRow.Band.Index == TasksNavigation.MAX_TASKS_HIERARCHY_LEVEL - 1)
            {
                MessageBox.Show("Родительская задача имеет максимальный уровень вложенности", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                TaskStub newTask = _tasksNavigation.returnedTasks.AddNew(parentTaskID);
                // если родительская запись не существует - добавить подчиненную нельзя
                if ((asChild) && (newTask == null))
                {
                    MessageBox.Show("Невозможно создать подчиненную задачу." + Environment.NewLine + "Возможно родительская задача не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                taskRow = _tasksNavigation.returnedTasks.GetNavigationRowByID(newTask.ID);
            }
            finally
            {
                if (taskRow != null)
                {
                    _tasksNavigation.ugTasks.Selected.Rows.Clear();
                    _tasksNavigation.ugTasks.ActiveRow = taskRow;
                    taskRow.Selected = true;
                    if (_activeTask.LockByUser != -1)
                        SaveActiveTaskStateIntoDataBase(false);
                }
            }

        }

        /// <summary>
        /// меняет параметры в подчиненных задачах, устанавливая их как у родительской
        /// </summary>
        internal void ChangeChildsTaskParams()
        {
            IUsersManager um = Workplace.ActiveScheme.UsersManager;
            TaskParams taskParams = new TaskParams();

            DataTable dtUsers = um.GetUsersPermissionsForObject(ActiveTask.ID, 18000);
            DataTable dtGroups = um.GetGroupsPermissionsForObject(ActiveTask.ID, 18000);

            if (FormChildTaskParams.ShowChildTaskParamsForm(taskParams))
            {
                List<int> childRowIDs = UltraGridHelper.GetChildsIDs(_tasksNavigation.ugTasks.ActiveRow);
                DataTable lockTasks = null;
                int closedTaskCount = 0;
                int lockTasksCount = 0;
                this.Workplace.OperationObj.Text = "Обработка данных";
                this.Workplace.OperationObj.StartOperation();
                try
                {
                    foreach (int id in childRowIDs)
                    {
                        TaskStub task = _tasksNavigation.returnedTasks[id];
                        // если задача заблокирована, пропускаем ее
                        if (task.LockByUser != -1)
                        {
                            if (lockTasks == null)
                            {
                                lockTasks = new DataTable();
                                lockTasks.Columns.Add("ID", typeof(int));
                                lockTasks.Columns.Add("Name", typeof(string));
                                lockTasks.Columns.Add("User", typeof(string));
                            }
                            DataRow row = lockTasks.NewRow(); //(string.Format("ID: {0}, наименование: {1}, пользователь: {2};", id, task.Headline, um.GetUserNameByID(task.LockByUser)));
                            row["ID"] = id;
                            row["Name"] = task.Headline;
                            row["User"] = um.GetUserNameByID(task.LockByUser);
                            lockTasks.Rows.Add(row);
                            lockTasksCount++;
                            continue;
                        }
                        // если задача закрыта, пропускаем ее
                        if (task.State == "Закрыта")
                        {
                            closedTaskCount++;
                            continue;
                        }

                        task.BeginUpdate("Редактировать");
                        if (taskParams.BeginDate)
                            task.FromDate = ActiveTask.FromDate;
                        if (taskParams.EndDate)
                            task.ToDate = ActiveTask.ToDate;
                        if (taskParams.Owner)
                        {
                            task.Owner = ActiveTask.Owner;
                        }
                        if (taskParams.Doer)
                        {
                            task.Doer = ActiveTask.Doer;
                        }
                        if (taskParams.Curator)
                        {
                            task.Curator = ActiveTask.Curator;
                        }
                        if (taskParams.TaskKind)
                            task.RefTasksTypes = ActiveTask.RefTasksTypes;

                        if (taskParams.Users)
                        {
                            DataTable dt = um.GetUsersPermissionsForObject(id, 18000);
                            for (int i = 0; i <= dt.Rows.Count - 1; i++)
                            {
                                if (Convert.ToBoolean(dt.Rows[i][3]) != Convert.ToBoolean(dtUsers.Rows[i][3]))
                                    dt.Rows[i][3] = dtUsers.Rows[i][3];
                            }
                            DataTable changes = dt.GetChanges();
                            if (changes != null)
                                um.ApplayUsersPermissionsChanges(id, 18000, changes);
                        }

                        if (taskParams.Groups)
                        {
                            DataTable dt = um.GetGroupsPermissionsForObject(id, 18000);
                            for (int i = 0; i <= dt.Rows.Count - 1; i++)
                            {
                                if (Convert.ToBoolean(dt.Rows[i][2]) != Convert.ToBoolean(dtGroups.Rows[i][2]))
                                    dt.Rows[i][2] = dtGroups.Rows[i][2];
                            }
                            DataTable changes = dt.GetChanges();
                            if (changes != null)
                                um.ApplayGroupsPermissionsChanges(id, 18000, changes);
                        }

                        task.RefreshNavigationRow();
                        task.SaveStateIntoDatabase();
                        task.EndUpdate();
                        task.RefreshNavigationRow();
                    }
                }
                finally
                {
                    this.Workplace.OperationObj.StopOperation();
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Обработано задач: {0}", childRowIDs.Count));
                sb.AppendLine(string.Format("Свойства изменены у задач: {0}", childRowIDs.Count - closedTaskCount - lockTasksCount));
                if (closedTaskCount > 0)
                    sb.AppendLine(string.Format("Пропущено закрытых задач: {0}", closedTaskCount));
                if (lockTasksCount > 0)
                {
                    sb.AppendLine(string.Format("Пропущено заблокированых пользователями задач: {0}, в том числе:", lockTasksCount));
                    //sb.AppendLine("  в том числе:");
                }
                FormTaskParamsResult.ShowResults(sb.ToString(), lockTasks);
            }
        }

        private void SetEnableChildTaskParamsBtn(bool isEnabled)
        {
            _tasksNavigation.utbTasksActions.Tools["btnChangeParams"].SharedProps.Enabled = _tasksNavigation.AllowEditTask && isEnabled;
        }

        /// <summary>
        /// Обработчик нажатия на кнопки применить/отменить объекта просмотра
        /// </summary>
        private void btnCancelApply_Click(object sender, EventArgs e)
        {
            if (_activeTask.LockByUser == -1)
            {
                if (MessageBox.Show("Задача разблокирована другим пользователем. Обновить список задач?", "Задача разблокирована", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    _tasksNavigation.ReloadData(false);
                return;
            }

            switch (((Button)sender).Name)
            {
                case "btnCancel":
                    if (!_activeTask.InEdit && !Workplace.ActiveScheme.UsersManager.CheckPermissionForTask(
                            ActiveTask.ID, ActiveTask.RefTasksTypes,
                            (int)TaskTypeOperations.CanCancelEdit, false))
                    {
                        if (MessageBox.Show("Задача заблокирована другим пользователем. Обновить список задач?", "Задача заблокирована", MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)
                            _tasksNavigation.ReloadData(false);
                        return;
                    }
                    CancelChanges();
                    break;
                case "btnApply":
                    if (_activeTask.LockByUser != Workplace.ActiveScheme.UsersManager.GetCurrentUserID())
                    {
                        if (MessageBox.Show("Задача заблокирована другим пользователем. Обновить список задач?", "Задача заблокирована", MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)
                            _tasksNavigation.ReloadData(false);
                        return;
                    }
                    SaveChanges();
                    break;
            }
            if (ActiveTask != null)
                SetEnableChildTaskParamsBtn(!ActiveTask.InEdit);
            else
                SetEnableChildTaskParamsBtn(false);
            ((Button)sender).Focus();
        }

        /// <summary>
        /// Заполнение выпадающего списка допустимых для данного состояния задачи действий
        /// </summary>
        private void uddbDoAction_DroppingDown(object sender, CancelEventArgs e)
        {
            string[] allowedActions = ActiveTask.GetActionsForState(ActiveTask.State);
            _tasksView.lbActions.Items.Clear();
            foreach (string item in allowedActions)
            {
                TaskActions action = Workplace.ActiveScheme.TaskManager.Tasks.FindActionsFromCaption(item);
                if (action != TaskActions.taDelete)
                    _tasksView.lbActions.Items.Add(item);
            }


            _tasksView.lbActions.Enabled = _tasksView.lbActions.Items.Count > 0;
            if (_tasksView.lbActions.Items.Count == 0)
                _tasksView.lbActions.Items.Add("<Нет доступных действий>");
        }

        /// <summary>
        /// Обработчик закрытия списка с действиями = выполнение действия над задачей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uddbDoAction_ClosedUp(object sender, EventArgs e)
        {
            int selIndex = _tasksView.lbActions.SelectedIndex;
            if (selIndex != -1)
            {
                // получаем выполнимое действие
                string selText = _tasksView.lbActions.Items[selIndex].ToString();
                // устанавливаем название действия
                SetTaskActionCaption(selText);
                // определяем вид действия
                TaskActions action = Workplace.ActiveScheme.TaskManager.Tasks.FindActionsFromCaption(selText);
                if (action == TaskActions.taDelete)
                {
                    _tasksNavigation.DeleteActiveTask(true);
                    return;
                }
                // определяем следующее состояние задачи
                string nextState = Workplace.ActiveScheme.TaskManager.Tasks.GetStateAfterAction(selText);
                try
                {
                    int currentUser = Workplace.ActiveScheme.UsersManager.GetCurrentUserID();
                    if ((action == TaskActions.taExecute || action == TaskActions.taContinueExecute) && ActiveTask.Doer != currentUser)
                    {
                        _tasksView.tbDoer.Text = Workplace.ActiveScheme.UsersManager.GetCurrentUserName();
                        _tasksView.tbDoer.Tag = Workplace.ActiveScheme.UsersManager.GetCurrentUserID();
                        ActiveTask.Doer = Workplace.ActiveScheme.UsersManager.GetCurrentUserID();
                    }
                    // пытаемся получить последние версии документов с сервера
                    SyncLocalDocumentsWithServer();
                    // пытаемся перевести задачу в режим редактирования
                    ActiveTask.BeginUpdate(selText);
                }
                catch
                {
                    // кто то успел заблокировать вперед нас
                    if (MessageBox.Show("Невозможно перейти в режим редактирования." + Environment.NewLine +
                        "Задача заблокирована другим пользователем" + Environment.NewLine +
                        "Обновить список задач?", "Задача заблокирована", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        _tasksNavigation.ReloadData();
                    }
                    return;
                }
                // запоминаем его
                if (nextState != String.Empty)
                    ActiveTask.State = nextState;
                // переводим элементы интерфейса в режим редактирования
                EnableTaskEditMode(true);
                // инициируем перегрузку страницы документов
                ClearDocumentsPage();
                // обновляем строку в гриде навигации
                ActiveTask.RefreshNavigationRow();

                SetEnableChildTaskParamsBtn(!ActiveTask.InEdit);
            }
        }

        private void utcPages_SelectedTabChanging(object sender, SelectedTabChangingEventArgs e)
        {
            CanDeactivate = true;
            // сохраняем изменения основной страницы прав 
            RefreshActiveTaskFields();
            // получаем текущуб активную страницу
            UltraTab curTab = _tasksView.utcPages.SelectedTab;
            if (curTab == null) return;
            switch (curTab.Key)
            {
                // для страниц прав проверяем и сохраняем изменения
                case "utpGroupsPermissions":
                case "utpUsersPermissions":
                    ClearUsersPermissionsPage();
                    break;
                // для параметров и констант - запрещаем переход если есть ошибки в данных
                case "utpConsts":
                    ugeConstsParams_OnSaveChanges(_tasksView.ugeConsts);
                    e.Cancel = !CanDeactivate;
                    break;
                case "utpParameters":
                    ugeConstsParams_OnSaveChanges(_tasksView.ugeParams);
                    e.Cancel = !CanDeactivate;
                    break;
            }
        }

        /// <summary>
        /// Обработчик переключения между закладками грида просмотра
        /// </summary>
        private void utcPages_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            // Загружаем данные задачи в соответствии с активной страницей
            LoadTaskData();
        }

        /// <summary>
        /// Обработчик нажатия на кнопки выбора пользователя (владельца и исполнителя)
        /// </summary>
        private void SelectUserBtnClick(object sender, EventArgs e)
        {
            string userName = string.Empty;
            int userID = 0;
            Button btn = (Button)sender;
            if (usersModalForm == null)
                return;
            if (usersModalForm.ShowModal(NavigationNodeKind.ndAllUsers, ref userID, ref userName))
            {
                switch (btn.Name)
                {
                    case "btnSelectOwner":
                        _tasksView.tbOwner.Text = userName;
                        _tasksView.tbOwner.Tag = userID;
                        break;
                    case "btnSelectDoer":
                        _tasksView.tbDoer.Text = userName;
                        _tasksView.tbDoer.Tag = userID;
                        break;
                    case "btnSelectCurator":
                        _tasksView.tbCurator.Text = userName;
                        _tasksView.tbCurator.Tag = userID;
                        break;
                }
            }
        }

        private void SelectTaskTypeBtnClick(object sender, EventArgs e)
        {
            string taskTypeName = string.Empty;
            int id = 0;
            Button btn = (Button)sender;
            if (usersModalForm == null)
                return;
            if (usersModalForm.ShowModal(NavigationNodeKind.ndTasksTypes, ref id, ref taskTypeName))
            {
                switch (btn.Name)
                {
                    case "btnSelectTaskType":
                        _tasksView.tbTaskKind.Text = taskTypeName;
                        _tasksView.tbTaskKind.Tag = id;
                        break;
                }
            }
        }

        #endregion

        private void utcPages_VisibleChanged(object sender, EventArgs e)
        {
            SetViewObjectCaption();
        }


    }
}