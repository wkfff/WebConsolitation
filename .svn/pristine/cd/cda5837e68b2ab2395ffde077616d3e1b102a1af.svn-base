using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports
{
    public class NPOIHelper
    {
        public const int MaxRowNum = 65535;     // максимальный номер строки
        public const int MaxColumnNum = 255;    // максимальный номер колонки
        public const int UndefinedIndex = -1;

        /// <summary>
        /// Возвращает номер колонки по её буквенному коду
        /// </summary>
        /// <param name="columnCode">Буквенный код</param>
        public static int GetColumnIndex(string columnCode)
        {
            const int basa = 'Z' - 'A' + 1;
            const int offset = 'A' - 1;
            return columnCode.Aggregate(0, (current, c) => current*basa + c - offset) - 1;
        }

        /// <summary>
        /// Возвращает буквенный код колонки по её номеру
        /// </summary>
        /// <param name="columnIndex">Номер колонки</param>
        public static string GetColumnCode(int columnIndex)
        {
            const int basa = 'Z' - 'A' + 1;
            const int offset = 'A';
            var name = new StringBuilder();
            while (columnIndex >= 0)
            {
                name.Insert(0, ((char)(columnIndex % basa + offset)));
                columnIndex = columnIndex / basa - 1;
            }

            return name.ToString();
        }

        /// <summary>
        /// Возвращает копию формулы со смещёнными индексами колонок
        /// </summary>
        /// <param name="formula">Формула</param>
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
                const string indexRegExp = @"(?<!\$)(?<code>[A-Z]+)\$?\d+\b(?!\()";
                formula = Regex.Replace(formula, indexRegExp, column =>
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
        /// <param name="formula">Формула</param>
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
                const string indexRegExp = @"[A-Z]+(?<index>\d+)\b(?!\()";
                formula = Regex.Replace(formula, indexRegExp, row =>
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
        /// Возвращает копию формулы со смещёнными индексами строк и колонок
        /// </summary>
        /// <param name="formula">Формула</param>
        /// <param name="rowShift">Смещение строки</param>
        /// <param name="columnShift">Смещение колонки</param>
        public static string GetFormulaCopy(string formula, int rowShift, int columnShift)
        {
            formula = ShiftFormulaColumn(formula, columnShift, column => column >= 0);
            return ShiftFormulaRow(formula, rowShift, row => row >= 0);
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
        /// <param name="cloneRowNum">Копией какой строки эта строка является</param>/// 
        public static void CopyRow(HSSFWorkbook workbook, HSSFSheet worksheet, int sourceRowNum, int destinationRowNum, int cloneRowNum = -1)
        {
            // Get the source / new row
            var newRow = worksheet.GetRow(destinationRowNum);
            var sourceRow = worksheet.GetRow(sourceRowNum);

            // If the row exist in destination, push down all rows by 1 else create a new row
            if (newRow != null)
            {
                worksheet.ShiftRows(destinationRowNum, worksheet.LastRowNum, 1);
            }
            else
            {
                newRow = worksheet.CreateRow(destinationRowNum);
            }

            newRow.Height = sourceRow.Height;

            // Loop through source columns to add to new row
            for (var i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                var oldCell = sourceRow.GetCell(i);
                var newCell = newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    continue;
                }

                // Лимит экселя 4096(2003) стилей на книгу, если в каждой клетке плодить клоны стилей, то он быстро выбирается
                if (cloneRowNum >= 0)
                {
                    var row = worksheet.GetRow(cloneRowNum);
                    var cloneCell = row.GetCell(i);
                    newCell.CellStyle = cloneCell.CellStyle;
                }
                else
                {
                    newCell.CellStyle = oldCell.CellStyle;                    
                }

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
                        break;
                    case HSSFCell.CELL_TYPE_BOOLEAN:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case HSSFCell.CELL_TYPE_ERROR:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case HSSFCell.CELL_TYPE_FORMULA:
                        var formula = GetFormulaCopy(oldCell.CellFormula, destinationRowNum - sourceRowNum, 0);
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
            for (var i = 0; i < worksheet.NumMergedRegions; i++)
            {
                var cellRangeAddress = worksheet.GetMergedRegion(i);

                if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                {
                    var newCellRangeAddress = new CellRangeAddress(
                        newRow.RowNum,
                        (newRow.RowNum + (cellRangeAddress.FirstRow - cellRangeAddress.LastRow)),
                        cellRangeAddress.FirstColumn,
                        cellRangeAddress.LastColumn);
                    worksheet.AddMergedRegion(newCellRangeAddress);
                }
            }
        }

        /// <summary>
        /// Сдвигает строки с сохранением параметров и областей объединения
        /// </summary>
        /// <param name="sheet">Лист на котором расположены строки.</param>
        /// <param name="startRow">Индекс первой строки.</param>
        /// <param name="endRow">Индекс последней строки.</param>
        /// <param name="shift">Величина сдвига</param>
        public static void ShiftRows(HSSFSheet sheet, int startRow, int endRow, int shift)
        {
            if (startRow > endRow || shift == 0 || startRow + shift < 0 || endRow + shift > MaxRowNum)
            {
                return;
            }

            // смещаем области объединения сдвигаемых строк 
            var newStartRow = startRow + shift;
            var newEndRow = endRow + shift;

            for (var i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                var reg = sheet.GetMergedRegionAt(i);
                if (reg == null) continue;

                if (reg.RowFrom >= startRow && reg.RowTo <= endRow)
                {
                    reg.RowFrom = reg.RowFrom + shift;
                    reg.RowTo = reg.RowTo + shift;
                    continue;
                }

                if (reg.RowFrom >= newStartRow && reg.RowTo <= newEndRow)
                {
                    sheet.RemoveMergedRegion(i);
                }
            }

            // сдвигаем строки
            sheet.ShiftRows(startRow, endRow, shift, true, true, true);

            // удаляем то, что осталось после сдвига строк, иначе глючит
            var count = Math.Min(Math.Abs(shift), endRow - startRow + 1);
            var startDelRow = shift > 0 ? startRow : endRow - count + 1;

            for (var i = 0; i < count; i++)
            {
                var row = sheet.GetRow(startDelRow + i);
                if (row != null)
                {
                    sheet.RemoveRow(row);
                }
            }
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
            var row = sheet.GetRow(rowIndex);

            if (row != null)
            {
                var cell = row.GetCell(colIndex);

                if (cell != null)
                {
                    if (value is decimal || value is double || value is int)
                    {
                        cell.SetCellValue(Convert.ToDouble(value));
                    }
                    else if (value == null || value == DBNull.Value)
                    {
                        cell.SetCellType(HSSFCell.CELL_TYPE_BLANK); 
                    }
                    else if (value is DateTime)
                    {
                        cell.SetCellValue(Convert.ToDateTime(value));
                    }
                    else
                    {
                        cell.SetCellValue(Convert.ToString(value));
                    }
                }
            }
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
            var cell = GetCellByXY(sheet, rowIndex, colIndex);

            if (cell != null)
            {
                cell.SetCellFormula(formula);
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
            var row = sheet.GetRow(rowIndex);
            return row != null ? row.GetCell(colIndex) : null;
        }

        /// <summary>
        /// Получает строковое значение ячейки.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
        public static string GetCellStringValue(HSSFSheet sheet, int rowIndex, int colIndex)
        {
            var cell = GetCellByXY(sheet, rowIndex, colIndex);

            if (cell != null && cell.CellType != HSSFCell.CELL_TYPE_NUMERIC && cell.CellType != HSSFCell.CELL_TYPE_FORMULA)
            {
                return cell.StringCellValue;
            }

            return String.Empty;
        }

        public enum FormatAction
        {
            FontBold,
            FontItalic,
            FontNonItalic,
            FontNormal,
            BlueBG
        }

        public class RangeParams
        {
            public HSSFWorkbook wb { get; set; }
            public HSSFSheet sheet { get; set; }
            public FormatAction action { get; set; }
            public IEnumerable<int> rowsList { get; set; }
            public IEnumerable<int> colsList { get; set; }
        }

        /// <summary>
        /// Установка параметров оформления диапазона
        /// </summary>
        public static void SetRangeAction(RangeParams rangeParams)
        {
            foreach (var i in rangeParams.rowsList)
            {
                foreach (var j in rangeParams.colsList)
                {
                    var cell = GetCellByXY(rangeParams.sheet, i, j);

                    if (cell == null)
                    {
                        continue;
                    }

                    var font = cell.CellStyle.GetFont(rangeParams.wb);

                    if (font == null)
                    {
                        continue;
                    }

                    var newFont = CloneFont(rangeParams.wb, font);
                    var isNewFontStyle = rangeParams.action != FormatAction.BlueBG;

                    switch (rangeParams.action)
                    {
                        case FormatAction.FontBold:
                            newFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
                            break;
                        case FormatAction.FontItalic:
                            newFont.IsItalic = true;
                            break;
                        case FormatAction.FontNonItalic:
                            newFont.IsItalic = false;
                            break;
                        case FormatAction.FontNormal:
                            newFont.Boldweight = HSSFFont.BOLDWEIGHT_NORMAL;
                            break;
                        case FormatAction.BlueBG:
                            cell.CellStyle.FillForegroundColor = 170;
                            cell.CellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
                            break;
                    }

                    if (isNewFontStyle)
                    {
                        cell.CellStyle.SetFont(newFont);
                    }
                }
            }
        }

        private static HSSFFont CloneFont(HSSFWorkbook wb, HSSFFont font)
        {
            var newFont = wb.CreateFont();
            newFont.Boldweight = font.Boldweight;
            newFont.CharSet = font.CharSet;
            newFont.Color = font.Color;
            newFont.FontHeight = font.FontHeight;
            newFont.FontHeightInPoints = font.FontHeightInPoints;
            newFont.FontName = font.FontName;
            newFont.IsItalic = font.IsItalic;
            newFont.IsStrikeout = font.IsStrikeout;
            newFont.TypeOffset = font.TypeOffset;
            newFont.Underline = font.Underline;
            return newFont;
        }

        /// <summary>
        /// Вставка колонки
        /// </summary>
        public static void InsertColumn(HSSFSheet sheet, int columnIndex)
        {
            // сдвигаем все колонки вправо
            var lastColumn = -1;

            for (var i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }

                for (var j = 0; j < columnIndex; j++)
                {
                    var cell = row.GetCell(j);
                    if (cell != null && cell.CellType == HSSFCell.CELL_TYPE_FORMULA)
                    {
                        var formula = ShiftFormulaColumn(cell.CellFormula, 1, column => column >= columnIndex);
                        cell.SetCellFormula(formula);
                    }
                }

                for (var j = row.LastCellNum - 1; j >= columnIndex; j--)
                {
                    if (j >= MaxColumnNum)
                    {
                        continue;
                    }

                    var cell1 = row.GetCell(j);
                    var cell2 = row.GetCell(j + 1);

                    if (cell1 != null && cell2 == null)
                    {
                        cell2 = row.CreateCell(j + 1);
                    }

                    if (cell1 != null && cell1.CellType == HSSFCell.CELL_TYPE_FORMULA)
                    {
                        var formula = ShiftFormulaColumn(cell1.CellFormula, -1, column => column < columnIndex);
                        cell1.SetCellFormula(formula);
                    }

                    CopyCell(cell1, cell2);
                }

                if (lastColumn < row.LastCellNum - 1)
                {
                    lastColumn = row.LastCellNum - 1;
                }
            }

            // восстанавливаем параметры сдвинутых колонок
            for (var i = columnIndex; i < lastColumn; i++)
            {
                sheet.SetColumnWidth(i + 1, sheet.GetColumnWidth(i));
                sheet.SetColumnHidden(i + 1, sheet.IsColumnHidden(i));
            }

            // раздвигаем области объединения
            for (var i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                var reg = sheet.GetMergedRegion(i);
                if (reg == null)
                {
                    continue;
                }

                if (reg.FirstColumn >= columnIndex)
                {
                    reg.FirstColumn++;
                }

                if (reg.LastColumn >= columnIndex)
                {
                    reg.LastColumn++;
                }
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
        }

        /// <summary>
        /// Удаление колонки
        /// </summary>
        public static void DeleteColumn(HSSFWorkbook wb, HSSFSheet sheet, int columnIndex)
        {
            // удаляем области объединения сохраняя их параметры 
            var regs = new List<Region>();
            var values = new Dictionary<int, object>();
            var borders = new Dictionary<int, List<short>>();
            var lastMergedNum = sheet.NumMergedRegions - 1;
            for (var i = lastMergedNum; i >= 0; i--)
            {
                var reg = sheet.GetMergedRegionAt(i);
                if (reg == null)
                {
                    continue;
                }

                if (reg.ColumnFrom >= columnIndex || reg.ColumnTo >= columnIndex)
                {
                    if (reg.ColumnFrom == columnIndex)
                    {
                        var cell = GetCellByXY(sheet, reg.RowFrom, reg.ColumnFrom);
                        values.Add(regs.Count, GetCellValue(cell));
                    }
                    if (reg.ColumnTo == columnIndex)
                    {
                        var listBorders = new List<short>();
                        for (var n = reg.RowFrom; n <= reg.RowTo; n++)
                        {
                            var cell = GetCellByXY(sheet, n, columnIndex);
                            var borderRight = cell != null ? cell.CellStyle.BorderRight : HSSFCellStyle.BORDER_NONE;
                            listBorders.Add(borderRight);
                        }

                        borders.Add(regs.Count, listBorders);
                    }

                    regs.Add(reg);
                    sheet.RemoveMergedRegion(i);
                }
            }

            // сдвигаем все колонки влево
            var lastColumn = -1;

            for (var i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }

                for (var j = 0; j < columnIndex; j++)
                {
                    var cell = row.GetCell(j);
                    if (cell != null && cell.CellType == HSSFCell.CELL_TYPE_FORMULA)
                    {
                        Predicate<int> match = column => column >= columnIndex && column > cell.ColumnIndex + 1;
                        var formula = ShiftFormulaColumn(cell.CellFormula, -1, match);
                        cell.SetCellFormula(formula);
                    }
                }

                for (var j = columnIndex + 1; j < row.LastCellNum; j++)
                {
                    var cell1 = row.GetCell(j);
                    var cell2 = row.GetCell(j - 1);

                    if (cell1 != null && cell2 == null)
                    {
                        cell2 = row.CreateCell(j - 1);
                    }

                    if (cell1 != null && cell1.CellType == HSSFCell.CELL_TYPE_FORMULA)
                    {
                        var formula = ShiftFormulaColumn(cell1.CellFormula, 1, column => column < columnIndex);
                        cell1.SetCellFormula(formula);
                    }

                    CopyCell(cell1, cell2);
                }

                if (row.LastCellNum > lastColumn + 1)
                {
                    lastColumn = row.LastCellNum - 1;
                }

                if (row.LastCellNum > columnIndex)
                {
                    var cell = row.GetCell(row.LastCellNum - 1);
                    if (cell != null)
                    {
                        row.RemoveCell(cell);
                    }
                }
            }

            // восстанавливаем параметры сдвинутых колонок
            for (var i = columnIndex; i < lastColumn; i++)
            {
                sheet.SetColumnWidth(i, sheet.GetColumnWidth(i + 1));
                sheet.SetColumnHidden(i, sheet.IsColumnHidden(i + 1));
            }

            // восстанавливаем области объединения
            for (var i = 0; i < regs.Count; i++)
            {
                var reg = regs[i];
                if (reg.ColumnFrom > columnIndex)
                {
                    reg.ColumnFrom--;
                }
                if (reg.ColumnTo >= columnIndex)
                {
                    reg.ColumnTo--;
                }

                if (reg.ColumnFrom <= reg.ColumnTo)
                {
                    if (values.ContainsKey(i))
                    {
                        SetCellValue(sheet, reg.RowFrom, reg.ColumnFrom, values[i]);
                    }
                    if (reg.ColumnFrom != reg.ColumnTo || reg.RowFrom != reg.RowTo)
                    {
                        if (borders.ContainsKey(i))
                        {
                            for (var n = reg.RowFrom; n <= reg.RowTo; n++)
                            {
                                var cell = GetCellByXY(sheet, n, reg.ColumnTo);
                                var border = borders[i][n - reg.RowFrom];
                                if (cell != null && cell.CellStyle.BorderRight != border)
                                {
                                    cell.CellStyle = GetCellStyle(wb, cell.CellStyle, CellStyleProperty.BorderRight, border);
                                }
                            }
                        }
                        sheet.AddMergedRegion(reg);
                    }
                    else
                    {
                        var cell = GetCellByXY(sheet, reg.RowFrom, reg.ColumnFrom);
                        if (cell != null)
                        {
                            cell.CellStyle.WrapText = true;
                        }
                    }
                }
            }
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
                    var formula = GetFormulaCopy(oldCell.CellFormula,
                                                 newCell.RowIndex - oldCell.RowIndex,
                                                 newCell.ColumnIndex - oldCell.ColumnIndex);
                    newCell.SetCellFormula(formula);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Получение значения ячейки
        /// </summary>
        public static object GetCellValue(HSSFCell cell)
        {
            if (cell == null)
            {
                return null;
            }

            switch (cell.CellType)
            {
                case HSSFCell.CELL_TYPE_STRING:
                    return cell.StringCellValue;
                case HSSFCell.CELL_TYPE_NUMERIC:
                    return cell.NumericCellValue;
                case HSSFCell.CELL_TYPE_BOOLEAN:
                    return cell.BooleanCellValue;
                case HSSFCell.CELL_TYPE_FORMULA:
                    return cell.CellFormula;
            }

            return null;
        }

        /// <summary>
        /// Получение индекса области объединения ячейки
        /// </summary>
        public static int CellMergedRegion(HSSFSheet sheet, int rowIndex, int columnIndex)
        {
            for (var n = 0; n < sheet.NumMergedRegions; n++)
            {
                var reg = sheet.GetMergedRegionAt(n);
                if (reg == null)
                {
                    continue;
                }

                if (rowIndex >= reg.RowFrom &&
                    rowIndex <= reg.RowTo &&
                    columnIndex >= reg.ColumnFrom &&
                    columnIndex <= reg.ColumnTo)
                {
                    return n;
                }
            }

            return UndefinedIndex;
        }

        /// <summary>
        /// Удаление области объединения ячейки
        /// </summary>
        public static void RemoveCellMergedRegion(HSSFSheet sheet, int rowIndex, int columnIndex)
        {
            var regIndex = CellMergedRegion(sheet, rowIndex, columnIndex);
            if (regIndex != UndefinedIndex)
            {
                sheet.RemoveMergedRegion(regIndex);
            }
        }

        /// <summary>
        /// Получение индекса области объединения ячейки
        /// </summary>
        public static int CellMergedRegion(HSSFCell cell)
        {
            if (cell == null)
            {
                return UndefinedIndex;
            }

            var sheet = cell.Sheet;
            return CellMergedRegion(sheet, cell.RowIndex, cell.ColumnIndex);
        }

        /// <summary>
        /// Проверяет эквивалентны ли свойства стилей
        /// <para/>
        /// <param name=" skipProp">skipProp</param>: Наименование свойства, исключаемого из сравнения
        /// </summary>
        private static bool IsEqualCellStyles(HSSFCellStyle style1, HSSFCellStyle style2, string skipProp)
        {
            var notCompareProps = new List<string>
            {
                CellStyleProperty.Index,
                CellStyleProperty.ParentStyle,
                CellStyleProperty.UserStyleName
            };

            if (skipProp != null)
            {
                notCompareProps.Add(skipProp);
            }

            var type = typeof(HSSFCellStyle);
            var props = type.GetProperties();
            foreach (var property in props)
            {
                if (notCompareProps.Contains(property.Name))
                {
                    continue;
                }

                var value = property.GetValue(style1, null);
                if (!value.Equals(property.GetValue(style2, null)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Возвращает стиль со свойствами как у заданного за исключением свойства propName
        /// </summary>
        public static HSSFCellStyle GetCellStyle(HSSFWorkbook wb, HSSFCellStyle oldStyle, string propName, object propValue)
        {
            if (oldStyle == null)
            {
                return null;
            }
            
            var type = typeof(HSSFCellStyle);
            var numCellStyles = wb.NumCellStyles;
            for (short i = 0; i < numCellStyles; i++)
            {
                var style = wb.GetCellStyleAt(i);
                if (style == null)
                {
                    continue;
                }

                if (!IsEqualCellStyles(style, oldStyle, propName))
                {
                    continue;
                }

                var stylePropValue = type.GetProperty(propName).GetValue(style, null);
                if (Equals(stylePropValue, propValue))
                {
                    return style;
                }
            }
            
            var newStyle = wb.CreateCellStyle();
            newStyle.CloneStyleFrom(oldStyle);
            type.GetProperty(propName).SetValue(newStyle, propValue, null);
            return newStyle;
        }

        /// <summary>
        /// Копирует лист. (пришлось написать т.к. CloneSheet не работает, если на листе есть формулы)
        /// </summary>
        public static HSSFSheet CopySheet(HSSFWorkbook wb, int sheetIndex)
        {
            var sourceSheet = wb.GetSheetAt(sheetIndex);
            var cells = new Dictionary<HSSFCell, string>();

            for (var i = sourceSheet.FirstRowNum; i <= sourceSheet.LastRowNum; i++)
            {
                var row = sourceSheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }

                for (var j = row.FirstCellNum; j < row.LastCellNum; j++)
                {
                    var cell = row.GetCell(j);
                    if (cell == null || cell.CellType != HSSFCell.CELL_TYPE_FORMULA)
                    {
                        continue;
                    }

                    cells.Add(cell, cell.CellFormula);
                    cell.SetCellType(HSSFCell.CELL_TYPE_BLANK);
                }
            }

            var destSheet = wb.CloneSheet(sheetIndex);

            foreach (var cell in cells)
            {
                cell.Key.SetCellType(HSSFCell.CELL_TYPE_FORMULA);
                cell.Key.SetCellFormula(cell.Value);

                var destCell = GetCellByXY(destSheet, cell.Key.RowIndex, cell.Key.ColumnIndex);
                destCell.SetCellType(HSSFCell.CELL_TYPE_FORMULA);
                destCell.SetCellFormula(cell.Value);
            }

            return destSheet;
        }
    }

    public class CellStyleProperty
    {
        public const string Index = "Index";
        public const string BorderRight = "BorderRight";
        public const string BorderLeft = "BorderLeft";
        public const string UserStyleName = "UserStyleName";
        public const string ParentStyle = "ParentStyle";
        public const string DataFormat = "DataFormat";
    }
}
