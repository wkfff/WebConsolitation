using System.ComponentModel;
using System.Web.UI;
using System.Xml.Serialization;
using Ext.Net;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core.ExtMaxExtensions
{
    public class AdjacencyListStore : AbstractTreeStore
    {
        public AdjacencyListStore(Page page)
            : base(page)
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
            get { return "Ext.ux.maximgb.tg.AdjacencyListStore"; }
        }

        public virtual string ParentIdFieldName { get; set; }

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
                list.Add("parent_id_field_name", null, ParentIdFieldName);
                return list;
            }
        }
        
        public new abstract class Config : Store.Config
        {
            private object parentIdFieldName = null;

            [DefaultValue("PARENTID")]
            public virtual object Parent_id_field_name
            {
                get
                {
                    return this.parentIdFieldName;
                }

                set
                {
                    this.parentIdFieldName = value;
                }
            }
        }
    }
}
