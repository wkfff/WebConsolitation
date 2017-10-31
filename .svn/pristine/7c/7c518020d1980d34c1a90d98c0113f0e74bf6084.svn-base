using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandLineColorManager : DiagramEditorCommand
    {
        public CommandLineColorManager(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgColorLine];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            if (Diagram.Site.SelectedEntities.Count == 0)
            {
                return;
            }

            Color color = Diagram.Site.CmdHelper.ColorDialog();

            if (color.IsEmpty)
            {
                return;
            }

            foreach (DiagramEntity entity in Diagram.Site.SelectedEntities)
            {
                Command command = new CommandLineColorChange(Diagram, entity, entity.LineColor, color);
                command.Execute();
                Diagram.Site.UndoredoManager.Do(command);
            }
        }
    }
}
