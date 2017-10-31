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
    public partial class STAT_0001_0011 : CustomReportPage
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

        private string currentRegion;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //CustomParams.MakeMoParams("306", "id");

            UserParams.Mo.Value = !UserParams.Mo.Value.Contains("г.") ?
                        String.Format(".[{0}]", UserParams.Mo.Value) :
                        String.Format(".[{0}]", UserParams.Mo.Value.Replace("г.", "Город").Replace(" - ", "-"));

            currentRegion = UserParams.Mo.Value.Replace("Город ", "").Replace("город ", String.Empty).Replace("муниципальный район", "МР").Replace(".[", String.Empty).
                Replace("[", String.Empty).Replace("]", String.Empty).Replace(".DataMember", String.Empty);
            HeraldImageContainer.InnerHtml = String.Format("<img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/HeraldsMo/{0}.png\"></a>", HttpContext.Current.Session["CurrentMoID"]);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
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
            UltraChart3.Legend.Margins.Bottom = 30;
            UltraChart3.Height = 150;
            SetUpPieChart(UltraChart3);
            AddColorModelChart2(UltraChart3);
            UltraChart3.DataBinding += UltraChart3_DataBinding;
            UltraChart3.DataBind();

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
            string query = DataProvider.GetQueryText("STAT_0001_0011_chart1_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateSakhalin);

            DateTime currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeSakhalin, 5);
            string dateTimeStr = String.Format("&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>", currDateTimeSakhalin);

            DataTable dtPopulation = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0011_population");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulation);

            DataTable dtActivePopulation = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0011_active_population");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtActivePopulation);

            double value = 0;
            double populationValue;
            double activePopulationValue;
            if (dtActivePopulation.Rows[0][0] != DBNull.Value && dtPopulation.Rows[0][1] != DBNull.Value &&
                Double.TryParse(dtActivePopulation.Rows[0][0].ToString(), out activePopulationValue) &&
                Double.TryParse(dtPopulation.Rows[0][1].ToString(), out populationValue))
            {
                value = activePopulationValue / populationValue;
            }

            lbStatistics.Text = String.Format(
                "Численность экономически активного населения&nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;человек (на{3})<br/>Численность населения&nbsp;<span class='DigitsValue'>{1:N0}</span>&nbsp;человек (на начало года)<br/>Доля экономически активного населения в общей численности населения&nbsp;<span class='DigitsValue'>{2:P2}</span>&nbsp;",
                dtActivePopulation.Rows[0][0], dtPopulation.Rows[0][1], value, dateTimeStr);
        }

        private void BindRedudantLevelDescritionText()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0011_chart1_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateSakhalin);

            DateTime currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeSakhalin, 5);
            string dateTimeStr = String.Format("&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", currDateTimeSakhalin);

            DataTable dtRedudantLevel = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0011_redudantLevel");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtRedudantLevel);

            double thisRedudant;
            string grown = String.Empty;
            string middleLevel = String.Empty;

            IPadElementHeader1.Text = String.Format("Зарегистрированная безработица на {0:dd.MM.yyyy}", currDateTimeSakhalin);

            // Возьмем данные по федеральной базе
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов]", currDateTimeSakhalin, 4);
            DataTable dtDateCur = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0011_redundant_federal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateCur);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeSakhalin, 4);
            query = DataProvider.GetQueryText("STAT_0001_0011_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            DateTime currDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[1][4].ToString(), 3);
            DateTime lastDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[0][4].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов]", currDateTime, 4);

            query = DataProvider.GetQueryText("STAT_0001_0011_redundant_federal");
            DataTable dtRedundantFederal = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Уровень безработицы", dtRedundantFederal);

            if (dtDebtsDate.Rows.Count > 1)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != String.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != String.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", lastDateTime, 5);
            query = DataProvider.GetQueryText("STAT_0001_0011_commentText");
            DataTable dtCommentText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);

            double redundantLevelSakhalin;

            if (Double.TryParse(dtRedudantLevel.Rows[0][1].ToString(), out thisRedudant) &&
               Double.TryParse(dtRedudantLevel.Rows[0][4].ToString(), out redundantLevelSakhalin))
            {
                #region ДФО и РФ

                double redundantLevelRFValue = GetDoubleDTValue(dtRedundantFederal, "Российская  Федерация");

                string redundantLevelRFArrow;
                string redundantLevelRFDescription = String.Format("&nbsp;<span class='DigitsValue'>{0:P2}</span>", redundantLevelRFValue);
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

                double redundantLevelDfoValue = GetDoubleDTValue(dtRedundantFederal, "Дальневосточный федеральный округ");
                string redundantLevelDfoArrow;
                string redundantLevelDfoDescription = String.Format("&nbsp;<span class='DigitsValue'>{0:P2}</span>", redundantLevelDfoValue);
                if (thisRedudant > redundantLevelDfoValue)
                {
                    redundantLevelDfoArrow = "выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;чем в";
                }
                else if (thisRedudant < redundantLevelDfoValue)
                {
                    redundantLevelDfoArrow = "ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;чем в";
                }
                else
                {
                    redundantLevelDfoArrow = "соответствует&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;уровню";
                    redundantLevelDfoDescription = String.Empty;
                }
                string redundantLevelDfoGrow = String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} ДФО{1}<br/>", redundantLevelDfoArrow, redundantLevelDfoDescription);

                redundantLevelRFGrow = String.Format("{0}{1}", redundantLevelDfoGrow, redundantLevelRFGrow);

                #endregion

                string redundantLevelSakhalinArrow;
                string redundantLevelSakhalinDescription = String.Format("&nbsp;<span class='DigitsValue'>{0:P2}</span>", redundantLevelSakhalin);
                if (thisRedudant > redundantLevelSakhalin)
                {
                    redundantLevelSakhalinArrow = "выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (thisRedudant < redundantLevelRFValue)
                {
                    redundantLevelSakhalinArrow = "ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
                }
                else
                {
                    redundantLevelSakhalinArrow = "соответствует&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;уровню";
                    redundantLevelSakhalinDescription = String.Empty;
                }

                string redundantLevelSakhalinGrow = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}чем в Сахалинской области{1}", redundantLevelSakhalinArrow, redundantLevelSakhalinDescription);

                string img = String.Empty;
                if (dtRedudantLevel.Rows[0][2].ToString() == "1")
                {
                    img = "&nbsp;<img src='../../../images/starYellow.png'>";
                }
                else if (dtRedudantLevel.Rows[0][2].ToString() == dtRedudantLevel.Rows[0][3].ToString())
                {
                    img = "&nbsp;<img src='../../../images/starGray.png'>";
                }

                lbRedudantLevel.Text = String.Format("На{4}уровень регистрируемой в органах службы занятости безработицы составил&nbsp;<span class='DigitsValue'>{0:P2}</span>&nbsp;<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ранг в Сахалинской области&nbsp;<span class='DigitsValue'>{1:N0}</span>{7}<br/>{2} {3} {5}{6}", dtRedudantLevel.Rows[0][1], dtRedudantLevel.Rows[0][2], grown, middleLevel, dateTimeStr, redundantLevelSakhalinGrow, redundantLevelRFGrow, img);
            }
        }

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0011_chart1_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateSakhalin);

            DateTime currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeSakhalin, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", lastDateTimeSakhalin, 5);

            query = DataProvider.GetQueryText("STAT_0001_0011_Text_Sakhalin");
            DataTable dtCommentTextSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentTextSakhalin);

            if (dtCommentTextSakhalin.Rows.Count > 0)
            {
                string dateTimeStr = String.Format("&nbsp;{0:dd.MM.yyyy}&nbsp;", currDateTimeSakhalin);

                double totalCount = Convert.ToDouble(dtCommentTextSakhalin.Rows[1][1]);
                double totalRate = 1 + Convert.ToDouble(dtCommentTextSakhalin.Rows[1][3]);
                double totalGrow = Convert.ToDouble(dtCommentTextSakhalin.Rows[1][2]);

                string str1 = String.Format(@"Численность безработных&nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;человек<br/>", totalCount);

                string totalRateArrow = "не изменилось<br/>";
                if (totalRate > 1)
                {
                    totalRateArrow =
                        "увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;";
                }
                else if (totalRate < 1)
                {
                    totalRateArrow = "снизилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;";

                }

                if (totalRate != 1)
                {
                    totalRateArrow = String.Format("{0} на&nbsp;<span class='DigitsValue'>{1:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{2:P2}</span>)<br/>",
                                        totalRateArrow, Math.Abs(totalGrow), totalRate);
                }

                string str2 = String.Format(@"За период с&nbsp;<span class='DigitsValue'>{0:dd.MM}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM}</span>&nbsp;число безработных {2}",
                                     lastDateTimeSakhalin, currDateTimeSakhalin, totalRateArrow);

                double unemploued = Convert.ToDouble(dtCommentTextSakhalin.Rows[2][1]);
                double totalRateUnemploued = 1 + Convert.ToDouble(dtCommentTextSakhalin.Rows[2][3]);
                double totalGrowUnemploued = Convert.ToDouble(dtCommentTextSakhalin.Rows[2][2]);

                string str4 = String.Format(@"Численность населения без статуса безработных &nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;человек<br/>", unemploued);

                string totalRateArrowUnemploued = "не изменилось";
                if (totalRateUnemploued > 1)
                {
                    totalRateArrowUnemploued =
                        "увеличилось&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;";
                }
                else if (totalRateUnemploued < 1)
                {
                    totalRateArrowUnemploued = "снизилось&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;";
                }

                if (totalRateUnemploued != 1)
                {
                    totalRateArrowUnemploued = String.Format("{0} на&nbsp;<span class='DigitsValue'>{1:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{2:P2}</span>)<br/>",
                                        totalRateArrowUnemploued, Math.Abs(totalGrowUnemploued), totalRateUnemploued);
                }

                string str5 = String.Format(@"За период с&nbsp;<span class='DigitsValue'>{0:dd.MM}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM}</span>&nbsp;численность населения без статуса безработных {2}",
                                     lastDateTimeSakhalin, currDateTimeSakhalin, totalRateArrowUnemploued);

                lbRedudantCount.Text = String.Format("{0}{1}{2}{3}", str1, str2, str4, str5);

                string img = String.Empty;
                if (dtCommentTextSakhalin.Rows[6][4].ToString() == "1")
                {
                    img = "&nbsp;<img src='../../../images/starYellow.png'>";
                }
                else if (dtCommentTextSakhalin.Rows[6][4].ToString() == dtCommentTextSakhalin.Rows[6][5].ToString())
                {
                    img = "&nbsp;<img src='../../../images/starGray.png'>";
                }

                string str8 = String.Empty, str9 = String.Empty;

                string str7 = String.Format(@"Потребность в работниках, заявленная работодателями в органы службы занятости населения &nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;вакансий<br/>", dtCommentTextSakhalin.Rows[5][1]);
                if (MathHelper.AboveZero(dtCommentTextSakhalin.Rows[5][1]))
                {
                    str8 = String.Format(@"Коэффициент напряженности на рынке труда&nbsp;<span class='DigitsValueXLarge'>{0:N3}</span><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ранг в Сахалинской области&nbsp;<span class='DigitsValueLarge'>{1:N0}</span>{2}", dtCommentTextSakhalin.Rows[6][1], dtCommentTextSakhalin.Rows[6][4], img);
                    str9 = String.Format(@"<br/>Численность безработных на 1 вакансию&nbsp;<span class='DigitsValueXLarge'>{0:N1}</span>", dtCommentTextSakhalin.Rows[7][1]);
                }

                lbTensionDescription.Text = String.Format("{0}{1}{2}", str7, str8, str9);
                
                query = DataProvider.GetQueryText("STAT_0001_0011_debts_mo_date");
                DataTable dtDebtsDateSakhalin = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDateSakhalin);

                string str10;

                if (dtDebtsDateSakhalin.Rows.Count > 1)
                {
                    if (dtDebtsDateSakhalin.Rows[0][1] != DBNull.Value && dtDebtsDateSakhalin.Rows[0][1].ToString() != String.Empty)
                    {
                        debtsPeriodLastWeekDate.Value = dtDebtsDateSakhalin.Rows[0][1].ToString();
                        debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateSakhalin.Rows[0][1].ToString(), 3);
                    }
                    if (dtDebtsDateSakhalin.Rows[1][1] != DBNull.Value && dtDebtsDateSakhalin.Rows[1][1].ToString() != String.Empty)
                    {
                        debtsPeriodCurrentDate.Value = dtDebtsDateSakhalin.Rows[1][1].ToString();
                        debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateSakhalin.Rows[1][1].ToString(), 3);
                    }
                    debtsPeriodCurrentDate.Value = dtDebtsDateSakhalin.Rows[1][1].ToString();
                    debtsPeriodLastWeekDate.Value = dtDebtsDateSakhalin.Rows[0][1].ToString();

                    query = DataProvider.GetQueryText("STAT_0001_0011_debts_Sakhalin");
                    DataTable dtDebtsSakhalin = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsSakhalin);

                    double totalDebts = GetDoubleDTValue(dtDebtsSakhalin, "Cумма задолженности");
                    double totalLastWeekDebts = GetDoubleDTValue(dtDebtsSakhalin, "Cумма задолженности прошлая неделя");
                    string dateTimeDebtsStr = String.Format("{0:dd.MM.yyyy}", debtsCurrDateTime.AddMonths(1));
                    string dateLastTimeDebtsStr = String.Format("{0:dd.MM.yyyy}", debtsLastDateTime.AddMonths(1));
                    double slavesCount = GetDoubleDTValue(dtDebtsSakhalin, "Количество граждан, имеющих задолженность");
                    double debtsPercent = GetDoubleDTValue(dtDebtsSakhalin, "Прирост задолженности");
                    string debtsPercentArrow = debtsPercent == 0
                                                   ? "не изменилась"
                                                   : debtsPercent > 0
                                                   ? String.Format("увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<b>{0:N3}</b>&nbsp;тыс. руб.", Math.Abs(debtsPercent))
                                                   : String.Format("уменьшилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<b>{0:N3}</b>&nbsp;тыс. руб.", Math.Abs(debtsPercent));


                    if (totalLastWeekDebts == 0 && totalDebts == 0)
                    {
                        str10 = String.Format(@"На&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;отсутствует задолженность по выплате 
заработной платы.<br/>", dateTimeDebtsStr);
                    }
                    else if (totalDebts == 0)
                    {
                        str10 = String.Format(@"На&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;отсутствует задолженность по выплате заработной платы. 
Задолженность в сумме&nbsp;<span class='DigitsValue'>{2:N3}</span>&nbsp;тыс. руб. была погашена за период с&nbsp;<span class='DigitsValue'>{3}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{0}</span>.<br/>",
    dateTimeDebtsStr, "Сахалинской области", totalLastWeekDebts, dateLastTimeDebtsStr);
                    }
                    else
                    {
                        str10 = String.Format(@"На&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;задолженность по выплате заработной платы составляет 
&nbsp;<span class='DigitsValue'>{1:N3}</span>&nbsp;тыс. руб. (<span class='DigitsValue'>{2:N0}</span>&nbsp;чел.).<br/>За период с&nbsp;<span class='DigitsValue'>{5}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;задолженность {3}",
    dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, "Сахалинской области", dateLastTimeDebtsStr);
                    }
                }
                else
                {
                    str10 = String.Format(@"На&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;отсутствует задолженность по выплате 
заработной платы<br/>", currDateTimeSakhalin);
                }
                lbDebtDescription.Text = str10;
            }
        }

        #region Диаграммы

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0011_chart_investment"));
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]);

            string mo = dtChart.Rows[1][2].ToString();

            if (dtChart.Rows[1][2] != DBNull.Value)
                dtChart.Rows[1][2] = dtChart.Rows[1][2].ToString().Replace("Александровск-Сахалинский", "Александровск-\nСахалинский");

            dtChart.Rows[0][0] = String.Format("{0} {1:N0} чел. ({2:P2})", dtChart.Rows[0][0], dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1]) / total);
            dtChart.Rows[1][0] = String.Format("{0} {1:N0} чел.  ({2:P2})", dtChart.Rows[1][2], dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);

            UltraChart3.DataSource = dtChart;


            lbInvestDescription.Text = String.Format("Вклад МО&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;по численности безработных в общее число безработных в Сахалинской области составляет&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>(<span class='DigitsValue'>{2:N0}</span>&nbsp;человек&nbsp;из&nbsp;<span class='DigitsValue'>{3:N0}</span>&nbsp;человек)", mo, Convert.ToDouble(dtChart.Rows[1][1]) / total, dtChart.Rows[1][1], total);
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

        void chart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(String.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
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
            //chart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> человек (<PERCENT_VALUE:N2>%)";

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
            string query = DataProvider.GetQueryText("STAT_0001_0011_chart1");
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

        #endregion

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }
    }
}
