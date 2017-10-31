using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastScenarioAdjsRepository : ForecastRepositiory<T_Forecast_AdjValues>, IForecastScenarioAdjsRepository
    {
        public ForecastScenarioAdjsRepository(ILinqRepository<T_Forecast_AdjValues> repository)
            : base(repository)
        {
        }

        public IList<T_Forecast_AdjValues> GetAllAdjs()
        {
            return Repository.FindAll().ToList();
        }
    }
}