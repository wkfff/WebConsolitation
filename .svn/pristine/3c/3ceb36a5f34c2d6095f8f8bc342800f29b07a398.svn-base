using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomFourDimensionalNumericSeries : FilterablePropertyBase
    {
        private FourDimensionalNumericSeries _series;
        private FourDimensionalNumericSeriesDataAppearanceBrowse _dataAppearanceBrowse;

        [Category("Свойства")]
        [Description("Данные")]
        [DisplayName("Данные")]
        [Browsable(true)]
        public FourDimensionalNumericSeriesDataAppearanceBrowse Data
        {
            get { return this._dataAppearanceBrowse; }
            set { this._dataAppearanceBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Метка")]
        [DisplayName("Метка")]
        [Browsable(true)]
        public string Label
        {
            get { return this._series.Label; }
            set { this._series.Label = value; }
        }
        /*
        [Category("Свойства")]
        [Description("Стили элементов отображения")]
        [DisplayName("Стили элементов отображения")]
        [Browsable(true)]
        public PaintElementCollection PEs
        {
            get { return this._series.PEs; }
        }

        [Category("Свойства")]
        [Description("Точки данных")]
        [DisplayName("Точки данных")]
        [Browsable(true)]
        public XYDataPointCollection Points
        {
            get { return this._series.Points; }
        }
        */
        [Category("Свойства")]
        [Description("Показывать")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._series.Visible; }
            set { this._series.Visible = value; }
        }


        public CustomFourDimensionalNumericSeries(FourDimensionalNumericSeries series)
        {
            this._series = series;
            this._dataAppearanceBrowse = new FourDimensionalNumericSeriesDataAppearanceBrowse(series.Data);
        }

        public override string ToString()
        {
            return "";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FourDimensionalNumericSeriesDataAppearanceBrowse : FilterablePropertyBase
    {
        private FourDimensionalNumericSeriesDataAppearance _dataAppearance;

        [Category("Свойства")]
        [Description("Метка")]
        [DisplayName("Метка")]
        [Browsable(true)]
        public string LabelColumn
        {
            get { return this._dataAppearance.LabelColumn; }
            set { this._dataAppearance.LabelColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория W")]
        [DisplayName("Категория W")]
        [Browsable(true)]
        public string ValueWColumn
        {
            get { return this._dataAppearance.ValueWColumn; }
            set { this._dataAppearance.ValueWColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория X")]
        [DisplayName("Категория X")]
        [Browsable(true)]
        public string ValueXColumn
        {
            get { return this._dataAppearance.ValueXColumn; }
            set { this._dataAppearance.ValueXColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория Y")]
        [DisplayName("Категория Y")]
        [Browsable(true)]
        public string ValueYColumn
        {
            get { return this._dataAppearance.ValueYColumn; }
            set { this._dataAppearance.ValueYColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория Z")]
        [DisplayName("Категория Z")]
        [Browsable(true)]
        public string ValueZColumn
        {
            get { return this._dataAppearance.ValueZColumn; }
            set { this._dataAppearance.ValueZColumn = value; }
        }


        public FourDimensionalNumericSeriesDataAppearanceBrowse(FourDimensionalNumericSeriesDataAppearance dataAppearance)
        {
            this._dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return "";
        }


    }

}