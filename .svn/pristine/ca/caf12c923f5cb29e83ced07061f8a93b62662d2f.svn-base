using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class IT_0001_0009 : CustomReportPage
    {
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);


            lbCNwesRatingDescription.Text =
                "В структуре рейтинга CNews100 в&nbsp;<span class=\"DigitsValueLarge\">2009</span>&nbsp;году доля компаний, приоритетным направлением деятельности которых являются ИТ-услуги, составляет&nbsp;<span class=\"DigitsValueLarge\">52%</span>";
            lbCNwesRatingDescriptionRight.Text = "(доля увеличилась&nbsp;<img src=\"../../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;на&nbsp;<span class=\"DigitsValueLarge\">8%</span>&nbsp;с&nbsp;<span class=\"DigitsValueLarge\">2008</span>&nbsp;года).";
            lbCNwesRatingDescriptionPartTwo.Text = "Доля региональных компаний в CNews100 составляет&nbsp;<span class=\"DigitsValueLarge\">23%</span>";
            lbCNwesRatingDescriptionPartTwoRight.Text = "(доля уменьшилась&nbsp;<img src=\"../../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;на&nbsp;<span class=\"DigitsValueLarge\">3%</span>&nbsp;с&nbsp;<span class=\"DigitsValueLarge\">2008</span>&nbsp;года).";
            lbCNwesRatingDescriptionPartThree.Text = "Доля региональных компаний в совокупной выручке не изменилась  за год и равна&nbsp;<span class=\"DigitsValueLarge\">6%</span>.";
            lbGrownTempDescription.Text =
                "В развитых странах доля ИТ-услуг наиболее высокая. Предполагается, что и в России кризис ускорит процесс перераспределения долей сегментов ИТ-рынка в сторону<br/>ИТ-услуг.";

            reportDate = new DateTime(2010, 6, 1);

            //  UltraChartCNwesRating.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartCNwesRating_FillSceneGraph);
            UltraChartCNwesRating.Width = 545;
            UltraChartCNwesRating.Height = 440;
            DataTable dtCNwesRating = GetDtCNwesRating();
            SetUpPieChart(UltraChartCNwesRating);

            UltraChartCNwesRating.DataSource = dtCNwesRating;
            UltraChartCNwesRating.DataBind();

            HyperLink1.NavigateUrl = "http://www.cnews.ru";
            HyperLink1.Text = "CNews Analytics";

            BindTagCloud();
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
            chart.DoughnutChart.Labels.FontColor = Page.Theme.ToLower() == "iphone" ? Color.White : Color.Black;
            chart.DoughnutChart.Labels.LeaderLineColor = Page.Theme.ToLower() == "iphone" ? Color.White : Color.Black;
            chart.DoughnutChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            chart.DoughnutChart.Labels.LeaderEndStyle = LineCapStyle.DiamondAnchor;

            chart.DoughnutChart.RadiusFactor = 60;
            chart.DoughnutChart.StartAngle = 250;
            chart.Tooltips.FormatString = "";
            chart.DoughnutChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N0>%";

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 5; i++)
            {
                PaintElement pe = GetGradientPaintElement(GetColor(i), 50);
                
                chart.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static PaintElement GetGradientPaintElement(Color fillStartColor, byte fillOpacity)
        {
            PaintElement pe = new PaintElement();
            pe.ElementType = PaintElementType.Gradient;
            pe.Fill = fillStartColor;
            pe.FillStopColor = CRHelper.GetDarkColor(fillStartColor, 20);
            pe.FillOpacity = fillOpacity;
            pe.FillGradientStyle = GradientStyle.BackwardDiagonal;
            return pe;
        }

        private static DataTable GetDtCNwesRating()
        {
            DataTable dtCNwesRating = new DataTable();
            dtCNwesRating.Columns.Add(new DataColumn("Сегмент", typeof(string)));
            //dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));
            dtCNwesRating.Columns.Add(new DataColumn("%", typeof(double)));

            DataRow row = dtCNwesRating.NewRow();
            row[0] = "ИТ-услуги";
            row[1] = 52;
            dtCNwesRating.Rows.Add(row);

            row = dtCNwesRating.NewRow();
            row[0] = "Разработка ПО";
            row[1] = 16;
            dtCNwesRating.Rows.Add(row);

            row = dtCNwesRating.NewRow();
            row[0] = "Группа\nкомпаний";
            row[1] = 16;
            dtCNwesRating.Rows.Add(row);

            row = dtCNwesRating.NewRow();
            row[0] = "Производство и\nдистрибьюция АО";
            row[1] = 12;
            dtCNwesRating.Rows.Add(row);

            row = dtCNwesRating.NewRow();
            row[0] = "Другое";
            row[1] = 4;
            dtCNwesRating.Rows.Add(row);

            return dtCNwesRating;
        }


        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.LawnGreen;
                    }
                case 2:
                    {
                        return Color.Magenta;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.Cyan;
                    }
            }
            return Color.White;
        }

        private void BindTagCloud()
        {
            Dictionary<string, int> tags = new Dictionary<string, int>();

            FillTagsDirectOrder(tags);

            TagCloud1.ForeColor = Page.Theme.ToLower() == "iphone" ? Color.FromArgb(192, 192, 192) : Color.Black;
            TagCloud1.startFontSize = 14;
            TagCloud1.fontStep = 3;
            TagCloud1.groupCount = 4;
            TagCloud1.PaddingTop = "2px";
            TagCloud1.Render(tags);
        }

        private static void FillTagsDirectOrder(Dictionary<string, int> tags)
        {
            tags.Add(String.Format("НКК&nbsp;<img src='../../../images/starYellow.png'>&nbsp;54 млрд.руб.)"), 54432533);
            tags.Add(String.Format("Merlion (52 млрд.руб.)"), 52407277);
            tags.Add(String.Format("ЛАНИТ (34 млрд.руб.)"), 34000000);
            tags.Add(String.Format("Ситроникс (33 млрд.руб.)"), 33000000);
            tags.Add(String.Format("Техносерв (27 млрд.руб.)"), 27770157);
            tags.Add(String.Format("R-Style (20 млрд.руб.)"), 20405623);
            tags.Add(String.Format("Крок (20 млрд.руб.)"), 20011467);
            tags.Add(String.Format("IBS (17 млрд.руб.)"), 17061501);
            tags.Add(String.Format("Энвижн Груп (12 млрд.руб.)"), 12885000);
            tags.Add(String.Format("Компьюлинк (12 млрд.руб.)"), 12858472);
            tags.Add(String.Format("Ай-Теко (12 млрд.руб.)"), 12278865);
            tags.Add(String.Format("Лаборатория Касперского (11 млрд.руб.)"), 11575362);
            tags.Add(String.Format("Verysell (11 млрд.руб.)"), 11439245);
            tags.Add(String.Format("1C (11 млрд.руб.)"), 11050000);
            tags.Add(String.Format("Ниеншанц (10 млрд.руб.)"), 10274184);
            tags.Add(String.Format("Оптима (10 млрд.руб.)"), 10158139);
            tags.Add(String.Format("RRC (8 млрд.руб.)"), 8765014);
            tags.Add(String.Format("Softline (8 млрд.руб.)"), 8518978);
            tags.Add(String.Format("Астерос (8 млрд.руб.)"), 8196973);
            tags.Add(String.Format("ITG&nbsp;<img src='../../../images/starGray.png'>&nbsp;(Inline TechnologiesGroup) (8 млрд.руб.)"), 8085000);
        }

        private static void FillTagsCloudOrder(Dictionary<string, int> tags)
        {
            tags.Add(String.Format("Астерос (8 196 млрд.руб.)"), 8196973);
            tags.Add(String.Format("RRC (8 765 млрд.руб.)"), 8765014);
            tags.Add(String.Format("Ниеншанц (10 274 млрд.руб.)"), 10274184);
            tags.Add(String.Format("Verysell (11 439 млрд.руб.)"), 11439245);
            tags.Add(String.Format("Ай-Теко (12 278 млрд.руб.)"), 12278865);
            tags.Add(String.Format("Энвижн Груп (12 885 млрд.руб.)"), 12885000);
            tags.Add(String.Format("Крок (20 011 млрд.руб.)"), 20011467);
            tags.Add(String.Format("Техносерв (27 770 млрд.руб.)"), 27770157);
            tags.Add(String.Format("ЛАНИТ (34 000 млрд.руб.)"), 34000000);
            tags.Add(String.Format("НКК (54 432 млрд.руб.)"), 54432533);
            tags.Add(String.Format("Merlion (52 407 млрд.руб.)"), 52407277);
            tags.Add(String.Format("Ситроникс (33 000 млрд.руб.)"), 33000000);
            tags.Add(String.Format("R-Style (20 405 млрд.руб.)"), 20405623);
            tags.Add(String.Format("IBS (17 061 млрд.руб.)"), 17061501);
            tags.Add(String.Format("Компьюлинк (12 858 млрд.руб.)"), 12858472);
            tags.Add(String.Format("Лаборатория Касперского (11 575 млрд.руб.)"), 11575362);
            tags.Add(String.Format("1C (11 050 млрд.руб.)"), 11050000);
            tags.Add(String.Format("Оптима (10 158 млрд.руб.)"), 10158139);
            tags.Add(String.Format("Softline (8 518 млрд.руб.)"), 8518978);
            tags.Add(String.Format("ITG (Inline TechnologiesGroup) (8 085 млрд.руб.)"), 8085000);
        }
    }
}
