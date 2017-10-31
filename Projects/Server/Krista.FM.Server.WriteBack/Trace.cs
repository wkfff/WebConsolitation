﻿using System.Diagnostics;

using Krista.Diagnostics;

namespace Krista.FM.Server.WriteBack
{
    /// <summary>
    /// Предоставляет набор методов и свойств для трассировки сообщений.
    /// </summary>
    internal sealed class Trace
    {
        /// <summary>
        /// Имя источника
        /// </summary>
        private static string sourceName = "Krista.FM.Server.WriteBack";

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(eventType, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceCritical(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(TraceEventType.Critical, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceError(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(TraceEventType.Error, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(TraceEventType.Warning, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(TraceEventType.Information, sourceName, format, args);
        }

        /// <summary>
        /// Трассировочные сообщения системы запуска сервера среднего звена
        /// </summary>
        /// <param name="format">Строка формата сообщений</param>
        /// <param name="args">Аргументы формата</param>
        [Conditional("TRACE")]
        public static void TraceVerbose(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(TraceEventType.Verbose, sourceName, format, args);
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
    }
}
