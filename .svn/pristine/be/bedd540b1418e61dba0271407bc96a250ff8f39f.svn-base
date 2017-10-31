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
    public partial class FO_0003_0002 : CustomReportPage
    {
        private GridHeaderLayout headerLayout;

        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();

        DateTime date;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0003_0002_date");
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

            SetupGrid(UltraWebGrid1, "FO_0003_0002_incomes_all");
            SetupGrid(UltraWebGrid2, "FO_0003_0002_incomes_all");
            SetupGrid(UltraWebGrid3, "FO_0003_0002_incomes4");

            UltraWebGrid2.DisplayLayout.Bands[0].HeaderLayout.Clear();
            //SetupGrid("FO_0003_0002_incomes_all");

            lbDescription.Text = String.Format("Показатели исполнения бюджетов субъекта РФ&nbsp;<span class='DigitsValue'>на {0:dd.MM.yyyy}</span>, тыс.руб.", date.AddMonths(1));

        }

        void UltraWebGridIncomes_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[21].Value != null)
            {
                if (e.Row.Cells[21].Value.ToString() == "1")
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                    e.Row.Cells[0].Style.ForeColor = Color.White;
                }
                if (e.Row.Cells[21].Value.ToString() == "2")
                {
                    e.Row.Cells[0].Style.ForeColor = Color.White;
                }
                if (e.Row.Cells[21].Value.ToString() == "3")
                {
                    e.Row.Cells[0].Style.Padding.Left = 20;
                }
                if (e.Row.Cells[21].Value.ToString() == "4")
                {
                    e.Row.Cells[0].Style.Font.Italic = true;
                    e.Row.Cells[0].Style.Padding.Left = 20;
                }
            }
            if (e.Row.Cells[0].Value.ToString().Contains("Доля межбюджетных трансфертов из федерального бюджета (за исключением субвенций) в доходах"))
            {
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:P0}", e.Row.Cells[i].Value);
                    }
                }
            }

            if (e.Row.Cells[0].Value.ToString().Contains("бюджетной обеспеченности") &&
                !e.Row.Cells[0].Value.ToString().Contains("выравнивание"))
            {
                if (e.Row.Cells[0].Value.ToString().Contains("Место в РФ"))
                {
                    if (e.Row.Cells[1].Value != null)
                    {
                        e.Row.Cells[1].Value = String.Format("{0:N0}", e.Row.Cells[1].Value);
                    }
                    e.Row.Cells[1].ColSpan = 4;
                    e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;

                    if (e.Row.Cells[6].Value != null)
                    {
                        e.Row.Cells[5].Value = String.Format("{0:N0}", e.Row.Cells[6].Value);
                    }
                    e.Row.Cells[5].ColSpan = 5;
                    e.Row.Cells[5].Style.HorizontalAlign = HorizontalAlign.Center;

                    if (e.Row.Cells[10].Value != null)
                    {
                        e.Row.Cells[10].Value = String.Format("{0:N0}", e.Row.Cells[10].Value);
                    }
                    e.Row.Cells[10].ColSpan = 11;
                    e.Row.Cells[10].Style.HorizontalAlign = HorizontalAlign.Center;
                }
                else
                {
                    if (e.Row.Cells[1].Value != null)
                    {
                        e.Row.Cells[1].Value = String.Format("{0:P0}", e.Row.Cells[1].Value);
                    }
                    e.Row.Cells[1].ColSpan = 4;
                    e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;

                    if (e.Row.Cells[6].Value != null)
                    {
                        e.Row.Cells[5].Value = String.Format("{0:P0}", e.Row.Cells[6].Value);
                    }
                    e.Row.Cells[5].ColSpan = 5;
                    e.Row.Cells[5].Style.HorizontalAlign = HorizontalAlign.Center;

                    if (e.Row.Cells[10].Value != null)
                    {
                        e.Row.Cells[10].Value = String.Format("{0:P0}", e.Row.Cells[10].Value);
                    }
                    e.Row.Cells[10].ColSpan = 11;
                    e.Row.Cells[10].Style.HorizontalAlign = HorizontalAlign.Center;
                }

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
            grid.DataBind();
        }

        private void SetupGrid(string queryName)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

            foreach (DataRow row in dt.Rows)
            {
                TableRow tableRow = new TableRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    TableCell cell = new TableCell();
                    cell.Text = row[i].ToString();
                    if (i == 0)
                    {
                        cell.Style.Add("width", "300px");
                        cell.Text = row[i].ToString();
                    }
                    else
                    {
                        if (row[i] != DBNull.Value)
                        {
                            cell.Text = String.Format("{0:N2}", row[i]);
                        }
                    }
                    cell.CssClass = "HtmlTableCompact";
                    tableRow.Cells.Add(cell);
                }
                //detailTable.Rows.Add(tableRow);

            }

            foreach (DataRow row in dt.Rows)
            {
                TableRow tableRow = new TableRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    TableCell cell = new TableCell();
                    if (i == 0)
                    {
                        cell.Style.Add("width", "300px");
                        cell.Text = row[i].ToString();
                    }
                    else
                    {
                        if (row[i] != DBNull.Value)
                        {
                            cell.Text = String.Format("{0:N2}", row[i]);
                        }
                    }
                    cell.CssClass = "HtmlTableCompact";
                    tableRow.Cells.Add(cell);
                }
               // fixedTable.Rows.Add(tableRow);
            }
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
            e.Layout.Bands[0].Columns[11].Width = 110;

            e.Layout.Bands[0].Columns[12].Width = 110;
            e.Layout.Bands[0].Columns[13].Width = 110;
            e.Layout.Bands[0].Columns[14].Width = 110;
            e.Layout.Bands[0].Columns[15].Width = 110;
            e.Layout.Bands[0].Columns[16].Width = 110;
            e.Layout.Bands[0].Columns[17].Width = 110;
            e.Layout.Bands[0].Columns[18].Width = 110;

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P0");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P0");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[13], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[14], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[15], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[16], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[17], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[18], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[19], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[20], "P0");

            e.Layout.Bands[0].Columns[21].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[9].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[10].CellStyle.BorderDetails.WidthLeft = 3;

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("Показатель");

            GridHeaderCell cell = headerLayout.AddCell("2009 год");
            GridHeaderCell childCell = cell.AddCell("Исполнение год");
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");
            cell.AddCell("Исполнение (год) к плану");
            cell.AddCell("Исполнение (год) к прошлому году");

            cell = headerLayout.AddCell("2010 год");
            cell.AddCell("План");
            childCell = cell.AddCell("Исполнение год");
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");
            cell.AddCell("Исполнение (год) к плану");
            cell.AddCell("Исполнение (год) к прошлому году");

            cell = headerLayout.AddCell("2011 год");
            cell.AddCell("План");
            childCell = cell.AddCell("Уточненный план");
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");

            cell.AddCell("Темп уточ. плана к исп. прош. года");
            childCell = cell.AddCell(String.Format("Исполнение на {0:dd.MM.yyyy}", date.AddMonths(1)));
            childCell.AddCell("конс. бюджет");
            childCell.AddCell("в т.ч. бюджет субъекта");

            childCell = cell.AddCell("Темп к соотв.периоду прошл.года");
            childCell.AddCell("по субъекту");
            childCell.AddCell("по ФО");
            childCell.AddCell("по РФ");
            cell.AddCell("Оценка Минфина России 2011");
            cell.AddCell("Темп к прошлому году");

            headerLayout.ApplyHeaderInfo();
        }
    }
}
