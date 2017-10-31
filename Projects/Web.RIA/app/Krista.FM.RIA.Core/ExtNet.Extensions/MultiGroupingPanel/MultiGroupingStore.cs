using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Ext.Net;
    
[assembly: WebResource("Krista.FM.RIA.Core.ExtNet.Extensions.MultiGroupingPanel.js.MultiGrouping.js", "text/javascript")]

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Специализированный Store для группировки данных по нескольким полям.
    /// </summary>
    /// <example>
    /// var store = new MultiGroupingStore { ID = "dsGroups" };
    /// ...
    /// Controls.Add(store);
    /// store.ResourceManager.RegisterOnReadyScript("dsGroups.groupField = ['SomeField'];");
    /// </example>
    public class MultiGroupingStore : GroupStore
    {
        [Category("0. About")]
        [Description("")]
        public override string InstanceOf
        {
            get
            {
                return XType;
            }
        }

        public override string XType
        {
            get { return "Ext.ux.MultiGroupingStore"; }
        }

        protected override List<ResourceItem> Resources
        {
            get
            {
                List<ResourceItem> baseList = base.Resources;
                baseList.Capacity += 1;
                baseList.Add(new ClientScriptItem(
                        typeof(MultiGroupingStore),
                        "Krista.FM.RIA.Core.ExtNet.Extensions.MultiGroupingPanel.js.MultiGrouping.js",
                        "ux/extensions/maximgb/TreeGrid.js"));
                return baseList;
            }
        }
    }
}
