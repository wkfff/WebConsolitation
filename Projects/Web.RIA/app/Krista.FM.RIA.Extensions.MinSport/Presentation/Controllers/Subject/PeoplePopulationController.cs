using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controllers.Subject
{
    public class PeoplePopulationController : SchemeBoundController
    {
        private readonly ILinqRepository<T_People_Population> repPeoplePopulation;
        private readonly ILinqRepository<D_Territory_RF> repTerritory;

        public PeoplePopulationController(
            ILinqRepository<T_People_Population> repPeoplePopulation, 
            ILinqRepository<D_Territory_RF> repTerritory)
        {
            this.repPeoplePopulation = repPeoplePopulation;
            this.repTerritory = repTerritory;
        }

        [HttpGet]
        public ActionResult Read(int territoryId)
        {
            var view = from f in repPeoplePopulation.FindAll()
                       where f.Territory.ID == territoryId
                       select new
                              {
                                  f.ID,
                                  f.YearDayUNV,
                                  f.Val
                              };
            return new AjaxStoreResult(view, view.Count());
        }
    }
}
