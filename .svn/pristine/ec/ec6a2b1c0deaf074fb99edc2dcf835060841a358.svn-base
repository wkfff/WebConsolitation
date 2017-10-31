using System;
using System.Drawing;

using Krista.FM.Client.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandNewPackageEntity : DiagramEditorCommand
    {
        public CommandNewPackageEntity(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgNewPackage];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            IPackage package = Diagram.Site.NewPackageCommand.Execute();
            Diagram.Entities.Add(
                new UMLPackage(
                    package.Key,
                    Guid.NewGuid(),
                    Diagram,
                    ScaleTransform.TransformPoint(Diagram.Site.LastClickPoint, Diagram.Site.ZoomFactor).X,
                    ScaleTransform.TransformPoint(Diagram.Site.LastClickPoint, Diagram.Site.ZoomFactor).Y,
                    Color.Pink));

            Diagram.IsChanged = true;
            Diagram.Site.Invalidate();
        }
    }
}
