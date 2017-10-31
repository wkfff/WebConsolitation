using System;
using System.IO;
using System.Reflection;
using System.Data;
using System.Runtime.InteropServices;
using Krista.FM.Common;

namespace Krista.FM.Server.DataPumps.DataAccess
{

    /// <summary>
    /// Шрифт ячейки екселя
    /// </summary>
    public class ExcelCellFont
    {
        /// <summary>
        /// Жирный
        /// </summary>
        public bool Bold;

        /// <summary>
        /// Наклонный
        /// </summary>
        public bool Italic;

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name;

        /// <summary>
        /// Размер
        /// </summary>
        public double Size;

        /// <summary>
        /// Зачеркнутый
        /// </summary>
        public bool Strikeout;

        /// <summary>
        /// Подчеркнутый
        /// </summary>
        public bool Underline;
    }

    /// <summary>
    /// Ячейка екселя
    /// </summary>
    public class ExcelCell
    {
        /// <summary>
        /// Шрифт
        /// </summary>
        public ExcelCellFont Font;

        /// <summary>
        /// Значение
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// Индекс границы excel ячейки
    /// </summary>
    public enum ExcelBorderStyles
    {
        DiagonalDown = 5,
        DiagonalUp = 6,
        EdgeLeft = 7,
        EdgeTop = 8,
        EdgeBottom = 9,
        EdgeRight = 10,
        InsideVertical = 11,
        InsideHorizontal = 12,
    }

    /// <summary>
    /// стиль границы ячейки
    /// </summary>
    public enum ExcelLineStyles
    {
        LineStyleNone = -4142,
        Double = -4119,
        Dot = -4118,
        Dash = -4115,
        Continuous = 1,
        DashDot = 4,
        DashDotDot = 5,
        SlantDashDot = 13,
    }

    /// <summary>
    /// Класс для работы с MS Excel
    /// </summary>
    public class ExcelHelper : DisposableObject
    {

        #region Поля

        public bool skipHiddenRows = false;

        private object excelObj = null;
        private object workbooks = null;
        private object workbook = null;
        private object worksheets = null;
        private object worksheet = null;

        #endregion Поля

        #region Константы

        private const string PROG_ID = "Excel.Application";
        private const int XL_LAST_CELL = 11;
        private const int XL_PASTE_COLUMN_WIDTHS = 8;
        private const int XL_PASTE_SPECIAL_OPERATION_NONE = -4142;

        #endregion Константы

        #region Инициализация

        /// <summary>
        /// конструктор класса
        /// </summary>
        public ExcelHelper()
        {
            // Получаем ссылку на интерфейс IDispatch
            try
            {
                // получаем тип объекта
                Type objectType = Type.GetTypeFromProgID(PROG_ID);
                // создаем объект MS Excel
                excelObj = Activator.CreateInstance(objectType);
                // запоминаем ссылку во внутренний список
                //objectsList.Add(obj);
            }
            catch
            {
                // если не удалось - генерируем исключение об ошибке
                throw new Exception("Невозможно создать объект " + PROG_ID);
            }
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {

        }

        #endregion Инициализация

        #region Свойства

        public bool AskToUpdateLinks
        {
            set
            {
                excelObj.GetType().InvokeMember(
                    "AskToUpdateLinks", BindingFlags.SetProperty, null, excelObj, new object[] { value });
            }
        }

        /// <summary>
        /// Вывод встроенных подсказок и предупреждений 
        /// </summary>
        public bool DisplayAlerts
        {
            set
            {
                excelObj.GetType().InvokeMember(
                    "DisplayAlerts", BindingFlags.SetProperty, null, excelObj, new object[] { value });
            }
        }

        /// <summary>
        /// Включить события для объекта Application
        /// </summary>
        public bool EnableEvents
        {
            set
            {
                excelObj.GetType().InvokeMember(
                    "EnableEvents", BindingFlags.SetProperty, null, excelObj, new object[] { value });
            }
        }

        /// <summary>
        /// Версия Excel
        /// </summary>
        public string Version
        {
            get
            {
                try
                {
                    return Convert.ToString(excelObj.GetType().InvokeMember(
                        "Version", BindingFlags.GetProperty, null, excelObj, null));
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        #endregion Свойства

        #region Работа с документом

        /// <summary>
        /// Создать новый Excel-документ (по умолчанию открывается первый лист из коллекции листов)
        /// </summary>
        /// <param name="show">Показать Excel</param>
        public void CreateDocument(bool show)
        {
            excelObj.GetType().InvokeMember(
                "Visible", BindingFlags.SetProperty, null, excelObj, new object[] { show });
            workbooks = excelObj.GetType().InvokeMember(
                "Workbooks", BindingFlags.GetProperty, null, excelObj, null);
            workbook = workbooks.GetType().InvokeMember(
                "Add", BindingFlags.InvokeMethod, null, workbooks, null);
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { 1 });
        }

        /// <summary>
        /// Создать новый Excel-документ (по умолчанию открывается первый лист из коллекции листов)
        /// </summary>
        public void CreateDocument()
        {
            CreateDocument(false);
        }

        /// <summary>
        /// Открыть Excel-документ (по умолчанию открывается первый лист из коллекции листов)
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="show">Показать Excel</param>
        public void OpenDocument(string filename, bool show)
        {
            excelObj.GetType().InvokeMember(
                "Visible", BindingFlags.SetProperty, null, excelObj, new object[] { show });
            workbooks = excelObj.GetType().InvokeMember(
                "Workbooks", BindingFlags.GetProperty, null, excelObj, null);
            workbook = workbooks.GetType().InvokeMember(
                "Open", BindingFlags.InvokeMethod, null, workbooks, new object[] { filename, true });
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { 1 });
        }

        /// <summary>
        /// Открыть Excel-документ (по умолчанию открывается первый лист из коллекции листов)
        /// </summary>
        /// <param name="filename">Имя файла</param>
        public void OpenDocument(string filename)
        {
            OpenDocument(filename, false);
        }

        /// <summary>
        /// Сохранить документ в Excel-файл
        /// </summary>
        /// <param name="filename">Имя файла</param>
        public void SaveDocument(string filename)
        {
            if (File.Exists(filename))
            {
                workbook.GetType().InvokeMember(
                    "Save", BindingFlags.InvokeMethod, null, workbook, null);
            }
            else
            {
                workbook.GetType().InvokeMember(
                    "SaveAs", BindingFlags.InvokeMethod, null, workbook, new object[] { filename });
            }
        }

        /// <summary>
        /// Закрыть Excel-документ
        /// </summary>
        public void CloseDocument()
        {
            try
            {
                workbooks.GetType().InvokeMember(
                    "Close", BindingFlags.InvokeMethod, null, workbooks, null);
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(worksheets);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(workbooks);
                excelObj.GetType().InvokeMember(
                    "Quit", BindingFlags.InvokeMethod, null, excelObj, null);
                Marshal.ReleaseComObject(excelObj);
                GC.GetTotalMemory(true);
            }
            catch
            {
            }
        }

        #endregion Работа с документом

        #region Работа с книгами

        /// <summary>
        /// Добавить новую книгу
        /// </summary>
        /// <param name="activate">Сделать новую книгу активной</param>
        /// <returns>Объект книги</returns>
        public object AddWorkbook(bool activate)
        {
            object workbookObj = workbooks.GetType().InvokeMember(
                "Add", BindingFlags.InvokeMethod, null, workbooks, null);
            if (activate)
            {
                SetWorkbook(workbookObj);
            }
            return workbookObj;
        }

        /// <summary>
        /// Добавить новую книгу и активировать её
        /// </summary>
        /// <returns>Объект книги</returns>
        public object AddWorkbook()
        {
            return AddWorkbook(true);
        }

        /// <summary>
        /// Сохранить рабочую книгу в файл
        /// </summary>
        /// <param name="filename">Имя файла</param>
        public void SaveWorkbook(string filename)
        {
            workbook.GetType().InvokeMember(
                "SaveAs", BindingFlags.InvokeMethod, null, workbook, new object[] { filename });
        }

        /// <summary>
        /// Установить рабочую книгу по её номеру (нумерация начинается с 1)
        /// </summary>
        /// <param name="index">Номер книги в коллекции</param>
        /// <param name="sheetIndex">Номер рабочего листа в книге</param>
        public void SetWorkbook(int index, int sheetIndex)
        {
            workbook = workbooks.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, workbooks, new object[] { index });
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { sheetIndex });
        }

        /// <summary>
        /// Установить рабочую книгу по её номеру (нумерация начинается с 1)
        /// </summary>
        /// <param name="index">Номер книги в коллекции</param>
        public void SetWorkbook(int index)
        {
            SetWorkbook(index, 1);
        }

        /// <summary>
        /// Установить рабочую книгу
        /// </summary>
        /// <param name="workbookObj">Объект книги</param>
        public void SetWorkbook(object workbookObj)
        {
            workbook = workbookObj;
            worksheets = workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { 1 });
        }

        /// <summary>
        /// Получить книгу по её номеру
        /// </summary>
        /// <param name="index">Номер книги</param>
        /// <returns>Объект книги</returns>
        public object GetWorkbook(int index)
        {
            return workbooks.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, workbooks, new object[] { index });
        }

        /// <summary>
        /// Закрыть рабочую книгу
        /// </summary>
        public void CloseWorkbook()
        {
            workbook.GetType().InvokeMember(
                "Worksheets", BindingFlags.GetProperty, null, workbook, null);
        }

        #endregion Работа с книгами

        #region Работа с листами

        /// <summary>
        /// Получить количество листов в книге
        /// </summary>
        /// <returns>Количество листов в книге</returns>
        public int GetWorksheetsCount()
        {
            try
            {
                return Convert.ToInt32(worksheets.GetType().InvokeMember(
                    "Count", BindingFlags.GetProperty, null, worksheets, null));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Установить рабочий лист по его номеру (нумерация начинается с 1)
        /// </summary>
        /// <param name="index">Номер листа в книге</param>
        public void SetWorksheet(int index)
        {
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { index });
        }

        /// <summary>
        /// Установить рабочий лист по его названию
        /// </summary>
        /// <param name="name">Имя листа в книге</param>
        public void SetWorksheet(string name)
        {
            worksheet = worksheets.GetType().InvokeMember(
                "Item", BindingFlags.GetProperty, null, worksheets, new object[] { name });
        }

        /// <summary>
        /// Получить имя рабочего листа
        /// </summary>
        /// <returns></returns>
        public string GetWorksheetName()
        {
            try
            {
                return (string)worksheet.GetType().InvokeMember(
                    "Name", BindingFlags.GetProperty, null, worksheet, null);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion Работа с листами

        #region Работа с диапазонами ячеек

        /// <summary>
        /// Получить диапазон ячеек из рабочего листа
        /// </summary>
        /// <param name="cells">Диапазон ячеек в формате CR или C1R1:C2R2 (например, A1 или B1:E12)</param>
        /// <returns>Объект диапазона ячеек</returns>
        public object GetRange(string cells)
        {
            return worksheet.GetType().InvokeMember(
                "Range", BindingFlags.GetProperty, null, worksheet, new object[] { cells });
        }

        /// <summary>
        /// Объединить два диапазона
        /// </summary>
        /// <param name="range1">Объект первого диапазона ячеек</param>
        /// <param name="range2">Объект второго диапазона ячеек</param>
        /// <returns>Объединенный диапазон</returns>
        public object UnionRanges(object range1, object range2)
        {
            return excelObj.GetType().InvokeMember(
                "Union", BindingFlags.InvokeMethod, null, excelObj, new object[] { range1, range2 });
        }

        /// <summary>
        /// Получить значение ячейки рабочего листа
        /// </summary>
        /// <param name="cellAddress">Адрес ячейки</param>
        /// <returns>Значение</returns>
        public string GetValue(string cellAddress)
        {
            try
            {
                object range = GetRange(cellAddress);
                return Convert.ToString(range.GetType().InvokeMember(
                    "Value", BindingFlags.GetProperty, null, range, null));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Получить значение ячейки рабочего листа
        /// </summary>
        /// <param name="rowIndex">Номер строки (начиная с 1)</param>
        /// <param name="colIndex">Номер столбца (начиная с 1)</param>
        /// <returns>Значение</returns>
        public string GetValue(int rowIndex, int colIndex)
        {
            try
            {
                object cell = worksheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex, colIndex });
                return Convert.ToString(cell.GetType().InvokeMember(
                    "Value", BindingFlags.GetProperty, null, cell, null));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Записать значение в ячейку рабочего листа
        /// </summary>
        /// <param name="cellAddress">Адрес ячейки</param>
        /// <param name="cellValue">Значение</param>
        public void SetValue(string cellAddress, string cellValue)
        {
            try
            {
                object range = GetRange(cellAddress);
                range.GetType().InvokeMember(
                    "Value", BindingFlags.SetProperty, null, range, new object[] { cellValue });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Записать значение в ячейку рабочего листа
        /// </summary>
        /// <param name="rowIndex">Номер строки (начиная с 1)</param>
        /// <param name="colIndex">Номер столбца (начиная с 1)</param>
        /// <param name="cellValue">Значение</param>
        public void SetValue(int rowIndex, int colIndex, string cellValue)
        {
            try
            {
                object cell = worksheet.GetType().InvokeMember(
                   "Cells", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex, colIndex });
                cell.GetType().InvokeMember(
                    "Value", BindingFlags.SetProperty, null, cell, new object[] { cellValue });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Скопировать диапазон ячеек рабочего листа в буфер обмена
        /// </summary>
        /// <param name="range">Объект диапазона ячеек</param>
        public void CopyRange(object range)
        {
            // без параметров диапазон копируется в буфер обмена
            range.GetType().InvokeMember("Copy", BindingFlags.InvokeMethod, null, range, null);
        }

        /// <summary>
        /// Вставить диапазон ячеек из буфера обмена в указанный диапазон ячеек рабочего листа
        /// </summary>
        /// <param name="range">Объект диапазона ячеек</param>
        public void PasteRange(object range)
        {
            worksheet.GetType().InvokeMember(
                "Paste", BindingFlags.InvokeMethod, null, worksheet, new object[] { range, Type.Missing });
        }

        /// <summary>
        /// Специальная вставка диапазона ячеек из буфера обмена с сохранением ширины столбцов
        /// </summary>
        /// <param name="range">Объект диапазона ячеек</param>
        public void PasteSpecialRange(object range)
        {
            range.GetType().InvokeMember(
                "PasteSpecial", BindingFlags.InvokeMethod, null, range,
                new object[] { XL_PASTE_COLUMN_WIDTHS, XL_PASTE_SPECIAL_OPERATION_NONE, false, false });
            worksheet.GetType().InvokeMember(
                "Paste", BindingFlags.InvokeMethod, null, worksheet, new object[] { range, Type.Missing });
        }

        /// <summary>
        /// Получить количество строк в рабочем листе
        /// </summary>
        /// <returns>Количество строк</returns>
        public int GetRowsCount()
        {
            try
            {
                object cells = worksheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, worksheet, null);
                object range = cells.GetType().InvokeMember(
                    "SpecialCells", BindingFlags.GetProperty, null, cells, new object[] { XL_LAST_CELL });
                return Convert.ToInt32(range.GetType().InvokeMember(
                    "Row", BindingFlags.GetProperty, null, range, null));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Установить ширину столбца
        /// </summary>
        /// <param name="columnIndex">Номер столбца (начиная с 1)</param>
        /// <param name="width">Ширина</param>
        public void SetColumnWidth(int columnIndex, float width)
        {
            object range = worksheet.GetType().InvokeMember(
                "Columns", BindingFlags.GetProperty, null, worksheet, new object[] { columnIndex });
            object column = range.GetType().InvokeMember(
                "EntireColumn", BindingFlags.GetProperty, null, range, null);
            column.GetType().InvokeMember(
                "ColumnWidth", BindingFlags.SetProperty, null, column, new object[] { width });
        }

        /// <summary>
        /// Установить формат отображения данных для всех ячеек указанного столбца
        /// </summary>
        /// <param name="columnIndex">Номер столбца (начиная с 1)</param>
        /// <param name="format">Формат данных (например, "#,##0.00")</param>
        public void SetColumnFormat(int columnIndex, string format)
        {
            object range = worksheet.GetType().InvokeMember(
                "Columns", BindingFlags.GetProperty, null, worksheet, new object[] { columnIndex });
            range.GetType().InvokeMember(
                "NumberFormat", BindingFlags.SetProperty, null, range, new object[] { format });
        }

        /// <summary>
        /// Проверить, является ли строка скрытой
        /// </summary>
        /// <param name="rowIndex">Номер строки (начиная с 1)</param>
        /// <returns></returns>
        public bool IsRowHidden(int rowIndex)
        {
            try
            {
                object row = worksheet.GetType().InvokeMember(
                    "Rows", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex });
                return Convert.ToBoolean(row.GetType().InvokeMember(
                    "Hidden", BindingFlags.GetProperty, null, row, null));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// получить стиль границы ячейки
        /// </summary>
        /// <param name="row">номер строки</param>
        /// <param name="column">номер столбца</param>
        /// <param name="index">индекс границы</param>
        /// <returns></returns>
        public ExcelLineStyles GetBorderStyle(int rowIndex, int colIndex, ExcelBorderStyles borderIndex)
        {
            object cell = worksheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, worksheet, new object[] { rowIndex, colIndex });
            
            object border = cell.GetType().InvokeMember("Borders", BindingFlags.GetProperty, null, cell, new object[] {borderIndex});
            return (ExcelLineStyles)(border.GetType().InvokeMember("LineStyle", BindingFlags.GetProperty, null, border, null));
        }

        #endregion Работа с диапазонами ячеек

        #region Устаревшие методы

        #region Работа с объектом Excel

        // возвращает указатель для дальнейшей работы
        private object GetExcelObject()
        {
            return excelObj;
        }

        public void SetAskToUpdateLinks(object obj, bool displayAlert)
        {
            obj.GetType().InvokeMember("AskToUpdateLinks", BindingFlags.SetProperty, null, obj, new object[1] { displayAlert });
        }

        public void SetDisplayAlert(object obj, bool displayAlert)
        {
            obj.GetType().InvokeMember("DisplayAlerts", BindingFlags.SetProperty, null, obj, new object[1] { displayAlert });
        }

        /// <summary>
        /// Показать/скрыть объект MS Excel
        /// </summary>
        /// <param name="obj">объект MS Excel</param>
        /// <param name="visible">признак видимости</param>
        public void SetObjectVisible(object obj, bool visible)
        {
            obj.GetType().InvokeMember("Visible", BindingFlags.SetProperty, null, obj, new object[1] { visible });
        }

        /// <summary>
        /// Открыть документ Excel
        /// </summary>
        /// <param name="show">показать Excel</param>
        /// <returns>указатель на объект Excel для дальнейшей работы</returns>
        public object OpenExcel(bool show)
        {
            try
            {
                // загружаем Excel
                object obj = GetExcelObject();

                // Подавляем появление любых диалогов
                ReflectionHelper.SetProperty(obj, "ScreenUpdating", false);
                ReflectionHelper.SetProperty(obj, "Interactive", false);
                // отключаем предупреждения
                SetDisplayAlert(obj, false);
                // отключаем йобаные предупреждения о связях
                SetAskToUpdateLinks(obj, false);

                // показываем Excel если необходимо
                if (show)
                {
                    SetObjectVisible(obj, true);
                }

                if (obj == null)
                    throw new Exception();

                return obj;
            }
            catch
            {
                throw new Exception("Ошибка при запуске MS Excel. Возможно, он не установлен.");
            }
        }

        /// <summary>
        /// Сохранить файл
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="fileName"></param>
        public void SaveChanges(object workbook, string fileName)
        {
            try
            {
                // сохраняем в файл
                object[] args11 = new object[11] {
				fileName, // Filename: OleVariant; 
				Missing.Value,  // FileFormat: OleVariant; 
				Missing.Value,  // Password: OleVariant; 
				Missing.Value,  // WriteResPassword: OleVariant; 
				Missing.Value,  // ReadOnlyRecommended: OleVariant; 
				Missing.Value,  // CreateBackup: OleVariant; 
				Missing.Value,  // AccessMode: XlSaveAsAccessMode; 
				Missing.Value,  // ConflictResolution: OleVariant; 
				Missing.Value,  // AddToMru: OleVariant; 
				Missing.Value,  // TextCodepage: OleVariant; 
				Missing.Value//,  // TextVisualLayout: OleVariant; 
				//CurrentLCID     // lcid: Integer
			    };

                workbook.GetType().InvokeMember("SaveAs", BindingFlags.InvokeMethod, null, workbook, args11);
            }
            catch
            {
            }
        }

        public void CloseExcel(ref object obj)
        {
            // параметр obj уже не нужен, оставлен для совместимости
            if (excelObj == null)
            {
                obj = null;
                return;
            }
            // закрываем книги
            CloseWorkBooks(excelObj);
        }

        public override void Close()
        {
            try
            {
                // если Excel еще не закрыт - пытаемся закрыть
                // отключаем предупреждения 
                SetDisplayAlert(excelObj, false);
                // вызываем метод Quit
                excelObj.GetType().InvokeMember("Quit", BindingFlags.InvokeMethod, null, excelObj, null);
                // пытаемся уничтожить COM-объект
                Marshal.ReleaseComObject(excelObj);
                // Всякие Excel'ы не всегда сразу удаляют свои объект
                // Поэтому хотя бы уничтожаем все ссылки в памяти .NET Framework
                // Это тоже не всегда помогает...
                excelObj = null;
                GC.GetTotalMemory(true);
            }
            catch
            {
            }
            base.Close();
        }

        #endregion Работа с объектом Excel

        #region Работа с книгами

        /// <summary>
        /// получение коллекции Workbooks
        /// </summary>
        /// <param name="applicationObject">объект Excel</param>
        /// <returns>коллекция Workbooks</returns>
        private object GetWorkbooks(object applicationObject)
        {
            return applicationObject.GetType().InvokeMember(
                "Workbooks", BindingFlags.GetProperty, null, applicationObject, null);
        }

        /// <summary>
        /// Закрыть коллекцию книг
        /// </summary>
        /// <param name="obj">объект Excel</param>
        public void CloseWorkBooks(object obj)
        {
            object workbooks = GetWorkbooks(obj);
            SetDisplayAlert(obj, false);
            workbooks.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, workbooks, null);
        }

        public object InitWorkBook(ref object obj, string fileFullName)
        {
            // параметр obj уже не нужен, оставлен для совместимости
            obj = excelObj;
            return GetWorkbook(obj, fileFullName, true);
        }

        /// <summary>
        /// Создание рабочей книги
        /// </summary>
        /// <param name="obj">Объект Excel</param>
        /// <returns>Книга</returns>
        public object CreateWorkbook(object obj)
        {
            try
            {
                object workbooks = GetWorkbooks(obj);
                return workbooks.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, workbooks, null);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Проверка расширения файла. Используется для проверки корректности параметров методов
        /// </summary>
        /// <param name="fileName">имя фалйа</param>
        /// <returns>true/false в зависимости от принадлежности расширения</returns>
        private bool CheckFileExt(string fileName)
        {
            string fileExt = Path.GetExtension(fileName);
            return string.Compare(fileExt.ToUpper(), ".XLS") == 0;
        }

        /// <summary>
        /// Проверка имени файла на существование и принадлежность к объекту данного типа (по расширению).
        /// Используется для проверки параметров функций
        /// </summary>
        /// <param name="fileName">имя файла</param>
        private void CheckFile(string fileName)
        {
            // проверяем принадлежность
            if (!CheckFileExt(fileName)) throw new Exception("Файл '" + fileName + "' не является файлом Excel");

            // проверяем наличие файла
            if (!File.Exists(fileName)) throw new Exception("Файл '" + fileName + "' не существует");
        }

        /// <summary>
        /// Открывает книгу (файл) екселя
        /// </summary>
        /// <param name="obj">Объект екселя</param>
        /// <param name="fileName">имя файла</param>
        /// <param name="openReadOnly">открыть только для чтения</param>
        /// <returns>Книга</returns>
        public object GetWorkbook(object obj, string fileName, bool openReadOnly)
        {
            // проверяем корректность файла
            CheckFile(fileName);

            try
            {
                // получаем коллекцию Workbooks
                object workbooksObj = GetWorkbooks(obj);

                // Открываем файл
                object[] args15 = new object[15];
                args15[0] = fileName;
                args15[1] = Type.Missing;
                args15[2] = openReadOnly;
                args15[3] = Type.Missing;
                args15[4] = Type.Missing;
                args15[5] = Type.Missing;
                args15[6] = Type.Missing;
                args15[7] = Type.Missing;
                args15[8] = Type.Missing;
                args15[9] = Type.Missing;
                args15[10] = Type.Missing;
                args15[11] = Type.Missing;
                args15[12] = Type.Missing;
                args15[13] = Type.Missing;
                args15[14] = Type.Missing;

                // Пробуем открыть книгу
                return workbooksObj.GetType().InvokeMember("Open", BindingFlags.InvokeMethod, null, workbooksObj, args15);
            }
            catch
            {
                return null;
            }
        }

        #endregion Работа с книгами

        #region Работа с страницами

        /// <summary>
        /// Возвращает указанную страницу документа
        /// </summary>
        /// <param name="workbook">Документ</param>
        /// <param name="sheetIndex">Индекс страницы (начиная с 1)</param>
        /// <returns>Страница</returns>
        public object GetSheet(object workbook, int sheetIndex)
        {
            try
            {
                object sheets = workbook.GetType().InvokeMember(
                    "Sheets", BindingFlags.GetProperty, null, workbook, null);
                return sheets.GetType().InvokeMember("Item",
                    BindingFlags.GetProperty, null, sheets, new object[] { sheetIndex });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает указанную страницу документа
        /// </summary>
        /// <param name="workbook">Документ</param>
        /// <param name="sheetName">Название страницы</param>
        /// <returns>Страница</returns>
        public object GetSheet(object workbook, string sheetName)
        {
            try
            {
                object sheets = workbook.GetType().InvokeMember(
                    "Sheets", BindingFlags.GetProperty, null, workbook, null);
                return sheets.GetType().InvokeMember("Item",
                    BindingFlags.GetProperty, null, sheets, new object[] { sheetName });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает указанную страницу документа
        /// </summary>
        /// <param name="workbook">Документ</param>
        /// <param name="sheetIndex">Индекс страницы (начиная с 1)</param>
        /// <returns>Страница</returns>
        public int GetSheetCount(object workbook)
        {
            try
            {
                object sheets = workbook.GetType().InvokeMember(
                    "Sheets", BindingFlags.GetProperty, null, workbook, null);
                return Convert.ToInt32(sheets.GetType().InvokeMember("Count",
                    BindingFlags.GetProperty, null, sheets, null));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Возвращает название страницы
        /// </summary>
        /// <param name="sheet">Страница</param>
        /// <returns>Название</returns>
        public string GetSheetName(object sheet)
        {
            try
            {
                return Convert.ToString(sheet.GetType().InvokeMember(
                    "Name", BindingFlags.GetProperty, null, sheet, null));
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion Работа с страницами

        #region Работа с ячейками

        /// <summary>
        /// Инициализирует структуру ячейки данными екселя
        /// </summary>
        /// <param name="cell">Ячейка</param>
        /// <returns>Структура ячейки</returns>
        private ExcelCell InitExcelCell(object cell)
        {
            ExcelCell result = new ExcelCell();
            // !!! Шрифт ячейки инициализируем только если очень-очень нужно
            if (InitCellFont)
                result.Font = GetCellFont(cell);
            result.Value = GetCellValue(cell);
            return result;
        }

        /// <summary>
        /// Возвращает указанную ячейку страницы
        /// </summary>
        /// <param name="sheet">Страница</param>
        /// <param name="rowIndex">Номер строки (начиная с 1)</param>
        /// <param name="colIndex">Номер столбца (начиная с 1)</param>
        /// <returns>Ячейка</returns>
        public ExcelCell GetCell(object sheet, int rowIndex, int colIndex)
        {
            try
            {
                return InitExcelCell(sheet.GetType().InvokeMember(
                    "Cells", BindingFlags.GetProperty, null, sheet, new object[] { rowIndex, colIndex }));
            }
            catch
            {
                return InitExcelCell(null);
            }
        }

        /// <summary>
        /// Возвращает указанную ячейку страницы
        /// </summary>
        /// <param name="sheet">Страница</param>
        /// <param name="range">Адрес ячейки</param>
        /// <returns>Ячейка</returns>
        public ExcelCell GetCell(object sheet, string range)
        {
            return InitExcelCell(sheet.GetType().InvokeMember("Range",
                BindingFlags.GetProperty, null, sheet, new object[] { range }));
        }

        /// <summary>
        /// Возвращает значение ячейки
        /// </summary>
        /// <param name="cell">Ячейка</param>
        /// <returns>Значение ячейки</returns>
        private string GetCellValue(object cell)
        {
            if (cell != null)
            {
                return Convert.ToString(
                    cell.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, cell, null)).Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// Запись ячейки в страницу
        /// </summary>
        /// <param name="sheet">Страница</param>
        /// <param name="rowIndex">Номер строки (начиная с 1)</param>
        /// <param name="colIndex">Номер столбца (начиная с 1)</param>
        /// <param name="cellValue">Значение ячейки</param>
        public void SetCell(object sheet, int rowIndex, int colIndex, string cellValue)
        {
            try
            {
                sheet.GetType().InvokeMember(
                    "Cells", BindingFlags.SetProperty, null, sheet, new object[] { rowIndex, colIndex, cellValue });
            }
            catch
            {
            }
        }

        private bool initCellFont = false;
        public bool InitCellFont
        {
            get { return initCellFont; }
            set { initCellFont = value; }
        }

        /// <summary>
        /// Возвращает шрифт ячейки
        /// </summary>
        /// <param name="cell">Ячейка</param>
        /// <returns>Шрифт ячейки</returns>
        private ExcelCellFont GetCellFont(object cell)
        {
            ExcelCellFont result = new ExcelCellFont();

            if (cell != null)
            {
                object font = cell.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, cell, null);
                result.Bold = Convert.ToBoolean(font.GetType().InvokeMember("Bold", BindingFlags.GetProperty, null, font, null));
                result.Italic = Convert.ToBoolean(font.GetType().InvokeMember("Italic", BindingFlags.GetProperty, null, font, null));
                result.Name = Convert.ToString(font.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, font, null));
                result.Size = (float)Convert.ToDouble(font.GetType().InvokeMember("Size", BindingFlags.GetProperty, null, font, null));
                result.Strikeout = Convert.ToBoolean(font.GetType().InvokeMember("Strikethrough", BindingFlags.GetProperty, null, font, null));
                result.Underline = Convert.ToBoolean(font.GetType().InvokeMember("Underline", BindingFlags.GetProperty, null, font, null));
            }

            return result;
        }

        #endregion Работа с ячейками

        #region получение датасета по заданному маппингу

        private bool IsRowHidden(object row)
        {
            if (row == null)
                return false;
            return Convert.ToBoolean(row.GetType().InvokeMember("Hidden", BindingFlags.GetProperty, null, row, null));
        }

        public object GetExcelRow(object sheet, int rowIndex)
        {
            return sheet.GetType().InvokeMember("Rows", BindingFlags.GetProperty, null, sheet, new object[] { rowIndex });
        }

        /// <summary>
        /// Создает датасет по данным страницы файла
        /// </summary>
        /// <param name="sheet">Страница отчета</param>
        /// <param name="top">Номер верхней строки области данных датасета</param>
        /// <param name="bottom">Номер нижней строки области данных датасета</param>
        /// <param name="columnsMapping">Массив пар имя_поля-номер_столбца. Имя_поля - имя поля в датасете, куда
        /// будут заноситься данные из столбца отчета с номером номер_столбца. Если null - все столбцы.</param>
        /// <returns>Датасет</returns>
        public DataTable GetSheetDataTable(object sheet, int top, int bottom, object[] columnsMapping)
        {
            DataTable dt = new DataTable();

            // Создаем столбцы 
            dt.BeginInit();
            for (int i = 0; i < columnsMapping.GetLength(0); i += 2)
            {
                if (columnsMapping[i] != null)
                {
                    dt.Columns.Add(Convert.ToString(columnsMapping[i]));
                }
                else
                {
                    dt.Columns.Add();
                }
            }
            dt.EndInit();

            // Перекачиваем данные из отчета в датасет
            dt.BeginLoadData();
            for (int i = top; i <= bottom; i++)
            {
                if (skipHiddenRows)
                {
                    object excelRow = GetExcelRow(sheet, i);
                    if (IsRowHidden(excelRow))
                        continue;
                }

                DataRow row = dt.NewRow();

                for (int j = 0; j < columnsMapping.GetLength(0); j += 2)
                {
                    row[j / 2] = GetCell(sheet, i, Convert.ToInt32(columnsMapping[j + 1])).Value;
                }

                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        /// <summary>
        /// Создает датасет по данным страницы файла
        /// </summary>
        /// <param name="sheet">Страница отчета</param>
        /// <param name="top">Номер верхней строки области данных датасета</param>
        /// <param name="bottom">Номер нижней строки области данных датасета</param>
        /// <param name="startColumn">Номер первого столбца</param>
        /// <param name="endColumn">Номер последнего столбца</param>
        /// <returns>Датасет</returns>
        public DataTable GetSheetDataTable(object sheet, int top, int bottom, int startColumn, int endColumn)
        {
            object[] columnsMapping = new object[(endColumn - startColumn + 1) * 2];
            int index = 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                columnsMapping[index] = i;
                index += 2;
            }

            return GetSheetDataTable(sheet, top, bottom, columnsMapping);
        }

        // инициализация таблицы, по четным индексам маппинга - имена столбцов
        private DataTable InitDT(object[][] mappings)
        {
            DataTable dt = new DataTable();
            dt.BeginInit();
            for (int i = 0; i < mappings.GetLength(0); i++)
                for (int j = 0; j < mappings[i].GetLength(0); j += 2)
                    dt.Columns.Add(Convert.ToString(mappings[i][j]));
            dt.EndInit();
            return dt;
        }

        // загрузить данные листа в таблицу; маппинг: имя поля таблицы - индекс столбца 
        private void LoadData(ref DataTable dt, object[] sheets, int[] firstRows, int[] lastRows, object[][] mapping)
        {
            dt.BeginLoadData();
            int rowsCount = lastRows[0] - firstRows[0];
            for (int curRow = 0; curRow <= rowsCount; curRow++)
            {
                int rowIndex = 0;
                DataRow row = dt.NewRow();
                for (int i = 0; i < sheets.GetLength(0); i++)
                    try
                    {
                        for (int j = 0; j < mapping[i].GetLength(0); j += 2)
                        {
                            row[rowIndex] = GetCell(sheets[i], curRow + firstRows[i], Convert.ToInt32(mapping[i][j + 1])).Value;
                            rowIndex++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("При обработке строки {0} листа {1} возникла ошибка: {2}",
                            curRow + firstRows[i], GetSheetName(sheets[i]), ex.Message), ex);
                    }
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
        }

        /// <summary>
        /// Создает таблицу по данным листа
        /// </summary>
        /// <param name="sheets">Страница отчета</param>
        /// <param name="firstRow">с какой строки</param>
        /// <param name="lastRow">до какой строки</param>
        /// <param name="columnsMapping">маппинг: имя поля таблицы - индекс столбца </param>
        public DataTable GetSheetDataTable(object[] sheets, int[] firstRows, int[] lastRows, object[][] mappings)
        {
            DataTable dt = InitDT(mappings);
            LoadData(ref dt, sheets, firstRows, lastRows, mappings);
            return dt;
        }

        #endregion получение датасета по заданному маппингу

        #endregion Устаревшие методы

    }
}
