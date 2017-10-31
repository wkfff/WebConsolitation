using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ScatterLineChartBrowseClass
    {
        #region ֿמכÿ

        private LineChartBrowseClass lineChartBrowse;
        private ScatterChartBrowseClass scatterChartBrowse;

        #endregion

        #region ׁגמיסעגא

        /// <summary>
        /// ִטאדנאללא LineChart
        /// </summary>
        [Category("־בכאסעü הטאדנאללû")]
        [Description("ִטאדנאללא LineChart")]
        [DisplayName("LineChart")]
        [Browsable(true)]
        public LineChartBrowseClass LineChartBrowse
        {
            get { return lineChartBrowse; }
            set { lineChartBrowse = value; }
        }

        /// <summary>
        /// ִטאדנאללא ScatterChart
        /// </summary>
        [Category("־בכאסעü הטאדנאללû")]
        [Description("ִטאדנאללא ScatterChart")]
        [DisplayName("ScatterChart")]
        [Browsable(true)]
        public ScatterChartBrowseClass ScatterChartBrowse
        {
            get { return scatterChartBrowse; }
            set { scatterChartBrowse = value; }
        }

        #endregion

        public ScatterLineChartBrowseClass(ScatterChartAppearance scatterChartAppearance, LineChartAppearance lineChartAppearance)
        {
            lineChartBrowse = new LineChartBrowseClass(lineChartAppearance);
            scatterChartBrowse = new ScatterChartBrowseClass(scatterChartAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}