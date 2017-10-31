using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Ext.Net;

[assembly: WebResource("Krista.FM.RIA.Core.ExtNet.Extensions.BookCellSelectionModel.js.BookCellSelectionModel.js", "text/javascript")]

namespace Krista.FM.RIA.Core.ExtNet.Extensions.BookCellSelectionModel
{
    public class BookCellSelectionModel : CellSelectionModel
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
            get { return "Ext.ux.BookCellSelectionModel"; }
        }

        protected override List<ResourceItem> Resources
        {
            get
            {
                var baseList = base.Resources;
                baseList.Capacity += 1;
                baseList.Add(new ClientScriptItem(
                        typeof(BookCellSelectionModel),
                        "Krista.FM.RIA.Core.ExtNet.Extensions.BookCellSelectionModel.js.BookCellSelectionModel.js",
                        "ux/extensions/extnet/TreeGrid.js"));

                return baseList;
            }
        }
    }
}
