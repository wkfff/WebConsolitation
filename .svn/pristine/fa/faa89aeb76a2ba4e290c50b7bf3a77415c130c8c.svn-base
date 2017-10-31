using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.E86N.Services.RestfulService
{
    public class CacheRepositoryService : ICacheRepositoryService
    {
        // todo какието проблеммы с использованием Dictionary в потоках! почему то не отрабатывает ContainsKey.
        // todo При добавлении ругается на то что уже ключ добавлен!? попробовать ConcurrentDictionary
        // http://stackoverflow.com/questions/12190039/what-should-i-use-as-an-alternative-to-system-collections-concurrent-concurrentd
        /*private readonly Dictionary<string, object> repository;

        public CacheRepositoryService()
        {
            repository = new Dictionary<string, object>();
        }*/

        public virtual ILinqRepository<TDomain> GetRepository<TDomain>() where TDomain : DomainObject
        {
            return Resolver.Get<ILinqRepository<TDomain>>();

            /*var name = typeof(TDomain).Name;
            try
                {
                    if (repository.ContainsKey(name))
                    {
                        return repository[name] as ILinqRepository<TDomain>;
                    }

                    var rep = Resolver.Get<ILinqRepository<TDomain>>();
                    repository.Add(name, rep);
                    return rep;
                }
                catch (Exception e)
                {
                    Trace.TraceError("CacheRepositoryService: DomainName: {0}; Exception: {1}", name, Diagnostics.KristaDiagnostics.ExpandException(e));
                    return Resolver.Get<ILinqRepository<TDomain>>();
            }*/
        }

        public void ClearCash()
        {
            /*repository.Clear();*/
        }
    }
}