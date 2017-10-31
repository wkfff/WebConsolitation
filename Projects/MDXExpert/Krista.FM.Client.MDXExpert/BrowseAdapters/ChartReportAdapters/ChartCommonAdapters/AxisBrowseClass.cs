using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ��� ���������
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisBrowseClass : FilterablePropertyBase
    {
        #region ����

        private AxisAppearance axisAppearance;
        private UltraChart chart;

        private AxisLabelBrowseClass axisLabelsBrowse;
        private ScrollScaleBrowseClass scrollScaleBrowse;
        private StripLineBrowseClass stripLineBrowse2D;
        private StripLineBrowseClass stripLineBrowse3D;
        private AxisTickmarkBrowseClass tickmarkBrowse;
        private AxisRangeBrowseClass rangeBrowse;
        private AxisLineStyleBrowseClass lineStyleBrowse;
        private AxisGroupMarginsBrowseClass groupMarginsBrowse;
        private AxisGroupGridBrowseClass groupGridBrowse;
        private AxisSeriesLabelBrowseClass axisSeriesLabelBrowse;

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
        /// ����������� ����� ���
        /// </summary>
        [Browsable(false)]
        public bool LabelsEnable
        {
            get
            {
                return InfragisticsUtils.IsAvaibleLabelAxis(ChartType, axisAppearance.axisNumber);
            }
        }
        
        /// <summary>
        /// ����������� ����� �����
        /// </summary>
        [Browsable(false)]
        public bool SeriesLabelsEnable
        {
            get
            {
                return InfragisticsUtils.IsAvaibleSeriesLabelAxis(ChartType, axisAppearance.axisNumber);
            }
        }

        /// <summary>
        /// �������� �� ��������� � �����
        /// </summary>
        [Browsable(false)]
        public bool Is3D
        {
            get
            {
                return Common.InfragisticsUtils.Is3DChart(ChartType);
            }
        }

        /// <summary>
        /// ����������� ��������� ��������� ���
        /// </summary>
        [Browsable(false)]
        public bool AxisVisisbleEnable
        {
            get
            {
                return Common.InfragisticsUtils.IsAvaibleVisibleAxis(ChartType, axisAppearance.axisNumber);
            }
        }


        /// <summary>
        /// ����������� �������� "����� �����" ��� 2D ��������
        /// </summary>
        [Browsable(false)]
        public bool StripLinesEnable2D
        {
            get
            {
                return (axisAppearance.axisNumber == AxisNumber.X_Axis &&
                        (ChartType == ChartType.ColumnChart || ChartType == ChartType.ColumnLineChart)) ||
                       (axisAppearance.axisNumber == AxisNumber.Y_Axis &&
                        (ChartType == ChartType.BarChart || ChartType == ChartType.GanttChart));
            }
        }

        /// <summary>
        /// ����������� �������� "����� �����" ��� 3D ��������
        /// </summary>
        [Browsable(false)]
        public bool StripLinesEnable3D
        {
            get
            {
                return (axisAppearance.axisNumber == AxisNumber.X_Axis &&
                        (ChartType == ChartType.BarChart3D || ChartType == ChartType.CylinderBarChart3D ||
                         ChartType == ChartType.CylinderStackBarChart3D || ChartType == ChartType.Stack3DBarChart) ||
                       (axisAppearance.axisNumber == AxisNumber.Y_Axis &&
                        (ChartType == ChartType.AreaChart3D || ChartType == ChartType.SplineAreaChart3D ||
                         ChartType == ChartType.ColumnChart3D || ChartType == ChartType.CylinderColumnChart3D ||
                         ChartType == ChartType.CylinderStackColumnChart3D || ChartType == ChartType.Stack3DColumnChart ||
                         ChartType == ChartType.HeatMapChart3D || ChartType == ChartType.LineChart3D ||
                         ChartType == ChartType.SplineChart3D))) ||
                        ((axisAppearance.axisNumber == AxisNumber.X_Axis ||
                          axisAppearance.axisNumber == AxisNumber.Y_Axis ||
                          axisAppearance.axisNumber == AxisNumber.Z_Axis) &&
                        (ChartType == ChartType.BubbleChart3D || ChartType == ChartType.PointChart3D));
            }
        }

        /// <summary>
        /// ����������� �������� "�����"
        /// </summary>
        [Browsable(false)]
        public bool GridLinesEnable
        {
            get
            {
                return ChartType == ChartType.AreaChart3D ||
                        ChartType == ChartType.BarChart3D ||
                        ChartType == ChartType.ColumnChart3D ||
                        ChartType == ChartType.LineChart3D ||
                        ChartType == ChartType.CylinderBarChart3D ||
                        ChartType == ChartType.CylinderColumnChart3D ||
                        ChartType == ChartType.CylinderStackBarChart3D ||
                        ChartType == ChartType.BubbleChart3D ||
                        ChartType == ChartType.CylinderStackColumnChart3D ||
                        ChartType == ChartType.HeatMapChart3D ||
                        ChartType == ChartType.SplineAreaChart3D ||
                        ChartType == ChartType.SplineChart3D ||
                        ChartType == ChartType.PointChart3D ||
                        ChartType == ChartType.Stack3DBarChart ||
                        ChartType == ChartType.Stack3DColumnChart ||
                        ChartType == ChartType.BarChart ||
                        ChartType == ChartType.BubbleChart ||
                        ChartType == ChartType.ColumnChart ||
                        ChartType == ChartType.ColumnLineChart ||
                        ChartType == ChartType.StackColumnChart ||
                        ChartType == ChartType.StackBarChart ||
                        ChartType == ChartType.HeatMapChart ||
                        ChartType == ChartType.ScatterLineChart ||
                        ChartType == ChartType.ScatterChart ||
                        ChartType == ChartType.BoxChart ||
                        ChartType == ChartType.ParetoChart ||
                        ChartType == ChartType.ProbabilityChart ||
                        ChartType == ChartType.AreaChart ||
                        ChartType == ChartType.SplineAreaChart ||
                        ChartType == ChartType.StackAreaChart ||
                        ChartType == ChartType.StackSplineAreaChart ||
                        ChartType == ChartType.LineChart ||
                        ChartType == ChartType.SplineChart ||
                        ChartType == ChartType.StackLineChart ||
                        ChartType == ChartType.StackSplineChart ||
                        ChartType == ChartType.HistogramChart ||
                        ChartType == ChartType.PolarChart ||
                        (ChartType == ChartType.RadarChart && axisAppearance.axisNumber == AxisNumber.Y_Axis) ||
                        ChartType == ChartType.CandleChart ||
                        ChartType == ChartType.GanttChart ||
                        ChartType == ChartType.StepAreaChart ||
                        ChartType == ChartType.StepLineChart;
            }
        }

        /// <summary>
        /// ����������� ��������� �������� � ������ ���������
        /// </summary>
        [Browsable(false)]
        public bool ScrollScaleEnable
        {
            get
            {
                return InfragisticsUtils.IsAvaibleScrollScaleAxis(ChartType, axisAppearance.axisNumber);
            }
        }

        /// <summary>
        /// ����������� ��������� ���������
        /// </summary>
        [Browsable(false)]
        public bool RangeEnable
        {
            get
            {
                return !((ChartType == ChartType.RadarChart || ChartType == ChartType.PolarChart) &&
                        axisAppearance.axisNumber == AxisNumber.X_Axis) && (axisAppearance.Labels.ItemFormat != AxisItemLabelFormat.ItemLabel);
            }
        }

        /// <summary>
        /// ����������� ��������� ����� ���
        /// </summary>
        [Browsable(false)]
        public bool LineAxisEnable
        {
            get
            {
                return !((ChartType == ChartType.RadarChart || ChartType == ChartType.PolarChart) &&
                        axisAppearance.axisNumber == AxisNumber.X_Axis);
            }
        }

        /// <summary>
        /// ����� �� ��� ���������
        /// </summary>
        [Description("����� �� ��� ���������")]
        [DisplayName("�����")]
        [DynamicPropertyFilter("LabelsEnable", "True")]
        [Browsable(true)]
        public AxisLabelBrowseClass AxisLabelsBrowse
        {
            get { return axisLabelsBrowse; }
            set { axisLabelsBrowse = value; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [DynamicPropertyFilter("SeriesLabelsEnable", "True")]
        [Browsable(true)]
        public AxisSeriesLabelBrowseClass AxisSeriesLabelBrowse
        {
            get { return axisSeriesLabelBrowse; }
            set { axisSeriesLabelBrowse = value; }
        }

        /// <summary>
        /// ������ ������� ���
        /// </summary>
        [Description("������ ������� ���")]
        [DisplayName("������ ������� ���")]
        [DynamicPropertyFilter("ChartType", "ColumnChart, BarChart, AreaChart, LineChart, BubbleChart, "
                                            + "ScatterChart, HeatMapChart, StackBarChart, StackColumnChart, "
                                            + "SplineChart, SplineAreaChart, ColumnLineChart, ScatterLineChart, ParetoChart, "
                                            + "StackAreaChart, StackLineChart, StackSplineChart, StackSplineAreaChart, HistogramChart, "
                                            + "ProbabilityChart, BoxChart, GanttChart, StackColumnChart, StackBarChart")]
        [DefaultValue(80)]
        [Browsable(true)]
        public int Extent
        {
            get { return axisAppearance.Extent; }
            set { axisAppearance.Extent = value; }
        }

        /// <summary>
        /// ����� ���
        /// </summary>
        [Description("����� ���")]
        [DisplayName("�����")]
        [DynamicPropertyFilter("GridLinesEnable", "True")]
        [Browsable(true)]
        public AxisGroupGridBrowseClass AxisGroupGridBrowse
        {
            get { return groupGridBrowse; }
            set { groupGridBrowse = value; }
        }

        /// <summary>
        /// ����� ���
        /// </summary>
        [Description("����� ���")]
        [DisplayName("�����")]
        [DynamicPropertyFilter("LineAxisEnable", "True")]
        [Browsable(true)]
        public AxisLineStyleBrowseClass AxisLineStyleBrowse
        {
            get { return lineStyleBrowse; }
            set { lineStyleBrowse = value; }
        }

        /// <summary>
        /// ���� ���
        /// </summary>
        [Description("���� ���")]
        [DisplayName("����")]
        [DynamicPropertyFilter("ChartType", "ColumnChart, BarChart, AreaChart, LineChart, BubbleChart, "
                                            + "ScatterChart, HeatMapChart, StackBarChart, StacKColumnChart, "
                                            + "SplineChart, SplineAreaChart, ColumnLineChart, ScatterLineChart, ParetoChart, "
                                            + "StackAreaChart, StackLineChart, StackSplineChart, StackSplineAreaChart, HistogramChart, "
                                            + "ProbabilityChart, BoxChart, GanttChart, StackColumnChart, StackBarChart")]
        [Browsable(true)]
        public AxisGroupMarginsBrowseClass AxisGroupMarginsBrowse
        {
            get { return groupMarginsBrowse; }
            set { groupMarginsBrowse = value; }
        }
        
        /// <summary>
        /// ������� � ��������� ���
        /// </summary>
        [Description("������� � ��������� ���")]
        [DisplayName("������� � ���������")]
        [DynamicPropertyFilter("ScrollScaleEnable", "True")]
        [Browsable(true)]
        public ScrollScaleBrowseClass ScrollScaleBrowse
        {
            get { return scrollScaleBrowse; }
            set { scrollScaleBrowse = value; }
        }

        /// <summary>
        /// ����� ����� ���
        /// </summary>
        [Description("����� ����� ���")]
        [DisplayName("����� �����")]
        [DynamicPropertyFilter("StripLinesEnable2D", "True")]
        [Browsable(true)]
        public StripLineBrowseClass StripLineBrowse2D
        {
            get { return stripLineBrowse2D; }
            set { stripLineBrowse2D = value; }
        }

        /// <summary>
        /// ���������� ����� ���
        /// </summary>
        [Description("���������� ����� ���")]
        [DisplayName("���������� ����� ")]
        [DynamicPropertyFilter("StripLinesEnable3D", "True")]
        [Browsable(true)]
        public StripLineBrowseClass StripLineBrowse3D
        {
            get { return stripLineBrowse3D; }
            set { stripLineBrowse3D = value; }
        }

        /// <summary>
        /// ��������� ���
        /// </summary>
        [Description("���������� ���")]
        [DisplayName("����������")]
        [DynamicPropertyFilter("AxisVisisbleEnable", "True")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool Visible
        {
            get { return axisAppearance.Visible; }
            set { axisAppearance.Visible = value; }
        }

        /// <summary>
        /// �������
        /// </summary>
        [Description("������� ���")]
        [DisplayName("�������")]
        [Browsable(true)]
        public AxisTickmarkBrowseClass TickmarkBrowse
        {
            get { return tickmarkBrowse; }
            set { tickmarkBrowse = value; }
        }

        /// <summary>
        /// �������� ���
        /// </summary>
        [Description("�������� ���")]
        [DisplayName("��������")]
        [DynamicPropertyFilter("RangeEnable", "True")]
        [Browsable(true)]
        public AxisRangeBrowseClass RangeBrowse
        {
            get { return rangeBrowse; }
            set { rangeBrowse = value; }
        }

        #endregion

        public AxisBrowseClass(AxisAppearance axisAppearance, UltraChart chart)
        {
            this.axisAppearance = axisAppearance;
            this.chart = chart;

            axisLabelsBrowse = new AxisLabelBrowseClass(axisAppearance);
            scrollScaleBrowse = new ScrollScaleBrowseClass(axisAppearance.ScrollScale, axisAppearance, chart);
            stripLineBrowse2D = new StripLineBrowseClass(axisAppearance.StripLines, chart);
            stripLineBrowse3D = new StripLineBrowseClass(axisAppearance.StripLines, chart);
            tickmarkBrowse = new AxisTickmarkBrowseClass(axisAppearance, chart);
            rangeBrowse = new AxisRangeBrowseClass(axisAppearance, chart);
            lineStyleBrowse = new AxisLineStyleBrowseClass(axisAppearance);
            groupMarginsBrowse = new AxisGroupMarginsBrowseClass(axisAppearance);
            groupGridBrowse = new AxisGroupGridBrowseClass(axisAppearance, chart);

            this.axisSeriesLabelBrowse = new AxisSeriesLabelBrowseClass(axisAppearance.Labels.SeriesLabels, axisAppearance);
        }

        public override string ToString()
        {
            return lineStyleBrowse.LineColor.Name + "; " + LineDrawStyleTypeConverter.ToString(lineStyleBrowse.LineStyle) + "; " + lineStyleBrowse.LineThickness;
        }
    }
}