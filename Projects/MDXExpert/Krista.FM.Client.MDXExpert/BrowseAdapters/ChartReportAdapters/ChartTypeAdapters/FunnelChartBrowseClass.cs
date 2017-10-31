using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FunnelChartBrowseClass
    {
        #region ����

        private FunnelChartAppearance funnelChartAppearance;
        private HierarchicalChartLabelsApperanceBrowseClass chartLabelsAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ������������ ����
        /// </summary>
        [Category("������� ���������")]
        [Description("������������ ���� ���������")]
        [DisplayName("������������ ����")]
        [Browsable(true)]
        public HierarchicalChartAxis Axis
        {
            get { return funnelChartAppearance.Axis; }
            set { funnelChartAppearance.Axis = value; }
        }

        /// <summary>
        /// �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ���������")]
        [DisplayName("�����")]
        [Browsable(true)]
        public HierarchicalChartLabelsApperanceBrowseClass ChartLabelsAppearance
        {
            get { return chartLabelsAppearance; }
            set { chartLabelsAppearance = value; }
        }

        /// <summary>
        /// ������� ���� "������"
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ���� \"������\"")]
        [DisplayName("������� ���� \"������\"")]
        [Browsable(true)]
        public string OthersCategoryText
        {
            get { return funnelChartAppearance.OthersCategoryText; }
            set { funnelChartAppearance.OthersCategoryText = value; }
        }

        /// <summary>
        /// ����������� ������� ����
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������� ����")]
        [DisplayName("����������� ������� ����")]
        [Browsable(true)]
        public double OthersCategoryPercent
        {
            get { return funnelChartAppearance.OthersCategoryPercent; }
            set { funnelChartAppearance.OthersCategoryPercent = value; }
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� �����")]
        [DisplayName("����������")]
        [TypeConverter(typeof(SortStyleTypeConverter))]
        [Browsable(true)]
        public SortStyle Sort
        {
            get { return funnelChartAppearance.Sort; }
            set { funnelChartAppearance.Sort = value; }
        }

        /// <summary>
        /// ������� ������� ����������� ����
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ������� ����������� ����")]
        [DisplayName("������� ������� ����������� ����")]
        [Browsable(true)]
        public double Spacing
        {
            get { return funnelChartAppearance.Spacing; }
            set { funnelChartAppearance.Spacing = value; }
        }

        /// <summary>
        /// ����������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������")]
        [DisplayName("����������� ������")]
        [Browsable(true)]
        public double RadiusMin
        {
            get { return funnelChartAppearance.RadiusMin; }
            set { funnelChartAppearance.RadiusMin = value; }
        }

        /// <summary>
        /// ������������ ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������������ ������ ���������")]
        [DisplayName("������������ ������")]
        [Browsable(true)]
        public double RadiusMax
        {
            get { return funnelChartAppearance.RadiusMax; }
            set { funnelChartAppearance.RadiusMax = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ������")]
        [DisplayName("������� ������")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return funnelChartAppearance.ChartText; }
        }

        #endregion

        public FunnelChartBrowseClass(FunnelChartAppearance funnelChartAppearance)
        {
            this.funnelChartAppearance = funnelChartAppearance;
            chartLabelsAppearance = new HierarchicalChartLabelsApperanceBrowseClass(funnelChartAppearance.Labels);
        }

        public override string ToString()
        {
            return Axis + "; " + SortStyleTypeConverter.ToString(Sort);
        }
    }
}