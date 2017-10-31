using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Krista.FM.Client.MDXExpert.Grid.Style;

namespace Krista.FM.Client.MDXExpert.Grid
{
    public abstract class Painter
    {
        protected Painter()
        {
        }

        public abstract void DrawCollapseButton(Graphics graphics, Rectangle bounds, bool isExpanded);
        public abstract void DrawDimensionCell(Graphics graphics, Rectangle bounds, ControlState state,
            CellStyle cellStyle, string text, Rectangle textBounds);
        public abstract void DrawDropButton(Graphics graphics, Rectangle bounds, ControlState state, 
            bool select, CellStyle cellStyle, GridScale scale);

        public abstract void DrawComment(Graphics graphics, Rectangle bounds, CellStyle cellStyle, string text);
        public abstract void DrawCaption(Graphics graphics, Rectangle bounds, CellStyle cellStyle, 
            ControlState state, bool enabledSort, string text, Size clientSize);
        public abstract void DrawCaption(Graphics graphics, Rectangle bounds, CellStyle cellStyle, 
            ControlState state, string text, Rectangle textBounds);

        public abstract void DrawSortButton(Graphics graphics, RectangleF bounds, Krista.FM.Client.MDXExpert.Data.SortType sortOrder);

        public abstract void DrawShiftButton(Graphics graphics, Rectangle bounds, Direction direct);
        public abstract void DrawVSplitterLine(Graphics graphics, Rectangle bounds);
        public abstract void DrawHSplitterLine(Graphics graphics, Rectangle bounds);

        public abstract void DrawSelectionFrame(Graphics graphics, Rectangle bounds);


        protected static void DrawBorder(Graphics graphics, Rectangle bounds, Pen pen)
        {
            graphics.DrawRectangle(pen, bounds);
        }

        protected static void DrawMinus(Graphics graphics, RectangleF bounds, Pen pen)
        {
            float num = bounds.Location.Y + (bounds.Height / 2f);
            graphics.DrawLine(pen, bounds.Left, num, bounds.Right, num);
        }

        protected static void DrawPlus(Graphics graphics, RectangleF bounds, Pen pen)
        {
            float num = bounds.Location.Y + (bounds.Height / 2f);
            float num2 = bounds.Location.X + (bounds.Width / 2f);
            graphics.DrawLine(pen, bounds.Left, num, bounds.Right, num);
            graphics.DrawLine(pen, num2, bounds.Top, num2, bounds.Bottom);
        }

        protected static void DrawText(Graphics graphics, RectangleF bounds, string text, CellStyle style)
        {
            graphics.DrawString(text, style.Font, style.ForeColorBrush, bounds, style.StringFormat);
        }

        protected static void DrawTextCell(Graphics graphics, Rectangle bounds, string text, CellStyle style)
        {
            DrawTextCell(graphics, bounds, bounds, text, style);
        }

        protected static void DrawTextCell(Graphics graphics, Rectangle bounds, Rectangle textBounds, 
            string text, CellStyle style)
        {
            if (style.Gradient == Gradient.None)
            {
                graphics.FillRectangle(style.BackColorBrush, bounds);
            }
            else
            {
                using (Brush brush = GetBackBrush(bounds, style))
                {
                    graphics.FillRectangle(brush, bounds);
                }
            }

            DrawText(graphics, textBounds, text, style);
        }

        public abstract void DrawMeasureCell(Graphics graphics, Rectangle bounds, ControlState state, 
            CellStyle cellStyle, string text);

        public abstract void DrawTitle(Graphics graphics, Rectangle bounds, CellStyle cellStyle, string text);

        protected static void DrawSort(Graphics graphics, RectangleF bounds, Krista.FM.Client.MDXExpert.Data.SortType sortOrder)
        {
            GraphicsState gstate = graphics.Save();
            if (sortOrder != Krista.FM.Client.MDXExpert.Data.SortType.None)
            {
                PointF center = GetCenter(bounds);
                graphics.TranslateTransform(center.X, center.Y);
            }
            switch (sortOrder)
            {
                case Krista.FM.Client.MDXExpert.Data.SortType.ASC:
                case Krista.FM.Client.MDXExpert.Data.SortType.BASC:
                    {
                        PointF[] tfArray2 = GetTriangle(bounds.Width, bounds.Height);
                        graphics.DrawLine(Pens.White, tfArray2[0], tfArray2[1]);
                        graphics.DrawLine(Pens.Gray, tfArray2[1], tfArray2[2]);
                        graphics.DrawLine(Pens.Gray, tfArray2[2], tfArray2[0]);
                        break;
                    }
                case Krista.FM.Client.MDXExpert.Data.SortType.DESC:
                case Krista.FM.Client.MDXExpert.Data.SortType.BDESC:
                    {
                        graphics.RotateTransform(180f);
                        PointF[] triangle = GetTriangle(bounds.Width, bounds.Height - 1f);
                        graphics.DrawLine(Pens.White, triangle[0], triangle[1]);
                        graphics.DrawLine(Pens.Gray, triangle[1], triangle[2]);
                        graphics.DrawLine(Pens.Gray, triangle[2], triangle[0]);
                        break;
                    }
                case Krista.FM.Client.MDXExpert.Data.SortType.None:
                    {
                        PointF[] rectangle = GetRectanglePoint(bounds);
                        graphics.DrawLine(Pens.White, rectangle[0], rectangle[1]);
                        graphics.DrawLine(Pens.Gray, rectangle[1], rectangle[2]);
                        graphics.DrawLine(Pens.Gray, rectangle[2], rectangle[3]);
                        graphics.DrawLine(Pens.White, rectangle[3], rectangle[0]);
                        break;
                    }
            }
            graphics.Restore(gstate);
        }

        protected static void DrawDimensionBranch(Graphics graphics, RectangleF bounds, GridScale scale)
        {
            GraphicsState gstate = graphics.Save();
            //PointF center = GetCenter(bounds);
            
            graphics.DrawRectangle(Pens.Gray, bounds.X + scale.GetScaledValue(3), bounds.Y + scale.GetScaledValue(3), scale.GetScaledValue(4), scale.GetScaledValue(2));
            graphics.DrawRectangle(Pens.Gray, bounds.X + scale.GetScaledValue(6), bounds.Y + scale.GetScaledValue(7), scale.GetScaledValue(4), scale.GetScaledValue(2));
            graphics.DrawLine(Pens.Gray, bounds.X + scale.GetScaledValue(4), bounds.Y + scale.GetScaledValue(5), bounds.X + scale.GetScaledValue(4), bounds.Y + scale.GetScaledValue(10));
            graphics.DrawLine(Pens.Gray, bounds.X + scale.GetScaledValue(4), bounds.Y + scale.GetScaledValue(8), bounds.X + scale.GetScaledValue(6), bounds.Y + scale.GetScaledValue(8));
            
            graphics.Restore(gstate);
        }


        protected static void DrawTriangle(Graphics graphics, RectangleF bounds, Brush brush, Direction direct)
        {
            GraphicsState gstate = graphics.Save();
            PointF center = GetCenter(bounds);
            graphics.TranslateTransform(center.X, center.Y);
            graphics.RotateTransform(GetAngle(direct));
            using (Pen pen = new Pen(brush))
            {
                graphics.DrawPolygon(pen, GetTriangle(bounds.Width, bounds.Height));
            }
            graphics.Restore(gstate);
        }

        protected static void FillTriangle(Graphics graphics, RectangleF bounds, Brush brush, Direction direct)
        {
            GraphicsState gstate = graphics.Save();
            PointF center = GetCenter(bounds);
            graphics.TranslateTransform(center.X, center.Y);
            graphics.RotateTransform(GetAngle(direct));
            graphics.FillPolygon(brush, GetTriangle(bounds.Width, bounds.Height));
            graphics.Restore(gstate);
        }

        protected static float GetAngle(Direction direct)
        {
            switch (direct)
            {
                case Direction.RightToLeft:
                    return -90f;

                case Direction.LeftToRight:
                    return 90f;

                case Direction.TopDown:
                    return 180f;

                case Direction.BottomUp:
                    return 0f;
            }
            return 0f;
        }
        
        private static Brush GetBackBrush(RectangleF bounds, CellStyle style)
        {
            if (((style.Gradient == Gradient.None) || (bounds.Height < 1f)) || (bounds.Width < 1f))
            {
                return new SolidBrush(style.BackColorEnd);
            }
            return new LinearGradientBrush(bounds, style.BackColorStart, style.BackColorEnd, 
                (style.Gradient == Gradient.Horizontal) ? LinearGradientMode.Horizontal : LinearGradientMode.Vertical);
        }

        public virtual float GetCaptionStringWidth(Graphics graphics, string text)
        {
            float width;
            using (Graphics graphics2 = Graphics.FromHwnd(IntPtr.Zero))
            {
                using (Font font = new Font("Arial", 10f))
                {
                    width = graphics2.MeasureString(text, font).Width;
                }
            }
            return width;
        }

        protected static PointF GetCenter(RectangleF bounds)
        {
            return new PointF(bounds.Location.X + (bounds.Width / 2f), bounds.Location.Y + (bounds.Height / 2f));
        }

        private static StringAlignment GetHStringAligmen(ContentAlignment aligment)
        {
            if (((aligment == ContentAlignment.TopRight) || (aligment == ContentAlignment.MiddleRight)) || (aligment == ContentAlignment.BottomRight))
            {
                return StringAlignment.Far;
            }
            if (((aligment != ContentAlignment.TopLeft) && (aligment != ContentAlignment.MiddleLeft)) && (aligment != ContentAlignment.BottomLeft))
            {
                return StringAlignment.Center;
            }
            return StringAlignment.Near;
        }

        protected static RectangleF GetScaleRectangle(RectangleF rect, PointF scale)
        {
            PointF location = new PointF(rect.Location.X + ((rect.Width / 2f) * (1f - scale.X)), rect.Location.Y + ((rect.Height / 2f) * (1f - scale.Y)));
            return new RectangleF(location, new SizeF(rect.Width * scale.X, rect.Height * scale.Y));
        }

        protected static RectangleF GetScaleRectangle(RectangleF rect, float scale)
        {
            return GetScaleRectangle(rect, new PointF(scale, scale));
        }

        protected static PointF[] GetTriangle(float width, float height)
        {
            return new PointF[] { new PointF(0f, -height / 2f), new PointF(-width / 2f, height / 2f), new PointF(width / 2f, height / 2f) };
        }

        protected static PointF[] GetRectanglePoint(RectangleF rectangle)
        {
            return new PointF[] { rectangle.Location, new PointF(rectangle.Right, rectangle.Y), 
                new PointF(rectangle.Right, rectangle.Bottom), new PointF(rectangle.X, rectangle.Bottom) };
        }

        private static StringAlignment GetVStringAligmen(ContentAlignment aligment)
        {
            if (((aligment == ContentAlignment.BottomCenter) || (aligment == ContentAlignment.BottomLeft)) || (aligment == ContentAlignment.BottomRight))
            {
                return StringAlignment.Far;
            }
            if (((aligment != ContentAlignment.TopCenter) && (aligment != ContentAlignment.TopLeft)) && (aligment != ContentAlignment.TopRight))
            {
                return StringAlignment.Center;
            }
            return StringAlignment.Near;
        }
    }
}