using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Users;


namespace Krista.FM.Server.Tasks
{

    public partial class TaskCollection : RealTimeDBObjectsEnumerableCollection, ITaskCollection//, ITasksExport, ITasksImport
    {
        internal TaskActionManager _actionsManager;

        public TaskCollection(IScheme scheme)
            : base(scheme)
        {
            _actionsManager = new TaskActionManager();
        }

        #region Реализация интерфейса ICollection
        public override int Count
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                using (IDatabase db = this.GetDB())
                {
                    object cnt = db.ExecQuery("select count(ID) from TASKS", QueryResultTypes.Scalar);
                    return Convert.ToInt32(cnt);
                }
            }
        }
        #endregion


        /// <summary>
        /// Возвращает коллекцию ключей (ID)
        /// </summary>
        public ICollection Keys
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                using (IDatabase db = this.GetDB())
                {
                    DataTable dt = (DataTable)db.ExecQuery("select ID from TASKS order by ID", QueryResultTypes.DataTable);

                    ArrayList keyList = new ArrayList();
                    foreach (DataRow row in dt.Rows)
                        keyList.Add(Convert.ToInt32(row[0]));
                    return keyList;
                }
            }
        }

        public ICollection KeyRefs
        {
            get { return null; }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable GetIDRefs()
        {
            using (IDatabase db = this.GetDB())
            {
                return (DataTable)db.ExecQuery(
                    "select ID, RefTasks from TASKS where (not (RefTasks is NULL)) order by ID", 
                    QueryResultTypes.DataTable);
            }
        }

        public ITask this[int key]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                // задачу в любом слуае придется загрузить, нам потребуются значения ее полей
                Task tmpTask = new Task(this);
                tmpTask.ID = key;
                tmpTask.ReloadData();
                try
                {
                    tmpTask.CheckPermission((int)TaskOperations.View, true);
                    // теперь проверяем возможность пользователя просматривать задачу
                    // если задача заблокирована нами - она находится в состоянии редактирования
                    //if (tmpTask.LockByUser == (int)Authentication.UserID)
                    //    tmpTask.InEdit = true;
                    return tmpTask;
                }
                catch
                {
                    tmpTask.Dispose();
                    throw;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ITask AddNew()
        {
            return AddNew(-1);
        }

        /// <summary>
        /// Существует ли задача в базе.
        /// </summary>
        /// <param name="taskID">ID задачи.</param>
        /// <returns></returns>
        private bool CheckTask(int taskID)
        {
            using (IDatabase db = this.GetDB())
            {
                DataTable dt = (DataTable)db.ExecQuery("select ID from Tasks where ID = ?", 
                    QueryResultTypes.DataTable, db.CreateParameter("ID", taskID));
                return ((dt != null) && (dt.Rows.Count > 0));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ITask AddNew(int parentTaskID)
        {
            if ((parentTaskID != -1) && (!CheckTask(parentTaskID)))
                return null;


            // Проверяем возможность создания задачи
            UsersManager um = GetUsersManager();
            // новая задача имеет первый из типов разрешенных пользователю для создания
            ArrayList allowedTasksTypes = um.GetUserCreatableTaskTypes((int)Authentication.UserID);
            if ((allowedTasksTypes == null) || (allowedTasksTypes.Count == 0))
            {
                throw new Exception("Внутренняя ошибка: нет доступных для создания типов задач");
            }
            int newTaskType = (int)allowedTasksTypes[0];

            //um.CheckPermissionForTask(0, 0, (int)AllTasksOperations.CreateTask, true);

            int newID = 0;
            // проверим корректна ли родительская задача
            using (IDatabase db = this.GetDB())
            {
                newID = db.GetGenerator("g_Tasks");
            }
            Task tmpTask = new Task(this);
            tmpTask.ID = newID;
            tmpTask.IsNew = true;
            tmpTask.State = _actionsManager.States[TaskStates.tsCreated].Caption;
            tmpTask.Doer = (int) Authentication.UserID;
            tmpTask.Owner = (int) Authentication.UserID;
            tmpTask.Curator = (int) Authentication.UserID;
            tmpTask.ToDate = System.DateTime.Now;
            tmpTask.FromDate = System.DateTime.Now;
            tmpTask.Headline = "Новая задача";
            tmpTask.Job = "Не указано";
            tmpTask.RefTasksTypes = newTaskType;
            // новая задача заблокирована создавшим пользователем
            tmpTask.LockByUser = (int) Authentication.UserID;
            //tmpTask.SetLastAction(_actionsManager.Actions[TaskActions.taCreate].Caption);
            tmpTask.BeginUpdate(_actionsManager.Actions[TaskActions.taCreate].Caption);

            if (parentTaskID != -1)
                tmpTask.RefTasks = parentTaskID;

            tmpTask.SaveStateIntoDatabase();
            return tmpTask;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
#warning Метод DeleteTaskCascade перенести в класс Task
        internal static void DeleteTaskCascade(IDatabase db, int taskID, DataTable keyRefs)
        {
            // сначала удаляем все подчиненные 
            foreach (DataRow row in keyRefs.Rows)
            {
                int cur_id = Convert.ToInt32(row[0]);
                int cur_refid = Convert.ToInt32(row[1]);
                if (taskID == cur_refid)
                {
                    DeleteTaskCascade(db, cur_id, keyRefs);
                }
            }
            // теперь саму себя
            string queryText = String.Format("delete from ACTIONS where RefTasks = {0}", taskID);
            db.ExecQuery(queryText, QueryResultTypes.NonQuery);
            queryText = String.Format("delete from DOCUMENTS where RefTasks = {0}", taskID);
            db.ExecQuery(queryText, QueryResultTypes.NonQuery);
            queryText = String.Format("delete from TASKS where ID = {0}", taskID);
            db.ExecQuery(queryText, QueryResultTypes.NonQuery);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
#warning Метод DeleteTask перенести в класс Task
        public void DeleteTask(ITask task)
        {
            // Проверяем возможность удаления задачи
            UsersManager um = GetUsersManager();
            int userID = (int)Authentication.UserID;
            um.CheckPermissionForTask(task.ID, task.RefTasksTypes, (int)TaskTypeOperations.DelTaskAction, true);

            int lockedUser = ((Task)task).GetActualLockedUser();
            if (!((lockedUser == -1) || (lockedUser == userID)))
                throw new Exception(String.Format("Невозможно удалить задачу. Задача заблокирована пользователем '{0}'.", um.GetUserNameByID(lockedUser)));

            DataTable keyRefs = this.GetIDRefs();
            IDatabase db = this.GetDB();
            db.BeginTransaction();
            try
            {
                DeleteTaskCascade(db, task.ID, keyRefs);
                db.Commit();
                // теперь разрегистрируем объект соответвующий задаче (если он был)
                um.UnregisterSystemObject(task.ID.ToString());
            }
            catch (Exception e)
            {
                db.Rollback();
                throw new Exception("Невозможно удалить задачу. Возможно одна из дочерних задач заблокирована.", e);
            }
            finally
            {
                db.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteTask(int taskID)
        {
            ITask task = null;
            try
            {
                task = this[taskID];
                DeleteTask(task);
            }
            finally
            {
                if (task != null)
                    task.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDocument(int documentID)
        {
            using (IDatabase db = this.GetDB())
            {
                // обновляем всегда в кэше
                db.ExecQuery("delete from DocumentsTemp where ID = ?",
                    QueryResultTypes.NonQuery, db.CreateParameter("ID", documentID));
            }
        }

        #region Получение списка видимых пользователю задач
        // рекурсивная процедура проставления частичной видимости строк
        private void SetParentRowsAsFantom(DataTable tasks, DataColumn visibleColumn, int? refTasks)
        {
            // получаем родительскую задачу
            string filter = String.Format("ID = {0}", (int)refTasks);
            DataRow[] parentTask = tasks.Select(filter);
            // получаем видимость
            TaskVisibleInNavigation curVisible = (TaskVisibleInNavigation)Convert.ToInt32(parentTask[0][visibleColumn]);
            // если задача видима или фантом (уже обработана) - выходим
            if ((curVisible == TaskVisibleInNavigation.tvVisible) || (curVisible == TaskVisibleInNavigation.tvFantom))
                return;
            // ставим признак частичной видимости
            parentTask[0][visibleColumn] = TaskVisibleInNavigation.tvFantom;
            // получаем ссылку на родительскую задачу
            int? refParent = null;
            try
            {
                refParent = Convert.ToInt32(parentTask[0][TasksNavigationTableColumns.REFTASKS_COLUMN]);
            }
            catch { };
            // если есть родительская задача - вызываем процедуру и для нее
            if (refParent != null)
                SetParentRowsAsFantom(tasks, visibleColumn, refParent);
        }

        private static string queryTemplate = "select ID, State, FromDate, ToDate, " +
            "Doer, Owner, Curator, HeadLine, RefTasks, RefTasksTypes, LockByUser, Job, Description, {0} visible ";

        //private static string allUnlockedFilter = queryTemplate + 
        //    " from Tasks where (LockByUser is null) and (ID >= 0) order by ID";

        //private static string allLockByAnotherFilter = queryTemplate + 
        //    " from Tasks where (ID >= 0) and (not (LockByUser is null)) and (LockByUser <> {0}) order by ID";

        private static string allUnlockedAndLockByAnotherFilter =
            queryTemplate + "from Tasks where (ID >= 0) and (" +
            "(LockByUser is null) or " +
            "((not (LockByUser is null)) and (LockByUser <> {1}))" +
            ") order by ID";

        private static string allLockByMeFilter = queryTemplate + " from TasksTemp where (ID >= 0) and (LockByUser = {1}) order by ID";

        private static string visibleFilter = String.Format("visible <> {0}", (int)TaskVisibleInNavigation.tvInvsible);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable GetTasksInfo()
        {
            return GetTasksInfo(null);
        }

        private DataTable GetAllTasksForUser(IDatabase db, int userID)
        {
            // получаем все незаблокированные задачи и заблокированные не нами
            DataTable allTasks = (DataTable)db.ExecQuery(
                String.Format(allUnlockedAndLockByAnotherFilter, (int)TaskVisibleInNavigation.tvVisible, userID), QueryResultTypes.DataTable);
            // все заблокированные нами
            DataTable lockedTasks = (DataTable)db.ExecQuery(
                String.Format(allLockByMeFilter, (int)TaskVisibleInNavigation.tvVisible, userID), QueryResultTypes.DataTable);
            // объединяем их в одну
            allTasks.Merge(lockedTasks);
            /*
            allTasks.BeginLoadData();
            foreach (DataRow row in lockedTasks.Rows)
            {
                allTasks.Rows.Add(row.ItemArray);
            }
            // для ускорения загрузки выключаем констайнты
            allTasks.EndLoadData();*/
            allTasks.AcceptChanges();
            return allTasks;
        }

        private void SetVisibleFlag(DataTable allTasks, UsersManager um, int curUser)
        {
            // определяем может ли пользователь просматривать все задачи
			bool canViewAll = um.HasUserPermissionForOperation(curUser, (int)AllTasksOperations.ViewAllUsersTasks);
            // если пользователю доступны не все задачи - получаем вспомогательные списки
            ArrayList visibleTaskTypesForUser = null;
            ArrayList visibleTasksForUser = null;
            if (!canViewAll)
            {
                // получаем список типов задач которые может просматривать пользователь и все группы куда он входит
                visibleTaskTypesForUser = um.GetUserVisibleTaskTypes(curUser);
                // получаем список ID задач которые может просматривать пользователь и все группы куда он входит
                visibleTasksForUser = um.GetUserVisibleTasks(curUser);
            }

            allTasks.BeginInit();
            allTasks.BeginLoadData();

            #region проставляем признак видимости задачам на которые у пользователя имеются разрешения
            DataColumn visibleColumn = allTasks.Columns[TasksNavigationTableColumns.VISIBLE_COLUMN];
            // *** FMQ00006389 на SQL Server  почему-то колонка доступна только для чтения
            if (visibleColumn.ReadOnly)
                visibleColumn.ReadOnly = false;
            // *** 
            DataColumn doerColumn = allTasks.Columns[TasksNavigationTableColumns.DOER_COLUMN];
            DataColumn ownerColumn = allTasks.Columns[TasksNavigationTableColumns.OWNER_COLUMN];
            DataColumn curatorColumn = allTasks.Columns[TasksNavigationTableColumns.CURATOR_COLUMN];
            DataColumn idColumn = allTasks.Columns[TasksNavigationTableColumns.ID_COLUMN];
            DataColumn taskTypeColumn = allTasks.Columns[TasksNavigationTableColumns.REFTASKSTYPES_COLUMN];
            foreach (DataRow row in allTasks.Rows)
            {
                // если пользователь может просматривать все задачи - ставим признак видимости
                if (canViewAll)
                    continue;

                // если текущий пользователь - владелец, исполнитель или куратор - ставим признак видимости
                int doer = Convert.ToInt32(row[doerColumn]);
                int owner = Convert.ToInt32(row[ownerColumn]);
                int curator = Convert.ToInt32(row[curatorColumn]);
                if ((curUser == doer) || (curUser == owner) || (curUser == curator))
                    continue;

                // если задача имеет тип который пользователь может просматривать - ставим признак видимости
                int taskType = Convert.ToInt32(row[taskTypeColumn]);
                if ((visibleTaskTypesForUser != null) && (visibleTaskTypesForUser.Contains(taskType)))
                    continue;

                // если текущий пользователь имеет право на просмотр этой конкретной задачи - ставим признак видимости
                int taskID = Convert.ToInt32(row[idColumn]);
                if (visibleTasksForUser.Contains(taskID))
                    continue;

                // иначе - задача не видна
                row[visibleColumn] = TaskVisibleInNavigation.tvInvsible;
            }
            #endregion
            #region проставляем признак частичной видимости невидимым родительским задачам
            if (!canViewAll)
            {
                foreach (DataRow row in allTasks.Rows)
                {
                    // получаем ссылку на родителя и видимость задачи
                    int? refTasks = null;
                    try
                    {
                        refTasks = Convert.ToInt32(row[TasksNavigationTableColumns.REFTASKS_COLUMN]);
                    }
                    catch { };
                    TaskVisibleInNavigation curVisible = (TaskVisibleInNavigation)Convert.ToInt32(row[visibleColumn]);
                    // если задача видима и у нее есть родитель - проставляем фантомные ссылки
                    if ((refTasks != null) && (curVisible == TaskVisibleInNavigation.tvVisible))
                        SetParentRowsAsFantom(allTasks, visibleColumn, refTasks);
                }
            }
            #endregion
            allTasks.EndLoadData();
            allTasks.EndInit();
            allTasks.AcceptChanges();
        }

        private static void AppendAdditionalColumns(DataTable tasksTable)
        {
            tasksTable.Columns.Add(new DataColumn(TasksNavigationTableColumns.OWNERNAME_COLUMN, typeof(string)));
            tasksTable.Columns.Add(new DataColumn(TasksNavigationTableColumns.DOERNAME_COLUMN, typeof(string)));
            tasksTable.Columns.Add(new DataColumn(TasksNavigationTableColumns.CURATORNAME_COLUMN, typeof(string)));
            tasksTable.Columns.Add(new DataColumn(TasksNavigationTableColumns.LOCKEDUSERNAME_COLUMN, typeof(string)));
            tasksTable.Columns.Add(new DataColumn(TasksNavigationTableColumns.STATECODE_COLUMN, typeof(int)));
        }

        private void CopyVisibleTaskAndDoRenaming(DataTable allTasks, DataTable visibleTasks, IUsersManager um)
        {
            #region Отдаем пользователю только видимые и фантомные задачи
            DataRow[] filtered = allTasks.Select(visibleFilter);
            visibleTasks.BeginLoadData();
            for (int i = 0; i < filtered.Length; i++)
            {
                visibleTasks.Rows.Add(filtered[i].ItemArray);
            }

            #region Производим разыменовку пользователей

            // запоминаем колонки с ID
            DataColumn ownerColumn = visibleTasks.Columns[TasksNavigationTableColumns.OWNER_COLUMN];
            DataColumn doerColumn = visibleTasks.Columns[TasksNavigationTableColumns.DOER_COLUMN];
            DataColumn curatorColumn = visibleTasks.Columns[TasksNavigationTableColumns.CURATOR_COLUMN];
            DataColumn lockedUserColumn = visibleTasks.Columns[TasksNavigationTableColumns.LOCKBYUSER_COLUMN];
            DataColumn stateColumn = visibleTasks.Columns[TasksNavigationTableColumns.STATE_COLUMN];
            // и с именами
            DataColumn ownerNameColumn = visibleTasks.Columns[TasksNavigationTableColumns.OWNERNAME_COLUMN];
            DataColumn doerNameColumn = visibleTasks.Columns[TasksNavigationTableColumns.DOERNAME_COLUMN];
            DataColumn curatorNameColumn = visibleTasks.Columns[TasksNavigationTableColumns.CURATORNAME_COLUMN];
            DataColumn lockedUserNameColumn = visibleTasks.Columns[TasksNavigationTableColumns.LOCKEDUSERNAME_COLUMN];
            DataColumn stateCodeColumn = visibleTasks.Columns[TasksNavigationTableColumns.STATECODE_COLUMN];

            foreach (DataRow row in visibleTasks.Rows)
            {
                row.BeginEdit();
                row[ownerNameColumn] = um.GetUserNameByID(Convert.ToInt32(row[ownerColumn]));
                row[doerNameColumn] = um.GetUserNameByID(Convert.ToInt32(row[doerColumn]));
                row[curatorNameColumn] = um.GetUserNameByID(Convert.ToInt32(row[curatorColumn]));
                if (row[lockedUserColumn] != DBNull.Value)
                    row[lockedUserNameColumn] = um.GetUserNameByID(Convert.ToInt32(row[lockedUserColumn]));
                TaskStates ts = TaskStates.tsUndefined;
                try
                {
                    ts = FindStateFromCaption(Convert.ToString(row[stateColumn]));
                }
                catch { }
                row[stateCodeColumn] = (int)ts;
                row.EndEdit();
            }
            #endregion

            visibleTasks.EndLoadData();
            visibleTasks.AcceptChanges();
            visibleTasks.RemotingFormat = SerializationFormat.Binary;
            #endregion

        }

        private DataTable GetTasksInfo(IDatabase externalDb)
        {
            DataTable dt = null;
            DataTable allTasks = null;
            UsersManager um = (UsersManager)this.GetUsersManager();
            IDatabase db = null;
            if (externalDb != null)
                db = externalDb;
            else
                db = this.GetDB();

            //int start = Environment.TickCount;
            try
            {
                int curUser = (int)Authentication.UserID;

                // получаем все задачи каким-либо образом видимые пользователю
                allTasks = GetAllTasksForUser(db, curUser);
                // проставляем признак видимости
                SetVisibleFlag(allTasks, um, curUser);
                // создаем таблицу-клон и помещаем в нее поля для разыменовки
                dt = allTasks.Clone();
                dt.PrimaryKey = new DataColumn[1] { dt.Columns[TasksNavigationTableColumns.ID_COLUMN] };

                AppendAdditionalColumns(dt);
                // если данных нет - ничего более не делаем
                if (allTasks.Rows.Count == 0)
                    return dt;
                // если есть - перемещаем в клон видимые пользователю задачи
                CopyVisibleTaskAndDoRenaming(allTasks, dt, um);
            }
            finally
            {
                if ((db != null) && (externalDb == null))
                    db.Dispose();
            }
            //int elapsed = Environment.TickCount - start;
            return dt;
        }
        #endregion

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable GetCurrentUserLockedTasks()
        {
            IDatabase db = null;
            DataTable userTasks = null;
            try
            {
                db = this.GetDB();
                int curUser = (int)Authentication.UserID;
                string queryText = "select ID, State, CAction, Headline from TasksTemp where LockByUser = " + curUser.ToString();
                userTasks = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return userTasks;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetStateAfterAction(string actionCaption)
        {
            return _actionsManager.GetStateAfterAction(actionCaption);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TaskActions FindActionsFromCaption(string actionCaption)
        {
            TaskAction action = _actionsManager.FindActionFromCaption(actionCaption);
            if (action != null)
                return (TaskActions)action.Index;
            else
                return TaskActions.taUndefined;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TaskStates FindStateFromCaption(string stateCaption)
        {
            TaskState state = _actionsManager.FindStateFromCaption(stateCaption);
            if (state != null)
                return (TaskStates)state.Index;
            else
                return TaskStates.tsUndefined;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string[] GetAllStatesCaptions()
        {
            return _actionsManager.GetAllStatesCaptions();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable FindDocuments(string filter)
        {
            int curUserID = (int)Authentication.UserID;
            // текущему пользователю доступны - 
            string ownershipFilter = String.Format(
                "((doc.Ownership = {0}) or " +
                "((tsks.Owner = {1}) and (doc.Ownership = {2})) or " +
                "((tsks.Doer = {3}) and (doc.Ownership = {4})) or " +
                "((tsks.Curator = {5}) and (doc.Ownership = {6})))",
                (int)TaskDocumentOwnership.doGeneral,
                curUserID, (int)TaskDocumentOwnership.doOwner,
                curUserID, (int)TaskDocumentOwnership.doDoer,
                curUserID, (int)TaskDocumentOwnership.doCurator
            );

            string queryStr = String.Format(
                "select tsks.Headline, tsks.State, tsks.FromDate, tsks.ToDate, " +
                "tsks.Doer, tsks.Owner, tsks.Curator, " +
                "doc.ID, doc.DocumentType, doc.Name, doc.SourceFileName, doc.Version, " +
                "doc.Description, doc.Ownership, doc.RefTasks " +
                "from Documents doc, Tasks tsks " +
                "where (tsks.ID = doc.RefTasks) and {0} and {1} order by ID",
                ownershipFilter, filter);
            DataTable dt = null;
            Database db = null;
            try
            {
                db = (Database)this.GetDB();
                dt = (DataTable)db.ExecQuery(queryStr, QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }

        private static string updateDocumentDataQuery =
            "update {0} set Document = ? where ID = {1}";

        private static string updateDocumentVersionQuery =
            "update DocumentsTemp set Version = Version + 1 where ID = {0}";

        public void SetDocumentData(int documentID, byte[] documentData, bool updateTemp, IDatabase db)
        {
            string tableName = "DocumentsTemp";
            if (!updateTemp)
                tableName = "Documents";

            IDbDataParameter prm = db.CreateBlobParameter("Document", ParameterDirection.Input, documentData);
            // обновляем данные документа
            db.ExecQuery(
                String.Format(updateDocumentDataQuery, tableName, documentID),
                QueryResultTypes.NonQuery,
                prm
            );
            // если это запись в TEMP - увеличиваем версию
            if (updateTemp)
                db.ExecQuery(
                    String.Format(updateDocumentVersionQuery, documentID), 
                    QueryResultTypes.NonQuery
                );
            // документы большие, освободим память сразу
            GC.GetTotalMemory(true);
        }

    }

}