using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.PfhdService
{
    public interface IPfhdService : INewRestService
    {
        IEnumerable<F_Fin_finActPlan> GetItems(int? parentId);

        void Delete(int id);
    }
}
