using System;
using System.Runtime.Serialization;

namespace Krista.FM.Common
{
	/// <summary>
	/// Исключение создаваемок при отправке многомерного объекта на расчет.
	/// </summary>
	[Serializable]
	public class InvalidateOlapObjectException : OlapProcessorException
	{
		/// <summary>
		/// Создаёт новый объект класса.
		/// </summary>
		public InvalidateOlapObjectException()
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданным сообщением об ошибке.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		public InvalidateOlapObjectException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Конструктор для десериализации.
		/// </summary>
		/// <param name="info">Источник данных для десериализации.</param>
		/// <param name="context">Информация об источнике данных.</param>
		protected InvalidateOlapObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданными сообщением об ошибке и ссылкой на внутреннее исключение.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <param name="innerException">Ссылка на внутреннее исключение.</param>
		public InvalidateOlapObjectException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}