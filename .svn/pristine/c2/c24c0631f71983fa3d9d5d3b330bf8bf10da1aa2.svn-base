using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        public const string STYLE = "Style";
        public const string NAME = "Name";
        public const string NOTE = "Note";
        public const string LEVEL = "Level";
        public const string MERGED = "Merged";
        public const string IsRUB = "IsRub";
        public const string IsDATA = "IsData";
        private const string SUM = "Sum";
        private const string SUM0 = "Sum0";
        private const string SUM1 = "Sum1";

        public static int AddColumnToReport(DataTable tblReport, Type type, string name, int count = 1)
        {
            var index = tblReport.Columns.Count;
            for (var i = 0; i < count; i++)
            {
                var columnName = count > 1 ? String.Format("{0}{1}", name, i) : name;
                tblReport.Columns.Add(columnName, type);
            }

            if (count < 0)
            {
                index = -1;
            }

            return index;
        }

        private void DivideNotNullColumn(DataTable tbl, int columnIndex, string divider)
        {
            foreach (DataRow row in tbl.Rows)
            {
                if (row[columnIndex] != DBNull.Value)
                {
                    row[columnIndex] = DivideSumValue(row[columnIndex], divider);
                }
            }
        }

        private void DivideSum(DataTable repTable, int firstColumn, int columnCount, string divider)
        {
            for (var i = 0; i < columnCount; i++)
            {
                DivideNotNullColumn(repTable, firstColumn + i, divider);
            }
        }

        private static Object GetSumFieldValue(DataTable dt, string field, string filter)
        {
            var rows = dt.Select(filter);
            if (rows.Length == 0 || rows.All(r => r[field] == DBNull.Value))
            {
                return DBNull.Value;
            }

            return rows.Sum(r => GetDecimal(r[field]));
        }

        private static Object GetSumFieldValue(DataTable dt, int field, string filter)
        {
            return GetSumFieldValue(dt, dt.Columns[field].ColumnName, filter);
        }

        private static Object GetCalculatedSumFieldValue(DataTable dt, string field, string filter, string codes)
        {
            if (codes == GetAbsCodes(codes))
            {
                return GetSumFieldValue(dt, field,  String.Format(filter, codes));
            }

            var intCodes = ConvertToIntList(codes);
            var positiveCodes = intCodes.Where(code => code >= 0);
            var negativeCodes = intCodes.Where(code => code < 0);
            var positiveFilter = ConvertToString(positiveCodes);
            var negativeFilter = GetAbsCodes(ConvertToString(negativeCodes));
            var x = positiveFilter.Length > 0
                        ? GetSumFieldValue(dt, field, String.Format(filter, positiveFilter))
                        : DBNull.Value;
            var y = negativeFilter.Length > 0
                        ? GetSumFieldValue(dt, field, String.Format(filter, negativeFilter))
                        : DBNull.Value;
            return GetNotNullSumDifference(x, y);
        }

        public static List<int> GetColumnsList(int startColumn, int count)
        {
            var columns = new List<int>();

            for (var i = 0; i < count; i++)
            {
                columns.Add(startColumn + i);
            }

            return columns;
        }

        public static DataTable FilterNotNullData(DataTable dt, IEnumerable<int> sumColumns)
        {
            var dtResult = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                var foundNotNull = sumColumns.Any(sumColumn => GetDecimal(row[sumColumn]) != 0);

                if (foundNotNull)
                {
                    dtResult.ImportRow(row);
                }
            }

            return dtResult;
        }

        public static DataTable FilterNotExistData(DataTable dt, IEnumerable<int> sumColumns)
        {
            var dtResult = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                var foundNotNull = sumColumns.Any(sumColumn => row[sumColumn] != DBNull.Value);

                if (foundNotNull)
                {
                    dtResult.ImportRow(row);
                }
            }

            return dtResult;
        }

        private static object GetNotNullSumDifference(object sum1, object sum2)
        {
            if (sum1 != DBNull.Value || sum2 != DBNull.Value)
            {
                return GetDecimal(sum1) - GetDecimal(sum2);
            }

            return DBNull.Value;
        }

        public static bool IsEmptyColumn(DataTable dt, int columnIndex)
        {
            return dt.Rows.Cast<DataRow>().All(row => row[columnIndex] == DBNull.Value);
        }

        public static bool IsEmptyColumns(DataTable dt, IEnumerable<int> columns)
        {
            return columns.All(column => IsEmptyColumn(dt, column));
        }

        public static bool IsNullColumn(DataTable dt, int columnIndex)
        {
            return dt.Rows.Cast<DataRow>().All(row => GetDecimal(row[columnIndex]) == 0);
        }

        public static bool IsNullColumns(DataTable dt, IEnumerable<int> columns)
        {
            return columns.All(column => IsNullColumn(dt, column));
        }

        public static void RemoveEmptyColumns(DataTable dt, bool notRemoveAll = false)
        {
            var columns = (from DataColumn column in dt.Columns
                           where dt.Rows.Cast<DataRow>().All(row => row[column.Ordinal] == DBNull.Value)
                           select column.ColumnName).ToList();

            if (!(columns.Count == dt.Columns.Count && notRemoveAll))
            {
                foreach (var column in columns)
                {
                    dt.Columns.Remove(column);
                }
            }
        }

        public static DataTable GetTableCopy(DataTable dt, IEnumerable<int> columns)
        {
            var result = dt.Clone();
            
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                if (!columns.Contains(i))
                {
                    result.Columns.Remove(dt.Columns[i].ColumnName);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                result.Rows.Add(row.ItemArray.Where((e, i) => columns.Contains(i)).ToArray());
            }

            return result;
        }

        public static DataTable RemoveNullColumns(DataTable dt, IEnumerable<int> columns)
        {
            var delColumns = (from column in columns
                              where dt.Rows.Cast<DataRow>().All(row => GetDecimal(row[column]) == 0)
                              select column).ToList();

            var restColumns = (from DataColumn column in dt.Columns
                               where !delColumns.Contains(column.Ordinal)
                               select column.Ordinal).ToList();

            return GetTableCopy(dt, restColumns);
        }

        public static bool IsRubRef(object refUnits)
        {
            const string rubRefUnitsId = "47"; // показатель исчисляется в рублях
            return Convert.ToString(refUnits) == rubRefUnitsId;
        }
    }
}
