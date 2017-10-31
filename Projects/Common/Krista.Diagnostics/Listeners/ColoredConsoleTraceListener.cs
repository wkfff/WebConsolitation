using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Перехватчик отладочных сообщений с выводом их на консоль, 
    /// выделющий цветом разного типа сообщения.
    /// </summary>
    public class ColoredConsoleTraceListener : ConsoleTraceListener
    {
        /// <summary>
        /// Сдвиг текста протокола вправо
        /// </summary>
        new public void WriteIndent()
        {
            base.WriteIndent();
        }

        /// <summary>
        /// Подготавливает цвет текста консоли
        /// </summary>
        /// <param name="eventType"></param>
        private static void PrepareColor(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Critical: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case TraceEventType.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case TraceEventType.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case TraceEventType.Information: Console.ForegroundColor = ConsoleColor.Green; break;
                case TraceEventType.Verbose: Console.ForegroundColor = ConsoleColor.Gray; break;
                default: Console.ForegroundColor = ConsoleColor.Green; break;
            }
        }

        /// <summary>
        /// Список имен истоников сообщений, не имеющих вначале приставки .krista
        /// </summary>
        private static readonly List<string> BadSources = new List<string>();

        /// <summary>
        /// Отсекает от имени приставку Krista. и выводит однократное сообщение, если такой приставки не было
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        private static string PrepareSource(string sourceName)
        {
            if (sourceName.StartsWith("Krista."))
            {
                return sourceName.Substring(7);
            }
            lock (BadSources)
            {
                if (!BadSources.Contains(sourceName))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(string.Format("Имя источника трассировочных сообщений {0} должно быть изменено на Krista.{0}", sourceName));
                    BadSources.Add(sourceName);
                }
            }
            return sourceName;
        }

        /// <summary>
        /// Вывод сообщения на консоль
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="source">Источник сообщения</param>
        /// <param name="id">Код сообщения</param>
        /// <param name="format">Строка сообщения</param>
        /// <param name="args">Параметры строки, если она представляет из себя строку формата, или null</param>
        private void PrintMessage(TraceEventCache eventCache, TraceEventType eventType, string source, int id, string format, params object[] args)
        {
            if ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
            {
                lock (typeof (Console))
                {
#if DEBUG
                    string sourceLabel = PrepareSource(source);
#else
					string sourceLabel = source;
#endif
                    PrepareColor(eventType);
                    WriteIndent();
                    if (args == null)
                    {
                        Console.WriteLine(string.Format("{0}: {1}", sourceLabel, format));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0}: {1}", sourceLabel, string.Format(format, args)));
                    }
                }
            }
        }

        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            PrintMessage(eventCache, eventType, source, id, format, args);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            PrintMessage(eventCache, eventType, source, id, string.Empty, null);
        }
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            PrintMessage(eventCache, eventType, source, id, message, null);
        }
    }
}