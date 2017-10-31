using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
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

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0001_0002 : CustomReportPage
    {
        private DateTime startDate;
        private DateTime lastDate;
        private DateTime currentDate;

        private int chartWidth = 115;
        private int chartHeight = 57;

        private string currentRegion;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //CustomParams.MakeMoParams("131", "id");

            if (!UserParams.Mo.Value.Contains("г."))
            {
                UserParams.Mo.Value = String.Format(".[{0} муниципальный район]", UserParams.Mo.Value);
            }
            else
            {
                UserParams.Mo.Value = String.Format(".[{0}]", UserParams.Mo.Value.Replace("г.", "Город "));
            }

            currentRegion = UserParams.Mo.Value.Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР").Replace("[", "").Replace("]", "").Replace(".DataMember", "");

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0001_0002_incomes_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            string oldMoValue = UserParams.Mo.Value;

            UserParams.Mo.Value = oldMoValue;

            startDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);
            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDate.Rows[0][1].ToString();

            IncomesHeader.Text += String.Format(" на {0:dd.MM.yyyy}", currentDate);

            InitializeTable1();
            InitializeChart();
        }

        #region Таблица1
        private DataTable dt;

        private void InitializeTable1()
        {

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0001_0002_table1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dt);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 6; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                string gaugeValue = String.Format("{0:N2}", dt.Rows[i][1]);
                UltraGauge gauge = GetGauge(gaugeValue);
                PlaceHolder1.Controls.Add(gauge);
                row[0] = dt.Rows[i][0].ToString().Replace("Бензин ", "Бензин<br/>");
                row[1] = String.Format("<img src='../../../TemporaryImages/Oil_gauge{0}.png'>", gaugeValue);

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

                    row[2] = String.Format("{0}<br/>{2}&nbsp;{1:P2}", absoluteGrown, dt.Rows[i][3], img);
                }

                if (Double.TryParse(dt.Rows[i][4].ToString(), out value))
                {
                    value = Convert.ToDouble(dt.Rows[i][4].ToString());
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

                    row[3] = String.Format("{0}<br/>{2}&nbsp;{1:P2}", absoluteGrown, dt.Rows[i][5], img);
                }

                row[4] = String.Format("{0}<br/>{1:N2} руб.", dt.Rows[i][7].ToString().Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР"), dt.Rows[i][6]);
                row[5] = String.Format("{0}<br/>{1:N2} руб.", dt.Rows[i][9].ToString().Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР"), dt.Rows[i][8]);

                dtSource.Rows.Add(row);
            }


            UltraWebGrid1.DataSource = dtSource;
            UltraWebGrid1.DataBind();
        }

        private GridHeaderLayout headerLayout1;
        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);

            headerLayout1.AddCell("Наименование товара");
            headerLayout1.AddCell(String.Format("Цена на {0:dd.MM.yy}, руб.", currentDate));

            headerLayout1.AddCell(String.Format("Динамика к {0:dd.MM.yy}", lastDate));
            headerLayout1.AddCell(String.Format("Динамика к {0:dd.MM.yy}", startDate));

            GridHeaderCell headerCell = headerLayout1.AddCell("Сравнение с другими МО");
            headerCell.AddCell("<img src=\"../../../images/starYellow.png\">Мин. цена");
            headerCell.AddCell("<img src=\"../../../images/starGray.png\">Макс. цена");

            headerLayout1.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 16;
            e.Layout.Bands[0].Columns[0].Width = 144;
            e.Layout.Bands[0].Columns[1].Width = 130;
            e.Layout.Bands[0].Columns[2].Width = 90;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 150;
            e.Layout.Bands[0].Columns[5].Width = 150;
        }

        #endregion

        #region Диаграммы

        private DataTable dtChart;

        private void InitializeChart()
        {
            #region Настройка диаграммы

            UltraChart.Width = 750;
            UltraChart.Height = 500;

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.X.Extent = 250;
            UltraChart.Axis.X2.Extent = 0;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.Margin.Near.Value = 3;
            UltraChart.Axis.X.Margin.Far.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 7;
            UltraChart.Axis.Y.Margin.Far.Value = 3;
            UltraChart.Axis.Y.Extent = 40;
            UltraChart.Axis.Y2.Visible = true;
            UltraChart.Axis.Y2.Extent = 15;
            UltraChart.Axis.Y2.LineThickness = 0;
            UltraChart.Axis.Y2.Margin.Near.Value = 7;
            UltraChart.Axis.Y2.Margin.Far.Value = 3;
            UltraChart.Axis.Y2.Labels.Visible = false;

            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 10);

            UltraChart.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            UltraChart.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart.Axis.Y.MinorGridLines.Color = Color.Black;
            UltraChart.Axis.Y2.MinorGridLines.Color = Color.Black;
            UltraChart.Axis.Y2.MajorGridLines.Color = Color.Black;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            //UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            //UltraChart.Legend.Margins.Top = -5;
            UltraChart.Legend.SpanPercentage = 7;
            UltraChart.Legend.Font = new Font("Verdana", 10);
            UltraChart.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart.TitleLeft.Text = "руб. за литр";
            UltraChart.TitleLeft.Margins.Bottom = 100;

            UltraChart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL>\n<ITEM_LABEL>\n<b><DATA_VALUE:N2><b> руб.</span>";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            //UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            AddLineAppearencesUltraChart1();

            #endregion

            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0001_0002_chart");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                row[0] =
                    row[0].ToString().Replace("Город ", "").Replace("город ", "").Replace(
                        "муниципальный район", "МР");
            }

            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName =
                    col.ColumnName.Replace("Бензин марки ", "").Replace("Дизельное топливо", "ДТ");
            }

            UltraChart.Data.SwapRowsAndColumns = true;
            UltraChart.DataSource = dtChart;
            UltraChart.DataBind();
        }

        private void AddLineAppearencesUltraChart1()
        {
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.ApplyRowWise = true;
            UltraChart.ColorModel.Skin.PEs.Clear();

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
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }

                }
                pe.Fill = color;
                pe.FillStopColor = color;
                pe.StrokeWidth = 0;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 255;
                pe.FillStopOpacity = 150;
                UltraChart.ColorModel.Skin.PEs.Add(pe);
                pe.Stroke = Color.Black;
                pe.StrokeWidth = 0;
                LineAppearance lineAppearance2 = new LineAppearance();

                lineAppearance2.IconAppearance.Icon = SymbolIcon.Square;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Large;

                lineAppearance2.IconAppearance.PE = pe;

                UltraChart.LineChart.LineAppearances.Add(lineAppearance2);

                UltraChart.LineChart.Thickness = 0;
            }
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null &&
                    primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    // text.bounds.Width = 100;

                    if (text.GetTextString() == currentRegion)
                    {
                        text.labelStyle.Font = new Font("Verdana", 14);
                        text.labelStyle.FontColor = Color.White;
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

        #endregion

        #region Гейдж

        private UltraGauge GetGauge(string markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = chartWidth;
            gauge.Height = chartHeight;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Oil_gauge{0}.png", markerValue);

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
