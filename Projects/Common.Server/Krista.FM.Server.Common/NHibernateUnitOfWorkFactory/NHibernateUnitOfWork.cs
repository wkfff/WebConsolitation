using Krista.FM.Common;
using Krista.FM.ServerLibrary;

using INHibernateSession = NHibernate.ISession;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Определяет рамки бизнес транзакции на клиенте.
    /// </summary>
    /// <remarks>
    /// Если в рамках бизнес транзакции не был выполнен Commit, то в момент уничтожения
    /// UnitOfWork будет выполнен Rollback.
    /// </remarks>
    /// <example>
    /// using (var uow = scheme.SchemeDWH.CreateUnitOfWork())
    /// {
    ///     // Некоторые действия...
    /// 
    ///     uow.Commit();
    /// }
    /// </example>
    public class NHibernateUnitOfWork : DisposableObject, IUnitOfWork
    {
        private readonly INHibernateSession session;
        private readonly Session serverSession;

        public NHibernateUnitOfWork(INHibernateSession session, Session serverSession)
        {
            this.session = session;
            this.serverSession = serverSession;
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
                    serverSession.PersistenceSession = null;
                }

                serverSession.UnitOfWork = null;
            }

            base.Dispose(disposing);
        }
    }
}
