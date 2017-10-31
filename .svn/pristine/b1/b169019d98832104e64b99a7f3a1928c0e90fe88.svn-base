using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.MobileReports.Common
{
    [Serializable]
    public class SnapshotStartParams
    {
        private string serverURL;
        private string startParams;
        private string reportsHostAddress;

        public string ServerURL
        {
            get { return serverURL; }
        }

        public string StartParams
        {
            get { return startParams; }
        }
        
        public string ReportsHostAddress
        {
            get { return reportsHostAddress; }
        }

        public SnapshotStartParams(string serverURL, string startParams)
        {
            this.serverURL = serverURL;
            this.startParams = startParams;
        }

        public SnapshotStartParams(string serverURL, string startParams, string reportsHostAddress)
        {
            this.serverURL = serverURL;
            this.startParams = startParams;
            this.reportsHostAddress = reportsHostAddress;
        }
    }
}
