using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Krista.FM.Client.MobileReports.Common.ConfigElement
{
    public class StringConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = false)]
        public string Value 
        { 
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
