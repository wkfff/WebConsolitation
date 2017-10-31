using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class IdicPlanningView : View
    {
        private IdicPlanningControl grid;

        public IdicPlanningView()
        {
            grid = new IdicPlanningControl();
        }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            layout.North.Items.Add(grid.ToolBar()); // panel inside

            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false,
                Width = layout.Width,
                Height = layout.Height,
                Layout = "fit"
            };

            panel.Items.Add(grid.Build(page));

            layout.Center.Items.Add(panel);
            
            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
            viewport.Items.Add(layout);
            
            return new List<Component> { viewport };
        }
    }
}
