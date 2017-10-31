using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StackBarChartBrowseClass
    {
        #region ����

        private BarChartBrowseClass barChartBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// BarChart
        /// </summary>
        [Category("������� ���������")]
        [Description("BarChart")]
        [DisplayName("BarChart")]
        [Browsable(true)]
        public BarChartBrowseClass BarChartBrowse
        {
            get { return barChartBrowse; }
            set { barChartBrowse = value; }
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

        public StackBarChartBrowseClass(BarChartAppearance barChartAppearnce, StackAppearance stackAppearance)
        {
            barChartBrowse = new BarChartBrowseClass(barChartAppearnce);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}