using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Оси координат
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxiesBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private ChartGridAppearance chartGridApperance;
        private UltraChart chart;

        private AxisBrowseClass xAxisBrowse;
        private AxisBrowseClass yAxisBrowse;
        private AxisBrowseClass zAxisBrowse;
        private AxisBrowseClass x2AxisBrowse;
        private AxisBrowseClass y2AxisBrowse;
        private PaintElementBrowseClass paintElementBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип диаграммы
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
        }

        /// <summary>
        /// Цвет фона
        /// </summary>
        [Category("Оси координат")]
        [Description("Цвет фона")]
        [DisplayName("Цвет фона")]
        [DynamicPropertyFilter("ChartType", "AreaChart3D, BarChart3D, BubbleChart3D, "
                                            + "ColumnChart3D, CylinderBarChart3D, CylinderColumnChart3D, "
                                            + "CylinderStackBarChart3D, CylinderStackColumnChart3D, "
                                            + "HeatMapChart3D, LineChart3D, PointChart3D, "
                                            + "SplineAreaChart3D, SplineChart3D, Stack3DBarChart, Stack3DColumnChart")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return chartGridApperance.BackColor; }
            set { chartGridApperance.BackColor = value; }
        }

        /// <summary>
        /// Ось X
        /// </summary>
        [Category("Оси координат")]
        [Description("Ось X")]
        [DisplayName("Ось X")]
        [Browsable(true)]
        public AxisBrowseClass XAxisBrowse
        {
            get { return xAxisBrowse; }
            set { xAxisBrowse = value; }
        }

        /// <summary>
        /// Ось Y
        /// </summary>
        [Category("Оси координат")]
        [Description("Ось Y")]
        [DisplayName("Ось Y")]
        [Browsable(true)]
        public AxisBrowseClass YAxisBrowse
        {
            get { return yAxisBrowse; }
            set { yAxisBrowse = value; }
        }

        /// <summary>
        /// Ось Z
        /// </summary>
        [Category("Оси координат")]
        [Description("Ось Z")]
        [DisplayName("Ось Z")]
        [DynamicPropertyFilter("ChartType", "AreaChart3D, BarChart3D, BubbleChart3D, "
                                            + "ColumnChart3D, CylinderBarChart3D, CylinderColumnChart3D, "
                                            + "CylinderStackBarChart3D, CylinderStackColumnChart3D, "
                                            + "HeatMapChart3D, LineChart3D, PointChart3D, "
                                            + "SplineAreaChart3D, SplineChart3D, Stack3DBarChart, Stack3DColumnChart")]
        [Browsable(true)]
        public AxisBrowseClass ZAxisBrowse
        {
            get { return zAxisBrowse; }
            set { zAxisBrowse = value; }
        }

        /// <summary>
        /// Ось X2
        /// </summary>
        [Category("Оси координат")]
        [Description("Ось X2")]
        [DisplayName("Ось X2")]
        [DynamicPropertyFilter("ChartType", "ColumnChart, BarChart, AreaChart, LineChart, BubbleChart, "
                                            + "ScatterChart, HeatMapChart, StackBarChart, StackColumnChart, "
                                            + "SplineChart, SplineAreaChart, ColumnLineChart, ScatterLineChart, ParetoChart, "
                                            + "StackAreaChart, StackLineChart, StackSplineChart, StackSplineAreaChart, HistogramChart, "
                                            + "ProbabilityChart, BoxChart, GanttChart")]
        [Browsable(true)]
        public AxisBrowseClass X2AxisBrowse
        {
            get { return x2AxisBrowse; }
            set { x2AxisBrowse = value; }
        }

        /// <summary>
        /// Ось Y2
        /// </summary>
        [Category("Оси координат")]
        [Description("Ось Y2")]
        [DisplayName("Ось Y2")]
        [DynamicPropertyFilter("ChartType", "ColumnChart, BarChart, AreaChart, LineChart, BubbleChart, "
                                            + "ScatterChart, HeatMapChart, StackBarChart, StackColumnChart, "
                                            + "SplineChart, SplineAreaChart, ColumnLineChart, ScatterLineChart, ParetoChart, "
                                            + "StackAreaChart, StackLineChart, StackSplineChart, StackSplineAreaChart, HistogramChart, "
                                            + "ProbabilityChart, BoxChart, GanttChart")]
        [Browsable(true)]
        public AxisBrowseClass Y2AxisBrowse
        {
            get { return y2AxisBrowse; }
            set { y2AxisBrowse = value; }
        }

        /// <summary>
        /// Элемент отображения
        /// </summary>
        [Category("Оси координат")]
        [Description("Элемент отображения")]
        [DisplayName("Элемент отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        #endregion

        public AxiesBrowseClass(ChartGridAppearance chartGridApperance, UltraChart chart)
        {
            this.chartGridApperance = chartGridApperance;
            this.chart = chart;

            xAxisBrowse = new AxisBrowseClass(chartGridApperance.X, chart);
            yAxisBrowse = new AxisBrowseClass(chartGridApperance.Y, chart);
            zAxisBrowse = new AxisBrowseClass(chartGridApperance.Z, chart);
            x2AxisBrowse = new AxisBrowseClass(chartGridApperance.X2, chart);
            y2AxisBrowse = new AxisBrowseClass(chartGridApperance.Y2, chart);
            paintElementBrowse = new PaintElementBrowseClass(chartGridApperance.PE);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}