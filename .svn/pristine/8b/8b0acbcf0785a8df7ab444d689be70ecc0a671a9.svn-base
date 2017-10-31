using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Класс для регистрации и отката команды смены цвета заливки
    /// </summary>
    public class CommandChangeFillColor : DiagramEditorCommand, IUndoCommand
    {
        private readonly Color colorBefore;
        private readonly Color colorAfter;
        private readonly DiagramEntity entity;

        public CommandChangeFillColor(AbstractDiagram diagram, DiagramEntity entity, Color color_before, Color color_after)
            : base(diagram)
        {
            this.entity = entity;
            this.colorBefore = color_before;
            this.colorAfter = color_after;
        }

        #region Command Members

        public override void Execute()
        {
            Change(colorAfter);
        }

        #endregion

        #region IUndoCommand Members

        public void Undo()
        {
            Change(colorBefore);
        }

        private void Change(Color color)
        {
            if (entity != null)
            {
                entity.FillColor = color;

                Diagram.IsChanged = true;
                Diagram.Site.Invalidate();
            }
        }
        #endregion
    }
}
