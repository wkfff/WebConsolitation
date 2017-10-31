using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Krista.FM.Extensions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Устанавливает имя таблицы.
        /// </summary>
        /// <param name="table">Таблица для которой будет установнено имя.</param>
        /// <param name="name">Имя таблицы.</param>
        public static DataTable SetName(this DataTable table, string name)
        {
            table.TableName = name;
            return table;
        }

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

        /// <summary>
        /// Создает DataTable из списка ананимных объектов. 
        /// Список полей формируется из свойств первого объекта.
        /// </summary>
        /// <example>
        /// var table = new DataTable().FromAnonymous(
        ///     new List<object> { 
        ///         new { Col1 = "data1", Col2 = 10 },
        ///         new { Col1 = "data2", Col2 = 10 },
        ///         new { Col1 = "data3" }
        ///     });
        /// </example>
        /// <param name="table">Формируемая таблица.</param>
        /// <param name="data">Список ананимных объектов.</param>
        public static DataTable FromAnonymous(this DataTable table, IEnumerable<object> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            table.Clear();
            table.Columns.Clear();

            Dictionary<string, DataColumn> columnsInfo = null;
            foreach (object item in data)
            {
                if (columnsInfo == null)
                {
                    columnsInfo = new Dictionary<string, DataColumn>();
                    foreach (PropertyInfo propertyInfo in item.GetType().GetProperties())
                    {
                        Type propertyType = propertyInfo.PropertyType;
                        if (propertyType.IsGenericType)
                        {
                            propertyType = propertyType.GetGenericArguments()[0];
                        }

                        columnsInfo.Add(propertyInfo.Name, table.Columns.Add(propertyInfo.Name, propertyType));
                    }
                }

                DataRow row = table.NewRow();
                foreach (var property in item.GetType().GetProperties())
                {
                    if (columnsInfo.ContainsKey(property.Name))
                    {
                        object value = property.GetValue(item, null);
                        row[columnsInfo[property.Name]] = value ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
