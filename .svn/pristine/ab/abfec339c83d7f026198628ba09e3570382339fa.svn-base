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
    public class ExportImportManager : DisposableObject, IExportImportManager
    {
        IScheme _scheme;
        ExportImporterBase exportImporter;

        public ExportImportManager(IScheme scheme)
        {
            if (scheme == null)
                throw new Exception("Не задан интерфейс схемы");
            _scheme = scheme;
        }

        public IExportImporter GetExportImporter(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Classifier:
                    exportImporter = new ClassifiersExportImport(_scheme, this);
                    break;
                case ObjectType.ConversionTable:
                    exportImporter = new ConversionTablesExportImport(_scheme, this);
                    break;
                case ObjectType.FactTable:
                    exportImporter = new FactTablesExportImport(_scheme, this);
                    break;
            }

            return exportImporter;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // освобождаем управляемые ресурсы
                if (exportImporter != null)
                    exportImporter.Dispose();
            }
            // освобождаем неуправляемые ресурсы
            // ...
        }
    }

    public abstract partial class ExportImporterBase : DisposableObject, IExportImporter
    {
		protected IScheme _scheme;
		protected ImportPatams innerImportParams;
		protected ExportImportManager _exportImportManager;
		protected bool exportSelectedRows;

		protected int insertRowsCount = 0;
		protected int updateRowsCount = 0;
		protected int deleteRowsCount = 0;

        public ExportImporterBase(IScheme scheme, ExportImportManager exportImportManager)
        {
            _scheme = scheme;
            _exportImportManager = exportImportManager;
        }

        #region реализация интерфейса

        public virtual bool CheckXml(Stream stream, string objectKey, ref ImportPatams importParams, ref DataTable varianceTable)
        {
            return true;
        }

        public virtual void ExportData(Stream stream, ImportPatams importParams, ExportImportClsParams exportImportClsParams)
        {
            
        }

        public virtual Dictionary<int, int> ImportData(Stream stream, ExportImportClsParams exportImportClsParams)
        {
            return null;
        }

        public virtual void ExportMasterDetail(Stream masterStream, Dictionary<string, Stream> detailStreams,
            ExportImportClsParams exportImportClsParams, ImportPatams importParams)
        {
            
        }

        public virtual Dictionary<string, Dictionary<int, int>> ImportMasterDetail(Stream masterStream, Dictionary<string, Stream> detailStreams,
            ExportImportClsParams exportImportClsParams)
        {
            return null;
        }

        #endregion

        #region самые общие методы

        /// <summary>
        /// сохранение параметров импорта
        /// </summary>
        /// <param name="writer"></param>
        internal void SaveImportParams(XmlWriter writer)
        {
            writer.WriteStartElement(XmlConsts.settingsElement);
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.useOldIDAttribute, innerImportParams.useOldID.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.deleteDataBeforeImportAttribute, innerImportParams.deleteDataBeforeImport.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.deleteDevelopData, innerImportParams.deleteDeveloperData.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.useInnerUniqueAttributesAttribute, innerImportParams.refreshDataByAttributes.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.useSchemeUniqueAttributesAttribute, innerImportParams.refreshDataByUnique.ToString());
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.restoreDataSourceAttribute, innerImportParams.restoreDataSource.ToString());
            writer.WriteEndElement();
        }


        /// <summary>
        /// сохранение наименования и семантики объекта в виде атрибутов XML
        /// </summary>
        internal void CreateObjectAttributeAttributes(XmlWriter writer, string objectName,
            string fullCaption, string objectSemantic)
        {
            // сюда же пишем русские наименования семантики и наименования объекта
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.nameAttribute, objectName);
			ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.rusNameAttribute, fullCaption.Split(new char[] { '.' })[1]);
            ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.semanticAttribute, objectSemantic);
			ExportImportXmlHelper.CreateXMLAttribute(writer, XmlConsts.rusSemanticAttribute, fullCaption.Split(new char[] { '.' })[0]);
        }


        /// <summary>
        /// получение атрибута объекта схемы из XML
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal InnerAttribute GetXMLAttribute(XmlTextReader reader)
        {
            InnerAttribute attr = new InnerAttribute();
            reader.MoveToAttribute(XmlConsts.nameAttribute);
            attr.name = reader.Value;
            reader.MoveToAttribute(XmlConsts.sizeAttribute);
            attr.size = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XmlConsts.typeAttribute);
            attr.attributeType = ExportImportDBHelper.GetAttributeType(reader.Value);
            reader.MoveToAttribute(XmlConsts.nullableAttribute);
            attr.nullable = Convert.ToBoolean(reader.Value);
            if (reader.MoveToAttribute(XmlConsts.defaultValueAttribute))
                attr.defaultValue = (object)reader.Value;
            else
                attr.defaultValue = null;

            return attr;
        }


        /// <summary>
        /// получение параметров импорта из XML
        /// </summary>
        /// <param name="reader"></param>
        internal void GetImportSettings(XmlTextReader reader, ref ImportPatams importParams)
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


        /// <summary>
        /// читает из XML метаданные по объекту
        /// </summary>
        /// <param name="reader">объект чтения их XML</param>
        /// <param name="xmlObjectName">наименование объекта</param>
        /// <param name="xmlRusObjectName">русское наименование</param>
        /// <param name="xmlObjecSemantic">семантика объекта</param>
        /// <param name="xmlRusObjecSemantic">русское обозначение семантики</param>
        internal void GetObjectMetaData(XmlTextReader reader, ref string xmlObjectName,
            ref string xmlRusObjectName, ref string xmlObjecSemantic, ref string xmlRusObjecSemantic)
        {
            reader.MoveToAttribute(XmlConsts.nameAttribute);
            xmlObjectName = reader.Value;

            if (reader.MoveToAttribute(XmlConsts.rusNameAttribute))
                xmlRusObjectName = reader.Value;
            else
                xmlRusObjectName = string.Empty;

            reader.MoveToAttribute(XmlConsts.semanticAttribute);
            xmlObjecSemantic = reader.Value;

            if (reader.MoveToAttribute(XmlConsts.rusSemanticAttribute))
                xmlRusObjecSemantic = reader.Value;
            else
                xmlRusObjecSemantic = string.Empty;
        }

        #endregion
    }
}
