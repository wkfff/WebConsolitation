using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда смены цвета тени
    /// </summary>
    public class CommandShadowChange : DiagramEditorCommand, IUndoCommand
    {
        private readonly DiagramEntity entity;
        private readonly Color colorBefore;
        private readonly Color colorAfter;

        public CommandShadowChange(AbstractDiagram diagram, DiagramEntity entity, Color color_before, Color color_after)
            : base(diagram)
        {
            this.entity = entity;
            this.colorAfter = color_after;
            this.colorBefore = color_before;
        }

        public override void Execute()
        {
            Change(colorAfter);
        }

        #region IUndoCommand Members

        public void Undo()
        {
            Change(colorBefore);
        }

        #endregion

        private void Change(Color color)
        {
            if (entity.ShadowColor != color)
            {
                entity.ShadowColor = color;

                Diagram.IsChanged = true;
                Diagram.Site.Invalidate();
            }
        }
    }
}
