using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS016Filler : CommonUFNSReportFiller
    {
        private const int YearLevel = 3;
        private const int FirstDataColumn = 3;
        private const int YearsInPattern = 3;

        private DataTable GetColumnsTable(List<int> years, DataTable dtMarks)
        {
            const string title = "в том числе:";

            var dt = new DataTable();
            dt.Columns.Add(ReportDataServer.NAME, typeof(string));
            dt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            dt.Columns.Add(ReportDataServer.LEVEL, typeof(int));
            dt.Columns.Add(ReportDataServer.MERGED, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsRUB, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsDATA, typeof(bool));
            for (var i = 0; i < FirstDataColumn; i++)
            {
                dt.Rows.Add(DBNull.Value, i, YearLevel, false, false, true);
            }

            foreach (DataRow row in dtMarks.Rows)
            {
                var level = Convert.ToInt32(row[ReportDataServer.LEVEL]);
                var name = Convert.ToString(row[ReportDataServer.NAME]);
                if (level == 2 && name.StartsWith(title) && name.Length > title.Length)
                {
                    name = name.Substring(title.Length).Trim();
                }

                dt.Rows.Add(name, FirstDataColumn, level, true, false, false);
                if (level == 0)
                {
                    continue;
                }

                var hasChild = Convert.ToBoolean(row[ReportDataServer.MERGED]);
                var isRub = Convert.ToBoolean(row[ReportDataServer.IsRUB]);

                for (var i = 0; i < years.Count; i++)
                {
                    var yearIndex = i < YearsInPattern ? i : YearsInPattern - 1;
                    var style = yearIndex == 0 ? FirstDataColumn : FirstDataColumn + yearIndex * 2 - 1;
                    dt.Rows.Add(String.Format("{0} год", years[i]), style, YearLevel, false, isRub, true);
                    if (yearIndex > 0)
                    {
                        dt.Rows.Add(DBNull.Value, style + 1, YearLevel, false, false, false);
                    }
                }

                if (hasChild && level == 1)
                {
                    dt.Rows.Add(title, FirstDataColumn, level, true, false, false);
                }
            }

            return dt;
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var dtColumns = GetColumnsTable(years, tableList[1]);

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 10 - 1,
                StyleRowsCount = 5,
                StyleColumnsCount = 8,
                HeaderRowsCount = 5,
                FirstTotalColumn = FirstDataColumn,
                TotalRowsCount = 3,
                ExistHeaderNumColumn = true,
                RemoveUnusedColumns = false,
                Columns = GetDataColumnsIndexies(dtColumns),
                SumColumns = GetSumColumnsIndexies(dtColumns)
            };

            AddColumns(wb, sheetParams, dtColumns);
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
