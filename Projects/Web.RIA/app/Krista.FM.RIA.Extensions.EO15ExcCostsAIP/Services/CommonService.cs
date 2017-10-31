using System;
using System.Collections.Generic;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services
{
    public static class CommonService
    {
        public static int GetIntFromRecord(IDictionary<string, object> record, string paramName, string paramNameForException, bool throwException)
        {
            object typeId;
            if (!record.ContainsKey(paramName) && throwException)
            {
                throw new Exception("Параметр {0} отсутствует".FormatWith(paramNameForException));
            }

            record.TryGetValue(paramName, out typeId);
            int id;
            if (Int32.TryParse(typeId.ToString(), out id) && id > 0)
            {
                return id;
            }

            if (throwException)
            {
                throw new Exception("Параметр {0} имеет неверный тип".FormatWith(paramNameForException));
            }

            return 0;
        }

        public static string GetStringFromRecord(IDictionary<string, object> record, string paramName, string paramNameForException, bool throwException)
        {
            object typeId;
            if (!record.ContainsKey(paramName) && throwException)
            {
                throw new Exception("Параметр {0} отсутствует".FormatWith(paramNameForException));
            }

            record.TryGetValue(paramName, out typeId);
            if (typeId == null)
            {
                return string.Empty;
            }

            return typeId.ToString();
        }

        public static int GetIntFromRecord(IDictionary<string, object> record, string paramName, string paramNameForException)
        {
            return GetIntFromRecord(record, paramName, paramNameForException, true);
        }

        public static decimal GetDecimalFromRecord(IDictionary<string, object> record, string paramName, string paramNameForException)
        {
            return GetDecimalFromRecord(record, paramName, paramNameForException, true);
        }

        public static decimal GetDecimalFromRecord(IDictionary<string, object> record, string paramName, string paramNameForException, bool throwException)
        {
            object typeId;
            if (!record.ContainsKey(paramName) && throwException)
            {
                throw new Exception("Параметр {0} отсутствует".FormatWith(paramNameForException));
            }

            record.TryGetValue(paramName, out typeId);
            decimal id;
            if (Decimal.TryParse(typeId.ToString(), out id) && id > 0)
            {
                return id;
            }

            if (throwException)
            {
                throw new Exception("Параметр {0} имеет неверный тип".FormatWith(paramNameForException));
            }

            return 0;
        }

        public static decimal? GetDecimalNullFromRecord(IDictionary<string, object> record, string paramName, string paramNameForException, bool throwException)
        {
            object typeId;
            if (!record.ContainsKey(paramName) && throwException)
            {
                throw new Exception("Параметр {0} отсутствует".FormatWith(paramNameForException));
            }

            record.TryGetValue(paramName, out typeId);
            decimal id;
            if (typeId != null && typeId.ToString().Equals(string.Empty))
            {
                return null;
            }

            if (Decimal.TryParse(typeId.ToString(), out id))
            {
                return id;
            }

            if (throwException)
            {
                throw new Exception("Параметр {0} имеет неверный тип".FormatWith(paramNameForException));
            }

            return 0;
        }

        public static AjaxFormResult ErrorResult(string msg)
        {
            var result = new AjaxFormResult
            {
                Success = false,
                Script = null
            };

            result.ExtraParams["msg"] = "Ошибка сохранения.";
            result.ExtraParams["responseText"] = msg;

            return result;
        }
    }
}
