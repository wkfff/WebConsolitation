using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0001_0007 : CustomReportPage
    {
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraChartGrownTemp.Width = 750;
            UltraChartGrownTemp.Height = 257;
            //UltraChartGrownTemp.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartGrownTemp_FillSceneGraph);
            DataTable dtGrownTemp = GetDtGrownTemp();
            SetUpColumnChart(UltraChartGrownTemp);
            UltraChartGrownTemp.DataSource = dtGrownTemp;
            UltraChartGrownTemp.DataBind();

            UltraChart1.Width = 750;
            UltraChart1.Height = 257;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartGrownTemp_FillSceneGraph);
            //UltraChartGrownTemp.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartGrownTemp_FillSceneGraph);
            DataTable dtGrownTemp1 = GetDtGrownTemp1();
            SetUpAreaChart(UltraChart1);
            UltraChart1.DataSource = dtGrownTemp1;
            UltraChart1.DataBind();

            lbVolumeDescription.Text = "Число занятых в сфере ИТ в&nbsp;<span class=\"DigitsValueLarge\">2009</span>&nbsp;году составляет<br/><span class=\"DigitsValueLarge\">1 млн. 20 тыс.</span>&nbsp;человек (<span class=\"DigitsValueLarge\">1,34%</span>&nbsp;от трудоспособного населения). Из них&nbsp;<span class=\"DigitsValueLarge\">29%</span>&nbsp;(<span class=\"DigitsValueLarge\">299</span>&nbsp;тыс. человек) работают на предприятиях ИТ-индустрии, а&nbsp;<span class=\"DigitsValueLarge\">71%</span>&nbsp;(<span class=\"DigitsValueLarge\">721</span>&nbsp;тыс. человек) – на предприятиях других отраслей народного хозяйства.<br/>";
            Label3.Text = "<br/>Персонал ИТ-компаний распределен по основным сегментам ИТ-рынка следующим образом:";
            Label4.Text = "1,020 млн.чел.";
            BindTagCloud();

            HyperLink4.NavigateUrl = "http://www.apkit.ru";
            HyperLink4.Text = "АП КИТ";
            HyperLink4.ForeColor = Color.White;
        }

        private void BindTagCloud()
        {
            Dictionary<string, int> tags = new Dictionary<string, int>();

            FillTagsDirectOrder(tags);

            TagCloud1.ForeColor = Color.FromArgb(192, 192, 192);
            TagCloud1.startFontSize = 10;
            TagCloud1.fontStep = 2;
            TagCloud1.groupCount = 4;
           // TagCloud1.floatStyle = FloatStyle.Left;
            TagCloud1.Render(tags);
        }

        private static void FillTagsDirectOrder(Dictionary<string, int> tags)
        {
            tags.Add(String.Format("ИТ-услуги 46%"), 901);
            tags.Add(String.Format("Программное обеспечение 23%"), 701);
            tags.Add(String.Format("Экспорт ИТ-услуг 17%"), 501);
            tags.Add(String.Format("Аппаратное обеспечение 14%"), 301);
        }

        void UltraChartGrownTemp_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Primitive[] primitives = new Primitive[6];
            int textNum = 0;
            int count = 0;
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
                        if (textNum < 6)
                        {
                            text.bounds.Y -= 10;
                            //primitives[count] = e.SceneGraph[i];
                            //count++;
                        }
                        else {
                            text.bounds.Y += 30;
                            //primitives[count] = e.SceneGraph[i];
                            //count++;
                        }
                        
                        textNum++;
                    }
                }
            }
        }

        private void SetUpColumnChart(UltraChart chart)
        {
            chart.ChartType = ChartType.ColumnChart;
            chart.Data.ZeroAligned = true;
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /> <DATA_VALUE:N0> тыс.чел.</span>";
            chart.Legend.SpanPercentage = 20;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
            appearance.ChartTextFont = new Font("Verdana", 12);
            appearance.FontColor = Color.White;//Color.FromArgb(192, 192, 192);
            appearance.Visible = true;
            chart.ColumnChart.ChartText.Add(appearance);

            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
           // chart.ColumnChart.SeriesSpacing = 0;

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
            chart.Axis.X.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            chart.Axis.Y.Labels.Font = new Font("Verdana", 12);
            chart.Axis.Y.Extent = 60;
            chart.Axis.X.Extent = 20;

            chart.Legend.Font = new Font("Verdana", 12);

            SetupTitleLeft(chart, "тыс.чел.", 30);
        }

        private void SetUpAreaChart(UltraChart chart)
        {
            chart.ChartType = ChartType.SplineChart;
            chart.Data.ZeroAligned = true;
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /> <DATA_VALUE:N0>%</span>";
            chart.Legend.SpanPercentage = 20;

            chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Far.Value = 30;
            chart.Axis.X2.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Far.Value = 30;
           
            chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X.Margin.Near.Value = 30;
            chart.Axis.X2.Margin.Near.MarginType = LocationType.Pixels;
            chart.Axis.X2.Margin.Near.Value = 30;
            
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
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

            chart.Axis.X.Labels.Font = new Font("Verdana", 12);
            chart.Axis.Y.Labels.Font = new Font("Verdana", 12);
            chart.Axis.Y.Extent = 60;
            chart.Axis.X.Extent = 20;

            chart.Legend.Font = new Font("Verdana", 12);

            AddLineAppearencesUltraChart(chart, Color.FromArgb(78, 230, 228), Color.Red);
            
            SetupTitleLeft(chart, "тыс.чел.", 30);
        }

        private void AddLineAppearencesUltraChart(UltraChart chart, Color SeriesColor1, Color SeriesColor2)
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
            appearance.ChartTextFont = new Font("Verdana", 12);
            appearance.FontColor = Color.White; //Color.FromArgb(192, 192, 192);
            appearance.Visible = true;
            chart.SplineChart.ChartText.Add(appearance);
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

        private static DataTable GetDtGrownTemp()
        {
            DataTable dtGrownTemp = new DataTable();

            dtGrownTemp.Columns.Add(new DataColumn("Год", typeof(string)));
            dtGrownTemp.Columns.Add(new DataColumn("Потребность (модернизация)", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("Потребность (инерция)", typeof(double)));

            DataRow row = dtGrownTemp.NewRow();
            row[0] = "2010 год";
            row[1] = 167;
            row[2] = 81;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2011 год";
            row[1] = 204;
            row[2] = 76;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2012 год";
            row[1] = 261;
            row[2] = 78;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2013 год";
            row[1] = 326;
            row[2] = 92;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2014 год";
            row[1] = 347;
            row[2] = 88;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2015 год";
            row[1] = 356;
            row[2] = 95;
            dtGrownTemp.Rows.Add(row);

            return dtGrownTemp;
        }

        private static DataTable GetDtGrownTemp1()
        {
            DataTable dtGrownTemp = new DataTable();

            dtGrownTemp.Columns.Add(new DataColumn("Год", typeof(string)));
            dtGrownTemp.Columns.Add(new DataColumn("Занятость (модернизация)", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("Занятость (инерция)", typeof(double)));

            DataRow row = dtGrownTemp.NewRow();
            row[0] = "2010 год";
            row[1] = 1106;
            row[2] = 1021;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2011 год";
            row[1] = 1239;
            row[2] = 1027;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2012 год";
            row[1] = 1421;
            row[2] = 1036;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2013 год";
            row[1] = 1657;
            row[2] = 1047;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2014 год";
            row[1] = 1896;
            row[2] = 1063;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "2015 год";
            row[1] = 2129;
            row[2] = 1082;
            dtGrownTemp.Rows.Add(row);

            return dtGrownTemp;
        }
    }
}
