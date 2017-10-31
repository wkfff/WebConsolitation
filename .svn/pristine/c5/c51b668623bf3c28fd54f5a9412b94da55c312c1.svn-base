using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0013 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
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

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart1.Width = 750;
            UltraChart1.Height = 780;
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

            query = DataProvider.GetQueryText("data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtData);
                        
            Label1.Text = String.Format("<span style=\"color: White\"><b>{0:dd.MM.yyyy}г.</b></span>&nbsp;поступили:<br/>", date);

            SetLabelText(Label3, dtData.Rows[0][1]);
            SetLabelText(Label4, dtData.Rows[1][1]);
            SetLabelText(Label5, dtData.Rows[2][1]);
            SetLabelText(Label6, dtData.Rows[3][1]);
            SetLabelText(Label7, dtData.Rows[4][1]);
         
            // Получаем последнюю дату
            lastDataDate = GetLastDataDate();
            Label2.Text = String.Format("Кассовые поступления на&nbsp;<span style=\"color: White\"><b>{0:dd.MM.yyyy}г.</b></span>&nbsp;по областному бюджету&nbsp;исполнены на&nbsp;<span style=\"color: White\"><b>{1:P2}</b></span><br/><nobr>&nbsp;&nbsp;&nbsp;&nbsp;(план&nbsp;<span style=\"color: White\"><b>{2:N2}</b></span>&nbsp;тыс.руб.;&nbsp;факт&nbsp;<span style=\"color: White\"><b>{3:N2}</b>&nbsp;тыс.руб.</span>)</nobr>", lastDataDate, Convert.ToDouble(dtData.Rows[5][3]) / Convert.ToDouble(dtData.Rows[5][2]), Convert.ToDouble(dtData.Rows[5][2]) / 1000, Convert.ToDouble(dtData.Rows[5][3]) / 1000);
            UserParams.CubeName.Value = "[ФО_Исполнение кассового плана]";

            // Инициализируем параметры даты из последней даты
            UserParams.PeriodQuater.Value = CRHelper.PeriodMemberUName(String.Empty, lastDataDate, 3);

            SetStackChartAppearanceUnic();
            BindChartData("ChartOutcomesAll", 6);
        }

        private void SetLabelText(Label label, object cell)
        {
            if (cell.ToString() != "0")
            {
                label.Text = String.Format(label.Text, String.Format("&nbsp;<span style=\"color: White\"><b>{0:N2}</b></span>&nbsp;", cell)) + "<br/>";
            }
            else
            {
                label.Visible = false;
            }
        }

        private void SetStackChartAppearanceUnic()
        {
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;
            
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /><b><DATA_VALUE:N0></b>тыс.руб.</span>";
            SetLabelsClipTextBehavior(UltraChart1.Axis.X.Labels.SeriesLabels.Layout);
            
            UltraChart1.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart1.Axis.X.TickmarkInterval = 50;
            //UltraChart1.Axis.X.TickmarkIntervalType = AxisIntervalType.Months;

            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;

            UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Extent = 80;
            UltraChart1.Axis.X.Extent = 110;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 350;
            UltraChart1.Legend.Font = new Font("Verdana", 10);
            UltraChart1.Legend.SpanPercentage = 13;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 10; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
        }

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //Text text = e.Primitive as Text;
            //if (text != null)
            //{
            //    text.SetTextString("42");
            //    text.Visible = true;
            //}

        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            ReplaceAxisLabels(e.SceneGraph);
        }

        private DateTime GetLastDataDate()
        {
            string query = DataProvider.GetQueryText("date_last");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(
                query, dtDate);
            return new DateTime(
                Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                Convert.ToInt32(dtDate.Rows[0][4].ToString()));
        }

        private void BindChartData(string queryId, int monthColumnIndex)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryId);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtChart);
            RemoveRedudantRows(dtChart, monthColumnIndex);
            SplitColumnNames();
            SeriesToUpperFirstSymbol();
            dtChart.AcceptChanges();
            UltraChart1.DataSource = dtChart;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.DataBind();
        }

        private string axisMonth = String.Empty;

        private int labelVisible = 0;

        private int startLabelPos = 110;
        private int labelStep = 27;
        private int labelCount = 0;

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
                            if (labelVisible == 0)
                            {
                                if (axisMonth == textArray[1])
                                {
                                    ((Text)primitive).SetTextString(day.ToString());
                                }
                                else
                                {
                                    ((Text)primitive).SetTextString(
                                        String.Format("{0}-{1}",
                                                      CRHelper.ToUpperFirstSymbol(
                                                          CRHelper.RusMonth(CRHelper.MonthNum(textArray[1]))), day));
                                    axisMonth = textArray[1];
                                }
                                if (((Text)primitive).bounds.X == 0)
                                {
                                    ((Text)primitive).bounds.X = startLabelPos + labelStep * labelCount;
                                    ((Text)primitive).bounds.Y = 691;
                                    ((Text)primitive).bounds.Width = 12;
                                    ((Text)primitive).bounds.Height = 110;
                                    ((Text)primitive).labelStyle.HorizontalAlign = StringAlignment.Far;
                                }
                                labelCount++;
                            }
                            else
                            {
                                ((Text)primitive).Visible = false;
                            }
                            labelVisible++;
                            if (labelVisible == 4)
                            {
                                labelVisible = 0;
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
                }
            }
            // Поудаляем дни, которые не дни.
            foreach (int index in removingRows)
            {
                dtSource.Rows.RemoveAt(index);
            }
            dtSource.AcceptChanges();
        }


        private void SeriesToUpperFirstSymbol()
        {
            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName = CRHelper.ToUpperFirstSymbol(col.ColumnName);
            }
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

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(110, 189, 241);
                    }
                case 2:
                    {
                        return Color.FromArgb(214, 171, 133);
                    }
                case 3:
                    {
                        return Color.FromArgb(141, 178, 105);
                    }
                case 4:
                    {
                        return Color.FromArgb(192, 178, 224);
                    }
                case 5:
                    {
                        return Color.FromArgb(245, 187, 102);
                    }
                case 6:
                    {
                        return Color.FromArgb(142, 164, 236);
                    }
                case 7:
                    {
                        return Color.FromArgb(217, 230, 117);
                    }
                case 8:
                    {
                        return Color.FromArgb(162, 154, 98);
                    }
                case 9:
                    {
                        return Color.FromArgb(143, 199, 219);
                    }
                case 10:
                    {
                        return Color.FromArgb(176, 217, 117);
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
                        return Color.FromArgb(9, 135, 214);
                    }
                case 2:
                    {
                        return Color.FromArgb(138, 71, 10);
                    }
                case 3:
                    {
                        return Color.FromArgb(65, 124, 9);
                    }
                case 4:
                    {
                        return Color.FromArgb(44, 20, 91);
                    }
                case 5:
                    {
                        return Color.FromArgb(229, 140, 13);
                    }
                case 6:
                    {
                        return Color.FromArgb(11, 45, 160);
                    }
                case 7:
                    {
                        return Color.FromArgb(164, 184, 10);
                    }
                case 8:
                    {
                        return Color.FromArgb(110, 98, 8);
                    }
                case 9:
                    {
                        return Color.FromArgb(11, 100, 131);
                    }
                case 10:
                    {
                        return Color.FromArgb(102, 168, 9);
                    }
            }
            return Color.White;
        }

    }
}
