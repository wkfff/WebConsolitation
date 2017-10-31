using System;
using Krista.FM.Common;
using NHibernate;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public class SimpleSessionStorage : ISessionStorage
    {
        public ISession Session { get; set; }
        
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
