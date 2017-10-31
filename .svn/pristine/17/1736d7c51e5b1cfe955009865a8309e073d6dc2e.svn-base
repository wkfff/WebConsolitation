using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineChart3DBrowseClass
    {
        #region Поля

        private SplineChart3DAppearance splineChart3DAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Гладкость
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Гладкость диаграммы")]
        [DisplayName("Гладкость")]
        [Browsable(true)]
        public float Flatness
        {
            get { return splineChart3DAppearance.Flatness; }
            set { splineChart3DAppearance.Flatness = value; }
        }

        #endregion

        public SplineChart3DBrowseClass(SplineChart3DAppearance splineChart3DAppearance)
        {
            this.splineChart3DAppearance = splineChart3DAppearance;
        }

        public override string ToString()
        {
            return Flatness.ToString();
        }
    }
}