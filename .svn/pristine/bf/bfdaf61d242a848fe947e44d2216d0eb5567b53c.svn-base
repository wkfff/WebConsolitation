using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastSvodMOValuesMORepository : ForecastRepositiory<T_Forecast_ValuesMO>, IForecastSvodMOValuesMORepository
    {
        public ForecastSvodMOValuesMORepository(ILinqRepository<T_Forecast_ValuesMO> repository) : base(repository)
        {
        }
        
        public IList<T_Forecast_ValuesMO> GetAllValues()
        {
            return Repository.FindAll().ToList();
        }
    }
}