using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
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
using Primitive = Infragistics.UltraChart.Core.Primitives.Primitive;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0007_0003 : CustomReportPage
    {
        private DateTime currentDateFederal;
        private DateTime currentDateRegional;

        private int chartWidth = 115;
        private int chartHeight = 57;

        private double hmaoAvg;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string multitouchIcon = String.Empty;

            multitouchIcon = "<img src='../../../images/detail.png'>";
            detalizationIconDiv.InnerHtml = String.Format("<table><tr><td><a href='webcommand?showPinchReport=Oil_0007_0005_OIL={1}'>{0}</a></td>", multitouchIcon, HttpContext.Current.Session["CurrentOilID"]);
            detalizationIconDiv.InnerHtml += String.Format("<td><a href='webcommand?showPinchReport=Oil_0007_0004_OIL={1}'><img src='../../../images/lock.png'></a></td></tr></table>", multitouchIcon, HttpContext.Current.Session["CurrentOilID"]);

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/Oil_0007_0003/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/Oil_0007_0003/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"Oil_0007_0005_OIL={0}\" bounds=\"x=0;y=700;width=768;height=100\" openMode=\"incomes\"/><element id=\"Oil_0007_0004_OIL={0}\" bounds=\"x=0;y=800;width=768;height=230\" openMode=\"outcomes\"/></touchElements>", HttpContext.Current.Session["CurrentOilID"]));

            IPadElementHeader4.Text = String.Format("Динамика розничных цен на {0} по ЯНАО", CRHelper.ToLowerFirstSymbol(UserParams.Oil.Value));

            UltraChart1.Style.Add("margin-top", "-10px");
            UltraChart.Style.Add("margin-top", "-10px");

            switch (HttpContext.Current.Session["CurrentOilID"].ToString())
            {
                case "1":
                    {
                        AddLineAppearencesUltraChart1(UltraChart1, Color.Green);
                        AddLineAppearencesUltraChart1(UltraChart, Color.Green);
                        break;
                    }
                case "2":
                    {
                        AddLineAppearencesUltraChart1(UltraChart1, Color.Gold);
                        AddLineAppearencesUltraChart1(UltraChart, Color.Gold);
                        break;
                    }
                case "3":
                    {
                        AddLineAppearencesUltraChart1(UltraChart1, Color.Pink);
                        AddLineAppearencesUltraChart1(UltraChart, Color.Pink);
                        break;
                    }
                case "4":
                    {
                        AddLineAppearencesUltraChart1(UltraChart1, Color.Red);
                        AddLineAppearencesUltraChart1(UltraChart, Color.Red);
                        break;
                    }
                case "5":
                    {
                        AddLineAppearencesUltraChart1(UltraChart1, Color.Cyan);
                        AddLineAppearencesUltraChart1(UltraChart, Color.Cyan);
                        break;
                    }
            }

            InitializeTable1();
            UltraChart1.BackColor = Color.Transparent;
            SetupDynamicChart(UltraChart1, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.DataBind();
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0007_0003_chart1"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            //UltraChart1.Axis.Y.RangeMin = 18;
            //UltraChart1.Axis.Y.RangeMax = 30;

            //double value;
            //if (double.TryParse(dtChart.Rows[0][2].ToString(), out value))
            //{
            //    UltraChart1.Axis.Y.RangeMin = value - 1;
            //}
            //if (double.TryParse(dtChart.Rows[dtChart.Rows.Count - 1][2].ToString(), out value))
            //{
            //    UltraChart1.Axis.Y.RangeMax = value + 1;
            //}

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

                UltraChart1.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart1.Series.Add(series);
                }
            }
        }

        private void AddLineAppearencesUltraChart1(UltraChart chart, Color color)
        {
            chart.Width = 760;
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
            chart.Axis.X.Extent = 70;
            chart.Axis.X.Labels.Font = new Font("Verdana", 10);
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            chart.Axis.Y.Extent = 40;

            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = "руб. за литр";
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.VerticalAlign = StringAlignment.Near;
            chart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            chart.TitleLeft.Extent = 40;
            chart.TitleLeft.Margins.Top = 0;
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.FontColor = Color.White;

            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;
            chart.Axis.Y2.MinorGridLines.Color = Color.Black;
            chart.Axis.Y2.MajorGridLines.Color = Color.Black;

            UltraChart1.Axis.X.Margin.Near.Value = 3;
            UltraChart1.Axis.X.Margin.Far.Value = 2;
            UltraChart1.Axis.Y.Margin.Near.Value = 7;
            UltraChart1.Axis.Y.Margin.Far.Value = 3;

            UltraChart1.Data.ZeroAligned = false;

            chart.Data.EmptyStyle.Text = " ";
            chart.EmptyChartText = " ";

            chart.AreaChart.NullHandling = NullHandling.DontPlot;
            chart.Data.ZeroAligned = false;

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
                if (primitive is PointSet)
                {
                    var pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                    }
                }
            }
        }

        #region Таблица1
        private DataTable dt;

        private void InitializeTable1()
        {

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0007_0003_incomes_regional_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            currentDateRegional = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);

            elementCaption.Text = String.Format("Сравнение средней цены по МО ЯНАО на {0:dd.MM.yyyy}", currentDateRegional);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDate.Rows[0][1].ToString();

            dt = new DataTable();
            query = DataProvider.GetQueryText("Oil_0007_0003_table1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dt);

            dt.Rows[0][0] = String.Format("<table style='border-collapse: collapse; margin-top: -3px; margin-bottom: -3px'><tr><td align='left' style='border: none; text-align:left'>Ямало-Ненецкий автономный округ</td></tr><tr><td align='right' style='border: none'><span style='DigitsValue'>на {0:dd.MM.yyyy}</td></tr></table>", currentDateRegional);

            InitializeChart();

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0007_0003_incomes_federal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            currentDateFederal = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDate.Rows[0][1].ToString();

            DataTable dt2 = new DataTable();
            query = DataProvider.GetQueryText("Oil_0007_0003_table2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt2);

            foreach (DataRow row in dt2.Rows)
            {
                dt.ImportRow(row);
            }

            dt.Rows[1][0] = String.Format("<table style='border-collapse: collapse; margin-top: -3px; margin-bottom: -3px'><tr><td align='left' style='border: none; text-align:left'>Уральский федеральный округ</td></tr><tr><td align='right' style='border: none'><span style='DigitsValue'>на {0:dd.MM.yyyy}</td></tr></table>", currentDateFederal);
            dt.Rows[2][0] = String.Format("<table style='border-collapse: collapse; margin-top: -3px; margin-bottom: -3px'><tr><td align='left' style='border: none; text-align:left'>Российская Федерация</td></tr><tr><td align='right' style='border: none'><span style='DigitsValue'>на {0:dd.MM.yyyy}</td></tr></table>", currentDateFederal);

            dt.AcceptChanges();

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 6; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            if (HttpContext.Current.Session["CurrentOilID"].ToString() != "5")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dtSource.NewRow();

                    if (i > 0)
                    {
                        chartWidth = 100;
                        chartHeight = 44;
                    }
                    else
                    {
                        hmaoAvg = Convert.ToDouble(dt.Rows[i][1]);
                    }

                    string gaugeValue = String.Format("{0:N2}", dt.Rows[i][1]);

                    if (!File.Exists(Server.MapPath(String.Format("~/TemporaryImages/Oil_gauge{0}{1}.png", gaugeValue, chartWidth))))
                    {
                        UltraGauge gauge = GetGauge(gaugeValue);
                        FileStream stream = new FileStream(Server.MapPath(String.Format("~/TemporaryImages/Oil_gauge{0}{1}.png", gaugeValue, chartWidth)), FileMode.OpenOrCreate);
                        gauge.SaveTo(stream, GaugeImageType.Png, new Size(chartWidth, chartHeight));
                        stream.Flush();
                        stream.Close();
                    }
                    //PlaceHolder1.Controls.Add(gauge);

                    row[0] = dt.Rows[i][0].ToString().Replace("Бензин ", "Бензин<br/>");
                    row[1] = String.Format("<img src='../../../TemporaryImages/Oil_gauge{0}{1}.png'>", gaugeValue, chartWidth);

                    double value;
                    if (Double.TryParse(dt.Rows[i][2].ToString(), out value))
                    {
                        string absoluteGrown = value > 0
                                                   ? String.Format("+{0:N2}", value)
                                                   : String.Format("{0:N2}", value);

                        string img = String.Empty;
                        if (value != 0)
                        {
                            img = value > 0
                                      ? "<img src='../../../images/arrowRedUpBB.png'>"
                                      : "<img src='../../../images/arrowGreenDownBB.png'>";
                        }

                        row[2] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[i][3], img);
                    }

                    if (Double.TryParse(dt.Rows[i][4].ToString(), out value))
                    {
                        value = Convert.ToDouble(dt.Rows[i][4].ToString());
                        string absoluteGrown = value > 0
                                                   ? String.Format("+{0:N2}", value)
                                                   : String.Format("-{0:N2}", Math.Abs(value));

                        string img = String.Empty;
                        if (value != 0)
                        {
                            img = value > 0
                                      ? "<img src='../../../images/arrowRedUpBB.png'>"
                                      : "<img src='../../../images/arrowGreenDownBB.png'>";
                        }

                        row[3] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[i][5], img);
                    }

                    row[4] = String.Format("{0}<br/>{1:N2} руб.", dt.Rows[i][7].ToString().Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР"), dt.Rows[i][6]);
                    row[5] = String.Format("{0}<br/>{1:N2} руб.", dt.Rows[i][9].ToString().Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР"), dt.Rows[i][8]);

                    dtSource.Rows.Add(row);
                }
            }
            else
            {

                DataRow row = dtSource.NewRow();
                hmaoAvg = Convert.ToDouble(dt.Rows[0][1]);

                string gaugeValue = String.Format("{0:N2}", dt.Rows[0][1]);

                if (!File.Exists(Server.MapPath(String.Format("~/TemporaryImages/Oil_gauge{0}{1}.png", gaugeValue, chartWidth))))
                {
                    UltraGauge gauge = GetGauge(gaugeValue);
                    FileStream stream = new FileStream(Server.MapPath(String.Format("~/TemporaryImages/Oil_gauge{0}{1}.png", gaugeValue, chartWidth)), FileMode.OpenOrCreate);
                    gauge.SaveTo(stream, GaugeImageType.Png, new Size(chartWidth, chartHeight));
                    stream.Flush();
                    stream.Close();
                }

                row[0] = dt.Rows[0][0].ToString().Replace("Бензин ", "Бензин<br/>");
                row[1] = String.Format("<img src='../../../TemporaryImages/Oil_gauge{0}{1}.png'>", gaugeValue, chartWidth);

                double value;
                if (Double.TryParse(dt.Rows[0][2].ToString(), out value))
                {
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("{0:N2}", value);

                    string img = String.Empty;
                    if (value != 0)
                    {
                        img = value > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }

                    row[2] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[0][3], img);
                }

                if (Double.TryParse(dt.Rows[0][4].ToString(), out value))
                {
                    value = Convert.ToDouble(dt.Rows[0][4].ToString());
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("-{0:N2}", Math.Abs(value));

                    string img = String.Empty;
                    if (value != 0)
                    {
                        img = value > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }

                    row[3] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[0][5], img);
                }

                row[4] = String.Format("{0}<br/>{1:N2} руб.", dt.Rows[0][7].ToString().Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР"), dt.Rows[0][6]);
                row[5] = String.Format("{0}<br/>{1:N2} руб.", dt.Rows[0][9].ToString().Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР"), dt.Rows[0][8]);

                dtSource.Rows.Add(row);
            }

            UltraWebGrid1.DataSource = dtSource;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 3, 0, false);
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 5, 0, false);
            e.Row.Style.Padding.Top = 3;
            e.Row.Style.Padding.Bottom = 3;
        }

        private GridHeaderLayout headerLayout1;
        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);

            headerLayout1.AddCell("");
            headerLayout1.AddCell("Средняя розничная цена, руб.");

            GridHeaderCell headerCell = headerLayout1.AddCell("Динамика");

            headerCell.AddCell("за неделю");
            headerCell.AddCell("с начала года");

            headerLayout1.AddCell("<img src=\"../../../images/min.png\"><br/>Мин. цена");
            headerLayout1.AddCell("<img src=\"../../../images/max.png\"><br/>Макс. цена");

            headerLayout1.ApplyHeaderInfo();

            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 16;
            e.Layout.Bands[0].Columns[0].Width = 144;
            e.Layout.Bands[0].Columns[1].Width = 130;
            e.Layout.Bands[0].Columns[2].Width = 92;
            e.Layout.Bands[0].Columns[3].Width = 92;
            e.Layout.Bands[0].Columns[4].Width = 150;
            e.Layout.Bands[0].Columns[5].Width = 150;
        }

        #endregion


        #region Фонды

        private DataTable dtChart;

        private void InitializeChart()
        {
            #region Настройка диаграммы

            UltraChart.Width = 760;
            UltraChart.Height = 500;

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.Data.ZeroAligned = false;
            UltraChart.ColumnChart.NullHandling = NullHandling.DontPlot;

            UltraChart.Axis.X.Extent = 230;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.Y.Extent = 54;

            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 10);

            UltraChart.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            UltraChart.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart.Axis.Y.MinorGridLines.Color = Color.Black;
            UltraChart.Axis.Y2.MinorGridLines.Color = Color.Black;
            UltraChart.Axis.Y2.MajorGridLines.Color = Color.Black;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            //UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = false;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.FontColor = Color.White;
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart.TitleLeft.Text = "руб. за литр";
            UltraChart.TitleLeft.Margins.Bottom = 200;
            //UltraChart.TitleLeft.Margins.Left = 17;
            UltraChart.TitleLeft.VerticalAlign = StringAlignment.Far;

            UltraChart.BackColor = Color.Transparent;

            UltraChart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'>Средняя цена\n<ITEM_LABEL>\n<b><DATA_VALUE:N2><b> руб.</span>";

            //UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0007_0003_chart");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            dtChart.Columns.RemoveAt(0);

            foreach (DataRow row in dtChart.Rows)
            {
                row[0] =
                    row[0].ToString().Replace("Город ", "").Replace("город ", "").Replace(
                        "муниципальный район", "МР");
            }

            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName =
                    col.ColumnName.Replace("Бензин ", "").Replace("Дизельное топливо", "ДТ");
            }

            UltraChart.Data.SwapRowsAndColumns = true;
            UltraChart.DataSource = dtChart;
            UltraChart.DataBind();
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double RFStartLineX = 0;
            double RFStartLineY = 0;

            double RFEndLineX = 0;
            double RFEndLineY = 0;
            string RFheader = "";

            RFheader = string.Format("{0}: {1:N2} {2}", "Средняя по ЯНАО", hmaoAvg, "руб.");

            RFStartLineX = xAxis.Map(xAxis.Minimum);
            RFStartLineY = yAxis.Map(hmaoAvg);

            RFEndLineX = xAxis.Map(xAxis.Maximum);
            RFEndLineY = yAxis.Map(hmaoAvg);

            GenHorizontalLineAndLabel((int)RFStartLineX, (int)RFStartLineY, (int)RFEndLineX, (int)RFEndLineY,
                    Color.Green, RFheader, e, true);

        }

        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = Color.Cyan;
            Line.PE.StrokeWidth = 2;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;

            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
            textLabel.PE.Fill = System.Drawing.Color.White;
            //textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 8);
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10);

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY - 20, 500, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY + 1, 500, 15);
            }

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);

            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);
        }

        #endregion

        #region Гейдж

        private UltraGauge GetGauge(string markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = chartWidth;
            gauge.Height = chartHeight;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Oil_gauge{0}{1}.png", markerValue, chartWidth);

            // Настраиваем гейдж
            SegmentedDigitalGauge SegmentedGauge = new SegmentedDigitalGauge();
            SegmentedGauge.BoundsMeasure = Measure.Percent;
            SegmentedGauge.Digits = 4;
            SegmentedGauge.InnerMarginString = "2, 2, 2, 2, Pixels";
            SegmentedGauge.MarginString = "2, 2, 2, 2, Pixels";
            SegmentedGauge.Text = markerValue.Replace(',', '.');

            SolidFillBrushElement brushUnlit = new SolidFillBrushElement();
            brushUnlit.Color = Color.FromArgb(10, 10, 10, 10);
            SegmentedGauge.UnlitBrushElements.Add(brushUnlit);

            SolidFillBrushElement brushFont = new SolidFillBrushElement();
            brushFont.Color = Color.PaleGoldenrod;
            SegmentedGauge.FontBrushElements.Add(brushFont);

            SolidFillBrushElement brush = new SolidFillBrushElement();
            brush.Color = Color.Black;
            SegmentedGauge.BrushElements.Add(brush);

            gauge.Gauges.Add(SegmentedGauge);
            return gauge;
        }

        private const int ScaleStartExtent = 5;
        private const int ScaleEndExtent = 97;

        private static void AddMajorTickmarkScale(double endValue, LinearGauge linearGauge)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.MajorTickmarks.EndWidth = 2;
            scale.MajorTickmarks.StartWidth = 2;
            scale.MajorTickmarks.EndExtent = 40;
            scale.MajorTickmarks.StartExtent = 25;
            scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.White));
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 10));
            linearGauge.Scales.Add(scale);
        }

        private static void AddGradient(LinearGauge linearGauge)
        {
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(150, 150, 150);
            gradientBrush.StartColor = Color.FromArgb(10, 255, 255, 255);
            gradientBrush.GradientStyle = Gradient.BackwardDiagonal;
            linearGauge.BrushElements.Add(gradientBrush);
        }

        private static void AddMainScale(double endValue, LinearGauge linearGauge, double markerValue)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 5));

            AddMainScaleRange(scale, markerValue);
            SetMainScaleLabels(scale);
            linearGauge.Scales.Add(scale);
        }

        private static void SetMainScaleLabels(LinearGaugeScale scale)
        {

            scale.Labels.ZPosition = LinearTickmarkZPosition.AboveMarkers;
            scale.Labels.Extent = 9;
            Font font = new Font("Arial", 9);
            scale.Labels.Font = font;
            scale.Labels.EqualFontScaling = false;
            SolidFillBrushElement solidFillBrushElement = new SolidFillBrushElement(Color.White);
            solidFillBrushElement.RelativeBoundsMeasure = Measure.Percent;
            Rectangle rect = new Rectangle(0, 0, 80, 0);
            solidFillBrushElement.RelativeBounds = rect;
            scale.Labels.BrushElements.Add(solidFillBrushElement);
            scale.Labels.Shadow.Depth = 2;
            scale.Labels.Shadow.BrushElements.Add(new SolidFillBrushElement());
        }

        private static void AddMainScaleRange(LinearGaugeScale scale, double markerValue)
        {
            LinearGaugeRange range = new LinearGaugeRange();
            range.EndValue = markerValue;
            range.StartValue = 0;
            range.OuterExtent = 80;
            range.InnerExtent = 20;
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(1, 51, 75);
            gradientBrush.StartColor = Color.FromArgb(8, 218, 164);
            gradientBrush.GradientStyle = Gradient.Vertical;
            range.BrushElements.Add(gradientBrush);
            scale.Ranges.Add(range);
        }

        #endregion

    }
}
