using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ScenarioParamView : View
    {
        private ScenarioParamControl grid;
        private int scenId;
        private int baseyear;
        
        public ScenarioParamView()
        {
            grid = new ScenarioParamControl();
        }

        public void Initialize(int scenId, int baseyear)
        {
            this.scenId = scenId;
            this.baseyear = baseyear;
            grid.ScenId = scenId;
            grid.BaseYear = baseyear;
        }
        
        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            layout.North.Items.Add(grid.CreateToolBar()); // panel inside

            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false
            };
            
            panel.Items.Add(grid.Build(page));

            layout.Center.Items.Add(panel);

            ////layout.South.Items.Add(grid.CreateIndicators(page));

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
            viewport.Items.Add(layout);
            
            return new List<Component> { viewport };
        }
    }
}
