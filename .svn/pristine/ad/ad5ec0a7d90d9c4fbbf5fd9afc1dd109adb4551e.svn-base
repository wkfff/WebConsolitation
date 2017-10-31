using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Users
{
	/// <summary>
	/// Методы связанные с шаблонами.
	/// </summary>
	public sealed partial class UsersManager
	{
		private ArrayList ConvertTemplateTypesObjectsRowsToArray(DataRow[] templateTypesObjectsRows)
		{
			ArrayList taskTypesIds = new ArrayList();
			for (int i = 0; i < templateTypesObjectsRows.Length; i++)
			{
				string objName = Convert.ToString(templateTypesObjectsRows[i]["Name"]);
				int sepInd = objName.IndexOf('_');
				objName = objName.Substring(0, sepInd);
				//и помещаем их в список
				taskTypesIds.Add(Convert.ToInt32(objName));
			}
			return taskTypesIds;
		}

		public ArrayList GetTemplateTypesIdsWithAllowedOperation(TemplateTypeOperations op, int userID)
		{
			string fullFilter;
			// если есть разрешение на операцию родительского типа - возвращаем все типы задач
			if (CheckTemplateTypePermissionForTemplate(userID, -1, op))
			{
				fullFilter = String.Format("OBJECTTYPE = {0}", (int)SysObjectsTypes.TemplateType);
				return ConvertTemplateTypesObjectsRowsToArray(ObjectsTable.Select(fullFilter));
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
			return ConvertTemplateTypesObjectsRowsToArray(objects);
		}

		public ArrayList GetUserVisibleTemplateTypes(int userID)
		{
			return GetTemplateTypesIdsWithAllowedOperation(TemplateTypeOperations.ViewAllUsersTemplates, userID);
		}

		public ArrayList GetUserCreatableTemplateTypes(int userID)
		{
			return GetTemplateTypesIdsWithAllowedOperation(TemplateTypeOperations.CreateTemplate, userID);
		}

		public bool CurrentUserCanCreateTemplates()
		{
			ArrayList creatableTypesIds = GetUserCreatableTemplateTypes(GetCurrentUserID());
			return ((creatableTypesIds != null) && (creatableTypesIds.Count > 0));
		}

		public ArrayList GetUserVisibleTemplates(int userID)
		{
            ArrayList tasksID = new ArrayList();
            string fullFilter = GetUserFilterOnPermissionTableForOperation(userID, true,
                (int)TemplateOperations.ViewTemplate);
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
		public bool CheckAllTemplatesPermissionForTemplate(int userID, AllTemplatesOperations operation)
		{
			int allTemplatesObjectID = GetSystemObjectIDByName(SysObjectsTypes.AllTemplates.ToString());
			string filter = String.Format("(REFOBJECTS = {0}) and ({1})",
				allTemplatesObjectID,
				GetUserFilterOnPermissionTableForOperation(userID, true, (int)operation)
			);
			DataRow[] permission = PermissionsTable.Select(filter);
			return permission.Length > 0;
		}

		public bool CheckTemplateTypePermissionForTemplate(int userID, int taskType, TemplateTypeOperations operation)
		{
			bool succeded = false;

			// сначала проверим разрешения на родительские операции
			switch (operation)
			{
				case TemplateTypeOperations.CreateTemplate:
					succeded = CheckAllTemplatesPermissionForTemplate(userID, AllTemplatesOperations.CreateTemplate);
					break;
				case TemplateTypeOperations.ChangeTemplateHierarchyOrder:
					succeded = CheckAllTemplatesPermissionForTemplate(userID, AllTemplatesOperations.ChangeTemplateHierarchyOrder);
					break;
				case TemplateTypeOperations.EditTemplateAction:
					succeded = CheckAllTemplatesPermissionForTemplate(userID, AllTemplatesOperations.EditTemplateAction);
					break;
				case TemplateTypeOperations.ViewAllUsersTemplates:
					succeded = CheckAllTemplatesPermissionForTemplate(userID, AllTemplatesOperations.ViewAllUsersTemplates);
					break;
				case TemplateTypeOperations.AssignTemplateViewPermission:
					succeded = CheckAllTemplatesPermissionForTemplate(userID, AllTemplatesOperations.AssignTemplateViewPermission);
					break;
				/*case TaskTypeOperations.ExportTask:
					succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.ExportTask);
					break;
				case TaskTypeOperations.ImportTask:
					succeded = CheckAllTasksPermissionForTask(userID, AllTasksOperations.ImportTask);
					break;*/
			}

			if (succeded)
				return true;

			// если тип задачи не указан - возвращаем false
			if (taskType == -1)
				return false;

			// если нет разрешения на родительский тип - проверяем свое разрешение
			string taskTypeObjectName = taskType.ToString() + "_TemplateType";
			int taskTypeObjectID = GetSystemObjectIDByName(taskTypeObjectName);
			string filter = String.Format("(REFOBJECTS = {0}) and ({1})",
				taskTypeObjectID,
				GetUserFilterOnPermissionTableForOperation(userID, true, (int)operation)
			);
			DataRow[] permission = PermissionsTable.Select(filter);
			return permission.Length > 0;
		}

		private bool CheckTemplatePermissionForTemplate(int userID, int taskID, int taskType, TemplateOperations operation)
		{
			bool succeeded = false;
			// cначала проверим разрешения для всех задач и типа задач
			switch (operation)
			{
				case TemplateOperations.ViewTemplate:
					succeeded = CheckTemplateTypePermissionForTemplate(userID, taskType, TemplateTypeOperations.ViewAllUsersTemplates);
					break;
				case TemplateOperations.EditTemplateAction:
					succeeded = CheckTemplateTypePermissionForTemplate(userID, taskType, TemplateTypeOperations.EditTemplateAction);
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

		private bool CheckPermissionForTemplate(int userID, int taskID, int taskType, int operation)
		{
			string operationCaption = String.Empty;
			int? parentOperation = null;
			Type operationType = null;
			TypesHelper.GetOperationInfo(operation, ref operationCaption, ref operationType, ref parentOperation);
			switch (operationType.Name)
			{
				case "AllTemplatesOperations":
					return CheckAllTemplatesPermissionForTemplate(userID, (AllTemplatesOperations)operation);
				case "TemplateTypeOperations":
					return CheckTemplateTypePermissionForTemplate(userID, taskType, (TemplateTypeOperations)operation);
				case "TemplateOperations":
					return CheckTemplatePermissionForTemplate(userID, taskID, taskType, (TemplateOperations)operation);
				default:
					throw new Exception(String.Format("Метод не может быть вызван для операция типа '{0}'", operationType.ToString())); ;
			}
		}

		public bool CheckPermissionForTemplate(int taskID, int taskType, int operation, bool raiseException)
		{
			int userID = this.GetCurrentUserID();
			bool hasPermission = CheckPermissionForTemplate(userID, taskID, taskType, operation);

			if ((!hasPermission) && (raiseException))
			{
				string operationName = this.GetCaptionForOperation(operation);
				string userName = this.GetCurrentUserName();
				string taskName = String.Format("Шаблон (ID = {0})", taskID);
				throw new PermissionException(userName, taskName, operationName, "Доступ к шаблону запрещен");
			}

			return hasPermission;
		}


		/// <summary>
		/// Проверяет права пользователя или группы в которую он входит на проведение операции
		/// для дочерних операций подмножества ScenForecastOperations (для сценариев и вариантов )
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="taskType"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public Boolean CheckScenForecastPermission(Int32 userID, String taskType, ScenForecastOperations operation)
		{
			Boolean succeeded = false;
			// сначала проверим разрешения на родительские операции
			switch (operation)
			{
				case ScenForecastOperations.AssignParam:
					succeeded = CheckAllForecastPermission(userID, ForecastOperations.AssignParam);
					break;
				case ScenForecastOperations.Calculate:
					succeeded = CheckAllForecastPermission(userID, ForecastOperations.Calculate);
					break;
				case ScenForecastOperations.CreateNew:
					succeeded = CheckAllForecastPermission(userID, ForecastOperations.CreateNew);
					break;
			}

			if (succeeded)
				return true;
			
			// если нет разрешения на родительский тип - проверяем свое разрешение
			String taskTypeObjectName = String.Format("Forecast_{0}", taskType);
			Int32 taskTypeObjectID = GetSystemObjectIDByName(taskTypeObjectName);
			return CheckAllForecastPermission(userID, (Int32)operation, taskTypeObjectID);
		}

		/// <summary>
		/// Проверяет права пользователя или группы в которую он входит на проведение операции
		/// для родительских операций подмножества ForecastOperations
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public Boolean CheckAllForecastPermission(Int32 userID, ForecastOperations operation)
		{
			Int32 allForecastObjectID = GetSystemObjectIDByName(SysObjectsTypes.AllForecast.ToString());
			return CheckAllForecastPermission(userID, (Int32)operation, allForecastObjectID);
		}

		
		private Boolean CheckAllForecastPermission(Int32 userID, Int32 operation, Int32 objectID)
		{
			String filter = String.Format("(REFOBJECTS = {0}) and ({1})",
				objectID,
				GetUserFilterOnPermissionTableForOperation(userID, true, (Int32)operation));
			DataRow[] permission = PermissionsTable.Select(filter);
			return permission.Length > 0;
		}

		/// <summary>
		/// Проверяет права пользователя или группы в которую он входит на проведение операции
		/// для родительских операций подмножества Form2pForecastOperations
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public Boolean CheckForm2pForecastPermission(Int32 userID, Form2pForecastOperations operation)
		{
			Int32 form2pForecastObjectID = GetSystemObjectIDByName(SysObjectsTypes.Form2pForecast.ToString());
			return CheckAllForecastPermission(userID, (Int32)operation, form2pForecastObjectID);
        }
    }
}
