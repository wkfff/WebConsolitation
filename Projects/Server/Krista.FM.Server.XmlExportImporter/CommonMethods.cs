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
        /// <summary>
        /// по атрибутам схемы создаем таблицу
        /// </summary>
        /// <returns></returns>
        private static DataTable CreateDataTableFromAttributes(IDataAttribute[] attributes)
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
                    case DataAttributeTypes.dtUnknown:
                        dt.Columns.Add(attr.Name, typeof(Object));
                        break;
                }
            }
            return dt;
        }


        /// <summary>
        /// из типа атрибута 
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        private static DbType GetDBTypeFromAttribute(IDataAttribute attr)
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
        private static void CreateXMLAttribute(XmlWriter writer, string attributeName, string attributeValue)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteString(attributeValue);
            writer.WriteEndAttribute();
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

        #region Работа с иерархией и получением ID

        /// <summary>
        /// сохранение иерархических данных
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="db"></param>
        /// <param name="attributesRelationList"></param>
        private void SaveHierarchyData(DataSet dataSet, IDatabase db, List<AttributesRelation> attributesRelationList)
        {
            DataRow[] rows = dataSet.Tables[0].Select(string.Empty, String.Format("{0} ASC", refParentColumnName));
            foreach (DataRow row in rows)
            {
                SaveSingleDataRow(row, db, attributesRelationList);
            }
        }

        /// <summary>
        /// сохранение одной записи
        /// </summary>
        /// <param name="row"></param>
        /// <param name="db"></param>
        /// <param name="attributesRelationList"></param>
        private void SaveSingleDataRow(DataRow row, IDatabase db, List<AttributesRelation> attributesRelationList)
        {
            Dictionary<string, IDbDataParameter> dbParams = new Dictionary<string, IDbDataParameter>();
            IDbDataParameter[] paramsList = new IDbDataParameter[attributesRelationList.Count];
            string[] fields = new string[attributesRelationList.Count];
            GetRowParams(attributesRelationList, row, db, ref fields, ref dbParams);
            InsertOrUpdateRow(db, fields, dbParams);
        }


        /// <summary>
        /// получение необходимых объекттов для записи одной записи через запрос
        /// </summary>
        /// <param name="attributesRelationList"></param>
        /// <param name="row"></param>
        /// <param name="db"></param>
        /// <param name="paramsList"></param>
        /// <param name="fields"></param>
        private void GetRowParams(List<AttributesRelation> attributesRelationList, DataRow row, IDatabase db,
            ref string[] fields, ref Dictionary<string, IDbDataParameter> dbParams)
        {
            int columnCount = row.Table.Columns.Count;
            fields = new string[columnCount];
            for (int i = 0; i <= columnCount - 1; i++)
            {
                string columnName = row.Table.Columns[i].ColumnName;
                fields[i] = row.Table.Columns[i].ColumnName;
                dbParams.Add(columnName, db.CreateParameter(columnName, row[i],
                    GetDBTypeFromAttribute(classifierAttributes[i])));
            }

        }


        /// <summary>
        /// получение нормального значения генератора, которое не входит в уже загруженные данные
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="db"></param>
        /// <param name="generatorName"></param>
        /// <returns></returns>
        private void GetNormalGeneratorValue(DataTable Table, IDatabase db, string generatorName)
        {
            DataRow[] rows = Table.Select(string.Empty, "ID ASC");

            int curentID = db.GetGenerator(generatorName);
            // ID, которое сгенерится для последней записи в таблице
            int normalID = curentID + Table.Rows.Count - 1;
            // если ID в таблице все таки есть, то от него получаем первое ID 
            int firstID = Convert.ToInt32(lastId) - Table.Rows.Count;
            // если не попадают в диапазон, то выходим 

            if (!SessionContext.IsDeveloper)
                if (lastDeveloperId != 0)
                {
                    string developGeneratorName = GetGeneratorName((IEntity)schemeObject, true);
                    int currendDevelopId = db.GetGenerator(developGeneratorName);
                    int normalDevelopId = currendDevelopId + Table.Rows.Count - 1;
                    int firstDevelopId = Convert.ToInt32(lastDeveloperId) - Table.Rows.Count;

                    if (normalDevelopId < firstDevelopId)
                        while (currendDevelopId <= Convert.ToInt32(lastDeveloperId))
                        {
                            currendDevelopId = db.GetGenerator(developGeneratorName);
                        }
                }

            if (normalID < firstID)
                while (curentID <= Convert.ToInt32(lastId))
                {
                    curentID = db.GetGenerator(generatorName);
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
            GetNormalGeneratorValue(ds.Tables[0], db, generatorName);
            DataRelation rel = ds.Relations[0];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row.GetParentRow(rel) == null)
                {
                    int curentID = GetNewId(row, generatorName, ((IEntity)schemeObject), db);//db.GetGenerator(generatorName);
                    RestoreHierarchyIDRecursion(row.GetChildRows(rel), ds.Relations[0], generatorName, db, curentID);
                    row["ID"] = curentID;
                }
            }
        }

        private void RestoreHierarchyIDRecursion(DataRow[] rows, DataRelation Rel, string generatorName, IDatabase db, int ParentID)
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
                    int CurentID = GetNewId(row, generatorName, ((IEntity)schemeObject), db);//db.GetGenerator(generatorName);
                    RestoreHierarchyIDRecursion(row.GetChildRows(Rel), Rel, generatorName, db, CurentID);
                    row["ID"] = CurentID;
                }
            }
        }


        private DataTable GetInformationTable(ObjectType objectType)
        {
            DataTable checkProtocolTable = new DataTable();
            DataColumn column = checkProtocolTable.Columns.Add("ObjectSide", typeof(string));
            column.Caption = string.Format("В {0}", GetObjectType(objectType, false));
            column = checkProtocolTable.Columns.Add("XmlSide", typeof(string));
            column.Caption = "В XML";
            checkProtocolTable.Columns.Add("CriticalError", typeof(bool));
            return checkProtocolTable;
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


        private void GetNonGeneratedId(ObjectType objectType, List<object> rowCellsValues)
        {
            if (objectType == ObjectType.Classifier)
            {
                int tmpId = Convert.ToInt32(rowCellsValues[0]);
                if (tmpId >= developerLowBoundRangeId)
                {
                    if (tmpId > lastDeveloperId)
                        lastDeveloperId = tmpId;
                }
                else
                {
                    if (tmpId > lastId)
                        lastId = tmpId;
                }
            }
        }


        private string GetGeneratorName(IEntity schemeObject, bool developMode)
        {
            if (developMode)
                return schemeObject.DeveloperGeneratorName;
            else
            {
                string generatorName = string.Empty;
                string tableName = schemeObject.FullDBName;
                if (this._scheme.SchemeDWH.FactoryName.ToUpper() != "SYSTEM.DATA.SQLCLIENT")
                    generatorName = "g_" + tableName.Substring(0, tableName.Length > 28 ? 28 : tableName.Length);
                else
                    generatorName = tableName.Substring(0, tableName.Length > 28 ? 28 : tableName.Length);
                return generatorName;
            }
        }


        private int GetNewId(DataRow row, string generatorName, IEntity schemeObject, IDatabase db)
        {
            int oldId = Convert.ToInt32(row["ID"]);
            generatorName = schemeObject.GeneratorName;
            if (oldId >= developerLowBoundRangeId)
                generatorName = GetGeneratorName(schemeObject, true);
            return db.GetGenerator(generatorName);
        }


        /// <summary>
        /// ставит генераторам классификаторов значения больше тех, что импортировали
        /// </summary>
        /// <param name="lastImportedId"></param>
        /// <param name="lastImportedDeveloperId"></param>
        /// <param name="db"></param>
        private void SetGeneratorsNormalValues(int lastImportedId, int lastImportedDeveloperId, IDatabase db)
        {
            string developerGeneratorName = GetGeneratorName((IEntity)this.schemeObject, true);
            // получим название обычного генератора 
            string generatorName = GetGeneratorName((IEntity)this.schemeObject, false);

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
            }
        }

        #endregion

    }
}
