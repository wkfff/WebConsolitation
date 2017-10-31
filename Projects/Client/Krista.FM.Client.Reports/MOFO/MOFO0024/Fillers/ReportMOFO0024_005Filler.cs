using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0024.ReportFillers
{
    class ReportMOFO0024_005Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const string year = "YEAR";
            const string year1 = "YEAR_1";
            const int rowsInYear = 13;
            const int dtRowsInYear = 4;
            const int firstRow = 7 - 1;
            const int firstStyleRow = firstRow + rowsInYear;
            const int startRow = firstStyleRow + rowsInYear;
            var dataRows = new Dictionary<int, int[]>
                                {
                                    {2,  new [] {13}},
                                    {4,  new [] {1, 5, 9, 13}},
                                    {9,  new [] {1, 3, 7, 11}},
                                    {11, new [] {1, 3, 7, 11}},
                                };
            var divRows = new[] { 12, 10, 8, 5, 3, 0 };
            var sumRows = new[] { 2, 4, 7, 9, 11 };
            var sumColumns = new[] { 1, 3, 5, 7, 9, 11, 13 };
            var sheet = wb.GetSheetAt(0);
            var dt = tableList[0];
            var yearsCount = dt.Rows.Count/dtRowsInYear;
            var tblCaptions = tableList[tableList.Length - 1];
            var paramHelper = new ParamUFKHelper(tblCaptions.Rows[0]);
            var paramYears = Convert.ToString(paramHelper.GetParamValue(ParamUFKHelper.YEARS));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var precision = GetPrecisionFormat(paramHelper.GetParamValue(ParamUFKHelper.PRECISION));

            // устанавливаем формат чисел
            SetRangeDataFormat(wb, sheet, precision, sumRows.Select(i => i + firstStyleRow), sumColumns);

            for (var y = 0; y < yearsCount; y++)
            {
                var firstYearRow = startRow + y*rowsInYear;
                // копируем строки шаблона
                for (var i = 0; i < rowsInYear; i++)
                {
                    var rowIndex = firstYearRow + i;
                    NPOIHelper.CopyRow(wb, sheet, firstStyleRow + i, rowIndex);
                    // заполняем года в первой колонке
                    var cellValue = NPOIHelper.GetCellStringValue(sheet, rowIndex, 0);
                    cellValue = y > 0 && cellValue.Contains(year1)
                                    ? cellValue.Replace(year1, Convert.ToString(years[y - 1]))
                                    : cellValue;

                    cellValue = cellValue.Contains(year)
                                    ? cellValue.Replace(year, Convert.ToString(years[y]))
                                    : cellValue;

                    NPOIHelper.SetCellValue(sheet, rowIndex, 0, cellValue);
                }

                // заполняем ячейки с данными
                for (var i = 0; i < dataRows.Count; i++)
                {
                    var dataRow = dataRows.ElementAt(i);
                    for (var j = 0; j < dataRow.Value.Length; j++)
                    {
                        var rowIndex = firstYearRow + dataRow.Key;
                        var dataRowIndex = y * dtRowsInYear + i;
                        NPOIHelper.SetCellValue(sheet, rowIndex, dataRow.Value[j], dt.Rows[dataRowIndex][j]);
                    }
                }
            }

            // сдвигаем строки вверх
            var lastRowNum = sheet.LastRowNum;
            var shiftCount = rowsInYear*2;
            sheet.ShiftRows(startRow, lastRowNum, -shiftCount, true, true, true);
            foreach (var rowIndex in divRows.Select(i => firstRow + i + 1))
            {
                if (rowIndex <= lastRowNum - shiftCount)
                {
                    sheet.ShiftRows(rowIndex, lastRowNum - shiftCount, -1, true, true, true);
                }
                shiftCount++;
            }

            // удаляем то, что осталось после сдвига строк
            for (var i = 0; i < shiftCount; i++)
            {
                var row = sheet.GetRow(lastRowNum - i);
                if (row != null)
                {
                    sheet.RemoveRow(row);
                }
            }

            FillCaptionParams(wb, sheet, tblCaptions, ParamUFKHelper.GetParamList());
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
