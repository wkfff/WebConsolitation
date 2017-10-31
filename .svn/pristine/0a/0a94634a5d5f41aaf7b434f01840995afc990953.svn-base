using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DoughnutChartBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private DoughnutChartAppearance doughnutChartAppearance;
        private UltraChart chart;
        private DoughnutLabelClass doughnutLabel;

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
        /// Доступность отсоединения слоев
        /// </summary>
        [Browsable(false)]
        public bool BreakSlicesEnable
        {
            get
            {
                return !(ChartType == ChartType.DoughnutChart3D && Concentric);
            }
        }

        /// <summary>
        /// Доступность настройки расстояния меджу концентрическими кольцами
        /// </summary>
        [Browsable(false)]
        public bool ConcentricSpacingEnable
        {
            get
            {
                return ChartType == ChartType.DoughnutChart && Concentric;
            }
        }

        /// <summary>
        /// Доступность настройки расстояния отделения
        /// </summary>
        [Browsable(false)]
        public bool BreakDistanceEnable
        {
            get
            {
                return BreakAllSlices || BreakAlternatingSlices || BreakOthersSlice;
            }
        }

        /// <summary>
        /// Полное расслоение диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Полное расслоение диаграммы")]
        [DisplayName("Полное расслоение")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("BreakSlicesEnable", "True")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool BreakAllSlices
        {
            get { return doughnutChartAppearance.BreakAllSlices; }
            set { doughnutChartAppearance.BreakAllSlices = value; }
        }

        /// <summary>
        /// Поочередное расслоение диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Поочередное расслоение диаграммы")]
        [DisplayName("Поочередное расслоение")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("BreakSlicesEnable", "True")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool BreakAlternatingSlices
        {
            get { return doughnutChartAppearance.BreakAlternatingSlices; }
            set { doughnutChartAppearance.BreakAlternatingSlices = value; }
        }

        /// <summary>
        /// Отделение слоя "Прочие"
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отделение слоя \"Прочие\"")]
        [DisplayName("Отделение слоя \"Прочие\"")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("BreakSlicesEnable", "True")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool BreakOthersSlice
        {
            get { return doughnutChartAppearance.BreakOthersSlice; }
            set { doughnutChartAppearance.BreakOthersSlice = value; }
        }

        /// <summary>
        /// Расстояние отделения слоев
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние отделения слоев.  Действует при полном расслоении, поочередном расслоении и отделении слоя \"Прочие\"")]
        [DisplayName("Расстояние отделения")]
        [DynamicPropertyFilter("BreakDistanceEnable", "True")]
        [DefaultValue(10)]
        [Browsable(true)]
        public int BreakDistancePercentage
        {
            get { return doughnutChartAppearance.BreakDistancePercentage; }
            set { doughnutChartAppearance.BreakDistancePercentage = value; }
        }

        /// <summary>
        /// Внутренний радиус
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Внутренний радиус")]
        [DisplayName("Внутренний радиус")]
        [DefaultValue(50)]
        [Browsable(true)]
        public int InnerRadius
        {
            get { return doughnutChartAppearance.InnerRadius; }
            set { doughnutChartAppearance.InnerRadius = value; }
        }

        /// <summary>
        /// Внешний радиус
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Внешний радиус")]
        [DisplayName("Внешний радиус")]
        [DefaultValue(90)]
        [Browsable(true)]
        public int RadiusFactor
        {
            get { return doughnutChartAppearance.RadiusFactor; }
            set { doughnutChartAppearance.RadiusFactor = value; }
        }

        /// <summary>
        /// Толщина слоя диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина слоя диаграммы")]
        [DisplayName("Толщина слоя")]
        [DynamicPropertyFilter("ChartType", "DoughnutChart3D")]
        [DefaultValue(20)]
        [Browsable(true)]
        public int PieThickness
        {
            get { return doughnutChartAppearance.PieThickness; }
            set { doughnutChartAppearance.PieThickness = value; }
        }

        /// <summary>
        /// Концентрическое представление диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Концентрическое представление диаграммы")]
        [DisplayName("Концентрическое представление")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Concentric
        {
            get { return doughnutChartAppearance.Concentric; }
            set { doughnutChartAppearance.Concentric = value; }
        }

        /// <summary>
        /// Отображение концентрического представления в легенде
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение концентрического представления в легенде")]
        [DisplayName("Отображение концентрического представления в легенде")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("Concentric", "True")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowConcentricLegend
        {
            get { return doughnutChartAppearance.ShowConcentricLegend; }
            set { doughnutChartAppearance.ShowConcentricLegend = value; }
        }

        /// <summary>
        /// Расстояние между концентрическими кольцами
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние между концентрическими кольцами")]
        [DisplayName("Расстояние между концентрическими кольцами")]
        [DynamicPropertyFilter("ConcentricSpacingEnable", "True")]
        [DefaultValue(typeof(double), "0.25")]
        [Browsable(true)]
        public double ConcentricSpacing
        {
            get { return doughnutChartAppearance.ConcentricSpacing; }
            set { doughnutChartAppearance.ConcentricSpacing = value; }
        }

        /// <summary>
        /// Индекс категорий диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Ииндекс категорий диаграммы")]
        [DisplayName("Индекс категорий")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColumnIndex
        {
            get { return doughnutChartAppearance.ColumnIndex; }
            set { doughnutChartAppearance.ColumnIndex = value; }
        }

        /// <summary>
        /// Подпись слоя "Прочие"
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подпись слоя \"Прочие\"")]
        [DisplayName("Подпись слоя \"Прочие\"")]
        [DefaultValue("Прочие")]
        [Browsable(true)]
        public string OthersCategoryText
        {
            get { return doughnutChartAppearance.OthersCategoryText; }
            set { doughnutChartAppearance.OthersCategoryText = value; }
        }

        /// <summary>
        /// Минимальный отображаемый процент слоя
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Минимальный отображаемый процент слоя")]
        [DisplayName("Минимальный процент слоя")]
        [DefaultValue(typeof(double), "3")]
        [Browsable(true)]
        public double OthersCategoryPercent
        {
            get { return doughnutChartAppearance.OthersCategoryPercent; }
            set { doughnutChartAppearance.OthersCategoryPercent = value; }
        }

        /// <summary>
        /// Начальный угол поворота диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Начальный угол поворота диаграммы")]
        [DisplayName("Начальный угол поворота")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int StartAngle
        {
            get { return doughnutChartAppearance.StartAngle; }
            set { doughnutChartAppearance.StartAngle = value; }
        }

        /// <summary>
        /// Выноски
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Выноски диаграммы")]
        [DisplayName("Выноски")]
        [Browsable(true)]
        public DoughnutLabelClass Labels
        {
            get { return doughnutLabel; }
            set { doughnutLabel = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [DynamicPropertyFilter("ChartType", "DoughnutChart")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return doughnutChartAppearance.ChartText; }
        }

        #endregion

        public DoughnutChartBrowseClass(DoughnutChartAppearance doughnutChartAppearance, UltraChart chart)
        {
            this.doughnutChartAppearance = doughnutChartAppearance;
            this.chart = chart;

            doughnutLabel = new DoughnutLabelClass(doughnutChartAppearance.Labels, chart);
        }

        public override string ToString()
        {
            return RadiusFactor + "; " + InnerRadius + "; " + StartAngle;
        }
    }
}