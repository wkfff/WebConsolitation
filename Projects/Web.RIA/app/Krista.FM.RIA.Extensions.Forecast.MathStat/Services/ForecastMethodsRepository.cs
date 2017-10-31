﻿using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastMethodsRepository : ForecastRepositiory<D_Forecast_PlanningMethods>, IForecastMethodsRepository
    {
        public ForecastMethodsRepository(ILinqRepository<D_Forecast_PlanningMethods> repository) 
            : base(repository)
        {
        }

        public IList<D_Forecast_PlanningMethods> GetAllMethods()
        {
            return Repository.FindAll().ToList();
        }
    }
}