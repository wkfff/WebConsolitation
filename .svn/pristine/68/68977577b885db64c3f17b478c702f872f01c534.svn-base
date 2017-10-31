using System;
using System.Globalization;
using System.Reflection;

namespace Krista.FM.Domain.DataSetParser
{
    static class Util
    {
        public static bool IsValidDouble(string doubleValue)
        {
            double quantity;
            return TryConvertToDouble(doubleValue, out quantity);
        }

        #region ConvertDateStringToDateTime
        /// <summary>
        /// Преобразует строку DDMMYYYY в datetime.
        /// </summary>
        /// <param name="date">Строка даты для преобразования.</param>
        /// <param name="dateTime">Преобразованная дата.</param>
        /// <returns>True если успешно.</returns>
        public static bool ConvertDateStringToDateTime(string date, out DateTime dateTime)
        {
            bool result = true;

            try
            {
                if (date.Length == 6)
                {
                    dateTime = new DateTime(Convert.ToInt32(date.Substring(4, 2)) + 2000,
                                            Convert.ToInt32(date.Substring(2, 2)),
                                            Convert.ToInt32(date.Substring(0, 2)));
                }
                else
                {
                    dateTime = new DateTime();
                    result = false;
                }
            }
            catch
            {
                dateTime = new DateTime();
                result = false;
            }

            return result;
        }
        #endregion

        #region ConvertToDouble
        public static double ConvertToDouble(object obj)
        {
            return ConvertToDouble(obj.ToString());
        }

        /// <summary>
        /// Преобразует строку в double. 
        /// Использует , и . как разделитель.
        /// </summary>
        public static double ConvertToDouble(string doubleValue)
        {
            try
            {
                return Convert.ToDouble(doubleValue);
            }
            catch
            {
                double result;
                if (TryConvertWithChangedSeparator(doubleValue, out result))
                    return result;
            }
            return 0.00;
        }
        #endregion

        #region ConvertToShort
        public static short ConvertToShort(string value)
        {
            short retVal = 0;
            try
            {
                retVal = Convert.ToInt16(value);
            }
            catch { }
            return retVal;
        }

        #endregion

        #region TryConvertToShort
        public static bool TryConvertToShort(string value, out short retVal)
        {
            retVal = 0;
            try
            {
                retVal = Convert.ToInt16(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Compare strings
        public static bool CompareStrings(object a, object b)
        {
            string str_a = ToString(a);
            string str_b = ToString(b);
            return CompareStrings(str_a, str_b);
        }

        public static bool CompareStrings(string a, string b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            if (string.Compare(a.Trim(), b.Trim(), true) == 0)
                return true;

            return false;
        } 
        #endregion
        
        /// <summary>
        /// Определяет, что данная строка null или empty.
        /// Так же выполняется .Trim для non null строк перед сравнением.
        /// </summary>
        public static bool IsNullOrEmptyTrim(string str)
        {
            bool result = true;
            if(str != null)
            {
                str = str.Trim();
                result = string.IsNullOrEmpty(str);
            }
            return result;
        }

        #region To int32
        public static Int32 ToInt32(object o)
        {
            Int32 result;

            TryConvertToInt32(o, out result);

            return result;
        } 
        #endregion

        #region ToInt64
        public static Int64 ToInt64(object o)
        {
            Int64 result;

            TryConvertToInt64(o, out result);

            return result;
        } 
        #endregion

        #region TryConvertToInt32
        public static bool TryConvertToInt32(object o, out int result)
        {
            bool status = true;

            try
            {
                result = Convert.ToInt32(o);
            }
            catch
            {
                result = 0;
                status = false;
            }

            return status;
        } 
        #endregion

        #region TryConvertToInt64
        public static bool TryConvertToInt64(object o, out long result)
        {
            bool status = true;

            try
            {
                result = Convert.ToInt64(o);
            }
            catch
            {
                result = 0;
                status = false;
            }

            return status;
        }
        #endregion

        private const string DOT_SEPARATOR = ".";
        private const string COMMA_SEPARATOR = ",";

        #region To double
        public static bool TryConvertToDouble(object o, out double result)
        {

            try
            {
                result = Convert.ToDouble(o);
                return true;
            }
            catch
            {
                return TryConvertWithChangedSeparator(o, out result);
            }
        }

        private static bool TryConvertWithChangedSeparator(object o, out double result)
        {
            bool success = false;
            string value = o.ToString();
            result = 0;
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            switch (separator)
            {
                case DOT_SEPARATOR:
                    {
                        value = value.Replace(COMMA_SEPARATOR, DOT_SEPARATOR);
                        try
                        {
                            result = Convert.ToDouble(value);
                            success = true;
                        }
                        catch
                        { }
                    }
                    break;
                case COMMA_SEPARATOR:
                    {
                        value = value.Replace(DOT_SEPARATOR, COMMA_SEPARATOR);
                        try
                        {
                            result = Convert.ToDouble(value);
                            success = true;
                        }
                        catch
                        { }
                    }
                    break;
            }
            return success;
        } 

        public static double ToDouble(object o)
        {
            double d;

            TryConvertToDouble(o, out d);

            return d;
        }
        #endregion

        public static string ToString(object o)
        {
            return o == null || o == DBNull.Value ? null : o.ToString().Trim();
        }

        public static string ToStringNotNullable(object value)
        {
            return value == null || value == DBNull.Value ? string.Empty : ToString(value);
        }

        public static string ConvertVerbatimString(string s)
        {
            return s.Replace("\\n", "\n");
        }

        const string dateTimeFormatShort = "ddMMyy";
        const string dateTimeFormatLong = "ddMMyyyy";

        public static DateTime ToDateTime(string s)
        {
            DateTime dateTime = DateTime.MinValue;

            try
            {
                dateTime = DateTime.ParseExact(s, dateTimeFormatLong, CultureInfo.InvariantCulture.DateTimeFormat);
            }
            catch
            {
                try
                {
                    dateTime = DateTime.ParseExact(s, dateTimeFormatShort, CultureInfo.InvariantCulture.DateTimeFormat);
                }
                catch { }
            }

            return dateTime;
        }

        public static string DateTimeToString(DateTime date)
        {
            return date.ToString(dateTimeFormatLong);
        }

        public static DateTime DateTimeStartDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        public static DateTime DateTimeEndDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        #region Parse via reflection
        /// <summary>
        /// Получение значения из строки в зависимости от типа значения.
        /// (Для получения значения вызывается метод '.Parse'.)
        /// Зависим от текущей культуры.
        /// </summary>
        public static object ParseValue(Type valueType, object value)
        {
            if (value == null || value == DBNull.Value) 
            {
				if (valueType.Name == "Nullable`1") return null;
                if (valueType.IsValueType) value = string.Empty;
                else return null;
            }

            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (valueType == typeof(string))
                return value;

            BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod
                              | BindingFlags.IgnoreCase | BindingFlags.Static;

			if (valueType.IsGenericType)
			{
				valueType = valueType.GetGenericArguments()[0];
			}

        	return valueType.InvokeMember("Parse"
                                          , bf, null, null, new object[] { value.ToString() });
        }

		/// <summary>
		/// Пытается получить значение из строки в зависимости от типа значения.
		/// (Для получения значения вызывается метод '.Parse'.)
		/// Зависим от текущей культуры.
		/// </summary>
        public static bool TryParseValue(Type valueType, string value, out object result)
        {
            try
            {
                result = ParseValue(valueType, value);
            }
            catch
            {
                result = null;
            }

            return result != null;
        }

        #endregion

    }
}