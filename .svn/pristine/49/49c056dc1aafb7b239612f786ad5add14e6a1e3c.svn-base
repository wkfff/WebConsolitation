using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastValuesRepository : ForecastRepositiory<D_Forecast_PlanningValues>, IForecastValuesRepository
    {
        public ForecastValuesRepository(ILinqRepository<D_Forecast_PlanningValues> repository) 
            : base(repository)
        {
        }

        public IList<D_Forecast_PlanningValues> GetAllValue()
        {
            return Repository.FindAll().ToList();
        }
    }
}
