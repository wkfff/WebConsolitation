using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Krista.FM.Client.MobileReports.Common.ConfigElement
{
    public class BoolConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = false)]
        public bool Value
        {
            get { return (bool)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
