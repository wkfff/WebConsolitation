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
    public partial class FO_0038_0001_H : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();

        private int monthNum;
        private int yearNum;

        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0002_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            //period_day_fk
            //[Период].[Период].[Данные всех периодов].[2010].[Полугодие 2].[Квартал 4].[Октябрь]
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddMonths(-1), 4);
            Label1.Text = String.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            //Label3.Text = String.Format("Темп роста фактических показателей исполнения бюджета за {0} {1} {2} г. к аналогичному периоду предыдущего года по ВологО", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            TextBox1.Text = "Отдельные показатели исполнения бюджета ВологО";
            TextBox2.Text = String.Format("за {0} {1} {2} г., млн.руб.", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            query = DataProvider.GetQueryText("FO_0002_0002_incomes_h");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid);

            query = DataProvider.GetQueryText("FO_0002_0002_outcomes_h");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_deficite_h");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid2);

            dtGrid2.Columns[0].ColumnName = "Имя";

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_finsources_h");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();

            UltraWebGrid1.DataSource = dtGrid;
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 3;
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N1");

            e.Layout.Bands[0].Columns[0].Width = 109;
            e.Layout.Bands[0].Columns[1].Width = 67;
            e.Layout.Bands[0].Columns[2].Width = 67;
            e.Layout.Bands[0].Columns[3].Width = 49;
            e.Layout.Bands[0].Columns[4].Width = 67;
            e.Layout.Bands[0].Columns[5].Width = 67;
            e.Layout.Bands[0].Columns[6].Width = 49;

            headerLayout.AddCell("Имя");
            GridHeaderCell cell = headerLayout.AddCell("Конс. бюджет");
            cell.AddCell("План на год");
            cell.AddCell("Факт");
            cell.AddCell("% исп.");

            cell = headerLayout.AddCell("Обл. бюджет");
            cell.AddCell("План на год");
            cell.AddCell("Факт");
            cell.AddCell("% исп.");

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[3].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[4].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[5].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[6].Header.Style.Font.Size = FontUnit.Parse("14px");
        }
    }
}