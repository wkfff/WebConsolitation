using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Core.ColorModel;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Data.Series;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    public class ChartLayerAppearanceBrowseClass : FilterablePropertyBase
    {
        private ChartLayerAppearance _chartLayer;
        private ChartAreaBrowseClass _chartAreaBrowse;
        private AxisBrowseClass _axisXBrowse;
        private AxisBrowseClass _axisYBrowse;
        private AxisBrowseClass _axisY2Browse;

        [Category("Свойства")]
        [Description("Ось X")]
        [DisplayName("Ось X")]
        [Browsable(true)]
        public AxisBrowseClass AxisX
        {
            get { return this._axisXBrowse; }
            set { this._axisXBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Ось Y")]
        [DisplayName("Ось Y")]
        [Browsable(true)]
        public AxisBrowseClass AxisY
        {
            get { return this._axisYBrowse; }
            set { this._axisYBrowse = value; }
        }


        [Category("Свойства")]
        [Description("Ось Y2")]
        [DisplayName("Ось Y2")]
        [Browsable(true)]
        public AxisBrowseClass AxisY2
        {
            get { return this._axisY2Browse; }
            set { this._axisY2Browse = value; }
        }

        [Category("Свойства")]
        [Description("Область диаграммы")]
        [DisplayName("Область диаграммы")]
        [Browsable(true)]
        public ChartAreaBrowseClass ChartArea
        {
            get { return this._chartAreaBrowse; }
            set { this._chartAreaBrowse = value; }
        }
        /*
        [Category("Свойства")]
        [Description("Слой диаграммы")]
        [DisplayName("Слой диаграммы")]
        [Browsable(true)]
        public ChartLayer ChartLayer
        {
            get { return this._chartLayer.ChartLayer.; }
            set { this._chartLayer.ChartLayer = value; }
        }*/

        [Category("Свойства")]
        [Description("Тип диаграммы")]
        [DisplayName("Тип диаграммы")]
        [TypeConverter(typeof(ChartTypeConverter))]
        [Browsable(true)]
        public ChartType ChartType
        {
            get { return this._chartLayer.ChartType; }
            set { this._chartLayer.ChartType = value; }
        }
        /*
        [Category("Свойства")]
        [Description("Внешний вид диаграммы")]
        [DisplayName("Внешний вид диаграммы")]
        [Browsable(true)]
        public ChartTypeAppearance ChartTypeAppearance
        {
            get { return this._chartLayer.ChartTypeAppearance.; }
            set { this._chartLayer.ChartTypeAppearance = value; }
        }*/

        [Category("Свойства")]
        [Description("Тип элемента легенды")]
        [DisplayName("Тип элемента легенды")]
        [TypeConverter(typeof(LegendItemTypeConverter))]
        [Browsable(true)]
        public LegendItemType LegendItem
        {
            get { return this._chartLayer.LegendItem; }
            set { this._chartLayer.LegendItem = value; }
        }
        /*
        [Category("Свойства")]
        [Description("Серии")]
        [DisplayName("Серии")]
        [Browsable(true)]
        public SeriesCollection Series
        {
            get { return this._chartLayer.Series; }
        }
        */

        [Category("Свойства")]
        [Description("Поменять ряды и категории")]
        [DisplayName("Поменять ряды и категории")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool SwapRowsAndColumns
        {
            get { return this._chartLayer.SwapRowsAndColumns; }
            set { this._chartLayer.SwapRowsAndColumns = value; }
        }

        [Category("Свойства")]
        [Description("Показывать")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._chartLayer.Visible; }
            set { this._chartLayer.Visible = value; }
        }



        public ChartLayerAppearanceBrowseClass(ChartLayerAppearance chartLayer)
        {
            this._chartLayer = chartLayer;
            this._chartAreaBrowse = new ChartAreaBrowseClass(chartLayer.ChartArea);

            if (MainForm.Instance.ActiveChartElement != null)
            {
                UltraChart chart = MainForm.Instance.ActiveChartElement.Chart;
                this._axisXBrowse = new AxisBrowseClass(chartLayer.AxisX, chart);
                this._axisYBrowse = new AxisBrowseClass(chartLayer.AxisY, chart);
                this._axisY2Browse = new AxisBrowseClass(chartLayer.AxisY2, chart);
            }
        }

        public override string ToString()
        {
            return "Слой";
        }
    }
}