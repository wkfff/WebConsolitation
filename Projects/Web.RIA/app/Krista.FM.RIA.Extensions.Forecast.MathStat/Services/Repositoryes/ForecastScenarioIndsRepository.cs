using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastScenarioIndsRepository : ForecastRepositiory<T_Forecast_IndValues>, IForecastScenarioIndsRepository
    {
        public ForecastScenarioIndsRepository(ILinqRepository<T_Forecast_IndValues> repository)
            : base(repository)
        {
        }

        public IList<T_Forecast_IndValues> GetAllInds()
        {
            return Repository.FindAll().ToList();
        }
    }
}