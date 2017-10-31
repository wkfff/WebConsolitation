using System.Linq;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.E86N.Services.RestfulService
{
    public interface INewRestService
    {
        /// <summary>
        /// Наличие открытой транзакции
        /// </summary>
        bool HaveTransaction { get; }

        /// <summary>
        /// Получить репозиторий
        /// </summary>
        /// <typeparam name="TDomain"> Класс доменного объекта</typeparam>
        /// <returns>Возвращает репоситорий</returns>
        ILinqRepository<TDomain> GetRepository<TDomain>() where TDomain : DomainObject;

        /// <summary>
        /// Получить объект
        /// </summary>
        /// <typeparam name="TDomain">Класс доменного объекта</typeparam>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>Объект - записи</returns>
        TDomain GetItem<TDomain>(int id) where TDomain : DomainObject;

        TDomain GetItem<TDomain>(int? id) where TDomain : DomainObject;

        /// <summary>
        /// Получить прокси объект
        /// </summary>
        /// <typeparam name="TDomain">Класс доменного объекта</typeparam>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>Прокси объект - записи</returns>
        TDomain Load<TDomain>(int id) where TDomain : DomainObject;

        /// <summary>
        /// Получить все объекты из репозитория
        /// </summary>
        /// <typeparam name="TDomain">Класс доменного объекта</typeparam>
        /// <returns>Набор записей</returns>
        IQueryable<TDomain> GetItems<TDomain>() where TDomain : DomainObject;

        IQueryable<TDomain> GetItems<TDomain>(int parentId) where TDomain : DomainObject;

        /// <summary>
        /// Сохраняем объект в репозитории
        /// </summary>
        /// <typeparam name="TDomain">Класс доменного объекта</typeparam>
        /// <param name="item">Объект - запись</param>
        void Save<TDomain>(TDomain item) where TDomain : DomainObject;

        /// <summary>
        /// Удаляем объект из репозитория
        /// </summary>
        /// <typeparam name="TDomain">Класс доменного объекта</typeparam>
        /// <param name="id">Идентификатор записи</param>
        void Delete<TDomain>(int id) where TDomain : DomainObject;

        /// <summary>
        /// Удаляем объект из репозитория
        /// </summary>
        /// <typeparam name="TDomain">
        /// Класс доменного объекта
        /// </typeparam>
        /// <param name="entity">
        /// Удаляемая запись
        /// </param>
        void Delete<TDomain>(TDomain entity) where TDomain : DomainObject;

        /// <summary>
        /// Экшин удаления записи
        /// </summary>
        /// <typeparam name="TDomain">Класс доменного объекта</typeparam>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>Результат в виде RestResult</returns>
        RestResult DeleteAction<TDomain>(int id) where TDomain : DomainObject;

        /// <summary>
        /// Экшин удаления записи детализации документа
        /// </summary>
        RestResult DeleteDocDetailAction<TDomain>(int id, int docId) where TDomain : DomainObject;

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        void CommitChanges();

        /// <summary>
        /// Открыть транзакцию
        /// </summary>
        void BeginTransaction();
        
        /// <summary>
        /// Фиксировать транзакцию
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        void RollbackTransaction();
    }
}