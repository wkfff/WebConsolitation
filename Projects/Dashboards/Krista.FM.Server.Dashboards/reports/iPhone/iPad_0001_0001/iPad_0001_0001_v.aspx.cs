using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class iPad_0001_0001_v : CustomReportPage
    {
        private int monthNum;
        private int yearNum;

        private int monthNumDebt = 9;
        private int yearNumDebt = 2010;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }

            HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}' <img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/Heralds/{0}.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"], HttpContext.Current.Session["CurrentSiteRef"]);

            InitializeIncomes();
            InitializeOutcomes();

            InitializeBkku();
            InitializeBudget();

            InitializeDebts();
        }

        #region Доходы
        private DataTable dt;

        private void InitializeIncomes()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_incomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "месяц";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "месяца";
                        break;
                    }
                default:
                    {
                        monthStr = "месяцев";
                        break;
                    }
            }

            IncomesHeader.Text = string.Format("Доходы за {0} {1} {2}г.", monthNum, monthStr, yearNum);

            dt = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_Incomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbIncomesPlanValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Назначено"]) / 1000000);
            lbIncomesFactValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Исполнено"]) / 1000000);
            lbIncomesExecutedValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Процент выполнения назначений"]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["Ранг процент"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг процент"] != DBNull.Value && dt.Rows[0]["Худший ранг процент"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг процент"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            string img2 = string.Empty;
            if (dt.Rows[0]["Ранг процент РФ"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг процент РФ"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг процент РФ"] != DBNull.Value && dt.Rows[0]["Худший ранг процент РФ"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["Ранг процент РФ"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг процент РФ"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFO.Text = String.Format("ранг {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbIncomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг процент"], img1);
            lbIncomesRankRFValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["Ранг процент РФ"], img2);

            lbIncomesRankFOValue.Style.Add("line-height", "40px");


            img1 = String.Empty;
            if (dt.Rows[0]["Ранг среднедуш"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг среднедуш"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг среднедуш"] != DBNull.Value && dt.Rows[0]["Худший ранг среднедуш"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["Ранг среднедуш"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг среднедуш"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            img2 = string.Empty;
            if (dt.Rows[0]["Ранг среднедуш РФ"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг среднедуш РФ"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг среднедуш РФ"] != DBNull.Value && dt.Rows[0]["Худший ранг среднедуш РФ"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["Ранг среднедуш РФ"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг среднедуш РФ"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFOAverage.Text = String.Format("ранг {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbIncomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг среднедуш"], img1);
            lbIncomesRankRFAverageValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["Ранг среднедуш РФ"], img2);

            double value;
            if (Double.TryParse(dt.Rows[0]["Среднедушевые доходы"].ToString(), out value))
            {
                lbIncomesAverageValue.Text = String.Format("{0:N1}", value);
            }
            if (Double.TryParse(dt.Rows[0]["Численность постоянного населения"].ToString(), out value))
            {
                lbPopulationValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Численность постоянного населения"]));
            }
            lbIncomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0][2]) * 100);
            //  gauge.ToolTip = String.Format("Процент выполнения назначений: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("padding-left", "45px");
            GaugeIncomesPlaceHolder.Controls.Add(gauge);
        }

        #endregion

        #region Расходы

        private void InitializeOutcomes()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_outcomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            string lastlastYear = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodLastDate.Value = UserParams.PeriodDayFK.Value.Replace(UserParams.PeriodLastYear.Value, lastlastYear);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "месяц";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "месяца";
                        break;
                    }
                default:
                    {
                        monthStr = "месяцев";
                        break;
                    }
            }

            OutcomesHeader.Text = string.Format("Расходы за {0} {1} {2}г.", monthNum, monthStr, yearNum);

            dt = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_Outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbOutcomesPlanValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Назначено"]) / 1000000);
            lbOutcomesFactValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Исполнено"]) / 1000000);
            lbOutcomesExecutedValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Процент выполнения назначений"]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["Ранг процент"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг процент"] != DBNull.Value && dt.Rows[0]["Худший ранг процент"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг процент"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            string img2 = string.Empty;
            if (dt.Rows[0]["Ранг процент РФ"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг процент РФ"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг процент РФ"] != DBNull.Value && dt.Rows[0]["Худший ранг процент РФ"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["Ранг процент РФ"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг процент РФ"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOValue.Style.Add("line-height", "40px");

            lbOutcomesRankFO.Text = String.Format("ранг {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbOutcomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг процент"], img1);
            lbOutcomesRankRFValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["Ранг процент РФ"], img2);

            img1 = String.Empty;
            if (dt.Rows[0]["Ранг бюджетной обеспеченности"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг бюджетной обеспеченности"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг бюджетной обеспеченности"] != DBNull.Value && dt.Rows[0]["Худший ранг бюджетной обеспеченности"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["Ранг бюджетной обеспеченности"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг бюджетной обеспеченности"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            img2 = string.Empty;
            if (dt.Rows[0]["Ранг бюджетной обеспеченности РФ"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг бюджетной обеспеченности РФ"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг бюджетной обеспеченности РФ"] != DBNull.Value && dt.Rows[0]["Худший ранг бюджетной обеспеченности РФ"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["Ранг бюджетной обеспеченности РФ"]) == Convert.ToInt32(dt.Rows[0]["Худший ранг бюджетной обеспеченности РФ"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOAverage.Text = String.Format("ранг {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbOutcomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг бюджетной обеспеченности"], img1);
            lbOutcomesRankRFAverageValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["Ранг бюджетной обеспеченности РФ"], img2);

            double value;
            if (double.TryParse(dt.Rows[0]["Коэффициент бюджетной обеспеченности населения"].ToString(), out value))
            {
                lbOutcomesAverageValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["Коэффициент бюджетной обеспеченности населения"]));
            }
            lbOutcomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0][2]) * 100);
            // gauge.ToolTip = String.Format("Процент выполнения назначений: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("margin-left", "-40px");
            GaugeOutcomesPlaceHolder.Controls.Add(gauge);
        }

        #endregion


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

        #endregion

        #region БККУ

        private void InitializeBkku()
        {
            SetIndicatorsData();
        }

        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

        private void SetStatsData()
        {
            string query = DataProvider.GetQueryText("iPad_0001_0001_crimes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);
            CrimesBK.Text = dt.Rows[0][1].ToString();

            #region первая строка
            TableRow row = new TableRow();

            TableCell cell = new TableCell();
            Label label = new Label();
            label.Text = "Нарушений";
            label.CssClass = "InformationText";
            cell.Width = 83;
            cell.Controls.Add(label);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 90;
            label = new Label();
            label.ForeColor = Color.White;
            label.CssClass = "DigitsValueSmall";
            label.Text = dt.Rows[0][3].ToString();
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "InformationText";
            label.Text = string.Format("Среднее {0}", dt.Rows[0][4]);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "InformationText";
            label.Text = "Среднее РФ";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            //statsTable.Rows.Add(row);

            #endregion

            #region вторая строка
            row = new TableRow();

            cell = new TableCell();
            label = new Label();
            label.Text = "БК";
            label.CssClass = "InformationText";
            cell.Width = 83;
            cell.Controls.Add(label);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 90;
            label = new Label();
            label.CssClass = "DigitsValueSmall";
            label.Text = dt.Rows[0][1].ToString();
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "InformationText";
            label.Text = string.Format("{0:N1}", dt.Rows[1][1]);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "InformationText";
            label.Text = string.Format("{0:N1}", dt.Rows[2][1]);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            //statsTable.Rows.Add(row);

            #endregion
        }

        private string GetMbtDescription(int group)
        {
            switch (group)
            {
                case 1:
                    {
                        return "&nbsp;(доля МБТ более 60%)";
                    }
                case 2:
                    {
                        return "&nbsp;(доля МБТ от 20% до 60%)";
                    }
                case 3:
                    {
                        return "&nbsp;(доля МБТ от 5% до 20%)";
                    }
                case 4:
                    {
                        return "&nbsp;(доля МБТ менее 5%)";
                    }
                default:
                    {
                        return "&nbsp;(доля МБТ более 60%)";
                    }
            }
        }

        private void SetIndicatorsData()
        {

            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_date_year_bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText("iPad_0001_0001_date_months_bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateMonths);

            if (Convert.ToInt32(dtDateMonths.Rows[0][0].ToString()) > Convert.ToInt32(dtDateYear.Rows[0][0].ToString()))
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateMonths.Rows[0][4]);
                UserParams.Filter.Value = "По данным месячной отчетности";
            }
            else
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateYear.Rows[0][1]);
                UserParams.Filter.Value = "По данным годовой отчетности";
            }

            DataTable mbtGroup = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_mbtGroup");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, mbtGroup);

            lbRankCaption.Text = String.Format("{0}: ", mbtGroup.Rows[0][0]);
            lbRankCaption.CssClass = "InformationText";

            if (mbtGroup.Rows[0][1] != DBNull.Value)
            {
                Rank.Text = string.Format("&nbsp;{0:N0}", mbtGroup.Rows[0][1]);
                Rank.CssClass = "DigitsValue";

                lbRankDescription.Text = GetMbtDescription((int)Convert.ToDouble(mbtGroup.Rows[0][1].ToString()));
                lbRankDescription.CssClass = "InformationText";

                string imageUrl = String.Empty;
                switch (mbtGroup.Rows[0][1].ToString())
                {
                    case "1":
                        {
                            imageUrl = "~/images/ballRedBB.png";
                            break;
                        }
                    case "2":
                        {
                            imageUrl = "~/images/ballOrangeBB.png";
                            break;
                        }
                    case "3":
                        {
                            imageUrl = "~/images/ballYellowBB.png";
                            break;
                        }
                    case "4":
                        {
                            imageUrl = "~/images/ballGreenBB.png";
                            break;
                        }
                }

                imgMbt.ImageUrl = imageUrl;
            }
            else
            {
                Rank.Text = String.Empty;
                lbRankCaption.Text = String.Empty;
                lbRankDescription.Text = String.Empty;
            }

            DataTable dtCrime = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_crime");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtCrime);

            double crimeValue = 0;
            if (dtCrime.Rows.Count > 0 && dtCrime.Rows[0][0] != DBNull.Value)
            {
                crimeValue = Convert.ToDouble(dtCrime.Rows[0][0]);
            }
            bool isCrime = false;
            string mbtPart = String.Empty;
            string borderValue = String.Empty;
            switch (mbtGroup.Rows[0][1].ToString())
            {
                case "1":
                    {
                        isCrime = crimeValue >= 0.5;
                        mbtPart = "более";
                        borderValue = "0,5000";
                        break;
                    }
                case "2":
                case "3":
                case "4":
                    {
                        isCrime = crimeValue >= 1;
                        mbtPart = "менее";
                        borderValue = "1,0000";
                        break;
                    }
            }

            string crimeTooltip = isCrime ? String.Format("Отношение объема гос.долга (с учетом привлеченных бюджетных кредитов) к общему годовому объему доходов бюджета субъекта РФ без учета объема безвозмездных поступлений&nbsp;<span class=\"DigitsValue\">{0:N4}</span><br/>Доля МБТ&nbsp;<span class=\"DigitsValue\">{1} 60%</span><br/>Предельное значение&nbsp;<span class=\"DigitsValue\">{2}</span>", crimeValue, mbtPart, borderValue) :
                        String.Format("Отношение объема гос.долга (с учетом привлеченных бюджетных кредитов) к общему годовому объему доходов бюджета субъекта РФ без учета объема безвозмездных поступлений&nbsp;<span class=\"DigitsValue\">{0:N4}</span><br/>Доля МБТ&nbsp;<span class=\"DigitsValue\">{1} 60%</span><br/>Предельное значение&nbsp;<span class=\"DigitsValue\">{2}</span>", crimeValue, mbtPart, borderValue);

            // TooltipHelper.AddToolTip(crimeDiv, crimeTooltip, Page);
            gosDebtsCrime.Text = isCrime ? String.Format("Оценка соблюдения ст.107 БК РФ на {0:dd.MM.yyyy} г. &nbsp;<img src=\"../../../images/ballRedBB.png\">&nbsp;нарушение<br/>{1}", new DateTime(yearNum, monthNum, 1).AddMonths(1), crimeTooltip) : String.Format("Оценка соблюдения ст.107 БК РФ на {0:dd.MM.yyyy} г. &nbsp<img src=\"../../../images/ballGreenBB.png\">&nbsp;нет нарушения<br/>{1}", new DateTime(yearNum, monthNum, 1).AddMonths(1), crimeTooltip);

            query = DataProvider.GetQueryText("iPad_0001_0001_crimes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);

            CrimesBKTitle.Text =
                String.Format("{0} в {1}:&nbsp;", CrimesBKTitle.Text, RegionsNamingHelper.ShortName(UserParams.StateArea.Value));
            CrimesBK.Text = dt.Rows[0][1].ToString();
            imgCrimesBK.ImageUrl = dt.Rows[0][1].ToString() == "0" ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";

            DataTable dtIndicators = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_Bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);

            for (int i = 0; i < dtIndicators.Rows.Count; i++)
            {
                TableRow row;
                TableCell cellDescription;
                Label indicatorName;
                TableCell cellValues;
                Label value;
                TableCell cellImage;

                row = new TableRow();

                cellDescription = new TableCell();
                indicatorName = new Label();
                indicatorName.Text = string.Format("{0} ", dtIndicators.Rows[i][1].ToString().Split('(')[0]);
                indicatorName.CssClass = "TableFont";
                cellDescription.Controls.Add(indicatorName);
                Label indicatorDescription = new Label();
                indicatorDescription.Text = string.Format("{0}", dtIndicators.Rows[i][0]);
                indicatorDescription.CssClass = "InformationText";
                cellDescription.Controls.Add(indicatorDescription);
                cellDescription.Width = 565;
                cellDescription.VerticalAlign = VerticalAlign.Top;
                row.Cells.Add(cellDescription);

                cellValues = new TableCell();
                cellValues.Width = 150;
                value = new Label();
                value.SkinID = "TableFont";
                value.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб"
                                 ? string.Format("{0:N1}<br/>", dtIndicators.Rows[i][2])
                                 : string.Format("{0:N4}<br/>", dtIndicators.Rows[i][2]);
                cellValues.VerticalAlign = VerticalAlign.Top;
                cellValues.Controls.Add(value);
                Label measure = new Label();
                measure.Text = string.Format("{0}<br/>", dtIndicators.Rows[i][5]);
                measure.CssClass = "InformationText";
                cellValues.Controls.Add(measure);
                Label condition = new Label();
                condition.Text = string.Format("{0} {1:N1}", dtIndicators.Rows[i][3], Convert.ToDouble(dtIndicators.Rows[i][4]));
                condition.CssClass = "InformationTextGreenYellow";
                cellValues.Controls.Add(condition);
                cellValues.Style.Add("padding-left", "20px");
                cellValues.Style["border-right-style"] = "none";
                row.Cells.Add(cellValues);
                cellImage = new TableCell();
                System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                image.ImageUrl = "~/images/ballRedBB.png";
                cellImage.Controls.Add(image);
                cellImage.VerticalAlign = VerticalAlign.Top;
                cellImage.Style["border-left-style"] = "none";
                row.Cells.Add(cellImage);
                IndicatorsTable.Rows.Add(row);
            }

            dt = new DataTable();

            query = DataProvider.GetQueryText("iPad_0001_0001_date_year_ku");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText("iPad_0001_0001_date_months_ku");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateMonths);

            if (Convert.ToInt32(dtDateMonths.Rows[0][0].ToString()) > Convert.ToInt32(dtDateYear.Rows[0][0].ToString()))
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateMonths.Rows[0][4]);
                UserParams.Filter.Value = "По данным месячной отчетности";
            }
            else
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateYear.Rows[0][1]);
                UserParams.Filter.Value = "По данным годовой отчетности";
            }

            query = DataProvider.GetQueryText("iPad_0001_0001_crimes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);

            CrimesKUTitle.Text =
                String.Format("{0} в {1}:&nbsp;", CrimesKUTitle.Text, RegionsNamingHelper.ShortName(UserParams.StateArea.Value));
            CrimesKU.Text = dt.Rows[0][2].ToString();
            imgCrimesKU.ImageUrl = dt.Rows[0][2].ToString() == "0" ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";

            dtIndicators = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_ku");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);

            for (int i = 0; i < dtIndicators.Rows.Count; i++)
            {
                TableRow row;
                TableCell cellDescription;
                Label indicatorName;
                TableCell cellValues;
                Label value;
                TableCell cellImage;

                row = new TableRow();

                cellDescription = new TableCell();
                indicatorName = new Label();
                indicatorName.Text = string.Format("{0} ", dtIndicators.Rows[i][1].ToString().Split('(')[0]);
                indicatorName.CssClass = "TableFont";
                cellDescription.Controls.Add(indicatorName);
                Label indicatorDescription = new Label();
                indicatorDescription.Text = string.Format("{0}", dtIndicators.Rows[i][0]);
                indicatorDescription.CssClass = "InformationText";
                cellDescription.Controls.Add(indicatorDescription);
                cellDescription.Width = 565;
                cellDescription.VerticalAlign = VerticalAlign.Top;
                row.Cells.Add(cellDescription);

                cellValues = new TableCell();
                cellValues.Width = 150;
                value = new Label();
                value.SkinID = "TableFont";
                value.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб"
                                 ? string.Format("{0:N1}<br/>", dtIndicators.Rows[i][2])
                                 : string.Format("{0:N4}<br/>", dtIndicators.Rows[i][2]);
                cellValues.VerticalAlign = VerticalAlign.Top;
                cellValues.Controls.Add(value);
                Label measure = new Label();
                measure.Text = string.Format("{0}<br/>", dtIndicators.Rows[i][5]);
                measure.CssClass = "InformationText";
                cellValues.Controls.Add(measure);
                Label condition = new Label();
                condition.Text = string.Format("{0} {1:N1}", dtIndicators.Rows[i][3], Convert.ToDouble(dtIndicators.Rows[i][4]));
                condition.CssClass = "ServeTextGreenYellow";
                cellValues.Controls.Add(condition);
                cellValues.Style.Add("padding-left", "20px");
                cellValues.Style["border-right-style"] = "none";
                row.Cells.Add(cellValues);
                cellImage = new TableCell();
                System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                image.ImageUrl = "~/images/ballRedBB.png";
                cellImage.Controls.Add(image);
                cellImage.VerticalAlign = VerticalAlign.Top;
                cellImage.Style["border-left-style"] = "none";
                row.Cells.Add(cellImage);
                IndicatorsTable.Rows.Add(row);
            }
        }

        #endregion

        private void InitializeBudget()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_incomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) + 1).ToString();

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());

            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;

            UltraWebGridBudget.DataBind();
        }

        protected void UltraWebGridBudget_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_budget_incomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("iPad_0001_0001_budget_outcomes_all");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_budget_outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_budget_deficite");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            UltraWebGridBudget.Style.Add("margin-right", "-15px");
            UltraWebGridBudget.DataSource = dt;
        }

        protected void UltraWebGridBudget_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Width = 240;
            e.Layout.Bands[0].Columns[1].Width = 104;
            e.Layout.Bands[0].Columns[2].Width = 104;
            e.Layout.Bands[0].Columns[3].Width = 104;
            e.Layout.Bands[0].Columns[4].Width = 104;
            e.Layout.Bands[0].Columns[5].Width = 103;

            for (int i = 1; i <= e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("16px");
                //e.Layout.Bands[0].Columns[i].CellStyle.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = false;

            }
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].HeaderStyle.Height = 10;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("за {0} мес. 2010г. млн.руб.", monthNum);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("за {0} мес. 2011г. млн.руб.", monthNum);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            e.Layout.Bands[0].Columns[3].Header.Caption = "Темп роста %";

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
            e.Layout.Bands[0].Columns[4].Header.Caption = String.Format("Темп роста {0}, %", RegionsNamingHelper.ShortName(UserParams.Region.Value));

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N1");
            e.Layout.Bands[0].Columns[5].Header.Caption = "Темп роста РФ, %";

            e.Layout.RowStyleDefault.Padding.Top = 1;
            e.Layout.RowStyleDefault.Padding.Bottom = 1;
        }

        protected void UltraWebGridBudget_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().ToLower() != "итого доходов " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "итого расходов " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "профицит(+)/дефицит(-) ")
            {
                e.Row.Cells[0].Style.Padding.Left = 10;
            }
            if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                string img;
                if (value > 100)
                {
                    img = "~/images/arrowGreenUpBB.png";
                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";
                }
                e.Row.Cells[3].Style.BackgroundImage = img;
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 30px center; padding-top: 2px";
            }
            e.Row.Cells[1].Style.Padding.Right = 3;
            e.Row.Cells[2].Style.Padding.Right = 3;
            e.Row.Cells[3].Style.Padding.Right = 1;

            if (e.Row.Cells[0].Value.ToString().ToLower() == "профицит(+)/дефицит(-) ")
            {
                if (e.Row.Cells[1] != null &&
                e.Row.Cells[1].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[1].Value.ToString());
                    string img;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                    }
                    e.Row.Cells[1].Style.BackgroundImage = img;
                    e.Row.Cells[1].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                }

                if (e.Row.Cells[2] != null &&
                e.Row.Cells[2].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[2].Value.ToString());
                    string img;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                    }
                    e.Row.Cells[2].Style.BackgroundImage = img;
                    e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                }
            }
        }


        private void InitializeDebts()
        {
            UltraWebGridGosDebts.DataBinding += new EventHandler(UltraWebGridGosDebts_DataBinding);
            UltraWebGridGosDebts.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGridGosDebts_InitializeLayout);
            UltraWebGridGosDebts.InitializeRow += new InitializeRowEventHandler(UltraWebGridGosDebts_InitializeRow);

            UltraWebGridMunDebts.DataBinding += new EventHandler(UltraWebGridMunDebts_DataBinding);
            UltraWebGridMunDebts.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGridGosDebts_InitializeLayout);
            UltraWebGridMunDebts.InitializeRow += new InitializeRowEventHandler(UltraWebGridMunDebts_InitializeRow);

            DataTable debtsDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_debts_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, debtsDate);

            monthNumDebt = CRHelper.MonthNum(debtsDate.Rows[0][3].ToString());
            yearNumDebt = Convert.ToInt32(debtsDate.Rows[0][0].ToString());

            UserParams.PeriodYear.Value =
                    CRHelper.PeriodMemberUName("",
                                           new DateTime(yearNumDebt, monthNumDebt, 1), 4);
            UserParams.PeriodLastDate.Value =
                    CRHelper.PeriodMemberUName("",
                               new DateTime(yearNumDebt - 1, monthNumDebt, 1), 4);

            UserParams.PeriodFirstYear.Value =
                    CRHelper.PeriodMemberUName("",
                                       new DateTime(yearNumDebt, monthNumDebt - 1, 1), 4);
            UserParams.PeriodLastYear.Value =
                    CRHelper.PeriodMemberUName("",
                                   new DateTime(yearNumDebt - 1, monthNumDebt - 1, 1), 4);

            UltraWebGridGosDebts.DataBind();
            UltraWebGridMunDebts.DataBind();
        }

        void UltraWebGridMunDebts_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 2 && e.Row.Cells[1].Value != null)
            {
                e.Row.Cells[1].Value = (Convert.ToDouble(e.Row.Cells[1].Value)).ToString("N4");
            }
            else
            {
                e.Row.Cells[1].Value = (Convert.ToDouble(e.Row.Cells[1].Value)).ToString("N1");
            }

            SetConditionArrow(e, 4);

            SetRankImage(e, 2, 5, true);
            SetRankImage(e, 3, 6, true);

            switch (e.Row.Index)
            {
                case 0:
                    {
                        e.Row.Cells[0].Value = String.Format("Объем долга на {0:dd.MM.yyyy}, млн.руб.", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
                        break;
                    }
                case 1:
                    {
                        e.Row.Cells[0].Value = String.Format("Муниципальный долг на душу населения на {0:dd.MM.yyyy},<br/>руб./чел.", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
                        break;
                    }
                case 2:
                    {
                        e.Row.Cells[0].Value = String.Format("Долговая нагрузка на {0:dd.MM.yyyy}", new DateTime(yearNum, monthNum, 1).AddMonths(1));
                        break;
                    }
            }
            //if (e.Row.Index == 1)
            //{
            //    e.Row.Hidden = true;
            //}
        }

        void UltraWebGridGosDebts_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 2 && (dtGosDebts.Rows[1][1] == DBNull.Value || (dtGosDebts.Rows[1][1] != DBNull.Value &&
                Convert.ToDouble(dtGosDebts.Rows[1][1]) == 0)))
            {
                e.Row.Hidden = true;
                crimeDiv.Visible = false;
            }

            if (e.Row.Index == 4 && e.Row.Cells[1].Value != null)
            {
                e.Row.Cells[1].Value = (Convert.ToDouble(e.Row.Cells[1].Value)).ToString("N4");
            }
            else if (e.Row.Index == 2 && e.Row.Cells[1].Value != null)
            {
                e.Row.Cells[1].Value = (Convert.ToDouble(e.Row.Cells[1].Value)).ToString("N4");
            }
            else
            {
                e.Row.Cells[1].Value = (Convert.ToDouble(e.Row.Cells[1].Value)).ToString("N1");
            }

            SetConditionArrow(e, 4);

            SetRankImage(e, 2, 5, true);
            SetRankImage(e, 3, 6, true);

            switch (e.Row.Index)
            {
                case 0:
                    {
                        e.Row.Cells[0].Value = String.Format("Объем долга на {0:dd.MM.yyyy}, млн.руб.", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
                        break;
                    }
                case 1:
                    {
                        e.Row.Cells[0].Value = String.Format("Объем долга на {0:dd.MM.yyyy}, млн.руб.", new DateTime(yearNum, monthNum, 1).AddMonths(1));
                        break;
                    }
                case 2:
                    {
                        e.Row.Cells[0].Value = String.Format("Отношение долга к доходам (без безвозм.поступлений) на {0:dd.MM.yyyy}", new DateTime(yearNum, monthNum, 1).AddMonths(1));
                        break;
                    }
                case 3:
                    {
                        e.Row.Cells[0].Value = String.Format("Государственный долг на душу населения на {0:dd.MM.yyyy},<br/>руб./чел.", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
                        break;
                    }
                case 4:
                    {
                        e.Row.Cells[0].Value = String.Format("Долговая нагрузка на {0:dd.MM.yyyy}", new DateTime(yearNum, monthNum, 1).AddMonths(1));
                        break;
                    }
            }
            if (e.Row.Index == 1)
            {
                e.Row.Hidden = true;
            }
        }

        void UltraWebGridMunDebts_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_MunDebts_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            if (dt.Rows[0][1] == DBNull.Value ||
                Convert.ToDouble(dt.Rows[0][1]) == 0)
            {
                crimeText.Visible = true;
                crimeText.Style.Add("padding-left", "5px");
                UltraWebGridMunDebts.Visible = false;
            }
            else
            {
                UltraWebGridMunDebts.DataSource = dt;
            }
        }

        void UltraWebGridGosDebts_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Grid.Width = Unit.Empty;
            e.Layout.Grid.Height = Unit.Empty;

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[1].Width = 130;
            e.Layout.Bands[0].Columns[2].Width = 124;
            e.Layout.Bands[0].Columns[3].Width = 124;
            e.Layout.Bands[0].Columns[4].Width = 130;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            // CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");

            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;

            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Ранг {0}", UserParams.ShortRegion.Value);
            e.Layout.Bands[0].Columns[3].Header.Caption = "Ранг РФ";
            e.Layout.Bands[0].Columns[4].Header.Caption = "Темп роста к прошлому году, %";
        }

        private DataTable dtGosDebts;

        void UltraWebGridGosDebts_DataBinding(object sender, EventArgs e)
        {
            dtGosDebts = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_GosDebts_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGosDebts);
            if (dtGosDebts.Rows[0][1] == DBNull.Value ||
                Convert.ToDouble(dtGosDebts.Rows[0][1]) == 0)
            {
                crimeText1.Visible = true;
                crimeText1.Style.Add("padding-left", "5px");
                crimeDiv.Visible = false;
                UltraWebGridGosDebts.Visible = false;
            }
            else
            {
                UltraWebGridGosDebts.DataSource = dtGosDebts;
            }
        }

        private static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value > 100)
                {
                    img = "~/images/arrowRedUpBB.png";

                }
                else
                {
                    img = "~/images/arrowGreenDownBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
                //   e.Row.Cells[3].Title = title;
            }
        }

        private static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex, bool direct)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null &&
                e.Row.Cells[worseRankCelIndex] != null &&
                e.Row.Cells[worseRankCelIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = Convert.ToInt32(e.Row.Cells[worseRankCelIndex].Value.ToString());
                string img = String.Empty;
                //string title;
                if (direct)
                {
                    if (value == 1)
                    {
                        img = "~/images/StarYellow.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarGray.png";
                    }
                }
                else
                {
                    if (value == 1)
                    {
                        img = "~/images/StarGray.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarYellow.png";
                    }
                    e.Row.Cells[rankCellIndex].Value = worseRankValue - value + 1;
                }
                e.Row.Cells[rankCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[rankCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 40px center; padding-left: 2px; padding-right: 32px";
            }
        }
    }
}

