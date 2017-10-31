using System.ComponentModel;

namespace Krista.FM.RIA.Core.ExtNet.Extensions.TreeGridPanel
{
    /* GridView для иерархического представления */
    public class TreeGridView : Ext.Net.GridView
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
            get
            {
                return "Ext.ux.maximgb.tg.GridView";
            }
        }
    }
}
