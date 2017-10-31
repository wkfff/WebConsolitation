using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Диапазон осей
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisRangeBrowseClass
    {
        #region Поля 

        private AxisAppearance axisAppearance;
        private UltraChart chart;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип диапазона
        /// </summary>
        [Category("Оси")]
        [Description("Тип диапазона")]
        [DisplayName("Тип")]
        [TypeConverter(typeof(AxisRangeTypeConverter))]
        [DefaultValue(AxisRangeType.Automatic)]
        [Browsable(true)]
        public AxisRangeType RangeType
        {
            get { return axisAppearance.RangeType; }
            set
            {
                // проверяем диапазон на корректность
                if (!IsCorrectlyRangeForLog && value == AxisRangeType.Custom)
                {
                    //FormException.ShowErrorForm(new Exception("One or more of the following properties were invalid for NumericAxisType of Logarithmic scale.  Please check when AxisRangeType is Custom that both RangeMin and RangeMax are greater than zero."));
                    CommonUtils.ProcessException(new Exception("One or more of the following properties were invalid for NumericAxisType of Logarithmic scale.  Please check when AxisRangeType is Custom that both RangeMin and RangeMax are greater than zero."), true);
                    return;
                }

                if (!IsCorrectlyRangeForHeatMapChart(axisAppearance.RangeMin, axisAppearance.RangeMax, value))
                {
                    CommonUtils.ProcessException(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."), true);
                    //FormException.ShowErrorForm(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."));
                    return;
                }

                axisAppearance.RangeType = value;
            }
        }

        /// <summary>
        /// Минимальное значение диапазона
        /// </summary>
        [Category("Оси")]
        [Description("Минимальное значение диапазона. Работает при типе диапазона \"Пользовательский\".")]
        [DisplayName("Минимальное значение")]
        [DefaultValue(typeof(double), "0")]
        [Browsable(true)]
        public double RangeMin
        {
            get { return axisAppearance.RangeMin; }
            set
            {
                if (!IsCorrectlyRangeForHeatMapChart(value, axisAppearance.RangeMax, axisAppearance.RangeType))
                {
                    CommonUtils.ProcessException(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."), true);
                    //FormException.ShowErrorForm(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."));
                    return;
                }
                try
                {
                    axisAppearance.RangeMin = value;
                }
                catch
                {
                    axisAppearance.RangeType = AxisRangeType.Automatic;
                }

            }
        }

        /// <summary>
        /// Максимальное значение диапазона
        /// </summary>
        [Category("Оси")]
        [Description("Максимальное значение диапазона. Работает при типе диапазона \"Пользовательский\".")]
        [DisplayName("Максимальное значение")]
        [DefaultValue(typeof(double), "0")]
        [Browsable(true)]
        public double RangeMax
        {
            get { return axisAppearance.RangeMax; }
            set
            {
                if (!IsCorrectlyRangeForHeatMapChart(axisAppearance.RangeMin, value, axisAppearance.RangeType))
                {
                    CommonUtils.ProcessException(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."), true);
                    //FormException.ShowErrorForm(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."));
                    return;
                }
                try
                {
                    axisAppearance.RangeMax = value;
                }
                catch
                {
                    axisAppearance.RangeType = AxisRangeType.Automatic;
                }
            }
        }

        /// <summary>
        /// Корректно ли заданы границы диапазона
        /// </summary>
        [Browsable(false)]
        public bool IsCorrectlyRangeForLog
        {
            get
            {
                if (axisAppearance.NumericAxisType == NumericAxisType.Logarithmic)
                {
                    return axisAppearance.RangeMin > 0 && axisAppearance.RangeMin > 0;
                }
                return true;
            }
        }

        /// <summary>
        /// Корректно ли заданы границы диапазона
        /// </summary>
        public bool IsCorrectlyRangeForHeatMapChart(double minValue, double maxValue, AxisRangeType rangeType)
        {
                if ((axisAppearance.ChartComponent.ChartType == ChartType.HeatMapChart) &&
                    rangeType == AxisRangeType.Custom)
                {
                    return InfragisticsUtils.CheckHeatMapAxisAppearanceRange(chart, minValue,  maxValue, axisAppearance, rangeType);
                }
                return true;
        }

        #endregion

        public AxisRangeBrowseClass(AxisAppearance axisAppearance, UltraChart chart)
        {
            this.chart = chart;
            this.axisAppearance = axisAppearance;
        }

        public override string ToString()
        {
            return AxisRangeTypeConverter.ToString(RangeType) + "; " + RangeMin + "; " + RangeMax;
        }

    }
}
