using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ����� ����� ��� ���������
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisGridLinesBrowseClass : FilterablePropertyBase
    {
        #region ����

        private AxisAppearance axisAppearance;
        private GridlinesAppearance gridlinesAppearance;
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
        /// �������� �� ��������� � �����
        /// </summary>
        [Browsable(false)]
        public bool Is3D
        {
            get
            {
                return
                    (ChartType == ChartType.AreaChart3D || ChartType == ChartType.BarChart3D ||
                     ChartType == ChartType.ColumnChart3D || ChartType == ChartType.LineChart3D ||
                     ChartType == ChartType.CylinderBarChart3D || ChartType == ChartType.CylinderColumnChart3D ||
                     ChartType == ChartType.CylinderStackBarChart3D || ChartType == ChartType.BubbleChart3D ||
                     ChartType == ChartType.CylinderStackColumnChart3D || ChartType == ChartType.HeatMapChart3D ||
                     ChartType == ChartType.SplineAreaChart3D || ChartType == ChartType.PointChart3D ||
                     ChartType == ChartType.SplineChart3D || ChartType == ChartType.Stack3DBarChart ||
                     ChartType == ChartType.Stack3DColumnChart);
            }
        }

        /// <summary>
        /// ��������� ����� ��� Z ��� 3D ��������
        /// </summary>
        [Browsable(false)]
        public bool MajorGridVisible3DEnable
        {
            get { return Is3D && axisAppearance.axisNumber == AxisNumber.Z_Axis; }
        }

        /// <summary>
        /// ���� ����� �����
        /// </summary>
        [Category("���")]
        [Description("���� ����� �����")]
        [DisplayName("����")]
        [DefaultValue(typeof(Color), "Gainsboro")]
        [Browsable(true)]
        public Color LinesColor
        {
            get { return gridlinesAppearance.Color; }
            set { gridlinesAppearance.Color = value; }
        }

        /// <summary>
        /// ������������ ����� �����
        /// </summary>
        [Category("���")]
        [Description("������������ ����� �����")]
        [DisplayName("������������")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("Is3D", "False")]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte LinesAlphaLevel
        {
            get { return gridlinesAppearance.AlphaLevel; }
            set { gridlinesAppearance.AlphaLevel = value; }
        }

        /// <summary>
        /// ������� ����� �����
        /// </summary>
        [Category("���")]
        [Description("������� ����� �����")]
        [DisplayName("�������")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("Is3D", "False")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int LinesThickness
        {
            get { return gridlinesAppearance.Thickness; }
            set { gridlinesAppearance.Thickness = value; }
        }

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        [Category("���")]
        [Description("����� ����� �����")]
        [DisplayName("�����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DynamicPropertyFilter("Is3D", "False")]
        [DefaultValue(LineDrawStyle.Dot)]
        [Browsable(true)]
        public LineDrawStyle LinesStyle
        {
            get { return gridlinesAppearance.DrawStyle; }
            set { gridlinesAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// ��������� ����� �����
        /// </summary>
        [Category("���")]
        [Description("���������� ����� �����")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("MajorGridVisible3DEnable", "False")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool LinesVisible
        {
            get { return gridlinesAppearance.Visible; }
            set { gridlinesAppearance.Visible = value; }
        }

        #endregion

        public AxisGridLinesBrowseClass(GridlinesAppearance gridlinesAppearance, UltraChart chart, AxisAppearance axisAppearance)
        {
            this.gridlinesAppearance = gridlinesAppearance;
            this.chart = chart;
            this.axisAppearance = axisAppearance;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(LinesVisible) + "; " + LinesColor.Name + "; " + LineDrawStyleTypeConverter.ToString(LinesStyle);
        }
    }
}