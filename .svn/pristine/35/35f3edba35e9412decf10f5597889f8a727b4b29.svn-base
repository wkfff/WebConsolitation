using System.Xml.Schema;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Server.Common;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    public class UnityStarter
    {
        public static UnityContainer Initialize()
        {
            // Создаем контейнер
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            RegisterTypes(container);

            container.Configure<Interception>()
                .AddPolicy("UnitOfWork")
                .AddMatchingRule(new UnitOfWorkMatchingRule())
                .AddCallHandler<UnitOfWorkInterceptor>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor());

            ComponentRegistrar.AddComponentsTo(container, typeof(NHibernateTransactionManager));

            return container;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<BusGovRuPumpModule, BusGovRuPumpModule>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<VirtualMethodInterceptor>());
            Resolver.RegisterType<IUnitOfWorkFactory, NHibernateUnitOfWorkFactory>();
            Resolver.RegisterInstance(new XmlSchemaSet(), new ContainerControlledLifetimeManager());
        }
    }
}
