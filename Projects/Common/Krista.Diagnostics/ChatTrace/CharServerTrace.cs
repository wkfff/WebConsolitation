using System;
using System.Diagnostics;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Фасад системы трассировки сервера обмена сообщениями
    /// </summary>
    internal class CharServerTrace : KristaDiagnostics
    {
        /// <summary>
        /// Имя потока трассировочных сообщений ядра сервера среднего звена
        /// </summary>
        const string CharServerTraceSource = "Krista.ChatServer";
        /// <summary>
        /// Трассировочные сообщения системы ядра сервера среднего звена
        /// </summary>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(eventType, CharServerTraceSource, format, args);
        }
        /// <summary>
        /// Трассиковка исключительной ситуации
        /// </summary>
        /// <param name="e">Исключительная ситуация</param>
        [Conditional("TRACE")]
        public static void TraceEvent(Exception e)
        {
            KristaDiagnostics.TraceEvent(TraceEventType.Error, CharServerTraceSource, ExpandException(e));
        }
        /// <summary>
        /// Отладочные сообщения системы ядра сервера среднего звена
        /// </summary>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("DEBUG")]
        public static void DebugEvent(TraceEventType eventType, string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(eventType, CharServerTraceSource, format, args);
        }

    }
}