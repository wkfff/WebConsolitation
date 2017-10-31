using System;
using System.Runtime.Serialization;

namespace Krista.FM.Common.OfficeHelpers
{
    public class OfficeApplicationException : Exception
    {
        public OfficeApplicationException()
        {
        }

        public OfficeApplicationException(string message)
            : base(message)
        {
        }

        protected OfficeApplicationException(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {
        }

        public OfficeApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
