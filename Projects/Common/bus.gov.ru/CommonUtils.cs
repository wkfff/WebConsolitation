using System;
using System.Data.SqlTypes;

using Krista.FM.ServerLibrary;

namespace bus.gov.ru
{
    /// <summary>
    /// Утилитный класс для использования в разных солюшенах(закачки и веб приложение)
    /// </summary>
    public static class CommonUtils
    {
        /// <summary>
        /// Проверка на вхождение интервала дат в другой интервал
        ///     Если интервалы не определены то считаем что входит
        ///     Дата закрытия считается со следующей даты за указанной
        /// </summary>
        /// <typeparam name="T"> Тип проверяемого элемента </typeparam>
        /// <param name="element"> Проверяемый объект </param>
        /// <param name="dateBeginDoc"> Дата начала проверяемого интервала </param>
        /// <param name="dateEndDoc"> Дата окончания проверяемого интервала </param>
        /// <param name="beginDateAttribute"> Наименование поля содержащего дату включения </param>
        /// <param name="endDateAttribute"> Наименование поля содержащего дату закрытия </param>
        /// <returns>
        /// true - если входит
        /// </returns>
        public static bool IsEntryDates<T>(
            this T element, 
            DateTime? dateBeginDoc, 
            DateTime? dateEndDoc, 
            string beginDateAttribute, 
            string endDateAttribute)
        {

            var dateBeginElement = (DateTime?)element.GetType().GetProperty(beginDateAttribute).GetValue(element, null);
            var dateEndElement = (DateTime?)element.GetType().GetProperty(endDateAttribute).GetValue(element, null); 

            // если интервал не определен то все входит
            if (!(dateBeginDoc.HasValue || dateEndDoc.HasValue) || !(dateBeginElement.HasValue || dateEndElement.HasValue))
            {
                return true;
            }

            if (!dateBeginDoc.HasValue)
            {
                dateBeginDoc = DateTime.MinValue;
            }

            if (!dateEndDoc.HasValue)
            {
                dateEndDoc = DateTime.MaxValue;
            }

            if (!dateBeginElement.HasValue)
            {
                dateBeginElement = DateTime.MinValue;
            }

            if (!dateEndElement.HasValue)
            {
                dateEndElement = DateTime.MaxValue;
            }

            return !(dateBeginDoc > dateEndElement) && !(dateEndDoc < dateBeginElement);
        }

        /// <summary>
        ///  Запись сообщения в протокол закачки
        /// </summary>
        public static void WriteProtocolEvent(this IDataPumpProtocol dataPumpProtocol, DataPumpEventKind eventKind, string message)
        {
            dataPumpProtocol.WriteEventIntoDataPumpProtocol(
                eventKind,
                0,
                0,
                message);
        }

        /// <summary>
        ///   Проверка корректности даты
        /// </summary>
        public static bool IsValidSqlDate(DateTime date)
        {
            return (date >= (DateTime)SqlDateTime.MinValue) && (date <= (DateTime)SqlDateTime.MaxValue);
        }
    }
}