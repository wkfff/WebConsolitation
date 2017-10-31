using System;
using System.Data;

namespace Krista.FM.Domain.Reporitory
{
    public interface IDomainDataService
    {
        /// <summary>
        /// Возвращает таблицу с данными для указанного типа объекта.
        /// </summary>
        /// <param name="objectType">Тип объекта.</param>
        /// <param name="selectFilter">Условие фильтрации даннных.</param>
        DataRow[] GetObjectData(Type objectType, string selectFilter);

        /// <summary>
        /// Создает новый объект в базе данных.
        /// </summary>
        /// <param name="obj"></param>
        void Create(DomainObject obj);

        /// <summary>
        /// Обновляет объект в базе данных.
        /// </summary>
        /// <param name="obj"></param>
        void Update(DomainObject obj);
    }
}