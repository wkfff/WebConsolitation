using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ForecastIdicAdjsRepository : ForecastRepositiory<D_Forecast_IdicAdjs>, IForecastIdicAdjsRepository
    {
        public ForecastIdicAdjsRepository(ILinqRepository<D_Forecast_IdicAdjs> repository) 
            : base(repository)
        {
        }

        public IList<D_Forecast_IdicAdjs> GetAllIdicAdjs()
        {
            return Repository.FindAll().ToList();
        }
    }
}