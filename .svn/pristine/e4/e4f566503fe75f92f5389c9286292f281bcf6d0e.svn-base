using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core;
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
    public partial class FO_0035_0006 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private int lastDataElementIndex = 0;
        private DateTime lastDataDate;
                
        private DataTable dtData = new DataTable();
       
        private DateTime date;

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;
        // мера Остаток на начало
        private CustomParam measureStartBalance;
        // мера Остаток на конец
        private CustomParam measureEndBalance;

        #endregion

        public bool IsQuaterPlanType
        {
            get
            {
                return RegionSettingsHelper.Instance.CashPlanType.ToLower() == "quarter";
            }
        }

        public bool IsYar
        {
            get
            {
                return RegionSettingsHelper.Instance.Name.ToLower() == "ярославская область";
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart1.Width = 1010;
            UltraChart1.Height = 600;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Инициализация параметров запроса

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");
            measureStartBalance = UserParams.CustomParam("measure_start_balance");
            measureEndBalance = UserParams.CustomParam("measure_end_balance");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][5].ToString();


            if (!dtDate.Rows[0][4].ToString().Contains("Заключительные обороты"))
            {
                date = new DateTime(
                   Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                   CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                   Convert.ToInt32(dtDate.Rows[0][4].ToString()));
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            }

            if (IsQuaterPlanType)
            {
                measurePlan.Value = "План";
                measureFact.Value = "Факт";
            }
            else
            {
                measurePlan.Value = "План_Нарастающий итог";
                measureFact.Value = "Факт_Нарастающий итог";
            }

            measureStartBalance.Value = (IsYar) ? "Остаток средств на начало квартала" : RegionSettingsHelper.Instance.CashPlanBalance;
            measureEndBalance.Value = (measureStartBalance.Value == "Остаток средств")
                                          ? measureStartBalance.Value
                                          : "Остаток средств на конец квартала";

            query = DataProvider.GetQueryText("data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtData);

            double value;

            Label11.Visible = !IsQuaterPlanType;
            Label13.Visible = !IsQuaterPlanType;
            lbRestStartFact.Visible = !IsQuaterPlanType;

            if (measureStartBalance.Value == "Остаток средств")
            {
                Label1.Text = "Остаток средств";
            }
            else
            {
                Label1.Text = "Остаток средств на начало квартала&nbsp;";

            }
            Label2.Text = String.Format("Остаток средств на&nbsp;<span style=\"color: White\"><b>{0:dd.MM.yyyy}г.</b></span> по областному бюджету&nbsp;", date);
            if (Double.TryParse(dtData.Rows[0][1].ToString(), out value))
            {
                lbRestStartPlan.Text = (value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[0][2].ToString(), out value))
            {
                lbRestStartFact.Text = (value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[1][2].ToString(), out value))
            {
                lbRestEndFact.Text = (value / 1000).ToString("N0");
            }

            // Получаем последнюю дату
            lastDataDate = GetLastDataDate();

            UserParams.CubeName.Value = "[ФО_Исполнение кассового плана]";

            // Инициализируем параметры даты из последней даты
            UserParams.PeriodQuater.Value =
                CRHelper.PeriodMemberUName(String.Empty, lastDataDate, 3);

            // Выставляем параметры.
            SetRestsParams();
            // биндим данные.
            UltraChart1.DataBind();
        }

        private DateTime GetLastDataDate()
        {
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(
                query, dtDate);
            return new DateTime(
                Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                Convert.ToInt32(dtDate.Rows[0][4].ToString()));
        }

        protected void ultraChart_FillSceneGraphRests(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;
            Polyline lastLine = null;
            Polyline previousLine = null;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive pr = e.SceneGraph[i];
                if (pr is Polyline)
                {
                    Polyline line = ((Polyline)pr);
                    previousLine = lastLine;
                    lastLine = ((Polyline)pr);
                    line.Visible = line.points[0].point.X < line.points[line.points.Length - 1].point.X;
                    line.splineTension = 0.2f;
                }
            }
            if (previousLine != null)
            {
                previousLine.Visible = false;
                previousLine.points[1].Visible = false;
                previousLine.points[1].hitTestRadius = 0;
                previousLine.points[0].Visible = true;
                previousLine.points[0].hitTestRadius = 6;
            }
            double value;
            double.TryParse(dtChart.Rows[0][2].ToString(), out value);
            Box box = new Box(new Rectangle(
                (int)xAxis.Map(0) - 6, (int)yAxis.Map(value) - 6, 13, 13));

            box.PE.ElementType = PaintElementType.Gradient;
            box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
            box.PE.Fill = Color.FromArgb(101, 162, 203);
            box.PE.FillStopColor = Color.FromArgb(8, 106, 172);
            box.PE.Stroke = Color.Black;
            box.PE.StrokeWidth = 1;
            box.Row = 0;
            box.Column = 2;
            box.Value = 42;
            box.Layer = e.ChartCore.GetChartLayer();
            box.Chart = UltraChart1.ChartType;
            e.SceneGraph.Add(box);

            Color restsFillColor = Color.Empty;
            Color restsFillStopColor = Color.Empty;
            int restEndIndex = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                    }

                    PointSet tooltip = (PointSet)primitive;
                    if (tooltip.DataPoint != null)
                    {
                        tooltip.DataPoint.Label = "1 июля";
                    }
                }

                if (primitive.Path != null && primitive.Path.Contains("Legend") && primitive is Text)
                {
                    string legendText = ((Text)primitive).GetTextString();
                    Primitive prevPrimitive = e.SceneGraph[i - 1];
                    if (legendText == "Остаток ")
                    {
                        prevPrimitive.PE.Fill = Color.FromArgb(101, 162, 203);
                        prevPrimitive.PE.FillStopColor = Color.FromArgb(8, 106, 172);
                        ((Text)primitive).SetTextString("Остаток на начало квартала");
                    }
                    else if (legendText == "Остаток на начало квартала")
                    {
                        prevPrimitive.PE.Fill = Color.Lime;
                        prevPrimitive.PE.FillOpacity = 150;
                        prevPrimitive.PE.FillStopColor = Color.Lime;
                        ((Text)primitive).SetTextString("Остаток ");
                    }
                }
            }
            ReplaceAxisLabels(e.SceneGraph);
        }

        private string axisMonth = String.Empty;

        private void ReplaceAxisLabels(SceneGraph grahp)
        {
            for (int i = 0; i < grahp.Count; i++)
            {
                Primitive primitive = grahp[i];
                if (primitive is Text)
                {
                    string text = ((Text)primitive).GetTextString();
                    text = text.Trim();
                    // Проверяем формат
                    string[] textArray = text.Split();
                    if (textArray.Length == 2)
                    {
                        int day;
                        if (Int32.TryParse(textArray[0], out day) && CRHelper.IsMonthCaption(textArray[1]))
                        {
                            if (axisMonth == textArray[1])
                            {
                                ((Text)primitive).SetTextString(day.ToString());
                            }
                            else
                            {
                                ((Text)primitive).SetTextString(String.Format("{0}-{1}",
                                              CRHelper.ToUpperFirstSymbol(
                                                  CRHelper.RusMonth(CRHelper.MonthNum(textArray[1]))), day));
                                axisMonth = textArray[1];
                            }
                        }
                    }
                }
            }
        }

        private void RemoveRedudantRows(DataTable dtSource, int monthColumnIndex)
        {
            string lastDataMonth = CRHelper.RusMonth(lastDataDate.Month);
            int lastDataDay = lastDataDate.Day;
            List<int> removingRows = new List<int>();
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                int day;
                // Если день это не день
                if (!Int32.TryParse(dtSource.Rows[i][0].ToString(), out day))
                {
                    removingRows.Add(i - removingRows.Count);
                }
                else
                {
                    int monthNum = CRHelper.MonthNum(dtSource.Rows[i][monthColumnIndex].ToString());
                    string monthGenitive = CRHelper.RusMonthGenitive(monthNum);
                    string month = CRHelper.RusMonth(monthNum);
                    // Допишем месяц
                    dtSource.Rows[i][0] = String.Format("{0} {1}", dtSource.Rows[i][0], monthGenitive);
                    if (lastDataDay == day &&
                        lastDataMonth.ToLower() == month.ToLower())
                    {
                        lastDataElementIndex = i - removingRows.Count;
                    }
                }
            }
            // Поудаляем дни, которые не дни.
            foreach (int index in removingRows)
            {
                dtSource.Rows.RemoveAt(index);
            }
            dtSource.AcceptChanges();
        }

        private void SetRestsParams()
        {
            UserParams.CubeName.Value = "[ФО_Исполнение кассового плана";
            UltraChart1.DataBinding += new EventHandler(ultraChart_DataBindingRests);
            //  UltraChart1.InterpolateValues += new InterpolateValuesEventHandler(ultraChart_InterpolateValues);
            UltraChart1.AreaChart.NullHandling = NullHandling.DontPlot;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(ultraChart_FillSceneGraphRests);
        }

        private void SeriesToUpperFirstSymbol()
        {
            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName = CRHelper.ToUpperFirstSymbol(col.ColumnName);
            }
        }

        private void ultraChart_DataBindingRests(object sender, EventArgs e)
        {
            SetAreaChartAppearance();
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("ChartRests");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtChart);
            DataTable dtCopy = dtChart.Copy();
            DataColumn monthColumn = dtChart.Columns[2];
            dtChart.Columns.RemoveAt(2);
            DataColumn col = new DataColumn("Остаток на начало квартала", typeof(double));
            dtChart.Columns.Add(col);
            dtChart.Columns.Add(monthColumn);
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                dtChart.Rows[i][3] = dtCopy.Rows[i][2];
            }
            RemoveRedudantRows(dtChart, 3);
            SplitColumnNames();
            SeriesToUpperFirstSymbol();
            double valueRest = 0;
            int rowNum = 0;
            while (valueRest == 0 && rowNum < dtChart.Rows.Count)
            {
                if (double.TryParse(dtChart.Rows[rowNum][1].ToString(), out valueRest))
                {
                    dtChart.Rows[0][2] = valueRest;
                }
                rowNum++;
            }

            object value;
            value = dtChart.Rows[0][2];
            for (int i = 0; i < lastDataElementIndex; i++)
            {
                if (dtChart.Rows[i][1] == DBNull.Value)
                {
                    dtChart.Rows[i][1] = value;
                }
                value = dtChart.Rows[i][1];
            }

            UltraChart1.DataSource = dtChart;
            UltraChart1.Data.SwapRowsAndColumns = true;
        }

        private void SetAreaChartAppearance()
        {
            UltraChart1.ChartType = ChartType.LineChart;

            AddLineAppearence1(UltraChart1);
            AddLineAppearence2(UltraChart1);

            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.LineChart.NullHandling = NullHandling.DontPlot;

            EmptyAppearance item = new EmptyAppearance();
            item.EnableLineStyle = true;
            LineStyle style = new LineStyle();
            style.DrawStyle = LineDrawStyle.Dot;
            item.EnablePoint = false;

            item.LineStyle = style;
            UltraChart1.LineChart.EmptyStyles.Add(item);

            UltraChart1.Legend.SpanPercentage = 7;
            //  UltraChart1.Legend.Margins.Right = (int)UltraChart1.Width.Value / 2;
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /><b><DATA_VALUE:N0></b>тыс.руб.</span>";
            UltraChart1.Axis.X.Labels.SeriesLabels.FormatString = "<SERIES_LABEL>";
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;

            UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.Axis.X.Extent = 90;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.FontColor = Color.White;

            UltraChart1.Legend.Font = new Font("Verdana", 10);
        }

        private void AddLineAppearence1(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();

            PaintElement pe = new PaintElement();

            pe.Fill = Color.Lime;
            pe.FillStopColor = Color.Lime;
            pe.ElementType = PaintElementType.SolidFill;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = 150;
            pe.StrokeWidth = 4;
            chart.ColorModel.Skin.PEs.Add(pe);

            LineAppearance lineAppearance2 = new LineAppearance();
            lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance2.IconAppearance.PE = pe;
            lineAppearance2.Thickness = 5;

            chart.LineChart.LineAppearances.Add(lineAppearance2);
        }

        private void AddLineAppearence2(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;

            PaintElement pe = new PaintElement();

            pe.Fill = Color.Red;
            pe.FillStopColor = Color.Red;
            pe.ElementType = PaintElementType.SolidFill;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = 0;
            pe.StrokeWidth = 0;
            chart.ColorModel.Skin.PEs.Add(pe);

            LineAppearance lineAppearance2 = new LineAppearance();
            lineAppearance2.IconAppearance.Icon = SymbolIcon.None;
            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance2.IconAppearance.PE = pe;
            lineAppearance2.Thickness = 0;

            chart.LineChart.LineAppearances.Add(lineAppearance2);
        }

        private void SetLabelsClipTextBehavior(AxisLabelLayoutAppearance layout)
        {
            layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.ClipText = false;
            behavior.Enabled = true;
            behavior.Trimming = StringTrimming.None;
            behavior.UseOnlyToPreventCollisions = false;
            layout.BehaviorCollection.Add(behavior);
        }

        private void SplitColumnNames()
        {
            foreach (DataColumn col in dtChart.Columns)
            {
                string[] names = col.ColumnName.Split(';');
                if (names.Length == 2)
                {
                    col.ColumnName = names[0];
                }
            }
        }
    }
}
