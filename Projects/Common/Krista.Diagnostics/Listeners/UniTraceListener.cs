using System;
using System.Diagnostics;
using System.IO;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Универсальный трассировщик сообщений.
    /// В зависимости от типа приложения выводит отладочные сообщения
    /// в разные трассировщики. Для консоли на ColoredConsoleTraceListener,
    /// для сервиса - EventLogTraceListener, для WinForms - EventLogTraceListener
    /// </summary>
    public class UniTraceListener : TraceListener
    {
        /// <summary>
        /// Активный трассировщик
        /// </summary>
        private readonly TraceListener innerListener;

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public UniTraceListener()
        {
            switch (RunMode.RunModeType)
            {
                case RunModeType.Console: innerListener = new ColoredConsoleTraceListener(); break;
                case RunModeType.Service: innerListener = GetServiceTraceListener(string.Format("{0}_{1}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now)); break;
                case RunModeType.WinForm: innerListener = new EventLogTraceListener(); break;
            }
        }

        /// <summary>
        /// Конструктор, с параметром. Для сервисов - имя журнала.
        /// </summary>
        /// <param name="param">Для сервисов - имя журнала, для других типов не имеет значения.</param>
        public UniTraceListener(string param)
        {
            switch (RunMode.RunModeType)
            {
                case RunModeType.Console: innerListener = new ColoredConsoleTraceListener(); break;
                case RunModeType.Service: innerListener = GetServiceTraceListener(param); break;
                case RunModeType.WinForm: innerListener = new EventLogTraceListener(param); break;
            }
        }

        private static TraceListener GetServiceTraceListener(string param)
        {
            Uri logUri = new Uri(new Uri(AppDomain.CurrentDomain.SetupInformation.ApplicationBase), param);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(logUri.LocalPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(logUri.LocalPath));
                }
            }
            catch
            {
            }
            DelimitedListTraceListener delimiterListener = new DelimitedListTraceListener(logUri.LocalPath);
            delimiterListener.Delimiter = "::";
            delimiterListener.TraceOutputOptions = TraceOptions.DateTime;
            return delimiterListener;
        }

        /// <summary>
        /// Выводит отсуп. Используется только, если трассировщик консольный
        /// </summary>
        protected override void WriteIndent()
        {
            if ((innerListener != null) && (innerListener is ColoredConsoleTraceListener))
            {
                (innerListener as ColoredConsoleTraceListener).WriteIndent();
            }
        }
        /// <summary>
        /// Трассировочное сообщение
        /// </summary>
        /// <param name="eventCache">Кэш трассировочных сообщений</param>
        /// <param name="source">Источник сообщения</param>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="id">Идентификатор сообщения</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            if ((innerListener != null) && ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, string.Empty, null, null, null)))
            {
                lock (innerListener)
                {
                    innerListener.IndentLevel = IndentLevel;
                    innerListener.TraceEvent(eventCache, source, eventType, id);
                }
            }
        }
        /// <summary>
        /// Трассировочное сообщение
        /// </summary>
        /// <param name="eventCache">Кэш трассировочных сообщений</param>
        /// <param name="source">Источник сообщения</param>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="id">Идентификатор сообщения</param>
        /// <param name="format">Строка формата сообщения</param>
        /// <param name="args">Аргументы строки формата</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if ((innerListener != null) && ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)))
            {
                lock (innerListener)
                {
                    innerListener.IndentLevel = IndentLevel;
                    innerListener.TraceEvent(eventCache, source, eventType, id, format, args);
                }
            }
        }
        /// <summary>
        /// Трассировочное сообщение
        /// </summary>
        /// <param name="eventCache">Кэш трассировочных сообщений</param>
        /// <param name="source">Источник сообщения</param>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="id">Идентификатор сообщения</param>
        /// <param name="message">Трассировочное сообщение</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if ((innerListener != null) && ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null)))
            {
                lock (innerListener)
                {
                    innerListener.IndentLevel = IndentLevel;
                    innerListener.TraceEvent(eventCache, source, eventType, id, message);
                }
            }
        }
        /// <summary>
        /// Вывод трассировочного сообщения
        /// </summary>
        /// <param name="message">Сообщение</param>
        public override void Write(string message)
        {
            if (innerListener != null)
            {
                lock (innerListener)
                {
                    innerListener.IndentLevel = IndentLevel;
                    innerListener.Write(message);
                }
            }
        }

        /// <summary>
        /// Вывод трассировочного сообщения с преводом строки
        /// </summary>
        /// <param name="message">Сообщение</param>
        public override void WriteLine(string message)
        {
            if (innerListener != null)
            {
                lock (innerListener)
                {
                    innerListener.IndentLevel = IndentLevel;
                    innerListener.WriteLine(message);
                }
            }
        }
    }
}