using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;

using NHibernate;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Отвечает за хранение сессии NHibernate в клиентской сессии на сервере.
    /// </summary>
    public class PersistenceSessionStorage : ISessionStorage
    {
        public ISession Session
        {
            get
            {
                return SessionContext.Session.PersistenceSession;
            }
            set
            {
                SessionContext.Session.PersistenceSession = value;
            }
        }

        public string GetServerSessionId()
        {
            return ClientSession.SessionId;
        }

        public string GetServerUserName()
        {
            return LogicalCallContextData.GetContext().Principal.Identity.Name;
        }
    }
}
