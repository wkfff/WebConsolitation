using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTabs;

namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    internal class ExportImportHelper
    {
        #region константы
        private static string objectsQuery = "select ID, Name, Caption, Description, ObjectType from Objects";
        private static string permissionsQuery = "select ID, RefObjects, RefGroups, RefUsers, AllowedAction from Permissions where (RefUsers >= 100 or RefUsers is null)";
        private static string memberShipsQuery = "select ID, RefUsers, RefGroups from Memberships";
        #endregion

        #region вспомогательные методы
        
        private static DataTable GetObjects(IWorkplace workplace)
        {
            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                return (DataTable)db.ExecQuery(objectsQuery, QueryResultTypes.DataTable);
            }
            finally
            {
                db.Dispose();
            }
        }

        private static void RefreshIDs(string columnName, DataTable newIds, ref DataTable changedTable)
        {
            foreach (DataRow row in newIds.Rows)
            {
                DataRow[] changeRows = changedTable.Select(string.Format("{0} = {1}", columnName, row["ID"]));
                foreach (DataRow changeRow in changeRows)
                    changeRow[columnName] = row["NewID"];
            }
        }

        private static object RefreshId(DataTable newIds, int oldId)
        {
            if (oldId > -1)
            {
                return newIds.Select(string.Format("ID = {0}", oldId))[0]["NewID"];
            }
            else return DBNull.Value;
        }

        private static DataTable CreateIdsDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("NewID", typeof(int));
            return table;
        }

        #endregion

        #region экспорт всех данных из администрирования






        public static void Export(IWorkplace workplace)
        {
            InnerExport(workplace);
        }

        /// <summary>
        /// экспорт объектов системы, пользователей, групп, прав на объекты
        /// </summary>
        /// <param name="workplace"></param>
        private static void InnerExport(IWorkplace workplace)
        {
            XmlDocument objectsListXml = GetObjectsListXml(workplace);

            AppendOrganizations(workplace, objectsListXml);

            AppendDepartments(workplace, objectsListXml);

            AppendTasksTypes(workplace, objectsListXml);

            AppendGroups(workplace, objectsListXml);

            AppendUsers(workplace, objectsListXml);

            AppendMemberShips(workplace, objectsListXml);

            AppendPermissions(workplace, objectsListXml);

            string fileName = string.Empty;
            if (Krista.FM.Client.Common.ExportImportHelper.GetFileName("Объекты системы", Krista.FM.Client.Common.ExportImportHelper.fileExtensions.xml, true, ref fileName))
            {
                XmlHelper.Save(objectsListXml, fileName);
                XmlHelper.ClearDomDocument(ref objectsListXml);
            }
        }

        /// <summary>
        /// создаем структуру всего документа. Добавляем к нему секцию по объектам системы
        /// </summary>
        /// <param name="workplace"></param>
        /// <returns></returns>
        private static XmlDocument GetObjectsListXml(IWorkplace workplace)
        {
            DataTable objects = null;
            objects = GetObjects(workplace);

            XmlDocument objectsListXml = new XmlDocument();

            XmlNode rootNode = objectsListXml.CreateElement(ExportImportXmlConsts.RootNodeTagName);
            objectsListXml.AppendChild(rootNode);
            // атрибут с версией
            XmlAttribute versionAttr = objectsListXml.CreateAttribute(ExportImportXmlConsts.VersionTagName);
            versionAttr.Value = AppVersionControl.GetAssemblyVersion(Assembly.GetExecutingAssembly());
            rootNode.Attributes.Append(versionAttr);

            XmlNode rootObjectsNode = objectsListXml.CreateElement(ExportImportXmlConsts.ObjectsTagName);
            rootNode.AppendChild(rootObjectsNode);

            
            
            foreach (DataRow row in objects.Rows)
            {
                AppendSystemObjectToXml(rootObjectsNode, row);
            }

            return objectsListXml;
        }

        private static void AppendSystemObjectToXml(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.ObjectTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.NameTagName, Convert.ToString(row["NAME"]) },
                new string[2] { ExportImportXmlConsts.CaptionTagName, Convert.ToString(row["CAPTION"]) },
                new string[2] { ExportImportXmlConsts.DescriptionTagName, Convert.ToString(row["DESCRIPTION"]) },
                new string[2] { ExportImportXmlConsts.ObjectTypeTagName, Convert.ToString(row["OBJECTTYPE"]) }
            );
        }

        /// <summary>
        /// экспорт одного объекта. Переделать на использование XMLHelper
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="rootObjectsNode"></param>
        /// <param name="row"></param>
        /// <param name="objects"></param>
        private static void ExportSingleObject(XmlNode rootObjectsNode,
            DataRow row, DataTable objects, string objectName)
        {
            XmlElement objectElement = rootObjectsNode.OwnerDocument.CreateElement(objectName);
            rootObjectsNode.AppendChild(objectElement);
            // так как кроме объекта системы ничего другого не надо экспортировать
            foreach (DataColumn column in objects.Columns)
            {
                XmlAttribute attribute = objectElement.OwnerDocument.CreateAttribute(column.ColumnName);
                attribute.Value = row[column.ColumnName].ToString();
                objectElement.Attributes.Append(attribute);
            }
        }

        /// <summary>
        /// добавляет информацию об организациях в XML
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="objectsListXml"></param>
        private static void AppendOrganizations(IWorkplace workplace, XmlDocument objectsListXml)
        {
            XmlNode organizationsNodeXml = objectsListXml.CreateElement(ExportImportXmlConsts.OrganizationsTagName);
            objectsListXml.FirstChild.AppendChild(organizationsNodeXml);

            DataTable table = workplace.ActiveScheme.UsersManager.GetOrganizations();
            foreach (DataRow row in table.Rows)
            {
                AppendOrganization(organizationsNodeXml, row);
            }
        }

        private static void AppendOrganization(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.OrganizationTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.NameTagName, Convert.ToString(row["NAME"]) },
                new string[2] { ExportImportXmlConsts.DescriptionTagName, Convert.ToString(row["DESCRIPTION"]) }
            );
        }

        /// <summary>
        /// добавляет информацию о департаментах в XML
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="objectsListXml"></param>
        private static void AppendDepartments(IWorkplace workplace, XmlDocument objectsListXml)
        {
            XmlNode departmentsNodeXml = objectsListXml.CreateElement(ExportImportXmlConsts.DepartamentsTagName);
            objectsListXml.FirstChild.AppendChild(departmentsNodeXml);

            DataTable table = workplace.ActiveScheme.UsersManager.GetDepartments();
            foreach (DataRow row in table.Rows)
            {
                AppendDepartament(departmentsNodeXml, row);
            }
        }

        private static void AppendDepartament(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.DepartamentTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.NameTagName, Convert.ToString(row["NAME"]) },
                new string[2] { ExportImportXmlConsts.DescriptionTagName, Convert.ToString(row["DESCRIPTION"]) }
            );
        }

        private static void AppendTasksTypes(IWorkplace workplace, XmlDocument objectsListXml)
        {
            XmlNode tasksTypesNodeXml = objectsListXml.CreateElement(ExportImportXmlConsts.TasksTypesTagName);
            objectsListXml.FirstChild.AppendChild(tasksTypesNodeXml);

            DataTable table = workplace.ActiveScheme.UsersManager.GetTasksTypes();
            foreach (DataRow row in table.Rows)
            {
                AppendTaskType(tasksTypesNodeXml, row);
            }
        }

        private static void AppendTaskType(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.TasksTypeTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.TasksTypeCodeTagName, Convert.ToString(row["CODE"]) },
                new string[2] { ExportImportXmlConsts.NameTagName, Convert.ToString(row["NAME"]) },
                new string[2] { ExportImportXmlConsts.DescriptionTagName, Convert.ToString(row["DESCRIPTION"]) },
                new string[2] { ExportImportXmlConsts.TasksTypeTagName, Convert.ToString(row["TASKTYPE"]) }
            );
        }

        private static void AppendGroups(IWorkplace workplace, XmlDocument objectsListXml)
        {
            XmlNode groupsNodeXml = objectsListXml.CreateElement(ExportImportXmlConsts.GroupsTagName);
            objectsListXml.FirstChild.AppendChild(groupsNodeXml);

            DataTable table = workplace.ActiveScheme.UsersManager.GetGroups();
            foreach (DataRow row in table.Rows)
            {
                AppendGroup(groupsNodeXml, row);
            }
        }

        private static void AppendGroup(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.GroupTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.NameTagName, Convert.ToString(row["NAME"]) },
                new string[2] { ExportImportXmlConsts.DescriptionTagName, Convert.ToString(row["DESCRIPTION"]) },
                new string[2] { ExportImportXmlConsts.GroupBlockedTagName, Convert.ToString(row["BLOCKED"]) },
                new string[2] { ExportImportXmlConsts.GroupDnsNameTagName, Convert.ToString(row["DNSNAME"]) }
            );
        }

        private static void AppendUsers(IWorkplace workplace, XmlDocument objectsListXml)
        {
            XmlNode usersNodeXml = objectsListXml.CreateElement(ExportImportXmlConsts.UsersTagName);
            objectsListXml.FirstChild.AppendChild(usersNodeXml);

            DataTable table = workplace.ActiveScheme.UsersManager.GetUsers();
            foreach (DataRow row in table.Rows)
            {
                AppendUser(usersNodeXml, row);
            }
        }

        private static void AppendUser(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.UserTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.NameTagName, Convert.ToString(row["NAME"]) },
                new string[2] { ExportImportXmlConsts.DescriptionTagName, Convert.ToString(row["DESCRIPTION"]) },
                new string[2] { ExportImportXmlConsts.UserTypeTagName, Convert.ToString(row["USERTYPE"]) },
                new string[2] { ExportImportXmlConsts.UserBlockedTagName, Convert.ToString(row["BLOCKED"]) },
                new string[2] { ExportImportXmlConsts.UserDnsNameTagName, Convert.ToString(row["DNSNAME"]) },
                new string[2] { ExportImportXmlConsts.UserLastLoginTagName, Convert.ToString(row["LASTLOGIN"]) },
                new string[2] { ExportImportXmlConsts.UserFirstName, Convert.ToString(row["FIRSTNAME"]) },
                new string[2] { ExportImportXmlConsts.UserLastName, Convert.ToString(row["LASTNAME"]) },
                new string[2] { ExportImportXmlConsts.UserPatronymicTagName, Convert.ToString(row["PATRONYMIC"]) },
                new string[2] { ExportImportXmlConsts.UserJobTitleTagName, Convert.ToString(row["JOBTITLE"]) },
                new string[2] { ExportImportXmlConsts.UserRefDepartamentsTagName, Convert.ToString(row["REFDEPARTMENTS"]) },
                new string[2] { ExportImportXmlConsts.UserRefOrganizationsTagName, Convert.ToString(row["REFORGANIZATIONS"]) },
                new string[2] { ExportImportXmlConsts.UserAllowDomainAuthTagName, Convert.ToString(row["ALLOWDOMAINAUTH"]) },
                new string[2] { ExportImportXmlConsts.UserAllowPwdAuthTagName, Convert.ToString(row["ALLOWPWDAUTH"]) }
            );
        }


        // вхождение пользователей в группы
        // сохраняем Id группы, название группы, id пользователя, имя, вхождение в группу
        // при импорте смотрим по старым и новым Id кто где и проставляем вхождение
        private static void AppendMemberShips(IWorkplace workplace, XmlDocument objectsListXml)
        {
            XmlNode memberShipsNodeXml = objectsListXml.CreateElement(ExportImportXmlConsts.MemberShipsTagName);
            objectsListXml.FirstChild.AppendChild(memberShipsNodeXml);
            // получаем заведомо пустой список вхождений пользователей в группу
            DataTable table = null;
            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                table = (DataTable)db.ExecQuery(memberShipsQuery, QueryResultTypes.DataTable);
            }
            finally
            {
                db.Dispose();
            }

            foreach (DataRow row in table.Rows)
            {
                AppendMemberShip(memberShipsNodeXml, row);
            }
        }

        private static void AppendMemberShip(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.MemberShipTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.MemberShipRefUsers, Convert.ToString(row["REFUSERS"]) },
                new string[2] { ExportImportXmlConsts.MemberShipRefGroups, Convert.ToString(row["REFGROUPS"]) }
            );
        }


        private static void AppendPermissions(IWorkplace workplace, XmlDocument objectsListXml)
        {
            XmlNode permissionsNodeXml = objectsListXml.CreateElement(ExportImportXmlConsts.PermissionsTagName);
            objectsListXml.FirstChild.AppendChild(permissionsNodeXml);
            // получаем данные по разрешенным операциям
            DataTable table = null;
            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                table = (DataTable)db.ExecQuery(permissionsQuery, QueryResultTypes.DataTable);
            }
            finally
            {
                db.Dispose();
            }

            foreach (DataRow row in table.Rows)
            {
                AppendPermission(permissionsNodeXml, row);
            }
        }

        private static void AppendPermission(XmlNode parentNode, DataRow row)
        {
            XmlHelper.AddChildNode(parentNode, ExportImportXmlConsts.PermissionTagName,
                new string[2] { ExportImportXmlConsts.IDTagName, Convert.ToString(row["ID"]) },
                new string[2] { ExportImportXmlConsts.PermissionRefObjectsTagName, Convert.ToString(row["REFOBJECTS"]) },
                new string[2] { ExportImportXmlConsts.PermissionRefUsersTagName, Convert.ToString(row["REFUSERS"]) },
                new string[2] { ExportImportXmlConsts.PermissionRefGroupsTagName, Convert.ToString(row["REFGROUPS"]) },
                new string[2] { ExportImportXmlConsts.PermissionAllowedActionTagName, Convert.ToString(row["ALLOWEDACTION"]) }
            );
        }

        #endregion

        #region импорт

        // много практически одинаковых методов... типа дублирование кода, нужно будет это устранить

        private enum ImportTableName { Objects, Departaments, Organizations, TasksTypes, Groups, Users, MemberShips, Permissions }

        public static void Import(IWorkplace workplace)
        {
            InnerImport(workplace);
        }

        private static void InnerImport(IWorkplace workplace)
        {
            // импортируем в таком порядке:
            // сначала объекты без типов задач
            // в объекты не входят типы задач и сами задачи
            // организации, отделы, типы задач
            // потом группы, пользователи
            // а потом уже вхождения в группы и права

            string fileName = string.Empty;
            if (Krista.FM.Client.Common.ExportImportHelper.GetFileName("Объекты системы",
                Krista.FM.Client.Common.ExportImportHelper.fileExtensions.xml, false, ref fileName))
            {
                XmlDocument objectsListXml = new XmlDocument();//XmlHelper.Load(fileName);
                objectsListXml.Load(fileName);

                DataTable tasksTypesTableIds = null;
                ImpotrTasksTypes(workplace, objectsListXml, ref tasksTypesTableIds);

                DataTable objectsTableIds = null;
                ImportObjectsSection(workplace, objectsListXml, ref objectsTableIds);

                DataTable organizationsTableIds = null;
                ImportOrganizations(workplace, objectsListXml, ref organizationsTableIds);

                DataTable departamentsTableIds = null;
                ImportDepartaments(workplace, objectsListXml, ref departamentsTableIds);

                DataTable groupsTableIds = null;
                ImportGroups(workplace, objectsListXml, ref groupsTableIds);

                DataTable usersTableIds = null;
                ImportUsers(workplace, objectsListXml, departamentsTableIds, organizationsTableIds, ref usersTableIds);

                ImportMemberShips(workplace, objectsListXml, groupsTableIds, usersTableIds);

                ImportPermissions(workplace, objectsListXml, objectsTableIds, usersTableIds, groupsTableIds);

            }
        }

        #region импорт прав и членства

        

        private static void ImportMemberShips(IWorkplace workplace, XmlDocument objectsListXml, DataTable groupsTableIds, DataTable usersTableIds)
        {
            XmlNodeList memberShips = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}",
                ExportImportXmlConsts.RootNodeTagName,
                ExportImportXmlConsts.MemberShipsTagName,
                ExportImportXmlConsts.MemberShipTagName));

            DataTable memberShipsData = new DataTable();
            memberShipsData.Columns.Add("RefUserId", typeof(int));
            memberShipsData.Columns.Add("RefGroupId", typeof(int));
            
            GetMemberShipsData(memberShips, ref memberShipsData);
            // меняем старые ID на новые 
            // группы
            RefreshIDs("RefGroupId", groupsTableIds, ref memberShipsData);
            // пользователи
            RefreshIDs("RefUserId", usersTableIds, ref memberShipsData);
            
            foreach (DataRow groupRow in groupsTableIds.Rows)
            {
                int groupNewId = Convert.ToInt32(groupRow["NewID"]);
                DataTable users = workplace.ActiveScheme.UsersManager.GetUsersForGroup(groupNewId);
                foreach (DataRow userRow in usersTableIds.Rows)
                {
                    if (memberShipsData.Select(string.Format("RefUserId = {0} and RefGroupId = {1}", userRow["NewID"], groupNewId)).Length > 0)
                    {
                        DataRow user = users.Select(string.Format("ID = {0}", userRow["NewID"]))[0];
                        if (!Convert.ToBoolean(user["ISMEMBER"]))
                            user["ISMEMBER"] = true;
                    }
                }
                DataTable memberShipsChanges = users.GetChanges();
                if (memberShipsChanges != null)
                    workplace.ActiveScheme.UsersManager.ApplayMembershipChanges(groupNewId, memberShipsChanges, false);
            }
        }

        private static void GetMemberShipsData(XmlNodeList memberShips, ref DataTable memberShipsData)
        {
            foreach (XmlNode membership in memberShips)
            {
                int refUserId = XmlHelper.GetIntAttrValue(membership, ExportImportXmlConsts.MemberShipRefUsers, 0);
                int refGroupId = XmlHelper.GetIntAttrValue(membership, ExportImportXmlConsts.MemberShipRefGroups, 0);
                memberShipsData.Rows.Add(refUserId, refGroupId);
            }
        }

        private static void ImportPermissions(IWorkplace workplace, XmlDocument objectsListXml, DataTable objectsTableIds, DataTable usersTableIds, DataTable groupsTableIds)
        {
            XmlNodeList permissons = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}",
                ExportImportXmlConsts.RootNodeTagName,
                ExportImportXmlConsts.PermissionsTagName,
                ExportImportXmlConsts.PermissionTagName));

            DataTable permissionsData = new DataTable();
            permissionsData.Columns.Add("RefObjectId", typeof(int));
            permissionsData.Columns.Add("RefUserId", typeof(int));
            permissionsData.Columns.Add("RefGroupId", typeof(int));
            permissionsData.Columns.Add("AllowedAction", typeof(int));

            GetPermissions(permissons, ref permissionsData);
            // объекты
            RefreshIDs("RefObjectId", objectsTableIds, ref permissionsData);
            // группы
            RefreshIDs("RefGroupId", groupsTableIds, ref permissionsData);
            // пользователи
            RefreshIDs("RefUserId", usersTableIds, ref permissionsData);

            // получим список объектов, для которых были выгружены права
            List<int> objectsIdInPermissions = new List<int>();
            foreach (DataRow objectRow in permissionsData.Rows)
            {
                int id = Convert.ToInt32(objectRow["RefObjectId"]);
                if (!objectsIdInPermissions.Contains(id))
                    objectsIdInPermissions.Add(id);
            }

            foreach (int objectId in objectsIdInPermissions)
            {
                // для задач не восстанавливаем пока права
                DataRow[] objectRows = objectsTableIds.Select(string.Format("NewId = {0}", objectId));
                if (objectRows.Length < 1)
                    continue;

                DataRow[] objectPermussions = permissionsData.Select(string.Format("RefObjectId = {0}", objectId));
                int objectType = Convert.ToInt32(objectRows[0]["ObjectType"]);
                DataTable usersPermissionsForObject = workplace.ActiveScheme.UsersManager.GetUsersPermissionsForObject(objectId, objectType);
                DataTable groupsPermissionsForObject = workplace.ActiveScheme.UsersManager.GetGroupsPermissionsForObject(objectId, objectType);
                
                foreach (DataRow objectPermussion in objectPermussions)
                {
                    string allowedActionStr = objectPermussion[ExportImportXmlConsts.PermissionAllowedActionTagName].ToString();
                    DataRow row = null;
                    // разрешения для пользователя
                    int groupId = Convert.ToInt32(objectPermussion["RefGroupId"]);
                    if (groupId > -1)
                    {
                        row = groupsPermissionsForObject.Select(string.Format("ID = {0}", groupId))[0];
                        if (!Convert.ToBoolean(row[allowedActionStr]))
                            row[allowedActionStr] = true;
                    }
                    // разрешения для группы
                    int userId = Convert.ToInt32(objectPermussion["RefUserId"]);
                    if (userId > -1)
                    {
                        row = usersPermissionsForObject.Select(string.Format("ID = {0}", userId))[0];
                        if (!Convert.ToBoolean(row[allowedActionStr]))
                            row[allowedActionStr] = true;
                    }
                }
                DataTable usersPermissionsChange = usersPermissionsForObject.GetChanges();
                if (usersPermissionsChange != null)
                    workplace.ActiveScheme.UsersManager.ApplayUsersPermissionsChanges(objectId, objectType, usersPermissionsChange);
                DataTable groupsPermissionsChange = groupsPermissionsForObject.GetChanges();
                if (groupsPermissionsChange != null)
                    workplace.ActiveScheme.UsersManager.ApplayGroupsPermissionsChanges(objectId, objectType, groupsPermissionsChange);
            }
        }

        private static void GetPermissions(XmlNodeList permissons, ref DataTable permissionsData)
        {
            foreach (XmlNode permisson in permissons)
            {
                int refObjectId = XmlHelper.GetIntAttrValue(permisson, ExportImportXmlConsts.PermissionRefObjectsTagName, 0);
                int refUserId = XmlHelper.GetIntAttrValue(permisson, ExportImportXmlConsts.PermissionRefUsersTagName, -1);
                int refGroupId = XmlHelper.GetIntAttrValue(permisson, ExportImportXmlConsts.PermissionRefGroupsTagName, -1);
                int allowedAction = XmlHelper.GetIntAttrValue(permisson, ExportImportXmlConsts.PermissionAllowedActionTagName, 0);
                permissionsData.Rows.Add(refObjectId, refUserId, refGroupId, allowedAction);
            }
        }

        #endregion

        #region импорт групп

        private static void ImportGroups(IWorkplace workplace, XmlDocument objectsListXml, ref DataTable groupsTableIds)
        {
            XmlNodeList groups = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}",
                ExportImportXmlConsts.RootNodeTagName,
                ExportImportXmlConsts.GroupsTagName,
                ExportImportXmlConsts.GroupTagName));

            DataTable groupsTable = workplace.ActiveScheme.UsersManager.GetGroups();

            groupsTableIds = CreateIdsDataTable();

            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                // пытаемся добавить оргинизации
                foreach (XmlNode groupNode in groups)
                {
                    ImportGroup(db, groupNode, ref groupsTable, ref groupsTableIds);
                }
                // тут сохраняем изменения...
                DataTable changes = groupsTable.GetChanges();
                if (changes != null)
                    workplace.ActiveScheme.UsersManager.ApplayGroupsChanges(changes);
            }
            finally
            {
                db.Dispose();
            }
        }

        private static void ImportGroup(IDatabase db, XmlNode groupNode, ref DataTable groupsTable, ref DataTable groupsTableIds)
        {
            string groupName = XmlHelper.GetStringAttrValue(groupNode, ExportImportXmlConsts.NameTagName, string.Empty);
            string groupDNSName = XmlHelper.GetStringAttrValue(groupNode, ExportImportXmlConsts.GroupDnsNameTagName, string.Empty);
            int oldID = XmlHelper.GetIntAttrValue(groupNode, ExportImportXmlConsts.IDTagName, 0);
            string filter = string.Empty;
            if (groupDNSName != string.Empty)
                filter = String.Format("(Name like '{0}') and (DNSName like '{1}')", groupName, groupDNSName);
            else
                filter = String.Format("(Name like '{0}') and (DNSName is null)", groupName);
            DataRow[] rows = groupsTable.Select(filter);
            if (rows.Length > 0)
            {
                // группа присутствует
                groupsTableIds.Rows.Add(oldID, rows[0]["ID"]);
            }
            else
            {
                // добавляем новую группу
                string groupDescription = XmlHelper.GetStringAttrValue(groupNode, ExportImportXmlConsts.DescriptionTagName, string.Empty);
                bool groupIsBlocked = XmlHelper.GetBoolAttrValue(groupNode, ExportImportXmlConsts.GroupBlockedTagName, false);
                int newID = db.GetGenerator(ExportImportXmlConsts.GeneratorNameGroups);
                groupsTable.Rows.Add(newID, groupName, groupDescription, groupIsBlocked, groupDNSName);

                groupsTableIds.Rows.Add(oldID, newID);
            }
        }

        #endregion

        #region импорт пользователей

        private static void ImportUsers(IWorkplace workplace, XmlDocument objectsListXml,
            DataTable departamentsIds, DataTable organizationsIds, ref DataTable usersTableIds)
        {
            XmlNodeList users = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}",
                ExportImportXmlConsts.RootNodeTagName,
                ExportImportXmlConsts.UsersTagName,
                ExportImportXmlConsts.UserTagName));

            DataTable usersTable = workplace.ActiveScheme.UsersManager.GetUsers();

            usersTableIds = CreateIdsDataTable();

            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                // пытаемся добавить оргинизации
                foreach (XmlNode userNode in users)
                {
                    ImportUser(db, userNode, departamentsIds, organizationsIds, ref usersTable, ref usersTableIds);
                }
                // тут сохраняем изменения...
                DataTable changes = usersTable.GetChanges();
                if (changes != null)
                    workplace.ActiveScheme.UsersManager.ApplayUsersChanges(changes);
            }
            finally
            {
                db.Dispose();
            }
        }

        private static void ImportUser(IDatabase db, XmlNode userNode, DataTable departamentsIds, DataTable organizationsIds, ref DataTable usersTable, ref DataTable usersTableIds)
        {
            string userName = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.NameTagName, string.Empty);
            string userDNSName = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.UserDnsNameTagName, string.Empty);
            int oldID = XmlHelper.GetIntAttrValue(userNode, ExportImportXmlConsts.IDTagName, 0);

            string dnsNameFilter = string.Empty;
            if (userDNSName != string.Empty)
                dnsNameFilter = string.Format("(DNSName like '{0}')", userDNSName);
            else
                dnsNameFilter = "(DNSName is null)";
            string filter = String.Format("(Name like '{0}') and {1}", userName, dnsNameFilter);
            DataRow[] rows = usersTable.Select(filter);
            if (rows.Length > 0)
            {
                // пользователь присутствует
                usersTableIds.Rows.Add(oldID, rows[0]["ID"]);
            }
            else
            {
                // добавим нового пользователя

                int newID = db.GetGenerator(ExportImportXmlConsts.GeneratorNameUsers);
                string userDescription = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.DescriptionTagName, string.Empty);
                int userType = XmlHelper.GetIntAttrValue(userNode, ExportImportXmlConsts.UserTypeTagName, 0);
                bool userIsBlocked = XmlHelper.GetBoolAttrValue(userNode, ExportImportXmlConsts.UserBlockedTagName, false);
                string userLastLogin = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.UserLastLoginTagName, string.Empty);
                string userFirstName = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.UserFirstName, string.Empty);
                string userLastName = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.UserLastName, string.Empty);
                string userPatronymic = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.UserPatronymicTagName, string.Empty);
                string userJobTitle = XmlHelper.GetStringAttrValue(userNode, ExportImportXmlConsts.UserJobTitleTagName, string.Empty);
                
                // меняем старые ссылки по организациям и отделам на новые
                int userRefDepartaments = XmlHelper.GetIntAttrValue(userNode, ExportImportXmlConsts.UserRefDepartamentsTagName, -1);
                object userRefDepartamentsObj = RefreshId(departamentsIds, userRefDepartaments);
                int userRefOrganizations = XmlHelper.GetIntAttrValue(userNode, ExportImportXmlConsts.UserRefOrganizationsTagName, -1);
                object userRefOrganizationsObj = RefreshId(organizationsIds, userRefOrganizations);

                bool userAllowDomainAuth = XmlHelper.GetBoolAttrValue(userNode, ExportImportXmlConsts.UserAllowDomainAuthTagName, false);
                bool userAllowPwdAuth = XmlHelper.GetBoolAttrValue(userNode, ExportImportXmlConsts.UserAllowPwdAuthTagName, false);

                usersTable.Rows.Add(newID, userName, userDescription,
                    userType, userIsBlocked, userDNSName, userLastLogin,
                    userFirstName, userLastName, userPatronymic, userJobTitle,
                    userRefDepartamentsObj, userRefOrganizationsObj, userAllowDomainAuth, userAllowPwdAuth);

                usersTableIds.Rows.Add(oldID, newID);
            }
        }

        #endregion

        #region импорт типов задач

        private static void ImpotrTasksTypes(IWorkplace workplace, XmlDocument objectsListXml, ref DataTable taskTypesTableIds)
        {
            XmlNodeList tasksTypes = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}",
                ExportImportXmlConsts.RootNodeTagName,
                ExportImportXmlConsts.TasksTypesTagName,
                ExportImportXmlConsts.TasksTypeTagName));

            DataTable tasksTypesTable = workplace.ActiveScheme.UsersManager.GetTasksTypes();

            taskTypesTableIds = CreateIdsDataTable();

            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                // пытаемся добавить оргинизации
                foreach (XmlNode tasksTypeNode in tasksTypes)
                {
                    ImportTasksType(db, tasksTypeNode, ref tasksTypesTable, ref taskTypesTableIds);
                }
                // тут сохраняем изменения...
                DataTable changes = tasksTypesTable.GetChanges();
                if (changes != null)
                    workplace.ActiveScheme.UsersManager.ApplayTasksTypesChanges(changes);
            }
            finally
            {
                db.Dispose();
            }
        }

        private static void ImportTasksType(IDatabase db, XmlNode tasksTypeNode,
            ref DataTable tasksTypesTable, ref DataTable taskTypesTableIds)
        {
            string tasksTypeName = XmlHelper.GetStringAttrValue(tasksTypeNode, ExportImportXmlConsts.NameTagName, string.Empty);
            object tasksTypeTaskType = XmlHelper.GetIntAttrValue(tasksTypeNode, ExportImportXmlConsts.TasksTypeTaskTypeTagName, 0);
            int oldID = XmlHelper.GetIntAttrValue(tasksTypeNode, ExportImportXmlConsts.IDTagName, 0);

            string filter = String.Format("(Name like '{0}') and (TaskType = '{1}')", tasksTypeName, tasksTypeTaskType);
            DataRow[] rows = tasksTypesTable.Select(filter);
            if (rows.Length > 0)
            {
                // тип задач присутствует
                taskTypesTableIds.Rows.Add(oldID, rows[0]["ID"]);
            }
            else
            {
                // добавляем новый тип задач
                object tasksTypeCode = XmlHelper.GetIntAttrValue(tasksTypeNode, ExportImportXmlConsts.TasksTypeCodeTagName, 0);
                string tasksTypeDescription = XmlHelper.GetStringAttrValue(tasksTypeNode, ExportImportXmlConsts.DescriptionTagName, string.Empty);
                int newID = db.GetGenerator(ExportImportXmlConsts.GeneratorNameTasksTypes);
                tasksTypesTable.Rows.Add(newID, tasksTypeCode, tasksTypeName, tasksTypeDescription, tasksTypeTaskType);

                taskTypesTableIds.Rows.Add(oldID, newID);
            }
        }

        #endregion

        #region импорт организаций и отделов

        private static void ImportDepartaments(IWorkplace workplace, XmlDocument objectsListXml, ref DataTable departamentsTableIds)
        {
            XmlNodeList departaments = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}",
                ExportImportXmlConsts.RootNodeTagName,
                ExportImportXmlConsts.DepartamentsTagName,
                ExportImportXmlConsts.DepartamentTagName));

            DataTable departamentsTable = workplace.ActiveScheme.UsersManager.GetDepartments();

            departamentsTableIds = CreateIdsDataTable();

            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                // пытаемся добавить оргинизации
                foreach (XmlNode departamentNode in departaments)
                {
                    ImportDepartamentOrganization(db, departamentNode, ref departamentsTable, 
                        ref departamentsTableIds, ImportTableName.Departaments, ExportImportXmlConsts.GeneratorNameDepartments);
                }
                // тут сохраняем изменения...
                DataTable changes = departamentsTable.GetChanges();
                if (changes != null)
                    workplace.ActiveScheme.UsersManager.ApplayDepartmentsChanges(changes);
            }
            finally
            {
                db.Dispose();
            }
        }

        private static void ImportOrganizations(IWorkplace workplace, XmlDocument objectsListXml, ref DataTable organizationsTableIds)
        {
            XmlNodeList organizations = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}",
                ExportImportXmlConsts.RootNodeTagName,
                ExportImportXmlConsts.OrganizationsTagName,
                ExportImportXmlConsts.OrganizationTagName));

            DataTable organizationsTable = workplace.ActiveScheme.UsersManager.GetOrganizations();

            organizationsTableIds = CreateIdsDataTable();

            IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                // пытаемся добавить оргинизации
                foreach (XmlNode organizationNode in organizations)
                {
                    ImportDepartamentOrganization(db, organizationNode, ref organizationsTable, 
                        ref organizationsTableIds, ImportTableName.Organizations, ExportImportXmlConsts.GeneratorNameOrganizations);
                }
                // тут сохраняем изменения...
                DataTable changes = organizationsTable.GetChanges();
                if (changes != null)
                    workplace.ActiveScheme.UsersManager.ApplayOrganizationsChanges(changes);
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// импорт одной записи оргинизации или отдела
        /// </summary>
        /// <param name="db"></param>
        /// <param name="node"></param>
        /// <param name="table"></param>
        /// <param name="tableIds"></param>
        /// <param name="tableType"></param>
        /// <param name="generatorName"></param>
        private static void ImportDepartamentOrganization(IDatabase db, XmlNode node, ref DataTable table,
            ref DataTable tableIds, ImportTableName tableType, string generatorName)
        {
            string name = XmlHelper.GetStringAttrValue(node, ExportImportXmlConsts.NameTagName, string.Empty);
            int oldID = XmlHelper.GetIntAttrValue(node, ExportImportXmlConsts.IDTagName, 0);

            string filter = String.Format("Name like '{0}'", name);
            DataRow[] rows = table.Select(filter);
            if (rows.Length > 0)
            {
                // такая организация уже присутствует
                tableIds.Rows.Add(oldID, rows[0]["ID"]);
            }
            else
            {
                // добавляем новую организацию
                string description = XmlHelper.GetStringAttrValue(node, ExportImportXmlConsts.DescriptionTagName, string.Empty);
                int newID = db.GetGenerator(generatorName);
                table.Rows.Add(newID, name, description);
                tableIds.Rows.Add(oldID, newID);
            }
        }

        #endregion

        #region импорт объектов системы

        private static void ImportObjectsSection(IWorkplace workplace, XmlDocument objectsListXml, ref DataTable objectsTableIds)
        {
            XmlNodeList objects = objectsListXml.SelectNodes(String.Format("{0}/{1}/{2}", 
                ExportImportXmlConsts.RootNodeTagName, 
                ExportImportXmlConsts.ObjectsTagName, 
                ExportImportXmlConsts.ObjectTagName));

            DataTable objectsTable = GetObjects(workplace);
            // создаем таблицу со старыми и новыми ID добавляемых 
            objectsTableIds = CreateIdsDataTable();
            objectsTableIds.Columns.Add("ObjectType", typeof(int));
            // внимание, новые объекты не импортируем... 
            // получаем ID только для текущих объектов, если они уже присутствуют
            objectsTable.BeginLoadData();
            foreach (XmlNode objectNode in objects)
            {
                ImportObject(workplace, objectNode, objectsTable, ref objectsTableIds);
            }
            objectsTable.EndLoadData();
        }

        private static void ImportObject(IWorkplace workplace, XmlNode objectNode, DataTable objectsTable, ref DataTable objectsTableIds)
        {
            string objectName = XmlHelper.GetStringAttrValue(objectNode, ExportImportXmlConsts.NameTagName, string.Empty);
            int objectType = XmlHelper.GetIntAttrValue(objectNode, ExportImportXmlConsts.ObjectTypeTagName, 0);
            string objectCaption = XmlHelper.GetStringAttrValue(objectNode, ExportImportXmlConsts.CaptionTagName, string.Empty);
            string filter = string.Empty;
            // для поиска нужного объекта от типа задач строим несколько другой фильтр
            if (objectType == 19000)
                filter = String.Format("(Caption like '{0}') and (ObjectType = {1})", objectCaption, objectType);
            else
                filter = String.Format("(Name like '{0}') and (ObjectType = {1})", objectName, objectType);
            DataRow[] rows = objectsTable.Select(filter);
            if (rows.Length > 0)
            {
                int objectID = XmlHelper.GetIntAttrValue(objectNode, ExportImportXmlConsts.IDTagName, 0);
                // добавляемый объект уже присутствует
                // получаем ID этого объекта
                objectsTableIds.Rows.Add(objectID, rows[0]["ID"], objectType);
            }
        }

        #endregion

        #endregion
    } 
}
