using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public interface IForecastParamsRepository : ILinqRepository<D_Forecast_PlanningParams>
    {
        IList<D_Forecast_PlanningParams> GetAllParams();
    }

    public interface IForecastVariantsRepository : ILinqRepository<D_Forecast_PlanningVariants>
    {
        IList<D_Forecast_PlanningVariants> GetAllVariants();
    }

    public interface IForecastValuesRepository : ILinqRepository<D_Forecast_PlanningValues>
    {
        IList<D_Forecast_PlanningValues> GetAllValue();
    }

    public interface IForecastMethodsRepository : ILinqRepository<D_Forecast_PlanningMethods>
    {
        IList<D_Forecast_PlanningMethods> GetAllMethods();
    }
}
