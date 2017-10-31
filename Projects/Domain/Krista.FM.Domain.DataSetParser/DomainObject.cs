using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Krista.FM.Domain.DataSetParser.Attributes;

namespace Krista.FM.Domain.DataSetParser
{
	public abstract class DomainObject<T> : DomainObjectBase where T : DomainObjectBase
	{
		static DomainObject()
		{
			childs = new Dictionary<Type, IEnumerable<string>>();
		}

		protected delegate DomainObject<T> Creator();
		internal override void Fill(DataRow row)
		{
			if (row.Table != null)
			{
				foreach (DataColumn column in row.Table.Columns)
				{
					SetValue(column.ColumnName, row[column]);
				}
				FillChilds(row);
			}
		}

		private static readonly Dictionary<Type, IEnumerable<string>> childs;
		protected virtual void FillChilds(DataRow row)
        {
            Type currentType = GetType();
            if (childs.ContainsKey(currentType))
            {
                FillChilds(childs[currentType],row);
            }
            else
            {
            	List<string> names = new List<string>();
            	foreach (PropertyInfo pi in GetType().GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance))
            	{
					if (pi.GetCustomAttributes(typeof (DomainChildAttribute), false).Length != 0)
					{
						names.Add(pi.Name);
					}
            	}

				childs.Add(currentType, names);
				FillChilds(names, row);
            }
        }

		private void FillChilds(IEnumerable<string> properties, DataRow dataRow)
		{
			Type currentType = GetType();
			foreach (string s in properties)
			{
				PropertyInfo property = currentType.GetProperty(s);
				property.SetValue(this, GetOne(dataRow, property.PropertyType), null);
			}
		}

		protected virtual void SetValue(string fieldName, object value)
		{
			this[fieldName] = value;
		}

		protected static List<T> Get(DataTable table, Creator creator)
		{
			List<T> arr = new List<T>();
			foreach (DataRow row in table.Rows)
			{
				DomainObject<T> bo = creator();
				bo.Fill(row);
				arr.Add(bo as T);
			}
			return arr;
		}

		protected static List<T> Get(DataSet dataSet, Creator creator)
		{
			if (dataSet == null)
				return null;

			List<T> arr = new List<T>();
			foreach (DataTable table in dataSet.Tables)
				arr.AddRange(Get(table, creator));
			return arr;
		}

		protected object this[string fieldName]
		{
			set
            {
				List<PropertyInfo> keys = new List<PropertyInfo>();
            	foreach (KeyValuePair<PropertyInfo, DomainFieldAttribute> field in Fields)
            	{
					if (Util.CompareStrings(fieldName, field.Value.Name))
					{
						keys.Add(field.Key);
					}
            	}

				PropertyInfo[] properties = keys.ToArray();
                if (properties.Length > 1) 
					throw new DomainObjectException(String.Format("Некорректное сопоставление для столбца {0}. Найдено несколько сопоставлений.", fieldName));
				if (properties.Length != 0)
                {
                    PropertyInfo prop = properties[0];
                    prop.SetValue(this, GetConvertedObject(prop.PropertyType, value), null);
                }
               
            }
        }

		public static List<T> Get(DataSet dataSet)
		{
			if (dataSet.Tables.Count > 0)
			{
				return Get(dataSet.Tables[0], Get);
			}
			return null;
		}

		public static List<T> Get(DataTable dataTable)
		{
			return Get(dataTable, Get);
		}


		public static T GetOne(DataSet dataSet)
		{
			if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return GetOne(dataSet.Tables[0].Rows[0]);
			}
			return null;
		}

		public static T GetOne(DataRow row)
		{
			DomainObject<T> bo = Get();
			bo.Fill(row);
			return bo as T;
		}

		private static DomainObject<T> Get()
		{
			return (DomainObject<T>)Activator.CreateInstance(typeof(T), true);
		}
	}
}
