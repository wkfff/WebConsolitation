using System.Xml.Schema;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Server.Common;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Server.DataPumps.FnsEgrulPump
{
    public static class UnityStarter
    {
        public static UnityContainer Initialize()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            RegisterTypes(container);

            ComponentRegistrar.AddComponentsTo(container);

            return container;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<FnsEgrulPumpModule>(
                new InterceptionBehavior<PolicyInjectionBehavior>(), 
                new Interceptor<VirtualMethodInterceptor>());

            Resolver.RegisterType<IUnitOfWorkFactory, NHibernateUnitOfWorkFactory>();
            Resolver.RegisterInstance(new XmlSchemaSet(), new ContainerControlledLifetimeManager());
        }
    }
}
