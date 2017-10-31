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
    public partial class FO_0003_0006 : CustomReportPage
    {
        private GridHeaderLayout headerLayout;

        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();

        DateTime date;
        DateTime taxesDate;
        DateTime debtDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            CustomParam periodDebtDate = UserParams.CustomParam("period_debt_date");
            CustomParam periodTaxesDate = UserParams.CustomParam("period_taxes_date");

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0003_0006_date");
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

            DataTable dtDateDebts = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0006_date_debts");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDateDebts);
            if (dtDateDebts.Rows.Count > 0)
            {
                periodDebtDate.Value = dtDateDebts.Rows[0][1].ToString();
                debtDate = CRHelper.DateByPeriodMemberUName(dtDateDebts.Rows[0][1].ToString(), 3);
            }
            else
            {
                periodDebtDate.Value = UserParams.PeriodCurrentDate.Value;
                debtDate = date;
            }

            DataTable dtDateTaxes = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0006_date_taxes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDateTaxes);
            if (dtDateTaxes.Rows.Count > 0)
            {
                periodTaxesDate.Value = dtDateTaxes.Rows[0][1].ToString();
                taxesDate = CRHelper.DateByPeriodMemberUName(dtDateTaxes.Rows[0][1].ToString(), 3);
            }
            else
            {
                periodTaxesDate.Value = UserParams.PeriodCurrentDate.Value;
                taxesDate = date;
            }

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid2_InitializeLayout);

            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid2_InitializeRow);

            SetupGrid(UltraWebGrid1, "FO_0003_0006_grid1");
            SetupGrid(UltraWebGrid2, "FO_0003_0006_grid2");
            lbDescription.Text = String.Format("Показатели исполнения бюджетов субъекта РФ&nbsp;<span class='DigitsValue'>на {0:dd.MM.yyyy}</span>, тыс.руб.", date.AddMonths(1));
        }

        void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[5].Value.ToString() == "1")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "2")
            {
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "3")
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[5].Value.ToString() == "4")
            {
                e.Row.Cells[0].Style.Font.Italic = true;
                e.Row.Cells[0].Style.Padding.Left = 20;
            }

            e.Row.Cells[1].Value = e.Row.Cells[1].Value.ToString().ToLower();
            if (e.Row.Cells[1].Value.ToString().Contains("проц"))
            {
                for (int i = 2; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:P0}", e.Row.Cells[i].Value);
                    }
                }
            }

            if (e.Row.Index > 0 && e.Row.Index < 5 && debtDate != date)
            {
                e.Row.Cells[0].Value = String.Format("{0} (на {1:dd.MM.yyyy})", e.Row.Cells[0].Value, debtDate.AddMonths(1));
            }
            if (e.Row.Index > 4 && e.Row.Index < 7 && taxesDate != date)
            {
                e.Row.Cells[0].Value = String.Format("{0} (на {1:dd.MM.yyyy})", e.Row.Cells[0].Value, taxesDate.AddMonths(1));
            }
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[7].Value.ToString() == "1")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[7].Value.ToString() == "2")
            {
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[7].Value.ToString() == "3")
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[7].Value.ToString() == "4")
            {
                e.Row.Cells[0].Style.Font.Italic = true;
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 235;

            e.Layout.Bands[0].Columns[1].Width = 130;
            e.Layout.Bands[0].Columns[2].Width = 130;
            e.Layout.Bands[0].Columns[3].Width = 130;
            e.Layout.Bands[0].Columns[4].Width = 130;

            e.Layout.Bands[0].Columns[5].Hidden = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("Показатель");
            headerLayout.AddCell("Ед. измерения");
            headerLayout.AddCell("2009 год");
            headerLayout.AddCell("2010 год");  
            headerLayout.AddCell(String.Format("на {0:dd.MM.yyyy}", date.AddMonths(1)));

            headerLayout.ApplyHeaderInfo();
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 208;

            e.Layout.Bands[0].Columns[1].Width = 91;
            e.Layout.Bands[0].Columns[2].Width = 91;
            e.Layout.Bands[0].Columns[3].Width = 91;
            e.Layout.Bands[0].Columns[4].Width = 91;
            e.Layout.Bands[0].Columns[5].Width = 91;
            e.Layout.Bands[0].Columns[6].Width = 91;

            e.Layout.Bands[0].Columns[7].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthLeft = 3;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("Показатель");

            GridHeaderCell cell = headerLayout.AddCell("Исполнение 2009 год");
            cell.AddCell("конс. бюджет");
            cell.AddCell("в т.ч. бюджет субъекта");

            cell = headerLayout.AddCell("Исполнение 2010 год");
            cell.AddCell("конс. бюджет");
            cell.AddCell("в т.ч. бюджет субъекта");

            cell = headerLayout.AddCell(String.Format("Исполнение на {0:dd.MM.yyyy}", date.AddMonths(1)));
            cell.AddCell("конс. бюджет");
            cell.AddCell("в т.ч. бюджет субъекта");

            headerLayout.ApplyHeaderInfo();
        }
        
        private void SetupGrid(UltraWebGrid grid, string queryName)
        {
            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;
            
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

       
    }
}
