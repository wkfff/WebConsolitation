using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter
{
    class FactTablesExportImport : ClsDataExportImportBase
    {
        // реализация экспорта импорта и проверки XML таблиц фактов

        public FactTablesExportImport(IScheme scheme, ExportImportManager exportImportManager)
            : base(scheme, exportImportManager)
        {

        }

        #region внутренние переменные

        private int importerTaskId;
        private int importerPumpId;

        #endregion

        #region реализация интерфейсов

        public override void ExportData(Stream stream, ImportPatams importParams, ExportImportClsParams exportImportClsParams)
        {
            if (exportImportClsParams.NewIds != null && exportImportClsParams.NewIds.Count > 0)
                oldIds = exportImportClsParams.NewIds;
            schemeObject = exportImportClsParams.ExportImportObject;
            refVariant = exportImportClsParams.RefVariant;
            if (exportImportClsParams.SelectedRowsId.Count > 0)
            {
                InnerExportSelectedClsDataToXml(stream, exportImportClsParams.ClsObjectKey,
                    exportImportClsParams.DataSource, importParams, exportImportClsParams.Filter,
                    exportImportClsParams.FilterParams, exportImportClsParams.SelectedRowsId.ToArray());
            }
            else
            {
                InnerExportClsDataToXml(stream, exportImportClsParams.ClsObjectKey, exportImportClsParams.DataSource,
                    importParams, exportImportClsParams.Filter, exportImportClsParams.FilterParams);
            }
        }

        public override Dictionary<int, int> ImportData(Stream stream, ExportImportClsParams exportImportClsParams)
        {
            schemeObject = exportImportClsParams.ExportImportObject;
            refVariant = exportImportClsParams.RefVariant;
            InnerImportFactTable(stream, exportImportClsParams.ClsObjectKey,
                exportImportClsParams.DataSource, exportImportClsParams.PumpID,
                exportImportClsParams.TaskID, exportImportClsParams.Filter, exportImportClsParams.FilterParams);
            return newIds;
        }

        public override bool CheckXml(Stream stream, string objectUniqueKey, ref ImportPatams importParams, ref DataTable varianceTable)
        {
            return InnerCheckXml(stream, objectUniqueKey, ObjectType.FactTable, ref importParams, ref varianceTable);
        }

        #endregion

        #region получение свойств таблицы фактов

        /// <summary>
        /// получение объекта схемы по ключу и получение атрибутов объекта
        /// </summary>
        /// <param name="objectUniqueKey"></param>
        internal override void GetSchemeObject(string objectUniqueKey)
        {
            // работает только для классификаторов и таблиц фактов
            // получаем объект схемы по ключу
            schemeObject = _scheme.FactTables[objectUniqueKey];
            // пытаемся получить атрибуты объекта
            classifierAttributes = new List<IDataAttribute>();
            foreach (IDataAttribute attr in schemeObject.Attributes.Values)
            {
                classifierAttributes.Add(attr);
            }

            if (schemeObject.ObjectKey == "d3a9668b-0a65-4a6a-bca6-090768c822d0" ||
                schemeObject.ObjectKey == "fb029d1d-e648-46b4-8a1f-bff21ea0fbf5")
            {
                isHierarchy = true;
                refParentColumnName = "ParentID";
                parentColumnName = "ID";
            }
            else
                isHierarchy = false;

            dataOperationsObjectType = DataOperationsObjectTypes.FactTable;
        }

        #endregion

        #region импорт данных

        private int InnerImportFactTable(Stream stream, string objectUniqueKey, int currentDataSource,
            int currentPumpId, int currentTaskId, string filter, IDbDataParameter[] filterParams)
        {
            importerTaskId = currentTaskId;
            importerObjectDataSource = currentDataSource;
            importerPumpId = currentPumpId;
            IDatabase db = null;
            IClassifiersProtocol protocol = null;
            try
            {
                // получаем объект схемы, его атрибуты
                GetSchemeObject(objectUniqueKey);
                db = _scheme.SchemeDWH.DB;
                protocol = (IClassifiersProtocol)_scheme.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
                db.BeginTransaction();

                BeginImportProtocolMessage(protocol);

                ImportDataFromXML(stream, db, filter, filterParams, protocol);

                string protocolMessage = string.Format("Результаты операции импорта: добавлено записей: {0}; обновлено записей: {1}; удалено записей: {2}",
                    insertRowsCount, updateRowsCount, deleteRowsCount);
                SendMessageToClassifierProtocol(protocol, protocolMessage, ClassifiersEventKind.ceInformation);

                SuccefullEndImportProtocolMessage(protocol);
                db.Commit();

                InvalidateOlapObject();

                return importerObjectDataSource;
            }
            catch (Exception exception)
            {
                string message = exception.Message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                db.Rollback();
                SendMessageToClassifierProtocol(protocol, String.Format("Операция импорта завершилась с ошибкой \r\n {0}", message),
                    ClassifiersEventKind.ceError);
                throw exception;
            }
            finally
            {
                protocol.Dispose();
                db.Dispose();
            }
        }

        private void InvalidateOlapObject()
        {
            try
            {
                _scheme.Processor.InvalidatePartition(
                    schemeObject,
                    "Krista.FM.Server.XmlExportImporter",
                    ProcessorLibrary.InvalidateReason.ClassifierChanged,
                    schemeObject.OlapName);
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "При установки признака необходимости расчета для объекта \"{0}\" произошла ошибка: {1}",
                    schemeObject.FullName, e.Message);
            }
        }

        protected override void SetSystemRefs(DataRow newRow, int dataSourceIdIndex, int pumpIdIndex, int taskIdIndex)
        {
            // ставим текущий источник данных
            if (dataSourceIdIndex > 0 && importerObjectDataSource >= -1)
                newRow[dataSourceIdIndex] = importerObjectDataSource;
            // текущую закачку
            if (pumpIdIndex > 0 && importerPumpId >= -1)
                newRow[pumpIdIndex] = importerPumpId;
            // текущую задачу
            if (taskIdIndex > 0 && importerTaskId >= -1)
                newRow[taskIdIndex] = importerTaskId;
            // ссылка на вариант
            if (refVariant > -1 && newRow.Table.Columns.Contains("RefVariant"))
                newRow["RefVariant"] = refVariant;
        }

        protected override void SaveRow(DataRow newRow, List<AttributesRelation> attributesRelationList, IDatabase db)
        {
            // для таблиц фактов под SQL Server 2005 ID не получаем... вообще для таблиц фактов не надо устанавливать такой режим...
            int oldID = Convert.ToInt32(newRow["ID"]);
            if (_scheme.SchemeDWH.FactoryName.ToUpper() != "SYSTEM.DATA.SQLCLIENT")
            {
                int curentID = schemeObject.GetGeneratorNextValue;
                newIds.Add(oldID, curentID);
                newRow["ID"] = curentID;
            }
            else
                newRow["ID"] = DBNull.Value;

            // сохраняем одну запись
            SaveSingleDataRow(newRow, db, attributesRelationList);
            if (_scheme.SchemeDWH.FactoryName.ToUpper() == "SYSTEM.DATA.SQLCLIENT")
            {
                object queryResult = db.ExecQuery(string.Format("Select IDENT_CURRENT('{0}')", schemeObject.FullDBName), QueryResultTypes.Scalar);
                if (!(queryResult is DBNull))
                    newIds.Add(oldID, Convert.ToInt32(queryResult));
                newRow["ID"] = queryResult;
            }
        }

        /// <summary>
        /// получение необходимых объекттов для записи одной записи через запрос
        /// </summary>
        internal override void GetRowParams(List<AttributesRelation> attributesRelationList, DataRow row, IDatabase db,
            ref string[] fields, ref Dictionary<string, IDbDataParameter> dbParams)
        {
            int columnCount = row.Table.Columns.Count;
            IDataAttribute idAttribute = classifierAttributes[0];
            AttributesRelation idAttributeRel = attributesRelationList[0];
            if (string.Compare(_scheme.SchemeDWH.FactoryName, "SYSTEM.DATA.SQLCLIENT", true) == 0)
            {
                fields = new string[columnCount - 1];
                if (classifierAttributes[0].Name == "ID")
                {
                    classifierAttributes.RemoveAt(0);
                    
                }
                if (attributesRelationList[0].attrName == "ID")
                    attributesRelationList.RemoveAt(0);
            }
            else
                fields = new string[columnCount];
            int i = 0;
            foreach (DataColumn column in row.Table.Columns)
            {
                if (string.Compare(_scheme.SchemeDWH.FactoryName, "SYSTEM.DATA.SQLCLIENT", true) == 0 && 
                    column.ColumnName == "ID")
                    continue;
                fields[i] = column.ColumnName;
                object value = row[column];
                if (ExportImportDBHelper.GetDBTypeFromAttribute(classifierAttributes[i]) == DbType.Boolean)
                    if (!row.IsNull(column))
                        value = Convert.ToBoolean(value);
                dbParams.Add(fields[i], db.CreateParameter(fields[i], value,
                    ExportImportDBHelper.GetDBTypeFromAttribute(classifierAttributes[i])));
                i++;
            }

            if (string.Compare(_scheme.SchemeDWH.FactoryName, "SYSTEM.DATA.SQLCLIENT", true) == 0)
            {
                classifierAttributes.Insert(0, idAttribute);
                attributesRelationList.Insert(0, idAttributeRel);
            }
        }

        /// <summary>
        /// добавление записи в базу или в иерархический набор данных
        /// </summary>
        protected override void AddRow(List<AttributesRelation> attributesRelationList, DataSet dataSet,
            int dataSourceIdIndex, int pumpIdIndex, int taskIdIndex,
            List<int> refColumnsIndexes, List<object> rowCellsValues, IDatabase db)
        {
            // создаем строку с данными 
            DataRow newRow = dataSet.Tables[0].NewRow();
            newRow.BeginEdit();

            for (int i = 0; i <= attributesRelationList.Count - 1; i++)
            {
                AttributesRelation relation = attributesRelationList[i];
                if (relation.xmlAttributeIndex >= 0)
                {
                    object value = rowCellsValues[relation.xmlAttributeIndex];
                    if (value != null && !(value is DBNull))
                    {
                        if (relation.DBType == DbType.Boolean)
                        {
                            bool boolValue;
                            if (!Boolean.TryParse(value.ToString(), out boolValue))
                                value = Convert.ToInt32(value);
                            else
                                value = boolValue;

                        }
                        if (newRow.Table.Columns[relation.objectAttributeIndex].DataType == typeof(byte[]))
                            value = Convert.FromBase64String(value.ToString());
                        newRow[relation.objectAttributeIndex] = value;
                    }
                }
            }
            // ставим значения по умолчанию
            for (int index = 0; index <= newRow.ItemArray.Length - 1; index++)
            {
                if (newRow.IsNull(index) && classifierAttributes[index].DefaultValue != null)
                    newRow[index] = classifierAttributes[index].DefaultValue;
            }
            // чистим ссылки на сопоставимые
            // если импортируем с возможностью обновления, ничего не делаем
            // для таблиц фактов сохраняем ссылки такими какие они есть в xml
            /*
            if (!string.IsNullOrEmpty(uniqueAttributes) && (innerImportParams.refreshDataByUnique || innerImportParams.refreshDataByAttributes))
            {

            }
            else
                foreach (int i in refColumnsIndexes)
                    newRow[i] = -1;*/

            SetSystemRefs(newRow, dataSourceIdIndex, pumpIdIndex, taskIdIndex);

            newRow.EndEdit();
            // сохраняем строку
            if (!isHierarchy)
                SaveRow(newRow, attributesRelationList, db);
            else
                dataSet.Tables[0].Rows.Add(newRow);
        }

        protected override void SaveHierarchyData(DataSet dataSet, IDatabase db,
            List<AttributesRelation> attributesRelationList)
        {
            foreach (DataTable table in dataSet.Tables)
                table.BeginLoadData();
            try
            {
                if (string.Compare(_scheme.SchemeDWH.FactoryName, "SYSTEM.DATA.SQLCLIENT", true) == 0)
                {
                    Dictionary<int, int> parentIDs = new Dictionary<int, int>();
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        SaveRow(row, attributesRelationList, db);
                        int parentID = row.IsNull("ParentID") ? -1 : Convert.ToInt32(row["ParentID"]);
                        row["ParentID"] = DBNull.Value;
                        parentIDs.Add(Convert.ToInt32(row["ID"]), parentID);
                    }
                    foreach (KeyValuePair<int, int> kvp in parentIDs)
                    {
                        if (kvp.Value != -1)
                            db.ExecQuery(string.Format("update {0} set ParentID = {1} where id = {2}",
                                schemeObject.FullDBName, newIds[kvp.Value], kvp.Key), QueryResultTypes.NonQuery);
                    }
                }
                else
                {
                    RestoreHierarchyID(dataSet, schemeObject.GeneratorName, db);
                    // сохраняем данные
                    SaveHierarchyData(dataSet.Tables[0], db, attributesRelationList);
                }
            }
            finally
            {
                foreach (DataTable table in dataSet.Tables)
                {
                    table.EndLoadData();
                }
            }
        }

        /// <summary>
        /// восстановление ID в иерархическом классификаторе
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="generatorName"></param>
        /// <param name="db"></param>
        private void RestoreHierarchyID(DataSet ds, string generatorName, IDatabase db)
        {
            // Бежим по элементам верхнего уровня, если у них есть элементы нижнего уровня,
            // то вызываем рекурсивный метод, который проставляет все ID для этих элементов
            //GetNormalGeneratorValue(ds.Tables[0], db, generatorName);
            DataRelation rel = ds.Relations[0];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row.GetParentRow(rel) == null)
                {
                    int curentID = schemeObject.GetGeneratorNextValue;
                    RestoreHierarchyIDRecursion(row.GetChildRows(rel), ds.Relations[0], curentID);
                    newIds.Add(Convert.ToInt32(row["ID"]), curentID);
                    row["ID"] = curentID;
                }
            }
        }

        private void RestoreHierarchyIDRecursion(DataRow[] rows, DataRelation Rel, int ParentID)
        {
            // Бежим по элементам не самого верхнего уровня, и ставим в ссылку значение "ParentID"
            // Получаем текущее (следующее) значение генератора, запоминаем его
            // Если есть элементы уровнем ниже, то для них вызываем рекурсивно этот же метод
            // Записываем в колонку ID значение, которое запомнили
            foreach (DataRow row in rows)
            {
                if (row.RowState != DataRowState.Modified)
                {
                    row[refParentColumnName] = ParentID;
                    int curentID = schemeObject.GetGeneratorNextValue;
                    RestoreHierarchyIDRecursion(row.GetChildRows(Rel), Rel, curentID);
                    newIds.Add(Convert.ToInt32(row["ID"]), curentID);
                    row["ID"] = curentID;
                }
            }
        }

        protected override string GetGeneratorName(IEntity schemeObject, bool developMode)
        {
            return schemeObject.GeneratorName;
        }

        #endregion

        /// <summary>
        /// получение индекса поля с источником данных и набором индексов ссылок на
        /// сопоставимые классификаторы у классификаторов
        /// </summary>
        protected override void GetRefColumnsIndexes(ref int dataSourseIndex, ref int dataPumpIndex, ref int taskIndex, ref List<int> refAssociateColumns)
        {
            base.GetRefColumnsIndexes(ref dataSourseIndex, ref dataPumpIndex, ref taskIndex, ref refAssociateColumns);

            refAssociateColumns = new List<int>();
            // получаем наименования всех полей ссылок на сопоставимые классификаторы
            List<string> refClmnNames = new List<string>();

            foreach (IEntityAssociation item in schemeObject.Associations.Values)
            {
                IDataAttribute attr = schemeObject.Attributes[item.FullDBName];
                // берем все ссылки кроме ссылок на фиксированные классификаторы
                if (/*item.AssociationClassType == AssociationClassTypes.Link && */item.RoleBridge.ClassType != ClassTypes.clsFixedClassifier)
                    if (attr.Class == DataAttributeClassTypes.Reference && !attr.IsLookup)
                        refClmnNames.Add(item.FullDBName);
            }

            for (int i = 0; i <= classifierAttributes.Count - 1; i++)
            {
                if (classifierAttributes[i].Class == DataAttributeClassTypes.Reference)
                    foreach (string refName in refClmnNames)
                    {
                        if (refName.ToUpper() == classifierAttributes[i].Name.ToUpper())
                            refAssociateColumns.Add(i);
                    }
            }

        }
    }
}
