using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core
{
    public static class Resolver
    {
        public static T Get<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        public static T Get<T>(string key)
        {
            return ServiceLocator.Current.GetInstance<T>(key);
        }

        public static object Get(Type type)
        {
            return ServiceLocator.Current.GetInstance(type);
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return ServiceLocator.Current.GetAllInstances<T>();
        }

        public static void RegisterInstance<T>(T instance, LifetimeManagerType lifetimeManagerType)
        {
            IUnityContainer container = Get<IUnityContainer>();
            LifetimeManager manager = GetLifetimeManager<T>(lifetimeManagerType);
            container.RegisterInstance(instance, manager);
        }

        public static void RegisterType(Type fromType, Type toType, LifetimeManager manager)
        {
            IUnityContainer container = Get<IUnityContainer>();
            container.RegisterType(fromType, toType, manager);
        }

        public static void RegisterType<T>(LifetimeManagerType lifetimeManagerType)
        {
            IUnityContainer container = Get<IUnityContainer>();
            LifetimeManager manager = GetLifetimeManager<T>(lifetimeManagerType);
            container.RegisterType(typeof(T), manager);
        }

        public static void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            IUnityContainer container = Get<IUnityContainer>();
            container.RegisterType(typeof(TFrom), typeof(TTo), new PerResolveLifetimeManager());
        }

        private static LifetimeManager GetLifetimeManager<T>(LifetimeManagerType lifetimeManagerType)
        {
            switch (lifetimeManagerType)
            {
                case LifetimeManagerType.Session:
                    return new SessionLifetimeManager<T>();
                default:
                    throw new NotImplementedException(String.Format("LifetimeManager {0} not implemented.", lifetimeManagerType));
            }
        }
    }
}
