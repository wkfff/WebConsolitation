using System;
using System.Linq;
using System.Reflection;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainStore;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Infrastructure
{
    public class RebuildMappingService : IRebuildMappingService
    {
        private readonly IScheme scheme;

        public RebuildMappingService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public void Rebuild(Assembly[] domainAssemblies)
        {
            var assemblies = domainAssemblies.ToList();
            assemblies.Add(typeof(DomainObject).Assembly);

            var connectionString = scheme.SchemeDWH.ConnectionString;
            var factoryName = scheme.SchemeDWH.FactoryName;
            var serverVersion = scheme.SchemeDWH.ServerVersion;

            AppDomain.CurrentDomain.AssemblyResolve += DomainFormsAssembliesStore.CurrentDomainAssemblyResolve;

            NHibernateSession.RebuildNHibernateSessionFactory(connectionString, factoryName, serverVersion, assemblies.ToArray());
        }
    }
}
