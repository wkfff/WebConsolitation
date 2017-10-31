using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastReverseValuesRepository : ForecastRepositiory<D_Forecast_RevValues>, IForecastReverseValuesRepository
    {
        public ForecastReverseValuesRepository(ILinqRepository<D_Forecast_RevValues> repository)
            : base(repository)
        {
        }

        public IList<D_Forecast_RevValues> GetAllValues()
        {
            return Repository.FindAll().ToList();
        }
    }
}