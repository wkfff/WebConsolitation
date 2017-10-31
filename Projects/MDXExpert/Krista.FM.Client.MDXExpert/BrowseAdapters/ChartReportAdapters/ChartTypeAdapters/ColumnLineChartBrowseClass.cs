using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColumnLineChartBrowseClass
    {
        #region Поля

        private ColumnChartBrowseClass columnChartBrowse;
        private LineChartBrowseClass lineChartBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Колонки диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Колонки диаграммы")]
        [DisplayName("Колонки")]
        [Browsable(true)]
        public ColumnChartBrowseClass ColumnChartBrowseClass
        {
            get { return columnChartBrowse; }
            set { columnChartBrowse = value; }
        }

        /// <summary>
        /// Линии диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Линии диаграммы")]
        [DisplayName("Линии")]
        [Browsable(true)]
        public LineChartBrowseClass LineChartBrowseClass
        {
            get { return lineChartBrowse; }
            set { lineChartBrowse = value; }
        }

        #endregion

        public ColumnLineChartBrowseClass(ColumnChartAppearance columnChartAppearance, LineChartAppearance lineChartAppearance)
        {
            columnChartBrowse = new ColumnChartBrowseClass(columnChartAppearance);
            lineChartBrowse = new LineChartBrowseClass(lineChartAppearance);
        }

        public override string ToString()
        {
            return ColumnChartBrowseClass + "; " + LineChartBrowseClass;
        }
    }
}