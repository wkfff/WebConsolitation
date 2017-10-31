using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK021Filler : CommonUFNSReportFiller
    {
        private void ShiftMeasureCell(HSSFSheet sheet, int columnIndex)
        {
            var cell = GetParamCell(sheet, ParamUFKHelper.MEASURE);
            if (cell == null || cell.ColumnIndex != columnIndex)
            {
                return;
            }

            var row = sheet.GetRow(cell.RowIndex);
            var newCell = row.GetCell(cell.ColumnIndex - 1) ?? row.CreateCell(cell.ColumnIndex - 1);
            NPOIHelper.CopyCell(cell, newCell);
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int levelsInPattern = 3;
            const int yearsInPattern = 3;
            const int firstSumColumn = 1;
            var sheet = wb.GetSheetAt(0);
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var paramLevels = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.BDGT_LEVEL));
            var precision = GetPrecisionFormat(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.PRECISION));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var levels = ReportDataServer.ConvertToIntList(paramLevels);
            var dt = tableList[0];

            // удаляем в шаблоне колонки отсутствующих уровней бюджета
            for (var i = yearsInPattern - 1; i >= 0; i--)
            {
                var firstYearColumn = i == 0 ? firstSumColumn : firstSumColumn + levelsInPattern * (2 * i - 1);

                for (var l = levelsInPattern - 1; l >= 0; l--)
                {
                    if (levels.Contains(l))
                    {
                        continue;
                    }

                    var index = i == 0 ? firstYearColumn + l : firstYearColumn + l * 2;
                    if (i > 0)
                    {
                        ShiftMeasureCell(sheet, index + 1);
                        NPOIHelper.DeleteColumn(wb, sheet, index + 1);
                    }
                    ShiftMeasureCell(sheet, index);
                    NPOIHelper.DeleteColumn(wb, sheet, index);
                }
            }

            const int firstSumRow = 24 - 1;
            var sumRows = new List<int> {0, 1, 2, 4, 5, 6, 9, 10, 11, 14, 15, 16};
            sumRows = sumRows.Select(row => row + firstSumRow).ToList();
            // определяем колонки с суммами
            var sumColumns = new List<int>();
            for (var i = 0; i < years.Count; i++)
            {
                var firstYearColumn = i == 0 ? firstSumColumn : firstSumColumn + levels.Count*(2*i - 1);

                for (var l = 0; l < levels.Count; l++)
                {
                    var index = i == 0 ? firstYearColumn + l : firstYearColumn + l * 2;
                    sumColumns.Add(index);
                }
            }

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8 - 1,
                StyleRowsCount = 36,
                StyleColumnsCount = firstSumColumn + levels.Count * (2 * yearsInPattern - 1),
                HeaderRowsCount = 2,
                TotalRowsCount = 0,
                RemoveUnusedColumns = false
            };
            
            AddColumns(wb, sheetParams, tableList[1]);
            var tblCaptions = tableList[tableList.Length - 1];
            FillCaptionParams(wb, sheet, tblCaptions, ParamUFKHelper.GetParamList());

            // заполняем шапку и устанавливаем области объединения
            var headerRow1 = sheetParams.FirstRow - sheetParams.HeaderRowsCount;
            var headerRow2 = headerRow1 + 16;

            for (var i = 0; i < years.Count; i++)
            {
                var columnFrom = i == 0 ? firstSumColumn : firstSumColumn + levels.Count * (2 * i - 1);
                var columnTo = i == 0 ? columnFrom + levels.Count - 1 : columnFrom + levels.Count * 2 - 1;
                sheet.GetRow(headerRow1).GetCell(columnFrom).SetCellValue(years[i]);
                sheet.GetRow(headerRow2).GetCell(columnFrom).SetCellValue(years[i]);
                sheet.AddMergedRegion(new CellRangeAddress(headerRow1, headerRow1, columnFrom, columnTo));
                sheet.AddMergedRegion(new CellRangeAddress(headerRow2, headerRow2, columnFrom, columnTo));
                if (i <= 0) continue;

                var value = String.Format("к роста {0}/{1}", years[i], years[i - 1]);
                for (var l = 0; l < levels.Count; l++)
                {
                    sheet.GetRow(headerRow1 + 1).GetCell(columnFrom + l*2 + 1).SetCellValue(value);
                    sheet.GetRow(headerRow2 + 1).GetCell(columnFrom + l*2 + 1).SetCellValue(value);
                }
            }

            // заполняем отчет данными
            for (var i = 0; i < sumRows.Count; i++)
            {
                var row = dt.Rows[i];
                for (var n = 0; n < sumColumns.Count; n++)
                {
                    NPOIHelper.SetCellValue(sheet, sumRows[i], sumColumns[n], row[n]);
                }
            }

            sumRows = new List<int>();
            sumRows.AddRange(ReportDataServer.GetColumnsList(sheetParams.FirstRow, 12));
            sumRows.AddRange(ReportDataServer.GetColumnsList(sheetParams.FirstRow + 16, 20));
            
            // устанавливаем в колонках с суммами формат чисел (включая строки ИТОГО)
            SetRangeDataFormat(wb, sheet, precision, sumRows, sumColumns);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
