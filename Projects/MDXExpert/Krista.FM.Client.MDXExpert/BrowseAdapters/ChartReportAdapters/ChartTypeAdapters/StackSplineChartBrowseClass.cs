using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StackSplineChartBrowseClass
    {
        #region Поля

        private SplineChartBrowseClass splineChartBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// SplineChart
        /// </summary>
        [Category("Область диаграммы")]
        [Description("SplineChart")]
        [DisplayName("SplineChart")]
        [Browsable(true)]
        public SplineChartBrowseClass SplineChartBrowse
        {
            get { return splineChartBrowse; }
            set { splineChartBrowse = value; }
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

        public StackSplineChartBrowseClass(SplineChartAppearance splineChartAppearance, StackAppearance stackAppearance)
        {
            splineChartBrowse = new SplineChartBrowseClass(splineChartAppearance);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}