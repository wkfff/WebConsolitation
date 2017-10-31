using System;
using System.Drawing;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Базовый класс для объекта Пакет на диаграмме
    /// </summary>
    public class UMLPackage : DiagramRectangleEntity
    {
        #region Constructor

        public UMLPackage(string key, Guid id, AbstractDiagram diagram, int x, int y)
            : this(key, id, diagram, x, y, Color.AliceBlue)
        {
        }

        public UMLPackage(string key, Guid id, AbstractDiagram diagram, int x, int y, Color color)
            : base(key, id, diagram, x, y, color)
        {
        }
        #endregion

        #region Methods

        public override void RemoveEntity()
        {
            Diagram.Entities.Remove(this);
            Diagram.Site.Invalidate();
        }

        public override void CreateHeaderTextBox(Point point)
        {
            Size scrollOffset = new Size(Diagram.Site.AutoScrollPosition);

            base.CreateHeaderTextBox(point);

            HeaderText.Text = Text;

            HeaderText.Location = new Point(
                this.X + (IndentTextBox / 2) + scrollOffset.Width,
                this.Y + (this.Height / 2) + scrollOffset.Height - 5);

            HeaderText.BackColor = FillColor;

            HeaderText.Width = EntityRectangle.Width - 10;

            // высотта текста
            HeaderText.Height = this.Height + IndentTextBox;
        }

        /// <summary>
        /// Метод отрисовки пакета
        /// </summary>
        public override void Draw(System.Drawing.Graphics g, Size scrollOffset)
        {
            SolidBrush fillBrush = new SolidBrush(FillColor);
            SolidBrush textBrush = new SolidBrush(TextColor);
            try
            {
                // задаем параметры карандаша
                Pen.Color = LineColor;
                Pen.Width = LineWidth;

                Rectangle top = new Rectangle(EntityRectangle.X, EntityRectangle.Y, EntityRectangle.Width / 2, 20);
                Rectangle bottom = new Rectangle(EntityRectangle.X, EntityRectangle.Y + 20, EntityRectangle.Width, EntityRectangle.Height - 20);
                top.Offset(scrollOffset.Width, scrollOffset.Height);
                bottom.Offset(scrollOffset.Width, scrollOffset.Height);

                g.FillRectangle(fillBrush, top);
                g.FillRectangle(fillBrush, bottom);
                g.DrawRectangle(Pen, top);
                g.DrawRectangle(Pen, bottom);

                g.DrawString(Text, Font, textBrush, bottom, Format);
                SetMinSize(g, false);
            }
            finally
            {
                fillBrush.Dispose();
                textBrush.Dispose();
            }
        }
    
        #endregion
    }
}
