using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartAreaBrowseClass : FilterablePropertyBase
    {
        private ChartArea _chartArea;
        private PaintElementBrowseClass _paintElementBrowse;
        private PaintElementBrowseClass _gridPaintElementBrowse;
        private BorderBrowseClass _borderBrowse;
       //private RectangleBrowseClass _boundsBrowse;
        /*
        [Category("Свойства")]
        [Description("Оси")]
        [DisplayName("Оси")]
        [Browsable(true)]
        public AxisCollection Axes
        {
            get { return this._chartArea.Axes; }
        }*/

        [Category("Свойства")]
        [Description("Рамка")]
        [DisplayName("Рамка")]
        [Browsable(true)]
        public BorderBrowseClass Border
        {
            get { return this._borderBrowse; }
            set { this._borderBrowse = value; }
        }
        /*
        [Category("Свойства")]
        [Description("Границы")]
        [DisplayName("Границы")]
        [Browsable(true)]
        public RectangleBrowseClass Bounds
        {
            get { return this._boundsBrowse; }
            set { this._boundsBrowse = value; }
        }*/

        [Category("Свойства")]
        [Description("Тип измерения границ")]
        [DisplayName("Тип измерения границ")]
        [TypeConverter(typeof(MeasureTypeConverter))]
        [Browsable(true)]
        public MeasureType BoundsMeasureType
        {
            get { return this._chartArea.BoundsMeasureType; }
            set { this._chartArea.BoundsMeasureType = value; }
        }

        [Category("Свойства")]
        [Description("Стиль отображения сетки")]
        [DisplayName("Стиль отображения сетки")]
        [Browsable(true)]
        public PaintElementBrowseClass GridPE
        {
            get { return this._gridPaintElementBrowse; }
            set { this._gridPaintElementBrowse = value; }
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
        [Description("Показывать")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._chartArea.Visible; }
            set { this._chartArea.Visible = value; }
        }



        public ChartAreaBrowseClass(ChartArea chartArea)
        {
            this._chartArea = chartArea;
            this._paintElementBrowse = new PaintElementBrowseClass(chartArea.PE);
            this._gridPaintElementBrowse = new PaintElementBrowseClass(chartArea.GridPE);
            this._borderBrowse = new BorderBrowseClass(chartArea.Border);
            //this._boundsBrowse = new RectangleBrowseClass(chartArea.Bounds);
        }

        public override string ToString()
        {
            return "Область диаграммы";
        }
    }
}