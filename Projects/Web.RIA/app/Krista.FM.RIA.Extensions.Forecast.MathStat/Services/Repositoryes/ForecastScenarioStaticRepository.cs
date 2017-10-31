using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastScenarioStaticRepository : ForecastRepositiory<T_Forecast_StaticValues>, IForecastScenarioStaticRepository
    {
        public ForecastScenarioStaticRepository(ILinqRepository<T_Forecast_StaticValues> repository)
            : base(repository)
        {
        }

        public IList<T_Forecast_StaticValues> GetAllStatic()
        {
            return Repository.FindAll().ToList();
        }
    }
}