using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Команда перемещения объекта
    /// </summary>
    class CommandChangeLocation : ICommand
    {
        private Guid id;

        private Point point_before;
        private Point point_after;

        private DiagramEditor.DiargamEditor site;

        public CommandChangeLocation(DiagramEditor.DiargamEditor site, Guid id, Point point_before, Point point_after)
        {
            this.site = site;
            this.id = id;
            this.point_before = point_before;
            this.point_after = point_after;
        }
        #region ICommand Members

        public void Redo()
        {
            Change(point_after);
        }

        public void Undo()
        {
            Change(point_before);
        }

        private void Change(Point location)
        {
            DiagramRectangleEntity entity = site.Diagram.FindDiagramEntityByID(id) as DiagramRectangleEntity;

            if (entity != null)
            {
                entity.Location = location;
                site.Invalidate();
            }
        }

        #endregion
    }
}
