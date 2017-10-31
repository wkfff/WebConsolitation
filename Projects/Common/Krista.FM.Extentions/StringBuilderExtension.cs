using System;
using System.Text;

namespace Krista.FM.Extensions
{
    public static class StringBuilderExtension
    {
        /// <summary>
        /// Удаляет последний символ
        /// </summary>
        public static StringBuilder RemoveLastChar(this StringBuilder stringBuilder)
        {
            if (stringBuilder.Length > 0)
            {
                return stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder;
        }

        /// <summary>
        /// Проверка на null или Empty.
        /// </summary>
        public static bool IsNullOrEmpty(this StringBuilder stringBuilder)
        {
            return String.IsNullOrEmpty(stringBuilder.ToString());
        }

        /// <summary>
        /// Проверка на null или Empty.
        /// </summary>
        public static bool IsNotNullOrEmpty(this StringBuilder stringBuilder)
        {
            return !String.IsNullOrEmpty(stringBuilder.ToString());
        }
    }
}
