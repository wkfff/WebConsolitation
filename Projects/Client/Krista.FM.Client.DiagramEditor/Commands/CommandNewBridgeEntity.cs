using System.Drawing;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandNewBridgeEntity : CommandNewDiagramEntity
    {
        public CommandNewBridgeEntity(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgClsBridge];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            IEntity entity = Diagram.Site.NewEntityCommand.Execute(ClassTypes.clsBridgeClassifier);
            UMLEntityBase entityBase = AddEntity(
                entity,
                ScaleTransform.TransformPoint(Diagram.Site.LastClickPoint, Diagram.Site.ZoomFactor));

            Refresh(entity);
        }
    }
}
