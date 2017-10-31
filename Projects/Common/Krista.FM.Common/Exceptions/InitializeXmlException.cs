using System;
using System.Runtime.Serialization;

namespace Krista.FM.Common
{
    /// <summary>
    /// Исключение возникающее при инициализации объекта по Xml-описанию
    /// </summary>
    [Serializable]
    public class InitializeXmlException : Exception
    {
        /// <summary>
        /// Конструктор со ссылкой на внутреннее исключение.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public InitializeXmlException(string message, Exception innerException)
            : base(message + " Внутреннее исключение: " + innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Конструктор для десериализации.
        /// </summary>
        /// <param name="info">Источник данных для десериализации.</param>
        /// <param name="context">Информация об источнике данных.</param>
        protected InitializeXmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Создаёт новый объект класса.
        /// </summary>
        public InitializeXmlException()
            : base()
        {
        }

        /// <summary>
        /// Создаёт новый объект класса с заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public InitializeXmlException(string message)
            : base(message)
        {
        }
    }
}