using System.Collections.Generic;
using System.Web.UI;
using Ext.Net;

[assembly: WebResource("Krista.FM.RIA.Core.ExtNet.Plugins.CurrencyField.resources.CurrencyField.js", "text/javascript")]

namespace Krista.FM.RIA.Core.ExtNet.Plugins.CurrencyField
{
    public class CurrencyField : Plugin
    {
        public override string InstanceOf
        {
            get
            {
                return "Ext.ux.CurrencyField";
            }
        }

        protected override List<ResourceItem> Resources
        {
            get
            {
                List<ResourceItem> baseList = base.Resources;
                baseList.Capacity += 1;

                baseList.Add(new ClientScriptItem(
                    typeof(CurrencyField),
                    "Krista.FM.RIA.Core.ExtNet.Plugins.CurrencyField.resources.CurrencyField.js",
                    "ux/extensions/currencyfield/currencyfield.js"));

                return baseList;
            }
        }
    }
}
