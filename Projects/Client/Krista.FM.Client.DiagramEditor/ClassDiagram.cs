using Krista.FM.Client.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Diagrams
{
    /// <summary>
    /// Диаграмма классов
    /// </summary>
    public class ClassDiagram : AbstractDiagram
    {
        #region Конструктор

        public ClassDiagram(DiargamEditor site, IDocument document)
            : base(site, document)
        {
        }

        public ClassDiagram(DiargamEditor site)
            : base(site)
        {
        }

        #endregion Конструктор

        /// <summary>
        /// Инициализация команд
        /// </summary>
        public override void InitializeCommands()
        {
            Command newFixedEntity = new Commands.CommandNewFixedEntity(this);
            Command newBridgeEntity = new Commands.CommandNewBridgeEntity(this);
            Command newDataEntity = new Commands.CommandNewDataEntity(this);
            Command newFactEntity = new Commands.CommandNewFactEntity(this);
            Command newTableEntity = new Commands.CommandNewTableEntity(this);

            // Команды контекстного меню диаграммы
            DiagramСommands.Add("/ContextMenu/@Добавить класс/Фиксированный классификатор", newFixedEntity);
            DiagramСommands.Add("/ContextMenu/@Добавить класс/Сопоставимый классификатор", newBridgeEntity);
            DiagramСommands.Add("/ContextMenu/@Добавить класс/Классификатор данных", newDataEntity);
            DiagramСommands.Add("/ContextMenu/@Добавить класс/Таблица фактов", newFactEntity);
            DiagramСommands.Add("/ContextMenu/@Добавить класс/Таблица", newTableEntity);
            DiagramСommands.Add("/ContextMenu/Добавить пакет", new Commands.CommandNewPackageEntity(this));
            DiagramСommands.Add("/ContextMenu/Добавить ассоциацию", new Commands.CommandBeginCreateAssociate(this));
            DiagramСommands.Add("/ContextMenu/Добавить ассоциацию сопоставления", new Commands.CommandBeginCreateBridgeAssociate(this));
            DiagramСommands.Add("/ContextMenu/Добавить ассоциацию сопоставления версий", new Commands.CommandBeginCreateBridge2BridgeAssociate(this));
            DiagramСommands.Add("/ContextMenu/Добавить ассоциацию мастер-деталь", new Commands.CommandBeginCreateMasterDetailAssociate(this));

            DiagramСommands.Add("/MenuBar/Диаграмма/@Добавить класс/Фиксированный классификатор", newFixedEntity);
            DiagramСommands.Add("/MenuBar/Диаграмма/@Добавить класс/Сопоставимый классификатор", newBridgeEntity);
            DiagramСommands.Add("/MenuBar/Диаграмма/@Добавить класс/Классификатор данных", newDataEntity);
            DiagramСommands.Add("/MenuBar/Диаграмма/@Добавить класс/Таблица фактов", newFactEntity);
            DiagramСommands.Add("/MenuBar/Диаграмма/@Добавить класс/Таблица", newTableEntity);

            base.InitializeCommands();
        }

        /// <summary>
        /// Проверяет может ли объект находится на диаграмме.
        /// </summary>
        internal override bool IsAllowedEntity(object obj)
        {
            if ((obj is IEntity && ((IEntity)obj).ClassType != ClassTypes.DocumentEntity) 
                || obj is IEntityAssociation
                || obj is IPackage)
            {
                return true;
            }

            return false;
        }
    }
}
