using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Stack3DBarChartBrowseClass
    {
        #region Поля

        private BarChart3DBrowseClass barChart3DBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// BarChart3D
        /// </summary>
        [Category("Область диаграммы")]
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