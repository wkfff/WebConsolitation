using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда смены цвета линии
    /// </summary>
    public class CommandLineColorChange : DiagramEditorCommand, IUndoCommand
    {
        private readonly DiagramEntity entity;
        private readonly Color colorBefore;
        private readonly Color colorAfter;

        public CommandLineColorChange(AbstractDiagram diagram, DiagramEntity entity, Color color_before, Color color_after)
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
            if (entity.LineColor != color)
            {
                Diagram.IsChanged = true;
            }

            if (entity is UMLAssociationBase)
            {
                if (entity is UMLAssociation)
                {
                    ((UMLAssociation)entity).PenWithCap.Color = color;
                }

                entity.Pen.Color = color;
                Diagram.IsChanged = true;
            }
            else
            {
                entity.LineColor = color;
            }

            Diagram.Site.Invalidate();
        }
    }
}
