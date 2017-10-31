using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0001
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtIndicators = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private double normValue = 0;


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 150);
            UltraWebGrid.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.75 - 15);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 33);

            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Axis.X.Extent = 100;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N2>";

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel.AddRefreshTarget(indTitleLabel);
                chartWebAsyncPanel.AddRefreshTarget(indNameLabel);
                chartWebAsyncPanel.AddRefreshTarget(indContentLabel);
                chartWebAsyncPanel.AddRefreshTarget(indFormulaLabel);
                chartWebAsyncPanel.AddRefreshTarget(indNormValueLabel);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 100;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(true));
                ComboQuarter.SetСheckedState(baseQuarter, true);

                UserParams.SelectItem.Value = "Группа МО";
            }

            Page.Title = "Результаты мониторинга БК и КУ в разрезе районов";
            Label1.Text = Page.Title;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum;
            if (ComboQuarter.SelectedIndex == 0)
            {
                quarterNum = 0;
                Label2.Text = string.Format("за {0} год", yearNum);
            }
            else
            {
                quarterNum = ComboQuarter.SelectedIndex;
                Label2.Text = string.Format("за {0} квартал {1} года", quarterNum, yearNum);
            }
            
            string yearValue = ComboYear.SelectedValue;
            string quarterValue = ComboQuarter.SelectedValue;

            if (!Page.IsPostBack || !UserParams.PeriodYear.ValueIs(yearValue) || !UserParams.PeriodQuater.ValueIs(quarterValue))
            {
                UserParams.PeriodYear.Value = yearValue;
                UserParams.PeriodQuater.Value = quarterValue;
                
                if (quarterNum == 0)
                {
                    UserParams.PeriodDayFO.Value = string.Format("[{0}]", yearValue);
                }
                else
                {
                    string halfYear = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
                    string quarter = string.Format("Квартал {0}", quarterNum);
                    UserParams.PeriodDayFO.Value = string.Format("[{0}].[{1}].[{2}]", yearValue, halfYear, quarter);
                }

                UltraWebGrid.DataBind();

//                int defaultRowIndex = 1;
//                if (patternValue == string.Empty)
//                {
//                    patternValue = UserParams.StateArea.Value;
//                    defaultRowIndex = 0;
//                }
//
//                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            }

            UltraChart1.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_ActiveCellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Index > 0)
            {
                UserParams.SelectItem.Value = e.Cell.Column.Header.Caption;
                UltraChart1.DataBind();
            }
            else
            {
                e.Cancel = true;
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0016_0001_compare_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Район", dtGrid);

            UltraWebGrid.DataSource = dtGrid;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count < 33)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int width;
                switch (i)
                {
                    case 1:
                    case 3:
                        {
                            width = 42;
                            break;
                        }
                    case 9:
                    case 11:
                        {
                            width = 80;
                            break;
                        }
                    default:
                        {
                            width = 60;
                            break;
                        }
                }

                bool hidden = (i % 2 == 0);
                string formatString = (i == 1 || i == 3) ? "N0" : "N2";
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                SetColumnParams(e.Layout, 0, i, formatString, width, hidden);
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, caption, "");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            switch (Convert.ToInt32(e.Row.Cells[1].Value))
            {
                case 1:
                    {
                        e.Row.Cells[0].Style.BackColor = Color.LightGreen;
                        break;
                    }
                case 2:
                    {
                        e.Row.Cells[0].Style.BackColor = Color.LightYellow;
                        break;
                    }
                case 3:
                    {
                        e.Row.Cells[0].Style.BackColor = Color.Pink;
                        break;
                    }
            }

            // красим красным, если кол-во нарушений == 1
            for (int i = 1; i < e.Row.Cells.Count; i = i + 2)
            {
                if (e.Row.Cells[i + 1].Value != null && Convert.ToInt32(e.Row.Cells[i + 1].Value) == 1)
                {
                    e.Row.Cells[i].Style.CssClass = "BallRed";
                }
            }

            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                e.Row.Cells[0].TargetURL = string.Format("../../reports/fo_0016_0001/DefaultDetail.aspx?paramlist=periodYear={0};periodQuarter={1};region={2}",
                    ComboYear.SelectedValue, ComboQuarter.SelectedValue,  RegionsNamingHelper.LocalBudgetUniqueNames[e.Row.Cells[0].ToString()]);
            }
        }

        #endregion

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0016_0001_compare_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Район", dtChart);
            
            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("район", "р-н");
                }
            }

            NumericSeries serie = CRHelper.GetNumericSeries(1, dtChart);
            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(serie);

            lbIndicate.Text = string.Format("Значение индикатора {0}", UserParams.SelectItem.Value);

            query = DataProvider.GetQueryText("FO_0016_0001_compare_indicators");
            dtIndicators = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Индикатор", dtIndicators);

            if (dtIndicators == null || dtIndicators.Rows.Count == 0)
            {
                return;
            }

            indTitleLabel.Text = string.Format("Описание индикатора {0}", UserParams.SelectItem.Value);
            indNameLabel.Text = string.Format("{0}", dtIndicators.Rows[0][1]);
            indContentLabel.Text = string.Format("{0}", dtIndicators.Rows[1][1]);
            if (dtIndicators.Rows[2][1] != DBNull.Value && dtIndicators.Rows[2][1].ToString() != string.Empty)
            {
                indFormulaLabel.Visible = true;
                indFormulaLabel.Text = string.Format("{0}", dtIndicators.Rows[2][1]);
            }
            else
            {
                indFormulaLabel.Visible = false;
            }
            if (dtIndicators.Rows[3][1].ToString().Length > 2)
            {
                indNormValueLabel.Text = dtIndicators.Rows[3][1].ToString();
            }
            else
            {
                indNormValueLabel.Text = string.Format("{0}{1}", dtIndicators.Rows[3][1], dtIndicators.Rows[4][1]);
                string separator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
                normValue = double.Parse(dtIndicators.Rows[4][1].ToString().Replace(".", separator));
            }

            double minIndValue = Convert.ToDouble(dtIndicators.Rows[5][1]);
            double maxIndValue = Convert.ToDouble(dtIndicators.Rows[6][1]);
            double minDTValue = GetMinMaxTableValue(dtChart, 1, true);
            double maxDTValue = GetMinMaxTableValue(dtChart, 1, false);

            double min = Math.Min(minIndValue, minDTValue);
            double max = Math.Max(maxIndValue, maxDTValue);

            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
            UltraChart1.Axis.Y.RangeMin = min;
            UltraChart1.Axis.Y.RangeMax = max;
        }

        private static double GetMinMaxTableValue(DataTable dt, int columnIndex, bool asc)
        {
            string sort = asc ? "asc" : "desc";
            DataRow[] rows = dt.Select("", string.Format("{0} {1} ", dt.Columns[columnIndex].ColumnName, sort));
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i][columnIndex] != DBNull.Value && rows[i][columnIndex].ToString() != string.Empty)
                {
                    return Convert.ToDouble(rows[i][columnIndex]);
                }
            }

            return 0;
        }

        private bool IsFailure(string regionName)
        {
            DataRow[] rows = dtChart.Select(string.Format("Район = '{0}'", regionName));
            if (rows.Length > 0)
            {
                return Convert.ToInt32(rows[0][2]) == 1;
            }

            return false;
        }

        private void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            // для нулевого не выводим
            if (normValue != 0)
            {
                IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                int textWidht = 200;
                int textHeight = 10;

                Text text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds =
                    new Rectangle((int) xAxis.Map(0), ((int) yAxis.Map(normValue)) - textHeight, textWidht, textHeight);
                text.SetTextString(string.Format("Нормативное значение: {0}", normValue));
                e.SceneGraph.Add(text);

                Line line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dot;
                line.PE.Stroke = Color.Red;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)xAxis.MapMinimum, (int)yAxis.Map(normValue));
                line.p2 = new Point((int)xAxis.MapMaximum, (int)yAxis.Map(normValue));
                e.SceneGraph.Add(line);
            }

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null)
                        {
                            box.Series.Label = RegionsNamingHelper.FullName(box.Series.Label);
                        }

                        if (box.DataPoint != null && box.DataPoint.Label != string.Empty)
                        {
                            string regionName = box.DataPoint.Label;
                            if (IsFailure(regionName))
                            {
                                box.DataPoint.Label = string.Format("Нарушено\n{0}", box.DataPoint.Label);
                                box.PE.Fill = Color.Red;
                                box.PE.FillStopColor = Color.Maroon;
                            }
                            else
                            {
                                box.DataPoint.Label = string.Format("Нарушения нет\n{0}", box.DataPoint.Label);
                                box.PE.Fill = Color.Green;
                                box.PE.FillStopColor = Color.ForestGreen;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
