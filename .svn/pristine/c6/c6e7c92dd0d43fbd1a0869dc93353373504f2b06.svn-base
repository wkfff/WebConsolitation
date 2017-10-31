using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastSvodMOParamsMORepository : ForecastRepositiory<D_Forecast_ParamsMO>, IForecastSvodMOParamsMORepository
    {
        public ForecastSvodMOParamsMORepository(ILinqRepository<D_Forecast_ParamsMO> repository) : base(repository)
        {
        }

        public IList<D_Forecast_ParamsMO> GetAllParams()
        {
            return Repository.FindAll().ToList();
        }
    }
}