using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningStatView : View
    {
        private readonly IForecastExtension extension;
        private PlanningStatGridControl statGrid;
        
        public PlanningStatView(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public void Initialize(int varId)
        {
            statGrid = new PlanningStatGridControl(extension, String.Format("planningForm_{0}", varId));
        }

        public override List<Component> Build(ViewPage page)
        {
            var grid = statGrid.Build(page);
            return grid;
        }
    }
}
