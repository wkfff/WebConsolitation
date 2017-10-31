using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS011Filler : CommonUFNSReportFiller
    {
        private const int YearLevel = 3;
        private const int HalfYearLevel = 4;
        private const int YearsInPattern = 3;
        private const int FirstDataColumn = 3;

        private int GetDataColumnStyle(int halfsCount, int yearIndex, int halfIndex)
        {
            var y = yearIndex < YearsInPattern ? yearIndex : YearsInPattern - 1;
            var firstYearColumn = y == 0
                ? FirstDataColumn
                : FirstDataColumn + y * halfsCount * 2 - halfsCount;
            var style = y == 0
                ? firstYearColumn + halfIndex
                : firstYearColumn + halfIndex * 2;
            return style;
        }

        private DataTable AddColumns(DataTable dt, IList<int> years, IList<int> halfYear)
        {
            var dtResult = dt.Clone();
            dtResult.Rows.Add(DBNull.Value, 0, YearLevel, false, false, true);
            dtResult.Rows.Add(DBNull.Value, 1, YearLevel, false, false, true);
            dtResult.Rows.Add(DBNull.Value, 2, YearLevel, false, false, true);

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row = dtResult.Rows.Add();
                row.ItemArray = dt.Rows[i].ItemArray;

                if (Convert.ToBoolean(row[ReportDataServer.MERGED]))
                {
                    continue;
                }

                var rub = row[ReportDataServer.IsRUB];
                row[ReportDataServer.MERGED] = true;
                row[ReportDataServer.IsRUB] = false;

                for (var y = 0; y < years.Count; y++)
                {
                    dtResult.Rows.Add(String.Format("{0} год", years[y]), 3, YearLevel, true, false, false);

                    for (var n = 0; n < halfYear.Count; n++)
                    {
                        var style = GetDataColumnStyle(halfYear.Count, y, n);
                        dtResult.Rows.Add(DBNull.Value, style, HalfYearLevel, false, rub, true);
                        if (y > 0)
                        {
                            dtResult.Rows.Add(DBNull.Value, style + 1, HalfYearLevel, false, false, false);
                        }
                    }
                }
            }

            return dtResult;
        }

        public void RemoveHalfYearColumns(HSSFWorkbook wb, HSSFSheet sheet, IList<int> halfYear)
        {
            for (var i = YearsInPattern - 1; i >= 0; i--)
            {
                var firstYearColumn = i == 0 ? FirstDataColumn : FirstDataColumn + i * 4 - 2;
                var halfs = new[] {1, 0};
                foreach (var half in halfs.Where(e => !halfYear.Contains(e)))
                {
                    var column = i == 0 ? firstYearColumn + half : firstYearColumn + half*2;
                    if (i > 0)
                    {
                        NPOIHelper.DeleteColumn(wb, sheet, column + 1);
                    }
                    NPOIHelper.DeleteColumn(wb, sheet, column);
                }
            }
        }

        private int GetSheetFirstRowIndex(DataTable dtColumns, int index)
        {
            var count = -1;

            for (var i = 0; i < dtColumns.Rows.Count; i++)
            {
                var row = dtColumns.Rows[i];
                if (!Convert.ToBoolean(row[ReportDataServer.MERGED]))
                {
                    count++;
                }

                if (count == index)
                {
                    return i;
                }
            }

            return -1;
        }

        private IEnumerable<DataRow> GetParentRows(DataTable dtColumn, int index)
        {
            var rows = new List<DataRow>();
            var level = Convert.ToInt32(dtColumn.Rows[index][ReportDataServer.LEVEL]);

            while (index > 0 && level > 0)
            {
                index--;
                var newLevel = Convert.ToInt32(dtColumn.Rows[index][ReportDataServer.LEVEL]);
                if (newLevel < level)
                {
                    level = newLevel;
                    rows.Insert(0, dtColumn.Rows[index]);
                }
            }

            return rows;
        }

        private DataTable GetSheetColumns(DataTable dtColumns, int sheetNumber, int marksInSheet)
        {
            var dt = dtColumns.Clone();
            var index = GetSheetFirstRowIndex(dtColumns, sheetNumber * marksInSheet);
            if (index == -1)
            {
                return dt;
            }

            var parentRows = GetParentRows(dtColumns, index);
            foreach (var parentRow in parentRows)
            {
                dt.ImportRow(parentRow);
            }

            var count = 0;

            while (index < dtColumns.Rows.Count && count < marksInSheet)
            {
                var row = dtColumns.Rows[index++];
                dt.ImportRow(row);
                if (!Convert.ToBoolean(row[ReportDataServer.MERGED]))
                {
                    count++;
                }
            }

            return dt;
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var paramHalfYear = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.HALF_YEAR));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var halfYear = ReportDataServer.ConvertToIntList(paramHalfYear);
            var columnsInMark = years.Count*halfYear.Count*2 - halfYear.Count;
            var marksInSheet = (NPOIHelper.MaxColumnNum + 1 - FirstDataColumn)/columnsInMark;
            var sumColumnsInSheet = marksInSheet * years.Count * halfYear.Count;
            var marksCount = tableList[1].Rows.Cast<DataRow>().Count(row => !Convert.ToBoolean(row[ReportDataServer.MERGED]));
            var sheetCount = marksCount % marksInSheet > 0
                                 ? marksCount/marksInSheet + 1
                                 : marksCount/marksInSheet;
            RemoveHalfYearColumns(wb, wb.GetSheetAt(0), halfYear);

            for (var n = 1; n < sheetCount; n++)
            {
                NPOIHelper.CopySheet(wb, 0);
            }
            
            for (var n = 0; n < sheetCount; n++)
            {
                var dtSheetColumns = GetSheetColumns(tableList[1], n, marksInSheet);
                var dtHeader = AddColumns(dtSheetColumns, years, halfYear);
                var columns = GetDataColumnsIndexies(dtHeader);
                var firstRepColumn = FirstDataColumn + n * sumColumnsInSheet;
                var repColumnsCount = columns.Count - FirstDataColumn;
                var tableColumns = ReportDataServer.GetColumnsList(0, FirstDataColumn);
                tableColumns.AddRange(ReportDataServer.GetColumnsList(firstRepColumn, repColumnsCount));

                var sheetParams = new SheetParams
                {
                    TableIndex = 0,
                    SheetIndex = n,
                    FirstRow = 10 - 1,
                    StyleRowsCount = 5,
                    StyleColumnsCount = 13,
                    HeaderRowsCount = 6,
                    FirstTotalColumn = FirstDataColumn,
                    TotalRowsCount = 3,
                    ExistHeaderNumColumn = true,
                    RemoveUnusedColumns = false,
                    Columns = columns,
                    SumColumns = GetSumColumnsIndexies(dtHeader),
                    TableColumns = tableColumns
                };

                AddColumns(wb, sheetParams, dtHeader);
                FillSheet(wb, tableList, sheetParams);
            }
        }
    }
}
