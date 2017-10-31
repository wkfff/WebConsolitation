using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    public partial class OffsetPointControl : UserControl
    {
        #region Поля

        private MapPoint offsetPoint;

        #endregion

        #region Свойства

        public MapPoint OffsetPoint
        {
            get { return offsetPoint; }
            set { offsetPoint = value; }
        }

        #endregion

        public OffsetPointControl(MapPoint offsetPoint)
        {
            InitializeComponent();
            this.offsetPoint = offsetPoint;
        }

        private Point GetCentralPoint()
        {
            return new Point(this.drawPanel.Width / 2, this.drawPanel.Height / 2);
        }

        private Point GetOffsetPoint()
        {
            Point offset = new Point();
            Point centr = GetCentralPoint();

            double newX = centr.X + centr.X * offsetPoint.X;
            double newY = centr.Y - centr.Y * offsetPoint.Y;

            offset.X = (int)newX;
            offset.Y = (int)newY;

            return offset;
        }

        private void drawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = GetCentralPoint();
            offsetPoint.X = (float)(e.X - p.X) / p.X;
            offsetPoint.Y = (float)(p.Y - e.Y) / p.Y;
            this.drawPanel.Invalidate();
        }

        private void Draw(Graphics graphics)
        {
            Point centr = GetCentralPoint();
            Point offset = GetOffsetPoint();

            Brush brush = new SolidBrush(Color.Black);
            graphics.FillEllipse(brush, centr.X - 5, centr.Y - 5, 10, 10);

            brush = new SolidBrush(Color.Blue);
            graphics.FillEllipse(brush, offset.X - 5, offset.Y - 5, 10, 10);
        }

        private void drawPanel_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.Graphics);
        }

        private void drawPanel_Resize(object sender, EventArgs e)
        {
            drawPanel.Invalidate();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (Tag != null)
            {
                ((IWindowsFormsEditorService)Tag).CloseDropDown();
            }
            else
            {
                this.Hide();
            }

        }

        private void btDefault_Click(object sender, EventArgs e)
        {
            offsetPoint.X = 0;
            offsetPoint.Y = 0;
            this.drawPanel.Invalidate();
        }



    }
}
