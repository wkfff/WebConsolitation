using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Krista.FM.Client.MobileReports.Common.ConfigElement
{
    public class ScriptDownloadConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = false)]
        public ScriptsDownloadType Value
        {
            get { return (ScriptsDownloadType)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
