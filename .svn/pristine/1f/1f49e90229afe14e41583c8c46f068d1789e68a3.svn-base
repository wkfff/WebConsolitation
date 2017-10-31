using System;
using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Граница
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RectangleBrowseClass
    {
        #region Поля

        private Rectangle _rectangle;

        #endregion

        #region Свойства

        [Description("Расположение по горизонтали")]
        [DisplayName("X")]
        [Browsable(true)]
        public int X
        {
            get { return this._rectangle.X; }
            set { this._rectangle.X = value; }
        }

        [Description("Расположение по вертикали")]
        [DisplayName("Y")]
        [Browsable(true)]
        public int Y
        {
            get { return this._rectangle.Y; }
            set { this._rectangle.Y = value; }
        }

        [Description("Высота")]
        [DisplayName("Высота")]
        [Browsable(true)]
        public int Height
        {
            get { return this._rectangle.Height; }
            set { this._rectangle.Height = value; }
        }

        [Description("Ширина")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public int Width
        {
            get { return this._rectangle.Width; }
            set { this._rectangle.Width = value; }
        }

        #endregion

        public RectangleBrowseClass(Rectangle rectangle)
        {
            this._rectangle = rectangle;
        }

        public override string ToString()
        {
            return String.Format("{0}; {1}; {2}; {3}", this.X, this.Y, this.Width, this.Height);
        }
    }
}