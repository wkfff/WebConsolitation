using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastSvodMOValuesMERRepository : ForecastRepositiory<T_Forecast_ValuesMER>, IForecastSvodMOValuesMERRepository
    {
        public ForecastSvodMOValuesMERRepository(ILinqRepository<T_Forecast_ValuesMER> repository) : base(repository)
        {
        }

        public IList<T_Forecast_ValuesMER> GetAllValues()
        {
            return Repository.FindAll().ToList();
        }
    }
}