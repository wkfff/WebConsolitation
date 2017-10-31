using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HeatMapChartBrowseClass : FilterablePropertyBase
    {
        #region ����

        private HeatMapChartAppearance heatmapChartAppearance;
        private UltraChart chart;

        #endregion

        #region ��������

        /// <summary>
        /// ��� ���������
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
        }

        /// <summary>
        /// �������� �������������
        /// </summary>
        [Category("������� ���������")]
        [Description("�������� ������������� ���������")]
        [DisplayName("�������� �������������")]
        [DefaultValue(HeatMapRenderQuality.LowQuality)]
        [Browsable(true)]
        public HeatMapRenderQuality RenderQuality
        {
            get { return heatmapChartAppearance.RenderQuality; }
            set { heatmapChartAppearance.RenderQuality = value; }
        }

        /// <summary>
        /// ����������� ������ ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������ ��������")]
        [DisplayName("������ ��������")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [DefaultValue(NullHandling.Zero)]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return heatmapChartAppearance.NullHandling; }
            set { heatmapChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ������")]
        [DisplayName("������� ������")]
        [DynamicPropertyFilter("ChartType", "HeatMapChart")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return heatmapChartAppearance.ChartText; }
        }

        #endregion

        public HeatMapChartBrowseClass(HeatMapChartAppearance heatmapChartAppearance, UltraChart chart)
        {
            this.heatmapChartAppearance = heatmapChartAppearance;
            this.chart = chart;
        }

        public override string ToString()
        {
            return RenderQuality.ToString();
        }
    }
}