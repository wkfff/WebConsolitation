using System.Collections.Generic;
using Ext.Net;
using GridView = Krista.FM.RIA.Core.Gui.GridView;

namespace Krista.FM.RIA.Core.Tests.Stub.Gui
{
    public class GridViewDescendant : GridView
    {
        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var list = base.Build(page);
            list.Add(new Label("TestLabel"));
            return list;
        }
    }
}
