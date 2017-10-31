using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Stack3DColumnChartBrowseClass
    {
        #region ����

        private ColumnChart3DBrowseClass columnChart3DBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ColumnChart3D
        /// </summary>
        [Category("������� ���������")]
        [Description("ColumnChart3D")]
        [DisplayName("ColumnChart3D")]
        [Browsable(true)]
        public ColumnChart3DBrowseClass ColumnChart3DBrowse
        {
            get { return columnChart3DBrowse; }
            set { columnChart3DBrowse = value; }
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

        public Stack3DColumnChartBrowseClass(ColumnChart3DAppearance columnChart3DAppearance, StackAppearance stackAppearance)
        {
            columnChart3DBrowse = new ColumnChart3DBrowseClass(columnChart3DAppearance);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}