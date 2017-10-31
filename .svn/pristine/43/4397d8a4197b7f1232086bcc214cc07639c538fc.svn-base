using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BarChartBrowseClass
    {
        #region Поля

        private BarChartAppearance barChartAppearance;

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
            get { return barChartAppearance.BarSpacing; }
            set { barChartAppearance.BarSpacing = value; }
        }

        /// <summary>
        /// Рассторяние между рядами
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Рассторяние между рядами")]
        [DisplayName("Рассторяние между рядами")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int SeriesSpacing
        {
            get { return barChartAppearance.SeriesSpacing; }
            set { barChartAppearance.SeriesSpacing = value; }
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
            get { return barChartAppearance.NullHandling; }
            set { barChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return barChartAppearance.ChartText; }
        }

        #endregion

        public BarChartBrowseClass(BarChartAppearance barChartAppearance)
        {
            this.barChartAppearance = barChartAppearance;
        }

        public override string ToString()
        {
            return BarSpacing + "; " + SeriesSpacing;
        }
    }
}