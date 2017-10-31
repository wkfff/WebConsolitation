using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastSvodMOVarsMORepository : ForecastRepositiory<F_Forecast_VariantsMO>, IForecastSvodMOVarsMORepository
    {
        public ForecastSvodMOVarsMORepository(ILinqRepository<F_Forecast_VariantsMO> repository) : base(repository)
        {
        }

        public IList<F_Forecast_VariantsMO> GetAllVars()
        {
            return Repository.FindAll().ToList();
        }
    }
}