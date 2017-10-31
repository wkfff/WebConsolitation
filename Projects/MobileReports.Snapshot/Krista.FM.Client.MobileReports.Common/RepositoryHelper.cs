using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using System.Data;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Common.Xml;
using System.Diagnostics;

namespace Krista.FM.Client.MobileReports.Common
{
    /// <summary>
    /// ����� ������, ��� ���������� ������������� ��������� �����������
    /// </summary>
    public static class RepositoryHelper
    {
        /// <summary>
        /// �������� ��������� �����������
        /// </summary>
        public static CategoryInfo GetRepositoryStructure(IScheme scheme)
        {
            List<ReportInfo> reportsInfo = new List<ReportInfo>();
            List<CategoryInfo> categoritesInfo = new List<CategoryInfo>();

            ITemplatesRepository repository = scheme.TemplatesService.Repository;
            DataTable mobileTemplates = repository.GetTemplatesInfo(TemplateTypes.IPhone);

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                //�.�. ��������� � ���� ������ - �������� ���������� ��������, � ��� ���������� ��������� ����������� ���
                //������ ��������, ������� ��� ������ ����� � ������� ��� � ����.
                string sqlScript = "SELECT RefObjects, RefGroups, RefUsers FROM Permissions ORDER BY RefObjects";
                DataTable permissionsTable = db.SelectData(sqlScript, int.MaxValue);
                permissionsTable.TableName = "permissionsTable";

                //������� ������ ���� ������������� �� �������, �� ������� ����� ����������� �����, 
                //�.�. ��� ������� VARCHAR, � � ��� ����������� �����, � ���������� ����� �� ������, 
                //������������ ��������. ��� Oracl � MSSQL ��������� ������� ������, ����� ��������� ���.
                if (scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.SqlClient)
                    sqlScript = string.Format("SELECT ID, ObjectKey FROM Objects WHERE ObjectType = '{0}' ORDER BY CONVERT(int, ObjectKey)", (int)SysObjectsTypes.Template);
                else
                    sqlScript = string.Format("SELECT ID, ObjectKey FROM Objects WHERE ObjectType = '{0}' ORDER BY TO_NUMBER(ObjectKey)", (int)SysObjectsTypes.Template);
                DataTable objectsTable = db.SelectData(sqlScript, int.MaxValue);
                objectsTable.TableName = "objectsTable";

                sqlScript = string.Format("SELECT ID, Document FROM Templates WHERE RefTemplatesTypes = '{0}' ORDER BY ID", (int)TemplateTypes.IPhone);
                DataTable templatesTable = db.SelectData(sqlScript, int.MaxValue);
                templatesTable.TableName = "templatesTable";

                foreach (DataRow row in mobileTemplates.Rows)
                {
                    if (row[TemplateFields.Code] is DBNull)
                    {
                        Trace.TraceWarning("� �������� � ������: \"{0}\" �� ��������� ���� code", (string)row[TemplateFields.Name]);
                        continue;
                    }

                    BaseElementInfo baseElement = new BaseElementInfo();
                    baseElement.Id = Convert.ToInt32(row[TemplateFields.ID]);
                    baseElement.RefObject = GetSystemObjectID(objectsTable, baseElement.Id);

                    baseElement.ParentId = -1;
                    if (!(row[TemplateFields.ParentID] is DBNull))
                        baseElement.ParentId = Convert.ToInt32(row[TemplateFields.ParentID]);

                    baseElement.Code = ((string)row[TemplateFields.Code]).ToLower();

                    baseElement.Name = Consts.sqlNULL;
                    if (!(row[TemplateFields.Name] is DBNull))
                        baseElement.Name = (string)row[TemplateFields.Name];

                    baseElement.Description = Consts.sqlNULL;
                    if (!(row[TemplateFields.Description] is DBNull))
                        baseElement.Description = (string)row[TemplateFields.Description];

                    TemplateDocumentTypes documentType = GetDocumentType(row);
                    switch (documentType)
                    {
                        case TemplateDocumentTypes.Group:
                            {
                                CategoryInfo categoryInfo = new CategoryInfo(baseElement);
                                if (!categoritesInfo.Contains(categoryInfo))
                                {
                                    categoritesInfo.Add(categoryInfo);
                                    InitPermissions(permissionsTable, categoryInfo);
                                    InitTemplateDescription(templatesTable, categoryInfo);
                                }
                                break;
                            }
                        case TemplateDocumentTypes.WebReport:
                            {
                                ReportInfo reportInfo = new ReportInfo(baseElement);
                                if (!reportsInfo.Contains(reportInfo))
                                {
                                    reportsInfo.Add(reportInfo);
                                    InitPermissions(permissionsTable, reportInfo);
                                    InitTemplateDescription(templatesTable, reportInfo);
                                }
                                break;
                            }
                    }
                }
            }

            CategoryInfo rootCategory = new CategoryInfo();
            HierarchyCategoryStructure(rootCategory, reportsInfo, categoritesInfo);
            HierarchySettingsStructure(rootCategory, true);
            HierarchySettingsStructure(rootCategory, false);
            return rootCategory;
        }

        /// <summary>
        /// ������ �������� ����. ��� ������������ �����, ����������� �����. 
        /// ���� ����� ��������, ���� ����������, �������� �� � ���������.
        /// </summary>
        /// <param name="currentCategory">������� ���������</param>
        /// <param name="directOfParentToChildren">����������� �� ���� ���������� �����, 
        /// �� ��������� � �����, ��� ��������</param>
        public static void HierarchySettingsStructure(CategoryInfo currentCategory,
            bool directOfParentToChildren)
        {
            BaseElementInfo complementaryElement;
            BaseElementInfo complementedElement;

            foreach (ReportInfo report in currentCategory.ChildrenReports)
            {
                complementaryElement = directOfParentToChildren ? (BaseElementInfo)currentCategory : (BaseElementInfo)report;
                complementedElement = directOfParentToChildren ? (BaseElementInfo)report : (BaseElementInfo)currentCategory;
                ComplementPermissions(complementaryElement, complementedElement);

                if (complementedElement.TemplateType == MobileTemplateTypes.None)
                    complementedElement.TemplateType = complementaryElement.TemplateType;
            }

            foreach (CategoryInfo category in currentCategory.ChildrenCategory)
            {
                complementaryElement = directOfParentToChildren ? (BaseElementInfo)currentCategory : (BaseElementInfo)category;
                complementedElement = directOfParentToChildren ? (BaseElementInfo)category : (BaseElementInfo)currentCategory;

                if (!directOfParentToChildren)
                    HierarchySettingsStructure(category, directOfParentToChildren);

                ComplementPermissions(complementaryElement, complementedElement);

                if (directOfParentToChildren)
                {
                    if (complementedElement.TemplateType == MobileTemplateTypes.None)
                        complementedElement.TemplateType = complementaryElement.TemplateType;
                    HierarchySettingsStructure(category, directOfParentToChildren);
                }
            }
        }

        /// <summary>
        /// �������� ����� ��������
        /// </summary>
        /// <param name="complementaryElement">����������� �����</param>
        /// <param name="complementedElement">����������� �������</param>
        private static void ComplementPermissions(BaseElementInfo complementaryElement,
            BaseElementInfo complementedElement)
        {
            foreach (int groupPermission in complementaryElement.GroupsPermisions)
            {
                if (!complementedElement.GroupsPermisions.Contains(groupPermission))
                    complementedElement.GroupsPermisions.Add(groupPermission);
            }

            foreach (int userPermission in complementaryElement.UsersPermisions)
            {
                if (!complementedElement.UsersPermisions.Contains(userPermission))
                    complementedElement.UsersPermisions.Add(userPermission);
            }
        }

        /// <summary>
        /// ������� ������ ������� ������� � ��������� ���������, ����� �������� � currentTupleIndex � ����
        /// </summary>
        /// <param name="dataTable">��������������� �� columnName �������</param>
        /// <param name="currentTupleIndex">������ � �������� �������� �����</param>
        /// <param name="columnValue">������� ��������</param>
        /// <returns></returns>
        private static int GetStartTupleIndexForValue(DataTable dataTable, int currentTupleIndex, string columnName, 
            int columnValue)
        {
            int currentIndexValue = columnValue;
            if (currentTupleIndex > 0)
            {
                do
                {
                    currentTupleIndex--;
                    DataRow tuple = dataTable.Rows[currentTupleIndex];
                    currentIndexValue = Convert.ToInt32(tuple[columnName]);
                }
                while ((currentTupleIndex > 0) && (currentIndexValue == columnValue));
            }
            return (currentIndexValue == columnValue) ? currentTupleIndex : ++currentTupleIndex;
        }

        /// <summary>
        /// ������� ���������, � ��������������� �� "columnName" �������, ���� ������ ������� �������� � 
        /// "columnName" �������� ����� "columnValue"
        /// </summary>
        /// <param name="dataTable">�������������� �� "columnName" �������</param>
        /// <param name="startIndex">������ � �������� �������� ������</param>
        /// <param name="endIndex">������ �� �������� ����</param>
        /// <param name="columnName">��� �������</param>
        /// <param name="columnValue">�������� ������ � ������� "columnName"</param>
        /// <returns></returns>
        private static int GetIndexTupleInTableByColumnValue(DataTable dataTable, int startIndex, int endIndex, 
            string columnName,  int columnValue)
        {
            DataRow tuple = dataTable.Rows[startIndex];
            int startIndexValue = Convert.ToInt32(tuple[columnName]);

            tuple = dataTable.Rows[endIndex];
            int endIndexValue = Convert.ToInt32(tuple[columnName]);

            if (startIndexValue == columnValue)
                return GetStartTupleIndexForValue(dataTable, startIndex, columnName, columnValue);
            if (endIndexValue == columnValue)
                return GetStartTupleIndexForValue(dataTable, endIndex, columnName, columnValue);

            //��������� �� ����� �� �� �������� ������ ������� ��������, 
            //��� �� ��������� ���������� �������� � ��������� ������, �� ������ ���� ������� 2
            if ((startIndexValue < columnValue) && (columnValue < endIndexValue) && ((endIndex - startIndex) > 1))
            {
                int middleIndex = (endIndex - startIndex) / 2 + startIndex;
                tuple = dataTable.Rows[middleIndex];
                int middleIndexValue = Convert.ToInt32(tuple[columnName]);

                if (middleIndexValue > columnValue)
                    endIndex = middleIndex;
                else
                    startIndex = middleIndex;
                return GetIndexTupleInTableByColumnValue(dataTable, startIndex, endIndex, columnName, columnValue);
            }
            Trace.TraceError("� �������: {0}, �� ������� ��������:{1} ��� �������:{2}", dataTable.TableName, columnName, columnValue);
            return -1;
        }

        /// <summary>
        /// �������� ��������� id �������
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private static int GetSystemObjectID(DataTable objectsTable, int objectKey)
        {
            int tupleIndex = GetIndexTupleInTableByColumnValue(objectsTable, 0, objectsTable.Rows.Count - 1,
                "ObjectKey", objectKey);
            int result = -1;
            
            if ((tupleIndex >= 0) && (tupleIndex < objectsTable.Rows.Count))
            {
                DataRow refObjectRow = objectsTable.Rows[tupleIndex];
                result = Convert.ToInt32(refObjectRow[TemplateFields.ID]);
            }
            return result;
        }

        /// <summary>
        /// �������������� ����� ��������
        /// </summary>
        /// <param name="db"></param>
        /// <param name="elementInfo"></param>
        private static void InitPermissions(DataTable permissionsTable, BaseElementInfo elementInfo)
        {
            int startTupleIndex = GetIndexTupleInTableByColumnValue(permissionsTable, 0,
                permissionsTable.Rows.Count - 1, "RefObjects", elementInfo.RefObject);

            if ((startTupleIndex >= 0) && (startTupleIndex < permissionsTable.Rows.Count))
            {
                for (int i = startTupleIndex; i < permissionsTable.Rows.Count; i++)
                {
                    DataRow currentTuple = permissionsTable.Rows[i];
                    int refObjectsValue = Convert.ToInt32(currentTuple["RefObjects"]);
                    if (refObjectsValue == elementInfo.RefObject)
                    {
                        if (!(currentTuple["RefGroups"] is DBNull))
                        {
                            int groupID = Convert.ToInt32(currentTuple["RefGroups"]);
                            if (!elementInfo.GroupsPermisions.Contains(groupID))
                                elementInfo.GroupsPermisions.Add(groupID);
                        }

                        if (!(currentTuple["RefUsers"] is DBNull))
                        {
                            int userID = Convert.ToInt32(currentTuple["RefUsers"]);
                            if (!elementInfo.UsersPermisions.Contains(userID))
                                elementInfo.UsersPermisions.Add(userID);
                        }
                    }
                    else
                        return;
                }
            }
        }

        public static void InitTemplateDescription(DataTable templatesTable, BaseElementInfo element)
        {
            int tupleIndex = GetIndexTupleInTableByColumnValue(templatesTable, 0, templatesTable.Rows.Count - 1,
                TemplateFields.ID, element.Id);

            DataRow tuple = templatesTable.Rows[tupleIndex];
            if (!(tuple[TemplateFields.Document] is DBNull))
            {
                byte[] document = (byte[])tuple[TemplateFields.Document];

                if (document != null)
                {
                    string xmlDescriptor = Encoding.UTF8.GetString(document);
                    IPhoteTemplateDescriptor descriptor = XmlHelper.XmlStr2Obj<IPhoteTemplateDescriptor>(xmlDescriptor);
                    element.SetTemplateDescriptor(descriptor);
                }
            }
        }

        /// <summary>
        /// ����������� ���������
        /// </summary>
        /// <param name="currentLeaf">������� ���������</param>
        /// <param name="reportsInfo">������ � ��������</param>
        /// <param name="categoritesInfo">������ � �����������</param>
        public static void HierarchyCategoryStructure(CategoryInfo currentLeaf,
            List<ReportInfo> reportsInfo, List<CategoryInfo> categoritesInfo)
        {
            //������ ������ ������������� ������ ���������
            foreach (ReportInfo report in reportsInfo)
            {
                if (report.ParentId == currentLeaf.Id)
                {
                    currentLeaf.ChildrenReports.Add(report);
                    report.Parent = currentLeaf;
                }
            }

            //������ �������� ���������
            foreach (CategoryInfo category in categoritesInfo)
            {
                if ((category != currentLeaf) && (category.ParentId == currentLeaf.Id))
                {
                    currentLeaf.ChildrenCategory.Add(category);
                    category.Parent = currentLeaf;
                    HierarchyCategoryStructure(category, reportsInfo, categoritesInfo);
                }
            }
        }

        /// <summary>
        /// �������� ��� ��������
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static TemplateDocumentTypes GetDocumentType(DataRow row)
        {
            int value = 0;
            int.TryParse(row[TemplateFields.Type].ToString(), out value);
            return (TemplateDocumentTypes)value;
        }
    }
}
