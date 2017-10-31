using System;
using System.Runtime.Serialization;

namespace Krista.FM.Update.Framework
{
    public class NAppUpdateException : Exception
    {
        public NAppUpdateException() : base() { }
        public NAppUpdateException(string message) : base(message) { }
        public NAppUpdateException(string message, Exception ex) : base(message, ex) { }
        public NAppUpdateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class UpdateProcessFailedException : NAppUpdateException
    {
        public UpdateProcessFailedException() : base() { }
        public UpdateProcessFailedException(string message) : base(message) { }
        public UpdateProcessFailedException(string message, Exception ex) : base(message, ex) { }
        public UpdateProcessFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class ReadFeedException: Exception
    {
        public ReadFeedException() : base() { }
        public ReadFeedException(string message) : base(message) { }
        public ReadFeedException(string message, Exception ex) : base(message, ex) { }
        public ReadFeedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class PreparetaskException : Exception
    {
        public PreparetaskException() : base() { }
        public PreparetaskException(string message) : base(message) { }
        public PreparetaskException(string message, Exception ex) : base(message, ex) { }
        public PreparetaskException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class FileDownloaderException : Exception
    {
        public FileDownloaderException() : base() { }
        public FileDownloaderException(string message) : base(message) { }
        public FileDownloaderException(string message, Exception ex) : base(message, ex) { }
        public FileDownloaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class RollbackException : Exception
    {
        public RollbackException() : base() { }
        public RollbackException(string message) : base(message) { }
        public RollbackException(string message, Exception ex) : base(message, ex) { }
        public RollbackException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class FrameworkRemotingException : Exception
    {
        public FrameworkRemotingException() : base() { }
        public FrameworkRemotingException(string message) : base(message) { }
        public FrameworkRemotingException(string message, Exception ex) : base(message, ex) { }
        public FrameworkRemotingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }
}
