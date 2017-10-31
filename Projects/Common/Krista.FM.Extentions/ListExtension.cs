using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// Добавляет произвольное количество элементов в список.
        /// </summary>
        /// <example>
        /// var list = new List<int>();
        /// list.AddRange(1, 2, 3);
        /// </example>
        public static  void AddRange<T>(this IList<T>  list, params T[] objects)
        {
            foreach (T obj in  objects)
                list.Add(obj);
        }
    }
}
