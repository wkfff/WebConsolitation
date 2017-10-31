using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HeatMapChartBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private HeatMapChartAppearance heatmapChartAppearance;
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
        /// Качество представления
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Качество представления диаграммы")]
        [DisplayName("Качество представления")]
        [DefaultValue(HeatMapRenderQuality.LowQuality)]
        [Browsable(true)]
        public HeatMapRenderQuality RenderQuality
        {
            get { return heatmapChartAppearance.RenderQuality; }
            set { heatmapChartAppearance.RenderQuality = value; }
        }

        /// <summary>
        /// Отображение пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение пустых значений")]
        [DisplayName("Пустые значения")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [DefaultValue(NullHandling.Zero)]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return heatmapChartAppearance.NullHandling; }
            set { heatmapChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [DynamicPropertyFilter("ChartType", "HeatMapChart")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return heatmapChartAppearance.ChartText; }
        }

        #endregion

        public HeatMapChartBrowseClass(HeatMapChartAppearance heatmapChartAppearance, UltraChart chart)
        {
            this.heatmapChartAppearance = heatmapChartAppearance;
            this.chart = chart;
        }

        public override string ToString()
        {
            return RenderQuality.ToString();
        }
    }
}