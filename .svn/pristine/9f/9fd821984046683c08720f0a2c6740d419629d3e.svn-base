using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting;
using System.Text;
using Krista.FM.Client.MobileReports.Common;

namespace Krista.FM.Client.MobileReports.BootloaderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
                string bootloadServiceUri = String.Format("tcp://{0}/ReportsBootloaderService/Server.rem",
                        ConfigurationManager.AppSettings["BootloadServiceName"]);
                ISnapshotService server =
                    (ISnapshotService)
                    Activator.GetObject(typeof(ISnapshotService), bootloadServiceUri);
                server.Activate();
                Trace.TraceVerbose("Сервис генерации отчетов активирован");
            }
            catch (Exception e)
            {
#warning need error handler
                Trace.TraceError(e.Message);
                Trace.TraceError(e.StackTrace);
            }
            Console.ReadKey();
        }
    }
}
