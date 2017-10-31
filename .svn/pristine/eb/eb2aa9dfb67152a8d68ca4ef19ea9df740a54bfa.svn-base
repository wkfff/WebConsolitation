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
    public partial class FST_0001_0043 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart1.Width = 750;
            UltraChart1.Height = 300;

            UltraChart2.Width = 750;
            UltraChart2.Height = 300;

            UltraChart3.Width = 750;
            UltraChart3.Height = 300;

            SetUpAreaChart(UltraChart1, "", 35, 10);
            SetUpAreaChart(UltraChart2, "", 17, 100);
            SetUpAreaChart(UltraChart3, "", 17, 100);

            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText("FST_0001_0043_Chart1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart1.DataSource = dtChart;
                UltraChart1.DataBind();
            }
            else
            {
                UltraChart1.Visible = false;
                IPadElementHeader1.Visible = false;
            }

            dtChart = new DataTable();
            query = DataProvider.GetQueryText("FST_0001_0043_Chart2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart2.DataSource = dtChart;
                UltraChart2.DataBind();
            }
            else
            {
                UltraChart2.Visible = false;
                IPadElementHeader2.Visible = false;
            }

            dtChart = new DataTable();
            query = DataProvider.GetQueryText("FST_0001_0043_Chart3");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart3.DataSource = dtChart;
                UltraChart3.DataBind();
            }
            else
            {
                UltraChart3.Visible = false;
                IPadElementHeader1.Visible = false;
            }

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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

        private void SetUpAreaChart(UltraChart chart, string measureName, int legendSpan, int marginX)
        {
            chart.ChartType = ChartType.ColumnChart;
            chart.Data.ZeroAligned = true;
            chart.ColumnChart.NullHandling = NullHandling.DontPlot;
            chart.Tooltips.FormatString = String.Format(
                    "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL><br /><ITEM_LABEL> год<br /><b><DATA_VALUE:N0></b>&nbsp;{0}</span>", measureName);
            chart.Legend.SpanPercentage = legendSpan;

            chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Far.Value = marginX;
            chart.Axis.X2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Far.Value = marginX;

            chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Near.Value = marginX;
            chart.Axis.X2.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Near.Value = marginX;

            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;
            chart.Axis.Y2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y2.Margin.Far.Value = 10;

            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.ColumnChart.SeriesSpacing = 1;

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

            SetupTitleLeft(chart, measureName, 30);
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
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
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

        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FST_0001_0043_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 265;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            
            e.Layout.Bands[0].Columns[1].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = "2010";

            e.Layout.Bands[0].Columns[2].Width = 145;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[2].Header.Caption = "Прирост к прошлому году";

            e.Layout.Bands[0].Columns[3].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            e.Layout.Bands[0].Columns[3].Header.Caption = "2011";

            e.Layout.Bands[0].Columns[4].Width = 145;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            e.Layout.Bands[0].Columns[4].Header.Caption = "Прирост к прошлому году";

            int borderWidth = 3;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = borderWidth;
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.ForeColor = Color.White;
            if (e.Row.Index > 0 && e.Row.Index < 5)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
                e.Row.Cells[0].Style.ForeColor = Color.FromArgb(0xd1d1d1);
                e.Row.Cells[0].Value = String.Format("<i>{0}</i>", e.Row.Cells[0].Value.ToString());
            }
        }

        #endregion
    }
}
