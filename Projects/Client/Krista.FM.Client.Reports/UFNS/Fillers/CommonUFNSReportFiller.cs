using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Krista.FM.Client.Reports.Month.Fillers;
using Krista.FM.Client.Reports.UFK14.Helpers;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class CommonUFNSReportFiller : CommonMonthReportFiller
    {
        public const int UndefinedIndex = -1;

        public void ReplacePattern(HSSFCell cell, string pattern, object value)
        {
            if (cell != null && cell.CellType == HSSFCell.CELL_TYPE_STRING)
            {
                var cellValue = cell.StringCellValue;
                if (cellValue.Contains(pattern))
                {
                    cell.SetCellValue(cellValue.Replace(pattern, Convert.ToString(value)));
                }
            }
        }

        public HSSFCell GetParamCell(HSSFSheet sheet, string paramName)
        {
            for (var i = 0; i < sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }

                for (var j = 0; j < row.LastCellNum; j++)
                {
                    var cellValue = NPOIHelper.GetCellStringValue(sheet, i, j);
                    if (cellValue.Contains(paramName))
                    {
                        return NPOIHelper.GetCellByXY(sheet, i, j);
                    }
                }
            }

            return null;
        }

        public void DeleteParamCell(HSSFSheet sheet, string paramName)
        {
            for (var i = 0; i < sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }

                for (var j = 0; j < row.LastCellNum; j++)
                {
                    var cellValue = NPOIHelper.GetCellStringValue(sheet, i, j);
                    if (cellValue.Contains(paramName))
                    {
                        NPOIHelper.SetCellValue(sheet, i, j, null);
                    }
                }
            }
        }

        public void SetParamValue(HSSFSheet sheet, string paramName, object value)
        {
            for (var i = 0; i < sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }

                for (var j = 0; j < row.LastCellNum; j++)
                {
                    var cellValue = NPOIHelper.GetCellStringValue(sheet, i, j);
                    if (cellValue.Contains(paramName))
                    {
                        var newVal = cellValue.Replace(paramName, Convert.ToString(value));
                        NPOIHelper.SetCellValue(sheet, i, j, newVal);
                    }
                }
            }
        }

        private int ControlRowsCount(HSSFSheet sheet, int rowsCount)
        {
            const int BigRowsCount = 40000;
            const int MaxRowNum = NPOIHelper.MaxRowNum - 100; // 100 строк оставляем в резерв

            if (sheet.LastRowNum + rowsCount > MaxRowNum)
            {
                var count = rowsCount;
                rowsCount = MaxRowNum - sheet.LastRowNum - 1;
                var errStr = String.Format(
                    @"Отчет содержит {0} строк. На лист будут выведены только {1} строк. Продолжить?",
                    count,
                    rowsCount
                    );
                var result = MessageBox.Show(
                    errStr,
                    @"Ошибка формирования отчета",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                    );

                if (result != DialogResult.Yes)
                {
                    rowsCount = 0;
                }
            }
            else if (rowsCount > BigRowsCount)
            {
                var errStr = String.Format(
                    @"Отчет содержит {0} строк и будет долго формироваться. Продолжить?",
                    rowsCount
                    );
                var result = MessageBox.Show(
                    errStr,
                    @"Предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                    );

                if (result != DialogResult.Yes)
                {
                    rowsCount = 0;
                }
            }

            return rowsCount;
        }

        public Dictionary<int, StyleRow> GetStyleRows(HSSFSheet sheet, SheetParams sp, int firstDataColumn = 0)
        {
            var result = new Dictionary<int, StyleRow>();

            for (var i = 0; i < sp.StyleRowsCount; i++)
            {
                result.Add(i, new StyleRow
                                  {
                                      Row = sheet.GetRow(sp.FirstRow + i),
                                      FirstDataColumn = firstDataColumn,
                                      SumColumns = sp.SumColumns
                                  });
            }

            return result;
        }

        private List<int> RemoveUnusedColumns(HSSFWorkbook wb, HSSFSheet sheet, SheetParams sp, List<int> columns)
        {
            if (!sp.RemoveUnusedColumns)
            {
                return columns;
            }

            for (var i = sheet.GetRow(sp.FirstRow).LastCellNum - 1; i >= 0; i--)
            {
                if (columns.Contains(i))
                {
                    continue;
                }

                // перемещаем ячейку с ед. изм.
                if (sp.MoveMeasureCell && i > columns.Max())
                {
                    var cell = NPOIHelper.GetCellByXY(sheet, sp.FirstRow - sp.HeaderRowsCount - 1, i);
                    MoveMeasureCell(cell, -1);
                }

                NPOIHelper.DeleteColumn(wb, sheet, i);
                for (var n = 0; n < columns.Count; n++)
                {
                    if (columns[n] > i)
                    {
                        columns[n] = columns[n] - 1;
                    }
                }
            }

            return columns;
        }

        private Dictionary<int, int> GetColumns(HSSFWorkbook wb, HSSFSheet sheet, SheetParams sp, DataTable tblData)
        {
            var sheetColumns = sp.Columns ?? ReportDataServer.GetColumnsList(0, sheet.GetRow(sp.FirstRow).LastCellNum); // последняя ячейка имеет номер LastCellNum - 1
            if (sp.ToSortColumns)
            {
                sheetColumns.Sort();
            }

            sheetColumns = RemoveUnusedColumns(wb, sheet, sp, sheetColumns);
            var tableColumns = sp.TableColumns ?? ReportDataServer.GetColumnsList(0, tblData.Columns.Count - 1);
            var count = Math.Min(sheetColumns.Count, tableColumns.Count);
            var columns = new Dictionary<int, int>();
            
            for (var i = 0; i < count; i++)
            {
                if (sheetColumns[i] != UndefinedIndex)
                {
                    columns.Add(sheetColumns[i], tableColumns[i]);
                }
            }

            return columns;
        }


        public void FillSheet(HSSFWorkbook wb, DataTable[] tableList, SheetParams sp)
        {
            var sheet = wb.GetSheetAt(sp.SheetIndex);
            var dic = ParamUFKHelper.GetParamList();
            var tblData = sp.TableIndex == UndefinedIndex ? tableList[sp.SheetIndex] : tableList[sp.TableIndex];
            var firstRow = sp.FirstRow;
            var tblCaptions = tableList[tableList.Length - 1];
            var rowCaption = tblCaptions.Rows[0];
            FillCaptionParams(wb, sheet, tblCaptions, dic);
            var styleRows = sp.StyleRows ?? GetStyleRows(sheet, sp);

            // устанавливаем в колонках с суммами формат чисел (включая строки ИТОГО)
            if (sp.SumColumns != null && sp.SumColumns.Count > 0)
            {
                var precisionFormat = GetPrecisionFormat(rowCaption[dic[ParamUFKHelper.PRECISION]]);
                foreach (var styleRow in styleRows.Values)
                {
                    SetRangeDataFormat(wb,
                                       sheet,
                                       precisionFormat,
                                       new[] { styleRow.Row.RowNum },
                                       styleRow.SumColumns);
                }

                if (sp.TotalRowsCount > 0)
                {
                    var totalRows = new List<int>();
                    for (var i = 0; i < sp.TotalRowsCount; i++)
                    {
                        totalRows.Add(firstRow + sp.StyleRowsCount + i);
                    }

                    var totalColumns = sp.SumColumns.Where(sumColumn => sumColumn >= sp.FirstTotalColumn).ToList();
                    SetRangeDataFormat(wb, sheet, precisionFormat, totalRows, totalColumns);
                }
            }

            var columns = GetColumns(wb, sheet, sp, tblData);

            // заполняем строки
            var rowsCount = ControlRowsCount(sheet, tblData.Rows.Count - sp.TotalRowsCount);
            var startRow = firstRow + styleRows.Count;

            for (var i = 0; i < rowsCount; i++)
            {
                var row = tblData.Rows[i];
                var rowIndex = startRow + i;
                if (row[ReportDataServer.STYLE] == DBNull.Value)
                {
                    continue;
                }

                StyleRow style;
                if (!styleRows.TryGetValue(Convert.ToInt32(row[ReportDataServer.STYLE]), out style))
                {
                    continue;
                }

                NPOIHelper.CopyRow(wb, sheet, style.Row.RowNum, rowIndex);

                foreach (var column in columns.Where(column => column.Key >= style.FirstDataColumn))
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, column.Key, row[column.Value]);
                }
            }

            var restRows = tblData.Rows.Count - rowsCount - sp.TotalRowsCount;

            if (restRows > 0) // выведены не все строки
            {
                var rowIndex = startRow + rowsCount;
                NPOIHelper.CopyRow(wb, sheet, rowIndex - 1, rowIndex);
                var row = sheet.CreateRow(rowIndex);
                var cell = row.CreateCell(0);
                cell.SetCellValue(String.Format(@"... ещё {0} строк", restRows));
                rowsCount++;
            }

            // заполняем строки итого
            for (var i = 0; i < sp.TotalRowsCount; i++)
            {
                var ind = tblData.Rows.Count - sp.TotalRowsCount + i;
                if (ind >= 0 && ind < tblData.Rows.Count)
                {
                    var totalRow = tblData.Rows[ind];

                    foreach (var column in columns.Where(column => column.Key >= sp.FirstTotalColumn))
                    {
                        NPOIHelper.SetCellValue(sheet, startRow + rowsCount + i, column.Key, totalRow[column.Value]);
                    }
                }
            }

            // сдвигаем строки вверх
            var lastRowNum = sheet.LastRowNum;
            if (lastRowNum >= startRow && styleRows.Count > 0)
            {
                NPOIHelper.ShiftRows(sheet, startRow, lastRowNum, -styleRows.Count);
            }
            
            // удаляем то, что осталось после сдвига строк
            for (var i = 0; i < styleRows.Count; i++)
            {
                var row = sheet.GetRow(lastRowNum - i);
                if (row != null)
                {
                    sheet.RemoveRow(row);
                }
            }

            FillHeaderNumColumn(sheet, sp); // заполняем строку с номерами колонок
            sheet.ForceFormulaRecalculation = true;
        }

        private void FillHeaderNumColumn(HSSFSheet sheet, SheetParams sp)
        {
            if (!sp.ExistHeaderNumColumn)
            {
                return;
            }

            var headerRow = sheet.GetRow(sp.FirstRow - 1);
            var lastColumn = headerRow.LastCellNum; // последняя ячейка имеет номер LastCellNum - 1
            var num = 1;

            for (var i = 0; i < lastColumn; i++)
            {
                var numReg = NPOIHelper.CellMergedRegion(sheet, headerRow.RowNum, i);
                if (numReg != NPOIHelper.UndefinedIndex && i != sheet.GetMergedRegionAt(numReg).ColumnFrom)
                {
                    continue;
                }

                var cell = headerRow.GetCell(i) ?? headerRow.CreateCell(i);
                cell.SetCellType(HSSFCell.CELL_TYPE_BLANK);
                cell.SetCellType(HSSFCell.CELL_TYPE_NUMERIC);
                cell.SetCellValue(num++);
            }
        }

        private void SetMergedRegions(HSSFSheet sheet, int firstRow, DataTable columns)
        {
            if (!columns.Columns.Contains(ReportDataServer.MERGED))
            {
                return;
            }

            var maxLevel = columns.Rows.Cast<DataRow>().Max(row => Convert.ToInt32(row[ReportDataServer.LEVEL]));
            var column = 0;
            var mergs = new Dictionary<int, Region>();

            foreach (DataRow row in columns.Rows)
            {
                var level = Convert.ToInt32(row[ReportDataServer.LEVEL]);
                var merged = Convert.ToBoolean(row[ReportDataServer.MERGED]);
                if (row[ReportDataServer.NAME] != DBNull.Value)
                {
                    var name = Convert.ToString(row[ReportDataServer.NAME]);
                    NPOIHelper.SetCellValue(sheet, firstRow + level, column, name);
                }

                for (var i = mergs.Count - 1; i >= 0; i--)
                {
                    var merg = mergs.ElementAt(i);
                    if (merg.Key < level)
                    {
                        break;
                    }

                    if (merg.Value.ColumnTo > merg.Value.ColumnFrom ||
                        (merg.Value.ColumnTo == merg.Value.ColumnFrom && merg.Value.RowTo > merg.Value.RowFrom))
                    {
                        sheet.AddMergedRegion(merg.Value);
                    }
                    mergs.Remove(merg.Key);
                }

                if (mergs.Count > 0)
                {
                    mergs.ElementAt(mergs.Count - 1).Value.RowTo = firstRow + level - 1;
                }

                if (merged)
                {
                    mergs.Add(level, new Region(firstRow + level, column, firstRow + level, column - 1));
                    continue;
                }

                if (level < maxLevel)
                {
                    sheet.AddMergedRegion(new Region(firstRow + level, column, firstRow + maxLevel, column));
                }

                foreach (var merg in mergs)
                {
                    merg.Value.ColumnTo++;
                }

                column++;
            }

            foreach (var merg in mergs.Values)
            {
                if (merg.ColumnTo > merg.ColumnFrom ||
                    (merg.ColumnTo == merg.ColumnFrom && merg.RowTo > merg.RowFrom))
                {
                    sheet.AddMergedRegion(merg);
                }
            }
        }

        private List<int> GetNotMergedColumns(DataTable dtColumns)
        {
            var columns = ReportDataServer.GetColumnsList(0, dtColumns.Rows.Count);
            return dtColumns.Columns.Contains(ReportDataServer.MERGED)
                       ? columns.Where(i => !Convert.ToBoolean(dtColumns.Rows[i][ReportDataServer.MERGED])).ToList()
                       : columns;
        }

        public void AddColumns(HSSFWorkbook wb, SheetParams sp, DataTable columns)
        {
            var sheet = wb.GetSheetAt(sp.SheetIndex);
            var headerRow = sp.FirstRow - sp.HeaderRowsCount;
            var firstRow = sp.FirstRow + sp.StyleRowsCount + sp.TotalRowsCount;
            var rowsCount = sp.StyleRowsCount + sp.TotalRowsCount + sp.HeaderRowsCount;
            var indexies = GetNotMergedColumns(columns);
            var styleColumnsCount = sp.StyleColumnsCount;

            // сохраняем ширину колонок в массиве
            var columnWidth = new int[styleColumnsCount];

            for (var i = 0; i < styleColumnsCount; i++)
            {
                columnWidth[i] = sheet.GetColumnWidth(i);
            }

            // сохраняем области объединения
            var regions = new List<Region>();
            for (var i = 0; i < sheet.NumMergedRegions; i++)
            {
                var reg = sheet.GetMergedRegionAt(i);
                if (reg == null) 
                    continue;

                if (reg.RowFrom < headerRow && reg.RowTo >= firstRow)
                    continue;

                if (reg.ColumnFrom != reg.ColumnTo && (reg.ColumnFrom >= sp.FirstTotalColumn || reg.ColumnTo >= sp.FirstTotalColumn))
                    continue;

                regions.Add(reg);
            }

            // создаем новые строки
            if (firstRow <= sheet.LastRowNum)
            {
                sheet.ShiftRows(firstRow, sheet.LastRowNum, rowsCount);
            }

            for (var i = 0; i < rowsCount; i++)
            {
                var styleRow = sheet.GetRow(headerRow + i);
                if (styleRow != null)
                {
                    var row = sheet.CreateRow(firstRow + i);
                    row.Height = styleRow.Height;
                }
            }

            // создаем колонки в новых строках
            var headerRowIndex = sp.ExistHeaderNumColumn
                         ? firstRow + sp.HeaderRowsCount - 2
                         : firstRow + sp.HeaderRowsCount - 1;

            for (var n = 0; n < indexies.Count; n++)
            {
                var style = Convert.ToInt32(columns.Rows[indexies[n]][ReportDataServer.STYLE]);

                for (var i = 0; i < rowsCount; i++)
                {
                    var row = sheet.GetRow(firstRow + i);
                    if (row == null)
                    {
                        continue;
                    }

                    var styleRow = sheet.GetRow(headerRow + i);
                    NPOIHelper.CopyCell(styleRow.GetCell(style), row.CreateCell(n));
                }

                var headerCell = sheet.GetRow(headerRowIndex).GetCell(n);
                var name = columns.Rows[indexies[n]][ReportDataServer.NAME];
                if (name != DBNull.Value)
                {
                    headerCell.SetCellValue(Convert.ToString(columns.Rows[indexies[n]][ReportDataServer.NAME]));
                }

                sheet.SetColumnWidth(n, columnWidth[style]);
            }

            NPOIHelper.ShiftRows(sheet, firstRow, sheet.LastRowNum, -rowsCount);

            // восстанавливаем области объединения
            foreach (var reg in regions)
            {
                sheet.AddMergedRegion(reg);
            }

            // перемещаем ячейку с ед. изм.
            if (sp.MoveMeasureCell && indexies.Count != styleColumnsCount)
            {
                var cell = NPOIHelper.GetCellByXY(sheet, headerRow - 1, styleColumnsCount - 1);
                MoveMeasureCell(cell, indexies.Count - styleColumnsCount);
            }

            // создаем области объединения в иерархической шапке
            SetMergedRegions(sheet, sp.FirstRow - sp.HeaderRowsCount, columns);
        }

        public void MoveMeasureCell(HSSFCell measureCell, int shift)
        {
            if (measureCell == null || shift == 0)
            {
                return;
            }

            var style = measureCell.CellStyle;
            var value = measureCell.StringCellValue;
            var newColumnIndex = measureCell.ColumnIndex + shift;
            var row = measureCell.Sheet.GetRow(measureCell.RowIndex);
            row.RemoveCell(measureCell);
            var cell = row.CreateCell(newColumnIndex);
            cell.CellStyle = style;
            cell.SetCellValue(value);
        }

        public Dictionary<string, int> GetRowCodes(HSSFSheet sheet, int rowIndex, int firstColumn = 0)
        {
            // определяем по строке кодов соответствие колонки и кода
            var codes = new Dictionary<string, int>();
            var codeRow = sheet.GetRow(rowIndex);
            if (codeRow == null)
            {
                return codes;
            }

            for (var i = firstColumn; i < codeRow.LastCellNum; i++)
            {
                var cell = codeRow.GetCell(i);
                if (cell == null)
                {
                    continue;
                }

                var value = NPOIHelper.GetCellValue(cell);
                if (value == null)
                {
                    continue;
                }

                var code = Convert.ToString(value).Trim();
                if (code == String.Empty)
                {
                    continue;
                }

                if (!codes.Keys.Contains(code))
                {
                    codes.Add(code, i);
                }
            }

            return codes;
        }

        private List<int> GetTrueColumnsIndexies(DataTable dtColumns, string field)
        {
            var columns = new List<int>();
            var i = 0;

            foreach (var row in dtColumns.Rows.Cast<DataRow>().Where(row => !Convert.ToBoolean(row[ReportDataServer.MERGED])))
            {
                if (Convert.ToBoolean(row[field]))
                {
                    columns.Add(i);
                }
                i++;
            }

            return columns;
        }

        public List<int> GetSumColumnsIndexies(DataTable dtColumns)
        {
            return GetTrueColumnsIndexies(dtColumns, ReportDataServer.IsRUB);
        }

        public List<int> GetDataColumnsIndexies(DataTable dtColumns)
        {
            return GetTrueColumnsIndexies(dtColumns, ReportDataServer.IsDATA);
        }
    }

    class StyleRow
    {
        public HSSFRow Row;
        public int FirstDataColumn;
        public List<int> SumColumns;
    }

    class SheetParams
    {
        public int TableIndex { get; set; }
        public int SheetIndex { get; set; }
        public int FirstRow { get; set; }
        public int StyleRowsCount { get; set; }
        public int StyleColumnsCount { get; set; }
        public int FirstTotalColumn { get; set; }
        public int TotalRowsCount { get; set; }
        public int HeaderRowsCount { get; set; }
        public bool ExistHeaderNumColumn { get; set; }
        public bool ToSortColumns { get; set; }
        public bool RemoveUnusedColumns { get; set; }
        public bool MoveMeasureCell { get; set; }
        public List<int> Columns { get; set; }
        public List<int> SumColumns { get; set; }
        public List<int> TableColumns { get; set; }
        public Dictionary<int, StyleRow> StyleRows { get; set; }

        public SheetParams()
        {
            TableIndex = CommonUFNSReportFiller.UndefinedIndex;
            FirstRow = 4;
            FirstTotalColumn = 0;
            TotalRowsCount = 1;
            HeaderRowsCount = 1;
            ExistHeaderNumColumn = false;
            ToSortColumns = true;
            RemoveUnusedColumns = true;
            MoveMeasureCell = true;
        }
    }
}
