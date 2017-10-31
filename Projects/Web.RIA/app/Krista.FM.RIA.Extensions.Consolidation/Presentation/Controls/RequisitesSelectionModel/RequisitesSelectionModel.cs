using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Ext.Net;

[assembly: WebResource("Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.RequisitesSelectionModel.js.RequisitesSelectionModel.js", "text/javascript")]

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.RequisitesSelectionModel
{
    public class RequisitesSelectionModel : CellSelectionModel
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
            get { return "Ext.ux.RequisitesSelectionModel"; }
        }

        protected override List<ResourceItem> Resources
        {
            get
            {
                List<ResourceItem> baseList = base.Resources;
                baseList.Capacity += 1;
                baseList.Add(new ClientScriptItem(
                        typeof(RequisitesSelectionModel),
                        "Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.RequisitesSelectionModel.js.RequisitesSelectionModel.js",
                        "ux/extensions/extnet/RequisitesSelectionModel.js"));
                return baseList;
            }
        }
    }
}
