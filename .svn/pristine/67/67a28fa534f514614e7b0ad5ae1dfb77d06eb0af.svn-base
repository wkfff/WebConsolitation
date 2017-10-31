using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary.Forecast;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ScenarioController : SchemeBoundController
    {
        private readonly IForecastScenarioVarsRepository varsRepository;
        private readonly ILinqRepository<FX_Date_Year> dataYearRepository;
        private readonly ILinqRepository<DataSources> datasourceRepository;
        private readonly IForecastExtension extension;

        public ScenarioController(
                                  IForecastScenarioVarsRepository varsRepository,
                                  ILinqRepository<FX_Date_Year> dataYearRepository,
                                  ILinqRepository<DataSources> datasourceRepository,
                                  IForecastExtension extension)
        {
            this.varsRepository = varsRepository;
            this.dataYearRepository = dataYearRepository;
            this.datasourceRepository = datasourceRepository;
            this.extension = extension;
        }
        
        public ActionResult Load()
        {
            var list = from f in varsRepository.GetAllVars()
                       where !f.Parent.HasValue
                       select new
                       {
                           f.ID,
                           f.Name,
                           f.PercOfComplete,
                           f.ReadyToCalc,
                           Year = f.RefYear.ID,
                           Status = GetStatus(f.ReadyToCalc),
                           User = Scheme.UsersManager.GetUserNameByID(f.UserID)
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

        public ActionResult SetReadyToCalc(int varid)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "success";

            using (new ServerContext())
            {
                Scheme.ForecastService.SetScenarioStatus(varid, ScenarioStatus.ReadyToCalc);
            }

            return ar;
        }

        public ActionResult NewScenario(string name, int year)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "failure";

            FX_Date_Year refYear = (from f in dataYearRepository.FindAll()
                                 where f.ID == year
                                 select f).First();

            DataSources dataSources = (from f in datasourceRepository.FindAll()
                                       where (f.DataCode == 2) && (f.SupplierCode == "ЭО") && (f.DataName == "Прогноз") && (f.Year == year.ToString())
                                       select f).First();
            
            F_Forecast_Scenario forecastScenario = new F_Forecast_Scenario
            {
                Name = name,
                Period = 5,
                RefYear = refYear,
                SourceID = dataSources.ID,
                UserID = extension.UserID
            };

            varsRepository.Save(forecastScenario);
            varsRepository.DbContext.CommitChanges();

            var scenId = forecastScenario.ID;

            var list = (from f in varsRepository.GetAllVars()
                       where f.ReadyToCalc == -1
                       select new
                       {
                           f.ID
                       }).ToList().First();

            var parentId = list.ID;

            using (new ServerContext())
            {
                Scheme.ForecastService.CopyScenarioDetails(parentId, scenId, null);
                ar.Result = "success";
            }

            return ar;
        }

        private string GetStatus(int status)
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
                return "Рассчитан";
            }

            if (status == -1)
            {
                return "Базовый сценарий";
            }

            return "Статус не определен";
        }
    }
}
