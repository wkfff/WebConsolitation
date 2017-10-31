using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastIdicIndsRepository : ForecastRepositiory<D_Forecast_IdicInds>, IForecastIdicIndsRepository
    {
        public ForecastIdicIndsRepository(ILinqRepository<D_Forecast_IdicInds> repository) : base(repository)
        {
        }

        public IList<D_Forecast_IdicInds> GetAllIdicInds()
        {
            return Repository.FindAll().ToList();
        }
    }
}