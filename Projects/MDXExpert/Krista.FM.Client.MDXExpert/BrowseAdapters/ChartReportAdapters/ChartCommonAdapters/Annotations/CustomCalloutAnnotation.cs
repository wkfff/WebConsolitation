using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomCalloutAnnotation : FilterablePropertyBase
    {
        private CalloutAnnotation _calloutAnnotation;
        private BorderBrowseClass _borderBrowse;
        private LocationBrowseClass _locationBrowse;
        private LocationBrowseClass _offsetBrowse;
        private PaintElementBrowseClass _paintElementBrowse;
        private LabelStyleBrowseClass _labelStyleBrowse;

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
            get { return this._calloutAnnotation.FillColor; }
            set { this._calloutAnnotation.FillColor = value; }
        }

        [Category("Свойства")]
        [Description("Высота")]
        [DisplayName("Высота")]
        [Browsable(true)]
        public int Height
        {
            get { return this._calloutAnnotation.Height; }
            set { this._calloutAnnotation.Height = value; }
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
        [TypeConverter(typeof(LocationOffsetModeConverter))]
        [Browsable(true)]
        public LocationOffsetMode OffsetMode
        {
            get { return this._calloutAnnotation.OffsetMode; }
            set { this._calloutAnnotation.OffsetMode = value; }
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
            get { return this._calloutAnnotation.Text; }
            set { this._calloutAnnotation.Text = value; }
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
            get { return this._calloutAnnotation.Visible; }
            set { this._calloutAnnotation.Visible = value; }
        }

        [Category("Свойства")]
        [Description("Ширина")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public int Width
        {
            get { return this._calloutAnnotation.Width; }
            set { this._calloutAnnotation.Width = value; }
        }

        public CustomCalloutAnnotation(CalloutAnnotation calloutAnnotation)
        {
            this._calloutAnnotation = calloutAnnotation;
            this._borderBrowse = new BorderBrowseClass(calloutAnnotation.Border);
            this._locationBrowse = new LocationBrowseClass(calloutAnnotation.Location);
            this._offsetBrowse = new LocationBrowseClass(calloutAnnotation.Offset);
            this._paintElementBrowse = new PaintElementBrowseClass(calloutAnnotation.PE);
            this._labelStyleBrowse = new LabelStyleBrowseClass(calloutAnnotation.TextStyle);
        }

        public override string ToString()
        {
            return "Выноска";
        }
    }
}