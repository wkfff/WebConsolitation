using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Колонки в диаграмме HistogramChart
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HistogramColumnBrowseClass
    {
        #region Поля

        private HistogramColumnAppearance histogramColumnApperance;

        #endregion

        #region Свойства

        /// <summary>
        /// Расстояние между колонками
        /// </summary>
        [Category("Колонки")]
        [Description("Расстояние между колонками диаграммы")]
        [DisplayName("Расстояние между колонками")]
        [Browsable(true)]
        public double ColumnSpacing
        {
            get { return histogramColumnApperance.ColumnSpacing; }
            set { histogramColumnApperance.ColumnSpacing = value; }
        }

        /// <summary>
        /// Отображение колонок в легенде диаграммы
        /// </summary>
        [Category("Колонки")]
        [Description("Отображение колонок в легенде диаграммы")]
        [DisplayName("Отображение в легенде")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ShowInLegend
        {
            get { return histogramColumnApperance.ShowInLegend; }
            set { histogramColumnApperance.ShowInLegend = value; }
        }

        /// <summary>
        /// Видимость колонок в диаграмме
        /// </summary>
        [Category("Колонки")]
        [Description("Показывать колонки в диаграмме")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return histogramColumnApperance.Visible; }
            set { histogramColumnApperance.Visible = value; }
        }

        /// <summary>
        /// Строковые оси
        /// </summary>
        [Category("Колонки")]
        [Description("Строковые оси")]
        [DisplayName("Строковые оси")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool StringAxis
        {
            get { return histogramColumnApperance.StringAxis; }
            set { histogramColumnApperance.StringAxis = value; }
        }

        #endregion

        public HistogramColumnBrowseClass(HistogramColumnAppearance histogramColumnApperance)
        {
            this.histogramColumnApperance = histogramColumnApperance;
        }

        public override string ToString()
        {
            return ColumnSpacing.ToString();
        }
    }
}