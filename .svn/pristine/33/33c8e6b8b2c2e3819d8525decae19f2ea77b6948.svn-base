using System.Drawing;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда удаления объекта из схемы
    /// </summary>
    public class CommandDeletFromScheme : DiagramEditorCommand
    {
        public CommandDeletFromScheme(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgDeletFromScheme];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            // Удаляем объект из схемы
            foreach (DiagramEntity item in Diagram.Site.SelectedEntities)
            {
                Diagram.Site.SchemeEditor.DeleteObject(item.Key);
                if (Diagram.Site.SchemeEditor.Scheme.GetObjectByKey(item.Key) == null)
                {
                    // Удаляем символ
                    item.RemoveEntity();
                }
            }

            Diagram.Site.SelectedEntities.Clear();
        }
    }
}
