using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0010 : CustomReportPage
    {
        private DateTime currDateTime;
        private DateTime lastDateTime;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;

        private CustomParam populationYear;

        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;
        private DateTime redundantLevelRFDateTime;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;
        private DateTime debtsLastDateTime;
        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        private DateTime debtsCurrDateTime;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (populationYear == null)
            {
                populationYear = UserParams.CustomParam("population_year");
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

            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart3.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart3_ChartDrawItem);

            DataTable dtDateCur = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0010_chart1_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtDateCur);
            periodCurrentDate.Value = dtDateCur.Rows[1][1].ToString();

            UltraChart1.Width = 340;
            UltraChart1.Height = 310;
            UltraChart1.DataBinding += UltraChart1_DataBinding;
            SetUpPieChart(UltraChart1);
            UltraChart1.Legend.SpanPercentage = 52;
            AddColorModelChart1(UltraChart1);
            UltraChart1.DataBind();
            
            CommentTextDataBind();
            BindTagCloud();

            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0010_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodYear.Value = dtDate.Rows[0][4].ToString();

            UltraChart3.Width = 340;
            UltraChart3.Legend.SpanPercentage = 33;
            UltraChart3.Height = 225;
            SetUpPieChart(UltraChart3);
            AddColorModelChart2(UltraChart3);

            UltraChart3.DataBind();

            lbDescription.Text = GetDescritionText(CRHelper.PeriodDayFoDate(UserParams.PeriodYear.Value));
        }

        private string GetDescritionText(DateTime date)
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0010_text");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtText);

            double thisMot; //2
            double ufoMot;  //3
            double rfMot;   //4

            string description = String.Empty;

            if (Double.TryParse(dtText.Rows[2][1].ToString(), out thisMot) &&
               Double.TryParse(dtText.Rows[3][1].ToString(), out ufoMot))
            {
                double grownValue = ufoMot - thisMot;
                string grown;
                string middleLevel = String.Format(" чем в ДФО&nbsp;<b><span class='DigitsValue'>{0:P2}</span></b>", ufoMot); ;
                if (grownValue < 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше";
                }
                else if (grownValue > 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ниже";
                }
                else
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;соответствует";
                    middleLevel = "среднему уровню общей безработицы по ДФО";
                }

                description =
                    String.Format(
                        "{0}{1} {2}<br/> ", grown, GetImage(grown), middleLevel);
                description = String.Format(
                   "В&nbsp;<span class='DigitsValue'><b>{0} {1} года</b></span>&nbsp;уровень общей безработицы<br/>по методологии МОТ в Сахалинской области&nbsp;<span class='DigitsValueXLarge'><b>{2:P2}</b></span><br/>{3}", CRHelper.RusMonthPrepositional(date.Month), date.Year, thisMot, description);
            }

            string rfDescription;
            if (Double.TryParse(dtText.Rows[2][1].ToString(), out thisMot) &&
               Double.TryParse(dtText.Rows[4][1].ToString(), out rfMot))
            {
                double grownValue = rfMot - thisMot;
                string grown;
                string middleLevel = String.Format(" чем в РФ&nbsp;<b><span class='DigitsValue'>{0:P2}</span></b><br/>", rfMot); ;
                if (grownValue < 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше";
                }
                else if (grownValue > 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ниже";
                }
                else
                {
                    grown = "соответствует";
                    middleLevel = "среднему уровню общей безработицы по РФ";
                }

                rfDescription =
                    String.Format(
                        "{0}{1} {2}", grown, GetImage(grown), middleLevel);
                
            }
            else
            {
                rfDescription = "";
            }

            description = String.Format("{0}{1}<div style='height: 7px; clear: both'></div>", description, rfDescription);

            double thisMonth;
            double prevMonth;
            if (Double.TryParse(dtText.Rows[1][1].ToString(), out thisMonth) &&
                Double.TryParse(dtText.Rows[1][2].ToString(), out prevMonth))
            {
                double grownValue = prevMonth - thisMonth;
                string grown = grownValue < 0 ? "увеличилось" : "снизилось";
                // string compile = grownValue < 0 ? "составил" : "составило";
                double grownTemp = thisMonth / prevMonth;
                description += String.Format("За {1} {2} года число безработных<br/>{0}{5} на&nbsp;<span class='DigitsValue'><b>{3:N0}</b></span>&nbsp;чел. (темп роста&nbsp;<b><span class='DigitsValue'>{4:P2}</span></b>)<div style='height: 7px; clear: both'></div>", grown, CRHelper.RusMonth(date.Month), date.Year, Math.Abs(grownValue), grownTemp, GetImage(grown));
            }
            description += String.Format("Численность безработных&nbsp;<span class='DigitsValue'><b>{0:N0}</b></span>&nbsp;чел.", dtText.Rows[1][1]);

            return description;
        }

        private static string GetImage(string direction)
        {
            switch (direction.ToLower())
            {
                case "увеличилось":
                    return "&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'/>&nbsp;";
                case "снизилось":
                    return "&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'/>&nbsp;";
                case "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше":
                    return "&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                default:
                    return "&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
            }
        }

        void UltraChart3_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
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

        void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if ((primitive is Box))
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0}\n{1}", "Дальневосточный федеральный округ",
                                                            box.DataPoint.Label);
                    }
                }
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0010_chart3"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]);

            dtChart.Rows[0][0] = String.Format("{0} {1:N0} чел. ({2:P2})", dtChart.Rows[0][0], dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1]) / total);
            dtChart.Rows[1][0] = String.Format("{0} {1:N0} чел. ({2:P2})", dtChart.Rows[1][0], dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);

            dtChart.Rows[1][1] = Convert.ToDouble(dtChart.Rows[1][1]) * 7;

            UltraChart3.DataSource = dtChart;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0010_chart1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) + 
                            Convert.ToDouble(dtChart.Rows[1][1]) +
                            Convert.ToDouble(dtChart.Rows[2][1]);

            dtChart.Rows[0][0] = String.Format("Занятое население {0:N0} чел. ({1:P2})", dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1])/total);
            dtChart.Rows[1][0] = String.Format("Население без статуса\nбезработных {0:N0} чел ({1:P2})", dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);
            dtChart.Rows[2][0] = String.Format("Безработные {0:N0} чел ({1:P2})", dtChart.Rows[2][1], Convert.ToDouble(dtChart.Rows[2][1]) / total);

            dtChart.Rows[1][1] = Convert.ToDouble(dtChart.Rows[1][1]) * 10;
            dtChart.Rows[2][1] = Convert.ToDouble(dtChart.Rows[2][1]) * 10;

            UltraChart1.DataSource = dtChart;
        }

        private void BindTagCloud()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0010_chart1_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateSakhalin);

            DateTime currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeSakhalin, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", lastDateTimeSakhalin, 5);

            DataTable dtTagCloud = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0010_tagCloud_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtTagCloud);

            Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
            Collection<Color> colors = new Collection<Color>();

            foreach (DataRow row in dtTagCloud.Rows)
            {
                if (row["Уровень безработицы"] != DBNull.Value &&
                    row["Уровень безработицы"].ToString() != String.Empty)
                {
                    Tag tag = new Tag();
                    tag.weight = Convert.ToInt32(row["Численность экономически активного населения "]);
                    string rankImage = String.Empty;
                    if (row["Ранг "].ToString() == "1")
                    {
                        rankImage = "&nbsp;<img src='../../../images/StarGray.png'>";
                    }
                    else if (row["Ранг "].ToString() == row["Худший ранг "].ToString())
                    {
                        rankImage = "&nbsp;<img src='../../../images/StarYellow.png'>";
                    }

                    tag.key = String.Format("{0}{2}&nbsp;({1:P2})", row["Краткое наименование "], row["Уровень безработицы"], rankImage);
                    tag.toolTip = String.Empty;
                    tags.Add(tag.key, tag);
                }

                colors.Add(GetTagColor(row["Прирост"].ToString()));
            }
            CloudTag1.ForeColors = colors;
            CloudTag1.Render(tags);
        }

        private Color GetTagColor (string grown)
        {
            double value = Convert.ToDouble(grown);
            if (value <= 0)
            {
                return Color.Green;
            }
            return Color.Red;
        }

        private void SetUpPieChart(UltraChart chart)
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
            //chart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> чел. (<PERCENT_VALUE:N2>%)";
            
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
                            color = Color.Green;
                            stopColor = Color.ForestGreen;
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
        
        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0010_chart1_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateSakhalin);

            DateTime currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeSakhalin, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", lastDateTimeSakhalin, 5);

            query = DataProvider.GetQueryText("STAT_0001_0010_Text_Sakhalin");
            DataTable dtCommentTextSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentTextSakhalin);

            // Возьмем данные по федеральной базе
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов]", currDateTimeSakhalin, 4);
            DataTable dtDateCur = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0010_redundant_federal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateCur);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTimeSakhalin, 4);
            query = DataProvider.GetQueryText("STAT_0001_0010_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            currDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[1][4].ToString(), 3);
            lastDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[0][4].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов]", currDateTime, 4);

            query = DataProvider.GetQueryText("STAT_0001_0010_redundant_federal");
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
            query = DataProvider.GetQueryText("STAT_0001_0010_commentText");
            DataTable dtCommentText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);

            if (dtCommentText.Rows.Count > 0)
            {
                string dateTimeStr = String.Format("&nbsp;{0:dd.MM.yyyy}&nbsp;", currDateTimeSakhalin);
                double totalCount = Convert.ToDouble(dtCommentTextSakhalin.Rows[1][1]);
                double totalRate = 1 + Convert.ToDouble(dtCommentTextSakhalin.Rows[1][3]);
                double totalGrow = Convert.ToDouble(dtCommentTextSakhalin.Rows[1][2]);
                string totalRateArrow = totalRate > 1
                                               ? "увеличилось&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;"
                                               : "снизилось&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;";
                //string totalRateStr = totalRate > 0 ? "составил" : "составило";
                double unemploued = Convert.ToDouble(dtCommentTextSakhalin.Rows[2][1]);

                double redundantlevelValue = Convert.ToDouble(dtCommentTextSakhalin.Rows[3][1]);

                double redundantLevelRFValue = GetDoubleDTValue(dtRedundantFederal, "Российская  Федерация");

                string redundantLevelRFArrow;
                string redundantLevelRFDescription = String.Format("&nbsp;<span class='DigitsValue'>{0:P2}</span>", redundantLevelRFValue);
                if (redundantlevelValue > redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (redundantlevelValue < redundantLevelRFValue)
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
                if (redundantlevelValue > redundantLevelDfoValue)
                {
                    redundantLevelDfoArrow = "выше&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (redundantlevelValue < redundantLevelDfoValue)
                {
                    redundantLevelDfoArrow = "ниже&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
                }
                else
                {
                    redundantLevelDfoArrow = "соответствует&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;уровню";
                    redundantLevelDfoDescription = String.Empty;
                }
                string redundantLevelDfoGrow = redundantLevelDfoValue != 0 ?
                    String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}чем в ДФО{1}<br/>", redundantLevelDfoArrow, redundantLevelDfoDescription) :
                    "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;данные по ДФО отсутствуют<br/>";

                redundantLevelRFGrow = String.Format("{0}{1}", redundantLevelDfoGrow, redundantLevelRFGrow);

                double vacancyCount = GetDoubleDTValue(dtCommentText, "Потребность в работниках");

                double tensionKoeff = GetDoubleDTValue(dtCommentText, "Число зарегистрированных безработных в расчёте на 1 вакансию", Double.MinValue);

                string str1 =
                    String.Format(
                        @"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;уровень регистрируемой в органах службы занятости безработицы в Сахалинской области&nbsp;<span class='DigitsValueXLarge'><b>{1:P2}</b></span> {2} <div style='height: 7px; clear: both'></div>",
                        dateTimeStr, redundantlevelValue, redundantLevelRFGrow);

                string str2 = String.Format(@"Численность безработных&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;чел.<br>", totalCount);

                string str3 = String.Format(@"За период с&nbsp;<span class='DigitsValue'><b>{1:dd.MM}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{2:dd.MM}</b></span>&nbsp;число безработных {0} на&nbsp;<span class='DigitsValue'><b>{3:N0}</b></span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'><b>{4:P2}</b></span>)<br/>",
                        totalRateArrow, lastDateTimeSakhalin, currDateTimeSakhalin, Math.Abs(totalGrow), totalRate);

                string str5 = String.Format(@"Население без статуса безработных&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;чел.<div style='height: 7px; clear: both'></div>", unemploued);

                string str6 = String.Format(@"Потребность в работниках, заявленная работодателями в органы службы занятости населения&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;вакансий<br/>", dtCommentTextSakhalin.Rows[5][1]);

                string str7 = String.Format(@"Коэффициент напряженности на рынке труда&nbsp;<span class='DigitsValueXLarge'><b>{0:N2}</b></span><br/>", dtCommentTextSakhalin.Rows[6][1]);

                string str8 = String.Format(@"Численность безработных на 1 вакансию&nbsp;<span class='DigitsValueXLarge'><b>{0:N1}</b></span>", dtCommentTextSakhalin.Rows[7][1]);

                DataTable dtPopulation = new DataTable();
                query = DataProvider.GetQueryText("STAT_0001_0010_population");
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Год", dtPopulation);
                populationYear.Value = dtPopulation.Rows[0][0].ToString();

                string str9 = String.Format("<div style='height: 7px; clear: both'></div>Экономически активное население&nbsp;<span class='DigitsValue'><b>{0:N0}</b></span>&nbsp;чел.<br/>Численность населения Сахалинской области&nbsp;<span class='DigitsValue'><b>{1:N0}</b></span>&nbsp;чел.",
                                               dtCommentTextSakhalin.Rows[4][1], dtPopulation.Rows[0][1]);

                CommentText1.Text = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", str1, str2, str3, str5, str6, str7, str8, str9);

                query = DataProvider.GetQueryText("STAT_0001_0010_debts_mo_date");
                DataTable dtDebtsDateSakhalin = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDateSakhalin);

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
                }

                debtsPeriodCurrentDate.Value = dtDebtsDateSakhalin.Rows[1][1].ToString();
                debtsPeriodLastWeekDate.Value = dtDebtsDateSakhalin.Rows[0][1].ToString();

                query = DataProvider.GetQueryText("STAT_0001_0010_debts_Sakhalin");
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
                                               ? String.Format("увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N3}</span>&nbsp;тыс. руб.", Math.Abs(debtsPercent))
                                               : String.Format("уменьшилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N3}</span>&nbsp;тыс. руб.", Math.Abs(debtsPercent));

                string str10;
                if (totalLastWeekDebts == 0 && totalDebts == 0)
                {
                    str10 = String.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате 
заработной платы<br/>", dateTimeDebtsStr, "Сахалинской области");
                }
                else if (totalDebts == 0)
                {
                    str10 = String.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате заработной платы. 
Задолженность в сумме&nbsp;<span class='DigitsValue'><b>{2:N3}</b></span>&nbsp;тыс. руб. была погашена за период с&nbsp;<span class='DigitsValue'><b>{3}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{0}</b></span>.<br/>",
dateTimeDebtsStr, "Сахалинской области", totalLastWeekDebts, dateLastTimeDebtsStr);
                }
                else
                {
                    str10 = String.Format(@"На&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;задолженность по выплате заработной платы составляет 
&nbsp;<span class='DigitsValue'><b>{1:N3}</b></span>&nbsp;тыс. руб. (<span class='DigitsValue'><b>{2:N0}</b></span>&nbsp;чел.)<br/>За период с&nbsp;<span class='DigitsValue'><b>{5}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;задолженность {3}",
dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, "Сахалинской области", dateLastTimeDebtsStr);
                }
                string debtsMoList = String.Empty;

                if (dtDebtsDateSakhalin.Rows.Count > 0)
                {
                    
                    query = DataProvider.GetQueryText("STAT_0001_0010_debts_mo");
                    DataTable dtDebtsMo = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDebtsMo);
                    if (dtDebtsMo.Rows.Count > 0)
                    {
                        debtsMoList =
                            String.Format("Задолженность по выплате заработной платы присутствует в&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;муниципальных образованиях", dtDebtsMo.Rows.Count);
                    }
                }
                
                lbDebtDescription.Text = String.Format("{0} {1}", str10, debtsMoList);
            }
        }

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
