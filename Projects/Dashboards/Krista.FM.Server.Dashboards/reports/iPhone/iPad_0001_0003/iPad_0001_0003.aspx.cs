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

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class iPad_0001_0003 : CustomReportPage
    {
        #region Поля

        private DataTable dtChart;
        private DataTable dtDate;
        private DataTable dtOperative;
        private DataTable dtKoeff;

        private DateTime currDateTime;
        private DateTime lastDateTime;
        private DateTime debtsCurrDateTime;
        private DateTime debtsLastDateTime;
        private DateTime redundantLevelRFDateTime;

        // Текущая дата
        private CustomParam periodCurrentDate;
        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;

        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;

        //private DateTime redundantLevelRFDateTime;

        public bool IsYearJoint()
        {
            return (currDateTime.Year != lastDateTime.Year);
        }

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart1.Width = 740;
            UltraChart1.Height = 290;

            UltraChart2.Width = 740;
            UltraChart2.Height = 290;

            UltraChart3.Width = 360;
            UltraChart3.Height = 115;

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #region Настройка диаграммы

            SetupDynamicChart(UltraChart1, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.Text = "% от экономически активного населения";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 90;
            UltraChart1.TitleLeft.WrapText = true;
            UltraChart1.TitleLeft.Extent = 60;
            AddLineAppearencesUltraChart1();

            SetupDynamicChart(UltraChart2, "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N0> человек", "<DATA_VALUE:N0>");
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.FontColor = Color.White;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart2.TitleLeft.Text = "человек, на конец месяца";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.WrapText = true;
            AddLineAppearencesUltraChart();
            UltraChart2.TitleLeft.Extent = 60;
            UltraChart2.TitleLeft.Margins.Bottom = 90;

            //LineAppearance lineAppearance = new LineAppearance();
            //lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            //lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            //lineAppearance.Thickness = 4;
            //lineAppearance.SplineTension = (float)0.3;
            //UltraChart2.SplineChart.LineAppearances.Add(lineAppearance);

            UltraChart3.ChartType = ChartType.PieChart;
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Axis.Y.Extent = 5;
            UltraChart3.Axis.X.Extent = 5;
            UltraChart3.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> человек (<PERCENT_VALUE:N2>%)";
            UltraChart3.Legend.Visible = false;
            UltraChart3.Legend.Location = LegendLocation.Left;
            UltraChart3.Legend.SpanPercentage = 40;
            UltraChart3.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart3.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart3.Axis.X.Labels.Font = new Font("Verdana", 12);
            UltraChart3.Axis.X.Labels.FontColor = Color.White;
            UltraChart3.Data.ZeroAligned = true;
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N2>%";
            UltraChart3.PieChart.Labels.FontColor = Color.FromArgb(192, 192, 192);
            UltraChart3.PieChart.Labels.LeaderLineColor = Color.FromArgb(192, 192, 192);
            UltraChart3.PieChart.Labels.LeaderDrawStyle = LineDrawStyle.Dot;
            UltraChart3.PieChart.Labels.LeaderEndStyle = LineCapStyle.Square;
            UltraChart3.PieChart.RadiusFactor = 110;
            UltraChart3.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart3_ChartDrawItem);
            //UltraChart1.TitleLeft.Visible = true;
            // UltraChart1.TitleLeft.Text = "руб.";
            //UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Far;

            UltraChart3.ColorModel.ModelStyle = ColorModels.CustomSkin;
            // UltraChart3.ColorModel.Skin.ApplyRowWise = false;
            UltraChart3.ColorModel.Skin.PEs.Clear();
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
                UltraChart3.ColorModel.Skin.PEs.Add(pe);
            }

            //UltraChart3.PieChart3D.StartAngle = 300;
            //UltraChart3.PieChart3D.BreakDistancePercentage = 3;

            //UltraChart3.Transform3D.XRotation = -140;
            //UltraChart3.Transform3D.YRotation = -50;
            //UltraChart3.Transform3D.ZRotation = 0;
            UltraChart3.Style.Add("margin-top", " -10px");

            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);

            #endregion
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            string currentYear = string.Empty;

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

                //if (primitive is Polyline)
                //{
                //    Polyline polyline = (Polyline)primitive;
                //    foreach (DataPoint point in polyline.points)
                //    {
                //        if (point.DataPoint != null)
                //        {
                //            string monthStr = point.DataPoint.Label;
                //            string[] strs = monthStr.Split('-');
                //            if (strs.Length > 1)
                //            {
                //                currentYear = strs[0];
                //                monthStr = strs[1];
                //            }

                //            if (currentYear != string.Empty)
                //            {
                //                monthStr = string.Format("{0} {1} года", monthStr.ToLower(), currentYear);
                //            }

                //            point.DataPoint.Label = monthStr;
                //        }
                //    }
                //}
            }
        }

        private static void SetupDynamicChart(UltraChart chart, string tooltipsFormatString, string axisYLabelsFormatString)
        {
            chart.ChartType = ChartType.SplineChart;
            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 45;
            chart.Axis.X.Extent = 69;
            chart.Tooltips.FormatString = tooltipsFormatString;
            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.SpanPercentage = 25;
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.Axis.X.Labels.Font = new Font("Verdana", 7);
            chart.Axis.X.Labels.FontColor = Color.White;
            chart.Data.ZeroAligned = true;
            chart.SplineChart.NullHandling = NullHandling.DontPlot;
            chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            chart.Data.SwapRowsAndColumns = true;
            chart.Axis.Y.Labels.ItemFormatString = axisYLabelsFormatString;
            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

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

            CommentText1.Text = string.Empty;

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0012_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodYear.Value = dtDate.Rows[0][4].ToString();

            UserParams.SubjectFO.Value = String.Empty;
            UserParams.Filter.Value = ".DataMember";

            query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_unjobedOperative"));
            dtOperative = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtOperative);

            #region
            /*   LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 4;
            lineAppearance.SplineTension = 0.3f;
            lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 4;
            lineAppearance.SplineTension = 0.3f;
            lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 4;
            lineAppearance.SplineTension = 0.3f;
            lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Solid;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 4;
            lineAppearance.SplineTension = 0.3f;
            lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Solid;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);*/
            #endregion

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            CommentTextDataBind();

            lbDescription.Text = GetDescritionText(CRHelper.PeriodDayFoDate(UserParams.PeriodYear.Value));
        }

        private string GetDescritionText(DateTime date)
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0012_text");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtText);

            double thisMot;
            double ufoMot;

            string description = string.Empty;
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
                   "В&nbsp;<span style=\"color: white\"><b>{0} {1} года</b></span>&nbsp;уровень общей безработицы<br/>по методологии МОТ в УрФО&nbsp;<span style=\"color: white\"><b>{2:P2}</b></span><br/>({3})<br/><div style='height: 7px; clear: both'></div>", CRHelper.RusMonthPrepositional(date.Month), date.Year, thisMot, description);
            }

            double thisMonth;
            double prevMonth;
            if (Double.TryParse(dtText.Rows[1][1].ToString(), out thisMonth) &&
                Double.TryParse(dtText.Rows[1][2].ToString(), out prevMonth))
            {
                double grownValue = prevMonth - thisMonth;
                string grown = grownValue < 0 ? "выросло" : "снизилось";
                // string compile = grownValue < 0 ? "составил" : "составило";
                double grownTemp = thisMonth / prevMonth;
                description += String.Format("За {1} {2} года число безработных<br/>{0}{5} на&nbsp;<span style=\"color: white\"><b>{3:N2}</b></span>&nbsp;тыс. чел. (темп роста&nbsp;<b><span style=\"color: white\">{4:P2}</span></b>)<div style='height: 7px; clear: both'></div>", grown, CRHelper.RusMonth(date.Month), date.Year, Math.Abs(grownValue), grownTemp, GetImage(grown));
            }
            description += String.Format("Экономически активное население<br/><span style=\"color: white\"><b>{0:N2}</b></span>&nbsp;тыс. чел.<div style='height: 7px; clear: both'></div>Численность безработных&nbsp;<span style=\"color: white\"><b>{1:N2}</b></span>&nbsp;тыс. чел.",
                 dtText.Rows[0][1], dtText.Rows[1][1]);

            return description;
        }

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

        #region Обработчики диаграммы

        private void AddLineAppearencesUltraChart1()
        {
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.ColorModel.Skin.PEs.Clear();
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
                UltraChart1.ColorModel.Skin.PEs.Add(pe);

                lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.SplineTension = 0.3f;
                lineAppearance2.IconAppearance.PE = pe;
                lineAppearance2.Thickness = 10;

                UltraChart1.SplineChart.LineAppearances.Add(lineAppearance2);
            }
        }

        private void AddLineAppearencesUltraChart()
        {
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart2.ColorModel.Skin.ApplyRowWise = true;
            UltraChart2.ColorModel.Skin.PEs.Clear();
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
                UltraChart2.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance2 = new LineAppearance();
                lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.SplineTension = 0.3f;
                lineAppearance2.IconAppearance.PE = pe;
                lineAppearance2.Thickness = 10;

                UltraChart2.SplineChart.LineAppearances.Add(lineAppearance2);
            }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_chart1"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            int operativeRowNum = 0;

            // Проходим по всем строкам неоперативного грида
            for (int mainRowNum = 0; mainRowNum < dtChart.Rows.Count; mainRowNum++)
            {
                // если где-то пусто, ищем в оперативной
                if (dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы "] == DBNull.Value ||
                    dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] == DBNull.Value)
                {
                    string checkingPeriod =
                        dtChart.Rows[mainRowNum][1].ToString().Replace("[Период].[Год Квартал Месяц]", "[Период].[Период]");
                    while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                        (operativeRowNum < dtOperative.Rows.Count - 1 &&
                        dtOperative.Rows[operativeRowNum][1].ToString() == dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                    {
                        operativeRowNum++;
                    }

                    if (dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы "] == DBNull.Value)
                    {
                        dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы "] =
                            dtOperative.Rows[operativeRowNum][3];
                    }
                    if (dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] == DBNull.Value)
                    {
                        dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] =
                            dtOperative.Rows[operativeRowNum][4];
                    }
                    if (dtOperative.Columns.Count == 6 && dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы УрФО "] == DBNull.Value)
                    {
                        dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы УрФО "] =
                            dtOperative.Rows[operativeRowNum][5];
                    }
                }
            }

            string lastPeriod = dtChart.Rows[dtChart.Rows.Count - 1][1].ToString();
            DateTime lastDate = CRHelper.PeriodDayFoDate(lastPeriod);

            for (; operativeRowNum < dtOperative.Rows.Count; operativeRowNum++)
            {
                string periodOperative = dtOperative.Rows[operativeRowNum][1].ToString();
                DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                if (dateOperative > lastDate &&
                    (dateOperative.Month > lastDate.Month && dateOperative.Year == lastDate.Year))
                {
                    if (operativeRowNum < dtOperative.Rows.Count - 1)
                    {
                        DateTime dateOperativeNext = CRHelper.PeriodDayFoDate(dtOperative.Rows[operativeRowNum + 1][1].ToString());

                        if (dateOperativeNext.Month > dateOperative.Month)
                        {
                            DataRow row = dtChart.NewRow();
                            row[1] = periodOperative;
                            row["Уровень зарегистрированной безработицы "] = dtOperative.Rows[operativeRowNum][3];
                            row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum][4];
                            if (dtOperative.Columns.Count == 6)
                            {
                                row["Уровень зарегистрированной безработицы УрФО "] = dtOperative.Rows[operativeRowNum][5];
                            }
                            dtChart.Rows.Add(row);
                        }
                    }
                    else
                    {
                        DataRow row = dtChart.NewRow();
                        row[1] = periodOperative;
                        row["Уровень зарегистрированной безработицы "] = dtOperative.Rows[operativeRowNum][3];
                        if (dtOperative.Rows[operativeRowNum][4] != DBNull.Value)
                        {
                            row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum][4];
                        }
                        else
                        {
                            row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum - 1][4];
                        }
                        if (dtOperative.Columns.Count == 6)
                        {
                            row["Уровень зарегистрированной безработицы УрФО "] = dtOperative.Rows[operativeRowNum][5];
                        }
                        dtChart.Rows.Add(row);
                    }
                }
            }

            dtChart.Columns["Уровень зарегистрированной безработицы "].ColumnName =
                    String.Format("{0} {1}", "Уровень зарегистрированной безработицы ", "УрФО");
            dtChart.Columns["Уровень общей безработицы по методологии МОТ "].ColumnName =
                String.Format("{0} {1}", "Уровень общей безработицы по методологии МОТ ", "УрФО");

            dtChart.AcceptChanges();

            // DataTable dt = ConvertPeriodNames(dtChart);

            //UltraChart1.Series.Clear();
            //for (int i = 1; i < dt.Columns.Count; i++ )
            //{
            //    UltraChart1.Series.Add(CRHelper.GetNumericSeries(i, dt));
            //}

            UltraChart1.DataSource = ConvertPeriodNames(dtChart);
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_chart2"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            string period = dtChart.Rows[dtChart.Rows.Count - 1][1].ToString();
            DateTime date = CRHelper.PeriodDayFoDate(period);
            int operativeRowNum = 0;

            // Проходим по всем строкам неоперативного грида
            for (int mainRowNum = 0; mainRowNum < dtChart.Rows.Count; mainRowNum++)
            {
                // если где-то пусто, ищем в оперативной
                if (dtChart.Rows[mainRowNum]["Число безработных, зарегистрированных в службе занятости "] == DBNull.Value)
                {
                    string checkingPeriod =
                        dtChart.Rows[mainRowNum][1].ToString().Replace("[Период].[Год Квартал Месяц]", "[Период].[Период]");
                    while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                        (operativeRowNum < dtOperative.Rows.Count - 1 &&
                        dtOperative.Rows[operativeRowNum][1].ToString() == dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                    {
                        operativeRowNum++;
                    }

                    dtChart.Rows[mainRowNum]["Число безработных, зарегистрированных в службе занятости "] =
                        dtOperative.Rows[operativeRowNum][2];

                }
            }


            for (; operativeRowNum < dtOperative.Rows.Count; operativeRowNum++)
            {
                string periodOperative = dtOperative.Rows[operativeRowNum][1].ToString();
                DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                if (dateOperative > date &&
                    (dateOperative.Month > date.Month && dateOperative.Year == date.Year))
                {
                    if (operativeRowNum < dtOperative.Rows.Count - 1)
                    {
                        DateTime dateOperativeNext = CRHelper.PeriodDayFoDate(dtOperative.Rows[operativeRowNum + 1][1].ToString());

                        if (dateOperativeNext.Month > dateOperative.Month)
                        {
                            DataRow row = dtChart.NewRow();
                            row[1] = periodOperative;
                            row["Число безработных, зарегистрированных в службе занятости "] = dtOperative.Rows[operativeRowNum][2];
                            dtChart.Rows.Add(row);
                        }
                    }
                    else
                    {
                        DataRow row = dtChart.NewRow();
                        row[1] = periodOperative;
                        row["Число безработных, зарегистрированных в службе занятости "] = dtOperative.Rows[operativeRowNum][2];
                        dtChart.Rows.Add(row);
                    }
                }
            }
            dtChart.Columns["Число безработных, зарегистрированных в службе занятости "].ColumnName =
                String.Format("{0} {1}", "Число безработных, зарегистрированных в службе занятости ", "УрФО");
            dtChart.Columns["Общая численность безработных по методологии МОТ "].ColumnName =
               String.Format("{0} {1}", "Общая численность безработных по методологии МОТ ", "УрФО");
            dtChart.AcceptChanges();
            UltraChart2.DataSource = ConvertPeriodNames(dtChart);
        }

        void UltraChart3_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
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

        void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0}\n{1}", "Уральский федеральный округ", box.DataPoint.Label);
                    }
                }
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_chart3"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            UltraChart3.DataSource = dtChart;
        }

        private static DataTable ConvertPeriodNames(DataTable dt)
        {
            string year = String.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string period = row[1].ToString();
                DateTime date = CRHelper.PeriodDayFoDate(period);
                if (date.Year.ToString() == year)
                {
                    row[1] = String.Format("{0}", CRHelper.RusMonth(date.Month));
                }
                else
                {
                    row[1] = String.Format("{0} - {1}", date.Year, CRHelper.RusMonth(date.Month));
                    year = date.Year.ToString();
                }
            }
            dt.Columns.RemoveAt(0);
            dt.AcceptChanges();
            return dt;
        }

        #endregion

        private void CommentTextDataBind()
        {
            DataTable dtDateCur = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateCur);

            currDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[0][5].ToString(), 3);
            lastDateTime = currDateTime.AddDays(-7);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);

            query = DataProvider.GetQueryText("STAT_0001_0002_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            query = DataProvider.GetQueryText("STAT_0001_0002_redundantLevelRF_date");
            DataTable dtRedundantLevelRFDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtRedundantLevelRFDate);
            redundantLevelRFDate.Value = dtRedundantLevelRFDate.Rows[0][1].ToString();
            redundantLevelRFDateTime = CRHelper.DateByPeriodMemberUName(dtRedundantLevelRFDate.Rows[0][1].ToString(), 3);

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

            if (dtCommentText.Rows.Count > 0)
            {
                string dateTimeStr = string.Format("&nbsp;{0:dd.MM.yyyy}&nbsp;", currDateTime);
                double totalCount = GetDoubleDTValue(dtCommentText, "Общая численность по УрФО");
                double totalRate = 1 + GetDoubleDTValue(dtCommentText, "Общий темп прироста по УрФО");
                double totalGrow = GetDoubleDTValue(dtCommentText, "Общий прирост по УрФО");
                string totalRateArrow = totalRate > 1
                                               ? "выросло&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;"
                                               : "снизилось&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;";
                //string totalRateStr = totalRate > 0 ? "составил" : "составило";

                string[] growSubjectList = GetStringDTValue(dtCommentText, "Список субъектов с приростом").Split(',');
                string[] fallSubjectList = GetStringDTValue(dtCommentText, "Список субъектов с падением").Split(',');
                string[] normalSubjectList = GetStringDTValue(dtCommentText, "Список субъектов с неизменной численностью").Split(','); ;

                double redundantlevelValue = GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы ");

                double redundantLevelRFValue = GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы РФ");
                string redundantLevelRFArrow;
                string redundantLevelRFDescription = String.Format("&nbsp;<span style='color: white'><b>{0:N3}%</b></span>", redundantLevelRFValue);
                if (redundantlevelValue > redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "выше&nbsp;<img src=\"../../../images/ballRedBB.png\">&nbsp;уровня";
                }
                else if (redundantlevelValue < redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "ниже&nbsp;<img src=\"../../../images/ballGreenBB.png\">&nbsp;уровня";
                }
                else
                {
                    redundantLevelRFArrow = "соответствует&nbsp;<img src=\"../../../images/ballGreenBB.png\">&nbsp;уровню";
                    redundantLevelRFDescription = String.Empty;
                }
                string redundantLevelRFGrow = String.Format("<br/>({0} в целом по РФ{1})<br/>", redundantLevelRFArrow, redundantLevelRFDescription);

                double vacancyCount = GetDoubleDTValue(dtCommentText, "Потребность в работниках");

                string freeVacancyCountStr = (totalCount - vacancyCount) > 0
                       ? string.Format("ниже количества безработных на&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;единиц", Math.Abs(totalCount - vacancyCount))
                       : string.Format("выше количества безработных на&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;единиц", Math.Abs(totalCount - vacancyCount));
                double tensionKoeff = GetDoubleDTValue(dtCommentText, "Число зарегистрированных безработных в расчёте на 1 вакансию", double.MinValue);

                string str1 =
                    string.Format(
                        @"На&nbsp;<span style='color: white'><b>{0}</b></span>&nbsp;уровень регистрируемой в органах службы занятости безработицы в УрФО&nbsp;<span style='color: white'><b>{1:P3}</b></span> {2} <div style='height: 7px; clear: both'></div>",
                        dateTimeStr, redundantlevelValue / 100, redundantLevelRFGrow);

                string str2 = string.Format(@"За неделю с&nbsp;<span style='color: white'><b>{1:dd.MM}</b></span>&nbsp;по&nbsp;<span style='color: white'><b>{2:dd.MM}</b></span>&nbsp;число безработных {0} на&nbsp;<span style='color: white'><b>{3:N0}</b></span>&nbsp;чел. (темп роста&nbsp;<span style='color: white'><b>{4:P2}</b></span>)<br/>",
                        totalRateArrow, lastDateTime, currDateTime, Math.Abs(totalGrow), totalRate);

                int subjectCount = 0;

                string str3 = "<table style=\"border: 1px solid #323232; border-collapse: collapse; margin-left: 20px\"><tr>";
                for (int i = 0; i < growSubjectList.Length; i++)
                {
                    if (growSubjectList[i]!= String.Empty)
                    {
                        str3 +=
                            String.Format(
                                "<td style=\"border: 1px solid #323232;  background-position: right center; background-repeat: no-repeat; background-image: url(../../../images/arrowRedUpBB.png); width:\"80px\" height:\"16px\"\" width=\"80px\" height=\"16px\"><span style='color: white'><b>{0}</b> </span></td>",
                                growSubjectList[i]);
                        subjectCount++;
                        if (subjectCount == 3)
                        {
                            str3 += "</tr><tr>";
                        }
                    }
                }

                string str4 = String.Empty;
                for (int i = 0; i < normalSubjectList.Length && normalSubjectList[i]!= String.Empty; i++)
                {
                    if (normalSubjectList[i] != String.Empty)
                    {
                        str4 +=
                            String.Format(
                                "<td style=\"border: 1px solid #323232;\"><span style='color: white'><b>{0}</b> </span></td>", normalSubjectList[i]);
                        subjectCount++;
                        if (subjectCount == 3)
                        {
                            str4 += "</tr><tr>";
                        }
                    }
                }

                string str5 = String.Empty;
                for (int i = 0; i < fallSubjectList.Length && fallSubjectList[i]!= String.Empty; i++)
                {
                    if (fallSubjectList[i] != String.Empty)
                    {
                        str4 +=
                            String.Format(
                                "<td style=\"border: 1px solid #323232;  background-repeat: no-repeat; background-position: 70px center; background-image: url(../../../images/arrowGreenDownBB.png); width:\"90px\" height:\"17px\"\" width=\"90px\" height=\"17px\"><span style='color: white'><b>{0}</b> </span></td>",
                                fallSubjectList[i]);
                        subjectCount++;
                        if (subjectCount == 3)
                        {
                            str4 += "</tr><tr>";
                        }
                    }
                }

                str5 += "</tr></table><div style='height: 5px; clear: both'></div>";

                string str6 = string.Format(@"Численность безработных&nbsp;<span style='color: white'><b>{0:N3}</b></span>&nbsp;тыс. человек<div style='height: 7px; clear: both'></div>", totalCount / 1000);

                string str7 = string.Format(@"Потребность в работниках, заявленная работодателями в органы службы занятости населения&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;вакансий<div style='height: 7px; clear: both'></div>", vacancyCount);

                string str8 = tensionKoeff != double.MinValue ? string.Format(@"Число зарегистрированных безработных в расчёте на 1 вакансию&nbsp;<span style='color: white'><b>{0:N2}</b></span>&nbsp;", tensionKoeff) : string.Empty;


                CommentText1.Text = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", str1, str2, str3, str4, str5, str6, str7, str8);

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

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return string.Empty;
        }
    }
}
