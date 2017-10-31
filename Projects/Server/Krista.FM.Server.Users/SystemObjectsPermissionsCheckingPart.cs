using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;


namespace Krista.FM.Server.Users
{
    /// <summary>
    /// Проверка прав на общие типы системных объектов.
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {
        private DataRow[] FindMembership(int mainID, int rowID, bool isUsers)
        {
            string filter = "(REFUSERS = {0}) and (REFGROUPS = {1})";
            if (isUsers)
                filter = String.Format(filter, mainID, rowID);
            else
                filter = String.Format(filter, rowID, mainID);

            return MembershipsTable.Select(filter);
        }

        private bool GetUserMembershipsFilter(int userID, ref string filter)
        {
            string userFilter = String.Format("REFUSERS = {0}", userID);
            DataRow[] groups = MembershipsTable.Select(userFilter);
            // если пользователь не входит ни в одну из групп - возвращаем false
            if ((groups == null) || (groups.Length == 0))
                return false;

            //иначе - формируем большой фильтр для поиска разрешения
            StringBuilder sb = new StringBuilder();
            sb.Append('(');
            for (int i = 0; i < groups.Length; i++)
            {
                sb.Append(String.Format("(REFGROUPS = {0})", Convert.ToInt32(groups[i]["REFGROUPS"])));
                if (i != groups.Length - 1)
                    sb.Append("or");
            }
            sb.Append(')');
            filter = sb.ToString();
            return true;
        }

        private List<int> GetUserMembershipsFilter(int userID)
        {
            List<int> value = new List<int>();
            string userFilter = String.Format("REFUSERS = {0}", userID);
            DataRow[] groups = MembershipsTable.Select(userFilter);
            // если пользователь не входит ни в одну из групп - возвращаем false
            if ((groups == null) || (groups.Length == 0))
                return value;
            for (int i = 0; i < groups.Length; i++)
            {
                value.Add( Convert.ToInt32(groups[i]["REFGROUPS"]));
            }
            return value;
        }

        private bool CheckPermissionForUserGroups(int userID, int objectID, int allowedAction)
        {
            List<int> groups = GetUserMembershipsFilter(userID);
            if (groups.Count == 0)
                return false;

            DataRow[] permissions =
                PermissionsTable.Select(string.Format("REFOBJECTS = {0} and ALLOWEDACTION = {1}", objectID,
                                                      allowedAction));
            foreach (DataRow row in permissions)
            {
                if (row.IsNull("REFGROUPS"))
                    continue;

                int refGroup = Convert.ToInt32(row["REFGROUPS"]);
                if (groups.Contains(refGroup))
                    return true;
            }
            return false;
        }

        private bool CheckPermission(int mainID, int objectID, int allowedAction, bool isUser)
        {
            string filter = String.Empty;
            DataRow[] permission = null;
            if (isUser)
            {
                filter = String.Format("REFUSERS = {0} and REFOBJECTS = {1} and ALLOWEDACTION = {2}", mainID, objectID, allowedAction);
                permission = PermissionsTable.Select(filter);
                switch (permission.Length)
                {
                    case 1:
                        return true;
                    case 0:
                        return CheckPermissionForUserGroups(mainID, objectID, allowedAction);
                    default:
                        throw new Exception("Обнаружено несколько разрешений для одинакового набора параметров: " + filter);
                }
            }
            else
            {
                filter = String.Format("REFGROUPS = {0} and REFOBJECTS = {1} and ALLOWEDACTION = {2}", mainID, objectID, allowedAction);
                permission = PermissionsTable.Select(filter);
                switch (permission.Length)
                {
                    case 1:
                        return true;
                    case 0:
                        return false;
                    default:
                        throw new Exception("Обнаружено несколько разрешений для одинакового набора параметров: " + filter);
                }
            }
        }

        private string GetUserFilterOnPermissionTable(int userID, bool includeGroups)
        {
            string filter = String.Format("(RefUsers = {0})", userID);
            string groupsFilter = String.Empty;
            if ((includeGroups) && (GetUserMembershipsFilter(userID, ref groupsFilter)))
                filter = String.Format("({0} or {1})", filter, groupsFilter);
            return filter;
        }

        /// <summary>
		/// Возвращает фильтр по типу (AllowedAction = operations) and ((RefUsers = UserID) or ((REFGROUPS = includedGroup[0])or(REFGROUPS = includedGroup[1]or ...)))
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="includeGroups">включать ли в фильтр группы</param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private string GetUserFilterOnPermissionTableForOperation(int userID, bool includeGroups, int operation)
        {
            string userFilter = GetUserFilterOnPermissionTable(userID, includeGroups);
            string actionFilter = String.Format("(AllowedAction = {0})", operation);
            return String.Format("{0} and {1}", actionFilter, userFilter);
        }

        private int GetSystemObjectIDByName(string objectName, ref int objectType)
        {
            string filter = String.Format("Name = '{0}'", objectName);
            DataRow[] objects = ObjectsTable.Select(filter);
            switch (objects.Length)
            {
                case 0:
                    throw new ServerException(String.Format("Объект системы '{0}' не найден", objectName));
                case 1:
                    objectType = Convert.ToInt32(objects[0]["OBJECTTYPE"]);
                    return Convert.ToInt32(objects[0]["ID"]);
                default:
					throw new ServerException(String.Format("Найдено {0} объектов системы '{1}'", objects.Length, objectName));
            }
        }

        private int GetSystemObjectIDByName(string objectName)
        {
            int objType = 0;
            return GetSystemObjectIDByName(objectName, ref objType);
        }

        public bool CheckPermissionForDataSources(int operation)
        {
            bool hasPermission = false;
            // получаем ID текущего пользователя
            int userID = this.GetCurrentUserID();
            DataRow[] selfPermission = PermissionsTable.Select(GetUserFilterOnPermissionTableForOperation(userID, true, operation));
            hasPermission = selfPermission.Length > 0;
            return hasPermission;
        }

        #region Общий метод для проверки прав для всех объектов кроме задач
        public bool CheckPermissionForSystemObject(string objectName, int operation, bool raiseException)
        {
            string filter;
            bool hasPermission = false;
            lock (syncPermissions)
            {
                // получаем ID текущего пользователя
                int userID = this.GetCurrentUserID();
                // получаем ID объекта системы
                int objType = 0;
                int objID = this.GetSystemObjectIDByName(objectName, ref objType);

                // смотрим есть ли у объекта родительский тип
                int? parentType = TypesHelper.GetParentTypeInt(objType);
                // если есть проверяем разрешения для родительского типа
                if (parentType != null)
                {
                    string operationCaption = String.Empty;
                    Type operationType = null;
                    int? parentOperation = null;
                    TypesHelper.GetOperationInfo(operation, ref operationCaption, ref operationType, ref parentOperation);
                    if (parentOperation != null)
                    {
                        int parentObjID = this.GetSystemObjectIDByName(((SysObjectsTypes) parentType).ToString());
                        filter = String.Format("(REFOBJECTS = {0}) and ({1})",
                                               parentObjID,
                                               GetUserFilterOnPermissionTableForOperation(userID, true,
                                                                                          (int) parentOperation)
                            );
                        DataRow[] parentPermission = PermissionsTable.Select(filter);
                        hasPermission = parentPermission.Length > 0;
                    }
                }
                if (!hasPermission)
                {
                    // теперь разрешения для самого объекта
                    filter = String.Format("(REFOBJECTS = {0}) and ({1})",
                                           objID,
                                           GetUserFilterOnPermissionTableForOperation(userID, true, operation)
                        );
                    DataRow[] selfPermission = PermissionsTable.Select(filter);
                    hasPermission = selfPermission.Length > 0;
                }
                // если нужно, генерируем исключение о нарушении прав доступа
                if ((!hasPermission) && (raiseException))
                {
                    string operationName = this.GetCaptionForOperation(operation);
                    string userName = this.GetCurrentUserName();
                    throw new PermissionException(userName, objectName, operationName, "Доступ запрещен");
                }
            }
            return hasPermission;
        }

        private void FlushPermissionsChangesToDB()
        {
            DataTable permissionsChanges = PermissionsTable.GetChanges();
            if ((permissionsChanges != null) && (permissionsChanges.Rows.Count > 0))
            {
				using (DataUpdater upd = GetPermissionsUpdater())
				{
					try
					{
						upd.Update(ref permissionsChanges);
						PermissionsTable.AcceptChanges();
					}
					catch (Exception e)
					{
						PermissionsTable.RejectChanges();
						throw new ServerException(e.Message, e);
					}
				}
            }
        }

        #endregion

    }
}