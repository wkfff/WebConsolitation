using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Components;
using Krista.FM.Common;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common.ExportImport
{
    public class ExcelExportImportHelper
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
        private static void ExportDataToExcel(DataTable table, GridColumnsStates cs, bool exportAllColumns, object sheets, object excel)
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
                            ExportDataToExcel(dt, cs, exportAllColumns, sheets, excel.OfficeApp);
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


        private static List<string> GetColumnsNamesFromSheet(object sheet, int columnNameIndex)
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
                object range = GetRange(sheet, string.Format("{0}{1}", excelColumn, columnNameIndex));
                object comment = ReflectionHelper.GetProperty(range, "Comment");
                if (comment == null)
                {
                    Marshal.ReleaseComObject(range);
                    continue;
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


        private static void ImportFromExcel(DataTable dt, GridColumnsStates cs, object sheets, int columnNameIndex)
        {
            object sheet = null;
            try
            {
                try
                {
                    // пытаемся получить лист екселя с названием таблицы
                    sheet = GetSheet(sheets, 1);
                }
                catch
                {
                    return;
                }
                ReflectionHelper.CallMethod(sheet, "Activate");
                List<string> columns = GetColumnsNamesFromSheet(sheet, columnNameIndex);
                // последняя колонка в выбираемом диапазоне 
                string excelLastColumn = GetColumnNameFromNumber(columns.Count - 1);//Convert.ToChar('A' + (columns.Count % 26) - 1);
                // строка, с которой начинаются данные
                int excelRows = columnNameIndex + 1;
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
            GridColumnsStates cs, bool isHierarchy, bool restoreIDs, string fileName, IWorkplace workplace, string tableName, int columNameIndex)
        {
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
                            ImportFromExcel(dt, cs, sheets, columNameIndex);
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
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                foreach (DataTable dt in ds.Tables)
                    dt.EndLoadData();
                workplace.OperationObj.StopOperation();
            }
        }
    }
}
