using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Krista.FM.Common
{
	/// <summary>
	/// Базовый тип исключений создаваемый службой расчета многомерных объектов.
	/// </summary>
	[Serializable]
	public class OlapProcessorException: ServerException
	{
		/// <summary>
		/// Создаёт новый объект класса.
		/// </summary>
		public OlapProcessorException()
			: base()
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданным сообщением об ошибке.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		public OlapProcessorException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Конструктор для десериализации.
		/// </summary>
		/// <param name="info">Источник данных для десериализации.</param>
		/// <param name="context">Информация об источнике данных.</param>
		protected OlapProcessorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Создаёт новый объект класса с заданными сообщением об ошибке и ссылкой на внутреннее исключение.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <param name="innerException">Ссылка на внутреннее исключение.</param>
		public OlapProcessorException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
