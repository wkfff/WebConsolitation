using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningRegulatorsView : View
    {
        private PlanningRegulatorsControl grid;

        public PlanningRegulatorsView(IForecastRegulatorsValueRepository regulatorsValueRepository)
        {
            grid = new PlanningRegulatorsControl(regulatorsValueRepository);
        }

        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            layout.North.Items.Add(grid.Build(page));
            layout.North.Collapsible = false;

            layout.Center.Items.Add(grid.ValuesGrid(page));
            layout.Center.Collapsible = false;

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "center" };
            viewport.Items.Add(layout);

            ////grid.Store.DataBinding += StoreDataBinding;

            return new List<Component> { viewport };
        }
    }
}
