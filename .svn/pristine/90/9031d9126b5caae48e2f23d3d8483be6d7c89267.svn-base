using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Класс для регистрации и отката команды смены цвета заливки
    /// </summary>
    public class CommandChangeFillColor : ICommand
    {
        DiagramEditor.DiargamEditor site;

        ColorEntity colorEntity;

        public CommandChangeFillColor(DiagramEditor.DiargamEditor site, ColorEntity colorEntity)
        {
            this.site = site;
            this.colorEntity = colorEntity;
        }

        #region ICommand Members

        public void Redo()
        {
            Change(colorEntity.color_after);
        }

        public void Undo()
        {
            Change(colorEntity.color_before);
        }

        private void Change(Color color)
        {
            DiagramRectangleEntity entity = site.Diagram.FindDiagramEntityByID(colorEntity.id) as DiagramRectangleEntity;

            if (entity != null)
            {
                entity.FillColor = color;

                site.Invalidate();
            }
 
        }
        #endregion
    }

    public class ColorEntity
    {
        public Color color_before;

        public Color color_after;

        public Guid id;

        public ColorEntity(Color color_before, Color color_after, Guid id)
        {
            this.color_before = color_before;
            this.color_after = color_after;
            this.id = id;
        }
    }
}
