using System;
using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда удаления символа с диаграммы
    /// </summary>
    public class CommandDeleteSymbol : DiagramEditorCommand
    {
        public CommandDeleteSymbol(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgDeletSymbol];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            try
            {
                Command cmd = new CommandRemove(Diagram.Site.SelectedEntities);
                cmd.Execute();
                Diagram.Site.UndoredoManager.Do(cmd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            Diagram.Site.SelectedEntities.Clear();
        }
    }
}
