using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Krista.FM.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Форматирование строки.
        /// </summary>
        public static string FormatWith(this string format, params  object[] args)
        {
            return String.Format(format, args);
        }

        /// <summary>
        /// Комбитирование нескольчих частей пути.
        /// </summary>
        public static string PathCombine(this string[] paths)
        {
            var result = string.Empty;
            foreach (var path in paths)
                result = Path.Combine(result, path);
            return result;
        }

        /// <summary>
        /// Объединяет две строки пути.
        /// </summary>
        public static string PathCombine(this string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        /// <summary>
        /// Объединяет строки пути.
        /// </summary>
        public static string PathCombine(this string path1, params string[] paths)
        {
            return paths.Aggregate(path1, Path.Combine);
        }

        /// <summary>
        /// Проверка на null или Empty.
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Проверка на null или Empty.
        /// </summary>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !String.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Преобразует строку в элемент перечисления.
        /// </summary>
        /// <typeparam name="T">Тип перечисления.</typeparam>
        /// <param name="instance">Преобразуемая строка.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        public static T ToEnum<T>(this string instance, T defaultValue) where T : IComparable, IFormattable
        {
            T convertedValue = defaultValue;

            if (!String.IsNullOrEmpty(instance))
            {
                try
                {
                    convertedValue = (T)Enum.Parse(typeof(T), instance.Trim(), true);
                }
                catch (ArgumentException)
                {
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// Перенос по строкам.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <param name="separator">Разделитель новой строки.</param>
        /// <param name="width">Ширина строки.</param>
        public static string WordWrap(this string str, string separator, int width)
        {
            var parts = str.Split(new char[] { ' ' });
            StringBuilder s = new StringBuilder(str.Length + 10);
            StringBuilder currentLine = new StringBuilder();
            foreach (string part in parts)
            {
                if (currentLine.Length + part.Length > width)
                {
                    s.Append(currentLine);
                    s.Append(separator);
                    currentLine = new StringBuilder(part);
                }
                else
                {
                    if (currentLine.Length > 0)
                    {
                        currentLine.Append(' ');
                    }
                    currentLine.Append(part);
                }
            }
            s.Append(currentLine);
            return s.ToString();
        }

        /// <summary>
        /// Перенос по строкам для всех элементов коллекции.
        /// </summary>
        public static IEnumerable<string> WordWrap(this IList<string> values, string separator, int width)
        {
            foreach (string value in values)
            {
                yield return value.WordWrap(separator, width);
            }
        }

        /// <summary>
        /// Преобразовывает первый символ в верхний регистр
        /// </summary>
        /// <param name="str"> Исходная строка</param>
        /// <returns> Преобразованная строка</returns>
        public static string UppercaseFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
