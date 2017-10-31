using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Xml.Serialization;
using Ext.Net;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core.ExtMaxExtensions
{
    public class GridPanel : Ext.Net.GridPanel
    {
        public GridPanel(Page page)
        {
            Ext.Net.ResourceManager.GetInstance(page).RegisterStyle(
                 "MaximGB.TreeGrid.Style", "/Content/css/TreeGrid.css");
            Ext.Net.ResourceManager.GetInstance(page).RegisterStyle(
                 "MaximGB.TreeGridLevels.Style", "/Content/css/TreeGrid.css");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript(
                  "MaximGB.TreeGrid", "/Krista.FM.RIA.Core/ExtMaxExtensions/js/TreeGrid.js/extention.axd");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript(
                        "MaximGB.FixFilter", "/Krista.FM.RIA.Core/ExtMaxExtensions/js/MaxgbFixFilter.js/extention.axd");
            Listeners.AddListerer("FilterUpdate", "applyFilter = true");
        }

        public override string InstanceOf
        {
            get { return XType; }
        }

        public override string XType
        {
            get { return "Ext.ux.maximgb.tg.GridPanel"; }
        }

        [DefaultValue("NAME")]
        public virtual string MasterColumnId
        {
            get; set;
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

                list.Add("master_column_id", new ConfigOption("master_column_id", null, string.Empty, this.MasterColumnId));

                return list;
            }
        }

        public new abstract class Config : Ext.Net.GridPanel.Config
        {
            private object masterColumnId;

            [DefaultValue("")]
            public virtual object MasterColumnId
            {
                get { return masterColumnId; }
                set { masterColumnId = value; }
            }
        }
    }
}
