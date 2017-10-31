using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandBeginCreateBridgeAssociate : DiagramEditorCommand
    {
        public CommandBeginCreateBridgeAssociate(AbstractDiagram diagram)
            : base(diagram)
        {
            Image = Diagram.Site.ImageList[Images.imgBridgeAssociation];
            Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Diagram.Site.CreateBridgeAssociation = true;
            Diagram.Site.Cursor = System.Windows.Forms.Cursors.Cross;

            Diagram.Site.Invalidate();
        }
    }
}
