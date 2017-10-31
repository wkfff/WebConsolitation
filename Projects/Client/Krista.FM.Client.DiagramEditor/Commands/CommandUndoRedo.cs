using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда выполнить повторно
    /// </summary>
    public class CommandRedo : DiagramEditorCommand
    {
        public CommandRedo(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Enabled = false;

            this.Image = Diagram.Site.ImageList[Images.imgEditRedo];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Diagram.Site.UndoredoManager.Redo();
        }
    }
}
