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
    class ConversionTablesExportImport : ExportImporterBase, IConversionTableExportImporter
    {

        public ConversionTablesExportImport(IScheme scheme, ExportImportManager exportImportManager)
            : base(scheme, exportImportManager)
        {

        }

        #region внутренние переменные

        IAssociation currentAssociation;

        string filterConversionTable;

        string conversionRule;

        IConversionTable conversionTable;

        List<IDataAttribute> dataRoleAttributes;
        List<IDataAttribute> bridgeRoleAttributes;

        internal List<string> objectName;
        internal List<string> rusObjectFullCaption;
        internal List<string> objectSemantic;
        internal List<string> rusObjectSemantic;

        string curreantTableName;

        #endregion


        #region реализация интерфейса

        public void ExportData(Stream stream, string associationName, 
            string ruleName, ImportPatams importParams)
        {
            InnerExportConversionTable(stream, associationName, ruleName, importParams);
        }

        public void ExportSelectedData(Stream stream, string associationName, string ruleName,
            ImportPatams importParams, int[] selectedRowsID)
        {
            InnerExportSelectedConvessionTable(stream, associationName, ruleName, importParams, selectedRowsID);
        }

        public void ImportData(Stream stream, string associationName, string ruleName)
        {
            InnerImportConversionTable(stream, associationName, ruleName);
        }


        public override bool CheckXml(Stream stream, string objectUniqueKey, ref ImportPatams importParams, ref DataTable varianceTable)
        {
            return InnerCheckXml(stream, objectUniqueKey, ObjectType.ConversionTable, ref varianceTable);
        }

        #endregion


        #region вспомогательные методы

        internal DataTable GetInformationTable()
        {
            DataTable checkProtocolTable = new DataTable();
            DataColumn column = checkProtocolTable.Columns.Add("ObjectSide", typeof(string));
            column.Caption = "В таблице перекодировок";
            column = checkProtocolTable.Columns.Add("XmlSide", typeof(string));
            column.Caption = "В XML";
            checkProtocolTable.Columns.Add("CriticalError", typeof(bool));
            return checkProtocolTable;
        }


        /// <summary>
        /// получение атрибутов перекодировки
        /// </summary>
        /// <param name="associationName"></param>
        /// <param name="ruleName"></param>
        private void GetConversionTableAttributes(string associationName, string ruleName)
        {
            conversionRule = ruleName;

            conversionTable = this._scheme.ConversionTables[String.Format("{0}.{1}", associationName, ruleName)];

            IAssociation association = (IAssociation)this._scheme.Associations[associationName];
            IAssociateRule rule = ((IBridgeAssociation)association).AssociateRules[ruleName];
            dataRoleAttributes = new List<IDataAttribute>();
            bridgeRoleAttributes = new List<IDataAttribute>();

            currentAssociation = (IAssociation)this._scheme.Associations[conversionTable.Name];

            // получение атрибутов сопоставляемого и сопоставимого

            foreach (IAssociateMapping map in rule.Mappings)
            {
                if (map.DataValue.IsSample)
                    dataRoleAttributes.Add(map.DataValue.Attribute);
                else
                    foreach (string sourceName in map.DataValue.SourceAttributes)
                    {
                        dataRoleAttributes.Add(association.RoleData.Attributes[sourceName]);
                    }
            }

            foreach (IAssociateMapping map in rule.Mappings)
            {
                if (map.BridgeValue.IsSample)
                    bridgeRoleAttributes.Add(map.BridgeValue.Attribute);
                else
                {
                    foreach (string sourceName in map.BridgeValue.SourceAttributes)
                    {
                        bridgeRoleAttributes.Add(association.RoleBridge.Attributes[sourceName]);
                    }
                }
            }

            // получаем семантики и названия классификаторов, которые учавствуют в сопоставлении
            objectName = new List<string>();
            objectName.Add(association.RoleData.Name);
            objectName.Add(association.RoleBridge.Name);
            rusObjectFullCaption = new List<string>();
            rusObjectFullCaption.Add(association.RoleData.FullCaption);
            rusObjectFullCaption.Add(association.RoleBridge.FullCaption);

            objectSemantic = new List<string>();
            objectSemantic.Add(association.RoleData.Semantic);
            objectSemantic.Add(association.RoleBridge.Semantic);
            rusObjectSemantic = new List<string>();
            rusObjectSemantic.Add(ExportImportClsObjectsHelper.GetDataObjSemanticRus(association.RoleData, _scheme));
            rusObjectSemantic.Add(ExportImportClsObjectsHelper.GetDataObjSemanticRus(association.RoleBridge, _scheme));

            curreantTableName = conversionTable.Name + "." + conversionTable.RuleName;
        }

        #endregion


        #region проверка XML

        private bool InnerCheckXml(Stream stream, string objectKey, ObjectType ojectType, ref DataTable varianceTable)
        {
            string[] conversionParams = objectKey.Split('.');
            string rule = conversionParams[conversionParams.Length - 1];
            string name = objectKey.Replace("." + rule, string.Empty);
            GetConversionTableAttributes(name, rule);
            return ReadMetadataFromStream(stream, ojectType, ref varianceTable);
        }

        private bool ReadMetadataFromStream(Stream stream, ObjectType objectType, ref DataTable informationTable)
        {
            // объект для чтения из XML
            XmlTextReader reader = new XmlTextReader(stream);
            // отношения между данными. Хранит соответствие данных из XML атрибутам серверного объекта
            List<AttributesRelation> attributesRelationList = new List<AttributesRelation>();
            // таблица для хранения несоответствия 
            informationTable = GetInformationTable();

            // объекты для хранения семантик и наименований, полученных из XML
            List<string> xmlObjectNames = new List<string>();
            List<string> xmlObjecSemantics = new List<string>();
            // для русских наименований
            List<string> xmlRusObjectNames = new List<string>();
            List<string> xmlRusObjecSemantics = new List<string>();

            string xmlObjectName = string.Empty;
            string xmlObjecSemantic = string.Empty;
            string xmlRusObjectName = string.Empty;
            string xmlRusObjecSemantic = string.Empty;

            // структуры для хранения атрибутов, полученных из XML
            // для хранения атрибутов классификаторов и таблиц фактов
            List<InnerAttribute> xmlObjectAttributes = null;
            // для хранения атрибутов таблиц перекодировок
            List<InnerAttribute> xmlDataRoleAttributes = null;
            List<InnerAttribute> xmlBridgeRoleAttributes = null;

            xmlDataRoleAttributes = new List<InnerAttribute>();
            xmlBridgeRoleAttributes = new List<InnerAttribute>();
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
                                    CheckNamesAndSemantics(xmlObjectNames, xmlRusObjectNames, xmlObjecSemantics, xmlRusObjecSemantics, ref informationTable);
                                    // сравниваем атрибуты объекта и из XML
                                    canSaveData = CheckConversionTableAttributes(xmlDataRoleAttributes, xmlBridgeRoleAttributes, ref informationTable, ref attributesRelationList);
                                    break;
                            }
                            break;
                        case XmlNodeType.Element:
                            switch (reader.LocalName)
                            {
                                case XmlConsts.tableMetadataNode:
                                    reader.MoveToAttribute(XmlConsts.exportObjectTypeAttribute);

                                    if (!GetMetaDataVariance(reader.Value, objectType, ref informationTable))
                                        return false;

                                    if (reader.MoveToAttribute(XmlConsts.conversionRuleAttribute))
                                    {
                                        string xmlRule = reader.Value;
                                        if (xmlRule.ToUpper() != conversionRule.ToUpper())
                                            informationTable.Rows.Add(string.Format("Правило перекодировки '{0}'", conversionRule), string.Format("Правило перекодировки '{0}'", xmlRule), false);
                                    }
                                    break;
                                case XmlConsts.attributeElement:
                                    // получаем данные по отдельному отрибуту схемы, записанному в XML
                                    InnerAttribute attr = GetXMLAttribute(reader);
                                    xmlObjectAttributes.Add(attr);
                                    break;
                                case XmlConsts.dataRoleNode:
                                    // для таблиц перекодировок получаем параметры сопоставляемого классификатора
                                    xmlObjectAttributes = xmlDataRoleAttributes;
                                    GetObjectMetaData(reader, ref xmlObjectName, ref xmlRusObjectName, ref xmlObjecSemantic, ref xmlRusObjecSemantic);
                                    xmlObjectNames.Add(xmlObjectName);
                                    xmlRusObjectNames.Add(xmlRusObjectName);
                                    xmlObjecSemantics.Add(xmlObjecSemantic);
                                    xmlRusObjecSemantics.Add(xmlRusObjecSemantic);
                                    break;
                                case XmlConsts.bridgeRoleNode:
                                    // для таблиц перекодировок получаем параметры сопостовимого классификатора
                                    xmlObjectAttributes = xmlBridgeRoleAttributes;
                                    GetObjectMetaData(reader, ref xmlObjectName, ref xmlRusObjectName, ref xmlObjecSemantic, ref xmlRusObjecSemantic);
                                    xmlObjectNames.Add(xmlObjectName);
                                    xmlRusObjectNames.Add(xmlRusObjectName);
                                    xmlObjecSemantics.Add(xmlObjecSemantic);
                                    xmlRusObjecSemantics.Add(xmlRusObjecSemantic);
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
            if (xmlObjectType != XmlConsts.conversionTable)
            {
                informationTable.Rows.Add("Объект 'Таблица перекодировки'", string.Format("Объект '{0}'",
                    ExportImportClsObjectsHelper.GetObjectType(xmlObjectType)), true);
                return false;
            }
            return true;
        }

        #endregion


        #region экспорт

        private void InnerExportConversionTable(Stream stream, string associationName,
            string ruleName, ImportPatams importParams)
        {
            // получаем атрибуты таблицы перекодировки
            GetConversionTableAttributes(associationName, ruleName);

            // получаем параметры импорта
            innerImportParams = importParams;
            SaveDataToStream(stream);
        }


        private void InnerExportSelectedConvessionTable(Stream stream, string associationName, string ruleName, ImportPatams importParams, int[] selectedRowsID)
        {
            string[] filterID = new string[selectedRowsID.Length];
            for (int i = 0; i <= selectedRowsID.Length - 1; i++)
            {
                filterID[i] = string.Format("(ID = {0})", selectedRowsID[i]);
            }
            filterConversionTable = string.Join(" or ", filterID);
            ExportData(stream, associationName, ruleName, importParams);
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
                    ExportImportXmlHelper.CreateXMLAttribute(xmlWriter, XmlConsts.rusNameAttribute, attr.Caption);
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
            DataRow[] rows = null;

            if (filterConversionTable != string.Empty)
                rows = dt.Select(filterConversionTable);
            else
                rows = dt.Select();

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
            // закрываем отдельную таблицу
            writer.WriteEndElement();
            // закрываем секцию с данными
            writer.WriteEndElement();
        }


        /// <summary>
        /// построение XML
        /// </summary>
        /// <param name="stream"></param>
        private void SaveDataToStream(Stream stream)
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

            // запись атрибутов объекта схемы
            writer.WriteStartElement(XmlConsts.metadataRootNode);
            writer.WriteStartElement(XmlConsts.tableMetadataNode);
            // атрибут 
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.exportObjectTypeAttribute, XmlConsts.conversionTable);
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.conversionRuleAttribute, conversionRule);

            // для таблиц перекодировок метаданных становится больше
            // запись атрибутов сопоставляемого классификатора
            writer.WriteStartElement(XmlConsts.dataRoleNode);
            CreateObjectAttributeAttributes(writer, objectName[0], rusObjectFullCaption[0], objectSemantic[0]);
            SaveSchemeAttributesToXML(dataRoleAttributes, writer);
            writer.WriteEndElement();
            // запись атрибутов сопоставимого классификатора
            writer.WriteStartElement(XmlConsts.bridgeRoleNode);
            CreateObjectAttributeAttributes(writer, objectName[1], rusObjectFullCaption[1], objectSemantic[1]);
            SaveSchemeAttributesToXML(bridgeRoleAttributes, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndElement();

            WriteConvertionTableData(writer, conversionTable);

            // закрываем корневой элемент и документ в целом
            writer.WriteEndElement();
            writer.WriteEndDocument();

            // закрывает поток и записывает все в файл
            writer.Flush();
            writer.Close();
        }

        #endregion


        #region импорт

        private void InnerImportConversionTable(Stream stream, string associationName, string ruleName)
        {
            IDatabase db = null;
            IClassifiersProtocol protocol = null;
            try
            {
                db = this._scheme.SchemeDWH.DB;
                protocol = (IClassifiersProtocol)this._scheme.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);

                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceImportClassifierData, curreantTableName,
                    -1, -1, 4, "Начало операции импорта");

                GetConversionTableAttributes(associationName, ruleName);
                ReadFromStream(stream, db);

                string protocolMessage = string.Format("Результаты операции импорта: добавлено записей: {0}; удалено записей: {1}",
                    insertRowsCount, deleteRowsCount);

                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation, curreantTableName,
                    -1, -1, 4, protocolMessage);

                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceSuccefullFinished, curreantTableName,
                    -1, -1, 4, "Операции импорта завершилась успешно");

            }
            catch (Exception exception)
            {
                string message = exception.Message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceError, curreantTableName,
                    -1, -1, 4, message);
                throw new Exception(exception.Message);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }


        private void CheckNamesAndSemantics(List<string> xmlNames, List<string> xmlRusNames, List<string> xmlSemantics,
            List<string> xmlRusSemantics, ref DataTable pritocolTable)
        {
            IAssociation association = currentAssociation;
            // если это таблица перекодировок, то сравним семантики сопоставимого и сопоставляемого и правило с тем, что записано в  XML
            if (xmlNames[0] != objectName[0])
            {
                pritocolTable.Rows.Add(string.Format("Наименование сопоставляемого '{0}' ('{1}')", association.RoleData.Caption, objectName[0]),
                    string.Format("Наименование '{0}' ('{1}')", xmlRusNames[0], xmlNames[0]), false);
            }
            if (xmlSemantics[0] != objectSemantic[0])
            {
                pritocolTable.Rows.Add(string.Format("Семантика сопоставляемого '{0}' ('{1}')",
                    ExportImportClsObjectsHelper.GetDataObjSemanticRus(association.RoleData, _scheme), objectSemantic[0]),
                    string.Format("Наименование '{0}' ('{1}')", xmlRusSemantics[0], xmlSemantics[0]), false);
            }
            if (xmlNames[1] != objectName[1])
            {
                pritocolTable.Rows.Add(string.Format("Наименование сопоставляемого '{0}' ('{1}')", association.RoleBridge.Caption, objectName[1]),
                    string.Format("Наименование '{0}' ('{1}')", xmlRusNames[1], xmlSemantics[1]), false);
            }
            if (xmlSemantics[1] != objectSemantic[1])
            {
                pritocolTable.Rows.Add(string.Format("Семантика сопоставляемого '{0}' ('{1}')",
                    ExportImportClsObjectsHelper.GetDataObjSemanticRus(association.RoleBridge, _scheme), objectSemantic[1]),
                    string.Format("Наименование '{0}' ('{1}')", xmlRusSemantics[1], xmlSemantics[1]), false);
            }
        }

        /// <summary>
        /// сравниваем атрибуты у таблицы перекодировок
        /// </summary>
        /// <param name="xmlDataAttributes"></param>
        /// <param name="xmlBridgeAttributes"></param>
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
                attributeRelation.DBType = ExportImportDBHelper.GetDBTypeFromAttribute(attributesList[i]);
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
        /// предварительное удаление данных, если предусмотрено параметрами 
        /// </summary>
        /// <param name="objecType"></param>
        /// <param name="db"></param>
        /// <param name="protocol"></param>
        /// <param name="conversionDataTable"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        private void DeleteData(DataTable conversionDataTable)
        {
            if (innerImportParams.deleteDataBeforeImport)
            {
                foreach (DataRow row in conversionDataTable.Rows)
                {
                    conversionTable.DeleteRow(Convert.ToInt32(row["ID"]));
                    deleteRowsCount++;
                }
                conversionDataTable.Clear();
            }
        }


        private void ReadFromStream(Stream stream, IDatabase db)
        {
            // объект для чтения из XML
            XmlTextReader reader = new XmlTextReader(stream);
            // структура для хранения одной записи, получаемой из XML
            List<object> rowCellsValues = new List<object>();
            // отношения между данными. Хранит соответствие данных из XML атрибутам серверного объекта
            List<AttributesRelation> attributesRelationList = new List<AttributesRelation>();
            // таблица для хранения несоответствия 
            DataTable checkProtocolTable = GetInformationTable();
            // объект для хранения данных таблиц перекодировок
            DataTable conversionDataTable = null;

            int importedRowsCount = 0;
            // упдатер для получения и сохранения данных перекодировок
            IDataUpdater duConversionTable = null;
            // объекты для хранения семантик и наименований, полученных из XML
            List<string> xmlObjectNames = new List<string>();
            List<string> xmlObjecSemantics = new List<string>();
            // для русских наименований
            List<string> xmlRusObjectNames = new List<string>();
            List<string> xmlRusObjecSemantics = new List<string>();

            string xmlObjectName = string.Empty;
            string xmlObjecSemantic = string.Empty;
            string xmlRusObjectName = string.Empty;
            string xmlRusObjecSemantic = string.Empty;

            // структуры для хранения атрибутов, полученных из XML
            // для хранения атрибутов таблиц перекодировок
            List<InnerAttribute> xmlObjectAttributes = null;
            List<InnerAttribute> xmlDataRoleAttributes = new List<InnerAttribute>();
            List<InnerAttribute> xmlBridgeRoleAttributes = new List<InnerAttribute>();
            
            innerImportParams = new ImportPatams();
            // для таблиц перекодировок содаем свою таблицу для хранения данных

            conversionDataTable = new DataTable();
            duConversionTable = conversionTable.GetDataUpdater();
            duConversionTable.Fill(ref conversionDataTable);

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
                                CheckNamesAndSemantics(xmlObjectNames, xmlRusObjectNames, xmlObjecSemantics, xmlRusObjecSemantics, ref checkProtocolTable);
                                // сравниваем атрибуты объекта и из XML
                                bool canSaveData = true;
                                canSaveData = CheckConversionTableAttributes(xmlDataRoleAttributes, xmlBridgeRoleAttributes, ref checkProtocolTable, ref attributesRelationList);
                                if (!canSaveData)
                                    // сохранить данные не получится. чистим все коллекции и выходим
                                    return;
                                break;
                            case XmlConsts.rowNode:
                                importedRowsCount++;
                                insertRowsCount++;
                                // сохранение данных... По одной записи
                                SaveRow(conversionDataTable, duConversionTable, attributesRelationList, rowCellsValues);
                                break;
                        }
                        break;
                    case XmlNodeType.Element:
                        switch (reader.LocalName)
                        {
                            case XmlConsts.settingsElement:
                                // читаем параметры импорта из XML
                                GetImportSettings(reader, ref innerImportParams);
                                DeleteData(conversionDataTable);
                                break;
                            case XmlConsts.attributeElement:
                                // получаем данные по отдельному отрибуту схемы, записанному в XML
                                InnerAttribute attr = GetXMLAttribute(reader);
                                xmlObjectAttributes.Add(attr);
                                break;
                            case XmlConsts.dataRoleNode:
                                // для таблиц перекодировок получаем параметры сопоставляемого классификатора
                                xmlObjectAttributes = xmlDataRoleAttributes;
                                GetObjectMetaData(reader, ref xmlObjectName, ref xmlRusObjectName, ref xmlObjecSemantic, ref xmlRusObjecSemantic);
                                xmlObjectNames.Add(xmlObjectName);
                                xmlRusObjectNames.Add(xmlRusObjectName);
                                xmlObjecSemantics.Add(xmlObjecSemantic);
                                xmlRusObjecSemantics.Add(xmlRusObjecSemantic);
                                break;
                            case XmlConsts.bridgeRoleNode:
                                // для таблиц перекодировок получаем параметры сопостовимого классификатора
                                xmlObjectAttributes = xmlBridgeRoleAttributes;
                                GetObjectMetaData(reader, ref xmlObjectName, ref xmlRusObjectName, ref xmlObjecSemantic, ref xmlRusObjecSemantic);
                                xmlObjectNames.Add(xmlObjectName);
                                xmlRusObjectNames.Add(xmlRusObjectName);
                                xmlObjecSemantics.Add(xmlObjecSemantic);
                                xmlRusObjecSemantics.Add(xmlRusObjecSemantic);
                                break;
                            case XmlConsts.rowNode:
                                // производим подготовления к получению записи из XML
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

        #endregion
    }
}
