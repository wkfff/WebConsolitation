using System;
using System.Globalization;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    public class NpoiHelper
    {
        /// <summary>
        /// Числовой без дробной части
        /// </summary>
        public const short DataFormatInt = 1;

        /// <summary>
        /// Числовой с 2я знаками после запятой
        /// </summary>
        public const short DataFormatFloat = 2;

        /// <summary>
        /// Возвращает ячейку по ее имени.
        /// </summary>
        /// <param name="workbook">Книга в которой находится ячейка.</param>
        /// <param name="name">Имя ячейки.</param>
        public static HSSFCell GetCellByName(HSSFWorkbook workbook, string name)
        {
            HSSFName hssfName = workbook.GetNameAt(workbook.GetNameIndex(name));
            var cellRef = new CellReference(hssfName.Reference);
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
        public static HSSFCell SetCellValue(HSSFSheet sheet, int rowIndex, int colIndex, object value)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            var cell = row.GetCell(colIndex) ?? row.CreateCell(colIndex);

            if (value is decimal || value is double || value is int || value is float)
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else
            {
                cell.SetCellValue(Convert.ToString(value));
            }

            return cell;
        }

        /// <summary>
        /// Получает ячейку по координатам.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
        public static HSSFCell GetCellByXy(HSSFSheet sheet, int rowIndex, int colIndex)
        {
            HSSFRow row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            return row.GetCell(colIndex) ?? row.CreateCell(colIndex);
        }

        /// <summary>
        /// Получает строковое значение ячейки.
        /// </summary>
        /// <param name="sheet">Лист на котором расположена ячейка.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="colIndex">Номер столбца.</param>
        public static string GetCellStringValue(HSSFSheet sheet, int rowIndex, int colIndex)
        {
            HSSFCell cell = GetCellByXy(sheet, rowIndex, colIndex);

            if (cell != null)
            {
                if (cell.CellType == 0)
                {
                    return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                }

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
            HSSFCell cell = GetCellByXy(sheet, rowIndex, colIndex);

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
        public static HSSFCell SetCellFormula(HSSFSheet sheet, int rowIndex, int colIndex, string formula)
        {
            var cell = GetCellByXy(sheet, rowIndex, colIndex);
            cell.SetCellFormula(formula);
            return cell;
        }

        /// <summary>
        /// Объеденяем ячейки
        /// </summary>
        /// <param name="sheet">Лист книги</param>
        /// <param name="rowFrom">Начальная строка</param>
        /// <param name="colFrom">Начальный столбец</param>
        /// <param name="rowTo">Конечноая строка</param>
        /// <param name="colTo">Конечный столбец</param>
        public static void SetMergedRegion(HSSFSheet sheet, int rowFrom, int colFrom, int rowTo, int colTo)
        {
            var region = new Region(rowFrom, colFrom, rowTo, colTo);
            sheet.AddMergedRegion(region);
        }

        /// <summary>
        /// Растягивает содерживое ячейки rowFrom,colFrom до ячейки rowFrom,colTo с автоподгоном высоты!
        /// </summary>
        /// <param name="workBook">Книга в которой находится ячейка.</param>
        /// <param name="sheet">Лист документа из книги</param>
        /// <param name="rowFrom">Номер строкаи</param>
        /// <param name="colFrom">Начальный столбец</param>
        /// <param name="colTo">Конечный столбец</param>
        public static void SetAlignCenterSelection(HSSFWorkbook workBook, HSSFSheet sheet, int rowFrom, int colFrom, int colTo)
        {
            var cell = GetCellByXy(sheet, rowFrom, colFrom);
            var cellStyle = workBook.CreateCellStyle();
            cellStyle.CloneStyleFrom(cell.CellStyle);
            cellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER_SELECTION;
            for (int j = colFrom; j <= colTo; j++)
            {
                GetCellByXy(sheet, rowFrom, j).CellStyle = cellStyle;
            }
        }

        /// <summary>
        /// Преобразования номера колонки в букву
        /// </summary>
        /// <param name="column"> индекс колонки</param>
        /// <returns> буква колонки</returns>
        public static string ConvertToLetter(int column)
        {
            if (column <= 25)
            {
                return ((char)('A' + column)).ToString(CultureInfo.InvariantCulture);
            }

            int a = column / 26;
            int b = column % 26;
            a--;
            return ConvertToLetter(a) + ((char)('A' + b)).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Рисование рамки
        /// </summary>
        /// <param name="workBook"> книга рабочая</param>
        /// <param name="sheet"> страница книги </param>
        /// <param name="firstRow"> первоя строка </param>
        /// <param name="fistCol"> первая колонка </param>
        /// <param name="lastRow"> последняя строка </param>
        /// <param name="lastCol"> последняя колонка </param>
        public static void SetBorderBoth(HSSFWorkbook workBook, HSSFSheet sheet, int firstRow, int fistCol, int lastRow, int lastCol)
        {
            for (var i = fistCol; i <= lastCol; i++)
            {
                var cell = GetCellByXy(sheet, firstRow, i);
                var cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;

                cell = GetCellByXy(sheet, lastRow, i);
                cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;
            }

            for (var i = firstRow; i <= lastRow; i++)
            {
                var cell = GetCellByXy(sheet, i, fistCol);
                var cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;

                cell = GetCellByXy(sheet, i, lastCol);
                cellStyle = workBook.CreateCellStyle();

                if (cell.CellStyle != null)
                {
                    cellStyle.CloneStyleFrom(cell.CellStyle);
                }

                cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;

                cell.CellStyle = cellStyle;
            }
        }
    }
}
