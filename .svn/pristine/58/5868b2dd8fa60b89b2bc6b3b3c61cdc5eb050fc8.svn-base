using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

using Krista.FM.Server.Common;
using Krista.FM.Server.XmlExportImporter.Helpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter
{
    /// <summary>
    /// Общий класс экспорта импорта и проверки XML для таблиц фактов и классификаторов.
    /// </summary>
    public abstract class ClsDataExportImportBase : ExportImporterBase
    {
        public ClsDataExportImportBase(IScheme scheme, ExportImportManager exportImportManager)
            : base(scheme, exportImportManager)
        {
        }

        #region внутренние переменные
        // импортируемый источник данных
        protected int importerObjectDataSource;
        // импортируемый объект
        protected IEntity schemeObject;
        // атрибуты объекта
        protected List<IDataAttribute> classifierAttributes;
        // поле - ссылка на родителя
        protected string refParentColumnName;
        // поле - на которое ссылается ссылка на родителя
        protected string parentColumnName;
        // показывает, содержит ли объект иерархические данные
        protected bool isHierarchy;
        // наименование объекта данных
        protected string dataSourcesName;
        // импортируемый и экспортируемый источник данных
        protected IDataSource dataSourсe;
        // объект получения данных из базы
        protected IDataReader reader;
        // уникальные атрибуты, по которым идет обновление при импорте
        protected string uniqueAttributes;
        // нижняя граница диапазона ID разработчика
        protected const int developerLowBoundRangeId = 1000000000;

        protected DataOperationsObjectTypes dataOperationsObjectType;

        protected Dictionary<int, int> newIds;

        protected Dictionary<int, int> oldIds;

        protected int refVariant;

        #endregion

        #region методы интерфейса

        public override bool CheckXml(Stream stream, string objectKey, ref ImportPatams importParams, ref DataTable varianceTable)
        {
            return true;
        }

        public override void ExportData(Stream stream, ImportPatams importParams, ExportImportClsParams exportImportClsParams)
        {

        }

        public override Dictionary<int, int> ImportData(Stream stream, ExportImportClsParams exportImportClsParams)
        {
            return null;
        }

        public override void ExportMasterDetail(Stream masterStream, Dictionary<string, Stream> detailStreams,
            ExportImportClsParams exportImportClsParams, ImportPatams importParams)
        {
            GetSchemeObject(exportImportClsParams.ClsObjectKey);
            List<IEntity> details = ExportImportClsObjectsHelper.GetDetailList(schemeObject);
            ExportData(masterStream, importParams, exportImportClsParams);
            foreach (IEntity detail in details)
            {
                ExportImportClsParams detailExportParams = new ExportImportClsParams();
                detailExportParams.ClsObjectKey = detail.ObjectKey;
                detailExportParams.DataSource = exportImportClsParams.DataSource;
                detailExportParams.TaskID = -1;
                detailExportParams.ExportImportObject = detail;

                ImportPatams detailImportParams = new ImportPatams();
                detailImportParams.deleteDataBeforeImport = importParams.deleteDataBeforeImport;
                detailImportParams.deleteDeveloperData = importParams.deleteDeveloperData;
                detailImportParams.refreshDataByAttributes = false;
                detailImportParams.refreshDataByUnique = false;
                detailImportParams.restoreDataSource = false;
                detailImportParams.uniqueAttributesNames = string.Empty;
                detailImportParams.useOldID = false;
                // для каждой детали создаем новые параметры экспорта
                using (IExportImporter exportImporter = _exportImportManager.GetExportImporter(ObjectType.Classifier))
                {
                    exportImporter.ExportData(detailStreams[detail.ObjectKey], detailImportParams, detailExportParams);
                }
            }
        }

        public override Dictionary<string, Dictionary<int, int>> ImportMasterDetail(Stream masterStream,
            Dictionary<string, Stream> detailStreams, ExportImportClsParams exportImportClsParams)
        {
            GetSchemeObject(exportImportClsParams.ClsObjectKey);
            List<IEntity> details = ExportImportClsObjectsHelper.GetDetailList(schemeObject);
            Dictionary<int, int> _newIds = ImportData(masterStream, exportImportClsParams);
            _oldIDList.Add(schemeObject.ObjectKey, _newIds);
            foreach (IEntity detail in details)
            {
                ExportImportClsParams detailExportParams = new ExportImportClsParams();
                detailExportParams.ClsObjectKey = detail.ObjectKey;
                detailExportParams.DataSource = exportImportClsParams.DataSource;
                detailExportParams.TaskID = -1;
                detailExportParams.NewIds = _newIds;
                detailExportParams.ExportImportObject = detail;
                detailExportParams.DetailReferenceColumnName =
                    ExportImportClsObjectsHelper.GetMasterDetailAssociationName(schemeObject, detail);
                // для каждой детали создаем новые параметры экспорта
                using (IExportImporter exportImporter = _exportImportManager.GetExportImporter(ObjectType.Classifier))
                {
                    _oldIDList.Add(detail.ObjectKey,
                        exportImporter.ImportData(detailStreams[detail.ObjectKey], detailExportParams));
                }
            }
            return _oldIDList;
        }


        private Dictionary<string, Dictionary<int, int>> _oldIDList = new Dictionary<string, Dictionary<int, int>>();

        #endregion

        #region экспорт

        /// <summary>
        /// сохранение в XML всех данных для таблиц фактов и классификаторов
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="objectUniqueKey"></param>
        /// <param name="currentDataSource"></param>
        /// <param name="importParams"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        internal void InnerExportClsDataToXml(Stream stream, string objectUniqueKey, int currentDataSource, ImportPatams importParams, string filter, IDbDataParameter[] filterParams)
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
                if (!string.IsNullOrEmpty(filter))
                    query = String.Concat(query, " where ", filter);
                if (isHierarchy)
                    query = String.Concat(query, String.Format(" order by {0}", refParentColumnName));
                else
                    if (parentColumnName != null)
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
                SaveDataToStream(stream);
                tick = Environment.TickCount - tick;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// сохранение выделенных записей в XML для классификаторов и таблиц фактов
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="objectUniqueKey"></param>
        /// <param name="currentDataSource"></param>
        /// <param name="importParams"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        /// <param name="selectedRowsID"></param>
        internal void InnerExportSelectedClsDataToXml(Stream stream, string objectUniqueKey, int currentDataSource,
            ImportPatams importParams, string filter, IDbDataParameter[] filterParams, int[] selectedRowsID)
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
            InnerExportClsDataToXml(stream, objectUniqueKey, currentDataSource, importParams, filter, filterParams);
        }


        /// <summary>
        /// запись атрибутов объекта схемы в XML
        /// </summary>
        /// <param name="attributesList"></param>
        /// <param name="xmlWriter"></param>
        internal void SaveSchemeAttributesToXML(List<IDataAttribute> attributesList, XmlWriter xmlWriter)
        {
            foreach (IDataAttribute attr in attributesList)
            {
                xmlWriter.WriteStartElement(XmlConsts.attributeElement);
                if (attr != null)
                {
                    ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.nameAttribute, attr.Name);
                    if (attr.Caption != String.Empty)
                        ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.rusNameAttribute, attr.Caption);
                    else
                        ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.rusNameAttribute,
                            ExportImportClsObjectsHelper.GetDataObjSemanticRus(schemeObject, _scheme)
                            + ExportImportClsObjectsHelper.GetBridgeClsCaptionByRefName(schemeObject, attr.Name));
                    ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.sizeAttribute, attr.Size.ToString());
                    ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.typeAttribute, attr.Type.ToString());
                    ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.nullableAttribute, attr.IsNullable.ToString());
                    if (attr.DefaultValue != null)
                        ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.defaultValueAttribute, attr.DefaultValue.ToString());
                }
                xmlWriter.WriteEndElement();
            }
        }


        /// <summary>
        /// запись параметров источника данных в XML
        /// </summary>
        /// <param name="dataSourse"></param>
        /// <param name="xmlWriter"></param>
        internal void SaveDataSourceParamsToXML(IDataSource dataSourse, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(XmlConsts.usedDataSourcesNode);
            xmlWriter.WriteStartElement(XmlConsts.dataSourceElement);
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.idAttribute, dataSourse.ID.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.suppplierCodeAttribute, dataSourse.SupplierCode.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.dataCodeAttribute, dataSourse.DataCode);
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.dataNameAttribute, dataSourse.DataName);
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.kindsOfParamsAttribute, dataSourse.ParametersType.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.nameAttribute, dataSourse.BudgetName);
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.yearAttribute, dataSourse.Year.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.monthAttribute, dataSourse.Month.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.variantAttribute, dataSourse.Variant);
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.quarterAttribute, dataSourse.Quarter.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.territoryAttribute, dataSourse.Territory);
            ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.dataSourceNameAttribute, dataSourcesName);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// запись данных классификаторов и таблиц фактов в XML
        /// </summary>
        /// <param name="writer"></param>
        internal void WriteClsData(XmlWriter writer, int hierarchyIndex)
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
                    if (!(values[i] is byte[]))
                        writer.WriteValue(values[i].ToString());
                    else
                        //writer.WriteValue(Convert.ToBase64String((byte[])values[i]));
                        writer.WriteCData(Convert.ToBase64String((byte[])values[i], Base64FormattingOptions.InsertLineBreaks));
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

        /// <summary>
        /// возвращает объект для просмотра записей по одной штуке (DataReader)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IDataReader GetDataReader(IDatabase db, IDbConnection connection, string query, IDbDataParameter[] filterParams)
        {
            IDataReader reader = null;

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            IDbCommand command = connection.CreateCommand();

            if (filterParams != null)
                foreach (var param in filterParams)
                {
                    param.SourceVersion = DataRowVersion.Current;
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
        internal void SaveDataToStream(Stream stream)
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
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.versionAttribute, _scheme.UsersManager.ServerLibraryVersion());
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

            string objectClass = string.Empty;
            switch (schemeObject.ClassType)
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

            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.exportObjectTypeAttribute, objectClass);

            CreateObjectAttributeAttributes(writer, schemeObject.Name, schemeObject.FullCaption, schemeObject.Semantic);

            if (innerImportParams.uniqueAttributesNames != string.Empty)
                ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.uniqueAttributesAttribute, innerImportParams.uniqueAttributesNames);
            //  для каждого объекта схемы записываем все атрибуты с параметрами
            SaveSchemeAttributesToXML(classifierAttributes, writer);

            writer.WriteEndElement();
            writer.WriteEndElement();
            // параметры источника данных
            if (dataSourсe != null)
                SaveDataSourceParamsToXML(dataSourсe, writer);
            // запись секции с данными
            WriteClsData(writer, hierarchyIndex);
            // закрываем корневой элемент и документ в целом
            writer.WriteEndElement();
            writer.WriteEndDocument();
            // закрывает поток и записывает все в файл
            writer.Flush();
            writer.Close();
        }

        #endregion

        #region импорт

        /// <summary>
        /// запись в протокол о каких то несовпадениях в атрибутах
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="message"></param>
        /// <param name="classifiersEventKind"></param>
        internal void SendMessageToClassifierProtocol(IClassifiersProtocol protocol, string message, ClassifiersEventKind classifiersEventKind)
        {
            //if (schemeObject is IClassifier)
            protocol.WriteEventIntoClassifierProtocol(classifiersEventKind, schemeObject.OlapName,
                -1, importerObjectDataSource, (int)schemeObject.ClassType, message);
        }

        internal void BeginImportProtocolMessage(IClassifiersProtocol protocol)
        {
            SendMessageToClassifierProtocol(protocol, "Начало операции импорта", ClassifiersEventKind.ceImportClassifierData);
        }

        internal void SuccefullEndImportProtocolMessage(IClassifiersProtocol protocol)
        {
            SendMessageToClassifierProtocol(protocol, "Операции импорта завершилась успешно", ClassifiersEventKind.ceSuccefullFinished);
        }

        private bool CheckAttributes(string xmlName, string xmlRusName, string xmlSemantic,
            string xmlRusSemantic, List<InnerAttribute> xmlAttributes,
             ref List<AttributesRelation> attributesRelationList)
        {
            DataTable dtProtocol = GetInformationTable(ObjectType.Classifier, schemeObject.ClassType);
            CheckNamesAndSemantics(xmlName, xmlRusName, xmlSemantic, xmlRusSemantic, ref dtProtocol);
            return CheckAttributes(classifierAttributes, xmlAttributes, ref dtProtocol, ref attributesRelationList);
        }

        internal bool CheckAttributes(List<InnerAttribute> xmlAttributes, ref DataTable checkProtocolTable,
             ref List<AttributesRelation> attributesRelationList)
        {
            return CheckAttributes(classifierAttributes, xmlAttributes, ref checkProtocolTable, ref attributesRelationList);
        }


        internal void CheckNamesAndSemantics(string xmlName, string xmlRusName, string xmlSemantic,
            string xmlRusSemantic, ref DataTable pritocolTable)
        {
            // проверять классификаторы и таблицы перекодировок будем по разному... 
            // если это классификатор или таблица фактов, то сравним название и семантику объекта и то, что записано в XML
            if (xmlName != schemeObject.Name)
            {
                pritocolTable.Rows.Add(string.Format("Наименование '{0}' ('{1}')",
                    schemeObject.Caption, schemeObject.Name),
                    string.Format("Наименование '{0}' ('{1}')", xmlRusName, xmlName), false);
            }
            if (xmlSemantic != schemeObject.Semantic)
            {
                pritocolTable.Rows.Add(string.Format("Семантика '{0}' ('{1}')",
                    ExportImportClsObjectsHelper.GetDataObjSemanticRus(schemeObject, _scheme), schemeObject.Semantic),
                    string.Format("Семантика '{0}' ('{1}')", xmlRusSemantic, xmlSemantic), false);
            }
        }

        /// <summary>
        /// стравнивает две коллекции атрибутов. На выходе коллекция 
        /// какой атрибут из XML соответствует атрибуту из схемы
        /// </summary>
        /// <param name="attributesList"></param>
        /// <param name="xmlAttributes"></param>
        /// <param name="checkProtocolTable"></param>
        /// <param name="attributesRelationList"></param>
        internal bool CheckAttributes(List<IDataAttribute> attributesList, List<InnerAttribute> xmlAttributes,
            ref DataTable checkProtocolTable, ref List<AttributesRelation> attributesRelationList)
        {
            bool returnValue = true;
            for (int i = 0; i <= attributesList.Count - 1; i++)
            {
                AttributesRelation attributeRelation = new AttributesRelation();
                attributeRelation.objectAttributeIndex = i;
                attributeRelation.xmlAttributeIndex = -1;
                attributeRelation.attrName = attributesList[i].Name;
                attributeRelation.DBType = ExportImportDBHelper.GetDBTypeFromAttribute(attributesList[i]);
                for (int j = 0; j <= xmlAttributes.Count - 1; j++)
                {
                    if (attributesList[i].Name == xmlAttributes[j].name)
                    {
                        // при несовпадении каких либо свойств атрибутов будем все равно пытаться загрузить данные
                        if (attributesList[i].Size != xmlAttributes[j].size)
                        {
                            if (attributesList[i].Kind == DataAttributeKindTypes.Regular && attributesList[i].Visible)
                                checkProtocolTable.Rows.Add(string.Format("Свойство 'Размер' (size) = {0} атрибута '{1}' ({2})", attributesList[i].Size, attributesList[i].Caption, attributesList[i].Name),
                                string.Format("Свойство 'Размер' (size) = {0} атрибута '{1}' ({2})", xmlAttributes[j].size, attributesList[i].Caption, attributesList[i].Name), false);
                        }
                        if (!attributesList[i].IsNullable && xmlAttributes[j].nullable)
                        {
                            checkProtocolTable.Rows.Add(string.Format("Поле '{0}' ({1}) обязательно для заполнения", attributesList[i].Caption, attributesList[i].Name),
                                string.Format("Поле '{0}' ({1}) не обязательно для заполнения", attributesList[i].Caption, attributesList[i].Name), false);
                        }
                        attributeRelation.xmlAttributeIndex = j;
                        break;
                    }
                }
                // если названия атрибутов не совпали, заносим это в список
                string attribureName = attributesList[i].Name;
                string attributeCaption = attributesList[i].Caption;
                if (attributeCaption == string.Empty)
                    if (attributesList[i].Class == DataAttributeClassTypes.Reference)
                        attributeCaption = ExportImportClsObjectsHelper.GetDataObjSemanticRus(schemeObject, _scheme) +
                            ExportImportClsObjectsHelper.GetBridgeClsCaptionByRefName(schemeObject, attributesList[i].Name);

                if (attributeRelation.xmlAttributeIndex == -1)
                {
                    // не рассматриваем 3 поля: источник данных, задачу и закачку
                    if (attribureName != "SourceID" && attribureName != "TaskID" && attribureName != "PumpID")
                    {

                        if (attributesList[i].DefaultValue == null && !attributesList[i].IsNullable)
                        {
                            // атрибут, необходимый для записи данных в базу отсутствует в XML
                            if (attributesList[i].Kind == DataAttributeKindTypes.Regular && attributesList[i].Visible)
                                checkProtocolTable.Rows.Add(string.Format("Атрибут '{0}' ({1})",
                                    attributeCaption, attribureName), "Отсутствует", true);

                            returnValue = false;
                        }
                        else
                            if (attributesList[i].Kind == DataAttributeKindTypes.Regular && attributesList[i].Visible)
                                checkProtocolTable.Rows.Add(string.Format("Атрибут '{0}' ({1})",
                                    attributeCaption, attribureName), "Отсутствует", false);
                    }
                }
                attributesRelationList.Add(attributeRelation);
            }
            return returnValue;
        }

        internal IDbDataParameter[] CloneIDbDataParameters(IDatabase db, IDbDataParameter[] dataParameters)
        {
            IDbDataParameter[] tmpDataParameters = new IDbDataParameter[dataParameters.Length];
            for (int i = 0; i <= dataParameters.Length - 1; i++)
            {
                tmpDataParameters[i] = db.CreateParameter(dataParameters[i].ParameterName, dataParameters[i].Value);
            }
            return tmpDataParameters;
        }

        /// <summary>
        /// получение необходимых объекттов для записи одной записи через запрос
        /// </summary>
        /// <param name="attributesRelationList"></param>
        /// <param name="row"></param>
        /// <param name="db"></param>
        /// <param name="paramsList"></param>
        /// <param name="fields"></param>
        internal virtual void GetRowParams(List<AttributesRelation> attributesRelationList, DataRow row, IDatabase db,
            ref string[] fields, ref Dictionary<string, IDbDataParameter> dbParams)
        {
            int columnCount = row.Table.Columns.Count;
            fields = new string[columnCount];
            for (int i = 0; i <= columnCount - 1; i++)
            {
                string columnName = row.Table.Columns[i].ColumnName;
                fields[i] = row.Table.Columns[i].ColumnName;
                object value = row[i];
                if (ExportImportDBHelper.GetDBTypeFromAttribute(classifierAttributes[i]) == DbType.Boolean)
                    if (value != null && !(value is DBNull))
                    {
                        if (value is String)
                            value = Convert.ToBoolean(value);
                        else
                            value = Convert.ToInt32(value);
                    }
                dbParams.Add(columnName, db.CreateParameter(columnName, value,
                    ExportImportDBHelper.GetDBTypeFromAttribute(classifierAttributes[i])));
            }
        }

        /// <summary>
        /// сохранение одной записи
        /// </summary>
        /// <param name="row"></param>
        /// <param name="db"></param>
        /// <param name="attributesRelationList"></param>
        internal void SaveSingleDataRow(DataRow row, IDatabase db, List<AttributesRelation> attributesRelationList)
        {
            Dictionary<string, IDbDataParameter> dbParams = new Dictionary<string, IDbDataParameter>();
            string[] fields = new string[attributesRelationList.Count];
            GetRowParams(attributesRelationList, row, db, ref fields, ref dbParams);
            InsertOrUpdateRow(db, fields, dbParams);
        }

        #region добавление или изменение записи в таблице

        /// <summary>
        /// добавление новой записи
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fields"></param>
        /// <param name="dbParams"></param>
        internal void InsertNewRow(IDatabase db, string[] fields, Dictionary<string, IDbDataParameter> dbParams)
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


        internal Dictionary<long, long> UpdateIdList
        {
            get; set;
        }

        /// <summary>
        /// вставляет запись или апдейтит, если запись с некоторыми параметрами присутс
        /// </summary>
        internal void InsertOrUpdateRow(IDatabase db, string[] fields, Dictionary<string, IDbDataParameter> dbParams)
        {
            // если указан параметр обновления, то пытаемся обновить
            if (!string.IsNullOrEmpty(uniqueAttributes) && (innerImportParams.refreshDataByUnique || innerImportParams.refreshDataByAttributes))
            {
                // по параметрам уникальности запрашиваем количество записей с таким набором этих параметров
                // создаем параметры для запроса записи
                List<IDbDataParameter> selectParams = new List<IDbDataParameter>();
                // запрашиваем есть ли такая запись уже
                List<string> filter = new List<string>();
                // создаем параметры для запроса количества таких записей
                foreach (string str in uniqueAttributes.Split(','))
                {
                    if (dbParams.ContainsKey(str))
                    {
                        if (dbParams[str].DbType == DbType.AnsiStringFixedLength || dbParams[str].DbType == DbType.AnsiString)
                        {
                            selectParams.Add(db.CreateParameter(string.Format("p{0}", selectParams.Count),
                                Convert.ToString(dbParams[str].Value).ToUpper()));
                            filter.Add(String.Format("(UPPER({0}) = ?)", str));
                        }
                        else
                        {
                            selectParams.Add(db.CreateParameter(string.Format("p{0}", selectParams.Count), dbParams[str].Value));
                            filter.Add(String.Format("({0} = ?)", str));
                        }
                    }
                }

                // запрос данных по уникальному набору полей
                string queryFilter = string.Empty;
                if (importerObjectDataSource > 0)
                    queryFilter = string.Format("({0}) and (SourceID = {1}) and (RowType = 0)",
                        String.Join(" and ", filter.ToArray()), importerObjectDataSource);
                else
                    queryFilter = string.Format("({0}) and (RowType = 0)", String.Join(" and ", filter.ToArray()));

                string selectQuery = String.Format("select ID from {0} where {1}",
                        schemeObject.FullDBName, queryFilter);
                // количество записей, которые могут присутствовать с такими же значениями по нескольким полям
                DataTable records = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable, selectParams.ToArray());

                if (records.Rows.Count > 0)
                {
                    if (dbParams.ContainsKey("ID"))
                        UpdateIdList.Add(Convert.ToInt64(dbParams["ID"].Value), Convert.ToInt64(records.Rows[0]["ID"]));
                    // если нашли такую запись, то апдейтим у нее все поля кроме ID и ссылки на родителя
                    // хотя лучше не апдейтить вообще все ссылки и поля типа источник, задача и прочее
                    List<string> nonUpdateFields = new List<string>();
                    foreach (IDataAttribute attr in classifierAttributes)
                    {
                        if (!IsUpdatedAttribute(attr))
                            nonUpdateFields.Add(attr.Name);
                    }
                    // все данные, которые будут учавствовать в запросе как параметры
                    List<IDbDataParameter> updateParamList = new List<IDbDataParameter>();
                    List<string> updateFields = new List<string>();
                    // получаем параметры для апдейта записи
                    foreach (IDbDataParameter param in dbParams.Values)
                    {
                        if (!nonUpdateFields.Contains(param.ParameterName))
                        {
                            updateParamList.Add(db.CreateParameter(string.Format("p{0}", updateParamList.Count), param.Value, param.DbType));
                            updateFields.Add(String.Format("{0} = ?", param.ParameterName));
                        }
                    }

                    // если не нашлось полей для апдейта, ничего не делаем
                    if (updateParamList.Count == 0) return;

                    foreach (string str in uniqueAttributes.Split(','))
                    {
                        if (dbParams.ContainsKey(str))
                        {
                            if (dbParams[str].DbType == DbType.AnsiStringFixedLength || dbParams[str].DbType == DbType.AnsiString)
                            {
                                updateParamList.Add(db.CreateParameter(string.Format("p{0}", updateParamList.Count), Convert.ToString(dbParams[str].Value).ToUpper()));
                                filter.Add(String.Format("(UPPER({0}) = ?)", str));
                            }
                            else
                            {
                                updateParamList.Add(db.CreateParameter(string.Format("p{0}", updateParamList.Count), dbParams[str].Value));
                                filter.Add(String.Format("({0} = ?)", str));
                            }
                        }
                    }

                    // создаем запрос на обновление записи
                    string updateQuery = String.Format("update {0} set {1} where {2}",
                        schemeObject.FullDBName, string.Join(",", updateFields.ToArray()), queryFilter);

                    db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, updateParamList.ToArray());
                    updateRowsCount++;
                }
                else
                {
                    if (isHierarchy)
                    {
                        /*object parentId = dbParams["ParentID"].Value;
                        if (parentId != null && parentId != DBNull.Value && UpdateIdList.ContainsKey(Convert.ToInt64(parentId)))
                            dbParams["ParentID"].Value = UpdateIdList[Convert.ToInt64(parentId)];*/
                    }
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

        private bool IsUpdatedAttribute(IDataAttribute attr)
        {
            if (attr.Class == DataAttributeClassTypes.Reference)
                return true;
            if (attr.Kind == DataAttributeKindTypes.Sys)
                return false;
            if (attr.Name == "ID" || attr.Name == "ParentID" || attr.Name == "SourceID" ||
                attr.Name == "RowType" || attr.Name == "PumpID" || attr.Name == "TaskID" ||
                uniqueAttributes.Contains(attr.Name))
                return false;
            return true;
        }

        protected virtual void SaveHierarchyData(DataSet dataSet, IDatabase db,
            List<AttributesRelation> attributesRelationList)
        {

        }

        protected virtual void SaveRow(DataRow newRow, List<AttributesRelation> attributesRelationList, IDatabase db)
        {

        }

        protected virtual void SetSystemRefs(DataRow newRow, int dataSourceIdIndex, int pumpIdIndex, int taskIdIndex)
        {

        }

        /// <summary>
        /// добавление записи в базу или в иерархический набор данных
        /// </summary>
        protected virtual void AddRow(List<AttributesRelation> attributesRelationList, DataSet dataSet,
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
            if (!string.IsNullOrEmpty(uniqueAttributes) && (innerImportParams.refreshDataByUnique || innerImportParams.refreshDataByAttributes))
            {

            }
            else
            {
                foreach (int i in refColumnsIndexes)
                    newRow[i] = -1;
            }
            SetSystemRefs(newRow, dataSourceIdIndex, pumpIdIndex, taskIdIndex);
            newRow.EndEdit();
            // сохраняем строку
            if (!isHierarchy)
                SaveRow(newRow, attributesRelationList, db);
            else
                dataSet.Tables[0].Rows.Add(newRow);
        }

        #endregion

        /// <summary>
        /// получение параметров источника дынных из XML
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="xmlDataSource"></param>
        internal static void GetDataSourceParams(XmlTextReader reader, XMLDataSource xmlDataSource)
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
        /// общий метод для сохраниения данных из XML
        /// </summary>
        protected Dictionary<int, int> ImportDataFromXML(Stream xmlStream, IDatabase db,
            string filter, IDbDataParameter[] filterParams, IClassifiersProtocol protocol)
        {
            var xmlReader = new XmlTextReader(xmlStream);
            // структура для хранения одной записи, получаемой из XML
            var rowCellsValues = new List<object>();
            var refColumnsIndexes = new List<int>();
            // отношения между данными. Хранит соответствие данных из XML атрибутам серверного объекта
            var attributesRelationList = new List<AttributesRelation>();
            DataSet dsData = GetDataSet(isHierarchy);
            UpdateIdList = new Dictionary<long, long>();
            // объекты для хранения семантик и наименований, полученных из XML
            string xmlObjectName = string.Empty;
            string xmlObjecSemantic = string.Empty;
            // для русских наименований
            string xmlRusObjectName = string.Empty;
            string xmlRusObjecSemantic = string.Empty;
            // структуры для хранения атрибутов, полученных из XML
            // для хранения атрибутов классификаторов и таблиц фактов
            var xmlObjectAttributes = new List<InnerAttribute>();
            int dataSourceIdIndex = -1;
            int pumpIdIndex = -1;
            int taskIdIndex = -1;
            GetRefColumnsIndexes(ref dataSourceIdIndex, ref pumpIdIndex, ref taskIdIndex, ref refColumnsIndexes);
            newIds = new Dictionary<int, int>();
            var xmlDataSource = new XMLDataSource();
            dsData.Tables[0].BeginLoadData();
            // читаем данные
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        switch (xmlReader.LocalName)
                        {
                            case XmlConsts.tableMetadataNode:
                                // сравниваем атрибуты объекта и из XML
                                bool canSaveData = CheckAttributes(xmlObjectName, xmlRusObjectName,
                                    xmlObjecSemantic, xmlRusObjecSemantic, xmlObjectAttributes,
                                    ref attributesRelationList);
                                if (!canSaveData)
                                    // сохранить данные не получится. чистим все коллекции и выходим
                                    return newIds;
                                break;
                            case XmlConsts.rowNode:
                                GetNonGeneratedId(rowCellsValues);
                                AddRow(attributesRelationList, dsData, dataSourceIdIndex, pumpIdIndex, taskIdIndex, refColumnsIndexes, rowCellsValues, db);
                                break;
                            case XmlConsts.dataTableNode:
                                if (isHierarchy)
                                {
                                    SaveHierarchyData(dsData, db, attributesRelationList);
                                }
                                if (innerImportParams.useOldID)
                                    SetGeneratorValue(db);
                                    //SetGeneratorsNormalValues(LastId, lastDeveloperId, db);
                                break;
                        }
                        break;
                    case XmlNodeType.Element:
                        switch (xmlReader.LocalName)
                        {
                            case XmlConsts.settingsElement:
                                // читаем параметры импорта из XML
                                GetImportSettings(xmlReader, ref innerImportParams);
                                if (innerImportParams.deleteDataBeforeImport)
                                    DeleteData(db, filter, filterParams);
                                break;
                            case XmlConsts.tableMetadataNode:
                                GetObjectMetaData(xmlReader, ref xmlObjectName, ref xmlRusObjectName, ref xmlObjecSemantic, ref xmlRusObjecSemantic);
                                if (xmlReader.MoveToAttribute(XmlConsts.uniqueAttributesAttribute))
                                    uniqueAttributes = xmlReader.Value;
                                break;
                            case XmlConsts.attributeElement:
                                // получаем данные по отдельному отрибуту схемы, записанному в XML
                                InnerAttribute attr = GetXMLAttribute(xmlReader);
                                xmlObjectAttributes.Add(attr);
                                break;
                            case XmlConsts.dataSourceElement:
                                // получим параметры источника данных их XML
                                GetDataSourceParams(xmlReader, xmlDataSource);
                                // если стоит параметр импорта "восстанавливать источник"
                                if (innerImportParams.restoreDataSource)
                                    importerObjectDataSource = DataSourceHelper.AddDataSource(_scheme, xmlDataSource, schemeObject, protocol);
                                break;
                            case XmlConsts.rowNode:
                                // начинаем читать запись
                                rowCellsValues.Clear();
                                break;
                            case XmlConsts.cellElement:
                                // читаем одну ячейку
                                string newValue = xmlReader.ReadElementString();
                                if (!string.IsNullOrEmpty(newValue))
                                    rowCellsValues.Add(newValue);
                                else
                                    rowCellsValues.Add(DBNull.Value);
                                break;
                        }
                        break;
                }
            }
            return newIds;
        }

        /// <summary>
        /// метод получения 
        /// </summary>
        protected virtual void GetNonGeneratedId(List<object> rowCellsValues)
        {

        }

        private DataSet GetDataSet(bool isHierarchyData)
        {
            DataSet ds = isHierarchyData ? CreateHierarchyDataSet() : CreateFlatDataSet();
            return ds;
        }

        /// <summary>
        /// ставит генераторам классификаторов значения больше тех, что импортировали
        /// </summary>
        /// <param name="lastImportedId"></param>
        /// <param name="lastImportedDeveloperId"></param>
        /// <param name="db"></param>
        private void SetGeneratorsNormalValues(int lastImportedId, int lastImportedDeveloperId, IDatabase db)
        {
            /*string developerGeneratorName = GetGeneratorName(schemeObject, true);
            // получим название обычного генератора 
            string generatorName = GetGeneratorName(schemeObject, false);

            int newDevelopId = db.GetGenerator(developerGeneratorName);
            int newId = db.GetGenerator(generatorName);
            // значения для генератора в режиме разработчика
            if (newDevelopId <= lastImportedDeveloperId)
            {
                for (int i = 0; i < lastImportedDeveloperId - newDevelopId; i++)
                {
                    db.GetGenerator(developerGeneratorName);
                }
            }
            // значения для генератора не в режиме разработчика
            if (newId <= lastImportedId)
            {
                for (int i = 0; i < lastImportedId - newId; i++)
                {
                    db.GetGenerator(generatorName);
                }
            }*/
        }



        #endregion

        #region проверка XML

        public bool InnerCheckXml(Stream stream, string objectUniqueKey, ObjectType ojectType, ref ImportPatams importParams, ref DataTable varianceTable)
        {
            GetSchemeObject(objectUniqueKey);
            return ReadMetadataFromStream(stream, ojectType, ref importParams, ref varianceTable);
        }

        private bool ReadMetadataFromStream(Stream stream, ObjectType objectType, ref ImportPatams importParams, ref DataTable informationTable)
        {
            // объект для чтения из XML
            XmlTextReader reader = new XmlTextReader(stream);
            // отношения между данными. Хранит соответствие данных из XML атрибутам серверного объекта
            List<AttributesRelation> attributesRelationList = new List<AttributesRelation>();
            // таблица для хранения несоответствия 
            informationTable = GetInformationTable(objectType, schemeObject.ClassType);

            // объекты для хранения семантик и наименований, полученных из XML
            string xmlObjectName = string.Empty;
            string xmlObjecSemantic = string.Empty;
            // для русских наименований
            string xmlRusObjectName = string.Empty;
            string xmlRusObjecSemantic = string.Empty;

            // структуры для хранения атрибутов, полученных из XML
            // для хранения атрибутов классификаторов и таблиц фактов
            List<InnerAttribute> xmlObjectAttributes = new List<InnerAttribute>();

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
                                    CheckNamesAndSemantics(xmlObjectName, xmlRusObjectName, xmlObjecSemantic, xmlRusObjecSemantic, ref informationTable);
                                    // сравниваем атрибуты объекта и из XML
                                    canSaveData = CheckAttributes(xmlObjectAttributes, ref informationTable, ref attributesRelationList);
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

                                    GetObjectMetaData(reader, ref xmlObjectName, ref xmlRusObjectName, ref xmlObjecSemantic, ref xmlRusObjecSemantic);

                                    if (reader.MoveToAttribute(XmlConsts.uniqueAttributesAttribute))
                                        uniqueAttributes = reader.Value;
                                    break;
                                case XmlConsts.attributeElement:
                                    // получаем данные по отдельному отрибуту схемы, записанному в XML
                                    InnerAttribute attr = GetXMLAttribute(reader);
                                    xmlObjectAttributes.Add(attr);
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
            if (xmlObjectType == XmlConsts.conversionTable)
            {
                informationTable.Rows.Add(string.Format("Объект '{0}'",
                    ExportImportClsObjectsHelper.GetObjectType(objectType, schemeObject.ClassType)),
                    string.Format("Объект '{0}'", ExportImportClsObjectsHelper.GetObjectType(xmlObjectType)), true);
                return false;
            }

            if (schemeObject.ClassType == ClassTypes.clsBridgeClassifier && xmlObjectType != XmlConsts.bridgeClassifier)
            {
                informationTable.Rows.Add(string.Format("Объект '{0}'",
                    ExportImportClsObjectsHelper.GetObjectType(objectType, schemeObject.ClassType)),
                    string.Format("Объект '{0}'", ExportImportClsObjectsHelper.GetObjectType(xmlObjectType)), false);
            }

            if (schemeObject.ClassType == ClassTypes.clsDataClassifier && xmlObjectType != XmlConsts.dataClassifier)
            {
                informationTable.Rows.Add(string.Format("Объект '{0}'",
                    ExportImportClsObjectsHelper.GetObjectType(objectType, schemeObject.ClassType)),
                    string.Format("Объект '{0}'", ExportImportClsObjectsHelper.GetObjectType(xmlObjectType)), false);
            }

            if (schemeObject.ClassType == ClassTypes.clsFactData && xmlObjectType != XmlConsts.factTable)
            {
                informationTable.Rows.Add(string.Format("Объект '{0}'",
                    ExportImportClsObjectsHelper.GetObjectType(objectType, schemeObject.ClassType)),
                    string.Format("Объект '{0}'", ExportImportClsObjectsHelper.GetObjectType(xmlObjectType)), false);
            }

            return true;
        }

        protected virtual void GetRefColumnsIndexes(ref int dataSourseIndex, ref int dataPumpIndex, ref int taskIndex, ref List<int> refAssociateColumns)
        {
            // получаем индексы полей со ссылками на сопоставимые классификаторы... при импорте их в -1
            const string source = "SOURCEID";
            const string pump = "PUMPID";
            const string task = "TASKID";

            for (int i = 0; i <= classifierAttributes.Count - 1; i++)
            {
                if (classifierAttributes[i].Name.ToUpper() == source)
                    dataSourseIndex = i;
                if (classifierAttributes[i].Name.ToUpper() == pump)
                    dataPumpIndex = i;
                if (classifierAttributes[i].Name.ToUpper() == task)
                    taskIndex = i;
            }
        }

        #endregion

        #region вспомогательные методы

        /// <summary>
        /// создание иерархического DataSet
        /// </summary>
        /// <returns></returns>
        internal DataSet CreateHierarchyDataSet()
        {
            DataSet hierarchyDataSet = CreateFlatDataSet();
            DataRelation relation = new DataRelation("relation", hierarchyDataSet.Tables[0].Columns[parentColumnName],
                hierarchyDataSet.Tables[0].Columns[refParentColumnName]);
            hierarchyDataSet.Relations.Add(relation);
            return hierarchyDataSet;
        }

        /// <summary>
        /// создание объекта DataSet с одной таблицей
        /// </summary>
        /// <returns></returns>
        internal DataSet CreateFlatDataSet()
        {
            DataSet hierarchyDataSet = new DataSet();
            hierarchyDataSet.Tables.Add(ExportImportDBHelper.CreateDataTableFromAttributes(classifierAttributes.ToArray()));
            return hierarchyDataSet;
        }

        /// <summary>
        /// получение объекта схемы по ключу и получение атрибутов объекта
        /// </summary>
        /// <param name="objectUniqueKey"></param>
        internal virtual void GetSchemeObject(string objectUniqueKey)
        {

        }

        internal static DataTable GetInformationTable(ObjectType objectType, ClassTypes classType)
        {
            DataTable checkProtocolTable = new DataTable();
            DataColumn column = checkProtocolTable.Columns.Add("ObjectSide", typeof(string));
            column.Caption = string.Format("В {0}", ExportImportClsObjectsHelper.GetObjectType(objectType, classType, false));
            column = checkProtocolTable.Columns.Add("XmlSide", typeof(string));
            column.Caption = "В XML";
            checkProtocolTable.Columns.Add("CriticalError", typeof(bool));
            return checkProtocolTable;
        }

        /// <summary>
        /// предварительное удаление данных, если предусмотрено параметрами 
        /// </summary>
        internal void DeleteData(IDatabase db, string filter, IDbDataParameter[] filterParams)
        {
            // если фильтр пуст, создаем новый
            if (string.IsNullOrEmpty(filter))
            {
                // удаляем все записи по текущему источнику
                if (importerObjectDataSource >= 0)
                {
                    filter = string.Format("(RowType = 0 and SourceID = {0})", importerObjectDataSource);
                }
            }
            else
            {
                // если указано восстановление источника, то и удаляем записи по тому источнику
                string[] filterParts = filter.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries);
                string[] newFilter = new string[filterParts.Length];
                for (int i = 0; i <= filterParts.Length - 1; i++)
                {
                    if (filterParts[i].Contains("SourceID"))
                        newFilter[i] = string.Format("(SourceID = {0})", importerObjectDataSource);
                    else
                        newFilter[i] = filterParts[i].Trim();
                }
                filter = String.Join(" and ", newFilter);
            }

            if (!innerImportParams.deleteDeveloperData)
            {
                filter = String.Concat(filter, string.Format(" and ID < {0}", developerLowBoundRangeId));
            }
            deleteRowsCount = DeleteDataFromSchemeObject(db, filter, filterParams);
        }

        /// <summary>
        /// удаление данных из объекта
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        internal int DeleteDataFromSchemeObject(IDatabase db, string filter, IDbDataParameter[] filterParams)
        {
            string deleteQuery = String.Format("delete from {0} where {1}", schemeObject.FullDBName, filter);
            if (filterParams == null)
                return Convert.ToInt32(db.ExecQuery(deleteQuery, QueryResultTypes.NonQuery));
            return Convert.ToInt32(db.ExecQuery(deleteQuery, QueryResultTypes.NonQuery, filterParams));
        }

        /// <summary>
        /// Последняя id загруженных записей
        /// </summary>
        protected int LastId
        {
            get; set;
        }

        protected int lastDeveloperId = 0;
        protected string detailReferenceColumnName = string.Empty;

        internal virtual void SetGeneratorValue(IDatabase db)
        {
            
        }

        /// <summary>
        /// сохранение иерархических данных
        /// </summary>
        internal void SaveHierarchyData(DataTable dataTable, IDatabase db, List<AttributesRelation> attributesRelationList)
        {
            foreach (var row in dataTable.Rows.Cast<DataRow>().Where(w => w.IsNull(refParentColumnName)))
            {
                SaveSingleDataRow(row, db, attributesRelationList);
                SaveHierarchyData(dataTable, db, attributesRelationList, row);
            }
        }

        internal void SaveHierarchyData(DataTable dataTable, IDatabase db, List<AttributesRelation> attributesRelationList, DataRow parentRow)
        {
            foreach (var row in dataTable.Rows.Cast<DataRow>().Where(w => !w.IsNull(refParentColumnName) &&
                Convert.ToInt64(w[refParentColumnName]) == Convert.ToInt64(parentRow["ID"])))
            {
                SaveSingleDataRow(row, db, attributesRelationList);
                SaveHierarchyData(dataTable, db, attributesRelationList, row);
            }
        }

        protected virtual string GetGeneratorName(IEntity schemeObject, bool developMode)
        {
            return string.Empty;
        }

        #endregion

    }
}
