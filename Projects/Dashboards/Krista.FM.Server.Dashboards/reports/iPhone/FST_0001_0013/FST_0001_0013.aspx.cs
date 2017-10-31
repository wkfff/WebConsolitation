using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Xml;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0013 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string measureName = "���./���.�.";

            UltraChartGrownTemp.Width = 750;
            UltraChartGrownTemp.Height = 300;

            UltraChartGrownTemp.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartGrownTemp_FillSceneGraph);

            SetUpAreaChart(UltraChartGrownTemp, measureName);

            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText("FST_0001_0013_Chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                UltraChartGrownTemp.DataSource = dtChart;
                UltraChartGrownTemp.DataBind();
            }
            else
            {
                UltraChartGrownTemp.Visible = false;
                IPadElementHeader3.Visible = false;
            }

            TagCloudFST_0001_0013_Text2.TaxName = "����� ��� ���������";

            
            SetChart(TagCloudFST_0001_0013_Text2, "2", measureName);
        }

        void UltraChartGrownTemp_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
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

        private void SetUpAreaChart(UltraChart chart, string measureName)
        {
            chart.ChartType = ChartType.ColumnChart;
            chart.Data.ZeroAligned = true;
            chart.ColumnChart.NullHandling = NullHandling.DontPlot;
            chart.Tooltips.FormatString = String.Format(
                    "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL><br /><ITEM_LABEL> ���<br /><b><DATA_VALUE:N2></b>&nbsp;{0}</span>", measureName);
            chart.Legend.SpanPercentage = 25;

            chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Far.Value = 10;
            chart.Axis.X2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Far.Value = 10;

            chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Near.Value = 20;
            chart.Axis.X2.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Near.Value = 20;

            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;
            chart.Axis.Y2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y2.Margin.Far.Value = 10;
            
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            chart.ColumnChart.SeriesSpacing = 1;
                       
            chart.Data.SwapRowsAndColumns = true;
            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dash;
            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;

            chart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.SeriesLabels.Visible = true;

            chart.Axis.X.Labels.Font = new Font("Verdana", 10);
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            chart.Axis.Y.Extent = 60;
            chart.Axis.X.Extent = 20;

            chart.Legend.Font = new Font("Verdana", 10);

            AddLineAppearencesUltraChart(chart);

            SetupTitleLeft(chart, measureName, 0);
        }

        private void AddLineAppearencesUltraChart(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();
            
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.FromArgb(99, 163, 10);
                            stopColor = Color.FromArgb(65, 100, 18);
                            break;
                        }
                    case 2:
                        {
                            color = Color.FromArgb(0, 99, 166);
                            stopColor = Color.FromArgb(4, 68, 112);
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
            }

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.PositionFromRadius = 10;
            appearance.ItemFormatString = "<DATA_VALUE:N2>";
            appearance.ChartTextFont = new Font("Verdana", 10);
            appearance.FontColor = Color.White; //Color.FromArgb(192, 192, 192);
            appearance.Visible = true;
            chart.ColumnChart.ChartText.Add(appearance);
        }

        private static void SetupTitleLeft(UltraChart chart, string title, int marginBottom)
        {
            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = title;
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.WrapText = true;
            chart.TitleLeft.FontColor = Color.White;
            chart.TitleLeft.VerticalAlign = StringAlignment.Center;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Margins.Bottom = marginBottom;
        }

        private void SetChart(Dashboard.FST_0001_0003_Text textElement, string filter, string measure)
        {
            CustomParam measureParam = UserParams.CustomParam("measure");
            measureParam.Value = measure;

            string chartQuery = DataProvider.GetQueryText(String.Format("FST_0001_0013_gauge{0}", filter));
            string textQuery = DataProvider.GetQueryText(String.Format("FST_0001_0013_text{0}", filter));            
            
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(chartQuery, "dummy", dtChart);
            DataTable dtText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(textQuery, "dummy", dtText);

            textElement.DtChart = dtChart;
            textElement.DtText = dtText;
        }

        private bool CheckedArea(string name, string fileName)
        {
            // ��������� ��������
            string xmlFile = HttpContext.Current.Server.MapPath(fileName);
            if (!File.Exists(xmlFile))
            {
                return true;
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);
                // ���� ���� ��������
                foreach (XmlNode rootNode in doc.ChildNodes)
                {
                    if (rootNode.Name == "RegionsList")
                    {
                        foreach (XmlNode regionNode in rootNode.ChildNodes)
                        {
                            if (regionNode.Attributes["name"].Value == name)
                            {
                                return true;
                            }
                        }
                    }
                }
            }           
            return false;
        }
    }
}
