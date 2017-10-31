using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PointChart3DBrowseClass
    {
        #region Поля

        private PointChart3DAppearance pointChart3DAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Вид значка
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Вид значка диаграммы")]
        [DisplayName("Значок")]
        [TypeConverter(typeof(SymbolIcon3DTypeConverter))]
        [Browsable(true)]
        public SymbolIcon3D Icon
        {
            get { return pointChart3DAppearance.Icon; }
            set { pointChart3DAppearance.Icon = value; }
        }

        /// <summary>
        /// Размер значка
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Размер значка диаграммы")]
        [DisplayName("Размер значка")]
        [TypeConverter(typeof(SymbolIconSizeTypeConverter))]
        [Browsable(true)]
        public SymbolIconSize IconSize
        {
            get { return pointChart3DAppearance.IconSize; }
            set { pointChart3DAppearance.IconSize = value; }
        }

        #endregion

        public PointChart3DBrowseClass(PointChart3DAppearance pointChart3DAppearance)
        {
            this.pointChart3DAppearance = pointChart3DAppearance;
        }

        public override string ToString()
        {
            return SymbolIcon3DTypeConverter.ToString(Icon) + "; " + SymbolIconSizeTypeConverter.ToString(IconSize);
        }
    }
}