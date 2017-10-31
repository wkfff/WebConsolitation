using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastSvodMOValuesOIVRepository : ForecastRepositiory<T_Forecast_ValuesOIV>, IForecastSvodMOValuesOIVRepository
    {
        public ForecastSvodMOValuesOIVRepository(ILinqRepository<T_Forecast_ValuesOIV> repository) : base(repository)
        {
        }

        public IList<T_Forecast_ValuesOIV> GetAllValues()
        {
            return Repository.FindAll().ToList();
        }
    }
}