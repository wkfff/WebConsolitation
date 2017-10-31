using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class OIL_0004_0002_Chart : UserControl
    {        
        public int ChartWidth = 750;
        public int ChartHeight = 235;

        private double foAvg;
        private double rfAvg;

        private string queryName = "Oil_0004_0002_chart";

        public string QueryName
        {
            get
            {
                return queryName;
            }
            set
            {
                queryName = value;
            }
        }

        private Color appearanceColor = Color.Red;

        public Color AppearanceColor
        {
            get
            {
                return appearanceColor;
            }
            set
            {
                appearanceColor = value;
            }
        }

        private double rfMiddleLevel = 0;

        public double RfMiddleLevel
        {
            get
            {
                return rfMiddleLevel;
            }
            set
            {
                rfMiddleLevel = value;
            }
        }

        private double foMiddleLevel = 0;

        public double FoMiddleLevel
        {
            get
            {
                return foMiddleLevel;
            }
            set
            {
                foMiddleLevel = value;
            }
        }

        private DateTime reportDate;
        public DateTime ReportDate
        {
            get
            {
                return reportDate;
            }
            set
            {
                reportDate = value;
            }
        }

        private DateTime lastDate;
        public DateTime LastDate
        {
            get
            {
                return lastDate;
            }
            set
            {
                lastDate = value;
            }
        }

        private DateTime firstDate;
        public DateTime FirstDate
        {
            get
            {
                return firstDate;
            }
            set
            {
                firstDate = value;
            }
        }

        private string taxName = "Бензин марки АИ-80";

        public string TaxName
        {
            get
            {
                return taxName;
            }
            set
            {
                taxName = value;
            }
        }

        private string oilId = "1";

        public string OilId
        {
            get
            {
                return oilId;
            }
            set
            {
                oilId = value;
            }
        }

        DataTable dtChart;
        private string currentFO = Core.CustomParam.CustomParamFactory("region").Value;

        protected void Page_Load(object sender, EventArgs e)
        {
            CustomParams.MakeOilParams(oilId, "id");

            Core.CustomParam.CustomParamFactory("region").Value = RegionsNamingHelper.FullName(Core.CustomParam.CustomParamFactory("region").Value.Replace("УФО", "УрФО"));
            currentFO = Core.CustomParam.CustomParamFactory("region").Value;            
            IncomesHeader.Text = taxName;

            DataTable dtLabel = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0004_0002_Label");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtLabel);

            foAvg = Convert.ToDouble(dtLabel.Rows[0][6]);
            rfAvg = Convert.ToDouble(dtLabel.Rows[0][1]);

            SetColumnChartAppearance();

            string description = rfAvg > foAvg ? String.Format("<img src=\"../../../images/ballGreenBB.png\">&nbsp;ниже чем по РФ на&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;руб.", rfAvg - foAvg) :
                rfAvg < foAvg ? String.Format("<img src=\"../../../images/ballRedBB.png\">&nbsp;выше чем по РФ на&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;руб.", foAvg - rfAvg) :
                "соответствует уровню РФ";

            double value = Convert.ToDouble(dtLabel.Rows[0][7].ToString());
            string grownDescription = value < 0 ?
                String.Format("уменьшилась&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\">&nbsp;на&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;руб. темп прироста&nbsp;<span class='DigitsValue'>{1:P1}</span>", Convert.ToDouble(dtLabel.Rows[0][7].ToString().Replace("-", "")), dtLabel.Rows[0][8]) :
                value > 0 ?
                String.Format("увеличилась&nbsp;<img src=\"../../../images/arrowRedUpBB.png\">&nbsp;на&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;руб. темп прироста&nbsp;<span class='DigitsValue'>{1:P1}</span>", dtLabel.Rows[0][7], dtLabel.Rows[0][8]) :
                "не изменилась";

            Label1.Text = String.Format(
@"На&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy} г.</span>&nbsp;в&nbsp;<span class='DigitsValue'>{1}</span>&nbsp;средняя розничная цена на {2} составила&nbsp;<span class='DigitsValueXLarge'>{3:N2}</span>&nbsp;руб.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{4}<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;с&nbsp;<span class='DigitsValue'>{5:dd.MM.yyyy} г.</span>&nbsp;цена {6}<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;с&nbsp;<span class='DigitsValue'>{9:dd.MM.yyyy} г.</span>&nbsp;цена увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png'>&nbsp;на&nbsp;<span class='DigitsValue'>{7:N2}</span>&nbsp;руб. темп прироста&nbsp;<span class='DigitsValue'>{8:P1}</span>",
                reportDate, RegionsNamingHelper.ShortRegionsNames[currentFO], CRHelper.ToLowerFirstSymbol(taxName), dtLabel.Rows[0][6], description, lastDate, grownDescription, dtLabel.Rows[0][9], dtLabel.Rows[0][10],firstDate);
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

        private void SetColumnChartAppearance()
        {
            AddLineAppearencesUltraChart1(UltraChart1, appearanceColor);

            UltraChart1.Width = ChartWidth;
            UltraChart1.Height = ChartHeight;

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Data.ZeroAligned = false;
            UltraChart1.ColumnChart.NullHandling = NullHandling.DontPlot;
            UltraChart1.ColumnChart.SeriesSpacing = 0;

            UltraChart1.Axis.X.Extent = 60;
           // UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart1.Axis.Y.Extent = 54;

            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);

            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y2.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y2.MajorGridLines.Color = Color.Black;

            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";

            UltraChart1.Axis.X.Labels.WrapText = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart1.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            UltraChart1.Legend.Visible = false;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.Text = "руб. за литр";
            UltraChart1.TitleLeft.Margins.Bottom = 100;
            //UltraChart.TitleLeft.Margins.Left = 17;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Far;

            UltraChart1.BackColor = Color.Transparent;

            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL>\nСредняя цена\n<b><DATA_VALUE:N2><b> руб.</span>";

            dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            dtChart.Columns.RemoveAt(0);

            double max = Math.Max(Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 1][1]), rfAvg);
            double min = Math.Min(Convert.ToDouble(dtChart.Rows[0][1]), rfAvg);

            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
            UltraChart1.Axis.Y.RangeMax = max + 1;
            UltraChart1.Axis.Y.RangeMin = min - 3;

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName =
                    col.ColumnName.Replace("Бензин марки", "").Replace("Дизельное топливо", "ДТ");
            }

            UltraChart1.Data.SwapRowsAndColumns = true;
            //UltraChart1.DataSource = dtChart;
            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
            UltraChart1.DataBind();
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
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.Series != null)
                    {
                        box.Series.Label = RegionsNamingHelper.FullName(box.Series.Label);
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

            RFheader = string.Format("Средняя по {0}: {1:N2} {2}", "РФ", rfAvg, "руб.");

            RFStartLineX = xAxis.Map(xAxis.Minimum);
            RFStartLineY = yAxis.Map(rfAvg);

            RFEndLineX = xAxis.Map(xAxis.Maximum);
            RFEndLineY = yAxis.Map(rfAvg);

            GenHorizontalLineAndLabel((int)RFStartLineX, (int)RFStartLineY, (int)RFEndLineX, (int)RFEndLineY,
                    Color.Crimson, RFheader, e, rfAvg > foAvg);

            RFStartLineX = 0;
            RFStartLineY = 0;

            RFEndLineX = 0;
            RFEndLineY = 0;
            RFheader = "";

            RFheader = string.Format("Средняя по {0}: {1:N2} {2}", RegionsNamingHelper.ShortName(currentFO), foAvg, "руб.");

            RFStartLineX = xAxis.Map(xAxis.Minimum);
            RFStartLineY = yAxis.Map(foAvg);

            RFEndLineX = xAxis.Map(xAxis.Maximum);
            RFEndLineY = yAxis.Map(foAvg);

            GenHorizontalLineAndLabel((int)RFStartLineX, (int)RFStartLineY, (int)RFEndLineX, (int)RFEndLineY,
                    Color.Cyan, RFheader, e, rfAvg < foAvg);
        }

        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
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
                textLabel.bounds = new System.Drawing.Rectangle(startX + 20, StartY - 20, 500, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 20, StartY + 1, 500, 15);
            }

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);

            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);
        }
    }
}