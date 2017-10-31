using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandBeginCreateBridge2BridgeAssociate : DiagramEditorCommand
    {
        public CommandBeginCreateBridge2BridgeAssociate(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgBridgeAssociation];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Diagram.Site.CreateBridge2BridgeAssociation = true;
            Diagram.Site.Cursor = System.Windows.Forms.Cursors.Cross;

            Diagram.Site.Invalidate();
        }
    }
}
