using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandFontManager : DiagramEditorCommand
    {
        public CommandFontManager(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgFont];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            if (Diagram.Site.SelectedEntities.Count == 0)
            {
                return;
            }

            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true;

            fontDialog.Font = Diagram.Site.SelectedEntities[0].Font;
            fontDialog.Color = Diagram.Site.SelectedEntities[0].TextColor;

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (DiagramEntity entity in Diagram.Site.SelectedEntities)
                {
                    Command cmd = new Commands.CommandFontChange(Diagram, entity.Font, fontDialog.Font, entity.TextColor, fontDialog.Color, entity);
                    cmd.Execute();
                    Diagram.Site.UndoredoManager.Do(cmd);
                }

                Diagram.IsChanged = true;
                Diagram.Site.Invalidate();
            }
        }
    }
}
