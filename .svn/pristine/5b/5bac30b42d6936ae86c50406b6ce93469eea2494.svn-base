using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandChangeSize : Command, IUndoCommand
    {
        private Size offset;

        private DiagramEntity entity;

        public CommandChangeSize(DiagramEntity entity, Size offset)
        {
            this.entity = entity;
            this.offset = offset;
        }

        public override void Execute()
        {
            Change(offset);
        }

        #region IUndoCommand Members

        public void Undo()
        {
            Change(new Size(-offset.Width, -offset.Height));
        }

        #endregion

        private void Change(Size resize)
        {
            if (entity is DiagramRectangleEntity)
            {
                ((DiagramRectangleEntity)entity).Width += resize.Width;
                ((DiagramRectangleEntity)entity).Height += resize.Height;
            }
        }
    }
}
