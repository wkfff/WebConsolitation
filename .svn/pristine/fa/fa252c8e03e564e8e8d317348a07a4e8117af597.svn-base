using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0005 : CustomReportPage
    {
        private GridHeaderLayout headerLayout;

        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();

        DateTime date;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0003_0005_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UserParams.PeriodYear.Value = date.Year.ToString();
            UserParams.PeriodLastYear.Value = date.AddYears(-1).Year.ToString();
            CustomParam periodLastLastYear = UserParams.CustomParam("period_last_last_year");
            CustomParam periodThreeYearAgo = UserParams.CustomParam("period_3_last_year");
            CustomParam periodFourYearAgo = UserParams.CustomParam("period_4_last_year");
            periodLastLastYear.Value = date.AddYears(-2).Year.ToString();
            periodThreeYearAgo.Value = date.AddYears(-3).Year.ToString();
            periodFourYearAgo.Value = date.AddYears(-4).Year.ToString();

            SetupGrid(UltraWebGrid1, "FO_0003_0005_incomes");
            SetupGrid(UltraWebGrid2, "FO_0003_0005_incomes");
            UltraWebGrid2.DisplayLayout.Bands[0].HeaderLayout.Clear();

            lbDescription.Text = String.Format("Показатели исполнения бюджетов субъекта РФ&nbsp;<span class='DigitsValue'>на {0:dd.MM.yyyy}</span>, тыс.руб.", date.AddMonths(1));
        }

        void UltraWebGridIncomes_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[13].Value.ToString() == "1")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[13].Value.ToString() == "2")
            {
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[13].Value.ToString() == "3")
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[13].Value.ToString() == "4")
            {
                e.Row.Cells[0].Style.Font.Italic = true;
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[0].Value.ToString().Contains("Недостаток") ||
                e.Row.Cells[0].Value.ToString().Contains("дефицит ("))
            {
                SetConditionCorner(e, 1);
                SetConditionCorner(e, 2);
                SetConditionCorner(e, 5);
                SetConditionCorner(e, 6);
                SetConditionCorner(e, 7);
                SetConditionCorner(e, 8);
                SetConditionCorner(e, 9);
                SetConditionCorner(e, 10);
                SetConditionCorner(e, 12);
            }
        }

        public void SetConditionCorner(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Math.Round(Convert.ToDouble(e.Row.Cells[index].Value.ToString()));
                string img = string.Empty;
                if (value < 0)
                {
                    img = "~/images/cornerRed.gif";

                }
                else if (value > 0)
                {
                    img = "~/images/cornerGreen.gif";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-left: 2px";
            }
        }
        
        private void SetupGrid(UltraWebGrid grid, string queryName)
        {
            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
            grid.InitializeRow += new InitializeRowEventHandler(UltraWebGridIncomes_InitializeRow);
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);
                        
            grid.DataSource = dt;

            //foreach (DataRow row in dt.Rows)
            //{
            //    TableRow tableRow = new TableRow();
            //    for (int i = 0; i < dt.Columns.Count; i++)
            //    {
            //        TableCell cell = new TableCell();
            //        cell.Text = row[i].ToString();
            //        tableRow.Cells.Add(cell);

            //    }
            //    detailTable.Rows.Add(tableRow);

            //}
            //detailTable.Attributes.Add("unselectable", "on");

            grid.DataBind();
        }

        void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 300;

            e.Layout.Bands[0].Columns[1].Width = 110;
            e.Layout.Bands[0].Columns[2].Width = 110;
            e.Layout.Bands[0].Columns[3].Width = 110;
            e.Layout.Bands[0].Columns[4].Width = 110;
            e.Layout.Bands[0].Columns[5].Width = 110;

            e.Layout.Bands[0].Columns[6].Width = 110;
            e.Layout.Bands[0].Columns[7].Width = 110;
            e.Layout.Bands[0].Columns[8].Width = 110;
            e.Layout.Bands[0].Columns[9].Width = 110;
            e.Layout.Bands[0].Columns[10].Width = 110;

            e.Layout.Bands[0].Columns[11].Width = 130;
            e.Layout.Bands[0].Columns[12].Width = 110;

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);


            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "N0");

            e.Layout.Bands[0].Columns[13].Hidden = true;

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("Показатель");

            GridHeaderCell cell = headerLayout.AddCell("2009 год");
            GridHeaderCell childCell = cell.AddCell("Исполнение год");
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");

            cell = headerLayout.AddCell("2010 год");
            cell.AddCell("План");
            childCell = cell.AddCell("Исполнение год");
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");

            cell = headerLayout.AddCell("2011 год");
            cell.AddCell("План");
            childCell = cell.AddCell("Уточненный план");
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");

            childCell = cell.AddCell(String.Format("Исполнение на {0:dd.MM.yyyy}", date.AddMonths(1)));
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");

            childCell = cell.AddCell("Темп к соотв.периоду прошл.года");
            childCell.AddCell("по субъекту");
            cell.AddCell("Оценка Минфина России 2011");

            headerLayout.ApplyHeaderInfo();
        }
    }
}
