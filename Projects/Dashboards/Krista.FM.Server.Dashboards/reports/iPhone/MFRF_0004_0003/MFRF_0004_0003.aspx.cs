using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0004_0003 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            string query = DataProvider.GetQueryText("MFRF_0004_0003_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();

            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)));

            Label1.Text = string.Format("Исполнение за {0} {1} {2} года", CRHelper.MonthNum(month), CRHelper.RusManyMonthGenitive(CRHelper.MonthNum(month)), year);

            query = DataProvider.GetQueryText("MFRF_0004_0003");
            dtData = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtData);

            foreach (DataRow row in dtData.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 0 || i == 1))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            if (dtData != null && dtData.Rows.Count != 0)
            {
                string plan = dtData.Rows[0][0] != DBNull.Value ? Convert.ToDouble(dtData.Rows[0][0]).ToString("N3") : string.Empty;
                string fact = dtData.Rows[0][1] != DBNull.Value ? Convert.ToDouble(dtData.Rows[0][1]).ToString("N3") : string.Empty;
                string percent = dtData.Rows[0][2] != DBNull.Value ? Convert.ToDouble(dtData.Rows[0][2]).ToString("P2") : string.Empty;
                string rateFact = dtData.Rows[0][3] != DBNull.Value ? Convert.ToDouble(dtData.Rows[0][3]).ToString("P2") : string.Empty;
                string ratePlan = dtData.Rows[0][4] != DBNull.Value ? Convert.ToDouble(dtData.Rows[0][4]).ToString("P2") : string.Empty;
                string minPercent = dtData.Rows[0][5] != DBNull.Value ? Convert.ToDouble(dtData.Rows[0][5]).ToString("P2") : string.Empty;
                string minPercentGRBS = dtData.Rows[0][6] != DBNull.Value ? dtData.Rows[0][6].ToString() : string.Empty;
                string maxPercent = dtData.Rows[0][7] != DBNull.Value ? Convert.ToDouble(dtData.Rows[0][7]).ToString("P2") : string.Empty;
                string maxPercentGRBS = dtData.Rows[0][8] != DBNull.Value ? dtData.Rows[0][8].ToString() : string.Empty;

                Label2.Text = string.Format("Бюджетные ассигнования на {0} год", year);
                Label2_1.Text = string.Format("{0}", plan);

                Label3.Text = "Кассовое исполнение";
                Label3_1.Text = string.Format("{0}", fact);

                Label4.Text = "Исполнено";
                Label4_1.Text = string.Format("{0}", percent);

                Label5.Text = "Темп роста расходов к прошлому году";
                Label6.Text = string.Format("&nbsp;&nbsp;фактический");
                Label6_1.Text = string.Format("&nbsp;{0}", rateFact);
                Label7.Text = string.Format("&nbsp;&nbsp;плановый");
                Label7_1.Text = string.Format("&nbsp;{0}", ratePlan);
                
                Label12.Text = "Процент исполнения по ГРБС<br />";
                
                Label8.Text = "&nbsp;&nbsp;мин";
                Label8_1.Text = string.Format("&nbsp;{0}", minPercent);
                Label8_2.Text = string.Format("&nbsp;{0}", DataDictionariesHelper.GetShortGRBSName(minPercentGRBS));

                Label9.Text = "&nbsp;&nbsp;макс";
                Label9_1.Text = string.Format("&nbsp;{0}", maxPercent);
                Label9_2.Text = string.Format("&nbsp;{0}", DataDictionariesHelper.GetShortGRBSName(maxPercentGRBS));
                
                if (dtData.Rows[0][2] != DBNull.Value && dtData.Rows[0][2].ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(dtData.Rows[0][2]);
                    UltraGauge gauge = GetGauge(value * 100);
                    PlaceHolderGauge.Controls.Add(gauge);
                }
            }
        }

        private static UltraGauge GetGauge(double markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 300;
            gauge.Height = 61;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = "../../../TemporaryImages/gauge_imfrf04_03#SEQNUM(100).png";

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
    }
}
