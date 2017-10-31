using System;

namespace Krista.FM.ServerLibrary
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
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Завершает бизнес транзакцию с фиксацией изменений.
        /// </summary>
        void Commit();

        /// <summary>
        /// Завершает бизнес транзакцию с откатом изменений.
        /// </summary>
        void Rollback();
    }
}
