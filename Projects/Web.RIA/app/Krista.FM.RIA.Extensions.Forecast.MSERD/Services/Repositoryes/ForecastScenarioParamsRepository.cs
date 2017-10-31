using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ForecastScenarioParamsRepository : ForecastRepositiory<D_Forecast_Parametrs>, IForecastScenarioParamsRepository
    {
        public ForecastScenarioParamsRepository(ILinqRepository<D_Forecast_Parametrs> repository)
            : base(repository)
        {
        }

        public IList<D_Forecast_Parametrs> GetAllParams()
        {
            return Repository.FindAll().ToList();
        }
    }
}