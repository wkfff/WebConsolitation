using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0004 : CustomReportPage
    {
        private DataTable dt;
        private DataTable newDt;

        // год прошлого месяца
        private CustomParam prevMonthYear;
        // полугодие прошлого месяца
        private CustomParam prevMonthHalfYear;
        // квартал прошлого месяца
        private CustomParam prevMonthQuarter;
        // прошлый месяц
        private CustomParam prevMonth;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (prevMonthYear == null)
            {
                prevMonthYear = UserParams.CustomParam("prev_month_year");
            }
            if (prevMonthHalfYear == null)
            {
                prevMonthHalfYear = UserParams.CustomParam("prev_month_half_year");
            }
            if (prevMonthQuarter == null)
            {
                prevMonthQuarter = UserParams.CustomParam("prev_month_quarter");
            }
            if (prevMonth == null)
            {
                prevMonth = UserParams.CustomParam("prev_month");
            }

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0004_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);
            int day = Convert.ToInt32(dtDate.Rows[0][4]);

            int prevYear = yearNum;
            int prevMonthNum = monthNum;
            if (prevMonthNum == 1)
            {
                prevMonthNum = 12;
                prevYear--;
            }
            else
            {
                prevMonthNum--;
            }

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.PeriodDayFO.Value = day.ToString();

            prevMonthYear.Value = prevYear.ToString();
            prevMonthHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(prevMonthNum));
            prevMonthQuarter.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(prevMonthNum));
            prevMonth.Value = CRHelper.RusMonth(prevMonthNum);

            Label1.Text = string.Format("Информация по средствам областного бюджета на {0}&nbsp;{1}&nbsp;{2}&nbsp;года, тыс.руб.",
                                        day,
                                        CRHelper.RusMonthGenitive(monthNum),
                                        yearNum);
            
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            // создаем новую таблицу
            newDt = new DataTable();
            newDt.Columns.Add("column1", typeof (string));
            newDt.Columns.Add("column2", typeof (string));

            string query = DataProvider.GetQueryText("FO_0035_0004");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            double totalRemains = GetDoubleDTValue(dt, "Общий остаток средств на счете ");
            double distributionRemains = GetDoubleDTValue(dt, "Остаток средств к распределению ");
            double stateRemains = GetDoubleDTValue(dt, "Федеральные средства (заявлено) ");
            double depFinRemains = GetDoubleDTValue(dt, "Федеральные средства на счетах в ДФ ");
            double cashIncomes = GetDoubleDTValue(dt, "Кассовые поступления ");
            double cashIncomesRate = GetDoubleDTValue(dt, "Темп роста поступлений к предыдущему месяцу, % ");
            string cashIncomesRateArrow = cashIncomesRate == double.MinValue ? string.Empty :
                cashIncomesRate > 1
                ? "<img src=\"../../../images/arrowUpGreen.png\" width=\"13px\" height=\"16px\">"
                : "<img src=\"../../../images/arrowDownRed.png\" width=\"13px\" height=\"16px\">";
            double cashOutcomes = GetDoubleDTValue(dt, "Кассовые выплаты ");
            double cashOutcomesRate = GetDoubleDTValue(dt, "Темп роста выплат к предыдущему месяцу, % ");
            string cashOutcomesRateArrow = cashIncomesRate == double.MinValue ? string.Empty :
                cashOutcomesRate > 1
                ? "<img src=\"../../../images/arrowUpRed.png\" width=\"13px\" height=\"16px\">"
                : "<img src=\"../../../images/arrowDownGreen.png\" width=\"13px\" height=\"16px\">";

            DataRow row1 = newDt.NewRow();
            row1[0] = "Общий остаток на счете";
            row1[1] = totalRemains != double.MinValue ? string.Format("<span style=\"color:white;\">{0:N1}</span>", totalRemains) : string.Empty;
            newDt.Rows.Add(row1);

            DataRow row2 = newDt.NewRow();
            row2[0] = "Остаток к распределению";
            row2[1] = distributionRemains != double.MinValue ? string.Format("<span style=\"color:white;\">{0:N1}</span>", distributionRemains) : string.Empty;
            newDt.Rows.Add(row2);

            DataRow row3 = newDt.NewRow();
            row3[0] = "Федеральные средства (заявлено)";
            row3[1] = stateRemains != double.MinValue ? string.Format("<span style=\"color:white;\">{0:N1}</span>", stateRemains) : string.Empty;
            newDt.Rows.Add(row3);

            DataRow row4 = newDt.NewRow();
            row4[0] = "Федеральные средства на счетах в ДФ";
            row4[1] = depFinRemains != double.MinValue ? string.Format("<span style=\"color:white;\">{0:N1}</span>", depFinRemains) : string.Empty;
            newDt.Rows.Add(row4);

            DataRow row5 = newDt.NewRow();
            row5[0] = "Поступления за месяц";
            row5[1] = cashIncomes != double.MinValue ? string.Format("<span style=\"color:white;\">{0:N1}</span>", cashIncomes) : string.Empty;
            newDt.Rows.Add(row5);

            DataRow row6 = newDt.NewRow();
            row6[0] = "Темп роста поступлений к предыдущему месяцу";
            row6[1] = cashIncomesRate != double.MinValue ? string.Format("<span style=\"color:white;\">{0:P2}</span>&nbsp;{1}", cashIncomesRate, cashIncomesRateArrow) : string.Empty;
            newDt.Rows.Add(row6);

            DataRow row7 = newDt.NewRow();
            row7[0] = "Выплаты за месяц за счет областного бюджета";
            row7[1] = cashOutcomes != double.MinValue ? string.Format("<span style=\"color:white;\">{0:N1}</span>", cashOutcomes) : string.Empty;
            newDt.Rows.Add(row7);

            DataRow row8 = newDt.NewRow();
            row8[0] = "Темп роста выплат к предыдущему месяцу";
            row8[1] = cashOutcomesRate != double.MinValue ? string.Format("<span style=\"color:white;\">{0:P2}</span>&nbsp;{1}", cashOutcomesRate, cashOutcomesRateArrow) : string.Empty;
            newDt.Rows.Add(row8);

            UltraWebGrid.DataSource = newDt;
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, double.MinValue);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.ColHeadersVisibleDefault = ShowMarginInfo.No;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].Width = 208;
                e.Layout.Bands[0].Columns[1].Width = 100;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool bottomSplitRow = (e.Row.Index == 0 || e.Row.Index == 2 || e.Row.Index == 4);

                if (bottomSplitRow)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                }

                if (e.Row.Index == 5)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
            }
        }

        #endregion
    }
}
