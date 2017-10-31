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
    class ReportUFK023Filler : CommonUFNSReportFiller
    {
        private const int yearsInPattern = 3;
        private const int firstSumColumn = 1;
        private const int kvsrsInPattern = 2 + 1; // + колонка Всего
        private const int firstSumRow = 24 - 1;
        private const int firstHeaderRow = 6 - 1;
        private const int secondHeaderRow = 22 - 1;

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

        private int ColumnsInYear(int kvsrsCount)
        {
            return kvsrsCount < kvsrsInPattern - 1 ? kvsrsCount : kvsrsCount + 1;

        }

        private int GetStyleColumnsCount(int kvsrsCount)
        {
            return firstSumColumn + (2*yearsInPattern - 1)*ColumnsInYear(kvsrsCount);
        }

        private int DeleteColumns(HSSFWorkbook wb, HSSFSheet sheet, int kvsrsCount)
        {
            var delColumns = kvsrsCount < 2 ? new List<int> {0} : new List<int>();

            for (var i = kvsrsCount; i < kvsrsInPattern - 1; i++)
            {
                delColumns.Add(i + 1);
            }

            for (var i = yearsInPattern - 1; i >= 0; i--)
            {
                var firstYearColumn = i == 0 ? firstSumColumn : firstSumColumn + kvsrsInPattern*(2*i - 1);

                for (var l = kvsrsInPattern - 1; l >= 0; l--)
                {
                    if (!delColumns.Contains(l))
                    {
                        continue;
                    }

                    var index = i == 0 ? firstYearColumn + l : firstYearColumn + l*2;
                    if (i > 0)
                    {
                        ShiftMeasureCell(sheet, index + 1);
                        NPOIHelper.DeleteColumn(wb, sheet, index + 1);
                    }
                    ShiftMeasureCell(sheet, index);
                    NPOIHelper.DeleteColumn(wb, sheet, index);
                }
            }

            return GetStyleColumnsCount(kvsrsCount);
        }

        private int InsertColumns(HSSFWorkbook wb, HSSFSheet sheet, int kvsrsCount)
        {
            // вставляем фиктивные колонки в первый год
            for (var l = kvsrsInPattern - 1; l >= 0; l--)
            {
                NPOIHelper.InsertColumn(sheet, firstSumColumn + l);
            }

            var count = 0;

            for (var i = 0; i < yearsInPattern; i++)
            {
                var firstYearColumn = firstSumColumn + count + i * kvsrsInPattern * 2;

                for (var l = kvsrsInPattern - 1; l < kvsrsCount; l++)
                {
                    var index = firstYearColumn + 4;
                    NPOIHelper.InsertColumn(sheet, index);
                    NPOIHelper.InsertColumn(sheet, index);
                    NPOIHelper.CopyColumn(sheet, index - 1, index + 1);
                    count += 2;
                }
            }

            // удаляем фиктивные колонки
            for (var l = kvsrsCount; l >= 0; l--)
            {
                NPOIHelper.DeleteColumn(wb, sheet, firstSumColumn + l*2);
            }

            return GetStyleColumnsCount(kvsrsCount);
        }

        private List<int> GetSumColumns(int yearsCount, int kvsrsCount)
        {
            var sumColumns = new List<int>();

            for (var i = 0; i < yearsCount; i++)
            {
                var firstYearColumn = i == 0 ? firstSumColumn : firstSumColumn + ColumnsInYear(kvsrsCount) * (2 * i - 1);

                for (var l = 0; l < ColumnsInYear(kvsrsCount); l++)
                {
                    var index = i == 0 ? firstYearColumn + l : firstYearColumn + l * 2;
                    sumColumns.Add(index);
                }
            }

            return sumColumns;
        }

        private void FillHeader(HSSFSheet sheet, List<int> years, int kvsrsCount)
        {
            if (kvsrsCount == 0)
            {
                return;
            }

            var count = ColumnsInYear(kvsrsCount);

            for (var i = 0; i < years.Count; i++)
            {
                var columnFrom = i == 0 ? firstSumColumn : firstSumColumn + count * (2 * i - 1);
                var columnTo = i == 0 ? columnFrom + count - 1 : columnFrom + count * 2 - 1;
                sheet.GetRow(firstHeaderRow).GetCell(columnFrom).SetCellValue(years[i]);
                sheet.GetRow(secondHeaderRow).GetCell(columnFrom).SetCellValue(years[i]);
                sheet.AddMergedRegion(new CellRangeAddress(firstHeaderRow, firstHeaderRow, columnFrom, columnTo));
                sheet.AddMergedRegion(new CellRangeAddress(secondHeaderRow, secondHeaderRow, columnFrom, columnTo));

                for (var l = 0; l < count; l++)
                {
                    var index = i == 0 ? columnFrom + l : columnFrom + l * 2;
                    var name = sheet.GetRow(firstHeaderRow + 1).GetCell(index).StringCellValue;
                    sheet.GetRow(secondHeaderRow + 1).GetCell(index).SetCellValue(name);
                    if (i > 0)
                    {
                        var value = String.Format("к роста {0}/{1}", years[i], years[i - 1]);
                        sheet.GetRow(firstHeaderRow + 1).GetCell(index + 1).SetCellValue(value);
                        sheet.GetRow(secondHeaderRow + 1).GetCell(index + 1).SetCellValue(value);
                    }
                }
            }
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var paramKvsr = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.KVSR));
            var precision = GetPrecisionFormat(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.PRECISION));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var kvsrs = ReportDataServer.ConvertToIntList(paramKvsr);
            var dt = tableList[0];

            var styleColumnsCount = kvsrs.Count < kvsrsInPattern
                                        ? DeleteColumns(wb, sheet, kvsrs.Count)
                                        : InsertColumns(wb, sheet, kvsrs.Count);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8 - 1,
                StyleRowsCount = 36,
                StyleColumnsCount = styleColumnsCount,
                HeaderRowsCount = 2,
                TotalRowsCount = 0,
                RemoveUnusedColumns = false
            };

            AddColumns(wb, sheetParams, tableList[1]);
            FillHeader(sheet, years, kvsrs.Count);
            FillCaptionParams(wb, sheet, tableList[tableList.Length - 1], ParamUFKHelper.GetParamList());

            var sumRows = new List<int> { 0, 1, 2, 4, 5, 6, 9, 10, 11, 14, 15, 16 };
            sumRows = sumRows.Select(row => row + firstSumRow).ToList();
            var sumColumns = GetSumColumns(years.Count, kvsrs.Count);

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
