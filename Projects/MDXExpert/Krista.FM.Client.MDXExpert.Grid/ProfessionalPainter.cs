using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Grid.Painters
{
    public class ProfessionalPainter : Painter
    {
        private Color _highlightColor;
        private Color templateHighlightColor;

        public ProfessionalPainter()
        {
            this.HighlightColor = Color.FromArgb(242, 192, 104);
        }

        public ProfessionalPainter(Color highlightColor)
        {
            this.HighlightColor = highlightColor;
        }

        public override void DrawCollapseButton(Graphics graphics, Rectangle bounds, bool isExpanded)
        {
            bounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
            RoundRectangleShape shape = new RoundRectangleShape();
            shape.Round = 0.3f;
            using (GraphicsPath path = shape.CreatePath(bounds))
            {
                if ((bounds.Size.Height > 0.1) && (bounds.Size.Width > 0.1))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.White, SystemColors.Control, 45f))
                    {
                        Blend blend = new Blend(1);
                        blend.Factors[0] = 0.9f;
                        blend.Positions[0] = 0.7f;
                        brush.Blend = blend;
                        graphics.FillPath(brush, path);
                    }
                }
                graphics.DrawPath(SystemPens.ControlDark, path);
            }
            using (Pen pen = new Pen(Color.Black, 1f))
            {
                RectangleF scaleRectangle = Painter.GetScaleRectangle(bounds, (float)0.6f);
                if (isExpanded)
                {
                    Painter.DrawMinus(graphics, scaleRectangle, pen);
                }
                else
                {
                    Painter.DrawPlus(graphics, scaleRectangle, pen);
                }
            }
        }

        public override void DrawMeasureCell(Graphics graphics, Rectangle bounds, ControlState state, CellStyle cellStyle, string text)
        {
            DrawControl(graphics, bounds, state, cellStyle);
            DrawText(graphics, bounds, text, cellStyle);
        }

        private void DrawControl(Graphics graphics, Rectangle bounds, ControlState state, CellStyle cellStyle)
        {            
            Color bkColorEnd = cellStyle.BackColorEnd;
            Color bkColoStart = cellStyle.BackColorStart;
            float angle = 90f;
            if (state == ControlState.Hot)
            {
                bkColorEnd = this.HighlightColor;
            }
            if (state == ControlState.Selected)
            {
                //angle = 270f;
                bkColorEnd = Color.FromArgb(150, this.HighlightColor);
                bkColoStart = bkColorEnd;
            }

            if ((state == ControlState.Normal) && (bkColorEnd == bkColoStart))
            {
                graphics.FillRectangle(cellStyle.BackColorBrush, bounds);
            }
            else
            {
                if ((bounds.Size.Height > 0.1) && (bounds.Size.Width > 0.1))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, bkColoStart, bkColorEnd, angle))
                    {
                        Blend blend = new Blend(1);
                        blend.Factors[0] = 1.4f;
                        blend.Positions[0] = 0.7f;
                        brush.Blend = blend;
                        graphics.FillRectangle(brush, bounds);
                    }
                }
            }

            DrawBorder(graphics, bounds, cellStyle.BorderPen);
        }

        public override void DrawDimensionCell(Graphics graphics, Rectangle bounds, ControlState state,
            CellStyle cellStyle, string text, Rectangle textBounds)
        {
            this.DrawControl(graphics, bounds, state, cellStyle);
            Painter.DrawText(graphics, textBounds, text, cellStyle);
        }

        /// <summary>
        /// Получить изображение ячейки с указанным стилем, границей и текстом
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="cellStyle"></param>
        /// <param name="text"></param>
        public Image GetImageCell(Size size, CellStyle cellStyle, string text)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);

            using (Graphics bmpGraphics = Graphics.FromImage(bitmap))
            {
                Rectangle cellBounds = new Rectangle(0, 0, size.Width - 1, size.Height - 1);
                Rectangle textBounds = new Rectangle(0, 0, size.Width, size.Height);
                this.DrawDimensionCell(bmpGraphics, cellBounds, ControlState.Normal, cellStyle, text, textBounds);
            }

            return bitmap;
        }

        public override void DrawSortButton(Graphics graphics, RectangleF bounds, Krista.FM.Client.MDXExpert.Data.SortType sortOrder)
        {
            Painter.DrawSort(graphics, bounds, sortOrder);
        }

        public override void DrawDropButton(Graphics graphics, Rectangle bounds, ControlState state, bool select, CellStyle cellStyle, GridScale scale)
        {
            Brush brush = select ? Brushes.Blue : Brushes.Black;
            bounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
            
            RoundRectangleShape shape = new RoundRectangleShape();
            shape.Round = 0.3f;
            using (GraphicsPath path = shape.CreatePath(bounds))
            {
                Color white = Color.White;
                Color backgroundColor = cellStyle.BackColorEnd;
                if (state == ControlState.Hot)
                {
                    backgroundColor = this.HighlightColor;
                }
                float angle = 45f;
                if (state == ControlState.Selected)
                {
                    angle = 225f;
                }
                if ((bounds.Size.Height > 0.1) && (bounds.Size.Width > 0.1))
                {
                    using (LinearGradientBrush brush2 = new LinearGradientBrush(bounds, white, backgroundColor, angle))
                    {
                        Blend blend = new Blend(1);
                        blend.Factors[0] = 0.9f;
                        blend.Positions[0] = 0.7f;
                        brush2.Blend = blend;
                        graphics.FillPath(brush2, path);
                    }
                }
                graphics.DrawPath(SystemPens.ControlDark, path);
            }

            Painter.DrawDimensionBranch(graphics, bounds, scale);
            /*RectangleF scaleRectangle = Painter.GetScaleRectangle(bounds, (float)0.6f);
            scaleRectangle.Y++;
            scaleRectangle.Height -= 2f;
            Painter.FillTriangle(graphics, scaleRectangle, brush, Direction.TopDown);
            Painter.DrawTriangle(graphics, scaleRectangle, Krista.FM.Client.MDXExpert.Data.SortType.DESC);*/
        }
        
        public override void DrawCaption(Graphics graphics, Rectangle bounds, CellStyle cellStyle, ControlState state, bool enabledSort, string text, Size clientSize)
        {
            Rectangle empty = Rectangle.Empty;
            if (enabledSort)
            {
                empty = new Rectangle((bounds.Left + clientSize.Width) - 12, (bounds.Top + (bounds.Size.Height / 2)) - 5, 8, 8);
            }
            this.DrawControl(graphics, bounds, state, cellStyle);
            Painter.DrawText(graphics, new Rectangle(bounds.X, bounds.Y, (clientSize.Width - empty.Width) - 2, clientSize.Height), text, cellStyle);
            //Painter.DrawTriangle(graphics, empty, sortOrder);
        }

        public override void DrawCaption(Graphics graphics, Rectangle bounds, CellStyle cellStyle, ControlState state, string text, Rectangle textBounds)
        {
            this.DrawControl(graphics, bounds, state, cellStyle);
            Painter.DrawText(graphics, textBounds, text, cellStyle);
        }

        public override void DrawShiftButton(Graphics graphics, Rectangle bounds, Direction direct)
        {
            RectangleF scaleRectangle = Painter.GetScaleRectangle(bounds, (float)0.7f);
            Painter.FillTriangle(graphics, scaleRectangle, Brushes.Black, direct);
            Painter.DrawTriangle(graphics, scaleRectangle, Brushes.Gray, direct);
        }

        public override void DrawVSplitterLine(Graphics graphics, Rectangle bounds)
        {
            using (Pen pen = new Pen(Brushes.DarkGray, 1f))
            {
                pen.DashStyle = DashStyle.Dot;
                graphics.DrawLine(pen, bounds.Left - 1, bounds.Top, bounds.Left - 1, bounds.Bottom);
                graphics.DrawLine(pen, bounds.Left + 1, bounds.Top, bounds.Left + 1, bounds.Bottom);
                pen.DashOffset++;
                graphics.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            }
        }

        public override void DrawSelectionFrame(Graphics graphics, Rectangle bounds)
        {
            using (Pen pen = new Pen(Brushes.Black, 1f))
            {
                pen.DashStyle = DashStyle.Dot;
                //graphics.FillRectangle(Brushes.);
                graphics.DrawRectangle(pen, bounds);
                //pen.DashOffset++;
                graphics.DrawRectangle(pen, bounds.Left - 1, bounds.Top - 1, bounds.Width + 2, bounds.Height + 2);
                graphics.DrawRectangle(pen, bounds.Left + 1, bounds.Top + 1, bounds.Width - 2, bounds.Height - 2);
            }
        }


        public override void DrawHSplitterLine(Graphics graphics, Rectangle bounds)
        {
            using (Pen pen = new Pen(Brushes.DarkGray, 1f))
            {
                pen.DashStyle = DashStyle.Dot;
                graphics.DrawLine(pen, bounds.Left, bounds.Top - 1, bounds.Right, bounds.Top - 1);
                graphics.DrawLine(pen, bounds.Left, bounds.Top + 1, bounds.Right, bounds.Top + 1);
                pen.DashOffset++;
                graphics.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top); ;
            }
        }

        public override void DrawComment(Graphics graphics, Rectangle bounds, CellStyle cellStyle, string text)
        {
            Rectangle borderRectangle = bounds;
            borderRectangle.Width -= 1;
            borderRectangle.Height -= 1;

            Rectangle textRectangle = bounds;
            textRectangle.Location = new Point(textRectangle.Location.X + 4, textRectangle.Location.Y);
            textRectangle.Width -= 4;

            graphics.FillRectangle(cellStyle.BackColorBrush, bounds);
            Painter.DrawText(graphics, textRectangle, text, cellStyle);
            Painter.DrawBorder(graphics, borderRectangle, cellStyle.BorderPen);
        }

        public override void DrawTitle(Graphics graphics, Rectangle bounds, CellStyle cellStyle, string text)
        {
            this.DrawControl(graphics, bounds, ControlState.Normal, cellStyle);
            Painter.DrawText(graphics, bounds, text, cellStyle);
        }

        public override string ToString()
        {
            return "ProfessionalPainter";
        }

        /// <summary>
        /// Загружаем настройки рисовальщика
        /// </summary>
        /// <param name="painterNode"></param>
        /// <param name="isLoadTemplate"></param>
        public void Load(XmlNode painterNode, bool isLoadTemplate)
        {
            if (painterNode == null)
                return;

            XmlNode colorsNode = painterNode.SelectSingleNode(GridConsts.colors);
            if (colorsNode != null)
            {
                ColorConverter colorConvertor = new ColorConverter();
                string color;
                color = XmlHelper.GetStringAttrValue(colorsNode, GridConsts.highlightColor, string.Empty);
                if (color != string.Empty)
                {
                    this.HighlightColor = (Color)colorConvertor.ConvertFromString(color);
                }
            }
            if (isLoadTemplate)
                this.templateHighlightColor = this.HighlightColor;
        }


        /// <summary>
        /// Сохраняем настройки рисовальщика
        /// </summary>
        /// <param name="captionsNode"></param>
        public void Save(XmlNode painterNode)
        {
            if (painterNode == null)
                return;
            //Сохранение цветов
            XmlNode colorsNode = XmlHelper.AddChildNode(painterNode, GridConsts.colors);
            if (this.templateHighlightColor != this.HighlightColor)
            {
                ColorConverter colorConvertor = new ColorConverter();
                XmlHelper.SetAttribute(colorsNode, GridConsts.highlightColor,
                    colorConvertor.ConvertToString(this.HighlightColor));
            }
        }

        public Color HighlightColor
        {
            get
            {
                return this._highlightColor;
            }
            set
            {
                this._highlightColor = value;
            }
        }
    }
}


