using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOView : View
    {
        private readonly IForecastSvodMOParamsMORepository paramsRepository;
        private readonly SvodMOParamsGridControl grid;

        public SvodMOView(IForecastSvodMOParamsMORepository paramsRepository)
        {
            this.paramsRepository = paramsRepository;
            grid = new SvodMOParamsGridControl();
        }

        public override List<Component> Build(ViewPage page)
        {
         BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            ////layout.North.Items.Add(grid.TopPanel(page)); // panel inside

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
