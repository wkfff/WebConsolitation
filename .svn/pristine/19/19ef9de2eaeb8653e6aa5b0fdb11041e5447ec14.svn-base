using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColumnChartBrowseClass
    {
        #region Поля

        private ColumnChartAppearance columnChartAppearance;

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
        public int ColumnSpacing
        {
            get { return columnChartAppearance.ColumnSpacing; }
            set { columnChartAppearance.ColumnSpacing = value; }
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
            get { return columnChartAppearance.SeriesSpacing; }
            set { columnChartAppearance.SeriesSpacing = value; }
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
            get { return columnChartAppearance.NullHandling; }
            set { columnChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [DisplayName("Подписи данных")]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return columnChartAppearance.ChartText; }
        }

        #endregion

        public ColumnChartBrowseClass(ColumnChartAppearance columnChartAppearance)
        {
            this.columnChartAppearance = columnChartAppearance;
        }

        public override string ToString()
        {
            return ColumnSpacing + "; " + SeriesSpacing;
        }
    }
}