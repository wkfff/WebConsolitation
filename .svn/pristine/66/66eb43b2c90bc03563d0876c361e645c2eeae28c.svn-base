using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Стиль линий в диаграмме PolarChart
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineApperanceBrowseClass
    {
        #region Поля

        private SplineAppearance splineAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Стиль линии
        /// </summary>
        [Category("Линия")]
        [Description("Стиль линии")]
        [DisplayName("Стиль")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return splineAppearance.DrawStyle; }
            set { splineAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// Толщина линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии")]
        [DisplayName("Толщина")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int Thickness
        {
            get { return splineAppearance.Thickness; }
            set { splineAppearance.Thickness = value; }
        }

        /// <summary>
        /// Плавность линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Плавность линии")]
        [DisplayName("Плавность")]
        [Browsable(true)]
        public float SplineTension
        {
            get { return splineAppearance.SplineTension; }
            set { splineAppearance.SplineTension = value; }
        }

        #endregion

        public SplineApperanceBrowseClass(SplineAppearance splineAppearance)
        {
            this.splineAppearance = splineAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + Thickness + "; " + SplineTension;
        }
    }
}