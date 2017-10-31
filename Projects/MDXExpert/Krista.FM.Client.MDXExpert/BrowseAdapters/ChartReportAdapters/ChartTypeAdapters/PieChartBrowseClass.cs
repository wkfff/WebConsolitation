using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PieChartBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private PieChartAppearance pieChartAppearance;
        private UltraChart ultraChart;
        private PieLabelAppearanceBrowseClass pieLabel;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип диаграммы
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return ultraChart.ChartType; }
        }

        /// <summary>
        /// Доступность отсоединения слоев
        /// </summary>
        [Browsable(false)]
        public bool BreakSlicesEnable
        {
            get
            {
                return !(ChartType == ChartType.PieChart3D && Concentric);
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
                return ChartType == ChartType.PieChart && Concentric;
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
            get { return pieChartAppearance.BreakAllSlices; }
            set { pieChartAppearance.BreakAllSlices = value; }
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
            get { return pieChartAppearance.BreakAlternatingSlices; }
            set { pieChartAppearance.BreakAlternatingSlices = value; }
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
            get { return pieChartAppearance.BreakOthersSlice; }
            set { pieChartAppearance.BreakOthersSlice = value; }
        }

        /// <summary>
        /// Расстояние отделения слоев
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние отделения слоев. Действует при полном расслоении, поочередном расслоении и отделении слоя \"Прочие\"")]
        [DisplayName("Расстояние отделения")]
        [DynamicPropertyFilter("BreakDistanceEnable", "True")]
        [DefaultValue(10)]
        [Browsable(true)]
        public int BreakDistancePercentage
        {
            get { return pieChartAppearance.BreakDistancePercentage; }
            set { pieChartAppearance.BreakDistancePercentage = value; }
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
            get { return pieChartAppearance.RadiusFactor; }
            set { pieChartAppearance.RadiusFactor = value; }
        }

        /// <summary>
        /// Толщина слоя диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина слоя диаграммы")]
        [DisplayName("Толщина слоя")]
        [DynamicPropertyFilter("ChartType", "PieChart3D")]
        [DefaultValue(20)]
        [Browsable(true)]
        public int PieThickness
        {
            get { return pieChartAppearance.PieThickness; }
            set { pieChartAppearance.PieThickness = value; }
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
            get { return pieChartAppearance.Concentric; }
            set { pieChartAppearance.Concentric = value; }
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
            get { return pieChartAppearance.ShowConcentricLegend; }
            set { pieChartAppearance.ShowConcentricLegend = value; }
        }

        /// <summary>
        /// Расстояние меджу концентрическими кольцами
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние меджу концентрическими кольцами")]
        [DisplayName("Расстояние меджу концентрическими кольцами")]
        [DynamicPropertyFilter("ConcentricSpacingEnable", "True")]
        [DefaultValue(typeof(double), "0.25")]
        [Browsable(true)]
        public double ConcentricSpacing
        {
            get { return pieChartAppearance.ConcentricSpacing; }
            set { pieChartAppearance.ConcentricSpacing = value; }
        }

        /// <summary>
        /// Индекс категорий диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс категорий диаграммы")]
        [DisplayName("Индекс категорий")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColumnIndex
        {
            get { return pieChartAppearance.ColumnIndex; }
            set { pieChartAppearance.ColumnIndex = value; }
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
            get { return pieChartAppearance.OthersCategoryText; }
            set { pieChartAppearance.OthersCategoryText = value; }
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
            get { return pieChartAppearance.OthersCategoryPercent; }
            set { pieChartAppearance.OthersCategoryPercent = value; }
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
            get { return pieChartAppearance.StartAngle; }
            set { pieChartAppearance.StartAngle = value; }
        }

        /// <summary>
        /// Выноски
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Выноски диаграммы")]
        [DisplayName("Выноски")]
        [Browsable(true)]
        public PieLabelAppearanceBrowseClass Labels
        {
            get { return pieLabel; }
            set { pieLabel = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [DynamicPropertyFilter("ChartType", "PieChart")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return pieChartAppearance.ChartText; }
        }

        #endregion

        public PieChartBrowseClass(PieChartAppearance pieChartAppearance, UltraChart ultraChart)
        {
            this.pieChartAppearance = pieChartAppearance;
            this.ultraChart = ultraChart;
            pieLabel = new PieLabelAppearanceBrowseClass(pieChartAppearance.Labels, ultraChart);
        }

        public override string ToString()
        {
            return RadiusFactor + ";" + StartAngle;
        }
    }
}