using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PyramidChart3DBrowseClass
    {
        #region ����

        private Pyramid3DAppearance pyramid3DAppearance;
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
            get { return pyramid3DAppearance.Axis; }
            set { pyramid3DAppearance.Axis = value; }
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
            get { return pyramid3DAppearance.OthersCategoryText; }
            set { pyramid3DAppearance.OthersCategoryText = value; }
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
            get { return pyramid3DAppearance.OthersCategoryPercent; }
            set { pyramid3DAppearance.OthersCategoryPercent = value; }
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
            get { return pyramid3DAppearance.Sort; }
            set { pyramid3DAppearance.Sort = value; }
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
            get { return pyramid3DAppearance.Spacing; }
            set { pyramid3DAppearance.Spacing = value; }
        }

        #endregion

        public PyramidChart3DBrowseClass(Pyramid3DAppearance pyramid3DAppearance)
        {
            this.pyramid3DAppearance = pyramid3DAppearance;
            chart3DLabelsAppearance = new HierarchicalChart3DLabelsApperanceBrowseClass(pyramid3DAppearance.Labels);
        }

        public override string ToString()
        {
            return Axis + "; " + SortStyleTypeConverter.ToString(Sort);
        }
    }
}