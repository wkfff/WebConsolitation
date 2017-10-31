using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.E86N.Services.RestfulService
{
    public interface ICacheRepositoryService
    {
        ILinqRepository<TDomain> GetRepository<TDomain>() where TDomain : DomainObject;

        void ClearCash();
    }
}