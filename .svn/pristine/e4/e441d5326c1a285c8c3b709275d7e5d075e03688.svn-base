using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastParamsRepository : ForecastRepositiory<D_Forecast_PlanningParams>, IForecastParamsRepository
    {
        public ForecastParamsRepository(ILinqRepository<D_Forecast_PlanningParams> repository)
            : base(repository)
        {
        }

        public IList<D_Forecast_PlanningParams> GetAllParams()
        {
            return Repository.FindAll().ToList();
        }
    }
}
