using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0004_0002_1 : CustomReportPage
    {
        private DateTime currDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0004_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();

            currDate = CRHelper.PeriodDayFoDate(UserParams.PeriodCurrentDate.Value);

            FO_0004_0002_Chart1.QueryName = "FO_0004_0002_chart1";
            FO_0004_0002_Chart3.QueryName = "FO_0004_0002_chart3";
            FO_0004_0002_Chart1.Caption = "Структура назначений";
            FO_0004_0002_Chart3.Caption = "Структура фактических доходов";

            FO_0004_0003_Chart1.QueryName = "FO_0004_0003_chart1";
            FO_0004_0003_Chart3.QueryName = "FO_0004_0003_chart3";
            FO_0004_0003_Chart1.Caption = "Структура назначений";
            FO_0004_0003_Chart3.Caption = "Структура фактических расходов";

            FO_0004_0002_Chart1.Date = currDate;
            FO_0004_0002_Chart3.Date = currDate;
            FO_0004_0003_Chart1.Date = currDate;
            FO_0004_0003_Chart3.Date = currDate;

            FO_0004_0003_Chart1.ColorModelStartIndex = 7;
            FO_0004_0003_Chart3.ColorModelStartIndex = 7;

            SetupGrid(UltraWebGrid, "FO_0004_0001_table_incomes");
            SetupGrid(UltraWebGrid1, "FO_0004_0001_table_outcomes");

            //UltraWebGrid1.DisplayLayout.Bands[0].HeaderLayout.Clear();

            DataTable dtText = new DataTable();
            query = DataProvider.GetQueryText("FO_0004_0001_text");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtText);

            lbTitle.Text = currDate.Month == 1 ?
                String.Format("Основные показатели по состоянию за&nbsp;<b><span class='DigitsValue'>январь {1:yyyy} года</span></b>, млн.руб.", CRHelper.RusMonth(currDate.Month), currDate) :
                String.Format("Основные показатели по состоянию за&nbsp;<b><span class='DigitsValue'>январь&ndash;{0} {1:yyyy} года</span></b>, млн.руб.", CRHelper.RusMonth(currDate.Month), currDate);

            lbDescription.Text =
                String.Format(
                    "Численность постоянного населения&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;тыс.чел.", dtText.Rows[0]["Численность постоянного населения "]);

            Label1.Text =
                String.Format(
                    "Среднедушевые доходы&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;тыс.руб./чел", dtText.Rows[0]["Среднедушевые доходы обл бюджет"]);

            Label2.Text =
                String.Format(
                    "Бюджетные расходы на душу населения&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;тыс.руб./чел", dtText.Rows[0]["Среднедушевые расходы обл бюджет"]);
        }

        private void SetupGrid(UltraWebGrid grid, string queryName)
        {
            DataTable dtSource = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtSource);

            grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            grid.InitializeRow += new InitializeRowEventHandler(grid_InitializeRow);
            grid.Width = 760;

            DataTable dt = new DataTable();

            for (int i = 0; i < 5; i++)
            {
                dt.Columns.Add(new DataColumn());
            }

            foreach (DataRow row in dtSource.Rows)
            {
                DataRow newRow = dt.NewRow();

                newRow[0] = row[0];

                newRow[1] = String.Format("{0:N2}<br/>{1:P2}", row[1], row[3]);
                newRow[2] = String.Format("{0:N2}<br/>{1:P2}", row[2], row[4]);

                newRow[3] = String.Format("{0:N2}<br/>{1:P2}", row[5], row[7]);
                newRow[4] = String.Format("{0:N2}<br/>{1:P2}", row[6], row[8]);

                dt.Rows.Add(newRow);
            }

            grid.DataSource = dt;
            grid.DataBind();
        }

        int legendColorCounter = 1;

        void grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().Contains("Итого"))
            {
                e.Row.Style.Font.Bold = true;
            }
            else
            {
                e.Row.Cells[0].Value = String.Format("<div style='float: left; margin-right: 10px'><img src='../../../images/LegendColors/LegendColor{0}.png'></div>{1}", legendColorCounter, e.Row.Cells[0].Value);
                legendColorCounter++;
            }
        }

        private int borderWidth = 3;

        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Grid.Columns[0].Width = 350;
            e.Layout.Grid.Columns[1].Width = 102;
            e.Layout.Grid.Columns[2].Width = 102;
            e.Layout.Grid.Columns[3].Width = 102;
            e.Layout.Grid.Columns[4].Width = 102;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");

            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Right = 5;
            e.Layout.Bands[0].Columns[3].CellStyle.Padding.Right = 5;
            e.Layout.Bands[0].Columns[4].CellStyle.Padding.Right = 5;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;

            GridHeaderLayout headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell("Показатель");

            GridHeaderCell cell = headerLayout.AddCell(String.Format("{0} год", currDate.AddYears(-1).Year));
            cell.AddCell("план");
            cell.AddCell("факт");

            cell = headerLayout.AddCell(String.Format("{0} год", currDate.Year));
            cell.AddCell("план");
            cell.AddCell("факт");

            headerLayout.ApplyHeaderInfo();
        }
    }
}