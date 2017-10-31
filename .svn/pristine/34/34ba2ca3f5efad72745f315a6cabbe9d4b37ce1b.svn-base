using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0037_0001_V : CustomReportPage
    {
        private DataTable dt;

        private DateTime dateIncomes;
        private DateTime dateIncomesDayly;
        private DateTime dateOutcomes;

        private CustomParam periodMonthIncomes;
        private CustomParam periodMonthOutcomes;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (periodMonthIncomes == null)
            {
                periodMonthIncomes = UserParams.CustomParam("period_month_incomes");
            }
            if (periodMonthOutcomes == null)
            {
                periodMonthOutcomes = UserParams.CustomParam("period_month_outcomes");
            }

            DataTable dtDateIncomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0037_0001_date_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDateIncomes);
            
            dateIncomes =
                new DateTime(Convert.ToInt32(dtDateIncomes.Rows[0][0]),
                             CRHelper.MonthNum(dtDateIncomes.Rows[0][3].ToString()),
                             1);
            dateIncomes = dateIncomes.AddMonths(1);

            DataTable dtDateIncomesDayly = new DataTable();
            query = DataProvider.GetQueryText("FO_0037_0001_date_incomes_dayly");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDateIncomesDayly);
            
            dateIncomesDayly =
                new DateTime(Convert.ToInt32(dtDateIncomesDayly.Rows[0][0]),
                             CRHelper.MonthNum(dtDateIncomesDayly.Rows[0][3].ToString()),
                             Convert.ToInt32(dtDateIncomesDayly.Rows[0][4]));
            if (CRHelper.MonthLastDay(dateIncomesDayly.Month) == Convert.ToInt32(dateIncomesDayly.Day))
            {
                dateIncomesDayly = dateIncomes.AddDays(1);
            }
            if (dateIncomesDayly > dateIncomes)
            {
                periodMonthIncomes.Value = dtDateIncomesDayly.Rows[0][5].ToString();
                UserParams.CubeName.Value = "ФО_Исполнение бюджета_Доходы";
                dateIncomes = dateIncomesDayly;
            }
            else
            {
                periodMonthIncomes.Value = dtDateIncomes.Rows[0][5].ToString();
                UserParams.CubeName.Value = "ФО_Исполнение бюджета";
            }

            DataTable dtDateOutcomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0037_0001_date_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDateOutcomes);
            periodMonthOutcomes.Value = dtDateOutcomes.Rows[0][5].ToString();
            dateOutcomes =
                new DateTime(Convert.ToInt32(dtDateOutcomes.Rows[0][0]),
                             CRHelper.MonthNum(dtDateOutcomes.Rows[0][3].ToString()),
                             1);
            dateOutcomes = dateOutcomes.AddMonths(1);

            string period = String.Format("{0} {1} {2} года", dateIncomes.Day, CRHelper.RusMonthGenitive(dateIncomes.Month), dateIncomes.Year);
            Label3.Text = String.Format("Доходы на {0}<br/>", period);

            period = String.Format("{0} {1} {2} года", dateOutcomes.Day, CRHelper.RusMonthGenitive(dateOutcomes.Month), dateOutcomes.Year);
            Label4.Text = String.Format("Расходы на {0}<br/>", period);

            DateTime reportDate = dateIncomes > dateOutcomes ? dateIncomes : dateOutcomes;

            Label1.Text = string.Format("данные на {0} {1} {2} года", reportDate.Day, CRHelper.RusMonthGenitive(reportDate.Month), reportDate.Year);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            
            UltraWebGrid.DataBind();
            UltraWebGrid3.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0037_0001_V_grid1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);
            dt.Columns.RemoveAt(0);
            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1])/1000;
                }
              //  row[0] = DataDictionariesHelper.GetShortBugetName(row[0].ToString());
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid3_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0037_0001_V_grid2");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);
            dt.Columns.RemoveAt(0);
            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / 1000;
                }
               // row[0] = DataDictionariesHelper.GetShortBugetName(row[0].ToString());
            }

            UltraWebGrid3.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetupLayout(e, "Показатель доходов");
        }

        protected void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetupLayout(e, "Показатель расходов");
        }

        private static void SetupLayout(LayoutEventArgs e, string kindsCaption)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Header.Caption = kindsCaption;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Факт<br/>тыс. руб.";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Уд. вес к год. назнач. %";

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].HeaderStyle.Height = 5;
            
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");

            e.Layout.Bands[0].Columns[0].Width = 153;
            e.Layout.Bands[0].Columns[1].Width = 85;
            e.Layout.Bands[0].Columns[2].Width = 70;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);

                if (i != 0)
                {
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }
            }
            if (e.Row.Cells[0].ToString() == "Налоговые и неналоговые доходы" ||
                e.Row.Cells[0].ToString() == "Расходы бюджета-итого")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                   // cell.Style.ForeColor = Color.White;
                    cell.Style.Font.Bold = true;
                }
            }
        }
    }
}
