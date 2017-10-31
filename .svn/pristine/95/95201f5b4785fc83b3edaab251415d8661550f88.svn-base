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
    public abstract partial class ExportImporterBase : DisposableObject
    {
        /// <summary>
        /// по атрибутам схемы создаем таблицу
        /// </summary>
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


        internal static DataAttributeTypes GetAttributeType(string p)
        {
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

        }


        /// <summary>
        /// из типа атрибута 
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
                case DataAttributeTypes.dtUnknown:
                    return DbType.Object;
            }
            return DbType.Object;
        }

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

    }
}
