using System.Drawing;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandBackGroundPositionManager : Command
    {
        private DiagramEntity entity;

        public CommandBackGroundPositionManager(DiagramEntity entity)
        {
            this.Image = entity.Diagram.Site.ImageList[Images.imgBackGround];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));

            this.entity = entity;
        }

        protected DiagramEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        public override void Execute()
        {
            Command cmd = new CommandBackGround(entity);
            cmd.Execute();
            entity.Diagram.Site.UndoredoManager.Do(cmd);
        }
    }
}
