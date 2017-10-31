using System;
using System.Collections;
using System.Text;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Users
{
    /// <summary>
    /// Методы связанные с задачами
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {

        /// <summary>
        /// Получить ID системного объекта соответствующего шаблону/задаче.
        /// </summary>
        /// <param name="objectID">ID объекта (из таблицы шаблонов/задач).</param>
        /// <returns>-1 если объект не найден, ID объекта в противном случае</returns>
        /// <param name="sysObjectType">Тип объекта.</param>
        public int GetSystemObjectID(int objectID, SysObjectsTypes sysObjectType)
        {
            int? systemId = GetSystemObjectID(objectID, false, string.Empty, sysObjectType);
            return systemId ?? -1;
        }

		/// <summary>
		/// Получить ID системного объекта соответствующего шаблону/задаче.
		/// </summary>
		/// <param name="objectID">ID объекта (из таблицы шаблонов/задач).</param>
		/// <param name="addIfNotFound">если объекта нет - добавить.</param>
		/// <returns>null если объект не найден и не добавлен, ID объекта в противном случае</returns>
		/// <param name="objectName">Имя объекта.</param>
		/// <param name="sysObjectType">Тип объекта.</param>
		private int? GetSystemObjectID(int objectID, bool addIfNotFound, string objectName, SysObjectsTypes sysObjectType)
		{
			int? objID = null;
			// объекты-шаблоны называются по ID, поэтому делаем соответсвующий фильтр
			string filter = String.Format("NAME = '{0}'", objectID);
			// фильтруем таблицу объектов
			DataRow[] objects = ObjectsTable.Select(filter);
			if (objects.Length == 0)
			{
				// если объект не найден и запрошено его создание - добавляем новый системный объект
				if (addIfNotFound)
					objID = RegisterSystemObject(objectID.ToString(), objectName, sysObjectType);
			}
			else
				// если объект уже есть - возвращаем его ID
				objID = Convert.ToInt32(objects[0]["ID"]);
			return objID;
		}

		public bool HasUserPermissionForOperation(int userID, int operationCode)
        {
            string fullFilter = GetUserFilterOnPermissionTableForOperation(userID, true, operationCode);
            DataRow[] permissions = PermissionsTable.Select(fullFilter);
            return permissions.Length > 0;
        }

        private ArrayList ConvertTaskTypesObjectsRowsToArray(DataRow[] taskTypesObjectsRows)
        {
            ArrayList taskTypesIds = new ArrayList();
            for (int i = 0; i < taskTypesObjectsRows.Length; i++)
            {
                string objName = Convert.ToString(taskTypesObjectsRows[i]["Name"]);
                int sepInd = objName.IndexOf('_');
                objName = objName.Substring(0, sepInd);
                //и помещаем их в список
                taskTypesIds.Add(Convert.ToInt32(objName));
            }
            return taskTypesIds;
        }

        private ArrayList GetTaskTypesIdsWithAllowedOperation(TaskTypeOperations op, int userID)
        {
            string fullFilter = String.Empty;
            // если есть разрешение на операцию родительского типа - возвращаем все типы задач
            if (CheckTaskTypePermissionForTask(userID, -1, op))
            {
                fullFilter = String.Format("OBJECTTYPE = {0}", (int)SysObjectsTypes.TaskType);
                return ConvertTaskTypesObjectsRowsToArray(ObjectsTable.Select(fullFilter));
            }

            // формируем фильтр на разрешенные пользователю операции с учетом прав групп
            fullFilter = GetUserFilterOnPermissionTableForOperation(userID, true, (int)op);
            // после фильтрации получим типы объектов над которыми пользователь может выполнять операцию
            DataRow[] permissions = PermissionsTable.Select(fullFilter);
            // если ничего не найдено - сразу выходим
            if (permissions.Length == 0)
                return null;
            // теперь по ID системных объектов формируем фильтр для выборки из ObjectsTable
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < permissions.Length; i++)
            {
                sb.Append(String.Format("(ID = {0})", permissions[i]["REFOBJECTS"]));
                if (i != permissions.Length - 1)
                    sb.Append("or");
            }
            fullFilter = sb.ToString();
            // получаем список объектов-типов задач
            DataRow[] objects = ObjectsTable.Select(fullFilter);
            // конвертируем в список ID
            return ConvertTaskTypesObjectsRowsToArray(objects);
        }

        public ArrayList GetUserVisibleTaskTypes(int userID)
        {
            return GetTaskTypesIdsWithAllowedOperation(TaskTypeOperations.ViewAllUsersTasks, userID);
        }

        public ArrayList GetUserCreatableTaskTypes(int userID)
        {
            return GetTaskTypesIdsWithAllowedOperation(TaskTypeOperations.CreateTask, userID);
        }

        public bool CurrentUserCanCreateTasks()
        {
            ArrayList creatableTypesIds = GetUserCreatableTaskTypes(this.GetCurrentUserID());
            return ((creatableTypesIds != null) && (creatableTypesIds.Count > 0));
        }

        public ArrayList GetUserVisibleTasks(int userID)
        {
            ArrayList tasksID = new ArrayList();
            string fullFilter = GetUserFilterOnPermissionTableForOperation(userID, true,
                (int)TaskOperations.View);
            DataRow[] permissions = PermissionsTable.Select(fullFilter);
            // теперь по ID системных объектов формируем фильтр для выборки из ObjectsTable
            foreach (DataRow permissionRow in permissions)
            {
                string filter = String.Format("(ID = {0})", permissionRow["REFOBJECTS"]);
                DataRow[] objects = ObjectsTable.Select(filter);
                for (int j = 0; j < objects.Length; j++)
                {
                    int taskID = Convert.ToInt32(objects[j]["Name"]);
                    //и помещаем их в список
                    tasksID.Add(taskID);
                }
            }
            return tasksID;
        }

        // задачи являются особыми системными объектами и права на них проверяются несколько необычно
        public bool CheckAllTasksPermissionForTask(int userID, AllTasksOperations operation)
        {
            int allTasksObjectID = GetSystemObjectIDByName(SysObjectsTypes.AllTasks.ToString());
            string filter = String.Format("(REFOBJECTS = {0}) and ({1})",
                allTasksObjectID,
                GetUserFilterOnPermissionTableForOperation(userID, true, (int)operation)
            );
            DataRow[] permission = PermissionsTable.Select(filter);
            return permission.Length > 0;
        }

        private bool CheckTaskTypePermissionForTask(int userID, int taskType, TaskTypeOperations operation)
        {
            bool succeded = false;

            // сначала проверим разрешения на родительские операции
            switch (operation)
            {
                case TaskTypeOperations.CreateTask:
                    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.CreateTask);
                    break;
                case TaskTypeOperations.ChangeTaskHierarchyOrder:
                    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.ChangeTaskHierarchyOrder);
                    break;
                case TaskTypeOperations.DelTaskAction:
                    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.DelTaskAction);
                    break;
                case TaskTypeOperations.EditTaskAction:
                    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.EditTaskAction);
                    break;
                case TaskTypeOperations.ViewAllUsersTasks:
                    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.ViewAllUsersTasks);
                    break;
                case TaskTypeOperations.AssignTaskViewPermission:
                    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.AssignTaskViewPermission);
                    break;
                case TaskTypeOperations.CanCancelEdit:
                    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.CanCancelEdit);
                    break;
				//case TaskTypeOperations.ExportTask:
				//    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.ExportTask);
				//    break;
				//case TaskTypeOperations.ImportTask:
				//    succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.ImportTask);
				//    break;
            }

            if (succeded)
                return true;

            // если тип задачи не указан - возвращаем false
            if (taskType == -1)
                return false;

            // если нет разрешения на родительский тип - проверяем свое разрешение
            string taskTypeObjectName = taskType.ToString() + "_TaskType";
            int taskTypeObjectID = GetSystemObjectIDByName(taskTypeObjectName);
            string filter = String.Format("(REFOBJECTS = {0}) and ({1})",
                taskTypeObjectID,
                GetUserFilterOnPermissionTableForOperation(userID, true, (int)operation)
            );
            DataRow[] permission = PermissionsTable.Select(filter);
            return permission.Length > 0;
        }

        private bool CheckTaskPermissionForTask(int userID, int taskID, int taskType, TaskOperations operation)
        {
            bool succeeded = false;
            // cначала проверим разрешения для всех задач и типа задач
            switch (operation)
            {
                case TaskOperations.View:
                    succeeded = CheckTaskTypePermissionForTask(userID, taskType, TaskTypeOperations.ViewAllUsersTasks);
                    break;
            }

            if (succeeded)
                return true;

            // теперь проверим свои разрешения 
            string taskName = taskID.ToString();
            try
            {
                int taskObjectID = GetSystemObjectIDByName(taskName);
                string filter = String.Format("(REFOBJECTS = {0}) and ({1})",
                    taskObjectID,
                    GetUserFilterOnPermissionTableForOperation(userID, true, (int)operation)
                );
                DataRow[] permission = PermissionsTable.Select(filter);
                return permission.Length > 0;
            }
            catch
            {
                // задача может быть не зарегистрирована в системе - в этом случае никаких прав на нее не выделено
                return false;
            }
        }

        private bool CheckPermissionForTask(int userID, int taskID, int taskType, int operation)
        {
            string operationCaption = String.Empty;
            int? parentOperation = null;
            Type operationType = null;
            TypesHelper.GetOperationInfo(operation, ref operationCaption, ref operationType, ref parentOperation);
            switch (operationType.Name)
            {
                case "AllTasksOperations":
                    return CheckAllTasksPermissionForTask(userID, (AllTasksOperations)operation);
                case "TaskTypeOperations":
                    if (operation == (int)TaskTypeOperations.DelTaskAction)
                    {
                        DataTable dt = this._scheme.TaskManager.Tasks.GetTasksInfo();
                        if (dt.Select(string.Format("ID = {0}", taskID)).Length > 0)
                        {
                            ITask task = this._scheme.TaskManager.Tasks[taskID];
                            if (task.Owner == userID)
                                return true;
                        }
                        else
                            return true;
                    }
                    return CheckTaskTypePermissionForTask(userID, taskType, (TaskTypeOperations)operation);
                case "TaskOperations":
                    return CheckTaskPermissionForTask(userID, taskID, taskType, (TaskOperations)operation);
                default:
                    throw new Exception(String.Format("Метод не может быть вызван для операция типа '{0}'", operationType.ToString())); ;
            }
        }

        public bool CheckPermissionForTask(int taskID, int taskType, int operation, bool raiseException)
        {
            int userID = this.GetCurrentUserID();
            bool hasPermission = CheckPermissionForTask(userID, taskID, taskType, operation);

            if ((!hasPermission) && (raiseException))
            {
                string operationName = this.GetCaptionForOperation(operation);
                string userName = this.GetCurrentUserName();
                string taskName = String.Format("Задача (ID = {0})", taskID);
                throw new PermissionException(userName, taskName, operationName, "Доступ к задаче запрещен");
            }

            return hasPermission;
        }
    }
}