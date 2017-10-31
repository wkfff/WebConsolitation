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
    public partial class FO_0135_0001 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;
        // мера Остаток на начало
        private CustomParam measureStartBalance;
        // мера Остаток на конец
        private CustomParam measureEndBalance;

        #endregion

        public bool IsQuaterPlanType
        {
            get
            {
                return RegionSettingsHelper.Instance.CashPlanType.ToLower() == "quarter";
            }
        }

        public bool IsYar
        {
            get
            {
                return RegionSettingsHelper.Instance.Name.ToLower() == "ярославская область";
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            #region Инициализация параметров запроса

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");
            measureStartBalance = UserParams.CustomParam("measure_start_balance");
            measureEndBalance = UserParams.CustomParam("measure_end_balance");

            #endregion
            
            
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][5].ToString();
            DateTime date;

            if (!dtDate.Rows[0][4].ToString().Contains("Заключительные обороты"))
            {
                 date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                 if (false)
                 {
                     lbQuater.Text = "&nbsp;за " + CRHelper.PeriodDescr(date, 3);
                     lbDate.Text = "<br/>(на&nbsp;" + date.ToString("dd.MM.yyyy") + "), тыс.руб.";
                 }
                 else
                 {
                     lbQuater.Text = string.Empty;
                     lbDate.Text = "&nbsp;на&nbsp;" + date.ToString("dd.MM.yyyy") + ", тыс.руб.";
                 }
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = "&nbsp;за " + CRHelper.PeriodDescr(date, 3);
                    lbDate.Text = "<br/>за &nbsp;" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года" + ", тыс.руб.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = " за&nbsp;" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года" + ", тыс.руб.";
                }
            }
                    
            if (IsQuaterPlanType)
            {
                measurePlan.Value = "План";
                measureFact.Value = "Факт";
            }
            else
            {
                measurePlan.Value = "План_Нарастающий итог";
                measureFact.Value = "Факт_Нарастающий итог";
            }

            measureStartBalance.Value = (IsYar) ? "Остаток средств на начало квартала" : RegionSettingsHelper.Instance.CashPlanBalance;
            measureEndBalance.Value = (measureStartBalance.Value == "Остаток средств")
                                          ? measureStartBalance.Value
                                          : "Остаток средств на конец квартала";

            query = DataProvider.GetQueryText("data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtData);

            double value;

            double planIncomes;
            double factIncomes;
            double planOutcomes;
            double factOutcomes;

            if (measureStartBalance.Value == "Остаток средств")
            {               
                Label2.Visible = false;
                Label3.Visible = false;
                Label4.Visible = false;
                lbRestEndPlan.Visible = false;
                lbRestEndFact.Visible = false;
                restEndTR.Visible = false;
            }
            else
            {                
                Label2.Visible = true;
                Label3.Visible = true;
                Label4.Visible = true;
                lbRestEndPlan.Visible = true;
                lbRestEndFact.Visible = true;
                restEndTR.Visible = true;
            }

            
            if (Double.TryParse(dtData.Rows[1][1].ToString(), out value))
            {
                lbRestEndPlan.Text = (value / 1000).ToString("N0") + "&nbsp;";
            }
            if (Double.TryParse(dtData.Rows[1][2].ToString(), out value))
            {
                lbRestEndFact.Text = (value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[2][1].ToString(), out planIncomes))
            {
                lbPlanIncomes.Text = (planIncomes / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[2][2].ToString(), out factIncomes))
            {
                lbFactIncomes.Text = (factIncomes / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[3][1].ToString(), out planOutcomes))
            {
                lbPlanOutcomes.Text = (planOutcomes / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[3][2].ToString(), out factOutcomes))
            {
                lbFactOutcomes.Text = (factOutcomes / 1000).ToString("N0");
            }

            lbExecutedIncomes.Text = (factIncomes/planIncomes).ToString("P2");
            lbExecutedOucomes.Text = (factOutcomes / planOutcomes).ToString("P2");

            if (planIncomes != 0)
            {
                UltraGauge gauge = GetGauge(factIncomes / planIncomes * 100);
                PlaceHolderIncomes.Controls.Add(gauge);
            }
            if (planOutcomes != 0)
            {
                UltraGauge gauge1 = GetGauge(factOutcomes / planOutcomes * 100);
                PlaceHolderOutcomes.Controls.Add(gauge1);
            }
        }

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
