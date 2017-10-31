using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastForma2pValueRepository : ForecastRepositiory<T_Forecast_ParamValues>, IForecastForma2pValueRepository
    {
        public ForecastForma2pValueRepository(ILinqRepository<T_Forecast_ParamValues> repository) : base(repository)
        {
        }

        public IList<T_Forecast_ParamValues> GetAllValues()
        {
            return Repository.FindAll().ToList();
        }
    }
}