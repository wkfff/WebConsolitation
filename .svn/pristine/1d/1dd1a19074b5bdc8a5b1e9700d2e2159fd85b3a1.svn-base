using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Мунеджер отображение/скрытия SQL-выражения
    /// </summary>
    public class CommandSQLExpressionManager : CommandWithPrm, IUndoCommand
    {
        private AbstractDiagram diagram;

        public CommandSQLExpressionManager(AbstractDiagram diagram)
        {
            this.diagram = diagram;
        }

        public override void Execute(object obj)
        {
            // параметр команды
            this.Parameter = (bool)obj;

            foreach (DiagramEntity entity in diagram.Site.SelectedEntities)
            {
                if (entity is DiagramRectangleEntity)
                {
                    CommandWithPrm command = new CommandSQLExpression(entity);
                    command.Execute(Parameter);
                }
            }

            diagram.Site.UndoredoManager.Do(this);
        }

        #region IUndoCommand Members

        public void Undo()
        {
            foreach (DiagramEntity entity in diagram.Site.SelectedEntities)
            {
                if (entity is DiagramRectangleEntity)
                {
                    CommandWithPrm command = new CommandSQLExpression(entity);
                    command.Execute(!(bool)Parameter);
                }
            }
        }

        #endregion
    }
}
