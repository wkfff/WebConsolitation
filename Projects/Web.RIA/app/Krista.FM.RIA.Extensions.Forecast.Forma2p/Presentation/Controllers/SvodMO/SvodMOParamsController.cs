using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOParamsController : SchemeBoundController
    {
        private IForecastSvodMOParamsMORepository paramsRepository;

        public SvodMOParamsController(IForecastSvodMOParamsMORepository paramsRepository)
        {
            this.paramsRepository = paramsRepository;
        }

        public ActionResult Load()
        {
            var list = from f in paramsRepository.GetAllParams()
                       select new
                       {
                           f.ID,
                           f.Name,
                           Units = f.RefUnits.Symbol,
                           Group = f.RefOGV.Name
                       };

            return new AjaxStoreResult(list, list.Count());
        }
    }
}
