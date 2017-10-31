using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Xml.Serialization;
using Ext.Net;
using Newtonsoft.Json;

[assembly: WebResource("Krista.FM.RIA.Core.ExtMaxExtensions.js.TreeGrid.js", "text/javascript")]
[assembly: WebResource("Krista.FM.RIA.Core.ExtMaxExtensions.css.TreeGrid.css", "text/javascript")]
[assembly: WebResource("Krista.FM.RIA.Core.ExtMaxExtensions.css.TreeGridLevels.css", "text/javascript")]

namespace Krista.FM.RIA.Core.ExtMaxExtensions
{
    public abstract class AbstractTreeStore : Store
    {
        /// <summary>
        /// is_leaf_field_name Record leaf flag field name.
        /// </summary>
        [Description("")]
        public const string LeafFieldName = "_is_leaf";

  /*      /// <summary>
        /// Current page offset.
        /// </summary>
        [Description("")]
        private int pageOffset = 0;*/

        public AbstractTreeStore(Page page)
        {
        }

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
            get { return "Ext.ux.maximgb.tg.AbstractTreeStore"; }
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
                list.Add("leaf_field_name", null, LeafFieldName);
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
                        typeof(EditorGridPanel),
                        "Krista.FM.RIA.Core.ExtMaxExtensions.js.TreeGrid.js",
                        "ux/extensions/maximgb/TreeGrid.js"));
                baseList.Add(new ClientStyleItem(
                        typeof(AbstractTreeStore),
                        "Krista.FM.RIA.Core.ExtMaxExtensions.css.TreeGrid.css",
                        "ux/extensions/maximgb/TreeGrid.css"));
                baseList.Add(new ClientStyleItem(
                        typeof(AbstractTreeStore),
                        "Krista.FM.RIA.Core.ExtMaxExtensions.css.TreeGridLevels.css",
                        "ux/extensions/maximgb/TreeGridLevels.css"));
                /*                baseList.Add(new ClientStyleItem(
                                        typeof(AbstractTreeStoreH),
                                        "Krista.FM.RIA.Core.ExtMaxExtensions.css.TreeGridLevels.css",
                                        "ux/extensions/maximgb/TreeGrid.js"));*/
                return baseList;
            }
        }

        public virtual void Remove(string record)
        {
            Call("remove", new JRawValue(JSON.Serialize(record)));
        }

        public virtual void ApplyTreeSort()
        {
            Call("applyTreeSort");
        }

        public virtual void CollapseNode(string record)
        {
            Call("collapseNode", new JRawValue(JSON.Serialize(record)));
        }

        public void ExpandNode(string record)
        {
            Call("expandNode", new JRawValue(JSON.Serialize(record)));
        }

        public virtual void ExpandAll()
        {
            Call("expandAll");
        }

        public virtual void CollapseAll()
        {
            Call("collapseAll");
        }

        public virtual void AddToNode(string parent, string child)
        {
            Call("addToNode", new JRawValue(JSON.Serialize(parent)), new JRawValue(JSON.Serialize(child)));
        }

        public virtual void RemoveFromNode(string parent, string child)
        {
            Call("removeFromNode", new JRawValue(JSON.Serialize(parent)), new JRawValue(JSON.Serialize(child)));
        }
   
        [Description("")]
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
    }
}
