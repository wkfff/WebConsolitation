using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;
using System.Drawing;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert.Common
{
    public struct InfragistisUtils
    {
        #region ����� ���� ���������
        private static bool oldXLogAxisType;
        private static bool oldX2LogAxisType;
        private static bool oldYLogAxisType;
        private static bool oldY2LogAxisType;


        /// <summary>
        /// �.�. ��� ����� ���� ��������� ��������� ��� ��������� ��� ����, ����� ���������� ��
        /// � ���������� ����� ����� ����
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="chartType"></param>
        static public void ChangeChartType(UltraChart chart, ChartType chartType)
        {
            try
            {
                chart.SuspendLayout();

                AxisAppearance oldX = new AxisAppearance();
                AxisAppearance oldY = new AxisAppearance();
                AxisAppearance oldZ = new AxisAppearance();
                AxisAppearance oldX2 = new AxisAppearance();
                AxisAppearance oldY2 = new AxisAppearance();
                AxisAppearance oldZ2 = new AxisAppearance();

                ChangeWithHistorgramType(chart, chartType);


                SynchronizeAxisAppearance(oldX, chart.Axis.X);
                SynchronizeAxisAppearance(oldY, chart.Axis.Y);
                SynchronizeAxisAppearance(oldZ, chart.Axis.Z);
                SynchronizeAxisAppearance(oldX2, chart.Axis.X2);
                SynchronizeAxisAppearance(oldY2, chart.Axis.Y2);
                SynchronizeAxisAppearance(oldZ2, chart.Axis.Z2);

                //���������� ��� ���������
                chart.ChartType = chartType;
                chart.DataBind();

                SynchronizeAxisAppearance(chart.Axis.X, oldX);
                SynchronizeAxisAppearance(chart.Axis.Y, oldY);
                SynchronizeAxisAppearance(chart.Axis.Z, oldZ);
                SynchronizeAxisAppearance(chart.Axis.X2, oldX2);
                SynchronizeAxisAppearance(chart.Axis.Y2, oldY2);
                SynchronizeAxisAppearance(chart.Axis.Z2, oldZ2);
            }
            finally
            {
                chart.ResumeLayout();
            }
        }

        private static void ChangeWithHistorgramType(UltraChart chart, ChartType chartType)
        {
            if (chartType == ChartType.HistogramChart)
            {
                oldXLogAxisType = (chart.Axis.X.NumericAxisType == NumericAxisType.Logarithmic);
                oldX2LogAxisType = (chart.Axis.X2.NumericAxisType == NumericAxisType.Logarithmic);
                oldYLogAxisType = (chart.Axis.Y.NumericAxisType == NumericAxisType.Logarithmic);
                oldY2LogAxisType = (chart.Axis.Y2.NumericAxisType == NumericAxisType.Logarithmic);

                chart.Axis.X.NumericAxisType = NumericAxisType.Linear;
                chart.Axis.X2.NumericAxisType = NumericAxisType.Linear;
                chart.Axis.Y.NumericAxisType = NumericAxisType.Linear;
                chart.Axis.Y2.NumericAxisType = NumericAxisType.Linear;
            }

            if (chart.ChartType == ChartType.HistogramChart)
            {
                chart.Axis.X.NumericAxisType = oldXLogAxisType
                                                   ? NumericAxisType.Logarithmic
                                                   : NumericAxisType.Linear;
                chart.Axis.X2.NumericAxisType = oldX2LogAxisType
                                                    ? NumericAxisType.Logarithmic
                                                    : NumericAxisType.Linear;
                chart.Axis.Y.NumericAxisType = oldYLogAxisType
                                                   ? NumericAxisType.Logarithmic
                                                   : NumericAxisType.Linear;
                chart.Axis.Y2.NumericAxisType = oldY2LogAxisType
                                                    ? NumericAxisType.Logarithmic
                                                    : NumericAxisType.Linear;
            }
        }

        /// <summary>
        /// �������������� �������� ����� ���� ���������
        /// </summary>
        /// <param name="axisLabel">���������������� �����</param>
        /// <param name="templateAxisLabel">������ ��� �������������</param>
        static public void SynchronizeAxisLabelAppearance(AxisLabelAppearance axisLabel,
            AxisLabelAppearance templateAxisLabel)
        {
            axisLabel.ClipText = templateAxisLabel.ClipText;
            axisLabel.Flip = templateAxisLabel.Flip;
            axisLabel.Font = templateAxisLabel.Font.Clone() as Font;
            axisLabel.FontColor = templateAxisLabel.FontColor;
            axisLabel.FontSizeBestFit = templateAxisLabel.FontSizeBestFit;
            axisLabel.HorizontalAlign = templateAxisLabel.HorizontalAlign;
            //��������� ������� ��� ����� � ��� �� ���� ������ �������� ������������, ������� ������ ���� �� ����� ����������������
            //axisLabel.ItemFormatString = templateAxisLabel.ItemFormatString;
            axisLabel.Orientation = templateAxisLabel.Orientation;
            axisLabel.OrientationAngle = templateAxisLabel.OrientationAngle;
            axisLabel.VerticalAlign = templateAxisLabel.VerticalAlign;
            axisLabel.Visible = templateAxisLabel.Visible;
            axisLabel.WrapText = templateAxisLabel.WrapText;

            // UltraChart ��� ���������� ������ ����� ����� �����, ������� �� �� ����� ����������������
            axisLabel.SeriesLabels.ClipText = templateAxisLabel.SeriesLabels.ClipText;
            axisLabel.SeriesLabels.Flip = templateAxisLabel.SeriesLabels.Flip;
            axisLabel.SeriesLabels.Font = templateAxisLabel.SeriesLabels.Font.Clone() as Font;
            axisLabel.SeriesLabels.FontColor = templateAxisLabel.SeriesLabels.FontColor;
            axisLabel.SeriesLabels.FontSizeBestFit = templateAxisLabel.SeriesLabels.FontSizeBestFit;
            axisLabel.SeriesLabels.HorizontalAlign = templateAxisLabel.SeriesLabels.HorizontalAlign;
            axisLabel.SeriesLabels.Orientation = templateAxisLabel.SeriesLabels.Orientation;
            axisLabel.SeriesLabels.OrientationAngle = templateAxisLabel.SeriesLabels.OrientationAngle;
            axisLabel.SeriesLabels.VerticalAlign = templateAxisLabel.SeriesLabels.VerticalAlign;
            axisLabel.SeriesLabels.Visible = templateAxisLabel.SeriesLabels.Visible;
            axisLabel.SeriesLabels.WrapText = templateAxisLabel.SeriesLabels.WrapText;
        }

        /// <summary>
        /// �������������� ��������� ��� ���������
        /// </summary>
        /// <param name="axis">���������������� ���</param>
        /// <param name="templateAxis">������ ��� �������������</param>
        static public void SynchronizeAxisAppearance(AxisAppearance axis, AxisAppearance templateAxis)    
        {
            axis.Extent = templateAxis.Extent;

            SynchronizeAxisLabelAppearance(axis.Labels, templateAxis.Labels);

            axis.LineColor = templateAxis.LineColor;
            axis.LineDrawStyle = templateAxis.LineDrawStyle;
            axis.LineEndCapStyle = templateAxis.LineEndCapStyle;
            axis.LineThickness = templateAxis.LineThickness;
            axis.LogBase = templateAxis.LogBase;
            axis.LogZero = templateAxis.LogZero;
            axis.MajorGridLines = (GridlinesAppearance)templateAxis.MajorGridLines.Clone();
            axis.Margin.Far.MarginType = templateAxis.Margin.Far.MarginType;
            axis.Margin.Far.Value = templateAxis.Margin.Far.Value;
            axis.Margin.Near.MarginType = templateAxis.Margin.Near.MarginType;
            axis.Margin.Near.Value = templateAxis.Margin.Near.Value;
            axis.MinorGridLines = (GridlinesAppearance)templateAxis.MinorGridLines.Clone();
            axis.NumericAxisType = templateAxis.NumericAxisType;
            axis.RangeMax = templateAxis.RangeMax;
            axis.RangeMin = templateAxis.RangeMin;
            axis.RangeType = templateAxis.RangeType;
            axis.ScrollScale = (ScrollScaleAppearance)templateAxis.ScrollScale.Clone();
            axis.StripLines = (StripLineAppearance)templateAxis.StripLines.Clone();
            axis.TickmarkInterval = templateAxis.TickmarkInterval;
            axis.TickmarkIntervalType = templateAxis.TickmarkIntervalType;
            axis.TickmarkPercentage = templateAxis.TickmarkPercentage;
            axis.TickmarkStyle = templateAxis.TickmarkStyle;
            templateAxis.TimeAxisStyle.CopyProperties(axis.TimeAxisStyle);
            axis.Visible = templateAxis.Visible;
        }
        #endregion

        /// <summary>
        /// �������� �������� �������� ������
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static ChartTextCollection[] GetChartTextCollection(UltraChart chart)
        {
            ChartTextCollection[] chartTexts = new ChartTextCollection[2];
            if (chart != null)
            {
                switch (chart.ChartType)
                {
                    case ChartType.StackAreaChart:
                    case ChartType.AreaChart:
                        {
                            chartTexts[0] = chart.AreaChart.ChartText;
                            break;
                        }
                    case ChartType.StackBarChart:
                    case ChartType.BarChart:
                        {
                            chartTexts[0] = chart.BarChart.ChartText;
                            break;
                        }
                    case ChartType.BubbleChart:
                        {
                            chartTexts[0] = chart.BubbleChart.ChartText;
                            break;
                        }
                    case ChartType.StackColumnChart:
                    case ChartType.ColumnChart:
                        {
                            chartTexts[0] = chart.ColumnChart.ChartText;
                            break;
                        }
                    case ChartType.ColumnLineChart:
                        {
                            chartTexts[0] = chart.ColumnLineChart.Column.ChartText;
                            chartTexts[1] = chart.ColumnLineChart.Line.ChartText;
                            break;
                        }
                    case ChartType.HeatMapChart:
                        {
                            chartTexts[0] = chart.HeatMapChart.ChartText;
                            break;
                        }
                    case ChartType.FunnelChart:
                        {
                            chartTexts[0] = chart.FunnelChart.ChartText;
                            break;
                        }
                    case ChartType.PyramidChart:
                        {
                            chartTexts[0] = chart.PyramidChart.ChartText;
                            break;
                        }
                    case ChartType.StackLineChart:
                    case ChartType.LineChart:
                        {
                            chartTexts[0] = chart.LineChart.ChartText;
                            break;
                        }
                    case ChartType.PieChart:
                        {
                            chartTexts[0] = chart.PieChart.ChartText;
                            break;
                        }
                    case ChartType.DoughnutChart:
                        {
                            chartTexts[0] = chart.DoughnutChart.ChartText;
                            break;
                        }
                    case ChartType.PolarChart:
                        {
                            chartTexts[0] = chart.PolarChart.ChartText;
                            break;
                        }
                    case ChartType.ProbabilityChart:
                        {
                            chartTexts[0] = chart.ProbabilityChart.ChartText;
                            break;
                        }
                    case ChartType.ScatterChart:
                        {
                            chartTexts[0] = chart.ScatterChart.ChartText;
                            break;
                        }
                    case ChartType.ScatterLineChart:
                        {
                            chartTexts[0] = chart.ScatterLineChart.Scatter.ChartText;
                            chartTexts[1] = chart.ScatterLineChart.Line.ChartText;
                            break;
                        }
                    case ChartType.StackSplineChart:
                    case ChartType.SplineChart:
                        {
                            chartTexts[0] = chart.SplineChart.ChartText;
                            break;
                        }
                    case ChartType.StackSplineAreaChart:
                    case ChartType.SplineAreaChart:
                        {
                            chartTexts[0] = chart.SplineAreaChart.ChartText;
                            break;
                        }
                }
            }
            return chartTexts;
        }

        /// <summary>
        /// �������� �� � ��������� �������� �������� ������
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static bool IsAvaibleChartText(UltraChart chart)
        {
            return (chart == null) ? false : IsAvaibleChartText(chart.ChartType);
        }

        /// <summary>
        /// �������� �� � ��������� �������� �������� ������
        /// </summary>
        /// <param name="chartType"></param>
        /// <returns></returns>
        public static bool IsAvaibleChartText(ChartType chartType)
        {
            return (chartType == ChartType.StackAreaChart || chartType == ChartType.AreaChart
                || chartType == ChartType.StackBarChart || chartType == ChartType.BarChart
                || chartType == ChartType.BubbleChart || chartType == ChartType.StackColumnChart
                || chartType == ChartType.ColumnChart || chartType == ChartType.ColumnLineChart
                || chartType == ChartType.HeatMapChart || chartType == ChartType.FunnelChart
                || chartType == ChartType.PyramidChart || chartType == ChartType.StackLineChart
                || chartType == ChartType.LineChart || chartType == ChartType.PieChart
                || chartType == ChartType.DoughnutChart || chartType == ChartType.PolarChart
                || chartType == ChartType.ProbabilityChart || chartType == ChartType.ScatterChart
                || chartType == ChartType.ScatterLineChart || chartType == ChartType.StackSplineChart
                || chartType == ChartType.SplineChart || chartType == ChartType.StackSplineAreaChart
                || chartType == ChartType.SplineAreaChart);
        }

        /// <summary>
        /// ���������� �� ��������� � ���� ������
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static bool Is3DChart(UltraChart chart)
        {
            return (chart == null) ? false : Is3DChart(chart.ChartType);
        }

        /// <summary>
        /// �������� �� ��� ��������� ���� ������
        /// </summary>
        /// <param name="chartType"></param>
        /// <returns></returns>
        public static bool Is3DChart(ChartType chartType)
        {
            return chartType.ToString().Contains("3D");
        }

        /// <summary>
        /// ������� �� � ��������� ���
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static bool IsAvaibleAxis(UltraChart chart)
        {
            return (chart == null) ? false : IsAvaibleAxis(chart.ChartType);
        }

        /// <summary>
        /// ������� �� � ��������� ���
        /// </summary>
        /// <param name="chartType"></param>
        /// <returns></returns>
        public static bool IsAvaibleAxis(ChartType chartType)
        {
            return !IsStructuralChart(chartType) && (chartType != ChartType.PolarChart);
        }

        /// <summary>
        /// �������� �� � ��������� �������� ������ ���
        /// </summary>
        public static bool IsAvailableAxisExtent(UltraChart chart)
        {
            return (chart == null) ? false : IsAvailableAxisExtent(chart.ChartType);
        }

        /// <summary>
        /// �������� �� � ���� ��������� �������� ������ ���
        /// </summary>
        public static bool IsAvailableAxisExtent(ChartType chartType)
        {
            return IsAvaibleX2andY2Axis(chartType);
        }

        /// <summary>
        /// �������� �� ��� ��������� ��� X2 � Y2
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static bool IsAvaibleX2andY2Axis(UltraChart chart)
        {
            return (chart == null) ? false : IsAvaibleX2andY2Axis(chart.ChartType);
        }

        /// <summary>
        /// �������� �� ��� ��������� ��� X2 � Y2
        /// </summary>
        /// <param name="chartType"></param>
        /// <returns></returns>
        public static bool IsAvaibleX2andY2Axis(ChartType chartType)
        {
            return !Is3DChart(chartType) && !IsStructuralChart(chartType);
        }

        /// <summary>
        /// �������� �� ��� ��������� ��� Z
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static bool IsAvaibleZAxis(UltraChart chart)
        {
            return (chart == null) ? false : IsAvaibleZAxis(chart.ChartType);
        }
        
        /// <summary>
        /// �������� �� ��� ��������� ��� Z
        /// </summary>
        /// <param name="chartType"></param>
        /// <returns></returns>
        public static bool IsAvaibleZAxis(ChartType chartType)
        {
            return Is3DChart(chartType) && !IsStructuralChart(chartType);
        }

        /// <summary>
        /// �������� �� ��������� ����������
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static bool IsStructuralChart(UltraChart chart)
        {
            return (chart == null) ? false : IsStructuralChart(chart.ChartType);
        }

        /// <summary>
        /// �������� �� ��������� ����������
        /// </summary>
        /// <param name="chartType"></param>
        /// <returns></returns>
        public static bool IsStructuralChart(ChartType chartType)
        {
            return (chartType == ChartType.ConeChart3D || chartType == ChartType.DoughnutChart3D 
                || chartType == ChartType.DoughnutChart || chartType == ChartType.FunnelChart3D
                || chartType == ChartType.FunnelChart || chartType == ChartType.PieChart3D
                || chartType == ChartType.PieChart || chartType == ChartType.PyramidChart3D
                || chartType == ChartType.PyramidChart);
        }

        /// <summary>
        /// �������� �� ��� ��� ��������� ���������
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="axisNumber"></param>
        /// <returns></returns>
        public static bool IsAvaibleVisibleAxis(ChartType chartType, AxisNumber axisNumber)
        {
            if (chartType == ChartType.RadarChart)
            {
                return axisNumber == AxisNumber.Y_Axis;
            }
            else
            {
                return !(chartType == ChartType.BubbleChart3D || chartType == ChartType.PointChart3D) && 
                    IsAvaibleAxis(chartType);
            }
        }

        /// <summary>
        /// �������� �� ��������� ����� ���
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="axisNumber"></param>
        /// <returns></returns>
        public static bool IsAvaibleLabelAxis(ChartType chartType, AxisNumber axisNumber)
        {
            return !((((chartType == ChartType.StackColumnChart)) &&
                                    (axisNumber == AxisNumber.X_Axis || axisNumber == AxisNumber.X2_Axis)) ||
                       (chartType == ChartType.StackBarChart) &&
                                    (axisNumber == AxisNumber.Y_Axis || axisNumber == AxisNumber.Y2_Axis));
        }

        /// <summary>
        /// �������� �� ��������� ����� ����� ���
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="axisNumber"></param>
        /// <returns></returns>
        public static bool IsAvaibleSeriesLabelAxis(ChartType chartType, AxisNumber axisNumber)
        {
            return   (chartType == ChartType.ColumnLineChart &&
                             axisNumber == AxisNumber.X_Axis) ||
                     ((chartType == ChartType.ColumnChart ||
                      chartType == ChartType.StackColumnChart) &&
                            (axisNumber == AxisNumber.X_Axis || axisNumber == AxisNumber.X2_Axis)) ||
                     (chartType == ChartType.StackBarChart &&
                            (axisNumber == AxisNumber.Y_Axis || axisNumber == AxisNumber.Y2_Axis)) ||
                     (chartType == ChartType.BarChart) &&
                            (axisNumber == AxisNumber.Y_Axis || axisNumber == AxisNumber.Y2_Axis);
        }

        /// <summary>
        /// �������� �� ��������� ������������ ����� ���
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="axisNumber"></param>
        /// <returns></returns>
        public static bool IsAvaibleAlignDescriptionLabelAxis(ChartType chartType, AxisNumber axisNumber)
        {
            return !(IsAvaibleAxis(chartType) && 
                    (axisNumber == AxisNumber.X_Axis || axisNumber == AxisNumber.X2_Axis));
        }

        /// <summary>
        /// �������� �� ��������� ��������������� ������������ ��� ����� ����� ���
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="axisNumber"></param>
        /// <returns></returns>
        public static bool IsAvaibleHAlingmentSeriesLabelAxis(ChartType chartType, AxisNumber axisNumber)
        {
            return !(chartType == ChartType.StackColumnChart && 
                     (axisNumber == AxisNumber.X_Axis || axisNumber == AxisNumber.X2_Axis));
        }

        /// <summary>
        /// �������� �� ��������� ������������� ������������ ��� ����� ����� ���
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="axisNumber"></param>
        /// <returns></returns>
        public static bool IsAvaibleVAlingmentSeriesLabelAxis(ChartType chartType, AxisNumber axisNumber)
        {
            return !((chartType == ChartType.StackColumnChart && axisNumber == AxisNumber.X2_Axis) ||
                    (chartType == ChartType.StackBarChart &&
                     (axisNumber == AxisNumber.Y_Axis || axisNumber == AxisNumber.Y2_Axis)));
        }

        /// <summary>
        /// �������� �� ��������� �������� � ������ ���������
        /// </summary>
        public static bool IsAvaibleScrollScaleAxis(ChartType chartType, AxisNumber axisNumber)
        {
            if (chartType == ChartType.RadarChart || chartType == ChartType.PolarChart)
            {
                return axisNumber == AxisNumber.Y_Axis;
            }
            else
            {
                return IsAvaibleAxis(chartType) && !Is3DChart(chartType);
            }
        }

        /// <summary>
        /// �������� �� ��������� ������� ����� ��������� ����
        /// </summary>
        public static bool IsAvaibleLabelsPadding(ChartType chartType)
        {
            return !Is3DChart(chartType) && (chartType != ChartType.RadarChart) && IsAvaibleAxis(chartType);
        }

        /// <summary>
        /// �������� �� ��������� ������� ����� ����� ����
        /// </summary>
        public static bool IsAvaibleSeriesLabelsPadding(ChartType chartType, AxisNumber axisNumber, AxisSeriesLabelAppearance axisSeriesLabelAppearance)
        {
            return IsAvaibleSeriesLabelAxis(chartType, axisNumber) && 
                   (axisSeriesLabelAppearance.Orientation == TextOrientation.Custom ? 
                    (axisSeriesLabelAppearance.OrientationAngle % 90 == 0 ||
                    axisSeriesLabelAppearance.OrientationAngle == 0) : true);
        }
    }
}
