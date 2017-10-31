using System.ComponentModel;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CompositeChartBrowseClass
    {
        #region Поля

        private CompositeChartAppearance compositeChartAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Области
        /// </summary>
        [Description("Области")]
        [DisplayName("Области")]
        [Editor(typeof(ChartAreaCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public ChartAreaCollection ChartAreas
        {
            get { return compositeChartAppearance.ChartAreas; }
        }

        /// <summary>
        /// Слои
        /// </summary>
        [Description("Слои")]
        [DisplayName("Слои")]
        [Editor(typeof(ChartLayerCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public ChartLayerCollection ChartLayers
        {
            get { return compositeChartAppearance.ChartLayers; }
        }

        /// <summary>
        /// Оси
        /// </summary>
        [Description("Оси")]
        [DisplayName("Оси")]
        [Browsable(false)]
        public AxisCollection ChartAxies
        {
            get { return compositeChartAppearance.ChartAreas[0].Axes; }
        }

//        /// <summary>
//        /// Легенды
//        /// </summary>
//        [Description("Легенды")]
//        [DisplayName("Легенды")]
//        [Browsable(true)]
//        public CompositeLegendCollection Legends
//        {
//            get { return compositeChartAppearance.Legends; }
//        }

        /// <summary>
        /// Серии
        /// </summary>
        [Description("Серии")]
        [DisplayName("Серии")]
        [Browsable(true)]
        [Editor(typeof(CustomSeriesCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public SeriesCollection Series
        {
            get { return compositeChartAppearance.Series; }
        }

        #endregion

        public CompositeChartBrowseClass(CompositeChartAppearance compositeChartAppearance)
        {
            this.compositeChartAppearance = compositeChartAppearance;
        }

        public override string ToString()
        {
            return "";
        }
    }
}