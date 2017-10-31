using System;
using System.Runtime.Serialization;

namespace Krista.FM.Common
{
	/// <summary>
	/// Идентифицирует исключения возникающие в процессе создания класса-одиночки.
	/// </summary>
	[Serializable]
	public class SingletonException
		: Exception
	{
		public SingletonException()
		{
		}

		public SingletonException(string message)
			: base(message)
		{
		}

		public SingletonException(Exception innerException)
			: base(null, innerException)
		{
		}

		public SingletonException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SingletonException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
