﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Krista.FM.Common.Exceptions
{
    /// <summary>
    /// Ошибки генерации справки
    /// </summary>
    [Serializable]
    public class HelpException : ServerException
    {
        /// <summary>
		/// Создаёт новый объект класса.
		/// </summary>
		public HelpException()
			: base()
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданным сообщением об ошибке.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		public HelpException(string message)
			: base(String.Format("Ошибка при генерации справки : {0}", message))
		{
		}

		/// <summary>
		/// Конструктор для десериализации.
		/// </summary>
		/// <param name="info">Источник данных для десериализации.</param>
		/// <param name="context">Информация об источнике данных.</param>
		protected HelpException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданными сообщением об ошибке и ссылкой на внутреннее исключение.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <param name="innerException">Ссылка на внутреннее исключение.</param>
        public HelpException(string message, Exception innerException)
            : base(String.Format("Ошибка при генерации справки : {0}", message), innerException)
		{
		}        
    }
}
