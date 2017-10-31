using NHibernate;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public class DbContext : IDbContext
    {
        public void BeginTransaction()
        {
            Session.BeginTransaction();
        }

        public void CommitChanges()
        {
            Session.Flush();
        }

        public void CommitTransaction()
        {
            Session.Transaction.Commit();
        }

        public void RollbackTransaction()
        {
            Session.Transaction.Rollback();
        }

        private static ISession Session
        {
            get
            {
                return NHibernateSession.Current;
            }
        }
    }
}
