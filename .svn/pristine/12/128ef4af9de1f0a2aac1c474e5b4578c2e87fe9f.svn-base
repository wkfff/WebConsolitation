using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда смены толщины линии
    /// </summary>
    public class CommandLineWidhtChange : DiagramEditorCommand, IUndoCommand
    {
        private readonly DiagramEntity entity;
        private readonly int widhtBefore;
        private readonly int widhtAfter;

        public CommandLineWidhtChange(AbstractDiagram diagram, DiagramEntity entity, int widht_before, int widht_after)
            : base(diagram)
        {
            this.entity = entity;
            this.widhtAfter = widht_after;
            this.widhtBefore = widht_before;
        }

        public override void Execute()
        {
            Change(widhtAfter);
        }

        #region IUndoCommand Members

        public void Undo()
        {
            Change(widhtBefore);
        }

        private void Change(int widht)
        {
            this.entity.LineWidth = widht;

            Diagram.IsChanged = true;
            Diagram.Site.Invalidate();
        }

        #endregion
    }
}
