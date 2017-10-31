using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда "Авторазмер"
    /// </summary>
    public class CommandAutoSize : DiagramEditorCommand
    {
        public CommandAutoSize(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgAutoSize];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public override void Execute()
        {
            foreach (DiagramEntity entity in Diagram.Site.SelectedEntities)
            {
                if (entity is DiagramRectangleEntity)
                {
                    ((DiagramRectangleEntity)entity).AutoSizeRec();
                }
            }
        }
    }
}
