using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastScenarioUnRegsRepository : ForecastRepositiory<T_Forecast_UnRegAdj>, IForecastScenarioUnRegsRepository
    {
        public ForecastScenarioUnRegsRepository(ILinqRepository<T_Forecast_UnRegAdj> repository) : base(repository)
        {
        }

        public IList<T_Forecast_UnRegAdj> GetAllUnRegs()
        {
            return Repository.FindAll().ToList();
        }
    }
}