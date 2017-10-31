using System.Reflection;

namespace Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache
{
    public class DefaultDynamicAssemblyDomainStorage : IDynamicAssemblyDomainStorage
    {
        public Assembly Get(string name)
        {
            return null;
        }

        public Assembly[] GetAll()
        {
            return new[] { typeof(DomainObject).Assembly };
        }

        public void Add(Assembly assembly)
        {
        }

        public void Remove(Assembly assembly)
        {
        }
    }
}