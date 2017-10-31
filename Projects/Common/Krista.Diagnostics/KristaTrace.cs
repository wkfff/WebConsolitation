using System.Diagnostics;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Трассировка сообщений
    /// </summary>
	public class KristaTrace : KristaDiagnostics
    {
        #region Простая форма
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceCritical(string source, string message)
        {
            TraceEvent(TraceEventType.Critical, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceError(string source, string message)
        {
            TraceEvent(TraceEventType.Error, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceWarning(string source, string message)
        {
            TraceEvent(TraceEventType.Warning, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceInformation(string source, string message)
        {
            TraceEvent(TraceEventType.Information, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceVerbose(string source, string message)
        {
            TraceEvent(TraceEventType.Verbose, source, message);
        }
        #endregion


        #region Простая форма с условием
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceCritical(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Critical, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceError(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Error, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        ///<param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceWarning(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Warning, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceInformation(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Information, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceVerbose(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Verbose, source, message);
        }
        #endregion


        #region Форма с использованием TraceOptions
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceCritical(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Critical, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceError(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Error, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceWarning(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Warning, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceInformation(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Information, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("TRACE")]
        public static void TraceVerbose(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Verbose, options.Source, message);
        }
        #endregion

    }

    /// <summary>
    /// Вывод сообщения отладочных сообщений
    /// </summary>
	public class KristaDebug : KristaDiagnostics
    {
        #region Простая форма
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceCritical(string source, string message)
        {
            TraceEvent(TraceEventType.Critical, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceError(string source, string message)
        {
            TraceEvent(TraceEventType.Error, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceWarning(string source, string message)
        {
            TraceEvent(TraceEventType.Warning, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceInformation(string source, string message)
        {
            TraceEvent(TraceEventType.Information, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceVerbose(string source, string message)
        {
            TraceEvent(TraceEventType.Verbose, source, message);
        }
        #endregion


        #region Простая форма с условием
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceCritical(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Critical, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceError(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Error, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        ///<param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceWarning(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Warning, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceInformation(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Information, source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceVerbose(bool condition, string source, string message)
        {
            if (condition) TraceEvent(TraceEventType.Verbose, source, message);
        }
        #endregion


        #region Форма с использованием TraceOptions
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceCritical(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Critical, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceError(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Error, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceWarning(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Warning, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceInformation(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Information, options.Source, message);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="options"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void TraceVerbose(TraceParams options, string message)
        {
            if ((options != null) && options.Enabled) TraceEvent(TraceEventType.Verbose, options.Source, message);
        }
        #endregion

    }
}
