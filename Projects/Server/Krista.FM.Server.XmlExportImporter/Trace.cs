using Krista.Diagnostics;

namespace Krista.FM.Server.XmlExportImporter
{
    /// <summary>
    /// Предоставляет набор методов и свойств для трассировки сообщений.
    /// </summary>
    internal sealed class Trace
    {
        /// <summary>
        /// Имя источника
        /// </summary>
        private static readonly string sourceName = "Krista.FM.Server.XmlExportImporter";

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceEvent(System.Diagnostics.TraceEventType eventType, string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(eventType, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceCritical(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Critical, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceError(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Error, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Warning, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Information, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceVerbose(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Verbose, sourceName, format, args);
        }

        /// <summary>
        /// Уделичивает уровень отступа
        /// </summary>
        [System.Diagnostics.Conditional("TRACE")]
        public static void Indent()
        {
            KristaDiagnostics.Indent();
        }

        /// <summary>
        /// Уменьшает уровень отступа
        /// </summary>
        [System.Diagnostics.Conditional("TRACE")]
        public static void Unindent()
        {
            KristaDiagnostics.Unindent();
        }

        //
        // Summary:
        //     Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        //     collection.
        //
        // Parameters:
        //   message:
        //     A message to write.
        [System.Diagnostics.Conditional("TRACE")]
        public static void Write(string message)
        {
            KristaTrace.TraceInformation(sourceName, message);
        }
        //
        // Summary:
        //     Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        //     collection.
        //
        // Parameters:
        //   message:
        //     A message to write.
        [System.Diagnostics.Conditional("TRACE")]
        public static void WriteLine(string message)
        {
            KristaTrace.TraceInformation(sourceName, message);
        }
    }
}
