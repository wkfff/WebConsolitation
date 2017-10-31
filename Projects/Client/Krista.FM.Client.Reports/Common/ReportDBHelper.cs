using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports
{
    public class ReportDBHelper
    {
        private readonly IScheme scheme;

        public ReportDBHelper(IScheme currentScheme)
        {
            scheme = currentScheme;
        }

        public DataTable GetEntityData(IEntity entity, string filter)
        {
            if (entity != null)
            {
                using (var upd = entity.GetDataUpdater(filter, null))
                {
                    var dt = new DataTable();
                    upd.Fill(ref dt);
                    return dt;
                }
            }

            return null;
        }

        public DataTable GetEntityData(string entityKey)
        {
            return GetEntityData(entityKey, String.Empty);
        }

        public DataTable GetEntityData(string entityKey, string filter)
        {
            var entity = scheme.RootPackage.FindEntityByName(entityKey);
            return GetEntityData(entity, filter);
        }

        public DataTable GetTableData(string selectStr)
        {
            using (var db = scheme.SchemeDWH.DB)
            {
                var tableData = (DataTable)db.ExecQuery(selectStr, QueryResultTypes.DataTable);
                return tableData;
            }
        }

        public object GetScalarData(string selectStr)
        {
            using (var db = scheme.SchemeDWH.DB)
            {
                return db.ExecQuery(selectStr, QueryResultTypes.Scalar);
            }
        }

        public int GetActiveSource(string entityKey)
        {
            var dt = GetTableData(String.Format(
                        "select sourceid from ObjectVersions where IsCurrent = 1 and objectkey='{0}'", entityKey));
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["sourceid"]) : -1;
        }

        public string GetSourceFilter(string entityKey)
        {
            var source = GetActiveSource(entityKey);
            return source != -1 ? String.Format("sourceid = {0}", source) : String.Empty;
        }

        public DataTable GetSources(string filter)
        {
            var tblSources = ConvertorSchemeLink.scheme.DataSourceManager.GetDataSourcesInfo();
            return DataTableUtils.FilterDataSet(tblSources, filter);
        }

        public DataTable GetTableSources(string tableKey)
        {
            var entity = ConvertorSchemeLink.GetEntity(tableKey);
            var tblSourceid = GetTableData(String.Format("select distinct sourceid from {0}", entity.FullDBName));
            var sources = tblSourceid.Rows.Cast<DataRow>().Select(row => Convert.ToString(row[0])).ToArray();
            var filter = sources.Length > 0
                             ? String.Format("{0} in ({1})", HUB_Datasources.id, String.Join(",", sources))
                             : ReportConsts.UndefinedKey;
            return GetSources(filter);
        }
    }
}
