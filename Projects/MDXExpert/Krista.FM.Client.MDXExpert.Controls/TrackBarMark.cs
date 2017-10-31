using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Controls
{
    [Serializable]
    public class TrackBarMark
    {
        private bool isActive = false;
        private bool isMoving = false;
        private Point[] regionPoints = new Point[5];
        private int width = 8;		
        private int height = 24;
        private int position;
        private Color colorShadowDark = Color.FromKnownColor(KnownColor.ControlDarkDark);
        private Pen penShadowLight = new Pen(Color.FromKnownColor(KnownColor.ControlLightLight));
        private Pen penShadowDark = new Pen(Color.FromKnownColor(KnownColor.ControlDarkDark));
        private SolidBrush brushRange = new SolidBrush(Color.FromKnownColor(KnownColor.Control));
        private double value;
        private RangeTrackBar trackBar;
        private int number;


        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                SetActive(value);
            }
        }

        public int Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.value = PosToValue(value);
                SetPosition(value);
            }
        }

        public double Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.position = ValueToPos(value);
                SetValue(value);
            }
        }

        public bool IsMoving
        {
            get { return this.isMoving; }
            set { this.isMoving = value; }
        }


        public TrackBarMark(RangeTrackBar parentTrackBar, double value, int number)
        {
            this.trackBar = parentTrackBar;
            this.Value = value;
            this.number = number; 
        }

        public Rectangle ClientArea
        {
            get { return GetClientArea(); }
        }

        public void Draw(Graphics g, int markyoff, bool isVertical)
        {
            if (isVertical)
            {
                DrawVerticalMark(g, markyoff);
            }
            else
            {
                DrawHorizontalMark(g, markyoff);
            }
        }

        private void DrawVerticalMark(Graphics g, int markyoff)
        {
            regionPoints[0].X = this.Position - this.Width;         regionPoints[0].Y = markyoff;
            regionPoints[1].X = this.Position;                      regionPoints[1].Y = markyoff;
            regionPoints[2].X = this.Position;                      regionPoints[2].Y = markyoff + 2 * this.trackBar.MarkHeight / 3;
            regionPoints[3].X = this.Position - this.Width / 2;     regionPoints[3].Y = markyoff + this.trackBar.MarkHeight;
            regionPoints[4].X = this.Position - this.Width;         regionPoints[4].Y = markyoff + 2 * this.trackBar.MarkHeight / 3;

            g.FillPolygon(brushRange, regionPoints);
            g.DrawLine(penShadowLight, regionPoints[3].X - 1, regionPoints[3].Y, regionPoints[4].X - 1, regionPoints[4].Y); 
            g.DrawLine(penShadowLight, regionPoints[0].X - 1, regionPoints[0].Y, regionPoints[4].X - 1, regionPoints[4].Y); 				
            g.DrawLine(penShadowLight, regionPoints[0].X, regionPoints[0].Y, regionPoints[1].X - 1, regionPoints[1].Y);
            g.DrawLine(penShadowDark, regionPoints[1].X, regionPoints[1].Y + 1, regionPoints[1].X, regionPoints[2].Y);
            g.DrawLine(penShadowDark, regionPoints[2].X, regionPoints[2].Y + 1, regionPoints[3].X, regionPoints[3].Y);

            if (this.IsActive)
            {
                g.DrawLine(penShadowLight, this.Position - this.Width / 2 - 1,
                                            markyoff + this.trackBar.MarkHeight / 3, 
                                            this.Position - this.Width / 2 - 1,
                                            markyoff + 2 * this.trackBar.MarkHeight / 3);

                g.DrawLine(penShadowDark, this.Position - this.Width / 2,
                                            markyoff + this.trackBar.MarkHeight / 3, 
                                            this.Position - this.Width / 2,
                                            markyoff + 2 * this.trackBar.MarkHeight / 3); 			
            }

            Font fontMark = new Font("Arial", this.Width);
            SolidBrush brushMark = new SolidBrush(colorShadowDark);
            StringFormat strformat = new StringFormat();
            strformat.Alignment = StringAlignment.Center;
            strformat.LineAlignment = StringAlignment.Near;


            for (int i = 0; i < this.trackBar.Marks.Count; i++)
            {
                if (this == this.trackBar.Marks[i])
                {
                    g.DrawString((i + 1).ToString(), fontMark, brushMark, this.Position - 4, markyoff - 12, strformat);
                    break;
                }
            }



        }

        private void DrawHorizontalMark(Graphics g, int markyoff)
        {
            regionPoints[0].X = markyoff;                                       regionPoints[0].Y = this.Position - this.Width;
            regionPoints[1].X = markyoff;                                       regionPoints[1].Y = this.Position;
            regionPoints[2].X = markyoff + 2 * this.trackBar.MarkHeight / 3;    regionPoints[2].Y = this.Position;
            regionPoints[3].X = markyoff + this.trackBar.MarkHeight;            regionPoints[3].Y = this.Position - this.Width / 2;
            regionPoints[4].X = markyoff + 2 * this.trackBar.MarkHeight / 3;    regionPoints[4].Y = this.Position - this.Width;
            g.FillPolygon(brushRange, regionPoints);

            g.DrawLine(penShadowLight, regionPoints[0].X - 1, regionPoints[0].Y, regionPoints[4].X - 1, regionPoints[4].Y); 				
            g.DrawLine(penShadowLight, regionPoints[3].X, regionPoints[3].Y, regionPoints[4].X, regionPoints[4].Y); 				
            g.DrawLine(penShadowLight, regionPoints[0].X - 1, regionPoints[0].Y, regionPoints[1].X - 1, regionPoints[1].Y); 

            g.DrawLine(penShadowDark, regionPoints[1].X, regionPoints[1].Y, regionPoints[2].X, regionPoints[2].Y); 
            g.DrawLine(penShadowDark, regionPoints[3].X, regionPoints[3].Y, regionPoints[2].X, regionPoints[2].Y); 

            if (this.IsActive)
            {
                g.DrawLine(penShadowLight, markyoff + this.trackBar.MarkHeight / 3, this.Position - this.Width / 2, markyoff + 2 * this.trackBar.MarkHeight / 3, this.Position - this.Width / 2);
                g.DrawLine(penShadowDark, markyoff + this.trackBar.MarkHeight / 3, this.Position - this.Width / 2 + 1, markyoff + 2 * this.trackBar.MarkHeight / 3, this.Position - this.Width / 2 + 1); 			
            }

            Font fontMark = new Font("Arial", this.Width);
            SolidBrush brushMark = new SolidBrush(colorShadowDark);
            StringFormat strformat = new StringFormat();
            strformat.Alignment = StringAlignment.Near;
            strformat.LineAlignment = StringAlignment.Center;

            for (int i = 0; i < this.trackBar.Marks.Count; i++)
            {
                if (this == this.trackBar.Marks[i])
                {
                    g.DrawString((i + 1).ToString(), fontMark, brushMark, markyoff - 10, this.Position - 3, strformat);
                    break;
                }
            }

        }

        private Rectangle GetClientArea()
        {
            Rectangle clientArea = new Rectangle(
                Math.Min(regionPoints[0].X, regionPoints[1].X),		// X
                Math.Min(regionPoints[0].Y, regionPoints[3].Y),		// Y
                Math.Abs(regionPoints[2].X - regionPoints[0].X),		// width
                Math.Max(Math.Abs(regionPoints[0].Y - regionPoints[3].Y), Math.Abs(regionPoints[0].Y - regionPoints[1].Y)));	// height

            return clientArea;
        }

        private int ValueToPos(double value)
        {
            int w;
            int posw;

            if (this.trackBar.Orientation == RangeTrackBar.RangeBarOrientation.horizontal)
                w = this.trackBar.Width;
            else
                w = this.trackBar.Height;
            posw = w - this.Width - 2;
            //posw = w - 2 * this.Width - 2;

            if (value < this.trackBar.Minimum)
                value = this.trackBar.Minimum;

            if (value > this.trackBar.Maximum)
                value = this.trackBar.Maximum;

            int position = this.trackBar.XPosMin + (int)Math.Round(posw * (value - this.trackBar.Minimum) / (this.trackBar.Maximum - this.trackBar.Minimum));

            return position;

        }

        private double PosToValue(int pos)
        {
            int w;
            int posw;
             
            if (this.trackBar.Orientation == RangeTrackBar.RangeBarOrientation.horizontal)
                w = this.trackBar.Width;
            else
                w = this.trackBar.Height;

            //posw = w - 2 * this.Width - 2;
            posw = w - this.Width - 2;

            double value = this.trackBar.Minimum + ((double)(this.trackBar.Maximum - this.trackBar.Minimum) * (double)(pos - this.trackBar.XPosMin) / (double)posw);
            value = CommonUtils.SetNumberPrecision(value, this.trackBar.DigitCount);
            return value;
        }

        private void SetActive(bool value)
        {
            if (value)
            {
                foreach(TrackBarMark mark in this.trackBar.Marks)
                {
                    mark.IsActive = false;
                }
            }
            this.isActive = value;
        }

        private void SetPosition(int value)
        {
            for(int i = 0; i < this.trackBar.Marks.Count; i++)
            {
                TrackBarMark mark = this.trackBar.Marks[i];
                if (this != mark) continue;

                if (i == 0)
                {
                    if (mark.Position < this.trackBar.XPosMin) mark.Position = this.trackBar.XPosMin;
                }
                if (i == this.trackBar.Marks.Count - 1)
                {
                    if (mark.Position > this.trackBar.XPosMax) mark.Position = this.trackBar.XPosMax;
                }


                if (i > 0)
                {
                    if (mark.Position < this.trackBar.Marks[i - 1].Position) mark.Position = this.trackBar.Marks[i - 1].Position;
                }

                if (i < this.trackBar.Marks.Count - 1)
                {
                    if (mark.Position > this.trackBar.Marks[i + 1].Position) this.trackBar.Marks[i].Position = this.trackBar.Marks[i + 1].Position;
                }
                this.trackBar.Invalidate(true);
                break;
            }
        }


        private void SetValue(double value)
        {
            for (int i = 0; i < this.trackBar.Marks.Count; i++)
            {
                TrackBarMark mark = this.trackBar.Marks[i];
                if (this != mark) continue;

                if (i == 0)
                {
                    if (mark.Value < this.trackBar.Minimum) mark.Value = CommonUtils.SetNumberPrecision(this.trackBar.Minimum, this.trackBar.DigitCount);
                }
                if (i == this.trackBar.Marks.Count - 1)
                {
                    if (mark.Value > this.trackBar.Maximum) mark.Value = CommonUtils.SetNumberPrecision(this.trackBar.Maximum, this.trackBar.DigitCount);
                }


                if (i > 0)
                {
                    if (mark.Value < this.trackBar.Marks[i - 1].Value) mark.Value = CommonUtils.SetNumberPrecision(this.trackBar.Marks[i - 1].Value, this.trackBar.DigitCount);
                }

                if (i < this.trackBar.Marks.Count - 1)
                {
                    if (mark.Value > this.trackBar.Marks[i + 1].Value) this.trackBar.Marks[i].Value = CommonUtils.SetNumberPrecision(this.trackBar.Marks[i + 1].Value, this.trackBar.DigitCount);
                }
                this.trackBar.Invalidate(true);
                break;
            }
        }


        public void Refresh()
        {
            this.Value = this.value;
        }
    }
}
