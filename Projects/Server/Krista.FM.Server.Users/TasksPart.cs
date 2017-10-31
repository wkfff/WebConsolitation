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
    /// ������ ��������� � ��������
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {

        /// <summary>
        /// �������� ID ���������� ������� ���������������� �������/������.
        /// </summary>
        /// <param name="objectID">ID ������� (�� ������� ��������/�����).</param>
        /// <returns>-1 ���� ������ �� ������, ID ������� � ��������� ������</returns>
        /// <param name="sysObjectType">��� �������.</param>
        public int GetSystemObjectID(int objectID, SysObjectsTypes sysObjectType)
        {
            int? systemId = GetSystemObjectID(objectID, false, string.Empty, sysObjectType);
            return systemId ?? -1;
        }

		/// <summary>
		/// �������� ID ���������� ������� ���������������� �������/������.
		/// </summary>
		/// <param name="objectID">ID ������� (�� ������� ��������/�����).</param>
		/// <param name="addIfNotFound">���� ������� ��� - ��������.</param>
		/// <returns>null ���� ������ �� ������ � �� ��������, ID ������� � ��������� ������</returns>
		/// <param name="objectName">��� �������.</param>
		/// <param name="sysObjectType">��� �������.</param>
		private int? GetSystemObjectID(int objectID, bool addIfNotFound, string objectName, SysObjectsTypes sysObjectType)
		{
			int? objID = null;
			// �������-������� ���������� �� ID, ������� ������ �������������� ������
			string filter = String.Format("NAME = '{0}'", objectID);
			// ��������� ������� ��������
			DataRow[] objects = ObjectsTable.Select(filter);
			if (objects.Length == 0)
			{
				// ���� ������ �� ������ � ��������� ��� �������� - ��������� ����� ��������� ������
				if (addIfNotFound)
					objID = RegisterSystemObject(objectID.ToString(), objectName, sysObjectType);
			}
			else
				// ���� ������ ��� ���� - ���������� ��� ID
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
                //� �������� �� � ������
                taskTypesIds.Add(Convert.ToInt32(objName));
            }
            return taskTypesIds;
        }

        private ArrayList GetTaskTypesIdsWithAllowedOperation(TaskTypeOperations op, int userID)
        {
            string fullFilter = String.Empty;
            // ���� ���� ���������� �� �������� ������������� ���� - ���������� ��� ���� �����
            if (CheckTaskTypePermissionForTask(userID, -1, op))
            {
                fullFilter = String.Format("OBJECTTYPE = {0}", (int)SysObjectsTypes.TaskType);
                return ConvertTaskTypesObjectsRowsToArray(ObjectsTable.Select(fullFilter));
            }

            // ��������� ������ �� ����������� ������������ �������� � ������ ���� �����
            fullFilter = GetUserFilterOnPermissionTableForOperation(userID, true, (int)op);
            // ����� ���������� ������� ���� �������� ��� �������� ������������ ����� ��������� ��������
            DataRow[] permissions = PermissionsTable.Select(fullFilter);
            // ���� ������ �� ������� - ����� �������
            if (permissions.Length == 0)
                return null;
            // ������ �� ID ��������� �������� ��������� ������ ��� ������� �� ObjectsTable
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < permissions.Length; i++)
            {
                sb.Append(String.Format("(ID = {0})", permissions[i]["REFOBJECTS"]));
                if (i != permissions.Length - 1)
                    sb.Append("or");
            }
            fullFilter = sb.ToString();
            // �������� ������ ��������-����� �����
            DataRow[] objects = ObjectsTable.Select(fullFilter);
            // ������������ � ������ ID
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
            // ������ �� ID ��������� �������� ��������� ������ ��� ������� �� ObjectsTable
            foreach (DataRow permissionRow in permissions)
            {
                string filter = String.Format("(ID = {0})", permissionRow["REFOBJECTS"]);
                DataRow[] objects = ObjectsTable.Select(filter);
                for (int j = 0; j < objects.Length; j++)
                {
                    int taskID = Convert.ToInt32(objects[j]["Name"]);
                    //� �������� �� � ������
                    tasksID.Add(taskID);
                }
            }
            return tasksID;
        }

        // ������ �������� ������� ���������� ��������� � ����� �� ��� ����������� ��������� ��������
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

            // ������� �������� ���������� �� ������������ ��������
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

            // ���� ��� ������ �� ������ - ���������� false
            if (taskType == -1)
                return false;

            // ���� ��� ���������� �� ������������ ��� - ��������� ���� ����������
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
            // c������ �������� ���������� ��� ���� ����� � ���� �����
            switch (operation)
            {
                case TaskOperations.View:
                    succeeded = CheckTaskTypePermissionForTask(userID, taskType, TaskTypeOperations.ViewAllUsersTasks);
                    break;
            }

            if (succeeded)
                return true;

            // ������ �������� ���� ���������� 
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
                // ������ ����� ���� �� ���������������� � ������� - � ���� ������ ������� ���� �� ��� �� ��������
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
                    throw new Exception(String.Format("����� �� ����� ���� ������ ��� �������� ���� '{0}'", operationType.ToString())); ;
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
                string taskName = String.Format("������ (ID = {0})", taskID);
                throw new PermissionException(userName, taskName, operationName, "������ � ������ ��������");
            }

            return hasPermission;
        }
    }
}