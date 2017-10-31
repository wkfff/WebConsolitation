using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public static class NPOIHelper
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
                        (newRow.RowNum + (cellRangeAddress.LastRow - cellRangeAddress.FirstRow)),
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
        /// Копирование колонки
        /// </summary>
        public static void CopyColumn(HSSFSheet sheet, int sourceColumn, int destColumn)
        {
            for (var i = 0; i <= sheet.LastRowNum; i++)
            {
                var sourceCell = GetCellByXY(sheet, i, sourceColumn);
                var destCell = GetCellByXY(sheet, i, destColumn);
                if (destCell == null && sourceCell != null)
                {
                    destCell = sheet.GetRow(i).CreateCell(destColumn);
                }

                CopyCell(sourceCell, destCell);
            }

            sheet.SetColumnWidth(destColumn, sheet.GetColumnWidth(sourceColumn));
        }

        /// <summary>
        /// Копирование клетки со стилем и значением
        /// </summary>
        public static void CopyCell(HSSFCell oldCell, HSSFCell newCell)
        {
            if (newCell == null)
            {
                return;
            }

            if (oldCell == null)
            {
                newCell.Sheet.GetRow(newCell.RowIndex).RemoveCell(newCell);
                return;
            }

            newCell.CellStyle = oldCell.CellStyle;
            newCell.SetCellType(HSSFCell.CELL_TYPE_BLANK); // чтобы обнулить значение ячейки
            newCell.SetCellType(oldCell.CellType);

            switch (oldCell.CellType)
            {
                case HSSFCell.CELL_TYPE_STRING:
                    newCell.SetCellValue(oldCell.StringCellValue);
                    break;
                case HSSFCell.CELL_TYPE_NUMERIC:
                    newCell.SetCellValue(oldCell.NumericCellValue);
                    break;
                case HSSFCell.CELL_TYPE_BOOLEAN:
                    newCell.SetCellValue(oldCell.BooleanCellValue);
                    break;
                case HSSFCell.CELL_TYPE_FORMULA:
                    var formula = GetFormulaCopy(oldCell.CellFormula, newCell.RowIndex - oldCell.RowIndex, newCell.ColumnIndex - oldCell.ColumnIndex);
                    newCell.SetCellFormula(formula);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Возвращает копию формулы со смещёнными индексами строк и колонок
        /// </summary>
        /// <param name="formula">Формула выражения</param>
        /// <param name="rowShift">Смещение строки</param>
        /// <param name="columnShift">Смещение колонки</param>
        public static string GetFormulaCopy(string formula, int rowShift, int columnShift)
        {
            formula = ShiftFormulaColumn(formula, columnShift, column => column >= 0);
            return ShiftFormulaRow(formula, rowShift, row => row >= 0);
        }

        /// <summary>
        /// Возвращает копию формулы со смещёнными индексами колонок
        /// </summary>
        /// <param name="formula">Формула выраженеия</param>
        /// <param name="columnShift">Смещение колонки</param>
        /// <param name="match">Делегат, определяющий условия поиска индекса сдвигаемой колонки</param>
        public static string ShiftFormulaColumn(string formula, int columnShift, Predicate<int> match)
        {
            if (String.IsNullOrEmpty(formula))
            {
                return formula;
            }

            if (columnShift != 0)
            {
                // Ищем соответствия, удовлетворяющие следующим условиям:
                //      - начинаются с одного или более символов A-Z, но не с $ (последовательность букв именуется "code")
                //      - далее идут одна или более цифр (перед цифрами возможно появление $)
                //      - после цифр должна быть граница (любые не алфавитно-цифровые символы), но не открывающая скобка
                const string IndexRegExp = @"(?<!\$)(?<code>[A-Z]+)\$?\d+\b(?!\()";
                formula = Regex.Replace(
                    formula, 
                    IndexRegExp, 
                    column =>
                {
                    var code = column.Groups["code"].Value;
                    var index = GetColumnIndex(code);
                    return match(index)
                               ? column.Value.Replace(code, Convert.ToString(GetColumnCode(index + columnShift)))
                               : column.Value;
                });
            }

            return formula;
        }

        /// <summary>
        /// Возвращает копию формулы со смещёнными индексами строк
        /// </summary>
        /// <param name="formula">Формула выражения</param>
        /// <param name="rowShift">Смещение строки</param>
        /// <param name="match">Делегат, определяющий условия поиска индекса сдвигаемой строки</param>
        public static string ShiftFormulaRow(string formula, int rowShift, Predicate<int> match)
        {
            if (String.IsNullOrEmpty(formula))
            {
                return formula;
            }

            if (rowShift != 0)
            {
                // Ищем соответствия, удовлетворяющие следующим условиям:
                //      - начинаются с одного или более символов A-Z
                //      - далее идут одна или более цифр (последовательность цифр именуется "index")
                //      - после цифр должна быть граница (любые не алфавитно-цифровые символы), но не открывающая скобка
                const string IndexRegExp = @"[A-Z]+(?<index>\d+)\b(?!\()";
                formula = Regex.Replace(
                    formula, 
                    IndexRegExp, 
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
        /// Возвращает номер колонки по её буквенному коду
        /// </summary>
        /// <param name="columnCode">Буквенный код</param>
        public static int GetColumnIndex(string columnCode)
        {
            const int Basa = 'Z' - 'A' + 1;
            const int Offset = 'A' - 1;
            return columnCode.Aggregate(0, (current, c) => (current * Basa) + c - Offset) - 1;
        }

        /// <summary>
        /// Возвращает буквенный код колонки по её номеру
        /// </summary>
        /// <param name="columnIndex">Номер колонки</param>
        public static string GetColumnCode(int columnIndex)
        {
            const int Basa = 'Z' - 'A' + 1;
            const int Offset = 'A';
            var name = new StringBuilder();
            while (columnIndex >= 0)
            {
                name.Insert(0, ((char)((columnIndex % Basa) + Offset)));
                columnIndex = (columnIndex / Basa) - 1;
            }

            return name.ToString();
        }
    }
}
