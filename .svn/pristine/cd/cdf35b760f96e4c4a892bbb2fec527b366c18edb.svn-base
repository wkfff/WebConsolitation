using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomNumericSeries : FilterablePropertyBase
    {
        private NumericSeries _numericSeries;
        private NumericSeriesDataAppearanceBrowse _numericSeriesDataAppearanceBrowse;
        
        [Category("Свойства")]
        [Description("Данные")]
        [DisplayName("Данные")]
        [Browsable(true)]
        public NumericSeriesDataAppearanceBrowse Data
        {
            get { return this._numericSeriesDataAppearanceBrowse; }
            set { this._numericSeriesDataAppearanceBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Метка")]
        [DisplayName("Метка")]
        [Browsable(true)]
        public string Label
        {
            get { return this._numericSeries.Label; }
            set { this._numericSeries.Label = value; }
        }
        /*
        [Category("Свойства")]
        [Description("Стили элементов отображения")]
        [DisplayName("Стили элементов отображения")]
        [Browsable(true)]
        public PaintElementCollection PEs
        {
            get { return this._numericSeries.PEs; }
        }

        [Category("Свойства")]
        [Description("Точки данных")]
        [DisplayName("Точки данных")]
        [Browsable(true)]
        public NumericDataPointCollection Points
        {
            get { return this._numericSeries.Points; }
        }
        */
        [Category("Свойства")]
        [Description("Показывать")]
        [DisplayName("Показывать")]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._numericSeries.Visible; }
            set { this._numericSeries.Visible = value; }
        }


        public CustomNumericSeries(NumericSeries numericSeries)
        {
            this._numericSeries = numericSeries;
            this._numericSeriesDataAppearanceBrowse = new NumericSeriesDataAppearanceBrowse(numericSeries.Data);
        }

        public override string ToString()
        {
            return "";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NumericSeriesDataAppearanceBrowse : FilterablePropertyBase
    {
        private NumericSeriesDataAppearance _dataAppearance;

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
        [Description("Значение")]
        [DisplayName("Значение")]
        [Browsable(true)]
        public string ValueColumn
        {
            get { return this._dataAppearance.ValueColumn; }
            set { this._dataAppearance.ValueColumn = value; }
        }



        public NumericSeriesDataAppearanceBrowse(NumericSeriesDataAppearance dataAppearance)
        {
            this._dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return "";
        }


    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NumericDataPointBrowseClass : FilterablePropertyBase
    {
        private NumericDataPoint _dataPoint;
        private PaintElementBrowseClass _paintElementBrowse;

        [Category("Свойства")]
        [Description("Подключить данные")]
        [DisplayName("Подключить данные")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool DataBound
        {
            get { return this._dataPoint.DataBound; }
            set { this._dataPoint.DataBound = value; }
        }

        [Category("Свойства")]
        [Description("Отображать пустые")]
        [DisplayName("Отображать пустые")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Empty
        {
            get { return this._dataPoint.Empty; }
            set { this._dataPoint.Empty = value; }
        }

        [Category("Свойства")]
        [Description("Метка")]
        [DisplayName("Метка")]
        [Browsable(true)]
        public string Label
        {
            get { return this._dataPoint.Label; }
            set { this._dataPoint.Label = value; }
        }

        [Category("Свойства")]
        [Description("Стиль элемента отображения")]
        [DisplayName("Стиль элемента отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PE
        {
            get { return this._paintElementBrowse; }
            set { this._paintElementBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Значение")]
        [DisplayName("Значение")]
        [Browsable(true)]
        public double Value
        {
            get { return this._dataPoint.Value; }
            set { this._dataPoint.Value = value; }
        }


        public NumericDataPointBrowseClass(NumericDataPoint dataPoint)
        {
            this._dataPoint = dataPoint;
            this._paintElementBrowse = new PaintElementBrowseClass(dataPoint.PE);
        }
    }
}