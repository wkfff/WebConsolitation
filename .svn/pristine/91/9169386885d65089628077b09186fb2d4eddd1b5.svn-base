using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Users
{
    #region Класс для управления пользователями и группами
    /// <summary>
    /// Класс для управления пользователями и группами
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {
        #region Базовые методы и поля класса
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // освобождаем управляемые ресурсы
                this.Clear();
            }
            // освобождаем неуправляемые ресурсы
            // ...
        }

        /// <summary>
        /// Список доступных групп
        /// </summary>
        private Dictionary<int, SysGroup> Groups = new Dictionary<int, SysGroup>();

        /// <summary>
        /// Список пользователей
        /// </summary>
        private Dictionary<int, SysUser> Users = new Dictionary<int, SysUser>();

    private DataTable PermissionsTable = new DataTable("Permissions");
        private DataTable MembershipsTable = new DataTable("Memberships");
        private DataTable ObjectsTable = new DataTable("Objects");

        // объекты для синхронизации доступа
        private static object syncGroups = new object();
        private static object syncUsers = new object();
        private static object syncPermissions = new object();
        private static object syncMemberships = new object();
        private static object syncObjects = new object();

        private IScheme _scheme;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public UsersManager(IScheme scheme)
        {
            if (scheme == null)
                throw new Exception("Необходимо задать интерфейс ISchemeDWH");

            _scheme = scheme;

            Trace.Indent();
            Trace.WriteLine("Создание коллекций", "UsersManager");
            Load();
            Trace.WriteLine("Проверка списка модулей просмотра", "UsersManager");
            RegisterUIModules();
            Trace.WriteLine("Проверка списка объектов", "UsersManager");
            RegisterGeneralObjectsTypes();
            Trace.WriteLine("Проверка прав фиксированных пользователей", "UsersManager");
            CheckFixedUsers();
            Trace.WriteLine("Проверка группы администраторов", "UsersManager");
            CheckAdministrators();
            Trace.Unindent();
        }
        #endregion

        #region Очистка коллекций
        private void ClearUsers()
        {
            lock (syncUsers)
            {
                Users.Clear();
            }
        }

        private void ClearGroups()
        {
            lock (syncGroups)
            {
                Groups.Clear();
            }
        }

        private void ClearObjects()
        {
            lock (syncObjects)
            {
                ObjectsTable.Clear();
            }
        }

        private void ClearPermissions()
        {
            lock (syncPermissions)
            {
                PermissionsTable.Clear();
            }
        }

        private void ClearMembership()
        {
            lock (syncMemberships)
            {
                MembershipsTable.Clear();
            }
        }

        private void Clear()
        {
            ClearUsers();
            ClearGroups();
            ClearObjects();
            ClearPermissions();
            ClearMembership();
        }
        #endregion

        #region Загрузка коллекций
        public void LoadUsers()
        {
            ClearUsers();
            lock (syncUsers)
            {
                using (DataUpdater du = GetUsersUpdater())
                {
                    DataTable dt = new DataTable("Users");
                    du.Fill(ref dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        SysUser user = new SysUser(row);
                        Users.Add(user.ID, user);
                    }
                    dt.Clear();
                }
            }
        }

        public void LoadGroups()
        {
            ClearGroups();
            lock (syncGroups)
            {
                using (DataUpdater du = GetGroupsUpdater())
                {
                    DataTable dt = new DataTable("Groups");
                    du.Fill(ref dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        SysGroup group = new SysGroup(row);
                        Groups.Add(group.ID, group);
                    }
                    dt.Clear();
                }
            }
        }

        private void SetParentType(DataRow row)
        {
            try
            {
                int objType = Convert.ToInt32(row["OBJECTTYPE"]);
                int? parentTypeInt = TypesHelper.GetParentTypeInt(objType);
                if (parentTypeInt != null)
                    row["REFOBJECTTYPE"] = parentTypeInt;
                else
                    row["REFOBJECTTYPE"] = DBNull.Value;
            }
            catch 
            { 
                // некритично, можно заглушить
            }
        }

        private void RefreshHierarchyColumn(DataTable dt)
        {
            // дописываем поле со ссылкой на родителя
            ObjectsTable.BeginLoadData();

            if (!dt.Columns.Contains("REFOBJECTTYPE"))
            {
                Type parentType = ObjectsTable.Columns["OBJECTTYPE"].DataType;
                DataColumn clmn = new DataColumn("REFOBJECTTYPE", parentType);
                clmn.AllowDBNull = true;
                ObjectsTable.Columns.Add(clmn);
            }

            foreach (DataRow row in ObjectsTable.Rows)
            {
                SetParentType(row);
            }
            ObjectsTable.EndLoadData();
            ObjectsTable.AcceptChanges();

        }

        public void LoadObjects()
        {
            ClearObjects();
            lock (syncObjects)
            {
                using (DataUpdater du = GetObjectsUpdater(null))
                {
                    ObjectsTable.Clear();

                    du.Fill(ref ObjectsTable);

                    RefreshHierarchyColumn(ObjectsTable);
                }
            }
        }

        public void LoadMembership()
        {
            ClearMembership();
            lock (syncMemberships)
            {
                using (DataUpdater du = GetMembershipUpdater())
                {
                    MembershipsTable.Clear();
                    du.Fill(ref MembershipsTable);
                }
            }
        }

        public void LoadPermissions()
        {
            ClearPermissions();
            lock (syncPermissions)
            {
                using (DataUpdater du = GetPermissionsUpdater())
                {
                    PermissionsTable.Clear();
                    du.Fill(ref PermissionsTable);
                }
            }
        }

        /// <summary>
        /// Загрузка объектов из базы
        /// </summary>
        public void Load()
        {
            Clear();
            LoadUsers();
            LoadGroups();
            LoadMembership();
            LoadObjects();
            LoadPermissions();
        }
        #endregion

		/// <summary>
		/// Возвращает массив названий навигационных объектов 
		/// доступных текукущему пользователю.
		/// </summary>
		public string[] GetViewObjectsNamesAllowedForCurrentUser()
        {
            // получаем ID текущего пользователя
            int curUser = this.GetCurrentUserID();
            // получаем список всех объектов просмотра
            Dictionary<string, string> allUIObjects = this.GetUIModulesList(true);
			List<string> allowedObjectsArr = new List<string>();
            // если текущий пользователь - фиксированный администратор, ему доступен только интерфейс администрирования
            if (curUser == (int)FixedUsers.FixedUsersIds.InstallAdmin)
            {
                allowedObjectsArr.Add(allUIObjects["AdministrationUI"]);
            }
            else
            {
                foreach (KeyValuePair<string, string> uiModule in allUIObjects)
                {
                    // если пользователь добавлен в группу администраторов или web-администраторов, то ему всегда доступен интерфейс администрирования
                    if (uiModule.Key == "AdministrationUI")
                    {
                        if (CurrentUserIsAdmin() || CurrentUserIsWebAdmin())
                            allowedObjectsArr.Add(uiModule.Value);
                    }
                    else
                    {
                        // для классификаторов и таблиц проверяем права на каждую часть интерфейса
                        if (string.Compare(uiModule.Key, "EntityNavigationListUI") == 0)
                        {
                            if (CheckPermissionForSystemObject(uiModule.Key, (int)EntityNavigationListUI.Display, false))
                                allowedObjectsArr.Add(uiModule.Value);
                            else
                            {
                                if (CheckPermissionForSystemObject("FixedClassifiers", (int)UIClassifiersSubmoduleOperation.Display, false) || 
                                    CheckPermissionForSystemObject("DataClassifiers", (int)UIClassifiersSubmoduleOperation.Display, false) ||
                                    CheckPermissionForSystemObject("AssociatedClassifiers", (int)UIClassifiersSubmoduleOperation.Display, false) ||
                                    CheckPermissionForSystemObject("FactTables", (int)UIClassifiersSubmoduleOperation.Display, false) ||
                                    CheckPermissionForSystemObject("Associations", (int)UIClassifiersSubmoduleOperation.Display, false) ||
                                    CheckPermissionForSystemObject("TranslationTables", (int)UIClassifiersSubmoduleOperation.Display, false))
                                    allowedObjectsArr.Add(uiModule.Value);
                            }
                        }
                        else
                        // для каждого из объектов просмотра проверяем права доступа
                        if (CheckPermissionForSystemObject(uiModule.Key, (int)UIModuleOperations.Display, false))
                            allowedObjectsArr.Add(uiModule.Value);
                    }
                }
            }
            return allowedObjectsArr.ToArray();
        }

        #region версии сборок серверной части
        private Dictionary<string, string> serverAssemblyes = null;
        /// <summary>
        /// Метод проверки версий серверных сборок.
        /// </summary>
        /// <returns>Список названий и версий серверных сборок</returns>
        public Dictionary<string, string> GetServerAssemblyesInfo(string filter)
        {
            if (serverAssemblyes == null)
                serverAssemblyes = AppVersionControl.GetAssemblyesVersions(filter);
            return serverAssemblyes;
        }

        public string ServerLibraryVersion()
        {
            return AppVersionControl.GetServerLibraryVersion();
        }
        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Выполняет синхронизацию объектов в таблице Objects с объектами схемы.
        /// </summary>
        public void SynchronizeObjectsWithScheme()
        {
            foreach (DataRow row in ObjectsTable.Select(String.Format("OBJECTTYPE in ({0}, {1}, {2}, {3})",
                (int)SysObjectsTypes.FactTable,
                (int)SysObjectsTypes.DataClassifier,
                (int)SysObjectsTypes.AssociatedClassifier,
                (int)SysObjectsTypes.Associate)))
            {
                string name = Convert.ToString(row["Name"]);

                // Проверим является ли идентификатор гвидом
                bool isNotGuid = false;
                try
                {
                    Guid guid = new Guid(name);
                }
                catch (FormatException)
                {
                    isNotGuid = true;
                }

                // Если идентификатор не гвид, то ищем объект по FullName
                if (isNotGuid)
                {
                    IKeyIdentifiedObject bean = null;

                    SysObjectsTypes sysObjectsType = (SysObjectsTypes) Convert.ToInt32(row["OBJECTTYPE"]);
                    switch(sysObjectsType)
                    {
                        case SysObjectsTypes.FactTable:
                            foreach (IFactTable item in _scheme.FactTables.Values)
                            {
                                if (item.FullName == name)
                                    bean = item;
                            }
                            break;
                        case SysObjectsTypes.DataClassifier:
                        case SysObjectsTypes.AssociatedClassifier:
                            foreach (IClassifier item in _scheme.Classifiers.Values)
                            {
                                if (item.FullName == name)
                                    bean = item;
                            }
                            break;
                        case SysObjectsTypes.Associate:
                            foreach (IEntityAssociation item in _scheme.Associations.Values)
                            {
                                if (item.FullName == name)
                                    bean = item;
                            }
                            break;
                    }

                    if (bean != null)
                    {
                        if (bean.ObjectKey != Guid.Empty.ToString())
                        {
                            row["Name"] = bean.ObjectKey;
                        }
                        row["ObjectKey"] = bean.ObjectKey;
                    }
                    else
                    {
                        row.Delete();
                    }
                }
            }

            using (DataUpdater du = GetObjectsUpdater(null))
            {
                du.Update(ref ObjectsTable);
                RefreshHierarchyColumn(ObjectsTable);
            }
        }

        #endregion
    }
    #endregion
}
