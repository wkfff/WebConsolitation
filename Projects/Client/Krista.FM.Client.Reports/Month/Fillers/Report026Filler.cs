using System;
using System.Collections.Generic;
using System.Data;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    class Report026Filler : CommonMonthReportFiller
    {
        public virtual void FillMonthReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var tblCaptions = tableList[tableList.Length - 1];
            var rowCaption = tblCaptions.Rows[0];

            for (var j = 0; j < tableList.Length - 1; j++)
            {
                var sheet = wb.GetSheetAt(j);
                var tblData = tableList[j];
                var startRow = j == 0 ? 10 : 8;
                FillCaptionParams(wb, sheet, tblCaptions, GetParamDictionary());
                var precision = GetPrecisionFormat(rowCaption[7]);
                SetRangeDataFormat(wb, sheet, precision, new List<int> { startRow + 2 }, new List<int> { 2 });

                for (var i = 2; i < tblData.Rows.Count; i++)
                {
                    NPOIHelper.CopyRow(wb, sheet, startRow, startRow + 1);
                }

                var colors = new[] { 155, 171 };
                var colorIndex = colors[0];
                var oldCode = -1;
                var newCode = -1;
                var odd = 1;

                for (var i = 0; i < tblData.Rows.Count; i++)
                {
                    var rowIndex = startRow + i;
                    SetRangeDataFormat(wb, sheet, precision, new List<int> { rowIndex }, new List<int> { 2 });
                    var rowData = tblData.Rows[i];
                    NPOIHelper.SetCellValue(sheet, rowIndex, 0, rowData[0]);
                    NPOIHelper.SetCellValue(sheet, rowIndex, 1, rowData[1]);
                    NPOIHelper.SetCellValue(sheet, rowIndex, 2, ReportDataServer.GetDecimal(rowData[2]));
                    var isGo = Convert.ToBoolean(rowData[4]);

                    newCode = Convert.ToInt32(rowData[3]);

                    if (newCode != oldCode)
                    {
                        odd = 1 - odd;
                        colorIndex = colors[odd];
                        oldCode = newCode;
                    }

                    if (j > 0 || isGo)
                    {
                        if (isGo)
                        {
                            colorIndex = 170;
                        }

                        var cell = NPOIHelper.GetCellByXY(sheet, rowIndex, 1);
                        var newStyle = wb.CreateCellStyle();
                        newStyle.CloneStyleFrom(cell.CellStyle);
                        newStyle.FillForegroundColor = (short)colorIndex;
                        newStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
                        cell.CellStyle = newStyle;
                    }
                }

                sheet.ForceFormulaRecalculation = true;
            }

            var sheetRgnVisible = Convert.ToBoolean(rowCaption[5]);
            var sheetStlVisible = Convert.ToBoolean(rowCaption[6]);

            if (!sheetRgnVisible)
            {
                wb.SetSheetHidden(0, true);
            }

            if (!sheetStlVisible)
            {
                wb.SetSheetHidden(1, true);
            }
        }
    }
}
