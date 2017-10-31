using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Text;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FO99Pump
{
    public class FO99XmlExporter
    {

        #region класс, описывающий экспортируемые объекты

        public sealed class ExportedObject
        {
            public IEntity entity;
            public FmObjectsTypes entityType;
            public IDataSource dataSource = null;
            public string queryConstraint = string.Empty;
            public string deleteConstraint = string.Empty;

            public ExportedObject(IEntity entity, FmObjectsTypes entityType, IDataSource dataSource, string queryConstraint, string deleteConstraint)
            {
                this.entity = entity;
                this.entityType = entityType;
                this.dataSource = dataSource;
                this.queryConstraint = queryConstraint;
                this.deleteConstraint = deleteConstraint;
            }

        }

        #endregion класс, описывающий экспортируемые объекты

        #region формирование хмл

        private void AddEntityNode(XmlNode usedEntityNode, ExportedObject exportedObject)
        {
            IEntity entity = exportedObject.entity;
            IDataUpdater du = entity.GetDataUpdater(exportedObject.queryConstraint, null, null);
            DataTable dt = new DataTable();
            du.Fill(ref dt);
            try
            {
                XmlNode entityNode = XmlHelper.AddChildNode(usedEntityNode, XmlConsts.fmObjectTag,
                                                            new string[] { XmlConsts.fmObjectKey, entity.ObjectKey },
                                                            new string[] { XmlConsts.fmObjectTypeAttr, exportedObject.entityType.ToString() },
                                                            new string[] { XmlConsts.fmObjectFullDbName, entity.FullDBName },
                                                            new string[] { XmlConsts.fmObjectDeleteConstraint, exportedObject.deleteConstraint });
                string[] attrNames = GetAttrName(entity);
                foreach (DataRow row in dt.Rows)
                {
                    XmlNode entityRowNode = XmlHelper.AddChildNode(entityNode, XmlConsts.fmObjectRowTag);
                    foreach (string attrName in attrNames)
                        XmlHelper.SetAttribute(entityRowNode, attrName, row[attrName].ToString());
                }
            }
            finally
            {
                dt.Clear();
            }
        }

        private string[] GetAttrName(IEntity entity)
        {
            string[] attrNames = new string[] { "Id" };
            // берем только пользовательские атрибуты
            IDataAttributeCollection dac = entity.Attributes;
            foreach (KeyValuePair<string, IDataAttribute> da in dac)
            {
                if (da.Value.Class != DataAttributeClassTypes.Typed)
                    continue;
                attrNames = (string[])CommonRoutines.ConcatArrays(attrNames, new string[] { da.Value.Name });
            }
            // + ссылки (сопоставимые не берем)
            IAssociationCollection assCol = (IAssociationCollection)(entity).Associations;
            foreach (KeyValuePair<string, IEntityAssociation> ea in assCol)
            {
                if (ea.Value.AssociationClassType != AssociationClassTypes.Link)
                    continue;
                attrNames = (string[])CommonRoutines.ConcatArrays(attrNames, new string[] { ea.Value.FullDBName });
            }
            return attrNames;
        }

        private void ExportDataSource(XmlNode dsNode, IDataSource ds)
        {
            XmlHelper.SetAttribute(dsNode, XmlConsts.supplierCodeAttr, ds.SupplierCode);
            XmlHelper.SetAttribute(dsNode, XmlConsts.dataCodeAttr, ds.DataCode);
            XmlHelper.SetAttribute(dsNode, XmlConsts.kindsOfParamsAttr, Convert.ToInt32(ds.ParametersType).ToString());
            XmlHelper.SetAttribute(dsNode, XmlConsts.yearAttr, ds.Year.ToString());
            XmlHelper.SetAttribute(dsNode, XmlConsts.monthAttr, ds.Month.ToString());
            XmlHelper.SetAttribute(dsNode, XmlConsts.quarterAttr, ds.Quarter.ToString());
            XmlHelper.SetAttribute(dsNode, XmlConsts.variantAttr, ds.Variant);
        }

        public string ExportDataToXml(ExportedObject[] exportedObjects, IDataSource commonDataSource)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                XmlProcessingInstruction pi = xmlDocument.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"Windows-1251\"");
                xmlDocument.AppendChild(pi);
                XmlElement rootNode = xmlDocument.CreateElement(XmlConsts.rootTag);
                xmlDocument.AppendChild(rootNode);

                XmlNode dsNode = XmlHelper.AddChildNode(rootNode, XmlConsts.commonDataSourceNodeTag);
                ExportDataSource(dsNode, commonDataSource);

                XmlNode entitiesNode = XmlHelper.AddChildNode(rootNode, XmlConsts.fmObjectsTag);

                foreach (ExportedObject exportedObject in exportedObjects)
                    AddEntityNode(entitiesNode, exportedObject);

                return xmlDocument.InnerXml;
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref xmlDocument);
            }
        }

        #endregion формирование хмл

    }

}
