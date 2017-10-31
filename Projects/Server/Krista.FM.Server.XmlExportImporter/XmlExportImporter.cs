using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter
{
    public partial class XmlExportImporter : DisposableObject, IXmlExportImporter
    {

        #region конструктор

        public XmlExportImporter(IScheme scheme)
        {
            _scheme = scheme;
        }

        #endregion

        
        #region внутренние переменные

        private IScheme _scheme;

        private IDataReader reader;

        // списки атрибутов 
        List<IDataAttribute> classifierAttributes;

        List<IDataAttribute> dataRoleAttributes;
        List<IDataAttribute> bridgeRoleAttributes;

        ImportPatams innerImportParams;
        // параметры наименований и семантик
        List<string> objectName;
        List<string> rusObjectName;
        List<string> objectSemantic;
        List<string> rusObjectSemantic;

        // атрибуты по 
        string uniqueAttributes = string.Empty;

        // параметры иерархии
        string parentColumnName = string.Empty;
        string refParentColumnName = string.Empty;
        bool isHierarchy;

        // параметры источника данных
        int importerObjectDataSource = -1;
        // Id задачи для таблиц фактов
        int importerTaskId = -1;

        int importerPumpId = -1;
        // название источника
        string dataSourcesName;

        IDataSource dataSourсe;

        ICommonDBObject schemeObject;

        IConversionTable conversionTable;

        string filterConversionTable = string.Empty;
        // показывает, экспортируем ли мы все записи или только выделенные
        private bool exportSelectedRows = false;

        private string conversionRule;

        private int lastId = 0;
        private int lastDeveloperId = 0;

        private static int developerLowBoundRangeId = 1000000000;

        private int updateRowsCount = 0;
        private int insertRowsCount = 0;
        private int deleteRowsCount = 0;

        #endregion


        #region реализация интерфейса IXmlExportImporter

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region экспорты

        public void ExportSelectedClsDataToXml(Stream stream, string objectUniqueKey, int currentDataSource, ImportPatams importParams, string filter, IDbDataParameter[] filterParams, int[] selectedRowsID)
        {
            InnerExportSelectedClsDataToXml(stream, objectUniqueKey, currentDataSource, importParams, filter, filterParams, selectedRowsID);
        }

        public void ExportSelectedRowsFromConversion(Stream stream, string associationName, string ruleName, ImportPatams importParams, int[] selectedRowsID)
        {
            InnerExportSelectedConvessionTable(stream, associationName, ruleName, importParams, selectedRowsID);
        }

        public void ExportConvertionTableXMLToStream(Stream stream, string associationName, string ruleName, ImportPatams importParams)
        {
            InnerExportConversionTable(stream, associationName, ruleName, importParams);
        }

        public void ExportClsDataToXml(Stream stream, string objectUniqueKey, int currentDataSource, ImportPatams importParams, string filter, IDbDataParameter[] filterParams)
        {
            InnerExportClsDataToXml(stream, objectUniqueKey, currentDataSource, importParams, filter, filterParams);
        }

        #endregion

        #region импорты

        public void ImportConversionTableFromStream(Stream stream, string associationName, string ruleName)
        {
            InnerImportConversionTable(stream, associationName, ruleName);
        }

        public int ImportClassifierFromXml(Stream stream, string objectUniqueKey, int currentDataSource, string filter, IDbDataParameter[] filterParams)
        {
            InnerImportFromStream(stream, objectUniqueKey, currentDataSource, filter, filterParams);
            return importerObjectDataSource;
        }

        public int ImportFactDataFromXml(Stream stream, string objectUniqueKey, int currentDataSource, int currentPumpId, int currentTaskId, string filter, IDbDataParameter[] filterParams)
        {
            InnerImportFactTable(stream, objectUniqueKey, currentDataSource, currentPumpId, currentTaskId, filter, filterParams);
            return importerObjectDataSource;
        }

        #endregion

        #endregion


        #region экспорт данных

        private void InnerExportClsDataToXml(Stream stream, string objectUniqueKey, int currentDataSource, ImportPatams importParams, string filter, IDbDataParameter[] filterParams)
        {
            IDbConnection connection = ((IConnectionProvider)_scheme.SchemeDWH).Connection;

            try
            {
                // получаем объект и его атрибуты
                GetSchemeObject(objectUniqueKey);

                dataSourсe = _scheme.DataSourceManager.DataSources[currentDataSource];
                dataSourcesName = _scheme.DataSourceManager.GetDataSourceName(currentDataSource);

                // получаем параметры для импорта
                innerImportParams = importParams;
                // строим запрос для получения данных 
                string[] tableFields = new string[classifierAttributes.Count];
                for (int i = 0; i <= classifierAttributes.Count - 1; i++)
                {
                    tableFields[i] = classifierAttributes[i].Name;
                }
                string query = String.Format("select {0} from {1}", String.Join(",", tableFields), schemeObject.FullDBName);
                if (filter != string.Empty)
                    query = String.Concat(query, " where ", filter);
                if (isHierarchy)
                    query = String.Concat(query, String.Format(" order by {0}", refParentColumnName));
                else
                    if (parentColumnName != string.Empty)
                        query = String.Concat(query, String.Format(" order by {0}", parentColumnName));
                    else
                        query = String.Concat(query, " order by ID");
                IDatabase db = _scheme.SchemeDWH.DB;
                try
                {
                    // получаем объект для чтения данных
                    reader = GetDataReader(db, connection, query, filterParams);
                }
                finally
                {
                    db.Dispose();
                }
                int tick = Environment.TickCount;
                SaveDataToStream(stream, ObjectType.Classifier);
                tick = Environment.TickCount - tick;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        private void InnerExportConversionTable(Stream stream, string associationName, string ruleName, ImportPatams importParams)
        {
            // получаем атрибуты таблицы перекодировки
            GetConversionTableAttributes(associationName, ruleName);

            // получаем параметры импорта
            innerImportParams = importParams;
            // никаких параметров специальных не надо... с ID вообще не работаем
            SaveDataToStream(stream, ObjectType.ConversionTable);
        }

        private void InnerExportSelectedClsDataToXml(Stream stream, string objectUniqueKey, int currentDataSource, ImportPatams importParams, string filter, IDbDataParameter[] filterParams, int[] selectedRowsID)
        {
            // формируем дополнительный фильтр
            string[] filterID = new string[selectedRowsID.Length];
            for (int i = 0; i <= selectedRowsID.Length - 1; i++)
            {
                filterID[i] = string.Format("(ID = {0})", selectedRowsID[i]);
            }
            filter = String.Concat(filter, string.Format(" and ({0})", string.Join(" or ", filterID)));
            // так как экспортируем не все записи, то иерархию если есть удаляем
            exportSelectedRows = true;
            ExportClsDataToXml(stream, objectUniqueKey, currentDataSource, importParams, filter, filterParams);
        }


        private void InnerExportSelectedConvessionTable(Stream stream, string associationName, string ruleName, ImportPatams importParams, int[] selectedRowsID)
        {
            string[] filterID = new string[selectedRowsID.Length];
            for (int i = 0; i <= selectedRowsID.Length - 1; i++)
            {
                filterID[i] = string.Format("(ID = {0})", selectedRowsID[i]);
            }
            filterConversionTable = string.Join(" or ", filterID);
            ExportConvertionTableXMLToStream(stream, associationName, ruleName, importParams);
        }

        #region сохранение различных параметров

        /// <summary>
        /// запись атрибутов объекта схемы в XML
        /// </summary>
        /// <param name="attributesList"></param>
        /// <param name="xmlWriter"></param>
        private void SaveSchemeAttributesToXML(List<IDataAttribute> attributesList, XmlWriter xmlWriter)
        {
            foreach (IDataAttribute attr in attributesList)
            {
                xmlWriter.WriteStartElement(XmlConsts.attributeElement);
                if (attr != null)
                {
                    CreateXMLAttribute(xmlWriter, XmlConsts.nameAttribute, attr.Name);
                    if (attr.Caption != string.Empty)
                        CreateXMLAttribute(xmlWriter, XmlConsts.rusNameAttribute, attr.Caption);
                    else
                        CreateXMLAttribute(xmlWriter, XmlConsts.rusNameAttribute, GetDataObjSemanticRus(this.schemeObject)
                            + GetBridgeClsCaptionByRefName((IEntity)this.schemeObject, attr.Name));
                    CreateXMLAttribute(xmlWriter, XmlConsts.sizeAttribute, attr.Size.ToString());
                    CreateXMLAttribute(xmlWriter, XmlConsts.typeAttribute, attr.Type.ToString());
                    CreateXMLAttribute(xmlWriter, XmlConsts.nullableAttribute, attr.IsNullable.ToString());
                    if (attr.DefaultValue != null)
                        CreateXMLAttribute(xmlWriter, XmlConsts.defaultValueAttribute, attr.DefaultValue.ToString());
                }
                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// запись параметров источника данных в XML
        /// </summary>
        /// <param name="dataSourse"></param>
        /// <param name="xmlWriter"></param>
        private void SaveDataSourceParamsToXML(IDataSource dataSourse, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(XmlConsts.usedDataSourcesNode);
            xmlWriter.WriteStartElement(XmlConsts.dataSourceElement);
            CreateXMLAttribute(xmlWriter, XmlConsts.idAttribute, dataSourse.ID.ToString());
            CreateXMLAttribute(xmlWriter, XmlConsts.suppplierCodeAttribute, dataSourse.SupplierCode.ToString());
            CreateXMLAttribute(xmlWriter, XmlConsts.dataCodeAttribute, dataSourse.DataCode);
            CreateXMLAttribute(xmlWriter, XmlConsts.dataNameAttribute, dataSourse.DataName);
            CreateXMLAttribute(xmlWriter, XmlConsts.kindsOfParamsAttribute, dataSourse.ParametersType.ToString());
            CreateXMLAttribute(xmlWriter, XmlConsts.nameAttribute, dataSourse.BudgetName);
            CreateXMLAttribute(xmlWriter, XmlConsts.yearAttribute, dataSourse.Year.ToString());
            CreateXMLAttribute(xmlWriter, XmlConsts.monthAttribute, dataSourse.Month.ToString());
            CreateXMLAttribute(xmlWriter, XmlConsts.variantAttribute, dataSourse.Variant);
            CreateXMLAttribute(xmlWriter, XmlConsts.quarterAttribute, dataSourse.Quarter.ToString());
            CreateXMLAttribute(xmlWriter, XmlConsts.territoryAttribute, dataSourse.Territory);
            CreateXMLAttribute(xmlWriter, XmlConsts.dataSourceNameAttribute, dataSourcesName);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// сохранение параметров импорта
        /// </summary>
        /// <param name="writer"></param>
        private void SaveImportParams(XmlWriter writer)
        {
            writer.WriteStartElement(XmlConsts.settingsElement);
            CreateXMLAttribute(writer, XmlConsts.useOldIDAttribute, innerImportParams.useOldID.ToString());
            CreateXMLAttribute(writer, XmlConsts.deleteDataBeforeImportAttribute, innerImportParams.deleteDataBeforeImport.ToString());
            CreateXMLAttribute(writer, XmlConsts.deleteDevelopData, innerImportParams.deleteDeveloperData.ToString());
            CreateXMLAttribute(writer, XmlConsts.useInnerUniqueAttributesAttribute, innerImportParams.refreshDataByAttributes.ToString());
            CreateXMLAttribute(writer, XmlConsts.useSchemeUniqueAttributesAttribute, innerImportParams.refreshDataByUnique.ToString());
            CreateXMLAttribute(writer, XmlConsts.restoreDataSourceAttribute, innerImportParams.restoreDataSource.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// сохранение наименования и семантики объекта в виде атрибутов XML
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="objectName"></param>
        /// <param name="objectSemantic"></param>
        private void CreateObjectAttributeAttributes(XmlWriter writer, string objectName,
            string rusObjectName, string objectSemantic, string rusObjectSemantic)
        {
            // сюда же пишем русские наименования семантики и наименования объекта
            CreateXMLAttribute(writer, XmlConsts.nameAttribute, objectName);
            CreateXMLAttribute(writer, XmlConsts.rusNameAttribute, rusObjectName);
            CreateXMLAttribute(writer, XmlConsts.semanticAttribute, objectSemantic);
            CreateXMLAttribute(writer, XmlConsts.rusSemanticAttribute, rusObjectSemantic);
        }

        #endregion

        #region запись блока данных в XML

        /// <summary>
        /// запись данных перекодировки в XML
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="conversionTable"></param>
        private void WriteConvertionTableData(XmlWriter writer, IConversionTable conversionTable)
        {
            IDataUpdater adapter = conversionTable.GetDataUpdater();
            DataTable dt = new DataTable();
            adapter.Fill(ref dt);

            writer.WriteStartElement(XmlConsts.dataNode);

            // начало таблицы с данными
            writer.WriteStartElement(XmlConsts.dataTableNode);
            // количество атрибутов
            int attrCount = dataRoleAttributes.Count + bridgeRoleAttributes.Count;
            object[] values = new object[attrCount];
            int tick = Environment.TickCount;
            DataRow[] rows = null;
            if (filterConversionTable != string.Empty)
            {
                rows = dt.Select(filterConversionTable);
            }
            else
            {
                rows = dt.Select();
            }
            foreach (DataRow row in rows)
            {
                writer.WriteStartElement(XmlConsts.rowNode);
                for (int i = 0; i <= attrCount - 1; i++)
                {
                    writer.WriteStartElement(XmlConsts.cellElement);
                    // так как ID сохранять не будем, то берем элементы начиная с первого
                    writer.WriteValue(row[i + 1].ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            tick = Environment.TickCount - tick;
            // закрываем отдельную таблицу
            writer.WriteEndElement();

            // закрываем секцию с данными
            writer.WriteEndElement();
        }

        /// <summary>
        /// запись данных классификаторов и таблиц фактов в XML
        /// </summary>
        /// <param name="writer"></param>
        private void WriteClassifierData(XmlWriter writer, int hierarchyIndex)
        {
            writer.WriteStartElement(XmlConsts.dataNode);
            // начало таблицы с данными
            writer.WriteStartElement(XmlConsts.dataTableNode);
            // количество атрибутов
            int attrCount = classifierAttributes.Count;
            object[] values = new object[attrCount];
            // читаем данные из базы
            while (reader.Read())
            {
                writer.WriteStartElement(XmlConsts.rowNode);
                reader.GetValues(values);
                for (int i = 0; i <= attrCount - 1; i++)
                {
                    writer.WriteStartElement(XmlConsts.cellElement);
                    // если индекс поля - ссылки на родителя равен индексу какого то поля,
                    // и экспортируем выделенные записи, то записываем
                    // в XML пустое значение по этому индексу
                    /*if ((i == hierarchyIndex) && exportSelectedRows)
                    {
                        writer.WriteValue(string.Empty);
                    }
                    else*/
                        writer.WriteValue(values[i].ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

            }
            reader.Close();
            // закрываем отдельную таблицу
            writer.WriteEndElement();
            // закрываем секцию с данными
            writer.WriteEndElement();
        }

        #endregion


        /// <summary>
        /// возвращает объект для просмотра записей по одной штуке (DataReader)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IDataReader GetDataReader(IDatabase db, IDbConnection connection, string query, IDbDataParameter[] filterParams)
        {
            IDataReader reader = null;

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            IDbCommand command = connection.CreateCommand();

            if (filterParams != null)
                foreach (IDbDataParameter param in filterParams)
                {
                    command.Parameters.Add(param);
                }

            command.CommandText = ((Database)db).GetQuery(query, command.Parameters);

            reader = command.ExecuteReader();

            return reader;
        }


        /// <summary>
        /// построение XML
        /// </summary>
        /// <param name="stream"></param>
        private void SaveDataToStream(Stream stream, ObjectType objecType)
        {
            List<string> attrNames = new List<string>();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.GetEncoding(1251);
            settings.CheckCharacters = false;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(stream, settings);

            // создаем XML файл со структурой... Сохраняем параметры записи
            // Сперва для каждой таблицы получаем параметры, записываем их
            // формируем XML документ
            writer.WriteStartDocument(true);
            writer.WriteStartElement(XmlConsts.rootNode);
            CreateXMLAttribute(writer, XmlConsts.versionAttribute, _scheme.UsersManager.ServerLibraryVersion());
            // запись параметров импорта
            SaveImportParams(writer);
            // если экспортируем не все записи, а только выделенные, то 
            // если классификатор иерархический, находим индекс поля со ссылкой на родителя
            int hierarchyIndex = -1;
            if (isHierarchy && exportSelectedRows)
                for (int i = 0; i <= classifierAttributes.Count - 1; i++)
                    if (classifierAttributes[i].Name == refParentColumnName)
                    {
                        hierarchyIndex = i;
                        break;
                    }

            // запись атрибутов объекта схемы
            writer.WriteStartElement(XmlConsts.metadataRootNode);
            writer.WriteStartElement(XmlConsts.tableMetadataNode);
            // атрибут 
            if (objecType == ObjectType.ConversionTable)
            {
                CreateXMLAttribute(writer, XmlConsts.exportObjectTypeAttribute, XmlConsts.conversionTable);
                CreateXMLAttribute(writer, XmlConsts.conversionRuleAttribute, conversionRule);
            }
            else
            {
                string objectClass = string.Empty;
                switch (((IEntity)schemeObject).ClassType)
                {
                    case ClassTypes.clsBridgeClassifier:
                        objectClass = XmlConsts.bridgeClassifier;
                        break;
                    case ClassTypes.clsDataClassifier:
                        objectClass = XmlConsts.dataClassifier;
                        break;
                    case ClassTypes.clsFactData:
                        objectClass = XmlConsts.factTable;
                        break;
                    case ClassTypes.clsFixedClassifier:
                        objectClass = XmlConsts.fixedClassifier;
                        break;
                }
                CreateXMLAttribute(writer, XmlConsts.exportObjectTypeAttribute, objectClass);
            }

            // для таблиц перекодировок метаданных становится больше
            if (objecType == ObjectType.Classifier || objecType == ObjectType.FactTable)
            {
                CreateObjectAttributeAttributes(writer, objectName[0], rusObjectName[0], objectSemantic[0], rusObjectSemantic[0]);

                if (innerImportParams.uniqueAttributesNames != string.Empty)
                    CreateXMLAttribute(writer, XmlConsts.uniqueAttributesAttribute, innerImportParams.uniqueAttributesNames);
                //  для каждого объекта схемы записываем все атрибуты с параметрами
                SaveSchemeAttributesToXML(classifierAttributes, writer);
            }
            else
            {
                // запись атрибутов сопоставляемого классификатора
                writer.WriteStartElement(XmlConsts.dataRoleNode);
                CreateObjectAttributeAttributes(writer, objectName[0], rusObjectName[0], objectSemantic[0], rusObjectSemantic[0]);
                SaveSchemeAttributesToXML(dataRoleAttributes, writer);
                writer.WriteEndElement();
                // запись атрибутов сопоставимого классификатора
                writer.WriteStartElement(XmlConsts.bridgeRoleNode);
                CreateObjectAttributeAttributes(writer, objectName[1], rusObjectName[1], objectSemantic[1], rusObjectSemantic[1]);
                SaveSchemeAttributesToXML(bridgeRoleAttributes, writer);
                writer.WriteEndElement();
            }
            
            writer.WriteEndElement();
            writer.WriteEndElement();

            // параметры источника данных
            if (dataSourсe != null)
                SaveDataSourceParamsToXML(dataSourсe, writer);

            // запись секции с данными
            if (objecType == ObjectType.Classifier || objecType == ObjectType.FactTable)
            {
                WriteClassifierData(writer, hierarchyIndex);
            }
            else if (objecType == ObjectType.ConversionTable)
            {
                WriteConvertionTableData(writer, conversionTable);
            }

            // закрываем корневой элемент и документ в целом
            writer.WriteEndElement();
            writer.WriteEndDocument();

            // закрывает поток и записывает все в файл
            writer.Flush();
            writer.Close();
        }

        #endregion


        #region импорт данных

        private void InnerImportFromStream(Stream stream, string objectUniqueKey, int currentDataSource,
            string filter, IDbDataParameter[] filterParams)
        {
            importerObjectDataSource = currentDataSource;
            IDatabase db = null;
            IClassifiersProtocol protocol = null;
            try
            {
                // получаем объект схемы, его атрибуты
                GetSchemeObject(objectUniqueKey);
                db = this._scheme.SchemeDWH.DB;
                protocol = (IClassifiersProtocol)this._scheme.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
                db.BeginTransaction();
                SendMessageToClassifierProtocol(protocol, "Начало операции импорта", ClassifiersEventKind.ceImportClassifierData);

                ReadFromStream(stream, db, filter, filterParams, protocol, ObjectType.Classifier);

                string protocolMessage = string.Format("Результаты операции импорта: {0} Записей добавлено: {1} {2} Записей обновлено: {3} {4} Записей удалено: {5}",
                    Environment.NewLine, insertRowsCount, Environment.NewLine, updateRowsCount, Environment.NewLine, deleteRowsCount);
                SendMessageToClassifierProtocol(protocol, protocolMessage, ClassifiersEventKind.ceInformation);

                SendMessageToClassifierProtocol(protocol, "Операция импорта завершилась успешно", ClassifiersEventKind.ceSuccefullFinished);
                db.Commit();
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

        private void InnerImportFactTable(Stream stream, string objectUniqueKey, int currentDataSource,
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
                db = this._scheme.SchemeDWH.DB;
                protocol = (IClassifiersProtocol)this._scheme.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
                db.BeginTransaction();
                SendMessageToClassifierProtocol(protocol, "Начало операции импорта", ClassifiersEventKind.ceImportClassifierData);
                ReadFromStream(stream, db, filter, filterParams, protocol, ObjectType.FactTable);

                string protocolMessage = string.Format("Результаты операции импорта: {0} Записей добавлено: {1} {2} Записей обновлено: {3} {4} Записей удалено: {5}",
                    Environment.NewLine, insertRowsCount, Environment.NewLine, updateRowsCount, Environment.NewLine, deleteRowsCount);
                SendMessageToClassifierProtocol(protocol, protocolMessage, ClassifiersEventKind.ceInformation);

                SendMessageToClassifierProtocol(protocol, "Операции импорта завершилась успешно", ClassifiersEventKind.ceSuccefullFinished);
                db.Commit();
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

        private void InnerImportConversionTable(Stream stream, string associationName, string ruleName)
        {
            IDatabase db = this._scheme.SchemeDWH.DB;
            try
            {
                GetConversionTableAttributes(associationName, ruleName);
                ReadFromStream(stream, db, string.Empty, null, null, ObjectType.ConversionTable);
            }
            finally
            {
                db.Dispose();
            }
        }

        #region проверки на совместимость (семантики и атрибуты)

        private void CheckNamesAndSemantics(List<string> xmlNames, List<string> xmlRusNames, List<string> xmlSemantics,
            List<string> xmlRusSemantics, ref DataTable pritocolTable, ObjectType objectType)
        {
            // проверять классификаторы и таблицы перекодировок будем по разному... 
            if ((objectType == ObjectType.Classifier) || (objectType == ObjectType.FactTable))
            {
                
                // если это классификатор или таблица фактов, то сравним название и семантику объекта и то, что записано в XML
                if (xmlNames[0] != objectName[0])
                {
                    pritocolTable.Rows.Add(string.Format("Наименование '{0}' ('{1}')", schemeObject.Caption, objectName[0]),
                        string.Format("Наименование '{0}' ('{1}')", xmlRusNames[0], xmlNames[0]), false);
                }
                if (xmlSemantics[0] != objectSemantic[0])
                {
                    pritocolTable.Rows.Add(string.Format("Семантика '{0}' ('{1}')", GetDataObjSemanticRus(schemeObject), objectSemantic[0]),
                        string.Format("Семантика '{0}' ('{1}')", xmlRusSemantics[0], xmlSemantics[0]), false);
                }
            }
            else
            {
                IAssociation association = (IAssociation)this._scheme.Associations[conversionTable.Name];
                // если это таблица перекодировок, то сравним семантики сопоставимого и сопоставляемого и правило с тем, что записано в  XML
                if (xmlNames[0] != objectName[0])
                {
                    pritocolTable.Rows.Add(string.Format("Наименование сопоставляемого '{0}' ('{1}')", association.RoleData.Caption, objectName[0]),
                        string.Format("Наименование '{0}' ('{1}')", xmlRusNames[0], xmlNames[0]), false);
                }
                if (xmlSemantics[0] != objectSemantic[0])
                {
                    pritocolTable.Rows.Add(string.Format("Семантика сопоставляемого '{0}' ('{1}')", GetDataObjSemanticRus(association.RoleData), objectSemantic[0]),
                        string.Format("Наименование '{0}' ('{1}')", xmlRusSemantics[0], xmlSemantics[0]), false);
                }
                if (xmlNames[1] != objectName[1])
                {
                    pritocolTable.Rows.Add(string.Format("Наименование сопоставляемого '{0}' ('{1}')", association.RoleBridge.Caption, objectName[1]),
                        string.Format("Наименование '{0}' ('{1}')", xmlRusNames[1], xmlSemantics[1]), false);
                }
                if (xmlSemantics[1] != objectSemantic[1])
                {
                    pritocolTable.Rows.Add(string.Format("Семантика сопоставляемого '{0}' ('{1}')", GetDataObjSemanticRus(association.RoleBridge), objectSemantic[1]),
                        string.Format("Наименование '{0}' ('{1}')", xmlRusSemantics[1], xmlSemantics[1]), false);
                }
            }
        }

        /// <summary>
        /// сравнивает атрибуты объекта и полученные их XML
        /// </summary>
        /// <param name="xmlAttributes">атрибуты, полученные из XML</param>
        /// <param name="checkProtocol">куда будут записываться сообщения, если что не так</param>
        /// <returns></returns>
        bool CheckAttributes(List<InnerAttribute> xmlAttributes, ref DataTable checkProtocolTable,
            ref List<AttributesRelation> attributesRelationList)
        {
            return CheckAttributes(classifierAttributes, xmlAttributes, string.Empty, 0, 0, ref checkProtocolTable, ref attributesRelationList);
        }

        /// <summary>
        /// сравниваем атрибуты у таблицы перекодировок
        /// </summary>
        /// <param name="xmlDataAttributes"></param>
        /// <param name="xmlBridgeAttributes"></param>
        /// <param name="checkProtocol"></param>
        /// <param name="attributesRelationList"></param>
        private bool CheckConversionTableAttributes(List<InnerAttribute> xmlDataAttributes, List<InnerAttribute> xmlBridgeAttributes,
            ref DataTable checkProtocolTable, ref List<AttributesRelation> attributesRelationList)
        {
            // проверяем атрибуты сопоставляемой части
            if (CheckAttributes(dataRoleAttributes, xmlDataAttributes, ">", 0, 0, ref checkProtocolTable, ref attributesRelationList))
                // проверяем атрибуты сопоставимой части
                return CheckAttributes(bridgeRoleAttributes, xmlBridgeAttributes, "<", dataRoleAttributes.Count, xmlDataAttributes.Count, ref checkProtocolTable, ref attributesRelationList);
            else
                return false;
        }

        /// <summary>
        /// стравнивает две коллекции атрибутов. На выходе коллекция 
        /// какой атрибут из XML соответствует атрибуту из схемы
        /// </summary>
        /// <param name="attributesList"></param>
        /// <param name="xmlAttributes"></param>
        /// <param name="attributeCaptionPrefix"></param>
        /// <param name="objectAttrIndex"></param>
        /// <param name="xmlAttrIndex"></param>
        /// <param name="checkProtocol"></param>
        /// <param name="attributesRelationList"></param>
        private bool CheckAttributes(List<IDataAttribute> attributesList, List<InnerAttribute> xmlAttributes,
            string attributeCaptionPrefix, int objectAttrIndex, int xmlAttrIndex,
            ref DataTable checkProtocolTable, ref List<AttributesRelation> attributesRelationList)
        {
            bool returnValue = true;
            for (int i = 0; i <= attributesList.Count - 1; i++)
            {
                AttributesRelation attributeRelation = new AttributesRelation();
                attributeRelation.objectAttributeIndex = i + objectAttrIndex;
                attributeRelation.xmlAttributeIndex = -1;
                attributeRelation.attrName = attributesList[i].Name;
                attributeRelation.DBType = GetDBTypeFromAttribute(attributesList[i]);
                for (int j = 0; j <= xmlAttributes.Count - 1; j++)
                {
                    if (attributesList[i].Name == xmlAttributes[j].name)
                    {
                        // при несовпадении каких либо свойств атрибутов будем все равно пытаться загрузить данные
                        if (attributesList[i].Size != xmlAttributes[j].size)
                        {
                            if (attributesList[i].Kind == DataAttributeKindTypes.Regular && attributesList[i].Visible)
                                checkProtocolTable.Rows.Add(string.Format("Свойство 'Размер' (size) = {0} атрибута '{1}{2}' ({3})", attributesList[i].Size, attributeCaptionPrefix, attributesList[i].Caption, attributesList[i].Name),
                                string.Format("Свойство 'Размер' (size) = {0} атрибута '{1}{2}' ({3})", xmlAttributes[j].size, attributeCaptionPrefix, attributesList[i].Caption, attributesList[i].Name), false);                         
                        }
                        if (!attributesList[i].IsNullable && xmlAttributes[j].nullable)
                        {
                            checkProtocolTable.Rows.Add(string.Format("Поле '{0}{1}' ({2}) обязательно для заполнения", attributeCaptionPrefix, attributesList[i].Caption, attributesList[i].Name),
                                string.Format("Поле '{0}{1}' ({2}) не обязательно для заполнения", attributeCaptionPrefix, attributesList[i].Caption, attributesList[i].Name), false);
                        }
                        attributeRelation.xmlAttributeIndex = j + xmlAttrIndex;
                        break;
                    }                       
                }
                // если названия атрибутов не совпали, заносим это в список
                string attribureName = attributesList[i].Name;
                string attributeCaption = attributesList[i].Caption;
                if (attributeCaption == string.Empty)
                    if (attributesList[i].Class == DataAttributeClassTypes.Reference)
                        attributeCaption = GetDataObjSemanticRus(this.schemeObject) + GetBridgeClsCaptionByRefName((IEntity)this.schemeObject, attributesList[i].Name);

                if (attributeRelation.xmlAttributeIndex == -1)
                {
                    // не рассматриваем 3 поля: источник данных, задачу и закачку
                    if (attribureName != "SourceID" && attribureName != "TaskID" && attribureName != "PumpID")
                    {

                        if (attributesList[i].DefaultValue == null && !attributesList[i].IsNullable)
                        {
                            // атрибут, необходимый для записи данных в базу отсутствует в XML
                            if (attributesList[i].Kind == DataAttributeKindTypes.Regular && attributesList[i].Visible)
                                checkProtocolTable.Rows.Add(string.Format("Атрибут '{0}{1}' ({2})",
                                    attributeCaptionPrefix, attributeCaption, attribureName), "Отсутствует", true);

                            returnValue = false;
                        }
                        else
                            if (attributesList[i].Kind == DataAttributeKindTypes.Regular && attributesList[i].Visible)
                                checkProtocolTable.Rows.Add(string.Format("Атрибут '{0}{1}' ({2})",
                                    attributeCaptionPrefix, attributeCaption, attribureName), "Отсутствует", false);
                    }
                }
                attributesRelationList.Add(attributeRelation);
            }
            return returnValue;
        }

        /// <summary>
        /// запись в протокол о каких то несовпадениях в атрибутах
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="message"></param>
        /// <param name="classifiersEventKind"></param>
        private void SendMessageToClassifierProtocol(IClassifiersProtocol protocol, string message, ClassifiersEventKind classifiersEventKind)
        {
            //if (schemeObject is IClassifier)
            //    protocol.WriteEventIntoClassifierProtocol(classifiersEventKind, ((IClassifier)schemeObject).OlapName,
            //        -1, importerObjectDataSource, message);
        }

        #endregion

        #region вспомогательные методы

        /// <summary>
        /// создание иерархического DataSet
        /// </summary>
        /// <returns></returns>
        private DataSet CreateHierarchyDataSet()
        {
            DataSet hierarchyDataSet = CreateFlatDataSet();
            DataRelation relation = new DataRelation("relation", hierarchyDataSet.Tables[0].Columns[parentColumnName],
                hierarchyDataSet.Tables[0].Columns[refParentColumnName]);
            hierarchyDataSet.Relations.Add(relation);
            return hierarchyDataSet;
        }

        private DataSet CreateFlatDataSet()
        {
            DataSet hierarchyDataSet = new DataSet();
            hierarchyDataSet.Tables.Add(CreateDataTableFromAttributes(classifierAttributes.ToArray()));
            return hierarchyDataSet;
        }

        /// <summary>
        /// получение индекса поля с источником данных и набором индексов ссылок на
        /// сопоставимые классификаторы у классификаторов
        /// </summary>
        /// <param name="objecType"></param>
        /// <param name="dataSourseIndex"></param>
        /// <returns></returns>
        private void GetRefColumnsIndexes(ref int dataSourseIndex, ref List<int> refAssociateColumns)
        {
            if (((IEntity)schemeObject).ClassType == ClassTypes.clsFactData)
                return;

            refAssociateColumns = new List<int>();
            // получаем наименования всех полей ссылок на сопоставимые классификаторы
            List<string> refClmnNames = new List<string>();

            foreach (IAssociation item in ((IClassifier)schemeObject).Associations.Values)
            {
                IDataAttribute attr = ((IClassifier)schemeObject).Attributes[item.FullDBName];
                if (item.AssociationClassType == AssociationClassTypes.Bridge)
                    if (attr.Class == DataAttributeClassTypes.Reference && !attr.IsLookup)
                        refClmnNames.Add(item.FullDBName);
            }

            // получаем индексы полей со ссылками на сопоставимые классификаторы... при импорте их в -1
            string sourceName = "SourceID";

            for (int i = 0; i <= classifierAttributes.Count - 1; i++)
            {
                if (classifierAttributes[i].Name.ToUpper() == sourceName.ToUpper())
                    dataSourseIndex = i;
                if (classifierAttributes[i].Class == DataAttributeClassTypes.Reference)
                    foreach (string refName in refClmnNames)
                    {
                        if (refName.ToUpper() == classifierAttributes[i].Name.ToUpper())
                            refAssociateColumns.Add(i);
                    }
            }
        }

        private void GetRefColumnsIndexes(ref int dataSourseIndex, ref int dataPumpIndex, ref int taskIndex)
        {
            // получаем индексы полей со ссылками на сопоставимые классификаторы... при импорте их в -1
            string sourceName = "SourceID";
            string pumpName = "PumpID";
            string taskName = "TaskID";

            for (int i = 0; i <= classifierAttributes.Count - 1; i++)
            {
                if (classifierAttributes[i].Name.ToUpper() == sourceName.ToUpper())
                    dataSourseIndex = i;
                if (classifierAttributes[i].Name.ToUpper() == pumpName.ToUpper())
                    dataPumpIndex = i;
                if (classifierAttributes[i].Name.ToUpper() == taskName.ToUpper())
                    taskIndex = i;
            }
        }

        #endregion

        #region сохранение данных в базу

        private IDbDataParameter[] CloneIDbDataParameters(IDatabase db, IDbDataParameter[] dataParameters)
        {
            IDbDataParameter[] tmpDataParameters = new IDbDataParameter[dataParameters.Length];
            for (int i = 0; i <= dataParameters.Length - 1; i++)
            {
                tmpDataParameters[i] = db.CreateParameter(dataParameters[i].ParameterName, dataParameters[i].Value);
            }
            return tmpDataParameters;
        }

        /// <summary>
        /// добавление новой записи
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fields"></param>
        /// <param name="dbParams"></param>
        private void InsertNewRow(IDatabase db, string[] fields, Dictionary<string, IDbDataParameter> dbParams)
        {
            string[] paramSigns = new string[fields.Length];
            IDbDataParameter[] paramsList = new IDbDataParameter[fields.Length];
            // получение параметров запроса
            for (int i = 0; i <= paramSigns.Length - 1; i++)
            {
                paramSigns[i] = "?";
                paramsList[i] = dbParams[fields[i]];
            }

            string insertQuery = String.Format("insert into {0} ({1}) values ({2})",
                schemeObject.FullDBName, String.Join(",", fields), String.Join(",", paramSigns));

            db.ExecQuery(insertQuery, QueryResultTypes.NonQuery, paramsList);
        }


        /// <summary>
        /// вставляет запись или ардейтит, если запись с некоторыми параметрами присутс
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fields"></param>
        /// <param name="paramsList"></param>
        private void InsertOrUpdateRow(IDatabase db, string[] fields, Dictionary<string, IDbDataParameter> dbParams)
        {
            // если указан параметр обновления, то пытаемся обновить
            if (innerImportParams.refreshDataByUnique || innerImportParams.refreshDataByAttributes)
            {
                // по параметрам уникальности запрашиваем количество записей с таким набором этих параметров
                int counter = 0;
                int uniqueItemsCount = uniqueAttributes.Split(',').Length;
                // создаем параметры для запроса записи
                IDbDataParameter[] selectParams = new IDbDataParameter[uniqueItemsCount];
                // запрашиваем есть ли такая запись уже
                string[] filter = new string[uniqueItemsCount];
                // создаем параметры для запроса количества таких записей
                foreach (string str in uniqueAttributes.Split(','))
                {
                    if (dbParams.ContainsKey(str))
                    {
                        if (dbParams[str].DbType == DbType.AnsiStringFixedLength || dbParams[str].DbType == DbType.AnsiString)
                        {
                            selectParams[counter] = db.CreateParameter(str, Convert.ToString(dbParams[str].Value).ToUpper());
                            filter[counter] = String.Format("(UPPER({0}) = ?)", str);
                        }
                        else
                        {
                            selectParams[counter] = db.CreateParameter(str, dbParams[str].Value);
                            filter[counter] = String.Format("({0} = ?)", str);
                        }
                        counter++;
                    }
                }
                // запрос данных по уникальному набору полей
                string queryFilter = string.Empty;
                if (importerObjectDataSource > 0)
                    queryFilter = string.Format("({0}) and (SourceID = {1}) and (RowType = 0)",
                        String.Join(" and ", filter), importerObjectDataSource);
                else
                    queryFilter = string.Format("({0}) and (RowType = 0)", String.Join(" and ", filter));

                string selectQuery = selectQuery = String.Format("select count(ID) from {0} where {1}",
                        schemeObject.FullDBName, queryFilter);
                // количество записей, которые могут присутствовать с такими же значениями по нескольким полям
                DataTable table = (DataTable) db.ExecQuery(selectQuery, QueryResultTypes.DataTable, selectParams);

                if (table.Rows.Count > 0)
                {
                    // если нашли такую запись, то апдейтим у нее все поля кроме ID и ссылки на родителя
                    // хотя лучше не апдейтить вообще все ссылки и поля типа источник, задача и прочее
                    List<string> nonUpdateFields = new List<string>();
                    foreach (IDataAttribute attr in classifierAttributes)
                    {
                        if (!IsUpdatedAttribute(attr))
                            nonUpdateFields.Add(attr.Name);
                    }
                    // все данные, которые будут учавствовать в запросе как параметры
                    IDbDataParameter[] updateParamList = new IDbDataParameter[dbParams.Count - nonUpdateFields.Count + selectParams.Length];
                    string[] updateFields = new string[dbParams.Count - nonUpdateFields.Count];
                    counter = 0;
                    // получаем параметры для апдейта записи
                    foreach (IDbDataParameter param in dbParams.Values)
                    {
                        if (!nonUpdateFields.Contains(param.ParameterName))
                        {
                            updateParamList[counter] = db.CreateParameter(param.ParameterName, param.Value, param.DbType);
                            updateFields[counter] = String.Format("{0} = ?", param.ParameterName);
                            counter++;
                        }
                    }

                    // если не нашлось полей для апдейта, ничего не делаем
                    if (counter == 0) return;

                    // обновляем параметры для фильтра
                    selectParams = CloneIDbDataParameters(db, selectParams);
                    int nullIndex = 0;
                    for (int i = 0; i <= updateParamList.Length - 1; i++)
                        if (updateParamList[i] == null)
                        {
                            nullIndex = i;
                            break;
                        }
                    // добавляем в общий список параметров
                    selectParams.CopyTo(updateParamList, nullIndex);

                    // создаем запрос на обновление записи
                    string updateQuery = String.Format("update {0} set {1} where {2}",
                        schemeObject.FullDBName, string.Join(",", updateFields), queryFilter);

                    db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, updateParamList);
                    updateRowsCount++;
                }
                else
                {
                    // если нет, то добавляем
                    InsertNewRow(db, fields, dbParams);
                    insertRowsCount++;
                }
            }
            else
            {
                InsertNewRow(db, fields, dbParams);
                insertRowsCount++;
            }
        }

        /// <summary>
        /// запрос на наличие записи в таблице
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fields"></param>
        /// <param name="uniqueAttributes"></param>
        /// <param name="dbParams"></param>
        private void GetSelectQuery(IDatabase db, string[] fields, string uniqueAttributes, Dictionary<string, IDbDataParameter> dbParams, ref string query, ref List<IDbDataParameter> queryParams)
        {
            queryParams = new List<IDbDataParameter>();

            List<string> nonUniqueFields = new List<string>();
            foreach (string field in fields)
            {
                if (!uniqueAttributes.Contains(field))
                    nonUniqueFields.Add(field);
            }

            List<string> nonUniqueQueryFilterParts = new List<string>();
            foreach (string str in nonUniqueFields)
            {
                IDbDataParameter param = dbParams[str];

                if (dbParams[str].DbType == DbType.AnsiString || dbParams[str].DbType == DbType.AnsiStringFixedLength)
                {
                    nonUniqueQueryFilterParts.Add(string.Format("UPPER({0}) = ?", str));
                    queryParams.Add(db.CreateParameter(param.ParameterName, Convert.ToString(param.Value).ToUpper(), param.DbType));
                }
                else
                {
                    nonUniqueQueryFilterParts.Add(string.Format("{0} = ?", str));
                    queryParams.Add(db.CreateParameter(param.ParameterName, param.Value, param.DbType));
                }
            }

            string[] uniqueFields = uniqueAttributes.Split(',');
            List<string> uniqueQueryParts = new List<string>();
            foreach (string str in uniqueFields)
            {
                IDbDataParameter param = dbParams[str];

                if (dbParams[str].DbType == DbType.AnsiString || dbParams[str].DbType == DbType.AnsiStringFixedLength)
                {
                    uniqueQueryParts.Add(string.Format("UPPER({0}) = ?", str));
                    queryParams.Add(db.CreateParameter(param.ParameterName, Convert.ToString(param.Value).ToUpper(), param.DbType));
                }
                else
                {
                    uniqueQueryParts.Add(string.Format("{0} = ?", str));
                    queryParams.Add(db.CreateParameter(param.ParameterName, param.Value, param.DbType));
                }
            }

            string queryFilter = string.Empty;
            if (importerObjectDataSource > 0)
                queryFilter = string.Format("SourceID = {1} and RowType = 0", importerObjectDataSource);
            else
                queryFilter = "RowType = 0";

            query = string.Format("select count(ID) from {0} where ({1}) and ({2}) and ({3})",
                schemeObject.FullDBName, String.Join(" and ", uniqueQueryParts.ToArray()), String.Join(" or ", nonUniqueQueryFilterParts.ToArray()), queryFilter);
        }


        private bool IsReferenceOnFixedClassifier(string clsName)
        {
            ICommonObject clsObject = _scheme.Classifiers[clsName];
            if (((IEntity)clsObject).ClassType == ClassTypes.clsFixedClassifier)
                return true;
            return false;
        }


        private bool IsUpdatedAttribute(IDataAttribute attr)
        {
            if (attr.Class == DataAttributeClassTypes.Reference)
                if (attr.IsLookup)
                    return IsReferenceOnFixedClassifier(attr.LookupObjectName);
                else
                    return false;
            if (attr.Kind == DataAttributeKindTypes.Sys)
                return false;
            if (attr.Name == "ID" || attr.Name == "ParentID" || attr.Name == "SourceID" ||
                attr.Name == "RowType" || attr.Name == "PumpID" || attr.Name == "TaskID" ||
                uniqueAttributes.Contains(attr.Name))
                return false;
            return true;
        }


        /// <summary>
        /// сохранение одной записи таблицы перекодировок
        /// </summary>
        /// <param name="conversionDataTable"></param>
        /// <param name="duConversionTable"></param>
        /// <param name="attributesRelationList"></param>
        /// <param name="rowCellsValues"></param>
        private void SaveRow(DataTable conversionDataTable, IDataUpdater duConversionTable, List<AttributesRelation> attributesRelationList, List<object> rowCellsValues)
        {
            Object[] paramsList = new Object[attributesRelationList.Count + 1];
            // сохраняем записи без ID
            foreach (AttributesRelation attrRel in attributesRelationList)
            {
                if (attrRel.xmlAttributeIndex >= 0)
                    paramsList[attrRel.objectAttributeIndex + 1] = rowCellsValues[attrRel.xmlAttributeIndex];
                else
                    paramsList[attrRel.objectAttributeIndex + 1] = DBNull.Value;
            }
            conversionDataTable.Rows.Add(paramsList);
            // пытаемся записать по одной записи в таблицу перекодировок
            try
            {
                duConversionTable.Update(ref conversionDataTable);
                conversionDataTable.AcceptChanges();
            }
            catch
            {
                conversionDataTable.RejectChanges();
            }
        }

        /// <summary>
        /// сохранение записи классификатора
        /// </summary>
        /// <param name="attributesRelationList"></param>
        /// <param name="dataSet"></param>
        /// <param name="refColumnsIndexes"></param>
        /// <param name="dataSourceIDIndex"></param>
        /// <param name="rowCellsValues"></param>
        /// <param name="db"></param>
        private void SaveRow(List<AttributesRelation> attributesRelationList, DataSet dataSet,
            List<int> refColumnsIndexes, int dataSourceIdIndex, List<object> rowCellsValues, IDatabase db)
        {
            Object[] paramsList = new Object[attributesRelationList.Count];
            foreach (AttributesRelation relation in attributesRelationList)
            {
                if (relation.xmlAttributeIndex >= 0)
                {
                    if (rowCellsValues[relation.xmlAttributeIndex] == null)
                        paramsList[relation.objectAttributeIndex] = DBNull.Value;
                    else
                        paramsList[relation.objectAttributeIndex] = rowCellsValues[relation.xmlAttributeIndex];
                }
                else
                    paramsList[relation.objectAttributeIndex] = DBNull.Value;
            }

            DataRow newRow = dataSet.Tables[0].Rows.Add(paramsList);
            newRow.AcceptChanges();
            newRow.BeginEdit();
            // ставим значения по умолчанию для записей.Очищаем сопоставление
            for (int index = 0; index <= newRow.ItemArray.Length - 1; index++)
            {
                if (newRow.ItemArray[index] == null || newRow.ItemArray[index] == DBNull.Value)
                    if (classifierAttributes[index].DefaultValue != null)
                        newRow[index] = classifierAttributes[index].DefaultValue;

                if (refColumnsIndexes != null)
                {
                    foreach (int i in refColumnsIndexes)
                        newRow[i] = -1;
                }
            }
            // ставим текущий источник данных
            if (dataSourceIdIndex > 0)
                // если текущий источник больше или равен -1, то проставляем его
                if (importerObjectDataSource >= -1)
                    newRow[dataSourceIdIndex] = importerObjectDataSource;

            newRow.EndEdit();

            if (!isHierarchy)
            {
                if (!innerImportParams.useOldID)
                    // для таблиц фактов под SQL Server 2005 ID не получаем... вообще для таблиц фактов не надо устанавливать такой режим...
                    if (!(schemeObject is IFactTable && (
						this._scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient || 
						this._scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess ||
						this._scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess)))
                    {
                        newRow["ID"] = GetNewId(newRow, ((IEntity)schemeObject).GeneratorName, (IEntity)schemeObject, db);
                    }
                // сохраняем одну запись
                SaveSingleDataRow(newRow, db, attributesRelationList);
                // очищаем датасет от записей
                dataSet.Tables[0].Rows.Clear();
            }
        }


        private void SaveRow(List<AttributesRelation> attributesRelationList, DataSet dataSet,
            int dataSourceIdIndex, int pumpIdIndex, int taskIdIndex, List<object> rowCellsValues, IDatabase db)
        {
            Object[] paramsList = new Object[attributesRelationList.Count];
            foreach (AttributesRelation relation in attributesRelationList)
            {
                if (relation.xmlAttributeIndex >= 0)
                {
                    if (rowCellsValues[relation.xmlAttributeIndex] == null)
                        paramsList[relation.objectAttributeIndex] = DBNull.Value;
                    else
                        paramsList[relation.objectAttributeIndex] = rowCellsValues[relation.xmlAttributeIndex];
                }
                else
                    paramsList[relation.objectAttributeIndex] = DBNull.Value;
            }

            DataRow newRow = dataSet.Tables[0].Rows.Add(paramsList);
            newRow.AcceptChanges();
            newRow.BeginEdit();
            // ставим значения по умолчанию для записей
            for (int index = 0; index <= newRow.ItemArray.Length - 1; index++)
            {
                if (newRow.ItemArray[index] == null || newRow.ItemArray[index] == DBNull.Value)
                    if (classifierAttributes[index].DefaultValue != null)
                        newRow[index] = classifierAttributes[index].DefaultValue;
            }
            // ставим текущий источник данных
            if (dataSourceIdIndex > 0)
                // если текущий источник больше или равен -1, то проставляем его
                if (importerObjectDataSource >= -1)
                    newRow[dataSourceIdIndex] = importerObjectDataSource;
            // текущую закачку
            if (pumpIdIndex > 0)
                if (importerPumpId >= -1)
                    newRow[pumpIdIndex] = importerPumpId;
            // текущую задачу
            if (taskIdIndex > 0)
                if (importerTaskId >= -1)
                    newRow[taskIdIndex] = importerTaskId;

            //if (!innerImportParams.useOldID)
            // для таблиц фактов под SQL Server 2005 ID не получаем... вообще для таблиц фактов не надо устанавливать такой режим...
            if (this._scheme.SchemeDWH.FactoryName.ToUpper() != "SYSTEM.DATA.SQLCLIENT")
            {
                int curentID = db.GetGenerator(((IEntity)schemeObject).GeneratorName);
                newRow["ID"] = curentID;
            }
            newRow.EndEdit();
            // сохраняем одну запись
            SaveSingleDataRow(newRow, db, attributesRelationList);
            // очищаем датасет от записей
            dataSet.Tables[0].Rows.Clear();
        }

        #endregion

        #region получение данных из XML

        /// <summary>
        /// получение атрибута объекта схемы из XML
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private InnerAttribute GetXMLAttribute(XmlTextReader reader)
        {
            InnerAttribute attr = new InnerAttribute();
            reader.MoveToAttribute(XmlConsts.nameAttribute);
            attr.name = reader.Value;
            reader.MoveToAttribute(XmlConsts.sizeAttribute);
            attr.size = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XmlConsts.nullableAttribute);
            attr.nullable = Convert.ToBoolean(reader.Value);
            if (reader.MoveToAttribute(XmlConsts.defaultValueAttribute))
                attr.defaultValue = (object)reader.Value;
            else
                attr.defaultValue = null;

            return attr;
        }

        /// <summary>
        /// получение параметров источника дынных из XML
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="xmlDataSource"></param>
        private void GetDataSourceParams(XmlTextReader reader, XMLDataSource xmlDataSource)
        {
            reader.MoveToAttribute(XmlConsts.suppplierCodeAttribute);
            xmlDataSource.suppplierCode = reader.Value;
            reader.MoveToAttribute(XmlConsts.dataCodeAttribute);
            xmlDataSource.dataCode = reader.Value;
            reader.MoveToAttribute(XmlConsts.dataNameAttribute);
            xmlDataSource.dataName = reader.Value;
            reader.MoveToAttribute(XmlConsts.kindsOfParamsAttribute);
            xmlDataSource.kindsOfParams = reader.Value;
            reader.MoveToAttribute(XmlConsts.nameAttribute);
            xmlDataSource.name = reader.Value;
            reader.MoveToAttribute(XmlConsts.yearAttribute);
            xmlDataSource.year = reader.Value;
            reader.MoveToAttribute(XmlConsts.monthAttribute);
            xmlDataSource.month = reader.Value;
            reader.MoveToAttribute(XmlConsts.variantAttribute);
            xmlDataSource.variant = reader.Value;
            reader.MoveToAttribute(XmlConsts.quarterAttribute);
            xmlDataSource.quarter = reader.Value;
            reader.MoveToAttribute(XmlConsts.territoryAttribute);
            xmlDataSource.territory = reader.Value;
            reader.MoveToAttribute(XmlConsts.dataSourceNameAttribute);
            xmlDataSource.dataSourceName = reader.Value;
        }

        /// <summary>
        /// получение параметров импорта из XML
        /// </summary>
        /// <param name="reader"></param>
        private void GetImportSettings(XmlTextReader reader, ref ImportPatams importParams)
        {
            reader.MoveToAttribute(XmlConsts.deleteDataBeforeImportAttribute);
            if (reader.Value == string.Empty)
                reader.MoveToAttribute(XmlConsts.deleteDataBeforeImportAttributeError);
            importParams.deleteDataBeforeImport = Convert.ToBoolean(reader.Value);
            if (reader.MoveToAttribute(XmlConsts.deleteDevelopData))
                importParams.deleteDeveloperData = Convert.ToBoolean(reader.Value);
            reader.MoveToAttribute(XmlConsts.useOldIDAttribute);
            importParams.useOldID = Convert.ToBoolean(reader.Value);
            reader.MoveToAttribute(XmlConsts.restoreDataSourceAttribute);
            importParams.restoreDataSource = Convert.ToBoolean(reader.Value);
            reader.MoveToAttribute(XmlConsts.useInnerUniqueAttributesAttribute);
            importParams.refreshDataByUnique = Convert.ToBoolean(reader.Value);
            reader.MoveToAttribute(XmlConsts.useSchemeUniqueAttributesAttribute);
            importParams.refreshDataByAttributes = Convert.ToBoolean(reader.Value);
        }

        #endregion

        #region удвление данных

        /// <summary>
        /// предварительное удаление данных, если предусмотрено параметрами 
        /// </summary>
        /// <param name="objecType"></param>
        /// <param name="db"></param>
        /// <param name="protocol"></param>
        /// <param name="conversionDataTable"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        private void DeleteData(ObjectType objecType, IDatabase db, IClassifiersProtocol protocol, DataTable conversionDataTable,
            string filter, IDbDataParameter[] filterParams)
        {
            if (!innerImportParams.deleteDataBeforeImport)
                return;

            if (innerImportParams.restoreDataSource)
            {
                // если указано восстановление источника, то и удаляем записи по тому источнику
                string[] filterParts = filter.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries);
                string[] newFilter = new string[filterParts.Length];
                for (int i = 0; i <= filterParts.Length - 1; i++)
                {
                    
                    if (filterParts[i].Contains("SourceID"))
                    {
                        newFilter[i] = string.Format("(SourceID = {0})", importerObjectDataSource);
                    }
                    else
                    {
                        newFilter[i] = filterParts[i].Trim();
                    }
                }
                filter = String.Join(" and ", newFilter);
            }
            if (objecType == ObjectType.Classifier || objecType == ObjectType.FactTable)
            {
                //SendMessageToClassifierProtocol(protocol, "Начало удаления данных во время операции импорта", ClassifiersEventKind.ceInformation);
                if (!innerImportParams.deleteDeveloperData)
                    filter = String.Concat(filter, string.Format(" and (ID < {0})", developerLowBoundRangeId));
                deleteRowsCount = DeleteDataFromSchemeObject(db, filter, filterParams);
                //SendMessageToClassifierProtocol(protocol, String.Format("Удаление данных успешно завершено. Удалено {0} записей",
                    //deletedRowsCount), ClassifiersEventKind.ceInformation);
            }
            else
            {
                foreach (DataRow row in conversionDataTable.Rows)
                {
                    conversionTable.DeleteRow(Convert.ToInt32(row["ID"]));
                }
                conversionDataTable.Clear();
            } 
        }

        /// <summary>
        /// удаление данных из объекта
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        private int DeleteDataFromSchemeObject(IDatabase db, string filter, IDbDataParameter[] filterParams)
        {
            string deleteQuery = String.Format("delete from {0} where {1}", schemeObject.FullDBName, filter);
            return Convert.ToInt32(db.ExecQuery(deleteQuery, QueryResultTypes.NonQuery, filterParams));
        }

        #endregion

        #region главный метод

        private void ReadFromStream(Stream stream, IDatabase db, string filter,
            IDbDataParameter[] filterParams, IClassifiersProtocol protocol, ObjectType objectType)
        {
            // объект для чтения из XML
            XmlTextReader reader = new XmlTextReader(stream);
            // структура для хранения одной записи, получаемой из XML
            List<object> rowCellsValues = new List<object>();
            // отношения между данными. Хранит соответствие данных из XML атрибутам серверного объекта
            List<AttributesRelation> attributesRelationList = new List<AttributesRelation>();
            // таблица для хранения несоответствия 
            DataTable checkProtocolTable = GetInformationTable(objectType);

            // строка запроса для записи данных в базу
            string selectQuery = string.Empty;
            // объект для хранения иерархических данных
            DataSet dataSet = null;
            // объект для хранения данных таблиц перекодировок
            DataTable conversionDataTable = null;

            int importedRowsCount = 0;
            // упдатер для получения и сохранения данных перекодировок
            IDataUpdater duConversionTable = null;
            // объекты для хранения семантик и наименований, полученных из XML
            List<string> xmlObjectName = new List<string>();
            List<string> xmlObjecSemantic = new List<string>();
            // для русских наименований
            List<string> xmlRusObjectName = new List<string>();
            List<string> xmlRusObjecSemantic = new List<string>();

            // структуры для хранения атрибутов, полученных из XML
            // для хранения атрибутов классификаторов и таблиц фактов
            List<InnerAttribute> xmlObjectAttributes = null;
            // для хранения атрибутов таблиц перекодировок
            List<InnerAttribute> xmlDataRoleAttributes = null;
            List<InnerAttribute> xmlBridgeRoleAttributes = null;

            if (objectType == ObjectType.Classifier || objectType == ObjectType.FactTable)
                xmlObjectAttributes = new List<InnerAttribute>();
            else
            {
                xmlDataRoleAttributes = new List<InnerAttribute>();
                xmlBridgeRoleAttributes = new List<InnerAttribute>();
            }

            innerImportParams = new ImportPatams();
            // для таблиц перекодировок содаем свою таблицу для хранения данных
            if (objectType == ObjectType.ConversionTable)
            {
                conversionDataTable = new DataTable();
                duConversionTable = conversionTable.GetDataUpdater();
                duConversionTable.Fill(ref conversionDataTable);
            }

            // для классификаторов и таблиц фактов создаем DataSet
            if (isHierarchy)
            {
                if (objectType == ObjectType.Classifier)
                {
                    // для иерархического добавляем иерархию
                    dataSet = CreateHierarchyDataSet();
                    dataSet.Tables[0].BeginLoadData();
                }
            }
            else if (objectType == ObjectType.FactTable || objectType == ObjectType.Classifier)
                dataSet = CreateFlatDataSet();

            int dataSourceIdIndex = -1;
            int pumpIdIndex = -1;
            int taskIdIndex = -1;
            List<int> refColumnsIndexes = null;
            if (objectType == ObjectType.Classifier)
                // получаем ссылки на сопоставимые классификаторы... 
                GetRefColumnsIndexes(ref dataSourceIdIndex, ref refColumnsIndexes);
            else if (objectType == ObjectType.FactTable)
                // получаем индексы полей с источником данных, закачкой и задачей для таблиц фактов 
                GetRefColumnsIndexes(ref dataSourceIdIndex, ref pumpIdIndex, ref taskIdIndex);


            XMLDataSource xmlDataSource = null;
            // читаем XML сначала и до конца
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        switch (reader.LocalName)
                        {
                            case XmlConsts.tableMetadataNode:
                                // сравниваем семантики объекта и из XML
                                CheckNamesAndSemantics(xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic, ref checkProtocolTable, objectType);
                                // сравниваем атрибуты объекта и из XML
                                bool canSaveData = true;
                                if (objectType == ObjectType.Classifier || objectType == ObjectType.FactTable)
                                    canSaveData = CheckAttributes(xmlObjectAttributes, ref checkProtocolTable, ref attributesRelationList);
                                else
                                    canSaveData = CheckConversionTableAttributes(xmlDataRoleAttributes, xmlBridgeRoleAttributes, ref checkProtocolTable, ref attributesRelationList);
                                if (!canSaveData)
                                {
                                    // сохранить данные не получится. чистим все коллекции и выходим
                                    return;
                                }
                                break;
                            case XmlConsts.dataSourceElement:
                                break;
                            case XmlConsts.rowNode:
                                importedRowsCount++;
                                GetNonGeneratedId(objectType, rowCellsValues);
                                // сохранение данных... По одной записи
                                if (objectType == ObjectType.ConversionTable)
                                    SaveRow(conversionDataTable, duConversionTable, attributesRelationList, rowCellsValues);
                                else if (objectType == ObjectType.Classifier)
                                    SaveRow(attributesRelationList, dataSet, refColumnsIndexes, dataSourceIdIndex, rowCellsValues, db);
                                else if (objectType == ObjectType.FactTable)
                                    SaveRow(attributesRelationList, dataSet, dataSourceIdIndex, pumpIdIndex, taskIdIndex, rowCellsValues, db);
                                break;
                            case XmlConsts.dataTableNode:
                                // сохраняем данные иерархического классификатора
                                if (isHierarchy)
                                {
                                    ClearFirstLevelParents(dataSet, dataSet.Relations[0]);
                                    dataSet.Tables[0].EndLoadData();
                                    // если объект обладает свойством иерархии, то сохраняем в базу данные только после загрузки всех данных из XML
                                    foreach (DataTable table in dataSet.Tables)
                                        table.BeginLoadData();
                                    try
                                    {
                                        // если выбран режим изменения ID, то перестраиваем иерархию
                                        // если выбран режим обновления, то тоже не обновляем ID
                                        if (!(innerImportParams.useOldID))
                                            RestoreHierarchyID(dataSet, ((IEntity)schemeObject).GeneratorName, db);
                                        // сохраняем данные
                                        SaveHierarchyData(dataSet, db, attributesRelationList);
                                    }
                                    finally
                                    {

                                        foreach (DataTable table in dataSet.Tables)
                                        {
                                            table.EndLoadData();
                                        }
                                    }
                                }

                                // установим id для режима сохранения id
                                if (innerImportParams.useOldID)
                                    SetGeneratorsNormalValues(lastId, lastDeveloperId, db);

                                //if (objectType == ObjectType.Classifier || objectType == ObjectType.FactTable)
                                //    SendMessageToClassifierProtocol(protocol, String.Format("В процессе импорта записей обработано: {0} ", importedRowsCount), ClassifiersEventKind.ceInformation);
                                break;
                        }
                        break;
                    case XmlNodeType.Element:
                        switch (reader.LocalName)
                        {
                            case XmlConsts.settingsElement:
                                // читаем параметры импорта из XML
                                GetImportSettings(reader, ref innerImportParams);
                                if (!innerImportParams.restoreDataSource)
                                    DeleteData(objectType, db, protocol, conversionDataTable, filter, filterParams);
                                break;
                            case XmlConsts.tableMetadataNode:
                                if (objectType == ObjectType.Classifier || objectType == ObjectType.FactTable)
                                {
                                    GetObjectMetaData(reader, xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic);
                                }
                                reader.MoveToAttribute(XmlConsts.uniqueAttributesAttribute);
                                uniqueAttributes = reader.Value;
                                break;
                            case XmlConsts.attributeElement:
                                // получаем данные по отдельному отрибуту схемы, записанному в XML
                                InnerAttribute attr = GetXMLAttribute(reader);
                                xmlObjectAttributes.Add(attr);
                                break;
                            case XmlConsts.dataRoleNode:
                                // для таблиц перекодировок получаем параметры сопоставляемого классификатора
                                xmlObjectAttributes = xmlDataRoleAttributes;

                                GetObjectMetaData(reader, xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic);
                                break;
                            case XmlConsts.bridgeRoleNode:
                                // для таблиц перекодировок получаем параметры сопостовимого классификатора
                                xmlObjectAttributes = xmlBridgeRoleAttributes;

                                GetObjectMetaData(reader, xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic);
                                break;
                            case XmlConsts.usedDataSourcesNode:

                                break;
                            case XmlConsts.dataSourceElement:
                                xmlDataSource = new XMLDataSource();
                                // получим параметры источника данных их XML
                                GetDataSourceParams(reader, xmlDataSource);
                                // если стоит параметр импорта "восстанавливать источник"
                                if (innerImportParams.restoreDataSource)
                                {
                                    // если источника нету, то добавим
                                    if (AddDataSource(xmlDataSource))
                                    {
                                        SendMessageToClassifierProtocol(protocol, String.Format("Добавлен источник с ID {0}", importerObjectDataSource), ClassifiersEventKind.ceInformation);
                                    }
                                    DeleteData(objectType, db, protocol, conversionDataTable, filter, filterParams);
                                }
                                break;
                            case XmlConsts.dataTableNode:
                                break;
                            case XmlConsts.rowNode:
                                // начинаем читать запись
                                rowCellsValues.Clear();
                                break;
                            case XmlConsts.cellElement:
                                // читаем одну ячейку
                                object newValue = (object)reader.ReadElementString();
                                if (newValue.ToString() != string.Empty)
                                    rowCellsValues.Add(newValue);
                                else
                                    rowCellsValues.Add(DBNull.Value);
                                break;
                        }
                        break;
                }
            }
        }

        
        /// <summary>
        /// читает из XML метаданные по объекту
        /// </summary>
        /// <param name="reader">объект чтения их XML</param>
        /// <param name="xmlObjectName">наименование объекта</param>
        /// <param name="xmlRusObjectName">русское наименование</param>
        /// <param name="xmlObjecSemantic">семантика объекта</param>
        /// <param name="xmlRusObjecSemantic">русское обозначение семантики</param>
        private void GetObjectMetaData(XmlTextReader reader, List<string> xmlObjectName,
            List<string> xmlRusObjectName, List<string> xmlObjecSemantic, List<string> xmlRusObjecSemantic)
        {
            reader.MoveToAttribute(XmlConsts.nameAttribute);
            xmlObjectName.Add(reader.Value);

            if (reader.MoveToAttribute(XmlConsts.rusNameAttribute))
                xmlRusObjectName.Add(reader.Value);
            else
                xmlRusObjectName.Add(string.Empty);

            reader.MoveToAttribute(XmlConsts.semanticAttribute);
            xmlObjecSemantic.Add(reader.Value);

            if (reader.MoveToAttribute(XmlConsts.rusSemanticAttribute))
                xmlRusObjecSemantic.Add(reader.Value);
            else
                xmlRusObjecSemantic.Add(string.Empty);
        }

        #endregion

        #endregion


        #region сверка XML и импортируемого объекта

        public bool CheckXmlMetadata(Stream stream, string objectUniqueKey, ObjectType importObjectType, ref ImportPatams importParams, ref DataTable informationTable)
        {
            return InnerCheckXmlMetadata(stream, objectUniqueKey, importObjectType, ref importParams, ref informationTable);
        }

        private bool InnerCheckXmlMetadata(Stream stream, string objectUniqueKey, ObjectType importObjectType, ref ImportPatams importParams, ref DataTable informationTable)
        {
            GetSchemeObject(objectUniqueKey);
            return ReadMetadataFromStream(stream, importObjectType, ref importParams, ref informationTable);
        }

        public bool CheckXmlMetadata(Stream stream, string associationName, string ruleName, ref ImportPatams importParams, ref DataTable informationTable)
        {
            return InnerCheckXmlMetadata(stream, associationName, ruleName, ref importParams, ref informationTable);
        }

        private bool InnerCheckXmlMetadata(Stream stream, string associationName, string ruleName, ref ImportPatams importParams, ref DataTable informationTable)
        {
            GetConversionTableAttributes(associationName, ruleName);
            return ReadMetadataFromStream(stream, ObjectType.ConversionTable, ref importParams, ref informationTable);
        }


        private bool ReadMetadataFromStream(Stream stream, ObjectType objectType, ref ImportPatams importParams, ref DataTable informationTable)
        {
            // объект для чтения из XML
            XmlTextReader reader = new XmlTextReader(stream);
            // отношения между данными. Хранит соответствие данных из XML атрибутам серверного объекта
            List<AttributesRelation> attributesRelationList = new List<AttributesRelation>();
            // таблица для хранения несоответствия 
            informationTable = GetInformationTable(objectType);

            // объекты для хранения семантик и наименований, полученных из XML
            List<string> xmlObjectName = new List<string>();
            List<string> xmlObjecSemantic = new List<string>();
            // для русских наименований
            List<string> xmlRusObjectName = new List<string>();
            List<string> xmlRusObjecSemantic = new List<string>();

            // структуры для хранения атрибутов, полученных из XML
            // для хранения атрибутов классификаторов и таблиц фактов
            List<InnerAttribute> xmlObjectAttributes = null;
            // для хранения атрибутов таблиц перекодировок
            List<InnerAttribute> xmlDataRoleAttributes = null;
            List<InnerAttribute> xmlBridgeRoleAttributes = null;

            if (objectType == ObjectType.Classifier || objectType == ObjectType.FactTable)
                xmlObjectAttributes = new List<InnerAttribute>();
            else
            {
                xmlDataRoleAttributes = new List<InnerAttribute>();
                xmlBridgeRoleAttributes = new List<InnerAttribute>();
            }

            XMLDataSource xmlDataSource = null;
            bool canSaveData = true;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            switch (reader.LocalName)
                            {
                                case XmlConsts.tableMetadataNode:
                                    // сравниваем семантики объекта и из XML
                                    CheckNamesAndSemantics(xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic, ref informationTable, objectType);
                                    // сравниваем атрибуты объекта и из XML
                                    if (objectType == ObjectType.Classifier || objectType == ObjectType.FactTable)
                                        canSaveData = CheckAttributes(xmlObjectAttributes, ref informationTable, ref attributesRelationList);
                                    else
                                        canSaveData = CheckConversionTableAttributes(xmlDataRoleAttributes, xmlBridgeRoleAttributes, ref informationTable, ref attributesRelationList);
                                    break;
                            }
                            break;
                        case XmlNodeType.Element:
                            switch (reader.LocalName)
                            {
                                case XmlConsts.dataSourceElement:
                                    xmlDataSource = new XMLDataSource();
                                    // получим параметры источника данных их XML
                                    GetDataSourceParams(reader, xmlDataSource);
                                    break;
                                case XmlConsts.dataNode:
                                    if (xmlDataSource == null)
                                        if (importParams.restoreDataSource)
                                            importParams.restoreDataSource = false;

                                    reader.Close();
                                    return canSaveData;
                                case XmlConsts.tableMetadataNode:
                                    reader.MoveToAttribute(XmlConsts.exportObjectTypeAttribute);

                                    if (!GetMetaDataVariance(reader.Value, objectType, ref informationTable))
                                        return false;

                                    if (objectType == ObjectType.ConversionTable)
                                    {
                                        if (reader.MoveToAttribute(XmlConsts.conversionRuleAttribute))
                                        {
                                            string xmlRule = reader.Value;
                                            if (xmlRule.ToUpper() != conversionRule.ToUpper())
                                                informationTable.Rows.Add(string.Format("Правило перекодировки '{0}'", conversionRule), string.Format("Правило перекодировки '{0}'", xmlRule), false);
                                        }
                                    }

                                    if (objectType == ObjectType.Classifier || objectType == ObjectType.FactTable)
                                    {
                                        GetObjectMetaData(reader, xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic);
                                    }
                                    reader.MoveToAttribute(XmlConsts.uniqueAttributesAttribute);
                                    uniqueAttributes = reader.Value;
                                    break;
                                case XmlConsts.attributeElement:
                                    // получаем данные по отдельному отрибуту схемы, записанному в XML
                                    InnerAttribute attr = GetXMLAttribute(reader);
                                    xmlObjectAttributes.Add(attr);
                                    break;
                                case XmlConsts.dataRoleNode:
                                    // для таблиц перекодировок получаем параметры сопоставляемого классификатора
                                    xmlObjectAttributes = xmlDataRoleAttributes;
                                    GetObjectMetaData(reader, xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic);
                                    break;
                                case XmlConsts.bridgeRoleNode:
                                    // для таблиц перекодировок получаем параметры сопостовимого классификатора
                                    xmlObjectAttributes = xmlBridgeRoleAttributes;
                                    GetObjectMetaData(reader, xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic);
                                    break;
                                case XmlConsts.settingsElement:
                                    GetImportSettings(reader, ref importParams);
                                    break;
                            }
                            break;
                    }
                }
            }
            finally
            {
                reader.Close();
            }
            return true;
        }

        private bool GetMetaDataVariance(string xmlObjectType, ObjectType objectType, ref DataTable informationTable)
        {
            if (objectType != ObjectType.ConversionTable)
            {
                ClassTypes objectClassType = ((IEntity)schemeObject).ClassType;

                if (xmlObjectType == XmlConsts.conversionTable)
                {
                    informationTable.Rows.Add(string.Format("Объект '{0}'", GetObjectType(objectType)), string.Format("Объект '{0}'", GetObjectType(xmlObjectType)), true);
                    return false;
                }

                if (objectClassType == ClassTypes.clsBridgeClassifier && xmlObjectType != XmlConsts.bridgeClassifier)
                {
                    informationTable.Rows.Add(string.Format("Объект '{0}'", GetObjectType(objectType)), string.Format("Объект '{0}'", GetObjectType(xmlObjectType)), false);
                }

                if (objectClassType == ClassTypes.clsDataClassifier && xmlObjectType != XmlConsts.dataClassifier)
                {
                    informationTable.Rows.Add(string.Format("Объект '{0}'", GetObjectType(objectType)), string.Format("Объект '{0}'", GetObjectType(xmlObjectType)), false);
                }

                if (objectClassType == ClassTypes.clsFactData && xmlObjectType != XmlConsts.factTable)
                {
                    informationTable.Rows.Add(string.Format("Объект '{0}'", GetObjectType(objectType)), string.Format("Объект '{0}'", GetObjectType(xmlObjectType)), false);
                }
            }
            else
            {
                if (xmlObjectType != XmlConsts.conversionTable)
                {
                    informationTable.Rows.Add(string.Format("Объект '{0}'", GetObjectType(objectType)), string.Format("Объект '{0}'", GetObjectType(xmlObjectType)), true);
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}
