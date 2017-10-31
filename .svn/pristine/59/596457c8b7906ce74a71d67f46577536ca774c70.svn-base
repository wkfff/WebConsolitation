using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports.Common
{
    public class ParamBookInfo
    {
        private const string TextSplitter = ";";
        private const string TextSplitterFull = "; ";

        public class SearchInfo
        {
            public string Values { get; set; }
            public string Caption { get; set; }
        }

        private readonly Dictionary<string, object> cache = new Dictionary<string, object>();
        public string notSelectedText = "Не выбран";

        public string EntityKey { get; set; }
        public bool HasHierarchy { get; set; }
        public bool DeepSelect { get; set; }
        public bool MultiSelect { get; set; }
        public bool HasSource { get; set; }
        public string SourceYear { get; set; }
        public string SourceName { get; set; }
        public string SourceCode { get; set; }
        public string SourceMonth { get; set; }
        public string SourceVariant { get; set; }
        public bool FullScreen { get; set; }
        public string ItemTemplate { get; set; }
        public string DefaultSort { get; set; }
        public bool ReadOnly { get; set; }
        public int ActiveSource { get; set; }
        public string ItemListIdFilter { get; set; }
        public string ItemFilter { get; set; }
        public bool WriteFullText { get; set; }
        public string IdField { get; set; }
        public string InfoField { get; set; }
        public string EditorSearchField { get; set; }

        public Collection<int> SkipLevels { get; set; }

        protected DataTable tblItems = new DataTable();
        protected DataTable tblSource = new DataTable();
        private readonly ReportDBHelper dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);

        private void CreateModifyList()
        {
            SkipLevels = new Collection<int>();
            EntityKey = String.Empty;
            SourceYear = String.Empty;
            SourceName = "ФО";
            SourceMonth = String.Empty;
            SourceCode = "29";
            SourceVariant = String.Empty;
            ItemTemplate = "Name";
            DefaultSort = String.Empty;
            ItemListIdFilter = String.Empty;
            WriteFullText = true;
            InfoField = "Name";
            IdField = "id";
        }

        public ParamBookInfo()
        {
            CreateModifyList();
        }

        public ParamBookInfo(string key)
        {
            CreateModifyList();
            EntityKey = key;
        }

        protected void SetItemsTable(DataTable tbl)
        {
            tblItems = tbl;
        }

        public virtual bool CheckSearchMask(object value)
        {
            return true;
        }

        public virtual SearchInfo GetSearchInfo(object value)
        {
            var result = new SearchInfo();
            var text = Convert.ToString(value).Trim();

            if (String.IsNullOrEmpty(text) || text == ReportConsts.UndefinedKey)
            {
                return result;
            }

            var lstValues = text.Split(new[] { TextSplitter }, StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();

            foreach (var clearValue in lstValues.Select(val => val.Trim()))
            {
                builder.Append(clearValue);
                builder.Append(",");
            }

            var tbl = dbHelper.GetEntityData(
                EntityKey, 
                String.Format("{0} in ({1})", EditorSearchField, builder.ToString().Trim(',')));

            var captions = new string[tbl.Rows.Count];
            var values = new string[tbl.Rows.Count];
            var counter = 0;

            foreach (DataRow row in tbl.Rows)
            {
                values[counter] = Convert.ToString(row[IdField]);
                captions[counter] = Convert.ToString(row[InfoField]);
                counter++;
            }

            result.Caption = String.Join(TextSplitterFull, captions);
            result.Values = String.Join(",", values);

            return result;
        }

        public virtual string CheckValue(object value)
        {
            var text = Convert.ToString(value).Trim();
            if (String.IsNullOrEmpty(text) || text == ReportConsts.UndefinedKey)
            {
                return ReportConsts.UndefinedKey;
            }

            var idValues = text.Split(',');
            var notFoundId = idValues.Where(id => !cache.ContainsKey(id)).ToArray();
            if (notFoundId.Length > 0)
            {
                foreach (var row in GetDataRows(String.Join(",", notFoundId)))
                {
                    cache.Add(Convert.ToString(row[IdField]), row[InfoField]);
                }
            }

            idValues = idValues.Where(id => cache.ContainsKey(id)).ToArray();
            return idValues.Length > 0 ? String.Join(",", idValues) : ReportConsts.UndefinedKey;
        }

        public virtual string GetText(object value)
        {
            var text = CheckValue(value);
            if (text == ReportConsts.UndefinedKey)
            {
                return String.Empty;
            }

            var idValues = text.Split(',');
            var names = (from id in idValues
                         let name = Convert.ToString(cache[id]).Trim()
                         where !String.IsNullOrEmpty(name)
                         select name).ToArray();

            if (names.Length == 0)
            {
                return String.Empty;
            }

            return WriteFullText
                       ? String.Join(TextSplitterFull, names)
                       : names[0];
        }

        protected virtual int GetActiveSource()
        {
            var dt = dbHelper.GetTableData(String.Format(
                        "select sourceid from ObjectVersions where IsCurrent = 1 and objectkey='{0}'", EntityKey));
            return dt.Rows.Count > 0
                       ? Convert.ToInt32(dt.Rows[0]["sourceid"])
                       : Convert.ToInt32(ReportConsts.UndefinedKey);
        }

        protected virtual int GetSourceID()
        {
            if (!HasSource)
            {
                return GetActiveSource();
            }

            var tblSources = GetSourcesTable();

            if (tblSources.Rows.Count > 0)
            {
                var activeSource = GetActiveSource();
                return tblSources.Rows.Cast<DataRow>().Any(row => activeSource == Convert.ToInt32(row[HUB_Datasources.id]))
                    ? activeSource
                    : Convert.ToInt32(tblSources.Rows[0][HUB_Datasources.id]);
            }

            return Convert.ToInt32(ReportConsts.UndefinedKey);
        }

        public string GetRowsFilter()
        {
            var filters = new List<string>{"RowType = 0"};

            ActiveSource = GetSourceID();
            if (HasSource || ActiveSource >= 0)
            {
                filters.Add(String.Format("SourceId = {0}", ActiveSource));
            }

            if (!String.IsNullOrEmpty(ItemFilter))
            {
                filters.Add(String.Format(ItemFilter));
            }

            return String.Join(" and ", filters.ToArray());
        }

        protected string GetIdFilter(string id)
        {
            return !String.IsNullOrEmpty(id) ? String.Format("{0} in ({1})", IdField, id) : String.Empty;
        }

        public virtual DataRow[] GetDataRows(string id = null)
        {
            var idFilter = GetIdFilter(id);
            if (tblItems.Rows.Count > 0)
            {
                return tblItems.Select(idFilter);
            }

            var filter = ReportDataServer.CombineAnd(GetRowsFilter(), idFilter);
            return dbHelper.GetEntityData(EntityKey, filter).Select();
        }

        public virtual DataTable CreateDataList()
        {
            if (tblItems.Rows.Count > 0)
            {
                return tblItems;
            }

            var sortFieldName = !String.IsNullOrEmpty(DefaultSort)
                ? DefaultSort
                : HasHierarchy ? InfoField : IdField;
            tblItems = dbHelper.GetEntityData(EntityKey, GetRowsFilter());
            tblItems = DataTableUtils.SortDataSet(tblItems, sortFieldName);
            return tblItems;
        }

        private static DataTable ApplySourceFilter(DataTable tblSources, string filterName, string filterValue)
        {
            if (String.IsNullOrEmpty(filterValue))
            {
                return tblSources;
            }
            var sourceFilter = String.Format("{0} in ({1})", filterName, filterValue);
            return DataTableUtils.FilterDataSet(tblSources, sourceFilter);
        }

        private DataTable GetSourcesTable()
        {
            var tblSources = ConvertorSchemeLink.scheme.DataSourceManager.GetDataSourcesInfo();
            tblSources = ApplySourceFilter(tblSources, HUB_Datasources.SUPPLIERCODE, String.Format("'{0}'", SourceName));
            tblSources = ApplySourceFilter(tblSources, HUB_Datasources.DATACODE, SourceCode);
            tblSources = ApplySourceFilter(tblSources, HUB_Datasources.YEAR, SourceYear);
            tblSources = ApplySourceFilter(tblSources, HUB_Datasources.MONTH, SourceMonth);
            tblSources = ApplySourceFilter(tblSources, HUB_Datasources.VARIANT, SourceVariant);
            tblSources = DataTableUtils.SortDataSet(tblSources, HUB_Datasources.id);
            return tblSources;
        }

        public virtual DataTable CreateSourceList()
        {
            if (HasSource)
            {
                if (tblSource.Rows.Count == 0)
                {
                    tblSource = GetSourcesTable();
                }
            }

            return tblSource;
        }

        protected virtual string GetItemFilter()
        {
            return !String.IsNullOrEmpty(ItemListIdFilter) ?
                String.Format("{1} in ({0})", ItemListIdFilter, IdField) : 
                String.Empty;
        }

        public void ClearItemsList()
        {
            tblSource.Clear();
            tblItems.Clear();
            cache.Clear();
        }
    }
}
