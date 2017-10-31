using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class NPOIHelper
    {
        public static string ShiftFormulaRow(string formula, int rowShift, Predicate<int> match)
        {
            if (String.IsNullOrEmpty(formula))
            {
                return formula;
            }

            if (rowShift != 0)
            {
                formula = Regex.Replace(
                    formula, 
                    @"[A-Z]+(?<index>\d+)\b(?!\()",
                    row =>
                        {
                            var code = row.Groups["index"].Value;
                            var index = Convert.ToInt32(code);
                            return match(index)
                                       ? row.Value.Replace(code, Convert.ToString(index + rowShift))
                                       : row.Value;
                        });
            }

            return formula;
        }

        /// <summary>
        /// HSSFRow Copy Command
        /// Description:  Inserts a existing row into a new row, will automatically push down
        ///               any existing rows.  Copy is done cell by cell and supports, and the
        ///               command tries to copy all properties available (style, merged cells, values, etc...)
        /// </summary>
        /// <param name="workbook">Workbook containing the worksheet that will be changed</param>
        /// <param name="worksheet">WorkSheet containing rows to be copied</param>
        /// <param name="sourceRowNum">Source Row Number</param>
        /// <param name="destinationRowNum">Destination Row Number</param>
        public static void CopyRow(HSSFWorkbook workbook, HSSFSheet worksheet, int sourceRowNum, int destinationRowNum)
        {
            // Get the source / new row
            HSSFRow newRow = worksheet.GetRow(destinationRowNum);
            HSSFRow sourceRow = worksheet.GetRow(sourceRowNum);

            // If the row exist in destination, push down all rows by 1 else create a new row
            if (newRow != null)
            {
                worksheet.ShiftRows(destinationRowNum, worksheet.LastRowNum, 1);

                for (var i = worksheet.LastRowNum; i > destinationRowNum; i--)
                {
                    var row1 = worksheet.GetRow(i - 1);
                    var row2 = worksheet.GetRow(i);

                    if (row2 != null && row1 != null)
                    {
                        row2.Height = row1.Height;
                    }
                }
            }
            else
            {
                newRow = worksheet.CreateRow(destinationRowNum);
            }

            // Loop through source columns to add to new row
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                HSSFCell oldCell = sourceRow.GetCell(i);
                HSSFCell newCell = newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }

                // ����� ������ 4096(2003) ������ �� �����, ���� � ������ ������ ������� ����� ������, �� �� ������ ����������
                // Copy style from old cell and apply to new cell                
                // HSSFCellStyle newCellStyle = workbook.CreateCellStyle();
                // newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                // newCell.CellStyle = newCellStyle;
                newCell.CellStyle = oldCell.CellStyle;

                // If there is a cell comment, copy
                if (newCell.CellComment != null)
                {
                    newCell.CellComment = oldCell.CellComment;
                }

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null)
                {
                    newCell.Hyperlink = oldCell.Hyperlink;
                }

                // Set the cell data type
                newCell.SetCellType(oldCell.CellType);

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    case HSSFCell.CELL_TYPE_BLANK:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case HSSFCell.CELL_TYPE_BOOLEAN:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case HSSFCell.CELL_TYPE_ERROR:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case HSSFCell.CELL_TYPE_FORMULA:
                        var formula = ShiftFormulaRow(oldCell.CellFormula, destinationRowNum - sourceRowNum, row => row >= 0);
                        newCell.SetCellFormula(formula);
                        break;
                    case HSSFCell.CELL_TYPE_NUMERIC:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case HSSFCell.CELL_TYPE_STRING:
                        newCell.SetCellValue(oldCell.RichStringCellValue);
                        break;
                    default:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                }
            }

            // If there are are any merged regions in the source row, copy to new row
            for (int i = 0; i < worksheet.NumMergedRegions; i++)
            {
                CellRangeAddress cellRangeAddress = worksheet.GetMergedRegion(i);
                if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                {
                    CellRangeAddress newCellRangeAddress = new CellRangeAddress(
                        newRow.RowNum,
                        (newRow.RowNum + (cellRangeAddress.FirstRow - cellRangeAddress.LastRow)),
                        cellRangeAddress.FirstColumn,
                        cellRangeAddress.LastColumn);
                    worksheet.AddMergedRegion(newCellRangeAddress);
                }
            }
        }

        /// <summary>
        /// ���������� ������ �� �� �����.
        /// </summary>
        /// <param name="workbook">����� � ������� ��������� ������.</param>
        /// <param name="name">��� ������.</param>
        public static HSSFCell GetCellByName(HSSFWorkbook workbook, string name)
        {
            HSSFName hssfName = workbook.GetNameAt(workbook.GetNameIndex(name));
            CellReference cellRef = new CellReference(hssfName.Reference);
            return workbook
                .GetSheet(cellRef.SheetName)
                .GetRow(cellRef.Row)
                .GetCell(cellRef.Col);
        }

        /// <summary>
        /// ������������� �������� ������.
        /// </summary>
        /// <param name="sheet">���� �� ������� ����������� ������.</param>
        /// <param name="rowIndex">����� ������.</param>
        /// <param name="colIndex">����� �������.</param>
        /// <param name="value">��������������� ��������.</param>
        public static void SetCellValue(HSSFSheet sheet, int rowIndex, int colIndex, object value)
        {
            HSSFRow row = sheet.GetRow(rowIndex);
            if (row != null)
            {
                HSSFCell cell = row.GetCell(colIndex);
                if (cell != null)
                {
                    if (value is decimal || value is double)
                    {
                        cell.SetCellValue(Convert.ToDouble(value));
                    }
                    else
                    {
                        cell.SetCellValue(Convert.ToString(value));
                    }
                }
            }
        }

        /// <summary>
        /// �������� ������ �� �����������.
        /// </summary>
        /// <param name="sheet">���� �� ������� ����������� ������.</param>
        /// <param name="rowIndex">����� ������.</param>
        /// <param name="colIndex">����� �������.</param>
        public static HSSFCell GetCellByXY(HSSFSheet sheet, int rowIndex, int colIndex)
        {
            HSSFRow row = sheet.GetRow(rowIndex);
            if (row != null)
            {
                return row.GetCell(colIndex);
            }
            
            return null;
        }

        /// <summary>
        /// �������� ��������� �������� ������.
        /// </summary>
        /// <param name="sheet">���� �� ������� ����������� ������.</param>
        /// <param name="rowIndex">����� ������.</param>
        /// <param name="colIndex">����� �������.</param>
        public static string GetCellStringValue(HSSFSheet sheet, int rowIndex, int colIndex)
        {
            HSSFCell cell = GetCellByXY(sheet, rowIndex, colIndex);

            if (cell != null && cell.CellType != 0)
            {
                return cell.StringCellValue;
            }

            return String.Empty;
        }

        /// <summary>
        /// �������� ������� ������.
        /// </summary>
        /// <param name="sheet">���� �� ������� ����������� ������.</param>
        /// <param name="rowIndex">����� ������.</param>
        /// <param name="colIndex">����� �������.</param>
        public static string GetCellFormula(HSSFSheet sheet, int rowIndex, int colIndex)
        {
            HSSFCell cell = GetCellByXY(sheet, rowIndex, colIndex);
            
            if (cell != null)
            {
                return cell.CellFormula;
            }

            return String.Empty;
        }

        /// <summary>
        /// ������������� ������� ������.
        /// </summary>
        /// <param name="sheet">���� �� ������� ����������� ������.</param>
        /// <param name="rowIndex">����� ������.</param>
        /// <param name="colIndex">����� �������.</param>
        /// <param name="formula">��������������� �������� �������.</param>
        public static void SetCellFormula(HSSFSheet sheet, int rowIndex, int colIndex, string formula)
        {
            HSSFCell cell = GetCellByXY(sheet, rowIndex, colIndex);

            if (cell != null)
            {
                cell.SetCellFormula(formula);
            }
        }

        /// <summary>
        /// ����������� ��������� �����
        /// </summary>
        /// <param name="workbook">�����, � ������� ��������� ������</param>
        /// <param name="worksheet">����, �� ������� ��������� ������</param>/// 
        /// <param name="sourceRowNumStart">��������� ������ ����������� ���������</param>
        /// <param name="sourceRowNumEnd">�������� ������ ����������� ���������</param>
        /// <param name="destinationRowNum">����� ������ ��� ����������� ���������</param>
        public static void CopyRows(HSSFWorkbook workbook, HSSFSheet worksheet, int sourceRowNumStart, int sourceRowNumEnd, int destinationRowNum)
        {
            for (int i = sourceRowNumStart; i < sourceRowNumEnd; i++)
            {
                CopyRow(workbook, worksheet, i, destinationRowNum + (i - sourceRowNumStart));
            }
        }

        /// <summary>
        /// �������� ��������� �����
        /// </summary>
        /// <param name="sheet">����, �� ������� ��������� ���������� ��������</param>/// 
        /// <param name="sourceRowNumStart">��������� ������ ���������� ���������</param>
        /// <param name="sourceRowNumEnd">�������� ������ ���������� ���������</param>
        public static void DeleteRows(HSSFSheet sheet, int sourceRowNumStart, int sourceRowNumEnd)
        {
            // ����� ����� ���������� �������� �������� � ��������� ������ �����
            int rowCount = sourceRowNumEnd - sourceRowNumStart + 1;
            int colCount = sheet.GetRow(sourceRowNumStart).PhysicalNumberOfCells;
            short newRowHeight = Convert.ToByte(sheet.GetRow(sourceRowNumStart).Height / rowCount);
            for (int i = sourceRowNumEnd; i >= sourceRowNumStart; i--)
            {
                HSSFRow row = sheet.GetRow(i);
                if (row != null)
                {
                    row.RemoveAllCells();
                    row.Height = newRowHeight;
                }
            }

            for (int kk = 0; kk < colCount; kk++)
            {
                sheet.AddMergedRegion(new CellRangeAddress(sourceRowNumStart, sourceRowNumEnd, kk, kk));
            }
        }

        /// <summary>
        /// �������� ��������� �����
        /// </summary>
        public static void DropRows(HSSFSheet sheet, int startRow, int endRow)
        {
            for (var i = endRow; i >= startRow; i--)
            {
                var row = sheet.GetRow(i);
                if (row != null)
                {
                    sheet.RemoveRow(sheet.GetRow(i));
                }
            }

            // sheet.ShiftRows(endRow + 1, sheet.LastRowNum, endRow - startRow + 1);
        }

        /// <summary>
        /// �������� ��������� �����
        /// </summary>
        public static void DeleteRow(HSSFSheet sheet, int rowIndex)
        {
            var row = sheet.GetRow(rowIndex);

            if (row != null)
            {
                sheet.RemoveRow(row);

                for (var i = 0; i < 255; i++)
                {
                    var cell = row.GetCell(i);

                    if (cell != null)
                    {
                        cell.SetCellValue(String.Empty);
                    }
                }
            }

            sheet.ShiftRows(rowIndex + 1, sheet.LastRowNum, -1);
        }

        public static List<HSSFSheet> CopySheets(HSSFWorkbook srcWb, HSSFWorkbook dstWb)
        {
            var newSheets = new List<HSSFSheet>();

            for (var j = 0; j < srcWb.NumberOfSheets; j++)
            {
                var maxColumnNum = 0;
                var srcSheet = srcWb.GetSheetAt(j);
                var newSheetName = srcWb.GetSheetName(srcWb.GetSheetIndex(srcSheet));

                if (newSheetName.Length > 24)
                {
                    newSheetName = newSheetName.Substring(0, 24);
                }

                newSheetName = String.Format("���{0}_{1}", Convert.ToString(dstWb.NumberOfSheets).PadLeft(2, '0'), newSheetName);
                var newSheet = dstWb.CreateSheet(newSheetName);
                var styleMap = new Dictionary<int, HSSFCellStyle>();

                for (var i = srcSheet.FirstRowNum; i <= srcSheet.LastRowNum; i++)
                {
                    var srcRow = srcSheet.GetRow(i);
                    var destRow = newSheet.CreateRow(i);

                    if (srcRow == null)
                    {
                        continue;
                    }

                    CopyRow(dstWb, srcSheet, newSheet, srcRow, destRow, styleMap);

                    if (srcRow.LastCellNum > maxColumnNum)
                    {
                        maxColumnNum = srcRow.LastCellNum;
                    }
                }

                for (var i = 0; i <= maxColumnNum; i++)
                {
                    newSheet.SetColumnWidth(i, srcSheet.GetColumnWidth(i));
                }

                newSheet.ForceFormulaRecalculation = true;
            }

            return newSheets;
        }

        private static void CopyRow(
            HSSFWorkbook dstWb,
            HSSFSheet srcSheet,
            HSSFSheet dstSheet,
            HSSFRow srcRow,
            HSSFRow destRow,
            Dictionary<int, HSSFCellStyle> styleMap)
        {
            destRow.Height = srcRow.Height;

            for (var j = srcRow.FirstCellNum; j <= srcRow.LastCellNum; j++)
            {
                var oldCell = srcRow.GetCell(j);
                var newCell = destRow.GetCell(j);

                if (oldCell == null)
                {
                    continue;
                }

                if (newCell == null)
                {
                    newCell = destRow.CreateCell(j);
                }

                CopyCell(dstWb, oldCell, newCell, styleMap);
                var mergedRegion = GetMergedRegion(srcSheet, srcRow.RowNum, (short)oldCell.ColumnIndex);

                if (mergedRegion == null)
                {
                    continue;
                }

                dstSheet.AddMergedRegion(mergedRegion);
            }
        }

        private static void CopyCell(
            HSSFWorkbook wb,
            HSSFCell oldCell,
            HSSFCell newCell,
            IDictionary<int, HSSFCellStyle> styleMap)
        {
            if (oldCell.CellStyle != null)
            {
                var styleHashCode = oldCell.CellStyle.GetHashCode();

                if (!styleMap.ContainsKey(styleHashCode))
                {
                    var newCellStyle = wb.CreateCellStyle();
                    newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                    styleMap.Add(styleHashCode, newCellStyle);
                }

                newCell.CellStyle = styleMap[styleHashCode];
                newCell.CellStyle.FillBackgroundColor = HSSFColor.CORAL.index;
            }
            else
            {
                newCell.CellStyle = null;
            }

            switch (oldCell.CellType)
            {
                case HSSFCell.CELL_TYPE_STRING:
                    newCell.SetCellValue(oldCell.StringCellValue);
                    break;
                case HSSFCell.CELL_TYPE_NUMERIC:
                    newCell.SetCellValue(oldCell.NumericCellValue);
                    break;
                case HSSFCell.CELL_TYPE_BLANK:
                    newCell.SetCellType(HSSFCell.CELL_TYPE_BLANK);
                    break;
                case HSSFCell.CELL_TYPE_BOOLEAN:
                    newCell.SetCellValue(oldCell.BooleanCellValue);
                    break;
                case HSSFCell.CELL_TYPE_ERROR:
                    newCell.SetCellValue(oldCell.ErrorCellValue);
                    break;
                case HSSFCell.CELL_TYPE_FORMULA:
                    newCell.SetCellType(HSSFCell.CELL_TYPE_FORMULA);
                    newCell.SetCellFormula(oldCell.CellFormula);
                    break;
                default:
                    break;
            }
        }

        private static CellRangeAddress GetMergedRegion(HSSFSheet sheet, int rowNum, short cellNum)
        {
            for (var i = 0; i < sheet.NumMergedRegions; i++)
            {
                var region = sheet.GetMergedRegion(i);

                if (rowNum == region.FirstRow && cellNum == region.FirstColumn)
                {
                    return region;
                }
            }

            return null;
        }
    }
}