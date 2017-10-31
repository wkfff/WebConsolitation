using System;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;

namespace Krista.Diagnostics.Log4Net
{
    /// <summary>
    /// Записывает все трассировочные сообщения в протоколизатор log4net.
    /// </summary>
    public class Log4NetTraceListener : TraceListener
    {
        private readonly Dictionary<string, ILog> sourceLogs = new Dictionary<string, ILog>();

        private ILog GetLog4Source(string source)
        {
            ILog log;
            lock (sourceLogs)
            {
                if (sourceLogs.ContainsKey(source))
                {
                    log = sourceLogs[source];
                }
                else
                {
                    log = LogManager.GetLogger(source);
                    sourceLogs.Add(source, log);
                }
            }

            return log;
        }

        private void LogMessage(TraceEventType eventType, string source, string format, object[] args)
        {
            ILog log = GetLog4Source(source);

            switch (eventType)
            {
                case TraceEventType.Critical:
                    log.FatalFormat(format, args);
                    break;
                case TraceEventType.Error:
                    log.ErrorFormat(format, args);
                    break;
                case TraceEventType.Information:
                    log.InfoFormat(format, args);
                    break;
                case TraceEventType.Verbose:
                    log.DebugFormat(format, args);
                    break;
                case TraceEventType.Warning:
                    log.WarnFormat(format, args);
                    break;
                default:
                    log.InfoFormat(format, args);
                    break;
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            LogMessage(eventType, source, format, args);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            LogMessage(eventType, source, string.Empty, null);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            LogMessage(eventType, source, message, null);
        }

        public override void Write(string message)
        {
            ILog log = GetLog4Source(String.Empty);
            log.Info(message);
        }

        public override void WriteLine(string message)
        {
            ILog log = GetLog4Source(String.Empty);
            log.Info(message);
        }
    }
}
