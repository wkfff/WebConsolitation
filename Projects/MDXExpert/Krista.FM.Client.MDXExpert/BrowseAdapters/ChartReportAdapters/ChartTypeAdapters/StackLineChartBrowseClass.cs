using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StackLineChartBrowseClass
    {
        #region Поля

        private LineChartBrowseClass lineChartBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// LineChart
        /// </summary>
        [Category("Область диаграммы")]
        [Description("LineChart")]
        [DisplayName("LineChart")]
        [Browsable(true)]
        public LineChartBrowseClass LineChartBrowse
        {
            get { return lineChartBrowse; }
            set { lineChartBrowse = value; }
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

        public StackLineChartBrowseClass(LineChartAppearance lineChartAppearance, StackAppearance stackAppearance)
        {
            lineChartBrowse = new LineChartBrowseClass(lineChartAppearance);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}