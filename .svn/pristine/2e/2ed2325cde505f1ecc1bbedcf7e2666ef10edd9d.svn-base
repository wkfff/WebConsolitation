using System;
using System.Data;
using System.Drawing;
using System.Web;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0004 : CustomReportPage
    {

        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;
        private DateTime debtsLastDateTime;
        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        private DateTime debtsCurrDateTime;

        private CustomParam periodPrevMonth;

        private string currentRegion;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.Mo.Value = !UserParams.Mo.Value.Contains("г.") ?
                        String.Format(".[{0} муниципальный район]", UserParams.Mo.Value) :
                        String.Format(".[{0}]", UserParams.Mo.Value.Replace("г.", "Город "));

            currentRegion = UserParams.Mo.Value.Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "МР").Replace("[", "").Replace("]", "").Replace(".DataMember", "");
            HeraldImageContainer.InnerHtml = String.Format("<img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/HeraldsMo/{0}.png\"></a>", HttpContext.Current.Session["CurrentMoID"]);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodPrevMonth == null)
            {
                periodPrevMonth = UserParams.CustomParam("period_prev_month");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (redundantLevelRFDate == null)
            {
                redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            }

            #endregion

            BindStatisticsText();
            BindRedudantLevelDescritionText();
            CommentTextDataBind();

            UltraChart3.Width = 520;
            UltraChart3.Legend.SpanPercentage = 61;
            UltraChart3.Legend.Margins.Bottom = 70;
            UltraChart3.Height = 150;
            SetUpPieChart(UltraChart3);
            AddColorModelChart2(UltraChart3);
            UltraChart3.DataBinding += UltraChart3_DataBinding;
            UltraChart3.DataBind();

            DataTable dtRedudantLevel = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0004_chart_Compare");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtRedudantLevel);

            UltraChart1.Width = 340;
            UltraChart1.Height = 310;
            UltraChart1.DataBinding += UltraChart1_DataBinding;
            SetUpPeopleChart(UltraChart1);
            UltraChart1.Legend.SpanPercentage = 52;
            AddColorModelChart1(UltraChart1);
            UltraChart1.Style.Add("margin-left", "-15px");
            UltraChart1.DataBind();
        }

        private void BindStatisticsText()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1_date");
            DataTable dtDateHmao = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateHmao);

            DateTime currDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[1][1].ToString(), 3);
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeHmao, 5);
            string dateTimeStr = string.Format("&nbsp;<span class='DigitsValue'><b>{0:dd.MM.yyyy}</b></span>", currDateTimeHmao);

            DataTable dtPopulation = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0004_population");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulation);

            DataTable dtActivePopulation = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0004_active_population");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtActivePopulation);

            double value = 0;
            double populationValue;
            double activePopulationValue;
            if (dtActivePopulation.Rows[0][0] != DBNull.Value && dtPopulation.Rows[0][0] != DBNull.Value &&
                Double.TryParse(dtActivePopulation.Rows[0][0].ToString(), out activePopulationValue) &&
                Double.TryParse(dtPopulation.Rows[0][0].ToString(), out populationValue))
            {
                value = activePopulationValue / populationValue;
            }

            lbStatistics.Text = String.Format(
                "Численность экономически активного населения &nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;человек (на{3})<br/> Численность населения&nbsp;<span class='DigitsValue'><b>{1:N0}</b></span>&nbsp;человек (на начало года)<br/> Доля экономически активного населения в общей численности населения&nbsp;<span class='DigitsValue'><b>{2:P2}</b></span>",
                dtActivePopulation.Rows[0][0], dtPopulation.Rows[0][0], value, dateTimeStr);
        }

        private void BindRedudantLevelDescritionText()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1_date");
            DataTable dtDateHmao = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateHmao);

            DateTime currDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[1][1].ToString(), 3);
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeHmao, 5);
            string dateTimeStr = string.Format("&nbsp;<span class='DigitsValue'><b>{0:dd.MM.yyyy}</b></span>&nbsp;", currDateTimeHmao);

            DataTable dtRedudantLevel = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0004_redudantLevel");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtRedudantLevel);

            double thisRedudant;
            double grownValue;
            string grown = String.Empty;
            string middleLevel = String.Empty;

            IPadElementHeader1.Text = String.Format("Зарегистрированная безработица на {0:dd.MM.yyyy}", currDateTimeHmao);

            // преобразуем дименшен даты в 2000 вид
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTimeHmao, 5);

            // Возьмем данные по федеральной базе
            DataTable dtDateCur = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateCur);

            DateTime currDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[0][5].ToString(), 3);
            DateTime lastDateTime = currDateTime.AddDays(-7);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);

            query = DataProvider.GetQueryText("STAT_0001_0002_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            query = DataProvider.GetQueryText("STAT_0001_0002_redundantLevelRF_date");
            DataTable dtRedundantLevelRFDate = new DataTable();

            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtRedundantLevelRFDate);
            redundantLevelRFDate.Value = dtRedundantLevelRFDate.Rows[0][1].ToString();
            DateTime redundantLevelRFDateTime = CRHelper.DateByPeriodMemberUName(dtRedundantLevelRFDate.Rows[0][1].ToString(), 3);

            if (dtDebtsDate.Rows.Count > 1)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }

            query = DataProvider.GetQueryText("STAT_0001_0002_commentText");
            DataTable dtCommentText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);


            double redundantLevelHmao;

            if (Double.TryParse(dtRedudantLevel.Rows[0][1].ToString(), out thisRedudant) &&
               Double.TryParse(dtRedudantLevel.Rows[0][4].ToString(), out redundantLevelHmao))
            {
                #region УрФО и РФ
                double redundantLevelRFValue = GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы РФ");

                string redundantLevelRFArrow;
                string redundantLevelRFDescription = String.Format("&nbsp;<span class='DigitsValue'><b>{0:N2}%</b></span>", redundantLevelRFValue);
                if (thisRedudant > redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (thisRedudant < redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
                }
                else
                {
                    redundantLevelRFArrow = "соответствует&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;уровню";
                    redundantLevelRFDescription = String.Empty;
                }
                string redundantLevelRFGrow = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}чем в РФ{1}<br/>", redundantLevelRFArrow, redundantLevelRFDescription);

                double redundantLevelUrfoValue = GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы УрФО");
                string redundantLevelUrfoArrow;
                string redundantLevelUrfoDescription = String.Format("&nbsp;<span class='DigitsValue'><b>{0:N2}%</b></span>", redundantLevelUrfoValue);
                if (thisRedudant > redundantLevelUrfoValue)
                {
                    redundantLevelUrfoArrow = "выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (thisRedudant < redundantLevelUrfoValue)
                {
                    redundantLevelUrfoArrow = "ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
                }
                else
                {
                    redundantLevelUrfoArrow = "соответствует&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;уровню";
                    redundantLevelUrfoDescription = String.Empty;
                }
                string redundantLevelUrfoGrow = String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}чем в УрФО{1}<br/>", redundantLevelUrfoArrow, redundantLevelUrfoDescription);

                redundantLevelRFGrow = String.Format("{0}{1}", redundantLevelUrfoGrow, redundantLevelRFGrow);
                #endregion

                string redundantLevelHmaoArrow;
                string redundantLevelHmaoDescription = String.Format("&nbsp;<span class='DigitsValue'><b>{0:P2}</b></span>", redundantLevelHmao);
                if (thisRedudant > redundantLevelHmao)
                {
                    redundantLevelHmaoArrow = "выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (thisRedudant < redundantLevelRFValue)
                {
                    redundantLevelHmaoArrow = "ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
                }
                else
                {
                    redundantLevelHmaoArrow = "соответствует&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;уровню";
                    redundantLevelHmaoDescription = String.Empty;
                }

                string redundantLevelHmaoGrow = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}чем в ХМАО-Югре{1}", redundantLevelHmaoArrow, redundantLevelHmaoDescription);

                string img = String.Empty;
                if (dtRedudantLevel.Rows[0][2].ToString() == "1")
                {
                    img = "&nbsp;<img src='../../../images/starYellow.png'>";
                }
                else if (dtRedudantLevel.Rows[0][2].ToString() == dtRedudantLevel.Rows[0][3].ToString())
                {
                    img = "&nbsp;<img src='../../../images/starGray.png'>";
                }

                lbRedudantLevel.Text = String.Format("На{4}уровень регистрируемой в органах службы занятости безработицы составил&nbsp;<span class='DigitsValue'><b>{0:P2}</span></b>&nbsp;<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ранг в ХМАО-Югре&nbsp;<span class='DigitsValue'><b>{1:N0}</span></b>{7}<br/>{2} {3} {5}{6}", dtRedudantLevel.Rows[0][1], dtRedudantLevel.Rows[0][2], grown, middleLevel, dateTimeStr, redundantLevelHmaoGrow, redundantLevelRFGrow, img);
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0001_chart_investment"));
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]);

            string mo = dtChart.Rows[1][2].ToString();

            dtChart.Rows[0][0] = String.Format("{0} {1:N0} чел. ({2:P2})", dtChart.Rows[0][0], dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1]) / total);
            dtChart.Rows[1][0] = String.Format("{0} {1:N0} чел.  ({2:P2})", dtChart.Rows[1][2], dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);

            UltraChart3.DataSource = dtChart;


            lbInvestDescription.Text = String.Format("Вклад МО&nbsp;<span class='DigitsValue'><b>{0}</span></b>&nbsp;по численности безработных в общее число безработных в ХМАО-Югра составляет&nbsp;<span class='DigitsValue'><b>{1:P2}</span></b><br/>(<span class='DigitsValue'><b>{2:N0}</span></b>&nbsp;человек&nbsp;из&nbsp;<span class='DigitsValue'><b>{3:N0}</span></b>&nbsp;человек)", mo, Convert.ToDouble(dtChart.Rows[1][1]) / total, dtChart.Rows[1][1], total);
        }

        private void SetUpPieChart(UltraChart chart)
        {
            chart.ChartType = ChartType.DoughnutChart;

            chart.StackChart.StackStyle = StackStyle.Complete;

            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 5;
            chart.Axis.X.Extent = 5;

            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.Y.Labels.Visible = true;
            chart.DoughnutChart.Labels.FontColor = Color.FromArgb(192, 192, 192);
            chart.DoughnutChart.Labels.LeaderLineColor = Color.FromArgb(192, 192, 192);
            chart.DoughnutChart.Labels.LeaderDrawStyle = LineDrawStyle.Dot;
            chart.DoughnutChart.Labels.LeaderEndStyle = LineCapStyle.Square;
            chart.DoughnutChart.Labels.Font = new Font("Verdana", 10);
            chart.DoughnutChart.Labels.Visible = false;
            chart.DoughnutChart.Labels.LeaderLineColor = Color.Transparent;

            chart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
            chart.ChartDrawItem += chart_ChartDrawItem;

            chart.BackColor = Color.Transparent;

            chart.DoughnutChart.RadiusFactor = 90;
            chart.DoughnutChart.InnerRadius = 20;
            chart.Tooltips.FormatString = "";
            chart.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            chart.DoughnutChart.OthersCategoryPercent = 0;
            chart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> человек (<PERCENT_VALUE:N2>%)";

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Right;
            chart.Legend.Font = new Font("Verdana", 9);
            chart.DoughnutChart.StartAngle = 340;

            chart.Data.SwapRowsAndColumns = false;
            chart.ColorModel.Skin.ApplyRowWise = true;
        }

        private void AddColorModelChart2(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Gold;
                            stopColor = Color.Orange;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Red;
                            stopColor = Color.Red;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                pe.Stroke = Color.FromArgb(40, 40, 40);
                chart.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private void CommentTextDataBind()
        {
            // Сначала выбираем даты и текст по базе хмао.
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1_date");
            DataTable dtDateHmao = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateHmao);

            DateTime currDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeHmao, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", lastDateTimeHmao, 5);

            query = DataProvider.GetQueryText("STAT_0001_0001_Text_Hmao");
            DataTable dtCommentTextHmao = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentTextHmao);

            periodPrevMonth.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeHmao.AddMonths(-1), 4);
            query = DataProvider.GetQueryText("STAT_0001_0004_unemployee");
            DataTable dtUnemployed = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtUnemployed);

            if (dtCommentTextHmao.Rows.Count > 0)
            {
                string dateTimeStr = string.Format("&nbsp;{0:dd.MM.yyyy}&nbsp;", currDateTimeHmao);

                double totalCount = Convert.ToDouble(dtCommentTextHmao.Rows[1][1]);
                double totalRate = 1 + Convert.ToDouble(dtCommentTextHmao.Rows[1][3]);
                double totalGrow = Convert.ToDouble(dtCommentTextHmao.Rows[1][2]);

                string str1 = string.Format(@"Численность безработных&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;человек<br/>", totalCount);

                string totalRateArrow = "не изменилось<br/>";
                if (totalRate > 1)
                {
                    totalRateArrow =
                        "увеличилось&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;";
                }
                else if (totalRate < 1)
                {
                    totalRateArrow = "снизилось&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;";
                    
                }

                if (totalRate != 1)
                {
                    totalRateArrow = String.Format("{0} на&nbsp;<span class='DigitsValue'><b>{1:N0}</b></span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'><b>{2:P2}</b></span>)<br/>", 
                                        totalRateArrow, Math.Abs(totalGrow), totalRate);
                }

                string str2 = string.Format(@"За период с&nbsp;<span class='DigitsValue'><b>{0:dd.MM}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{1:dd.MM}</b></span>&nbsp;число безработных {2}",
                                     lastDateTimeHmao, currDateTimeHmao, totalRateArrow);

                double unemploued = Convert.ToDouble(dtUnemployed.Rows[0][0]);
                double totalRateUnemploued = 1 + Convert.ToDouble(dtUnemployed.Rows[0][3]);
                double totalGrowUnemploued = Convert.ToDouble(dtUnemployed.Rows[0][2]);

                string str4 = string.Format(@"Численность населения без статуса безработных на&nbsp;<span class='DigitsValue'>{1:01.MM.yyyy}</span>&nbsp;составляет&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;человек<br/>", unemploued, currDateTimeHmao);
                
                string totalRateArrowUnemploued = "не изменилась";
                if (totalRateUnemploued > 1)
                {
                    totalRateArrowUnemploued =
                        "увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;";
                }
                else if (totalRateUnemploued < 1)
                {
                    totalRateArrowUnemploued = "снизилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;";
                }

                if (totalRateUnemploued != 1)
                {
                    totalRateArrowUnemploued = String.Format("{0} на&nbsp;<span class='DigitsValue'><b>{1:N0}</b></span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'><b>{2:P2}</b></span>)<br/>",
                                        totalRateArrowUnemploued, Math.Abs(totalGrowUnemploued), totalRateUnemploued);
                }

                string str5 = string.Format(@"За период с&nbsp;<span class='DigitsValue'>{0:01.MM}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:01.MM}</span>&nbsp;численность населения без статуса безработных {2}",
                                     currDateTimeHmao.AddMonths(-1), currDateTimeHmao, totalRateArrowUnemploued);

                lbRedudantCount.Text = string.Format("{0}{1}{2}{3}", str1, str2, str4, str5);

                string img = String.Empty;
                if (dtCommentTextHmao.Rows[6][4].ToString() == "1")
                {
                    img = "&nbsp;<img src='../../../images/starYellow.png'>";
                }
                else if (dtCommentTextHmao.Rows[6][4].ToString() == dtCommentTextHmao.Rows[6][5].ToString())
                {
                    img = "&nbsp;<img src='../../../images/starGray.png'>";
                }

                string str7 = string.Format(@"Потребность в работниках, заявленная работодателями в органы службы занятости населения&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;вакансий<br/>", dtCommentTextHmao.Rows[5][1]);
                string str8 = string.Format(@"Коэффициент напряженности на рынке труда&nbsp;<span class='DigitsValueXLarge'><b>{0:N3}</b></span><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ранг в ХМАО-Югре&nbsp;<span class='DigitsValueLarge'><b>{1:N0}</b></span>{2}", dtCommentTextHmao.Rows[6][1], dtCommentTextHmao.Rows[6][4], img);
                string str9 = string.Format(@"<br/>Численность безработных на 1 вакансию&nbsp;<span class='DigitsValueXLarge'><b>{0:N1}</b></span>", dtCommentTextHmao.Rows[7][1]);

                lbTensionDescription.Text = string.Format("{0}{1}{2}", str7, str8, str9);

                query = DataProvider.GetQueryText("STAT_0001_0012_debts_mo_date");
                DataTable dtDebtsDateHmao = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDateHmao);

                string str10;

                if (dtDebtsDateHmao.Rows.Count > 1)
                {
                    if (dtDebtsDateHmao.Rows[0][1] != DBNull.Value && dtDebtsDateHmao.Rows[0][1].ToString() != string.Empty)
                    {
                        debtsPeriodLastWeekDate.Value = dtDebtsDateHmao.Rows[0][1].ToString();
                        debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateHmao.Rows[0][1].ToString(), 3);
                    }
                    if (dtDebtsDateHmao.Rows[1][1] != DBNull.Value && dtDebtsDateHmao.Rows[1][1].ToString() != string.Empty)
                    {
                        debtsPeriodCurrentDate.Value = dtDebtsDateHmao.Rows[1][1].ToString();
                        debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateHmao.Rows[1][1].ToString(), 3);
                    }
                    debtsPeriodCurrentDate.Value = dtDebtsDateHmao.Rows[1][1].ToString();
                    debtsPeriodLastWeekDate.Value = dtDebtsDateHmao.Rows[0][1].ToString();

                    query = DataProvider.GetQueryText("STAT_0001_0012_debts_Hmao");
                    DataTable dtDebtsHmao = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsHmao);

                    double totalDebts = GetDoubleDTValue(dtDebtsHmao, "Cумма задолженности");
                    double totalLastWeekDebts = GetDoubleDTValue(dtDebtsHmao, "Cумма задолженности прошлая неделя");
                    string dateTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
                    string dateLastTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
                    double slavesCount = GetDoubleDTValue(dtDebtsHmao, "Количество граждан, имеющих задолженность");
                    double debtsPercent = GetDoubleDTValue(dtDebtsHmao, "Прирост задолженности");
                    string debtsPercentArrow = debtsPercent == 0
                                                   ? "не изменилась"
                                                   : debtsPercent > 0
                                                   ? string.Format("увеличилась <img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'> на <b>{0:N3}</b>&nbsp;млн.руб", Math.Abs(debtsPercent))
                                                   : string.Format("уменьшилась <img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'> на <b>{0:N3}</b>&nbsp;млн.руб", Math.Abs(debtsPercent));


                    if (totalLastWeekDebts == 0 && totalDebts == 0)
                    {
                        str10 = string.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате 
заработной платы.<br/>", dateTimeDebtsStr);
                    }
                    else if (totalDebts == 0)
                    {
                        str10 = string.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате заработной платы. 
Задолженность в сумме&nbsp;<span class='DigitsValue'><b>{2:N3}</b></span>&nbsp;млн.руб. была погашена за период с&nbsp;<span class='DigitsValue'><b>{3}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{0}</b></span>.<br/>",
    dateTimeDebtsStr, "ХМАО-Югре", totalLastWeekDebts, dateLastTimeDebtsStr);
                    }
                    else
                    {
                        str10 = string.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;задолженность по выплате заработной платы составляет 
&nbsp;<span class='DigitsValue'><b>{1:N3}</b></span>&nbsp;млн.рублей (<span class='DigitsValue'><b>{2:N0}</b></span>&nbsp;чел.).<br/>За период с&nbsp;<span class='DigitsValue'><b>{5}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;задолженность {3}",
    dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, "ХМАО-Югре", dateLastTimeDebtsStr);
                    }
                }
                else
                {
                    str10 = string.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате 
заработной платы<br/>", dateTimeStr);
                }
                lbDebtDescription.Text = str10;
            }
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        void chart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart3.Legend.Location == LegendLocation.Top) || (UltraChart3.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart3.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = (UltraChart3.Legend.SpanPercentage * (int)UltraChart3.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart3.Legend.Margins.Left + UltraChart3.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        private void SetUpPeopleChart(UltraChart chart)
        {
            chart.ChartType = ChartType.StackBarChart;

            chart.StackChart.StackStyle = StackStyle.Complete;

            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 5;
            chart.Axis.X.Extent = 5;

            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.Y.Labels.Visible = false;
            chart.Axis.Y.LineColor = Color.Black;
            chart.Axis.X.LineColor = Color.Black;

            chart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;

            chart.BackColor = Color.Transparent;

            chart.DoughnutChart.RadiusFactor = 70;
            chart.DoughnutChart.InnerRadius = 20;
            chart.Tooltips.FormatString = "";
            chart.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            chart.DoughnutChart.OthersCategoryPercent = 0;
            chart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> человек (<PERCENT_VALUE:N2>%)";

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.Font = new Font("Verdana", 9);
            chart.DoughnutChart.StartAngle = 340;

            chart.Data.SwapRowsAndColumns = true;
        }

        private void AddColorModelChart1(UltraChart chart)
        {
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
                pe.Stroke = Color.FromArgb(40, 40, 40);
                chart.ColorModel.Skin.PEs.Add(pe);
            }
            chart.ColorModel.Skin.ApplyRowWise = false;
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Green;
                    }
                case 2:
                    {
                        return Color.Gold;
                    }
                case 3:
                    {
                        return Color.Red;
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
                        return Color.ForestGreen;
                    }
                case 2:
                    {
                        return Color.Yellow;
                    }
                case 3:
                    {
                        return Color.Red;
                    }
            }
            return Color.White;
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(
                query, "Наименование показателя", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]) +
                            Convert.ToDouble(dtChart.Rows[2][1]);

            dtChart.Rows[0][0] = String.Format("Занятое население {0:N0} чел. ({1:P2})", dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1]) / total);
            dtChart.Rows[1][0] = String.Format("Население без статуса\nбезработных {0:N0} чел ({1:P2})", dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);
            dtChart.Rows[2][0] = String.Format("Безработные {0:N0} чел ({1:P2})", dtChart.Rows[2][1], Convert.ToDouble(dtChart.Rows[2][1]) / total);

            dtChart.Rows[1][1] = Convert.ToDouble(dtChart.Rows[1][1]) * 10;
            dtChart.Rows[2][1] = Convert.ToDouble(dtChart.Rows[2][1]) * 10;

            UltraChart1.DataSource = dtChart;
        }
    }
}
