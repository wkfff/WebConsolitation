using System;
using System.Runtime.Serialization;

namespace Krista.FM.Domain
{
    [Serializable]
    public abstract class DomainException : ApplicationException
    {
        public DomainException(string message)
            : this(message, null)
		{
		}

        public DomainException(string message, Exception e)
            : base(message, e)
        {
        }

        public DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
    }
}
