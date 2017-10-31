using System.Collections.Generic;

namespace Krista.FM.Extensions
{
    public static class StringListExtensions
    {
        /// <summary>
        /// Добавляет строку к списку строк, если не выполняется условие test.
        /// </summary>
        /// <returns>Возвращает значение параметра test.</returns>
        public static bool Test(this List<string> list, bool test, string format, params object[] args)
        {
            if (!test)
            {
                list.Add(format.FormatWith(args));
            }

            return test;
        }
    }
}
