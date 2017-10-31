using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using Krista.FM.Domain.DataSetParser.Attributes;

namespace Krista.FM.Domain.DataSetParser
{
	public abstract class DomainObjectBase : INotifyPropertyChanged
	{
		internal abstract void Fill(DataRow row);

		protected static DomainObjectBase GetOne(DataRow dataRow, Type type)
		{
			DomainObjectBase bo = (DomainObjectBase)Activator.CreateInstance(type, true);
			bo.Fill(dataRow);
			return bo;
		}

		private Dictionary<PropertyInfo, DomainFieldAttribute> fields;
		internal Dictionary<PropertyInfo, DomainFieldAttribute> Fields
		{
			get
            {
                if (fields == null)
                {
                    fields = new Dictionary<PropertyInfo, DomainFieldAttribute>();

                	IEnumerable<PropertyInfo> properties = GetType().GetProperties(
						BindingFlags.Public | BindingFlags.Instance |
						BindingFlags.GetProperty | BindingFlags.SetProperty);

                	foreach (PropertyInfo propertyInfo in properties)
                	{
                		if (propertyInfo.GetCustomAttributes(typeof(DomainFieldAttribute), true).Length != 0)
                		{
							DomainFieldAttribute attribute = (DomainFieldAttribute)propertyInfo.GetCustomAttributes(typeof(DomainFieldAttribute), true)[0];
							fields.Add(propertyInfo, attribute);
                		}
                	}
                }
                return fields;
            }
        }

		#region To methods
		protected virtual string ToString(object value)
		{
			return ToStringNotNullable(value);
		}

		protected virtual string ToStringNotNullable(object value)
		{
			return Util.ToStringNotNullable(value);
		}

		protected virtual DateTime ToDateTime(object value)
		{
			return value == null || value == DBNull.Value ? new DateTime() : Convert.ToDateTime(value);
		}

		protected virtual DateTime? ToDateTimeNullable(object value)
		{
			return value == null || value == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(value);
		}

		protected virtual Int32 ToInt32(object value)
		{
			return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
		}

		protected virtual Int32? ToInt32Nullable(object value)
		{
			return value == null || value == DBNull.Value ? null : (Int32?)Convert.ToInt32(value);
		}

		protected virtual Int64 ToInt64(object value)
		{
			return value == null || value == DBNull.Value ? 0 : Convert.ToInt64(value);
		}

		protected virtual Int64? ToInt64Nullable(object value)
		{
			return value == null || value == DBNull.Value ? null : (Int64?)Convert.ToInt64(value);
		}

		protected virtual double ToDouble(object value)
		{
			return Util.ToDouble(value);
		}

		protected virtual double? ToDoubleNullable(object value)
		{
			return value == null ? null : (double?)Util.ToDouble(value);
		}

		#endregion

		#region Implementation of INotifyPropertyChanged

		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected virtual object GetConvertedObject(Type type, object o)
		{
			if (type == typeof(int)) return ToInt32(o);
			if (type == typeof(int?)) return ToInt32Nullable(o);
			if (type == typeof(string)) return ToString(o);
			if (type == typeof(Double)) return ToDouble(o);
			if (type == typeof(Double?)) return ToDoubleNullable(o);
			if (type == typeof(DateTime)) return ToDateTime(o);
			if (type == typeof(DateTime?)) return ToDateTimeNullable(o);
			if (type == typeof(long)) return ToInt64(o);
			if (type == typeof(long?)) return ToInt64Nullable(o);
			if (type == typeof(byte[])) return o.Equals(DBNull.Value) ? null : o;
			if (type.IsEnum) return Enum.Parse(type, o.ToString(), true);
			if (type == typeof(Boolean))
				return o.ToString().Trim() == "1"
						   ? true : o.ToString().Trim() == "0"
										? false : Util.ParseValue(type, o);
			if (type == typeof(Boolean?))
				return o == null 
					? null : o.ToString().Trim() == "1"
						   ? true : o.ToString().Trim() == "0"
										? false : Util.ParseValue(type, o);
			return Util.ParseValue(type, o);
		}
	}
}
