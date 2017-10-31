using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastForma2pVarRepository : ForecastRepositiory<F_Forecast_VarForm2P>, IForecastForma2pVarRepository
    {
        public ForecastForma2pVarRepository(ILinqRepository<F_Forecast_VarForm2P> repository) : base(repository)
        {
        }

        public IList<F_Forecast_VarForm2P> GetAllVariants()
        {
            return Repository.FindAll().ToList();
        }
    }
}