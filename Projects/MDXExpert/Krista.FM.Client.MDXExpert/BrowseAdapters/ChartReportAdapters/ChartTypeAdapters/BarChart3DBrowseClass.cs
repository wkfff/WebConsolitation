using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BarChart3DBrowseClass
    {
        #region Поля

        private BarChart3DAppearance barChart3DAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Рассторяние между категориями
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Рассторяние между категориями")]
        [DisplayName("Рассторяние между категориями")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int BarSpacing
        {
            get { return barChart3DAppearance.BarSpacing; }
            set { barChart3DAppearance.BarSpacing = value; }
        }

        /// <summary>
        /// Отображение пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение пустых значений")]
        [DisplayName("Пустые значения")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [DefaultValue(NullHandling.Zero)]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return barChart3DAppearance.NullHandling; }
            set { barChart3DAppearance.NullHandling = value; }
        }

        #endregion

        public BarChart3DBrowseClass(BarChart3DAppearance barChart3DAppearance)
        {
            this.barChart3DAppearance = barChart3DAppearance;
        }

        public override string ToString()
        {
            return BarSpacing.ToString();
        }
    }
}