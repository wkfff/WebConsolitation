using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningCritsController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastExtension extension;

        public PlanningCritsController(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public ActionResult Show(int id)
        {
            var viewControl = Resolver.Get<PlanningCritsView>();

            viewControl.Initialize(id);

            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult LoadCrits(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            List<Criteria> lstCrit = ufc.DataService.GetCriteria();

            return new AjaxStoreResult(lstCrit, lstCrit.Count);
        }
    }
}
