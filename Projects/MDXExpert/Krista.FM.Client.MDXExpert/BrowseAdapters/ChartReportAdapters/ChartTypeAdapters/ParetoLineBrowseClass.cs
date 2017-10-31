using System.ComponentModel;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ParetoLineBrowseClass
    {
        #region Поля

        private LineStyle paretoLineStyle;

        #endregion

        #region Свойства

        /// <summary>
        /// Стиль линий
        /// </summary>
        [Category("Линии")]
        [Description("Стиль линий")]
        [DisplayName("Стиль")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return paretoLineStyle.DrawStyle; }
            set { paretoLineStyle.DrawStyle = value; }
        }

        /// <summary>
        /// Начало линии сегмента диаграммы
        /// </summary>
        [Category("Линии")]
        [Description("Начало линии сегмента диаграммы")]
        [DisplayName("Начало линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.RoundAnchor)]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return paretoLineStyle.StartStyle; }
            set { paretoLineStyle.StartStyle = value; }
        }

        /// <summary>
        /// Конец линии сегмента диаграммы
        /// </summary>
        [Category("Линии")]
        [Description("Конец линии сегмента диаграммы")]
        [DisplayName("Конец линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.RoundAnchor)]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return paretoLineStyle.EndStyle; }
            set { paretoLineStyle.EndStyle = value; }
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
            get { return paretoLineStyle.MidPointAnchors; }
            set { paretoLineStyle.MidPointAnchors = value; }
        }

        #endregion

        public ParetoLineBrowseClass(LineStyle paretoLineStyle)
        {
            this.paretoLineStyle = paretoLineStyle;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + LineCapStyleTypeConverter.ToString(StartStyle) + "; " + LineCapStyleTypeConverter.ToString(EndStyle);
        }
    }
}