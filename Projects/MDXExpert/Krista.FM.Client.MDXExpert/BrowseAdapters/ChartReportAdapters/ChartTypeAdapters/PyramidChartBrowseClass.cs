using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PyramidChartBrowseClass
    {
        #region Поля

        private PyramidChartAppearance pyramidChartAppearance;
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
            get { return pyramidChartAppearance.Axis; }
            set { pyramidChartAppearance.Axis = value; }
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
            get { return pyramidChartAppearance.OthersCategoryText; }
            set { pyramidChartAppearance.OthersCategoryText = value; }
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
            get { return pyramidChartAppearance.OthersCategoryPercent; }
            set { pyramidChartAppearance.OthersCategoryPercent = value; }
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
            get { return pyramidChartAppearance.Sort; }
            set { pyramidChartAppearance.Sort = value; }
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
            get { return pyramidChartAppearance.Spacing; }
            set { pyramidChartAppearance.Spacing = value; }
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
            get { return pyramidChartAppearance.ChartText; }
        }

        #endregion

        public PyramidChartBrowseClass(PyramidChartAppearance pyramidChartAppearance)
        {
            this.pyramidChartAppearance = pyramidChartAppearance;
            chartLabelsAppearance = new HierarchicalChartLabelsApperanceBrowseClass(pyramidChartAppearance.Labels);
        }

        public override string ToString()
        {
            return Axis + "; " + SortStyleTypeConverter.ToString(Sort);
        }
    }
}