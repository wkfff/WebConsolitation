using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastScenarioVarsRepository : ForecastRepositiory<F_Forecast_Scenario>, IForecastScenarioVarsRepository
    {
        public ForecastScenarioVarsRepository(ILinqRepository<F_Forecast_Scenario> repository)
            : base(repository)
        {
        }

        public IList<F_Forecast_Scenario> GetAllVars()
        {
            return Repository.FindAll().ToList();
        }
    }
}