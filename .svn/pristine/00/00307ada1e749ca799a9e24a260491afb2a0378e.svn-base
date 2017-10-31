using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ext.Net.MVC;

using Krista.FM.Common;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    public class EntityDataService : IEntityDataService
    {
        private readonly IScheme scheme;
        private readonly string concatenateChar;

        public EntityDataService(IScheme scheme)
        {
            this.scheme = scheme;

            concatenateChar = GetConcatenateChar(scheme.SchemeDWH.FactoryName);
        }

        public delegate StringBuilder BuildQueryString(IEntity entity, string concatenateChar);

        public AjaxStoreResult GetData(IEntity entity, int start, int limit, string dir, string sort, string filter, IDbDataParameter[] prms)
        {
            return GetData(entity, start, limit, dir, sort, filter, prms, new EntityDataQueryBuilder());
        }

        public AjaxStoreResult GetData(IEntity entity, int start, int limit, string dir, string sort, string filter, IDbDataParameter[] prms, IEntityDataQueryBuilder queryBuilder)
        {
            StringBuilder queryWithLookups = queryBuilder.BuildQuery(entity, concatenateChar);
            StringBuilder queryWithFilters = new StringBuilder();
            queryWithFilters
                .Append("select * from (")
                .Append(queryWithLookups)
                .Append(") T");
            if (!filter.IsNullOrEmpty())
            {
                queryWithFilters
                    .Append(" where ")
                    .Append(filter);
            }

            DataTable table;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                table = (DataTable)db.ExecQuery(queryWithFilters.ToString(), QueryResultTypes.DataTable, prms);
            }

            // Fix для SqlServer: названия полей должны быть в верхнем регистре
            table.Columns.Each<DataColumn>(x => x.ColumnName = x.ColumnName.ToUpper());

            return new AjaxStoreResult(
                table.SelectPage(start, limit, sort, dir), 
                table.Rows.Count);
        }

        public List<long> Create(IEntity entity, List<Dictionary<string, object>> table, Dictionary<long, long> idHierarchy)
        {
            var result = new List<long>();
            long? newId = null;
            bool hierarchyData = idHierarchy != null 
                                 && idHierarchy.Count > 0;

            using (IDataUpdater du = entity.GetDataUpdater("1 <> 1", null, null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);

                foreach (var item in table)
                {
                    DataRow row = dt.NewRow();

                    foreach (DataColumn column in dt.Columns)
                    {
                        if (item.ContainsKey(column.ColumnName.ToUpper()))
                        {
                            try
                            {
                                row[column] =
                                    Convert.ToString(item[column.ColumnName.ToUpper()]).IsNotNullOrEmpty()
                                        ? item[column.ColumnName.ToUpper()]
                                        : DBNull.Value;
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message, e);
                            }
                        }
                    }

                    try
                    {
                        newId = entity.GetGeneratorNextValue;
                        if (hierarchyData)
                        {
                            /* сохраняем старое значение идентификатора */
                            var oldId = long.Parse(row["ID"].ToString());
                            /* генерируем новый */
                            row["ID"] = newId;
                            /* заменяем в списке идентификаторов */
                            idHierarchy.Remove(oldId);
                            idHierarchy.Add(oldId, (long)newId);
                        }
                        else
                        {
                            row["ID"] = newId;
                        }
                    }
                    catch (Exception)
                    {
                        // для SqlServer глушим ошибку получения значения
                        // генератора для таблиц фактов
                    }

                    if (hierarchyData)
                    {
                        long parentId = -1;

                        // если родительский узел - тоже новая запись, ищем ее новый ID
                        try
                        {
                            parentId = long.Parse(row["PARENTID"].ToString());
                        }
                        catch (Exception)
                        {
                        }

                        long newParentId = parentId;

                        if (parentId < -1)
                        {
                            idHierarchy.TryGetValue(parentId, out newParentId);
                        }

                        if (newParentId != parentId)
                        {
                            row["PARENTID"] = newParentId;
                        }
                    }

                    dt.Rows.Add(row);
                }

                du.Update(ref dt);

                // для SqlServer получаем значения первичного ключа для 
                // вставленных записей
                if (newId == null && dt.Rows.Count > 0)
                {
                    newId = Convert.ToInt64(dt.Rows[0]["ID"]);
                }

                result.Add((long)newId);
            }

            return result;
        }

        public void Update(IEntity entity, List<Dictionary<string, object>> table)
        {
            string filter = "(ID = {0})".FormatWith(
                           String.Join(" or ID = ", table.Select(row => Convert.ToString(row["ID"])).ToArray()));

            using (IDataUpdater du = entity.GetDataUpdater(filter, null, null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);

                foreach (DataRow row in dt.Rows)
                {
                    string id = Convert.ToString(row["ID"]);
                    Dictionary<string, object> changedRow = table.Where(x => Convert.ToString(x["ID"]) == id).First();

                    foreach (DataColumn column in dt.Columns)
                    {
                        if (changedRow.ContainsKey(column.ColumnName.ToUpper()))
                        {
                            row[column] = Convert.ToString(changedRow[column.ColumnName.ToUpper()]).IsNullOrEmpty()
                                                        ? DBNull.Value
                                                        : changedRow[column.ColumnName.ToUpper()];
                        }
                    }
                }

                du.Update(ref dt);
            }
        }

        public void Delete(IEntity entity, List<Dictionary<string, object>> table)
        {
            string filter = "(ID = {0})".FormatWith(
                            String.Join(" or ID = ", table.Select(row => Convert.ToString(row["ID"])).ToArray()));
            using (IDataUpdater du = entity.GetDataUpdater(filter, null, null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);

                foreach (var item in table)
                {
                    DataRow[] row = dt.Select("ID = {0}".FormatWith(item["ID"]));
                    if (row.Count() == 1)
                    {
                        row[0].Delete();
                    }
                }

                du.Update(ref dt);
            }
        }

        public void DeleteDependentData(IEntity entity, int rowId)
        {
            foreach (IEntityAssociation item in entity.Associated.Values)
            {
                string refField = item.RoleDataAttribute.Name;
                string refTable = item.RoleData.FullDBName;

                string queryText = String.Format("delete from {0} where {1} = ?", refTable, refField);
                using (var db = scheme.SchemeDWH.DB)
                {
                    db.ExecQuery(queryText, QueryResultTypes.NonQuery, new DbParameterDescriptor("refId", rowId));
                }
            }
        }

        private static string GetConcatenateChar(string factoryName)
        {
            if (factoryName == ProviderFactoryConstants.OracleClient ||
                factoryName == ProviderFactoryConstants.OracleDataAccess ||
                factoryName == ProviderFactoryConstants.MSOracleDataAccess)
            {
                return "||";
            }

            if (factoryName == ProviderFactoryConstants.SqlClient)
            {
                return "+";
            }

            throw new Exception(String.Format("Поддержка провайдера {0} не реализована.", factoryName));
        }
    }
}
