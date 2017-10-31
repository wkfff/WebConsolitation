using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;


namespace Krista.FM.Client.Common.Wizards
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof (Krista.FM.Client.Common.Wizards.LineFrame), "Krista.FM.Client.Common.Wizards.LineFrame.bmp")]
    [Designer(
      "Krista.FM.Client.Design.LineFrameDesigner, Krista.FM.Client.Design, Version=2.4.1.0, Culture=neutral, PublicKeyToken=null"
        )]
    [System.Diagnostics.DebuggerStepThrough]
    public class LineFrame : System.Windows.Forms.Control
    {
        #region Class constants

        public enum TLineStyle
        {
            Horizontal,
            Vertical
        }

        #endregion

        #region Class members

        private static Point m_pntStart = new Point(0, 1);

        private Point m_pntEnd = new Point(0, 1);
        private TLineStyle m_enStyle = TLineStyle.Horizontal;

        #endregion

        #region Class Properties

        [Category("Appearance"),
         DefaultValue(typeof (TLineStyle), "Horizontal"),
         Description("GET/SET style of line")]
        public TLineStyle LineStyle
        {
            get { return m_enStyle; }
            set
            {
                if (value != m_enStyle)
                {
                    m_enStyle = value;
                    OnLineStyleChanged();
                }
            }
        }

        protected override System.Drawing.Size DefaultSize
        {
            get
            {
                if (m_enStyle == TLineStyle.Horizontal)
                    return new Size(75, 4);
                else
                    return new Size(4, 75);
            }
        }

        #endregion

        #region Class Initialize/Finilize code

        public LineFrame()
        {
            ControlStyles styleTrue = ControlStyles.ResizeRedraw |
                                      ControlStyles.DoubleBuffer |
                                      ControlStyles.AllPaintingInWmPaint |
                                      ControlStyles.UserPaint |
                                      ControlStyles.FixedHeight;

            ControlStyles styleFalse = ControlStyles.SupportsTransparentBackColor |
                                       ControlStyles.Selectable;

            SetStyle(styleTrue, true);
            SetStyle(styleFalse, false);

            base.Size = new Size(75, 4);
            base.TabStop = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //m_gdi.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Class Overrides

        protected virtual void OnLineStyleChanged()
        {
            SetStyle(ControlStyles.FixedHeight, (m_enStyle == TLineStyle.Horizontal));
            SetStyle(ControlStyles.FixedWidth, (m_enStyle == TLineStyle.Vertical));

            m_pntEnd.X = 1;
            m_pntEnd.Y = Height;

            Invalidate();
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);

            if (m_enStyle == TLineStyle.Horizontal)
            {
                base.Size = new Size(Size.Width, 4);
                m_pntEnd.X = Width;
                m_pntEnd.Y = 1;
            }
            else
            {
                base.Size = new Size(4, Size.Height);
                m_pntEnd.X = 1;
                m_pntEnd.Y = Height;
            }
        }

        /// <summary>
        /// Draw 3d Line. 3D Line is a simple line wich contains one dark and one light line.
        /// By dark and light line we create optical 3D effect.
        /// </summary>
        /// <param name="graph">Graphics object which used by function to draw</param>
        /// <param name="pnt1">Start point</param>
        /// <param name="pnt2">End point</param>
        public void Draw3DLine(Graphics graph, Point pnt1, Point pnt2)
        {
            Pen penDark = new Pen(SystemColors.ControlDarkDark);
            Pen penLight = new Pen(SystemColors.ControlLightLight);

            Point[] arrPoint = {pnt1, pnt2}; // create copy of Point input params
            graph.DrawLine(penLight, pnt1, pnt2); // draw first line

            if (pnt1.X == pnt2.X)
            {
                arrPoint[0].X--;
                arrPoint[1].X--;
            }
            else if (pnt1.Y == pnt2.Y)
            {
                arrPoint[0].Y--;
                arrPoint[1].Y--;
            }
            else
            {
                arrPoint[0].X--;
                arrPoint[0].Y--;
                arrPoint[1].X--;
                arrPoint[1].Y--;
            }

            graph.DrawLine(penDark, arrPoint[0], arrPoint[1]);

            penDark.Dispose();
            penLight.Dispose();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Draw3DLine(pe.Graphics,
                       (m_enStyle == TLineStyle.Horizontal) ? m_pntStart : new Point(1, 0),
                       m_pntEnd);
        }

        #endregion
    }
}