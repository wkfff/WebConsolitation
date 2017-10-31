using System;
using System.Data;
using System.Drawing;
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
    public partial class STAT_0001_0007 : CustomReportPage
    {

        #region Поля

        private DataTable dtChart;
        private DataTable dtDate;

        // Текущая дата
        private CustomParam periodCurrentDate;
        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;

        private CustomParam periodPrevMonth;

        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;

        //private DateTime redundantLevelRFDateTime;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart1.Width = 360;
            UltraChart1.Height = 125;

            UltraChart2.Width = 740;
            UltraChart2.Height = 450;

            UltraChart3.Width = 740;
            UltraChart3.Height = 350;

            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #region Настройка диаграммы 1

            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.Extent = 5;
            UltraChart1.Axis.X.Extent = 5;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> человек (<PERCENT_VALUE:N2>%)";
            UltraChart1.Legend.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N2>%";
            UltraChart1.PieChart.Labels.FontColor = Color.FromArgb(192, 192, 192);
            UltraChart1.PieChart.Labels.LeaderLineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.PieChart.Labels.LeaderDrawStyle = LineDrawStyle.Dot;
            UltraChart1.PieChart.Labels.LeaderEndStyle = LineCapStyle.Square;
            UltraChart1.PieChart.Labels.Font = new Font("Verdana", 10);
            UltraChart1.PieChart.RadiusFactor = 110;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
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
                            stopColor = Color.Gold;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }

            UltraChart1.Style.Add("margin-top", " -10px");

            #endregion

            #region Настройка диаграммы 2

            SetupDynamicChart(UltraChart2, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.FontColor = Color.White;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart2.TitleLeft.Text = "% от экономически активного населения";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = 90;
            UltraChart2.TitleLeft.WrapText = true;
            UltraChart2.TitleLeft.Extent = 60;
            UltraChart2.Legend.SpanPercentage = 30;
            AddLineAppearencesUltraChart2();

            #endregion

            #region Настройка диаграммы 3

            SetupDynamicChart(UltraChart3, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N0> человек", "<DATA_VALUE:N0>");
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.TitleLeft.Visible = true;
            UltraChart3.TitleLeft.FontColor = Color.White;
            UltraChart3.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart3.TitleLeft.Text = "человек, на конец месяца";
            UltraChart3.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart3.TitleLeft.WrapText = true;
            AddLineAppearencesUltraChart();
            UltraChart3.TitleLeft.Extent = 60;
            UltraChart3.TitleLeft.Margins.Bottom = 90;
            UltraChart3.Legend.SpanPercentage = 22;
            UltraChart3.Axis.Y.Extent = 75;

            #endregion

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string query;

            #region Инициализация параметров запроса

            periodCurrentDate = UserParams.CustomParam("period_current_date");
            debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            periodPrevMonth = UserParams.CustomParam("prev_month");

            #endregion

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0007_part1_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDate);
            periodPrevMonth.Value = dtDate.Rows[0]["Месяц"].ToString();
            UserParams.PeriodMonth.Value = dtDate.Rows[1]["Месяц"].ToString();
            UltraChart1.DataBind();
            lbDescription.Text = GetDescritionText(CRHelper.DateByPeriodMemberUName(UserParams.PeriodMonth.Value, 3));
            CommentTextDataBind();
            
            UltraChart2.DataBind();
            UltraChart3.DataBind();

        }

        #region Обработчики диаграммы

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            string currentYear = String.Empty;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 15;
                    }
                }
                else if (primitive is Text)
                {
                    Text text = primitive as Text;
                    if (!String.IsNullOrEmpty(text.Path) && (text.Path == "Border.Title.Grid.X"))
                    {
                        if  ((text.GetTextString().Split(' ').Length == 3) &&
                            (CRHelper.IsMonthCaption(text.GetTextString().Split(' ')[0])) &&
                            (MathHelper.IsDouble(text.GetTextString().Split(' ')[1])) &&
                            (text.GetTextString().Split(' ')[2] == "года"))
                        {
                            string year = text.GetTextString().Split(' ')[1].Trim();
                            string month = CRHelper.ToLowerFirstSymbol(text.GetTextString().Split(' ')[0].Trim());
                            if (year != currentYear)
                            {
                                currentYear = year;
                                text.SetTextString(String.Format("{0} - {1}", year, month));
                            }
                            else
                            {
                                text.SetTextString(month);
                            }
                        }
                    }
                }
            }
        }

        private static void SetupDynamicChart(UltraChart chart, string tooltipsFormatString, string axisYLabelsFormatString)
        {
            chart.ChartType = ChartType.SplineChart;
            chart.Border.Thickness = 0;
            chart.Axis.X.Extent = 100;
            chart.Tooltips.FormatString = tooltipsFormatString;
            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.Font = new Font("Verdana", 10);
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.Axis.X.Labels.Font = new Font("Verdana", 10);
            chart.Axis.X.Labels.FontColor = Color.White;
            chart.Data.ZeroAligned = true;
            chart.SplineChart.NullHandling = NullHandling.DontPlot;
            chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            chart.Data.SwapRowsAndColumns = true;
            chart.Axis.Y.Labels.ItemFormatString = axisYLabelsFormatString;
            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;
            chart.Axis.Y.Extent = 45;
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);
        }

        private static DataTable ConvertPeriodNames(DataTable dt)
        {
            string year = String.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string period = row[1].ToString();
                DateTime date = CRHelper.PeriodDayFoDate(period);
                row[1] = String.Format("{1} {0} года", date.Year, CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(date.Month)));
                /*if (date.Year.ToString() == year)
                {
                    row[1] = String.Format("{0}", CRHelper.RusMonth(date.Month));
                }
                else
                {
                    row[1] = String.Format("{0} - {1}", date.Year, CRHelper.RusMonth(date.Month));
                    year = date.Year.ToString();
                }*/
            }
            dt.Columns.RemoveAt(0);
            dt.AcceptChanges();
            return dt;
        }

        #endregion

        #region Часть 1 "Безработица по критериям МОТ"

        private string GetDescritionText(DateTime date)
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0007_part1_text");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtText);

            double thisMot;
            double ufoMot;

            string description = String.Empty;
            if (Double.TryParse(dtText.Rows[2][1].ToString(), out thisMot) &&
               Double.TryParse(dtText.Rows[4][1].ToString(), out ufoMot))
            {
                double grownValue = ufoMot - thisMot;
                string grown;
                string middleLevel = String.Format(" уровня в целом по РФ&nbsp;<b><span style=\"color: white\">{0:P2}</span></b>", ufoMot); ;
                if (grownValue < 0)
                {
                    grown = "выше";
                }
                else if (grownValue > 0)
                {
                    grown = "ниже";
                }
                else
                {
                    grown = "соответствует";
                    middleLevel = "среднему уровню общей безработицы по РФ";
                }

                description =
                    String.Format(
                        "{0}{1} {2}", grown, GetImage(grown), middleLevel);
                description = String.Format(
                   "В&nbsp;<span style=\"color: white\"><b>{0} {1} года</b></span>&nbsp;уровень общей безработицы<br/>по методологии МОТ в СФО&nbsp;<span style=\"color: white\"><b>{2:P2}</b></span><br/>({3})<br/><div style='height: 7px; clear: both'></div>", CRHelper.RusMonthPrepositional(date.Month), date.Year, thisMot, description);
            }

            double thisMonth;
            double prevMonth;
            if (Double.TryParse(dtText.Rows[1][1].ToString(), out thisMonth) &&
                Double.TryParse(dtText.Rows[1][2].ToString(), out prevMonth))
            {
                double grownValue = prevMonth - thisMonth;
                string grown = grownValue < 0 ? "выросло" : "снизилось";
                double grownTemp = thisMonth / prevMonth;
                description += String.Format("За {1} {2} года число безработных<br/>{0}{5} на&nbsp;<span style=\"color: white\"><b>{3:N2}</b></span>&nbsp;тыс. чел. (темп&nbsp;роста&nbsp;<b><span style=\"color: white\">{4:P2}</span></b>)<div style='height: 7px; clear: both'></div>", grown, CRHelper.RusMonth(date.Month), date.Year, Math.Abs(grownValue), grownTemp, GetImage(grown));
            }
            description += String.Format("Экономически активное население<br/><span style=\"color: white\"><b>{0:N2}</b></span>&nbsp;тыс. чел.<div style='height: 7px; clear: both'></div>Численность безработных&nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp;тыс. чел.",
                 dtText.Rows[0][1], dtText.Rows[1][1]);

            return description;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0007_part1_chart1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            dtChart.Rows[0][0] = "Занятое\nнаселение";
            dtChart.Rows[1][0] = "Безработные\nпо МОТ";

            UltraChart1.DataSource = dtChart;
        }

        #endregion

        #region Часть 2 "Зарегистрированная безработица"

        private void CommentTextDataBind()
        {
            DateTime lastDate, prevDate;
            DataTable dtLastDate = new DataTable();
            DataTable dtPrevDate = new DataTable();
            DataTable dtData = new DataTable();
            DataTable dtRfData = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0007_part2_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Последняя актуальная дата", dtLastDate);
            if (dtLastDate.Rows.Count == 0)
                return;
            string str0 = String.Empty, str1 = String.Empty, str2 = String.Empty, str3 = String.Empty, str4 = String.Empty, str5 = String.Empty, str6 = String.Empty, img = String.Empty;
            UserParams.PeriodCurrentDate.Value = dtLastDate.Rows[0]["День"].ToString();
            lastDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3);
            query = DataProvider.GetQueryText("STAT_0001_0007_part2_prev_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Предыдущая актуальная дата", dtPrevDate);
            if (dtPrevDate.Rows.Count == 2)
            {
                UserParams.PeriodLastDate.Value = dtPrevDate.Rows[0]["День"].ToString();
            }
            else
            {
                UserParams.PeriodLastDate.Value = UserParams.PeriodCurrentDate.Value;
            }
            prevDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3);
            query = DataProvider.GetQueryText("STAT_0001_0007_part2_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные", dtData);

            object value = dtData.Rows[0]["Уровень безработицы"];
            if (MathHelper.IsDouble(value))
            {
                str0 = String.Format("На&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;уровень регистрируемой в органах службы занятости безработицы в СФО&nbsp;<span class='DigitsValue'>{1:N3}%</span>&nbsp;",
                    lastDate, value);
            }

            query = DataProvider.GetQueryText("STAT_0001_0007_part2_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "РФ", dtRfData);
            object valueRf = dtRfData.Rows[0]["Уровень зарегистрированной безработицы РФ"];
            if (MathHelper.IsDouble(value) && MathHelper.IsDouble(valueRf))
            {
                if (Convert.ToDouble(value) < Convert.ToDouble(valueRf))
                {
                    img = "(ниже&nbsp;<img src=\"../../../images/ballGreenBB.png\">&nbsp;уровня";
                }
                else if (Convert.ToDouble(value) > Convert.ToDouble(valueRf))
                {
                    img = "(выше&nbsp;<img src=\"../../../images/ballRedBB.png\">&nbsp;уровня";
                }
                else
                {
                    img = "(соответствует&nbsp;<img src=\"../../../images/ballYellowBB.png\">&nbsp;уровню";
                }
                str1 = String.Format("<br />{0} в целом по РФ &nbsp;<span class='DigitsValue'>{1:N3}%</span>&nbsp;)", img, valueRf);
            }

            if (dtPrevDate.Rows.Count == 2 && MathHelper.IsDouble(dtData.Rows[0]["Изменение числа безработных"]))
            {
                if (Convert.ToDouble(dtData.Rows[0]["Изменение числа безработных"]) < 0)
                {
                    img = String.Format(
                        "снизилось&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"/>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{1:P2}</span>)",
                        Math.Abs(Convert.ToDouble(dtData.Rows[0]["Изменение числа безработных"])), dtData.Rows[0]["Темп роста числа безработных"]);
                }
                else if (Convert.ToDouble(dtData.Rows[0]["Изменение числа безработных"]) > 0)
                {
                    img = String.Format(
                        "выросло&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"/>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;чел. (темп роста&nbsp;<span class='DigitsValue'>{1:P2}</span>)",
                        Math.Abs(Convert.ToDouble(dtData.Rows[0]["Изменение числа безработных"])), dtData.Rows[0]["Темп роста числа безработных"]);
                }
                else
                {
                    img = "осталось на прежнем уровне";
                }

                str2 = String.Format("<div style='height: 7px; clear: both'></div>За период с&nbsp;<span class='DigitsValue'>{0:dd.MM}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM}</span>&nbsp;число безработных {2}",
                    prevDate, lastDate, img);

                DataTable dtSubjects = new DataTable();
                query = DataProvider.GetQueryText("STAT_0001_0007_part2_regions");
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Субъект", dtSubjects);
                str3 = "<br /><table style=\"border: 1px solid #323232; border-collapse: collapse; margin-left: 20px\"><tr>";
                for (int i = 0; i < dtSubjects.Rows.Count; ++i)
                //foreach (DataRow row in dtSubjects.Rows)
                {
                    DataRow row = dtSubjects.Rows[i];
                    string subjectName = RegionsNamingHelper.ShortName(row["Субъект"].ToString().Replace("обл.", "область").Replace("кр.", "край").Replace("Р. ", "Республика "));
                    double delta = Convert.ToDouble(row["Изменение числа безработных"]);
                    if (delta < 0)
                    {
                        str3 += String.Format(
                            "<td style=\"border: 1px solid #323232; background-repeat: no-repeat; background-position: 70px center; background-image: url(../../../images/arrowGreenDownBB.png); width:\"90px\" height:\"17px\"\" width=\"90px\" height=\"17px\"><span style='color: white'><b>{0}</b> </span></td>",
                            subjectName);
                    }
                    else if (delta > 0)
                    {
                        str3 += String.Format(
                            "<td style=\"border: 1px solid #323232; background-repeat: no-repeat; background-position: 70px center; background-image: url(../../../images/arrowRedUpBB.png); width:\"90px\" height:\"17px\"\" width=\"90px\" height=\"17px\"><span style='color: white'><b>{0}</b> </span></td>",
                            subjectName);
                    }
                    else
                    {
                        str3 += String.Format(
                            "<td style=\"border: 1px solid #323232;\"><span style='color: white'><b>{0}</b> </span></td>",
                            subjectName);
                    }
                    if (i % 3 == 2)
                    {
                        str3 += "</tr><tr>";
                    }
                }
                str3 += "</tr></table>";
            }

            str4 = String.Format("<br />Численность безработных&nbsp;<span class='DigitsValue'>{0:N3}</span>&nbsp;тыс. чел.",
                Convert.ToDouble(dtData.Rows[0]["Численность безработных"]) / 1000);

            str5 = String.Format("<div style='height: 7px; clear: both'></div>Потребность в работниках, заявленная работодателями " +
                "в органы службы занятости населения&nbsp;<span class='DigitsValue'>{0:N0}</span>&nbsp;вакансий", dtData.Rows[0]["Число вакансий"]);

            str6 = String.Format("<div style='height: 7px; clear: both'></div>Коэффициент напряженности на рынке труда&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;",
                dtData.Rows[0]["Уровень напряженности"]);

            lbComment.Text = String.Format("{0}{1}{2}{3}{4}{5}{6}", str0, str1, str2, str3, str4, str5, str6);

        }

        #endregion

        #region Диаграмма "Уровень безработицы"

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0007_part3_chart"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            dtChart.Columns["Уровень зарегистрированной безработицы "].ColumnName =
                    String.Format("{0} {1}", "Уровень зарегистрированной безработицы", "СФО");
            dtChart.Columns["Уровень общей безработицы по методологии МОТ "].ColumnName =
                String.Format("{0} {1}", "Уровень общей безработицы по методологии МОТ", "СФО");

            dtChart.AcceptChanges();

            UltraChart2.DataSource = ConvertPeriodNames(dtChart);
        }

        private void AddLineAppearencesUltraChart2()
        {
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart2.ColorModel.Skin.ApplyRowWise = true;
            UltraChart2.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 5; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;

                LineAppearance lineAppearance2 = new LineAppearance();

                switch (i)
                {
                    case 1:
                        {
                            color = Color.FromArgb(72, 105, 28);
                            stopColor = Color.FromArgb(72, 125, 2);
                            lineAppearance2.LineStyle.DrawStyle = LineDrawStyle.Dot;
                            break;
                        }
                    case 2:
                        {
                            color = Color.FromArgb(12, 74, 117);
                            stopColor = Color.FromArgb(1, 99, 165);
                            lineAppearance2.LineStyle.DrawStyle = LineDrawStyle.Dot;
                            break;
                        }
                    case 3:
                        {
                            color = Color.FromArgb(150, 126, 9);
                            stopColor = Color.FromArgb(222, 183, 1);
                            lineAppearance2.LineStyle.DrawStyle = LineDrawStyle.Solid;
                            break;
                        }
                    case 4:
                        {
                            color = Color.FromArgb(141, 8, 11);
                            stopColor = Color.FromArgb(209, 1, 6);
                            lineAppearance2.LineStyle.DrawStyle = LineDrawStyle.Solid;
                            break;
                        }
                }

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 180;
                pe.StrokeWidth = 4;
                pe.StrokeOpacity = 180;
                UltraChart2.ColorModel.Skin.PEs.Add(pe);

                lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.SplineTension = 0.3f;
                lineAppearance2.IconAppearance.PE = pe;
                lineAppearance2.Thickness = 10;

                UltraChart2.SplineChart.LineAppearances.Add(lineAppearance2);
            }
        }
        
        #endregion

        #region Диаграмма "Численность безработных"
        
        private void AddLineAppearencesUltraChart()
        {
            UltraChart3.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart3.ColorModel.Skin.ApplyRowWise = true;
            UltraChart3.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.FromArgb(150, 126, 9);
                            stopColor = Color.FromArgb(222, 183, 1);
                            break;
                        }
                    case 2:
                        {
                            color = Color.FromArgb(141, 8, 11);
                            stopColor = Color.FromArgb(209, 1, 6);
                            break;
                        }
                }

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 180;
                pe.StrokeWidth = 4;
                pe.StrokeOpacity = 180;
                UltraChart3.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance2 = new LineAppearance();
                lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.SplineTension = 0.3f;
                lineAppearance2.IconAppearance.PE = pe;
                lineAppearance2.Thickness = 10;

                UltraChart3.SplineChart.LineAppearances.Add(lineAppearance2);
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0007_part4_chart"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            dtChart.Columns["Число безработных, зарегистрированных в службе занятости "].ColumnName =
                String.Format("{0} {1}", "Число безработных, зарегистрированных в службе занятости", "СФО");
            dtChart.Columns["Общая численность безработных по методологии МОТ "].ColumnName =
               String.Format("{0} {1}", "Общая численность безработных по методологии МОТ", "СФО");
            dtChart.AcceptChanges();
            UltraChart3.DataSource = ConvertPeriodNames(dtChart);
        }

        #endregion

        private static string GetImage(string direction)
        {
            if (direction.ToLower() == "выросло")
            {
                return "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"/>&nbsp;";
            }
            else if (direction.ToLower() == "снизилось")
            {
                return "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"/>&nbsp;";
            }
            else if (direction.ToLower() == "выше")
            {
                return "&nbsp;<img src=\"../../../images/ballRedBB.png\">&nbsp;";
            }
            else
            {
                return "&nbsp;<img src=\"../../../images/ballGreenBB.png\">&nbsp;";
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

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return String.Empty;
        }
    }
}
