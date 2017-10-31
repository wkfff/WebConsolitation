using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningParamsController : SchemeBoundController
    {
        private readonly IForecastParamsRepository paramsRepository;

        public PlanningParamsController(IForecastParamsRepository paramsRepository)
        {
            this.paramsRepository = paramsRepository;
        }

        public ActionResult Load()
        {
            var view = from f in paramsRepository.GetAllParams()
                       where f.ParentID != null
                       select new
                       {
                           f.ID,
                           f.Name,
                           f.XMLString,
                           refOKEI = f.RefOKEI.Symbol,
                           f.Prognose,
                           f.Form2P,
                           f.ParentID,
                           grp = (from v in paramsRepository.GetAllParams()
                                 where (v.ID == f.ParentID)
                                 select v.Name).SingleOrDefault()
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult LoadListParams()
        {
            var view = from f in paramsRepository.GetAllParams()
                       where f.ParentID != null
                       select new
                       {
                           f.ID,
                           f.Name,
                           grp = (from v in paramsRepository.GetAllParams()
                                  where (v.ID == f.ParentID)
                                  select v.Name).SingleOrDefault()
                       };

            return new AjaxStoreResult(view, view.Count());
        }
    }
}
