using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomLineImageAnnotation : FilterablePropertyBase
    {
        private LineImageAnnotation _lineImageAnnotation;
        private LineStyleBrowseClass _lineStyleBrowse;
        private LocationBrowseClass _locationBrowse;
        private LocationBrowseClass _offsetBrowse;


        [Category("Свойства")]
        [Description("Высота")]
        [DisplayName("Высота")]
        [Browsable(true)]
        public int Height
        {
            get { return this._lineImageAnnotation.Height; }
            set { this._lineImageAnnotation.Height = value; }
        }


        [Category("Свойства")]
        [Description("Изображение")]
        [DisplayName("Изображение")]
        [Browsable(true)]
        public Image Image
        {
            get { return this._lineImageAnnotation.Image; }
            set { this._lineImageAnnotation.Image = value; }
        }

        [Category("Свойства")]
        [Description("Стиль подгонки изображения")]
        [DisplayName("Стиль подгонки изображения")]
        [TypeConverter(typeof(ImageFitStyleTypeConverter))]
        [Browsable(true)]
        public ImageFitStyle ImageFitStyle
        {
            get { return this._lineImageAnnotation.ImageFitStyle; }
            set { this._lineImageAnnotation.ImageFitStyle = value; }
        }

        [Category("Свойства")]
        [Description("Путь к изображению")]
        [DisplayName("Путь к изображению")]
        [Browsable(true)]
        public string ImagePath
        {
            get { return this._lineImageAnnotation.ImagePath; }
            set { this._lineImageAnnotation.ImagePath = value; }
        }

        [Category("Свойства")]
        [Description("Цвет линии")]
        [DisplayName("Цвет линии")]
        [Browsable(true)]
        public Color LineColor
        {
            get { return this._lineImageAnnotation.LineColor; }
            set { this._lineImageAnnotation.LineColor = value; }
        }

        [Category("Свойства")]
        [Description("Стиль линии")]
        [DisplayName("Стиль линии")]
        [Browsable(true)]
        public LineStyleBrowseClass LineStyle
        {
            get { return this._lineStyleBrowse; }
            set { this._lineStyleBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Толщина линии")]
        [DisplayName("Толщина линии")]
        [Browsable(true)]
        public int LineThickness
        {
            get { return this._lineImageAnnotation.LineThickness; }
            set { this._lineImageAnnotation.LineThickness = value; }
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
        [Description("Смещение")]
        [DisplayName("Смещение")]
        [Browsable(true)]
        public LocationBrowseClass Offset
        {
            get { return this._offsetBrowse; }
            set { this._offsetBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Тип смещения")]
        [DisplayName("Тип смещения")]
        [Browsable(true)]
        public LocationOffsetMode OffsetMode
        {
            get { return this._lineImageAnnotation.OffsetMode; }
            set { this._lineImageAnnotation.OffsetMode = value; }
        }

        [Category("Свойства")]
        [Description("Показывать")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._lineImageAnnotation.Visible; }
            set { this._lineImageAnnotation.Visible = value; }
        }

        [Category("Свойства")]
        [Description("Ширина")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public int Width
        {
            get { return this._lineImageAnnotation.Width; }
            set { this._lineImageAnnotation.Width = value; }
        }

        public CustomLineImageAnnotation(LineImageAnnotation lineImageAnnotation)
        {
            this._lineImageAnnotation = lineImageAnnotation;
            this._lineStyleBrowse = new LineStyleBrowseClass(lineImageAnnotation.LineStyle);
            this._locationBrowse = new LocationBrowseClass(lineImageAnnotation.Location);
            this._offsetBrowse = new LocationBrowseClass(lineImageAnnotation.Offset);
        }

        public override string ToString()
        {
            return "Изображение";
        }
    }
}