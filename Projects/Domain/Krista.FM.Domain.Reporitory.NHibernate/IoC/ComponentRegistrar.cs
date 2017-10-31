using System;
using Krista.FM.Common;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Domain.Reporitory.NHibernate.IoC
{
    public static class ComponentRegistrar
    {
        public static void AddComponentsTo(IUnityContainer container)
        {
            AddComponentsTo(container, typeof(NHibernateTransactionManager));
        }

        public static void AddComponentsTo(IUnityContainer container, Type transactionManagerType)
        {
            Check.Require(container != null, "container");

            container.RegisterType<ITransactionManager, NHibernateTransactionManager>();

            // Регистрируем обобщенные репозитории
            container.RegisterType(typeof(ILinqRepository<>), typeof(NHibernateLinqRepository<>));
            container.RegisterType(typeof(IRepository<>), typeof(NHibernateRepository<>));

            if (!container.IsRegistered<UnitOfWorkInterceptor>())
            {
                container.Configure<Interception>()
                    .AddPolicy("UnitOfWork")
                    .AddMatchingRule(new UnitOfWorkMatchingRule())
                    .AddCallHandler<UnitOfWorkInterceptor>(
                        new ContainerControlledLifetimeManager(),
                        new InjectionConstructor());
            }
        }
    }
}
