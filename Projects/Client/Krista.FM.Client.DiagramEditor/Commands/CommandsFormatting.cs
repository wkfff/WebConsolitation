using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда форматирования объекта по-умолчанию
    /// </summary>
    public class CommandsStandartFormatting : DiagramEditorCommand
    {
        public CommandsStandartFormatting(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgStandartFormatting];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            foreach (DiagramEntity entity in Diagram.Site.SelectedEntities)
            {
                entity.InitializeDefault();
            }

            Diagram.Site.Invalidate();
        }
    }
}
