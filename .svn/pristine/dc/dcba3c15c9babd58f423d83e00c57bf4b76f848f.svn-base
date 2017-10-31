using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandShadowVisibleManager : CommandWithPrm, IUndoCommand
    {
        private AbstractDiagram diagram;

        public CommandShadowVisibleManager(AbstractDiagram diagram)
        {
            this.diagram = diagram;
        }

        public override void Execute(object obj)
        {
            // Параметр команды
            Parameter = obj;

            foreach (DiagramEntity entity in diagram.Site.SelectedEntities)
            {
                if (entity is DiagramRectangleEntity)
                {
                    CommandWithPrm command = new CommandShadowVisibleChange(entity);
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
                    CommandWithPrm command = new CommandShadowVisibleChange(entity);
                    command.Execute(!(bool)Parameter);
                }
            }
        }

        #endregion
    }
}
