using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Server.DataPumpApp;


namespace Krista.FM.Server.DataPumpHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            try
            {
                DataPumpLog.InitializePumpTrace(args[2] + ".txt", false);
                string userName = args[4].Split(' ')[0];
                Trace.WriteLine(string.Format("ID процесса: {0}", Process.GetCurrentProcess().Id));

                Trace.WriteLine(string.Format("Имя пользователя: {0}", userName));

                Trace.WriteLine(string.Join(" ", args), "DataPumpHost");

                DataPumpApplication dataPumpHost = null;//new DataPumpApplication("tcp://localhost:8008/FMServer/Server.rem", "_Подопытная", "Form16Pump", "PumpData");
                if (args.GetLength(0) == 5)
                {
                    // Параметры:
                    // УРЛ сервера приложений
                    // Имя схемы
                    // Идентификатор программы звкачки данных
                    // Стартовое состояние закачки
                    // параметры пользователя - имя, хост, сессия
                    dataPumpHost = new DataPumpApplication(args[0], args[1], args[2], args[3], args[4]);
                }
                else if (args.GetLength(0) == 6)
                {
                    // Параметры:
                    // УРЛ сервера приложений
                    // Имя схемы
                    // Идентификатор программы звкачки данных
                    // Стартовое состояние закачки
                    // ИД закачки (для режима удаления)
                    // ИД источника (для режима удаления)
                    dataPumpHost = new DataPumpApplication(args[0], args[1], args[2], args[3], args[4], args[5]);
                }

                dataPumpHost.Run();

                Trace.WriteLine("Процесс программы закачки данных завершен.", "DataPumpHost");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("CRITICAL ERROR: " + ex.ToString(), "DataPumpHost");
            }
            finally
            {
                DataPumpLog.CloseLogFile();
            }
        }
    }
}