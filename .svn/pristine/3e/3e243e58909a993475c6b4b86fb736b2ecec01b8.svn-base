using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FunnelChartBrowseClass
    {
        #region Поля

        private FunnelChartAppearance funnelChartAppearance;
        private HierarchicalChartLabelsApperanceBrowseClass chartLabelsAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Расположение осей
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расположение осей диаграммы")]
        [DisplayName("Расположение осей")]
        [Browsable(true)]
        public HierarchicalChartAxis Axis
        {
            get { return funnelChartAppearance.Axis; }
            set { funnelChartAppearance.Axis = value; }
        }

        /// <summary>
        /// Метки
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Метки диаграммы")]
        [DisplayName("Метки")]
        [Browsable(true)]
        public HierarchicalChartLabelsApperanceBrowseClass ChartLabelsAppearance
        {
            get { return chartLabelsAppearance; }
            set { chartLabelsAppearance = value; }
        }

        /// <summary>
        /// Подпись слоя "Прочие"
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подпись слоя \"Прочие\"")]
        [DisplayName("Подпись слоя \"Прочие\"")]
        [Browsable(true)]
        public string OthersCategoryText
        {
            get { return funnelChartAppearance.OthersCategoryText; }
            set { funnelChartAppearance.OthersCategoryText = value; }
        }

        /// <summary>
        /// Минимальный процент слоя
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Минимальный процент слоя")]
        [DisplayName("Минимальный процент слоя")]
        [Browsable(true)]
        public double OthersCategoryPercent
        {
            get { return funnelChartAppearance.OthersCategoryPercent; }
            set { funnelChartAppearance.OthersCategoryPercent = value; }
        }

        /// <summary>
        /// Сортировка слоев
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Сортировка слоев")]
        [DisplayName("Сортировка")]
        [TypeConverter(typeof(SortStyleTypeConverter))]
        [Browsable(true)]
        public SortStyle Sort
        {
            get { return funnelChartAppearance.Sort; }
            set { funnelChartAppearance.Sort = value; }
        }

        /// <summary>
        /// Процент толщины отображения слоя
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Процент толщины отображения слоя")]
        [DisplayName("Процент толщины отображения слоя")]
        [Browsable(true)]
        public double Spacing
        {
            get { return funnelChartAppearance.Spacing; }
            set { funnelChartAppearance.Spacing = value; }
        }

        /// <summary>
        /// Минимальный радиус
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Минимальный радиус")]
        [DisplayName("Минимальный радиус")]
        [Browsable(true)]
        public double RadiusMin
        {
            get { return funnelChartAppearance.RadiusMin; }
            set { funnelChartAppearance.RadiusMin = value; }
        }

        /// <summary>
        /// Максимальный радиус
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Максимальный радиус диаграммы")]
        [DisplayName("Максимальный радиус")]
        [Browsable(true)]
        public double RadiusMax
        {
            get { return funnelChartAppearance.RadiusMax; }
            set { funnelChartAppearance.RadiusMax = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return funnelChartAppearance.ChartText; }
        }

        #endregion

        public FunnelChartBrowseClass(FunnelChartAppearance funnelChartAppearance)
        {
            this.funnelChartAppearance = funnelChartAppearance;
            chartLabelsAppearance = new HierarchicalChartLabelsApperanceBrowseClass(funnelChartAppearance.Labels);
        }

        public override string ToString()
        {
            return Axis + "; " + SortStyleTypeConverter.ToString(Sort);
        }
    }
}