using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Linq;
using Krista.FM.Server.XmlExportImporter.Helpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter
{
    class ClassifiersExportImport : ClsDataExportImportBase
    {
        public ClassifiersExportImport(IScheme scheme, ExportImportManager exportImportManager)
            : base(scheme, exportImportManager)
        {

        }

        #region реализация интерфейсов

        public override void ExportData(Stream stream, ImportPatams importParams, ExportImportClsParams exportImportClsParams)
        {
            schemeObject = exportImportClsParams.ExportImportObject;
            if (exportImportClsParams.SelectedRowsId != null && exportImportClsParams.SelectedRowsId.Count > 0)
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
            if (exportImportClsParams.NewIds != null && exportImportClsParams.NewIds.Count > 0)
                oldIds = exportImportClsParams.NewIds;
            schemeObject = exportImportClsParams.ExportImportObject;
            refVariant = exportImportClsParams.RefVariant;
            if (!string.IsNullOrEmpty(exportImportClsParams.DetailReferenceColumnName))
                detailReferenceColumnName = exportImportClsParams.DetailReferenceColumnName;
            InnerImportFromStream(stream, exportImportClsParams.ClsObjectKey, exportImportClsParams.DataSource, exportImportClsParams.Filter, exportImportClsParams.FilterParams);
            return newIds;
        }

        public override bool CheckXml(Stream stream, string objectUniqueKey, ref ImportPatams importParams, ref DataTable varianceTable)
        {
            return InnerCheckXml(stream, objectUniqueKey, ObjectType.Classifier, ref importParams, ref varianceTable);
        }

        #endregion

        #region получение свойств классификатора

        /// <summary>
        /// получение объекта схемы по ключу(гвиду) и получение атрибутов объекта
        /// </summary>
        /// <param name="objectUniqueKey"></param>
        internal override void GetSchemeObject(string objectUniqueKey)
        {
            // работает только для классификаторов и таблиц фактов
            // получаем объект схемы по ключу
            if (schemeObject == null)
                schemeObject = _scheme.Classifiers[objectUniqueKey];

            classifierAttributes = new List<IDataAttribute>();
            foreach (IDataAttribute attr in schemeObject.Attributes.Values)
            {
                classifierAttributes.Add(attr);
            }

            IClassifier classifier = schemeObject as IClassifier;
            if (classifier != null)
            {
                string levelWithTemplateName = string.Empty;
                switch (classifier.Levels.HierarchyType)
                {
                    case HierarchyType.ParentChild:
                        foreach (IDimensionLevel item in classifier.Levels.Values)
                        {
                            if (item.LevelType != LevelTypes.All)
                            {
                                levelWithTemplateName = item.ObjectKey;
                                break;
                            }
                        }
                        if (String.IsNullOrEmpty(levelWithTemplateName))
                            break;
                        refParentColumnName = classifier.Levels[levelWithTemplateName].ParentKey.Name;
                        parentColumnName = classifier.Levels[levelWithTemplateName].MemberKey.Name;
                        break;
                    case HierarchyType.Regular:
                        refParentColumnName = null;
                        parentColumnName = null;
                        break;
                }
            }
            if (string.IsNullOrEmpty(parentColumnName) && string.IsNullOrEmpty(refParentColumnName))
                isHierarchy = false;
            else
                isHierarchy = true;

            // тип объекта для протоколов
            switch (schemeObject.ClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    dataOperationsObjectType = DataOperationsObjectTypes.BridgeClassifier;
                    break;
                case ClassTypes.clsDataClassifier:
                    dataOperationsObjectType = DataOperationsObjectTypes.DataClassifier;
                    break;
                case ClassTypes.clsFactData:
                    dataOperationsObjectType = DataOperationsObjectTypes.FactTable;
                    break;
                case ClassTypes.clsFixedClassifier:
                    dataOperationsObjectType = DataOperationsObjectTypes.FixedClassifier;
                    break;
                case ClassTypes.Table:
                    dataOperationsObjectType = DataOperationsObjectTypes.SystemTable;
                    break;
            }
        }

        #endregion

        #region импорт данных

        private int InnerImportFromStream(Stream stream, string objectUniqueKey, int currentDataSource,
            string filter, IDbDataParameter[] filterParams)
        {
            importerObjectDataSource = currentDataSource;
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
                throw new Exception(exception.Message, exception.InnerException);
            }
            finally
            {
                if (protocol != null)
                    protocol.Dispose();
                if (db != null)
                    db.Dispose();
            }
        }


        private void InvalidateOlapObject()
        {
            try
            {
                _scheme.Processor.InvalidateDimension(
                    schemeObject,
                    "Krista.FM.Server.XmlExportImporter",
                    Krista.FM.Server.ProcessorLibrary.InvalidateReason.ClassifierChanged,
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
            // если текущий источник больше или равен -1, то проставляем его
            if (dataSourceIdIndex > 0 && importerObjectDataSource >= -1)
                newRow[dataSourceIdIndex] = importerObjectDataSource;
        }

        protected override void SaveRow(DataRow newRow, List<AttributesRelation> attributesRelationList, IDatabase db)
        {
            if (detailReferenceColumnName == string.Empty)
            {
                if (!innerImportParams.useOldID)
                {
                    int curentID = GetNewId(newRow, schemeObject.GeneratorName, schemeObject, db);
                    newIds.Add(Convert.ToInt32(newRow["ID"]), curentID);
                    newRow["ID"] = curentID;
                }
                // сохраняем одну запись
                SaveSingleDataRow(newRow, db, attributesRelationList);
            }
            else
            {
                if (newRow.IsNull(detailReferenceColumnName))
                {
                    return;
                }
                int oldReference = Convert.ToInt32(newRow[detailReferenceColumnName]);
                if (oldIds.ContainsKey(oldReference))
                {
                    newRow[detailReferenceColumnName] = oldIds[oldReference];
                    int curentID = GetNewId(newRow, schemeObject.GeneratorName, schemeObject, db);
                    newIds.Add(Convert.ToInt32(newRow["ID"]), curentID);
                    newRow["ID"] = curentID;
                    SaveSingleDataRow(newRow, db, attributesRelationList);
                }
            }
        }

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
                if (/*item.AssociationClassType == AssociationClassTypes.Bridge && */item.RoleBridge.ClassType != ClassTypes.clsFixedClassifier)
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

        #region Работа с иерархией и получением ID

        protected override void SaveHierarchyData(DataSet dataSet, IDatabase db,
            List<AttributesRelation> attributesRelationList)
        {
            ClearFirstLevelParents(dataSet, dataSet.Relations[0]);
            // если объект обладает свойством иерархии, то сохраняем в базу данные только после загрузки всех данных из XML
            foreach (DataTable table in dataSet.Tables)
                table.BeginLoadData();
            try
            {
                // сохраняем данные
                if (!innerImportParams.useOldID)
                    SetNewHierarchy(dataSet);
                SaveHierarchyData(dataSet.Tables[0], db, attributesRelationList);
            }
            finally
            {
                foreach (DataTable table in dataSet.Tables)
                {
                    //table.EndLoadData();
                }
            }
        }

        private Dictionary<long, long> OldHierarchyIds
        {
            get; set;
        }

        /// <summary>
        /// установка новых ID в иерархическом классификаторе
        /// </summary>
        private void SetNewHierarchy(DataSet ds)
        {
            OldHierarchyIds = new Dictionary<long, long>();

            foreach (DataRow row in ds.Tables[0].Rows.Cast<DataRow>().Where(w => w.IsNull(refParentColumnName)))
            {
                var newId = schemeObject.GetGeneratorNextValue;
                var oldId = Convert.ToInt32(row["ID"]);
                OldHierarchyIds.Add(oldId, newId);
                row["ID"] = newId;
            }

            foreach (DataRow row in ds.Tables[0].Select(string.Empty, "ID asc"))
            {
                var oldId = Convert.ToInt32(row["ID"]);
                if (row.IsNull(refParentColumnName))
                    continue;
                var newId = schemeObject.GetGeneratorNextValue;
                OldHierarchyIds.Add(oldId, newId);
                row["ID"] = newId;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row.IsNull(refParentColumnName))
                    continue;
                var oldRef = Convert.ToInt64(row[refParentColumnName]);
                row[refParentColumnName] = OldHierarchyIds[oldRef];
            }
        }

        protected override void GetNonGeneratedId(List<object> rowCellsValues)
        {
            int tmpId = Convert.ToInt32(rowCellsValues[0]);
            if (tmpId >= developerLowBoundRangeId)
            {
                if (tmpId > lastDeveloperId)
                    lastDeveloperId = tmpId;
            }
            else
            {
                if (tmpId > LastId)
                    LastId = tmpId;
            }
        }


        private int GetNewId(DataRow row, string generatorName, IEntity schemeObject, IDatabase db)
        {
            return schemeObject.GetGeneratorNextValue;
        }

        /// <summary>
        /// очищает сцылки на родителя у записей верхнего уровня, если такие есть
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="rel"></param>
        private void ClearFirstLevelParents(DataSet ds, DataRelation rel)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row.GetParentRow(rel) == null)
                {
                    row["ParentID"] = DBNull.Value;
                }
            }
            ds.AcceptChanges();
        }

        protected override string GetGeneratorName(IEntity schemeObject, bool developMode)
        {
            if (developMode)
                return schemeObject.DeveloperGeneratorName;

            string generatorName = string.Empty;
            string tableName = schemeObject.FullDBName;
            if (_scheme.SchemeDWH.FactoryName.ToUpper() != "SYSTEM.DATA.SQLCLIENT")
                generatorName = "g_" + tableName.Substring(0, tableName.Length > 28 ? 28 : tableName.Length);
            else
                generatorName = tableName.Substring(0, tableName.Length > 28 ? 28 : tableName.Length);
            return generatorName;
        }

        internal override void SetGeneratorValue(IDatabase db)
        {
            DatabaseHelper.SetNextId(_scheme, LastId, schemeObject, db);
        }

        #endregion

        #endregion
    }
}
