using System;
using System.Threading;
using Krista.FM.Common;
using NHibernate;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public class ThreadSessionStorage : IUnitOfWorkSessionStorage
    {
        private readonly ThreadSafeDictionary<string, SimpleSessionStorage> perThreadSessionStorage =
            new ThreadSafeDictionary<string, SimpleSessionStorage>();

        #region IUnitOfWorkSessionStorage Members

        public ISession Session
        {
            get { return GetSimpleSessionStorageForThread().Session; } 
            set { GetSimpleSessionStorageForThread().Session = value; }
        }

        public string GetServerSessionId()
        {
            return ClientSession.SessionId;
        }

        public string GetServerUserName()
        {
            return ClientAuthentication.UserName;
        }

        public void EndUnitOfWork(bool closeSessions)
        {
            if (closeSessions)
            {
                NHibernateSession.CloseSession();
                perThreadSessionStorage.Remove(GetCurrentThreadName());
            }
        }

        #endregion

        private static string GetCurrentThreadName()
        {
            return Thread.CurrentThread.Name ?? (Thread.CurrentThread.Name = Guid.NewGuid().ToString());
        }

        private SimpleSessionStorage GetSimpleSessionStorageForThread()
        {
            string currentThreadName = GetCurrentThreadName();
            SimpleSessionStorage sessionStorage;
            if (!perThreadSessionStorage.TryGetValue(currentThreadName, out sessionStorage))
            {
                sessionStorage = new SimpleSessionStorage();
                perThreadSessionStorage.Add(currentThreadName, sessionStorage);
            }

            return sessionStorage;
        }
    }
}
