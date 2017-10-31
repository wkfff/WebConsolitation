using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Stack3DBarChartBrowseClass
    {
        #region ����

        private BarChart3DBrowseClass barChart3DBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// BarChart3D
        /// </summary>
        [Category("������� ���������")]
        [Description("BarChart3D")]
        [DisplayName("BarChart3D")]
        [Browsable(true)]
        public BarChart3DBrowseClass BarChart3DBrowse
        {
            get { return barChart3DBrowse; }
            set { barChart3DBrowse = value; }
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

        public Stack3DBarChartBrowseClass(BarChart3DAppearance barChart3DAppearance, StackAppearance stackAppearance)
        {
            barChart3DBrowse = new BarChart3DBrowseClass(barChart3DAppearance);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}