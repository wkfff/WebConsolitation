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
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0001_0005 : CustomReportPage
    {
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            lbMarketConsumeDescription.Text =
                "Российский ИТ-рынок считается бизнес-ориентированным, на&nbsp;<span class=\"DigitsValueLarge\">корпоративный сектор</span>&nbsp;приходится порядка&nbsp;<span class=\"DigitsValueLarge\">55%</span>&nbsp;ИТ-затрат в стране.";
            Label4.Text = "В структуре затрат на ИТ преобладает поддержка существующих систем (<span class=\"DigitsValueLarge\">68%</span>), инвестиции в дальнейшее развитие ИТ составляют около&nbsp;<span class=\"DigitsValueLarge\">32%</span>.";
            reportDate = new DateTime(2010, 6, 1);
           
            UltraChartConsume.Width = 400;
            DataTable dtConsume = GetDtConsume();
            SetUpPieChart(UltraChartConsume);

            UltraChartConsume.DataSource = dtConsume;
            UltraChartConsume.DataBind();

            HyperLink1.NavigateUrl = "http://www.real-it.ru";
            HyperLink1.Text = "REAL-IT";
            HyperLink1.ForeColor = Color.White;

            HyperLink2.NavigateUrl = "http://www.gartner.com";
            HyperLink2.Text = "Gartner";
            HyperLink2.ForeColor = Color.White;

            HyperLink3.NavigateUrl = "http://www.real-it.ru";
            HyperLink3.Text = "REAL-IT";
            HyperLink3.ForeColor = Color.White;

            HyperLink4.NavigateUrl = "http://www.real-it.ru";
            HyperLink4.Text = "REAL-IT";
            HyperLink4.ForeColor = Color.White;

            BindTagCloud();

            UltraChart1.Width = 750;
            UltraChart1.Height = 380;
            SetUpColumnChart(UltraChart1);
            DataTable dtRegional = GetDtRegional();
            UltraChart1.DataSource = dtRegional;
            UltraChart1.DataBind();
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
            chart.DoughnutChart.RadiusFactor = 75;
            chart.Tooltips.FormatString = "";
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

        private void SetUpColumnChart(UltraChart chart)
        {
            
            chart.ChartType = ChartType.StackBarChart;
            chart.Data.ZeroAligned = true;
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /> <DATA_VALUE_ITEM:N1>%</span>";
            chart.Legend.SpanPercentage = 39;

            chart.ColumnChart.SeriesSpacing = 0;

            chart.ColumnChart.NullHandling = NullHandling.DontPlot;

            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dash;
            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;

            chart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Labels.Font = new Font("Verdana", 12);

            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            
            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 12);

            chart.Axis.Y.Labels.SeriesLabels.WrapText = true;
            chart.Axis.Y.Labels.SeriesLabels.FontSizeBestFit = false;
            chart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            //chart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            //ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            //behavior.ClipText = false;
            //behavior.HideText = false;
            //behavior.Trimming = StringTrimming.None;
            //chart.Axis.Y.Labels.SeriesLabels.Layout.BehaviorCollection.Add(behavior);

            chart.Axis.Y.Extent = 200;
            chart.Axis.X.Extent = 20;

            chart.Legend.Font = new Font("Verdana", 12);

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 9; i++)
            {
                PaintElement pe = GetGradientPaintElement(GetColorColumnChart(i), 50);

                chart.ColorModel.Skin.PEs.Add(pe);
            }
            chart.ColorModel.Skin.ApplyRowWise = false;
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

        private static Color GetColorColumnChart(int i)
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
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
            }
            return Color.White;
        }

        private static DataTable GetDtConsume()
        {
            DataTable dtMarketSegments = new DataTable();
            dtMarketSegments.Columns.Add(new DataColumn("Сегмент", typeof(string)));
            //dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));
            dtMarketSegments.Columns.Add(new DataColumn("Сумма, млрд.руб.", typeof(double)));

            DataRow row = dtMarketSegments.NewRow();
            row[0] = "Госсектор";
            row[1] = 30;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "Корпоративный\nсектор";
            row[1] = 55;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "Частный\nсектор";
            row[1] = 15;
            dtMarketSegments.Rows.Add(row);

            return dtMarketSegments;
        }

        private static DataTable GetDtRegional()
        {
            DataTable dtGrownTemp = new DataTable();

            dtGrownTemp.Columns.Add(new DataColumn("Сектор", typeof(string)));
            dtGrownTemp.Columns.Add(new DataColumn("г.Москва", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("УФО", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("ПФО", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("ЦФО (без г.Москвы)", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("СФО", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("г.Санкт-Петербург", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("СЗФО (без г.СПб)", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("ЮФО", typeof(double)));
            dtGrownTemp.Columns.Add(new DataColumn("ДФО", typeof(double)));

            DataRow row = dtGrownTemp.NewRow();
            row[0] = "Общий объем затрат\nна ИТ-услуги\nвнешних организаций ";
            row[1] = 31.1;
            row[2] = 17.5;
            row[3] = 16.6;
            row[4] = 8.8;
            row[5] = 8.6;
            row[6] = 6.2;
            row[7] = 4.3;
            row[8] = 4.2;
            row[9] = 2.7;
            dtGrownTemp.Rows.Add(row);

            row = dtGrownTemp.NewRow();
            row[0] = "Общий объем\nвнутренних ИТ-затрат";
            row[1] = 26.4;
            row[2] = 13.6;
            row[3] = 27.1;
            row[4] = 9.2;
            row[5] = 9.3;
            row[6] = 6.5;
            row[7] = 2.9;
            row[8] = 2.8;
            row[9] = 2.2;
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

        private void BindTagCloud()
        {
            Dictionary<string, int> tags = new Dictionary<string, int>();

            FillTagsDirectOrder(tags);

            //tags.Add(String.Format("Микротест (6 940 млрд.руб.)"),6940048);
            // tags.Add(String.Format("Инфосистемы Джет (6 732 млрд.руб.)"), 6732250);
            // tags.Add(String.Format("АМТ-Груп (6 195 млрд.руб.)"), 6195732);
            // tags.Add(String.Format("ЭкоПрог (5 935 млрд.руб.)"), 5935000);
            // tags.Add(String.Format("Рамэк-ВС (5 320 млрд.руб.)"), 5320000);
            //tags.Add(String.Format("Epam Systems (4 780 млрд.руб.)"), 4780012);
            //tags.Add(String.Format("АйТи (4 189 млрд.руб.)"), 4189790);
            //tags.Add(String.Format("Центр финансовых технологий (4 163 млрд.руб.)"), 4163003);
            //tags.Add(String.Format("ИТСК (3 855 млрд.руб.)"), 3855060);
            //tags.Add(String.Format("Армада (3 785 млрд.руб.)"), 3785000);

            TagCloud1.ForeColor = Color.FromArgb(192, 192, 192);
            TagCloud1.startFontSize = 12;
            TagCloud1.fontStep = 4;

            TagCloud1.Render(tags);
        }

        private static void FillTagsDirectOrder(Dictionary<string, int> tags)
        {
            tags.Add(String.Format("Финансы (20%)"), 1000);
            tags.Add(String.Format("Энергетика (19%)"), 500);
            tags.Add(String.Format("Телеком (7%)"), 250);
            tags.Add(String.Format("Торговля (3%)"), 100);
            tags.Add(String.Format("Металлургия (3%)"), 100);
        }
    }
}
