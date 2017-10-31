using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastVariantsRepository : ForecastRepositiory<D_Forecast_PlanningVariants>, IForecastVariantsRepository
    {
        public ForecastVariantsRepository(ILinqRepository<D_Forecast_PlanningVariants> repository)
            : base(repository)
        {
        }

        public IList<D_Forecast_PlanningVariants> GetAllVariants()
        {
            return Repository.FindAll().ToList();
        }
    }
}
