using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LineChartBrowseClass
    {
        #region Поля

        private LineChartAppearance lineChartAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Стиль линий
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль линий")]
        [DisplayName("Стиль")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return lineChartAppearance.DrawStyle; }
            set { lineChartAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// Начало линии сегмента диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Начало линии сегмента диаграммы")]
        [DisplayName("Начало линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return lineChartAppearance.StartStyle; }
            set { lineChartAppearance.StartStyle = value; }
        }

        /// <summary>
        /// Конец линии сегмента диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Конец линии сегмента диаграммы")]
        [DisplayName("Конец линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return lineChartAppearance.EndStyle; }
            set { lineChartAppearance.EndStyle = value; }
        }

        /// <summary>
        /// Отображение меток между соседними сегментами диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение меток между соседними сегментами диаграммы")]
        [DisplayName("Отображение промежуточных меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return lineChartAppearance.MidPointAnchors; }
            set { lineChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// Толщина линии диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии диаграммы")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(3)]
        [Browsable(true)]
        public int Thickness
        {
            get { return lineChartAppearance.Thickness; }
            set { lineChartAppearance.Thickness = value; }
        }

        /// <summary>
        /// Подсветка линий
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подсветка линий")]
        [DisplayName("Подсветка линий")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool HighLightLines
        {
            get { return lineChartAppearance.HighLightLines; }
            set { lineChartAppearance.HighLightLines = value; }
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
            get { return lineChartAppearance.NullHandling; }
            set { lineChartAppearance.NullHandling = value; }
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
            get { return lineChartAppearance.ChartText; }
        }

        /// <summary>
        /// Стили отображения пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стили отображения пустых значений")]
        [DisplayName("Стили отображения пустых значений")]
        [Browsable(true)]
        public EmptyAppearanceCollection EmptyStyles
        {
            get { return lineChartAppearance.EmptyStyles; }
        }

        /// <summary>
        /// Стили линий
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стили линий")]
        [DisplayName("Стили линий")]
        [Browsable(true)]
        public LineAppearanceCollection LineAppearances
        {
            get { return lineChartAppearance.LineAppearances; }
        }

        #endregion

        public LineChartBrowseClass(LineChartAppearance lineChartAppearance)
        {
            this.lineChartAppearance = lineChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + Thickness + "; " + BooleanTypeConverter.ToString(MidPointAnchors);
        }
    }
}