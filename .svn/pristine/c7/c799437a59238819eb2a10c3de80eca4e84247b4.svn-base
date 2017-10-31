using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineAreaChartBrowseClass
    {
        #region Поля

        private SplineAreaChartAppearance splineAreaChartAppearance;

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
        public LineDrawStyle LineDrawStyle
        {
            get { return splineAreaChartAppearance.LineDrawStyle; }
            set { splineAreaChartAppearance.LineDrawStyle = value; }
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
        public LineCapStyle LineStartCapStyle
        {
            get { return splineAreaChartAppearance.LineStartCapStyle; }
            set { splineAreaChartAppearance.LineStartCapStyle = value; }
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
        public LineCapStyle LineEndStyle
        {
            get { return splineAreaChartAppearance.LineEndCapStyle; }
            set { splineAreaChartAppearance.LineEndCapStyle = value; }
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
            get { return splineAreaChartAppearance.MidPointAnchors; }
            set { splineAreaChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// Толщина линии диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии диаграммы")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int LineThickness
        {
            get { return splineAreaChartAppearance.LineThickness; }
            set { splineAreaChartAppearance.LineThickness = value; }
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
            get { return splineAreaChartAppearance.SplineTension; }
            set { splineAreaChartAppearance.SplineTension = value; }
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
            get { return splineAreaChartAppearance.NullHandling; }
            set { splineAreaChartAppearance.NullHandling = value; }
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
            get { return splineAreaChartAppearance.ChartText; }
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
            get { return splineAreaChartAppearance.EmptyStyles; }
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
            get { return splineAreaChartAppearance.LineAppearances; }
        }

        #endregion

        public SplineAreaChartBrowseClass(SplineAreaChartAppearance splineAreaChartAppearance)
        {
            this.splineAreaChartAppearance = splineAreaChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(LineDrawStyle) + ";" + LineThickness + ";" + BooleanTypeConverter.ToString(MidPointAnchors);
        }
    }
}