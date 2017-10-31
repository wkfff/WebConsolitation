using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandAttributesManager : DiagramEditorCommand
    {
        private bool vis;

        public CommandAttributesManager(AbstractDiagram diagram, bool vis)
            : base(diagram)
        {
            this.vis = vis;

            this.Image = vis ? Diagram.Site.ImageList[Images.imgShow] : Diagram.Site.ImageList[Images.imgHide];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Command cmd = new CommandAttributes(Diagram.Site.SelectedEntities, vis);
            cmd.Execute();
            Diagram.Site.UndoredoManager.Do(cmd);
        }
    }
}
