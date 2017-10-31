using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ScatterLineChartBrowseClass
    {
        #region ����

        private LineChartBrowseClass lineChartBrowse;
        private ScatterChartBrowseClass scatterChartBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ��������� LineChart
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� LineChart")]
        [DisplayName("LineChart")]
        [Browsable(true)]
        public LineChartBrowseClass LineChartBrowse
        {
            get { return lineChartBrowse; }
            set { lineChartBrowse = value; }
        }

        /// <summary>
        /// ��������� ScatterChart
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ScatterChart")]
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