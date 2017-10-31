using System.ComponentModel;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LineStyleBrowseClass
    {
        #region Поля

        private LineStyle lineStyle;

        #endregion

        #region Свойства

        /// <summary>
        /// Стиль линий
        /// </summary>
        [Description("Стиль линий")]
        [DisplayName("Стиль")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return lineStyle.DrawStyle; }
            set { lineStyle.DrawStyle = value; }
        }

        /// <summary>
        /// Начало линии
        /// </summary>
        [Description("Начало линии")]
        [DisplayName("Начало линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.NoAnchor)]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return lineStyle.StartStyle; }
            set { lineStyle.StartStyle = value; }
        }

        /// <summary>
        /// Конец линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Конец линии")]
        [DisplayName("Конец линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.NoAnchor)]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return lineStyle.EndStyle; }
            set { lineStyle.EndStyle = value; }
        }

        /// <summary>
        /// Отображение промежуточных точек
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение промежуточных точек")]
        [DisplayName("Отображение промежуточных точек")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return lineStyle.MidPointAnchors; }
            set { lineStyle.MidPointAnchors = value; }
        }


        #endregion

        public LineStyleBrowseClass(LineStyle lineStyle)
        {
            this.lineStyle = lineStyle;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + LineCapStyleTypeConverter.ToString(StartStyle) + "; " + LineCapStyleTypeConverter.ToString(EndStyle);
        }
    }
}