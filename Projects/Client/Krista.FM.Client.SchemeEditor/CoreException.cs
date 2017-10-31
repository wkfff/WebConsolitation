using System;
using System.Runtime.Serialization;

namespace Krista.FM.Client.SchemeEditor
{
    [Serializable()]
    public class CoreException : ApplicationException
    {
        public CoreException()
            : base()
        {
        }

        public CoreException(string message)
            : base(message)
        {
        }

        public CoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
