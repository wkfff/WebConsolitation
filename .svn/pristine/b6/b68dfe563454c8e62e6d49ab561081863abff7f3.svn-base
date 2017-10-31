using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Oil_0002_0001_h : CustomReportPage
    {
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraChart1.Width = 472;

            UltraChart1.Height = 250;

            SetupDynamicChart(UltraChart1, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            AddLineAppearencesUltraChart1(UltraChart1);

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

            UltraChart1.DataBind();

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0002_0001_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);

            Label1.Text = string.Format("данные на {0} {1} {2} года", currentDate.Day, CRHelper.RusMonthGenitive(currentDate.Month), currentDate.Year);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0002_0001_chart1_v"));
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


            UltraChart1.Axis.Y.RangeType = AxisRangeType.Automatic;

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

                //UltraChart1.Series.Clear();
                //for (int i = 1; i < dtChart.Columns.Count; i++)
                //{
                //    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                //    series.Label = dtChart.Columns[i].ColumnName;
                //    UltraChart1.Series.Add(series);
                //}
                UltraChart1.DataSource = dtChart;
                UltraChart1.Data.SwapRowsAndColumns = true;
            }
        }


        private void AddLineAppearencesUltraChart1(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Pink;
                            break;
                        }
                    case 4:
                        {
                            color = Color.Cyan;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = color;
                pe.StrokeWidth = 0;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 255;
                pe.FillStopOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
                pe.StrokeWidth = 4;

                LineAppearance lineAppearance2 = new LineAppearance();
                lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.IconAppearance.PE = pe;

                chart.SplineChart.LineAppearances.Add(lineAppearance2);
            }
        }

        private void SetupDynamicChart(UltraChart chart, string tooltipsFormatString, string axisYLabelsFormatString)
        {
            chart.ChartType = ChartType.SplineChart;
            chart.Border.Thickness = 0;

            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br/>на <SERIES_LABEL><br/><b><DATA_VALUE:N2></b>&nbsp;руб.</span>";
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.Axis.X.Extent = 50;
            chart.Axis.X.Labels.Font = new Font("Verdana", 8);
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            chart.Axis.Y.Extent = 40;

            chart.Axis.X.Margin.Near.Value = 1;
            chart.Axis.X.Margin.Far.Value = 1;
            chart.Axis.Y.Margin.Near.Value = 1;
            chart.Axis.Y.Margin.Far.Value = 1;

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

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Top;
            chart.Legend.SpanPercentage = 25;

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
                            point.Series.Label = string.Format("{2}\nна {3}\n<b>{0:N2}</b>{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

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