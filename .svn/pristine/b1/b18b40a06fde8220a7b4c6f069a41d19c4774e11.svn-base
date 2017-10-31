using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services
{
    public interface IConstructionService
    {
        IEnumerable<D_ExcCosts_CObject> FindAll();

        IEnumerable<D_ExcCosts_CObject> GetByFilter(List<int> filters, int? year, int? clientId, int? programId, int? regionId);

        D_ExcCosts_CObject GetOne(int id);

        void Delete(D_ExcCosts_CObject cobj);

        void Save(D_ExcCosts_CObject cobj);
    }
}
