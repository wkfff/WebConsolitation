using System;
using System.Runtime.Serialization;

namespace Krista.FM.DigitalSignature
{
    [Serializable]
    public class SignException : Exception
    {
        public SignException()
        {
        }

        public SignException(string message)
            : base(message)
        {
        }

        public SignException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SignException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}