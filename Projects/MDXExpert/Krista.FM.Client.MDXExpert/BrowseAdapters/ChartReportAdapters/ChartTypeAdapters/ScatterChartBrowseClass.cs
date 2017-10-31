using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ScatterChartBrowseClass
    {
        #region Поля

        private ScatterChartAppearance scatterChartAppearance;
        private SplineApperanceBrowseClass splineApperanceBrowse;

        #endregion

        #region Свойства

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
            get { return scatterChartAppearance.ChartText; }
        }

        /// <summary>
        /// Символ метки
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Символ метки")]
        [DisplayName("Символ метки")]
        [Browsable(true)]
        public Char Character
        {
            get { return scatterChartAppearance.Character; }
            set { scatterChartAppearance.Character = value; }
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
            get { return scatterChartAppearance.CharacterFont; }
            set { scatterChartAppearance.CharacterFont = value; }
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
            get { return scatterChartAppearance.ColumnX; }
            set { scatterChartAppearance.ColumnX = value; }
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
            get { return scatterChartAppearance.ColumnY; }
            set { scatterChartAppearance.ColumnY = value; }
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
            get { return scatterChartAppearance.ConnectWithLines; }
            set { scatterChartAppearance.ConnectWithLines = value; }
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
            get { return scatterChartAppearance.Icon; }
            set { scatterChartAppearance.Icon = value; }
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
            get { return scatterChartAppearance.IconSize; }
            set { scatterChartAppearance.IconSize = value; }
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
            get { return scatterChartAppearance.GroupByColumn; }
            set { scatterChartAppearance.GroupByColumn = value; }
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
            get { return scatterChartAppearance.UseGroupByColumn; }
            set { scatterChartAppearance.UseGroupByColumn = value; }
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
            get { return scatterChartAppearance.NullHandling; }
            set { scatterChartAppearance.NullHandling = value; }
        }

        #endregion

        public ScatterChartBrowseClass(ScatterChartAppearance scatterChartAppearance)
        {
            this.scatterChartAppearance = scatterChartAppearance;
            splineApperanceBrowse = new SplineApperanceBrowseClass(scatterChartAppearance.LineAppearance);
        }

        public override string ToString()
        {
            return SymbolIconTypeConverter.ToString(Icon) + "; " + LineDrawStyleTypeConverter.ToString(splineApperanceBrowse.DrawStyle);
        }
    }
}