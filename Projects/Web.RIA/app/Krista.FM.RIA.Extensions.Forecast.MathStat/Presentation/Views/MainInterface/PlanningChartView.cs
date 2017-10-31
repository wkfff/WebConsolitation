using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningChartView : View
    {
        private readonly IForecastExtension extension;
        private PlanningChartControl chart;

        public PlanningChartView(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public void Initialize(int varId)
        {
            chart = new PlanningChartControl(extension, String.Format("planningForm_{0}", varId));
        }

        public override List<Component> Build(ViewPage page)
        {
            return chart.Build(page);
        }
    }
}
