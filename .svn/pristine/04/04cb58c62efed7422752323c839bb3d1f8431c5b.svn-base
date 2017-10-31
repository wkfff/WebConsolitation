using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastSvodMOVarsMERRepository : ForecastRepositiory<F_Forecast_VariantsMER>, IForecastSvodMOVarsMERRepository
    {
        public ForecastSvodMOVarsMERRepository(ILinqRepository<F_Forecast_VariantsMER> repository) : base(repository)
        {
        }

        public IList<F_Forecast_VariantsMER> GetAllVars()
        {
            return Repository.FindAll().ToList();
        }
    }
}