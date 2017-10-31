using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.ServerLibrary;
using ISession = NHibernate.ISession;

namespace Krista.FM.Server.Common
{
    public class SimpleUnitOfWork : DisposableObject, IUnitOfWork
    {
        private ISession session;

        public SimpleUnitOfWork(ISession session)
        {
            this.session = session;
            session.Transaction.Begin();
        }

        /// <summary>
        /// Завершает бизнес транзакцию с фиксацией изменений.
        /// </summary>
        public void Commit()
        {
            session.Transaction.Commit();
        }

        /// <summary>
        /// Завершает бизнес транзакцию с откатом изменений.
        /// </summary>
        public void Rollback()
        {
            session.Transaction.Rollback();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (session.Transaction.IsActive)
                {
                    session.Transaction.Rollback();
                }

                if (session.IsOpen)
                {
                    session.Close();
                    NHibernateSession.Storage.Session = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
