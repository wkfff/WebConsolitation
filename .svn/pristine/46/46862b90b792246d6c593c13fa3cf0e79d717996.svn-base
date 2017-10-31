using System;
using System.Web;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using NHibernate;

namespace Krista.FM.RIA.Core.NHibernate
{
    public class WebSessionStorage : ISessionStorage
    {
        private const string HttpContextSessionStorageKey = "HttpContextSessionStorageKey";

        public WebSessionStorage(HttpApplication app)
        {
            app.EndRequest += Application_EndRequest;
        }

        public ISession Session
        {
            get
            {
                SimpleSessionStorage storage = GetSimpleSessionStorage();
                return storage.Session;
            }

            set
            {
                SimpleSessionStorage storage = GetSimpleSessionStorage();
                storage.Session = value;
            }
        }

        public string GetServerSessionId()
        {
            var session = HttpContext.Current.Session;
            var logicalCallContext = session[ConnectionHelper.LogicalCallContextDataKeyName] as LogicalCallContextData;
            return logicalCallContext["SessionID"] as string;
        }

        public string GetServerUserName()
        {
            var session = HttpContext.Current.Session;
            var logicalCallContext = session[ConnectionHelper.LogicalCallContextDataKeyName] as LogicalCallContextData;
            return logicalCallContext.Principal.Identity.Name;
        }

        private static SimpleSessionStorage GetSimpleSessionStorage()
        {
            HttpContext context = HttpContext.Current;
            SimpleSessionStorage storage = context.Items[HttpContextSessionStorageKey] as SimpleSessionStorage;
            if (storage == null)
            {
                storage = new SimpleSessionStorage();
                context.Items[HttpContextSessionStorageKey] = storage;
            }

            return storage;
        }

        private static void Application_EndRequest(object sender, EventArgs e)
        {
            NHibernateSession.CloseSession();

            HttpContext context = HttpContext.Current;
            context.Items.Remove(HttpContextSessionStorageKey);
        }
    }
}
