using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastSvodMOVarsOIVRepository : ForecastRepositiory<F_Forecast_VariantsOIV>, IForecastSvodMOVarsOIVRepository
    {
        public ForecastSvodMOVarsOIVRepository(ILinqRepository<F_Forecast_VariantsOIV> repository) : base(repository)
        {
        }

        public IList<F_Forecast_VariantsOIV> GetAllVars()
        {
            return Repository.FindAll().ToList();
        }
    }
}