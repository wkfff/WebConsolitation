using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomBoxSetSeries : FilterablePropertyBase
    {
        private BoxSetSeries _series;
        private BoxSetSeriesDataAppearanceBrowse _dataAppearanceBrowse;

        [Category("Свойства")]
        [Description("Данные")]
        [DisplayName("Данные")]
        [Browsable(true)]
        public BoxSetSeriesDataAppearanceBrowse Data
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


        public CustomBoxSetSeries(BoxSetSeries series)
        {
            this._series = series;
            this._dataAppearanceBrowse = new BoxSetSeriesDataAppearanceBrowse(series.Data);
        }

        public override string ToString()
        {
            return "";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BoxSetSeriesDataAppearanceBrowse : FilterablePropertyBase
    {
        private BoxSetSeriesDataAppearance _dataAppearance;

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
        [Description("Категория с максимумами")]
        [DisplayName("Имя категории, в которой содержатся максимумы")]
        [Browsable(true)]
        public string MaxColumn
        {
            get { return this._dataAppearance.MaxColumn; }
            set { this._dataAppearance.MaxColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория со минимумами")]
        [DisplayName("Имя категории, в которой содержатся минимумы")]
        [Browsable(true)]
        public string MinColumn
        {
            get { return this._dataAppearance.MinColumn; }
            set { this._dataAppearance.MinColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория со квартилем 1")]
        [DisplayName("Имя категории, в которой содержатся квартили 1")]
        [Browsable(true)]
        public string Q1Column
        {
            get { return this._dataAppearance.Q1Column; }
            set { this._dataAppearance.Q1Column = value; }
        }

        [Category("Свойства")]
        [Description("Категория со квартилем 2")]
        [DisplayName("Имя категории, в которой содержатся квартили 2")]
        [Browsable(true)]
        public string Q2Column
        {
            get { return this._dataAppearance.Q2Column; }
            set { this._dataAppearance.Q2Column = value; }
        }

        [Category("Свойства")]
        [Description("Категория со квартилем 3")]
        [DisplayName("Имя категории, в которой содержатся квартили 3")]
        [Browsable(true)]
        public string Q3Column
        {
            get { return this._dataAppearance.Q3Column; }
            set { this._dataAppearance.Q3Column = value; }
        }


        public BoxSetSeriesDataAppearanceBrowse(BoxSetSeriesDataAppearance dataAppearance)
        {
            this._dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return "";
        }


    }

}