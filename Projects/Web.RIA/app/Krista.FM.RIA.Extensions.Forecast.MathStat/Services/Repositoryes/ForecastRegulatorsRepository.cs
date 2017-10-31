using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastRegulatorsRepository : ForecastRepositiory<D_Forecast_Regs>, IForecastRegulatorsRepository
    {
        public ForecastRegulatorsRepository(ILinqRepository<D_Forecast_Regs> repository) : base(repository)
        {
        }

        public IList<D_Forecast_Regs> GetAllRegulators()
        {
            return Repository.FindAll().ToList();
        }
    }
}
