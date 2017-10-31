using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0032 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        // выбранный период
        private CustomParam selectedPeriodRests;

        private DateTime date;
        private DateTime dateRests;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedPeriodRests = UserParams.CustomParam("selected_period_rests");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0022_rests_Date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            dateRests = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            selectedPeriodRests.Value = dtDate.Rows[0][1].ToString();

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0032_date_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);
            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            UserParams.PeriodYear.Value = CRHelper.PeriodMemberUName("", date, 1);
            UserParams.PeriodQuater.Value = CRHelper.PeriodMemberUName("", date, 3);

            IncomesGrid.Width = Unit.Empty;
            IncomesGrid.Height = Unit.Empty;
            IncomesGrid.DisplayLayout.NoDataMessage = "Нет данных";
            IncomesGrid.DataBind();

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0032_date_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);
            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            lbDescription.Text = String.Format("Платежный календарь областного бюджета Новосибирской области по состоянию на &nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, тыс.руб.", date);

            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("", date.AddYears(-1), 5);
            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0032_date_outcomes_last_year");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);            
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][1].ToString();

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";
            OutcomesGrid.DataBind();

            #region левые лейблы            

            query = DataProvider.GetQueryText("FO_0035_0022_rests");
            DataTable dtRests = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtRests);

            lbRests.Text = String.Format("<table style='border-collapse: collapse; margin-top: 10px'><tr><td style='width: 500px'>Свободный остаток средств на счете<br/>(без учета невыясненных поступлений) на&nbsp;<b><span class='DigitsValueLarge'>{1:dd.MM.yyyy}</span></b>&nbsp;</td><td align='right'>&nbsp;<b><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp<span class='InformationText'>тыс. руб.<span></td><td><a href='webcommand?showReport=FO_0035_0016'><img src='../../../images/detail.png'></a></td></tr><tr><td style='height: 23px'></td></tr>", dtRests.Rows[0][1], dateRests);

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0022_orders_Date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            query = DataProvider.GetQueryText("FO_0035_0022_orders");
            DataTable dtOrders = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtOrders);
            //lbOrders.Text = String.Format("Платежных поручений на рассмотрении ФО&nbsp;<b><span class='DigitsValue'>{1:dd.MM.yyyy} {0:N2}</span></b>&nbsp;руб.", dtOrders.Rows[0][1], date);
            lbRests.Text += String.Format("<tr><td>Платежные поручения на рассмотрении ФО на&nbsp;<b><span class='DigitsValueLarge'>{1:dd.MM.yyyy}</span></b>&nbsp;</td><td align='right'>&nbsp;<b><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp<span class='InformationText'>тыс. руб.</span></td><td><a href='webcommand?showReport=FO_0035_0024'><img src='../../../images/detail.png'></a></td></tr><tr><td style='height: 23px'></td></tr>", dtOrders.Rows[0][1], date);

            lbRests.Text += String.Format("<tr><td>Платежные поручения, отправленные в УФК (банк) на&nbsp;<b><span class='DigitsValueLarge'>{1:dd.MM.yyyy}</span></b>&nbsp;</td><td align='right'>&nbsp;<b><nobr><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp<span class='InformationText'>тыс. руб.</span></td><td></td></tr><tr><td style='height: 13px'></td></tr></table>", dtOrders.Rows[1][1], date);
            #endregion
        }
        

        #region Обработчики грида

        protected void IncomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0032_Grid_incomes");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            DataTable dtSource = new DataTable();
            dtSource.Columns.Add(new DataColumn());
            dtSource.Columns.Add(new DataColumn());

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                dtSource.Rows.Add(dtSource.NewRow());
                dtSource.Rows[i][0] = dtGrid.Rows[i][0];
            }

            dtSource.Rows[4][0] = String.Format("За {0:dd.MM.yyyy}", date);

            dtSource.Rows[0][1] = String.Format("{0:N2}", dtGrid.Rows[0][1]);
            dtSource.Rows[1][1] = String.Format("{0:N2}", dtGrid.Rows[1][1]);
            dtSource.Rows[2][1] = String.Format("{0:N2}", dtGrid.Rows[2][1]);
            dtSource.Rows[4][1] = String.Format("{0:N2}", dtGrid.Rows[4][1]);
            dtSource.Rows[5][1] = String.Format("{0:N2}", dtGrid.Rows[5][1]);
            dtSource.Rows[6][1] = String.Format("{0:N2}", dtGrid.Rows[6][1]);

            dtSource.Rows[3][1] = String.Format("{0:P2}", dtGrid.Rows[3][1]);
            dtSource.Rows[7][1] = String.Format("{0:P2}", dtGrid.Rows[7][1]);

            IncomesGrid.DataSource = dtSource;
        }

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0032_Grid_outcomes");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            DataTable dtSource = new DataTable();
            dtSource.Columns.Add(new DataColumn());
            dtSource.Columns.Add(new DataColumn());

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                dtSource.Rows.Add(dtSource.NewRow());
                dtSource.Rows[i][0] = dtGrid.Rows[i][0];
            }

            dtSource.Rows[4][0] = String.Format("За {0:dd.MM.yyyy}", date);

            dtSource.Rows[0][1] = String.Format("{0:N2}", dtGrid.Rows[0][1]);
            dtSource.Rows[1][1] = String.Format("{0:N2}", dtGrid.Rows[1][1]);
            dtSource.Rows[2][1] = String.Format("{0:N2}", dtGrid.Rows[2][1]);
            dtSource.Rows[4][1] = String.Format("{0:N2}", dtGrid.Rows[4][1]);
            dtSource.Rows[5][1] = String.Format("{0:N2}", dtGrid.Rows[5][1]);
            dtSource.Rows[6][1] = String.Format("{0:N2}", dtGrid.Rows[6][1]);

            dtSource.Rows[3][1] = String.Format("{0:P2}", dtGrid.Rows[3][1]);
            dtSource.Rows[7][1] = String.Format("{0:P2}", dtGrid.Rows[7][1]);
            dtSource.Rows[8][1] = String.Format("{0:N2}", dtGrid.Rows[8][1]);

            OutcomesGrid.DataSource = dtSource;

        }

        protected void IncomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 500;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 260;

            e.Layout.Bands[0].HeaderLayout.Clear();
        }

        protected void IncomesGrid_InitializeRow(object sender, RowEventArgs e)
        {            
            //if (e.Row.Cells[0].Value.ToString() == "Всего")
            //{
            //    foreach (UltraGridCell cell in e.Row.Cells)
            //    {
            //        cell.Style.Font.Bold = true;
            //    }
            //}

            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 6, 1, true); 
        }

        #endregion
    }
}
