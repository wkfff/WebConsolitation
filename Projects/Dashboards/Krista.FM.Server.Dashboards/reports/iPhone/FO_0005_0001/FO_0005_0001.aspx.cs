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
    public partial class FO_0005_0001_1 : CustomReportPage
    {
        private DataTable dtStruct;
        private DataTable dtIncomes;
        private DataTable dtOutcomes;
        private DataTable dtFinsources;
        private DataTable dtDebt;
        private DataTable dtSaldo;

        private DateTime currDate;
        private DateTime lastDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string query;

            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid3.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid3.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid2_InitializeLayout);

            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            UltraWebGrid4.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid4_InitializeLayout);
            UltraWebGrid5.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid5_InitializeLayout);

            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_rests_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            currDate = CRHelper.PeriodDayFoDate(UserParams.PeriodCurrentDate.Value);

            dtStruct = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_table_struct");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Вид долгового обязательства", dtStruct);

            dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_table_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            dtOutcomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_table_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtOutcomes);

            dtFinsources = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_table_finsources");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ИФДБ", dtFinsources);

            dtDebt = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_table_debt");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDebt);

            dtSaldo = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_table_saldo");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtSaldo);

            UltraWebGrid.DataSource = dtStruct;
            UltraWebGrid.DataBind();

            UltraWebGrid1.DataSource = dtIncomes;
            UltraWebGrid1.DataBind();

            UltraWebGrid2.DataSource = dtOutcomes;
            UltraWebGrid2.DataBind();

            UltraWebGrid3.DataSource = dtFinsources;
            UltraWebGrid3.DataBind();

            UltraWebGrid4.DataSource = dtDebt;
            UltraWebGrid4.DataBind();

            UltraWebGrid5.DataSource = dtSaldo;
            UltraWebGrid5.DataBind();

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid2.Width = Unit.Empty;
            UltraWebGrid3.Width = Unit.Empty;
            UltraWebGrid4.Width = Unit.Empty;
            UltraWebGrid5.Width = Unit.Empty;

            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid2.Height = Unit.Empty;
            UltraWebGrid3.Height = Unit.Empty;
            UltraWebGrid4.Height = Unit.Empty;
            UltraWebGrid5.Height = Unit.Empty;

            UltraWebGrid1.Style.Add("margin-left", "41px");
            UltraWebGrid2.Style.Add("margin-left", "-23px");
            UltraWebGrid3.Style.Add("margin-top", "5px");
            UltraWebGrid4.Style.Add("margin-top", "5px");
            UltraWebGrid4.Style.Add("margin-left", "41px");
            UltraWebGrid.Style.Add("margin-top", "5px");

            lbDescription.Text =
                String.Format(
                    "Информация по государственному внутреннему долгу Новосибирской области<br/>по состоянию на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>, тыс.руб.",
                    currDate.AddMonths(1));

            IPadElementHeader1.Text = String.Format("Структура государственного внутреннего долга НСО на {0:dd.MM.yyyy}", currDate.AddMonths(1));
            IPadElementHeader2.Text = currDate.Month == 1 ? String.Format("Операции НСО на внутреннем рынке заимствований<br/>за январь {1} г.", CRHelper.RusMonth(currDate.Month), currDate.Year) : String.Format("Операции НСО на внутреннем рынке заимствований<br/>за январь-{0} {1} г.", CRHelper.RusMonth(currDate.Month), currDate.Year);
            IPadElementHeader4.Text = String.Format("Расходы на обслуживание государственного внутреннего долга на {0:dd.MM.yyyy}", currDate.AddMonths(1));
            IPadElementHeader3.Text = String.Format("Источники финансирования дефицита областного бюджета на {0:dd.MM.yyyy}", currDate.AddMonths(1));
            DataTable dtResult = new DataTable();
            query = DataProvider.GetQueryText("FO_0005_0001_result");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtResult);

            Label1.Text = String.Format(
                    "<table><tr><td>Результат исполнения областного бюджета НСО&nbsp;<span class='DigitsValueXLarge'>{0:N2}</span>&nbsp;тыс.руб.</td><td>&nbsp;<a href='webcommand?showPinchReport=FO_0005_0002'><img src='../../../images/detail.png'></a></td></tr></table>",
                    dtResult.Rows[0][0]);
        }

        void UltraWebGrid5_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 130;
            e.Layout.Bands[0].Columns[1].Width = 130;
            
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].HeaderLayout.Clear();
        }

        void UltraWebGrid4_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 226;
            e.Layout.Bands[0].Columns[1].Width = 227;
            e.Layout.Bands[0].Columns[2].Width = 227;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[0], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;            
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 230;
            e.Layout.Bands[0].Columns[1].Width = 200;

            e.Layout.Bands[0].Columns[0].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 280;
            e.Layout.Bands[0].Columns[1].Width = 200;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Header.Style.BackColor = Color.Black;

            e.Layout.Bands[0].Columns[0].Header.Style.BorderDetails.ColorTop = Color.Black;
            e.Layout.Bands[0].Columns[0].Header.Style.BorderDetails.ColorBottom = Color.Transparent;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().Contains("Итого"))
            {
                e.Row.Cells[0].Style.ForeColor = Color.White;
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[1].Style.Font.Bold = true;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
            }
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 1)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorLeft = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorRight = Color.Transparent;
                e.Row.Cells[1].Style.BorderDetails.ColorLeft = Color.Transparent;
                e.Row.Cells[1].Style.BorderDetails.ColorRight = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }

            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;

                e.Row.Cells[1].Style.Font.Bold = true;
            }
        }

        private int borderWidth = 3;



        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 560;
            e.Layout.Bands[0].Columns[1].Width = 200;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("на {0:dd.MM.yyyy}", currDate.AddMonths(1));

            e.Layout.Bands[0].HeaderLayout.Clear();
        }
    }
}