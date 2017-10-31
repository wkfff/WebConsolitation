using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StackSplineAreaChartBrowseClass
    {
        #region Поля

        private SplineAreaChartBrowseClass splineAreaChartBrowse;
        private StackChartBrowseClass stackChartBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// SplineAreaChart
        /// </summary>
        [Category("Область диаграммы")]
        [Description("SplineAreaChart")]
        [DisplayName("SplineAreaChart")]
        [Browsable(true)]
        public SplineAreaChartBrowseClass SplineAreaChartBrowse
        {
            get { return splineAreaChartBrowse; }
            set { splineAreaChartBrowse = value; }
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

        public StackSplineAreaChartBrowseClass(SplineAreaChartAppearance splineAreaChartAppearance, StackAppearance stackAppearance)
        {
            splineAreaChartBrowse = new SplineAreaChartBrowseClass(splineAreaChartAppearance);
            stackChartBrowse = new StackChartBrowseClass(stackAppearance);
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}