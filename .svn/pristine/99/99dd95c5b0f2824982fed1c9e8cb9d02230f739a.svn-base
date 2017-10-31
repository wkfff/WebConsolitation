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
    public partial class STAT_0001_0006 : CustomReportPage
    {

        private DateTime debtsLastDateTime;
        private DateTime debtsCurrDateTime;

        private CustomParam populationYear;
        private CustomParam populationDate;
        private CustomParam outOfWorkDate;
        private CustomParam outOfWorkFoDate;
        private CustomParam unemployedLastDate;
        private CustomParam unemployedPrevDate;
        private CustomParam tensionDate;
        private CustomParam debtsLastDate;
        private CustomParam debtsPrevDate;
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Mo.Value))
                CustomParams.MakeMoParams("48", "id");

            UserParams.Mo.Value = !UserParams.Mo.Value.Contains("г.") ?
                        UserParams.Mo.Value.Replace(" муниципальный", String.Empty).Replace(" район", " муниципальный район") :
                        UserParams.Mo.Value.Replace("г. ", "Город ");

            HeraldImageContainer.InnerHtml = String.Format("<img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/HeraldsMo/{0}.png\"></a>", HttpContext.Current.Session["CurrentMoID"]);

            #region Инициализация параметров запроса

            populationDate = UserParams.CustomParam("population_date");
            populationYear = UserParams.CustomParam("population_year");
            outOfWorkDate = UserParams.CustomParam("period_not_work_fo_date");
            outOfWorkFoDate = UserParams.CustomParam("period_not_work_date");
            unemployedLastDate = UserParams.CustomParam("period_unemployed_last_date");
            unemployedPrevDate = UserParams.CustomParam("period_unemployed_prev_date");
            tensionDate = UserParams.CustomParam("period_tension_date");
            debtsLastDate = UserParams.CustomParam("period_debts_last_date");
            debtsPrevDate = UserParams.CustomParam("period_debts_prev_date");

            #endregion

            #region Получение дат

            DataTable dtDates = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Даты", dtDates);

            DataRow rowDates = dtDates.Rows[0];
            populationDate.Value = rowDates["Дата для численности трудоспособного населения"].ToString();
            populationYear.Value = rowDates["Год для численности населения"].ToString();
            outOfWorkDate.Value = rowDates["Дата для уровня безработицы"].ToString();
            outOfWorkFoDate.Value = rowDates["Дата для уровня безработицы ФО"].ToString();
            unemployedLastDate.Value = rowDates["Последняя дата для численности незанятых граждан, состоящих на учете"].ToString();
            unemployedPrevDate.Value = rowDates["Предыдущая дата для численности незанятых граждан, состоящих на учете"].ToString();
            tensionDate.Value = rowDates["Последняя дата для коэффициента напряженности"].ToString();
            debtsLastDate.Value = rowDates["Последняя дата для задолженности по выплате заработной платы"].ToString();
            debtsPrevDate.Value = rowDates["Предыдущая дата для задолженности по выплате заработной платы"].ToString();

            #endregion

            BindStatisticsText();
            BindOutOfWorkLevelText();
            BindUnEmployedText();
            BindTensionText();
            BindDebtsText();

            UltraChart3.Width = 510;
            UltraChart3.Legend.SpanPercentage = 70;
            UltraChart3.Legend.Margins.Bottom = 70;
            UltraChart3.Height = 150;
            SetUpPieChart(UltraChart3);
            AddColorModelChart2(UltraChart3);
            UltraChart3.DataBinding += UltraChart3_DataBinding;
            UltraChart3.DataBind();

            UltraChart1.Width = 340;
            UltraChart1.Height = 225;
            UltraChart1.DataBinding += UltraChart1_DataBinding;
            SetUpPeopleChart(UltraChart1);
            UltraChart1.Legend.SpanPercentage = 33;
            AddColorModelChart1(UltraChart1);
            UltraChart1.Style.Add("margin-left", "-15px");
            UltraChart1.DataBind();
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
        }

        private void BindStatisticsText()
        {
            DataTable dtData = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_text_near_herald");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtData);
            DataRow rowData = dtData.Rows[0];

            lbStatistics.Text = String.Format(
                "По данным на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;численность трудоспособного населения в трудоспособном возрасте&nbsp;<span class='DigitsValueXLarge'>{1:N0}</span>&nbsp;человек<br/>" +
                "На начало&nbsp;<span class='DigitsValueLarge'>{3}</span>&nbsp; года численность населения&nbsp;<span class='DigitsValueLarge'>{2:N0}</span>&nbsp;человек (<span class='DigitsValue'>{5:N0}</span>&nbsp;место)<br/>" +
                "Доля трудоспособного населения в трудоспособном возрасте в общей численности населения&nbsp;<span class='DigitsValueLarge'>{4:P2}</span>&nbsp;",
                CRHelper.DateByPeriodMemberUName(populationDate.Value, 3), rowData["Численность трудоспособного населения"], rowData["Численность населения"], populationYear.Value, rowData["Доля трудоспособного населения"], rowData["Ранг"]);
        }

        private void BindOutOfWorkLevelText()
        {
            DataTable dtOutOfWork = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_not_work_level_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtOutOfWork);
            double rfOutOfWorkLevel = MathHelper.IsDouble(dtOutOfWork.Rows[0]["Уровень зарегистрированной безработицы РФ"]) ?
                Convert.ToDouble(dtOutOfWork.Rows[0]["Уровень зарегистрированной безработицы РФ"]):
                -1;

            dtOutOfWork = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0006_not_work_level");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Уровень безработицы", dtOutOfWork);
            DataRow row = dtOutOfWork.Rows[0];
            if (!MathHelper.IsDouble(row[UserParams.Mo.Value]))
                return;
            double moOutOfWorkLevel = Convert.ToDouble(row[UserParams.Mo.Value]);
            int rank = Convert.ToInt32(row["Ранг МО"]);
            int bestRank = Convert.ToInt32(row["Худший ранг"]);
            double subjectOutOfWorkLevel = MathHelper.IsDouble(row["Новосибирская область"]) ?
                Convert.ToDouble(row["Новосибирская область"]):
                -1;
            double foOutOfWorkLevel = MathHelper.IsDouble(row["Сибирский федеральный округ"]) ?
                Convert.ToDouble(row["Сибирский федеральный округ"]):
                -1;

            IPadElementHeader1.Text = String.Format("Зарегистрированная безработица на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", CRHelper.DateByPeriodMemberUName(outOfWorkDate.Value, 3));
            string str0 = String.Format("На&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;уровень регистрируемой в органах службы занятости безработицы, " +
                "в % от численности трудоспособного населения в трудоспособном возрасте, составил&nbsp;<span class='DigitsValueLarge'>{1:N3}%</span>&nbsp;<br />",
                CRHelper.DateByPeriodMemberUName(outOfWorkDate.Value, 3), moOutOfWorkLevel);

            string img = String.Empty;
            if (rank == 1)
                img = "<img src='../../../images/starGray.png'>";
            else if (rank == bestRank)
                img = "<img src='../../../images/starYellow.png'>";
                
            string str1 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ранг в Новосибирской области&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;{1}<br />", rank, img);

            string str2 = String.Empty;
            if (subjectOutOfWorkLevel != -1)
            {
                if (moOutOfWorkLevel < subjectOutOfWorkLevel)
                {
                    str2 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp; чем в Новосибирской области&nbsp;<span class='DigitsValue'>{0:N3}%</span><br />", subjectOutOfWorkLevel);
                }
                else if (moOutOfWorkLevel > subjectOutOfWorkLevel)
                {
                    str2 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp; чем в Новосибирской области&nbsp;<span class='DigitsValue'>{0:N3}%</span><br />", subjectOutOfWorkLevel);
                }
                else
                {
                    str2 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;соответствует&nbsp;<img src='../../../images/ballYellowBB.png'>&nbsp; уровню Новосибирской области&nbsp;<span class='DigitsValue'>{0:N3}%</span><br />", subjectOutOfWorkLevel);
                }
            }

            string str3 = String.Empty;
            if (foOutOfWorkLevel != -1)
            {
                if (moOutOfWorkLevel < foOutOfWorkLevel)
                {
                    str3 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp; чем в СФО&nbsp;<span class='DigitsValue'>{0:N3}%</span><br />", foOutOfWorkLevel);
                }
                else if (moOutOfWorkLevel > foOutOfWorkLevel)
                {
                    str3 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp; чем в СФО&nbsp;<span class='DigitsValue'>{0:N3}%</span><br />", foOutOfWorkLevel);
                }
                else
                {
                    str3 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;соответствует&nbsp;<img src='../../../images/ballYellowBB.png'>&nbsp; уровню СФО&nbsp;<span class='DigitsValue'>{0:N3}%</span><br />", foOutOfWorkLevel);
                }
            }

            string str4 = String.Empty;
            if (rfOutOfWorkLevel != -1)
            {
                if (moOutOfWorkLevel < rfOutOfWorkLevel)
                {
                    str4 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp; чем в РФ&nbsp;<span class='DigitsValue'>{0:P3}</span><br />", rfOutOfWorkLevel);
                }
                else if (moOutOfWorkLevel > rfOutOfWorkLevel)
                {
                    str4 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp; чем в РФ&nbsp;<span class='DigitsValue'>{0:P3}</span><br />", rfOutOfWorkLevel);
                }
                else
                {
                    str4 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;соответствует&nbsp;<img src='../../../images/ballYellowBB.png'>&nbsp; уровню РФ&nbsp;<span class='DigitsValue'>{0:P3}</span><br />", rfOutOfWorkLevel);
                }
            }
            lbUnemployedLevel.Text = String.Format("{0}{1}{2}{3}{4}", str0, str1, str2, str3, str4);
        }

        protected void BindUnEmployedText()
        {
            DataTable dtUnEmployed = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_unemployed");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Значение", dtUnEmployed);
            DataRow row = dtUnEmployed.Rows[0];
            DateTime lastDate = CRHelper.DateByPeriodMemberUName(unemployedLastDate.Value, 3);
            DateTime prevDate = CRHelper.DateByPeriodMemberUName(unemployedPrevDate.Value, 3);
            double value = Convert.ToDouble(row["Численность безработных на последнюю дату"]);
            double delta = Convert.ToDouble(row["Изменение численности безработных"]);
            double growth = Convert.ToDouble(row["Темп роста численности безработных"]);
            string str0 = String.Format("Численность официально зарегистрированных безработных&nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;человек<br />", value);
            string img = String.Empty;
            if (delta < 0)
            {
                img = String.Format("снизилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{1:P2}</span>)<br />",
                    Math.Abs(delta), growth);
            }
            else
            {
                img = String.Format("увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{1:P2}</span>)<br />",
                    delta, growth);
            }
            string str1 = String.Format("За период с&nbsp;<span class='DigitsValue'>{0:dd.MM}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM}</span>&nbsp;" +
                "численность официально зарегистрированных безработных {2}", prevDate, lastDate, img);

            value = Convert.ToDouble(row["Численность граждан, не получивших статус безработных на последнюю дату"]);
            delta = Convert.ToDouble(row["Изменение численности граждан, не получивших статус безработных"]);
            growth = Convert.ToDouble(row["Темп роста численности граждан, не получивших статус безработных, состоящих на учете"]);
            string separator = value < 10 ? "<br />" : " ";
            string str2 = String.Format("Граждане, не получившие статус{1}безработных&nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;человек<br />", value, separator);
            if (delta < 0)
            {
                img = String.Format("снизилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{1:P2}</span>)<br />",
                    Math.Abs(delta), growth);
            }
            else
            {
                img = String.Format("увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{1:P2}</span>)<br />",
                    delta, growth);
            }
            string str3 = String.Format("За период с&nbsp;<span class='DigitsValue'>{0:dd.MM}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM}</span>&nbsp;" +
                "численность граждан, не получивших статус безработных {2}", prevDate, lastDate, img);

            lbUnemployedCount.Text = String.Format("{0}{1}", str0, str1);
            
        }

        private void BindTensionText()
        {
            DataTable dtTension = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_tension");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "МО", dtTension);
            DataRow row = dtTension.Rows[0];
            string str0 = String.Format("Потребность в работниках, заявленная работодателями в органы службы занятости населения&nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;вакансий<br />",
                row["Вакансии"]);
            string str1 = String.Format("Уровень напряженности на рынке труда&nbsp;<span class='DigitsValueXLarge'>{0:N3}</span>&nbsp;<br />", row["Уровень напряженности"]);
            string rankIndication = String.Empty;
            if (Convert.ToInt32(row["Ранг"]) == Convert.ToInt32(row["Наибольший ранг"]))
            {
                rankIndication = "&nbsp;<img src='../../../images/StarYellow.png'><br />";
            }
            else if (Convert.ToInt32(row["Ранг"]) == 1)
            {
                rankIndication = "&nbsp;<img src='../../../images/StarGray.png'><br />";
            }
            else
            {
                rankIndication = "<br />";
            }
            string str2 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ранг в Новосибирской области&nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;{1}", row["Ранг"], rankIndication);
            string str3 = String.Format("Численность безработных на 1 вакансию&nbsp;<span class='DigitsValueXLarge'>{0:N3}</span>", row["Безработных на 1 вакансию"]);
            lbTensionDescription.Text = String.Format("{0}{1}{2}", str0, str1, str2);
        }

        private void BindDebtsText()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0006_debts");
            DataTable dtDebtsNso = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsNso);

            debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(debtsLastDate.Value, 3);
            debtsLastDateTime = CRHelper.DateByPeriodMemberUName(debtsPrevDate.Value, 3);

            double totalDebts = GetDoubleDTValue(dtDebtsNso, "Cумма задолженности");
            double totalLastWeekDebts = GetDoubleDTValue(dtDebtsNso, "Cумма задолженности предыдущий месяц");
            string dateTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
            string dateLastTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
            double slavesCount = GetDoubleDTValue(dtDebtsNso, "Количество граждан, имеющих задолженность");
            double debtsPercent = GetDoubleDTValue(dtDebtsNso, "Прирост задолженности");
            string debtsPercentArrow = debtsPercent == 0
                                           ? "не изменилась"
                                           : debtsPercent > 0
                                           ? string.Format("увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<b>{0:N3}</b>&nbsp;тыс.руб", Math.Abs(debtsPercent))
                                           : string.Format("уменьшилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<b>{0:N3}</b>&nbsp;тыс.руб", Math.Abs(debtsPercent));

            string str10;
            if (totalLastWeekDebts == 0 && totalDebts == 0)
            {
                str10 = string.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате 
заработной платы<br/>", dateTimeDebtsStr, "НСО");
            }
            else if (totalDebts == 0)
            {
                str10 = string.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате заработной платы. 
Задолженность в сумме&nbsp;<span class='DigitsValue'><b>{2:N3}</b></span>&nbsp;тыс.руб. была погашена за период с&nbsp;<span class='DigitsValue'><b>{3}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{0}</b></span>.<br/>",
dateTimeDebtsStr, "НСО", totalLastWeekDebts, dateLastTimeDebtsStr);
            }
            else
            {
                str10 = string.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;задолженность по выплате заработной платы составляет 
&nbsp;<span class='DigitsValue'><b>{1:N3}</b></span>&nbsp;тыс.руб. (<span class='DigitsValue'><b>{2:N0}</b></span>&nbsp;чел.).<br/>За период с&nbsp;<span class='DigitsValue'><b>{5}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;задолженность {3}",
dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, "НСО", dateLastTimeDebtsStr);
            }

            lbDebtDescription.Text = String.Format("{0}", str10);
        }

        #region Диаграммы

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
            chart.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            chart.DoughnutChart.OthersCategoryPercent = 0;

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
                if (i == 2)
                    continue;
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
            string query = DataProvider.GetQueryText("STAT_0001_0006_chart1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]);

            dtChart.Rows[0][0] = String.Format("Занятое население {0:N0} чел.", dtChart.Rows[0][1]);
            dtChart.Rows[1][0] = String.Format("Незанятое население {0:N0} чел.", dtChart.Rows[1][1]);

            dtChart.Rows[1][1] = Convert.ToDouble(dtChart.Rows[1][1]) * 10;

            UltraChart1.DataSource = dtChart;
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0006_chart3"));
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            dtChart.Rows[0][1] = MathHelper.Minus(dtChart.Rows[0][1], dtChart.Rows[1][1]);
            dtChart.AcceptChanges();

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]);

            string mo = dtChart.Rows[1][2].ToString();

            dtChart.Rows[0][0] = String.Format("{0} {1:N0} чел. ({2:P3})", dtChart.Rows[0][0], dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1]) / total);
            dtChart.Rows[1][0] = String.Format("{0} {1:N0} чел.  ({2:P3})", dtChart.Rows[1][2], dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);

            UltraChart3.DataSource = dtChart;


            lbInvestDescription.Text = String.Format("Вклад МО&nbsp;<span class='DigitsValue'><b>{0}</span></b>&nbsp;по численности безработных в общее число безработных в Новосибирской области составляет&nbsp;<span class='DigitsValue'><b>{1:P3}</span></b><br/>(<span class='DigitsValue'><b>{2:N0}</span></b>&nbsp;человек&nbsp;из&nbsp;<span class='DigitsValue'><b>{3:N0}</span></b>&nbsp;человек)", mo, Convert.ToDouble(dtChart.Rows[1][1]) / total, dtChart.Rows[1][1], total);
        }

        #endregion

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

    }
}
