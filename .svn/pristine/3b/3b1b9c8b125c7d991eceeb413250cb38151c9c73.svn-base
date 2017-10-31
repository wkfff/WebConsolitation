using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using Microsoft.Win32;

using Krista.FM.Common;

namespace Krista.FM.Server.DataPumps.DataAccess
{
    #region ExcelSheet
    /// <summary>
    /// Класс для доступа к данным одного листа Excel
    /// </summary>
    public sealed class ExcelSheet
    {
        #region Поля
        // родительский ExcelReader (используется для подгрузки данных)
        private ExcelReader parent = null;
        // название листа (используется для запроса данных)
        private string sheetName = String.Empty;
        public string SheetName
        {
            get { return sheetName; }
        }
        // данные листа
        private DataTable sheetData = null;
        // признак загруженности листа
        private bool sheetLoaded = false;
        #endregion

        #region Конструкторы, деструкторы, очистка ресурсов
        // стандартный конструктор скроем - объекты этого типа нельзя создавать явно
        private ExcelSheet()
        {
        }

        /// <summary>
        /// Конструктор, используется родительским ExcelReader при инициализации списка листов
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sheetName"></param>
        internal ExcelSheet(ExcelReader parent, string sheetName)
        {
            this.parent = parent;
            this.sheetName = sheetName;
        }

        /// <summary>
        /// Выгрузить данные листа. Должен использовать при пакетной обработке файлов больших размеров,
        /// для экономии памяти
        /// </summary>
        public void UnloadData()
        {
            if (sheetData != null)
            {
                sheetData.Clear();
                sheetData = null;
                sheetLoaded = false;
            }
        }
        #endregion

        #region Доступ к данным
        // Проверить, загружены ли данные листа, если нужно - загрузить
        private bool CheckLoaded()
        {
            // если данные уже загружены - ничего далее не делаем
            if (sheetLoaded)
                return sheetLoaded;

            // формируем текст запроса - будем выбирать все что есть на листе
            //Trace.WriteLine(String.Format("Загрузка данных листа '{0}'", sheetName));
            string cmdText = String.Format(@"SELECT * FROM [{0}]", sheetName);
            OleDbCommand cmd = null;
            OleDbDataReader rdr = null;
            try
            {
                // загружаем DataTable 
                cmd = new OleDbCommand(cmdText, parent.Connection);
                cmd.CommandType = CommandType.Text;
                rdr = cmd.ExecuteReader();
                sheetData = new DataTable(sheetName);
                sheetData.BeginLoadData();
                sheetData.Load(rdr);
                sheetData.EndLoadData();
                // выставляем признак загруженности данных листа
                sheetLoaded = true;
            }
            catch
            {
                // вские макросы и форматирование создают листы которые невозможно прочитать через JET
            }
            finally
            {
                // освобождаем объекты
                if (rdr != null)
                    rdr.Close();
                if (cmd != null)
                    cmd.Dispose();
            }
            return sheetLoaded;
        }

        /// <summary>
        /// Количество строк данных. Нумерация - с нуля. При использвании в циклах - кэшировать
        /// </summary>
        /// <returns></returns>
        public int GetRowsCount()
        {
            if (!CheckLoaded())
                return -1;
            else
                return sheetData.Rows.Count;
        }

        /// <summary>
        /// Количество колонок данных. Нумерация - с нуля. При использвании в циклах - кэшировать
        /// </summary>
        /// <returns></returns>
        public int GetColumnsCount()
        {
            if (!CheckLoaded())
                return -1;
            else 
                return sheetData.Columns.Count;
        }

        /// <summary>
        /// Получить данные ячейки. Проверок индексов на вхождение в диапазон не производится.
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <param name="columnIndex">Индекс столбца</param>
        /// <returns></returns>
        public string GetCellValue(int rowIndex, int columnIndex)
        {
            if (!CheckLoaded())
                return "Не удалось загрузить лист";
            else
            {
                return sheetData.Rows[rowIndex][columnIndex].ToString();
            }
        }

        // инициализация таблицы, по четным индексам маппинга - имена столбцов
        private DataTable InitDT(object[] mapping)
        {
            DataTable dt = new DataTable();
            dt.BeginInit();
            for (int i = 0; i < mapping.GetLength(0); i += 2)
                dt.Columns.Add(Convert.ToString(mapping[i]));
            dt.EndInit();
            return dt;
        }

        // загрузить данные листа в таблицу; маппинг: имя поля - индекс столбца 
        private void LoadData(ref DataTable dt, int firstRow, int lastRow, object[] mapping)
        {
            dt.BeginLoadData();
            for (int curRow = firstRow; curRow <= lastRow; curRow++)
                try
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < mapping.GetLength(0); i += 2)
                        row[i / 2] = GetCellValue(curRow, Convert.ToInt32(mapping[i + 1]));
                    dt.Rows.Add(row);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} листа {1} возникла ошибка: {2}",
                        curRow, SheetName, ex.Message), ex);
                }
            dt.EndLoadData();
        }

        /// <summary>
        /// Создает таблицу по данным листа
        /// </summary>
        /// <param name="top">с какой строки</param>
        /// <param name="bottom">до какой строки</param>
        /// <param name="columnsMapping">маппинг: имя поля - индекс столбца </param>
        public DataTable GetDT(int firstRow, int lastRow, object[] mapping)
        {
            DataTable dt = InitDT(mapping);
            LoadData(ref dt, firstRow, lastRow, mapping);
            return dt;
        }

        #endregion
    }
    #endregion

    #region ExcelReader
    /// <summary>
    /// Класс для чтения данных из файлов MS Excel. Рекомендуется для пакетной обработки множества файлов больших объемов.
    /// </summary>
    public class ExcelReader : DisposableObject
    {
        #region Поля
        // коллекция листов
        private Dictionary<string, ExcelSheet> sheets = new Dictionary<string, ExcelSheet>();
        // соединение с файлом
        private OleDbConnection connection = null;
        #endregion

        #region Конструкторы/Деструкторы/освобождение ресурсов
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public ExcelReader()
        {
            // если провайдер Jet не установлен или поврежден - генерируем исклюение
            string jetProviderErrors = CheckJetProvider();
            if (!String.IsNullOrEmpty(jetProviderErrors))
                throw new Exception(
                    String.Format(
                    "Невозможно создать экземпляр класса {0}. Ошибка инициализации провайдера MS Jet4.0: '{1}'",
                    this.GetType().FullName, jetProviderErrors));
        }

        /// <summary>
        /// Детерминированное освобождение ресурсов
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Очистка данных всех листов
        /// </summary>
        public void Clear()
        {
            // очищаем данные листов
            foreach (ExcelSheet sheet in sheets.Values)
            {
                sheet.UnloadData();
            }
            sheets.Clear();
            // закрываем соединения с файлом
            CloseConection();
        }
        #endregion

        #region Проверка MS Jet 4.0
        private const string JET_EXCEL_REGISTRY_SUBKEY_NAME = 
            "SOFTWARE\\Microsoft\\Jet\\4.0\\Engines\\Excel";

        private const string JET_EXCEL_WIN32EXT_KEY_NAME =
            "win32";

        private const string JET_EXCEL_TYPEGUESSROWS_KEY_NAME =
            "TypeGuessRows";

        private string CheckJetProvider()
        {
            string result = String.Empty;
            // установлено ли расширение для чтения файлов Excel
            RegistryKey jetExcelKey = null;
            try
            {
                jetExcelKey = Registry.LocalMachine.OpenSubKey(JET_EXCEL_REGISTRY_SUBKEY_NAME, false);
                if (jetExcelKey == null)
                {
                    result = String.Format("Невозможно прочитать ключ '{0}'", JET_EXCEL_REGISTRY_SUBKEY_NAME);
                }
                else
                {
                    string dllPath = jetExcelKey.GetValue(JET_EXCEL_WIN32EXT_KEY_NAME).ToString();
                    // библиотека (DLL) расширения присутвует на диске?
                    if (!File.Exists(dllPath))
                    {
                        result = String.Format(
                            "Не найдена библиотека-расширение для работы с файлами Excel '{0}'",
                            dllPath);
                    }
                    else
                    {
                        // установлен ли режим полного сканирования файла?
                        int typeGuessRowsValue = Convert.ToInt32(jetExcelKey.GetValue(JET_EXCEL_TYPEGUESSROWS_KEY_NAME));
                        if (typeGuessRowsValue != 0)
                        {
                            // переоткрываем ключ реестра в режиме записи
                            jetExcelKey.Close();
                            jetExcelKey = Registry.LocalMachine.OpenSubKey(JET_EXCEL_REGISTRY_SUBKEY_NAME, true);
                            // устанавливаем режим полного сканирования файла
                            jetExcelKey.SetValue(JET_EXCEL_REGISTRY_SUBKEY_NAME, 0);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = String.Format("Необработанная ошибка при проверке параметров провайдера MS JET 4.0 '{0}'", e.Message);
            }
            finally
            {
                if (jetExcelKey != null)
                    jetExcelKey.Close();
            }
            return result;
        }
        #endregion

        #region Работа с подключением к файлу
        // шаблон строки подключения к файлу Excel
        private const string CONNECTION_STRING_TEMPLATE =
            // .. использовать провайдер MS JET 4.0
            "Provider=Microsoft.Jet.OLEDB.4.0;" + 
            // .. датасоурс - путь к файлу
            "Data Source={0};" + 
            // .. расширеные свойства (специфические для Excel)
            "Extended Properties=" + 
            // .. тип файла Excel 2000 и выше
            "\"Excel 8.0;" +
            // .. не инициализировать заголовки таблицы по первой строке файла
            "HDR=NO;" +
            // .. интерпретировать все данные как текст
            "IMEX=1;\";" +
            // .. не хранить параметры безопасности
            "Persist Security Info=False";

        /// <summary>
        /// Соединение с файлом Excel
        /// </summary>
        internal OleDbConnection Connection
        {
            get { return connection; }
        }

        // Открыть соединение с файлом
        private void OpenConnection(string filePath)
        {
            CloseConection();
            //Trace.WriteLine("Открытие OleDb соединения");
            connection = new OleDbConnection(String.Format(CONNECTION_STRING_TEMPLATE, filePath));
            connection.Open();
        }

        // Закрыть соединение с файлом
        private void CloseConection()
        {
            if ((connection != null) && (connection.State != ConnectionState.Closed))
            {
                connection.Close();
                connection = null;
            }
        }
        #endregion

        #region Общего назначения
        // Инициализировать коллекцию листов
        private void InitSheetsCollection()
        {
            DataTable schemaTable = null;
            try
            {
                //Trace.WriteLine("Получение схемы источника данных");
                // получаем схему файла
                schemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                // для каждого листа создаем элемент
                foreach (DataRow sheetRow in schemaTable.Rows)
                {
                    string sheetName = Convert.ToString(sheetRow["TABLE_NAME"]);
                    sheets.Add(sheetName.ToUpper(), new ExcelSheet(this, sheetName));
                }
            }
            finally
            {
                // на всякий случай - очищаем таблицу схемы
                if (schemaTable != null)
                    schemaTable.Clear();
            }
        }

        /// <summary>
        /// Открыть файл, инициализировать внутренние структуры для чтения данных
        /// </summary>
        /// <param name="filePath">путь к файлу</param>
        public void OpenFile(string filePath)
        {
            // очищаем то что было загружено ранее
            Clear();
            // проверям корректность имени файла и его наличие на диске
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            // открываем соединение с файлом
            OpenConnection(filePath);
            // инициализируем коллекцию листов
            InitSheetsCollection();
        }

        /// <summary>
        /// закрыть предыдущий открытый файл, освободить ресурсы
        /// </summary>
        public void CloseFile()
        {
            Clear();
        }
        #endregion

        #region Эмуляция наиболее используемых методов ExcelHelper
        /// <summary>
        /// Получить лист по индексу
        /// </summary>
        /// <param name="sheetIndex">Индекс листа. Нумерация с нуля</param>
        /// <returns>Лист</returns>
        public ExcelSheet GetSheet(int sheetIndex)
        {
            // проверяем индекс на корректность
            if ((sheetIndex < 0) || (sheetIndex > sheets.Count - 1))
            {
                throw new IndexOutOfRangeException(
                    String.Format("Запрошен лист с индексом {0}. Общее количество листов {1}", sheetIndex, sheets.Count));
            }
            // что-то либо я туплю, либо из коллекции никак не получить элемент по индексу
            int curIndex = 0;
            foreach (ExcelSheet sheet in sheets.Values)
            {
                if (curIndex == sheetIndex)
                    return sheet;
                curIndex++;
            }
            return null;
        }

        /// <summary>
        /// Получить лист по имени
        /// </summary>
        /// <param name="sheetName">Имя листа. Регистронезависимое</param>
        /// <returns>Лист</returns>
        public ExcelSheet GetSheet(string sheetName)
        {
            ExcelSheet sheet = null;
            // приводим имя к формату Jet, т.е.  '<sheetName>$'
            string jetName = String.Concat("'", sheetName.ToUpper(), "$'");
            if (!sheets.TryGetValue(jetName, out sheet))
                // встретились файлы, в которых имена листов в обычном виде <sheetName>$ - почему то нет апострофов
                if (!sheets.TryGetValue(String.Concat(sheetName.ToUpper(), "$"), out sheet))
                    throw new IndexOutOfRangeException(
                        String.Format("Коллекция не содержит листа с именем '{0}'", sheetName));
            return sheet;
        }

        /// <summary>
        /// Получить количество листов в файле
        /// </summary>
        /// <returns></returns>
        public int GetSheetCount()
        {
            return sheets.Count;
        }
        #endregion

    }
    #endregion
}
