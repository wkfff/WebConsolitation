using System;
using System.Data;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Drawing;
using System.Web;
using System.IO;
using System.Xml;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0033_Horizontal : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string measureName = "руб./куб.м.";

            UltraChartGrownTemp.Width = 1010;
            UltraChartGrownTemp.Height = 450;

            UltraChartGrownTemp.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartGrownTemp_FillSceneGraph);

            SetUpAreaChart(UltraChartGrownTemp, measureName);

            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText("FST_0001_0033_Horizontal_Chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            UltraChartGrownTemp.DataSource = dtChart;
            UltraChartGrownTemp.DataBind();

            TagCloudFST_0001_0033_Horizontal_Text2.TaxName = "тариф для населения";
            TagCloudFST_0001_0033_Horizontal_Text3.TaxName = "тариф для бюджетных потребителей";
            TagCloudFST_0001_0033_Horizontal_Text4.TaxName = "тариф для прочих потребителей";

            SetChart(TagCloudFST_0001_0033_Horizontal_Text1, "1", measureName);
            SetChart(TagCloudFST_0001_0033_Horizontal_Text2, "2", measureName);
            SetChart(TagCloudFST_0001_0033_Horizontal_Text3, "3", measureName);
            SetChart(TagCloudFST_0001_0033_Horizontal_Text4, "4", measureName);
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
            chart.ChartType = ChartType.SplineChart;
            chart.Data.ZeroAligned = false;
            chart.Tooltips.FormatString = String.Format(
                    "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br /><SERIES_LABEL> год<br /><b><DATA_VALUE:N2></b>&nbsp;{0}</span>", measureName);
            chart.Legend.SpanPercentage = 18;

            chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Far.Value = 10;
            chart.Axis.X2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Far.Value = 10;

            chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Near.Value = 30;
            chart.Axis.X2.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Near.Value = 30;

            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;
            chart.Axis.Y2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y2.Margin.Far.Value = 10;

            chart.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Near.Value = 10;
            chart.Axis.Y2.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.Y2.Margin.Near.Value = 10;

            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            chart.ColumnChart.SeriesSpacing = 0;

            chart.SplineChart.NullHandling = NullHandling.DontPlot;
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

            SetupTitleLeft(chart, measureName, 20);
        }

        private void AddLineAppearencesUltraChart(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 5; i++)
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
                    case 3:
                        {
                            color = Color.Cyan;
                            stopColor = Color.Cyan;
                            break;
                        }
                    case 4:
                        {
                            color = Color.Moccasin;
                            stopColor = Color.Moccasin;
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
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Large;
                lineAppearance2.IconAppearance.PE = pe;
                lineAppearance2.Thickness = 10;

                chart.SplineChart.LineAppearances.Add(lineAppearance2);
            }

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N2>";
            appearance.ChartTextFont = new Font("Verdana", 10);
            appearance.FontColor = Color.White; //Color.FromArgb(192, 192, 192);
            appearance.Visible = true;
            chart.SplineChart.ChartText.Add(appearance);
        }

        private static void SetupTitleLeft(UltraChart chart, string title, int marginTop)
        {
            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = title;
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.WrapText = true;
            chart.TitleLeft.FontColor = Color.White;
            chart.TitleLeft.VerticalAlign = StringAlignment.Center;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Margins.Top = marginTop;
        }

        private void SetChart(Dashboard.FST_0001_0003_Text textElement, string filter, string measure)
        {
            CustomParam measureParam = UserParams.CustomParam("measure");
            measureParam.Value = measure;

            string chartQuery = DataProvider.GetQueryText(String.Format("FST_0001_0033_Horizontal_gauge{0}", filter));
            string textQuery = DataProvider.GetQueryText(String.Format("FST_0001_0033_Horizontal_text{0}", filter));            
            
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(chartQuery, "dummy", dtChart);
            DataTable dtText = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(textQuery, "dummy", dtText);

            textElement.DtChart = dtChart;
            textElement.DtText = dtText;
        }

        private bool CheckedArea(string name, string fileName)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath(fileName);
            if (!File.Exists(xmlFile))
            {
                return true;
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);
                // Ищем узел регионов
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
