using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Data;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomGanttSeries : FilterablePropertyBase
    {
        private GanttSeries _series;
        private GanttSeriesDataAppearanceBrowse _dataAppearanceBrowse;

        [Category("Свойства")]
        [Description("Данные")]
        [DisplayName("Данные")]
        [Browsable(true)]
        public GanttSeriesDataAppearanceBrowse Data
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


        public CustomGanttSeries(GanttSeries series)
        {
            this._series = series;
            this._dataAppearanceBrowse = new GanttSeriesDataAppearanceBrowse(series.Data);
        }

        public override string ToString()
        {
            return "";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GanttSeriesDataAppearanceBrowse : FilterablePropertyBase
    {
        private GanttSeriesDataAppearance _dataAppearance;

        [Category("Свойства")]
        [Description("Категория с конечной датой")]
        [DisplayName("Имя категории, в которой содержатся конечные даты")]
        [Browsable(true)]
        public string CloseColumn
        {
            get { return this._dataAppearance.EndTimeColumn; }
            set { this._dataAppearance.EndTimeColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория ID")]
        [DisplayName("Имя категории, в которой содержатся ccылки на ID")]
        [Browsable(true)]
        public string DateColumn
        {
            get { return this._dataAppearance.LinkToIDColumn; }
            set { this._dataAppearance.LinkToIDColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория с владельцами")]
        [DisplayName("Имя категории, в которой содержатся владельцы")]
        [Browsable(true)]
        public string HighColumn
        {
            get { return this._dataAppearance.OwnerColumn; }
            set { this._dataAppearance.OwnerColumn = value; }
        }

        [Category("Свойства")]
        [Description("Процентная категория")]
        [DisplayName("Имя категории, в которой содержатся проценты выполнения")]
        [Browsable(true)]
        public string LabelColumn
        {
            get { return this._dataAppearance.PercentCompleteColumn; }
            set { this._dataAppearance.PercentCompleteColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория с начальной датой")]
        [DisplayName("Имя категории, в которой содержатся начальные даты")]
        [Browsable(true)]
        public string LowColumn
        {
            get { return this._dataAppearance.StartTimeColumn; }
            set { this._dataAppearance.StartTimeColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория с ID точек входа")]
        [DisplayName("Имя категории, в которой содержатся ID точек входа")]
        [Browsable(true)]
        public string OpenColumn
        {
            get { return this._dataAppearance.TimeEntryIDColumn; }
            set { this._dataAppearance.TimeEntryIDColumn = value; }
        }


        public GanttSeriesDataAppearanceBrowse(GanttSeriesDataAppearance dataAppearance)
        {
            this._dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return "";
        }


    }

}