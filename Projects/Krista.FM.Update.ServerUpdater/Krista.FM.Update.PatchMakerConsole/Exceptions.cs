using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Krista.FM.Update.PatchMakerConsole
{
    public class CreatePatchFolderException : Exception
    {
        public CreatePatchFolderException() : base() { }
        public CreatePatchFolderException(string message) : base(message) { }
        public CreatePatchFolderException(string message, Exception ex) : base(message, ex) { }
        public CreatePatchFolderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
