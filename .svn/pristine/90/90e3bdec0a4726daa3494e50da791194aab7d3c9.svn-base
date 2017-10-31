using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomXYZSeries : FilterablePropertyBase
    {
        private XYZSeries _xyzSeries;
        private XYZSeriesDataAppearanceBrowse _dataAppearanceBrowse;

        [Category("Свойства")]
        [Description("Данные")]
        [DisplayName("Данные")]
        [Browsable(true)]
        public XYZSeriesDataAppearanceBrowse Data
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
            get { return this._xyzSeries.Label; }
            set { this._xyzSeries.Label = value; }
        }
        /*
        [Category("Свойства")]
        [Description("Стили элементов отображения")]
        [DisplayName("Стили элементов отображения")]
        [Browsable(true)]
        public PaintElementCollection PEs
        {
            get { return this._xySeries.PEs; }
        }

        [Category("Свойства")]
        [Description("Точки данных")]
        [DisplayName("Точки данных")]
        [Browsable(true)]
        public XYDataPointCollection Points
        {
            get { return this._xySeries.Points; }
        }
        */
        [Category("Свойства")]
        [Description("Показывать")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._xyzSeries.Visible; }
            set { this._xyzSeries.Visible = value; }
        }


        public CustomXYZSeries(XYZSeries xyzSeries)
        {
            this._xyzSeries = xyzSeries;
            this._dataAppearanceBrowse = new XYZSeriesDataAppearanceBrowse(xyzSeries.Data);
        }

        public override string ToString()
        {
            return "";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class XYZSeriesDataAppearanceBrowse : FilterablePropertyBase
    {
        private XYZSeriesDataAppearance _dataAppearance;

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


        public XYZSeriesDataAppearanceBrowse(XYZSeriesDataAppearance dataAppearance)
        {
            this._dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return "";
        }


    }

}