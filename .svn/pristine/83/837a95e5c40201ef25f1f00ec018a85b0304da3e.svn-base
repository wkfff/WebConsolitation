using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastValuesRepository : ForecastRepositiory<D_Forecast_PValues>, IForecastValuesRepository
    {
        public ForecastValuesRepository(ILinqRepository<D_Forecast_PValues> repository) 
            : base(repository)
        {
        }

        public IList<D_Forecast_PValues> GetAllValues()
        {
            return Repository.FindAll().ToList();
        }
    }
}
