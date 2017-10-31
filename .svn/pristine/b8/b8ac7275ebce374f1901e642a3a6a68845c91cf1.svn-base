using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Ext.Net;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    public static class JsonUtils
    {
        public static JsonObject FromJsonRaw(string jsonEncodedData)
        {
            try
            {
                return JSON.Deserialize<JsonObject>(jsonEncodedData);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("JsonUtils::FromJsonRaw: Ошибка преобразования строки в JSON: " + e.Message, e);
            }
        }

        public static JsonObject[] FromJsonRaws(string jsonEncodedData)
        {
            try
            {
                return JSON.Deserialize<JsonObject[]>(jsonEncodedData);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("JsonUtils::FromJsonRaw: Ошибка преобразования строки в JSON: " + e.Message, e);
            }
        }

        public static TDomainObject ParseRepositoryId<TDomainObject>(ILinqRepository<TDomainObject> repository, JsonObject jsonSource, string jsonFieldName, string errMessage)
        {
            int id = GetFieldOrDefault(jsonSource, jsonFieldName, -1);
            TDomainObject item = repository.FindOne(id);
            CheckNull(item, errMessage);
            return item;
        }

        public static string GetFieldOrDefault(JsonObject json, string jsonField, string defaultValue)
        {
            try
            {
                return json[jsonField].ToString();
            }
            catch 
            {
                return defaultValue;
            }
        }

        public static string ValueOrDefault(string val, string defaultValue)
        {
            return (val == string.Empty) ? defaultValue : val;
        }

        public static int GetFieldOrDefault(JsonObject json, string jsonField, int defaultValue)
        {
            return json.Any(f => f.Key == jsonField) ? ValueOrDefault(json[jsonField].ToString(), defaultValue) : defaultValue;
        }

        public static int ValueOrDefault(string val, int defaultValue)
        {
            try
            {
                return (val == string.Empty) ? defaultValue : Convert.ToInt32(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static decimal GetFieldOrDefault(JsonObject json, string jsonField, decimal defaultValue)
        {
            return json.Any(f => f.Key == jsonField) ? ValueOrDefault(json[jsonField].ToString(), defaultValue) : defaultValue;
        }

        public static decimal ValueOrDefault(string val, decimal defaultValue)
        {
            try
            {
                return (val == string.Empty) ? defaultValue : Convert.ToDecimal(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static decimal? ValueOrDefault(string val, decimal? defaultValue)
        {
            try
            {
                if (val == null)
                {
                    return null;
                }

                return (val == string.Empty) ? defaultValue : Convert.ToDecimal(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool GetFieldOrDefault(JsonObject json, string jsonField, bool defaultValue)
        {
            return json.Any(f => f.Key == jsonField) ? ValueOrDefault(json[jsonField].ToString(), defaultValue) : defaultValue;
        }

        public static bool ValueOrDefault(string val, bool defaultValue)
        {
            try
            {
                return (val == string.Empty) ? defaultValue : Convert.ToBoolean(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime GetFieldOrDefault(JsonObject json, string jsonField, DateTime defaultValue)
        {
            return json.Any(f => f.Key == jsonField) ? ValueOrDefault(json[jsonField].ToString(), defaultValue) : defaultValue;
        }

        public static DateTime ValueOrDefault(string val, DateTime defaultValue)
        {
            try
            {
                return (val == string.Empty) ? defaultValue : Convert.ToDateTime(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime? GetFieldOrDefault(JsonObject json, string jsonField, DateTime? defaultValue)
        {
            return json.Any(f => f.Key == jsonField) ? ValueOrDefault(json[jsonField].ToString(), defaultValue) : defaultValue;
        }

        public static DateTime? ValueOrDefault(string val, DateTime? defaultValue)
        {
            try
            {
                return (val == string.Empty) ? defaultValue : Convert.ToDateTime(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void CheckNull(object val, string message)
        {
            if (val == null)
            {
                throw new NullReferenceException(message);
            }
        }
        
        /// <summary>
        /// проверяет поле(модели) на наличие значения
        /// </summary>
        /// <typeparam name="T">Класс модели от ViewModelBase</typeparam>
        /// <param name="jo">объект JsonObject</param>
        /// <param name="expr">Лябда с указанием поля модели</param>
        /// <returns>fals - если имеет значение, true - если пусто</returns>
        public static bool CheckNull<T>(this JsonObject jo, Expression<Func<T>> expr) 
        {
            return Convert.ToString(jo[UiBuilders.NameOf(expr)]).Trim().IsNullOrEmpty();
        }

        /// <summary>
        /// Возвращает значение(строковое) поля модели
        /// </summary>
        /// <typeparam name="T">Класс модели от ViewModelBase</typeparam>
        /// <param name="jo">объект JsonObject</param>
        /// <param name="expr">Лябда с указанием поля модели</param>
        /// <returns>значение в виде строки</returns>
        public static string GetValue<T>(this JsonObject jo, Expression<Func<T>> expr)
        {
            return Convert.ToString(jo[UiBuilders.NameOf(expr)]).Trim();
        }

        /// <summary>
        /// Возвращает значение(целого) поля модели
        /// </summary>
        /// <typeparam name="T">Класс модели от ViewModelBase</typeparam>
        /// <param name="jo">объект JsonObject</param>
        /// <param name="expr">Лябда с указанием поля модели</param>
        /// <param name="defaultValue">значение по умолчанию </param>
        /// <returns>значение поля целого типа</returns>
        public static int GetValueToIntOrDefault<T>(this JsonObject jo, Expression<Func<T>> expr, int defaultValue)
        {
           return ValueOrDefault(jo[UiBuilders.NameOf(expr)].ToString(), defaultValue);
        }

        public static decimal GetValueToDecemalOrDefault<T>(this JsonObject jo, Expression<Func<T>> expr, decimal defaultValue = 0)
        {
            return ValueOrDefault(jo[UiBuilders.NameOf(expr)].ToString(), defaultValue);
        }
    }
}