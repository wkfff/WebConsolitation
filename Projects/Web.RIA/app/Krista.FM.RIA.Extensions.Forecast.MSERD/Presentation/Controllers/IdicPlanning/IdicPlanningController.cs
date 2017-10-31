using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class IdicPlanningController : SchemeBoundController
    {
        private IForecastIdicPlanRepository idicPlanRepository;
        private IForecastScenarioVarsRepository scenarioVarsRepository;
        private IForecastExtension extension;

        public IdicPlanningController(
                                      IForecastIdicPlanRepository planRepository,
                                      IForecastScenarioVarsRepository scenarioVarsRepository,
                                      IForecastExtension extension)
        {
            this.idicPlanRepository = planRepository;
            this.scenarioVarsRepository = scenarioVarsRepository;
            this.extension = extension;
        }

        public ActionResult Load()
        {
            var list = from f in idicPlanRepository.GetAllIdicPlan()
                       select new
                       {
                           f.ID,
                           f.Name,
                           f.Parent,
                           User = Scheme.UsersManager.GetUserNameByID(f.UserId)
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult LoadParentScen()
        {
            var list = from f in scenarioVarsRepository.GetAllVars()
                       where f.Parent != null
                       select new
                       {
                           Value = f.ID,
                           Text = f.Name
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult NewIdicPlan(int parentId, string name)
        {
            var ar = new AjaxResult();

            D_Forecast_IdicPlan idicPlan = new D_Forecast_IdicPlan
            {
                Name = name,
                Parent = parentId,
                UserId = extension.UserID
            };

            idicPlanRepository.Save(idicPlan);
            idicPlanRepository.DbContext.CommitChanges();
            ar.Result = "success";

            return ar;
        }
    }
}
