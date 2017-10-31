using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ConeChart3DBrowseClass
    {
        #region Поля

        private Cone3DAppearance cone3DAppearance;
        private HierarchicalChart3DLabelsApperanceBrowseClass chart3DLabelsAppearance;

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
            get { return cone3DAppearance.Axis; }
            set { cone3DAppearance.Axis = value; }
        }

        /// <summary>
        /// Метки
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Метки диаграммы")]
        [DisplayName("Метки")]
        [Browsable(true)]
        public HierarchicalChart3DLabelsApperanceBrowseClass Chart3DLabelsAppearance
        {
            get { return chart3DLabelsAppearance; }
            set { chart3DLabelsAppearance = value; }
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
            get { return cone3DAppearance.OthersCategoryText; }
            set { cone3DAppearance.OthersCategoryText = value; }
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
            get { return cone3DAppearance.OthersCategoryPercent; }
            set { cone3DAppearance.OthersCategoryPercent = value; }
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
            get { return cone3DAppearance.Sort; }
            set { cone3DAppearance.Sort = value; }
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
            get { return cone3DAppearance.Spacing; }
            set { cone3DAppearance.Spacing = value; }
        }

        #endregion

        public ConeChart3DBrowseClass(Cone3DAppearance cone3DAppearance)
        {
            this.cone3DAppearance = cone3DAppearance;
            chart3DLabelsAppearance = new HierarchicalChart3DLabelsApperanceBrowseClass(cone3DAppearance.Labels);
        }

        public override string ToString()
        {
            return Axis + "; " + SortStyleTypeConverter.ToString(Sort);
        }
    }
}