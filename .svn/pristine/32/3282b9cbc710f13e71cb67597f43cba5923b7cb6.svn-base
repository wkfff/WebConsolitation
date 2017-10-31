using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Primitive = Infragistics.UltraGauge.Resources.Primitive;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0001_0001_v : CustomReportPage
    {


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraChart1.Width = 750;
            UltraChart2.Width = 750;
            UltraChart3.Width = 750;
            UltraChart4.Width = 750;

            UltraChart1.Height = 250;
            UltraChart2.Height = 250;
            UltraChart3.Height = 250;
            UltraChart4.Height = 250;

            SetupDynamicChart(UltraChart1, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart2, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart3, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart4, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");

            AddLineAppearencesUltraChart1(UltraChart1, Color.Green);
            AddLineAppearencesUltraChart1(UltraChart2, Color.Gold);
            AddLineAppearencesUltraChart1(UltraChart3, Color.Pink);
            AddLineAppearencesUltraChart1(UltraChart4, Color.Cyan);

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0001_0001_chart1_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            UltraChart1.Axis.Y.RangeMin = 18;
            UltraChart1.Axis.Y.RangeMax = 30;

            double value;
            if (double.TryParse(dtChart.Rows[0][2].ToString(), out value))
            {
                UltraChart1.Axis.Y.RangeMin = value - 1;
            }
            if (double.TryParse(dtChart.Rows[dtChart.Rows.Count - 1][2].ToString(), out value))
            {
                UltraChart1.Axis.Y.RangeMax = value + 1;
            }

            
            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart1.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart1.Series.Add(series);
                }
            }
        }

        void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0001_0001_chart2_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            UltraChart2.Axis.Y.RangeMin = 18;
            UltraChart2.Axis.Y.RangeMax = 30;

            double value;
            if (double.TryParse(dtChart.Rows[0][2].ToString(), out value))
            {
                UltraChart2.Axis.Y.RangeMin = value - 1;
            }
            if (double.TryParse(dtChart.Rows[dtChart.Rows.Count - 1][2].ToString(), out value))
            {
                UltraChart2.Axis.Y.RangeMax = value + 1;
            }

            UltraChart2.Axis.Y.RangeType = AxisRangeType.Custom;

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart2.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart2.Series.Add(series);
                }
            }
        }

        void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0001_0001_chart3_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            UltraChart3.Axis.Y.RangeMin = 18;
            UltraChart3.Axis.Y.RangeMax = 30;

            double value;
            if (double.TryParse(dtChart.Rows[0][2].ToString(), out value))
            {
                UltraChart3.Axis.Y.RangeMin = value - 1;
            }
            if (double.TryParse(dtChart.Rows[dtChart.Rows.Count - 1][2].ToString(), out value))
            {
                UltraChart3.Axis.Y.RangeMax = value + 1;
            }

            UltraChart3.Axis.Y.RangeType = AxisRangeType.Custom;

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart3.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart3.Series.Add(series);
                }
            }
        }

        void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0001_0001_chart4_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            UltraChart4.Axis.Y.RangeMin = 18;
            UltraChart4.Axis.Y.RangeMax = 30;

            UltraChart4.Axis.Y.RangeType = AxisRangeType.Custom;

            double value;
            if (double.TryParse(dtChart.Rows[0][2].ToString(), out value))
            {
                UltraChart4.Axis.Y.RangeMin = value - 1;
            }
            if (double.TryParse(dtChart.Rows[dtChart.Rows.Count - 1][2].ToString(), out value))
            {
                UltraChart4.Axis.Y.RangeMax = value + 1;
            }
            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart4.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart4.Series.Add(series);
                }
            }
        }

        private void AddLineAppearencesUltraChart1(UltraChart chart, Color color)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();

            PaintElement pe = new PaintElement();
            
            pe.Fill = color;
            pe.FillStopColor = color;
            pe.StrokeWidth = 0;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = 255;
            pe.FillStopOpacity = 200;
            chart.ColorModel.Skin.PEs.Add(pe);
            pe.Stroke = Color.Black;
            pe.StrokeWidth = 0;

            LineAppearance lineAppearance2 = new LineAppearance();
            lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance2.IconAppearance.PE = pe;

            chart.AreaChart.LineAppearances.Add(lineAppearance2);
        }

        private void SetupDynamicChart(UltraChart chart, string tooltipsFormatString, string axisYLabelsFormatString)
        {
            chart.ChartType = ChartType.AreaChart;
            chart.Border.Thickness = 0;
            
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL></span>";
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.Axis.X.Extent = 50;
            chart.Axis.X.Labels.Font = new Font("Verdana", 8);
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            chart.Axis.Y.Extent = 40;

            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = "руб. за литр";
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            chart.TitleLeft.Extent = 40;
            chart.TitleLeft.Margins.Top = 0;
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.FontColor = Color.White;

            chart.ColorModel.ModelStyle = ColorModels.CustomLinear;

            chart.Data.EmptyStyle.Text = " ";
            chart.EmptyChartText = " ";

            chart.AreaChart.NullHandling = NullHandling.Zero;
            
            chart.Legend.Visible = false;

            chart.InvalidDataReceived +=
                new ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            chart.FillSceneGraph += new FillSceneGraphEventHandler(chart_FillSceneGraph);
        }

        void chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = " руб.";
                            point.DataPoint.Label = string.Format("{2}\nна {3}\n<b>{0:N2}</b>{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                }
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                    }
                }
            }
        }

    }
}

