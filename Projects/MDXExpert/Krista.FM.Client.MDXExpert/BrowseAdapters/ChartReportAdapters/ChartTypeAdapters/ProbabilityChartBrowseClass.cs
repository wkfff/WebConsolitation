using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Диаграмма ProbabilityChart
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ProbabilityChartBrowseClass
    {
        #region Поля

        private ProbabilityChartAppearance probabilityChartAppearance;
        private SplineApperanceBrowseClass splineApperanceBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Символ метки
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Символ метки")]
        [DisplayName("Символ метки")]
        [Browsable(true)]
        public Char Character
        {
            get { return probabilityChartAppearance.Character; }
            set { probabilityChartAppearance.Character = value; }
        }

        /// <summary>
        /// Шрифт символа
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Шрифт символа")]
        [DisplayName("Шрифт символа")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font CharacterFont
        {
            get { return probabilityChartAppearance.CharacterFont; }
            set { probabilityChartAppearance.CharacterFont = value; }
        }

        /// <summary>
        /// Индекс оси X
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс оси X")]
        [DisplayName("Индекс оси X")]
        [Browsable(true)]
        public int ColumnX
        {
            get { return probabilityChartAppearance.ColumnX; }
            set { probabilityChartAppearance.ColumnX = value; }
        }

        /// <summary>
        /// Индекс оси Y
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс оси Y")]
        [DisplayName("Индекс оси Y")]
        [Browsable(true)]
        public int ColumnY
        {
            get { return probabilityChartAppearance.ColumnY; }
            set { probabilityChartAppearance.ColumnY = value; }
        }

        /// <summary>
        /// Соединение меток линией
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Соединение меток линией")]
        [DisplayName("Соединение меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ConnectWithLines
        {
            get { return probabilityChartAppearance.ConnectWithLines; }
            set { probabilityChartAppearance.ConnectWithLines = value; }
        }

        /// <summary>
        /// Вид значка
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Вид значка")]
        [DisplayName("Значок")]
        [TypeConverter(typeof(SymbolIconTypeConverter))]
        [Browsable(true)]
        public SymbolIcon Icon
        {
            get { return probabilityChartAppearance.Icon; }
            set { probabilityChartAppearance.Icon = value; }
        }

        /// <summary>
        /// Размер значка
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Размер значка")]
        [DisplayName("Размер значка")]
        [TypeConverter(typeof(SymbolIconSizeTypeConverter))]
        [Browsable(true)]
        public SymbolIconSize IconSize
        {
            get { return probabilityChartAppearance.IconSize; }
            set { probabilityChartAppearance.IconSize = value; }
        }

        /// <summary>
        /// Индекс группировки по колонкам
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс группировки по колонкам")]
        [DisplayName("Индекс группировки по колонкам")]
        [Browsable(true)]
        public int GroupByColumn
        {
            get { return probabilityChartAppearance.GroupByColumn; }
            set { probabilityChartAppearance.GroupByColumn = value; }
        }

        /// <summary>
        /// Группировка по колонкам
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Группировка по колонкам")]
        [DisplayName("Группировка по колонкам")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool UseGroupByColumn
        {
            get { return probabilityChartAppearance.UseGroupByColumn; }
            set { probabilityChartAppearance.UseGroupByColumn = value; }
        }

        /// <summary>
        /// Стиль линии в диаграмме
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль линии в диаграмме")]
        [DisplayName("Линия")]
        [Browsable(true)]
        public SplineApperanceBrowseClass PolarLineBrowse
        {
            get { return splineApperanceBrowse; }
            set { splineApperanceBrowse = value; }
        }

        /// <summary>
        /// Отображение пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение пустых значений")]
        [DisplayName("Пустые значения")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return probabilityChartAppearance.NullHandling; }
            set { probabilityChartAppearance.NullHandling = value; }
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
            get { return probabilityChartAppearance.ChartText; }
        }

        #endregion

        public ProbabilityChartBrowseClass(ProbabilityChartAppearance probabilityChartAppearance)
        {
            this.probabilityChartAppearance = probabilityChartAppearance;
            splineApperanceBrowse = new SplineApperanceBrowseClass(probabilityChartAppearance.LineAppearance);
        }

        public override string ToString()
        {
            return SymbolIconTypeConverter.ToString(Icon) + "; " + LineDrawStyleTypeConverter.ToString(splineApperanceBrowse.DrawStyle);
        }
    }
}