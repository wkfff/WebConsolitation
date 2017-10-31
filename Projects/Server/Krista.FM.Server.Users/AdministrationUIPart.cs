using System;
using System.Text;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Users
{
    /// <summary>
    /// Методы, отвечающие за передачу/прием информации от клиентской части (интерфейса администрирования)
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {
        #region Передача информации клиенту

        public void AppendUserNameColumn(DataTable dt, string refUsersColumnName)
        {
            lock (syncUsers)
            {
                if (dt == null)
                    throw new Exception("Не задана таблица");

                if (!dt.Columns.Contains(refUsersColumnName))
                    throw new Exception(String.Format("Таблица не содержит ссылочного поля {0}", refUsersColumnName));

                dt.Columns.Add(new DataColumn(refUsersColumnName + "_Name", typeof (string)));
                dt.BeginLoadData();
                try
                {
                    int userID = 0;
                    string userName = String.Empty;
                    int userIDColumnInd = dt.Columns.IndexOf(refUsersColumnName);
                    int userNameColumnInd = dt.Columns.IndexOf(refUsersColumnName + "_Name");
                    foreach (DataRow row in dt.Rows)
                    {
                        try
                        {
                            userID = Convert.ToInt32(row[userIDColumnInd]);
                            userName = this.GetUserNameByID(userID);
                            row[userNameColumnInd] = userName;
                        }
                        catch
                        {
                            row[userNameColumnInd] = "Пользователь не определен";
                        }
                    }
                }
                finally
                {
                    dt.EndLoadData();
                    dt.AcceptChanges();
                }
            }
        }

        // TypesHelper
        public string GetCaptionForOperation(int operation)
        {
            string operationCaption = String.Empty;
            Type operationType = null;
            int? parentOperation = null;
            TypesHelper.GetOperationInfo(operation, ref operationCaption, ref operationType, ref parentOperation);
            return operationCaption;
        }

        public string GetNameFromDirectoryByID(DirectoryKind dk, int id)
        {
            string name;
            string queryText = "select Name from {0} where ID = " + id;
            switch (dk)
            {
                case DirectoryKind.dkOrganizations:
                    queryText = String.Format(queryText, "Organizations");
                    break;
                case DirectoryKind.dkDepartments:
                    queryText = String.Format(queryText, "Departments");
                    break;
                case DirectoryKind.dkTasksTypes:
                    queryText = String.Format(queryText, "TasksTypes");
                    break;
            }

            using (IDatabase db = _scheme.SchemeDWH.DB)
            {
                name = Convert.ToString(
					db.ExecQuery(queryText, QueryResultTypes.Scalar));
            }
            return name ?? "Значение не найдено";
        }

        private DataTable InternalGetDirectoryEntry(DirectoryKind dk)
        {
            DataUpdater upd = null;
            switch (dk)
            {
                case DirectoryKind.dkUsers:
                    upd = GetUsersUpdater(FixedUsers.InstallAdminFilter);
                    break;
                case DirectoryKind.dkGroups:
                    upd = GetGroupsUpdater();
                    break;
                case DirectoryKind.dkDepartments:
                    upd = GetDepartmentsUpdater();
                    break;
                case DirectoryKind.dkOrganizations:
                    upd = GetOrganizationsUpdater();
                    break;
                case DirectoryKind.dkTasksTypes:
                    upd = GetTasksTypesUpdater(null);
                    break;
            }
            DataTable dt = new DataTable(dk.ToString());
            try
            {
                upd.Fill(ref dt);
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
            }
            return dt;
        }

        public DataTable GetUsers()
        {
            lock (syncUsers)
            {
                return InternalGetDirectoryEntry(DirectoryKind.dkUsers);
            }
        }

        public DataTable GetGroups()
        {
            lock (syncGroups)
            {
                return InternalGetDirectoryEntry(DirectoryKind.dkGroups);
            }
        }

        public DataTable GetOrganizations()
        {
            lock (syncObjects)
            {
                return InternalGetDirectoryEntry(DirectoryKind.dkOrganizations);
            }
        }

        public DataTable GetDepartments()
        {
            lock (syncObjects)
            {
                return InternalGetDirectoryEntry(DirectoryKind.dkDepartments);
            }
        }

        public DataTable GetTasksTypes()
        {
            lock (syncObjects)
            {
                return InternalGetDirectoryEntry(DirectoryKind.dkTasksTypes);
            }
        }

        public DataTable GetObjects()
        {
            // в интерфейсе не показываем объекты типа "задача" и "шаблон"
            // и интерфейс администрирования
            int admObjID = this.GetSystemObjectIDByName("AdministrationUI");

			string filter = String.Format("(OBJECTTYPE <> {0}) and (OBJECTTYPE <> {1}) and (ID <> {2})",
				(int)SysObjectsTypes.Task, (int)SysObjectsTypes.Template, admObjID);

            lock (syncObjects)
            {
                DataRow[] filtered = ObjectsTable.Select(filter);

                DataTable filteredObjects = ObjectsTable.Clone();
                filteredObjects.Columns.Add("RusObjectType", typeof (string));
                if (filtered.Length > 0)
                {
                    filteredObjects.BeginLoadData();
                    for (int i = 0; i < filtered.Length; i++)
                    {
                        SysObjectsTypes objectType = (SysObjectsTypes) Convert.ToInt32(filtered[i]["OBJECTTYPE"]);
                        string typeCaption = TypesHelper.GetCaptionForObjectType(objectType);
                        DataRow row = filteredObjects.Rows.Add(filtered[i].ItemArray);
                        row["RusObjectType"] = typeCaption;
                    }
                    filteredObjects.EndLoadData();
                    filteredObjects.AcceptChanges();
                }

                return filteredObjects;
            }
        }

        private DataTable GetMembershipTable(int id, bool getUsers)
        {
            DataTable dt = new DataTable();
            //создаем структуру таблицы
            dt.BeginInit();
            try
            {
                DataColumn clmn = new DataColumn("ID", typeof(int));
                dt.Columns.Add(clmn);
                clmn = new DataColumn("Name", typeof(string));
                dt.Columns.Add(clmn);
                clmn = new DataColumn("IsMember", typeof(bool));
                dt.Columns.Add(clmn);
            }
            finally
            {
                dt.EndInit();
            }
            // заполняем таблицу данными
            dt.BeginLoadData();
            try
            {
                if (getUsers)
                {
                    lock (syncUsers)
                    {
                        foreach (SysUser us in Users.Values)
                        {
                            int userID = us.ID;
                            if ((userID == (int) FixedUsers.FixedUsersIds.InstallAdmin) ||
                                (userID >= FixedUsers.MaxFixedUserID))
                            {
                                // получаем всех пользователей группы
                                string filter = String.Format("REFUSERS = {0} and REFGROUPS = {1}", userID, id);
                                DataRow[] rows = MembershipsTable.Select(filter);
                                bool isMember = (rows != null) && (rows.Length > 0);
                                dt.Rows.Add(userID, us.Name, isMember);
                            }
                        }
                    }
                }
                else
                {
                    lock (syncGroups)
                    {
                        foreach (SysGroup grp in Groups.Values)
                        {
                            int groupID = grp.ID;
                            // получаем всех группы
                            string filter = String.Format("REFUSERS = {0} and REFGROUPS = {1}", id, groupID);
                            DataRow[] rows = MembershipsTable.Select(filter);
                            bool isMember = (rows != null) && (rows.Length > 0);
                            dt.Rows.Add(groupID, grp.Name, isMember);
                        }
                    }
                }
            }
            finally
            {
                dt.EndLoadData();
                dt.AcceptChanges();
            }
            return dt;
        }

        public DataTable GetUsersForGroup(int groupID)
        {
            return GetMembershipTable(groupID, true);
        }

        public DataTable GetGroupsForUser(int userID)
        {
            return GetMembershipTable(userID, false);
        }

        #region права на объект для пользователей

        public DataTable GetUsersPermissionsForObject(int objectID, int objectType)
        {
            SysObjectsTypes st = (SysObjectsTypes)objectType;
            string[] allowedActions = TypesHelper.GetOperationsForType(st);
            DataTable dt = new DataTable("UsersPermissions");
            lock (syncPermissions)
            {
                #region создаем таблицу

                dt.BeginInit();
                try
                {
                    DataColumn clmn = new DataColumn("ID", typeof (int));
                    dt.Columns.Add(clmn);
                    clmn = new DataColumn("Name", typeof (string));
                    dt.Columns.Add(clmn);
                    for (int i = 0; i < allowedActions.Length; i++)
                    {
                        clmn = new DataColumn("grp_" + allowedActions[i], typeof (bool));
                        dt.Columns.Add(clmn);
                        clmn = new DataColumn(allowedActions[i], typeof (bool));
                        dt.Columns.Add(clmn);
                    }
                }
                catch
                {
                    dt.EndInit();
                }

                #endregion

                // для объектов типа "Задача" получаем ID
                int? taskID = null;
                if (objectType == (int) SysObjectsTypes.Task)
                    taskID = GetSystemObjectID(objectID, false, "Задача", SysObjectsTypes.Task);
                else if (objectType == (int) SysObjectsTypes.Template)
                    taskID = GetSystemObjectID(objectID, false, "Шаблон", SysObjectsTypes.Template);

                // наполняем таблицу данными
                dt.BeginLoadData();
                try
                {
                    // сначала формируем кол-во строк по кол-ву пользователей
                    // 1) формируем заготовку для новой записи (первые два поля - 
                    //    ID и Name, остальные - булевские для операций)
                    object[] paramsArray = new object[dt.Columns.Count];
                    for (int i = 2; i < dt.Columns.Count; i++)
                        paramsArray[i] = false;
                    // 2) теперь дописываем ID и Name (в зависимости от типа коллекции) и добавляем в таблицу
                    string permissionsFilter = String.Empty;
                    foreach (SysUser us in Users.Values)
                    {
                        if ((us.ID == (int) FixedUsers.FixedUsersIds.InstallAdmin) ||
                            (us.ID >= FixedUsers.MaxFixedUserID))
                        {
                            paramsArray[0] = us.ID;
                            paramsArray[1] = us.Name;
                            dt.Rows.Add(paramsArray);
                        }
                    }

                    #region права пользователя

                    if (objectType == (int) SysObjectsTypes.Task || objectType == (int) SysObjectsTypes.Template)
                    {
                        if (taskID != null)
                            permissionsFilter = String.Format(
                                "REFOBJECTS = {0}", taskID);
                        else
                            permissionsFilter = "REFOBJECTS is NULL";
                    }
                    else
                    {
                        permissionsFilter = String.Format(
                            "REFOBJECTS = {0}", objectID);
                    }

                    // теперь получаем разрешения для всех пользователей/групп на объект
                    // (ID, REFOBJECTS, REFGROUPS, REFUSERS, ALLOWEDACTION)
                    DataRow[] permissionsRows = PermissionsTable.Select(permissionsFilter);
                    // если есть какие-то разрешения - дописываем их в массив
                    foreach (DataRow permissionRow in permissionsRows)
                    {
                        if (permissionRow.IsNull("REFUSERS") && !permissionRow.IsNull("REFGROUPS"))
                            continue;

                        // получаем ИД пользователя и код разрешенной операции
                        int searchedID = 0;
                        searchedID = Convert.ToInt32(permissionRow["REFUSERS"]);
                        int allowedAction = Convert.ToInt32(permissionRow["ALLOWEDACTION"]);
                        // получаем ссылку на разрешения пользователя в основной таблице
                        DataRow[] searchedRow = dt.Select(String.Format("ID = {0}", searchedID));
                        if (searchedRow.Length > 0 && searchedRow[0].Table.Columns.Contains(allowedAction.ToString()))
                        {
                            // устанавливаем разрешение
                            searchedRow[0][Convert.ToString(allowedAction)] = true;
                        }
                    }

                    #endregion

                    #region Унаследованные права (права группы)

                    foreach (DataRow row in dt.Rows)
                    {
                        int userID = Convert.ToInt32(row[0]);
                        for (int i = 0; i < allowedActions.Length; i++)
                        {
                            int allowedAction = Convert.ToInt32(allowedActions[i]);
                            bool groupPermission = false;
                            if (objectType == (int) SysObjectsTypes.Task || objectType == (int) SysObjectsTypes.Template)
                            {
                                if (taskID != null)
                                    groupPermission = CheckPermissionForUserGroups(userID, (int) taskID, allowedAction);
                                //CheckPermissionForUserGroups(userID, (int)taskID, allowedAction);
                            }
                            else
                                groupPermission = CheckPermissionForUserGroups(userID, objectID, allowedAction);
                            //CheckPermissionForUserGroups(userID, objectID, allowedAction);

                            if (groupPermission)
                                row["grp_" + allowedActions[i]] = true;
                        }
                    }

                    #endregion
                }
                finally
                {
                    dt.EndLoadData();
                    dt.AcceptChanges();
                }
            }
            return dt;
        }

        #endregion

        #region права для групп на объект

        public DataTable GetGroupsPermissionsForObject(int objectID, int objectType)
        {
            SysObjectsTypes st = (SysObjectsTypes)objectType;
            string[] allowedActions = TypesHelper.GetOperationsForType(st);
            DataTable dt = new DataTable("GroupsPermissions");
            lock (syncPermissions)
            {
                #region создаем таблицу

                dt.BeginInit();
                try
                {
                    DataColumn clmn = new DataColumn("ID", typeof (int));
                    dt.Columns.Add(clmn);
                    clmn = new DataColumn("Name", typeof (string));
                    dt.Columns.Add(clmn);
                    for (int i = 0; i < allowedActions.Length; i++)
                    {
                        clmn = new DataColumn(allowedActions[i], typeof (bool));
                        dt.Columns.Add(clmn);
                    }
                }
                catch
                {
                    dt.EndInit();
                }

                #endregion

                // для объектов типа "Задача" получаем ID
                int? taskID = null;
                if (objectType == (int) SysObjectsTypes.Task)
                    taskID = GetSystemObjectID(objectID, false, "Задача", SysObjectsTypes.Task);
                if (objectType == (int) SysObjectsTypes.Template)
                    taskID = GetSystemObjectID(objectID, false, "Шаблон", SysObjectsTypes.Template);

                // наполняем таблицу данными
                dt.BeginLoadData();
                try
                {
                    // сначала формируем кол-во строк по кол-ву пользователей
                    // 1) формируем заготовку для новой записи (первые два поля - 
                    //    ID и Name, остальные - булевские для операций)
                    object[] paramsArray = new object[dt.Columns.Count];
                    for (int i = 2; i < dt.Columns.Count; i++)
                        paramsArray[i] = false;
                    // 2) теперь дописываем ID и Name (в зависимости от типа коллекции) и добавляем в таблицу
                    foreach (SysGroup grp in Groups.Values)
                    {
                        paramsArray[0] = grp.ID;
                        paramsArray[1] = grp.Name;
                        dt.Rows.Add(paramsArray);
                    }

                    string permissionsFilter = String.Empty;
                    if (objectType == (int) SysObjectsTypes.Task || objectType == (int) SysObjectsTypes.Template)
                    {
                        if (taskID != null)
                            permissionsFilter = String.Format(
                                "REFOBJECTS = {0}", taskID);
                        else
                            permissionsFilter = "REFOBJECTS is NULL";
                    }
                    else
                    {
                        permissionsFilter = String.Format(
                            "REFOBJECTS = {0}", objectID);
                    }
                    // теперь получаем разрешения для всех пользователей/групп на объект
                    // (ID, REFOBJECTS, REFGROUPS, REFUSERS, ALLOWEDACTION)
                    DataRow[] permissionsRows = PermissionsTable.Select(permissionsFilter);
                    // если есть какие-то разрешения - дописываем их в массив
                    foreach (DataRow permissionRow in permissionsRows)
                    {
                        if (permissionRow.IsNull("REFGROUPS") && !permissionRow.IsNull("REFUSERS"))
                            continue;
                        // получаем ИД пользователя и код разрешенной операции
                        int searchedID = 0;
                        searchedID = Convert.ToInt32(permissionRow["REFGROUPS"]);

                        int allowedAction = Convert.ToInt32(permissionRow["ALLOWEDACTION"]);
                        // получаем ссылку на разрешения пользователя в основной таблице
                        DataRow[] searchedRow = dt.Select(String.Format("ID = {0}", searchedID));
                        if ((searchedRow == null) || (searchedRow.Length != 1))
                            throw new Exception("Обнаружено разрешение для несуществующей группы");
                        // устанавливаем разрешение
                        if (searchedRow[0].Table.Columns.Contains(allowedAction.ToString()))
                        {
                            searchedRow[0][Convert.ToString(allowedAction)] = true;
                        }
                    }
                }
                finally
                {
                    dt.EndLoadData();
                    dt.AcceptChanges();
                }
            }
            return dt;
        }

        #endregion

        public DataTable GetAssignedPermissions(int mainID, bool isUser)
        {
            DataTable dt = new DataTable("AssignedPermissions");
            dt.BeginInit();
            try
            {
                DataColumn clmn = new DataColumn("ObjectID", typeof(int));
                dt.Columns.Add(clmn);
                clmn = new DataColumn("ObjectCaption", typeof(string));
                dt.Columns.Add(clmn);
                clmn = new DataColumn("AllowedAction", typeof(int));
                dt.Columns.Add(clmn);
                clmn = new DataColumn("AllowedActionCaption", typeof(string));
                dt.Columns.Add(clmn);
            }
            finally
            {
                dt.EndInit();
            }
            lock (syncObjects)
            {
                foreach (DataRow row in ObjectsTable.Rows)
                {
                    int objID = Convert.ToInt32(row["ID"]);
                    int objType = Convert.ToInt32(row["OBJECTTYPE"]);
                    string objCaption = Convert.ToString(row["CAPTION"]);
                    if (objType == (int) SysObjectsTypes.Task || objType == (int) SysObjectsTypes.Template)
                    {
                        objCaption = String.Format("Задача (ID={0})", row["NAME"]);
                    }
                    if ((SysObjectsTypes) objType == SysObjectsTypes.WebReports)
                    {

                    }
                    int[] allowedActions = TypesHelper.GetOperationsForTypeInt((SysObjectsTypes) objType);
                    for (int i = 0; i < allowedActions.Length; i++)
                    {
                        if (CheckPermission(mainID, objID, allowedActions[i], isUser))
                        {
                            DataRow newRow = dt.Rows.Add();
                            newRow["ObjectID"] = objID;
                            newRow["ObjectCaption"] = objCaption;
                            newRow["AllowedAction"] = allowedActions[i];
                            newRow["AllowedActionCaption"] = this.GetCaptionForOperation(allowedActions[i]);
                        }
                    }
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        #endregion

        #region Прием изменений и сопутствующие методы
        private string GetSimpleTableChangesEventMsg(UsersOperationEventKind eventKind, DataTable changes)
        {
            if (changes == null)
                return String.Empty;

            string addRecordTemplate = String.Empty;
            string delRecordTemplate = String.Empty;
            string changeRecordTemplate = String.Empty;

            switch (eventKind)
            {
                case UsersOperationEventKind.uoeChangeGroupsTable:
                    addRecordTemplate = "Добавлена группа '{0}' (ID={1})";
                    delRecordTemplate = "Удалена группа '{0}' (ID={1})";
                    changeRecordTemplate = "Изменены параметры группы '{0}' (ID={1})";
                    break;
                case UsersOperationEventKind.uoeChangeUsersTable:
                    addRecordTemplate = "Добавлен пользователь '{0}' (ID={1})";
                    delRecordTemplate = "Удален пользователь '{0}' (ID={1})";
                    changeRecordTemplate = "Изменены параметры пользователя '{0}' (ID={1})";
                    break;
                case UsersOperationEventKind.uoeChangeDepartmentsTable:
                    addRecordTemplate = "Добавлен отдел '{0}' (ID={1})";
                    delRecordTemplate = "Удален отдел '{0}' (ID={1})";
                    changeRecordTemplate = "Изменены параметры отдела '{0}' (ID={1})";
                    break;
                case UsersOperationEventKind.uoeChangeOrganizationsTable:
                    addRecordTemplate = "Добавлена организация '{0}' (ID={1})";
                    delRecordTemplate = "Удалена организация '{0}' (ID={1})";
                    changeRecordTemplate = "Изменены параметры организации '{0}' (ID={1})";
                    break;
                case UsersOperationEventKind.uoeChangeTasksTypes:
                    addRecordTemplate = "Добавлен вид задач '{0}' (ID={1})";
                    delRecordTemplate = "Удален вид задач '{0}' (ID={1})";
                    changeRecordTemplate = "Изменены параметры типа задач '{0}' (ID={1})";
                    break;
                default:
                    throw new Exception(String.Format("Внутренняя ошибка. Метод вызван для неподдерживаемого типа {0}", eventKind.ToString()));
            }

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in changes.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        sb.AppendLine(String.Format(addRecordTemplate, Convert.ToString(row["NAME"]), Convert.ToString(row["ID"])));
                        break;
                    case DataRowState.Deleted:
                        sb.AppendLine(String.Format(delRecordTemplate, Convert.ToString(row["NAME", DataRowVersion.Original]), Convert.ToString(row["ID", DataRowVersion.Original])));
                        break;
                    case DataRowState.Modified:
                        sb.AppendLine(String.Format(changeRecordTemplate, Convert.ToString(row["NAME"]), Convert.ToString(row["ID"])));
                        break;
                }
            }
            return sb.ToString();
        }

        private string GetMembershipsTableChangesEventMsg(DataTable changes)
        {
            if (changes == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in changes.Rows)
            {
                string userName = String.Empty;
                string groupName = String.Empty;
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        userName = GetUserNameByID(Convert.ToInt32(row["REFUSERS"]));
                        groupName = GetGroupNameByID(Convert.ToInt32(row["REFGROUPS"]));
                        sb.AppendLine(String.Format("Пользователь {0} добавлен в группу {1}", userName, groupName));
                        break;
                    case DataRowState.Deleted:
                        userName = GetUserNameByID(Convert.ToInt32(row["REFUSERS", DataRowVersion.Original]));
                        groupName = GetGroupNameByID(Convert.ToInt32(row["REFGROUPS", DataRowVersion.Original]));
                        sb.AppendLine(String.Format("Пользователь {0} удален из группы {1}", userName, groupName));
                        break;
                }
            }
            return sb.ToString();
        }

        private void WriteAdministrationTableChangeEvent(UsersOperationEventKind eventKind, DataTable changes)
        {
            string eventMsg = String.Empty;

            if (eventKind == UsersOperationEventKind.uoeChangeMembershipsTable)
                eventMsg = GetMembershipsTableChangesEventMsg(changes);
            else
                eventMsg = GetSimpleTableChangesEventMsg(eventKind, changes);

            if (eventMsg == String.Empty)
                return;
            IUsersOperationProtocol log = null;
            try
            {
                LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
                string userHost = callerContext["Host"].ToString();
                log = (IUsersOperationProtocol)this._scheme.GetProtocol("Krista.FM.Server.Users.dll");
                log.WriteEventIntoUsersOperationProtocol(eventKind, eventMsg, userHost);
            }
            finally
            {
                if (log != null)
                    log.Dispose();
            }
        }

        private bool InternalApplayDirectoryChanges(DirectoryKind dk, IDatabase db, DataTable changes)
        {
            if ((changes == null) || (changes.Rows.Count == 0))
                return false;
            lock (syncUsers)
            {
                DataUpdater upd = null;
                UsersOperationEventKind ek = UsersOperationEventKind.uoeChangeUsersTable;
                switch (dk)
                {
                    case DirectoryKind.dkUsers:
                        upd = GetUsersUpdater();
                        ek = UsersOperationEventKind.uoeChangeUsersTable;
                        break;
                    case DirectoryKind.dkGroups:
                        upd = GetGroupsUpdater();
                        ek = UsersOperationEventKind.uoeChangeGroupsTable;
                        break;
                    case DirectoryKind.dkOrganizations:
                        upd = GetOrganizationsUpdater();
                        ek = UsersOperationEventKind.uoeChangeOrganizationsTable;
                        break;
                    case DirectoryKind.dkDepartments:
                        upd = GetDepartmentsUpdater();
                        ek = UsersOperationEventKind.uoeChangeDepartmentsTable;
                        break;
                    case DirectoryKind.dkTasksTypes:
                        upd = GetTasksTypesUpdater(db);
                        ek = UsersOperationEventKind.uoeChangeTasksTypes;
                        break;
                }
                try
                {
                    DataTable changesDublicate = changes.GetChanges();
                    upd.Update(ref changes);
                    WriteAdministrationTableChangeEvent(ek, changesDublicate);
                }
                finally
                {
                    if (upd != null)
                        upd.Dispose();
                }
            }
            return true;
        }

        public void ApplayObjectsChanges(DataTable changes)
        {
            if ((changes == null) || (changes.Rows.Count == 0))
                return;

            DataUpdater upd = null;
            try
            {
                upd = GetObjectUpdaterForUpdateDescription();
                upd.Update(ref changes);
                LoadObjects();
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
            }
        }

        public void ApplayUsersChanges(DataTable changes)
        {
            if (InternalApplayDirectoryChanges(DirectoryKind.dkUsers, null, changes))
            {
                LoadUsers();
                LoadMembership();
                LoadPermissions();
            }
        }

        public void ApplayGroupsChanges(DataTable changes)
        {
            if (InternalApplayDirectoryChanges(DirectoryKind.dkGroups, null, changes))
            {
                LoadGroups();
                LoadMembership();
                LoadPermissions();
            }
        }

        public void ApplayDepartmentsChanges(DataTable changes)
        {
            InternalApplayDirectoryChanges(DirectoryKind.dkDepartments, null, changes);
        }

        public void ApplayOrganizationsChanges(DataTable changes)
        {
            InternalApplayDirectoryChanges(DirectoryKind.dkOrganizations, null, changes);
        }

        public void ApplayTasksTypesChanges(DataTable changes)
        {
            ApplayTasksTypesChanges(null, changes);
        }

        public void ApplayTasksTypesChanges(IDatabase db, DataTable changes)
        {
            if ((changes == null) || (changes.Rows.Count == 0))
                return;
            lock (syncObjects)
            {
                // регистрируем/разрегистриуем объекты системы типа "Тип задачи"
                foreach (DataRow row in changes.Rows)
                {
                    string objName = String.Empty;
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            objName = Convert.ToString(row["ID"]) + '_' + SysObjectsTypes.TaskType.ToString();
                            RegisterSystemObject(db, objName, Convert.ToString(row["Name"]), SysObjectsTypes.TaskType);
                            break;
                        case DataRowState.Deleted:
                            objName = Convert.ToString(row["ID", DataRowVersion.Original]) + '_' +
                                      SysObjectsTypes.TaskType.ToString();
                            UnregisterSystemObject(objName);
                            break;
                    }
                }

                InternalApplayDirectoryChanges(DirectoryKind.dkTasksTypes, db, changes);
                // обновляем иерерхию в таблице объектов
                RefreshHierarchyColumn(ObjectsTable);
            }
        }

        public void ApplayMembershipChanges(int mainID, DataTable changes, bool isUsers)
        {
            if ((changes == null) || (changes.Rows.Count == 0))
                return;

            lock (syncMemberships)
            {
                // синхронизируем состояние внутренней таблицы членства с переданными изменениями
                foreach (DataRow row in changes.Rows)
                {
                    // строка может быть только изменена
                    switch (row.RowState)
                    {
                        case DataRowState.Modified:
                            // получаем ID из строки (это может быть и группа и пользователь)
                            int rowID = Convert.ToInt32(row["ID"]);
                            // получаем значение интересующей нас колонки (членства)
                            bool isMember = Convert.ToBoolean(row["ISMEMBER"]);
                            // получаем фактическое состояние
                            DataRow[] membership = FindMembership(mainID, rowID, isUsers);
                            // если найдено более одной записи - это ошибка
                            if (membership.Length > 1)
                                throw new Exception("обнаружены дублирующиеся вхождения");
                            // в зависимости от изменений обновляем фактическое состояние
                            if (isMember)
                            {
                                // только если такого вхождения еще нет - добавляем
                                if (membership.Length == 0)
                                {
                                    int membershipID = 0;
                                    Database db = (Database) _scheme.SchemeDWH.DB;
                                    try
                                    {
                                        membershipID = db.GetGenerator("g_MEMBERSHIPS");
                                    }
                                    finally
                                    {
                                        db.Dispose();
                                    }
                                    DataRow newRow = MembershipsTable.Rows.Add();
                                    newRow["ID"] = membershipID;
                                    if (isUsers)
                                    {
                                        newRow["REFUSERS"] = mainID;
                                        newRow["REFGROUPS"] = rowID;
                                    }
                                    else
                                    {
                                        newRow["REFUSERS"] = rowID;
                                        newRow["REFGROUPS"] = mainID;
                                    }
                                }
                            }
                            else
                            {
                                // если вхождение есть - удаляем (помечаем как удаленную)
                                if (membership.Length == 1)
                                    membership[0].Delete();
                            }
                            break;
                        default:
                            throw new Exception("Недопустимое состояние строки: " + row.RowState.ToString());
                    }
                }
                // сохраняем изменения в базе
                DataTable membershipsChanges = MembershipsTable.GetChanges();
                if ((membershipsChanges != null) && (membershipsChanges.Rows.Count > 0))
                {
                    DataUpdater du = GetMembershipUpdater();
                    try
                    {
                        DataTable changesDublicate = membershipsChanges.GetChanges();
                        du.Update(ref membershipsChanges);
                        WriteAdministrationTableChangeEvent(UsersOperationEventKind.uoeChangeMembershipsTable,
                                                            changesDublicate);
                        MembershipsTable.AcceptChanges();
                    }
                    catch
                    {
                        MembershipsTable.RejectChanges();
                        throw;
                    }
                    finally
                    {
                        if (du != null)
                            du.Dispose();
                    }
                }
            }
        }

        private void WritePermissionTableChangeEvent(bool added, int objID, int objectType, int idUserGroup, 
            bool isUsers, int operation)
        {
            lock (syncPermissions)
            {
                string nameUserGroup;
                string specifier;

                if (isUsers)
                {
                    specifier = "пользователя";
                    nameUserGroup = Users[idUserGroup].Name;
                }
                else
                {
                    specifier = "группы";
                    nameUserGroup = Groups[idUserGroup].Name;
                }

                string objectName;
                if (objectType == (int) SysObjectsTypes.Task || objectType == (int) SysObjectsTypes.Template)
                {
                    objectName = String.Format("Задача (ID = {0})", objID);
                }
                else if (objectType == (int) SysObjectsTypes.Template)
                {
                    objectName = String.Format("Шаблон (ID = {0})", objID);
                }
                else
                {
                    objectName = Convert.ToString(ObjectsTable.Select(String.Format("ID={0}", objID))[0]["CAPTION"]);
                }
                string operationCaption = GetCaptionForOperation(operation);
                string action = "Добавлено";
                if (!added)
                    action = "Удалено";

                string eventMsg =
                    String.Format("{0} разрешение на выполнение операции '{1}' над объектом '{2}' для {3} '{4}'",
                                  action, operationCaption, objectName, specifier, nameUserGroup);

                using (
                    IUsersOperationProtocol protocol =
                        (IUsersOperationProtocol) _scheme.GetProtocol("Krista.FM.Server.UsersManager.dll"))
                {
                    LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
                    string userHost = callerContext["Host"].ToString();
                    protocol.WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeChangePermissionsTable,
                                                                  eventMsg, userHost);
                }
            }
        }

        private void ApplayPermissionsChanges(int objectID, int objectType, bool isUsers, DataTable changes)
        {
            if ((changes == null) || (changes.Rows.Count == 0))
                return;

            lock (syncPermissions)
            {
                SysObjectsTypes st = (SysObjectsTypes) objectType;
                string[] allowedActions = TypesHelper.GetOperationsForType(st);

                string filter = String.Empty;

                // если тип объекта - задача, проверяем была ли она зарегистрирована в системе
                int taskID = 0;
                if (objectType == (int) SysObjectsTypes.Task)
                    taskID = (int) GetSystemObjectID(objectID, true, "Задача", SysObjectsTypes.Task);
                else if (objectType == (int) SysObjectsTypes.Template)
                    taskID = (int) GetSystemObjectID(objectID, true, "Шаблон", SysObjectsTypes.Template);

                // формируем фильтр на объект
                string objectFilter = "(REFOBJECTS = {0})";
                if (objectType == (int) SysObjectsTypes.Task || objectType == (int) SysObjectsTypes.Template)
                    objectFilter = String.Format(objectFilter, taskID);
                else
                    objectFilter = String.Format(objectFilter, objectID);

                // синхронизируем состояние внутренней таблицы членства с переданными изменениями
                foreach (DataRow row in changes.Rows)
                {
                    // строка может быть только изменена
                    switch (row.RowState)
                    {
                        case DataRowState.Modified:
                            // проверяем все допустимые операции
                            for (int i = 0; i < allowedActions.Length; i++)
                            {
                                // получаем новое состояние операции
                                bool actionEnabled = false;
                                try
                                {
                                    actionEnabled = Convert.ToBoolean(row[allowedActions[i]]);
                                }
                                catch
                                {
                                    throw new Exception("Обнаружена не поддерживаемая операция: " + allowedActions[i]);
                                }

                                // получаем ID пользователя/группы
                                int ID = Convert.ToInt32(row["ID"]);
                                // получаем текущее состояние операции
                                if (isUsers)
                                {
                                    filter = objectFilter + String.Format(
                                        " and (ALLOWEDACTION = {0}) and (REFUSERS = {1})",
                                        allowedActions[i], ID
                                                                );
                                }
                                else
                                {
                                    filter = objectFilter + String.Format(
                                        " and (ALLOWEDACTION = {0}) and (REFGROUPS = {1})",
                                        allowedActions[i], ID
                                                                );
                                }

                                DataRow[] permission = PermissionsTable.Select(filter);
                                switch (permission.Length)
                                {
                                        // разрешения не было
                                    case 0:
                                        // если теперь оно есть - добавляем в таблицу
                                        if (actionEnabled)
                                        {
                                            int newID;
                                            using (Database db = (Database) _scheme.SchemeDWH.DB)
                                            {
                                                newID = db.GetGenerator("g_PERMISSIONS");
                                            }
                                            DataRow newPermission = PermissionsTable.Rows.Add();
                                            newPermission["ID"] = newID;
                                            if (objectType == (int) SysObjectsTypes.Task ||
                                                objectType == (int) SysObjectsTypes.Template)
                                                newPermission["REFOBJECTS"] = taskID;
                                            else
                                                newPermission["REFOBJECTS"] = objectID;

                                            newPermission["ALLOWEDACTION"] = Convert.ToInt32(allowedActions[i]);
                                            if (isUsers)
                                                newPermission["REFUSERS"] = ID;
                                            else
                                                newPermission["REFGROUPS"] = ID;
                                            // пишем в лог
                                            WritePermissionTableChangeEvent(true, objectID, objectType, ID, isUsers,
                                                                            Convert.ToInt32(allowedActions[i]));
                                        }
                                        break;
                                        // разрешение было
                                    case 1:
                                        // если теперь его нет - удаляем из таблицы
                                        if (!actionEnabled)
                                        {
                                            permission[0].Delete();
                                            // пишем в лог
                                            WritePermissionTableChangeEvent(false, objectID, objectType, ID, isUsers,
                                                                            Convert.ToInt32(allowedActions[i]));
                                        }
                                        break;
                                    default:
                                        throw new Exception(
                                            "Обнаружено несколько разрешений для одинакового набора параметров: " +
                                            filter);
                                }
                            }
                            break;
                        default:
                            throw new Exception("Недопустимое состояние строки: " + row.RowState.ToString());
                    }
                }

                // синхронизируем изменения с базой (если действительно что-то изменилось)
                FlushPermissionsChangesToDB();

                // для задач проверить наличие других разрешений
                // если ничего не осталось - разрегистрировать объект
                if (objectType == (int) SysObjectsTypes.Task || objectType == (int) SysObjectsTypes.Template)
                {
                    filter = String.Format("REFOBJECTS = {0}", taskID);
                    DataRow[] taskpermissions = PermissionsTable.Select(filter);
                    if (taskpermissions.Length == 0)
                        UnregisterSystemObject(objectID.ToString());
                }
            }
        }

        public void ApplayUsersPermissionsChanges(int objectID, int objectType, DataTable changes)
        {
            ApplayPermissionsChanges(objectID, objectType, true, changes);
        }

        public void ApplayGroupsPermissionsChanges(int objectID, int objectType, DataTable changes)
        {
            ApplayPermissionsChanges(objectID, objectType, false, changes);
        }

        #endregion
    }
}