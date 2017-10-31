using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RadarChartBrowseClass
    {
        #region Поля

        private RadarChartAppearance radarChartAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Стиль линии
        /// </summary>
        [Category("Линия")]
        [Description("Стиль линии")]
        [DisplayName("Стиль линии")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle LineDrawStyle
        {
            get { return radarChartAppearance.LineDrawStyle; }
            set { radarChartAppearance.LineDrawStyle = value; }
        }

        /// <summary>
        /// Толщина линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int LineThickness
        {
            get { return radarChartAppearance.LineThickness; }
            set { radarChartAppearance.LineThickness = value; }
        }

        /// <summary>
        /// Плавность линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Плавность линии")]
        [DisplayName("Плавность линии")]
        [Browsable(true)]
        public float SplineTension
        {
            get { return radarChartAppearance.SplineTension; }
            set { radarChartAppearance.SplineTension = value; }
        }

        /// <summary>
        /// Конец линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Конец линии")]
        [DisplayName("Конец линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle LineEndCapStyle
        {
            get { return radarChartAppearance.LineEndCapStyle; }
            set { radarChartAppearance.LineEndCapStyle = value; }
        }

        /// <summary>
        /// Отображение меток
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение меток")]
        [DisplayName("Отображение меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return radarChartAppearance.MidPointAnchors; }
            set { radarChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// Заливка области
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Заливка области")]
        [DisplayName("Заливка области")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ColorFill
        {
            get { return radarChartAppearance.ColorFill; }
            set { radarChartAppearance.ColorFill = value; }
        }

        /// <summary>
        /// Расстояние вокруг диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние вокруг диаграммы")]
        [DisplayName("Расстояние вокруг диаграммы")]
        [Browsable(true)]
        public int SpacingAroundChart
        {
            get { return radarChartAppearance.SpacingAroundChart; }
            set { radarChartAppearance.SpacingAroundChart = value; }
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
            get { return radarChartAppearance.NullHandling; }
            set { radarChartAppearance.NullHandling = value; }
        }

        #endregion

        public RadarChartBrowseClass(RadarChartAppearance radarChartAppearance)
        {
            this.radarChartAppearance = radarChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(LineDrawStyle) + "; " + LineThickness + ": " + LineCapStyleTypeConverter.ToString(LineEndCapStyle);
        }
    }
}