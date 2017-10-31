using System;
using System.Data;
using System.Drawing;
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
    public partial class FO_0002_0004 : CustomReportPage
    {
        private DateTime date;
        // выбранный период
        private CustomParam gosDebtFilter;

        protected override void Page_Load(object sender, EventArgs e)
        {
            gosDebtFilter = UserParams.CustomParam("gos_debt_filter");

            base.Page_Load(sender, e);

            iPadBricks.iPadBricks.MoPassportHelper.InitRegionSettings(UserParams);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0004_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            date = new DateTime(year, month, 1).AddMonths(1);

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            CustomParam populationDate = UserParams.CustomParam("population_date");
            DataTable dtPopulationDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0004_population_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulationDate);
            populationDate.Value = dtPopulationDate.Rows[0][1].ToString();

            DataTable dtPopulation = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0004_population");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulation);

            lbPopulation.Text = String.Format("Численность постоянного населения&nbsp;<span style=\"color: white\"><b>{0:N0}</b></span>&nbsp;чел.", dtPopulation.Rows[0][0]);

            InitializeIncomes();
            InitializeOutcomes();
            InitializeBudget();


            if (UserParams.BudgetLevel.Value == "Конс.бюджет субъекта")
            {
                gosDebtFilter.Value = @", [Показатели__СопМесОСпВнтД].[Показатели__СопМесОСпВнтД].[Filter]";
                IPadElementHeader5.Text = "Государственный и муниципальный долг";
                InitializeFonds();
                CreditDebtsDiv.Visible = false;
            }
            else
            {
                gosDebtFilter.Value = ", [Показатели__СопМесОСпВнтД].[Показатели__СопМесОСпВнтД].[Все показатели].[ГОСУДАРСТВЕННЫЙ ВНУТРЕННИЙ ДОЛГ СУБЪЕКТА РОССИЙСКОЙ ФЕДЕРАЦИИ, всего]";
                FinHelp.Visible = false;
                DataTable dtСreditDebts = new DataTable();
                query = DataProvider.GetQueryText("FO_0935_0015_credit_debts");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtСreditDebts);

                creditDebts.Text = String.Format("Всего просроченная задолженность<br/><span style=\"color: white\"><b>{0:N0}</b></span>&nbsp;тыс.руб., в том числе по:<br/>&nbsp;&nbsp;&nbsp;&nbsp;оплате труда<br/>&nbsp;&nbsp;&nbsp;&nbsp;с начислениями:&nbsp;<span style=\"color: white\"><b>{1:N0}</b></span>&nbsp;тыс.руб.<br/>&nbsp;&nbsp;&nbsp;&nbsp;коммунальным {4}услугам:&nbsp;<span style=\"color: white\"><b>{2:N0}</b></span>&nbsp;тыс.руб.<br/>&nbsp;&nbsp;&nbsp;&nbsp;прочим выплатам:&nbsp;<span style=\"color: white\"><b>{3:N0}</b></span>&nbsp;тыс.руб.", dtСreditDebts.Rows[0][2], dtСreditDebts.Rows[1][2], dtСreditDebts.Rows[2][2], dtСreditDebts.Rows[3][2], Convert.ToDouble(dtСreditDebts.Rows[2][2].ToString()) > 100000 ? string.Empty : string.Empty);

                fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("FNS28nSplitting"));
                cubeName.Value = fns28nSplitting ? "ФНС_28н_с расщеплением" : "ФНС_28н_без расщепления";

                UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

                topRegionsCount.Value = "5";

                UltraWebGrid.DataBind();
                UltraWebGrid.Height = Unit.Empty;
            }

            InitializeDebts();
        }

        #region Доходы
        private DataTable dt;
        private DataTable newDt;

        private void InitializeIncomes()
        {
            IPadElementHeader2.Text = String.Format("Доходы за {0} {1} {2}г.", month, CRHelper.RusManyMonthGenitive(month), year);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0004_Incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbIncomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Годовые назначения"]) / 1000000);
            lbIncomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Факт"]) / 1000000);
            lbIncomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);

            double value;
            if (Double.TryParse(dt.Rows[0]["Среднедушевые доходы"].ToString(), out value))
            {
                lbIncomesAverageValue.Text = String.Format("{0:N0}", value);
            }


            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);

            gauge.Style.Add("padding-left", "45px");
            GaugeIncomesPlaceHolder.Controls.Add(gauge);
        }

        #endregion

        #region Расходы

        private void InitializeOutcomes()
        {
            IPadElementHeader3.Text = String.Format("Расходы за {0} {1} {2}г.", month, CRHelper.RusManyMonthGenitive(month), year);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0004_Outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbOutcomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Годовые назначения"]) / 1000000);
            lbOutcomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["Факт"]) / 1000000);
            lbOutcomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% выполнения годовых назначений"]) * 100);

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

        #region Фонды

        private void InitializeFonds()
        {
            DataTable dtDate = new DataTable();
            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_fonds_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodLastYear.Value = "2010";
            UserParams.PeriodYear.Value = "2011";

            query = DataProvider.GetQueryText("iPad_0001_0001_Fonds");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.TrimEnd('_');
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.Replace("br", "\n");
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        dt.Rows[i][j] = Convert.ToDouble(dt.Rows[i][j]) / 1000;
                    }
                }
            }


            UltraChartFonds.Series.Clear();
            for (int j = 1; j < dt.Columns.Count; j++)
            {
                UltraChartFonds.Series.Add(CRHelper.GetNumericSeries(j, dt));
            }


            //UltraChartFonds.DataSource = dt;
            UltraChartFonds.DataBind();

            UltraChartFonds.Width = 350;
            UltraChartFonds.Height = 345;
            // UltraChartFonds.Style.Add("margin-right", "")
            ((GradientEffect)UltraChartFonds.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChartFonds.ChartType = ChartType.StackColumnChart;
            UltraChartFonds.TitleLeft.Visible = true;
            UltraChartFonds.TitleLeft.Text = "                      Млн.руб.";
            UltraChartFonds.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChartFonds.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChartFonds.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            UltraChartFonds.TitleLeft.Font = new Font("Verdana", 10);
            UltraChartFonds.Axis.X.Extent = 50;
            UltraChartFonds.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChartFonds.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChartFonds.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChartFonds.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChartFonds.Axis.Y.Extent = 40;

            UltraChartFonds.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL> <ITEM_LABEL>\n<b><DATA_VALUE:N3></b>&nbsp;млн.руб.</span>";
            UltraChartFonds.Data.SwapRowsAndColumns = false;

            UltraChartFonds.Legend.Visible = true;
            UltraChartFonds.Legend.Location = LegendLocation.Bottom;
            UltraChartFonds.Legend.SpanPercentage = 10;
            UltraChartFonds.Legend.Font = new Font("Verdana", 10);
            //UltraChartFonds.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChartFonds_FillSceneGraph);
        }

        #endregion

        #region Бюджет

        private int monthNum = 1;

        private void InitializeBudget()
        {
            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;

            UltraWebGridBudget.DataBind();
        }

        protected void UltraWebGridBudget_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText("FO_0002_0004_budget_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("FO_0002_0004_budget_outcomes_all");
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
            query = DataProvider.GetQueryText("FO_0002_0004_budget_outcomes");
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
            query = DataProvider.GetQueryText("FO_0002_0004_budget_deficite");
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

            e.Layout.Bands[0].Columns[0].Width = 151;
            e.Layout.Bands[0].Columns[1].Width = 91;
            e.Layout.Bands[0].Columns[2].Width = 91;
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
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("за {0} мес. {1}г. млн.руб.", month, year - 1);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("за {0} мес. {1}г. млн.руб.", month, year);

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

        #region БККУ

        private int yearNum;

        private int monthNumDebt;
        private int yearNumDebt;

        private void InitializeDebts()
        {
            SetIndicatorsData();
        }

        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

        private string GetMbtDescription(string group)
        {
            switch (group)
            {
                case "1":
                    {
                        return "(более 60%)";
                    }
                case "2":
                    {
                        return "(от 20% до 60%)";
                    }
                case "3":
                    {
                        return "(от 5% до 20%)";
                    }
                case "4":
                    {
                        return "(менее 5%)";
                    }
                default:
                    {
                        return "(более 60)%";
                    }
            }
        }

        private void SetIndicatorsData()
        {
            #region Индикаторы
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

            lbRankCaption.Text = String.Format("Группа по доле МБТ на {0} год: ", yearNum);
            lbRankDescription.Text = GetMbtDescription(mbtGroup.Rows[0][1].ToString());
            lbRankCaption.CssClass = "InformationText";
            lbRankDescription.CssClass = "InformationText";

            if (mbtGroup.Rows[0][1] != DBNull.Value)
            {
                Rank.Text = string.Format("&nbsp;{0:N0}", mbtGroup.Rows[0][1]);
                Rank.CssClass = "DigitsValue";

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
            }
            else
            {
                Rank.Text = String.Empty;
                lbRankCaption.Text = String.Empty;
                lbRankDescription.Text = String.Empty;
            }
            #endregion

            DataTable debtsDate = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_debts_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, debtsDate);

            monthNumDebt = CRHelper.MonthNum(debtsDate.Rows[0][3].ToString());
            yearNumDebt = Convert.ToInt32(debtsDate.Rows[0][0].ToString());

            UserParams.PeriodYear.Value =
                    CRHelper.PeriodMemberUName("",
                                           new DateTime(yearNumDebt, monthNumDebt, 1), 4);
            UserParams.PeriodLastDate.Value =
                    CRHelper.PeriodMemberUName("",
                               new DateTime(yearNumDebt - 1, monthNumDebt, 1), 4);
            if (monthNumDebt != 1)
            {
                UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(-1), 4);
                UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt, monthNumDebt, 1).AddYears(-1).AddMonths(-1), 4);
            }
            else
            {
                UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt, 12, 1), 4);
                UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt, 12, 1).AddYears(-1), 4);
            }

            DataTable debts = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_GosDebts_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", debts);

            gosDebt.Text = String.Format("{0:N0}", debts.Rows[0][0]);
            Label5.Text = String.Format("{0:N4}", debts.Rows[0][1]);
            gosDebtAvg.Text = String.Format("{0:N0}", debts.Rows[0][2]);
            Label9.Text = String.Format("{0:N0}", debts.Rows[0][3]);


            Label1.Text = String.Format("Объем долга на {0:dd.MM.yyyy}", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
            Label3.Text = String.Format("Отношение долга к доходам (без безвозмездных поступлений) на {0:dd.MM.yyyy}", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
            Label2.Text = String.Format("На душу населения на {0:dd.MM.yyyy}", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
            Label7.Text = String.Format("Расходы на обслуживание долга<br/>на {0:dd.MM.yyyy}&nbsp;", new DateTime(yearNumDebt, monthNumDebt, 1).AddMonths(1));
        }
        
        #endregion

        #region недоимка
        private DataTable dtGrid;

        private bool fns28nSplitting;
        private int month;
        private int year;

        #region Параметры запроса

        // уровень МР и ГО

        // куб
        private CustomParam cubeName;
        // число выбранных регионов
        private CustomParam topRegionsCount;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (cubeName == null)
            {
                cubeName = UserParams.CustomParam("cube_name");
            }
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
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0001_date");
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

            dtGrid = new DataTable();
            string queryName = "FNS_0001_0001";
            query = DataProvider.GetQueryText(queryName);
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

            string dateMonth = string.Format("Недоимка на 01.{0:00}.{1} млн.руб.", reportMonth, reportYear);

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
                e.Row.Cells[3].Value = String.Format("{0:P2}", e.Row.Cells[3].Value);
            }
        }
        #endregion

        #endregion
    }
}