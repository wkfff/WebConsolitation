using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineChartBrowseClass
    {
        #region Поля

        private SplineChartAppearance splineChartAppearance;

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
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return splineChartAppearance.DrawStyle; }
            set { splineChartAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// Начало линии сегмента диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Начало линии сегмента диаграммы")]
        [DisplayName("Начало линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return splineChartAppearance.StartStyle; }
            set { splineChartAppearance.StartStyle = value; }
        }

        /// <summary>
        /// Конец линии сегмента диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Конец линии сегмента диаграммы")]
        [DisplayName("Конец линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return splineChartAppearance.EndStyle; }
            set { splineChartAppearance.EndStyle = value; }
        }

        /// <summary>
        /// Отображение меток между соседними сегментами диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение меток между соседними сегментами диаграммы")]
        [DisplayName("Отображение промежуточных меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return splineChartAppearance.MidPointAnchors; }
            set { splineChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// Толщина линии диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии диаграммы")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int Thickness
        {
            get { return splineChartAppearance.Thickness; }
            set { splineChartAppearance.Thickness = value; }
        }

        /// <summary>
        /// Плавность линии диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Плавность линии диаграммы")]
        [DisplayName("Плавность линии")]
        [Browsable(true)]
        public float SplineTension
        {
            get { return splineChartAppearance.SplineTension; }
            set { splineChartAppearance.SplineTension = value; }
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
            get { return splineChartAppearance.NullHandling; }
            set { splineChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// Подсветка линий
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подсветка линий")]
        [DisplayName("Подсветка линий")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool HighLightLines
        {
            get { return splineChartAppearance.HighLightLines; }
            set { splineChartAppearance.HighLightLines = value; }
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
            get { return splineChartAppearance.ChartText; }
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
            get { return splineChartAppearance.EmptyStyles; }
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
            get { return splineChartAppearance.LineAppearances; }
        }

        #endregion

        public SplineChartBrowseClass(SplineChartAppearance splineChartAppearance)
        {
            this.splineChartAppearance = splineChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + ";" + Thickness + ";" + BooleanTypeConverter.ToString(MidPointAnchors);
        }
    }
}