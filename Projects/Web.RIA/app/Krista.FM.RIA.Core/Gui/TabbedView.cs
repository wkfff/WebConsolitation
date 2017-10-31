using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;

namespace Krista.FM.RIA.Core.Gui
{
    public class TabbedView : Control
    {
        public TabbedView()
        {
            Tabs = new List<Control>();
        }

        public List<Control> Tabs { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            TabPanel tabPanel = new TabPanel { ID = "tabPanel", Border = false };

            foreach (Control tabItem in Tabs)
            {
                tabPanel.Items.Add(tabItem.Build(page));
            }

            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            layout.Center.Items.Add(tabPanel);

            Viewport viewport = new Viewport { ID = "viewportMain" };
            viewport.Items.Add(layout);

            return new List<Component> { viewport };
        }
    }
}
