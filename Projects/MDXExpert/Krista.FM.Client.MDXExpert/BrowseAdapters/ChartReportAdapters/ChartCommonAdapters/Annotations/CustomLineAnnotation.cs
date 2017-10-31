using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomLineAnnotation : FilterablePropertyBase
    {
        private LineAnnotation _lineAnnotation;
        private LocationBrowseClass _locationBrowse;
        private LocationBrowseClass _offsetBrowse;
        private LineStyleBrowseClass _lineStyleBrowse;

        [Category("Свойства")]
        [Description("Цвет")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return this._lineAnnotation.Color; }
            set { this._lineAnnotation.Color = value; }
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
            get { return this._lineAnnotation.OffsetMode; }
            set { this._lineAnnotation.OffsetMode = value; }
        }

        [Category("Свойства")]
        [Description("Тип линии")]
        [DisplayName("Тип линии")]
        [Browsable(true)]
        public LineStyleBrowseClass Style
        {
            get { return this._lineStyleBrowse; }
            set { this._lineStyleBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Толщина")]
        [DisplayName("Толщина")]
        [Browsable(true)]
        public int Thickness
        {
            get { return this._lineAnnotation.Thickness; }
            set { this._lineAnnotation.Thickness = value; }
        }

        public CustomLineAnnotation(LineAnnotation lineAnnotation)
        {
            this._lineAnnotation = lineAnnotation;
            this._locationBrowse = new LocationBrowseClass(lineAnnotation.Location);
            this._offsetBrowse = new LocationBrowseClass(lineAnnotation.Offset);
            this._lineStyleBrowse = new LineStyleBrowseClass(lineAnnotation.Style);
        }

        public override string ToString()
        {
            return "Линия";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LocationBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private Location _location;

        #endregion
        
        #region Свойства

        [Description("Категория")]
        [DisplayName("Категория")]
        [DynamicPropertyFilter("Type", "RowColumn")]
        [DefaultValue(true)]
        [Browsable(true)]
        public int Column
        {
            get { return this._location.Column; }
            set { this._location.Column = value; }
        }

        [Description("Расстояние по горизонтали")]
        [DisplayName("X")]
        [DynamicPropertyFilter("Type", "Percentage, Pixels")]
        [DefaultValue(true)]
        [Browsable(true)]
        public double LocationX
        {
            get { return this._location.LocationX; }
            set { this._location.LocationX = value; }

        }

        [Description("Расстояние по вертикали")]
        [DisplayName("Y")]
        [DynamicPropertyFilter("Type", "Percentage, Pixels")]
        [DefaultValue(true)]
        [Browsable(true)]
        public double LocationY
        {
            get { return this._location.LocationY; }
            set { this._location.LocationY = value; }
        }

        [Description("Ряд")]
        [DisplayName("Ряд")]
        [DynamicPropertyFilter("Type", "RowColumn")]
        [DefaultValue(true)]
        [Browsable(true)]
        public int Row
        {
            get { return this._location.Row; }
            set { this._location.Row = value; }
        }

        [Description("Тип размещения")]
        [DisplayName("Тип размещения")]
        [TypeConverter(typeof(AnnotationLocationTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public LocationType Type
        {
            get { return this._location.Type; }
            set { this._location.Type = value; }
        }

        [Description("Значение на оси X")]
        [DisplayName("Значение X")]
        [DynamicPropertyFilter("Type", "DataValues")]
        [DefaultValue(true)]
        [Browsable(true)]
        public double ValueX
        {
            get { return this._location.ValueX; }
            set { this._location.ValueX = value; }
        }

        [Description("Значение на оси Y")]
        [DisplayName("Значение Y")]
        [DynamicPropertyFilter("Type", "DataValues")]
        [DefaultValue(true)]
        [Browsable(true)]
        public double ValueY
        {
            get { return this._location.ValueY; }
            set { this._location.ValueY = value; }
        }


        #endregion

        public LocationBrowseClass(Location location)
        {
            this._location = location;
        }

        public override string ToString()
        {
            return "";
        }

    }
}