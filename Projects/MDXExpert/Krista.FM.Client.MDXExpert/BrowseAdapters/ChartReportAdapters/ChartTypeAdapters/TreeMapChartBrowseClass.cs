using System.ComponentModel;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TreeMapChartBrowseClass : FilterablePropertyBase
    {
        #region ����

        private TreeMapChartAppearance treeMapChartAppearance;
        private LabelStyleBrowseClass labelStyleBrowse;
        private PaintElementBrowseClass paintElementBrowse;
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
        /// ���� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("���� �������")]
        [DisplayName("���� �������")]
        [DefaultValue(typeof(Color), "Gray")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return treeMapChartAppearance.BorderColor; }
            set { treeMapChartAppearance.BorderColor = value; }
        }

        /// <summary>
        /// ������ �������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ �������")]
        [DisplayName("������ �������")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int BorderWidth
        {
            get { return treeMapChartAppearance.BorderWidth; }
            set { treeMapChartAppearance.BorderWidth = value; }
        }

        /// <summary>
        /// ��������� ��������� ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ��������� ��������")]
        [DisplayName("��������� ��������� ��������")]
        [DefaultValue("")]
        [Browsable(true)]
        public string ChartTitle
        {
            get { return treeMapChartAppearance.ChartTitle; }
            set { treeMapChartAppearance.ChartTitle = value; }
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������ �����")]
        [DisplayName("������ �����")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColorValueIndex
        {
            get { return treeMapChartAppearance.ColorValueIndex; }
            set
            {
                treeMapChartAppearance.ColorValueIndex = ChartReportElement.CheckColumnIndexValue((DataTable)chart.DataSource, value);
                treeMapChartAppearance.ColorValueLabel = ChartReportElement.GetColumnName((DataTable)chart.DataSource, treeMapChartAppearance.ColorValueIndex);
            }
        }

        /// <summary>
        /// ������ �������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ �������")]
        [DisplayName("������ �������")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int SizeValueIndex
        {
            get { return treeMapChartAppearance.SizeValueIndex; }
            set
            {
                treeMapChartAppearance.SizeValueIndex = ChartReportElement.CheckColumnIndexValue((DataTable)chart.DataSource, value);
                treeMapChartAppearance.SizeValueLabel = ChartReportElement.GetColumnName((DataTable)chart.DataSource, treeMapChartAppearance.SizeValueIndex);
            }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [DefaultValue("COLOR_LABEL")]
        [Browsable(true)]
        public string ColorValueLabel
        {
            get { return treeMapChartAppearance.ColorValueLabel; }
            set { treeMapChartAppearance.ColorValueLabel = value; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �������")]
        [DisplayName("����� �������")]
        [DefaultValue("SIZE_LABEL")]
        [Browsable(true)]
        public string SizeValueLabel
        {
            get { return treeMapChartAppearance.SizeValueLabel; }
            set { treeMapChartAppearance.SizeValueLabel = value; }
        }

        /// <summary>
        /// ������������ �������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������������ �������� �����")]
        [DisplayName("������������ �������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool DisableColorValues
        {
            get { return treeMapChartAppearance.DisableColorValues; }
            set { treeMapChartAppearance.DisableColorValues = value; }
        }

        /// <summary>
        /// ������� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� �������")]
        [DisplayName("������� �������")]
        [Browsable(true)]
        public int[] IndexOrder
        {
            get { return treeMapChartAppearance.IndexOrder; }
            set { treeMapChartAppearance.IndexOrder = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������")]
        [DisplayName("������")]
        [DefaultValue(6)]
        [Browsable(true)]
        public int Margin
        {
            get { return treeMapChartAppearance.Margin; }
            set { treeMapChartAppearance.Margin = value; }
        }

        /// <summary>
        ///  ���� ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("���� ��������")]
        [DisplayName("���� ��������")]
        [DefaultValue(typeof(double), "45")]
        [DynamicPropertyFilter("TreeMapType", "Circular")]
        [Browsable(true)]
        public double Rotation
        {
            get { return treeMapChartAppearance.Rotation; }
            set { treeMapChartAppearance.Rotation = value; }
        }

        /// <summary>
        ///  ���
        /// </summary>
        [Category("������� ���������")]
        [Description("���")]
        [DisplayName("���")]
        [DefaultValue(typeof(TreeMapType), "Rectangular")]
        [TypeConverter(typeof(TreeMapTypeConverter))]
        [Browsable(true)]
        public TreeMapType TreeMapType
        {
            get { return treeMapChartAppearance.TreeMapType; }
            set { treeMapChartAppearance.TreeMapType = value; }
        }

        /// <summary>
        ///  ���������� ��� ����� � �������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ��� ����� � �������")]
        [DisplayName("���������� ��� ����� � �������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowColorIntervalAxis
        {
            get { return treeMapChartAppearance.ShowColorIntervalAxis; }
            set { treeMapChartAppearance.ShowColorIntervalAxis = value; }
        }

        /// <summary>
        ///  ���������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� �����")]
        [DisplayName("���������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowLabels
        {
            get { return treeMapChartAppearance.ShowLabels; }
            set { treeMapChartAppearance.ShowLabels = value; }
        }

        /// <summary>
        ///  ���������� ��������� ��� ����� ��� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ��������� ��� ����� ��� �����")]
        [DisplayName("���������� ��������� ��� ����� ��� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowHeadersForUnlabeledPoints
        {
            get { return treeMapChartAppearance.ShowHeadersForUnlabeledPoints; }
            set { treeMapChartAppearance.ShowHeadersForUnlabeledPoints = value; }
        }

        /// <summary>
        ///  ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Browsable(true)]
        public LabelStyleBrowseClass LabelStyleBrowse
        {
            get { return labelStyleBrowse; }
            set { labelStyleBrowse  = value; }
        }

        /// <summary>
        ///  �������������� ������� �����������
        /// </summary>
        [Category("������� ���������")]
        [Description("�������������� ������� �����������")]
        [DisplayName("�������������� ������� �����������")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        /// <summary>
        ///  ������������ �������������� ������� �����������
        /// </summary>
        [Category("������� ���������")]
        [Description("������������ �������������� ������� �����������")]
        [DisplayName("������������ �������������� ������� �����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool UseStaticLeafPE
        {
            get { return treeMapChartAppearance.UseStaticLeafPE; }
            set { treeMapChartAppearance.UseStaticLeafPE = value; }
        }

        /// <summary>
        ///  ��������� ��������� �����������
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ��������� �����������")]
        [DisplayName("��������� ��������� �����������")]
        [Browsable(true)]
        public PaintElementCollection PEs
        {
            get { return treeMapChartAppearance.PEs; }
        }

        #endregion

        public TreeMapChartBrowseClass(TreeMapChartAppearance treeMapChartAppearance, UltraChart chart)
        {
            this.treeMapChartAppearance = treeMapChartAppearance;
            this.chart = chart;

            labelStyleBrowse = new LabelStyleBrowseClass(treeMapChartAppearance.LabelStyle);
            paintElementBrowse = new PaintElementBrowseClass(treeMapChartAppearance.StaticLeafPE);
        }

        public override string ToString()
        {
            return TreeMapTypeConverter.ToString(TreeMapType) + "; " + BooleanTypeConverter.ToString(ShowLabels);
        }
    }
}