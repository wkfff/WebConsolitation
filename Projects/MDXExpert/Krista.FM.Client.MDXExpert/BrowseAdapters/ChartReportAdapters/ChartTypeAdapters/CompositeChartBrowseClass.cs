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
        #region ����

        private CompositeChartAppearance compositeChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// �������
        /// </summary>
        [Description("�������")]
        [DisplayName("�������")]
        [Editor(typeof(ChartAreaCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public ChartAreaCollection ChartAreas
        {
            get { return compositeChartAppearance.ChartAreas; }
        }

        /// <summary>
        /// ����
        /// </summary>
        [Description("����")]
        [DisplayName("����")]
        [Editor(typeof(ChartLayerCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public ChartLayerCollection ChartLayers
        {
            get { return compositeChartAppearance.ChartLayers; }
        }

        /// <summary>
        /// ���
        /// </summary>
        [Description("���")]
        [DisplayName("���")]
        [Browsable(false)]
        public AxisCollection ChartAxies
        {
            get { return compositeChartAppearance.ChartAreas[0].Axes; }
        }

//        /// <summary>
//        /// �������
//        /// </summary>
//        [Description("�������")]
//        [DisplayName("�������")]
//        [Browsable(true)]
//        public CompositeLegendCollection Legends
//        {
//            get { return compositeChartAppearance.Legends; }
//        }

        /// <summary>
        /// �����
        /// </summary>
        [Description("�����")]
        [DisplayName("�����")]
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