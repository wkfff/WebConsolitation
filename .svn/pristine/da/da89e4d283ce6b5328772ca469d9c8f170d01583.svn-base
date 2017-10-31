using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Common.Exceptions
{
    /// <summary>
    /// Ошибка на стороне клиента
    /// </summary>
    public class ClientException : Exception
    {
         /// <summary>
        /// Создаёт новый объект класса.
        /// </summary>
        public ClientException()
            : base()
        {
        }

        /// <summary>
        /// Создаёт новый объект класса с заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public ClientException(string message)
            : base(String.Format("Ошибка на стороне клиента: {0}", message))
        {
        }
        
        /// <summary>
        /// Создаёт новый объект класса с заданными сообщением об ошибке и ссылкой на внутреннее исключение.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="innerException">Ссылка на внутреннее исключение.</param>
        public ClientException(string message, Exception innerException)
            : base(String.Format("Ошибка на стороне клиента: {0}", message), innerException)
        {
        }
    }
}
