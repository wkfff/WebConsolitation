using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StackChartBrowseClass
    {
        #region Поля

        private StackAppearance stackAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Стиль представления
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль представления")]
        [DisplayName("Стиль")]
        [TypeConverter(typeof(StackStyleTypeConverter))]
        [Browsable(true)]
        public StackStyle StackStyle
        {
            get { return stackAppearance.StackStyle; }
            set { stackAppearance.StackStyle = value; }
        }

        #endregion

        public StackChartBrowseClass(StackAppearance stackAppearance)
        {
            this.stackAppearance = stackAppearance;
        }

        public override string ToString()
        {
            return StackStyleTypeConverter.ToString(StackStyle);
        }
    }
}