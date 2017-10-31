using System.Collections.Generic;

using Krista.FM.RIA.Extensions.Consolidation.Models;

namespace Krista.FM.RIA.Extensions.Consolidation.Data
{
    public interface ICollectingPeriodRepository
    {
        IEnumerable<CollectingPeriodPresentation> GetAllPeriods();
    }
}