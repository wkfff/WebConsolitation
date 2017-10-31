using System;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public class NHibernateInitializer
    {
        private static NHibernateInitializer instance;
        private bool nHibernateSessionIsLoaded;
        private static readonly object syncLock = new object();

        protected NHibernateInitializer()
        {
        }

        public void InitializeNHibernateOnce(Action initMethod)
        {
            lock (syncLock)
            {
                if (!nHibernateSessionIsLoaded)
                {
                    initMethod();
                    nHibernateSessionIsLoaded = true;
                }
            }
        }

        public static NHibernateInitializer Instance()
        {
            if (instance == null)
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new NHibernateInitializer();
                    }
                }
            }
            return instance;
        }
    }
}
