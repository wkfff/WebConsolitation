using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Xml.Serialization;
using Ext.Net;
using Krista.FM.RIA.Extensions.Consolidation.Models;
using Newtonsoft.Json;

[assembly: WebResource("Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelGridView.js.ExcelGridView.js", "text/javascript")]
[assembly: WebResource("Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelGridView.css.ExcelGridView.css", "text/css")]

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelGridView
{
    /// <summary>
    /// Обертка компонента ExcelGridView.
    /// </summary>
    public partial class ExcelGridView : GridView
    {
        [ConfigOption("layoutMarkup", JsonMode.Custom)]
        public LayoutMarkupViewModel LayoutMarkup { get; set; }

        public override string InstanceOf
        {
            get { return XType; }
        }

        public override string XType
        {
            get { return "Ext.ux.ExcelGridView"; }
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
                baseList.Capacity += 2;
                baseList.Add(new ClientScriptItem(
                        typeof(ExcelGridView),
                        "Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelGridView.js.ExcelGridView.js",
                        "ux/extensions/extnet/ExcelGridView.js"));
                baseList.Add(new ClientStyleItem(
                        typeof(ExcelGridView),
                        "Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelGridView.css.ExcelGridView.css",
                        "ux/extensions/extnet/ExcelGridView.css"));
                return baseList;
            }
        }
    }
}
