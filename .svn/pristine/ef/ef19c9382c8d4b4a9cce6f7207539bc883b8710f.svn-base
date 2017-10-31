using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastReverseRepository : ForecastRepositiory<D_Forecast_RevPlan>, IForecastReverseRepository
    {
        public ForecastReverseRepository(ILinqRepository<D_Forecast_RevPlan> repository) 
            : base(repository)
        {
        }

        public IList<D_Forecast_RevPlan> GetAllVariants()
        {
            return Repository.FindAll().ToList();
        }
    }
}