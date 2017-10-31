using System;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class NPOIHelper
    {
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

                // Лимит экселя 4096(2003) стилей на книгу, если в каждой клетке плодить клоны стилей, то он быстро выбирается
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
                        newCell.SetCellFormula(oldCell.CellFormula);
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
        /// Возвращает ячейку по ее имени.
        /// </summary>
        /// <param name="workbook">Книга в которой находится ячейка.</param>
        /// <param name="name">Имя ячейки.</param>
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
        /// Устанавливает значение ячейки.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
        /// <param name="value">Устанавливаемое значение.</param>
        public static void SetCellValue(HSSFSheet sheet, int rowIndex, int colIndex, object value)
        {
            HSSFCell cell = GetCellByXY(sheet, rowIndex, colIndex);
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

        /// <summary>
        /// Устанавливает значение ячейки и формат.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
        /// <param name="value">Устанавливаемое значение.</param>
        /// <param name="cellStyle">Формат ячейки.</param>
        public static void SetCellValue(HSSFSheet sheet, int rowIndex, int colIndex, object value, HSSFCellStyle cellStyle)
        {
            HSSFCell cell = GetCellByXY(sheet, rowIndex, colIndex);
            if (cell != null)
            {
                cell.CellStyle = cellStyle;

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

        /// <summary>
        /// Получает ячейку по координатам.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
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
        /// Получает строковое значение ячейки.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
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
        /// Получает формулу ячейки.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
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
        /// Устанавливает формулу ячейки.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
        /// <param name="formula">Устанавливаемое значение формулы.</param>
        public static void SetCellFormula(HSSFSheet sheet, int rowIndex, int colIndex, string formula)
        {
            HSSFCell cell = GetCellByXY(sheet, rowIndex, colIndex);

            if (cell != null)
            {
                cell.SetCellFormula(formula);
            }
        }

        /// <summary>
        /// Копирование диапазона строк
        /// </summary>
        /// <param name="workbook">Книга, в которой находится ячейка</param>
        /// <param name="worksheet">Лист, на котором находится ячейка</param>/// 
        /// <param name="sourceRowNumStart">Стартовая строка копируемого диапазона</param>
        /// <param name="sourceRowNumEnd">Конечная строка копируемого диапазона</param>
        /// <param name="destinationRowNum">Номер строки для копирования диапазона</param>
        public static void CopyRows(HSSFWorkbook workbook, HSSFSheet worksheet, int sourceRowNumStart, int sourceRowNumEnd, int destinationRowNum)
        {
            for (int i = sourceRowNumStart; i < sourceRowNumEnd; i++)
            {
                CopyRow(workbook, worksheet, i, destinationRowNum + (i - sourceRowNumStart));
            }
        }

        /// <summary>
        /// Удаление диапазона строк
        /// </summary>
        /// <param name="sheet">Лист, на котором находится копируемый диапазон</param>/// 
        /// <param name="sourceRowNumStart">Стартовая строка удаляемого диапазона</param>
        /// <param name="sourceRowNumEnd">Конечная строка удаляемого диапазона</param>
        public static void DeleteRows(HSSFSheet sheet, int sourceRowNumStart, int sourceRowNumEnd)
        {
            // ввиду глюка компонента удаление сводится к установке высоты строк
            int rowCount = sourceRowNumEnd - sourceRowNumStart + 1;
            int colCount = sheet.GetRow(sourceRowNumStart).PhysicalNumberOfCells;
            var newRowHeight = (short)(sheet.GetRow(sourceRowNumStart).Height / rowCount);
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
    }
}
