using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;


namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class iPad_0001_0002_v : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }
            ImageBasket.ImageUrl = "~/Images/Basket.png";
            SetupChart(UltraChart, 23);
            AddLineAppearencesUltraChart(UltraChart, Color.FromArgb(78, 230, 228), Color.FromArgb(168, 48, 137), Color.FromArgb(170, 70, 53));
            SetupChart(UltraChart1, 23);
            AddLineAppearencesUltraChart(UltraChart1, Color.FromArgb(32, 211, 150), Color.FromArgb(3, 12, 200), Color.FromArgb(174, 150, 157));
            SetupChart(UltraChart2, 30);
            AddLineAppearencesUltraChart(UltraChart2, Color.FromArgb(159, 179, 80), Color.FromArgb(206, 97, 3), Color.FromArgb(129, 12, 157));

            UltraChart.DataBinding += new EventHandler(UltraChart_DataBinding);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);

            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart.LineChart.NullHandling = NullHandling.DontPlot;

            UltraChart.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        void UltraChart_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.DataSource = GetDataSource("iPad_0001_0002_v_chart1");
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart.DataSource = GetDataSource("iPad_0001_0002_v_chart2");
        }

        void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            UltraChart2.DataSource = GetDataSource("iPad_0001_0002_v_chart3");
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
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

        private static DataTable GetDataSource(string queryName)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);
            string year = String.Empty;

            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString() == year)
                {
                    dt.Columns[i].ColumnName = String.Format("{0}", dt.Columns[i].ColumnName);
                }
                else
                {
                    dt.Columns[i].ColumnName = String.Format("{0} - {1}", dt.Rows[0][i], dt.Columns[i].ColumnName);
                    year = dt.Rows[0][i].ToString();
                }

            }
            dt.Rows.RemoveAt(0);
            dt.AcceptChanges();
            return dt;
        }

        private void SetupChart(UltraChart chart, int legendSpanPercentage)
        {
            chart.Width = 740;
            chart.Height = 290;
            chart.ChartType = ChartType.SplineChart;

            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.Axis.X.Extent = 80;
            chart.Axis.Y.Extent = 40;
            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;

            chart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2>";

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.SpanPercentage = legendSpanPercentage;

            chart.Axis.Y.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);

            chart.Axis.X.Labels.Visible = true;
        }

        private void AddLineAppearencesUltraChart(UltraChart chart, Color SeriesColor1, Color SeriesColor2, Color SeriesColor3)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = SeriesColor1;
                            stopColor = SeriesColor1;
                            break;
                        }
                    case 2:
                        {
                            color = SeriesColor2;
                            stopColor = SeriesColor2;
                            break;
                        }
                    
                    case 3:
                        {
                            color = SeriesColor3;
                            stopColor = SeriesColor3;
                            break;
                        }
                }

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.SolidFill;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                pe.StrokeWidth = 4;
                chart.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance2 = new LineAppearance();
                lineAppearance2.IconAppearance.Icon = SymbolIcon.Diamond;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.IconAppearance.PE = pe;
                lineAppearance2.Thickness = 10;

                chart.SplineChart.LineAppearances.Add(lineAppearance2);
            }
        }
    }
}

