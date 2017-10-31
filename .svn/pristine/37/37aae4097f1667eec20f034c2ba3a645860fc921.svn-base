using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Common
{
    public static class Resolver
    {
        public static T Get<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        public static object Get(Type type)
        {
            return ServiceLocator.Current.GetInstance(type);
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return ServiceLocator.Current.GetAllInstances<T>();
        }

        public static void RegisterType(Type fromType, Type toType, LifetimeManager manager)
        {
            IUnityContainer container = Get<IUnityContainer>();
            container.RegisterType(fromType, toType, manager);
        }

        public static void RegisterType<TFrom, TTo>(LifetimeManager manager)
        {
            IUnityContainer container = Get<IUnityContainer>();
            container.RegisterType(typeof(TFrom), typeof(TTo), manager);
        }

        public static void RegisterType<TFrom, TTo, TInterceptor>()
        {
            RegisterType<TFrom, TTo, TInterceptor>(new PerResolveLifetimeManager());
        }


        public static void RegisterType<TFrom, TTo, TInterceptor>(LifetimeManager manager)
        {
            IUnityContainer container = Get<IUnityContainer>();
            container.RegisterType(
                typeof(TFrom), 
                typeof(TTo), 
                manager,
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor(typeof(TInterceptor)));
        }

        public static void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            IUnityContainer container = Get<IUnityContainer>();
            container.RegisterType(typeof(TFrom), typeof(TTo), new PerResolveLifetimeManager());
        }

        public static void RegisterInstance<T>(T instance, LifetimeManager lifetimeManager)
        {
            IUnityContainer container = Get<IUnityContainer>();
            container.RegisterInstance(instance, lifetimeManager);
        }
    }
}
