using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create()
        {
            return new NHibernateUnitOfWork(NHibernateSession.Current, SessionContext.Session);
        }
    }
}
