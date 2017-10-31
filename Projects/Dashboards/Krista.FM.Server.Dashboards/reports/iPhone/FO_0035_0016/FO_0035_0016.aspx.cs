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
    public partial class FO_0035_0016_1 : CustomReportPage
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

            DataRow row = dtToday.NewRow();
            row[0] = "<span class='ServeText'>доходы от возврата остатков субсидий и субвенций прошлых лет в части ФБ из местных бюджетов</span>";
            row[4] = dtToday.Rows[1][5];
            dtToday.Rows.InsertAt(row, 2);

            row = dtToday.NewRow();
            row[0] = "<span class='ServeText'>возврат остатков субсидий и субвенций из бюджета в ФБ</span>";
            row[4] = dtToday.Rows[1][6];
            dtToday.Rows.InsertAt(row, 3);

            dtToday.AcceptChanges();
            row = dtYesterday.NewRow();
            dtYesterday.Rows.InsertAt(row, 2);

            row = dtYesterday.NewRow();
            dtYesterday.Rows.InsertAt(row, 3);
            dtYesterday.AcceptChanges();

            UltraWebGrid.DataSource = dtToday;
            UltraWebGrid.DataBind();

            UltraWebGrid.Width = 760;

            lbDescription.Text =
                String.Format(
                    "Расчет остатков средств областного бюджета, свободных для распределения по направлениям финансирования на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>, тыс.руб.",
                    currDate);
        }

        void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = "<a href='webcommand?showPinchReport=FO_0035_0033'><img style='padding-top: 5px' src='../../../images/Detail.png'/></a>";                
            }
            if (e.Row.Index == 1)
            {
                e.Row.Cells[0].Value = "<img src='../../../images/Incomes.png'/><br/><a href='webcommand?showPinchReport=FO_0035_0017'><img style='padding-top: 5px' src='../../../images/Detail.png'/></a>";
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[1].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
                SetGrown(e);
            }
            else if (e.Row.Index == 2 || e.Row.Index == 3)
            {
                e.Row.Cells[2].Value = e.Row.Cells[1].Value;
                e.Row.Cells[1].Value = String.Empty;
                e.Row.Cells[2].ColSpan = 3;
                e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
                e.Row.Cells[1].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[1].Style.BorderDetails.ColorTop = Color.Transparent;
            }
            else if (e.Row.Index == 4)
            {
                e.Row.Cells[0].Value = "<img src='../../../images/CashOutcomes.png'/><br/><a href='webcommand?showPinchReport=FO_0035_0018'><img style='padding-top: 5px' src='../../../images/Detail.png'/></a>";
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
                SetGrown(e);
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index == 9)
            {
                e.Row.Cells[0].Value = "<img src='../../../images/CashRests.png'/><br/><a href='webcommand?showPinchReport=FO_0035_0019'><img style='padding-top: 5px' src='../../../images/Detail.png'/></a>";
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
                SetGrown(e);
            }
            else if ((e.Row.Index > 4 && e.Row.Index < 9) || (e.Row.Index > 0 && e.Row.Index < 3))
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }
            for (int i = 1; i < 6; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 2;
            }


            if (e.Row.Index == 4 || e.Row.Index == 9 || e.Row.Index == 1)
            {
                e.Row.Style.BorderDetails.WidthTop = borderWidth;
            }

            if (e.Row.Index > 3 && e.Row.Index != 9)
            {
                e.Row.Cells[4].Value = String.Empty;
            }
        }

        private int borderWidth = 3;

        private void SetGrown(RowEventArgs e)
        {
            for (int i = 1; i < 5; i++)
            {
                if (e.Row.Index > 3 && i == 3 && e.Row.Index != 9)
                {
                    e.Row.Cells[i + 1].Value = String.Empty;
                }
                else
                {
                    double todayValue = Convert.ToDouble(dtToday.Rows[e.Row.Index][i]);
                    double yesterdayValue = Convert.ToDouble(dtYesterday.Rows[e.Row.Index][i]);
                    double grownValue = todayValue - yesterdayValue;
                    string value = grownValue > 0 ? String.Format("+{0:N2}", grownValue) : String.Format("{0:N2}", grownValue);
                    string grown = grownValue > 0 ? String.Format("прирост с {0:dd.MM}", lastDate) : String.Format("снижение с {0:dd.MM}", lastDate);
                    string image = string.Empty;
                    if (e.Row.Index == 9)
                    {
                        image = grownValue > 0 ? "<img src='../../../images/ArrowGreenUpIPad.png'/>" : "<img src='../../../images/ArrowRedDownIPad.png'/>";
                        e.Row.Cells[i + 1].Value =
                            String.Format(
                                "<span class='DigitsValueXLarge'>{0:N2}</span><br/><div style='clear: both; height: 7px'></div><span class='ServeText'>{3}</span><br/>{2}&nbsp;<span class='InformationText'>{1}</span>",
                                todayValue, value, image, grown);
                    }
                    else
                    {
                        e.Row.Cells[i + 1].Value =
                            String.Format(
                                "{0:N2}<br/><div style='clear: both; height: 7px'></div><span class='ServeText'>{3}</span><br/>{2}&nbsp;<span class='InformationText'>{1}</span>",
                                todayValue, value, image, grown);
                    }
                }
            }
        }

        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid.Columns.Insert(0, new UltraGridColumn());

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            UltraWebGrid.Columns[6].Hidden = true;
            UltraWebGrid.Columns[7].Hidden = true;

            UltraWebGrid.Columns[0].Width = 53;
            UltraWebGrid.Columns[1].Width = 182;
            UltraWebGrid.Columns[2].Width = 130;
            UltraWebGrid.Columns[3].Width = 130;
            UltraWebGrid.Columns[4].Width = 130;
            UltraWebGrid.Columns[5].Width = 130;

            UltraWebGrid.Columns[2].CellStyle.Padding.Right = 3;
            UltraWebGrid.Columns[3].CellStyle.Padding.Right = 3;
            UltraWebGrid.Columns[4].CellStyle.Padding.Right = 3;
            UltraWebGrid.Columns[5].CellStyle.Padding.Right = 3;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
        }
    }
}