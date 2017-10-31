using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Data;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert.Common
{
    public struct CompositeChartUtils
    {
        public static AxisItem GetAxisItem(UltraChart chart, AxisNumber axisNumber)
        {
            foreach (AxisItem axis in chart.CompositeChart.ChartAreas[0].Axes)
            {
                if (axis.axisNumber == axisNumber)
                {
                    return axis;
                }
            }

            return null;
        }

        public static AxisItem GetCompositeAxisX(UltraChart chart, int index)
        {
            if (chart.ChartType == ChartType.Composite && chart.CompositeChart.ChartAreas.Count >0 && chart.CompositeChart.ChartAreas[0].Axes.Count > 3 * index)
            {
                return chart.CompositeChart.ChartAreas[0].Axes[3 * index];
            }
            return null;
        }

        public static AxisItem GetCompositeAxisY(UltraChart chart, int index)
        {
            if (chart.ChartType == ChartType.Composite && chart.CompositeChart.ChartAreas.Count > 0 && chart.CompositeChart.ChartAreas[0].Axes.Count > 3 * index + 1)
            {
                return chart.CompositeChart.ChartAreas[0].Axes[3 * index + 1];
            }
            return null;
        }

        public static AxisItem GetCompositeAxisY2(UltraChart chart, int index)
        {
            if (chart.ChartType == ChartType.Composite && chart.CompositeChart.ChartAreas.Count > 0 && chart.CompositeChart.ChartAreas[0].Axes.Count > 3 * index + 2)
            {
                return chart.CompositeChart.ChartAreas[0].Axes[3 * index + 2];
            }
            return null;
        }

        public static bool GetVisibleCompositeAxisY(UltraChart chart, int index)
        {
            if (chart.ChartType == ChartType.Composite && chart.CompositeChart.ChartAreas.Count > 0 && chart.CompositeChart.ChartAreas[0].Axes.Count > 3 * index + 1)
            {
                return chart.CompositeChart.ChartAreas[0].Axes[3 * index + 1].Visible;
            }
            return false;
        }

        public static bool GetVisibleCompositeAxisY2(UltraChart chart, int index)
        {
            if (chart.ChartType == ChartType.Composite && chart.CompositeChart.ChartAreas.Count > 0 && chart.CompositeChart.ChartAreas[0].Axes.Count > 3 * index + 2)
            {
                return chart.CompositeChart.ChartAreas[0].Axes[3 * index + 2].Visible;
            }
            return false;
        }

        /// <summary>
        /// Совместим ли тип диаграммы с композитной
        /// </summary>
        /// <param name="chartType">тип диаграммы</param>
        /// <returns>true, если совместим</returns>
        public static bool IsCompositeCompatibleType(ChartType chartType)
        {
            return chartType == ChartType.AreaChart || chartType == ChartType.BarChart || 
                   chartType == ChartType.BubbleChart || chartType == ChartType.CandleChart ||
                   chartType == ChartType.ColumnChart || chartType == ChartType.GanttChart ||
                   chartType == ChartType.LineChart || chartType == ChartType.ParetoChart ||
                   chartType == ChartType.PolarChart || chartType == ChartType.ProbabilityChart ||
                   chartType == ChartType.ScatterChart || chartType == ChartType.SplineChart ||
                   chartType == ChartType.SplineAreaChart || chartType == ChartType.StackAreaChart ||
                   chartType == ChartType.StackBarChart || chartType == ChartType.StackColumnChart ||
                   chartType == ChartType.StackSplineChart || chartType == ChartType.StackLineChart ||
                   chartType == ChartType.StackSplineAreaChart ||  chartType == ChartType.StepAreaChart ||
                   chartType == ChartType.StepLineChart;
        }

        /// <summary>
        /// Диаграммы, для которых нужно инвертировать горизонтальное выравнивание оси Y
        /// </summary>
        /// <param name="chartType">тип диаграммы</param>
        /// <returns>true, если нужно</returns>
        public static bool IsInvertAxisHorizontalAlignment(ChartType chartType)
        {
            return chartType == ChartType.BarChart || chartType == ChartType.StackBarChart ||
                   chartType == ChartType.BubbleChart || chartType == ChartType.ScatterChart ||
                   chartType == ChartType.PolarChart || chartType == ChartType.ProbabilityChart;
        }

        /// <summary>
        /// Является ли диаграммой с накоплением
        /// </summary>
        /// <param name="chartType">тип диаграммы</param>
        /// <returns>true, если является</returns>
        public static bool IsStackChart(ChartType chartType)
        {
            return chartType == ChartType.StackColumnChart || chartType == ChartType.StackBarChart ||
                   chartType == ChartType.StackLineChart || chartType == ChartType.StackSplineChart ||
                   chartType == ChartType.StackAreaChart || chartType == ChartType.StackSplineAreaChart;
        }

        /// <summary>
        /// Совместим ли тип диаграммы с композитной
        /// </summary>
        /// <param name="chartTypeStr">строка типа диаграммы</param>
        /// <returns>true, если совместим</returns>
        public static bool IsCompositeCompatibleType(string chartTypeStr)
        {
            ChartType chartType = (ChartType)Enum.Parse(typeof(ChartType), chartTypeStr);
            return IsCompositeCompatibleType(chartType);
        }

        /// <summary>
        /// Инвертирование выравнивания
        /// </summary>
        /// <param name="alignment">исходное выравнивание</param>
        /// <returns>инвертированное выравнивание</returns>
        public static StringAlignment InvertAlignment(StringAlignment alignment)
        {
            if (alignment == StringAlignment.Near)
            {
                return StringAlignment.Far;
            }
            else
            {
                if (alignment == StringAlignment.Far)
                {
                    return StringAlignment.Near;
                }
                else
                {
                    return StringAlignment.Center;
                }
            }
        }

        #region Получение осей

        public static bool IsNumericAxisX(ChartType chartType)
        {
            return chartType == ChartType.BarChart || chartType == ChartType.BubbleChart ||
                   chartType == ChartType.PolarChart || chartType == ChartType.ProbabilityChart ||
                   chartType == ChartType.ScatterChart || chartType == ChartType.BubbleChart ||
                   chartType == ChartType.StackBarChart;
        }

        public static bool IsNumericAxisY(ChartType chartType)
        {
            return chartType == ChartType.AreaChart || chartType == ChartType.BubbleChart ||
                   chartType == ChartType.CandleChart || chartType == ChartType.ColumnChart ||
                   chartType == ChartType.LineChart || chartType == ChartType.ParetoChart ||
                   chartType == ChartType.PolarChart || chartType == ChartType.ProbabilityChart ||
                   chartType == ChartType.ScatterChart || chartType == ChartType.SplineChart ||
                   chartType == ChartType.SplineAreaChart || chartType == ChartType.StackAreaChart ||
                   chartType == ChartType.StackColumnChart || chartType == ChartType.StackSplineChart ||
                   chartType == ChartType.StackSplineAreaChart || chartType == ChartType.StepAreaChart ||
                   chartType == ChartType.StepLineChart || chartType == ChartType.StackLineChart;
        }

        public static bool IsNumericAxisY2(ChartType chartType)
        {
            return chartType == ChartType.CandleChart || chartType == ChartType.ParetoChart;
        }

        public static bool IsStringAxisX(ChartType chartType)
        {
            return chartType == ChartType.AreaChart || chartType == ChartType.CandleChart ||
                   chartType == ChartType.ColumnChart || chartType == ChartType.LineChart ||
                   chartType == ChartType.ParetoChart || chartType == ChartType.SplineChart ||
                   chartType == ChartType.SplineAreaChart || chartType == ChartType.StackAreaChart ||
                   chartType == ChartType.StackColumnChart || chartType == ChartType.StackSplineChart ||
                   chartType == ChartType.StackSplineAreaChart || chartType == ChartType.StackLineChart;
        }

        public static bool IsStringAxisY(ChartType chartType)
        {
            return chartType == ChartType.BarChart || chartType == ChartType.GanttChart ||
                   chartType == ChartType.StackBarChart;
        }

        public static bool IsTimeAxisX(ChartType chartType)
        {
            return chartType == ChartType.AreaChart || chartType == ChartType.GanttChart ||
                   chartType == ChartType.LineChart || chartType == ChartType.ProbabilityChart ||
                   chartType == ChartType.ScatterChart || chartType == ChartType.SplineChart ||
                   chartType == ChartType.SplineAreaChart || chartType == ChartType.StackAreaChart ||
                   chartType == ChartType.StackSplineChart || chartType == ChartType.StackSplineAreaChart ||
                   chartType == ChartType.StepAreaChart || chartType == ChartType.StepLineChart || 
                   chartType == ChartType.StackLineChart;
        }

        public static bool IsTimeAxisY(ChartType chartType)
        {
            return false;
        }

        public static bool IsContinuousLabelAxisX(ChartType chartType)
        {
            return chartType == ChartType.AreaChart || chartType == ChartType.LineChart ||
                   chartType == ChartType.LineChart || chartType == ChartType.ParetoChart ||
                   chartType == ChartType.SplineChart || chartType == ChartType.SplineAreaChart ||
                   chartType == ChartType.StackAreaChart || chartType == ChartType.StackSplineChart ||
                   chartType == ChartType.StackSplineAreaChart || chartType == ChartType.StackLineChart;
        }

        public static bool IsContinuousLabelAxisY(ChartType chartType)
        {
            return false;
        }

        public static bool IsGroupBySeriesLabelAxisX(ChartType chartType)
        {
            return chartType == ChartType.ColumnChart || chartType == ChartType.StackColumnChart;
        }

        public static bool IsGroupBySeriesLabelAxisY(ChartType chartType)
        {
            return chartType == ChartType.BarChart || chartType == ChartType.StackBarChart;
        }

        private static AxisItem GetAxis(AxisAppearance axisAppearance)
        {
            AxisItem axisItem = new AxisItem();

            // для полярной диаграммы скрываем оси
//            if (axisAppearance.ChartComponent.ChartType == ChartType.PolarChart)
//            {
//                axisItem.Visible = false;
//            }
//            else
//            {
//                axisItem.Visible = axisAppearance.Visible;
//            }

            axisItem.Margin.Far.MarginType = axisAppearance.Margin.Far.MarginType;
            axisItem.Margin.Near.MarginType = axisAppearance.Margin.Near.MarginType;
            axisItem.Margin.Far.Value = axisAppearance.Margin.Far.Value;
            axisItem.Margin.Near.Value = axisAppearance.Margin.Near.Value;

            axisItem.Visible = axisAppearance.Visible;
            axisItem.Labels.ClipText = axisAppearance.Labels.ClipText;
            axisItem.Labels.Flip = axisAppearance.Labels.Flip;
            axisItem.Labels.Font = axisAppearance.Labels.Font;
            axisItem.Labels.FontColor = axisAppearance.Labels.FontColor;
            axisItem.Labels.FontSizeBestFit = axisAppearance.Labels.FontSizeBestFit;

            axisItem.Labels.LabelStyle.HorizontalAlign = axisAppearance.Labels.HorizontalAlign;
            axisItem.Labels.LabelStyle.HorizontalAlign = axisAppearance.Labels.LabelStyle.HorizontalAlign;

            axisItem.Labels.ItemFormat = axisAppearance.Labels.ItemFormat;
            axisItem.Labels.ItemFormatString = axisAppearance.Labels.ItemFormatString;
            axisItem.Labels.Orientation = axisAppearance.Labels.Orientation;
            axisItem.Labels.OrientationAngle = axisAppearance.Labels.OrientationAngle;
            axisItem.Labels.ReverseText = axisAppearance.Labels.ReverseText;
            axisItem.Labels.VerticalAlign = axisAppearance.Labels.VerticalAlign;
            axisItem.Labels.WrapText = axisAppearance.Labels.WrapText;
            axisItem.Labels.Visible = axisAppearance.Labels.Visible;
            axisItem.Labels.Layout.Padding = axisAppearance.Labels.Layout.Padding;
            // копируем вручную все свойства оси, кроме Behavior
            
            
            axisItem.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            /*axisItem.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            StaggerAxisLabelLayoutBehavior staggerBehavior1 = new StaggerAxisLabelLayoutBehavior();
            FontScalingAxisLabelLayoutBehavior fontScalingBehavior1 = new FontScalingAxisLabelLayoutBehavior();
            fontScalingBehavior1.EnableRollback = false;
            axisItem.Labels.Layout.BehaviorCollection.Add(staggerBehavior1);
            axisItem.Labels.Layout.BehaviorCollection.Add(fontScalingBehavior1);*/
            
            axisItem.LineColor = axisAppearance.LineColor;
            axisItem.LineDrawStyle = axisAppearance.LineDrawStyle;
            axisItem.LineEndCapStyle = axisAppearance.LineEndCapStyle;
            axisItem.LineThickness = axisAppearance.LineThickness;
            axisItem.LogBase = axisAppearance.LogBase;
            axisItem.LogZero = axisAppearance.LogZero;
            axisItem.MajorGridLines = axisAppearance.MajorGridLines;
            axisItem.Margin = axisAppearance.Margin;
            axisItem.MinorGridLines = axisAppearance.MinorGridLines;
            axisItem.RangeMax = axisAppearance.RangeMax;
            axisItem.RangeMin = axisAppearance.RangeMin;
            axisItem.RangeType = axisAppearance.RangeType;
            axisItem.ScrollScale = axisAppearance.ScrollScale;
            axisItem.StripLines = axisAppearance.StripLines;
            axisItem.TickmarkInterval = axisAppearance.TickmarkInterval;
            axisItem.TickmarkIntervalType = axisAppearance.TickmarkIntervalType;
            axisItem.TickmarkPercentage = axisAppearance.TickmarkPercentage;
            axisItem.TickmarkStyle = axisAppearance.TickmarkStyle;
            axisItem.TimeAxisStyle = axisAppearance.TimeAxisStyle;
            axisItem.Labels.LabelStyle.ClipText = axisAppearance.Labels.LabelStyle.ClipText;
            axisItem.Labels.LabelStyle.Dx = axisAppearance.Labels.LabelStyle.Dx;
            axisItem.Labels.LabelStyle.Dy = axisAppearance.Labels.LabelStyle.Dy;
            axisItem.Labels.LabelStyle.Flip = axisAppearance.Labels.LabelStyle.Flip;
            axisItem.Labels.LabelStyle.Font = axisAppearance.Labels.LabelStyle.Font;
            axisItem.Labels.LabelStyle.FontColor = axisAppearance.Labels.LabelStyle.FontColor;
            axisItem.Labels.LabelStyle.FontSizeBestFit = axisAppearance.Labels.LabelStyle.FontSizeBestFit;
            //axisItem.Labels.LabelStyle.HorizontalAlign = StringAlignment.Near;
            //axisItem.Labels.LabelStyle.HorizontalAlign = axisAppearance.Labels.LabelStyle.HorizontalAlign;
            axisItem.Labels.LabelStyle.Orientation = axisAppearance.Labels.LabelStyle.Orientation;
            axisItem.Labels.LabelStyle.ReverseText = axisAppearance.Labels.LabelStyle.ReverseText;
            axisItem.Labels.LabelStyle.RotationAngle = axisAppearance.Labels.LabelStyle.RotationAngle;
            axisItem.Labels.LabelStyle.Trimming = axisAppearance.Labels.LabelStyle.Trimming;
            axisItem.Labels.LabelStyle.VerticalAlign = axisAppearance.Labels.LabelStyle.VerticalAlign;
            axisItem.Labels.LabelStyle.WrapText = axisAppearance.Labels.LabelStyle.WrapText;

            axisItem.Labels.SeriesLabels.ClipText = axisAppearance.Labels.SeriesLabels.ClipText;
            axisItem.Labels.SeriesLabels.Flip = axisAppearance.Labels.SeriesLabels.Flip;
            axisItem.Labels.SeriesLabels.Font = axisAppearance.Labels.SeriesLabels.Font;
            axisItem.Labels.SeriesLabels.FontColor = axisAppearance.Labels.SeriesLabels.FontColor;
            axisItem.Labels.SeriesLabels.FontSizeBestFit = axisAppearance.Labels.SeriesLabels.FontSizeBestFit;
            axisItem.Labels.SeriesLabels.Format = axisAppearance.Labels.SeriesLabels.Format;
            axisItem.Labels.SeriesLabels.FormatString = axisAppearance.Labels.SeriesLabels.FormatString;
            axisItem.Labels.SeriesLabels.HorizontalAlign = axisAppearance.Labels.SeriesLabels.HorizontalAlign;
            
            axisItem.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            StaggerAxisLabelLayoutBehavior staggerBehavior = new StaggerAxisLabelLayoutBehavior();
            FontScalingAxisLabelLayoutBehavior fontScalingBehavior = new FontScalingAxisLabelLayoutBehavior();
            fontScalingBehavior.EnableRollback = false;
            WrapTextAxisLabelLayoutBehavior wrapTextBehavior = new WrapTextAxisLabelLayoutBehavior();
            wrapTextBehavior.EnableRollback = false;
            axisItem.Labels.SeriesLabels.Layout.BehaviorCollection.Add(fontScalingBehavior);
            axisItem.Labels.SeriesLabels.Layout.BehaviorCollection.Add(staggerBehavior);
            axisItem.Labels.SeriesLabels.Layout.BehaviorCollection.Add(wrapTextBehavior);
            
            //axisItem.Labels.SeriesLabels.Layout.Behavior = axisAppearance.Labels.SeriesLabels.Layout.Behavior;
            
            axisItem.Labels.SeriesLabels.Layout.Padding = axisAppearance.Labels.SeriesLabels.Layout.Padding;
            axisItem.Labels.SeriesLabels.Orientation = axisAppearance.Labels.SeriesLabels.Orientation;
            axisItem.Labels.SeriesLabels.OrientationAngle = axisAppearance.Labels.SeriesLabels.OrientationAngle;
            axisItem.Labels.SeriesLabels.ReverseText = axisAppearance.Labels.SeriesLabels.ReverseText;
            axisItem.Labels.SeriesLabels.VerticalAlign = axisAppearance.Labels.SeriesLabels.VerticalAlign;
            axisItem.Labels.SeriesLabels.Visible = axisAppearance.Labels.SeriesLabels.Visible;
            axisItem.Labels.SeriesLabels.WrapText = false;
            axisItem.Labels.SeriesLabels.LabelStyle.ClipText = axisAppearance.Labels.SeriesLabels.LabelStyle.ClipText;
            axisItem.Labels.SeriesLabels.LabelStyle.Dx = axisAppearance.Labels.SeriesLabels.LabelStyle.Dx;
            axisItem.Labels.SeriesLabels.LabelStyle.Dy = axisAppearance.Labels.SeriesLabels.LabelStyle.Dy;
            axisItem.Labels.SeriesLabels.LabelStyle.Flip = axisAppearance.Labels.SeriesLabels.LabelStyle.Flip;
            axisItem.Labels.SeriesLabels.LabelStyle.Font = axisAppearance.Labels.SeriesLabels.LabelStyle.Font;
            axisItem.Labels.SeriesLabels.LabelStyle.FontColor = axisAppearance.Labels.SeriesLabels.LabelStyle.FontColor;
            axisItem.Labels.SeriesLabels.LabelStyle.FontSizeBestFit = axisAppearance.Labels.SeriesLabels.LabelStyle.FontSizeBestFit;
            axisItem.Labels.SeriesLabels.LabelStyle.HorizontalAlign = axisAppearance.Labels.SeriesLabels.LabelStyle.HorizontalAlign;
            axisItem.Labels.SeriesLabels.LabelStyle.Orientation = axisAppearance.Labels.SeriesLabels.LabelStyle.Orientation;
            axisItem.Labels.SeriesLabels.LabelStyle.ReverseText = axisAppearance.Labels.SeriesLabels.LabelStyle.ReverseText;
            axisItem.Labels.SeriesLabels.LabelStyle.RotationAngle = axisAppearance.Labels.SeriesLabels.LabelStyle.RotationAngle;
            axisItem.Labels.SeriesLabels.LabelStyle.Trimming = axisAppearance.Labels.SeriesLabels.LabelStyle.Trimming;
            axisItem.Labels.SeriesLabels.LabelStyle.VerticalAlign = axisAppearance.Labels.SeriesLabels.LabelStyle.VerticalAlign;
            axisItem.Labels.SeriesLabels.LabelStyle.WrapText = false;

            return axisItem;

        }

        public static AxisItem GetAxisX(AxisAppearance axisAppearance)
        {
            AxisItem axisItem = GetAxis(axisAppearance);
            axisItem.OrientationType = AxisNumber.X_Axis;
            
            if (IsNumericAxisX(axisAppearance.ChartComponent.ChartType))
            {
                axisItem.DataType = AxisDataType.Numeric;
                //axisItem.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            }
            else
            {
                if (IsStringAxisX(axisAppearance.ChartComponent.ChartType))
                {
                    axisItem.DataType = AxisDataType.String;
                    //axisItem.Labels.ItemFormat = AxisItemLabelFormat.ItemLabel;
                }
                else
                {
                    axisItem.DataType = AxisDataType.Time;
                    //axisItem.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
                }
            }

            if (IsGroupBySeriesLabelAxisX(axisAppearance.ChartComponent.ChartType))
            {
                axisItem.SetLabelAxisType = SetLabelAxisType.GroupBySeries;
            }
            else
            {
                if (IsContinuousLabelAxisX(axisAppearance.ChartComponent.ChartType))
                {
                    axisItem.SetLabelAxisType = SetLabelAxisType.ContinuousData;
                }
                else
                {
                    axisItem.SetLabelAxisType = SetLabelAxisType.DateData;
                }
            }

            return axisItem;
        }

        public static AxisItem GetAxisY(AxisAppearance axisAppearance)
        {
            AxisItem axisItem = GetAxis(axisAppearance);
            axisItem.OrientationType = AxisNumber.Y_Axis;

            if (IsNumericAxisY(axisAppearance.ChartComponent.ChartType))
            {
                axisItem.DataType = AxisDataType.Numeric;
                //axisItem.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            }
            else
            {
                if (IsStringAxisY(axisAppearance.ChartComponent.ChartType))
                {
                    axisItem.DataType = AxisDataType.String;
                    //axisItem.Labels.ItemFormat = AxisItemLabelFormat.ItemLabel;
                }
                else
                {
                    axisItem.DataType = AxisDataType.Time;
                    //axisItem.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
                }
            }

            if (IsGroupBySeriesLabelAxisY(axisAppearance.ChartComponent.ChartType))
            {
                axisItem.SetLabelAxisType = SetLabelAxisType.GroupBySeries;
            }
            else
            {
                if (IsContinuousLabelAxisY(axisAppearance.ChartComponent.ChartType))
                {
                    axisItem.SetLabelAxisType = SetLabelAxisType.ContinuousData;
                }
                else
                {
                    axisItem.SetLabelAxisType = SetLabelAxisType.DateData;
                }
            }

            return axisItem;
        }

        public static AxisItem GetAxisX2(AxisAppearance axisAppearance)
        {
            AxisItem axisItem = GetAxisX(axisAppearance);
            axisItem.OrientationType = AxisNumber.X2_Axis;
            return axisItem;
        }

        public static AxisItem GetAxisY2(AxisAppearance axisAppearance)
        {
            AxisItem axisItem = GetAxisY(axisAppearance);
            axisItem.OrientationType = AxisNumber.Y2_Axis;
            return axisItem;
        }

        #endregion

        #region Получение серий

        public static bool IsNumericSeries(ChartType chartType)
        {
            return chartType == ChartType.ColumnChart || chartType == ChartType.StackColumnChart ||
                   chartType == ChartType.BarChart || chartType == ChartType.StackBarChart ||
                   chartType == ChartType.LineChart || chartType == ChartType.SplineChart ||
                   chartType == ChartType.AreaChart || chartType == ChartType.SplineAreaChart ||
                   chartType == ChartType.StackLineChart || chartType == ChartType.StackAreaChart ||
                   chartType == ChartType.StackSplineChart || chartType == ChartType.StackSplineAreaChart || 
                   chartType == ChartType.ParetoChart;
        }

        public static bool IsXYSeries(ChartType chartType)
        {
            return chartType == ChartType.ScatterChart || chartType == ChartType.PolarChart ||
                   chartType == ChartType.ProbabilityChart;
        }

        public static bool IsXYZSeries(ChartType chartType)
        {
            return chartType == ChartType.BubbleChart;
        }

        public static bool IsTimeSeries(ChartType chartType)
        {
            return chartType == ChartType.StepLineChart || chartType == ChartType.StepAreaChart;
        }

        public static bool IsCandleSeries(ChartType chartType)
        {
            return chartType == ChartType.CandleChart;
        }

        public static bool IsGanttSeries(ChartType chartType)
        {
            return chartType == ChartType.GanttChart;
        }

        public static bool IsBoxSeries(ChartType chartType)
        {
            return chartType == ChartType.BoxChart;
        }

        public static SeriesBase GetSeries(int index, string name, UltraChart chart, object dataSource)
        {
            ChartType chartType = chart.ChartType;
            if (dataSource == null)
            {
                return null;
            }

            try
            {
                if (IsNumericSeries(chartType))
                {
                    return GetNumericSeries(index, dataSource);
                }
                if (IsXYSeries(chartType))
                {
                    int indexX = 1;
                    int indexY = 2;
                    switch (chartType)
                    {
                        case ChartType.ProbabilityChart:
                            {
                                indexX = chart.ProbabilityChart.ColumnX;
                                indexY = chart.ProbabilityChart.ColumnY;
                                break;
                            }
                        case ChartType.PolarChart:
                            {
                                indexX = chart.PolarChart.ColumnX;
                                indexY = chart.PolarChart.ColumnY;
                                break;
                            }
                        case ChartType.ScatterChart:
                            {
                                indexX = chart.ScatterChart.ColumnX;
                                indexY = chart.ScatterChart.ColumnY;
                                break;
                            }
                    }

                    return GetXYSeries(indexX, indexY, dataSource);
                }
                if (IsXYZSeries(chartType))
                {
                    return GetXYZSeries(chart.BubbleChart.ColumnX, 
                                        chart.BubbleChart.ColumnY, 
                                        chart.BubbleChart.ColumnZ, 
                                        dataSource);
                }
                if (IsTimeSeries(chartType))
                {
                    return GetTimeSeries(0, 1, dataSource);
                }
                if (IsCandleSeries(chartType))
                {
                    return GetCandleSeries(name, dataSource);
                }
                if (IsBoxSeries(chartType))
                {
                    return GetBoxSeries(name, dataSource);
                }
                if (IsGanttSeries(chartType))
                {
                    return GetGanttSeries(chart.GanttChart.Columns.ItemLabelsColumnIndex, 
                                          chart.GanttChart.Columns.OwnerColumnIndex,
                                          chart.GanttChart.Columns.LinkToColumnIndex,
                                          chart.GanttChart.Columns.PercentCompleteColumnIndex,
                                          chart.GanttChart.Columns.StartTimeColumnIndex,
                                          chart.GanttChart.Columns.EndTimeColumnIndex,
                                          chart.GanttChart.Columns.IDColumnIndex,
                                          dataSource);
                }
            }
            catch (Exception e)
            {
                return null;                 
            }
            return null;  
        }

        public static NumericSeries GetNumericSeries(int index, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            NumericSeries numericSeries = new NumericSeries();
            numericSeries.Label = dataTable.Columns[index].ColumnName;
            

            if (dataTable.Columns[0].ColumnName == "Series Name")
            {
                numericSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            }

            numericSeries.Data.ValueColumn = dataTable.Columns[index].ColumnName;

            numericSeries.Data.DataSource = dataTable;
            numericSeries.DataBind();

            return numericSeries;
        }

        public static XYSeries GetXYSeries(int indexX, int indexY, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            XYSeries xySeries = new XYSeries();
            //xySeries.Label = name;
            xySeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            xySeries.Data.ValueXColumn = dataTable.Columns[indexX].ColumnName;
            xySeries.Data.ValueYColumn = dataTable.Columns[indexY].ColumnName;

            xySeries.Data.DataSource = dataSource;
            xySeries.DataBind();

            return xySeries;
        }

        public static XYZSeries GetXYZSeries(int indexX, int indexY, int indexZ, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            XYZSeries xyzSeries = new XYZSeries();
            //xyzSeries.Label = name;
            xyzSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            xyzSeries.Data.ValueXColumn = dataTable.Columns[indexX].ColumnName;
            xyzSeries.Data.ValueYColumn = dataTable.Columns[indexY].ColumnName;
            xyzSeries.Data.ValueZColumn = dataTable.Columns[indexZ].ColumnName;

            xyzSeries.Data.DataSource = dataSource;
            xyzSeries.DataBind();

            return xyzSeries;
        }

        public static NumericTimeSeries GetTimeSeries(int timeIndex, int valueIndex, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            NumericTimeSeries timeSeries = new NumericTimeSeries();
            //timeSeries.Label = name;
            timeSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            timeSeries.Data.TimeValueColumn = dataTable.Columns[timeIndex].ColumnName;
            timeSeries.Data.ValueColumn = dataTable.Columns[valueIndex].ColumnName;

            timeSeries.Data.DataSource = dataSource;
            timeSeries.DataBind();

            return timeSeries;
        }

        public static CandleSeries GetCandleSeries(string name, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            CandleSeries candleSeries = new CandleSeries();
            candleSeries.Label = name;
            candleSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            candleSeries.Data.DateColumn = dataTable.Columns[1].ColumnName;
            candleSeries.Data.OpenColumn = dataTable.Columns[2].ColumnName;
            candleSeries.Data.CloseColumn = dataTable.Columns[3].ColumnName;
            candleSeries.Data.LowColumn = dataTable.Columns[4].ColumnName;
            candleSeries.Data.HighColumn = dataTable.Columns[5].ColumnName;
            candleSeries.Data.VolumeColumn = dataTable.Columns[6].ColumnName;

            candleSeries.Data.DataSource = dataSource;
            candleSeries.DataBind();

            return candleSeries;
        }

        public static GanttSeries GetGanttSeries(int labelIndex, int ownerIndex, int linkToIDIndex, 
                                                 int PerserntCompleteIndex, int StartTimeIndex, int EndTimeIndex,
                                                 int TimeEntryIDIndex, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            GanttSeries ganttSeries = new GanttSeries();
            //ganttSeries.Label = name;
            ganttSeries.Data.LabelColumn = dataTable.Columns[labelIndex].ColumnName;
            ganttSeries.Data.OwnerColumn = dataTable.Columns[ownerIndex].ColumnName;
            ganttSeries.Data.LinkToIDColumn = dataTable.Columns[linkToIDIndex].ColumnName;
            ganttSeries.Data.PercentCompleteColumn = dataTable.Columns[PerserntCompleteIndex].ColumnName;
            ganttSeries.Data.StartTimeColumn = dataTable.Columns[StartTimeIndex].ColumnName;
            ganttSeries.Data.EndTimeColumn = dataTable.Columns[EndTimeIndex].ColumnName;
            ganttSeries.Data.TimeEntryIDColumn = dataTable.Columns[TimeEntryIDIndex].ColumnName;

            ganttSeries.Data.DataSource = dataSource;
            ganttSeries.DataBind();

            return ganttSeries;
        }

        public static BoxSetSeries GetBoxSeries(string name, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            BoxSetSeries boxSeries = new BoxSetSeries();
            boxSeries.Label = name;
            boxSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            boxSeries.Data.MinColumn = dataTable.Columns[1].ColumnName;
            boxSeries.Data.MaxColumn = dataTable.Columns[2].ColumnName;
            boxSeries.Data.Q1Column = dataTable.Columns[3].ColumnName;
            boxSeries.Data.Q2Column = dataTable.Columns[5].ColumnName;
            boxSeries.Data.Q3Column = dataTable.Columns[6].ColumnName;

            boxSeries.Data.DataSource = dataSource;
            boxSeries.DataBind();

            return boxSeries;
        }

        #endregion

        #region Получение слоев

        public static ChartLayerAppearance GetLayerAppearance(UltraChart chart, string key)
        {
            ChartLayerAppearance chartLayerAppearance = new ChartLayerAppearance();
            chartLayerAppearance.Key = key;
            chartLayerAppearance.ChartType = chart.ChartType;

            switch (chart.ChartType)
            {
                case ChartType.AreaChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.AreaChart;
                        break;
                    }
                case ChartType.BarChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.BarChart;
                        break;
                    }
                case ChartType.BubbleChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.BubbleChart;
                        break;
                    }
                case ChartType.CandleChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.CandleChart;
                        break;
                    }
                case ChartType.ColumnChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.ColumnChart;
                        break;
                    }
                case ChartType.GanttChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.GanttChart;
                        break;
                    }
                case ChartType.LineChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.LineChart;
                        break;
                    }
                case ChartType.ParetoChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.ParetoChart;
                        break;
                    }
                case ChartType.PolarChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.PolarChart;
                        break;
                    }
                case ChartType.ProbabilityChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.ProbabilityChart;
                        break;
                    }
                case ChartType.ScatterChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.ScatterChart;
                        break;
                    }
                case ChartType.SplineChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.SplineChart;
                        break;
                    }
                case ChartType.SplineAreaChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.SplineAreaChart;
                        break;
                    }
                case ChartType.StackAreaChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.AreaChart;
                        break;
                    }
                case ChartType.StackBarChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.BarChart;
                        break;
                    }
                case ChartType.StackColumnChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.ColumnChart;
                        break;
                    }
                case ChartType.StackLineChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.LineChart;
                        break;
                    }
                case ChartType.StackSplineChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.SplineChart;
                        break;
                    }
                case ChartType.StackSplineAreaChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.SplineAreaChart;
                        break;
                    }
                case ChartType.StepAreaChart:
                    {
                        //chartLayerAppearance.ChartTypeAppearance = chart.AreaChart;
                        chartLayerAppearance.ChartTypeAppearance = new LineChartAppearance();
                        LineChartAppearance lineChartAppearance = ((LineChartAppearance)(chartLayerAppearance.ChartTypeAppearance));

                        lineChartAppearance.DrawStyle = chart.AreaChart.LineDrawStyle;
                        lineChartAppearance.EndStyle = chart.AreaChart.LineEndCapStyle;
                        lineChartAppearance.StartStyle = chart.AreaChart.LineStartCapStyle;
                        lineChartAppearance.Thickness = chart.AreaChart.LineThickness;
                        lineChartAppearance.MidPointAnchors = chart.AreaChart.MidPointAnchors;
                        lineChartAppearance.NullHandling = chart.AreaChart.NullHandling;
                        lineChartAppearance.TreatDateTimeAsString = chart.AreaChart.TreatDateTimeAsString;
                        break;
                    }
                case ChartType.StepLineChart:
                    {
                        chartLayerAppearance.ChartTypeAppearance = chart.LineChart;
                        break;
                    }
            }

            return chartLayerAppearance;
        }

        #endregion
    }

    // Список ключей диаграмм
    public struct ChartKeyList
    {
        // ключ композитной диаграммы
        public string chartKey;
        // ключи дочерних диаграмм
        public List<string> charts;
    }
}

