using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomBoxAnnotation : FilterablePropertyBase
    {
        private BoxAnnotation _boxAnnotation;
        private BorderBrowseClass _borderBrowse;
        private LabelStyleBrowseClass _labelStyleBrowse;
        private LocationBrowseClass _locationBrowse;
        private PaintElementBrowseClass _paintElementBrowse;


        [Category("Свойства")]
        [Description("Граница")]
        [DisplayName("Граница")]
        [Browsable(true)]
        public BorderBrowseClass Border
        {
            get { return this._borderBrowse; }
            set { this._borderBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Цвет")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color FillColor
        {
            get { return this._boxAnnotation.FillColor; }
            set { this._boxAnnotation.FillColor = value; }
        }

        [Category("Свойства")]
        [Description("Высота")]
        [DisplayName("Высота")]
        [Browsable(true)]
        public int Height
        {
            get { return this._boxAnnotation.Height; }
            set { this._boxAnnotation.Height = value; }
        }

        [Category("Свойства")]
        [Description("Расположение")]
        [DisplayName("Расположение")]
        [Browsable(true)]
        public LocationBrowseClass Location
        {
            get { return this._locationBrowse; }
            set { this._locationBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Стиль элемента отображения")]
        [DisplayName("Стиль элемента отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElement
        {
            get { return this._paintElementBrowse; }
            set { this._paintElementBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Текст")]
        [DisplayName("Текст")]
        [Browsable(true)]
        public string Text
        {
            get { return this._boxAnnotation.Text; }
            set { this._boxAnnotation.Text = value; }
        }

        [Category("Свойства")]
        [Description("Стиль текста")]
        [DisplayName("Стиль текста")]
        [Browsable(true)]
        public LabelStyleBrowseClass TextStyle
        {
            get { return this._labelStyleBrowse; }
            set { this._labelStyleBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Показывать")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._boxAnnotation.Visible; }
            set { this._boxAnnotation.Visible = value; }
        }

        [Category("Свойства")]
        [Description("Ширина")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public int Width
        {
            get { return this._boxAnnotation.Width; }
            set { this._boxAnnotation.Width = value; }
        }

        public CustomBoxAnnotation(BoxAnnotation boxAnnotation)
        {
            this._boxAnnotation = boxAnnotation;
            this._borderBrowse = new BorderBrowseClass(boxAnnotation.Border);
            this._labelStyleBrowse = new LabelStyleBrowseClass(boxAnnotation.TextStyle);
            this._locationBrowse = new LocationBrowseClass(boxAnnotation.Location);
            this._paintElementBrowse = new PaintElementBrowseClass(boxAnnotation.PE);
        }

        public override string ToString()
        {
            return "Прямоугольник";
        }
    }
}