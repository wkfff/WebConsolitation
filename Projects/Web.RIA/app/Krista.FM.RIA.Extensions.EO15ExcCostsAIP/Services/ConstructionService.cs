using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services
{
    public class ConstructionService : IConstructionService
    {
        private readonly ILinqRepository<D_ExcCosts_CObject> constrRepository;

        public ConstructionService(ILinqRepository<D_ExcCosts_CObject> constrRepository)
        {
            this.constrRepository = constrRepository;
        }

        public IEnumerable<D_ExcCosts_CObject> FindAll()
        {
            return constrRepository.FindAll();
        }

        public IEnumerable<D_ExcCosts_CObject> GetByFilter(List<int> filters, int? year, int? clientId, int? programId, int? regionId)
        {
            return FindAll().Where(x =>
                           !filters.Contains(x.RefStat.ID) &&
                           (year == null || (x.StartConstruction != null && x.StartConstruction.Value.Year <= year && x.EndConstruction != null && x.EndConstruction.Value.Year >= year)) &&
                           (clientId == null || x.RefClients.ID == clientId) &&
                           (regionId == null || x.RefTerritory.ID == regionId) && 
                           (programId == null || x.RefListPrg.ID == programId)).ToList();
        }

        public D_ExcCosts_CObject GetOne(int id)
        {
            return constrRepository.FindOne(id);
        }

        public void Delete(D_ExcCosts_CObject cobj)
        {
            constrRepository.Delete(cobj);
        }

        public void Save(D_ExcCosts_CObject cobj)
        {
            constrRepository.Save(cobj);
        }
    }
}
