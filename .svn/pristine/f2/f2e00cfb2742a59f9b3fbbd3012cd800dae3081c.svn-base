using System;

namespace Krista.FM.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Возвращает номер квартала в котором находится дата.
        /// </summary>
        /// <param name="dateTime">Дата, над которой будут производиться вычисления.</param>
        /// <returns>Возвращает число от 1 до 4 определяющее квартал в котором находится дата.</returns>
        public static int Quarter(this DateTime dateTime)
        {
            return ((dateTime.Month - 1) / 3) + 1;
        }
    }
}
