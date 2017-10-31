using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.ServiceProcess;
using System.Text;
using Krista.FM.Client.MobileReports.Common;

namespace Krista.FM.Client.MobileReports.BootloaderService
{
    public partial class BootloaderService : ServiceBase
    {
        public BootloaderService()
        {
            InitializeComponent();
        }

        private ISnapshotService server;

        protected override void OnStart(string[] args)
        {
           // System.Runtime.Remoting.RemotingServices.
            // TODO: Add code here to start your service.
            try
            {
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
                string bootloadServiceUri = String.Format("tcp://{0}/ReportsBootloaderService/Server.rem",
                        ConfigurationManager.AppSettings["BootloadServiceName"]);
                server =
                    (ISnapshotService)
                    Activator.GetObject(typeof (ISnapshotService), bootloadServiceUri);
                server.Activate();
            }
            catch(Exception e)
            {
#warning need error handler
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            ((IDisposable)server).Dispose();
            server = null;
        }
    }
}
