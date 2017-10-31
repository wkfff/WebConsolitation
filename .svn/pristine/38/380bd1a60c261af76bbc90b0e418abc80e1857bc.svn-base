using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData.FNS;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFNS.ReportQueries;
using Krista.FM.Client.Reports.Common.CommonParamForm;

namespace Krista.FM.Client.Reports
{
    public enum UFNSMark
    {
        Debts = 0,      // задолженность
        Arrears,        // недоимка
        Compens,        // возмещено
        Income,         // поступило
        Charge,         // начислено
        Overpayment     // переплата
    }

    public partial class ReportDataServer
    {
        private string GetRefDGroupByKD(string filterKD)
        {
            if (filterKD == String.Empty)
            {
                return String.Empty;
            }

            const string undefinedRefDGroup = "-1";
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var fltKD = filterHelper.RangeFilter(b_KD_Bridge.ID, filterKD);
            var entityKD = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var tblKD = dbHelper.GetEntityData(entityKD, fltKD);
            var refDGroups = (from DataRow row in tblKD.Rows
                              let refDGroup = Convert.ToString(row[b_KD_Bridge.RefDGroup])
                              where refDGroup != undefinedRefDGroup
                              select refDGroup).ToArray();
            return String.Join(",", refDGroups);
        }

        public static string GetUFNSMarkCodes(UFNSMark mark)
        {
            switch (mark)
            {
                case UFNSMark.Debts:
                    return GetCalculatedCode("40100");
                case UFNSMark.Arrears:
                    return "40200";
                case UFNSMark.Compens:
                    return GetCalculatedCode("30300");;
                case UFNSMark.Income:
                    return GetCalculatedCode("30200");;
                case UFNSMark.Charge:
                    return GetCalculatedCode("30100");;
                case UFNSMark.Overpayment:
                    return GetCalculatedCode("50000");;
                default:
                    return String.Empty;
            }
        }

        public static string GetCalculatedCode(string code)
        {
            switch (code)
            {
                case "30100":
                    return "30110, 30120, 30130, 30140";
                case "30200":
                    return "30210, 30220, 30230, 30240";
                case "30300":
                    return "30310, 30320, 30330, 30340";
                case "40100":
                    return "40110, 40120, 40130, 40140, 40150";
                case "40600":
                    return "40610, 40620, 40630, 40640";
                case "40700":
                    return "40710, 40720, 40730";
                case "40800":
                    return "40810, 40820, 40830";
                case "50000":
                    return "50100, 50200, 50300, 50400, 50500";
                case "920001":
                    return "30210, -30310";
                case "920002":
                    return "30220, -30320";
                case "920003":
                    return "30230, -30330";
                case "920004":
                    return "30240, -30340";
                case "920000":
                    return "30210, -30310, 30220, -30320, 30230, -30330, 30240, -30340";
                default:
                    return code;
            }
        }

        public static string GetAbsCodes(string codes)
        {
            return codes.Replace("-", "");
        }

        public static string GetContainsCodes(string codes, string container)
        {
            var listCodes = ConvertToIntList(codes);
            var listContainer = ConvertToIntList(container);
            var resultList = listCodes.Where(listContainer.Contains);
            return ConvertToString(resultList);
        }

        public static string GetNotContainsCodes(string codes, string container)
        {
            var listCodes = ConvertToIntList(codes);
            var listContainer = ConvertToIntList(container);
            var resultList = listCodes.Where(code => !listContainer.Contains(code));
            return ConvertToString(resultList);
        }

        public static string GetDistinctCodes(string codes)
        {
            var list = codes.Split(',');
            list = list.Select(code => code.Trim()).ToArray();
            return String.Join(",", list.Distinct().ToArray());
        }

        public static List<int> ConvertToIntList(string codes)
        {
            var list = codes.Split(',');
            list = list.Select(code => code.Trim()).ToArray();
            var intCodes = new List<int>();
            foreach (var code in list.Where(code => code != String.Empty))
            {
                int res;
                if (Int32.TryParse(code, out res))
                {
                    intCodes.Add(res);
                }
            }

            return intCodes;
        }

        public static string SortCodes(string codes)
        {
            var list = ConvertToIntList(codes);
            list.Sort();
            return ConvertToString(list);
        }

        public static string ConvertToString(IEnumerable<int> codes)
        {
            var strCodes = codes.Select(code => Convert.ToString(code));
            return String.Join(",", strCodes.ToArray());
        }

        public static string ConvertToString(IEnumerable<string> codes)
        {
            return String.Join(",", codes.Where(code => code != String.Empty).ToArray());
        }

        private string FormatOkved(object okvedVal)
        {
            var strOkved = Convert.ToString(okvedVal);
            const int okvedLen = 10;
            var ind = strOkved.IndexOf(' ');

            if (strOkved == String.Empty || ind > okvedLen)
            {
                return strOkved;
            }

            var okved = strOkved.Substring(0, ind).PadLeft(okvedLen, '0');

            for (var i = 1; i < okvedLen / 2; i++)
            {
                okved = okved.Insert(i * 3 - 1, ".");
            }

            return String.Format("{0}{1}", okved, strOkved.Substring(ind));
        }

        private static List<int> GetSelectedYears(string yearCodes, bool addCurrentYearIfEmpty = true)
        {
            yearCodes = ReportMonthMethods.CheckBookValue(yearCodes);
            if (yearCodes == String.Empty)
            {
                return addCurrentYearIfEmpty ? new List<int> {DateTime.Now.Year} : new List<int>();
            }

            return (from i in ConvertToIntList(yearCodes)
                    let year = ReportMonthMethods.GetSelectedYear(Convert.ToString(i))
                    orderby year
                    select year).ToList();
        }

        private static string GetYearsBoundsFilter(IEnumerable<int> years, bool usePrefix = false)
        {
            var filterHelper = new QFilterHelper();
            var yearBounds = new List<string>();
            const string fltSplitter = " or\r\n\t ";

            foreach (var year in years)
            {
                var yearLoBound = GetUNVYearLoBound(year);
                var yearHiBound = GetUNVYearEnd(year);
                var yearPeriod = filterHelper.PeriodFilter(f_D_FNS3Cons.RefYearDayUNV, yearLoBound, yearHiBound, usePrefix);
                yearBounds.Add(yearPeriod);
            }

            var filter = String.Join(fltSplitter, yearBounds.ToArray());
            return years.Count() > 1 ? String.Format("({0})", filter) : filter;
        }
    }

    public class Hierarchy
    {
        const int RootLevel = -1;
        const string fieldId = "id";
        const string fieldParentId = "parentid";

        private int _level = RootLevel;
        private int _depth = 0;
        private List<Hierarchy> _child;
        public int Id { get; private set; }
        public DataRow Row { get; private set; }
        public Hierarchy Parent { get; private set; }
        public bool HasChild { get { return Child.Count > 0; } }
        public void Sort(string rowField, bool descending = false)
        {
            if (_child == null)
            {
                return;
            }

            _child = !descending
                         ? _child.OrderBy(e => e.Row[rowField]).ToList()
                         : _child.OrderByDescending(e => e.Row[rowField]).ToList();

            foreach (var node in _child)
            {
                node.Sort(rowField, descending);
            }
        }

        public List<Hierarchy> Child
        {
            get { return _child != null ? new List<Hierarchy>(_child) : new List<Hierarchy>(); }
        }

        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;

                foreach (var node in Child)
                {
                    node.Level = _level + 1;
                }
            }
        }

        public int Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
                if (Parent != null && Parent.Id != Id)
                {
                    Parent.Depth = _depth + 1;
                }
            }
        }

        public void AddChild(Hierarchy child)
        {
            if (child == null)
            {
                return;
            }

            if (_child == null)
            {
                _child = new List<Hierarchy>();
            }

            child.Parent = this;
            child.Level = Level + 1;
            Depth = child.Depth + 1;
            _child.Add(child);
        }

        private Hierarchy(int id, DataRow row)
        {
            Parent = null;
            Id = id;
            Row = row;
        }

        private Hierarchy GetParentNode(int parentId, Dictionary<int, Hierarchy> nodes, string tableKey)
        {
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            var id = parentId;

            while (!nodes.ContainsKey(id))
            {
                var dt = dbHelper.GetEntityData(tableKey, String.Format("{0} = {1}", fieldId, id));
                if (dt.Rows.Count == 0 || dt.Rows[0][fieldParentId] == DBNull.Value)
                {
                    return this;
                }

                id = Convert.ToInt32(dt.Rows[0][fieldParentId]);
            }

            return nodes[id];
        }

        private void CreateHierarchy(DataTable dt, string tableKey)
        {
            var nodes = (from DataRow row in dt.Rows
                         let id = Convert.ToInt32(row[fieldId])
                         let node = new Hierarchy(id, row)
                         select new {id, node}).ToDictionary(e => e.id, e => e.node);

            foreach (DataRow row in dt.Rows)
            {
                var id = Convert.ToInt32(row[fieldId]);
                var parent = row[fieldParentId] != DBNull.Value
                                 ? GetParentNode(Convert.ToInt32(row[fieldParentId]), nodes, tableKey)
                                 : this;
                if (id != parent.Id)
                {
                    parent.AddChild(nodes[id]);
                }
            }
        }

        public Hierarchy(DataTable dt, string tableKey)
        {
            CreateHierarchy(dt, tableKey);
        }

        public Hierarchy(string tableKey, string filter)
        {
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            if (filter == String.Empty)
            {
                CreateHierarchy(dbHelper.GetEntityData(tableKey), tableKey);
                return;
            }

            var entity = ConvertorSchemeLink.GetEntity(tableKey);
            var query = String.Format("select {0}, {1} from {2} where {3}",
                                      fieldId,
                                      fieldParentId,
                                      entity.FullDBName,
                                      "{0}");

            var tblIds = dbHelper.GetTableData(String.Format(query, filter));
            var ids = (from DataRow row in tblIds.Rows
                       let id = Convert.ToInt32(row[fieldId])
                       let parentId = row[fieldParentId]
                       select new {id, parentId}).ToDictionary(e => e.id, e => e.parentId);
            for (var i = 0; i < ids.Count; i++)
            {
                var id = ids.ElementAt(i);
                if (id.Value == DBNull.Value)
                {
                    continue;
                }

                var parentId = Convert.ToInt32(id.Value);
                if (ids.ContainsKey(parentId))
                {
                    continue;
                }

                var filterId = String.Format("{0} in ({1})", fieldId, parentId);
                var tblId = dbHelper.GetTableData(String.Format(query, filterId));
                if (tblId.Rows.Count > 0)
                {
                    ids.Add(parentId, tblId.Rows[0][fieldParentId]);
                }
            }

            if (ids.Count > 0)
            {
                var keys = ids.Keys.Select(key => Convert.ToString(key)).ToArray();
                var filterId = String.Format("{0} in ({1})", fieldId, String.Join(",", keys));
                CreateHierarchy(dbHelper.GetEntityData(tableKey, filterId), tableKey);
            }
        }

        public List<Hierarchy> ToList()
        {
            var result = new List<Hierarchy> {this};
            result.AddRange(ChildAll());
            return result;
        }

        public List<Hierarchy> ChildAll()
        {
            var result = new List<Hierarchy>();
            foreach (var node in Child)
            {
                result.Add(node);
                result.AddRange(node.ChildAll());
            }

            return result;
        }

        public List<Hierarchy> ChildOfLevel(int level)
        {
            var result = new List<Hierarchy>();
            foreach (var node in Child)
            {
                if (node.Level == level)
                {
                    result.Add(node);
                }
                else
                {
                    result.AddRange(node.ChildOfLevel(level));
                }
            }

            return result;
        }

        public List<Hierarchy> ChildOfLastLevel()
        {
            var result = new List<Hierarchy>();
            foreach (var node in Child)
            {
                if (node.Child.Count == 0)
                {
                    result.Add(node);
                }
                else
                {
                    result.AddRange(node.ChildOfLastLevel());
                }
            }

            return result;
        }

        public DataTable HierarchyTable()
        {
            var dt = new DataTable();
            dt.Columns.Add(fieldId, typeof(int));
            dt.Columns.Add(fieldParentId, typeof(int));
            foreach(var node in ChildAll())
            {
                var row = node.Parent.Level != RootLevel
                                   ? dt.Rows.Add(node.Id, node.Parent.Id)
                                   : dt.Rows.Add(node.Id, DBNull.Value);
            }
            return dt;
        }
    }

    public class MarksNDFLTable
    {
        public string TableKey = b_Marks_FNS5NDFLBridge.InternalKey;
        public string Id = b_Marks_FNS5NDFLBridge.ID;
        public string Name = b_Marks_FNS5NDFLBridge.Name;
        public string Code = b_Marks_FNS5NDFLBridge.Code;
        public Enum FilterKey;

        public MarksNDFLTable(Enum key, Enum bridgeKey)
        {
            FilterKey = bridgeKey;
            if (ParamStore.container[ReportConsts.ParamMark].BookInfo.EntityKey == d_Marks_FNS5NDFL.InternalKey)
            {
                TableKey = d_Marks_FNS5NDFL.InternalKey;
                Id = d_Marks_FNS5NDFL.ID;
                Name = d_Marks_FNS5NDFL.Name;
                Code = d_Marks_FNS5NDFL.Code;
                FilterKey = key;
            }
        }

        public string GetRowsFilter()
        {
            return ParamStore.container[ReportConsts.ParamMark].BookInfo.GetRowsFilter();
        }
    }

    public class MarksDDKTable
    {
        public string TableKey = b_Marks_FNS5DDKBridge.InternalKey;
        public string Id = b_Marks_FNS5DDKBridge.ID;
        public string Name = b_Marks_FNS5DDKBridge.Name;
        public string Code = b_Marks_FNS5DDKBridge.Code;
        public string RefUnits = b_Marks_FNS5DDKBridge.RefUnits;
        public Enum FilterKey;

        public MarksDDKTable(Enum key, Enum bridgeKey)
        {
            FilterKey = bridgeKey;
            if (ParamStore.container[ReportConsts.ParamMark].BookInfo.EntityKey == d_Marks_FNS5DDK.InternalKey)
            {
                TableKey = d_Marks_FNS5DDK.InternalKey;
                Id = d_Marks_FNS5DDK.ID;
                Name = d_Marks_FNS5DDK.Name;
                Code = d_Marks_FNS5DDK.Code;
                RefUnits = d_Marks_FNS5DDK.RefUnits;
                FilterKey = key;
            }
        }

        public string GetRowsFilter()
        {
            return ParamStore.container[ReportConsts.ParamMark].BookInfo.GetRowsFilter();
        }
    }

    public class MarksYSNTable
    {
        public string TableKey = b_Marks_FNS5YSNBridge.InternalKey;
        public string Id = b_Marks_FNS5YSNBridge.ID;
        public string Name = b_Marks_FNS5YSNBridge.Name;
        public string Code = b_Marks_FNS5YSNBridge.Code;
        public string RefUnits = b_Marks_FNS5YSNBridge.RefUnits;
        public Enum FilterKey;

        public MarksYSNTable(Enum key, Enum bridgeKey)
        {
            FilterKey = bridgeKey;
            if (ParamStore.container[ReportConsts.ParamMark].BookInfo.EntityKey == d_Marks_FNS5YSN.InternalKey)
            {
                TableKey = d_Marks_FNS5YSN.InternalKey;
                Id = d_Marks_FNS5YSN.ID;
                Name = d_Marks_FNS5YSN.Name;
                Code = d_Marks_FNS5YSN.Code;
                RefUnits = d_Marks_FNS5YSN.RefUnits;
                FilterKey = key;
            }
        }

        public string GetRowsFilter()
        {
            return ParamStore.container[ReportConsts.ParamMark].BookInfo.GetRowsFilter();
        }
    }

    public class ReportUFNSHelper
    {
        public static bool IsRubFNS5NDFLMark(object markName)
        {
            const string unitId = "шт."; // показатель исчисляется в штуках
            return !Convert.ToString(markName).ToLower().Contains(unitId);
        }

        public static string GetFNS5NDFLMarkShortCode(object markCode)
        {
            const int codeLength = 4; // длина укороченного кода
            var code = Convert.ToString(markCode);
            return code.Length >= codeLength ? code.Substring(0, codeLength) : String.Empty;
        }

    }
}
