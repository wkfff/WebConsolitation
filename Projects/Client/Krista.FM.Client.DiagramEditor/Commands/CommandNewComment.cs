using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Обратимая команда
    /// </summary>
    public class CommandNewComment : DiagramEditorCommand, IUndoCommand
    {
        private UMLLabel label;

        private Point location;
        
        public CommandNewComment(AbstractDiagram diagram)
            : base(diagram)
        {
            this.Image = Diagram.Site.ImageList[Images.imgNewComment];
            this.Image.MakeTransparent(Color.FromArgb(transparentColor));
        }

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public override void Execute()
        {
            Point point = ScaleTransform.TransformPoint(location, Diagram.Site.ZoomFactor);
            this.label = new UMLLabel(Guid.NewGuid(), Diagram, point.X, point.Y);
            Diagram.Entities.Add(label);

            Diagram.IsChanged = true;
        }

        #region IUndoCommand Members

        public void Undo()
        {
            // Отмена
            if (label != null)
            {
                if (Diagram.Entities.Contains(label))
                {
                    location = label.Location;

                    Diagram.Entities.Remove(label);
                    Diagram.Site.Invalidate();
                }
            }
        }

        #endregion
    }
}
