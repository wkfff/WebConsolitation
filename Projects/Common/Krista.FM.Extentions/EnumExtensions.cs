using System;
using System.ComponentModel;
using System.Reflection;

namespace Krista.FM.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Возвращает количество элементов в перечислении.
        /// </summary>
        public static int GetItemsCount(this Enum enumType)
        {
            return Enum.GetValues(enumType.GetType()).GetLength(0);
        }

        /// <summary>
        /// Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="variable">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable == null)
                return false;

            if (value == null)
                throw new ArgumentNullException("value");

            // Not as good as the .NET 4 version of this function, but should be good enough
            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentNullException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            ulong num = Convert.ToUInt64(value);
            return ((Convert.ToUInt64(variable) & num) == num);
        }

        /// <summary>
        /// Возвращает орисание элемента перечисления указанного в атрибуте DescriptionAttribute.
        /// </summary>
        /// <returns>Описание элемента перечисления.</returns>
        public static string GetDescription(this Enum enumValue)
        {
            Type type = enumValue.GetType();
            FieldInfo fi = type.GetField(Enum.GetName(type, enumValue));
            DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));
            return da.Description;
        }
    }
}
