using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    class Report020Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            const int firstMonthColumn = 1;
            var sheet = wb.GetSheetAt(sheetIndex);
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var year = Convert.ToInt32(paramHelper.GetParamValue(ParamUFKHelper.YEAR));
            var paramMonth = Convert.ToString(paramHelper.GetParamValue(ParamUFKHelper.MONTH));
            var months = ReportDataServer.ConvertToIntList(paramMonth);
            
            // заполняем таблицу колонок
            var columns = new List<int> { 0 };
            var sumColumns = new List<int> ();
            var columnsDt = new DataTable();
            columnsDt.Columns.Add(ReportDataServer.NAME);
            columnsDt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            columnsDt.Rows.Add(DBNull.Value, 0);

            for (var i = 0; i < months.Count; i++)
            {
                var column = i == 0 ? firstMonthColumn : firstMonthColumn + i*2 - 1;
                var patternColumn = i < 3 ? column : firstMonthColumn + 2*2 - 1;

                columns.Add(column);
                sumColumns.Add(column);
                columnsDt.Rows.Add(DBNull.Value, patternColumn);
                if (i > 0)
                {
                    sumColumns.Add(column + 1);
                    columnsDt.Rows.Add(DBNull.Value, patternColumn + 1);
                }
            }

            if (months.Count > 2)
            {
                columnsDt.Rows.Add(DBNull.Value, 6);
                sumColumns.Add(firstMonthColumn + months.Count*2 - 1);
            }

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 7 - 1,
                StyleRowsCount = 3,
                RemoveUnusedColumns = false,
                TotalRowsCount = 0,
                StyleColumnsCount = 7,
                HeaderRowsCount = 2,
                ExistHeaderNumColumn = true,
                Columns = columns,
                SumColumns = sumColumns
            };

            // Настраиваем области объединения в заголовке
            NPOIHelper.RemoveCellMergedRegion(sheet, 0, 1);
            NPOIHelper.RemoveCellMergedRegion(sheet, 1, 1);
            sheet.AddMergedRegion(new Region(0, 1, 0, sumColumns[sumColumns.Count - 1]));
            sheet.AddMergedRegion(new Region(1, 1, 1, sumColumns[sumColumns.Count - 1]));

            // Настраиваем колонки в шаблоне
            AddColumns(wb, sheetParams, columnsDt);
            var headerRowIndex = sheetParams.FirstRow - sheetParams.HeaderRowsCount;

            for (var i = 0; i < months.Count; i++)
            {
                var date = months[i] < 12 ? new DateTime(year, months[i] + 1, 1) : new DateTime(year + 1, 1, 1);
                var column = i == 0 ? firstMonthColumn : firstMonthColumn + i*2 - 1;
                var cell = NPOIHelper.GetCellByXY(sheet, headerRowIndex, column);
                ReplacePattern(cell, "01.MM.YEAR", date.ToShortDateString());
                if (i > 0)
                {
                    var nextCell = NPOIHelper.GetCellByXY(sheet, headerRowIndex, column + 1);
                    ReplacePattern(nextCell, "01.MM.YEAR", date.ToShortDateString());
                    var prevMonthDate = new DateTime(year, months[i - 1] + 1, 1);
                    ReplacePattern(nextCell, "01.MM-1.YEAR", prevMonthDate.ToShortDateString());
                }

                if (i == months.Count - 1 && months.Count > 2)
                {
                    var lastCell = NPOIHelper.GetCellByXY(sheet, headerRowIndex, column + 2);
                    ReplacePattern(lastCell, "01.MM.YEAR", date.ToShortDateString());
                    var firstMonthDate = new DateTime(year, months[0] + 1, 1);
                    ReplacePattern(lastCell, "01.MM0.YEAR", firstMonthDate.ToShortDateString());
                }
            }

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
