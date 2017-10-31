using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Controls
{
	public partial class RangeTrackBar : UserControl
	{
		public delegate void RangeChangedEventHandler(object sender, EventArgs e);

		public delegate void RangeChangingEventHandler(object sender, EventArgs e);

        #region поля

        private Color colorShadowDark = Color.FromKnownColor(KnownColor.ControlDarkDark);
        private Pen penShadowDark = new Pen(Color.FromKnownColor(KnownColor.ControlDarkDark));
        private SolidBrush brushShadowLight = new SolidBrush(Color.FromKnownColor(KnownColor.ControlLightLight));
        private SolidBrush brushShadowDark = new SolidBrush(Color.FromKnownColor(KnownColor.ControlDarkDark));

        private int sizeShadow = 1;
        private double minimum = 0;
        private double maximum = 10;

        private int digitCount = 2;

        public int DigitCount
        {
            get { return digitCount; }
            set 
            { 
                digitCount = value; 
            }
        }

        private RangeBarOrientation barOrientation = RangeBarOrientation.horizontal;
        private TopBottomOrientation scaleOrientation = TopBottomOrientation.bottom;
        private int barHeight = 8;
        private int markWidth = 8;
        private int markHeight = 24;
        private int tickHeight = 6;
        private int numAxisDivision = 10;

        private int xPosMin;
        private int xPosMax;

        private List<TrackBarMark> marks;

        #endregion

        #region свойства

        /// <summary>
        /// высота отметок
        /// </summary>
        public int TickHeight
        {
            set
            {
                tickHeight = Math.Min(Math.Max(1, value), barHeight);
                Invalidate();
                Update();
            }
            get
            {
                return tickHeight;
            }
        }

        /// <summary>
        /// высота бегунков
        /// </summary>
        public int MarkHeight
        {
            set
            {
                markHeight = Math.Max(barHeight + 2, value);
                Invalidate();
                Update();
            }
            get
            {
                return markHeight;
            }
        }

        /// <summary>
        /// высота трекбара
        /// </summary>
        public int BarHeight
        {
            set
            {
                barHeight = Math.Min(value, markHeight - 2);
                Invalidate();
                Update();
            }
            get
            {
                return barHeight;
            }

        }

        /// <summary>
        /// ориентация трекбара(по горизонтали/по вертикали)
        /// </summary>
        public RangeBarOrientation Orientation
        {
            set
            {
                this.barOrientation = value;
                Invalidate();
                Update();
            }
            get
            {
                return this.barOrientation;
            }
        }

        /// <summary>
        /// размещение шкалы относительно трекбара
        /// </summary>
        public TopBottomOrientation ScaleOrientation
        {
            set
            {
                scaleOrientation = value;
                Invalidate();
                Update();
            }
            get
            {
                return scaleOrientation;
            }
        }

        /// <summary>
        /// количество отметок
        /// </summary>
        public int DivisionNum
        {
            set
            {
                numAxisDivision = value;
                Refresh();
            }
            get { return numAxisDivision; }
        }

        /// <summary>
        /// текущее минимальное значение
        /// </summary>
        public double Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                foreach (TrackBarMark mark in this.Marks)
                    mark.Refresh();
                Invalidate(true);
            }
        }

        /// <summary>
        /// текущее максимальное значение
        /// </summary>
        public double Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                foreach (TrackBarMark mark in this.Marks)
                    mark.Refresh();

                Invalidate(true);
            }
        }

        /// <summary>
        /// минимальное положение бегунка на трекбаре(в пикселах)
        /// </summary>
        public int XPosMin
        {
            get { return xPosMin; }
            set { xPosMin = value; }
        }

        /// <summary>
        /// максимальное положение бегунка на трекбаре(в пикселах)
        /// </summary>
        public int XPosMax
        {
            get { return xPosMax; }
            set { xPosMax = value; }
        }

        /// <summary>
        /// бегунки
        /// </summary>
        public List<TrackBarMark> Marks
        {
            get { return marks; }
            set { marks = value; }
        }

        #endregion


		public RangeTrackBar()
		{
			InitializeComponent();
		    this.minimum = 0;
		    this.maximum = 100;
		    this.marks = new List<TrackBarMark>();
		}

        public void SetPrecision(int value)
        {
            this.digitCount = value;
            foreach (TrackBarMark mark in this.Marks)
            {
                mark.Value = CommonUtils.SetNumberPrecision(mark.Value, value);
            }
        }

		public enum RangeBarOrientation { horizontal, vertical };
		public enum TopBottomOrientation { top,bottom,both };

        private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            this.BackColor = SystemColors.Window;
			int h = this.Height;
			int w = this.Width;
			int baryoff,markyoff,tickyoff1,tickyoff2;

			XPosMin = markWidth +1;
			if( this.Orientation==RangeBarOrientation.horizontal )
                XPosMax = w - 1;
                //XPosMax = w - markWidth - 1;
            else
				XPosMax = h - 1;
				//XPosMax = h - markWidth - 1;
			
            //горизонтальный трекбар
			if( this.Orientation==RangeBarOrientation.horizontal )
			{

				baryoff = (h - barHeight)/2;			
				markyoff = baryoff + (barHeight - markHeight)/2 -1;
				
			    DrawRangeBarFrame(e.Graphics, false);

                if (scaleOrientation == TopBottomOrientation.bottom)
                {
                    tickyoff1 = tickyoff2 = baryoff + barHeight + 2;
                }
                else if (scaleOrientation == TopBottomOrientation.top)
                {
                    tickyoff1 = tickyoff2 = baryoff - tickHeight - 4;
                }
                else
                {
                    tickyoff1 = baryoff + barHeight + 2;
                    tickyoff2 = baryoff - tickHeight - 4;
                }

                DrawScale(e.Graphics, false, tickyoff1, tickyoff2);

                foreach(TrackBarMark mark in this.Marks)
                {
                    mark.Draw(e.Graphics, markyoff, true);
                    if (mark.IsMoving)
                    {
                        Font fontMark = new Font("Arial", this.markWidth);
                        SolidBrush brushMark = new SolidBrush(colorShadowDark);
                        StringFormat strformat = new StringFormat();
                        strformat.Alignment = StringAlignment.Center;
                        strformat.LineAlignment = StringAlignment.Near;
                        e.Graphics.DrawString(mark.Value.ToString(), fontMark, brushMark, mark.Position - 5, tickyoff1 + tickHeight + 2, strformat);
                    }

                }
                
			}
			else // вертикальный трекбар
			{
                baryoff = (w + BarHeight) / 2;
                markyoff = baryoff - BarHeight / 2 - MarkHeight / 2;


                if (this.ScaleOrientation == TopBottomOrientation.bottom)
                {
                    tickyoff1 = tickyoff2 = baryoff + 2;
                }
                else if (this.ScaleOrientation == TopBottomOrientation.top)
                {
                    tickyoff1 = tickyoff2 = baryoff - BarHeight - 2 - TickHeight;
                }
                else
                {
                    tickyoff1 = baryoff + 2;
                    tickyoff2 = baryoff - BarHeight - 2 - TickHeight;
                }

                DrawRangeBarFrame(e.Graphics, true);

                DrawScale(e.Graphics, true, tickyoff1, tickyoff2);

                foreach (TrackBarMark mark in this.Marks)
                {
                    mark.Draw(e.Graphics, markyoff, false);
                    if (mark.IsMoving)
                    {
                        Font fontMark = new Font("Arial", markWidth);
                        SolidBrush brushMark = new SolidBrush(colorShadowDark);
                        StringFormat strformat = new StringFormat();
                        strformat.Alignment = StringAlignment.Near;
                        strformat.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(mark.Value.ToString(), fontMark, brushMark, tickyoff1 + tickHeight + 5, mark.Position - 3, strformat);
                    }
                }
			}
			
		}

        private void DrawRangeBarFrame(Graphics g, bool verticalBar)
        {
            int barYOff;

            if(!verticalBar)
            {
                barYOff = (this.Height - barHeight) / 2;

                g.FillRectangle(brushShadowDark, 0, barYOff, this.Width - 1, sizeShadow);	// top
                g.FillRectangle(brushShadowDark, 0, barYOff, sizeShadow, barHeight - 1);	// left
                g.FillRectangle(brushShadowLight, 0, barYOff + barHeight - 1 - sizeShadow, this.Width - 1, sizeShadow);	// bottom
                g.FillRectangle(brushShadowLight, this.Width - 1 - sizeShadow, barYOff, sizeShadow, barHeight - 1);	// right

            }
            else
            {
                barYOff = (this.Width + barHeight) / 2;

                g.FillRectangle(brushShadowDark, barYOff - barHeight, 0, barHeight, sizeShadow);	// top
                g.FillRectangle(brushShadowDark, barYOff - barHeight, 0, sizeShadow, this.Height - 1);	// left				
                g.FillRectangle(brushShadowLight, barYOff, 0, sizeShadow, this.Height - 1);	// right
                g.FillRectangle(brushShadowLight, barYOff - barHeight, this.Height - sizeShadow, barHeight, sizeShadow);	// bottom

            }
        }

        // Шкала
        private void DrawScale(Graphics g, bool verticalBar, int tickyoff1, int tickyoff2)
        {
            double dtick;
            int tickpos;

            if (!verticalBar)
            {
                if (numAxisDivision > 1)
                {
                    dtick = (double) (XPosMax - XPosMin)/(double) numAxisDivision;
                    for (int i = 0; i < numAxisDivision + 1; i++)
                    {
                        tickpos = (int) Math.Round((double) i*dtick);
                        if (scaleOrientation == TopBottomOrientation.bottom
                            || scaleOrientation == TopBottomOrientation.both)
                        {
                            g.DrawLine(penShadowDark, markWidth + 1 + tickpos,
                                       tickyoff1,
                                       markWidth + 1 + tickpos,
                                       tickyoff1 + tickHeight);
                        }
                        if (scaleOrientation == TopBottomOrientation.top
                            || scaleOrientation == TopBottomOrientation.both)
                        {
                            g.DrawLine(penShadowDark, markWidth + 1 + tickpos,
                                       tickyoff2,
                                       markWidth + 1 + tickpos,
                                       tickyoff2 + tickHeight);
                        }
                    }
                }
            }
            else
            {
                
                if (numAxisDivision > 1)
                {
                    dtick = (double)(XPosMax - XPosMin) / (double)numAxisDivision;
                    for (int i = 0; i < numAxisDivision + 1; i++)
                    {
                        tickpos = (int)Math.Round((double)i * dtick);
                        if (scaleOrientation == TopBottomOrientation.bottom
                            || scaleOrientation == TopBottomOrientation.both)
                            g.DrawLine(penShadowDark,
                                tickyoff1,
                                markWidth + 1 + tickpos,
                                tickyoff1 + tickHeight,
                                markWidth + 1 + tickpos
                                );
                        if (scaleOrientation == TopBottomOrientation.top
                            || scaleOrientation == TopBottomOrientation.both)
                            g.DrawLine(penShadowDark,
                                tickyoff2,
                                markWidth + 1 + tickpos,
                                tickyoff2 + tickHeight,
                                markWidth + 1 + tickpos
                                );
                    }
                }

            }
        }

	    private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( this.Enabled )
			{
                foreach(TrackBarMark mark in this.Marks)
                {
                    if (mark.ClientArea.Contains(e.X, e.Y))
                    {
                        this.Capture = true;
                        mark.IsMoving = true;
                        mark.IsActive = true;
                        Invalidate(true);
                        break;
                    }
                }
			}
		}

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.Enabled)
            {
                for (int i = 0; i < this.Marks.Count; i++)
                {
                    if (this.Marks[i].IsActive)
                    {
                        if (e.Delta < 0)
                            this.Marks[i].Value -= CommonUtils.SetNumberPrecision((this.Maximum - this.Minimum) / 100, this.digitCount);
                        if (e.Delta > 0)
                            this.Marks[i].Value += CommonUtils.SetNumberPrecision((this.Maximum - this.Minimum) / 100, this.digitCount);
                        break;
                    }
                }
                Invalidate(true);
                OnRangeChanged(EventArgs.Empty);

            }
        }

		private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( this.Enabled )
			{
				this.Capture = false;
                foreach(TrackBarMark mark in this.Marks)
                {
                    mark.IsMoving = false;
                }

			    Invalidate();
				OnRangeChanged(EventArgs.Empty);
			}
		}
		
		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( this.Enabled )
			{
                foreach(TrackBarMark mark in this.Marks)
                {
                    if( mark.ClientArea.Contains(e.X, e.Y))
                    {
                        if (this.Orientation == RangeBarOrientation.horizontal)
                            this.Cursor = Cursors.SizeWE;
                        else
                            this.Cursor = Cursors.SizeNS;
                        break;
                    }
                    this.Cursor = Cursors.Arrow;
                }

                for (int i = 0; i < this.Marks.Count; i++ )
                {
                    if (this.Marks[i].IsMoving)
                    {
                        if (this.Orientation == RangeBarOrientation.horizontal)
                            this.Cursor = Cursors.SizeWE;
                        else
                            this.Cursor = Cursors.SizeNS;
                        if (this.Orientation == RangeBarOrientation.horizontal)
                            this.Marks[i].Position = e.X;
                        else
                            this.Marks[i].Position = e.Y;
                        
                        this.Marks[i].IsActive = true;

                        Invalidate(true);

                        OnRangeChanging(EventArgs.Empty);
                        break;
                    }
                }

			}
		}
		
		private void OnResize(object sender, System.EventArgs e)
		{
            foreach (TrackBarMark mark in this.Marks)
            {
                mark.Refresh();
            }
			Invalidate(true);
		}
		
		private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if( this.Enabled )
			{
                for (int i = 0; i < this.Marks.Count; i++ )
                {
                    if (this.Marks[i].IsActive)
                    {
                        if (e.KeyChar == '+')
                        {
                            this.Marks[i].Value += CommonUtils.SetNumberPrecision((this.Maximum - this.Minimum) / 100, this.digitCount);
                            OnRangeChanged(EventArgs.Empty);
                        }
                        else if (e.KeyChar == '-')
                        {
                            this.Marks[i].Value -= CommonUtils.SetNumberPrecision((this.Maximum - this.Minimum) / 100, this.digitCount);
                            OnRangeChanged(EventArgs.Empty);
                        }
                        break;
                    }
                }
				Invalidate(true);
			}
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
			SetStyle(ControlStyles.DoubleBuffer,true);
			SetStyle(ControlStyles.AllPaintingInWmPaint,true);
			SetStyle(ControlStyles.UserPaint,true);
		}

		private void OnSizeChanged(object sender, System.EventArgs e)
		{
			Invalidate(true);
			Update();			
		}

		
		public event RangeChangedEventHandler RangeChanged; 
		public event RangeChangedEventHandler RangeChanging; 
		
		public virtual void OnRangeChanged(EventArgs e) 
		{
			if( RangeChanged != null )
				RangeChanged(this, e);
		}

		public virtual void OnRangeChanging(EventArgs e) 
		{
			if( RangeChanging != null )
				RangeChanging(this, e);
		}


	}
}
