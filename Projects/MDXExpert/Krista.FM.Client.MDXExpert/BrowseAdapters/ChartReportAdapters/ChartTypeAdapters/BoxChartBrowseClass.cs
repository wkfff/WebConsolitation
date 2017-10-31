using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BoxChartBrowseClass
    {
        #region Поля

        private BoxChartAppearance boxChartAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Доля ширины бокса
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Доля ширины бокса диаграммы")]
        [DisplayName("Доля ширины бокса")]
        [DefaultValue(typeof(double), "1")]
        [Browsable(true)]
        public double BoxWidthFactor
        {
            get { return boxChartAppearance.BoxWidthFactor; }
            set { boxChartAppearance.BoxWidthFactor = value; }
        }

        #endregion

        public BoxChartBrowseClass(BoxChartAppearance boxChartAppearance)
        {
            this.boxChartAppearance = boxChartAppearance;
        }

        public override string ToString()
        {
            return BoxWidthFactor.ToString();
        }
    }
}