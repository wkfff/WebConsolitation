using System;
using System.Security;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Helpers;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Servises;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Presentation.Controllers
{
    public class AreasController : SchemeBoundController
    {
        private readonly IAreaService service;
        private readonly IUserCredentials userCredentials;

        public AreasController(
                               IAreaService service,
                               IUserCredentials userCredentials)
        {
            this.service = service;
            this.userCredentials = userCredentials;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult GetAreasTable(bool[] projStatusFilters)
        {
            bool filterEdit = projStatusFilters[0];
            bool filterReview = projStatusFilters[1];
            bool filterAccepted = projStatusFilters[2];
            
            var data = service.GetAreasTable(filterEdit, filterReview, filterAccepted, userCredentials);
            return new RestResult { Success = true, Data = data };
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        public RestResult DeleteArea(int id)
        {
            var result = new RestResult();
            try
            {
                service.DeleteProject(id, userCredentials);
                return new RestResult { Success = true, Message = "Карточка удалена." };
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = String.Format("Ошибка удаления: {0}", e.Message);
                return result;
            }
        }
    }
}
