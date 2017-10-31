using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandForeGroundPositionManager : Command
    {
        private readonly DiagramEntity entity;

        public CommandForeGroundPositionManager(DiagramEntity entity)
        {
            this.Image = entity.Diagram.Site.ImageList[Images.imgForeGround];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));

            this.entity = entity;
        }

        public override void Execute()
        {
            Command cmd = new CommandForeGround(entity);
            cmd.Execute();
            entity.Diagram.Site.UndoredoManager.Do(cmd);
        }
    }
}
