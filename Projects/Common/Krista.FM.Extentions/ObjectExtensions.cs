using System;
using System.ComponentModel;
using System.Globalization;

namespace Krista.FM.Extensions
{
    /// <summary>
    /// Расширения для конвертации типов.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Пытается конвертировать объект в тип объекта <paramref name="converted"/>.
        /// </summary>
        /// <typeparam name="T">Любой тип.</typeparam>
        /// <param name="toConvert">Объект, который будет сконвертирован.</param>
        /// <param name="converted">Объект, в который будет происходить конвертация.</param>
        /// <returns>True, если конвертация прошла успешно, иначе false.</returns>
        public static bool TryConvertInto<T>(this object toConvert, out T converted)
        {
            converted = default(T);

            if (toConvert == null || string.IsNullOrEmpty(toConvert.ToString()))
            {
                return false;
            }

            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                converted = (T)converter.ConvertFromInvariantString(Convert.ToString(toConvert, CultureInfo.InvariantCulture));
                return true;
            }
            catch (NotSupportedException)
            {
            }
            
            return false;
        }

        /// <summary>
        /// Пытается конвертировать объект в тип объекта <paramref name="{T}"/>.
        /// </summary>
        /// <typeparam name="T">Любой тип.</typeparam>
        /// <param name="toConvert">Объект, который будет сконвертирован.</param>
        /// <returns>
        /// Значение объекта преобразованное в тип <paramref name="{T}"/>, 
        /// если значение невозможно преобразовать, то возвращает значение по умолчанию для типа <paramref name="{T}"/>.
        /// </returns>
        public static T Value<T>(this object toConvert)
        {
            T result;

            if (toConvert == null || string.IsNullOrEmpty(toConvert.ToString()) || !toConvert.TryConvertInto<T>(out result))
            {
                return default(T);
            }

            return result;
        }

        /// <summary>
        /// Пытается конвертировать объект в тип объекта <paramref name="{T}"/>.
        /// </summary>
        /// <typeparam name="T">Любой тип.</typeparam>
        /// <param name="toConvert">Объект, который будет сконвертирован.</param>
        /// <returns>
        /// Значение объекта преобразованное в тип <paramref name="{T}"/>, 
        /// если значение невозможно преобразовать, то возвращает null.
        /// </returns>
        public static T AsOrNull<T>(this object toConvert) where T : class 
        {
            return toConvert as T;
        }
    }
}
