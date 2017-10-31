using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BubbleChartBrowseClass
    {
        #region ����

        private BubbleChartAppearance bubbleChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ����� ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ��������")]
        [DisplayName("����� ��������")]
        [TypeConverter(typeof(BubbleShapeTypeConverter))]
        [DefaultValue(BubbleShape.Circle)]
        [Browsable(true)]
        public BubbleShape BubbleShape
        {
            get { return bubbleChartAppearance.BubbleShape; }
            set { bubbleChartAppearance.BubbleShape = value; }
        }

        /// <summary>
        /// ���������� ������� �� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ������� �� �������")]
        [DisplayName("���������� �� �������")]
        [TypeConverter(typeof(ChartSortTypeConverter))]
        [DefaultValue(ChartSortType.None)]
        [Browsable(true)]
        public ChartSortType SortByRadius
        {
            get { return bubbleChartAppearance.SortByRadius; }
            set { bubbleChartAppearance.SortByRadius = value; }
        }

        /// <summary>
        /// ������ ����� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ����� ���������")]
        [DisplayName("������ �����")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColorCueColumn
        {
            get { return bubbleChartAppearance.ColorCueColumn; }
            set { bubbleChartAppearance.ColorCueColumn = value; }
        }

        /// <summary>
        /// ������ ��� X
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ��� X ���������")]
        [DisplayName("������ ��� X")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int ColumnX
        {
            get { return bubbleChartAppearance.ColumnX; }
            set { bubbleChartAppearance.ColumnX = value; }
        }

        /// <summary>
        /// ������ ��� Y
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ��� Y ���������")]
        [DisplayName("������ ��� Y")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int ColumnY
        {
            get { return bubbleChartAppearance.ColumnY; }
            set { bubbleChartAppearance.ColumnY = value; }
        }

        /// <summary>
        /// ������ ��� Z
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ��� Z ���������")]
        [DisplayName("������ ��� Z")]
        [DefaultValue(2)]
        [Browsable(true)]
        public int ColumnZ
        {
            get { return bubbleChartAppearance.ColumnZ; }
            set { bubbleChartAppearance.ColumnZ = value; }
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
            get { return bubbleChartAppearance.NullHandling; }
            set { bubbleChartAppearance.NullHandling = value; }
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
            get { return bubbleChartAppearance.ChartText; }
        }

        #endregion

        public BubbleChartBrowseClass(BubbleChartAppearance bubbleChartAppearance)
        {
            this.bubbleChartAppearance = bubbleChartAppearance;
        }

        public override string ToString()
        {
            return BubbleShapeTypeConverter.ToString(BubbleShape) + "; " + ChartSortTypeConverter.ToString(SortByRadius);
        }
    }
}