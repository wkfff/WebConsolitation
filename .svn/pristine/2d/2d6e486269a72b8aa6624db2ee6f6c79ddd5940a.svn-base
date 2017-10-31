using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Data
{
    public partial class FormHeader : UserControl
    {
        public FormHeader()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            /*
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);*/

        }
        
        protected override void OnPaint(PaintEventArgs e) 
        {
            //base.OnPaint(e);
            Draw(e.Graphics);            
        }
        
        private void Draw(Graphics graphics)
        {
            int x = this.Width / 2;
            int y = this.Height / 2;

            Rectangle[] rects = new Rectangle[6];
            rects[0] = new Rectangle(x - 13, y, 1, 1);
            rects[1] = new Rectangle(x - 7, y, 1, 1);
            rects[2] = new Rectangle(x - 1, y, 1, 1);
            rects[3] = new Rectangle(x + 5, y, 1, 1);
            rects[4] = new Rectangle(x + 11, y, 1, 1);
            rects[5] = new Rectangle(x + 17, y, 1, 1);

            System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.DarkGray, 1);
            graphics.DrawRectangles(p, rects);

            rects[0] = new Rectangle(x - 14, y - 1, 1, 1);
            rects[1] = new Rectangle(x - 8, y - 1, 1, 1);
            rects[2] = new Rectangle(x - 2, y - 1, 1, 1);
            rects[3] = new Rectangle(x + 4, y - 1, 1, 1);
            rects[4] = new Rectangle(x + 10, y - 1, 1, 1);
            rects[5] = new Rectangle(x + 16, y - 1, 1, 1);

            p = new System.Drawing.Pen(System.Drawing.Color.Black, 1);
            graphics. DrawRectangles(p, rects);
            p.Dispose();
        }

        private void Gripper_SizeChanged(object sender, EventArgs e)
        {
         //   Refresh();
        }


        private bool canMove = false;
        private Point pStartMove;

        private void FormHeader_MouseDown(object sender, MouseEventArgs e)
        {
            pStartMove = new Point(e.X, e.Y);
            canMove = true;
        }

        private void FormHeader_MouseUp(object sender, MouseEventArgs e)
        {
            canMove = false;
        }

        private void FormHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (canMove)
            {
                ParentForm.Left += e.X - pStartMove.X;
                ParentForm.Top += e.Y - pStartMove.Y;
                //ParentForm.Invalidate(true);
                //Refresh();
            }

        }

    }
}
