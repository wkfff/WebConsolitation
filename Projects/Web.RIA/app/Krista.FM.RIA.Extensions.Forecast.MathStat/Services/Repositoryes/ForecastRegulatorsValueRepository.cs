using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastRegulatorsValueRepository : ForecastRepositiory<D_Forecast_RegValues>, IForecastRegulatorsValueRepository
    {
        public ForecastRegulatorsValueRepository(ILinqRepository<D_Forecast_RegValues> repository) : base(repository)
        {
        }

        public IList<D_Forecast_RegValues> GetAllValues()
        {
            return Repository.FindAll().ToList();
        }
    }
}
