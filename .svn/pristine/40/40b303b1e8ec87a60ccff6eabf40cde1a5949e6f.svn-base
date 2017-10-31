using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports.MOFO0023.ReportFillers
{
    class ReportMOFO0023_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int levelsCount = 3;
            var sheet = wb.GetSheetAt(0);
            var repDt = tableList[0];
            var columns = repDt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName, column => column.Ordinal);
            columns.Remove(ReportDataServer.STYLE);
            ReportDataServer.RemoveEmptyColumns(repDt, true);
            var indexies = (from column in columns where repDt.Columns.Contains(column.Key) select column.Value).ToList();

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7 - 1,
                StyleRowsCount = 5,
                StyleColumnsCount = 4,
                FirstTotalColumn = 1,
                HeaderRowsCount = 3,
                ExistHeaderNumColumn = true,
                Columns = indexies,
                SumColumns = indexies.Where(i => i > 0).ToList()
            };

            // создаем колонки
            var dt = tableList[1];
            var columnsDt = new DataTable();
            columnsDt.Columns.Add(ReportDataServer.NAME);
            columnsDt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            columnsDt.Rows.Add(DBNull.Value, 0);

            foreach (var rowDt in dt.Rows)
            {
                columnsDt.Rows.Add(DBNull.Value, 1);
                columnsDt.Rows.Add(DBNull.Value, 2);
                columnsDt.Rows.Add(DBNull.Value, 3);
            }

            AddColumns(wb, sheetParams, columnsDt);

            // заполняем шапку и устанавливаем области объединения
            var headerRow = sheetParams.FirstRow - sheetParams.HeaderRowsCount;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var columnFrom = 1 + i * levelsCount;
                var columnTo = columnFrom + levelsCount - 1;
                sheet.GetRow(headerRow).GetCell(columnFrom).SetCellValue(Convert.ToString(dt.Rows[i][0]));
                sheet.AddMergedRegion(new CellRangeAddress(headerRow, headerRow, columnFrom, columnTo));
            }

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
