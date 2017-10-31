using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ValuationParamView : View
    {
        private ValuationParamControl grid;
        private int scenId;
        private string key;

        private IForecastExtension extension;

        public ValuationParamView(IForecastExtension extension)
        {
            this.extension = extension;
            grid = new ValuationParamControl(extension);
        }

        public void Initialize(int scenId, int baseId, int baseYear)
        {
            this.scenId = scenId;
            grid.ScenId = scenId;
            grid.BaseYear = baseYear;
            grid.RefScenId = baseId;
            
            key = "valuation_{0}".FormatWith(scenId);
            if (extension.Forms.ContainsKey(key))
            {
                extension.Forms.Remove(key);
            }
            
            extension.Forms.Add(key, new UserFormsControls());
            grid.Initialize(key);
        }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            layout.North.Items.Add(grid.CreateToolBar()); // panel inside

            /*Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false
            };*/

            ////panel.Items.Add(grid.Build(page));

            layout.Center.Items.Add(grid.Build(page));

            ////layout.South.Items.Add(grid.CreateIndicators(page));

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
            viewport.Items.Add(layout);

            return new List<Component> { viewport };
        }
    }
}
