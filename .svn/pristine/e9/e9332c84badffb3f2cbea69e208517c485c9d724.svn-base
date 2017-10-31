using System;

using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Спользуется для установки текущему потоку контекста персистенции.
    /// </summary>
    /// <example>
    /// using(new PersistenceContext())
    /// {
    ///     var somePersistenceObject = someRepository.Get<SomePersistenceClass>(id);
    ///     // Какой-то код...
    ///     someRepository.Save(somePersistenceObject);
    /// }
    /// </example>
    public class PersistenceContext : IDisposable
    {
        public void Dispose()
        {
            if (SessionContext.Session.UnitOfWork == null)
            {
                NHibernateSession.CloseSession();
            }
        }
    }
}
