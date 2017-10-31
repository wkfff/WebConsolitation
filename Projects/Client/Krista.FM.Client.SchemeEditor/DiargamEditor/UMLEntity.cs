using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Krista.FM.Client.SchemeEditor.DiargamEditor
{
    /// <summary>
    /// Базовый класс для элементов имеющих форму
    /// </summary>
    public class UMLEntityBase : DiagramEntity
    {
        #region Fields
        /// <summary>
        /// the rectangle on which any shape lives
        /// </summary>
        protected internal Rectangle rectangle;
        /// <summary>
        /// the backcolor of the shapes
        /// </summary>
        protected Color shapeColor = Color.SteelBlue;
        /// <summary>
        /// the brush corresponding to the backcolor
        /// </summary>
        protected Brush shapeBrush;
        /// <summary>
        /// the text on the shape
        /// </summary>
        protected string text = string.Empty;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width of the shape
        /// </summary>
        [Browsable(true), Description("The width of the shape"), Category("Layout")]
        public int Width
        {
            get { return rectangle.Width; }
            set
            {
                Resize(value, Height);
                //site.DrawTree();
                site.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the height of the shape
        /// </summary>		
        [Browsable(true), Description("The height of the shape"), Category("Layout")]
        public int Height
        {
            get { return this.rectangle.Height; }
            set
            {
                Resize(this.Width, value);
                //site.DrawTree();
                site.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the text of the shape
        /// </summary>
        [Browsable(true), Description("The text shown on the shape"), Category("Layout")]
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                Fit();
                //site.DrawTree();
                site.Invalidate();
            }
        }

        /// <summary>
        /// the x-coordinate of the upper-left corner
        /// </summary>
        [Browsable(false), Description("The x-coordinate of the upper-left corner"), Category("Layout")]
        public int X
        {
            get { return rectangle.X; }
            set
            {
                Point p = new Point(value - rectangle.X, rectangle.Y);
                this.Move(p);
                site.Invalidate(); //note that 'this.Invalidate()' will not be enough
            }
        }

        /// <summary>
        /// the y-coordinate of the upper-left corner
        /// </summary>
        [Browsable(false), Description("The y-coordinate of the upper-left corner"), Category("Layout")]
        public int Y
        {
            get { return rectangle.Y; }
            set
            {
                Point p = new Point(rectangle.X, value - rectangle.Y);
                this.Move(p);
                site.Invalidate();
            }
        }
        /// <summary>
        /// The backcolor of the shape
        /// </summary>
        [Browsable(true), Description("The backcolor of the shape"), Category("Layout")]
        public Color ShapeColor
        {
            get { return shapeColor; }
            set { shapeColor = value; SetBrush(); Invalidate(); }
        }
        /// <summary>
        /// Gets or sets whether the shape is visible
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        /// <summary>
        /// Gets or sets the location of the shape;
        /// </summary>
        [Browsable(false)]
        public Point Location
        {
            get { return new Point(this.rectangle.X, this.rectangle.Y); }
            set
            {
                //we use the move method but it requires the delta value, not an absolute position!
                Point p = new Point(value.X - rectangle.X, value.Y - rectangle.Y);
                //if you'd use this it would indeed move the shape but not the connector s of the shape
                //this.rectangle.X = value.X; this.rectangle.Y = value.Y; Invalidate();
                this.Move(p);
            }
        }

        /// <summary>
        /// Gets the left coordiante of the rectangle
        /// </summary>
        [Browsable(false)]
        public int Left
        {
            get { return this.rectangle.Left; }
        }

        /// <summary>
        /// Gets the right coordiante of the rectangle
        /// </summary>
        [Browsable(false)]
        public int Right
        {
            get { return this.rectangle.Right; }
        }

        /// <summary>
        /// Get the bottom coordinate of the shape
        /// </summary>
        [Browsable(false)]
        public int Bottom
        {
            get { return this.rectangle.Bottom; }
        }

        /// <summary>
        /// Gets the top coordinate of the rectangle
        /// </summary>
        [Browsable(false)]
        public int Top
        {
            get { return this.rectangle.Top; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default ctor
        /// </summary>
        public UMLEntityBase(string key)
            : base(key)
        {
            Init();
        }
        /// <summary>
        /// Constructor with the site of the shape
        /// </summary>
        /// <param name="site">the graphcontrol instance to which the shape is attached</param>
        public UMLEntityBase(string key, DiargamEditor site)
            : base(key, site)
        {
            Init();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resizes the shape's rectangle in function of the containing text
        /// </summary>
        public void Fit()
        {
            Graphics g = Graphics.FromHwnd(site.Handle);
            Size s = Size.Round(g.MeasureString(text, Font));
            rectangle.Height = s.Height + 8;
            Invalidate();
        }

        /// <summary>
        /// Summarizes the initialization used by the constructors
        /// </summary>
        private void Init()
        {
            rectangle = new Rectangle(0, 0, 100, 70);
            SetBrush();
        }



        /// <summary>
        /// Sets the brush corresponding to the backcolor
        /// </summary>
        protected void SetBrush()
        {
            if (isSelected)
                shapeBrush = new SolidBrush(Color.YellowGreen);
            else
                shapeBrush = new SolidBrush(shapeColor);


        }

        /// <summary>
        /// Overrides the abstract paint method
        /// </summary>
        /// <param name="g">a graphics object onto which to paint</param>
        public override void Draw(System.Drawing.Graphics g)
        {
            g.DrawRectangle(Pens.Black, rectangle);
            return;
        }

        /// <summary>
        /// Override the abstract Hit method
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            Rectangle r = new Rectangle(p, new Size(2, 2));
            return rectangle.Contains(r);
        }

        /// <summary>
        /// Overrides the abstract Invalidate method
        /// </summary>
        public override void Invalidate()
        {
            site.Invalidate(rectangle);
        }



        /// <summary>
        /// Moves the shape with the given shift
        /// </summary>
        /// <param name="p">represent a shift-vector, not the absolute position!</param>
        public override void Move(Point p)
        {
            this.rectangle.X += p.X;
            this.rectangle.Y += p.Y;
            this.Invalidate();

        }

        /// <summary>
        /// Resizes the shape 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void Resize(int width, int height)
        {
            this.rectangle.Height = height;
            this.rectangle.Width = width;
            this.site.Invalidate();
        }

        #endregion
    }
}
