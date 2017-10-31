using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Krista.FM.Client.SchemeEditor.DiargamEditor
{
    /// <summary>
    /// Абстрактный базовый класс для каждого объекта диаграммы
    /// </summary>
    public abstract class DiagramEntity
    {
        #region Fields
        /// <summary>
        /// tells whether the current entity is hovered by the mouse
        /// </summary>
        protected internal bool hovered = false;
        /// <summary>
        /// the control to which the eneity belongs
        /// </summary>
        protected internal DiargamEditor site;
        /// <summary>
        /// tells whether the entity is selected
        /// </summary>
        protected bool isSelected = false;

        /// <summary>
        /// Default font for drawing text
        /// </summary>
        protected Font font = new Font("Verdana", 10F);

        /// <summary>
        /// Default black pen
        /// </summary>
        protected Pen blackPen = new Pen(Brushes.Black, 1f);

        /// <summary>
        /// a red pen
        /// </summary>
        protected Pen redPen = new Pen(Brushes.Red, 2f);

        /// <summary>
        /// a thicker version of the black pen
        /// </summary>
        protected Pen thickPen = new Pen(Color.Black, 1.7f);
        /// <summary>
        /// the unique identifier
        /// </summary>
        protected string key;

        /// <summary>
        /// whatever pen you use instead of the black one
        /// </summary>
        protected Pen pen;

        /// <summary>
        /// the visibility bit
        /// </summary>
        protected internal bool visible = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the entity is selected
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }
        /// <summary>
        /// Gets or sets the site of the entity
        /// </summary>
        [Browsable(false)]
        public DiargamEditor Site
        {
            get { return site; }
            set { site = value; }
        }

        /// <summary>
        /// The globally unique identifier of the entity.
        /// Mostly useful for serialization.
        /// </summary>
        [Browsable(false)]
        public string Key
        {
            get { return key; }
            set { key = value; }
        }


        /// <summary>
        /// The font used for drawing the text (if any)
        /// </summary>
        [Browsable(false)]
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default ctor
        /// </summary>
        public DiagramEntity(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Ctor with the site of the entity
        /// </summary>
        /// <param name="site"></param>
        public DiagramEntity(string key, DiargamEditor site)
            : this(key)
        {
            this.site = site;
        }


        #endregion

        #region Methods
        /// <summary>
        /// Paints the entity on the control
        /// </summary>
        /// <param name="g">the graphics object to paint on</param>
        public abstract void Draw(Graphics g);
        /// <summary>
        /// Tests whether the shape is hit by the mouse
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract bool Hit(Point p);
        /// <summary>
        /// Invalidates the entity
        /// </summary>
        public abstract void Invalidate();
        /// <summary>
        /// Moves the entity on the canvas
        /// </summary>
        /// <param name="p">the shifting vector, not an absolute position!</param>
        public abstract void Move(Point p);

        #endregion
    }
}
