using System;
using System.Runtime.Serialization;

namespace Krista.FM.Common.TaskDocuments
{
    [Serializable]
    public class OfficePluginFactoryException : Exception
    {
        public OfficePluginFactoryException()
        {
        }

        public OfficePluginFactoryException(string message)
            : base(message)
        {
        }

        protected OfficePluginFactoryException(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {
        }

        public OfficePluginFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}