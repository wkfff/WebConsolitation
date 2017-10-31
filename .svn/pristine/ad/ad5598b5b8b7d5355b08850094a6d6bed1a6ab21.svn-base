using System.ComponentModel;

namespace Krista.FM.RIA.Core.ExtNet.Extensions.TreeGridPanel
{
    /* PagingToolbar для иерархического представления */
    public class TreeGridPagingToolbar : Ext.Net.PagingToolbar
    {
        public override string XType
        {
            get
            {
                return "Ext.ux.maximgb.tg.PagingToolbar";
            }
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

        public virtual void UpdateInfo()
        {
            Call("updateInfo");
        }

        public virtual void MoveFirst()
        {
            Call("moveFirst");
        }

        public virtual void MovePrevious()
        {
            Call("movePrevious");
        }

        public virtual void MoveNext()
        {
            Call("moveNext");
        }

        public virtual void MoveLast()
        {
            Call("moveLast");
        }
    }
}
