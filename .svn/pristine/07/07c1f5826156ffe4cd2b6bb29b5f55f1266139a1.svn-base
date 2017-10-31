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
    public partial class Oil_0004_0003 : CustomReportPage
    {
        private DateTime startDate;
        private DateTime lastDate;
        private DateTime currentDate;

        private int chartWidth = 115;
        private int chartHeight = 57;

        private string currentRegion = "";
        private string currentFo = "СЗФО";

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            currentRegion = UserParams.StateArea.Value;
            currentFo = RegionsNamingHelper.ShortRegionsNames[UserParams.Region.Value];

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0004_0003_incomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);


             //CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);
            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);
            startDate = new DateTime(currentDate.Year - 1, 12, 30);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", startDate, 5);//dtDate.Rows[0][1].ToString();

            IncomesHeader.Text += String.Format(" на {0:dd.MM.yyyy}", currentDate);

            InitializeTable1();
          //  InitializeChart();

            UltraChart2.Width = 750;
            UltraChart3.Width = 750;
           // UltraChart4.Width = 750;
            UltraChart5.Width = 750;

            UltraChart2.Height = 250;
            UltraChart3.Height = 250;
           // UltraChart4.Height = 250;
            UltraChart5.Height = 250;
                       
            SetupDynamicChart(UltraChart2, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart3, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
           // SetupDynamicChart(UltraChart4, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            SetupDynamicChart(UltraChart5, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
                        
            AddLineAppearencesUltraChart1(UltraChart2, Color.Gold);
            AddLineAppearencesUltraChart1(UltraChart3, Color.Pink);
         //   AddLineAppearencesUltraChart1(UltraChart4, Color.Cyan);
            AddLineAppearencesUltraChart1(UltraChart5, Color.Red);
                        
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
         //   UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);
            UltraChart5.DataBinding += new EventHandler(UltraChart5_DataBinding);
                       
            UltraChart2.DataBind();
            UltraChart3.DataBind();
         //   UltraChart4.DataBind();
            UltraChart5.DataBind();
        }

        #region Таблица1
        private DataTable dt;

        private void InitializeTable1()
        {

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0004_0003_table1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 6; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                string gaugeValue = String.Format("{0:N2}", dt.Rows[i][1]);
                double subjectValue;
                Double.TryParse(gaugeValue, out subjectValue);

                UltraGauge gauge = GetGauge(gaugeValue);
                PlaceHolder1.Controls.Add(gauge);
                row[0] = dt.Rows[i][0].ToString().Replace("Бензин марки ", "Бензин<br/>марки<br/>");
                row[1] = String.Format("<img src='../../../TemporaryImages/Oil_gauge{0}.png'>", gaugeValue);

                double value;
                if (Double.TryParse(dt.Rows[i][2].ToString(), out value))
                {
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("{0:N2}", Math.Abs(value));

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
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("{0:N2}", Math.Abs(value));

                    string img = String.Empty;
                    if (value != 0)
                    {
                        img = value > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }

                    row[3] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[i][5], img);
                }

                double foValue = Convert.ToDouble(dt.Rows[i][11]);
                string description = subjectValue > 0 ? (foValue < subjectValue ? String.Format("<table style='border-collapse: collapse; margin-right: -3px'><tr><td style='border: none'><img src='../../../images/ballRedBB.png' style='margin-right: -5px; margin-left: -3px'></td><td style='border: none; width: 100%'>выше {0} на&nbsp;<span class='DigitsValue'>{1:N2}</span></td></tr></table>", currentFo, subjectValue - foValue) :
                    foValue > subjectValue ? String.Format("<table style='border-collapse: collapse; margin-right: -3px'><tr><td style='border: none'><img src='../../../images/ballGreenBB.png' style='margin-right: -5px; margin-left: -3px'></td><td style='border: none; width: 100%'>ниже {0} на&nbsp;<span class='DigitsValue'>{1:N2}</span></td></tr></table>", currentFo, foValue - subjectValue) : String.Format("соответствует {0}<br/>", currentFo)) :
                    String.Empty;

                if (Double.TryParse(dt.Rows[i][12].ToString(), out value))
                {
                    string absoluteGrown = value >= 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("-{0:N2}", Math.Abs(value));

                    string img = String.Empty;
                    if (value != 0)
                    {
                        img = value > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }
                    description += String.Format("{0} {1:N2}&nbsp;{2}&nbsp;{3}", currentFo, foValue, img, absoluteGrown);
                }

                row[4] = String.Format("{0}", description);

                double rfValue = Convert.ToDouble(dt.Rows[i][6]);
                description = subjectValue > 0 ? (rfValue < subjectValue ? String.Format("<table style='border-collapse: collapse; margin-right: -3px'><tr><td style='border: none'><img src='../../../images/ballRedBB.png' style='margin-right: -5px; margin-left: -3px'></td><td style='border: none; width: 100%'>выше {0} на&nbsp;<span class='DigitsValue'>{1:N2}</span></td></tr></table>", "РФ", subjectValue - rfValue) :
                    rfValue > subjectValue ? String.Format("<table style='border-collapse: collapse; margin-right: -3px'><tr><td style='border: none'><img src='../../../images/ballGreenBB.png' style='margin-right: -5px; margin-left: -3px'></td><td style='border: none; width: 100%'>ниже {0} на&nbsp;<span class='DigitsValue'>{1:N2}</span></td></tr></table>", "РФ", rfValue - subjectValue) : String.Format("соответствует {0}", "РФ")) :
                    String.Empty;

                if (Double.TryParse(dt.Rows[i][7].ToString(), out value))
                {
                    string absoluteGrown = value >= 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("-{0:N2}", Math.Abs(value));

                    string img = String.Empty;
                    if (value != 0)
                    {
                        img = value > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }
                    description += String.Format("{0} {1:N2}&nbsp;{2}&nbsp;{3}", "РФ", rfValue, img, absoluteGrown);
                }
                row[5] = String.Format("{0}<br/>", description);

                dtSource.Rows.Add(row);
            }


            UltraWebGrid1.DataSource = dtSource;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 3, 0, false);
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 5, 0, false);
        }

        private GridHeaderLayout headerLayout1;
        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);

            headerLayout1.AddCell(" ");
            headerLayout1.AddCell(String.Format("Средняя розничная цена на {0:dd.MM.yy}, руб.", currentDate));

            GridHeaderCell headerCell = headerLayout1.AddCell("Динамика");
            headerCell.AddCell(String.Format("за неделю с {0:dd.MM.yy}", lastDate));
            headerCell.AddCell(String.Format("к {0:dd.MM.yy}", startDate));

            headerCell = headerLayout1.AddCell(String.Format("Сравнение на {0:dd.MM.yy} с", currentDate));
            headerCell.AddCell(UserParams.Region.Value);
            headerCell.AddCell("Российская Федерация");

            headerLayout1.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 16;
            e.Layout.Bands[0].Columns[0].Width = 128;
            e.Layout.Bands[0].Columns[1].Width = 130;
            e.Layout.Bands[0].Columns[2].Width = 87;
            e.Layout.Bands[0].Columns[3].Width = 87;
            e.Layout.Bands[0].Columns[4].Width = 176;
            e.Layout.Bands[0].Columns[5].Width = 155;
        }

        #endregion

        private bool VeryOldData(DataTable dtChart)
        {
            DateTime chartDate = CRHelper.DateByPeriodMemberUName(dtChart.Rows[dtChart.Rows.Count - 1][1].ToString(), 3);
            return currentDate.AddMonths(-1) > chartDate;
        }

        void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0004_0003_chart2_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            if (dtChart.Rows.Count == 0 ||
                VeryOldData(dtChart))            
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
            string query = DataProvider.GetQueryText(String.Format("Oil_0004_0003_chart3_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);
            UltraChart3.Axis.Y.RangeMin = 18;
            UltraChart3.Axis.Y.RangeMax = 30;

            if (dtChart.Rows.Count == 0 ||
                VeryOldData(dtChart))
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
               

        void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Oil_0004_0003_chart5_v"));
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            if (dtChart.Rows.Count == 0 ||
                VeryOldData(dtChart))
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

            chart.Axis.X.Margin.Near.Value = 3;
            chart.Axis.X.Margin.Far.Value = 2;
            chart.Axis.Y.Margin.Far.Value = 3;

            chart.Data.ZeroAligned = false;

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
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                    }
                }
            }
        }

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
