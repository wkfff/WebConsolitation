using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class IdicPlanParamView : View
    {
        private IdicPlanParamControl grid;

        private int varId;
        private int parentId;
        private IForecastExtension extension;
        
        public IdicPlanParamView(IForecastExtension extension)
        {
            grid = new IdicPlanParamControl();
            this.extension = extension;
        }

        public string Key { get; set; }

        public void Initialize(int varId, int parentId)
        {
            this.varId = varId;
            this.parentId = parentId;

            grid.Key = Key;
            grid.ParentId = parentId;
            grid.VarId = varId;
        }
        
        public override List<Ext.Net.Component> Build(System.Web.Mvc.ViewPage page)
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
