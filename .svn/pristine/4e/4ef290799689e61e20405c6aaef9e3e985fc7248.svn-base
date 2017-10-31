using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class SaveDiagramCommand : DiagramEditorCommand
    {
        public SaveDiagramCommand(AbstractDiagram diagram)
            : base(diagram)
        {
            Enabled = false;

            this.Image = Diagram.Site.ImageList[Images.imgSaveDiagram];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }
                
        public override void Execute()
        {
            if (Diagram.Site != null)
            {
                Diagram.Site.SaveDiagram();
            }

            Enabled = false;
        }
    }
}
