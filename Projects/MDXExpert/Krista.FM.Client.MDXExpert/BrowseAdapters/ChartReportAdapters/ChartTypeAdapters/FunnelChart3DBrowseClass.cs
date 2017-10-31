using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FunnelChart3DBrowseClass
    {
        #region ����

        private Funnel3DAppearance funnel3DAppearance;
        private HierarchicalChart3DLabelsApperanceBrowseClass chart3DLabelsAppearance;

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
            get { return funnel3DAppearance.Axis; }
            set { funnel3DAppearance.Axis = value; }
        }

        /// <summary>
        /// �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ���������")]
        [DisplayName("�����")]
        [Browsable(true)]
        public HierarchicalChart3DLabelsApperanceBrowseClass Chart3DLabelsAppearance
        {
            get { return chart3DLabelsAppearance; }
            set { chart3DLabelsAppearance = value; }
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
            get { return funnel3DAppearance.OthersCategoryText; }
            set { funnel3DAppearance.OthersCategoryText = value; }
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
            get { return funnel3DAppearance.OthersCategoryPercent; }
            set { funnel3DAppearance.OthersCategoryPercent = value; }
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
            get { return funnel3DAppearance.Sort; }
            set { funnel3DAppearance.Sort = value; }
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
            get { return funnel3DAppearance.Spacing; }
            set { funnel3DAppearance.Spacing = value; }
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
            get { return funnel3DAppearance.RadiusMin; }
            set { funnel3DAppearance.RadiusMin = value; }
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
            get { return funnel3DAppearance.RadiusMax; }
            set { funnel3DAppearance.RadiusMax = value; }
        }

        /// <summary>
        /// ������� ���
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ��� ���������")]
        [DisplayName("������� ���")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Flat
        {
            get { return funnel3DAppearance.Flat; }
            set { funnel3DAppearance.Flat = value; }
        }

        #endregion

        public FunnelChart3DBrowseClass(Funnel3DAppearance funnel3DAppearance)
        {
            this.funnel3DAppearance = funnel3DAppearance;
            chart3DLabelsAppearance = new HierarchicalChart3DLabelsApperanceBrowseClass(funnel3DAppearance.Labels);
        }

        public override string ToString()
        {
            return Axis + "; " + SortStyleTypeConverter.ToString(Sort);
        }
    }
}