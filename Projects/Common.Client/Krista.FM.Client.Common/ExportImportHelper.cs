using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;

using Krista.FM.Client.Components;
using Krista.FM.Common;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common

{
    /// <summary>
    /// структура для импорта экспорта из екселя
    /// </summary>
    public struct ExcelExportImportHelper
    {
        private const string tmpSheetName = "tmp";

        /// <summary>
        /// экспорт одной таблицы в ексель
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cs"></param>
        /// <param name="isHierarchy"></param>
        /// <param name="sheets"></param>
        /// <param name="excel"></param>
        private static void ExportDataToExcel(DataTable table, GridColumnsStates cs, bool isHierarchy, bool exportAllColumns, object sheets, object excel)
        {
            object sheet = AddSheet(sheets, table.TableName);
            //список колонок, данные по которым войдут в итоговый документ
            List<string> columnsNames = new List<string>();
            List<string> columnsCaptions = new List<string>();
            // Для иерархических классификаторов сохраняем ID
            if (table.Columns.Contains("ParentID"))
            {
                columnsNames.Add("ID");
                columnsCaptions.Add("ID");
            }
            
            foreach (DataColumn column in table.Columns)
            {
                if (!cs.ContainsKey(column.ColumnName))
                {
                    columnsNames.Add(column.ColumnName);
                    columnsCaptions.Add(column.ColumnName);
                }
                else
                {
                    // все системные невидимые колонки не будут экспортироваться
                    if (!cs[column.ColumnName].IsSystem)
                    {
                        if (column.ColumnName == "ID")
                            continue;
                        columnsNames.Add(column.ColumnName);
                        columnsCaptions.Add(cs[column.ColumnName].ColumnCaption);
                    }
                    else if (exportAllColumns)
                    {
                        columnsNames.Add(column.ColumnName);
                        columnsCaptions.Add(cs[column.ColumnName].ColumnCaption);
                    }
                }
            }
            // получаем индекс колонки, дальше которого записывать ничего не будем
            string excelLastColumns = GetColumnNameFromNumber(columnsNames.Count - 1);

            // получаем коллекцию колонок в екселе
            object columns = ReflectionHelper.GetProperty(sheet, "Columns");
            // устанавливаем границы для колонок 
            SetBorders(excel, columns, string.Format("A:{0}", excelLastColumns));
            // получаем строковый диапазон, начиная с которого будем записывать данные
            string rangeDiapazon = string.Format("A1:{0}{1}", excelLastColumns, 1);
            object range = GetRange(sheet, rangeDiapazon);
            // устанавливаем параметры для колонок в листе согласно заголовкам
            for (int i = 1; i <= columnsNames.Count; i++)
            {
                string columnName = columnsNames[i - 1];
                object column = ReflectionHelper.GetProperty(columns, "Item", i);
                SetColumnType(excel, column, table.Columns[columnName]);
                // выставляем ширину колонки
                int clmnWidth = 0;
                if (cs.ContainsKey(columnName))
                {
                    clmnWidth = cs[columnName].ColumnWidth;
                    if (clmnWidth > 250) clmnWidth = 50;
                    if (clmnWidth < 250) clmnWidth = 25;
                    if (clmnWidth < 10) clmnWidth = 10;
                }
                else
                    clmnWidth = 25;
                ReflectionHelper.SetProperty(column, "ColumnWidth", clmnWidth);
                // подсвечиваем колонки в разные цвета
                if (cs.ContainsKey(columnName))
                {
                    if (cs[columnName].IsSystem)
                        SetColumnColor(excel, column, 15);
                    else if (cs[columnName].IsNullable)
                        SetColumnColor(excel, column, 19);
                    else
                        SetColumnColor(excel, column, 0);
                }
                Marshal.ReleaseComObject(column);
            }
            Marshal.ReleaseComObject(columns);
            // Устанавливаем заголовки колонок в екселе
            ReflectionHelper.SetProperty(range, "Value", GetArrayFromList(columnsCaptions));

            SetCommentsForRange(sheet, columnsNames);

            columnsCaptions.Clear();
            ReflectionHelper.SetProperty(range, "WrapText", true);
            Marshal.ReleaseComObject(range);

            // делаем первую строчку нескроллируемой
            range = GetRange(sheet, "A2");
            ReflectionHelper.CallMethod(range, "Select");
            Marshal.ReleaseComObject(range);

            object ActiveWindow = ReflectionHelper.GetProperty(excel, "ActiveWindow");
            ReflectionHelper.SetProperty(ActiveWindow, "FreezePanes", true);
            Marshal.ReleaseComObject(ActiveWindow);
            // само сохранение данных в лист екселя
            // идем по записям и херачим их в лист
            int rowCounter = 2;
            object[] values = new object[columnsNames.Count];
            foreach (DataRow row in table.Rows)
            {
                int i = 0;
                foreach (string columnName in columnsNames)
                {
                     values[i] = row[columnName];
                     i++;
                }
                rangeDiapazon = string.Format("A{0}:{1}{0}", rowCounter, excelLastColumns);
                range = GetRange(sheet, rangeDiapazon);
                //ReflectionHelper.SetProperty(range, "NumberFormat", "General");
                ReflectionHelper.SetProperty(range, "Value", values);
                Marshal.ReleaseComObject(range);
                rowCounter++;
            }

            ProtectSheet(excel, sheet, excelLastColumns);

            Marshal.ReleaseComObject(sheet);
        }


        /// <summary>
        /// сохранение данных датасета в документ екселя
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        /// <param name="fileName"></param>
        /// <param name="workplace"></param>
        public static void ExportDataToExcel(DataSet ds, GridColumnsStates cs, string fileName,
            IWorkplace workplace, bool isHierarchy, bool exportAllColumns)
        {
            string exportFileName = string.Empty;
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref exportFileName))
            {
                // если файл существует, то удалим его перед экспортом
                if (File.Exists(exportFileName))
                    File.Delete(exportFileName);

                workplace.OperationObj.Text = "Сохранение данных в файл";
                workplace.OperationObj.StartOperation();

                // получаем экземпляр экселя
                using (ExcelApplication excel = OfficeHelper.CreateExcelApplication())
                {
                    object wb = null;
                    try
                    {
                        excel.ScreenUpdating = false;
                        // создаем новый документ
                        object wbs = excel.GetWorkbooks();
                        wb = ReflectionHelper.CallMethod(wbs, "Add");
                        object sheets = ReflectionHelper.GetProperty(wb, "Sheets");
                        // удаляем лишние листы их книги
                        DeleteSheetsFromExcelBook(wb);
                        // по количеству сохраняемых таблиц создаем необходимое количество листов
                        foreach (DataTable dt in ds.Tables)
                        {
                            ExportDataToExcel(dt, cs, isHierarchy, exportAllColumns, sheets, excel.OfficeApp);
                        }
                        // удаляем временный лист
                        DeleteSheet(sheets, tmpSheetName);
                        Marshal.ReleaseComObject(sheets);
                        Marshal.ReleaseComObject(wbs);
                    }
                    catch
                    {
                        workplace.OperationObj.StopOperation();
                        MessageBox.Show("Экспорт закончился неудачно", "Ошибка при экспорте");
                    }
                    finally
                    {
                        workplace.OperationObj.StopOperation();
                        excel.ScreenUpdating = true;
                        excel.DisplayAlerts = false;

                        // если все хорошо, сохраняем книгу екселя по указанному названию
                        ReflectionHelper.CallMethod(wb, "SaveAs", exportFileName);
                        Marshal.ReleaseComObject(wb);

                        excel.CloseWorkBooks();
                        try
                        {
                            excel.Quit();
                        }
                        catch
                        {
                            ;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// делаем защищенными ячейки с названиями колонок
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="sheet"></param>
        /// <param name="lastColumnLetter"></param>
        private static void ProtectSheet(object excel, object sheet, string lastColumnLetter)
        {
            object cells = ReflectionHelper.GetProperty(sheet, "Cells");
            ReflectionHelper.CallMethod(cells, "Select");
            Marshal.ReleaseComObject(cells);

            object selection = GetSelection(excel);
            ReflectionHelper.SetProperty(selection, "Locked", false);
            ReflectionHelper.SetProperty(selection, "FormulaHidden", false);
            Marshal.ReleaseComObject(selection);

            string rangeDiapazon = string.Format("A1:{0}1", lastColumnLetter);
            object range = GetRange(sheet, rangeDiapazon);
            ReflectionHelper.CallMethod(range, "Select");
            selection = GetSelection(excel);
            ReflectionHelper.SetProperty(selection, "Locked", true);

            ReflectionHelper.CallMethod(sheet, "Protect", "password", true, true, true, false,
                true, true, true, false, false, false, false, false, true, true, false);

            Marshal.ReleaseComObject(selection);
            Marshal.ReleaseComObject(range);
        }


        private static string GetColumnNameFromNumber(int number)
        {
            string excelLastColumns = string.Empty;

            int firstcolumn = number / 26;
            int secondColumn = number % 26;

            if (firstcolumn > 0)
            {
                excelLastColumns = Convert.ToChar('A' + firstcolumn - 1).ToString();
                //secondColumn++;
            }
            return excelLastColumns + Convert.ToChar('A' + secondColumn);
        }


        /// <summary>
        /// устанавливаем комментарии в ячейках с названиями колонок
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnsNames"></param>
        private static void SetCommentsForRange(object sheet, List<string> columnsNames)
        {
            for (int i = 0; i <= columnsNames.Count - 1; i++)
            {
                object range = GetRange(sheet, GetColumnNameFromNumber(i) + "1");
                object comment = ReflectionHelper.CallMethod(range, "AddComment");
                ReflectionHelper.CallMethod(comment, "Text", columnsNames[i]);
                ReflectionHelper.SetProperty(comment, "Visible", false);
                Marshal.ReleaseComObject(range);
                Marshal.ReleaseComObject(comment);
            }
        }

        /// <summary>
        /// удаляет все листы из документа екселя
        /// </summary>
        /// <param name="workBook"></param>
        private static void DeleteSheetsFromExcelBook(object workBook)
        {
            object sheets = ReflectionHelper.GetProperty(workBook, "Sheets");
            List<string> sheetsNames = new List<string>();
            int sheetsCount = (int)ReflectionHelper.GetProperty(sheets, "Count");
            object deleteSheet = null;
            for (int i = 1; i <= sheetsCount; i++)
            {
                deleteSheet = GetSheet(sheets, i);
                string sheetName = Convert.ToString(ReflectionHelper.GetProperty(deleteSheet, "Name"));
                sheetsNames.Add(sheetName);
            }
            object tmpSheet = AddSheet(sheets, tmpSheetName);
            foreach (string name in sheetsNames)
            {
                DeleteSheet(sheets, name);
            }
            Marshal.ReleaseComObject(deleteSheet);
            Marshal.ReleaseComObject(sheets);
            Marshal.ReleaseComObject(tmpSheet);
            sheetsNames.Clear();
        }

        /// <summary>
        /// переводит список в двухмерный массив
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static object[,] GetArrayFromList(List<string> list)
        {
            object[,] array = new object[1, list.Count];
            int counter = 0;
            foreach (object val in list)
            {
                array[0, counter] = val;
                counter++;
            }
            return array;
        }

        #region функции работы с Excel, конечно их лучше вынести куда нибудь в более общее место

        /// <summary>
        /// раскрашивает колонку в определенный цвет
        /// </summary>
        /// <param name="excelApplication"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        private static void SetColumnColor(object excelApplication, object column, object color)
        {
            ReflectionHelper.CallMethod(column, "Select");
            object selection = GetSelection(excelApplication);
            if (selection != null)
            {
                object interior = ReflectionHelper.GetProperty(selection, "Interior");
                ReflectionHelper.SetProperty(interior, "ColorIndex", color);
                Marshal.ReleaseComObject(interior);
            }
            Marshal.ReleaseComObject(selection);
        }

        private static object GetColumnsSelection(object excelApp, object columns, string selectionDiapazon)
        {
            object selectionColumns = ReflectionHelper.GetProperty(columns, "Item", selectionDiapazon);
            ReflectionHelper.CallMethod(selectionColumns, "Select");
            Marshal.ReleaseComObject(selectionColumns);
            return GetSelection(excelApp);
        }

        /// <summary>
        /// устанавливает параметры границ колонок, ячейки которые могут содержать данные 
        /// </summary>
        /// <param name="excelApplication"></param>
        /// <param name="columns"></param>
        /// <param name="selectionDiapazon"></param>
        private static void SetBorders(object excelApplication, object columns, string selectionDiapazon)
        {
            // получение диапазона колонок
            object selection = GetColumnsSelection(excelApplication, columns, selectionDiapazon);
            // установка строкового формата
            //ReflectionHelper.SetProperty(selection, "NumberFormat", "@");
            // установка переноса на следующую строку
            ReflectionHelper.SetProperty(selection, "WrapText", true);
            // установка границ диапазона в виде расчерченых границ и самих ячеек
            object border = GetBorder(selection, Borders.xlEdgeBottom);
            ReflectionHelper.SetProperty(border, "Weight", 1);
            border = GetBorder(selection, Borders.xlEdgeLeft);
            ReflectionHelper.SetProperty(border, "Weight", 1);
            border = GetBorder(selection, Borders.xlEdgeRight);
            ReflectionHelper.SetProperty(border, "Weight", 1);
            border = GetBorder(selection, Borders.xlEdgeTop);
            ReflectionHelper.SetProperty(border, "Weight", 1);
            border = GetBorder(selection, Borders.xlInsideHorizontal);
            ReflectionHelper.SetProperty(border, "Weight", 1);
            border = GetBorder(selection, Borders.xlInsideVertical);
            ReflectionHelper.SetProperty(border, "Weight", 1);

            Marshal.ReleaseComObject(border);
            Marshal.ReleaseComObject(selection);
        }

        private static void SetColumnType(object excelApplication, object excelColumn, DataColumn dataColumn)
        {
            ReflectionHelper.CallMethod(excelColumn, "Select");
            object selection = GetSelection(excelApplication);
            if (dataColumn.DataType == typeof(String))
            {
                ReflectionHelper.SetProperty(selection, "NumberFormat", "@");
            }
            else if (dataColumn.DataType == typeof(DateTime))
            {
                ReflectionHelper.SetProperty(selection, "NumberFormat", "@");
            }
            else
            {
                ReflectionHelper.SetProperty(selection, "NumberFormat", "0");
            }
        }

        private static object GetBorder(object selection, Borders borderKind)
        {
            object borders = ReflectionHelper.GetProperty(selection, "Borders");
            object border = ReflectionHelper.GetProperty(borders, "Item", borderKind);
            Marshal.ReleaseComObject(borders);
            return border;
        }

        private enum Borders
        {
            xlEdgeBottom = 9,
            xlEdgeLeft = 7,
            xlEdgeRight = 10,
            xlEdgeTop = 8,
            xlInsideHorizontal = 12,
            xlInsideVertical = 11
        }

        /// <summary>
        /// возвращает объект Range по указанным координатам на сетке
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rangeDiapazon"></param>
        /// <returns></returns>
        private static object GetRange(object sheet, string rangeDiapazon)
        {
            return ReflectionHelper.GetProperty(sheet, "Range", rangeDiapazon);
        }

        /// <summary>
        /// возвращает объект Selection 
        /// </summary>
        /// <param name="excelApplication"></param>
        /// <returns></returns>
        private static object GetSelection(object excelApplication)
        {
            return ReflectionHelper.GetProperty(excelApplication, "Selection");
        }

        /// <summary>
        /// возвращает лист с указанным именем или индексом
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="sheetName_Index"></param>
        /// <returns></returns>
        private static object GetSheet(object sheets, object sheetName_Index)
        {
            return ReflectionHelper.GetProperty(sheets, "Item", sheetName_Index);
        }

        /// <summary>
        /// добавляет лист с указанным именем
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        private static object AddSheet(object sheets, string sheetName)
        {
            object sheet = ReflectionHelper.CallMethod(sheets, "Add", Missing.Value, Missing.Value, 1, Missing.Value);
            ReflectionHelper.SetProperty(sheet, "Name", sheetName);
            return sheet;
        }

        /// <summary>
        /// удаляет лист с указанным именем или индексом
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="sheetName_Index"></param>
        private static void DeleteSheet(object sheets, object sheetName_Index)
        {
            object deleteSheet = GetSheet(sheets, sheetName_Index);
            if (deleteSheet != null)
                ReflectionHelper.CallMethod(deleteSheet, "Delete");
            Marshal.ReleaseComObject(deleteSheet);
        }


        private static List<string> GetColumnsNamesFromSheet(object sheet)
        {
            List<string> columns = new List<string>();
            List<string> excelColumnsNames = new List<string>();
            string pref = string.Empty;
            for (int i = 0; i <= 2; i++)
            {
                for (char ch = 'A'; ch <= 'Z'; ch++)
                {
                    excelColumnsNames.Add(pref + ch);
                }
                pref = Convert.ToChar('A' + i).ToString();
            }

            foreach (string excelColumn in excelColumnsNames)
            {
                object range = GetRange(sheet, string.Format("{0}1", excelColumn));
                object comment = ReflectionHelper.GetProperty(range, "Comment");
                if (comment == null)
                {
                    Marshal.ReleaseComObject(range);
                    break;
                }
                object text = ReflectionHelper.CallMethod(comment, "Text");

                string columnName = Convert.ToString(text);
                columns.Add(columnName);
                Marshal.ReleaseComObject(range);
                Marshal.ReleaseComObject(comment);
            }
            return columns;
        }


        #endregion


        private static void ImportFromExcel(DataTable dt, GridColumnsStates cs, object sheets)
        {
            object sheet = null;
            try
            {
                try
                {
                    // пытаемся получить лист екселя с названием таблицы
                    sheet = GetSheet(sheets, dt.TableName);
                }
                catch
                {
                    return;
                }
                ReflectionHelper.CallMethod(sheet, "Activate");
                List<string> columns = GetColumnsNamesFromSheet(sheet);
                // последняя колонка в выбираемом диапазоне 
                string excelLastColumn = GetColumnNameFromNumber(columns.Count - 1);//Convert.ToChar('A' + (columns.Count % 26) - 1);
                // строка, с которой начинаются данные
                int excelRows = 2;
                // списки для получения данных из екселя
                object[] values = new object[columns.Count];

                // получаем индексы колонок, которые not null 
                // и если в одной из таких колонок встретили пустое значение
                // то конец данным
                List<int> notNullColumnIndexes = new List<int>();
                int columnIndex = 0;
                foreach (string columnName in columns)
                {
                    if (!cs.ContainsKey(columnName.ToUpper()))
                        continue;
                    if (!cs[columnName].IsNullable && !cs[columnName].IsSystem)
                    {
                        notNullColumnIndexes.Add(columnIndex);
                    }
                    columnIndex++;
                }
                // читаем данные из экселевского листа
                while (true)
                {
                    // получаем диапазон для одной строки
                    string rangeDiapazon = string.Format("A{0}:{1}{0}", excelRows, excelLastColumn);
                    object range = null;
                    try
                    {
                        range = GetRange(sheet, rangeDiapazon);
                        // получаем значения из этого диапазона
                        object val = ReflectionHelper.GetProperty(range, "Value");

                        // получаем массив из этих значений
                        object[,] rowValues = (object[,])val;
                        int counter = 0;
                        foreach (object obj in rowValues)
                        {
                            values[counter] = obj;
                            counter++;
                        }
                        // если полученное значение равно null, значит данные закончились, выходим
                        // проверяем только поля, которые обязательны для редактирования...
                        // остальные даже если что и записано, то не важно
                        bool endOfData = false;
                        if (notNullColumnIndexes.Count != 0)
                        {
                            foreach (int index in notNullColumnIndexes)
                            {
                                if (values[index] == null)
                                    endOfData = true;
                                else
                                {
                                    endOfData = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (object value in values)
                            {
                                if (value == null)
                                    endOfData = true;
                                else
                                {
                                    endOfData = false;
                                    break;
                                }
                            }
                        }
                        if (endOfData)
                            break;
                        // добавляем новую строку в таблицу
                        // будем менять на добавление сперва записи, а потом уже записывать туда значения
                        DataRow row = dt.NewRow();
                        for (int i = 0; i <= columns.Count - 1; i++)
                        {
                            if (!dt.Columns.Contains(columns[i]))
                                continue;
                            if (values[i] != null)
                                row[columns[i]] = values[i];
                            else
                                row[columns[i]] = DBNull.Value;
                        }
                        dt.Rows.Add(row);
                    }
                    finally
                    {
                        if (range != null)
                            Marshal.ReleaseComObject(range);
                    }
                    excelRows++;
                }
            }
            finally
            {
                if (sheet != null)
                    Marshal.ReleaseComObject(sheet);
            }
        }


        /// <summary>
        /// экспорт данных из екселя
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentClmnName"></param>
        /// <param name="refClmnName"></param>
        /// <param name="cs"></param>
        /// <param name="isHierarchy"></param>
        /// <param name="restoreIDs"></param>
        /// <param name="fileName"></param>
        /// <param name="workplace"></param>
        /// <param name="tableName"></param>
        public static bool ImportFromExcel(DataSet ds, string parentClmnName, string refClmnName,
            GridColumnsStates cs, bool isHierarchy, bool restoreIDs, string fileName, IWorkplace workplace, string tableName)
        {
            // считаем, что ничего плохого не произошло
        bool returnValue = true;
        string exportFileName = fileName;

            workplace.OperationObj.Text = "Загрузка данных из файла";
            workplace.OperationObj.StartOperation();
            try
            {
                foreach (DataTable dt in ds.Tables)
                    dt.BeginLoadData();

                // получаем экземпляр экселя, загружаем в него файлик выбранный
                using (ExcelApplication excel = OfficeHelper.CreateExcelApplication())
                {
                    object wb = null;
                    object sheets = null;
                    object wbs = null;
                    try
                    {
                        excel.OpenFile(exportFileName, true, false);
                        wbs = excel.GetWorkbooks();

                        string bookName = Path.GetFileName(exportFileName);

                        wb = ReflectionHelper.GetProperty(wbs, "Item", bookName);
                        sheets = ReflectionHelper.GetProperty(wb, "Sheets");
                        foreach (DataTable dt in ds.Tables)
                            ImportFromExcel(dt, cs, sheets);
                    }
                    finally
                    {
                        if (wbs != null)
                            Marshal.ReleaseComObject(wbs);
                        if (wb != null)
                            Marshal.ReleaseComObject(wb);
                        if (sheets != null)
                            Marshal.ReleaseComObject(sheets);

                        // освобождаем Excel
                        excel.DisplayAlerts = false;
                        excel.Quit();
                    }
                }

                // данные загрузили, теперь будем менять ID в записях
                string[] generators = ExportImportHelper.GetGeneratorsName(ds, tableName);
                // считаем, что все что загрузили имеет линейный вид... 
                // хотя для нескольких таблиц можно сделать по-другому
                if (restoreIDs)
                {
                    IDatabase db = workplace.ActiveScheme.SchemeDWH.DB;
                    try
                    {
                        ExportImportHelper.RestoreIDInDataSet(ds, parentClmnName, refClmnName, generators, db, isHierarchy);
                    }
                    finally
                    {
                        db.Dispose();
                    }
                }
                returnValue = true;
            }
            catch ( Exception e)
            {
                returnValue = false;
                throw new Exception(e.Message);
            }
            finally
            {
                foreach (DataTable dt in ds.Tables)
                    dt.EndLoadData();
                workplace.OperationObj.StopOperation();
            }
            return returnValue;
        }
    }


	/// <summary>
	///  Структура для импортов и экспортов
	/// </summary>
	public struct ExportImportHelper
	{
        public enum fileExtensions { Unknown, xml, xls, doc, txt };

        /// <summary>
        /// возвращает название файла, убрав из него символы, недопустимые для названий файлов
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetCorrectFileName(string fileName)
        {
            string correctFileName = fileName;
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char chr in invalidChars)
            {
                if (correctFileName.IndexOf(chr) != -1)
                    correctFileName = correctFileName.Replace(chr, '_');
            }
            return correctFileName;
        }


        /// <summary>
        /// создает файловый диалог для записи/чтения файлов XML или Excel
        /// </summary>
        /// <param name="startFileName"></param>
        /// <param name="fileExt"></param>
        /// <param name="toSave"></param>
        /// <param name="finishFileName"></param>
        /// <returns></returns>
        public static bool GetFileName(string startFileName, fileExtensions fileExt, bool toSave, ref string finishFileName)
        {
            if (fileExt == fileExtensions.Unknown)
                return GetFileName(startFileName, "*", toSave, ref finishFileName);
            return GetFileName(startFileName, fileExt.ToString(), toSave, ref finishFileName);
        }

        public static bool GetFileName(string startFileName, string fileExtension, bool toSave, ref string finishFileName)
        {
            FileDialog dlg = toSave ? (FileDialog)new SaveFileDialog() : (FileDialog)new OpenFileDialog();
            dlg.AddExtension = true;
            fileExtension = fileExtension.Replace(".", string.Empty);

            switch (fileExtension)
            {
                case "xls":
                    dlg.Filter = OfficeHelper.GetExcelVersionNumber() < 12 ? "Excel документы *.xls|*.xls" : "Excel документы *.xlsx|*.xlsx|Excel документы 97 - 2003 *.xls|*.xls";
                    fileExtension = OfficeHelper.GetExcelVersionNumber() < 12 ? "xls" : "xlsx";
                    break;
                case "xml":
                    dlg.Filter = "XML документы *.xml|*.xml";
                    break;
                case "txt":
                    dlg.Filter = "Текстовые документы *.txt|*.txt";
                    break;
                default: dlg.Filter = string.Format("Произвольные документы *.{0}|*.{0}", fileExtension);
                    break;
            }
            //startFileName = startFileName + "." + fileExtension;
            dlg.FileName = GetCorrectFileName(startFileName);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                finishFileName = dlg.FileName;
                dlg.Dispose();
                return true;
            }

            dlg.Dispose();
            return false;
        }

        /// <summary>
        /// получение имени файла исходя из каталога, в котором установлен клиент
        /// </summary>
        /// <param name="startFileName"></param>
        /// <param name="fileExt"></param>
        /// <param name="finishFileName"></param>
        public static void GetFileName(string startFileName, fileExtensions fileExt, ref string finishFileName)
        {
            finishFileName = AppDomain.CurrentDomain.BaseDirectory + GetCorrectFileName(startFileName) + "." + fileExt;
        }

        public static void GetLocalFileName(string startFileName, string fileExtension, ref string finishFileName)
        {
            fileExtension = fileExtension.Replace(".", string.Empty);
            finishFileName = AppDomain.CurrentDomain.BaseDirectory + GetCorrectFileName(startFileName) + "." + fileExtension;
        }

		/// <summary>
		///  Экспорт в Excel
		/// </summary>
		/// <param name="grid">грид, из которого производится экспотр</param>
        /// <param name="FileName">имя файла, куда будет записываться</param>
		public static void ExportToExcel(UltraGrid grid, string FileName)
		{
            string exportFileName = string.Empty;
            GetFileName(FileName, fileExtensions.xls, ref exportFileName);

            UltraGridExcelExporter excelExpoter = new UltraGridExcelExporter();
            excelExpoter.Export(grid, exportFileName);
            excelExpoter.Dispose();

		    using (ExcelApplication excel = OfficeHelper.CreateExcelApplication())
		    {
		        excel.OpenFile(exportFileName, false, true);
		    }
		}


        private static void CreateXMLAttribute(XmlWriter writer, string attributeName, string attributeValue)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteString(attributeValue);
            writer.WriteEndAttribute();
        }
		
		/// <summary>
		///  Сохранение в XML данных из DataSet
		/// </summary>
		/// <param name="ds">сам DataSet</param>
		/// <param name="FileName"></param>
		public static void SaveToXML(DataSet ds, string FileName)
		{
            string exportFileName = string.Empty;
            if (GetFileName(FileName, fileExtensions.xml, true, ref exportFileName))
			{
                XmlTextWriter writer = new XmlTextWriter(exportFileName, Encoding.GetEncoding(1251));
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = ('\t');
                writer.WriteStartDocument(true);

                ds.WriteXml(writer, XmlWriteMode.IgnoreSchema);

                writer.BaseStream.Dispose();
			}
		}


        /// <summary>
        ///  Сохранение в XML данных из DataSet
        /// </summary>
        /// <param name="ds">сам DataSet</param>
        /// <param name="FileName"></param>
        /// <param name="nameAndSemantic"></param>
        public static void SaveToOuterFormatXML(DataSet ds, string FileName, List<string> nameAndSemantic)
		{
            string exportFileName = string.Empty;
            if (GetFileName(FileName, fileExtensions.xml, true, ref exportFileName))
			{
                FileStream stream = new FileStream(exportFileName, FileMode.Create);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.GetEncoding(1251);
                settings.CheckCharacters = false;
                settings.Indent = true;

                XmlWriter writer = XmlWriter.Create(stream, settings);

                writer.WriteStartDocument(true);
                writer.WriteStartElement("TableMetadata");

                for (int i = 0; i <= nameAndSemantic.Count - 1; i = i + 2)
                    CreateXMLAttribute(writer, nameAndSemantic[i], nameAndSemantic[i + 1]);
      
                ds.WriteXml(writer, XmlWriteMode.IgnoreSchema);

                writer.WriteEndElement();

                writer.WriteEndDocument();

                writer.Flush();

                writer.Close();

                stream.Close();
                stream.Dispose();
			}
		}


		/// <summary>
		/// Выгрузка данных из XML в DataSet
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="workplace"></param>
		/// <returns></returns>
		/// <param name="FileName"></param>
		public static bool LoadFromXML(DataSet ds, IWorkplace workplace, string FileName)
		{
            bool returnValue;
            Debug.Print("ExportImportHelper.LoadFromXml called");
            Debug.Print("Source DataSet cloning started");
			DataSet tmpDs = ds.Clone();
            Debug.Print("Source DataSet cloning finished");
            string importFileName = FileName;

			try
			{
				workplace.OperationObj.Text = "Обработка данных";
				workplace.OperationObj.StartOperation();
                Debug.Print("DataSet loading started");
                tmpDs.ReadXml(importFileName, XmlReadMode.Auto);
                Debug.Print("DataSet loading finished");
                Debug.Print("Copy temporary DataSet to source DataSet started");
                DataTableHelper.CopyDataSet(tmpDs, ref ds, "ID Asc");
                Debug.Print("Copy temporary DataSet to source DataSet finished");
                returnValue = true;
			}
			finally
			{
				workplace.OperationObj.StopOperation();
			}
            return returnValue;
		}

		/// <summary>
		/// Выгрузка данных из XML в DataSet
		/// </summary>
		/// <param name="ds"></param>
        /// <param name="parentClmnName"></param>
        /// <param name="refClmnName"></param>
		/// <param name="isHierarchy"></param>
		/// <param name="tableName"></param>
		/// <param name="workplace"></param>
		/// <param name="fileName"></param>
		public static bool LoadFromXML(DataSet ds, string parentClmnName, string refClmnName, bool isHierarchy, string tableName, IWorkplace workplace, string fileName)
		{
            Debug.Print("ExportImportHelper.LoadFromXml called");
            int tick = Environment.TickCount;
            string importFileName = fileName;

            // Создаем временный DataSet. Этот DataSet копия того, который передаем
            // В нем сохраняется стрктура переданного DataSet'а
            Debug.Print("Source DataSet cloning started");
            DataSet tmpDs = ds.Clone();
            foreach (DataTable table in tmpDs.Tables)
            {
                table.BeginLoadData();
            }
            Debug.Print("Source DataSet cloning finished");
            
            Debug.Print("Array of generators names created");
            // Если выбрали файл, то грузим из него данные 
            //if (GetFileName(fileName, fileExtensions.Xml, false, ref importFileName))
            {
                Debug.Print("File select: {0}", importFileName);
                workplace.OperationObj.Text = "Загрузка данных из файла";
                IDatabase db1 = workplace.ActiveScheme.SchemeDWH.DB;
                try
                {
                    workplace.OperationObj.StartOperation();
                    // Загрузка данных
                    Debug.Print("DataSet loading started");
                    tmpDs.ReadXml(importFileName, XmlReadMode.Auto);
                    Debug.Print("DataSet loading finished");

                    // Получение списка названий таблиц в новом DataSet
                    // будут использоваться для получения имен генераторов из базы
                    string[] generators = GetGeneratorsName(ds, tableName);

                    // Восстановление новых ID в зависимости от количества таблиц в DataSet 
                    RestoreIDInDataSet(tmpDs, parentClmnName, refClmnName, generators, db1, isHierarchy);
                    
                    Debug.Print("Restore ID finished");
                    // Копирование загруженных данных из временного DataSet'а в переданый DataSet

                    foreach (DataTable table in tmpDs.Tables)
                    {
                        table.EndLoadData();
                    }
                    Debug.Print("DataSet load mode disabled");

                    Debug.Print("Copy temporary DataSet to source DataSet started");
                    if (tmpDs.Tables.Count == 1)
                    {
                        string filter = string.Empty;

                        if (refClmnName != string.Empty)
                            filter = string.Format("{0} ASC", refClmnName);

                        DataTableHelper.CopyDataSet(tmpDs, ref ds, filter);
                    }
                    else
                    {
                        DataTableHelper.CopyDataSet(tmpDs, ref ds);
                    }
                    Debug.Print("Copy temporary DataSet to source DataSet finished");
                }
                catch (Exception exception)
                {
                    Debug.Print(exception.Message);
                    throw new Exception(exception.Message, exception);
                }
                finally
                {
                    db1.Dispose();
                    Debug.Print("IDatabase disposed");
                    tmpDs.Dispose();
                    Debug.Print("Temporary DataSet disposed");
                    workplace.OperationObj.StopOperation();
                }
                tick = Environment.TickCount - tick;
                return true;
            }
			//return false;
		}

        /// <summary>
        /// восстанавливает ID в датасете
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="parentClmnName"></param>
        /// <param name="refClmnName"></param>
        /// <param name="generators"></param>
        /// <param name="db"></param>
        /// <param name="isHierarchy"></param>
        internal static void RestoreIDInDataSet(DataSet dataSet, string parentClmnName,
            string refClmnName, string[] generators, IDatabase db, bool isHierarchy)
        {
            if (dataSet.Tables.Count == 1)
                if (!isHierarchy)
                {
                    // Если не надо восстанавливать ID в ссылках на родителя
                    Debug.Print("Restore ID for one NOT hierarchical table starting");
                    RestoreID(dataSet, parentClmnName, generators, db);
                }
                else
                {
                    // Если надо восстанавливать ID в ссылках на родителя
                    Debug.Print("Restore ID for one hierarchical table starting");
                    RestoreIDInHierarchy(dataSet, parentClmnName, refClmnName, generators, db);
                }
            else
            {
                // Если таблиц несколько и нужно восстанавливать ссылки на родителя
                Debug.Print("Restore ID for one multi-table DataSet starting");
                RestoreID(dataSet, parentClmnName, refClmnName, generators, db);
            }
        }

        internal static string[] GetGeneratorsName(DataSet ds, string tableName)
        {
            string[] generators = new string[ds.Tables.Count];
            if (tableName != string.Empty)
                if (!tableName.Contains("g_"))
                    generators[0] = "g_" + tableName;
                else
                    generators[0] = tableName;
            else
            {
                generators[0] = ds.Tables[0].TableName.Split('-')[0].Insert(0, "g_");
                foreach (DataRelation rel in ds.Relations)
                {
                    int index = ds.Relations.IndexOf(rel) + 1;
                    generators[index] = rel.ChildTable.TableName.Split('-')[0].Insert(0, "g_");
                }
            }
            return generators;
        }

        /// <summary>
        /// получает значение генератора, которое не будет входить в диапазон ID загружаемых записей
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="db"></param>
        /// <param name="generatorName"></param>
        /// <param name="refClmnName"></param>
        /// <returns></returns>
        private static int GetNormalGeneratorValue(DataTable Table, IDatabase db, string generatorName, string refClmnName)
        {
            DataRow[] rows = Table.Select("", "ID ASC");
            object lastID = rows[rows.Length - 1]["ID"];

            int CurentID = db.GetGenerator(generatorName);
            // ID, которое сгенерится для последней записи в таблице
            int normalID = CurentID + Table.Rows.Count - 1;
            // если не нашли ни одного ID, то удаляем иерархию, т.к. ее нету
            if (lastID.ToString() == string.Empty)
            {
                DestroyHierarchy(Table, refClmnName);
                return CurentID;
            }
            // если ID в таблице все таки есть, то от него получаем первое ID 
            int firstID = Convert.ToInt32(lastID) - Table.Rows.Count;
            // если не попадают в диапазон, то выходим 
            if (normalID < firstID)
                return normalID;

            while (CurentID <= Convert.ToInt32(lastID))
            {
                CurentID = db.GetGenerator(generatorName);
            }
            return CurentID;
        }

        /// <summary>
        ///  Восстанавливает ID при импорте из XML для плоских классификаторов
        /// </summary>
        /// <param name="ds">DataSet, в котором нужно изменить все ID</param>
        /// <param name="ParentClmnName">колонка, на которую ссылаются</param>
        /// <param name="Generators">ассив строк, содержит названия для построения имен генераторов</param>
        /// <param name="db">объект IDataBase</param>
        internal static void RestoreID(DataSet ds, string ParentClmnName, string[] Generators, IDatabase db)
        {
            // Ссылки не надо менять. Просто бежим по таблице и вместо старых ID проставляем новые,
            // которые получили как новое значение генератора
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                row["ID"] = db.GetGenerator(Generators[0]);
            }
        }

        /// <summary>
        ///  Восстанавливает ID при импорте из XML для иерархических классификаторов
        /// </summary>
        /// <param name="ds">DataSet, в котором нужно изменить все ID и ссылки на них</param>
        /// <param name="ParentClmnName">колонка, на которую ссылаются</param>
        /// <param name="RefClmnName">колонка, которая содержит ссылку</param>
        /// <param name="Generators">массив строк, содержит названия для построения имен генераторов</param>
        /// <param name="db">объект IDataBase</param>
        internal static void RestoreIDInHierarchy(DataSet ds, string ParentClmnName, string RefClmnName, string[] Generators, IDatabase db)
        {
            // Бежим по элементам верхнего уровня, если у них есть элементы нижнего уровня,
            // то вызываем рекурсивный метод, который проставляет все ID для этих элементов
            int CurentID = GetNormalGeneratorValue(ds.Tables[0], db, Generators[0], RefClmnName);
            DataRelation rel = ds.Relations[0];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row.GetParentRow(rel) == null)
                {
                    CurentID = db.GetGenerator(Generators[0]);
                    RestoreID(row.GetChildRows(rel), ds.Relations[0], RefClmnName, Generators, db, CurentID);
                    row["ID"] = CurentID;
                }
            }
        }

        /// <summary>
        ///  Вспомогательный рекурсивный метод. Восстанавливает ID при импорте из XML иерархических классификаторов
        /// </summary>
        /// <param name="rows">массив записей</param>
        /// <param name="Rel">отношение, которое определяет видимую иерархию</param>
        /// <param name="RefClmnName">колонка, которая содержит ссылку</param>
        /// <param name="Generators">массив строк, содержит названия для построения имен генераторов</param>
        /// <param name="db">объект IDataBase</param>
        /// <param name="ParentID">значение ID, которое нужно вставить в поле со ссылкой</param>
        internal static void RestoreID(DataRow[] rows, DataRelation Rel, string RefClmnName, string[] Generators, IDatabase db, int ParentID)
        {
            // Бежим по элементам не самого верхнего уровня, и ставим в ссылку значение "ParentID"
            // Получаем текущее (следующее) значение генератора, запоминаем его
            // Если есть элементы уровнем ниже, то для них вызываем рекурсивно этот же метод
            // Записываем в колонку ID значение, которое запомнили
            //if (rows == null)
            foreach (DataRow row in rows)
            {
                if (row.RowState != DataRowState.Modified)
                {
                    row[RefClmnName] = ParentID;
                    int CurentID = db.GetGenerator(Generators[0]);
                    RestoreID(row.GetChildRows(Rel), Rel, RefClmnName, Generators, db, CurentID);
                    row["ID"] = CurentID;
                }
            }
        }

        /// <summary>
        ///  Восстанавливает ID при импорте из XML 2-х уровневая иерархия с разными таблицами
        /// </summary>
        /// <param name="ds">DataSet, в котором нужно изменить все ID и ссылки на них</param>
        /// <param name="ParentClmnName">колонка, на которую ссылаются</param>
        /// <param name="RefClmnName">колонка, которая содержит ссылку</param>
        /// <param name="Tables">массив строк, содержит названия таблиц в DataSet</param>
        /// <param name="Generators">массив строк, содержит названия для построения имен генераторов</param>
        /// <param name="db">объект IDataBase</param>
        internal static void RestoreID(DataSet ds, string ParentClmnName, string RefClmnName, string[] Generators, IDatabase db)
        {
            // Есть 2 уровня иерархии. Несколько таблиц разных. Бежим по верхнему уровню.
            // Получаем ID запоминаем, бежим по таблицам нижнего уровня (второго). 
            // Проставляем соответствующее значение генератора вместо старого ID, проставляем ссылки. 
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int CurentID = db.GetGenerator(Generators[0]);
                foreach (DataRelation rel in ds.Relations)
                {
                    DataRow[] childRows = row.GetChildRows(rel);
                    int genIndex = ds.Relations.IndexOf(rel);
                    foreach (DataRow childRow in childRows)
                    {
                        int genValue = db.GetGenerator(Generators[genIndex + 1]);
                        childRow[ParentClmnName] = genValue;
                        childRow[RefClmnName] = CurentID;
                    }
                }
                row[ParentClmnName] = CurentID;
            }
        }

        /// <summary>
        /// убираем иерархию
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="refClmnName"></param>
        internal static void DestroyHierarchy(DataTable dt, string refClmnName)
        {
            dt.BeginLoadData();
            foreach (DataRow row in dt.Rows)
                row[refClmnName] = DBNull.Value;
            dt.EndLoadData();
        }

	}
}
