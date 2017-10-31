using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.UFK.ReportMaster
{
    public class TableField
    {
        public string TableKey = String.Empty;
        public string Name = String.Empty;

        public TableField(string tableKey, string field)
        {
            TableKey = tableKey;
            Name = field;
        }

        public static bool operator ==(TableField a, TableField b)
        {
            return a.TableKey == b.TableKey && a.Name == b.Name;
        }

        public static bool operator !=(TableField a, TableField b)
        {
            return a.TableKey != b.TableKey || a.Name != b.Name;
        }
    }

    public class TableFieldComparer : IEqualityComparer<TableField>
    {
        public bool Equals(TableField a, TableField b)
        {
            return a == b;
        }

        public int GetHashCode(TableField tableField)
        {
            return tableField.GetHashCode();
        }
    }

    public class ReportLookupField
    {
        public int Index = -1;
        public Type Type = typeof(string);
        public List<TableField> Path;
        public TableField Field
        {
            get
            {
                if (Path == null || Path.Count == 0)
                {
                    return null;
                }

                return Path[Path.Count - 1];
            }
        }

        public ReportLookupField(List<TableField> path)
        {
            Path = path;
        }

        public ReportLookupField(string tableKey, string field)
        {
            AddField(tableKey, field);
        }

        public TableField AddField(string tableKey, string field)
        {
            if (Path == null)
            {
                Path = new List<TableField>();
            }

            var tableField = new TableField(tableKey, field);
            Path.Add(tableField);
            return tableField;
        }

        public object GetValue(object id, Dictionary<string, DataTable> bridges)
        {
            var field = id;

            foreach (var tableField in Path)
            {
                if (!bridges.ContainsKey(tableField.TableKey))
                {
                    return null;
                }

                var dtRows = bridges[tableField.TableKey].Select(String.Format("id = {0}", field));
                if (dtRows.Length == 0)
                {
                    return null;
                }

                field = dtRows[0][tableField.Name];
            }

            return field;
        }
    }

    public enum ReportColumnType
    {
        Value,
        Calculated,
        Caption
    }

    public class ReportColumn
    {
        public Report Report = null;
        public int Index = -1;
        public int Style = 0;
        public string Header = null;
        public Type Type = typeof(decimal);
        public ReportColumnType ColumnType = ReportColumnType.Value;
        public bool SumNestedRows = false;

        public virtual object GetValue(ReportRow row)
        {
            return null;
        }
    }

    public class StoreKeyPair
    {
        public int Index { get; private set; }
        public int Level { get; private set; }
        public StoreKeyPair(int index, int level) {Index = index; Level = level;}
    }

    public class Store<T>
    {
        private readonly Dictionary<int, SortedDictionary<int, T>> store;
        public int Count { get { return store.Values.Sum(level => level.Count); } }

        public Store()
        {
            store = new Dictionary<int, SortedDictionary<int, T>>();
        }

        public bool ContainsKey(int index, int level)
        {
            return store.ContainsKey(index) && store[index].ContainsKey(level);
        }

        public T this[int index, int level]
        {
            get { return store[index][level]; }
            set
            {
                if (!store.ContainsKey(index))
                {
                    store.Add(index, new SortedDictionary<int, T>());
                }

                store[index][level] = value;
            }
        }

        public T this[StoreKeyPair key]
        {
            get { return this[key.Index, key.Level]; }
            set { this[key.Index, key.Level] = value; }
        }

        public void RemoveKey(int index, int level)
        {
            if (ContainsKey(index, level))
            {
                store[index].Remove(level);
            }
        }

        public void RemoveKey(StoreKeyPair key)
        {
            RemoveKey(key.Index, key.Level);
        }

        public void RemoveKey(int index)
        {
            if (store.ContainsKey(index))
            {
                store.Remove(index);
            }
        }

        public void ChangeKey(int oldIndex, int newIndex)
        {
            if (!store.ContainsKey(oldIndex)) return;

            var value = store[oldIndex];
            store.Remove(oldIndex);
            store[newIndex] = value;
        }

        public Collection<StoreKeyPair> Keys()
        {
            var keyPairCollection = new Collection<StoreKeyPair>();
            foreach (var index in store.Keys)
            {
                foreach (var level in store[index].Keys)
                {
                    keyPairCollection.Add(new StoreKeyPair(index, level));
                }
            }

            return keyPairCollection;
        }

        public Collection<StoreKeyPair> Keys(int index)
        {
            var keyPairCollection = new Collection<StoreKeyPair>();
            if (store.ContainsKey(index))
            {
                foreach (var level in store[index].Keys)
                {
                    keyPairCollection.Add(new StoreKeyPair(index, level));
                }
            }

            return keyPairCollection;
        }
    }
    
    public class ColumnOutMask
    {
        public int GroupingIndex;
        public int Level;
        public string Mask = String.Empty;
        public List<TableField> OutFields = null;
        public bool ForAllLevels = false;

        public ColumnOutMask(params TableField[] fields)
        {
            if (fields.Length == 0)
            {
                return;
            }

            var masks = new string[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {
                masks[i] = String.Format("{0}{1}{2}", "{", i, "}");
            }

            Mask = String.Join(" ", masks);
            OutFields = fields.ToList();
        }

        public ColumnOutMask(string mask)
        {
            Mask = mask;
        }
    }

    public class ReportCaptionColumn : ReportColumn
    {
        public Store<ColumnOutMask> OutMasks = new Store<ColumnOutMask>();

        private static ReportRow FindRowUp(ReportRow row, int groupingIndex, int level)
        {
            if (row == null || row.GroupingIndex < groupingIndex ||
                (row.GroupingIndex == groupingIndex && row.Level < level))
            {
                return null;
            }

            return row.GroupingIndex == groupingIndex && row.Level == level
                ? row
                : FindRowUp(row.Parent, groupingIndex, level);
        }

        private ColumnOutMask GetOutMask(ReportRow row)
        {
            if (OutMasks.ContainsKey(row.GroupingIndex, row.Level))
            {
                return OutMasks[row.GroupingIndex, row.Level];
            }

            var key = OutMasks.Keys(row.GroupingIndex).FirstOrDefault(e => OutMasks[e].ForAllLevels);
            return key != null ? OutMasks[key] : null;
        }

        public override object GetValue(ReportRow row)
        {
            var mask = GetOutMask(row);
            if (mask == null)
            {
                return null;
            }

            if (mask.OutFields == null)
            {
                return mask.Mask;
            }

            var maskRow = !mask.ForAllLevels ? FindRowUp(row, mask.GroupingIndex, mask.Level) : row;
            if (maskRow == null)
            {
                return null;
            }
            var grouping = Report.Groupings[maskRow.GroupingIndex];
            var count = mask.OutFields.Count;
            var outFields = new object[count];

            for (var i = 0; i < count; i++)
            {
                if (grouping.Field == mask.OutFields[i])
                {
                    outFields[i] = maskRow.Key;
                    continue;
                }

                var lookupFieldIndex = grouping.GetLookupFieldIndex(mask.OutFields[i]);
                if (lookupFieldIndex != -1)
                {
                    outFields[i] = maskRow.LookupFields[lookupFieldIndex];
                }
            }

            if (mask.Mask == null && outFields.Length > 0)
            {
                return outFields[0];
            }

            return mask.Mask != null ? String.Format(mask.Mask, outFields).Trim() : null;
        }

        public ColumnOutMask SetMask(ReportGrouping grouping, int level, ColumnOutMask mask)
        {
            mask.GroupingIndex = grouping.Index;
            mask.Level = level;
            OutMasks[grouping.Index, level] = mask;
            return mask;
        }

        public ColumnOutMask SetMask(ReportGrouping grouping, int level, params TableField[] fields)
        {
            return SetMask(grouping, level, new ColumnOutMask(fields));
        }

        public ColumnOutMask SetMask(ReportGrouping grouping, int level, string tableKey, params string[] fields)
        {
            var tableFields = fields.Select(field => new TableField(tableKey, field));
            return SetMask(grouping, level, tableFields.ToArray());
        }

        public ColumnOutMask SetMask(ReportGrouping grouping, int level, string mask)
        {
            return SetMask(grouping, level, new ColumnOutMask(mask));
        }

        public void SetMasks(ReportGrouping grouping, SortedDictionary<int, ColumnOutMask> masks)
        {
            foreach (var columnOutMask in masks)
            {
                SetMask(grouping, columnOutMask.Key, columnOutMask.Value);
            }
        }

        public void RemoveMask(int groupingIndex, int level)
        {
            OutMasks.RemoveKey(groupingIndex, level);
            foreach (var key in OutMasks.Keys())
            {
                var mask = OutMasks[key];
                if (mask.GroupingIndex == groupingIndex && mask.Level == level)
                {
                    OutMasks.RemoveKey(key);
                }
            }
        }

        public void RemoveMask(int groupingIndex)
        {
            OutMasks.RemoveKey(groupingIndex);
            foreach (var key in OutMasks.Keys().Where(key => OutMasks[key].GroupingIndex == groupingIndex))
            {
                OutMasks.RemoveKey(key);
            }
        }

        public void ChangeMaskIndex(int oldIndex, int newIndex)
        {
            OutMasks.ChangeKey(oldIndex, newIndex);
            foreach (var key in OutMasks.Keys())
            {
                var mask = OutMasks[key];
                if (mask.GroupingIndex == oldIndex)
                {
                    mask.GroupingIndex = newIndex;
                }
            }
        }
    }

    public class ReportValueColumn : ReportColumn
    {
        public TableField Field = null;
        public string RowsFilter = String.Empty;
        public decimal K = 1; // коэффициент
        public decimal Divider = 1;
        public override object GetValue(ReportRow row)
        {
            return K != 1 && row.Values[Index] != null && Type == typeof(decimal)
                ? K * Convert.ToDecimal(row.Values[Index])
                : row.Values[Index];
        }
    }

    public class ReportCalcColumn : ReportColumn
    {
        public delegate object Calculate(ReportRow row, int index);

        public decimal K = 1; // коэффициент
        public decimal Divider = 1;
        public Calculate Function = null;

        public override object GetValue(ReportRow row)
        {
            var result = Function != null ? Function(row, Index) : null;
            return K != 1 && result != null && Type == typeof(decimal)
                ? K * Convert.ToDecimal(result)
                : result;
        }
    }

    public class ReportGrouping
    {
        public delegate void Control(List<ReportRow> rows, int groupingIndex);

        public int Index = -1;
        public DataTable HierarchyTable = null;
        public bool HideHierarchyLevels = false;
        public bool SkipHierarchyLevels = false;
        public int MaxHierarchyLevel = -1;
        public Report Report = null;
        public TableField Field = null;
        public bool IsHierarchical { get { return HierarchyTable != null; } }
        public Type Type = typeof(int);
        public List<ReportLookupField> LookupFields = null;
        public List<object> FixedValues = null;
        public Dictionary<int, RowViewParams> ViewParams = new Dictionary<int, RowViewParams>();
        public Control Function = null;

        public ReportLookupField AddLookupField(string tableKey, string field)
        {
            if (LookupFields == null)
            {
                LookupFields = new List<ReportLookupField>();
            }

            var lookupField = new ReportLookupField(tableKey, field);
            lookupField.Index = LookupFields.Count;
            LookupFields.Add(lookupField);
            return lookupField;
        }

        public void AddLookupField(string tableKey, params string[] fields)
        {
            foreach (var field in fields)
            {
                AddLookupField(tableKey, field);
            }
        }

        public int GetLookupFieldIndex(TableField field)
        {
            if (LookupFields == null)
            {
                return -1;
            }

            foreach (var lookupField in LookupFields.Where(lookupField => lookupField.Field == field))
            {
                return lookupField.Index;
            }

            return -1;
        }

        public void AddSortField(int level, TableField field, int sortType = RowViewParams.SortAsc)
        {
            if (!ViewParams.ContainsKey(level))
            {
                ViewParams.Add(level, new RowViewParams{Style = Index});
            }

            var vp = ViewParams[level];
            if (vp.SortOrder == null)
            {
                vp.SortOrder = new Dictionary<TableField, int> ();
            }

            vp.SortOrder.Add(field, sortType);
        }

        public void AddSortField(TableField field, int sortType = RowViewParams.SortAsc)
        {
            foreach (var vp in ViewParams)
            {
                AddSortField(vp.Key, field, sortType);
            }
        }

        public void AddSortField(string tableKey, string field, int sortType = RowViewParams.SortAsc)
        {
            AddSortField(new TableField(tableKey, field), sortType);
        }

        public void SetFixedValues(string values)
        {
            values = values.Trim();
            if (values == String.Empty)
            {
                FixedValues = null;
                return;
            }

            var list = values.Split(',').Select(value => value.Trim());
            var typedValues = list.Select(value => Convert.ChangeType(value, Type));
            FixedValues = typedValues.Select(value => value).ToList();
        }
    }

    public class ReportRow
    {
        public object Key;
        public int GroupingIndex;
        public int Level = 0;
        public object Style = null;
        public bool BreakSumming = false;
        public bool Hide = false;
        public object[] LookupFields = null;
        public object[] Values = null;
        private bool _fixed = false;
        private ReportRow _parent = null;
        private List<ReportRow> _child = null;

        public bool Fixed
        {
            get { return _fixed; }
            set
            {
                if (_fixed == value)
                {
                    return;
                }

                _fixed = value;
                if (value && Parent != null)
                {
                    Parent.Fixed = true;
                }
            }
        }

        public ReportRow Parent
        {
            get { return _parent; }
            private set
            {
                if (_parent != null)
                {
                    _parent.RemoveChild(this);
                }
                _parent = value;
            }
        }

        public List<ReportRow> Child
        {
            get { return _child != null ? new List<ReportRow>(_child) : new List<ReportRow>(); }
        }

        public bool ContainsChild(object key, int grouping)
        {
            return _child == null ? false : _child.FindIndex(row => row.Key.Equals(key) && row.GroupingIndex == grouping) > -1;
        }

        public List<ReportRow> GetChild(object key, int grouping)
        {
            return _child != null ? _child.FindAll(row => row.Key.Equals(key)) : new List<ReportRow>();
        }

        public void RemoveChild(ReportRow childRow)
        {
            if (childRow != null)
            {
                _child.Remove(childRow);
            }
        }

        public List<ReportRow> GetChildRows(int groupingIndex)
        {
            return Child.Where(child => child.GroupingIndex == groupingIndex).ToList();
        }

        public List<ReportRow> GetNestedRows()
        {
            var rowsList = Child;
            foreach (var row in Child)
            {
                rowsList.AddRange(row.GetNestedRows());
            }
            return rowsList;
        }

        public List<ReportRow> GetNestedRows(int groupingIndex)
        {
            var rowsList = new List<ReportRow>();
            rowsList.AddRange(GetChildRows(groupingIndex));

            foreach (var row in Child.Where(child => child.GroupingIndex <= groupingIndex))
            {
                rowsList.AddRange(row.GetNestedRows(groupingIndex));
            }
            return rowsList;
        }

        public void AddValues(object[] values, IEnumerable<int> sumColumnsIndexies)
        {
            if (sumColumnsIndexies == null)
            {
                return;
            }

            foreach (var i in sumColumnsIndexies.Where(i => Values[i] != null || values[i] != null))
            {
                Values[i] = ReportDataServer.GetDecimal(Values[i]) + ReportDataServer.GetDecimal(values[i]);
            }
        }

        public void AddChild(ReportRow childRow)
        {
            if (_child == null)
            {
                _child = new List<ReportRow>();
            }

            childRow.Parent = this;
            childRow.Level = childRow.GroupingIndex == GroupingIndex ? Level + 1 : 0;
            _child.Add(childRow);
            if (childRow.Fixed)
            {
                Fixed = true;
            }
        }

        public void AddUniqueChild(ReportRow childRow, IEnumerable<int> sumColumnsIndexies = null)
        {
            if (!ContainsChild(childRow.Key, childRow.GroupingIndex))
            {
                AddChild(childRow);
                return;
            }

            var ownChildRow = GetChild(childRow.Key, childRow.GroupingIndex)[0];
            ownChildRow.AddValues(childRow.Values, sumColumnsIndexies);
            ownChildRow.AddUniqueChild(childRow.Child, sumColumnsIndexies);
        }

        public void AddUniqueChild(IEnumerable<ReportRow> childRows, IEnumerable<int> sumColumnsIndexies = null)
        {
            foreach (var childRow in childRows)
            {
                AddUniqueChild(childRow, sumColumnsIndexies);
            }
        }

        public void RemoveNextLevelChild(IEnumerable<int> sumColumnsIndexies)
        {
            foreach (var childRow in GetNestedRows(GroupingIndex))
            {
                AddValues(childRow.Values, sumColumnsIndexies);
            }

            foreach (var childRow in GetChildRows(GroupingIndex))
            {
                AddUniqueChild(childRow.GetNestedRows(GroupingIndex + 1), sumColumnsIndexies);
                _child.Remove(childRow);
            }

            if (_child != null && _child.Count == 0)
            {
                _child = null;
            }
        }
    }

    public class RowViewParams
    {
        public enum ShowType { BeforeChild, AfterChild, SkipBeforeChild, SkipAfterChild };
        public delegate bool Function(ReportRow row, List<ReportColumn> columns);

        public const int SortAsc = 1;
        public const int SortDsc = -1;

        public int Style = 0;
        public ShowType ShowOrder = ShowType.BeforeChild;
        public Dictionary<TableField, int> SortOrder = null;
        public bool BreakSumming = false;
        public Function Filter = null;
        public bool ForAllLevels = false;
    }

    public class ReportRowComparer : IComparer<ReportRow>
    {
        private const int SortByKeyField = -2;

        private class SortFieldParams
        {
            public int Index;
            public Type Type;
            public int SortType;
        }

        private class SortField
        {
            public List<SortFieldParams> SortOrder;
            public bool ForAllLevels;
        }

        private readonly Store<SortField> SortFields;

        public ReportRowComparer(IEnumerable<ReportGrouping> groupings)
        {
            if (groupings == null)
            {
                return;
            }

            SortFields = new Store<SortField>();

            foreach (var grouping in groupings.Where(gr => gr.ViewParams != null))
            {
                foreach (var vp in grouping.ViewParams.Where(vp => vp.Value.SortOrder != null))
                {
                    SortFields[grouping.Index, vp.Key] = new SortField
                    {
                        SortOrder = new List<SortFieldParams>(),
                        ForAllLevels = vp.Value.ForAllLevels
                    };


                    foreach (var so in vp.Value.SortOrder)
                    {
                        var index = grouping.GetLookupFieldIndex(so.Key);
                        if (index >= 0)
                        {
                            var lookupField = grouping.LookupFields[index];
                            SortFields[grouping.Index, vp.Key].SortOrder.Add(new SortFieldParams
                                                                                 {
                                                                                     Index = lookupField.Index,
                                                                                     Type = lookupField.Type,
                                                                                     SortType = so.Value
                                                                                 });
                            continue;
                        }

                        if (grouping.Field == so.Key)
                        {
                            SortFields[grouping.Index, vp.Key].SortOrder.Add(new SortFieldParams
                                                                                 {
                                                                                     Index = SortByKeyField,
                                                                                     Type = grouping.Type,
                                                                                     SortType = so.Value
                                                                                 });
                        }
                    }
                }
            }
  
            if (SortFields.Count == 0)
            {
                SortFields = null;
            }
        }
  
        private int CompareValues(object x, object y, Type type)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            if (type == typeof(string))
            {
                var xv = Convert.ToString(x);
                var yv = Convert.ToString(y);
                return xv.CompareTo(yv);
            }
            if (type == typeof(int))
            {
                var xv = Convert.ToInt32(x);
                var yv = Convert.ToInt32(y);
                return xv.CompareTo(yv);
            }

            if (type == typeof(decimal))
            {
                var xv = Convert.ToDecimal(x);
                var yv = Convert.ToDecimal(y);
                return xv.CompareTo(yv);
            }
            if (type == typeof(DateTime))
            {
                var xv = Convert.ToDateTime(x);
                var yv = Convert.ToDateTime(y);
                return xv.CompareTo(yv);
            }

            return 0;
        }

        private IEnumerable<SortFieldParams> GetSortOrder(int groupingIndex, int level)
        {
            if (SortFields.ContainsKey(groupingIndex, level))
            {
                return SortFields[groupingIndex, level].SortOrder;
            }

            var key = SortFields.Keys(groupingIndex).FirstOrDefault(e => SortFields[e].ForAllLevels);
            return key != null ? SortFields[key].SortOrder : null;
        }

        public int Compare(ReportRow x, ReportRow y)
        {
            var result = 0;

            if (SortFields == null || x.Level != y.Level || x.GroupingIndex != y.GroupingIndex)
            {
                return result;
            }

            var sortOrder = GetSortOrder(x.GroupingIndex, x.Level);
            if (sortOrder == null)
            {
                return result;
            }

            foreach (var sp in sortOrder)
            {
                result = sp.Index == SortByKeyField
                             ? CompareValues(x.Key, y.Key, sp.Type)*sp.SortType
                             : CompareValues(x.LookupFields[sp.Index], y.LookupFields[sp.Index], sp.Type)*sp.SortType;
                if (result != 0)
                {
                    break;
                }
            }

            return result;
        }
    }

    public class Report
    {
        public const int NoRef = -1;
        const int MaxBridgeFilterLength = 200;

        public bool AddNumColumn = false;
        public bool AddTotalRow = true;
        public decimal Divider = 1;
        public string TableKey;
        public List<ReportColumn> Columns;
        public List<ReportGrouping> Groupings;
        public ReportRow Root;
        public RowViewParams.Function RowFilter = Functions.IsNotNullRow;
        private ReportRowComparer comparer = null;

        public Report(string tableKey)
        {
            TableKey = tableKey;
            Columns = new List<ReportColumn>();
            Groupings = new List<ReportGrouping>();
            Root = new ReportRow { GroupingIndex = NoRef };
        }

        public void Clear()
        {
            Root = new ReportRow { GroupingIndex = NoRef };
        }

        public ReportCalcColumn AddCalcColumn(ReportCalcColumn.Calculate function)
        {
            var column = new ReportCalcColumn
            {
                Report = this,
                ColumnType = ReportColumnType.Calculated,
                Index = Columns.Count,
                Function = function,
                Divider = Divider,
                SumNestedRows = true
            };

            Columns.Add(column);
            return column;
        }

        public ReportCalcColumn AddCalcColumn()
        {
            return AddCalcColumn(null);
        }

        public ReportValueColumn AddValueColumn(string tableKey, string field, string filter)
        {
            var column = new ReportValueColumn
            {
                Report = this,
                Index = Columns.Count,
                Field = new TableField(tableKey, field),
                Divider = Divider,
                SumNestedRows = true,
                RowsFilter = filter
            };
            Columns.Add(column);
            return column;
        }

        public ReportValueColumn AddValueColumn(string field, string filter)
        {
            return AddValueColumn(TableKey, field, filter);
        }

        public ReportValueColumn AddValueColumn(string field)
        {
            return AddValueColumn(field, String.Empty);
        }

        public ReportCaptionColumn AddCaptionColumn()
        {
            var column = new ReportCaptionColumn
            {
                Report = this,
                Index = Columns.Count,
                ColumnType = ReportColumnType.Caption,
                Type = typeof(string)
            };
            Columns.Add(column);
            return column;
        }

        public ReportCaptionColumn AddCaptionColumn(ReportGrouping grouping, params TableField[] fields)
        {
            var column = AddCaptionColumn();
            column.SetMask(grouping, 0, fields);
            return column;
        }

        public ReportCaptionColumn AddCaptionColumn(ReportGrouping grouping, string tableKey, params string[] fields)
        {
            var tableFields = fields.Select(field => new TableField(tableKey, field));
            return AddCaptionColumn(grouping, tableFields.ToArray());
        }

        public ReportCaptionColumn InsertCaptionColumn(int index)
        {
            if (index < 0) return null;

            var column = AddCaptionColumn();
            if (index < column.Index)
            {
                Columns.RemoveAt(column.Index);
                Columns.Insert(index, column);
                for (var i = index; i < Columns.Count; i++)
                {
                    Columns[i].Index = i;
                }
            }

            return column;
        }

        public void RemoveCaptionColumn(int index)
        {
            if (index < 0 || index >= Columns.Count) return;

            Columns.RemoveAt(index);
            for (var i = index; i < Columns.Count; i++)
            {
                Columns[i].Index = i;
            }
        }

        public ReportGrouping AddGrouping(string field, ReportGrouping grouping)
        {
            grouping.Report = this;
            grouping.Field = new TableField(TableKey, field);
            grouping.Index = Groupings.Count;
            Groupings.Add(grouping);
            return grouping;
        }

        public ReportGrouping AddGrouping(string field)
        {
            var grouping = AddGrouping(field, new ReportGrouping());
            grouping.ViewParams.Add(0, new RowViewParams { Style = grouping.Index });
            return grouping;
        }

        public void RemoveGrouping(int index)
        {
            if (index < 0 || index >= Groupings.Count) return;

            Groupings.RemoveAt(index);
            foreach (ReportCaptionColumn column in Columns.Where(column => column.ColumnType == ReportColumnType.Caption))
            {
                column.RemoveMask(index);
            }

            for (var i = index; i < Groupings.Count; i++)
            {
                foreach (ReportCaptionColumn column in Columns.Where(column => column.ColumnType == ReportColumnType.Caption))
                {
                    column.ChangeMaskIndex(Groupings[i].Index, i);
                }

                Groupings[i].Index = i;
            }
        }

        private ReportRow CreateReportRow(object childRowKey, int groupingIndex)
        {
            var row = new ReportRow
            {
                GroupingIndex = groupingIndex,
                Key = childRowKey,
                Values = new object[Columns.Count]
            };

            var column = Groupings[groupingIndex];
            if (column.LookupFields != null)
            {
                row.LookupFields = new object[column.LookupFields.Count];
            }

            return row;
        }
  
        private void InsertFixedRows(ReportRow parentRow)
        {
            var groupingIndex = parentRow.GroupingIndex + 1;
            if (groupingIndex >= Groupings.Count)
            {
                return;
            }

            var grouping = Groupings[groupingIndex];

            // вставляем все возможные значения
            if (grouping.FixedValues != null && grouping.FixedValues.Count > 0)
            {
                foreach (var value in grouping.FixedValues)
                {
                    if (!parentRow.ContainsChild(value, groupingIndex))
                    {
                        parentRow.AddChild(CreateReportRow(value, groupingIndex));
                    }

                    foreach (var child in parentRow.GetChild(value, groupingIndex))
                    {
                        child.Fixed = true;
                    }
                }
            }

            foreach (var row in parentRow.Child)
            {
                InsertFixedRows(row);
            }
        }

        private void ProcessRow(ReportValueColumn column, DataRow dtRow, IList<int> indexies)
        {
            var row = Root;

            for (var i = 0; i < indexies.Count; i++)
            {
                var key = dtRow[indexies[i]];
                if (!row.ContainsChild(key, i))
                {
                    row.AddChild(CreateReportRow(key, i));
                }
                row = row.GetChild(key, i)[0];
            }

            var value = row.Values[column.Index];
            row.Values[column.Index] = value != null
                                           ? ReportDataServer.GetDecimal(value) +
                                             ReportDataServer.GetDecimal(dtRow[column.Field.Name])
                                           : dtRow[column.Field.Name];
        }

        private void ProcessRows(ReportValueColumn column, IEnumerable<DataRow> rows)
        {
            if (rows == null || rows.Count() == 0)
            {
                return;
            }

            var fieldIndexies = new List<int>();
            var tblColumns = rows.First().Table.Columns;
            var i = 0;
            while (i < Groupings.Count && tblColumns.Contains(Groupings[i].Field.Name))
            {
                fieldIndexies.Add(tblColumns[Groupings[i].Field.Name].Ordinal);
                i++;
            }

            if (fieldIndexies.Count == 0)
            {
                return;
            }

            foreach (var row in rows)
            {
                ProcessRow(column, row, fieldIndexies);
            }
        }

        public void ProcessDataRows(IEnumerable<DataRow> dtRows, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Columns.Count)
            {
                return;
            }

            var column = Columns[columnIndex];
            if (column.ColumnType != ReportColumnType.Value)
            {
                return;
            }

            ProcessRows((ReportValueColumn)column, dtRows);
        }

        public void ProcessTable(string tableKey, DataTable dt)
        {
            foreach (ReportValueColumn column in Columns.Where(column => column.ColumnType == ReportColumnType.Value))
            {
                if (column.Field.TableKey != String.Empty && column.Field.TableKey != tableKey)
                {
                    continue;
                }

                ProcessRows(column, dt.Select(column.RowsFilter));
            }
        }

        public void ProcessTable(DataTable dt)
        {
            ProcessTable(TableKey, dt);
        }

        private List<object> GetColumnsValues(IEnumerable<ReportRow> rows, List<int> columns)
        {
            if (columns.Count == 0)
            {
                return new List<object>();
            }

            var values = (from row in rows
                          where columns.Contains(row.GroupingIndex)
                          select row.Key).ToList();

            if (values.Count() > MaxBridgeFilterLength)
            {
                return null;
            }

            foreach (var row in rows)
            {
                var childValues = GetColumnsValues(row.Child, columns);
                if (childValues == null)
                {
                    return null;
                }

                values.AddRange(childValues.Where(value => !values.Contains(value)));
            }

            return values.Count <= MaxBridgeFilterLength ? values : null;
        }

        private void SetChildLevels(ReportRow row, int maxLevel)
        {
            if (maxLevel == row.Level)
            {
                row.RemoveNextLevelChild(SumColumnsIndexies());
                return;
            }

            foreach (var child in row.GetChildRows(row.GroupingIndex))
            {
                child.Level = row.Level + 1;
                SetChildLevels(child, maxLevel);
            }
        }

        private void BuildHierarchyGroupings(ReportRow parentRow)
        {
            var groupingIndex = parentRow.GroupingIndex + 1;
            if (groupingIndex >= Groupings.Count)
            {
                return;
            }

            var rows = parentRow.Child;
            foreach (var row in rows)
            {
                BuildHierarchyGroupings(row);
            }

            if (rows.Count == 0 || !Groupings[groupingIndex].IsHierarchical)
            {
                return;
            }

            var keyRows = rows.Select(row => row).ToDictionary(row => row.Key, row => row);
            var initialKeys = new List<object>(keyRows.Keys);
            var hTable = Groupings[groupingIndex].HierarchyTable;

            for (var i = 0; i < keyRows.Count; i++)
            {
                var child = keyRows.ElementAt(i).Value;
                var hRows = hTable.Select(String.Format("id = {0}", child.Key));
                if (hRows.Length == 0)
                {
                    continue;
                }

                var parentKey = hRows[0]["parentid"];
                if (parentKey == DBNull.Value)
                {
                    continue;
                }

                if (!keyRows.ContainsKey(parentKey))
                {
                    var newRow = CreateReportRow(parentKey, groupingIndex);
                    parentRow.AddChild(newRow);
                    keyRows.Add(newRow.Key, newRow);
                }

                keyRows[parentKey].AddChild(child);
            }

            if (Groupings[groupingIndex].SkipHierarchyLevels)
            {
                foreach (var row in keyRows.Where(row => !initialKeys.Contains(row.Key)))
                {
                    row.Value.Parent.RemoveChild(row.Value);

                    foreach (var child in row.Value.Child)
                    {
                        row.Value.Parent.AddChild(child);
                    }
                }
            }
            else if (Groupings[groupingIndex].HideHierarchyLevels)
            {
                foreach (var row in keyRows.Where(row => !initialKeys.Contains(row.Key)))
                {
                    row.Value.Hide = true;
                }
            }

            var maxLevel = Groupings[groupingIndex].MaxHierarchyLevel;
            foreach (var row in parentRow.Child)
            {
                SetChildLevels(row, maxLevel);
            }
        }

        private Dictionary<string, DataTable> GetBridges()
        {
            var bridges = new Dictionary<string, DataTable>();
            var bridgesColumns = new Dictionary<string, List<int>>();
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);

            // Получаем классификаторы со списком колонок, в которых они используются
            for (var n = 0; n < Groupings.Count; n++)
            {
                var column = Groupings[n];
                if (column.LookupFields == null)
                {
                    continue;
                }

                foreach (var lookupField in column.LookupFields)
                {
                    for (var i = 0; i < lookupField.Path.Count; i++)
                    {
                        // Список колонок заполняется только для классификаторов, которые используются на первом
                        // шаге в пути поиска, если классификатор используется на последующих шагах, то список колонок уничтожается
                        var tableField = lookupField.Path[i];
                        if (bridgesColumns.ContainsKey(tableField.TableKey))
                        {
                            if (i == 0)
                            {
                                var list = bridgesColumns[tableField.TableKey];
                                if (list != null && !list.Contains(n))
                                {
                                    list.Add(n);
                                }
                            }
                            else
                            {
                                bridgesColumns[tableField.TableKey] = null;
                            }
                        }
                        else
                        {
                            bridgesColumns.Add(tableField.TableKey, i == 0 ? new List<int> {n} : null);
                        }
                    }
                }
            }

            // Получаем таблицы классификаторов 
            foreach (var bridge in bridgesColumns)
            {
                // если список колонок отсутствует, то получаем таблицу целиком
                if (bridge.Value == null)
                {
                    bridges.Add(bridge.Key, dbHelper.GetEntityData(bridge.Key));
                }
                else
                {
                    // получаем список значений колонок,
                    var columnsValues = GetColumnsValues(Root.Child, bridge.Value);
                    if (columnsValues == null) // если список значений отсутствует, то получаем таблицу целиком
                    {
                        bridges.Add(bridge.Key, dbHelper.GetEntityData(bridge.Key));
                    }
                    else
                    {
                        if (columnsValues.Count > 0)
                        {
                            var filter = String.Join(",", columnsValues.Select(Convert.ToString).ToArray());
                            filter = String.Format("id in ({0})", filter);
                            bridges.Add(bridge.Key, dbHelper.GetEntityData(bridge.Key, filter));
                        }
                    }
                }
            }

            return bridges;
        }

        private void SetRowsLookupFields(IEnumerable<ReportRow> rows)
        {
            var bridges = GetBridges();

            foreach (var row in rows)
            {
                var grouping = Groupings[row.GroupingIndex];
                if (grouping.LookupFields == null)
                {
                    continue;
                }

                foreach (var lookupField in grouping.LookupFields)
                {
                    row.LookupFields[lookupField.Index] = lookupField.GetValue(row.Key, bridges);
                }
            }
        }

        private DataTable GetReportTable()
        {
            var dt = new DataTable();

            foreach (var column in Columns)
            {
                dt.Columns.Add(String.Format("Column{0}", column.Index), column.Type);
            }

            dt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            return dt;
        }

        public virtual void InsertNumColumn(DataTable dt)
        {
            if (!AddNumColumn)
            {
                return;
            }

            const int index = 0;
            var column = dt.Columns.Add("Num", typeof(int));
            column.SetOrdinal(index);
            var count = AddTotalRow ? dt.Rows.Count - 1 : dt.Rows.Count;
            for (var i = 0; i < count; i++)
            {
                dt.Rows[i][index] = i + 1;
            }
        }

        private void DivideValues(IEnumerable<ReportRow> rows)
        {
            var dividers = new Dictionary<int, Decimal>();

            foreach (var column in Columns)
            {
                decimal divider = 1;

                switch (column.ColumnType)
                {
                    case ReportColumnType.Value:
                        divider = ((ReportValueColumn) column).Divider;
                        break;
                    case ReportColumnType.Calculated:
                        divider = ((ReportCalcColumn) column).Divider;
                        break;
                }

                if (divider != 1)
                {
                    dividers.Add(column.Index, divider);
                }
            }

            if (dividers.Count == 0)
            {
                return;
            }

            foreach (var row in rows)
            {
                foreach (var divider in dividers)
                {
                    var index = divider.Key;
                    var value = row.Values.Length > index ? row.Values[index] : null;
                    if (value == null)
                    {
                        continue;
                    }

                    row.Values[index] = ReportDataServer.GetDecimal(value) / divider.Value;
                }
            }
        }

        public object[] AddValues(object[] v1, object[] v2, IEnumerable<int> indexies)
        {
            foreach (var i in indexies.Where(i => v1[i] != null || v2[i] != null))
            {
                v1[i] = ReportDataServer.GetDecimal(v1[i]) + ReportDataServer.GetDecimal(v2[i]);
            }

            return v1;
        }

        private RowViewParams GetViewParams(ReportRow row)
        {
            if (row.GroupingIndex < 0 || row.GroupingIndex >= Groupings.Count)
            {
                return null;
            }

            var grouping = Groupings[row.GroupingIndex];
            if (row.Level < 0 || grouping.ViewParams.Count == 0)
            {
                return null;
            }

            if (grouping.ViewParams.ContainsKey(row.Level))
            {
                return grouping.ViewParams[row.Level];
            }

            return grouping.ViewParams.FirstOrDefault(e => e.Value.ForAllLevels).Value;
        }

        private object[] SumValues(IEnumerable<ReportRow> rows, IEnumerable<int> indexies)
        {
            var sumValues = new object[Columns.Count];

            foreach (var row in rows)
            {
                var values = SumValues(row.Child, indexies);
                row.Values = AddValues(row.Values, values, indexies);

                var vp = GetViewParams(row);
                var breakSumming = row.BreakSumming || vp == null ? row.BreakSumming : vp.BreakSumming;

                if (!breakSumming)
                {
                    sumValues = AddValues(sumValues, row.Values, indexies);
                }
            }

            return sumValues;
        }

        public IEnumerable<int> SumColumnsIndexies()
        {
            return from column in Columns where column.SumNestedRows select column.Index;
        }

        private void InsertRowToReport(ReportRow row, DataTable dt, RowViewParams vp)
        {
            if (row.Hide)
            {
                return;
            }

            var dtRow = dt.Rows.Add();

            foreach (var column in Columns)
            {
                dtRow[column.Index] = column.GetValue(row) ?? DBNull.Value;
            }

            if (row.Style != null)
            {
                dtRow[ReportDataServer.STYLE] = row.Style;
            }
            else if (vp != null)
            {
                dtRow[ReportDataServer.STYLE] = vp.Style;
            }
        }

        private void InsertRowsToReport(IEnumerable<ReportRow> rows, DataTable dt)
        {
            rows = rows.OrderBy(row => row, comparer);

            foreach (var row in rows)
            {
                if (row.GroupingIndex == NoRef) // строка итого
                {
                    InsertRowsToReport(row.Child, dt);
                    if (AddTotalRow)
                    {
                        InsertRowToReport(row, dt, null);
                    }
                    continue;
                }

                var groupingIndex = row.GroupingIndex;
                var viewParams = GetViewParams(row);
                var filter = viewParams != null ? viewParams.Filter ?? RowFilter : RowFilter;
                if (!row.Fixed && filter != null && !filter(row, Columns))
                {
                    continue;
                }

                var nextLevelChild = row.Child.Where(item => item.GroupingIndex > groupingIndex);
                var sameLevelChild = row.Child.Where(item => item.GroupingIndex == groupingIndex);
                var showOrder = viewParams != null ? viewParams.ShowOrder : RowViewParams.ShowType.BeforeChild;

                switch (showOrder)
                {
                    case RowViewParams.ShowType.BeforeChild:
                        InsertRowToReport(row, dt, viewParams);
                        InsertRowsToReport(nextLevelChild, dt);
                        InsertRowsToReport(sameLevelChild, dt);
                        break;
                    case RowViewParams.ShowType.AfterChild:
                        InsertRowsToReport(sameLevelChild, dt);
                        InsertRowToReport(row, dt, viewParams);
                        InsertRowsToReport(nextLevelChild, dt);
                        break;
                    case RowViewParams.ShowType.SkipBeforeChild:
                        InsertRowsToReport(nextLevelChild, dt);
                        InsertRowsToReport(sameLevelChild, dt);
                        break;
                    case RowViewParams.ShowType.SkipAfterChild:
                        InsertRowsToReport(sameLevelChild, dt);
                        InsertRowsToReport(nextLevelChild, dt);
                        break;
                }
            }
        }
        
        private void ControlRows(List<ReportRow> rows)
        {
            for (var i = 0; i < Groupings.Count; i++)
            {
                var grouping = Groupings[i];
                if (grouping.Function != null)
                {
                    grouping.Function(rows, i);
                }
            }
        }
        
        public DataTable GetReportData()
        {
            InsertFixedRows(Root);
            BuildHierarchyGroupings(Root);
            var rows = Root.GetNestedRows();
            SetRowsLookupFields(rows);
            ControlRows(rows);
            DivideValues(rows);
            Root.Values = SumValues(Root.Child, SumColumnsIndexies());
            var dt = GetReportTable();
            comparer = new ReportRowComparer(Groupings);
            InsertRowsToReport(new List<ReportRow> { Root }, dt);
            InsertNumColumn(dt);
            return dt;
        }
    }

    public class Functions
    {
        public static object Divide(object value, object divider, bool inPerc = true)
        {
            var div = ReportDataServer.GetDecimal(divider);
            if (value != null && div != 0)
            {
                var res = ReportDataServer.GetDecimal(value)/div;
                return inPerc ? res * 100 : res;
            }

            return null;
        }

        public static object Sum(IEnumerable<object> values)
        {
            object sum = null;

            foreach (var value in values.Where(value => value != null))
            {
                sum = ReportDataServer.GetDecimal(sum) + ReportDataServer.GetDecimal(value);
            }

            return sum;
        }

        public static bool IsNotNullRow(ReportRow row, List<ReportColumn> columns)
        {
            foreach (var column in columns.Where(column => column.ColumnType == ReportColumnType.Value))
            {
                if (ReportDataServer.GetDecimal(row.Values[column.Index]) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotUnknownKey(ReportRow row, List<ReportColumn> columns)
        {
            return row.Key != DBNull.Value && !row.Key.Equals(-1);
        }

        public static bool HideRow(ReportRow row, List<ReportColumn> columns)
        {
            return false;
        }
    }

    public class AteGrouping : ReportGrouping
    {
        public const int typeSubject = 3;  // тип территории - субъект РФ
        public const int typeMR = 4;       // тип территории - муниципальный район
        public const int typeCity = 7;     // тип территории - городской округ
        public const int typeTownSettle = 5;    // тип территории - городское поселение
        public const int typeVillageSettle = 6; // тип территории - сельское поселение
        public string MRBudgetCaption = "Бюджет района";
        public string ConsBudgetCaption = "ИТОГО";
        public bool FixedAddedRows = false;
        public bool HideConsBudjetRow { get; set; }
        public string AteMainId { get; private set; }
        public int LastLevel { get { return ViewParams.Max(vp => vp.Key); } }
        public string SubjectId { get; private set; }
        public string ConsBudjetId { get; private set; }

        public string GetRegionsId(bool withoutSettlement = false)
        {
            var reportHelper = new ReportMonthMethods(ConvertorSchemeLink.scheme);
            var tblAteKey = b_Regions_Bridge.InternalKey;
            if (!withoutSettlement)
            {
                return reportHelper.GetNestedIDByField(tblAteKey, b_Regions_Bridge.ID, ConsBudjetId);
            }

            var twoLevelId = reportHelper.GetIDByField(tblAteKey, b_Regions_Bridge.ParentID, ConsBudjetId);
            var threeLevelId = reportHelper.GetIDByField(tblAteKey, b_Regions_Bridge.ParentID, twoLevelId);
            var id = new[] {AteMainId, twoLevelId, threeLevelId};
            return String.Join(",", id);
        }

        // выносит запись субъекта на уровень вверх
        public void MoveSubjectRow (List<ReportRow> rows, int groupingIndex)
        {
            const string tblAteKey = b_Regions_Bridge.InternalKey;
            var nameField = new TableField(tblAteKey, b_Regions_Bridge.Name);
            var ateNameIndex = GetLookupFieldIndex(nameField);
            var codeField = new TableField(tblAteKey, b_Regions_Bridge.Code);
            var codeFieldIndex = GetLookupFieldIndex(codeField);

            if (SubjectId != ReportConsts.UndefinedKey)
            {
                foreach (var row in rows.Where(row => row.GroupingIndex == groupingIndex
                                                        && row.Level == 1
                                                        && row.Key.Equals(Convert.ToInt32(SubjectId))))
                {

                    row.Parent.LookupFields[codeFieldIndex] = Convert.ToInt32(row.LookupFields[codeFieldIndex]) - 1;
                    // при сортировке запись субъекта окажется после записи КБС
                    row.Parent.Parent.AddChild(row);
                }
            }

            if (ConsBudjetId != ReportConsts.UndefinedKey)
            {
                foreach (var row in rows.Where(row => row.GroupingIndex == groupingIndex
                                                        && row.Key.Equals(Convert.ToInt32(ConsBudjetId))))
                {
                    row.Hide = HideConsBudjetRow;

                    if (ConsBudgetCaption != null)
                    {
                        row.LookupFields[ateNameIndex] = ConsBudgetCaption;
                    }
                }
            }
        }

        // добавляет у каждого МР бюджет района
        public void InsertMRBudjet(List<ReportRow> rows, int groupingIndex)
        {
            const string tblAteKey = b_Regions_Bridge.InternalKey;
            var typeField = new TableField(tblAteKey, b_Regions_Bridge.RefTerrType);
            var ateTypeIndex = GetLookupFieldIndex(typeField);
            var groupingRows = rows.Where(row => row.GroupingIndex == groupingIndex && row.LookupFields[ateTypeIndex].Equals(typeMR));
            if (groupingRows.Count() <= 0) return;

            var level = groupingRows.First().Level + 1;
            if (MaxHierarchyLevel >= 0 && MaxHierarchyLevel < level) return;

            var newRows = InsertMainChild(groupingRows, MRBudgetCaption, level);
            rows.AddRange(newRows);
        }

        // добавляет у каждой АТЕ в списке дочернюю строку, которой подчиняет другие дочерние строки другого уровня
        public List<ReportRow> InsertMainChild(IEnumerable<ReportRow> rows, string name, int level, bool copyValues = true)
        {
            const string tblAteKey = b_Regions_Bridge.InternalKey;
            var nameField = new TableField(tblAteKey, b_Regions_Bridge.Name);
            var ateNameIndex = GetLookupFieldIndex(nameField);
            var newRows = new List<ReportRow>();

            foreach (var row in rows)
            {
                if (row.ContainsChild(row.Key, row.GroupingIndex))
                {
                    continue;
                }

                var newRow = new ReportRow
                {
                    Key = row.Key, // новая дочерняя строка будет иметь ключ родителя, что гарантирует уникальность среди дочерних строк
                    GroupingIndex = row.GroupingIndex,
                    Fixed = FixedAddedRows,
                    LookupFields = new object[row.LookupFields.Length],
                    Values = new object[row.Values.Length]
                };

                row.AddChild(newRow);
                newRow.Level = level;

                if (copyValues)
                {
                    row.Values.CopyTo(newRow.Values, 0);
                    row.Values = new object[row.Values.Length];
                }

                row.LookupFields.CopyTo(newRow.LookupFields, 0);

                if (name != null)
                {
                    newRow.LookupFields[ateNameIndex] = name;
                }

                foreach (var child in row.Child.Where(item => item.GroupingIndex != row.GroupingIndex))
                {
                    newRow.AddChild(child);
                }

                newRows.Add(newRow);
            }

            return newRows;
        }

        public static DataRow GetSubjectRow(DataTable tblAte)
        {
            var filter = String.Format("{0} = {1}", b_Regions_Bridge.RefTerrType, typeSubject);
            var rows = tblAte.Select(filter);
            return rows.Length > 0 ? rows[0] : null;
        }

        public AteGrouping(int style, bool hideSettlement = false)
        {
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            const string tblAteKey = b_Regions_Bridge.InternalKey;
            const string tblTerTypeKey = fx_FX_TerritorialPartitionType.InternalKey;
            HierarchyTable = dbHelper.GetEntityData(tblAteKey);

            // определяем верхние элементы иерархии
            var row = GetSubjectRow(HierarchyTable);
            SubjectId = row != null ? Convert.ToString(row[b_Regions_Bridge.ID]) : ReportConsts.UndefinedKey;
            ConsBudjetId = row != null ? Convert.ToString(row[b_Regions_Bridge.ParentID]) : ReportConsts.UndefinedKey;
            AteMainId = String.Format("{0},{1}", SubjectId, ConsBudjetId);

            // порядковый номер
            AddLookupField(tblAteKey, b_Regions_Bridge.CodeLine);
            // полное наименование типа АТЕ
            var terTypeFullName = AddLookupField(tblAteKey, b_Regions_Bridge.RefTerrType);
            terTypeFullName.AddField(tblTerTypeKey, fx_FX_TerritorialPartitionType.FullName);
            // наименование типа АТЕ
            var terTypeName = AddLookupField(tblAteKey, b_Regions_Bridge.RefTerrType);
            terTypeName.AddField(tblTerTypeKey, fx_FX_TerritorialPartitionType.Name);
            // наименование АТЕ
            AddLookupField(tblAteKey, b_Regions_Bridge.Name);
            // код
            var codeField = AddLookupField(tblAteKey, b_Regions_Bridge.Code);
            codeField.Type = typeof (int);
            // id типа АТЕ
            AddLookupField(tblAteKey, b_Regions_Bridge.RefTerrType);
            // id АТЕ
            AddLookupField(tblAteKey, b_Regions_Bridge.ID);

            if (hideSettlement)
                MaxHierarchyLevel = 2;
            ViewParams = new AteViewParams(style);
            HideConsBudjetRow = false;
            Function = new Control(MoveSubjectRow) + InsertMRBudjet;
        }
    }

    public class AteViewParams : Dictionary<int, RowViewParams>
    {
        private Dictionary<TableField, int> GetSortOrder()
        {
            return new Dictionary<TableField, int>
            {
                {new TableField(b_Regions_Bridge.InternalKey, b_Regions_Bridge.Code), RowViewParams.SortAsc},
                {new TableField(b_Regions_Bridge.InternalKey, b_Regions_Bridge.ID), RowViewParams.SortAsc}
            };
        }

        public AteViewParams(int style)
        {
            // уровень конс. бюджета
            // уровень области (с учетом выноса записи области на уровень вверх)
            Add(0,
                new RowViewParams
                {
                    Style = style + 4,
                    ShowOrder = RowViewParams.ShowType.AfterChild,
                    SortOrder = GetSortOrder()
                }
            );

            Add(1, new RowViewParams { Style = style, SortOrder = GetSortOrder() }); // уровень области + 1
            Add(2, new RowViewParams { Style = style + 1, SortOrder = GetSortOrder() }); // уровень районов и городских округов
            Add(3, new RowViewParams { Style = style + 2, SortOrder = GetSortOrder()}); // уровень суммарных бюджетов поселений
            Add(4, new RowViewParams { Style = style + 3, SortOrder = GetSortOrder()}); // уровень поселений
        }
    }

    public class AteOutMasks : SortedDictionary<int, ColumnOutMask>
    {
        public AteOutMasks(params TableField[] outFields)
        {
            const string regionsKey = b_Regions_Bridge.InternalKey;
            const string terTypeKey = fx_FX_TerritorialPartitionType.InternalKey;
            var name = new TableField(regionsKey, b_Regions_Bridge.Name);
            
            var allFields = new []
                {
                    new TableField(regionsKey, b_Regions_Bridge.CodeLine), 
                    new TableField(terTypeKey, fx_FX_TerritorialPartitionType.FullName),
                    new TableField(regionsKey, b_Regions_Bridge.Name) 
                };

            var fields = outFields.Length > 0 ? outFields : allFields;

            if (fields.Contains(name, new TableFieldComparer()))
            {
                Add(0, new ColumnOutMask(name)); // уровень конс. бюджета, уровень области (с учетом выноса записи области на уровень вверх)
                Add(1, new ColumnOutMask(name)); // уровень области + 1
                Add(3, new ColumnOutMask(name)); // уровень бюджетов поселений
            }

            Add(2, new ColumnOutMask(fields)); // уровень районов и городских округов
            Add(4, new ColumnOutMask(fields)); // уровень поселений
        }
    }
}
