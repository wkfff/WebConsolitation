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
    public class ValuationController : SchemeBoundController
    {
        private readonly IForecastScenarioVarsRepository varsRepository;
        private readonly IForecastExtension extension;
        
        public ValuationController(IForecastScenarioVarsRepository varsRepository, IForecastExtension extension)
        {
            this.varsRepository = varsRepository;
            this.extension = extension;
        }

        public ActionResult Load()
        {
            var list = from f in varsRepository.GetAllVars()
                       where f.Parent.HasValue
                       select new
                       {
                           f.ID,
                           f.Name,
                           ////f.PercOfComplete,
                           ////f.ReadyToCalc,
                           Year = f.RefYear.ID,
                           ////Status = GetStatus(f.ReadyToCalc),
                           User = Scheme.UsersManager.GetUserNameByID(f.UserID)
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult LoadParentScen()
        {
            var list = from f in varsRepository.GetAllVars()
                       where (!f.Parent.HasValue) && (f.ReadyToCalc == 2)
                       select new
                       {
                           Value = f.ID,
                           Text = f.Name,
                           ////f.PercOfComplete,
                           ////f.ReadyToCalc,
                           ////Status = GetStatus(f.ReadyToCalc),
                       };

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult Calc(int varid)
        {
            AjaxResult ar = new AjaxResult();

            if (varid != -1)
            {
                ar.Result = "success";

                using (new ServerContext())
                {
                    Scheme.ForecastService.CalcModel(varid);
                }
            }
            else
            {
                ar.Result = "failure";
            }

            return ar;
        }

        public ActionResult NewValuation(int parentId, string name)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "failure";

            var parent = varsRepository.FindOne(parentId);

            F_Forecast_Scenario forecastScenario = new F_Forecast_Scenario
            {
                Parent = parentId,
                Name = name,
                Period = parent.Period,
                RefYear = parent.RefYear,
                SourceID = parent.SourceID,
                UserID = extension.UserID
            };

            varsRepository.Save(forecastScenario);
            varsRepository.DbContext.CommitChanges();

            var scenId = forecastScenario.ID;

            using (new ServerContext())
            {
                Scheme.ForecastService.CopyScenarioDetails(parentId, scenId, null);
                ar.Result = "success";
            }

            return ar;
        }

       /* public ActionResult SetReadyToCalc(int varid)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "success";

            using (new ServerContext())
            {
                Scheme.ForecastService.SetScenarioStatus(varid, ScenarioStatus.ReadyToCalc);
            }

            return ar;
        }*/

        /*public ActionResult NewScenario(string name, int year)
        {
            AjaxResult ar = new AjaxResult();

            return ar;
        }*/

        /*private string GetStatus(int status)
        {
            if (status == 0)
            {
                return "Новый";
            }

            if (status == 1)
            {
                return "Готов к рассчету";
            }

            if (status == 2)
            {
                return "Рассичтан";
            }

            if (status == -1)
            {
                return "Базовый сценарий";
            }

            return "Статус не определен";
        }*/
    }
}
