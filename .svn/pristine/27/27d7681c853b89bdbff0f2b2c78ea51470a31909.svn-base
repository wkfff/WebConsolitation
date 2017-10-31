using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    public class SimpleUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create()
        {
            return new SimpleUnitOfWork(NHibernateSession.Current);
        }
    }
}
