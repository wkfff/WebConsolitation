using System.Collections.Generic;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.WriteBack
{
    public interface ISchemeService
    {
        ICollection<IFactTable> GetFactTables();
        IEntityAssociationCollection GetAllAssociations();

        /// <summary>
        /// Возвращает классификатор по полному английскому имени.
        /// Если классификатор не найден, то возвращает null.
        /// </summary>
        IClassifier GetSchemeClassifierByFullName(string clsFullName);

        /// <summary>
        /// Проверяет наличие ссылки из таблицы фактов на классификатор
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="classifierName">Полное наименование классификатора</param>
        bool ContainClassifierInFact(IEntity factTable, string classifierName);

        /// <summary>
        /// Проверка существования задачи и условий, 
        /// позволяющих запись в контексте этой задачи.
        /// </summary>
        void CheckTask(int taskID);

        bool IsOracle();
        bool IsMsSql();
        IDatabase GetDb();
        IProcessor GetOlapProcessor();
        string GetRopositoryDirectory();
    }
}