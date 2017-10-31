using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Data
{
    public partial class Gripper : UserControl
    {
        public Gripper()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint| ControlStyles.ResizeRedraw, true);

       }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Draw(e.Graphics);
        }
        
        private void Draw(Graphics graphics)
        {
            int x = this.Width;
            int y = this.Height;

            Rectangle[] rects = new Rectangle[6];
            rects[0] = new Rectangle(x - 4, y - 4, 1, 1);
            rects[1] = new Rectangle(x - 9, y - 4, 1, 1);
            rects[2] = new Rectangle(x - 14, y - 4, 1, 1);
            rects[3] = new Rectangle(x - 4, y - 9, 1, 1);
            rects[4] = new Rectangle(x - 9, y - 9, 1, 1);
            rects[5] = new Rectangle(x - 4, y - 14, 1, 1);


            System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.DarkGray, 1);
            graphics.DrawRectangles(p, rects);

            rects[0] = new Rectangle(x - 5, y - 5, 1, 1);
            rects[1] = new Rectangle(x - 10, y - 5, 1, 1);
            rects[2] = new Rectangle(x - 15, y - 5, 1, 1);
            rects[3] = new Rectangle(x - 5, y - 10, 1, 1);
            rects[4] = new Rectangle(x - 10, y - 10, 1, 1);
            rects[5] = new Rectangle(x - 5, y - 15, 1, 1);

            p = new System.Drawing.Pen(System.Drawing.Color.Black, 1);
            graphics.DrawRectangles(p, rects);
            p.Dispose();

        }

        private bool canResize = false;
        private Point pStartMove;
        private Rectangle formArea;

        private void Gripper_MouseDown(object sender, MouseEventArgs e)
        {
            pStartMove = new Point(e.X, e.Y);
            formArea = ParentForm.ClientRectangle;
            canResize = true;
        }

        private void Gripper_MouseUp(object sender, MouseEventArgs e)
        {
            canResize = false;
        }

        private void Gripper_MouseMove(object sender, MouseEventArgs e)
        {
            if (canResize)
            {
                int newWidth = ParentForm.Width + e.X - pStartMove.X;
                int newHeight = ParentForm.Height + e.Y - pStartMove.Y;
                
              //  formArea = ParentForm.ClientRectangle;
               // ParentForm.Invalidate(formArea);
                    
                ParentForm.Width = newWidth;
                ParentForm.Height = newHeight;
                //Refresh();
            }
        }

    }
}
