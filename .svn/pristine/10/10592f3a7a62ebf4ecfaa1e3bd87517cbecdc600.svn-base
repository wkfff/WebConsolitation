using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Text = Infragistics.UltraChart.Core.Primitives.Text;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0015_v : CustomReportPage
    {        
        private CustomParam regionsLevel;

        protected override void Page_Load(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_Load(sender, e);

            iPadBricks.iPadBricks.MoPassportHelper.InitRegionSettings(UserParams);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0015_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            // HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}' <img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/Heralds/{0}.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"], HttpContext.Current.Session["CurrentSiteRef"]);

            if (UserParams.Mo.Value == null ||
                UserParams.Mo.Value == String.Empty)
            {
                UserParams.Mo.Value = "Ахтубинский район";
            }
            UserParams.Filter.Value = RegionsNamingHelper.LocalBudgetUniqueNames[UserParams.Mo.Value];

            if (UserParams.Mo.Value.Contains("г.") || UserParams.Mo.Value.Contains("р.п.") || UserParams.Mo.Value.ToLower().Contains("город ") || UserParams.Mo.Value.ToLower().Contains("знаменск"))
            {
                UltraWebGridBudgetSettlement.Visible = false;
                settlementHeader.Visible = false;
                selfBudgetHeader.Visible = false;
                UltraWebGridBudgetSelf.Visible = false;
                Label2.Text = "Бюджет городского округа";
                lbSettlementsList.Visible = false;
            }
            else
            {
                InitializeBudgetSettlement();
            }

            InitializeBkku();
            InitializeBudget();
            InitializeCreditDebts();
            //creditDebts.Text = String.Format("Всего просроченная задолженность&nbsp;<span style=\"color: white\"><b>{0:N0}</b></span>, в том числе:<br/>&nbsp;&nbsp;&nbsp;&nbsp;по оплате труда с начислениями:&nbsp;<span style=\"color: white\"><b>{1:N0}</b></span><br/>&nbsp;&nbsp;&nbsp;&nbsp;по коммунальным услугам:&nbsp;<span style=\"color: white\"><b>{2:N0}</b></span><br/>&nbsp;&nbsp;&nbsp;&nbsp;по прочим выплатам:&nbsp;<span style=\"color: white\"><b>{3:N0}</b></span>", dtСreditDebts.Rows[0][1], dtСreditDebts.Rows[1][1], dtСreditDebts.Rows[2][1], dtСreditDebts.Rows[3][1]);

            fns28nSplitting = false;
            cubeName.Value = fns28nSplitting ? "ФНС_28н_с расщеплением" : "ФНС_28н_без расщепления";

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            topRegionsCount.Value = "5";

            UltraWebGrid.DataBind();
            UltraWebGridOkved.DataBind();
            UltraWebGridArrearAll.DataBind();
        }

        private void InitializeCreditDebts()
        {
            DataTable dtСreditDebts = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0015_credit_debts");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtСreditDebts);

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.DataSource = dtСreditDebts;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (!e.Row.Cells[0].Value.ToString().Contains("Всего"))
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }

            int grownCellIndex = 3;

            if (e.Row.Cells[grownCellIndex] != null &&
                e.Row.Cells[grownCellIndex].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[grownCellIndex].Value.ToString());

                double ethalonValue = 0;

                string img = String.Empty;
                if (value > ethalonValue)
                {
                    img = "~/images/arrowRedUpBB.png";
                }
                else if (value < ethalonValue)
                {
                    img = "~/images/arrowGreenDownBB.png";
                }
                e.Row.Cells[grownCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[grownCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 40px center; padding-top: 2px";
                //   e.Row.Cells[3].Title = title;
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            e.Layout.Bands[0].Columns[0].Width = 190;
            e.Layout.Bands[0].Columns[1].Width = 190;
            e.Layout.Bands[0].Columns[2].Width = 191;
            e.Layout.Bands[0].Columns[3].Width = 191;

            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");

            int reportYear = year;
            int reportMonth = month + 1;

            string dateMonth = string.Format("Задолженность на 01.{0:00}.{1} тыс.руб.", reportMonth, reportYear);

            e.Layout.Bands[0].Columns[1].Header.Caption = "Задолженность на начало года тыс.руб.";
            e.Layout.Bands[0].Columns[2].Header.Caption = dateMonth;
            e.Layout.Bands[0].Columns[3].Header.Caption = "Рост/снижение с начала года тыс.руб.";

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

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
            object[] fictiveValue = { ToolTip, 100 };
            actionDataTable.Rows.Add(fictiveValue);
            chart.DataSource = actionDataTable;
            chart.DataBind();
        }


        private void InitializeBkku()
        {
            UltraChart11.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart11_FillSceneGraph);
            UltraChart12.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart12_FillSceneGraph);
            UltraChart13.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart13_FillSceneGraph);

            DataTable dtInnerDebt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0015_Inner_Debt");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtInnerDebt);

            DataTable incomesWithoutBp = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0015_Incomes_without_BP");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", incomesWithoutBp);

            DataTable innerFinSource = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0015_inner_fin_source");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", innerFinSource);

            DataTable deficite = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0015_Deficite");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", deficite);

            DataTable outcomesDebt = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0015_Outcomes_debt");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", outcomesDebt);

            double innerDebtValue;
            Double.TryParse(dtInnerDebt.Rows[0][1].ToString(), out innerDebtValue);

            double incomesWithoutBpValue;
            Double.TryParse(incomesWithoutBp.Rows[0][1].ToString(), out incomesWithoutBpValue);

            double budjetCreditValue;
            Double.TryParse(innerFinSource.Rows[0][1].ToString(), out budjetCreditValue);

            double restsChangeValue;
            Double.TryParse(innerFinSource.Rows[0][2].ToString(), out restsChangeValue);

            double sellIncomesValue;
            Double.TryParse(innerFinSource.Rows[0][3].ToString(), out sellIncomesValue);

            double deficiteProficiteValue;
            Double.TryParse(deficite.Rows[0][1].ToString(), out deficiteProficiteValue);

            double bpValue;
            Double.TryParse(incomesWithoutBp.Rows[0][2].ToString(), out bpValue);

            double outcomesServeValue;
            Double.TryParse(outcomesDebt.Rows[0][1].ToString(), out outcomesServeValue);

            double outcomesAllValue;
            Double.TryParse(outcomesDebt.Rows[0][2].ToString(), out outcomesAllValue);

            BindDebtIndicator(innerDebtValue, incomesWithoutBpValue, budjetCreditValue);
            BindDeficiteIndicator(incomesWithoutBpValue, restsChangeValue, sellIncomesValue, deficiteProficiteValue);
            BindServeIndicator(bpValue, outcomesServeValue, outcomesAllValue);
        }

        private void BindServeIndicator(double bpValue, double outcomesServeValue, double outcomesAllValue)
        {
            double outcomesWithoutBp = outcomesAllValue - bpValue;
            double limitServe = outcomesWithoutBp * 0.15;
            double indicator = outcomesServeValue > 0 ? outcomesServeValue / outcomesWithoutBp : 0;
            bool crime = limitServe - outcomesServeValue < 0;

            string serveDescription = outcomesServeValue == 0 ? "Расходы на обслуживание долга &nbsp;<span style=\"color: white\"><b>отсутствуют</b></span>&nbsp;" : String.Format("Расходы на обслуживание долга {0}: &nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp; тыс.руб.", String.Format("за {0} месяцев {1} года", month, year), outcomesServeValue / 1000);

            string debtOutcomesTooltip = String.Format("<span style=\"color: white\"><b>Соблюдение ограничения расходов на обслуживание долга</b></span>&nbsp;<br/>Приведены данные по собственному бюджету района без учета бюджетов поселений<br/>Расходы на обслуживание долга МО ограничиваются 15% расходов бюджета (без учета расходов за счет финансовой помощи от других бюджетов)<br/>Значение индикатора: &nbsp;<span style=\"color: white\"><b>{0:P2}</b></span>&nbsp;<br/><span style=\"color: white\"><b>{1}</b></span>&nbsp;<br/>{2}<br/>Предельный размер расходов на обслуживание долга: &nbsp;<span style=\"color: white\"><b>{3:N2}</b></span>&nbsp; тыс.руб.",
                                                indicator, GetCrime(crime), serveDescription, limitServe / 1000);

            if (serveDescription.Contains("Расходы на обслуживание долга &nbsp;<span style=\"color: white\"><b>отсутствуют</b></span>&nbsp;"))
            {
                text3 = "Нет";
            }
            else
            {
                text3 = String.Format("{0:P3}", indicator);
            }

            ConfigureActionChart(UltraChart13, crime, debtOutcomesTooltip);
            lbDebtServe.Text = debtOutcomesTooltip;
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
                deficiteDescription = String.Format("Профицит бюджета по плану на {0} год: &nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp; тыс.руб.<br/>Предельный дефицит (с учетом изменения остатков и поступлений от продажи акций): &nbsp;<span style=\"color: white\"><b>{2:N2}</b></span>&nbsp; тыс.руб.", year, deficiteProficiteValue, deficiteLimit / 1000);
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
            string deficiteTooltip = String.Format("<span style=\"color: white\"><b>Соблюдение ограничения дефицита бюджета</b></span>&nbsp;<br/>Приведены данные по собственному бюджету района без учета бюджетов поселений<br/>Дефицит бюджета МО (с учетом изменения остатков и поступлений от продажи акций) не может превышать 10% доходов без учета финансовой помощи от других бюджетов и средств областного фонда компенсаций<br/>Значение индикатора: &nbsp;<span style=\"color: white\"><b>{0:P2}</b></span>&nbsp;<br/><span style=\"color: white\"><b>{1}</b></span>&nbsp;<br/>{2}",
                                                      indicator, GetCrime(crime), deficiteDescription);

            ConfigureActionChart(UltraChart12, crime, deficiteTooltip);
            lbDeficite.Text = deficiteTooltip;
        }

        private void BindDebtIndicator(double innerDebtValue, double incomesWithoutBpValue, double budjetCreditValue)
        {
            double debtLimit = incomesWithoutBpValue + budjetCreditValue;
            double debtIndicator = incomesWithoutBpValue != 0 ? innerDebtValue / incomesWithoutBpValue : 0;
            bool crime = innerDebtValue + budjetCreditValue > incomesWithoutBpValue;

            string debtDescription = innerDebtValue == 0 ? "Муниципальный долг &nbsp;<span style=\"color: white\"><b>отсутствует</b></span>&nbsp;" : String.Format("Объем долга МО {0}: &nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp; тыс.руб.", String.Format("за {0} месяцев {1} года", month, year), innerDebtValue / 1000);

            string debtTooltip = String.Format("<span style=\"color: white\"><b>Соблюдение ограничения на объем долга, установленного БК РФ</b></span>&nbsp;<br/>Приведены данные по собственному бюджету района без учета бюджетов поселений<br/>Объем долга МО не может превышать объем доходов с учетом бюджетных кредитов и без учета финансовой помощи от других бюджетов и средств областного фонда компенсаций<br/>Значение индикатора: &nbsp;<span style=\"color: white\"><b>{0:P2}</b></span>&nbsp;<br/><span style=\"color: white\"><b>{1}</b></span>&nbsp;<br/>{2}<br/>Предельное значение объема долга МО: &nbsp;<span style=\"color: white\"><b>{3:N2}</b></span>&nbsp; тыс.руб.",
                                                debtIndicator, GetCrime(crime), debtDescription, debtLimit / 1000);

            if (debtDescription.Contains("Муниципальный долг &nbsp;<span style=\"color: white\"><b>отсутствует</b></span>&nbsp;"))
            {
                text1 = "Нет";
            }
            else
            {
                text1 = String.Format("{0:P2}", debtIndicator);
            }
            ConfigureActionChart(UltraChart11, crime, debtTooltip);
            lbDebts.Text = debtTooltip;
        }

        private static string GetCrime(bool crime)
        {
            return crime ? "Нарушение БК РФ" : "Нарушений нет";
        }

        #endregion

        #region Бюджет

        private int monthNum = 1;

        private void InitializeBudget()
        {
            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;
            UltraWebGridBudget.DataBind();

            UltraWebGridBudgetSelf.Width = Unit.Empty;
            UltraWebGridBudgetSelf.Height = Unit.Empty;
            UltraWebGridBudgetSelf.DataBind();
        }

        protected void UltraWebGridBudget_DataBinding(object sender, EventArgs e)
        {
            UserParams.BudgetLevelEnum.Value = "[Уровни бюджета].[СКИФ].[Конс.бюджет МО ]";

            DataTable dt = GetBudetDataSource();

            UltraWebGridBudget.Style.Add("margin-right", "-10px");
            UltraWebGridBudget.DataSource = dt;
        }

        protected void UltraWebGridBudgetSelf_DataBinding(object sender, EventArgs e)
        {
            UserParams.BudgetLevelEnum.Value = "[Уровни бюджета].[СКИФ].[Собств.бюджет МО ]";

            DataTable dt = GetBudetDataSource();

            UltraWebGridBudgetSelf.Style.Add("margin-right", "-10px");
            UltraWebGridBudgetSelf.DataSource = dt;
        }

        private static DataTable GetBudetDataSource()
        {
            DataTable dt = new DataTable();

            string query = DataProvider.GetQueryText("FO_0035_0015_budget_incomes_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("FO_0035_0015_budget_outcomes_all_v");
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
            query = DataProvider.GetQueryText("FO_0035_0015_budget_outcomes_v");
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
            query = DataProvider.GetQueryText("FO_0035_0015_budget_deficite_v");
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

            dt.AcceptChanges();

            foreach (DataRow row in dt.Rows)
            {
                double valueLastYear;
                double valueCurrYear;
                if (Double.TryParse(row["Факт по области прошлый год "].ToString(), out valueLastYear) &&
                   Double.TryParse(row["Факт по области этот год "].ToString(), out valueCurrYear) &&
                   valueLastYear != 0)
                {
                    row["Темп роста по области"] = valueCurrYear / valueLastYear;
                }
            }
            dt.AcceptChanges();
            return dt;
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
            e.Layout.Bands[0].Columns[1].Width = 93;
            e.Layout.Bands[0].Columns[2].Width = 93;
            e.Layout.Bands[0].Columns[3].Width = 93;
            e.Layout.Bands[0].Columns[4].Width = 104;
            e.Layout.Bands[0].Columns[5].Width = 103;
            e.Layout.Bands[0].Columns[6].Width = 105;

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
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("За {0} мес. {1}г. тыс.руб.", month, year - 1);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("План на {0}г. тыс.руб.", year);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            e.Layout.Bands[0].Columns[3].Header.Caption = String.Format("За {0} мес. {1}г. тыс.руб.", month, year);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            e.Layout.Bands[0].Columns[4].Header.Caption = "% исп.";

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P1");
            e.Layout.Bands[0].Columns[5].Header.Caption = "Темп роста %";

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P1");
            e.Layout.Bands[0].Columns[6].Header.Caption = "Темп роста по области %";

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
            e.Layout.Bands[0].Columns[7].Header.Caption = "Ранг";

            e.Layout.RowStyleDefault.Padding.Top = 1;
            e.Layout.RowStyleDefault.Padding.Bottom = 1;
            e.Layout.RowStyleDefault.Padding.Left = 1;
            e.Layout.RowStyleDefault.Padding.Right = 1;

            e.Layout.Bands[0].Columns[7].Hidden = true;
            e.Layout.Bands[0].Columns[8].Hidden = true;
            e.Layout.Bands[0].Columns[9].Hidden = true;
            e.Layout.Bands[0].Columns[10].Hidden = true;
        }

        protected void UltraWebGridBudget_InitializeRow(object sender, RowEventArgs e)
        {
            int grownCellIndex = 5;
            int ethalonGrownCellIndex = 6;
            int rankCellIndex = 7;
            int worseRankCellIndex = 8;

            if (e.Row.Cells[0].Value.ToString().ToLower() != "итого доходов " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "итого расходов " &&
                !e.Row.Cells[0].Value.ToString().ToLower().Contains("профицит(+)/"))
            {
                e.Row.Cells[0].Style.Padding.Left = 10;
            }

            if (e.Row.Cells[grownCellIndex] != null &&
                e.Row.Cells[grownCellIndex].Value != null &&
                e.Row.Cells[ethalonGrownCellIndex] != null &&
                   e.Row.Cells[ethalonGrownCellIndex].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[grownCellIndex].Value.ToString());

                double ethalonValue = Convert.ToDouble(e.Row.Cells[ethalonGrownCellIndex].Value.ToString());

                string img;
                if (value > ethalonValue)
                {
                    img = "~/images/CornerGreen.gif";
                }
                else
                {
                    img = "~/images/CornerRed.gif";
                }
                e.Row.Cells[grownCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[grownCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 2px";
            }

            string img1 = String.Empty;

            if (e.Row.Cells[grownCellIndex] != null &&
                e.Row.Cells[grownCellIndex].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[grownCellIndex].Value.ToString());

                if (value > 1)
                {
                    img1 = String.Format("<img src=\"../../../images/arrowGreenUpBB.png\" style=\"float: left\">");

                }
                else
                {
                    img1 = String.Format("<img src=\"../../../images/arrowRedDownBB.png\" style=\"float: left\">");
                }
                e.Row.Cells[grownCellIndex].Value = String.Format("{0} {1:P1}", img1, e.Row.Cells[grownCellIndex].Value);
            }

            if (RankPresent(e.Row.Cells[rankCellIndex], e.Row.Cells[worseRankCellIndex]))
            {
                SetRankImage(e.Row.Cells[rankCellIndex], e.Row.Cells[worseRankCellIndex]);
            }

            e.Row.Cells[1].Style.Padding.Right = 3;
            e.Row.Cells[2].Style.Padding.Right = 3;
            e.Row.Cells[3].Style.Padding.Right = 1;

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

                if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
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
                    e.Row.Cells[3].Style.BackgroundImage = img;
                    e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                    //  e.Row.Cells[2].Title = title;
                }
            }
        }

        private static void SetRankImage(UltraGridCell rankCell, UltraGridCell worseRankCell)
        {
            double rank = Convert.ToDouble(rankCell.Value.ToString());
            double worseRank = Convert.ToDouble(worseRankCell.Value.ToString());
            string img = String.Empty;
            string title;
            if (rank == 1)
            {
                img = "~/images/starYellow.png";
                title = "Рост к прошлому году";
            }
            else if (rank == worseRank)
            {
                img = "~/images/starGray.png";
                title = "Падение к прошлому году";
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

        //        private static string ParseDTValue(DataTable dt, int rowIndex, int columnIndex)
        //        {
        //            if (dt == null || dt.Rows[rowIndex][columnIndex] == DBNull.Value)
        //            {
        //                return string.Empty;
        //            }
        //
        //            return dt.Rows[rowIndex][columnIndex].ToString();
        //        }

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

        protected void UltraWebGridArrearAll_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string queryName = "FNS_0001_0001_v";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            UltraWebGridArrearAll.Width = Unit.Empty;
            UltraWebGridArrearAll.Height = Unit.Empty;
            UltraWebGridArrearAll.DataSource = dtGrid;
        }

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
            string queryName = "FNS_0001_0001_v_incomes_debt";
            query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGridOkved_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string queryName = "FNS_0001_0001_v_incomes_okved";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            UltraWebGridOkved.Width = Unit.Empty;
            UltraWebGridOkved.Height = Unit.Empty;
            UltraWebGridOkved.DataSource = dtGrid;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = 190;
            e.Layout.Bands[0].Columns[1].Width = 91;
            e.Layout.Bands[0].Columns[2].Width = 91;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 130;
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
            int rowIndex = 0;
            int percentColumnIndex = 4;
            int indincateColumnIndex = 4;

            e.Row.Cells[0].Style.Font.Size = 12;
            e.Row.Cells[1].Style.Font.Size = 12;

            //if (!e.Row.Cells[0].Value.ToString().Contains("Всего"))
            //{
            //    e.Row.Cells[0].Style.Padding.Left = 20;
            //}
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
        #endregion

        #endregion

        #region Поселения

        private void InitializeBudgetSettlement()
        {
            DataTable dtSettlementsCount = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0015_settlements_count");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtSettlementsCount);

            Label8.Text = String.Format("Бюджеты поселений ({0})", dtSettlementsCount.Rows[0][0]);

            DataTable dtSettlementsList = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0015_settlements_list");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtSettlementsList);
            lbSettlementsList.Text = String.Empty;

            foreach (DataRow row in dtSettlementsList.Rows)
            {
                lbSettlementsList.Text = String.Format("{0}, {1}", lbSettlementsList.Text, row[0]);
            }

            lbSettlementsList.Text = lbSettlementsList.Text.Trim(',').Trim();

            UltraWebGridBudgetSettlement.Width = Unit.Empty;
            UltraWebGridBudgetSettlement.Height = Unit.Empty;

            UltraWebGridBudgetSettlement.DataBind();
        }

        protected void UltraWebGridBudgetSettlement_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            string query = DataProvider.GetQueryText("FO_0035_0015_settlement_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("FO_0035_0015_settlement_outcomes_all");
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
            query = DataProvider.GetQueryText("FO_0035_0015_settlement_outcomes");
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
            query = DataProvider.GetQueryText("FO_0035_0015_settlement_deficite");
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

            UltraWebGridBudgetSettlement.Style.Add("margin-right", "-10px");
            UltraWebGridBudgetSettlement.DataSource = dt;
        }

        #endregion
    }
}