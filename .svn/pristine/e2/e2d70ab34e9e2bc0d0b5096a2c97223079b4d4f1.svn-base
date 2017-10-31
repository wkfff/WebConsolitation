using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0022 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0022_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();            
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", date.AddYears(-1), 5);

            DataTable dtLastYearDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0022_date_appg");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtLastYearDate);
            
            if (dtLastYearDate.Rows.Count > 0)
            {
                UserParams.PeriodLastYear.Value = dtLastYearDate.Rows[0][1].ToString();
            }
            else
            {
                UserParams.PeriodLastYear.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[1000].[Полугодие 1].[Квартал 1].[Февраль].[22] ";
            }

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();

            #region левые лейблы
            lbDescription.Text = String.Format("Предельные объемы финансирования областного бюджета Новосибирской области<br/>на&nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, тыс. руб.", date);

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0022_rests_Date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            if (dtDate.Rows.Count > 0)
            {
                date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                selectedPeriod.Value = dtDate.Rows[0][1].ToString();
            }

            query = DataProvider.GetQueryText("FO_0035_0022_rests");
            DataTable dtRests = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtRests);

            lbRests.Text = String.Format("<table style='border-collapse: collapse; margin-top: 10px; height: 100px'><tr><td style='width: 500px'>Свободный остаток средств на счете<br/>(без учета невыясненных поступлений) на&nbsp;<b><span class='DigitsValueLarge'>{1:dd.MM.yyyy}</span></b>&nbsp;</td><td align='right'>&nbsp;<nobr><b><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp<span class='InformationText'>тыс. руб.</span></nobr></td><td><a href='webcommand?showReport=FO_0035_0016'><img src='../../../images/detail.png'></a></td></tr><tr><td style='height: 10px'></td></tr>", dtRests.Rows[0][1], date);

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0022_orders_Date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            query = DataProvider.GetQueryText("FO_0035_0022_orders");
            DataTable dtOrders = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtOrders);
            //lbOrders.Text = String.Format("Платежных поручений на рассмотрении ФО&nbsp;<b><span class='DigitsValue'>{1:dd.MM.yyyy} {0:N2}</span></b>&nbsp;руб.", dtOrders.Rows[0][1], date);
            lbRests.Text += String.Format("<tr><td>Платежные поручения на рассмотрении ФО на&nbsp;<b><span class='DigitsValueLarge'>{1:dd.MM.yyyy}</span></b>&nbsp;</td><td align='right'><nobr>&nbsp;<b><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp<span class='InformationText'>тыс. руб.</span></nobr></td><td><a href='webcommand?showReport=FO_0035_0024'><img src='../../../images/detail.png'></a></td></tr><tr><td style='height: 10px'></td></tr>", dtOrders.Rows[0][1], date);

            lbRests.Text += String.Format("<tr><td>Платежные поручения, отправленные в УФК (банк) на&nbsp;<b><span class='DigitsValueLarge'>{1:dd.MM.yyyy}</span></b>&nbsp;</td><td align='right'>&nbsp;<b><nobr><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp<span class='InformationText'>тыс. руб.</span></td><td></td></tr><tr><td style='height: 13px'></td></tr></table>", dtOrders.Rows[1][1], date);
            #endregion
        }        

        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0022_OutcomesGrid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtGrid);

            DataTable dtSource = new DataTable();
            dtSource.Columns.Add(new DataColumn("0", typeof(string)));
            dtSource.Columns.Add(new DataColumn("1", typeof(string)));
            dtSource.Columns.Add(new DataColumn("2", typeof(double)));
            dtSource.Columns.Add(new DataColumn("3", typeof(string)));
            dtSource.Columns.Add(new DataColumn("4", typeof(double)));
            dtSource.Columns.Add(new DataColumn("5", typeof(double)));
            dtSource.Columns.Add(new DataColumn("6", typeof(double)));
            dtSource.Columns.Add(new DataColumn("7", typeof(double)));

            foreach (DataRow dataRow in dtGrid.Rows)
            {
                DataRow newRow = dtSource.NewRow();

                string outcomesId = CustomParams.GetOutcomeIdByName(dataRow[0].ToString().Replace("ОБЩЕГОСУДАРСТ- ВЕННЫЕ РАСХОДЫ", "ОБЩЕГОСУДАРСТВЕННЫЕ РАСХОДЫ"));
                newRow[0] = String.Format("{0}", 
                        CRHelper.ToUpperFirstSymbol(dataRow[0].ToString().ToLower().Replace("Данные всех источников", "Всего").Replace(" фб", " ФБ")));
                

                if (outcomesId == String.Empty)
                {
                    newRow[1] = String.Empty;
                }
                else
                {
                    newRow[1] =
                        String.Format(
                            "<div style='float: right'><a href='webcommand?showPinchReport=FO_0035_0023_OUTCOMES={0}'><img src='../../../images/detail.png'></a></div>",
                            outcomesId);
                }

                newRow[2] = dataRow[2];

                if (dataRow[3] != DBNull.Value)
                {
                    newRow[3] = dataRow[3].ToString().Contains("-") ? String.Format("{0:N2}<br/>{1:N2}", dataRow[4], dataRow[3]) : String.Format("{0:N2}<br/>+{1:N2}", dataRow[4], dataRow[3]);
                }
                else
                {
                    newRow[3] = String.Format("{0:N2}", dataRow[4]);
                }
                newRow[4] = dataRow[5];
                newRow[5] = dataRow[6];
                newRow[6] = dataRow[7];
                newRow[7] = dataRow[8];

                dtSource.Rows.Add(newRow);
            }

            dtSource.AcceptChanges();
            OutcomesGrid.DataSource = dtSource;
        }

        private int borderWidth = 3;

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            UltraGridColumn col = e.Layout.Bands[0].Columns[1];

            e.Layout.Bands[0].Columns.RemoveAt(1);
            e.Layout.Bands[0].Columns.Insert(0, col);

            e.Layout.Bands[0].Columns[0].Width = 48;

            e.Layout.Bands[0].Columns[1].Width = 140;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[2].Width = 101;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");

            e.Layout.Bands[0].Columns[3].Width = 101;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].Columns[4].Width = 101;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");

            e.Layout.Bands[0].Columns[5].Width = 125;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");

            e.Layout.Bands[0].Columns[6].Width = 73;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].Columns[7].Width = 73;
            e.Layout.Bands[0].Columns[7].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");

            for (int i = 2; i < 8; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 3;
            }

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell(" ");
            headerLayout.AddCell("Расходы");
            headerLayout.AddCell(String.Format("План"));
            GridHeaderCell cell = headerLayout.AddCell("Профинансировано");            
            cell.AddCell(String.Format("с начала месяца, в том числе за {0}", date.ToString("dd.MM"), CRHelper.RusMonthGenitive(date.Month)));
            cell.AddCell(String.Format("за тот&nbsp;же период пред.года", CRHelper.RusMonthGenitive(date.Month)));
            headerLayout.AddCell(String.Format("Остатки за {0}", CRHelper.RusMonth(date.Month)));
            headerLayout.AddCell("% исп.");
            headerLayout.AddCell("Темп роста к пред. году");

            headerLayout.ApplyHeaderInfo();

            //e.Layout.Bands[0].Columns[2].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            //e.Layout.Bands[0].Columns[3].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);

            //e.Layout.Bands[0].Columns[5].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            //e.Layout.Bands[0].Columns[6].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Style.Height = 46;

            iPadBricks.iPadBricks.iPadBricksHelper.SetConditionCorner(e, 5, 0, true);

            if (e.Row.Cells[1].Value.ToString() == "Всего")
            {
                foreach (UltraGridCell cell in  e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
                e.Row.Cells[5].Style.Font.Size = 14;
            }
            e.Row.Cells[5].Style.Padding.Right = 3;
        }

        #endregion
    }
}
