using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0037_0001 : CustomReportPage
    {
        private DateTime dateIncomes;
        private DateTime dateIncomesDayly;
        private DateTime dateOutcomes;

        private CustomParam periodMonthIncomes;
        private CustomParam periodMonthOutcomes;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (periodMonthIncomes == null)
            {
                periodMonthIncomes = UserParams.CustomParam("period_month_incomes");
            }
            if (periodMonthOutcomes == null)
            {
                periodMonthOutcomes = UserParams.CustomParam("period_month_outcomes");
            }

            DataTable dtDateIncomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0037_0001_date_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDateIncomes);

            dateIncomes =
                new DateTime(Convert.ToInt32(dtDateIncomes.Rows[0][0]),
                             CRHelper.MonthNum(dtDateIncomes.Rows[0][3].ToString()),
                             1);
            dateIncomes = dateIncomes.AddMonths(1);

            DataTable dtDateIncomesDayly = new DataTable();
            query = DataProvider.GetQueryText("FO_0037_0001_date_incomes_dayly");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDateIncomesDayly);

            dateIncomesDayly =
                new DateTime(Convert.ToInt32(dtDateIncomesDayly.Rows[0][0]),
                             CRHelper.MonthNum(dtDateIncomesDayly.Rows[0][3].ToString()),
                             Convert.ToInt32(dtDateIncomesDayly.Rows[0][4]));
            if (CRHelper.MonthLastDay(dateIncomesDayly.Month) == Convert.ToInt32(dateIncomesDayly.Day))
            {
                dateIncomesDayly = dateIncomes.AddDays(1);
            }
            if (dateIncomesDayly > dateIncomes)
            {
                periodMonthIncomes.Value = dtDateIncomesDayly.Rows[0][5].ToString();
                UserParams.CubeName.Value = "ФО_Исполнение бюджета_Доходы";
                dateIncomes = dateIncomesDayly;
            }
            else
            {
                periodMonthIncomes.Value = dtDateIncomes.Rows[0][5].ToString();
                UserParams.CubeName.Value = "ФО_Исполнение бюджета";
            }

            DataTable dtDateOutcomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0037_0001_date_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDateOutcomes);
            periodMonthOutcomes.Value = dtDateOutcomes.Rows[0][5].ToString();
            dateOutcomes =
                new DateTime(Convert.ToInt32(dtDateOutcomes.Rows[0][0]),
                             CRHelper.MonthNum(dtDateOutcomes.Rows[0][3].ToString()),
                             1);
            dateOutcomes = dateOutcomes.AddMonths(1);

            dataBinding();
        }

        protected void dataBinding()
        {
            /* double value = Convert.ToDouble(dtGrid.Rows[rowIndex][percentColumnIndex]);
            e.Row.Cells[indincateColumnIndex].Style.BackgroundImage = (value > 0)
                                                       ? 
                                                       : ;*/

            //lbHeader.Text = String.Format("Исполнение на 1.{0}, тыс.руб.<br/>", period);

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0037_0001_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (!(i == 3 || i == 5)
                        && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

            lbHeader2.Text = String.Format("<b>Доходы на {0:dd.MM.yyyy}, тыс.руб.</b><br/>Исполнено&nbsp;", dateIncomes);
            lbValue2.Text = String.Format("{0:N0}<br/>", dt.Rows[0][2]);
            lbHeader3.Text = "Годовые назначения&nbsp;";
            lbValue3.Text = String.Format("{0:N0}<br/>", dt.Rows[0][1]);

            string changeDirection = Convert.ToDouble(dt.Rows[0][5]) < 0 ? "Снижение" : "Рост";
            Image1.ImageUrl = changeDirection == "Снижение"
                                  ? "../../../images/arrowDownRed.png"
                                  : "../../../images/arrowUpGreen.png";
            Image1.Height = 20;
            lbHeader4.Text = "Удельный вес к назнач.&nbsp;";
            lbValue4.Text = String.Format("{0:N2}%<br/>", dt.Rows[0][3]);
            lbHeader8.Text = String.Format("{0} к {1} году на&nbsp;", changeDirection, dateIncomes.Year - 1);
            lbValue8.Text = String.Format("{0:N2}%", Math.Abs(Convert.ToDouble(dt.Rows[0][5])));

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0][3]));
            PlaceHolderIncomes.Controls.Add(gauge);

            dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0037_0001_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (!(i == 3 || i == 5)
                        && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

            lbHeader5.Text = String.Format("<br/><b>Расходы на {0:dd.MM.yyyy}, тыс.руб.</b><br/>Исполнено&nbsp;", dateOutcomes);
            lbValue5.Text = String.Format("{0:N0}<br/>", dt.Rows[0][2]);
            lbHeader6.Text = "Годовые назначения&nbsp;";
            lbValue6.Text = String.Format("{0:N0}<br/>", dt.Rows[0][1]);
            changeDirection = Convert.ToDouble(dt.Rows[0][5]) < 0 ? "Снижение" : "Рост";
            Image2.ImageUrl = changeDirection == "Снижение"
                                  ? "../../../images/arrowDownRed.png"
                                  : "../../../images/arrowUpGreen.png";
            Image2.Height = 20;
            lbHeader7.Text = "Удельный вес к назнач.&nbsp;";
            lbValue7.Text = String.Format("{0:N2}%<br/>", dt.Rows[0][3]);
            lbHeader9.Text = String.Format("{0} к {1} году на&nbsp;", changeDirection, dateOutcomes.Year - 1);
            lbValue9.Text = String.Format("{0:N2}%", Math.Abs(Convert.ToDouble(dt.Rows[0][5])));
      
            gauge = GetGauge(Convert.ToDouble(dt.Rows[0][3]));
            PlaceHolderOutcomes.Controls.Add(gauge);
        }

        private static UltraGauge GetGauge(double markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 300;
            gauge.Height = 59;
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
