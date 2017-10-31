using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Krista.FM.Common;
using Krista.FM.Server.TemplatesService;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Server.Scheme
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

            // Правила инекций
            container.Configure<Interception>()
                .AddPolicy("non-negative")
                .AddCallHandler<LogInterceptor>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor());

            ComponentRegistrar.AddComponentsTo(container, typeof(NHibernateTransactionManager));

            return container;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            Resolver.RegisterType<IScheme, SchemeClass>(new ContainerControlledLifetimeManager());
            Resolver.RegisterType<ITemplatesService, TemplatesService.TemplatesService, VirtualMethodInterceptor>();
            Resolver.RegisterType<ITemplatesRepository, TemplatesRepository>();
            Resolver.RegisterType<IUnitOfWorkFactory, NHibernateUnitOfWorkFactory>();
        }
    }
}
