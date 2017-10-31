using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services
{
    public class ClientService : NHibernateRepository<D_ExcCosts_Clients>, IClientService
    {
        public new IEnumerable<D_ExcCosts_Clients> GetAll()
        {
            return base.GetAll();
        }
    }
}
