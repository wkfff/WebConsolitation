using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.FMService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            Trace.WriteLine("----------------------------------------------------------------------");
            Trace.WriteLine("Server startup at " + DateTime.Now.ToString());

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            ServicesToRun = new ServiceBase[] { new FMService() };

            ServiceBase.Run(ServicesToRun);

            Trace.WriteLine("Server shutdown at " + DateTime.Now.ToString());
        }
    }
}