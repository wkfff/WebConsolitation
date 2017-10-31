using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Krista.FM.Domain.Reporitory
{
    public static class DataTableHelper
    {
        /// <summary>
        /// Создает DataTable из списка переданных объектов.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="data">Список объектов.</param>
        public static DataTable CreateDataTable<T>(IList<T> data)
        {
            DataTable table = new DataTable(typeof(T).Name);

            Dictionary<DataColumn, PropertyInfo> columnsInfo = new Dictionary<DataColumn, PropertyInfo>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                Type propertyType = propertyInfo.PropertyType;
                if (propertyType.IsGenericType)
                {
                    propertyType = propertyType.GetGenericArguments()[0];
                }

                columnsInfo.Add(table.Columns.Add(propertyInfo.Name, propertyType), propertyInfo);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (KeyValuePair<DataColumn, PropertyInfo> pair in columnsInfo)
                {
                    object value = pair.Value.GetValue(item, null);
                    row[pair.Key] = value ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Добавляет в DataTable строку со значениями объекта data.
        /// </summary>
        public static void AddRow<T>(this DataTable table, T data, object newId)
        {
            DataRow row = table.NewRow();
            row.UpdateRow(data);
            row["ID"] = newId;
            table.Rows.Add(row);
        }

        /// <summary>
        /// Обновляет строку значениями объекта data.
        /// </summary>
        public static void UpdateRow<T>(this DataRow row, T data)
        {
            foreach (PropertyInfo propertyInfo in data.GetType().GetProperties())
            {
                object value = propertyInfo.GetValue(data, null);
                row[propertyInfo.Name] = value ?? DBNull.Value;
            }
        }
    }
}
