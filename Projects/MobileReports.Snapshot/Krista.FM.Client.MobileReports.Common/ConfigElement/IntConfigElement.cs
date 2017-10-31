using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Krista.FM.Client.MobileReports.Common.ConfigElement
{
    public class IntConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = false)]
        public int Value
        {
            get { return (int)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
