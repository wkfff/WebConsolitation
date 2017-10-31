using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;

namespace Krista.FM.RIA.Core.Gui
{
    public class GridView : GridModelControl
    {
        public override List<Component> Build(ViewPage page)
        {
            Viewport viewport = new Viewport();
            BorderLayout borderLayout = new BorderLayout();
            borderLayout.Center.Items.Add(base.Build(page));
            viewport.Items.Add(borderLayout);

            return new List<Component> { viewport };
        }
    }
}
