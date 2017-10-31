using System.Diagnostics;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Трассировщик сообщений в системный EventLog
    /// </summary>
    public class EventLogTraceListener : TraceListener
    {
        private readonly string eventLogName = "Application";
        private EventLog eventLog;

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        /// <param name="name">Имя журнала трассировки</param>
        public EventLogTraceListener(string name)
            : base(name)
        {
            eventLogName = name;
            eventLog = new EventLog(eventLogName);
        }

        /// <summary>
        /// Базовый конструктор. Журнал трассировки "Application"
        /// </summary>
        public EventLogTraceListener()
        {
            eventLog = new EventLog(eventLogName);
        }

        /// <summary>
        /// Преобразует тип сообщения трассировки в тип сообщения журанла
        /// </summary>
        /// <param name="eventType">Тип сообщения трассировки</param>
        /// <returns>Тип сообщения журнала</returns>
        private static EventLogEntryType GetEntryType(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Error: return EventLogEntryType.Error;
                case TraceEventType.Critical: return EventLogEntryType.Error;
                case TraceEventType.Warning: return EventLogEntryType.Warning;
                default: return EventLogEntryType.Information;
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
            TraceEvent(eventCache, source, eventType, id, string.Empty, null);
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
            if ((eventLog != null) && ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)))
            {
                CkeckSource(source);
                eventLog = new EventLog(eventLogName, ".", source);
                //_event_log.Source = source;
                eventLog.WriteEntry(args == null ? format : string.Format(format, args), GetEntryType(eventType), id);
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
            TraceEvent(eventCache, source, eventType, id, message, null);
        }
        /// <summary>
        /// Проверяет, находится ли источник сообщения в трассировконом журнале
        /// </summary>
        /// <param name="name">Имя источника</param>
        private void CkeckSource(string name)
        {
            if (EventLog.SourceExists(name))
            {
                string logName = EventLog.LogNameFromSourceName(name, ".");
                EventLog.DeleteEventSource(name);
                EventLog.CreateEventSource(name, logName);
                //				if (LogName != _event_log.Log)
                //              {
                //            }
            }
            else
            {
                EventLog.CreateEventSource(name, eventLog.Log);
            }
        }
        /// <summary>
        /// Вывод трассировочного сообщения
        /// </summary>
        /// <param name="message">Сообщение</param>
        public override void Write(string message)
        {
            eventLog.WriteEntry(message, EventLogEntryType.Information, 0);
        }

        /// <summary>
        /// Вывод трассировочного сообщения с преводом строки
        /// </summary>
        /// <param name="message">Сообщение</param>
        public override void WriteLine(string message)
        {
            Write(message);
        }
    }
}