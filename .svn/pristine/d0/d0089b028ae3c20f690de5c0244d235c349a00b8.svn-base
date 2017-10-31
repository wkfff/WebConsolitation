using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningCritsView : View
    {
        private readonly IForecastExtension extension;
        private PlanningCritsControl crits;

        public PlanningCritsView(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public void Initialize(int varId)
        {
            crits = new PlanningCritsControl(extension, String.Format("planningForm_{0}", varId));
        }

        public override List<Component> Build(ViewPage page)
        {
            return crits.Build(page);
        }
    }
}
