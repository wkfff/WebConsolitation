using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ValuationView : View
    {
        private ValuationControl grid;

        public ValuationView()
        {
            grid = new ValuationControl();
        }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            layout.North.Items.Add(grid.ToolBar()); // panel inside

            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false,
                ////AutoWidth = true,
                Width = layout.Width,
                Height = layout.Height,
                ////AutoHeight = true,
                ////AutoWidth = true, 
                Layout = "fit"
            };

            panel.Items.Add(grid.Build(page));

            layout.Center.Items.Add(panel);

            ////((RowSelectionModel)grid.GridPanel.SelectionModel[0]).Listeners.RowSelect.Handler = "dsUsers.reload()";

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
            viewport.Items.Add(layout);
            
            return new List<Component> { viewport };
        }
    }
}
