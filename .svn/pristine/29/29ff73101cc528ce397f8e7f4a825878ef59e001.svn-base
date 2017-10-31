using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0001 : CustomReportPage
    {
        private CustomParam settlementLevel;
        private CustomParam regionsConsolidateLevel;
        private CustomParam incomeTotal;
        private CustomParam outcomeFKRTotal;
        private CustomParam regionsLevel;
        private CustomParam populationDimension;

        private int month;
        private int year;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InitRegionSettings();

            UserParams.Filter.Value = RegionsNamingHelper.LocalSettlementUniqueNames[UserParams.Settlement.Value];
            
            DataTable terrType = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0001_terrType");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", terrType);
            RegionLabel.Text = String.Format("{0} {1} ({2})",terrType.Rows[0][0].ToString().Replace("ГП", "Городское поселение").Replace("СП", "Сельское поселение"), UserParams.Settlement.Value, RegionsNamingHelper.LocalSettlementUniqueNames[UserParams.Settlement.Value].Split('.')[5].Trim('[').Trim(']'));

            CustomParam populationDate = UserParams.CustomParam("population_date");
            DataTable dtPopulationDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0335_0015_population_date");

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulationDate);
            populationDate.Value = dtPopulationDate.Rows[0][1].ToString();

            DataTable dtPopulation = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0001_population");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulation);

            lbPopulation.Text = String.Format("Численность постоянного населения&nbsp;<span style=\"color: white\"><b>{0:N0}</b></span>&nbsp;чел.", dtPopulation.Rows[0][0]);

            MakeDateParams("FO_0002_0001_incomes_date");
            InitializeIncomes();

            MakeDateParams("FO_0002_0001_outcomes_date");
            InitializeOutcomes();

            InitializeBudget();
            InitializeBkku();
            
            UltraWebGridArrearAll.DataBind();
        }

        protected void UltraWebGridArrearAll_DataBinding(object sender, EventArgs e)
        {
            MakeDateParams("FNS_0001_0001_date");

            DataTable dtGrid = new DataTable();
            string queryName = "FNS_0001_0001_v";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            UltraWebGridArrearAll.Width = Unit.Empty;
            UltraWebGridArrearAll.Height = Unit.Empty;
            UltraWebGridArrearAll.DataSource = dtGrid;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = 187;
            e.Layout.Bands[0].Columns[1].Width = 91;
            e.Layout.Bands[0].Columns[2].Width = 91;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 127;
            e.Layout.Bands[0].Columns[5].Width = 110;
            e.Layout.Bands[0].Columns[6].Width = 60;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            int reportYear = year;
            int reportMonth = month + 1;

            if (reportMonth == 13)
            {
                reportYear++;
                reportMonth = 1;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            string dateMonth = string.Format("Недоимка на 01.{0:00}.{1} тыс.руб.", reportMonth, reportYear);

            e.Layout.Bands[0].Columns[1].Header.Caption = "Недоимка на начало года тыс.руб.";
            e.Layout.Bands[0].Columns[2].Header.Caption = dateMonth;
            e.Layout.Bands[0].Columns[3].Header.Caption = "Прирост недоимки тыс.руб.";
            e.Layout.Bands[0].Columns[4].Header.Caption = e.Layout.Bands[0].Columns[4].Header.Caption.Split(';')[0];
            e.Layout.Bands[0].Columns[5].Header.Caption = e.Layout.Bands[0].Columns[5].Header.Caption.Split(';')[0];

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[7].Hidden = true;
            e.Layout.Bands[0].Columns[8].Hidden = true;
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int percentColumnIndex = 4;
            int indincateColumnIndex = 4;

            e.Row.Cells[0].Style.Font.Size = 12;
            e.Row.Cells[1].Style.Font.Size = 12;

            if (e.Row.Cells[percentColumnIndex].Value != null &&
                 e.Row.Cells[percentColumnIndex].Value.ToString() != string.Empty)
            {
                double value = Convert.ToDouble(e.Row.Cells[percentColumnIndex].Value);
                e.Row.Cells[indincateColumnIndex].Style.BackgroundImage = (value > 0)
                                                           ? "../../../images/arrowRedUpBB.png"
                                                           : "../../../images/arrowGreenDownBB.png";
                e.Row.Cells[indincateColumnIndex].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 5px center; padding-top: 2px";
            }

            int rankCellIndex = 6;
            int worseRankCellIndex = 8;

            if (RankPresent(e.Row.Cells[rankCellIndex], e.Row.Cells[worseRankCellIndex]))
            {
                SetRankImage(e.Row.Cells[rankCellIndex], e.Row.Cells[worseRankCellIndex]);
            }
        }

        private static void SetRankImage(UltraGridCell rankCell, UltraGridCell worseRankCell)
        {
            double rank = Convert.ToDouble(rankCell.Value.ToString());
            double worseRank = Convert.ToDouble(worseRankCell.Value.ToString());
            string img = String.Empty;
            if (rank == 1)
            {
                img = "~/images/starYellow.png";
            }
            else if (rank == worseRank)
            {
                img = "~/images/starGray.png";
            }
            rankCell.Style.BackgroundImage = img;
            rankCell.Style.CustomRules = "background-repeat: no-repeat; background-position: 5px center; padding-top: 2px";
        }

        private static bool RankPresent(UltraGridCell rankCell, UltraGridCell worseRankCell)
        {
            return rankCell != null &&
                            rankCell.Value != null &&
                            worseRankCell != null &&
                            worseRankCell.Value != null;
        }

        private void MakeDateParams(string query)
        {
            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText(query);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));
        }

        private void InitRegionSettings()
        {
            settlementLevel = UserParams.CustomParam("settlement_level");
            settlementLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("SettlementLevel");

            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsConsolidateLevel");

            incomeTotal = UserParams.CustomParam("income_total");
            incomeTotal.Value = RegionSettingsHelper.Instance.GetPropertyValue("IncomeTotal");

            outcomeFKRTotal = UserParams.CustomParam("outcome_FKR_total");
            outcomeFKRTotal.Value = RegionSettingsHelper.Instance.GetPropertyValue("OutcomeFKRTotal");

            regionsLevel = UserParams.CustomParam("regions_level");
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;

            populationDimension = UserParams.CustomParam("population_measure");
            populationDimension.Value = RegionSettingsHelper.Instance.PopulationMeasure;

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
        }

        #region Доходы

        private void InitializeIncomes()
        {
            chart3ElementCaption.Text = String.Format("Доходы за {0} {1} {2}г.", month, CRHelper.RusManyMonthGenitive(month), year);

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0001_Incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbIncomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Годовые назначения"]) / 1000);
            lbIncomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Факт"]) / 1000);
            lbIncomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);

            lbIncomesRankAllValue.Text = GetRankText(dt, "Ранг процент", "Худший ранг процент");
            lbIncomesRankAllValue.Style.Add("line-height", "40px");
            lbIncomesRankAllAverageValue.Text = GetRankText(dt, "Ранг среднедуш", "Худший ранг среднедуш");
            lbIncomesRankAllAverageValue.Style.Add("line-height", "40px");

            lbIncomesRankFOValue.Text = GetRankText(dt, "Ранг процент среди соседей", "Худший ранг процент среди соседей");
            lbIncomesRankFOValue.Style.Add("line-height", "40px");
            lbIncomesRankFOAverageValue.Text = GetRankText(dt, "Ранг среднедуш среди соседей", "Худший ранг среднедуш среди соседей");
            lbIncomesRankFOAverageValue.Style.Add("line-height", "40px");

            double value;
            if (Double.TryParse(dt.Rows[0]["Среднедушевые доходы"].ToString(), out value))
            {
                lbIncomesAverageValue.Text = String.Format("{0:N0}", value);
            }

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);
            gauge.Style.Add("padding-left", "45px");
            gauge.Style.Add("margin-top", "-5px");
            GaugeIncomesPlaceHolder.Controls.Add(gauge);
        }

        private static string GetRankText(DataTable dt, string rankColumnName, string worseRankColumnName)
        {
            return String.Format("{0:N0}{1}&nbsp;", dt.Rows[0][rankColumnName], GetRankImg(dt, rankColumnName, worseRankColumnName));
        }

        private static string GetRankImg(DataTable dt, string rankColumnName, string worseRankColumnName)
        {
            if (IsBestRank(dt, rankColumnName))
            {
                return string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (IsWorseRank(dt, rankColumnName, GetIntValue(dt, worseRankColumnName)))
            {
                return string.Format("<img src=\"../../../images/starGray.png\">");
            }
            return String.Empty;
        }        

        private static bool IsWorseRank(DataTable dt, string rankColumnName, int worseRank)
        {
            return GetIntValue(dt, rankColumnName) == worseRank;
        }

        private static bool IsBestRank(DataTable dt, string rankColumnName)
        {
            return GetIntValue(dt, rankColumnName) == 1;
        }

        private static int GetIntValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0][columnName]);
            }
            return 0;
        }

        #endregion

        #region Расходы

        private void InitializeOutcomes()
        {
            Label1.Text = String.Format("Расходы за {0} {1} {2}г.", month, CRHelper.RusManyMonthGenitive(month), year);

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0001_Outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbOutcomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Годовые назначения"]) / 1000);
            lbOutcomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Факт"]) / 1000);
            lbOutcomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);

            lbOutcomesRankAllValue.Text = GetRankText(dt, "Ранг процент", "Худший ранг процент");
            lbOutcomesRankAllValue.Style.Add("line-height", "40px");                          
            lbOutcomesRankAllAverageValue.Text = GetRankText(dt, "Ранг бюджетной обеспеченности", "Худший ранг бюджетной обеспеченности");
            lbOutcomesRankAllAverageValue.Style.Add("line-height", "40px");

            lbOutcomesRankFOValue.Text = GetRankText(dt, "Ранг процент среди соседей", "Худший ранг процент среди соседей");
            lbOutcomesRankFOValue.Style.Add("line-height", "40px");
            lbOutcomesRankFOAverageValue.Text = GetRankText(dt, "Ранг бюджетной обеспеченности среди соседей", "Худший ранг бюджетной обеспеченности среди соседей");
            lbOutcomesRankFOAverageValue.Style.Add("line-height", "40px");

            double value;
            if (double.TryParse(dt.Rows[0]["Коэффициент бюджетной обеспеченности населения"].ToString(), out value))
            {
                lbOutcomesAverageValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Коэффициент бюджетной обеспеченности населения"]));
            }            

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);
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
            Infragistics.UltraGauge.Resources.LinearGauge linearGauge = new Infragistics.UltraGauge.Resources.LinearGauge();
            linearGauge.CornerExtent = 10;
            linearGauge.MarginString = "2, 10, 2, 10, Pixels";

            // Выбираем максимум шкалы 
            double endValue = (Math.Max(100, markerValue));

            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = new Infragistics.UltraGauge.Resources.LinearGaugeScale();
            scale.EndExtent = 98;
            scale.StartExtent = 2;
            scale.OuterExtent = 93;
            scale.InnerExtent = 52;

            Infragistics.UltraGauge.Resources.SimpleGradientBrushElement gradientBrush = new Infragistics.UltraGauge.Resources.SimpleGradientBrushElement();
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

        private static void AddMajorTickmarkScale(double endValue, Infragistics.UltraGauge.Resources.LinearGauge linearGauge)
        {
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = new Infragistics.UltraGauge.Resources.LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.MajorTickmarks.EndWidth = 2;
            scale.MajorTickmarks.StartWidth = 2;
            scale.MajorTickmarks.EndExtent = 40;
            scale.MajorTickmarks.StartExtent = 25;
            scale.MajorTickmarks.BrushElements.Add(new Infragistics.UltraGauge.Resources.SolidFillBrushElement(Color.White));
            scale.Axes.Add(new Infragistics.UltraGauge.Resources.NumericAxis(0, endValue + endValue / 30, endValue / 10));
            linearGauge.Scales.Add(scale);
        }

        private static void AddGradient(Infragistics.UltraGauge.Resources.LinearGauge linearGauge)
        {
            Infragistics.UltraGauge.Resources.SimpleGradientBrushElement gradientBrush = new Infragistics.UltraGauge.Resources.SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(150, 150, 150);
            gradientBrush.StartColor = Color.FromArgb(10, 255, 255, 255);
            gradientBrush.GradientStyle = Infragistics.UltraGauge.Resources.Gradient.BackwardDiagonal;
            linearGauge.BrushElements.Add(gradientBrush);
        }

        private static void AddMainScale(double endValue, Infragistics.UltraGauge.Resources.LinearGauge linearGauge, double markerValue)
        {
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = new Infragistics.UltraGauge.Resources.LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.Axes.Add(new Infragistics.UltraGauge.Resources.NumericAxis(0, endValue + endValue / 30, endValue / 5));

            AddMainScaleRange(scale, markerValue);
            SetMainScaleLabels(scale);
            linearGauge.Scales.Add(scale);
        }

        private static void SetMainScaleLabels(Infragistics.UltraGauge.Resources.LinearGaugeScale scale)
        {

            scale.Labels.ZPosition = Infragistics.UltraGauge.Resources.LinearTickmarkZPosition.AboveMarkers;
            scale.Labels.Extent = 9;
            Font font = new Font("Arial", 9);
            scale.Labels.Font = font;
            scale.Labels.EqualFontScaling = false;
            Infragistics.UltraGauge.Resources.SolidFillBrushElement solidFillBrushElement = new Infragistics.UltraGauge.Resources.SolidFillBrushElement(Color.White);
            solidFillBrushElement.RelativeBoundsMeasure = Infragistics.UltraGauge.Resources.Measure.Percent;
            Rectangle rect = new Rectangle(0, 0, 80, 0);
            solidFillBrushElement.RelativeBounds = rect;
            scale.Labels.BrushElements.Add(solidFillBrushElement);
            scale.Labels.Shadow.Depth = 2;
            scale.Labels.Shadow.BrushElements.Add(new Infragistics.UltraGauge.Resources.SolidFillBrushElement());
        }

        private static void AddMainScaleRange(Infragistics.UltraGauge.Resources.LinearGaugeScale scale, double markerValue)
        {
            Infragistics.UltraGauge.Resources.LinearGaugeRange range = new Infragistics.UltraGauge.Resources.LinearGaugeRange();
            range.EndValue = markerValue;
            range.StartValue = 0;
            range.OuterExtent = 80;
            range.InnerExtent = 20;
            Infragistics.UltraGauge.Resources.SimpleGradientBrushElement gradientBrush = new Infragistics.UltraGauge.Resources.SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(1, 51, 75);
            gradientBrush.StartColor = Color.FromArgb(8, 218, 164);
            gradientBrush.GradientStyle = Infragistics.UltraGauge.Resources.Gradient.Vertical;
            range.BrushElements.Add(gradientBrush);
            scale.Ranges.Add(range);
        }

        #endregion

        #region Бюджет

        private void InitializeBudget()
        {
            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;

            UltraWebGridBudget.DataBind();
            UltraWebGridBudget.Style.Add("margin-top", "-4px");

            UltraWebGridOutcomes.Width = Unit.Empty;
            UltraWebGridOutcomes.Height = Unit.Empty;

            UltraWebGridOutcomes.DataBind();
            UltraWebGridOutcomes.Style.Add("margin-top", "-4px");            
        }

        protected void UltraWebGridBudget_DataBinding(object sender, EventArgs e)
        {
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0001_budget_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtIncomes);

            DataTable dtOutcomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0001_budget_outcomes_all");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtOutcomes);
            
            DataTable source = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0001_budget_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dtOutcomes.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dtOutcomes.Rows.Add(newRow);
            }

            dtIncomes.Columns.RemoveAt(5);
            dtIncomes.Columns.RemoveAt(3);
            UltraWebGridBudget.Style.Add("margin-right", "-10px");
            UltraWebGridBudget.DataSource = dtIncomes;

            dtOutcomes.Columns.RemoveAt(5);
            dtOutcomes.Columns.RemoveAt(3);
            UltraWebGridOutcomes.Style.Add("margin-right", "-10px");
            UltraWebGridOutcomes.DataSource = dtOutcomes;
        }

        protected void UltraWebGridBudget_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Width = 142;
            e.Layout.Bands[0].Columns[1].Width = 81;
            e.Layout.Bands[0].Columns[2].Width = 81;
            e.Layout.Bands[0].Columns[3].Width = 70;

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

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            e.Layout.Bands[0].Columns[3].Header.Caption = "Темп роста %";

            e.Layout.RowStyleDefault.Padding.Top = 1;
            e.Layout.RowStyleDefault.Padding.Bottom = 1;
            e.Layout.RowStyleDefault.Padding.Left = 1;
            e.Layout.RowStyleDefault.Padding.Right = 1;
        }

        protected void UltraWebGridBudget_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().ToLower() != "итого доходов " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "итого расходов")
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
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 6px center; padding-top: 2px";
            }
            e.Row.Cells[1].Style.Padding.Right = 3;
            e.Row.Cells[2].Style.Padding.Right = 3;
            e.Row.Cells[3].Style.Padding.Right = 1;
        }

        #endregion

        String text2 = string.Empty;

        void UltraChart12_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, text2, 2);
        }

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
            object[] fictiveValue = { ToolTip, 100 };
            actionDataTable.Rows.Add(fictiveValue);
            chart.DataSource = actionDataTable;
            chart.DataBind();
        }

        private void InitializeBkku()
        {            
            UltraChart12.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart12_FillSceneGraph);           

            DataTable incomesWithoutBp = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0001_Incomes_without_BP");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", incomesWithoutBp);

            DataTable innerFinSource = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0001_inner_fin_source");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", innerFinSource);

            DataTable deficite = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0001_Deficite");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", deficite);

            double incomesWithoutBpValue;
            Double.TryParse(incomesWithoutBp.Rows[0][1].ToString(), out incomesWithoutBpValue);
            
            double restsChangeValue;
            Double.TryParse(innerFinSource.Rows[0][2].ToString(), out restsChangeValue);

            double sellIncomesValue;
            Double.TryParse(innerFinSource.Rows[0][3].ToString(), out sellIncomesValue);

            double deficiteProficiteValue;
            Double.TryParse(deficite.Rows[0][1].ToString(), out deficiteProficiteValue);

            BindDeficiteIndicator(incomesWithoutBpValue, restsChangeValue, sellIncomesValue, deficiteProficiteValue);
        }       

        private void BindDeficiteIndicator(double incomesWithoutBpValue, double restsChangeValue, double sellIncomesValue, double deficiteProficiteValue)
        {
            double deficiteValue = deficiteProficiteValue > 0 ? 0 : deficiteProficiteValue * -1;
            double indicator = deficiteValue > 0 && incomesWithoutBpValue != 0 ? (deficiteValue - restsChangeValue - sellIncomesValue) / incomesWithoutBpValue : 0;
            indicator = (deficiteValue - restsChangeValue - sellIncomesValue) < 0 ? 0 : indicator;
            double deficiteLimit = deficiteValue == 0 ? 0 : incomesWithoutBpValue * 0.1 + restsChangeValue + sellIncomesValue;
            bool crime = indicator > 0.1;

            string deficiteDescription = String.Empty;

            if (deficiteProficiteValue == 0)
            {
                deficiteDescription = String.Format("Сбалансированный бюджет<br/>Предельный дефицит (с учетом изменения остатков и поступлений от продажи акций): &nbsp;<span style=\"color: white\"><b>{0:N2}</b></span>&nbsp; тыс.руб.", deficiteLimit / 1000);
            }
            else if (deficiteProficiteValue > 0)
            {
                deficiteDescription = 
                    String.Format("Профицит бюджета по плану на {0} год: &nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp; тыс.руб.<br/>Предельный дефицит (с учетом изменения остатков и поступлений от продажи акций): &nbsp;<span style=\"color: white\"><b>{2:N2}</b></span>&nbsp; тыс.руб.", 
                    year, deficiteProficiteValue / 1000, deficiteLimit / 1000);
            }
            else
            {
                deficiteDescription =
                    String.Format(
                        "Дефицит бюджета МО (с учетом изменения остатков и поступлений от продажи акций):<br/>по плану на {0} год: &nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp; тыс.руб.<br/>предельный дефицит: &nbsp;<span style=\"color: white\"><b>{2:N2}</b></span>&nbsp; тыс.руб.",
                        year, deficiteValue / 1000, deficiteLimit / 1000);
            }

            if (deficiteDescription.Contains("Профицит бюджета"))
            {
                text2 = "Нет";
            }
            else
            {
                text2 = String.Format("{0:P2}", indicator);
            }
            string deficiteTooltip = String.Format("<span style=\"color: white\"><b>Соблюдение ограничения дефицита бюджета</b></span><br/>Дефицит бюджета МО (с учетом изменения остатков и поступлений от продажи акций) не может превышать 10% доходов без учета финансовой помощи от других бюджетов и средств областного фонда компенсаций<br/>Значение индикатора: &nbsp;<span style=\"color: white\"><b>{0:P2}</b></span>&nbsp;<br/><span style=\"color: white\"><b>{1}</b></span>&nbsp;<br/>{2}",
                                                      indicator, GetCrime(crime), deficiteDescription);

            ConfigureActionChart(UltraChart12, crime, deficiteTooltip);
            lbDeficite.Text = deficiteTooltip;
        }

        private static string GetCrime(bool crime)
        {
            return crime ? "Нарушение БК РФ" : "Нарушений нет";
        }
    }
}