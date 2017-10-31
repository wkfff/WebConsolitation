using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StackColumnChartBrowseClass
    {
        #region Поля

        private ColumnChartBrowseClass columnChartBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// ColumnChart
        /// </summary>
        [Category("Область диаграммы")]
        [Description("ColumnChart")]
        [DisplayName("ColumnChart")]
        [Browsable(true)]
        public ColumnChartBrowseClass ColumnChartBrowse
        {
            get { return columnChartBrowse; }
            set { columnChartBrowse = value; }
        }

        /// <summary>
        /// StackChart
        /// </summary>
        [Category("Область диаграммы")]
        [Description("StackChart")]
        [DisplayName("StackChart")]
        [Browsable(true)]
        public StackChartBrowseClass StackChartBrowse
        {
            get { return stackChartBrowse; }
            set { stackChartBrowse = value; }
        }

        #endregion

        public StackColumnChartBrowseClass(ColumnChartAppearance columnChartAppearance, StackAppearance stackAppearance)
        {
            columnChartBrowse = new ColumnChartBrowseClass(columnChartAppearance);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}