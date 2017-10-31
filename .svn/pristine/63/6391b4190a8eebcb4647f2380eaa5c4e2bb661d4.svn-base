using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LeaderLabelStyleBrowseClass
    {
        #region Поля

        private LineStyle lineStyle;
        private HierarchicalChartLabelsAppearance chartLabelApperance;

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
            get { return lineStyle.DrawStyle; }
            set { lineStyle.DrawStyle = value; }
        }

        /// <summary>
        /// Начало линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Начало линии")]
        [DisplayName("Начало линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
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
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return lineStyle.EndStyle; }
            set { lineStyle.EndStyle = value; }
        }

        /// <summary>
        /// Отображение промежуточных меток
        ///  </summary>
        [Category("Область диаграммы")]
        [Description("Отображение промежуточных меток")]
        [DisplayName("Отображение промежуточных меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return lineStyle.MidPointAnchors; }
            set { lineStyle.MidPointAnchors = value; }
        }

        /// <summary>
        /// Цвет линии
        ///  </summary>
        [Category("Область диаграммы")]
        [Description("Цвет линии")]
        [DisplayName("Цвет линии")]
        [Browsable(true)]
        public Color LeaderLineColor
        {
            get { return chartLabelApperance.LeaderLineColor; }
            set { chartLabelApperance.LeaderLineColor = value; }
        }

        /// <summary>
        /// Толщина линии
        ///  </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int LeaderLineThickness
        {
            get { return chartLabelApperance.LeaderLineThickness; }
            set { chartLabelApperance.LeaderLineThickness = value; }
        }

        #endregion

        public LeaderLabelStyleBrowseClass(HierarchicalChartLabelsAppearance chartLabelApperance)
        {
            lineStyle = chartLabelApperance.LeaderLineStyle;
            this.chartLabelApperance = chartLabelApperance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + LineCapStyleTypeConverter.ToString(StartStyle) + "; " + LineCapStyleTypeConverter.ToString(EndStyle);
        }
    }
}