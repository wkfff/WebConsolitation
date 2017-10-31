using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Services
{
    /// <summary>
    /// Класс для трассировочных сообщений
    /// </summary>
    public static class LoggingService
    {
        static LoggingService()
        {
        }

        public static void Debug(object message)
        {
            System.Diagnostics.Debug.Write(message);
        }

        public static void DebugFormatted(string format, params object[] args)
        {
            System.Diagnostics.Debug.Write(String.Format(format, args));
        }

        public static void Info(object message)
        {
            System.Diagnostics.Trace.Write(message);
        }

        public static void InfoFormatted(string format, params object[] args)
        {
            System.Diagnostics.Trace.Write(String.Format(format, args));
        }

        public static void Warn(object message)
        {
            System.Diagnostics.Trace.Write(message);
        }

        public static void Warn(object message, Exception exception)
        {
            System.Diagnostics.Trace.Write(message);
        }

        public static void WarnFormatted(string format, params object[] args)
        {
            System.Diagnostics.Trace.Write(String.Format(format, args));
        }

        public static void Error(object message)
        {
        }

        public static void Error(object message, Exception exception)
        {
        }

        public static void ErrorFormatted(string format, params object[] args)
        {
        }
    }
}
