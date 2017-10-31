using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ForecastIdicPlanRepository : ForecastRepositiory<D_Forecast_IdicPlan>, IForecastIdicPlanRepository
    {
        public ForecastIdicPlanRepository(ILinqRepository<D_Forecast_IdicPlan> repository) 
            : base(repository)
        {
        }

        public IList<D_Forecast_IdicPlan> GetAllIdicPlan()
        {
            return Repository.FindAll().ToList();
        }
    }
}