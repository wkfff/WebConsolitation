using System.Drawing;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandNewDataEntity : CommandNewDiagramEntity
    {
        public CommandNewDataEntity(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgClsData];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            IEntity entity = Diagram.Site.NewEntityCommand.Execute(ClassTypes.clsDataClassifier);
            UMLEntityBase entityBase = AddEntity(
                entity,
                ScaleTransform.TransformPoint(Diagram.Site.LastClickPoint, Diagram.Site.ZoomFactor));

            Refresh(entity);
        }
    }
}
