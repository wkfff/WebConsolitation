using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandBeginCreateMasterDetailAssociate : DiagramEditorCommand
    {
        public CommandBeginCreateMasterDetailAssociate(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgMDAssociation];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Diagram.Site.CreateMasterDetailAssociation = true;
            Diagram.Site.Cursor = System.Windows.Forms.Cursors.Cross;

            Diagram.Site.Invalidate();
        }
    }
}
