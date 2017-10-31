using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Krista.FM.Common
{
    /// <summary>
    /// Исключение на стороне сервера.
    /// </summary>
    [Serializable]
    public class ServerException : Exception
    {
        /// <summary>
        /// Создаёт новый объект класса.
        /// </summary>
        public ServerException()
            : base()
        {
        }

        /// <summary>
        /// Создаёт новый объект класса с заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public ServerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Конструктор для десериализации.
        /// </summary>
        /// <param name="info">Источник данных для десериализации.</param>
        /// <param name="context">Информация об источнике данных.</param>
        protected ServerException(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {
        }

        /// <summary>
        /// Создаёт новый объект класса с заданными сообщением об ошибке и ссылкой на внутреннее исключение.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="innerException">Ссылка на внутреннее исключение.</param>
        public ServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
