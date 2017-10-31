using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandLineWidthManager : DiagramEditorCommand
    {
        public CommandLineWidthManager(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgLineWidht];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            if (Diagram.Site.SelectedEntities.Count == 0)
            {
                return;
            }

            Tools.LineWidthModalForm lineWidthForm = new Krista.FM.Client.DiagramEditor.Tools.LineWidthModalForm(Diagram.Site.SelectedEntities[0].LineWidth);

            if (lineWidthForm.ShowDialog() == DialogResult.OK)
            {
                foreach (DiagramEntity entity in Diagram.Site.SelectedEntities)
                {
                    Command cmd = new Commands.CommandLineWidhtChange(Diagram, entity, entity.LineWidth, lineWidthForm.LineWidth);
                    cmd.Execute();
                    Diagram.Site.UndoredoManager.Do(cmd);
                }
            }
            else
            {
                lineWidthForm.Close();
            }
        }
    }
}
