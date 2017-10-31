using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Krista.FM.Client.MDXExpert.Grid.Style;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// «аполн€ет свободное место возникающие в случае присутстви€ в таблице колонок и показателей, и отсутствии
    /// строк
    /// </summary>
    public class MeasuresStub : GridCell
    {
        private Rectangle _visibleBounds = Rectangle.Empty;

        public MeasuresStub(ExpertGrid grid)
            : base(grid, GridObject.MeasureStub)
        {
            this.Style = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);
            this.Style.StringFormat.Alignment = StringAlignment.Center;
            //если текст не помещаетс€ в €чейку, обрезаем его и ставим многоточие
            this.Style.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
        }

        public override void OnClick(System.Drawing.Point mousePosition)
        {
        }

        public override void OnPaint(System.Drawing.Graphics graphics, Painter painter)
        {
            painter.DrawCaption(graphics, this.Bounds, base.Style, base.State, base.Text, this.Bounds);
        }

        /// <summary>
        /// ≈сли нажат дочерний элемент контрола
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool IsClickChildElement(Point point)
        {
            return false;
        }

        /// <summary>
        /// ≈сли требуетс€ рисуем заглушку
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public void Draw(System.Drawing.Graphics graphics, Painter painter)
        {
            if (this.IsVisible)
            {
                this.OnPaint(graphics, painter);
            }
        }

        /// <summary>
        /// ќтносительные координаты, у заглушки показателей всегда равны абсолютным
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Rectangle GetOffsetBounds()
        {
            return this.Bounds;
        }

        public override Rectangle GetVisibleBounds()
        {
            return this.Bounds;
        }

        /// <summary>
        /// ѕолучить видимую область родител€
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetParentVisibleBounds()
        {
            return this.GetVisibleBounds();
        }

        /// <summary>
        /// ќчищаем заглушку
        /// </summary>
        public new void Clear()
        {
            base.Text = string.Empty;
        }

        /// <summary>
        /// ѕризнак отображени€ заглушки
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return !this.Grid.ColumnCaptions.IsEmpty && !this.Grid.MeasureCaptionsSections.IsEmpty &&
                    this.Grid.RowCaptions.IsEmpty;
            }
        }

        /// <summary>
        /// √раница
        /// </summary>
        public new Rectangle Bounds
        {
            get
            {
                Point point = new Point(this.Grid.ColumnCaptions.Location.X, 
                    this.Grid.MeasureCaptionsSections.Location.Y);
                Size size = new Size(this.Grid.ColumnCaptions.Width,
                    this.Grid.GridScale.GetScaledValue(this.Grid.MeasureCaptionsSections.Height) + this.Grid.MeasuresData.Height);
                return new Rectangle(point, size);
            }
        }
    }
}
