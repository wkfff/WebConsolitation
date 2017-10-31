using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;

using ICSharpCode.SharpZipLib.Zip;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.Common
{
	/// <summary>
	/// Класс с функциями, общими для всех закачек
	/// </summary>
	public static class CommonRoutines
    {
        #region Поля

        private static Dictionary<char, int> numeration36 = new Dictionary<char, int>(36);

        #endregion Поля

        #region Константы

        private const string ExtractingDirectoryPrefix = "_EXTRACTED_";

        #endregion Константы

        #region Функции преобразования коллекций, списков и т.п.

        /// <summary>
		/// Преобразует коллекцию в строку (элементы через запятую)
		/// </summary>
		/// <param name="collection">Коллекция</param>
		/// <returns>Результат</returns>
		public static string CollectionToString(ICollection collection)
		{
			string result = string.Empty;

			if (collection != null)
			{
                List<string> list = new List<string>();

				// формируем список имен столбцов
				foreach (DictionaryEntry item in collection)
				{
					list.Add(Convert.ToString(item.Value));
				}
				result = string.Join(", ", list.ToArray());
			}

			return result;
		}

        /// <summary>
        /// Массив целочисленных значений в строку (элементы через запятую)
        /// </summary>
        /// <param name="list">Массив</param>
        /// <returns>Результат</returns>
        public static string ListToString(List<int> list)
        {
            string result = string.Empty;

            if (list != null)
            {
                List<string> tmpList = new List<string>();

                // формируем список имен столбцов
                for (int i = 0; i < list.Count; i++)
                {
                    tmpList.Add(Convert.ToString(list[i]));
                }
                result = string.Join(", ", tmpList.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Массив строковых значений в строку (элементы через запятую)
        /// </summary>
        /// <param name="list">Массив</param>
        /// <returns>Результат</returns>
        public static string ListToString(List<string> list)
        {
            string result = string.Empty;

            if (list != null)
            {
                result = string.Join(", ", list.ToArray());
            }

            return result;
        }

        #endregion Функции преобразования коллекций, списков и т.п.

        #region Функции преобразования структур данных (баз данных)

        /// <summary>
		/// Преобразует коллекцию в строку (элементы через запятую)
		/// </summary>
		/// <param name="collection">Коллекция</param>
		/// <returns>Результат</returns>
		public static string DataAttributeCollectionToString(IDataAttributeCollection attrs)
		{
			List<string> list = new List<string>();
			ICollection<IDataAttribute> coll = attrs.Values;
			string result = string.Empty;

			if (attrs != null)
			{
				// формируем список имен столбцов
				foreach (IDataAttribute item in coll)
				{
					list.Add(item.Name);
				}
				result = string.Join(", ", list.ToArray());
			}

			return result;
		}

		/// <summary>
		/// Создает строку из значений полей столбца
		/// </summary>
		/// <param name="dt">Таблица</param>
		/// <param name="columnName">Имя столбца</param>
		/// <returns>Строка</returns>
		public static string DataTableColumnToString(DataTable dt, string columnName)
		{
			string result = string.Empty;
			if (dt == null || !dt.Columns.Contains(columnName)) return result;

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (!dt.Rows[i].IsNull(columnName))
				{
					result += Convert.ToString(dt.Rows[i][columnName]) + ", ";
				}
			}

			// Убираем последнюю запятую
			if (result != string.Empty) result = result.Remove(result.Length - 2);
			return result;
        }

        #endregion Функции преобразования структур данных (баз данных)

        #region Функции для работы с датами

        /// <summary>
        /// Массив месяцев
        /// </summary>
        public static string[] MonthByNumber = new string[12] {
            "Январь",
            "Февраль",
            "Март",
            "Апрель",
            "Май",
            "Июнь",
            "Июль",
            "Август",
            "Сентябрь",
            "Октябрь",
            "Ноябрь",
            "Декабрь" };

        /// <summary>
        /// Массив месяцев (в склонении)
        /// </summary>
        public static string[] DeclMonthByNumber = new string[12] {
            "Января",
            "Февраля",
            "Марта",
            "Апреля",
            "Мая",
            "Июня",
            "Июля",
            "Августа",
            "Сентября",
            "Октября",
            "Ноября",
            "Декабря" };

        /// <summary>
		/// Преобразует дату формата Д.ММ.ГГГГ, ДММГГГГ или ДММГГ в ГГГГММДД
		/// </summary>
		/// <param name="date">Исходная дата</param>
        /// <param name="century">Век, к которому принадлежит год - для формата ДММГГ</param>
		/// <returns>Новая дата</returns>
		public static int ShortDateToNewDate(string date, int century)
		{
            if (date.Trim() == string.Empty) return 0;

            string result = TrimLetters(
                date.Replace("-", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty));

            if (result.Length == 0)
            {
                return 0;
            }

            if (result.Length == 5 || result.Length == 7)
            {
                result = "0" + result;
            }
            else if (result.Length < 5)
            {
                return 0;
            }

            if (result.Length == 8)
            {
                return Convert.ToInt32(string.Format("{0}{1}{2}",
                    result.Substring(4, 4), result.Substring(2, 2), result.Substring(0, 2)));
            }
            else
            {
                return Convert.ToInt32(string.Format("{0}{1}{2}{3}",
                    century - 1, result.Substring(4, 2), result.Substring(2, 2), result.Substring(0, 2)));
            }
		}

        /// <summary>
		/// Преобразует дату формата Д.ММ.ГГГГ, ДММГГГГ или ДММГГ в ГГГГММДД
		/// </summary>
		/// <param name="date">Исходная дата</param>
		/// <returns>Новая дата</returns>
        public static int ShortDateToNewDate(string date)
        {
            if (date == string.Empty) return -1;

            int year = Convert.ToInt32(date.Substring(date.Length - 2));

            if (year > 90)
            {
                return ShortDateToNewDate(date, 20);
            }
            else
            {
                return ShortDateToNewDate(date, 21);
            }
        }

        /// <summary>
		/// Преобразует дату формата Д.ММ.ГГГГ (в зависимости от региональных настроек) или ДММГГГГ в ГГГГММДД
		/// </summary>
		/// <param name="date">Исходная дата</param>
		/// <returns>Новая дата</returns>
        public static string GlobalShortDateToNewDate(string date)
        {
            DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime dt = System.DateTime.ParseExact(
                date, dateTimeFormat.ShortDatePattern + " " + dateTimeFormat.LongTimePattern, null);

            return string.Format("{0}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
        }

        /// <summary>
        /// Преобразует дату в зависимости от региональных настроек в формат DD.MM.YYYY HH:MM:SS
        /// </summary>
        /// <param name="date">Исходная дата</param>
        /// <returns>Новая дата</returns>
        public static string GlobalDateToLongDate(string date)
        {
            DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime dt = System.DateTime.ParseExact(
                date, dateTimeFormat.ShortDatePattern + " " + dateTimeFormat.LongTimePattern, null);

            return string.Format(
                "{0:00}.{1:00}.{2} {3:00}:{4:00}:{5:00}", 
                dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
        }

        /// <summary>
        /// Преобразует дату формата ГГГГММДД в ДД.ММ.ГГГГ
        /// </summary>
        /// <param name="date">Исходная дата</param>
        /// <returns>Новая дата</returns>
        public static string NewDateToLongDate(string date)
        {
            if (date.Trim() == string.Empty)
            {
                return string.Empty;
            }

            return string.Format("{0}.{1}.{2}", date.Substring(6, 2), date.Substring(4, 2), date.Substring(0, 4));
        }

        /// <summary>
        /// Конвертирует дату типа "3 мая 2005 года" в YYYYMMDD
        /// </summary>
        /// <param name="date">Исходная дата</param>
        /// <returns>Новая дата</returns>
        public static string LongDateToNewDate(string date)
        {
            if (date.Trim() == string.Empty) return string.Empty;

            string result = TrimLetters(date.ToUpper());

            // Находим в строке даты месяц. Из его позиции вычисляем позицию дня и года
            int monthPos = -1;
            int monthIndex;
            for (monthIndex = 0; monthIndex < DeclMonthByNumber.GetLength(0); monthIndex++)
            {
                monthPos = result.IndexOf(DeclMonthByNumber[monthIndex].ToUpper());
                if (monthPos > 0) break;
            }
            if (monthPos <= 0) throw new ArgumentException(string.Format("Ошибка при преобразовании даты .", date));

            // Формуруем новую дату
            result = string.Format(
                "{0}{1}{2}", 
                result.Substring(monthPos + DeclMonthByNumber[monthIndex].Length).Trim(),
                Convert.ToString(monthIndex + 1).PadLeft(2, '0'),
                result.Substring(0, monthPos - 1).Trim().PadLeft(2, '0'));

            return result;
        }

        /// <summary>
        /// Разбивает дату формата YYYYMMDD на год, месяц, день
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="day">День</param>
        public static void DecodeNewDate(int date, out int year, out int month, out int day)
        {
            year = date / 10000;
            month = (date / 100) % 100;
            day = date % 100;
        }

        /// <summary>
        /// Разбивает дату формата DD.MM.YYYY на год, месяц, день
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="day">День</param>
        public static void DecodeShortDate(string date, out int year, out int month, out int day)
        {
            DecodeNewDate(ShortDateToNewDate(date), out year, out month, out day);
        }

        /// <summary>
        /// Собирает дату формата YYYYMMDD из года, месяца и дня
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="day">День</param>
        public static int EncodeNewDate(int year, int month, int day)
        {
            return Convert.ToInt32(
                string.Format("{0}{1}{2}", year, month.ToString().PadLeft(2, '0'), day.ToString().PadLeft(2, '0')));
        }

        /// <summary>
        /// Возвращает количество дней в месяце
        /// </summary>
        /// <param name="month">Месяц</param>
        /// <param name="year">Год</param>
        /// <returns>Количество дней</returns>
        public static int GetDaysInMonth(int month, int year)
        {
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
            {
                return 31;
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11)
            {
                return 30;
            }
            else if (month == 2)
            {
                if (year % 4 == 0)
                {
                    return 29;
                }
                else
                {
                    return 28;
                }
            }

            return -1;
        }

        /// <summary>
        /// Проверяет, чтобы в дате у месяцев было столько дней, сколько положено
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Дней сколько нужно или нет</returns>
        public static bool CheckDaysInMonth(int date)
        {
            int year = date / 10000;
            int month = (date % 10000) / 100;
            int day =  date % 100;

            if (day > GetDaysInMonth(month, year)) return false;

            return true;
        }

        /// <summary>
        /// Если дата = первое число (например, 20050101), то в качестве даты для данных 
        /// берется предыдущий месяц (20041200).
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Новая дата</returns>
        public static int DecrementDate(int date)
        {
            if (date % 100 == 1)
            {
                int year = date / 10000;
                int month = (date / 100) % 100;
                month--;
                if (month == 0)
                {
                    month = 12;
                    year--;
                }

                return year * 10000 + month * 100;
            }
            else
            {
                return (date / 100) * 100;
            }
        }

        /// <summary>
        /// Если дата = первое число (например, 20050101), то в качестве даты для данных 
        /// берется предыдущий месяц (20041200).
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Новая дата</returns>
        public static int DecrementDate(string date)
        {
            return DecrementDate(Convert.ToInt32(date));
        }

        /// <summary>
        /// получить дату с последним числом предыдущего месяца 
        /// </summary>
        public static int DecrementDateWithLastDay(int date)
        {
            int year = date / 10000;
            int month = (date / 100) % 100;
            month--;
            if (month == 0)
            {
                month = 12;
                year--;
            }
            return year * 10000 + month * 100 + GetDaysInMonth(month, year);
        }


        #endregion Функции для работы с датами

        #region Функции для работы с массивами

        /// <summary>
        /// Меняет размерность массива
        /// </summary>
        /// <param name="origArray">Исходный массив</param>
        /// <param name="newSize">Новый размер (количество элементов)</param>
        /// <returns>Измененный массив</returns>
        public static Array RedimArray(Array origArray, int newSize)
        {
            // Опеределяем тип элементов массива
            Type t = origArray.GetType().GetElementType();

            // Создаем новый массив с требуемым количеством элементов
            Array newArray = Array.CreateInstance(t, newSize);

            // Копируем элементы из исходного массива
            Array.Copy(origArray, 0, newArray, 0, Math.Min(origArray.Length, newSize));

            return newArray;
        }

        /// <summary>
        /// Объединяет несколько массивов в один
        /// </summary>
        /// <param name="arrays">Список массивов</param>
        /// <returns>Массив</returns>
        public static Array ConcatArrays(params Array[] arrays)
        {
            if (arrays == null || arrays.GetLength(0) == 0)
            {
                return null;
            }

            Array result = arrays[0];
            int count = arrays.GetLength(0);
            for (int i = 1; i < count; i++)
            {
                if (arrays[i] != null)
                {
                    if (result == null)
                    {
                        result = (Array)new object[0];
                    }

                    result = RedimArray(result, result.GetLength(0) + arrays[i].GetLength(0));

                    Array.Copy(arrays[i], 0, result, result.GetLength(0) - arrays[i].GetLength(0),
                        arrays[i].GetLength(0));
                }
            }

            return result;
        }

        /// <summary>
        /// Преобразует массив строк в список чисел
        /// </summary>
        /// <param name="array">Массив строк</param>
        /// <returns>Список чисел</returns>
        public static List<int> StringArrayToIntList(string[] array)
        {
            List<int> list = new List<int>(array.GetLength(0));

            int count = array.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                list.Add(Convert.ToInt32(array[i]));
            }

            return list;
        }

        /// <summary>
        /// Преобразует массив строк в список чисел
        /// </summary>
        /// <param name="array">Массив строк</param>
        /// <returns>Список чисел</returns>
        public static List<string> StringArrayToStringList(string[] array)
        {
            List<string> list = new List<string>(array.GetLength(0));

            int count = array.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                list.Add(array[i]);
            }

            return list;
        }

        /// <summary>
        /// Преобразует массив символов в массив байтов
        /// </summary>
        /// <param name="charArray">Массив символов</param>
        /// <returns>Массив байтов</returns>
        public static byte[] CharArrayToByteArray(char[] charArray)
        {
            byte[] byteArray = new byte[charArray.GetLength(0)];

            int count = charArray.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                byteArray[i] = Convert.ToByte(charArray[i]);
            }

            return byteArray;
        }

        /// <summary>
        /// Преобразует массив байтов в строку
        /// </summary>
        /// <param name="byteArray">Массив байтов</param>
        /// <returns>Строка</returns>
        public static string BytesArrayToString(byte[] byteArray)
        {
            string result = string.Empty;

            int count = byteArray.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result += Convert.ToString((char)byteArray[i]);
            }

            return result;
        }

        /// <summary>
        /// Преобразует массив символов в строку
        /// </summary>
        /// <param name="charArray">Массив символов</param>
        /// <returns>Строка</returns>
        public static string CharArrayToString(char[] charArray)
        {
            string result = string.Empty;

            int count = charArray.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result += Convert.ToString(charArray[i]);
            }

            return result;
        }

        /// <summary>
        /// Преобразует тип элементов массива
        /// </summary>
        /// <param name="sourceArray">Исходный массив</param>
        /// <param name="destArrayType">Требуемый тип элементов</param>
        /// <returns>Новый массив требуемого типа</returns>
        public static Array ConvertArrayType(Array sourceArray, Type destArrayType)
        {
            Array destArray = Array.CreateInstance(destArrayType, sourceArray.GetLength(0));
            Array.Copy(sourceArray, destArray, sourceArray.GetLength(0));

            return destArray;
        }

        /// <summary>
        /// Удаляет часть массива
        /// </summary>
        /// <param name="origArray">Массив</param>
        /// <param name="startIndex">Индекс элемента, от которого начинается удаление</param>
        /// <returns>Новый массив</returns>
        public static Array RemoveArrayPart(Array origArray, int startIndex)
        {
            return CommonRoutines.RemoveArrayPart(origArray, startIndex, -1);
        }

        /// <summary>
        /// Удаляет часть массива
        /// </summary>
        /// <param name="origArray">Массив</param>
        /// <param name="startIndex">Индекс элемента, от которого начинается удаление</param>
        /// <param name="length">Длина удаляемой части (-1 - удалять до конца)</param>
        /// <returns>Новый массив</returns>
        public static Array RemoveArrayPart(Array origArray, int startIndex, int length)
        {
            if (length == 0)
            {
                return origArray;
            }

            // Создаем новый массив такого же типа, что и исходный
            Array newArray = Array.CreateInstance(origArray.GetType().GetElementType(), origArray.GetLength(0) - length);
            
            // Копируем в него все элементы до и после удаляемой области
            if (length > 0 && length < origArray.GetLength(0))
            {
                Array.Copy(origArray, 0, newArray, 0, startIndex);
                Array.Copy(origArray, startIndex + length, newArray, startIndex, origArray.GetLength(0) - length - startIndex);
            }
            else
            {
                Array.Copy(origArray, startIndex, newArray, 0, origArray.GetLength(0) - startIndex);
            }

            return newArray;
        }

        /// <summary>
        /// Удаляет элементы массива, равные указанному
        /// </summary>
        /// <param name="origArray">Исходный массив</param>
        /// <param name="element">Значение элемента</param>
        /// <returns>Массив</returns>
        public static object[] RemoveArrayElements(object[] origArray, object element)
        {
            object[] result = origArray;

            for (int i = result.GetLength(0) - 1; i >= 0; i--)
            {
                if (result[i] == element) result = (object[])RemoveArrayPart(result, i, 1);
            }

            return result;
        }

        /// <summary>
        /// Сравнивает два массива
        /// </summary>
        /// <param name="sourceArray">Исходный массив</param>
        /// <param name="destArray">Второй массив :)</param>
        /// <param name="type">Тип элементов массивов</param>
        /// <returns>Равны или нет</returns>
        public static bool CompareArrays(int[] sourceArray, int[] destArray)
        {
            if (sourceArray == null || destArray == null)
            {
                return false;
            }

            if (sourceArray.GetLength(0) != destArray.GetLength(0))
            {
                return false;
            }

            int count = sourceArray.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (sourceArray[i] != destArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Возвращает индекс указанного значения в массиве, если его там нет - null
        /// </summary>
        /// <param name="value">Значение для поиска</param>
        /// <param name="valuesArr">Массив</param>
        public static int? GetValueEntryIndex(string value, string[] valuesArr)
        {
            int count = valuesArr.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (valuesArr[i].ToUpper() == value.ToUpper()) return i;
            }

            return null;
        }

        /// <summary>
        /// Сверяет, входит ли значение в массив допустимых значений
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="valuesArr">Массив допустимых значений</param>
        /// <returns>Входит или нет</returns>
        public static bool CheckValueEntry(int value, int[] valuesArr)
        {
            if (valuesArr == null) return false;

            int count = valuesArr.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (valuesArr[i] == value) return true;
            }

            return false;
        }

        /// <summary>
        /// Сверяет, входит ли значение в массив допустимых значений
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="valuesArr">Массив допустимых значений</param>
        /// <returns>Входит или нет</returns>
        public static bool CheckValueEntry(string value, string[] valuesArr)
        {
            if (valuesArr == null) return false;

            int count = valuesArr.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (String.Compare(valuesArr[i], value, true) == 0) return true;
            }

            return false;
        }

        /// <summary>
        /// Инициализирует все элементы массива указанным значением
        /// </summary>
        /// <param name="array">Массив</param>
        /// <param name="value">Значение</param>
        public static void InitArray(Array array, object value)
        {
            if (array == null) return;

            for (int i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
            {
                array.SetValue(value, i);
            }
        }

        #endregion Функции для работы с массивами

        #region Функции для работы со строками

        /// <summary>
        /// Удаляет буквы и знаки препинания в начале и конце строки
        /// </summary>
        /// <param name="str">Строка</param>
        #warning Переделать. Так программы писать нельзя
        public static string TrimLetters(string str)
        {
            string result = str;

            // Удаляем ненужные символы в начале строки
            while (result.Length > 0 && (result[0] < '0' || result[0] > '9'))
            {
                // Знак "-" перед числом нужно оставить, если он есть
                if (result[0] == '-' && result.Length > 1 && result[1] >= '0' && result[1] <= '9') break;

                result = result.Remove(0, 1);
            }

            // Удаляем ненужные символы в конце строки
            while (result.Length > 0 && (result[result.Length - 1] < '0' || result[result.Length - 1] > '9'))
            {
                result = result.Remove(result.Length - 1, 1);
            }

            return result;
        }

        /// <summary>
        /// Удаляет буквы и знаки препинания в строке
        /// </summary>
        /// <param name="str">Строка</param>
        #warning Переделать. Так программы писать нельзя
        public static string RemoveLetters(string str)
        {
            string result = str;

            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (result[i] < '0' || result[i] > '9')
                {
                    result = result.Remove(i, 1);
                }
            }

            return result;
        }

        /// <summary>
        /// Удаляет все вхождения указанных подстрок в строке
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="subStr">Подстроки</param>
        public static string RemoveSubStr(string str, string subStr)
        {
            return ReplaceSubStr(str, subStr, string.Empty);
        }

        /// <summary>
        /// Заменяет все вхождения указанных подстрок в строке
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="subStr">Подстроки</param>
        #warning Переделать. Так программы писать нельзя
        public static string ReplaceSubStr(string str, string subStr, string newSubStr)
        {
            string result = str;

            while (result.Contains(subStr))
            {
                result = result.Replace(subStr, newSubStr);
            }

            return result;
        }

        /// <summary>
        /// Удаляет цифры в начале и конце строки
        /// </summary>
        /// <param name="str">Строка</param>
        #warning Переделать. Так программы писать нельзя
        public static string TrimNumbers(string str)
        {
            string result = str;

            // Удаляем ненужные символы в начале строки
            while (result.Length > 0 && (result[0] >= '0' && result[0] <= '9'))
            {
                result = result.Remove(0, 1);
            }
            
            // Удаляем ненужные символы в конце строки
            while (result.Length > 0 && (result[result.Length - 1] >= '0' && result[result.Length - 1] <= '9'))
            {
                result = result.Remove(result.Length - 1, 1);
            }
            
            return result;
        }

        /// <summary>
        /// Удаляет цифры в строке
        /// </summary>
        /// <param name="str">Строка</param>
        #warning Переделать. Так программы писать нельзя
        public static string RemoveNumbers(string str)
        {
            string result = str;

            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (result[i] >= '0' && result[i] <= '9')
                {
                    result = result.Remove(i, 1);
                }
            }

            return result;
        }

        /// <summary>
        /// Преобразует строку чисел заданного формата в массив
        /// </summary>
        /// <param name="paramsString">Строка. Формат:
        /// "num" - число;
        /// "num1;num2;..." - 2 числа: num1, num2;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2)</param>
        /// <returns></returns>
        public static int[] ParseParamsString(string paramsString)
        {
            if (paramsString == string.Empty) return new int[0];

            int[] result = new int[0];

            string[] parts = paramsString.Split(';');

            int count = parts.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string[] intervals = parts[i].Split(new string[] { ".." }, StringSplitOptions.None);

                if (intervals.GetLength(0) == 0)
                {
                    continue;
                }
                else if (intervals.GetLength(0) == 1)
                {
                    Array.Resize(ref result, result.GetLength(0) + 1);
                    result[result.GetLength(0) - 1] = Convert.ToInt32(intervals[0]);
                }
                else
                {
                    int lo = Convert.ToInt32(intervals[0]);
                    int hi =  Convert.ToInt32(intervals[1]);

                    for (int j = lo; j <= hi; j++)
                    {
                        Array.Resize(ref result, result.GetLength(0) + 1);
                        result[result.GetLength(0) - 1] = j;
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// Преобразует строку чисел заданного формата в массив
        /// </summary>
        /// <param name="paramsString">Строка. Формат:
        /// "num" - число;
        /// "num1;num2;..." - 2 числа: num1, num2;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2)</param>
        /// <returns></returns>
        public static List<int> ParseParamsStringToList(string paramsString)
        {
            int[] num = ParseParamsString(paramsString);
            List<int> result = new List<int>(num.GetLength(0));

            int count = num.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result.Add(num[i]);
            }

            return result;
        }

        #endregion Функции для работы со строками

        #region Функции для работы с числами

        /// <summary>
        /// Приводит дробное число к к формату текущих региональных настроек
        /// </summary>
        /// <param name="value">Исходное число</param>
        /// <returns>Число текущего формата</returns>
        public static double ReduceDouble(string value)
        {
            return Convert.ToDouble(value.Trim().Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator).Replace(
                ".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(" ", string.Empty).PadLeft(1, '0'));
        }

        public static decimal ReduceDecimal(string value)
        {
            return Convert.ToDecimal(value.Trim().Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator).Replace(
                ".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(" ", string.Empty).PadLeft(1, '0'));
        }

        /// <summary>
        /// Приводит целое число к к формату текущих региональных настроек
        /// </summary>
        /// <param name="value">Исходное число</param>
        /// <returns>Число текущего формата</returns>
        public static int ReduceInt(string value)
        {
            return Convert.ToInt32(value.Trim().Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator).Replace(
                " ", string.Empty).PadLeft(1, '0'));
        }
        
        /// <summary>
        /// Меняет местами значения двух переменных
        /// </summary>
        /// <typeparam name="MyType">Какой-то тип</typeparam>
        /// <param name="myA">Переменная А</param>
        /// <param name="myB">Переменная B</param>
        public static void SwapValues<MyType>(ref MyType myA, ref MyType myB)
        {
            /*value1 ^= value2 ^= value1;
            value2^=value1;*/

            MyType temp = myB;
            myB = myA;
            myA = temp;
        }

        #endregion Функции для работы с числами

        #region Функции для работы с архивными файлами

        /// <summary>
		/// Исполняет файл программы с заданными аргументами в сеансе ДОС
		/// </summary>
		/// <param name="settingsFileName">Файл</param>
		/// <param name="arguments">Аргументы</param>
		/// <param name="output">Это то, что сказал ДОС в ответ на команду</param>
		/// <returns>Строка ошибки</returns>
		public static string ExecuteDOSCommand(string fileName, string arguments, out string output)
		{
			Process prc = null;
			output = string.Empty;

			try
			{
				// Устанавливаем параметры запуска процесса
				prc = new Process();
				prc.StartInfo.FileName = fileName;
				prc.StartInfo.Arguments = arguments;
				prc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				prc.StartInfo.UseShellExecute = false;
				prc.StartInfo.CreateNoWindow = true;
				prc.StartInfo.RedirectStandardOutput = false;

				// Старт
				prc.Start();

				// Ждем пока процесс не завершится
				prc.WaitForExit();
				//output = prc.StandardOutput.ReadToEnd();

				return string.Empty;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			finally
			{
				if (prc != null) prc.Close();
			}
		}

        #region Добавление в архив

        /// <summary>
        /// Добавить файл в Arj-архив
        /// </summary>
        /// <param name="archive">Имя файла архива</param>
        /// <param name="files">Массив имен файлов, которые следует добавить в архив</param>
        /// <param name="output">Это то, что сказал ДОС в ответ на команду</param>
        /// <returns>Строка ошибки</returns>
        public static string AddToArjArchive(string archive, string[] files, out string output)
        {
            output = string.Empty;
            string result = string.Empty;
            // Путь, где сейчас находится архиватор
            string arjCurrentPath = GetCurrentDir().FullName + "\\ARJ32.EXE";
            try
            {
                // Проверяем, на месте ли архиватор
                if (!File.Exists(arjCurrentPath))
                {
                    return "Файл ARJ32.EXE не найден.";
                }

                // Создаем каталог для разорхивации
                string filenames = String.Join("\", \"", files);

                // Формируем строку параметров для архиватора
                string arcParams = string.Format("a -e \"{0}\" \"{1}\"", archive, filenames);

                // Вызываем архиватор
                result = ExecuteDOSCommand(arjCurrentPath, arcParams, out output);

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
            }

        }

        /// <summary>
        /// Добавить файл в архив
        /// </summary>
        /// <param name="archive">Имя файла архива</param>
        /// <param name="files">Массив имен файлов, которые следует добавить в архив</param>
        /// <param name="archivatorName">Наименование архиватора</param>
        public static void AddToArchiveFile(string archive, string[] files, ArchivatorName archivatorName)
        {
            string str = string.Empty;
            string err = string.Empty;

            if (archivatorName == ArchivatorName.Arj)
            {
                err = CommonRoutines.AddToArjArchive(archive, files, out str);
            }

            if (err != string.Empty)
            {
                throw new Exception(
                    string.Format("Ошибка при добавлении файла в архив {0}: {1}.", archive, err));
            }
        }

        #endregion Добавление в архив

        #region Распаковка архивов

        /// <summary>
		/// Распаковывает файл ARJ
		/// </summary>
		/// <param name="file">Файл архива</param>
		/// <param name="outDir">Каталог куда распаковывать (пустая строка - текущий каталог)</param>
		/// <param name="output">Это то, что сказал ДОС в ответ на команду</param>
		/// <returns>Строка ошибки</returns>
        public static string ExtractARJ(string file, string outDir, out string output)
		{
			output = string.Empty;
			string result = string.Empty;

			// Путь, где сейчас находится архиватор
			string arjCurrentPath = GetCurrentDir().FullName + "\\ARJ32.EXE";

			try
			{
				// Проверяем, на месте ли архиватор
                if (!File.Exists(arjCurrentPath))
                {
                    return "Файл ARJ32.EXE не найден.";
                }

				// Создаем каталог для разархивации
                if (outDir != string.Empty)
                {
                    Directory.CreateDirectory(outDir);
                }

				// Формируем строку параметров для архиватора
				string arcParams = string.Format("x \"{0}\" \"{1}\" -u -v -y", file, outDir);

				// Вызываем архиватор
				result = ExecuteDOSCommand(arjCurrentPath, arcParams, out output);
				
				return result;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			finally
			{
			}
		}

        public static string ExtractRar(string file, string outDir, out string output, string fileMask)
        {
            output = string.Empty;
            string result = string.Empty;
            // Путь, где сейчас находится архиватор
            string rarCurrentPath = GetCurrentDir().FullName + "\\UnRar.EXE";
            try
            {
                // Проверяем, на месте ли архиватор
                if (!File.Exists(rarCurrentPath))
                {
                    return "Файл UnRar.EXE не найден.";
                }
                // Создаем каталог для разархивации
                if (outDir != string.Empty)
                {
                    Directory.CreateDirectory(outDir);
                }
                // Формируем строку параметров для архиватора
                string arcParams = string.Format("x \"{0}\" {1} \"{2}\"", file, fileMask, outDir);
                // Вызываем архиватор
                result = ExecuteDOSCommand(rarCurrentPath, arcParams, out output);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
            }
        }

        public static string ExtractRar(string file, string outDir, out string output)
        {
            return ExtractRar(file, outDir, out output, string.Empty);
        }

        /// <summary>
        /// Распаковывает файл ZIP
        /// </summary>
        /// <param name="file">Файл архива</param>
        /// <param name="outDir">Каталог куда распаковывать (пустая строка - текущий каталог)</param>
        /// <param name="output">Это то, что сказал ДОС в ответ на команду</param>
        /// <returns>Строка ошибки</returns>
        public static string ExtractZIP(string file, string outDir, out string output)
        {
            output = string.Empty;
            string result = string.Empty;

            try
            {
                // Создаем каталог для разархивации
                if (outDir != string.Empty)
                {
                    Directory.CreateDirectory(outDir);
                }

                // Вызываем архиватор
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(file, outDir, string.Empty);

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Распаковывает все архивные файлы во временный каталог
        /// </summary>
        /// <param name="sourceDir">Каталог с файлами</param>
        /// <param name="archivatorName">Наименование архиватора</param>
        /// <param name="filesExtractingOption">Настройки процесса распаковки файлов</param>
        /// <returns>Временный каталог</returns>
        public static string ExtractArchiveFilesToTempDir(string sourceDir, ArchivatorName archivatorName,
            FilesExtractingOption filesExtractingOption)
        {
            DirectoryInfo tmpDir = GetTempDir();

            ExtractArchiveFiles(sourceDir, tmpDir.FullName, archivatorName, filesExtractingOption);

            return tmpDir.FullName;
        }

        /// <summary>
        /// Распаковывает все архивные файлы
        /// </summary>
        /// <param name="sourceDir">Каталог с файлами</param>
        /// <param name="destDir">Каталог куда распаковывать (пустая строка - текеущий каталог)</param>
        /// <param name="archivatorName">Наименование архиватора</param>
        /// <param name="filesExtractingOption">Настройки процесса распаковки файлов</param>
        /// <returns>Временный каталог</returns>
        public static void ExtractArchiveFiles(string sourceDir, string destDir, ArchivatorName archivatorName,
            FilesExtractingOption filesExtractingOption)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            FileInfo[] files = new FileInfo[0];
            switch (archivatorName)
            {
                case ArchivatorName.Arj: files = dir.GetFiles("*.arj", SearchOption.AllDirectories);
                    break;
                case ArchivatorName.Rar: files = dir.GetFiles("*.rar", SearchOption.AllDirectories);
                    break;
                case ArchivatorName.Zip: files = dir.GetFiles("*.zip", SearchOption.AllDirectories);
                    break;
            }

            for (int i = 0; i < files.GetLength(0); i++)
            {
                ExtractArchiveFile(files[i].FullName, destDir, archivatorName, filesExtractingOption);
            }
        }

        /// <summary>
        /// Распаковывает все архивные файлы в текущий каталог
        /// </summary>
        /// <param name="sourceDir">Каталог с файлами</param>
        /// <param name="archivatorName">Наименование архиватора</param>
        /// <param name="filesExtractingOption">Настройки процесса распаковки файлов</param>
        /// <returns>Временный каталог</returns>
        public static void ExtractArchiveFiles(string sourceDir, ArchivatorName archivatorName,
            FilesExtractingOption filesExtractingOption)
        {
            ExtractArchiveFiles(sourceDir, sourceDir, archivatorName, filesExtractingOption);
        }

        /// <summary>
        /// Распаковывает архивный файл во временный каталог
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="archivatorName">Наименование архиватора</param>
        /// <param name="filesExtractingOption">Настройки процесса распаковки файлов</param>
        /// <returns>Временный каталог</returns>
        public static string ExtractArchiveFileToTempDir(string file, ArchivatorName archivatorName,
            FilesExtractingOption filesExtractingOption)
        {
            DirectoryInfo tmpDir = GetTempDir();

            ExtractArchiveFile(file, tmpDir.FullName, archivatorName, filesExtractingOption);

            return tmpDir.FullName;
        }

        public static DirectoryInfo ExtractArchiveFileToTempDir(string file, FilesExtractingOption filesExtractingOption,
            ArchivatorName archivatorName)
        {
            string tempDirPath = CommonRoutines.ExtractArchiveFileToTempDir(file, archivatorName, filesExtractingOption);
            return new DirectoryInfo(tempDirPath);
        }

        /// <summary>
        /// Распаковывает архивный файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="destDir">Каталог куда распаковывать (пустая строка - текеущий каталог)</param>
        /// <param name="archivatorName">Наименование архиватора</param>
        /// <param name="filesExtractingOption">Настройки процесса распаковки файлов</param>
        /// <returns>Временный каталог</returns>
        public static string ExtractArchiveFile(string file, string destDir, ArchivatorName archivatorName,
            FilesExtractingOption filesExtractingOption)
        {
            string str = string.Empty;
            string err = string.Empty;

            DirectoryInfo destPath = new DirectoryInfo(destDir);
            FileInfo fileInfo = new FileInfo(file);

            switch (filesExtractingOption)
            {
                case FilesExtractingOption.SeparateSubDirs:
                    destDir = destPath.CreateSubdirectory(ExtractingDirectoryPrefix + fileInfo.Name.Replace(".", string.Empty)).FullName;
                    break;

                case FilesExtractingOption.SingleDirectory:
                    break;
            }

            switch (archivatorName)
            {
                case ArchivatorName.Arj: err = CommonRoutines.ExtractARJ(file, destDir, out str);
                    break;
                case ArchivatorName.Rar: err = CommonRoutines.ExtractRar(file, destDir, out str);
                    break;
                case ArchivatorName.Zip: err = CommonRoutines.ExtractZIP(file, destDir, out str);
                    break;
            }

            if (err != string.Empty)
            {
                throw new Exception(
                    string.Format("Ошибка при разархивировании файла {0}: {1}.", file, err));
            }

            return destDir;
        }

        #endregion Распаковка архивов

        #endregion Функции для работы с архивными файлами

        #region Функции для работы с файлами и каталогами

        /// <summary>
        /// Возвращает массив строк файла
        /// </summary>
        /// <param name="file">Файл</param>
        public static string[] GetFileContent(FileInfo file, Encoding encoding)
        {
            FileStream fs = null;
            StreamReader sr = null;
            string[] fileContent;

            try
            {
                // Чтение данных из файла
                fs = file.OpenRead();
                // Вызываем объект управления временeм жизни файлстрима
                ILease lease = (ILease)fs.InitializeLifetimeService();
                if (lease.CurrentState == LeaseState.Initial)
                {
                    // Он будет жить вечно!
                    lease.InitialLeaseTime = TimeSpan.Zero;
                }
                sr = new StreamReader(fs, encoding);

                fileContent = sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                return fileContent;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Удаляет подкаталоги в заданном каталоге
        /// </summary>
        /// <param name="rootDir">Каталог, подкаталоги которого бущут удаляться</param>
        /// <param name="searchPattern">Маска</param>
        /// <returns>Сообщение об ошибке</returns>
        public static string DeleteDirectories(DirectoryInfo rootDir, string searchPattern)
        {
            if (rootDir == null || !rootDir.Exists) return string.Empty;

            DirectoryInfo[] dirs = rootDir.GetDirectories(searchPattern, SearchOption.AllDirectories);

            try
            {
                for (int i = 0; i < dirs.GetLength(0); i++)
                {
                    dirs[i].Delete(true);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        /// <summary>
        /// Удаляет подкаталоги с разорхивированными файлами в заданном каталоге
        /// </summary>
        /// <param name="rootDir">Каталог, подкаталоги которого бущут удаляться</param>
        /// <returns>Сообщение об ошибке</returns>
        public static string DeleteExtractedDirectories(DirectoryInfo rootDir)
        {
            return DeleteDirectories(rootDir, ExtractingDirectoryPrefix + "*");
        }

        // удалить директорию и все ее содержимое
        public static void DeleteDirectory(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.AllDirectories))
                try
                {
                    file.Delete();
                }
                catch
                {
                }
            try
            {
                dir.Delete(true);
            }
            catch
            {
            }
        }

        #endregion Функции для работы с файлами и каталогами

        #region Функции работы со средой

        /// <summary>
        /// Текущий каталог
        /// </summary>
        /// <returns>Текущий каталог</returns>
        public static DirectoryInfo GetCurrentDir()
        {
            return new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
        }

        /// <summary>
        /// Пингает указанный хост
        /// </summary>
        /// <param name="hostName">Название самого хоста или путь, содержащий его имя</param>
        /// <returns>Пингается или нет</returns>
        public static bool PingHost(string hostName)
        {
            // Получаем имя хоста
            string host = hostName.TrimStart('\\');
            if (host.Trim() == string.Empty)
            {
                return false;
            }

            string[] strings = host.Split('\\');
            if (strings.GetLength(0) > 0)
            {
                Ping ping = new Ping();
                try
                {
                    PingReply reply = ping.Send(strings[0], 10);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                finally
                {
                    ping.Dispose();
                }
            }

            return false;
        }

        /// <summary>
        /// Создает временный подкаталог в указанном каталоге
        /// </summary>
        public static DirectoryInfo GetTempDir(DirectoryInfo dir)
        {
            string subDir = string.Empty;
            if (dir == null)
            {
                subDir = Environment.GetEnvironmentVariable("TEMP").TrimEnd('\\');
            }
            else
            {
                subDir = dir.FullName.TrimEnd('\\');
            }

            string path = subDir + "\\" + Guid.NewGuid().ToString();
            Directory.CreateDirectory(path);

            return new DirectoryInfo(path);
        }

        /// <summary>
        /// Создает каталог во временной папке винды
        /// </summary>
        public static DirectoryInfo GetTempDir()
        {
            return GetTempDir(null);
        }

        #endregion Функции работы со средой

        #region Функции для работы с системами счисления

        /// <summary>
        /// Заполняет кэш 36-ричной системы счисления
        /// </summary>
        private static void FillNumeration36()
        {
            if (numeration36.Count == 0)
            {
                numeration36.Add('0', 0);
                numeration36.Add('1', 1);
                numeration36.Add('2', 2);
                numeration36.Add('3', 3);
                numeration36.Add('4', 4);
                numeration36.Add('5', 5);
                numeration36.Add('6', 6);
                numeration36.Add('7', 7);
                numeration36.Add('8', 8);
                numeration36.Add('9', 9);
                numeration36.Add('A', 10);
                numeration36.Add('B', 11);
                numeration36.Add('C', 12);
                numeration36.Add('D', 13);
                numeration36.Add('E', 14);
                numeration36.Add('F', 15);
                numeration36.Add('G', 16);
                numeration36.Add('H', 17);
                numeration36.Add('I', 18);
                numeration36.Add('J', 19);
                numeration36.Add('K', 20);
                numeration36.Add('L', 21);
                numeration36.Add('M', 22);
                numeration36.Add('N', 23);
                numeration36.Add('O', 24);
                numeration36.Add('P', 25);
                numeration36.Add('Q', 26);
                numeration36.Add('R', 27);
                numeration36.Add('S', 28);
                numeration36.Add('T', 29);
                numeration36.Add('U', 30);
                numeration36.Add('V', 31);
                numeration36.Add('W', 32);
                numeration36.Add('X', 33);
                numeration36.Add('Y', 34);
                numeration36.Add('Z', 35);
            }
        }

        /// <summary>
        /// Преобразует значение 36-й системы счисления в 10-ю
        /// </summary>
        /// <param name="digit36">Значение 36-й системы счисления</param>
        /// <returns>Значение 10-й системы счисления</returns>
        public static int Numeration36To10(char digit36)
        {
            FillNumeration36();

            if (!numeration36.ContainsKey(digit36))
            {
                throw new Exception(string.Format(
                    "Ошибка при преобразовании 36-ричной системы счисления в десятичную - не найден символ {0}", digit36));
            }

            return numeration36[digit36];
        }

        #endregion Функции для работы с системами счисления

        #region функции работы с текстовыми файлами

        private const int DOS_CODE_PAGE = 866;
        public static int GetTxtDosCodePage()
        {
            return DOS_CODE_PAGE;
        }

        private const int WIN_CODE_PAGE = 1251;
        public static int GetTxtWinCodePage()
        {
            return WIN_CODE_PAGE;
        }

        // получить массив строк текстового файла
        public static string[] GetTxtReportData(FileInfo file, int codePage)
        {
            string[] reportData = new string[0];
            FileStream fs = file.OpenRead();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(fs, Encoding.GetEncoding(codePage));
                reportData = sr.ReadToEnd().Split(new string[] { "\r" }, StringSplitOptions.None);
                return reportData;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (fs != null)
                    fs.Close();
            }
        }

        #endregion функции работы с текстовыми файлами

    }

    #region Структуры, перечисления

    /// <summary>
    /// Наименование архиватора
    /// </summary>
    public enum ArchivatorName
    {
        Arj,
        Zip,
        Rar
    }


    /// <summary>
    /// Настройки процесса распаковки файлов
    /// </summary>
    public enum FilesExtractingOption
    {
        /// <summary>
        /// Распаковывать все архивы в один каталог
        /// </summary>
        SingleDirectory,

        /// <summary>
        /// Распаковывать каждый архив в отдельный подкаталог
        /// </summary>
        SeparateSubDirs
    }

    #endregion Структуры, перечисления
}