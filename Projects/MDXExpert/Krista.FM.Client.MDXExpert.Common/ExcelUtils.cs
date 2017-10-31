using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Drawing;
using System.IO;
using Krista.FM.Client.Common;
using System.Windows.Forms;
using OfficeHelper = Krista.FM.Common.OfficeHelpers.OfficeHelper;

namespace Krista.FM.Client.MDXExpert.Common
{
    public struct ExcelUtils
    {
        /// <summary>
        /// Создаем приложение Excel
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        static public Excel.Application StartExcel(bool visible)
        {
            try
            {
                Excel.Application result = new Excel.Application();
                result.Visible = visible;
                return result;
            }
            catch
            {
                throw new Exception("Не удалось создать приложение Microsoft Office Excel, возможно оно не установленно на компьютере");
            }
        }

        /// <summary>
        /// Если есть запущенный Excel, вернет его
        /// </summary>
        /// <returns></returns>
        static public Excel.Application GetActiveExcel()
        {
            try
            {
                //return (Excel.Application)OfficeHelper.GetOfficeObject("Excel.Application",
                //    true, false);
                return (Excel.Application)OfficeHelper.CreateExcelApplication();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Установка признака интерактивности Excel
        /// </summary>
        /// <param name="excelApl"></param>
        /// <param name="value"></param>
        static public void SetScreenUpdating(Excel.Application excelApl, bool value)
        {
            try
            {
                excelApl.Interactive = value;
                excelApl.ScreenUpdating = value;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Установить фокус запущенному приложению
        /// </summary>
        static public void SetExcelAplFocus()
        {
            SetExcelAplFocus(GetActiveExcel());
        }

        /// <summary>
        /// Установить фокус приложению
        /// </summary>
        static public void SetExcelAplFocus(Excel.Application excelApl)
        {
            if (excelApl != null)
            {
                excelApl.Visible = true;
            }
        }

        /// <summary>
        /// Получаем книгу для экспорта. Если путь к книге не указан, создадим новую.
        /// </summary>
        /// <param name="excelBook"></param>
        static public Excel.Workbook GetExcelBookForExport(string bookPath)
        {
            Excel.Application excel = null; ;

            Excel.Workbook excelBook = null;

            if (bookPath != string.Empty)
            {
                //Если Excel уже открыт, получим ссылку на него 
                //excel = (Excel.Application)OfficeHelper.GetOfficeObject("Excel.Application", true, false);
                excel = (Excel.Application)OfficeHelper.CreateExcelApplication();
                if (excel == null)
                    //Если нет, то создадим новый
                    excel = StartExcel(true);

                //посмотрим не открыта ли она уже в запущенном Excel
                excelBook = GetOpenedWorkbook(excel, bookPath);

                if (excelBook == null)
                    excelBook = OpenWorkbook(excel, bookPath, false);
            }
            else
            {
                excel = StartExcel(true);
                excelBook = excel.Workbooks.Add(Type.Missing);

                //Перед экспортом удалим все листы в книге, кроме одного, его оставим для заметок
                for (int i = excelBook.Sheets.Count; i > 1; i--)
                {
                    Excel.Worksheet tempSheet = (Excel.Worksheet)excelBook.Sheets[i];
                    tempSheet.Delete();

                    if (excelBook.Sheets.Count == 1)
                    {
                        tempSheet = (Excel.Worksheet)excelBook.Sheets[1];
                        tempSheet.Name = "Для заметок";
                    }
                }
            }

            excel.DisplayStatusBar = true;
            excel.StatusBar = " ";
            return excelBook;
        }

        /// <summary>
        /// Закрываем Excel
        /// </summary>
        /// <param name="excelApl"></param>
        static public void FinishExcel(Excel.Application excelApl)
        {
            if (excelApl != null)
            {
                excelApl.Quit();
                excelApl = null;
                GC.GetTotalMemory(true); // вызов сборщика мусора
            }
        }

        /// <summary>
        /// Получить уже открытую книгу в приложении
        /// </summary>
        /// <param name="excelApl"></param>
        /// <param name="bookPath"></param>
        /// <returns></returns>
        static public Excel.Workbook GetOpenedWorkbook(Excel.Application excelApl, string bookPath)
        {
            if ((excelApl != null) && (excelApl.Workbooks.Count != 0))
            {
                foreach (Excel.Workbook book in excelApl.Workbooks)
                {
                    if (book.FullName == bookPath)
                        return book;
                }
            }
            return null;
        }

        /// <summary>
        /// Открывает книгу по указанному пути... если такой книги нет, можно создать новую
        /// </summary>
        /// <param name="excelApl"></param>
        /// <param name="bookPath"></param>
        /// <param name="forceCreate"></param>
        /// <returns></returns>
        static public Excel.Workbook OpenWorkbook(Excel.Application excelApl, string bookPath, bool forceCreate)
        {
            Excel.Workbook result = null;
            if (excelApl != null)
            {
                FileInfo fileInfo = (bookPath == string.Empty) ? null : new FileInfo(bookPath);
                if ((fileInfo != null) && (fileInfo.Exists))
                {
                    try
                    {
                        result = excelApl.Workbooks.Open(
                                    bookPath, // FileName
                                    false, // UpdateLinks
                                    false, //  ReadOnly
                                    Type.Missing, // Format
                                    Type.Missing, // Password
                                    Type.Missing, // WriteResPassword
                                    true, // IgnoreReadOnlyRecommended
                                    Type.Missing, // Origin
                                    Type.Missing, // Delimiter
                                    true, // Editable
                                    Type.Missing, //  Notify
                                    Type.Missing, // Converter
                                    false, // AddToMru
                                    Type.Missing, // Local
                                    Type.Missing // CorruptLoad
                                );
                    }
                    catch
                    {
                        throw new Exception(String.Format("Невозможно открыть файл '{0}'. Возможно он поврежден или используется другим процессом.", bookPath));
                    }
                }
                else
                {
                    if (forceCreate)
                    {
                        result = excelApl.Workbooks.Add(Type.Missing);
                    }
                }
            }

            return result;
        }

        static public void WorkbookSaveAs(Excel.Workbook book, string bookFullName)
        {
            if (book != null)
            {
                book.SaveAs(
                    bookFullName,//FileName
                    Type.Missing,//FileFormat
                    Type.Missing,//Password
                    Type.Missing,//WriteResPassword
                    false,//ReadOnlyRecommend
                    Type.Missing,//CreateBackup
                    Excel.XlSaveAsAccessMode.xlShared,//AccessMode
                    Type.Missing,//ConflictResolution
                    Type.Missing,//AddtoMru
                    Type.Missing,//TextCodePage
                    Type.Missing,//TextVisualLayout
                    Type.Missing);//Local
            }
        }

        /// <summary>
        /// Безопасный способ получения диапазона, на который ссылается имя...
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public Excel.Range GetRefersToRange(Excel.Name name)
        {
            try
            {
                return name.RefersToRange;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Разместить текст в ячейке с указанными координатами
        /// </summary>
        static public Excel.Range MapExcelCell(Excel.Worksheet sheet, string text, Point cellLocation)
        {
            return MapExcelCell(sheet, text, cellLocation.X, cellLocation.Y, cellLocation.X, cellLocation.Y);
        }

        /// <summary>
        /// Разместить текст в ячейке с указанными координатами
        /// </summary>
        static public Excel.Range MapExcelCell(Excel.Worksheet sheet, string text, int startRow, int startColumn)
        {
            return MapExcelCell(sheet, text, startRow, startColumn, startRow, startColumn);
        }

        /// <summary>
        /// Разместить текст в ячейке с указанными координатами
        /// </summary>
        static public Excel.Range MapExcelCell(Excel.Worksheet sheet, string text, Point startLocation, Point endLocation)
        {
            return MapExcelCell(sheet, text, startLocation.X, startLocation.Y, endLocation.X, endLocation.Y);
        }

        /// <summary>
        /// Разместить текст в ячейке с указанными координатами
        /// </summary>
        static public Excel.Range MapExcelCell(Excel.Worksheet sheet, string text, int startRow, int startColumn, 
            int endRow, int endColumn)
        {
            Excel.Range sheetCell = GetExcelRange(sheet, startRow, startColumn, endRow, endColumn);

            if ((startRow != endRow) || (startColumn != endColumn))
            {
                sheetCell.Merge(false);
            }
            sheetCell.Value2 = text;

            return sheetCell;
        }


        /// <summary>
        /// Разместить текст в ячейке с указанными координатами
        /// </summary>
        static public Excel.Range MapExcelCellWithoutFormatting(Excel.Worksheet sheet, string text, Point startLocation, Point endLocation)
        {
            return MapExcelCellWithoutFormatting(sheet, text, startLocation.X, startLocation.Y, endLocation.X, endLocation.Y);
        }
        /// <summary>
        /// Разместить текст в ячейке с указанными координатами без автоформата экселя
        /// </summary>
        static public Excel.Range MapExcelCellWithoutFormatting(Excel.Worksheet sheet, string text, int startRow, int startColumn,
            int endRow, int endColumn)
        {
            Excel.Range sheetCell = GetExcelRange(sheet, startRow, startColumn, endRow, endColumn);

            if ((startRow != endRow) || (startColumn != endColumn))
            {
                sheetCell.Merge(false);
            }
            sheetCell.NumberFormat = "@";
            sheetCell.Value2 = text; 

            return sheetCell;
        }


        /// <summary>
        /// Получить экселевский диапазон
        /// </summary>
        static public Excel.Range GetExcelRange(Excel.Worksheet sheet, Point startLocation, Point endLocation)
        {
            return GetExcelRange(sheet, startLocation.X, startLocation.Y, endLocation.X, endLocation.Y);
        }

        /// <summary>
        /// Получить экселевский диапазон
        /// </summary>
        static public Excel.Range GetExcelRange(Excel.Worksheet sheet, Point location)
        {
            return GetExcelRange(sheet, location.X, location.Y);
        }

        /// <summary>
        /// Получить экселевский диапазон
        /// </summary>
        static public Excel.Range GetExcelRange(Excel.Worksheet sheet, int row, int column)
        {
            return GetExcelRange(sheet, row, column, row, column);
        }

        /// <summary>
        /// Получить экселевский диапазон
        /// </summary>
        static public Excel.Range GetExcelRange(Excel.Range range1, Excel.Range range2)
        {
            if ((range1 == null) && (range2 == null))
                return null;

            if (range1 == null)
                return range2;
            else
                if (range2 == null)
                    return range1;

            return range1.get_Range(range1, range2);
        }

        /// <summary>
        /// Получить экселевский диапазон
        /// </summary>
        static public Excel.Range GetExcelRange(Excel.Worksheet sheet, int startRow, int startColumn, 
            int endRow, int endColumn)
        {
            if ((startRow < 1) || (startRow > 65536) || (endRow < 1) || (endRow > 65536) ||
                (startColumn < 1) || (startColumn > 256) || (endColumn < 1) || (endColumn > 256))
                return null;

            Excel.Range result = (Excel.Range)sheet.Cells[startRow, startColumn];
            if ((startRow != endRow) || (startColumn != endColumn))
            {
                Excel.Range endCell = (Excel.Range)sheet.Cells[endRow, endColumn];
                result = sheet.get_Range(result, endCell);
            }
            return result;
        }

        /// <summary>
        /// Пометить экселевский диапазон именем, так же указывается видимость нового имени
        /// </summary>
        /// <param name="range"></param>
        /// <param name="name"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        static public Excel.Name MarkExcelName(Excel.Range range, string name, bool isVisible)
        {
            if (range != null)
            {
                string address = GetAddressR1C1(range);
                try
                {
                    return range.Worksheet.Names.Add(name, Type.Missing, isVisible, Type.Missing, Type.Missing,
                                                     Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                     "=" + address, Type.Missing);
                }
                catch(Exception exc)
                {
                    MessageBox.Show(exc.Message);
                    return null;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Получить экселевское имя
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static public Excel.Name GetExcelName(Excel.Worksheet sheet, string name)
        {
            Excel.Name result = null;
            if ((sheet != null) && (name != string.Empty))
            {
                try
                {
                    result = sheet.Names.Item(Type.Missing, name, Type.Missing);
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// Получить адрес диапазона
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        static public string GetAddressR1C1(Excel.Range range)
        {
            if (range != null)
            {
                return range.get_Address(Type.Missing, Type.Missing, Excel.XlReferenceStyle.xlR1C1,
                    Type.Missing, Type.Missing);
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Возваращает объединенный диапазон
        /// </summary>
        /// <param name="range1"></param>
        /// <param name="range2"></param>
        /// <returns></returns>
        static public Excel.Range UnionRange(Excel.Range range1, Excel.Range range2)
        {
            if ((range1 == null) && (range2 == null))
                return null;

            if (range1 == null)
                return range2;
            else
                if (range2 == null)
                    return range1;

            Excel.Application excelAppl = (range1 != null)? range1.Application : range2.Application;
            return excelAppl.Union(range1, range2, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                Type.Missing, Type.Missing, Type.Missing);
        }

        /// <summary>
        /// Если объект является листом книги, вернет ссылку на него
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        static public Excel.Worksheet GetWorksheet(object sheet)
        {
            Excel.Worksheet result = null;
            if (sheet != null)
            {
                if (sheet is Excel.Worksheet)
                    result = (Excel.Worksheet)sheet;
            }
            return result;
        }

        /// <summary>
        /// Вернет имя листа не конфликтующее с другими именами в книге (если уже есть, прибавляет к 
        /// имени число)
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        static public string GetSheetName(Excel.Workbook book, string sheetName)
        {
            if (String.IsNullOrEmpty(sheetName))
            {
                sheetName = "Лист";
            }

            //ограничение Excel
            if (sheetName.Length > 31)
            {
                sheetName = sheetName.Remove(27) + "...";
            }

            string result = sheetName;
            if (book == null)
                return result;

            bool isNameExist = false;
            int i = 1;
            do
            {
                isNameExist = (FindWorksheet(book, result) != null);
                if (isNameExist)
                {
                    result = sheetName + i.ToString();
                    i++;
                }
            }
            while (isNameExist);
            return result;
        }

        /// <summary>
        /// По указаному имени, ищет в книге лист
        /// </summary>
        /// <param name="book"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        static public Excel.Worksheet FindWorksheet(Excel.Workbook book, string sheetName)
        {
            if (book != null)
            {
                for (int i = 1; i <= book.Sheets.Count; i++)
                {
                    Excel.Worksheet sheet = GetWorksheet(book.Sheets[i]);
                    if ((sheet != null) && (sheet.Name == sheetName))
                        return sheet;
                }
            }
            return null;
        }

        /// <summary>
        /// Как известно, метод AutoFit для подбора высоты объединенных ячеек не срабатывает. 
        /// Высоту у таких ячеек мы будем подбирать самостоятельно
        /// </summary>
        /// <param name="mergeCell"></param>
        static public void AutoFitMergeCell(Excel.Range mergeCell)
        {
            if ((mergeCell == null) || !(bool)mergeCell.MergeCells)
                return;

            mergeCell = ((Excel.Range)mergeCell.Cells[1, 1]).MergeArea;

            if (mergeCell.Columns.Count < 2)
                return;

            Excel.Range startCell = (Excel.Range)mergeCell.Cells[1, 1];

            double oldStartCellHeight = 0;
            double.TryParse(startCell.RowHeight.ToString(), out oldStartCellHeight);

            double startCellWidth = (double)startCell.ColumnWidth;

            //Ширина объединенной области
            double mergeCellWidht = 0;
            for (int i = 1; i <= mergeCell.Columns.Count; i++)
                mergeCellWidht += (double)(mergeCell.Cells[1, i] as Excel.Range).ColumnWidth;

            //Высота объединенной области
            double oldMergeCellHeight = (double)mergeCell.Height;
            //for (int i = 1; i <= mergeCell.Rows.Count; i++)
            //    oldMergeCellHeight += (double)(mergeCell.Cells[i, 1] as Excel.Range).RowHeight;

            mergeCell.MergeCells = false;

            startCell.ColumnWidth = mergeCellWidht;
            if (!(bool)startCell.WrapText)
                startCell.WrapText = true;

            startCell.EntireRow.AutoFit();
            double newMergeCellHeight = (double)startCell.RowHeight;
            startCell.ColumnWidth = startCellWidth;

            mergeCell.MergeCells = true;

            bool multipleRow = mergeCell.Rows.Count > 1;

            startCell.RowHeight = multipleRow ? oldStartCellHeight : newMergeCellHeight;

            //Если высота родителя больше высоты дочерних ячеек, значит добавим к последнему 
            //ребенку недостоющие пикселы
            if (multipleRow && (newMergeCellHeight > oldMergeCellHeight))
            {
                Excel.Range endCell = (Excel.Range)mergeCell.Cells[mergeCell.Rows.Count, 1];
                endCell.RowHeight = (double)endCell.RowHeight + newMergeCellHeight - oldMergeCellHeight;
            }
        }

        /// <summary>
        /// Получить инекс строки которой принадлежит данная координата
        /// </summary>
        /// <returns></returns>
        static public int GetRowIndex(Excel.Worksheet sheet, double coordinate)
        {
            int result = 1;
            if (sheet == null)
                return result;

            Excel.Range currentRow = (Excel.Range)sheet.Cells[result, 1];
            double commonRowHeight = (double)currentRow.RowHeight;
            while (commonRowHeight < coordinate)
            {
                result++;
                currentRow = (Excel.Range)sheet.Cells[result, 1];
                commonRowHeight += (double)currentRow.RowHeight;
            }

            return result;
        }

        /// <summary>
        /// Получить инекс колонки которой принадлежит данная координата
        /// </summary>
        /// <param name="sheet">листа на котором ищем колонку</param>
        /// <param name="coordinate">в пикселях</param>
        /// <returns></returns>
        static public int GetColumnIndex(Excel.Worksheet sheet, double coordinate)
        {
            int result = 1;
            if (sheet == null)
                return result;

            Excel.Range currentColumn = (Excel.Range)sheet.Cells[1, result];
            while ((double)currentColumn.Left < coordinate)
            {
                result++;
                currentColumn = (Excel.Range)sheet.Cells[1, result];
            }

            return result - 1;
        }

        /// <summary>
        /// Вернет нижнию границу строки, которой принаделжит указанная координата
        /// </summary>
        /// <returns></returns>
        static public double GetRowBottom(Excel.Worksheet sheet, float coordinate)
        {
            double result = 0;
            if (sheet == null)
                return result;

            int rowIndex = 1;
            Excel.Range currentRow = (Excel.Range)sheet.Cells[rowIndex, 1];
            result = (double)currentRow.RowHeight;
            while (result < coordinate)
            {
                rowIndex++;
                currentRow = (Excel.Range)sheet.Cells[rowIndex, 1];
                result += (double)currentRow.RowHeight;
            }

            return result;
        }


        /// <summary>
        /// Вернет верхнюю границу строки, которой принаделжит указанная координата
        /// </summary>
        /// <returns></returns>
        static public double GetRowTop(Excel.Worksheet sheet, int rowNumber)
        {
            if (rowNumber == 1)
            {
                return 0;
            }

            return (double)GetExcelRange(sheet, 1, 1, rowNumber - 1, 1).Height;
        }

        /// <summary>
        /// Вернет правую границу колнки, которой принаделжит указанная координата
        /// </summary>
        /// <returns></returns>
        static public double GetColumnRight(Excel.Worksheet sheet, float coordinate)
        {
            if (sheet == null)
                return 0;

            int columnIndex = 1;
            Excel.Range currentColumn = (Excel.Range)sheet.Cells[1, columnIndex];

            while ((double)currentColumn.Left < coordinate)
            {
                columnIndex++;
                currentColumn = (Excel.Range)sheet.Cells[1, columnIndex];
            }

            return (double)currentColumn.Left - 1;
        }

        /// <summary>
        /// Вставка текста в Excel.TextBox, происходит довольно странно, максимальная длинна 
        /// вставляемого текста (за один раз) не должна превышать 200 символов, поэтому приходиться
        /// вставлять его по кускам
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="text"></param>
        static public void SetTextToTextBox(Excel.TextBox textBox, string text)
        {
            if ((textBox == null) || (text == string.Empty))
                return;

            text = text.Replace("\r\n", "\n");
            text = text.Replace("\t", "    ");

            if (textBox.Application.Version.Contains("12."))
            {
                //в 2007 офисе видать починили, здесь просто присваиваем текст и сматываемся...
                textBox.Text = text;
            }
            else
            {
                int currentCharacter = 0;
                int insertLenght = Math.Min(200, text.Length);
                textBox.Text = text.Substring(currentCharacter, insertLenght);

                if (text.Length > 200)
                {
                    Excel.Characters characters;

                    int i = text.Length / 200;
                    for (int j = 1; j <= i; j++)
                    {
                        characters = textBox.get_Characters(j * 200 + 1, Type.Missing);
                        characters.Insert(text.Substring(j * 200, Math.Min(200, text.Length - (j * 200))));
                    }
                }
            }
        }

        /// <summary>
        /// Cинхронизирует значения шрифтоф
        /// </summary>
        /// <param name="clockedFont">синхронизируемый шрифт</param>
        /// <param name="templateFont">шаблон шрифта, по которому идет синхронизация</param>
        static public void SynchronizeFont(Excel.Font clockedFont, Font templateFont)
        {
            if ((clockedFont != null) && (templateFont != null))
            {
                clockedFont.Name = templateFont.Name;
                clockedFont.Bold = templateFont.Bold;
                clockedFont.Italic = templateFont.Italic;
                clockedFont.Strikethrough = templateFont.Strikeout;
                clockedFont.Underline = templateFont.Underline;
                clockedFont.Size = templateFont.SizeInPoints;
            }
        }

        /// <summary>
        /// Стандартная высота ячейки, от версии к версии Excel - разная
        /// </summary>
        /// <param name="excelApl"></param>
        /// <returns></returns>
        static public float DefaultRowHeight(Excel.Worksheet sheet)
        {
            float rowHeight = 12.75f;

            if (sheet != null)
            {
                Excel.Range range = GetExcelRange(sheet, 65536, 1);
                if (range != null)
                    rowHeight = (float)(double)range.RowHeight;
            }

            return rowHeight;
        }


        static public void ExportDataTable(DataTable table, string fileName)
        {
            Excel.Workbook excelBook = GetExcelBookForExport(fileName);
            if (excelBook == null)
                return;

            SetExcelAplFocus(excelBook.Application);

            excelBook.Application.Interactive = false;
            excelBook.Application.ScreenUpdating = false;

            Excel.Worksheet commonSheet = (Excel.Worksheet)excelBook.Sheets.Add(excelBook.Sheets[1],
                        Type.Missing, Type.Missing, Type.Missing);

            commonSheet.Name = ExcelUtils.GetSheetName(excelBook, table.TableName);

            int i = 0;
            int j = 0;
            for (j = 0; j <= table.Columns.Count - 1; j++)
            {
                commonSheet.Cells[i + 1, j + 1] = !String.IsNullOrEmpty(table.Columns[j].Caption) ? table.Columns[j].Caption : table.Columns[j].ColumnName;
            }
            
            for (i = 0; i <= table.Rows.Count - 1; i++)
            {
                excelBook.Application.StatusBar = String.Format("Идет экспорт {0} из {1} строк.", i + 1, table.Rows.Count);
                for (j = 0; j <= table.Columns.Count - 1; j++)
                {
                    commonSheet.Cells[i + 2, j + 1] = table.Rows[i][j];
                }
            }

            excelBook.Application.Interactive = true;
            excelBook.Application.ScreenUpdating = true;
            excelBook.Application.StatusBar = false;

        }
    }
}
