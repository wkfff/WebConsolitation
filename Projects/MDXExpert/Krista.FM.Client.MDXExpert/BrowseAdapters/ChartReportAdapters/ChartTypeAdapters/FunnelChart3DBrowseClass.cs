using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FunnelChart3DBrowseClass
    {
        #region Поля

        private Funnel3DAppearance funnel3DAppearance;
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
            get { return funnel3DAppearance.Axis; }
            set { funnel3DAppearance.Axis = value; }
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
            get { return funnel3DAppearance.OthersCategoryText; }
            set { funnel3DAppearance.OthersCategoryText = value; }
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
            get { return funnel3DAppearance.OthersCategoryPercent; }
            set { funnel3DAppearance.OthersCategoryPercent = value; }
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
            get { return funnel3DAppearance.Sort; }
            set { funnel3DAppearance.Sort = value; }
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
            get { return funnel3DAppearance.Spacing; }
            set { funnel3DAppearance.Spacing = value; }
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
            get { return funnel3DAppearance.RadiusMin; }
            set { funnel3DAppearance.RadiusMin = value; }
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
            get { return funnel3DAppearance.RadiusMax; }
            set { funnel3DAppearance.RadiusMax = value; }
        }

        /// <summary>
        /// Плоский вид
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Плоский вид диаграммы")]
        [DisplayName("Плоский вид")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Flat
        {
            get { return funnel3DAppearance.Flat; }
            set { funnel3DAppearance.Flat = value; }
        }

        #endregion

        public FunnelChart3DBrowseClass(Funnel3DAppearance funnel3DAppearance)
        {
            this.funnel3DAppearance = funnel3DAppearance;
            chart3DLabelsAppearance = new HierarchicalChart3DLabelsApperanceBrowseClass(funnel3DAppearance.Labels);
        }

        public override string ToString()
        {
            return Axis + "; " + SortStyleTypeConverter.ToString(Sort);
        }
    }
}