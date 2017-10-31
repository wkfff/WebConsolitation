using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.BrowseAdapters.ChartReportAdapters.ChartConverters;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Отметки осей
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisTickmarkBrowseClass : FilterablePropertyBase
    {
        #region Поля 

        private AxisAppearance axisAppearance;
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
        /// Доступность логарифмического типа оси
        /// </summary>
        [Browsable(false)]
        public bool LogTypeEnable
        {
            get
            {
                return (ChartType == ChartType.AreaChart || ChartType == ChartType.SplineAreaChart ||
                        ChartType == ChartType.StackAreaChart || ChartType == ChartType.StackSplineAreaChart ||
                        ChartType == ChartType.StepAreaChart || ChartType == ChartType.BarChart ||
                        ChartType == ChartType.StackBarChart || ChartType == ChartType.GanttChart ||
                        ChartType == ChartType.BubbleChart || ChartType == ChartType.CandleChart ||
                        ChartType == ChartType.ColumnChart || ChartType == ChartType.ColumnLineChart ||
                        ChartType == ChartType.StackColumnChart || ChartType == ChartType.HeatMapChart ||
                        ChartType == ChartType.LineChart || ChartType == ChartType.ScatterLineChart ||
                        ChartType == ChartType.SplineChart || ChartType == ChartType.StackLineChart ||
                        ChartType == ChartType.StackSplineChart || ChartType == ChartType.StepLineChart ||
                        ChartType == ChartType.PolarChart || ChartType == ChartType.ParetoChart ||
                        ChartType == ChartType.RadarChart || ChartType == ChartType.BoxChart ||
                        ChartType == ChartType.ScatterChart || ChartType == ChartType.ProbabilityChart);
            }
        }

        /// <summary>
        /// Доступность логарифмических свойств оси
        /// </summary>
        [Browsable(false)]
        public bool LogPropEnable
        {
            get
            {
                return (axisAppearance.NumericAxisType == NumericAxisType.Logarithmic) &&
                        LogTypeEnable; 
            }
        }

        /// <summary>
        /// Доступность настройки стиля временной оси
        /// </summary>
        [Browsable(false)]
        public bool RulerGenreEnable
        {
            get
            {
                return (TickmarkIntervalType != AxisIntervalType.NotSet) &&
                       (ChartType == ChartType.StepLineChart || ChartType == ChartType.StepAreaChart ||
                       ChartType == ChartType.GanttChart);
            }
        }

        /// <summary>
        /// Корректно ли заданы границы диапазона
        /// </summary>
        [Browsable(false)]
        public bool IsCorrectlyRange
        {
            get
            {
                return CheckCorrectlyRange(chart.Axis.X) &&
                       CheckCorrectlyRange(chart.Axis.X2) &&
                       CheckCorrectlyRange(chart.Axis.Y) &&
                       CheckCorrectlyRange(chart.Axis.Y2);
            }
        }

        /// <summary>
        /// Единицы отметок времени
        /// </summary>
        [Category("Оси")]
        [Description("Единицы отметок времени")]
        [DisplayName("Единицы отметок времени")]
        [TypeConverter(typeof(AxisIntervalTypeConverter))]
        [DefaultValue(AxisIntervalType.NotSet)]
        [Browsable(true)]
        public AxisIntervalType TickmarkIntervalType
        {
            get { return axisAppearance.TickmarkIntervalType; }
            set { axisAppearance.TickmarkIntervalType = value; }
        }

        /// <summary>
        /// Интервал отметок
        /// </summary>
        [Category("Оси")]
        [Description("Интервал отметок")]
        [DisplayName("Интервал отметок")]
        [DynamicPropertyFilter("TickmarkStyle", "DataInterval")]
        [DefaultValue(typeof(double), "0")]
        [Browsable(true)]
        public double TickmarkInterval
        {
            get { return axisAppearance.TickmarkInterval; }
            set { axisAppearance.TickmarkInterval = value; }
        }

        /// <summary>
        /// Процентное соотношение отметок
        /// </summary>
        [Category("Оси")]
        [Description("Процентное соотношение отметок")]
        [DisplayName("Процентное соотношение отметок")]
        [DynamicPropertyFilter("TickmarkStyle", "Percentage")]
        [DefaultValue(typeof(double), "10")]
        [Browsable(true)]
        public double TickmarkPercentage
        {
            get { return axisAppearance.TickmarkPercentage; }
            set { axisAppearance.TickmarkPercentage = value; }
        }

        /// <summary>
        /// Стиль оси
        /// </summary>
        [Category("Оси")]
        [Description("Стиль оси")]
        [DisplayName("Стиль оси")]
        [TypeConverter(typeof(RulerGenreTypeConverter))]
        [DynamicPropertyFilter("RulerGenreEnable", "True")]
        [DefaultValue(RulerGenre.Continuous)]
        [Browsable(true)]
        public RulerGenre TimeAxisStyle
        {
            get { return axisAppearance.TimeAxisStyle.TimeAxisStyle; }
            set { axisAppearance.TimeAxisStyle.TimeAxisStyle = value; }
        }

        /// <summary>
        /// Разбиение оси
        /// </summary>
        [Category("Оси")]
        [Description("Разбиение оси")]
        [DisplayName("Разбиение оси")]
        [DefaultValue(AxisTickStyle.Percentage)]
        [TypeConverter(typeof(AxisTickStyleTypeConverter))]
        [Browsable(true)]
        public AxisTickStyle TickmarkStyle
        {
            get { return axisAppearance.TickmarkStyle; }
            set { axisAppearance.TickmarkStyle = value; }
        }

        /// <summary>
        /// Тип оси
        /// </summary>
        [Category("Оси")]
        [Description("Тип оси")]
        [DisplayName("Тип оси")]
        [TypeConverter(typeof(NumericAxisTypeConverter))]
        [DynamicPropertyFilter("LogTypeEnable", "True")]
        [DefaultValue(NumericAxisType.Linear)]
        [Browsable(true)]
        public NumericAxisType NumericAxisType
        {
            get { return axisAppearance.NumericAxisType; }
            set 
            {
                // проверяем диапазон на корректность
                if (!IsCorrectlyRange && value == NumericAxisType.Logarithmic)
                {
                    //FormException.ShowErrorForm(new Exception("One or more of the following properties were invalid for NumericAxisType of Logarithmic scale.  Please check when AxisRangeType is Custom that both RangeMin and RangeMax are greater than zero."));
                    CommonUtils.ProcessException(new Exception("One or more of the following properties were invalid for NumericAxisType of Logarithmic scale.  Please check when AxisRangeType is Custom that both RangeMin and RangeMax are greater than zero."), true);
                    return;
                }

                //т.к. при выставлении типа отметок в логарифмический может 
                //возникнуть исключение если диаграмма отображается, поэтому 
                //будем ее скрывать, а если ошибки нет то при отрисовке она отобразится
                chart.Visible = false;
                axisAppearance.NumericAxisType = value;
                this.chart.Parent.Invalidate(true);
            }
        }

        /// <summary>
        /// Логарифмический ноль
        /// </summary>
        [Category("Оси")]
        [Description("Логарифмический ноль")]
        [DisplayName("Логарифмический ноль")]
        [DynamicPropertyFilter("LogPropEnable", "True")]
        [DefaultValue(typeof(double), "NaN")]
        [Browsable(true)]
        public double LogZero
        {
            get { return axisAppearance.LogZero; }
            set { axisAppearance.LogZero = value; }
        }
        
        /// <summary>
        /// Логарифмическое основание
        /// </summary>
        [Category("Оси")]
        [Description("Логарифмическое основание")]
        [DisplayName("Логарифмическое основание")]
        [DynamicPropertyFilter("LogPropEnable", "True")]
        [DefaultValue(typeof(double), "10")]
        [Browsable(true)]
        public double LogBase
        {
            get { return axisAppearance.LogBase; }
            set { axisAppearance.LogBase = value; }
        }

        #endregion

        public AxisTickmarkBrowseClass(AxisAppearance axisAppearance, UltraChart chart)
        {
            this.axisAppearance = axisAppearance;
            this.chart = chart;
        }

        private static bool CheckCorrectlyRange(AxisAppearance axis)
        {
            if (axis.RangeType == AxisRangeType.Custom)
            {
                return axis.RangeMin > 0 && axis.RangeMin > 0;
            }
            return true;
        }

        public override string ToString()
        {
            string str = LogTypeEnable ? NumericAxisTypeConverter.ToString(NumericAxisType) + "; " : string.Empty;
            return str + AxisTickStyleTypeConverter.ToString(TickmarkStyle) + "; " +
                   AxisIntervalTypeConverter.ToString(TickmarkIntervalType);
        }
    }
}
