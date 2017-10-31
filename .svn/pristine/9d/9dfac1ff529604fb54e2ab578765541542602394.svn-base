using System;
using System.Data;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Выбирает указанную страницу данных.
        /// </summary>
        /// <param name="table">Целевой объект.</param>
        /// <param name="start">С какой записи начинать выборку страницы. 0 - с первой записи.</param>
        /// <param name="limit">Размер страницы.</param>
        public static DataTable SelectPage(this DataTable table, int start, int limit)
        {
            return table.SelectPage(start, limit, String.Empty, String.Empty);
        }

        /// <summary>
        /// Выбирает указанную страницу данных с учетом сортировки.
        /// </summary>
        /// <param name="table">Целевой объект.</param>
        /// <param name="start">С какой записи начинать выборку страницы. 0 - с первой записи.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="sort">Имя поля для сортировки.</param>
        /// <param name="dir">Направление сортировки (ASC/DESC).</param>
        public static DataTable SelectPage(this DataTable table, int start, int limit, string sort, string dir)
        {
            DataTable dest = table.Clone();

            int indx = 1;
            int count = 0;
            if (start < 0)
            {
                start = 0;
            }

            if (limit == -1)
            {
                limit = Int32.MaxValue;
            }

            DataRow[] rows = sort.IsNullOrEmpty()
                                 ? table.Select("1 = 1")
                                 : table.Select("1 = 1", "{0} {1}".FormatWith(sort, dir));
            foreach (var row in rows)
            {
                if (indx > start && count < limit)
                {
                    dest.Rows.Add(row.ItemArray);
                    count++;
                }

                if (count == limit)
                {
                    break;
                }

                indx++;
            }

            return dest;
        }
    }
}
