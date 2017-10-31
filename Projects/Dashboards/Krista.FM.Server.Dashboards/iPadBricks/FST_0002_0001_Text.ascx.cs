using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FST_0002_0001_Text : UserControl
    {
        #region Поля и свойства

        public int ChartWidth = 750;
        public int ChartHeight = 235;

        DataTable dtChartIncrease;
        DataTable dtChartDecrease;
        private DataTable dt;

        private double MinRange = 10;
        private double MaxRange = 50;

        private const int chartBarHeight = 90;
        private const int chartBarLimit = 40;
        private const double chartDeviationLimit = 0.2;

        private string reportCode;
        public string ReportCode
        {
            get
            {
                return reportCode;
            }
            set
            {
                reportCode = value; 
            }
        }

        private string detalizationReportCode;
        public string DetalizationReportCode
        {
            get
            {
                return detalizationReportCode;
            }
            set
            {
                detalizationReportCode = value;
            }
        }

        private string reportPrefix;
        public string ReportPrefix
        {
            get
            {
                return reportPrefix;
            }
            set
            {
                reportPrefix = value;
            }
        }

        public int ControlHeight
        {
            get
            {
                int incHeight = UltraChartIncrease.Visible ? Convert.ToInt32(UltraChartIncrease.Height.Value) : 0;
                int decHeight = UltraChartDescrease.Visible ? Convert.ToInt32(UltraChartDescrease.Height.Value) : 0;    

                return incHeight + decHeight + 230;
            }
        }

        private DateTime reportDate;
        public DateTime ReportDate
        {
            get
            {
                return reportDate;
            }
            set
            {
                reportDate = value;
            }
        }

        private DateTime lastDate;
        public DateTime LastDate
        {
            get
            {
                return lastDate;
            }
            set
            {
                lastDate = value;
            }
        }

        private string serviceName = "Водоснабжение";
        public string ServiceName
        {
            get
            {
                return serviceName;
            }
            set
            {
                serviceName = value;
            }
        }

        private string subjectId = "2";
        public string SubjectId
        {
            get
            {
                return subjectId;
            }
            set
            {
                subjectId = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            IncomesHeader.Text = serviceName;
            IncomesHeader.MultitouchReport = String.Format("{0}_{1}", detalizationReportCode, subjectId);
            
            CRHelper.SaveToErrorLog(IncomesHeader.MultitouchReport);

            SetBarChart(UltraChartIncrease, Color.Red);
            SetBarChart(UltraChartDescrease, Color.Green);

            CustomParam.CustomParamFactory("service_name").Value = serviceName;

            MinRange = Double.MaxValue;
            MaxRange = Double.MinValue;

            if (CustomParam.CustomParamFactory("state_area").Value == "Ханты-Мансийский автономный округ")
            {
                CustomParam.CustomParamFactory("state_area").Value = "Ханты-Мансийский автономный округ - Югра";
            }

            ChartDataBindIncrease(UltraChartIncrease, "Increase", IncreaseLabel);
            ChartDataBindDecrease(UltraChartDescrease, "Decrease", DecreaseLabel);

            UltraChartIncrease.Axis.X.RangeMax = 1.3 * MaxRange;
            UltraChartIncrease.Axis.X.RangeMin = 0.7 * MinRange;

            UltraChartDescrease.Axis.X.RangeMax = 1.3 * MaxRange;
            UltraChartDescrease.Axis.X.RangeMin = 0.7 * MinRange;
        }

        private void SetBarChart(UltraChart chart, Color deviationColor)
        {
            chart.ChartType = ChartType.StackBarChart;

            chart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartIncrease_FillSceneGraph);

            chart.Data.SwapRowsAndColumns = true;

            chart.Axis.Y.Extent = 300;
            chart.Axis.Y.Labels.SeriesLabels.Visible = true;
            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;

            chart.Axis.X.RangeType = AxisRangeType.Custom;
//            chart.Axis.X.RangeMax = axisXMaxRange;
//            chart.Axis.X.RangeMin = axisXMinRange;
            chart.Axis.X.Extent = 30;

            chart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Labels.Visible = true;

            chart.Data.ZeroAligned = false;
            chart.Legend.Visible = false;

            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL></span>";

            chart.Axis.X.MajorGridLines.Visible = false;
            chart.TitleBottom.Visible = true;
            chart.TitleBottom.Text = "Руб.";
            chart.TitleBottom.Font = new Font("Verdana", 8);
            chart.TitleBottom.FontColor = Color.FromArgb(209, 209, 209);
            chart.TitleBottom.HorizontalAlign = StringAlignment.Far;
            chart.TitleBottom.Margins.Left = UltraChartIncrease.Axis.Y.Extent;

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LightBlue;
            Color color2 = deviationColor;

            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            chart.ColorModel.Skin.ApplyRowWise = false;

            chart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            chart.Effects.Enabled = true;
            chart.Effects.Effects.Add(effect);
        }

        private void ChartDataBindIncrease(UltraChart chart, string queryPostfix, Label label)
        {
            string query = DataProvider.GetQueryText(ReportCode + "_chart" + "_" + queryPostfix);
            dtChartIncrease = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtChartIncrease);
            
            string lastMonthString = String.Format("{0} {1}", CRHelper.RusMonthTvorit(lastDate.Month), lastDate.Year);

            if (dtChartIncrease.Rows.Count > 0)
            {
                double max = Double.MinValue;
                double min = Double.MaxValue;

                if (dtChartIncrease.Columns.Count > 0)
                {
                    dtChartIncrease.Columns.RemoveAt(0);
                }

                foreach (DataRow row in dtChartIncrease.Rows)
                {

                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("\"'", "'").Replace(";", "\n").Replace("муниципальный район", "МР");
                    }

                    double lastValue = GetRowValue(row, 1);
                    double currValue = GetRowValue(row, 2);

                    lastValue = lastValue == Double.MinValue ? 0 : lastValue;
                    currValue = currValue == Double.MinValue ? 0 : currValue;

                    max = Math.Max(max, lastValue + currValue);
                    min = Math.Min(min, lastValue);


                }

                if (max != Double.MinValue)
                {
                    MaxRange = max;
                }

                if (min != Double.MaxValue)
                {
                    MinRange = min;
                }

                dt = new DataTable();
                bool limitEnable = CopyDt(dtChartIncrease, ref dt, chartBarLimit);

                string limitDescription = String.Empty;
                if (limitEnable)
                {
                    limitDescription = String.Format("(показано&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;из&nbsp;<span class='DigitsValue'>{1}</span>, полный список в детализации)",
                            chartBarLimit, dtChartIncrease.Rows.Count);
                }

                chart.Width = ChartWidth;
                chart.Height = dt.Rows.Count * chartBarHeight + 66;

                chart.Series.Clear();
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dt);
                    series.Label = dt.Columns[i].ColumnName;
                    chart.Series.Add(series);
                }

                chart.DataBind();
                chart.Visible = true;

                label.Text = String.Format("<span class='DigitsValue'>Рост тарифа</span>&nbsp;на&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;за&nbsp;<span class='DigitsValue'>{1} {2}</span>&nbsp;г. по сравнению с&nbsp;<span class='DigitsValue'>{4}</span>&nbsp;г. наблюдался в следующих муниципальных образованиях и организациях {3}",
                    serviceName.ToLower(), CRHelper.RusMonth(reportDate.Month).ToLower(), reportDate.Year, limitDescription, lastMonthString);
            }
            else
            {
                chart.Visible = false;
                label.Text = String.Format("<span class='DigitsValue'>Роста тарифа</span>&nbsp;на&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;за&nbsp;<span class='DigitsValue'>{1} {2}</span>&nbsp;г. по сравнению с&nbsp;<span class='DigitsValue'>{3}</span>&nbsp;г. не наблюдалось",
                    serviceName.ToLower(), CRHelper.RusMonth(reportDate.Month).ToLower(), reportDate.Year, lastMonthString);
            }
        }

//        double lastValue;
//        double deviationValue;
//        double deviationPercent;

        private bool CopyDt(DataTable source, ref DataTable dest, int rowCount)
        {
            dest = source.Clone();
            int limit = Math.Min(source.Rows.Count, rowCount);



            int i = 0;
            while (i < limit && i < source.Rows.Count)
            {
//                lastValue = GetRowValue(source.Rows[i], 1);
//                deviationValue = GetRowValue(source.Rows[i], 2);
//
//                lastValue = lastValue == Double.MinValue ? 0 : lastValue;
//                deviationValue = deviationValue == Double.MinValue ? 0 : deviationValue;
//                deviationPercent = lastValue == 0 ? 0 : deviationValue / lastValue;

//                if (deviationPercent <= chartDeviationLimit)
//                {
                    dest.ImportRow(source.Rows[i]);
                    i++;
//                }
            }
            dest.AcceptChanges();

            return rowCount < source.Rows.Count;
        }

        private void ChartDataBindDecrease(UltraChart chart, string queryPostfix, Label label)
        {
            string query = DataProvider.GetQueryText(ReportCode + "_chart" + "_" + queryPostfix);
            dtChartDecrease = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtChartDecrease);

            string lastMonthString = String.Format("{0} {1}", lastDate.Month == 12 ? "декабрем" : "ноябрем", lastDate.Year);

            if (dtChartDecrease.Rows.Count > 0)
            {
                double max = Double.MinValue;
                double min = Double.MaxValue;

                if (dtChartDecrease.Columns.Count > 0)
                {
                    dtChartDecrease.Columns.RemoveAt(0);
                }

                foreach (DataRow row in dtChartDecrease.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("\"'", "'").Replace(";", "\n").Replace("муниципальный район", "МР");
                    }

                    double lastValue = GetRowValue(row, 1);
                    double currValue = GetRowValue(row, 2);

                    lastValue = lastValue == Double.MinValue ? 0 : lastValue;
                    currValue = currValue == Double.MinValue ? 0 : currValue;

                    max = Math.Max(max, lastValue + currValue);
                    min = Math.Min(min, lastValue);
                }

                if (max != Double.MinValue)
                {
                    MaxRange = Math.Max(MaxRange, max);
                }

                if (min != Double.MaxValue)
                {
                    MinRange = Math.Min(MinRange, min);
                }

                dt = new DataTable();
                bool limitEnable = CopyDt(dtChartDecrease, ref dt, 5);

                string limitDescription = String.Empty;
                if (limitEnable)
                {
                    limitDescription = String.Format("(показано&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;из&nbsp;<span class='DigitsValue'>{1}</span>, полный список в детализации)",
                            5, dtChartDecrease.Rows.Count);
                }

                chart.Width = ChartWidth;
                chart.Height = dt.Rows.Count * chartBarHeight + 66;

                chart.Series.Clear();
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dt);
                    series.Label = dt.Columns[i].ColumnName;
                    chart.Series.Add(series);
                }

                chart.DataBind();
                chart.Visible = true;
                label.Text = String.Format("<span class='DigitsValue'>Снижение тарифа</span>&nbsp;на&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;за&nbsp;<span class='DigitsValue'>{1} {2}</span>&nbsp;г. по сравнению с&nbsp;<span class='DigitsValue'>{4}</span>&nbsp;г. наблюдалось в следующих муниципальных образованиях и организациях {3}",
                    serviceName.ToLower(), CRHelper.RusMonth(reportDate.Month).ToLower(), reportDate.Year, limitDescription, lastMonthString);
            }
            else
            {
                chart.Visible = false;
                label.Text = String.Format("<span class='DigitsValue'>Снижения тарифа</span>&nbsp;на&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;за&nbsp;<span class='DigitsValue'>{1} {2}</span>&nbsp;г. по сравнению с&nbsp;<span class='DigitsValue'>{3}</span>&nbsp;г. не наблюдалось",
                    serviceName.ToLower(), CRHelper.RusMonth(reportDate.Month).ToLower(), reportDate.Year, lastMonthString);
            }
        }

        protected void UltraChartIncrease_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];

            if (xAxis == null)
            {
                return;
            }

            double xMax = xAxis.MapMaximum;
            int deviationTextWidth = 185;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text)primitive;
                    if (text.Row == -1)
                    {
                        text.bounds = new Rectangle(5, text.bounds.Y - chartBarHeight / 4, ((UltraChart)sender).Axis.Y.Extent, chartBarHeight);
                        text.labelStyle.HorizontalAlign = StringAlignment.Far;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                        text.labelStyle.WrapText = true;
                    }
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        dt = (UltraChart) sender == UltraChartIncrease ? dtChartIncrease : dtChartDecrease;

                        if (dt == null) return;

                        string serieName = GetRowStringValue(dt.Rows[box.Row], 0).Replace("\"", "'");
                        serieName = GetWrappedText(serieName, 35);
                        double lastValue = GetRowValue(dt.Rows[box.Row], 1);
                        double increaseValue = GetRowValue(dt.Rows[box.Row], 2);

                        if (lastValue != Double.MinValue && increaseValue != Double.MinValue)
                        {
                            string currPeriod = String.Format("{0} {1} г.", CRHelper.RusMonth(reportDate.Month).ToLower(), reportDate.Year);
                            string lastPeriod = String.Format("{0} {1} г.", CRHelper.RusMonth(lastDate.Month).ToLower(), lastDate.Year);

                            if ((UltraChart)sender == UltraChartIncrease)
                            {
                                double currValue = lastValue + increaseValue;
                                double increasePercent = currValue == 0 ? 0 : increaseValue/lastValue;

                                box.DataPoint.Label =
                                    String.Format(
                                        "{0}\n{1}\n{2}:&nbsp;<b>{3:N2}</b>&nbsp;руб.\n{4}:&nbsp;<b>{5:N2}</b>&nbsp;руб.\nРост&nbsp;<b>{6:N2}</b>&nbsp;руб. (<b>+{7:P2}</b>)",
                                        serieName, serviceName.ToLower(), lastPeriod, lastValue, currPeriod, currValue,
                                        increaseValue, increasePercent);

                                if (box.Column == 0)
                                {
                                    Text text = new Text();
                                    text.bounds = new Rectangle(Convert.ToInt32(xMax - deviationTextWidth), box.rect.Top,
                                                                deviationTextWidth, box.rect.Height);
                                    text.labelStyle.Font = new Font("Verdana", 9);
                                    text.labelStyle.FontColor = Color.White;
                                    text.SetTextString(String.Format("+{0:N2} руб. ({1:P2})", increaseValue,
                                                                     increasePercent));
                                    text.labelStyle.HorizontalAlign = StringAlignment.Far;
                                    e.SceneGraph.Add(text);
                                }
                            }
                            else
                            {
                                lastValue = lastValue + increaseValue;

                                double currValue = lastValue - increaseValue;
                                increaseValue = -increaseValue;
                                double increasePercent = currValue == 0 ? 0 : increaseValue / lastValue;

                                box.DataPoint.Label =
                                    String.Format(
                                        "{0}\n{1}\n{2}:&nbsp;<b>{3:N2}</b>&nbsp;руб.\n{4}:&nbsp;<b>{5:N2}</b>&nbsp;руб.\nСнижение&nbsp;<b>{6:N2}</b>&nbsp;руб. (<b>{7:P2}</b>)",
                                        serieName, serviceName.ToLower(), lastPeriod, lastValue, currPeriod, currValue,
                                        increaseValue, increasePercent);

                                if (box.Column == 0)
                                {
                                    Text text = new Text();
                                    text.bounds = new Rectangle(Convert.ToInt32(xMax - deviationTextWidth), box.rect.Top, deviationTextWidth, box.rect.Height);
                                    text.labelStyle.Font = new Font("Verdana", 9);
                                    text.labelStyle.FontColor = Color.White;
                                    text.SetTextString(String.Format("{0:N2} руб. ({1:P2})", increaseValue,
                                                                     increasePercent));
                                    text.labelStyle.HorizontalAlign = StringAlignment.Far;
                                    e.SceneGraph.Add(text);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetWrappedText(string text, int wrapLength)
        {
            string resultText = String.Empty;

            int rowLength = 0;
            foreach (Char ch in text)
            {
                if (ch == '\n')
                {
                    rowLength = 0;
                }
                {
                    if (rowLength > wrapLength && Char.IsWhiteSpace(ch))
                    {
                        resultText += Environment.NewLine;
                        rowLength = 0;
                    }
                    else
                    {
                        resultText += ch;
                    }

                    rowLength++;
                }
            }

            return resultText;
        }

        private static double GetRowValue(DataRow row, int index)
        {
            if (row.Table.Columns.Count > index)
            {
                if (row[index] != DBNull.Value && row[index].ToString() != String.Empty)
                {
                    return Convert.ToDouble(row[index]);
                }
            }

            return Double.MinValue;
        }

        private static string GetRowStringValue(DataRow row, int index)
        {
            if (row.Table.Columns.Count > index)
            {
                if (row[index] != DBNull.Value && row[index].ToString() != String.Empty)
                {
                    return row[index].ToString();
                }
            }

            return String.Empty;
        }
    }
}