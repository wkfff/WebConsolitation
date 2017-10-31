using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services
{
    public interface IClientService
    {
        IEnumerable<D_ExcCosts_Clients> GetAll();

        D_ExcCosts_Clients Get(int id);
    }
}
