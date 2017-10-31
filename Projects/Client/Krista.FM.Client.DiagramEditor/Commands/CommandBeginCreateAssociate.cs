using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Командя начала создания ассоциации
    /// </summary>
    public class CommandBeginCreateAssociate : DiagramEditorCommand
    {
        public CommandBeginCreateAssociate(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgNewAssociation];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Diagram.Site.CreateAssociation = true;
            Diagram.Site.Cursor = System.Windows.Forms.Cursors.Cross;

            Diagram.Site.Invalidate();
        }
    }
}
