using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Krista.FM.Common.Services
{
    /// <summary>
    /// Генерируется когда GlobalResource менеджер не может найти требуемый ресурс.
    /// </summary>
    [Serializable()]
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string resource)
            : base("Ресурс не найден : " + resource)
        {
        }

        public ResourceNotFoundException()
            : base()
        {
        }

        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
