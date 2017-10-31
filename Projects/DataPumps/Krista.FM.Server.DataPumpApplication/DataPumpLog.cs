using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace Krista.FM.Server.DataPumpApp
{
    /// <summary>
    /// Класс для работы с логом закачки
    /// </summary>
    public static class DataPumpLog
    {
        private static TextWriterTraceListener textWriter = null;
        private static TextWriterTraceListener consoleWriter = null;

        public static void InitializePumpTrace(string fileName, bool useConsole)
        {
            System.Threading.Thread.CurrentThread.Name = AppDomain.CurrentDomain.FriendlyName;

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;

            string logsDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\DataPumpLogs\\";
            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }

            Trace.Listeners.Clear();
            TextWriter wr = new StreamWriter(logsDir + fileName, true, Encoding.GetEncoding(1251));
            textWriter = new TextWriterTraceListener(wr, "DataPumpLog");
            Trace.Listeners.Add(textWriter);

            if (useConsole)
            {
                TextWriterTraceListener consoleWriter = new TextWriterTraceListener(System.Console.Out, "DataPumpConsoleLog");
                Trace.Listeners.Add(consoleWriter);
            }
            Trace.AutoFlush = true;
            Trace.WriteLine("----------------------------------------------------------------------");
            Trace.WriteLine("DataPump startup at " + DateTime.Now.ToString());
        }

        /// <summary>
        /// Удаление объектов вывода логов
        /// </summary>
        public static void CloseLogFile()
        {
            Debug.WriteLine("Удаление TextWriterTraceListener", "DataPumpLog");

            if (textWriter != null)
            {
                Trace.Listeners.Remove(textWriter);
                textWriter.Close();
                textWriter = null;
            }
            Debug.WriteLine("TextWriterTraceListener удален", "DataPumpLog");

            if (consoleWriter != null)
            {
                Trace.Listeners.Remove(consoleWriter);
                consoleWriter.Close();
                consoleWriter = null;
            }
        }
    }
}
