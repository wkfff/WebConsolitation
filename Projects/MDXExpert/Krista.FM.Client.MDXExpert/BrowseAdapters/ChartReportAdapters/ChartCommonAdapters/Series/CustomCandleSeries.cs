using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomCandleSeries : FilterablePropertyBase
    {
        private CandleSeries _series;
        private CandleSeriesDataAppearanceBrowse _dataAppearanceBrowse;

        [Category("Свойства")]
        [Description("Данные")]
        [DisplayName("Данные")]
        [Browsable(true)]
        public CandleSeriesDataAppearanceBrowse Data
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


        public CustomCandleSeries(CandleSeries series)
        {
            this._series = series;
            this._dataAppearanceBrowse = new CandleSeriesDataAppearanceBrowse(series.Data);
        }

        public override string ToString()
        {
            return "";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CandleSeriesDataAppearanceBrowse : FilterablePropertyBase
    {
        private CandleSeriesDataAppearance _dataAppearance;

        [Category("Свойства")]
        [Description("Закрытая категория")]
        [DisplayName("Имя категории, в которой содержатся закрытые значения")]
        [Browsable(true)]
        public string CloseColumn
        {
            get { return this._dataAppearance.CloseColumn; }
            set { this._dataAppearance.CloseColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория с датами")]
        [DisplayName("Имя категории, в которой содержатся даты")]
        [Browsable(true)]
        public string DateColumn
        {
            get { return this._dataAppearance.DateColumn; }
            set { this._dataAppearance.DateColumn = value; }
        }

        [Category("Свойства")]
        [Description("Высшая категория")]
        [DisplayName("Имя категории, в которой содержаться высшие значения")]
        [Browsable(true)]
        public string HighColumn
        {
            get { return this._dataAppearance.HighColumn; }
            set { this._dataAppearance.HighColumn = value; }
        }

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
        [Description("Низшая категория")]
        [DisplayName("Имя категории, в которой содержаться низшие значения")]
        [Browsable(true)]
        public string LowColumn
        {
            get { return this._dataAppearance.LowColumn; }
            set { this._dataAppearance.LowColumn = value; }
        }

        [Category("Свойства")]
        [Description("Открытая категория")]
        [DisplayName("Имя категории, в которой содержаться открытые значения")]
        [Browsable(true)]
        public string OpenColumn
        {
            get { return this._dataAppearance.OpenColumn; }
            set { this._dataAppearance.OpenColumn = value; }
        }

        [Category("Свойства")]
        [Description("Категория величины")]
        [DisplayName("Имя категории, в которой содержаться величины")]
        [Browsable(true)]
        public string VolumeColumn
        {
            get { return this._dataAppearance.VolumeColumn; }
            set { this._dataAppearance.VolumeColumn = value; }
        }



        public CandleSeriesDataAppearanceBrowse(CandleSeriesDataAppearance dataAppearance)
        {
            this._dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return "";
        }


    }

}