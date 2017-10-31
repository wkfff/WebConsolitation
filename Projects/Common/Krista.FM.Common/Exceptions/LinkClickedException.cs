using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Common.Exceptions
{
    /// <summary>
    /// Тип исключения, возникающий при переходе по гиперссылке
    /// </summary>
    public class LinkClickedException : ClientException
    {
        /// <summary>
		/// Создаёт новый объект класса.
		/// </summary>
		public LinkClickedException()
			: base()
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданным сообщением об ошибке.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		public LinkClickedException(string message)
            : base(String.Format("Ошибка при переходе по гиперссылке: {0}", message))
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданными сообщением об ошибке и ссылкой на внутреннее исключение.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <param name="innerException">Ссылка на внутреннее исключение.</param>
        public LinkClickedException(string message, Exception innerException)
            : base(String.Format("Ошибка при переходе по гиперссылке: {0}", message), innerException)
		{
		}
    }
}
