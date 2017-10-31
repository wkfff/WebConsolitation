using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;


namespace Krista.FM.Server.DataPumps.DataAccess
{
    /// <summary>
    /// Описание формата 1н.
    /// Структура описания:
    /// Ключ - имя блока, значение - данные блока.
    /// Структура данных блока:
    /// Ключ - имя поля, значение - номер позиции поля.
    /// </summary>
    public sealed class Format1nDescription : Dictionary<string, Dictionary<string, int>> { }


    /// <summary>
    /// Данные отчета формата 1н в иерархическом виде - данные одного блока сведены в один список.
    /// Структура описания:
    /// Ключ - имя блока, значение - данные блока (список всех строк со значениями блока).
    /// Структура данных блока:
    /// Ключ - имя поля, значение - значение поля.
    /// </summary>
    public sealed class Format1nHierarchicalData : Dictionary<string, List<Dictionary<string, string>>> { }


    /// <summary>
    /// Данные блока файла формата 1н
    /// </summary>
    public struct Format1nBlock
    {
        /// <summary>
        /// Имя блока
        /// </summary>
        public string BlockName;

        /// <summary>
        /// Данные блока.
        /// Структура данных блока:
        /// Ключ - имя поля, значение - значение поля.
        /// </summary>
        public Dictionary<string, string> BlockData;
    }


    /// <summary>
    /// Данные отчета формата 1н в линейном виде - в той последовательности, какая приведена в файле.
    /// Структура описания:
    /// Ключ - имя блока, значение - данные блока (список всех строк со значениями блока).
    /// Структура данных блока:
    /// Ключ - имя поля, значение - значение поля.
    /// </summary>
    public sealed class Format1nLinearData : List<Format1nBlock> { }


    /// <summary>
    /// Формат 1н
    /// </summary>
    public enum Format1n
    {
        /// <summary>
        /// Ведомость кассовых поступлений
        /// </summary>
        VKP,

        /// <summary>
        /// Реестр перечисленных поступлений 
        /// </summary>
        DPR
    }


    /// <summary>
    /// Класс разбора форматов 1н
    /// </summary>
    public sealed class Formats1nParser : DisposableObject
    {
        #region Поля

        private Format1nDescription vkpDescription;
        private Format1nDescription dprDescription;
        
        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (vkpDescription != null) vkpDescription.Clear();
                if (dprDescription != null) dprDescription.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Свойства класса

        /// <summary>
        /// Описание формата "Ведомость кассовых поступлений в бюджет (ежедневная/ежемесячная)"
        /// </summary>
        public Format1nDescription VkpDescription
        {
            get 
            {
                // Если описание еще не заполнено - заполняем
                if (vkpDescription == null)
                {
                    vkpDescription = new Format1nDescription();

                    // Блоки
                    vkpDescription.Add("VKP", new Dictionary<string, int>(100));
                    vkpDescription.Add("VKPSTBK", new Dictionary<string, int>(100));

                    // Описание структуры блоков
                    vkpDescription["VKP"].Add("DATE_VED", 3);

                    vkpDescription["VKPSTBK"].Add("DEB_SUM", 11);
                    vkpDescription["VKPSTBK"].Add("CRED_SUM", 12);
                    vkpDescription["VKPSTBK"].Add("KOD_DOH", 8);
                }

                return vkpDescription; 
            }
        }

        /// <summary>
        /// Описание формата "Реестр перечисленных поступлений"
        /// </summary>
        public Format1nDescription DprDescription
        {
            get
            {
                // Если описание еще не заполнено - заполняем
                if (dprDescription == null)
                {
                    dprDescription = new Format1nDescription();

                    // Блоки
                    dprDescription.Add("FK", new Dictionary<string, int>(10));
                    dprDescription.Add("DP", new Dictionary<string, int>(100));
                    dprDescription.Add("DPR", new Dictionary<string, int>(100));

                    // Описание структуры блоков
                    dprDescription["FK"].Add("NUM_VER", 1);

                    dprDescription["DP"].Add("NAME_FO", 1);
                    dprDescription["DP"].Add("DP_DATE", 5);
                    dprDescription["DP"].Add("POL_ACC", 11);

                    dprDescription["DPR"].Add("STR_SUM", 1);
                    dprDescription["DPR"].Add("KOD_DOH", 2);
                }

                return dprDescription;
            }
        }

        #endregion Свойства класса


        #region Функции класса

        /// <summary>
        /// Возвращает данные файла формата 1н.
        /// Данные отчета формата 1н в иерархическом виде - данные одного блока сведены в один список.
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="format1n">Формат файла</param>
        /// <returns>Данные файла</returns>
        public Format1nHierarchicalData ParseHierarchicalFile(FileInfo file, Format1n format1n)
        {
            if (!file.Exists) return null;

            // Находим описание формата файла и разбираем его
            return GetHierarchicalDataFromFile(file, GetFormatDescription(format1n));
        }

        /// <summary>
        /// Возвращает данные файла формата 1н.
        /// Данные отчета формата 1н в линейном виде - в той последовательности, какая приведена в файле.
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="format1n">Формат файла</param>
        /// <returns>Данные файла</returns>
        public Format1nLinearData ParseLinearFile(FileInfo file, Format1n format1n)
        {
            if (!file.Exists) return null;

            // Находим описание формата файла и разбираем его
            return GetLinearDataFromFile(file, GetFormatDescription(format1n));
        }

        /// <summary>
        /// Возвращает описание формата по его имени
        /// </summary>
        /// <param name="format1n">Наименование формата</param>
        /// <returns>Описание</returns>
        private Format1nDescription GetFormatDescription(Format1n format1n)
        {
            switch (format1n)
            {
                case Format1n.VKP: return this.VkpDescription;

                case Format1n.DPR: return this.DprDescription;
            }

            return null;
        }

        /// <summary>
        /// Инициализация структуры данных файла указанного формата
        /// </summary>
        /// <param name="format1nData">Данные файла</param>
        /// <param name="formatDescription">Описание формата</param>
        private void InitFormat1nData(ref Format1nHierarchicalData format1nData, Format1nDescription formatDescription)
        {
            // Заполняем структуру format1nData пустыми данными
            if (format1nData != null) format1nData.Clear();
            format1nData = new Format1nHierarchicalData();

            foreach (KeyValuePair<string, Dictionary<string, int>> kvp in formatDescription)
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string,string>>(1000);
                //list.Add(new Dictionary<string, string>(blockDescription.Count));

                format1nData.Add(kvp.Key, list);

                //Dictionary<string, int> fields = kvp.Value;
                //foreach (KeyValuePair<string, int> fieldsKvp in fields)
                //{
                //    list[0].Add(fieldsKvp.Key);
                //}
            }
        }

        /// <summary>
        /// Парсит файл и возвращает структуру с его данными
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="formatDescription">Описание формата</param>
        /// <returns>Данные файла</returns>
        private Format1nHierarchicalData GetHierarchicalDataFromFile(FileInfo file, Format1nDescription formatDescription)
        {
            Format1nHierarchicalData result = null;
            InitFormat1nData(ref result, formatDescription);

            // Загружаем файл в массив
            string[] fileContent = CommonRoutines.GetFileContent(file, Encoding.GetEncoding(1251));

            // Обходим массив с данными файла и разбираем их согласно описания формата
            int count = fileContent.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                // Массив полей
                string[] fields = fileContent[i].Split('|');
                if (formatDescription.ContainsKey(fields[0]))
                {
                    // Описание данных блока
                    Dictionary<string, int> blockDescription = formatDescription[fields[0]];

                    // Добавляем в список данных блока еще одну строку
                    List<Dictionary<string, string>> blockData = result[fields[0]];
                    Dictionary<string, string> blockRow = new Dictionary<string, string>(blockDescription.Count);
                    blockData.Add(blockRow);

                    foreach (KeyValuePair<string, int> kvp in blockDescription)
                    {
                        blockRow.Add(kvp.Key, fields[kvp.Value]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Парсит файл и возвращает структуру с его данными
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="formatDescription">Описание формата</param>
        /// <returns>Данные файла</returns>
        private Format1nLinearData GetLinearDataFromFile(FileInfo file, Format1nDescription formatDescription)
        {
            Format1nLinearData result = new Format1nLinearData();
            result.Capacity = 2000;

            // Загружаем файл в массив
            string[] fileContent = CommonRoutines.GetFileContent(file, Encoding.GetEncoding(1251));

            // Обходим массив с данными файла и разбираем их согласно описания формата
            int count = fileContent.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                // Массив полей
                string[] fields = fileContent[i].Split('|');
                if (formatDescription.ContainsKey(fields[0]))
                {
                    // Описание данных блока
                    Dictionary<string, int> blockDescription = formatDescription[fields[0]];

                    // Добавляем в список данных еще один блок
                    Format1nBlock block = new Format1nBlock();
                    block.BlockName = fields[0];
                    Dictionary<string, string> blockRow = new Dictionary<string, string>(blockDescription.Count);
                    block.BlockData = blockRow;
                    result.Add(block);

                    foreach (KeyValuePair<string, int> kvp in blockDescription)
                    {
                        blockRow.Add(kvp.Key, fields[kvp.Value]);
                    }
                }
            }

            return result;
        }

        #endregion Функции класса
    }
}