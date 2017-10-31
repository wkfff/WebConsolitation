using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Krista.FM.Client.MobileReports.Common.ConfigElement
{
    public class SnapshotModeConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = false)]
        public MobileReportsSnapshotMode Value
        {
            get { return (MobileReportsSnapshotMode)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
