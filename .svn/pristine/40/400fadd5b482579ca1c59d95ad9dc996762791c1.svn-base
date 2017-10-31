using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports.MOFO0028.ReportFillers
{
    class ReportMOFO0028_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int markColumnsCount = 7;
            const int firstDataColumn = 3;
            var sheet = wb.GetSheetAt(0);
            var repDt = tableList[0];
            var dt = tableList[1];
            var columns = repDt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName, column => column.Ordinal);
            columns.Remove(ReportDataServer.STYLE);
            tableList[0] = ReportDataServer.RemoveNullColumns(repDt, columns.Values.Where(i => i >= firstDataColumn));
            var indexies = (from column in columns
                            where tableList[0].Columns.Contains(column.Key)
                            select column.Value).ToList();
            var sumColumnsIndexies = new List<int> {3, 4, 5, 6};
            var sumColumns = new List<int>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                sumColumns.AddRange(sumColumnsIndexies.Select(index => firstDataColumn + i * markColumnsCount + index));
            }

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6 - 1,
                StyleRowsCount = 5,
                StyleColumnsCount = 10,
                FirstTotalColumn = firstDataColumn,
                TotalRowsCount = 3,
                HeaderRowsCount = 3,
                ExistHeaderNumColumn = true,
                RemoveUnusedColumns = true,
                Columns = indexies,
                SumColumns = sumColumns
            };

            // создаем колонки
            var columnsDt = new DataTable();
            columnsDt.Columns.Add(ReportDataServer.NAME);
            columnsDt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            columnsDt.Rows.Add(DBNull.Value, 0);
            columnsDt.Rows.Add(DBNull.Value, 1);
            columnsDt.Rows.Add(DBNull.Value, 2);

            foreach (var rowDt in dt.Rows)
            {
                for (var i = 0; i < markColumnsCount; i++)
                {
                    columnsDt.Rows.Add(DBNull.Value, firstDataColumn + i);
                }
            }

            AddColumns(wb, sheetParams, columnsDt);

            // заполняем шапку и устанавливаем области объединения
            var headerRow = sheetParams.FirstRow - sheetParams.HeaderRowsCount;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var columnFrom = firstDataColumn + i * markColumnsCount;
                var columnTo = columnFrom + markColumnsCount - 1;
                sheet.GetRow(headerRow).GetCell(columnFrom).SetCellValue(Convert.ToString(dt.Rows[i][0]));
                sheet.AddMergedRegion(new CellRangeAddress(headerRow, headerRow, columnFrom, columnTo));
            }

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
