using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.Consolidation.Models;

namespace Krista.FM.RIA.Extensions.Consolidation.Data
{
    public class CollectingPeriodRepository : ICollectingPeriodRepository
    {
        private readonly ILinqRepository<D_CD_Period> periodRepository;

        public CollectingPeriodRepository(ILinqRepository<D_CD_Period> periodRepository)
        {
            this.periodRepository = periodRepository;
        }

        public IEnumerable<CollectingPeriodPresentation> GetAllPeriods()
        {
            return periodRepository.FindAll().Select(p => new CollectingPeriodPresentation
            {
                Id = p.ID,
                Name = p.Name,
                Kind = p.RefRepKind.Name
            });
        }
    }
}
