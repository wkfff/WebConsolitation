using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Krista.FM.Client.MDXExpert.Grid
{
    public class RoundRectangleShape
    {
        private float round = 0.2f;

        public object Clone()
        {
            RoundRectangleShape shape = new RoundRectangleShape();
            shape.round = this.round;
            return shape;
        }

        public GraphicsPath CreatePath(RectangleF rect)
        {
            GraphicsPath path = new GraphicsPath();
            float num = Math.Min(rect.Height, rect.Width) * this.round;
            path.StartFigure();
            path.AddLine(rect.Left + num, rect.Top, rect.Right - num, rect.Top);
            path.AddArc(rect.Right - (2f * num), rect.Top, 2f * num, 2f * num, -90f, 90f);
            path.AddLine(rect.Right, rect.Top + num, rect.Right, rect.Bottom - num);
            path.AddArc((float) (rect.Right - (2f * num)), (float) (rect.Bottom - (2f * num)), (float) (2f * num), (float) (2f * num), 0f, 90f);
            path.AddLine(rect.Right - num, rect.Bottom, rect.Left + num, rect.Bottom);
            path.AddArc(rect.Left, rect.Bottom - (2f * num), 2f * num, 2f * num, 90f, 90f);
            path.AddLine(rect.Left, rect.Bottom - num, rect.Left, rect.Top + num);
            path.AddArc(rect.Left, rect.Top, 2f * num, 2f * num, 180f, 90f);
            return path;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (base.GetType() != obj.GetType()))
            {
                return false;
            }
            RoundRectangleShape shape = (RoundRectangleShape) obj;
            return (shape.round == this.round);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public float Round
        {
            get
            {
                return this.round;
            }
            set
            {
                this.round = value;
            }
        }
    }
}

