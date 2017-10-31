using System;
using System.Data;
using System.Drawing;
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
    public partial class IT_0001_0001 : CustomReportPage
    {
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            lbVolumeDescription.Text =
                "Объем российского ИТ-рынка в&nbsp;<span class=\"DigitsValueLarge\">2009</span>&nbsp;году составил&nbsp;<span class=\"DigitsValueLarge\">496,5</span>&nbsp;млрд.рублей.<br/>ИТ-рынок в общем объеме ВВП занимает &nbsp;<span class=\"DigitsValueLarge\">1,4%</span>.";

            lbGrownTempDescription.Text = "В&nbsp;<span class=\"DigitsValueLarge\">2009</span>&nbsp;году объем ИТ-рынка сократился на &nbsp;<span class=\"DigitsValueLarge\">13%</span>&nbsp;в сравнении с &nbsp;<span class=\"DigitsValueLarge\">2008</span>&nbsp;годом. В&nbsp;<span class=\"DigitsValueLarge\">2010</span>&nbsp;и&nbsp;<span class=\"DigitsValueLarge\">2011</span>&nbsp;годах ожидается рост по разным вариантам прогноза от&nbsp;<span class=\"DigitsValueLarge\">1%</span>&nbsp;до&nbsp;<span class=\"DigitsValueLarge\">21%</span>.";

            lbMarketSegmentsDescription.Text =
                "В развитых странах доля ИТ-услуг наиболее высокая. Предполагается, что и в России кризис ускорит процесс перераспределения долей сегментов ИТ-рынка в сторону<br/>ИТ-услуг.";

            reportDate = new DateTime(2010, 6, 1);

            UltraChartGrownTemp.Width = 750;
            UltraChartGrownTemp.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartGrownTemp_FillSceneGraph);
            DataTable dtGrownTemp = GetDtGrownTemp();
            SetUpColumnChart(UltraChartGrownTemp);
            UltraChartGrownTemp.DataSource = dtGrownTemp;
            UltraChartGrownTemp.DataBind();

            UltraChartMarketSegments.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartMarketSegments_FillSceneGraph);
            UltraChartMarketSegments.Width = 750;
            DataTable dtMarketSegments = GetDtMarketSegments();
            SetUpPieChart(UltraChartMarketSegments);
            
            UltraChartMarketSegments.DataSource = dtMarketSegments;
            UltraChartMarketSegments.DataBind();

            HyperLink1.NavigateUrl = "http://www.economy.gov.ru";
            HyperLink1.Text = "Минэкономразвития России";
            HyperLink1.ForeColor = Color.White;
        }

        void UltraChartGrownTemp_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int boxWidth = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        boxWidth = box.rect.Width;
                        if (box.DataPoint.Label.Contains("Факт"))
                        {
                            box.rect.X += box.rect.Width / 2;
                            box.rect.Width = box.rect.Width * 2;
                        }
                        else
                        {
                            box.rect.X -= box.rect.Width / 2;
                        }
                    }
                }
            }
            int textNum = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    if (text.Column == -1 &&
                        text.Row == -1 &&
                        text.Path == null &&
                        text.DataPoint == null)
                    {
                        if (textNum < 2)
                        {
                            text.bounds.X += boxWidth;
                        }
                        else
                        {
                            text.bounds.X -= boxWidth / 2;
                        }
                        textNum++;
                    }
                }
            }
        }

        void UltraChartMarketSegments_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Wedge)
                {
                    Wedge wedge = (Wedge)primitive;
                    if (wedge.DataPoint != null)
                    {
                        if (wedge.DataPoint.Label.Contains("Аппаратное"))
                        {
                            wedge.DataPoint.Label = GetHardvareDetalization();
                        }
                        else if (wedge.DataPoint.Label.Contains("Программное"))
                        {
                            wedge.DataPoint.Label = GetSoftwareDetalization();
                        }
                        else if (wedge.DataPoint.Label.Contains("ИТ-услуги"))
                        {
                            wedge.DataPoint.Label = GetServeDetalization();
                        }

                    }
                }
            }
        }

        private string GetHardvareDetalization()
        {
            string detalization =
                String.Format(
                    "Объем сегмента аппаратного обеспечения в&nbsp;<b>2009<b/>&nbsp;году составил <b>255,2<b/>&nbsp;млрд.руб. (<b>20%<b/>&nbsp;от общего объема внутреннего ИТ-рынка), в том числе:");
            return detalization;
        }

        private string GetSoftwareDetalization()
        {
            string detalization =
                String.Format(
                    "Объем сегмента программного обеспечения в&nbsp;<b>2009<b/>&nbsp;году составил <b>99б3<b/>&nbsp;млрд.руб. (<b>51,4%<b/>&nbsp;от общего объема внутреннего ИТ-рынка), в том числе:");
            return detalization;
        }

        private string GetServeDetalization()
        {
            string detalization =
                String.Format(
                    "Объем сегмента ИТ-услуг в&nbsp;<b>2009<b/>&nbsp;году составил <b>142<b/>&nbsp;млрд.руб. (<b>28,6%<b/>&nbsp;от общего объема внутреннего ИТ-рынка), в том числе:");
            return detalization;
        }

        private void SetUpPieChart(UltraChart chart)
        {
            chart.ChartType = ChartType.DoughnutChart;

            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 5;
            chart.Axis.X.Extent = 5;
            
            chart.Legend.Visible = false;
            
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Labels.Font = new Font("Verdana", 12);
            chart.Axis.X.Labels.FontColor = Color.White;
            
            chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            chart.DoughnutChart.Labels.Font = new Font("Arial", 14);
            chart.DoughnutChart.Labels.FontColor = Color.White;
            chart.DoughnutChart.Labels.LeaderLineColor = Color.White;
            chart.DoughnutChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            chart.DoughnutChart.Labels.LeaderEndStyle = LineCapStyle.DiamondAnchor;
           
            chart.DoughnutChart.StartAngle = 85;
            chart.Tooltips.FormatString = "";
            chart.DoughnutChart.Labels.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> млрд.руб.\n<PERCENT_VALUE:N2>%";

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);
                
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private void SetUpColumnChart(UltraChart chart)
        {
            chart.ChartType = ChartType.ColumnChart;
            chart.Data.ZeroAligned = true;
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /> <DATA_VALUE:N0>%</span>";
            chart.Legend.SpanPercentage = 14;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>%";
            appearance.ChartTextFont = new Font("Verdana", 12);
            appearance.FontColor = Color.White;//Color.FromArgb(192, 192, 192);
            appearance.Visible = true;
            chart.ColumnChart.ChartText.Add(appearance);

            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.ColumnChart.SeriesSpacing = 0;

            chart.ColumnChart.NullHandling = NullHandling.DontPlot;

            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dash;
            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;

            chart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = true;

            chart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            chart.Axis.Y.Labels.Font = new Font("Verdana", 12);
            chart.Axis.Y.Extent = 60;
            chart.Axis.X.Extent = 40;

            chart.Legend.Font = new Font("Verdana", 12);

            SetupTitleLeft(chart, "% к предыдущему году", 30);
        }

        private static void SetupTitleLeft(UltraChart chart, string title, int marginBottom)
        {
            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = title;
            chart.TitleLeft.Font = new Font("Verdana", 12);
            chart.TitleLeft.WrapText = true;
            chart.TitleLeft.FontColor = Color.White;
            chart.TitleLeft.VerticalAlign = StringAlignment.Center;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Margins.Bottom = marginBottom;
        }

        private static DataTable GetDtMarketSegments()
        {
            DataTable dtMarketSegments = new DataTable();
            dtMarketSegments.Columns.Add(new DataColumn("Сегмент", typeof(string)));
            //dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));
            dtMarketSegments.Columns.Add(new DataColumn("Сумма, млрд.руб.", typeof(double)));

            DataRow row = dtMarketSegments.NewRow();
            row[0] = "Аппаратное обеспечение";
            row[1] = 255.2;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "Программное обеспечение";
            row[1] = 99.3;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "ИТ-услуги";
            row[1] = 141;
            dtMarketSegments.Rows.Add(row);

            return dtMarketSegments;
        }

        private static DataTable GetDtGrownTemp()
        {
            DataTable dtGrownTemp = new DataTable();

            dtGrownTemp.Columns.Add(new DataColumn("Год", typeof(string)));
            dtGrownTemp.Columns.Add(new DataColumn("Факт", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("Прогноз, вариант 1", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("Прогноз, вариант 2", typeof(double)));

            DataRow row = dtGrownTemp.NewRow();
            row[0] = "2008 год";
            row[1] = 102;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2009 год";
            row[1] = 87;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2010 год";
            row[2] = 101;
            row[3] = 121;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2011 год";
            row[2] = 101;
            row[3] = 121;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2012 год";
            row[2] = 104;
            row[3] = 124;
            dtGrownTemp.Rows.Add(row);

            return dtGrownTemp;
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(192, 178, 224);
                    }
                case 2:
                    {
                        return Color.FromArgb(214, 171, 133);
                    }
                case 3:
                    {
                        return Color.FromArgb(245, 187, 102);
                    }
            }
            return Color.White;
        }
        
        private static Color GetStopColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(44, 20, 91);
                    }
                case 2:
                    {
                        return Color.FromArgb(138, 71, 10);
                    }
                case 3:
                    {
                        return Color.FromArgb(229, 140, 13);
                    }
            }
            return Color.White;
        }
    }
}
