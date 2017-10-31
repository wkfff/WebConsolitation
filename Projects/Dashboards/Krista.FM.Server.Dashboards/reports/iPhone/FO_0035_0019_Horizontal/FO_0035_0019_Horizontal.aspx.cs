using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0019_Horizontal : CustomReportPage
    {
        int legendBoxCount = 0;
        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            SetupChart(UltraChart1, 27, 30);
            SetupChart(UltraChart2, 15, 400);
            // AddLineAppearencesUltraChart(UltraChart1, Color.FromArgb(78, 230, 228), Color.FromArgb(168, 48, 137));
            AddLineAppearencesUltraChart(UltraChart2, Color.Khaki, Color.FromArgb(78, 230, 228));

            dt1 = GetDataSource("FO_0035_0018_chart_rests");
            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dt1));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dt1));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(3, dt1));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(4, dt1));
            UltraChart1.DataBind();

            dt2 = GetDataSource("FO_0035_0018_chart_hz");
            UltraChart2.Series.Clear();
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(1, dt2));
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(2, dt2));
            UltraChart2.DataBind();

            lbDescription.Text = String.Format("Остатки средств областного бюджета, свободных для распределения по направлениям финансирования");
        }

        private int lastDataElementIndex = 0;

        private DataTable GetDataSource(string queryName)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][1] = String.Format("{0} {1}", dt.Rows[i][0], dt.Rows[i][1]);

                if (dt.Rows[i][2] != DBNull.Value)
                {
                    if (dt.Columns.Count > 3 && dt.Rows[i][3] != DBNull.Value)
                    {
                        lastDataElementIndex = i;
                    }
                }
            }

            dt.Columns.RemoveAt(0);
            dt.AcceptChanges();
            return dt;
        }

        private void SetupChart(UltraChart chart, int legendSpanPercentage, int legendMargins)
        {
            chart.Width = 1010;
            chart.Height = 330;
            chart.ChartType = ChartType.AreaChart;
            chart.Data.SwapRowsAndColumns = false;
            chart.Data.ZeroAligned = false;
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.AreaChart.NullHandling = NullHandling.DontPlot;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.Axis.X.Extent = 80;
            chart.Axis.Y.Extent = 90;
            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;

            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;

            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL>\n<ITEM_LABEL><b><DATA_VALUE:N1></b> тыс.руб.</span>";

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.SpanPercentage = legendSpanPercentage;

            chart.Axis.Y.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);

            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.Font = new Font("Verdana", 10);

            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = "тыс.руб.";
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.FontColor = Color.White;
            chart.TitleLeft.Margins.Bottom = 150;

            chart.Legend.Font = new Font("Verdana", 10);

            chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Far.Value = 10;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 4; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;

                LineAppearance lineAppearance2 = new LineAppearance();

                switch (i)
                {
                    case 1:
                        {
                            color = Color.Aqua;
                            stopColor = Color.Aqua;
                            peType = PaintElementType.Gradient;
                            lineAppearance2.IconAppearance.Icon = SymbolIcon.None;
                            break;
                        }
                    case 2:
                        {
                            color = Color.MediumSeaGreen;
                            stopColor = Color.Green;
                            peType = PaintElementType.Gradient;
                            lineAppearance2.IconAppearance.Icon = SymbolIcon.None;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Magenta;
                            stopColor = Color.Magenta;
                            peType = PaintElementType.SolidFill;
                            lineAppearance2.IconAppearance.Icon = SymbolIcon.Diamond;
                            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Large;
                            lineAppearance2.LineStyle.StartStyle = LineCapStyle.NoAnchor;
                            lineAppearance2.LineStyle.EndStyle = LineCapStyle.NoAnchor;
                            break;
                        }
                    case 4:
                        {
                            color = Color.Lime;
                            stopColor = Color.Lime;
                            peType = PaintElementType.SolidFill;
                            lineAppearance2.IconAppearance.Icon = SymbolIcon.Diamond;
                            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Large;
                            lineAppearance2.LineStyle.StartStyle = LineCapStyle.NoAnchor;
                            lineAppearance2.LineStyle.EndStyle = LineCapStyle.NoAnchor;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = (byte)150;
                pe.FillStopOpacity = (byte)150;

                lineAppearance2.IconAppearance.PE = pe;

                UltraChart1.AreaChart.LineAppearances.Add(lineAppearance2);
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.PointSet")
                {
                    foreach (Primitive child in primitive.GetPrimitives())
                    {
                        if (child.ToString() == "Infragistics.UltraChart.Core.Primitives.DataPoint")
                        {
                            DataPoint dataPoint = (DataPoint)child;
                            if (dataPoint.DataPoint == null)
                            {
                                dataPoint.Visible = false;
                            }
                        }
                    }
                }

                if (primitive is Polyline)
                {
                    Polyline pointSet = (Polyline)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                        if (point.Series != null && !(point.Series.Label.Contains("<br/>")))
                        {
                            point.Series.Label = GetName(point.Series.Label);
                        }
                        if (point.DataPoint != null)
                        {
                            int year = Convert.ToInt32(dt1.Rows[point.Column]["Год"].ToString());
                            point.DataPoint.Label = point.Series.Label.Contains("прошлого") ?
                                    String.Empty : String.Format("{0} {1} {2} года<br/>", point.DataPoint.Label.Split()[0], CRHelper.RusMonthGenitive(CRHelper.MonthNum(point.DataPoint.Label.Split()[1])), year);
                        }
                    }
                }

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    if (polyline.Series.Label.Contains("прошлого"))
                    {
                        primitive.Visible = false;
                        e.SceneGraph[i + 1].Visible = false;
                    }
                }

                if (primitive is Box &&
                    primitive.Path.Contains("Legend"))
                {
                    Box box = (Box)primitive;
                    if (box.rect.Width == box.rect.Height)
                    {
                        legendBoxCount++;
                        if (legendBoxCount > 2)
                        {
                            int x = box.rect.X;
                            int y = box.rect.Y;
                            int width = box.rect.Width;
                            int height = box.rect.Height;
                            Point[] ponts = new Point[4];
                            ponts[0] = new Point(x + width / 2, y);
                            ponts[1] = new Point(x + width, y + height / 2);
                            ponts[2] = new Point(x + width / 2, y + height);
                            ponts[3] = new Point(x, y + height / 2);
                            Polygon pl = new Polygon(ponts);

                            pl.PE = primitive.PE;
                            e.SceneGraph.Add(pl);
                            primitive.Visible = false;
                        }
                    }
                }
            }
            ReplaceAxisLabels(e.SceneGraph);
            axisMonth = String.Empty;
            legendBoxCount = 1;
        }

        private string axisMonth = String.Empty;

        private void ReplaceAxisLabels(SceneGraph grahp)
        {
            for (int i = 0; i < grahp.Count; i++)
            {
                Primitive primitive = grahp[i];
                if (primitive is Text)
                {
                    string text = ((Text)primitive).GetTextString();
                    text = text.Trim();
                    // Проверяем формат
                    string[] textArray = text.Split();
                    if (textArray.Length == 2)
                    {
                        int day;
                        if (Int32.TryParse(textArray[0], out day) && CRHelper.IsMonthCaption(textArray[1]))
                        {
                            if (axisMonth == textArray[1])
                            {
                                ((Text)primitive).SetTextString(day.ToString());
                            }
                            else
                            {
                                ((Text)primitive).SetTextString(String.Format("{0}-{1}",
                                              CRHelper.ToUpperFirstSymbol(
                                                  CRHelper.RusMonth(CRHelper.MonthNum(textArray[1]))), day));
                                axisMonth = textArray[1];
                            }
                        }
                    }
                }
            }
        }

        private void AddLineAppearencesUltraChart(UltraChart chart, Color SeriesColor1, Color SeriesColor2)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;

                LineAppearance lineAppearance2 = new LineAppearance();

                switch (i)
                {
                    case 1:
                        {
                            color = SeriesColor1;
                            stopColor = SeriesColor1;
                            peType = PaintElementType.Gradient;
                            lineAppearance2.IconAppearance.Icon = SymbolIcon.None;
                            break;
                        }
                    case 2:
                        {
                            color = SeriesColor1;
                            stopColor = SeriesColor1;
                            peType = PaintElementType.SolidFill;
                            lineAppearance2.IconAppearance.Icon = SymbolIcon.Diamond;
                            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Large;
                            break;
                        }

                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = (byte)150;
                pe.FillStopOpacity = (byte)150;

                lineAppearance2.IconAppearance.PE = pe;

                chart.AreaChart.LineAppearances.Add(lineAppearance2);
                chart.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private string GetName(string name)
        {

            if (name.Length > 40)
            {
                int k = 0;
                for (int j = 0; j < name.Length; j++)
                {
                    k++;
                    if (k > 30 && name[j] == ' ')
                    {
                        name = name.Insert(j, "<br/>");
                        k = 0;
                    }
                }
            }
            return name;
        }
    }
}