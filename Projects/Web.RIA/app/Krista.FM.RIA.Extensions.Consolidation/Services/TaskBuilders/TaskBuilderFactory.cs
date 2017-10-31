using System;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class TaskBuilderFactory : ITaskBuilderFactory
    {
        public ITaskBuilder CreateBuilder(FX_FX_Periodicity periodicity)
        {
            switch (periodicity.ID)
            {
                case 1:
                    return Resolver.Get<YearTaskBuilder>();
                case 2:
                    return Resolver.Get<QuarterTaskBuilder>();
                case 3:
                    return Resolver.Get<MonthTaskBuilder>();
                default:
                    throw new InvalidOperationException("{0}".FormatWith(periodicity.ID));
            }
        }
    }
}
