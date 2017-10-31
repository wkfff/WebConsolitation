using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomEllipseAnnotation : FilterablePropertyBase
    {
        private EllipseAnnotation _ellipseAnnotation;
        private LocationBrowseClass _locationBrowse;
        private LabelStyleBrowseClass _labelStyleBrowse;
        private PaintElementBrowseClass _paintElementBrowse;

        [Category("Свойства")]
        [Description("Цвет")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color FillColor
        {
            get { return this._ellipseAnnotation.FillColor; }
            set { this._ellipseAnnotation.FillColor = value; }
        }

        [Category("Свойства")]
        [Description("Высота")]
        [DisplayName("Высота")]
        [Browsable(true)]
        public int Height
        {
            get { return this._ellipseAnnotation.Height; }
            set { this._ellipseAnnotation.Height = value; }
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
        [Description("Цвет линии")]
        [DisplayName("Цвет линии")]
        [Browsable(true)]
        public Color OutlineColor
        {
            get { return this._ellipseAnnotation.OutlineColor; }
            set { this._ellipseAnnotation.OutlineColor = value; }
        }

        [Category("Свойства")]
        [Description("Стиль линии")]
        [DisplayName("Стиль линии")]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle OutlineDrawStyle
        {
            get { return this._ellipseAnnotation.OutlineDrawStyle; }
            set { this._ellipseAnnotation.OutlineDrawStyle = value; }
        }

        [Category("Свойства")]
        [Description("Толщина линии")]
        [DisplayName("Толщина линии")]
        [Browsable(true)]
        public int OutlineThickness
        {
            get { return this._ellipseAnnotation.OutlineThickness; }
            set { this._ellipseAnnotation.OutlineThickness = value; }
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
            get { return this._ellipseAnnotation.Text; }
            set { this._ellipseAnnotation.Text = value; }
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
            get { return this._ellipseAnnotation.Visible; }
            set { this._ellipseAnnotation.Visible = value; }
        }

        [Category("Свойства")]
        [Description("Ширина")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public int Width
        {
            get { return this._ellipseAnnotation.Width; }
            set { this._ellipseAnnotation.Width = value; }
        }

        public CustomEllipseAnnotation(EllipseAnnotation ellipseAnnotation)
        {
            this._ellipseAnnotation = ellipseAnnotation;
            this._locationBrowse = new LocationBrowseClass(ellipseAnnotation.Location);
            this._labelStyleBrowse = new LabelStyleBrowseClass(ellipseAnnotation.TextStyle);
            this._paintElementBrowse = new PaintElementBrowseClass(ellipseAnnotation.PE);
        }

        public override string ToString()
        {
            return "Эллипс";
        }
    }
}