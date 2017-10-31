using System;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainStore
{
    public class DomainStoreInitializer
    {
        private static readonly object SyncLock = new object();
        private static volatile DomainStoreInitializer instance;
        private bool domainSessionIsLoaded;

        protected DomainStoreInitializer()
        {
        }

        public static DomainStoreInitializer Instance()
        {
            if (instance == null)
            {
                lock (SyncLock)
                {
                    if (instance == null)
                    {
                        instance = new DomainStoreInitializer();
                    }
                }
            }

            return instance;
        }

        public void InitializeOnce(Action initMethod)
        {
            lock (SyncLock)
            {
                if (!domainSessionIsLoaded)
                {
                    initMethod();
                    domainSessionIsLoaded = true;
                }
            }
        }
    }
}
