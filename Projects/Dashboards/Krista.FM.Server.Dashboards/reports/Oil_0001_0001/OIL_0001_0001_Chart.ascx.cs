using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class OIL_0001_0001_Chart : UserControl
    {
        public int ChartWidth = 750;
        public int ChartHeight = 235;
        private double avg;
        private string shortFO;

        private DataTable dtChart;
        private DataTable dtAvg;
        private DataTable dtLimit;

        private Dictionary<string, double> limitDictionary;

        private DateTime reportDate;

        public DateTime ReportDate
        {
            get { return reportDate; }
            set { reportDate = value; }
        }

        private DateTime lastDate;

        public DateTime LastDate
        {
            get { return lastDate; }
            set { lastDate = value; }
        }

        private string oilName = "Бензин марки АИ-80";

        public string OilName
        {
            get { return oilName; }
            set { oilName = value; }
        }

        private string foName = "Российская Федерация";

        public string FoName
        {
            get { return foName; }
            set { foName = value; }
        }

        public string ShortFO
        {
            get { return shortFO; }
            set { shortFO = value; }
        }

        private string selectedFO = "0";

        public string SelectedFO
        {
            get { return selectedFO; }
            set { selectedFO = value; }
        }

        private string selectedOil = "";

        public string SelectedOil
        {
            get { return selectedOil; }
            set { selectedOil = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CustomParam.CustomParamFactory("selected_fo_chart").Value = SelectedFO;
            CustomParam.CustomParamFactory("selected_oil_chart").Value = selectedOil;

            AvgDataBind();
            LimitDataBind();
            SetChartAppearance();
            ChartDataBind();
        }

        #region Обработчики диаграмм

        private void SetChartAppearance()
        {
            chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 60);
            chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight/1.2);
            chart.EmptyChartText = "Нет данных";

            chart.ChartType = ChartType.StackBarChart;
            chart.Data.SwapRowsAndColumns = true;
            chart.Axis.Y.Extent = 180;
            chart.Axis.Y.Labels.SeriesLabels.Visible = true;
            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
            chart.Axis.X.RangeType = AxisRangeType.Custom;
            chart.Axis.X.RangeMax = 45;
            chart.Axis.X.RangeMin = 20;
            chart.Axis.X.Extent = 20;
            chart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Labels.Visible = true;
            chart.Data.ZeroAligned = false;
            chart.Legend.Visible = false;
            chart.Tooltips.FormatString = String.Format("<ITEM_LABEL>");
            chart.Tooltips.Font.Name = "Verdana";
            chart.Axis.X.MajorGridLines.Visible = false;
            chart.TitleBottom.Visible = true;
            chart.TitleBottom.Text = "Руб.";
            chart.TitleBottom.Font = new Font("Verdana", 8);
            chart.TitleBottom.FontColor = Color.Black;
            chart.TitleBottom.HorizontalAlign = StringAlignment.Far;
            chart.TitleBottom.Margins.Left = chart.Axis.Y.Extent;
            chart.Border.Color = Color.White;

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LightBlue;
            Color color2 = Color.Red;
            Color color3 = Color.Green;

            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            chart.ColorModel.Skin.ApplyRowWise = false;

            chart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            chart.Effects.Enabled = true;
            chart.Effects.Effects.Add(effect);
        }

        protected void ChartDataBind()
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0001_0001_Chart1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtChart);

            string foComment = shortFO == "РФ"
                                   ? "субъектах РФ"
                                   : String.Format("субъектах РФ, входящих в состав {0},", shortFO);

            if (dtChart.Rows.Count > 0)
            {
                chart.FillSceneGraph += new FillSceneGraphEventHandler(Chart_FillSceneGraph);
                chart.Height = dtChart.Rows.Count*38 + 66;
                if (dtChart.Columns.Count > 0)
                {
                    dtChart.Columns.RemoveAt(0);
                }

                chart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    chart.Series.Add(series);
                }

                chart.Visible = true;
                chartElementCaption.Text =
                    String.Format(
                        "На <b>{0:dd.MM.yyyy}</b> г. рост цен на <b>{2}</b> к {1:dd.MM.yyyy} г. и\\или превышение пороговых цен, установленных Минэнерго России на 31.12.2011, наблюдается в следующих субъектах РФ и компаниях",
                        reportDate, lastDate, CRHelper.ToLowerFirstSymbol(oilName));
            }
            else
            {
                chart.Visible = false;
                chartElementCaption.Text =
                    String.Format(
                        "На <b>{0:dd.MM.yyyy}</b> г. в {3} <b>не наблюдается</b> роста цен к {1:dd.MM.yyyy} г. и\\или превышение пороговых цен, установленных Минэнерго России на 31.12.2011 на&nbsp;<b>{2}</b>",
                        reportDate, lastDate, CRHelper.ToLowerFirstSymbol(oilName), foComment);
            }

        }

        private void AvgDataBind()
        {
            string query = DataProvider.GetQueryText("Oil_0001_0001_avg");
            dtAvg = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtAvg);

            avg = 0;
            if (dtAvg.Rows.Count > 0)
            {
                avg = GetRowValue(dtAvg.Rows[0], "Среднее");
            }
        }

        private void LimitDataBind()
        {
            string query = DataProvider.GetQueryText("Oil_0001_0001_limit");
            dtLimit = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtLimit);

            limitDictionary = new Dictionary<string, double>();

            foreach (DataRow row in dtLimit.Rows)
            {
                double limit = GetRowValue(row, "Предел");
                if (limit != Double.MinValue)
                {
                    limitDictionary.Add(row[0].ToString(), limit);
                }
            }
        }

        private static string GetShortRegionName(string fullName)
        {
            fullName = fullName.Replace("автономный округ", "АО");
            fullName = fullName.Replace("автономная область", "АО");
            fullName = fullName.Replace("Чувашская республика-Чувашия", "Республика Чувашия");
            fullName = fullName.Replace("Республика Адыгея (Адыгея)", "Республика Адыгея");
            return fullName;
        }

        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            double xMax = xAxis.MapMaximum;
            double yMax = yAxis.MapMaximum;
            double yMin = yAxis.MapMinimum;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text) primitive;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text) primitive;
                    if (text.Row == -1)
                    {
                        text.bounds.Width = 170;

                        text.labelStyle.HorizontalAlign = StringAlignment.Far;

                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                        text.labelStyle.WrapText = true;

                        text.SetTextString(GetShortRegionName(text.GetTextString()));
                    }
                }
                if (primitive is Box)
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string serieName = GetRowStringValue(dtChart.Rows[box.Row], "Наименование").Replace("\"", "'");
                        double lastValue = GetRowValue(dtChart.Rows[box.Row], "На начало года");
                        double increaseValue = GetRowValue(dtChart.Rows[box.Row], "Прирост");
                        double decreaseValue = GetRowValue(dtChart.Rows[box.Row], "Снижение");

                        if (lastValue != Double.MinValue && increaseValue != Double.MinValue &&
                            decreaseValue != Double.MinValue)
                        {
                            lastValue = lastValue + decreaseValue;

                            double currValue = lastValue + increaseValue - decreaseValue;

                            double deviationValue = increaseValue == 0 ? -decreaseValue : increaseValue;
                            double deviationPercent = currValue == 0 ? 0 : deviationValue/lastValue;
                            double limit = GetSubjectLimit(limitDictionary, serieName);

                            string limitString = String.Empty;
                            if (limit > 0)
                            {
                                limitString = currValue > limit
                                                  ? String.Format(
                                                      "\n<b><span style='color:red;'>Превышен</span></b>&nbsp;порог Минэнерго России\n31.12.2011:&nbsp;<b>{0:N2}</b>&nbsp;руб.",
                                                      limit)
                                                  : String.Format(
                                                      "\nПорог Минэнерго России &nbsp;\n31.12.2011:&nbsp;<b>{0:N2}</b>&nbsp;руб.",
                                                      limit);
                            }

                            string devaitonString = deviationValue >= 0 ? "Рост" : "Снижение";

                            string growString = deviationValue == 0
                                                    ? String.Format("Цена не изменилась")
                                                    : String.Format(
                                                        "{2}&nbsp;<b>{0:N2}</b>&nbsp;руб. (<b>{3}{1:P2}</b>)",
                                                        deviationValue, deviationPercent, devaitonString,
                                                        deviationValue >= 0 ? "+" : String.Empty);

                            box.DataPoint.Label =
                                String.Format(
                                    "{0}\n{1}\n{2:dd.MM.yyyy}:&nbsp;<b>{3:N2}</b>&nbsp;руб.\n{4:dd.MM.yyyy}:&nbsp;<b>{5:N2}</b>&nbsp;руб.\n{6}{7}",
                                    serieName, oilName, lastDate, lastValue, reportDate, currValue, growString,
                                    limitString);

                            if (box.Column == 0)
                            {
                                int textWidth = 160;

                                Text text = new Text();
                                text.bounds = new Rectangle(Convert.ToInt32(xMax - textWidth), box.rect.Top, textWidth,
                                                            box.rect.Height);
                                text.labelStyle.Font = new Font("Verdana", 9);
                                text.labelStyle.FontColor = Color.Black;

                                if (deviationValue == 0)
                                {
                                    text.SetTextString("Цена не изменилась");
                                }
                                else
                                {
                                    text.SetTextString(String.Format("{2}{0:N2} руб. ({1:P2})", deviationValue,
                                                                     deviationPercent,
                                                                     deviationValue >= 0 ? "+" : String.Empty));
                                }

                                e.SceneGraph.Add(text);

                                if (limit != 0 && currValue > limit)
                                {
                                    Ellipse ellipse =
                                        new Ellipse(new Point((int) xMax - 10, box.rect.Top + box.rect.Height/2), 10);
                                    ellipse.PE.Fill = Color.Red;
                                    e.SceneGraph.Add(ellipse);
                                }
                            }
                        }
                    }
                }
            }

            if (avg != Double.MinValue)
            {
                int textWidth = 130;
                int textHeight = 30;

                Line line = new Line();
                line.p1 = new Point((int) xAxis.Map(avg), (int) yMin + textHeight);
                line.p2 = new Point((int) xAxis.Map(avg), (int) yMax);
                line.PE.Stroke = Color.Gray;
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                e.SceneGraph.Add(line);

                Text text = new Text();
                text.bounds = new Rectangle(Convert.ToInt32(xAxis.Map(avg) - textWidth*0.5), (int) yMin + textHeight - 5,
                                            textWidth, textHeight);
                text.labelStyle.Font = new Font("Verdana", 9);
                text.labelStyle.FontColor = Color.Black;
                text.labelStyle.HorizontalAlign = StringAlignment.Center;
                text.SetTextString(String.Format("Среднее по {1}\n{0:N2} руб.", avg, shortFO));
                e.SceneGraph.Add(text);
            }
        }

        private double GetSubjectLimit(Dictionary<string, double> limitDictionary, string name)
        {
            foreach (string key in limitDictionary.Keys)
            {
                if (name.Contains(key))
                {
                    return limitDictionary[key];
                }
            }

            return Double.MaxValue;
        }

        private static double GetRowValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return Convert.ToDouble(row[columnName]);
                }
            }

            return Double.MinValue;
        }

        private static string GetRowStringValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return row[columnName].ToString();
                }
            }

            return String.Empty;
        }

        #endregion
    }
}