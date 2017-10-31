using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StackAreaChartBrowseClass
    {
        #region ����

        private AreaChartBrowseClass areaChartBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// AreaChart
        /// </summary>
        [Category("������� ���������")]
        [Description("AreaChart")]
        [DisplayName("AreaChart")]
        [Browsable(true)]
        public AreaChartBrowseClass AreaChartBrowse
        {
            get { return areaChartBrowse; }
            set { areaChartBrowse = value; }
        }

        /// <summary>
        /// StackChart
        /// </summary>
        [Category("������� ���������")]
        [Description("StackChart")]
        [DisplayName("StackChart")]
        [Browsable(true)]
        public StackChartBrowseClass StackChartBrowse
        {
            get { return stackChartBrowse; }
            set { stackChartBrowse = value; }
        }

        #endregion

        public StackAreaChartBrowseClass(AreaChartAppearance areaChartAppearance, StackAppearance stackAppearance)
        {
            areaChartBrowse = new AreaChartBrowseClass(areaChartAppearance);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}