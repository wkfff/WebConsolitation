using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда смены шрифта
    /// </summary>
    public class CommandFontChange : DiagramEditorCommand, IUndoCommand
    {
        private readonly Font fontBefore;
        private readonly Font fontAfter;

        private readonly Color fontColorBefore;
        private readonly Color fontColorAfter;

        private readonly DiagramEntity entity;

        public CommandFontChange(AbstractDiagram diagram, Font font_before, Font font_after, Color fontColor_before, Color fontColor_after, DiagramEntity entity)
            : base(diagram)
        {
            this.fontAfter = font_after;
            this.fontBefore = font_before;

            this.fontColorAfter = fontColor_after;
            this.fontColorBefore = fontColor_before;

            this.entity = entity;
        }

        public override void Execute()
        {
            Change(fontAfter, fontColorAfter);
        }

        #region IUndoCommand Members

        public void Undo()
        {
            Change(fontBefore, fontColorBefore);
        }

        #endregion

        private void Change(Font font, Color color)
        {
            if (entity is DiagramRectangleEntity)
            {
                entity.Font = font;
                entity.TextColor = color;

                ((DiagramRectangleEntity)entity).AutoSizeRec();
            }
        }
    }
}
