using System;
using System.Data;
using System.Drawing;
using System.Web;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0001_0003 : CustomReportPage
    {
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            lbGrownTempDescription.Text = "Выручка российских компаний, занимающихся экспортной разработкой ПО в&nbsp;<span class=\"DigitsValueLarge\">2009</span>&nbsp;году составила&nbsp;<span class=\"DigitsValueLarge\">2,65</span>&nbsp;млрд.долл. США и сохранилась на уровне&nbsp;<span class=\"DigitsValueLarge\">2008</span>&nbsp;года. Прогнозируется, что в&nbsp;<span class=\"DigitsValueLarge\">2010</span>&nbsp;году объем экспорта ИТ вырастет на&nbsp;<span class=\"DigitsValueLarge\">13%</span>&nbsp;и составит&nbsp;<span class=\"DigitsValueLarge\">3</span>&nbsp;млрд.долл. США. На&nbsp;<span class=\"DigitsValueLarge\">2011</span>&nbsp;год прогнозируется рост экспорта на&nbsp;<span class=\"DigitsValueLarge\">10%</span>.";

            lbMarketSegmentsDescription.Text =
                 "Около&nbsp;<span class=\"DigitsValueLarge\">28%</span>&nbsp;в совокупной выручке приходятся на экспортеров коробочного ПО (Abbyy, «Лаборатория Касперского» и др.),&nbsp;<span class=\"DigitsValueLarge\">52%</span>&nbsp;составляет разработка заказного программного обеспечения (Luxoft, Epam и др.) и оставшиеся&nbsp;<span class=\"DigitsValueLarge\">20%</span>&nbsp;занимают расположенные в России центры разработки зарубежных компаний.";
           

            reportDate = new DateTime(2010, 6, 1);

            UltraChartGrownTemp.Width = 750;
            UltraChartGrownTemp.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartGrownTemp_FillSceneGraph);
            DataTable dtGrownTemp = GetDtGrownTemp();
            SetUpLineChart(UltraChartGrownTemp);
            UltraChartGrownTemp.DataSource = dtGrownTemp;
            UltraChartGrownTemp.DataBind();

           // UltraChartMarketSegments.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartMarketSegments_FillSceneGraph);
            UltraChartMarketSegments.Width = 750;
            DataTable dtMarketSegments = GetDtMarketSegments();
            SetUpPieChart(UltraChartMarketSegments);
            
            UltraChartMarketSegments.DataSource = dtMarketSegments;
            UltraChartMarketSegments.DataBind();

            HyperLink1.NavigateUrl = "http://www.russoft.ru";
            HyperLink1.Text = "Руссофт";
            HyperLink1.ForeColor = Color.White;
        }

        void UltraChartGrownTemp_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
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
                        text.bounds.Y -= 10;
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
            chart.Tooltips.FormatString = "<ITEM_LABEL><br/><PERCENT_VALUE:N0>%";
            chart.DoughnutChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N0>%";


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

        private void SetUpLineChart(UltraChart chart)
        {
            chart.ChartType = ChartType.SplineChart;
            chart.Data.SwapRowsAndColumns = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL> <br /> <DATA_VALUE:N0> млн.долл. США</span>";
           
            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;
            chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Far.Value = 30;
            chart.Axis.Y2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y2.Margin.Far.Value = 10;
            chart.Axis.X2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Far.Value = 30;

            chart.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Near.Value = 10;
            chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Near.Value = 20;
            chart.Axis.Y2.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.Y2.Margin.Near.Value = 10;
            chart.Axis.X2.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Near.Value = 20;

            chart.Legend.Visible = false;
           
            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(92, 92, 92);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dash;
            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;

            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.FontSizeBestFit = false;
            chart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            chart.Axis.X.Labels.WrapText = false;
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;

            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.ClipText = false;
            behavior.HideText = false;
            behavior.Trimming = StringTrimming.None;
            chart.Axis.X.Labels.Layout.BehaviorCollection.Add(behavior);

            chart.Axis.X.Labels.HorizontalAlign = StringAlignment.Center;
            chart.Axis.X.Labels.Font = new Font("Verdana", 12);
            chart.Axis.Y.Labels.Font = new Font("Verdana", 12);
            chart.Axis.Y.Extent = 50;
            chart.Axis.X.Extent = 40;

            SetupTitleLeft(chart, "млн.долл. США", 90);
            AddLineAppearencesUltraChart(chart, Color.FromArgb(78, 230, 228));
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

        private void AddLineAppearencesUltraChart(UltraChart chart, Color SeriesColor)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();
            
            PaintElement pe = new PaintElement();
            Color color = Color.White;
            Color stopColor = Color.White;
           
            color = SeriesColor;
            stopColor = SeriesColor;

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

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
            appearance.ChartTextFont = new Font("Verdana", 12);
            appearance.FontColor = Color.White;//Color.FromArgb(192, 192, 192);
            appearance.Visible = true;
            chart.SplineChart.ChartText.Add(appearance);
        }

        private static DataTable GetDtMarketSegments()
        {
            DataTable dtMarketSegments = new DataTable();
            dtMarketSegments.Columns.Add(new DataColumn("Сегмент", typeof(string)));
            //dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));
            dtMarketSegments.Columns.Add(new DataColumn("Сумма, млрд.руб.", typeof(double)));

            DataRow row = dtMarketSegments.NewRow();
            row[0] = "Разработка заказного ПО";
            row[1] = 52;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "Продажа коробочного ПО";
            row[1] = 28;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "Разработка ПО в R&D центрах";
            row[1] = 20;
            dtMarketSegments.Rows.Add(row);

            return dtMarketSegments;
        }

        private static DataTable GetDtGrownTemp()
        {
            DataTable dtGrownTemp = new DataTable();

            dtGrownTemp.Columns.Add(new DataColumn("Год", typeof(string)));
            dtGrownTemp.Columns.Add(new DataColumn("Факт", typeof(double)));

            DataRow row = dtGrownTemp.NewRow();
            row[0] = "2004";
            row[1] = 760;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2005";
            row[1] = 972;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2006";
            row[1] = 1450;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2007";
            row[1] = 2200;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2008";
            row[1] = 2650;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2009";
            row[1] = 2650;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2010\nпрогноз";
            row[1] = 3000;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2011\nпрогноз";
            row[1] = 3300;
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