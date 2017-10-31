using Krista.FM.Client.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Diagrams
{
    public class DocumentEntityDiagram : AbstractDiagram
    {
        public DocumentEntityDiagram(DiargamEditor site, IDocument document)
            : base(site, document)
        {
        }

        public DocumentEntityDiagram(DiargamEditor site)
            : base(site)
        {
        }

        /// <summary>
        /// Инициализация команд
        /// </summary>
        public override void InitializeCommands()
        {
            Command newDocumentEntity = new Commands.CommandNewDocumentEntity(this);

            // Команды контекстного меню диаграммы
            DiagramСommands.Add("/ContextMenu/@Добавить класс/Табличный документ", newDocumentEntity);
            DiagramСommands.Add("/ContextMenu/Добавить пакет", new Commands.CommandNewPackageEntity(this));
            DiagramСommands.Add("/ContextMenu/Добавить ассоциацию", new Commands.CommandBeginCreateAssociate(this));

            DiagramСommands.Add("/MenuBar/Диаграмма/@Добавить класс/Табличный документ", newDocumentEntity);

            base.InitializeCommands();
        }

        /// <summary>
        /// Проверяет может ли объект находится на диаграмме.
        /// </summary>
        internal override bool IsAllowedEntity(object obj)
        {
            if (obj is IEntity && (
                ((IEntity)obj).ClassType == ClassTypes.DocumentEntity ||
                ((IEntity)obj).ClassType == ClassTypes.clsDataClassifier ||
                ((IEntity)obj).ClassType == ClassTypes.clsFixedClassifier))
            {
                return true;
            }

            return false;
        }
    }
}
