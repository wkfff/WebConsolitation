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
    public partial class Oil_0008_0002_v : CustomReportPage
    {

        static string shortPeriodFirstYearString = String.Empty;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!UserParams.Mo.Value.Contains("�."))
            {
                UserParams.Mo.Value = String.Format(".[{0} ������������� �����].DataMember", UserParams.Mo.Value);
            }
            else
            {
                UserParams.Mo.Value = String.Format(".[{0}]", UserParams.Mo.Value.Replace("�.", "����� "));
            }

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0008_0002_incomes_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDate.Rows[0][1].ToString();

            shortPeriodFirstYearString = String.Format("{0:dd.MM.yy}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodFirstYear.Value, 3));

            UltraChart1.Width = 750;
            UltraChart2.Width = 750;
            UltraChart3.Width = 750;
            UltraChart4.Width = 750;
            UltraChart5.Width = 750;

            UltraChart1.Height = 250;
            UltraChart2.Height = 250;
            UltraChart3.Height = 250;
            UltraChart4.Height = 250;
            UltraChart5.Height = 250;

            SetupDynamicChart(UltraChart1, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart2, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart3, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart4, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart5, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");

            AddLineAppearencesUltraChart1(UltraChart1, Color.Green);
            AddLineAppearencesUltraChart1(UltraChart2, Color.Gold);
            AddLineAppearencesUltraChart1(UltraChart3, Color.Pink);
            AddLineAppearencesUltraChart1(UltraChart4, Color.Cyan);
            AddLineAppearencesUltraChart1(UltraChart5, Color.Red);

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);
            UltraChart5.DataBinding += new EventHandler(UltraChart5_DataBinding);

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();
            UltraChart5.DataBind();
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0008_0002_chart1_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            if (dtChart.Rows.Count == 0)
            {
                UltraChart1.Visible = false;
                IPadElementHeader1.Visible = false;
                return;
            }

            UltraChart1.Axis.Y.RangeMin = 18;
            UltraChart1.Axis.Y.RangeMax = 30;

            double min = Double.MaxValue;
            double max = Double.MinValue;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double value;
                if (double.TryParse(dtChart.Rows[i][2].ToString(), out value))
                {
                    if (value > max)
                    {
                        max = value;
                    }
                    if (value < min)
                    {
                        min = value;
                    }
                }

            }

            UltraChart1.Axis.Y.RangeMin = min - 1;
            UltraChart1.Axis.Y.RangeMax = max + 1;
            
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
            string query = DataProvider.GetQueryText(String.Format("Oil_0008_0002_chart2_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            if (dtChart.Rows.Count == 0)
            {
                UltraChart2.Visible = false;
                IPadElementHeader2.Visible = false;
                return;
            }

            UltraChart2.Axis.Y.RangeMin = 18;
            UltraChart2.Axis.Y.RangeMax = 30;

            double min = Double.MaxValue;
            double max = Double.MinValue;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double value;
                if (double.TryParse(dtChart.Rows[i][2].ToString(), out value))
                {
                   if (value > max)
                   {
                       max = value;
                   }
                   if (value < min)
                   {
                       min = value;
                   }
                }
                
            }
                        
            UltraChart2.Axis.Y.RangeMin = min - 1;
            UltraChart2.Axis.Y.RangeMax = max + 1;
            
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
            string query = DataProvider.GetQueryText(String.Format("Oil_0008_0002_chart3_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            UltraChart3.Axis.Y.RangeMin = 18;
            UltraChart3.Axis.Y.RangeMax = 30;

            if (dtChart.Rows.Count == 0)
            {
                UltraChart3.Visible = false;
                IPadElementHeader3.Visible = false;
                return;
            }

            double min = Double.MaxValue;
            double max = Double.MinValue;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double value;
                if (double.TryParse(dtChart.Rows[i][2].ToString(), out value))
                {
                    if (value > max)
                    {
                        max = value;
                    }
                    if (value < min)
                    {
                        min = value;
                    }
                }

            }

            UltraChart3.Axis.Y.RangeMin = min - 1;
            UltraChart3.Axis.Y.RangeMax = max + 1;

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
            string query = DataProvider.GetQueryText(String.Format("Oil_0008_0002_chart4_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            if (dtChart.Rows.Count ==0)
            {
                UltraChart4.Visible = false;
                IPadElementHeader4.Visible = false;
                return;
            }

            UltraChart4.Axis.Y.RangeMin = 18;
            UltraChart4.Axis.Y.RangeMax = 30;

            UltraChart4.Axis.Y.RangeType = AxisRangeType.Custom;

            double min = Double.MaxValue;
            double max = Double.MinValue;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double value;
                if (double.TryParse(dtChart.Rows[i][2].ToString(), out value))
                {
                    if (value > max)
                    {
                        max = value;
                    }
                    if (value < min)
                    {
                        min = value;
                    }
                }

            }

            UltraChart4.Axis.Y.RangeMin = min - 1;
            UltraChart4.Axis.Y.RangeMax = max + 1;

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

        void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0008_0002_chart5_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            if (dtChart.Rows.Count == 0)
            {
                UltraChart5.Visible = false;
                IPadElementHeader5.Visible = false;
                return;
            }

            UltraChart5.Axis.Y.RangeType = AxisRangeType.Custom;

            double min = Double.MaxValue;
            double max = Double.MinValue;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double value;
                if (double.TryParse(dtChart.Rows[i][2].ToString(), out value))
                {
                    if (value > max)
                    {
                        max = value;
                    }
                    if (value < min)
                    {
                        min = value;
                    }
                }

            }

            UltraChart5.Axis.Y.RangeMin = min - 1;
            UltraChart5.Axis.Y.RangeMax = max + 1;

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

                UltraChart5.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart5.Series.Add(series);
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
            chart.TitleLeft.Text = "���. �� ����";
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            chart.TitleLeft.Extent = 40;
            chart.TitleLeft.Margins.Top = 0;
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.FontColor = Color.White;

            chart.Axis.X.Margin.Near.Value = 3;
            chart.Axis.X.Margin.Far.Value = 3;

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
                //CRHelper.SaveToUserAgentLog(primitive.ToString());
                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = " ���.";
                            /*if (point.DataPoint != null && point.DataPoint.Label.Contains(shortPeriodFirstYearString))
                            {
                                Ellipse ellipse = new Ellipse();
                                ellipse.DataPoint = point.DataPoint;
                                PaintElement pe = new PaintElement();
                                pe.Fill = Color.Red;
                                ellipse.PE = pe;
                                e.SceneGraph.Add(ellipse);
                            }*/
                            point.DataPoint.Label = String.Format("{2}\n�� {3}\n<b>{0:N2}</b>{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);
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
                if (primitive is Text && !String.IsNullOrEmpty(primitive.Path) && primitive.Path == "Border.Title.Grid.X")
                {
                    Text text = primitive as Text;
                    if (text.GetTextString() == shortPeriodFirstYearString)
                    {
                        LabelStyle style = text.labelStyle.Clone();
                        style.FontColor = Color.Red;
                        text.labelStyle = style;
                    }
                }
            }
        }

    }
}

