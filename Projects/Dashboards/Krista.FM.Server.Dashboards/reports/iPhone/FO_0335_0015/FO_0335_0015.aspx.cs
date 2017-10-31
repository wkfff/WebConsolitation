using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Text = Infragistics.UltraChart.Core.Primitives.Text;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0335_0015 : CustomReportPage
    {
        private int worseRank;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            iPadBricks.iPadBricks.MoPassportHelper.InitRegionSettings(UserParams);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0335_0015_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            if (string.IsNullOrEmpty(UserParams.Mo.Value))
            {
                UserParams.Mo.Value = "Алексеевский";
            }

            UserParams.Filter.Value = RegionsNamingHelper.LocalBudgetUniqueNames[UserParams.Mo.Value];

            bool normalFoRankCount = true;
            string goTypeName = normalFoRankCount ? "ранг МО&nbsp;" : "ранг ГО&nbsp;";
            string moTypeName = normalFoRankCount ? "ранг МО&nbsp;" : "ранг МР&nbsp;";

            DataTable dtMr = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_mr_count");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtMr);
            int worseRankMR = dtMr.Rows.Count;

            DataTable dtGo = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_go_count");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtGo);
            int worseRankGo = dtGo.Rows.Count;

            DataTable dtSettlementsCount = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_settlements_count");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtSettlementsCount);

            if (dtSettlementsCount.Rows[0][0].ToString() == "0")
            {
                settlementHeader.Visible = false;
                UltraWebGridSettlements.Visible = false;
                settlementsDiv.Visible = false;
                lbIncomesRankFO.Text = goTypeName;
                lbOutcomesRankFO.Text = goTypeName;
                lbIncomesRankFOAverage.Text = goTypeName;
                lbOutcomesRankFOAverage.Text = goTypeName;
                worseRank = normalFoRankCount ? worseRankGo + worseRankMR : worseRankGo;
            }
            else
            {
                InitializeSettlement();
                settlementsDiv.Visible = false;
                lbIncomesRankFO.Text = moTypeName;
                lbOutcomesRankFO.Text = moTypeName;
                lbIncomesRankFOAverage.Text = moTypeName;
                lbOutcomesRankFOAverage.Text = moTypeName;
                worseRank = normalFoRankCount ? worseRankGo + worseRankMR : worseRankMR;
            }

            CustomParam populationDate = UserParams.CustomParam("population_date");
            DataTable dtPopulationDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_population_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulationDate);
            populationDate.Value = dtPopulationDate.Rows[0][1].ToString();

            DataTable dtPopulation = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_population");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulation);

            lbPopulation.Text = String.Format("Численность постоянного населения&nbsp;<span style=\"color: white\"><b>{0:N0}</b></span>&nbsp;чел.", dtPopulation.Rows[0][0]);

            HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}' <img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/HeraldsMo/{0}.png\"></a>", HttpContext.Current.Session["CurrentMoID"], HttpContext.Current.Session["CurrentMoSiteRef"]);

            InitializeIncomes();
            InitializeOutcomes();
            InitializeBkku();
            InitializeBudget();

            DataTable dtСreditDebts = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_credit_debts");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtСreditDebts);

            creditDebts.Text = String.Format("Всего просроченная задолженность<br/><span style=\"color: white\"><b>{0:N0}</b></span>&nbsp;тыс.руб., в том числе по:<br/>&nbsp;&nbsp;&nbsp;&nbsp;оплате труда<br/>&nbsp;&nbsp;&nbsp;&nbsp;с начислениями:&nbsp;<span style=\"color: white\"><b>{1:N0}</b></span>&nbsp;тыс.руб.<br/>&nbsp;&nbsp;&nbsp;&nbsp;коммунальным {4}услугам:&nbsp;<span style=\"color: white\"><b>{2:N0}</b></span>&nbsp;тыс.руб.<br/>&nbsp;&nbsp;&nbsp;&nbsp;прочим выплатам:&nbsp;<span style=\"color: white\"><b>{3:N0}</b></span>&nbsp;тыс.руб.", dtСreditDebts.Rows[0][2], dtСreditDebts.Rows[1][2], dtСreditDebts.Rows[2][2], dtСreditDebts.Rows[3][2], Convert.ToDouble(dtСreditDebts.Rows[2][2].ToString()) > 100000 ? string.Empty : string.Empty);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            topRegionsCount.Value = "5";

            UltraWebGrid.DataBind();
            UltraWebGrid.Height = Unit.Empty;
        }

        #region Доходы
        private DataTable dt;
        private DataTable newDt;

        private void InitializeIncomes()
        {
            chart3ElementCaption.Text = String.Format("Доходы за {0} {1} {2}г.", month, CRHelper.RusManyMonthGenitive(month), year);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0335_0015_Incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbIncomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Годовые назначения"]) / 1000);
            lbIncomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Факт"]) / 1000);
            lbIncomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["Ранг процент"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг процент"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг процент"], img1);

            lbIncomesRankFOValue.Style.Add("line-height", "40px");


            img1 = String.Empty;
            if (dt.Rows[0]["Ранг среднедуш"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг среднедуш"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг среднедуш"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг среднедуш"]) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг среднедуш"], img1);

            double value;
            if (Double.TryParse(dt.Rows[0]["Среднедушевые доходы"].ToString(), out value))
            {
                lbIncomesAverageValue.Text = String.Format("{0:N0}", value);
            }
            //if (Double.TryParse(dt.Rows[0]["Численность постоянного населения"].ToString(), out value))
            //{
            //    lbPopulationValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Численность постоянного населения"]));
            //}
            lbIncomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);
            //  gauge.ToolTip = String.Format("Процент выполнения назначений: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("padding-left", "45px");
            gauge.Style.Add("margin-top", "-5px");
            GaugeIncomesPlaceHolder.Controls.Add(gauge);
        }

        #endregion

        #region Расходы

        private void InitializeOutcomes()
        {
            Label1.Text = String.Format("Расходы за {0} {1} {2}г.", month, CRHelper.RusManyMonthGenitive(month), year);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0335_0015_Outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbOutcomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Годовые назначения"]) / 1000);
            lbOutcomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Факт"]) / 1000);
            lbOutcomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["Ранг процент"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг процент"] != DBNull.Value && dt.Rows[0]["Худший ранг процент"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["Ранг процент"]) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOValue.Style.Add("line-height", "40px");

            lbOutcomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг процент"], img1);

            img1 = String.Empty;
            if (dt.Rows[0]["Ранг бюджетной обеспеченности"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["Ранг бюджетной обеспеченности"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["Ранг бюджетной обеспеченности"] != DBNull.Value && dt.Rows[0]["Худший ранг бюджетной обеспеченности"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["Ранг бюджетной обеспеченности"]) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["Ранг бюджетной обеспеченности"], img1);

            double value;
            if (double.TryParse(dt.Rows[0]["Коэффициент бюджетной обеспеченности населения"].ToString(), out value))
            {
                lbOutcomesAverageValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Коэффициент бюджетной обеспеченности населения"]));
            }
            lbOutcomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);
            // gauge.ToolTip = String.Format("Процент выполнения назначений: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("padding-left", "15px");
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

        String text1 = string.Empty;
        String text2 = string.Empty;
        String text3 = string.Empty;

        private static void AddText(FillSceneGraphEventArgs e, string textValue, int offsetX)
        {
            Label lb = new Label();
            Text text = new Text(new Point(GetOffsetX(textValue), 37), textValue);
            LabelStyle style = new LabelStyle();

            style.Font = new Font("Arial", 12, FontStyle.Bold);
            //style.Font.Bold = true;
            style.FontColor = Color.White;
            text.SetLabelStyle(style);
            e.SceneGraph.Add(text);
        }

        private static int GetOffsetX(string text)
        {
            switch (text.Length)
            {
                case 3:
                    return 22;
                case 5:
                    return 17;
                case 6:
                    return 10;
                case 7:
                    return 4;
                default:
                    return 2;
            }
        }

        void UltraChart11_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, text1, 10);
        }

        void UltraChart12_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, text2, 2);
        }

        void UltraChart13_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, text3, 2);
        }

        private void ConfigureActionChart(UltraChart chart, bool crime, string ToolTip)
        {
            chart.Width = 75;
            chart.Height = 75;
            chart.ChartType = ChartType.PieChart;
            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 10;
            chart.Axis.X.Extent = 10;
            chart.Axis.Y2.Extent = 10;
            chart.Axis.X2.Extent = 10;
            chart.Tooltips.FormatString = "<SERIES_LABEL>";
            chart.Legend.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            chart.PieChart.Labels.Visible = false;
            chart.PieChart.Labels.LeaderLineThickness = 0;

            // chart.PieChart.RadiusFactor = 70;

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            chart.ColorModel.Skin.PEs.Clear();

            Color color = Color.Gray;
            Color colorEnd = Color.Gray;
            PaintElement pe = new PaintElement();

            if (crime)
            {
                color = Color.Red;
                colorEnd = Color.Red;
            }
            else
            {
                color = Color.FromArgb(70, 118, 5);
                colorEnd = Color.FromArgb(70, 118, 5);
            }

            pe.Fill = color;
            pe.FillStopColor = colorEnd;
            pe.ElementType = PaintElementType.Gradient;
            pe.FillGradientStyle = GradientStyle.Horizontal;
            pe.FillOpacity = 150;
            chart.ColorModel.Skin.PEs.Add(pe);

            //chart.Style.Add("margin-top", " -10px");

            DataTable actionDataTable = new DataTable();
            actionDataTable.Columns.Add("name", typeof(string));
            actionDataTable.Columns.Add("value", typeof(double));
            object[] fictiveValue = { String.Format("<span style=' font-family: Verdana; font-size: 14px'>{0}</span>", ToolTip), 100 };
            actionDataTable.Rows.Add(fictiveValue);
            chart.DataSource = actionDataTable;
            chart.DataBind();
        }

        private void InitializeBkku()
        {
            DataTable dtInnerDebt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0335_0015_Inner_Debt");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtInnerDebt);

            DataTable outcomesDebt = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_Outcomes_debt");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", outcomesDebt);

            double innerDebtValue;
            Double.TryParse(dtInnerDebt.Rows[0][1].ToString(), out innerDebtValue);

            double bujetCredits;
            Double.TryParse(dtInnerDebt.Rows[1][1].ToString(), out bujetCredits);

            double creditCredits;
            Double.TryParse(dtInnerDebt.Rows[2][1].ToString(), out creditCredits);

            double garants;
            Double.TryParse(dtInnerDebt.Rows[3][1].ToString(), out garants);

            double serve;
            Double.TryParse(outcomesDebt.Rows[0][1].ToString(), out serve);

            munDebt.Text = String.Format("Всего&nbsp;<span style=\"color: white\"><b>{0:N2}</b></span>&nbsp;тыс.руб., в том числе<div style='padding-left: 20px'>бюджетные кредиты&nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp;тыс.руб.<br/>кредиты от кредитных организаций&nbsp;<span style=\"color: white\"><b>{2:N2}</b></span>&nbsp;тыс.руб.<br/>муниципальные гарантии&nbsp;<span style=\"color: white\"><b>{3:N2}</b></span>&nbsp;тыс.руб.</div>Расходы на обслуживание долговых обязательств&nbsp;<span style=\"color: white\"><b>{4:N2}</b><span>&nbsp;тыс.руб.",
                innerDebtValue, bujetCredits, creditCredits, garants, serve);
        }


        #endregion

        #region Бюджет

        private int monthNum = 1;

        private void InitializeBudget()
        {
            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;

            UltraWebGridBudget.DataBind();
            UltraWebGridBudget.Style.Add("margin-top", "-4px");
        }

        protected void UltraWebGridBudget_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText("FO_0335_0015_budget_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("FO_0335_0015_budget_outcomes_all");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

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
            query = DataProvider.GetQueryText("FO_0335_0015_budget_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

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
            query = DataProvider.GetQueryText("FO_0335_0015_budget_deficite");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            dt.Columns.RemoveAt(5);
            //dt.Columns.RemoveAt(3);
            UltraWebGridBudget.Style.Add("margin-right", "-10px");
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

            e.Layout.Bands[0].Columns[0].Width = 171;
            e.Layout.Bands[0].Columns[1].Width = 81;
            e.Layout.Bands[0].Columns[2].Width = 81;
            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[4].Width = 70;

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

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("за {0} мес. {1}г. тыс.руб.", month, year - 1);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("за {0} мес. {1}г. тыс.руб.", month, year);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
            e.Layout.Bands[0].Columns[4].Header.Caption = "Темп роста %";

            e.Layout.RowStyleDefault.Padding.Top = 1;
            e.Layout.RowStyleDefault.Padding.Bottom = 1;
            e.Layout.RowStyleDefault.Padding.Left = 1;
            e.Layout.RowStyleDefault.Padding.Right = 1;
        }

        protected void UltraWebGridBudget_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().ToLower() != "итого доходов " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "итого расходов " &&
               !e.Row.Cells[0].Value.ToString().ToLower().Contains("профицит(+)/"))
            {
                e.Row.Cells[0].Style.Padding.Left = 10;
            }
            if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                string img;
                string title;
                if (value > 0)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "Рост к прошлому году";
                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "Падение к прошлому году";
                }
                e.Row.Cells[4].Style.BackgroundImage = img;
                e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: 6px center; padding-top: 2px";
                //   e.Row.Cells[3].Title = title;
            }
            e.Row.Cells[1].Style.Padding.Right = 3;
            e.Row.Cells[2].Style.Padding.Right = 3;
            e.Row.Cells[4].Style.Padding.Right = 1;

            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("профицит(+)/") && e.Row.Cells[0].Value.ToString().ToLower().Contains("дефицит(-)"))
            {
                if (e.Row.Cells[1] != null &&
                e.Row.Cells[1].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[1].Value.ToString());
                    string img;
                    string title;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                        title = "Профицит";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                        title = "Дефицит";
                    }
                    e.Row.Cells[1].Style.BackgroundImage = img;
                    e.Row.Cells[1].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                    //   e.Row.Cells[1].Title = title;
                }

                if (e.Row.Cells[2] != null &&
                e.Row.Cells[2].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[2].Value.ToString());
                    string img;
                    string title;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                        title = "Профицит";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                        title = "Дефицит";
                    }
                    e.Row.Cells[2].Style.BackgroundImage = img;
                    e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                    //  e.Row.Cells[2].Title = title;
                }
            }
        }

        #endregion

        #region недоимка
        private DataTable dtGrid;

        // private bool fns28nSplitting;
        private int month;
        private int year;

        #region Параметры запроса

        // уровень МР и ГО

        // куб

        // число выбранных регионов
        private CustomParam topRegionsCount;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса


            if (topRegionsCount == null)
            {
                topRegionsCount = UserParams.CustomParam("top_regions_count");
            }

            #endregion
        }


        #region Методы получения значений DataTable

        private static string ParseDTValue(DataRow row, int columnIndex)
        {
            if (row == null || row[columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[columnIndex].ToString();
        }

        private static string ParseDoubleDTValue(DataTable dt, int rowIndex, int columnIndex, string format)
        {
            if (dt == null || dt.Rows[rowIndex][columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDouble(dt.Rows[rowIndex][columnIndex]).ToString(format);
        }

        private static string ParseDoubleDTValue(DataRow row, int columnIndex, string format)
        {
            if (row == null || row[columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDouble(row[columnIndex]).ToString(format);
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string queryName = "FNS_0001_0001";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtGrid);
            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.DataSource = dtGrid;
            //UltraWebGrid.Style.Add("margin-left", "-6px");
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = 114;
            e.Layout.Bands[0].Columns[1].Width = 120;
            e.Layout.Bands[0].Columns[3].Width = 115;

            e.Layout.Bands[0].Columns[2].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;

            int reportYear = year;
            int reportMonth = month + 1;

            if (reportMonth == 13)
            {
                reportYear++;
                reportMonth = 1;
            }

            string dateMonth = string.Format("Недоимка на 01.{0:00}.{1} тыс.руб.", reportMonth, reportYear);

            e.Layout.Bands[0].Columns[0].Header.Caption = dateMonth;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[0], "N0");

            e.Layout.Bands[0].Columns[1].Header.Caption = e.Layout.Bands[0].Columns[1].Header.Caption.Split(';')[1];
            e.Layout.Bands[0].Columns[3].Header.Caption = e.Layout.Bands[0].Columns[3].Header.Caption.Split(';')[1];

            UltraWebGrid.Style.Add("margin-left", "-3px");
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int rowIndex = 0;
            int percentColumnIndex = 2;
            int indincateColumnIndex = 1;
            int rankColumnIndex = 4;

            if (dtGrid.Rows[rowIndex][percentColumnIndex] != DBNull.Value &&
                 dtGrid.Rows[rowIndex][percentColumnIndex].ToString() != string.Empty)
            {
                double value = Convert.ToDouble(dtGrid.Rows[rowIndex][percentColumnIndex]);
                e.Row.Cells[indincateColumnIndex].Style.BackgroundImage = (value > 0)
                                                           ? "../../../images/arrowRedUpBB.png"
                                                           : "../../../images/arrowGreenDownBB.png";
                e.Row.Cells[indincateColumnIndex].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 20px center; padding-top: 2px";
                e.Row.Cells[indincateColumnIndex].Value =
                    String.Format("{0:N0}<br/>{1:P2}", e.Row.Cells[indincateColumnIndex].Value,
                                  e.Row.Cells[percentColumnIndex].Value);
            }

            if (dtGrid.Rows[rowIndex][rankColumnIndex] != DBNull.Value &&
                 dtGrid.Rows[rowIndex][rankColumnIndex].ToString() != string.Empty)
            {
                double value = Convert.ToDouble(dtGrid.Rows[rowIndex][rankColumnIndex]);
                if (value == 1)
                {
                    e.Row.Cells[3].Style.BackgroundImage = "../../../images/starYellow.png";
                }
                else
                {
                    double worseRank = Convert.ToDouble(dtGrid.Rows[rowIndex][6]);
                    if (value == worseRank)
                    {
                        e.Row.Cells[3].Style.BackgroundImage = "../../../images/starGray.png";
                    }
                }
                e.Row.Cells[3].Value = String.Format("{0:P2}<br>ранг {1}", e.Row.Cells[3].Value, value);

                e.Row.Cells[indincateColumnIndex].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 20px center; padding-top: 2px";
                e.Row.Cells[3].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 20px bottom; padding-top: 2px";
            }
        }
        #endregion

        #endregion

        #region Доходы поселения


        void UltraWebGridSettlements_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Width = 90;
            e.Layout.Bands[0].Columns[1].Width = 87;
            e.Layout.Bands[0].Columns[2].Width = 87;
            e.Layout.Bands[0].Columns[3].Width = 85;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("за {0} мес. {1}г. тыс.руб.", month, year - 1);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("за {0} мес. {1}г. тыс.руб.", month, year);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            e.Layout.Bands[0].Columns[3].Header.Caption = "Темп роста %";

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].HeaderStyle.Height = 10;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

        }

        void UltraWebGridSettlements_DataBinding(object sender, EventArgs e)
        {
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0335_0015_Incomes_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            DataTable dtOutcomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_Outcomes_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtOutcomes);

            dt = new DataTable();

            dt.Columns.Add(" ", typeof(string));
            dt.Columns.Add("План тыс.руб.", typeof(double));
            dt.Columns.Add("Факт тыс.руб.", typeof(double));
            dt.Columns.Add("% исп", typeof(double));

            DataRow row = dt.NewRow();
            row[0] = "Доходы";
            if (dtIncomes.Rows[0][0] != DBNull.Value)
            {
                row[1] = Convert.ToDouble(dtIncomes.Rows[0][0]);
            }
            if (dtIncomes.Rows[0][1] != DBNull.Value)
            {
                row[2] = Convert.ToDouble(dtIncomes.Rows[0][1]);
            }
            if (dtIncomes.Rows[0][2] != DBNull.Value)
            {
                row[3] = Convert.ToDouble(dtIncomes.Rows[0][2]);
            }
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "Расходы";
            if (dtOutcomes.Rows[0][0] != DBNull.Value)
            {
                row[1] = Convert.ToDouble(dtOutcomes.Rows[0][0]);
            }
            if (dtOutcomes.Rows[0][1] != DBNull.Value)
            {
                row[2] = Convert.ToDouble(dtOutcomes.Rows[0][1]);
            }
            if (dtOutcomes.Rows[0][2] != DBNull.Value)
            {
                row[3] = Convert.ToDouble(dtOutcomes.Rows[0][2]);
            }
            dt.Rows.Add(row);

            UltraWebGridSettlements.DataSource = dt;
        }

        #endregion

        private void InitializeSettlement()
        {
            DataTable dtSettlementsCount = new DataTable();
            string query = DataProvider.GetQueryText("FO_0335_0015_settlements_count");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtSettlementsCount);

            Label6.Text = String.Format("Бюджеты поселений ({0})", dtSettlementsCount.Rows[0][0]);

            UltraWebGridSettlements.Width = Unit.Empty;
            UltraWebGridSettlements.Height = Unit.Empty;

            UltraWebGridSettlements.DataBinding += new EventHandler(UltraWebGridSettlements_DataBinding);
            UltraWebGridSettlements.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGridSettlements_InitializeLayout);
            UltraWebGridSettlements.InitializeRow += new InitializeRowEventHandler(UltraWebGridSettlements_InitializeRow);
            UltraWebGridSettlements.DataBind();

            InitializeDebtSettlement();
            InitializeFnsSettlement();
        }

        void UltraWebGridSettlements_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[1].Style.CustomRules = "padding-right: 4px";
            e.Row.Cells[2].Style.CustomRules = "padding-right: 4px";
            e.Row.Cells[3].Style.CustomRules = "padding-right: 4px";

            if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                string img;
                string title;
                if (value > 100)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "Рост к прошлому году";
                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "Падение к прошлому году";
                }
                e.Row.Cells[3].Style.BackgroundImage = img;
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 2px center; padding-top: 2px; padding-right: 2px";
                //   e.Row.Cells[3].Title = title;
            }
        }

        private void InitializeDebtSettlement()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0001_debt_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            settlementDebts.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0][0]));
        }

        private void InitializeFnsSettlement()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0001_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            //settlementFns.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0][0]));
        }
    }
}