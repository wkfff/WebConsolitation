using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Сетки оси
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisGroupGridBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private AxisAppearance axisAppearance;
        private AxisGridLinesBrowseClass axisMajorGridLinesBrowse;
        private AxisGridLinesBrowseClass axisMinorGridLinesBrowse;
        private UltraChart chart;

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
        /// Номер оси
        /// </summary>
        [Browsable(false)]
        public AxisNumber AxisNumber
        {
            get { return axisAppearance.axisNumber; }
        }

        /// <summary>
        /// Объемная ли диаграмма с осями
        /// </summary>
        [Browsable(false)]
        public bool Is3D
        {
            get {
                return
                    (ChartType == ChartType.AreaChart3D || ChartType == ChartType.BarChart3D ||
                     ChartType == ChartType.ColumnChart3D || ChartType == ChartType.LineChart3D ||
                     ChartType == ChartType.CylinderBarChart3D || ChartType == ChartType.CylinderColumnChart3D ||
                     ChartType == ChartType.CylinderStackBarChart3D || ChartType == ChartType.BubbleChart3D ||
                     ChartType == ChartType.CylinderStackColumnChart3D || ChartType == ChartType.HeatMapChart3D ||
                     ChartType == ChartType.SplineAreaChart3D || ChartType == ChartType.PointChart3D ||
                     ChartType == ChartType.SplineChart3D || ChartType == ChartType.Stack3DBarChart ||
                     ChartType == ChartType.Stack3DColumnChart);
            }
        }

        /// <summary>
        /// Плоская ли диаграмма с осями 1го вида
        /// </summary>
        [Browsable(false)]
        public bool Is2D1
        {
            get
            {
                return
                       (ChartType == ChartType.BarChart ||
                        ChartType == ChartType.BubbleChart ||
                        ChartType == ChartType.ColumnChart || 
                        ChartType == ChartType.ColumnLineChart ||
                        ChartType == ChartType.StackColumnChart ||
                        ChartType == ChartType.StackBarChart ||
                        ChartType == ChartType.HeatMapChart ||
                        ChartType == ChartType.ScatterLineChart || 
                        ChartType == ChartType.ScatterChart ||
                        ChartType == ChartType.BoxChart ||
                        ChartType == ChartType.ParetoChart ||
                        ChartType == ChartType.HistogramChart ||
                        ChartType == ChartType.ProbabilityChart ||
                        ChartType == ChartType.CandleChart ||
                        ChartType == ChartType.GanttChart);
            }
        }

        /// <summary>
        /// Плоская ли диаграмма с осями 2го вида
        /// </summary>
        [Browsable(false)]
        public bool Is2D2
        {
            get
            {
                return
                    (ChartType == ChartType.AreaChart ||
                     ChartType == ChartType.SplineAreaChart ||
                     ChartType == ChartType.StackAreaChart ||
                     ChartType == ChartType.StackSplineAreaChart ||
                     ChartType == ChartType.LineChart ||
                     ChartType == ChartType.SplineChart ||
                     ChartType == ChartType.StackLineChart ||
                     ChartType == ChartType.StackSplineChart ||
                     ChartType == ChartType.StepAreaChart ||
                     ChartType == ChartType.StepLineChart);
            }
        }

        /// <summary>
        /// Доступность основной сетки
        /// </summary>
        [Browsable(false)]
        public bool MajorGridEnable
        {
            get {
                return (Is3D && (AxisNumber == AxisNumber.X_Axis || AxisNumber == AxisNumber.Y_Axis || AxisNumber == AxisNumber.Z_Axis)) ||
                       (Is2D1 || (Is2D2 && (AxisNumber == AxisNumber.Y_Axis || AxisNumber == AxisNumber.Y2_Axis))) || 
                       (ChartType == ChartType.PolarChart) || 
                       (ChartType == ChartType.RadarChart && AxisNumber == AxisNumber.Y_Axis);
                }
        }

        /// <summary>
        /// Доступность второстепенной сетки
        /// </summary>
        [Browsable(false)]
        public bool MinorGridEnable
        {
            get
            {
                return (Is2D1 || Is2D2) ||
                       (ChartType == ChartType.PolarChart && AxisNumber == AxisNumber.Y_Axis);
            }
        }

        /// <summary>
        /// Основная сетка
        /// </summary>
        [Category("Оси")]
        [Description("Линии основной сетки")]
        [DisplayName("Основная сетка")]
        [DynamicPropertyFilter("MajorGridEnable", "True")]
        [Browsable(true)]
        public AxisGridLinesBrowseClass AxisMajorGridLinesBrowse
        {
            get { return axisMajorGridLinesBrowse; }
            set { axisMajorGridLinesBrowse = value; }
        }

        /// <summary>
        /// Второстепенная сетка
        /// </summary>
        [Category("Оси")]
        [Description("Линии второстепенной сетки")]
        [DisplayName("Второстепенная сетка")]
        [DynamicPropertyFilter("MinorGridEnable", "True")]
        [Browsable(true)]
        public AxisGridLinesBrowseClass AxisMinorGridLinesBrowse
        {
            get { return axisMinorGridLinesBrowse; }
            set { axisMinorGridLinesBrowse = value; }
        }

        #endregion

        public AxisGroupGridBrowseClass(AxisAppearance axisAppearance, UltraChart chart)
        {
            this.axisAppearance = axisAppearance;
            axisMajorGridLinesBrowse = new AxisGridLinesBrowseClass(axisAppearance.MajorGridLines, chart, axisAppearance);
            axisMinorGridLinesBrowse = new AxisGridLinesBrowseClass(axisAppearance.MinorGridLines, chart, axisAppearance);

            this.chart = chart;
        }

        public override string ToString()
        {
            string sepStr = (MinorGridEnable && MajorGridEnable) ? "; " : String.Empty;
            string str = MinorGridEnable ? BooleanTypeConverter.ToString(axisMinorGridLinesBrowse.LinesVisible) + sepStr : string.Empty;
            return MajorGridEnable ? str + BooleanTypeConverter.ToString(axisMajorGridLinesBrowse.LinesVisible) : str;
        }
    }
}
