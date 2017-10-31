using System;
using System.Runtime.Serialization;

namespace Krista.FM.Utils.Common
{
    public class OpenFMMDAllExeption : Exception
    {
        public OpenFMMDAllExeption(string message) : base (message)
        {}

        public OpenFMMDAllExeption(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }

        public OpenFMMDAllExeption() : base()
        {}
    }

    public class CreateFoldersException : Exception
    {
        public CreateFoldersException(string message)
            : base(message)
        {}

        public CreateFoldersException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {}

        public CreateFoldersException()
            : base()
        {}
    }

    public class SplitException : Exception
    {
        public SplitException(string message)
            : base(message)
        { }

        public SplitException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }

        public SplitException()
            : base()
        { }
    }

    public class JoinException : Exception
    {
        public JoinException(string message)
            : base(message)
        { }

        public JoinException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }

        public JoinException()
            : base()
        { }
    }

    public class SynthesisException : Exception
    {
        public SynthesisException(string message)
            : base(message)
        { }

        public SynthesisException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }

        public SynthesisException()
            : base()
        { }
    }

    public class Trace
    {
        public static void TraceError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void TraceWapning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void TraceInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void TraceSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
