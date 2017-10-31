using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Krista.FM.Client.WebReportsCommon.ConfigElement
{
    public class ReportExtensionConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = false)]
        public ReportExtension Value
        {
            get { return (ReportExtension)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
