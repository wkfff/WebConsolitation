using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Xml.Serialization;
using Ext.Net;
using Newtonsoft.Json;

[assembly: WebResource("Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelRequisitesGridView.js.ExcelRequisitesGridView.js", "text/javascript")]

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelRequisitesGridView
{
    public class ExcelRequisitesGridView : ExcelGridView.ExcelGridView
    {
        public override string InstanceOf
        {
            get { return XType; }
        }

        public override string XType
        {
            get { return "Ext.ux.ExcelRequisitesGridView"; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        [JsonIgnore]
        public override ConfigOptionsCollection ConfigOptions
        {
            get
            {
                ConfigOptionsCollection list = base.ConfigOptions;

                list.Add("layoutMarkup", new ConfigOption("layoutMarkup", new SerializationOptions("layoutMarkup", JsonMode.Object), String.Empty, LayoutMarkup));

                return list;
            }
        }

        protected override List<ResourceItem> Resources
        {
            get
            {
                List<ResourceItem> baseList = base.Resources;
                baseList.Capacity += 1;
                baseList.Add(new ClientScriptItem(
                        typeof(ExcelGridView.ExcelGridView),
                        "Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelRequisitesGridView.js.ExcelRequisitesGridView.js",
                        "ux/extensions/extnet/ExcelRequisitesGridView.js"));
                return baseList;
            }
        }
    }
}
