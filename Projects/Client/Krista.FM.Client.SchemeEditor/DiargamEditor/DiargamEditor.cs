using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.DiargamEditor
{
    public partial class DiargamEditor : ScrollableControl
    {
        private SchemeEditor schemeEditor;

        private List<DiagramEntity> entities = new List<DiagramEntity>();

        /// <summary>
        /// just a reference point for the OnMouseDown event
        /// </summary>
        protected Point refp;
		/// <summary>
		/// the unique entity currently selected
		/// </summary>
		protected DiagramEntity selectedEntity;

        public DiargamEditor()
        {
            //double-buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            //allow scrolling
            this.AutoScroll = true;
            this.HScroll = true;
            this.VScroll = true;

            this.AllowDrop = true;
            this.DragOver += new DragEventHandler(DiargamEditor_DragOver);
            this.DragDrop += new DragEventHandler(DiargamEditor_DragDrop);

            InitializeComponent();
        }

        void DiargamEditor_DragDrop(object sender, DragEventArgs e)
        {
            entities.Add(new UMLEntityBase("123", this));
            Invalidate();
        }

        void DiargamEditor_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        public SchemeEditor SchemeEditor
        {
            get { return schemeEditor; }
            set { schemeEditor = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //use the best quality, with a performance penalty
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //zoom
            g.ScaleTransform(1f, 1f);

            //draw the Connections

            //loop over the shapes and draw
            foreach (DiagramEntity entity in entities)
            {
                entity.Draw(g);
            }
        }
        
        /// <summary>
        /// Paints the background
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            
            Graphics g = e.Graphics;

            g.DrawString("DiargamEditor Control [version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "]", Font, Brushes.SlateGray, new Point(20, 10));
        }

        /// <summary>
        /// Handles the mouse-down event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Point p = new Point(e.X, e.Y);

            foreach (DiagramEntity entity in entities)
            {
                if(entity.Hit(p))
                {
                    selectedEntity = entity;
                }
            }
            refp = p; //useful for all kind of things
            return;
        }

        /// <summary>
		/// Handles the mouse-move event
		/// </summary>
		/// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point p = new Point(e.X, e.Y);
            //move the whole diagram

            if (selectedEntity != null)
            {
                //move just one and its kids
                selectedEntity.Move(new Point(p.X - refp.X, p.Y - refp.Y));
                refp = p;
                Invalidate();
            }
        }

        /// <summary>
        /// Handles the mouse-up event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            //test if we connected a connection
            if (selectedEntity != null)
            {
                selectedEntity = null;
            }

        }
    }
}
