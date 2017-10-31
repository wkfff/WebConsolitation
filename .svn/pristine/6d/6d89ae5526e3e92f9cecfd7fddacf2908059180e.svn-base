using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Scheme.Classes;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Работа с фиксированными записями
    /// </summary>
    partial class Classifier
    {
        /// <summary>
        /// Фиксированные строки для вставки
        /// </summary>
        private string xmlFixedRowsData = String.Empty;

        #region XML -> DataTable
        
        private List<string> GetFixedRowsTableAttributes()
        {
            List<string> attributesList = new List<string>();
            foreach (DataAttribute attr in this.Attributes.Values)
            {
                if (attr.Name == DataAttribute.RowTypeColumnName ||
                    attr.Class == DataAttributeClassTypes.Reference)
                    continue;
                attributesList.Add(attr.Name);
            }
            return attributesList;
        }

        private DataTable GetFixedRowsEmptyDataTable()
        {
            DataTable t = new DataTable("Table");

            foreach (DataAttribute attr in Attributes.Values)
            {
                if (attr.Name == DataAttribute.RowTypeColumnName ||
                    attr.Class == DataAttributeClassTypes.Reference)
                    continue;

                t.Columns.Add(new DataColumn(attr.Name, DataAttribute.TypeOfAttribute(attr.Type)));
            }

            DataRow row = t.NewRow();
            foreach (DataColumn column in t.Columns)
            {
                EntityDataAttribute da = (EntityDataAttribute)DataAttributeCollection.GetAttributeByKeyName(Attributes, column.ColumnName, column.ColumnName);
                object defaultValue = da.GetDefaultValue;
                if (String.IsNullOrEmpty(Convert.ToString(defaultValue)))
                    defaultValue = DataAttribute.GetStandardDefaultValue(da, "Значение не указано");
                row[column.ColumnName] = defaultValue;
            }
            t.Rows.Add(row);

            return t;
        }

        private DataTable GetFixedRowsDataTable(XmlNode xmlValuesNode)
        {
            DataTable dt = new DataTable("Table");
            foreach (DataAttribute attr in this.Attributes.Values)
            {
                if (attr.Name == DataAttribute.RowTypeColumnName ||
                    attr.Class == DataAttributeClassTypes.Reference)
                    continue;

                dt.Columns.Add(new DataColumn(attr.Name, DataAttribute.TypeOfAttribute(attr.Type)));
            }

            object[] row = new object[GetFixedRowsTableAttributes().Count];

            XmlNamespaceManager nm = new XmlNamespaceManager(xmlValuesNode.OwnerDocument.NameTable);
            nm.AddNamespace("xmluml", "");
            foreach (XmlNode item in xmlValuesNode.SelectNodes("Row"))
            {
                int i = 0;
                row[i++] = Convert.ToInt32(item.Attributes["id"].Value);
                //row[i++] = 0;
                foreach (DataAttribute attr in this.Attributes.Values)
                {
                    if (attr.Name == DataAttribute.IDColumnName ||
                        attr.Name == DataAttribute.RowTypeColumnName ||
                        attr.Class == DataAttributeClassTypes.Reference)
                        continue;
                    XmlNode xmlNode = item.SelectSingleNode(String.Format("Column[@name='{0}']", attr.Name));
                    // пытаемся вставить значение по умолчанию
                    if (xmlNode == null && attr.DefaultValue != null)
                        row[i++] = attr.DefaultValue;
                    else if (xmlNode == null && !attr.IsNullable)
                        Trace.TraceWarning(
                            string.Format("Обновление фиксированных строк. Не найдено обязательное поле {0}. Классификатор: {1}. Пакет: {2}", attr.Name,
                                          this.FullCaption, this.ParentPackage.Name));
                    else
                    {
                        if (xmlNode != null && xmlNode.Attributes["value"] != null && !String.IsNullOrEmpty(xmlNode.Attributes["value"].Value))
                            row[i++] = xmlNode.Attributes["value"].Value;
                        else
                            row[i++] = null;
                    }

                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        private XmlNode GetFixedRowsNodeListFromFile(string pathFile)
        {
            string errMsg;

            XmlDocument doc = new XmlDocument();
            doc.Load(SchemeClass.Instance.BaseDirectory + "\\" + pathFile);
            // добавляем атрибут пространства имен. Без пространств имен проверка невозможна!
            XmlAttribute attr = doc.DocumentElement.SetAttributeNode("xmlns", "http://www.w3.org/2000/xmlns/");
            attr.Value = "xmluml";

            if (!Validator.Validate(doc.InnerXml, "ServerConfiguration.xsd", "xmluml", out errMsg))
                throw new Exception(errMsg);

            return doc.SelectSingleNode(String.Format("/ServerConfiguration/{0}/Data/Values", tagElementName));
        }

        private XmlNode GetFixedRowsValues()
        {
            string errMsg;

            if (String.IsNullOrEmpty(Instance.xmlFixedRowsData))
            {
                if (ClassType == ClassTypes.clsFixedClassifier)
                    SetInstance.xmlFixedRowsData = SaveDataTable2XmlString(GetFixedRowsEmptyDataTable());
                else
                    return null;// SetInstance.xmlFixedRowsData = SaveDataTable2XmlString(GetFixedRowsEmptyDataTable());
            }

            XmlDocument doc = new XmlDocument();
            string xmlConfig = String.Format(
                "<ServerConfiguration><FixedValues name=\"{0}\"><Data>{1}</Data></FixedValues></ServerConfiguration>", 
                FullName, Instance.xmlFixedRowsData);
            doc.LoadXml(xmlConfig);
            // добавляем атрибут пространства имен. Без пространств имен проверка невозможна!
            XmlAttribute attr = doc.DocumentElement.SetAttributeNode("xmlns", "http://www.w3.org/2000/xmlns/");
            attr.Value = "xmluml";

            if (!Validator.Validate(doc.InnerXml, "ServerConfiguration.xsd", "xmluml", out errMsg))
                throw new Exception(errMsg);

            return doc.SelectSingleNode("/ServerConfiguration/FixedValues/Data/Values");
        }

        #endregion XML -> DataTable

        #region DataTable -> XML

        private void SaveDataTable2XmlNode(DataTable dt, XmlNode xmlNode)
        {
            // 1. Определяем в каких столбцах есть данные для сохранения
            List<string> columns = new List<string>();
            foreach (DataColumn column in dt.Columns)
            {
                if (column.ColumnName == DataAttribute.IDColumnName ||
                    column.ColumnName == DataAttribute.RowTypeColumnName)
                    continue;
                
                DataRow[] rows = dt.Select(String.Format("{0} is not null", column.ColumnName));
                if (rows.Length > 0)
                    columns.Add(column.ColumnName);
            }

            // 2. Сохраняем строки в XML узел
            xmlNode.InnerXml = "";
            foreach (DataRow row in dt.Rows)
            {
                XmlNode xmlRowNode = XmlHelper.AddChildNode(xmlNode, "Row", new string[] { "id", Convert.ToString(row[DataAttribute.IDColumnName]) });
                foreach (string columnName in columns)
                {
                    XmlHelper.AddChildNode(xmlRowNode, "Column", 
                        new string[] { "name", columnName },
                        new string[] { "value", Convert.ToString(row[columnName]) });
                }
            }
        }

        #endregion DataTable -> XML

        /// <summary>
        /// Обновляет фиксированные значения в базе из таблицы dt
        /// </summary>
        /// <param name="dt">Таблица в которой находятся данные для обновления</param>
        private void UpdateDataRows(ModificationContext context, DataTable dt)
        {
			if (SchemeClass.Instance.Server.GetConfigurationParameter(SchemeClass.UseNullScriptingEngineImplParamName) != null)
				return;
            
			DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            string query = String.Format("select {0} from {1}",
                String.Join(", ", GetFixedRowsTableAttributes().ToArray()), this.FullDBName);
            IDataUpdater du = context.Database.GetDataUpdater(query);

            du.Update(ref ds);
        }

        /// <summary>
        /// Вставляет в таблицу фиксированные записи из XML определения
        /// </summary>
        protected void InsertData(ModificationContext context)
        {
            XmlNode xmlNode = GetFixedRowsValues();
            if (xmlNode != null)
                UpdateDataRows(context, GetFixedRowsDataTable(xmlNode));
        }

        /// <summary>
        /// Устанавливает конфигурацию фиксированных значений
        /// </summary>
        /// <param name="xmlConfigiration">Конфигурация фиксированных значений</param>
        internal void SetFixedRowsXmlConfigiration(string xmlConfigiration)
        {
            SetInstance.xmlFixedRowsData = xmlConfigiration;
        }

        /// <summary>
        /// Возвращает таблицу фиксированных записей
        /// </summary>
        /// <returns>Таблица фиксированных записей</returns>
        public DataTable GetFixedRowsTable()
        {
            XmlNode xmlNode = GetFixedRowsValues();
            if (xmlNode != null)
                return GetFixedRowsDataTable(xmlNode);
            else
                return GetFixedRowsEmptyDataTable();
        }

        /// <summary>
        /// Устанавливает таблицу фиксированных записей
        /// </summary>
        public void SetFixedRowsTable(DataTable dt)
        {
            if (dt.GetChanges().Rows.Count > 0)
            {
                XmlNode xmlNode = GetFixedRowsValues();
                if (xmlNode == null)
                    throw new NotImplementedException("Данная возможность не реализована.");
                SaveDataTable2XmlNode(dt, xmlNode);
                SetInstance.xmlFixedRowsData = xmlNode.OuterXml;

                dt.AcceptChanges();
            }
        }

        internal string SaveDataTable2XmlString(DataTable dt)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("Values");
            SaveDataTable2XmlNode(dt, node);
            return node.OuterXml;
        }

        /// <summary>
        /// Находит отличия фиксированных значений
        /// </summary>
        /// <param name="toClassifier"></param>
        /// <returns></returns>
        private ModificationItem GetChangesFixedRows(Classifier toClassifier)
        {
            if (String.IsNullOrEmpty(xmlFixedRowsData) && String.IsNullOrEmpty(toClassifier.xmlFixedRowsData))
                return null;

            DataTable toTable = toClassifier.GetFixedRowsTable();
            DataTable fromTable = GetFixedRowsTable();
            SynchronizeColumns(fromTable, toTable);

            DataRowModificationItem miFixedRows = new DataRowModificationItem(ModificationTypes.Modify,
                                                                              "Изменение фиксированных значений",
                                                                              this, toClassifier);
            miFixedRows.XmlConfigiration = toClassifier.xmlFixedRowsData;

            return DataTableModifications.GetChangesDataTable(miFixedRows, FullDBName, DataAttribute.IDColumnName,
                                                              toTable, fromTable, null);
        }

        /// <summary>
        /// Добавляет новые столбцы и удаляет удаленные:)
        /// </summary>
        /// <param name="fromTable"></param>
        /// <param name="toTable"></param>
        private static void SynchronizeColumns(DataTable fromTable, DataTable toTable)
        {
            List<string> newColumns = new List<string>();
            List<string> removeColumns = new List<string>();
            foreach (DataColumn column in toTable.Columns)
            {
                newColumns.Add(column.ColumnName);
            }

            foreach (DataColumn column in fromTable.Columns)
            {
                newColumns.Remove(column.ColumnName);
                if (!toTable.Columns.Contains(column.ColumnName))
                {
                    removeColumns.Add(column.ColumnName);
                }
            }

            foreach (string name in removeColumns)
            {
                // удаляем старые столбцы
                fromTable.Columns.Remove(name);
            }

            foreach (string name in newColumns)
            {
                // добавляем новые столбцы
                fromTable.Columns.Add(name, toTable.Columns[name].DataType);
            }
        }
    }
}
