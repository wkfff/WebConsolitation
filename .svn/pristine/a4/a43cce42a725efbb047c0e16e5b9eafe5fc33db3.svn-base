using System.Drawing;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandNewTableEntity : CommandNewDiagramEntity
    {
        public CommandNewTableEntity(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgNewtable];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            IEntity entity = Diagram.Site.NewEntityCommand.Execute(ClassTypes.Table);
            UMLEntityBase entityBase = AddEntity(
                entity,
                ScaleTransform.TransformPoint(Diagram.Site.LastClickPoint, Diagram.Site.ZoomFactor));

            Refresh(entity);
        }
    }
}
