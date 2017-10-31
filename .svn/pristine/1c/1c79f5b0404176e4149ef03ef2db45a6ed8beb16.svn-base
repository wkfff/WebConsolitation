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
    public partial class FO_0002_0062_1 : CustomReportPage
    {
        private DataTable dtTable, dtDataPrevYear;

        private int endYear;
        private GridHeaderLayout headerLayout;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0062_rests_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            string dimension = "";

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            DateTime newDate = new DateTime(endYear, 1, 1);
            dimension = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", newDate, 1);
            UserParams.PeriodDimension.Value = dimension;
            UserParams.PeriodHalfYear.Value = "";
            dtTable = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0062_table");

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ГРБС", dtTable);

            //теперь для сравнения с пред годом найдем данные на пред год
            DateTime newDate2 = new DateTime(endYear - 1, 1, 1);
            dimension = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", newDate2, 1);
            UserParams.PeriodDimension.Value = dimension;


            UserParams.PeriodHalfYear.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Конс.бюджет субъекта] ,";
            dtDataPrevYear = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0062_table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ГРБС", dtDataPrevYear);

            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataSource = dtTable;
            /*decimal sum = 0;
            for (int i = 0; i < dtTable.Rows.Count - 1; i++)
            {
                object val1 = dtTable.Rows[i]["Расходы на 1 работника"];
                if (val1 != DBNull.Value)
                {
                    sum += (decimal)val1;
                }
            }
            int indexLastRow = dtTable.Rows.Count - 1;
            dtTable.Rows[indexLastRow]["Расходы на 1 работника"] = sum;

            if (dtDataPrevYear.Rows.Count > 0 && dtDataPrevYear.Rows[0][1] != DBNull.Value)
            {
                decimal sum2 = 0;
                for (int i = 0; i < dtDataPrevYear.Rows.Count - 1; i++)
                {
                    object val1 = dtDataPrevYear.Rows[i]["Расходы на 1 работника"];
                    if (val1 != DBNull.Value)
                    {
                        sum2 += (decimal)val1;
                    }
                }
                int indexLastRow2 = dtDataPrevYear.Rows.Count - 1;
                dtDataPrevYear.Rows[indexLastRow]["Расходы на 1 работника"] = sum2;
            }*/
            UltraWebGrid.DataBind();

            UltraWebGrid.Width = Unit.Empty;

            lbDescription.Text =
                String.Format(
                    "Анализ расходов на содержание органов государственной власти за&nbsp;<b><span class='DigitsValue'>{0}</span></b>&nbsp;год, тыс. руб.",
                    endYear);
        }

        void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            setStar(e, 2);
            setTemp(e, 3);
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                e.Row.Cells[i].Style.Padding.Bottom = 2;
                e.Row.Cells[i].Style.Padding.Top = 2;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[0].Style.Padding.Right = 2;

            }
            if (e.Row.Cells[0].Value.ToString().ToLower() == "итого")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }
            /* if (e.Row.Cells[4].Value != null)
             {
                 e.Row.Cells[0].Value = e.Row.Cells[4].Value;
             }*/
        }

        private int borderWidth = 3;

        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {

            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            UltraWebGrid.Columns[0].Width = 191;
            UltraWebGrid.Columns[1].Width = 191;
            UltraWebGrid.Columns[2].Width = 191;
            UltraWebGrid.Columns[3].Width = 191;

            //скроем ненужных нам 2 поля
            UltraWebGrid.Columns[4].Hidden = true;
            UltraWebGrid.Columns[5].Hidden = true;
            UltraWebGrid.Columns[6].Hidden = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);
            headerLayout.AddCell("ГРБС");
            headerLayout.AddCell("Расходы на содержание ОГВ");
            headerLayout.AddCell("Среднесписочная численность");
            headerLayout.AddCell("Расходы на 1 работника");
        }

        void setStar(RowEventArgs e, int i)
        {
            string img = "";
            //string title;
            object val = dtTable.Rows[e.Row.Index][i];
            if (val != DBNull.Value)
            {
                val = dtTable.Rows[e.Row.Index][i+3];
                if (val != DBNull.Value)
                    if (e.Row.Cells[i].Value.ToString() == e.Row.Cells[i + 3].Value.ToString())
                    {
                        img = "~/images/max.png";

                    }
                val = dtTable.Rows[e.Row.Index][i+4];
                if (val != DBNull.Value)
                    if (e.Row.Cells[i].Value.ToString() == e.Row.Cells[i + 4].Value.ToString())
                    {
                        img = "~/images/min.png";
                    }
                e.Row.Cells[i].Style.BackgroundImage = img;
                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 30% center; padding-left: 2px";
            }
        }
        void setTemp(RowEventArgs e, int i)
        {
            string img = "";
            if (dtDataPrevYear.Rows.Count > 0 && dtDataPrevYear.Rows[0][1] != DBNull.Value && dtDataPrevYear.Rows.Count > e.Row.Index)
            {
                object val = dtDataPrevYear.Rows[e.Row.Index][i];
                if (val != DBNull.Value)
                {
                    decimal valueLastYear = (decimal)val;
                    val = e.Row.Cells[i].Value;
                    if (val != DBNull.Value)
                    {
                        decimal value = (decimal)val;

                        if (value > valueLastYear)
                        { img = "~/images/arrowRedUpBB.png"; }
                        else
                        { img = "~/images/arrowGreenDownBB.png"; }
                        e.Row.Cells[i].Style.BackgroundImage = img;
                        /*e.Row.Cells[i].Title = (img == "~/images/arrowRedUpBB.png")
       ? "Рост по отношению к предыдущему году"
       : "Сокращение по отношению к предыдущему году";*/
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 30% center; padding-left: 2px";
                    }
                }

            }
            /*else
            {
                object val = e.Row.Cells[i].Value;
                if (val != DBNull.Value)
                {
                    decimal value = (decimal)val;
                    if (value > 0)
                    { img = "~/images/arrowRedUpBB.png"; }
                    else
                    { img = "~/images/arrowGreenDownBB.png"; }
                    e.Row.Cells[i].Style.BackgroundImage = img;
                    e.Row.Cells[i].Title = (img == "~/images/arrowRedUpBB.png")
    ? "Рост по отношению к предыдущему году"
    : "Сокращение по отношению к предыдущему году";
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 30% center; padding-left: 2px";
                }
            }*/
        }
    }
}