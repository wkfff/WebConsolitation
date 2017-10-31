using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0004_0003_1 : CustomReportPage
    {
        private DateTime currDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Настройка диаграммы
            
            #endregion

            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0004_0003_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy",dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();

            currDate = CRHelper.PeriodDayFoDate(UserParams.PeriodCurrentDate.Value);

            

           // FO_0004_0003_Chart2.QueryName = "FO_0004_0003_chart2";
           
         //   FO_0004_0003_Chart4.QueryName = "FO_0004_0003_chart4";

            
         //   FO_0004_0003_Chart2.Caption = "Структура назначений областного бюджета";
            
         //   FO_0004_0003_Chart4.Caption = "Структура фактических расходов областного бюджета";

            lbDescription.Text =
                String.Format(
                    "Расходы консолидированного и областного бюджетов Самарской области<br/>по состоянию на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>",
                    currDate.AddMonths(1));

            DataTable dtGauges = new DataTable();
            query = DataProvider.GetQueryText("FO_0004_0003_gauges");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtGauges);

            Label1.Text = String.Format("{0:N2}", dtGauges.Rows[0][1]);
            Label2.Text = String.Format("{0:N2}", dtGauges.Rows[0][2]);
            Label3.Text = String.Format("{0:N2}", dtGauges.Rows[0][3]);

            Label4.Text = String.Format("{0:N2}", dtGauges.Rows[1][1]);
            Label5.Text = String.Format("{0:N2}", dtGauges.Rows[1][2]);
            Label6.Text = String.Format("{0:N2}", dtGauges.Rows[1][3]);

            GaugeIncomesPlaceHolder.Controls.Add(GetGauge(Convert.ToDouble(dtGauges.Rows[0][3].ToString())));
            GaugeOutcomesPlaceHolder.Controls.Add(GetGauge(Convert.ToDouble(dtGauges.Rows[1][3].ToString())));

            UltraChart12.Width = 760;
            UltraChart12.Height = 290;
            UltraChart12.Legend.SpanPercentage = 100;
            UltraChart12.Legend.Font = new Font("Verdana", 12);
            SetupCustomSkin(UltraChart12);

            UltraChart12.Axis.Y.Visible = false;
            UltraChart12.Axis.Y.MinorGridLines.Visible = false;
            UltraChart12.Axis.Y.MajorGridLines.Visible = false;

            UltraChart12.Axis.X.Visible = false;
            UltraChart12.Axis.X.MinorGridLines.Visible = false;
            UltraChart12.Axis.X.MajorGridLines.Visible = false;

            UltraChart12.Axis.X.Labels.Visible = false;
            UltraChart12.Axis.X.Labels.SeriesLabels.Visible = false;

            DataTable dtLegend = new DataTable();
            query = DataProvider.GetQueryText("FO_0004_0003_chart2");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtLegend);

            UltraChart12.DataSource = dtLegend;
            UltraChart12.Data.SwapRowsAndColumns = true;
            UltraChart12.DataBind();

            DataTable dtStrucrure = new DataTable();
            query = DataProvider.GetQueryText("FO_0004_0003_chart_structure");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtStrucrure);

            dtStrucrure.Rows[0][0] = String.Format("Конс. бюджет\nПлан");
            dtStrucrure.Rows[1][0] = String.Format("Конс. бюджет\nФакт");
            dtStrucrure.Rows[2][0] = String.Format("Собств. бюджет\nПлан");
            dtStrucrure.Rows[3][0] = String.Format("Собств. бюджет\nФакт");

            //UltraChart2.Width = 760;
            //UltraChart2.Height = 190;
            //SetupCustomSkin(UltraChart2);

            //UltraChart2.Style.Add("margin-left", "-15px");
            //UltraChart2.DataSource = dtStrucrure;
            //UltraChart2.ChartType = ChartType.StackBarChart;
            //UltraChart2.StackChart.StackStyle = StackStyle.Complete;
            //UltraChart2.Axis.Y.Extent = 180;
            //UltraChart2.Axis.Y2.Extent = 20;
            //UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            //UltraChart2.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            //UltraChart2.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Center;
            //UltraChart2.Axis.X.Visible = false;
            ////UltraChart.Legend.Margins.Right = -2;
            //UltraChart2.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br/><SERIES_LABEL>&nbsp;<b><DATA_VALUE_ITEM:N2></b></span>";

            //UltraChart2.Legend.MoreIndicatorText = " ";

            //UltraChart2.Legend.Visible = false;

            //UltraChart2.DataBind();
        }

        #region Гейдж

        private static UltraGauge GetGauge(double markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 300;
            gauge.Height = 61;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = "../../../TemporaryImages/gauge_imfrf02_01_#SEQNUM(100).png";

            // Настраиваем гейдж
            LinearGauge linearGauge = new LinearGauge();
            linearGauge.CornerExtent = 10;
            linearGauge.MarginString = "2, 10, 2, 10, Pixels";

            // Выбираем максимум шкалы 
            double endValue = (Math.Max(100, markerValue));

            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = 98;
            scale.StartExtent = 2;
            scale.OuterExtent = 93;
            scale.InnerExtent = 52;

            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(0, 255, 255, 255);
            gradientBrush.StartColor = Color.FromArgb(120, 255, 255, 255);
            scale.BrushElements.Add(gradientBrush);
            linearGauge.Scales.Add(scale);
            AddMainScale(endValue, linearGauge, markerValue);
            AddMajorTickmarkScale(endValue, linearGauge);
            AddGradient(linearGauge);

            linearGauge.Margin.Top = 1;
            linearGauge.Margin.Bottom = 1;

            gauge.Gauges.Add(linearGauge);
            return gauge;
        }

        private const int ScaleStartExtent = 5;
        private const int ScaleEndExtent = 97;

        private static void AddMajorTickmarkScale(double endValue, LinearGauge linearGauge)
        {
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = new LinearGaugeScale();
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

        private void SetupCustomSkin(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = false;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 7; i <= 12; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.Hatch = FillHatchStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(110, 189, 241);
                    }
                case 2:
                    {
                        return Color.FromArgb(214, 171, 133);
                    }
                case 3:
                    {
                        return Color.FromArgb(141, 178, 105);
                    }
                case 4:
                    {
                        return Color.FromArgb(192, 178, 224);
                    }
                case 5:
                    {
                        return Color.FromArgb(245, 187, 102);
                    }
                case 6:
                    {
                        return Color.FromArgb(142, 164, 236);
                    }
                case 7:
                    {
                        return Color.FromArgb(217, 230, 117);
                    }
                case 8:
                    {
                        return Color.FromArgb(162, 154, 98);
                    }
                case 9:
                    {
                        return Color.FromArgb(143, 199, 219);
                    }
                case 10:
                    {
                        return Color.FromArgb(176, 217, 117);
                    }
                case 11:
                    {
                        return Color.Cyan;
                    }
                case 12:
                    {
                        return Color.Gold;
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
                        return Color.FromArgb(9, 135, 214);
                    }
                case 2:
                    {
                        return Color.FromArgb(138, 71, 10);
                    }
                case 3:
                    {
                        return Color.FromArgb(65, 124, 9);
                    }
                case 4:
                    {
                        return Color.FromArgb(44, 20, 91);
                    }
                case 5:
                    {
                        return Color.FromArgb(229, 140, 13);
                    }
                case 6:
                    {
                        return Color.FromArgb(11, 45, 160);
                    }
                case 7:
                    {
                        return Color.FromArgb(164, 184, 10);
                    }
                case 8:
                    {
                        return Color.FromArgb(110, 98, 8);
                    }
                case 9:
                    {
                        return Color.FromArgb(11, 100, 131);
                    }
                case 10:
                    {
                        return Color.FromArgb(102, 168, 9);
                    }
                case 11:
                    {
                        return Color.Cyan;
                    }
                case 12:
                    {
                        return Color.Gold;
                    }
            }
            return Color.White;
        }
    }
}