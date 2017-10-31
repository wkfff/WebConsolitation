using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineAreaChart3DBrowseClass
    {
        #region Поля

        private SplineAreaChart3DAppearance splineAreaChart3DAppearance;

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
            get { return splineAreaChart3DAppearance.Flatness; }
            set { splineAreaChart3DAppearance.Flatness = value; }
        }

        #endregion

        public SplineAreaChart3DBrowseClass(SplineAreaChart3DAppearance splineAreaChart3DAppearance)
        {
            this.splineAreaChart3DAppearance = splineAreaChart3DAppearance;
        }

        public override string ToString()
        {
            return Flatness.ToString();
        }
    }
}