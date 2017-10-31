using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter
{
    public class XMLDataSource
    {
        internal string suppplierCode = string.Empty;
        internal string dataName = string.Empty;
        internal string dataCode = string.Empty;
        internal string kindsOfParams = string.Empty;
        internal string name = string.Empty;
        internal string year = string.Empty;
        internal string month = string.Empty;
        internal string variant = string.Empty;
        internal string quarter = string.Empty;
        internal string territory = string.Empty;
        internal string dataSourceName = string.Empty;
    }

    public class AttributesRelation
    {
        internal int objectAttributeIndex;
        internal int xmlAttributeIndex;
        internal string attrName;
        internal DbType DBType;
    }

    public class InnerAttribute
    {
        public string name;
        public int size;
        public bool nullable;
        public object defaultValue;
        public DataAttributeTypes attributeType;
    }

    internal class ExportImportDBHelper
    {
        /// <summary>
        /// создание пустой таблицы по атрибутам
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        internal static DataTable CreateDataTableFromAttributes(IDataAttribute[] attributes)
        {
            DataTable dt = new DataTable();
            foreach (IDataAttribute attr in attributes)
            {
                switch (attr.Type)
                {
                    case DataAttributeTypes.dtBoolean:
                        dt.Columns.Add(attr.Name, typeof(Boolean));
                        break;
                    case DataAttributeTypes.dtChar:
                        dt.Columns.Add(attr.Name, typeof(Char));
                        break;
                    case DataAttributeTypes.dtDate:
                        dt.Columns.Add(attr.Name, typeof(DateTime));
                        break;
                    case DataAttributeTypes.dtDateTime:
                        dt.Columns.Add(attr.Name, typeof(DateTime));
                        break;
                    case DataAttributeTypes.dtDouble:
                        dt.Columns.Add(attr.Name, typeof(Double));
                        break;
                    case DataAttributeTypes.dtInteger:
                        if (attr.Size >= 10)
                            dt.Columns.Add(attr.Name, typeof(Int64));
                        else
                            dt.Columns.Add(attr.Name, typeof(Int32));
                        break;
                    case DataAttributeTypes.dtString:
                        dt.Columns.Add(attr.Name, typeof(String));
                        break;
                    case DataAttributeTypes.dtBLOB:
                        dt.Columns.Add(attr.Name, typeof(Byte[]));
                        break;
                    case DataAttributeTypes.dtUnknown:
                        dt.Columns.Add(attr.Name, typeof(Object));
                        break;
                }
            }
            return dt;
        }

        /// <summary>
        /// получение атрибута по его названию из XML
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static DataAttributeTypes GetAttributeType(string p)
        {
            return (DataAttributeTypes)Enum.Parse(typeof(DataAttributeTypes), p);
            /*
            if (DataAttributeTypes.dtBLOB.ToString() == p)
                return DataAttributeTypes.dtBLOB;
            if (DataAttributeTypes.dtBoolean.ToString() == p)
                return DataAttributeTypes.dtBoolean;
            if (DataAttributeTypes.dtChar.ToString() == p)
                return DataAttributeTypes.dtChar;
            if (DataAttributeTypes.dtDate.ToString() == p)
                return DataAttributeTypes.dtDate;
            if (DataAttributeTypes.dtDateTime.ToString() == p)
                return DataAttributeTypes.dtDateTime;
            if (DataAttributeTypes.dtDouble.ToString() == p)
                return DataAttributeTypes.dtDouble;
            if (DataAttributeTypes.dtInteger.ToString() == p)
                return DataAttributeTypes.dtInteger;
            if (DataAttributeTypes.dtString.ToString() == p)
                return DataAttributeTypes.dtString;

            return DataAttributeTypes.dtUnknown;
             * */

        }

        /// <summary>
        /// получение типа DB атрибута
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        internal static DbType GetDBTypeFromAttribute(IDataAttribute attr)
        {
            switch (attr.Type)
            {
                case DataAttributeTypes.dtBoolean:
                    return DbType.Boolean;
                case DataAttributeTypes.dtChar:
                    return DbType.Byte;
                case DataAttributeTypes.dtDate:
                    return DbType.Date;
                case DataAttributeTypes.dtDateTime:
                    return DbType.DateTime;
                case DataAttributeTypes.dtDouble:
                    return DbType.Double;
                case DataAttributeTypes.dtInteger:
                    if (attr.Size >= 10)
                        return DbType.Int64;
                    return DbType.Int32;
                case DataAttributeTypes.dtString:
                    return DbType.AnsiString;
                case DataAttributeTypes.dtBLOB:
                    return DbType.Binary;
                case DataAttributeTypes.dtUnknown:
                    return DbType.Object;
            }
            return DbType.Object;
        }


    }

    internal class ExportImportXmlHelper
    {
        /// <summary>
        /// создает и записываем XML атрибут
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        internal static void CreateXMLAttribute(XmlWriter writer, string attributeName, string attributeValue)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteString(attributeValue);
            writer.WriteEndAttribute();
        }
    }

    internal class ExportImportClsObjectsHelper
    {
        internal static ObjectType GetExportImportObjectType(ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.clsFixedClassifier:
                case ClassTypes.clsDataClassifier:
                case ClassTypes.clsBridgeClassifier:
                    return ObjectType.Classifier;
                case ClassTypes.clsFactData:
                    return ObjectType.FactTable;
            }
            return ObjectType.ConversionTable;
        }

        internal static string GetDataObjSemanticRus(IEntity cmnObj, IScheme scheme)
        {
            string semanticText;
            if (scheme.Semantics.TryGetValue(cmnObj.Semantic, out semanticText))
                return semanticText;
            return cmnObj.Semantic;
        }

        internal static string GetBridgeClsCaptionByRefName(IEntity activeDataObject, string refName)
        {
            IAssociationCollection assCol = (IAssociationCollection)(activeDataObject).Associations;
            string fullName = String.Concat(activeDataObject.Name, ".", refName);
            IEntity bridgeCls = null;
            foreach (IEntityAssociation item in assCol.Values)
            {
                if (String.Compare(item.Name, fullName, true) == 0)
                {
                    bridgeCls = item.RoleBridge;
                    break;
                }
            }
            return bridgeCls == null ? string.Empty : bridgeCls.Caption;
        }

        internal static string GetObjectType(ObjectType objecType, ClassTypes classType, bool toObject)
        {
            switch (objecType)
            {
                case ObjectType.Classifier:
                    switch (classType)
                    {
                        case ClassTypes.clsBridgeClassifier:
                            if (toObject)
                                return "сопоставимого классификатора";
                            else
                                return "сопоставимом классификаторе";
                        case ClassTypes.clsDataClassifier:
                            if (toObject)
                                return "классификатора данных";
                            else
                                return "классификаторе данных";
                    }
                    break;
                case ObjectType.FactTable:
                    if (toObject)
                        return "таблицы фактов";
                    else
                        return "таблице фактов";
                case ObjectType.ConversionTable:
                    if (toObject)
                        return "таблицы перекодировок";
                    else
                        return "таблице перекодировок";
            }
            return string.Empty;
        }

        internal static string GetObjectType(ObjectType objecType, ClassTypes classType)
        {
            switch (objecType)
            {
                case ObjectType.Classifier:
                    switch (classType)
                    {
                        case ClassTypes.clsBridgeClassifier:
                            return "Сопоставимый классификатор";
                        case ClassTypes.clsDataClassifier:
                            return "Классификатор данных";
                        case ClassTypes.clsFixedClassifier:
                            return "Фиксированый классификатор";
                    }
                    break;
                case ObjectType.FactTable:
                    return "Таблица фактов";
                case ObjectType.ConversionTable:
                    return "Таблица перекодировки";
            }
            return string.Empty;
        }

        internal static string GetObjectType(string xmlObjectType)
        {
            switch (xmlObjectType)
            {
                case XmlConsts.factTable:
                    return "Таблица фактов";
                case XmlConsts.fixedClassifier:
                    return "Фиксированый классификатор";
                case XmlConsts.dataClassifier:
                    return "Классификатор данных";
                case XmlConsts.bridgeClassifier:
                    return "Сопоставимый классификатор";
                case XmlConsts.conversionTable:
                    return "Таблица перекодировки";
            }
            return string.Empty;
        }

        /// <summary>
        /// получение списка деталей
        /// </summary>
        /// <param name="masterObject"></param>
        /// <returns></returns>
        internal static List<IEntity> GetDetailList(IEntity masterObject)
        {
            List<IEntity> detailsList = new List<IEntity>();
            foreach (IEntityAssociation association in masterObject.Associated.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.MasterDetail)
                {
                    detailsList.Add(association.RoleData);
                }
            }
            return detailsList;
        }

        /// <summary>
        /// получение наименования ссылки детали на мастер
        /// </summary>
        /// <param name="masterObject"></param>
        /// <returns></returns>
		internal static string GetMasterDetailAssociationName(IEntity masterObject, IEntity detail)
        {
            foreach (IEntityAssociation association in masterObject.Associated.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.MasterDetail)
                {
					if (association.RoleData.ObjectKey == detail.ObjectKey)
						return association.RoleDataAttribute.Name;
                }
            }
            return string.Empty;
        }

        internal static string GetMasterDetailRefName(IEntity masterObject)
        {
            foreach (IEntityAssociation association in masterObject.Associated.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.MasterDetail)
                {
                    return association.FullName;
                }
            }
            return string.Empty;
        }
    }

    internal class ExportImportDataSourcesHelper
    {
        /// <summary>
        /// проверяет, существует ли источник данных из XML
        /// </summary>
        /// <returns>true, если источник существует</returns>
        internal int AddDataSource(XMLDataSource loadedDataSource, IScheme scheme)
        {
            IDataSourceManager sourceManager = scheme.DataSourceManager;

            IDataSource ds = sourceManager.DataSources.CreateElement();
            ds.BudgetName = loadedDataSource.name;
            ds.DataCode = loadedDataSource.dataCode;
            ds.DataName = loadedDataSource.dataName;
            ds.Month = Convert.ToInt32(loadedDataSource.month);
            ds.ParametersType = GetParamsType(loadedDataSource.kindsOfParams);
            ds.Quarter = Convert.ToInt32(loadedDataSource.quarter);
            ds.SupplierCode = loadedDataSource.suppplierCode;
            ds.Territory = loadedDataSource.territory;
            ds.Variant = loadedDataSource.variant;
            ds.Year = Convert.ToInt32(loadedDataSource.year);
            if (!sourceManager.DataSources.Contains(ds))
            {
                // если источника нету, то добавляем его
                return sourceManager.DataSources.Add(ds);
            }
            else
            {
                // если есть, то палучаем его ID
                return Convert.ToInt32(sourceManager.DataSources.FindDataSource(ds));
            }
        }

        /// <summary>
        /// получение из строки типа параметров источника
        /// </summary>
        /// <param name="kindsOfParams"></param>
        /// <returns></returns>
        private static ParamKindTypes GetParamsType(string kindsOfParams)
        {
            switch (kindsOfParams)
            {
                case "YearVariant":
                    return ParamKindTypes.YearVariant;
                case "YearTerritory":
                    return ParamKindTypes.YearTerritory;
                case "YearQuarterMonth":
                    return ParamKindTypes.YearQuarterMonth;
                case "YearQuarter":
                    return ParamKindTypes.YearQuarter;
                case "YearMonthVariant":
                    return ParamKindTypes.YearMonthVariant;
                case "YearMonth":
                    return ParamKindTypes.YearMonth;
                case "Year":
                    return ParamKindTypes.Year;
                case "WithoutParams":
                    return ParamKindTypes.WithoutParams;
                case "Budget":
                    return ParamKindTypes.Budget;
                case "Variant":
                    return ParamKindTypes.Variant;
                case "YearVariantMonthTerritory":
                    return ParamKindTypes.YearVariantMonthTerritory;
            }
            return ParamKindTypes.NoDivide;
        }
    }
}
