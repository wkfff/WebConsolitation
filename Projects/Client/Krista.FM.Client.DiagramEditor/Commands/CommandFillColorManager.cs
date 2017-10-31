using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда управления сменой фона
    /// </summary>
    public class CommandFillColorManager : DiagramEditorCommand
    {
        public CommandFillColorManager(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgFillColor];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            Color color = Diagram.Site.CmdHelper.ColorDialog();

            if (color.IsEmpty)
            {
                return;
            }

            foreach (DiagramEntity entity in Diagram.Site.SelectedEntities)
            {
                if (entity is DiagramRectangleEntity)
                {
                    if (entity.FillColor != color)
                    {
                        // создаем команду
                        Command cmdFillColor = new CommandChangeFillColor(Diagram, entity, entity.FillColor, color);

                        // выполняем операцию
                        cmdFillColor.Execute();

                        // если команда обратима, то...
                        if (cmdFillColor is IUndoCommand)
                        {
                            // добавляем в стек
                            Diagram.Site.UndoredoManager.Do(cmdFillColor);
                        }
                    }
                }
            }

            Diagram.Site.Invalidate();
        }
    }
}
