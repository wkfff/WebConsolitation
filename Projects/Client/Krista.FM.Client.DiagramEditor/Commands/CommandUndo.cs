using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда отменить
    /// </summary>
    public class CommandUndo : DiagramEditorCommand
    {
        public CommandUndo(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Enabled = false;

            this.Image = Diagram.Site.ImageList[Images.imgEditUndo];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Diagram.Site.UndoredoManager.Undo();
        }
    }
}
