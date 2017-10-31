using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Server.DataPumpApp;


namespace Krista.FM.Server.DataPumpConsole
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
                if (args.GetLength(0) != 4 && args.GetLength(0) != 6)
                {
                    throw new Exception(string.Format("Неверное количество параметров ({0})", args.GetLength(0)));
                }

                DataPumpLog.InitializePumpTrace(args[2] + ".txt", true);

                Trace.WriteLine(string.Format("ID процесса: {0}", Process.GetCurrentProcess().Id));

                Trace.WriteLine(string.Format("Имя пользователя: {0}", Environment.UserName));

                Trace.WriteLine(string.Join(" ", args), "DataPumpConsole");

                DataPumpApplication dataPumpHost = null;//new DataPumpApplication(
                //    "tcp://localhost:8008/FMServer/Server.rem", "Develop", "SKIFMonthRepPump", "PumpData");

                if (args.GetLength(0) == 4)
                {
                    // Параметры:
                    // УРЛ сервера приложений
                    // Имя схемы
                    // Идентификатор программы звкачки данных
                    // Стартовое состояние закачки
                    dataPumpHost = new DataPumpApplication(args[0], args[1], args[2], args[3], string.Empty);
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

                Trace.WriteLine("Процесс программы закачки данных завершен.", "DataPumpConsole");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("CRITICAL ERROR: " + ex.ToString(), "DataPumpConsole");
            }
            finally
            {
                DataPumpLog.CloseLogFile();
            }
        }
    }
}