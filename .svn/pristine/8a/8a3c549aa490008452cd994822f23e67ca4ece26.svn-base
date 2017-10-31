using System.ComponentModel;
using System.Web.UI;
using Ext.Net;

[assembly: WebResource("/Content/js/TreeGrid.packed.js", "text/javascript")]
[assembly: WebResource("/Content/js/RegMaxXomponents.js", "text/javascript")]

namespace Krista.FM.RIA.Core.ExtMaxExtensions
{
    public class GridView : Ext.Net.GridView
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
            get { return "Ext.ux.maximgb.tg.GridView"; }
        }
    }
}
