using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.Gui;
using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class TabbedViewTests
    {
        [Test]
        public void CanBuildTest()
        {
            TabbedView tv = new TabbedView();
            tv.Id = "tv";
            NavigationList list = new NavigationList(null);
            list.Items = new List<NavigationItem>();
            list.Items.Add(new NavigationItem { 
                ID = "n1", 
                Icon = Icon.Zoom, 
                Link = "/", 
                OrderPosition = 1
            });
            tv.Tabs.Add(list);

            ViewPage viewPage = new ViewPage();
            List<Component> components = tv.Build(viewPage);

            Assert.AreEqual(1, components.Count(x => x.ID == "viewportMain"));

            Viewport viewport = (Viewport)components.Find(x => x.ID == "viewportMain");
            Assert.AreEqual(typeof(BorderLayout), viewport.Items[0].GetType());

            BorderLayout layout = (BorderLayout)viewport.Items[0];
            Assert.AreEqual(typeof(TabPanel), layout.Center.Items[0].GetType());
        }
    }
}
