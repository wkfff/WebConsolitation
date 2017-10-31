using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ProtocolService : IProtocolService
    {
        private readonly ILinqRepository<D_CD_Protocol> protocolRepository;

        public ProtocolService(ILinqRepository<D_CD_Protocol> protocolRepository)
        {
            this.protocolRepository = protocolRepository;
        }

        public IList<D_CD_Protocol> GetRegionProtocol(int regionId)
        {
            return protocolRepository.FindAll()
                .Where(r => r.RefTask.RefSubject.ID == regionId)
                .ToList();
        }

        public IList<D_CD_Protocol> GetTaskProtocol(int taskId)
        {
            return protocolRepository.FindAll()
                .Where(r => r.RefTask.ID == taskId)
                .ToList();
        }
    }
}
