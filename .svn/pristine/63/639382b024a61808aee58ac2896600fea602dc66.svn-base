using System;
using System.Diagnostics;

using Krista.Diagnostics;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.E86N
{
    /// <summary>
    /// Предоставляет набор методов и свойств для трассировки сообщений.
    /// </summary>
    internal sealed class Trace
    {
        /// <summary>
        ///  Имя источника
        /// </summary>
        private static string sourceName = "Krista.FM.RIA.Extensions.E86N";

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, string format, params object[] args)
        {
            TraceEventLocal(eventType, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceCritical(string format, params object[] args)
        {
            TraceEventLocal(TraceEventType.Critical, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceError(string format, params object[] args)
        {
            TraceEventLocal(TraceEventType.Error, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args)
        {
            TraceEventLocal(TraceEventType.Warning, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args)
        {
            TraceEventLocal(TraceEventType.Information, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceVerbose(string format, params object[] args)
        {
            TraceEventLocal(TraceEventType.Verbose, format, args);
        }

        /// <summary>
        /// Уделичивает уровень отступа
        /// </summary>
        [Conditional("TRACE")]
        public static void Indent()
        {
            KristaDiagnostics.Indent();
        }

        /// <summary>
        /// Уменьшает уровень отступа
        /// </summary>
        [Conditional("TRACE")]
        public static void Unindent()
        {
            KristaDiagnostics.Unindent();
        }

        /// <summary>
        /// Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        /// collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        [Conditional("TRACE")]
        public static void Write(string message)
        {
            TraceEventLocal(TraceEventType.Information, message);
        }

        /// <summary>
        /// Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        /// collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        [Conditional("TRACE")]
        public static void WriteLine(string message)
        {
            TraceEventLocal(TraceEventType.Information, message);
        }

        private static void TraceEventLocal(TraceEventType eventType, string format, params object[] args)
        {
            var formatStr = "[{0}] {1}".FormatWith(DateTime.Now, format);
            KristaDiagnostics.TraceEvent(eventType, sourceName, formatStr, args);
        }
    }
}
