using System;

namespace Krista.FM.Domain.DataSetParser
{
	public class DomainObjectException : Exception
	{
		public DomainObjectException(string message)
			: base(message)
		{
		}

		public DomainObjectException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
