using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0021.ReportFillers
{
    class ReportMOFO0021_001Filler : CommonUFNSReportFiller
    {
        private const int MainLevel = 0;
        private const int FirstDataColumn = 1;
        private const int ColumnsInMark = 5;

        private DataTable AddColumns(DataTable dtColumns)
        {
            var dt = new DataTable();
            dt.Columns.Add(ReportDataServer.NAME, typeof(string));
            dt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            dt.Columns.Add(ReportDataServer.LEVEL, typeof(int));
            dt.Columns.Add(ReportDataServer.MERGED, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsRUB, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsDATA, typeof(bool));

            for (var i = 0; i < FirstDataColumn; i++)
            {
                dt.Rows.Add(DBNull.Value, i, MainLevel + 1, false, false, true);
            }

            foreach (DataRow row in dtColumns.Rows)
            {
                dt.Rows.Add(row[0], FirstDataColumn, MainLevel, true, false, false);

                for (var i = 0; i < ColumnsInMark; i++)
                {
                    dt.Rows.Add(DBNull.Value, FirstDataColumn + i, MainLevel + 1, false, true, true);
                }
            }

            return dt;
        }

        private DataTable GetSheetColumns(DataTable dtColumns, int sheetNumber, int marksInSheet)
        {
            var dt = dtColumns.Clone();
            var firstIndex = sheetNumber*marksInSheet;
            
            for (var i = firstIndex; i < Math.Min(firstIndex + marksInSheet, dtColumns.Rows.Count); i++)
            {
                dt.ImportRow(dtColumns.Rows[i]);
            }

            return dt;
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var firstSheet = wb.GetSheetAt(0);
            var year = Convert.ToInt32(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEAR));
            SetParamValue(firstSheet, "YEAR-1", year - 1);
            SetParamValue(firstSheet, "YEAR+1", year + 1);
            SetParamValue(firstSheet, "YEAR+2", year + 2);
            SetParamValue(firstSheet, "YEAR+3", year + 3);
            SetParamValue(firstSheet, "YEAR", year);

            var marksCount = tableList[1].Rows.Count;
            var delColumns = new List<int>();

            for (var i = 0; i < marksCount; i++)
            {
                delColumns.Add(FirstDataColumn + i * (ColumnsInMark + 1)); // колонки "План"
            }

            // если в колонках "План" есть данные, то убираем колонки "Факт"
            if (ReportDataServer.IsNullColumns(tableList[0], delColumns))
            {
                NPOIHelper.DeleteColumn(wb, firstSheet, FirstDataColumn);
            }
            else // или убираем колонки "План"
            {
                delColumns = delColumns.Select(e => ++e).ToList(); // колонки "Факт"
                NPOIHelper.DeleteColumn(wb, firstSheet, FirstDataColumn + 1);
            }

            delColumns.Reverse();
            foreach (var column in delColumns)
            {
                tableList[0].Columns.RemoveAt(column);
            }
            const int marksInSheet = (NPOIHelper.MaxColumnNum + 1 - FirstDataColumn) / ColumnsInMark;
            const int sumColumnsInSheet = marksInSheet * ColumnsInMark;
            var sheetCount = marksCount % marksInSheet > 0
                                 ? marksCount / marksInSheet + 1
                                 : marksCount / marksInSheet;

            for (var n = 1; n < sheetCount; n++)
            {
                NPOIHelper.CopySheet(wb, 0);
            }

            var sheetParams = new SheetParams
            {
                TableIndex = 0,
                FirstRow = 7 - 1,
                StyleRowsCount = 7,
                StyleColumnsCount = FirstDataColumn + ColumnsInMark,
                HeaderRowsCount = 3,
                FirstTotalColumn = FirstDataColumn,
                TotalRowsCount = 1,
                ExistHeaderNumColumn = true,
                RemoveUnusedColumns = false,
                MoveMeasureCell = false
            };

            var styleRows = GetStyleRows(firstSheet, sheetParams);
            styleRows[1].FirstDataColumn = FirstDataColumn;
            styleRows[2].FirstDataColumn = FirstDataColumn;
            sheetParams.StyleRows = styleRows;

            for (var n = 0; n < sheetCount; n++)
            {
                var dtSheetColumns = GetSheetColumns(tableList[1], n, marksInSheet);
                var dtHeader = AddColumns(dtSheetColumns);
                var columns = GetDataColumnsIndexies(dtHeader);
                var firstRepColumn = FirstDataColumn + n * sumColumnsInSheet;
                var repColumnsCount = columns.Count - FirstDataColumn;
                var tableColumns = ReportDataServer.GetColumnsList(0, FirstDataColumn);
                tableColumns.AddRange(ReportDataServer.GetColumnsList(firstRepColumn, repColumnsCount));
                sheetParams.SheetIndex = n;
                sheetParams.Columns = columns;
                sheetParams.SumColumns = GetSumColumnsIndexies(dtHeader);
                sheetParams.TableColumns = tableColumns;
                AddColumns(wb, sheetParams, dtHeader);
                FillSheet(wb, tableList, sheetParams);
            }
        }
    }
}
