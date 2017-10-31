using System;
using System.Runtime.Serialization;

namespace Krista.FM.Update.PatchMakerLibrary
{
    public class AddTaskException : Exception
    {
        public AddTaskException() : base() { }
        public AddTaskException(string message) : base(message) { }
        public AddTaskException(string message, Exception ex) : base(message, ex) { }
        public AddTaskException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
