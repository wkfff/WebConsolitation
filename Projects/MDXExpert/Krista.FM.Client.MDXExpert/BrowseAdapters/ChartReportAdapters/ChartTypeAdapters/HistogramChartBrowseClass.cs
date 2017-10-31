using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HistogramChartBrowseClass
    {
        #region Поля

        private HistogramColumnBrowseClass histogramColumnBrowse;
        private HistogramLineBrowseClass histogramLineBrowse;
        private HistogramChartAppearance histogramChartAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Колонки
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение колонок в диаграмме")]
        [DisplayName("Колонки")]
        [Browsable(true)]
        public HistogramColumnBrowseClass HistogramColumnBrowse
        {
            get { return histogramColumnBrowse; }
            set { histogramColumnBrowse = value; }
        }

        /// <summary>
        /// Линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение линий в диаграмме")]
        [DisplayName("Линии")]
        [Browsable(true)]
        public HistogramLineBrowseClass HistogramLineBrowse
        {
            get { return histogramLineBrowse; }
            set { histogramLineBrowse = value; }
        }

        /// <summary>
        /// Индекс колонки диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс колонки диаграммы")]
        [DisplayName("Индекс колонки")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColumnIndex
        {
            get { return histogramChartAppearance.ColumnIndex; }
            set { histogramChartAppearance.ColumnIndex = value; }
        }

        #endregion

        public HistogramChartBrowseClass(HistogramChartAppearance histogramChartAppearance)
        {
            this.histogramChartAppearance = histogramChartAppearance;
            histogramColumnBrowse = new HistogramColumnBrowseClass(histogramChartAppearance.ColumnAppearance);
            histogramLineBrowse = new HistogramLineBrowseClass(histogramChartAppearance.LineAppearance);
        }

        public override string ToString()
        {
            return HistogramLineBrowse + "; " + HistogramColumnBrowse;
        }
    }
}