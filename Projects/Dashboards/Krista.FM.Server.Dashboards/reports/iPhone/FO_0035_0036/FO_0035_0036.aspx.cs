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
    public partial class FO_0035_0036 : CustomReportPage
    {
        private DataTable dtYesterday;
        private DataTable dtToday;

        private DateTime currDate;
        private DateTime lastDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0016_rests_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[1][5].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[0][5].ToString();

            currDate =
                new DateTime(Convert.ToInt32(dtDate.Rows[1][0].ToString()),
                             CRHelper.MonthNum(dtDate.Rows[1][3].ToString()),
                             Convert.ToInt32(dtDate.Rows[1][4].ToString()));

            lastDate = new DateTime(Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                             CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                             Convert.ToInt32(dtDate.Rows[0][4].ToString()));

            dtToday = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0016_table_today");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtToday);

            dtYesterday = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0016_table_yesterday");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtYesterday);

            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);

            UltraWebGrid.DataSource = dtToday;
            UltraWebGrid.DataBind();

            UltraWebGrid.Width = Unit.Empty;

            lbDescription.Text =
                String.Format(
                    "Остатки средств областного бюджета, свободные для распределения по направлениям финансирования на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>, тыс.руб.",
                    currDate);

            UltraWebGrid.Bands[0].HeaderLayout.Clear();
        }

        void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {            
            if (e.Row.Index == 1)
            {
                e.Row.Cells[0].Value = "<img src='../../../images/Incomes.png'>";                
                SetGrown(e);
            }
            else if (e.Row.Index == 2)
            {
                e.Row.Cells[0].Value = "<img src='../../../images/CashOutcomes.png'>";                
                SetGrown(e);
            }
            else if (e.Row.Index == 3)
            {
                e.Row.Cells[0].Value = "<img src='../../../images/CashRests.png'>";                
                SetGrown(e);
            }
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
        }

        private int borderWidth = 3;

        private void SetGrown(RowEventArgs e)
        {
            int i = 1;

            double todayValue = Convert.ToDouble(dtToday.Rows[e.Row.Index][i]);
            double yesterdayValue = Convert.ToDouble(dtYesterday.Rows[e.Row.Index][i]);
            double grownValue = todayValue - yesterdayValue;
            string value = grownValue > 0 ? String.Format("+{0:N2}", grownValue) : String.Format("{0:N2}", grownValue);
            string grown = grownValue > 0 ? String.Format("прирост с {0:dd.MM}", lastDate) : String.Format("снижение с {0:dd.MM}", lastDate);
            string image = string.Empty;
            if (e.Row.Index == 3)
            {
                image = grownValue > 0 ? "<img src='../../../images/ArrowGreenUpIPad.png'>" : "<img src='../../../images/ArrowRedDownIPad.png'>";
                e.Row.Cells[i + 1].Value =
                    String.Format(
                        "<span class='DigitsValue'>{0:N2}</span><br/><div style='clear: both; height: 4px'></div><span class='ServeText'>{3}</span><br/>{2}&nbsp;<span class='InformationText'>{1}</span>",
                        todayValue, value, image, grown);
            }           
        }

        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid.Columns.Insert(0, new UltraGridColumn());

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            UltraWebGrid.Columns[0].Width = 40;
            UltraWebGrid.Columns[1].Width = 150;
            UltraWebGrid.Columns[2].Width = 125;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            UltraWebGrid.Columns[2].CellStyle.Padding.Right = 1;
        }
    }
}