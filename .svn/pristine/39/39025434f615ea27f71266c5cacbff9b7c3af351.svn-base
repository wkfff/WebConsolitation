using System.Collections.Generic;

namespace Krista.FM.Domain.Reporitory
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Возвращает реальный объект из БД по его идентификатору. Если объекта в базе нет,
        /// то вернет null. Всегда выполняет операцию select к базе данных, если объекта нет в кеше
        /// первого или второго уровня.
        /// </summary>
        /// <param name="id">Идентификатор объекта (суррогатный ключ).</param>
        T Get(int id);

        /// <summary>
        /// Возвращает прокси-объект с указанным идентификатором, если объекта нет 
        /// в кеше первого или второго уровня, иначе вернет реальный объект из кеша. 
        /// Никогда не выполняет операцию select к БД.
        /// </summary>
        /// <param name="id">Идентификатор объекта (суррогатный ключ).</param>
        T Load(int id);

        IList<T> GetAll();

        void Save(T entity);
        
        void Delete(T entity);

        IDbContext DbContext { get; }
    }
}
