using System.Reflection;

namespace Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache
{
    /// <summary>
    /// Хранилище динамически генерируемых доменных сборок.
    /// </summary>
    public interface IDynamicAssemblyDomainStorage
    {
        /// <summary>
        /// Возвращает сборку из хранилища.
        /// </summary>
        /// <param name="name">Полное имя сборки (FullName).</param>
        Assembly Get(string name);

        /// <summary>
        /// Возвращает все сборки из хранилища.
        /// </summary>
        Assembly[] GetAll();

        /// <summary>
        /// Добавляет сборку в хранилище.
        /// </summary>
        /// <param name="assembly">Сборка добавляемая в хранилище.</param>
        void Add(Assembly assembly);

        /// <summary>
        /// Удаляет сборку из хранилища.
        /// </summary>
        /// <param name="assembly">Удаляемая сборка.</param>
        void Remove(Assembly assembly);
    }
}