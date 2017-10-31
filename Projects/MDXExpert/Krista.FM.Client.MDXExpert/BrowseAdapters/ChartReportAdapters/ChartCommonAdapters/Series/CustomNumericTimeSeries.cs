using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomNumericTimeSeries : FilterablePropertyBase
    {
        private NumericTimeSeries _series;
        private NumericTimeSeriesDataAppearanceBrowse _dataAppearanceBrowse;

        [Category("Свойства")]
        [Description("Данные")]
        [DisplayName("Данные")]
        [Browsable(true)]
        public NumericTimeSeriesDataAppearanceBrowse Data
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


        public CustomNumericTimeSeries(NumericTimeSeries series)
        {
            this._series = series;
            this._dataAppearanceBrowse = new NumericTimeSeriesDataAppearanceBrowse(series.Data);
        }

        public override string ToString()
        {
            return "";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NumericTimeSeriesDataAppearanceBrowse : FilterablePropertyBase
    {
        private NumericTimeSeriesDataAppearance _dataAppearance;

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
        [Description("Категория с временем")]
        [DisplayName("Имя категории, в которой содержится время")]
        [Browsable(true)]
        public string TimeValueColumn
        {
            get { return this._dataAppearance.TimeValueColumn; }
            set { this._dataAppearance.TimeValueColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория со значениями")]
        [DisplayName("Имя категории, в которой содержатся значения")]
        [Browsable(true)]
        public string ValueColumn
        {
            get { return this._dataAppearance.ValueColumn; }
            set { this._dataAppearance.ValueColumn = value; }
        }


        public NumericTimeSeriesDataAppearanceBrowse(NumericTimeSeriesDataAppearance dataAppearance)
        {
            this._dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return "";
        }


    }

}