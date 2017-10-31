using System;
using System.Runtime.Serialization;

namespace Krista.FM.Server.Scheme.ScriptingEngine
{
    [Serializable]
    public class ScriptingEngineException : Exception
    {
        public ScriptingEngineException()
        {
        }

        public ScriptingEngineException(string message)
            : base(message)
        {
        }

        public ScriptingEngineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ScriptingEngineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
