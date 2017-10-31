using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0037 : CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            currentDate = CubeInfoHelper.FoMonthReportDebtInfo.LastDate;

            lbDescription.Text = String.Format("Кредиторская задолженность консолидированного бюджета Новосибирской области по состоянию на&nbsp;<span class=\"DigitsValue\">{0:dd.MM.yyyy}</span>&nbsp;г., тыс.руб.", 
                new DateTime(currentDate.Year + 1, 1, 1));

            UltraWebGrid.Grid.EnableAppStyling = DefaultableBoolean.False;
            UltraWebGrid.BrowserSizeAdapting = false;
            UltraWebGrid.Height = 700;
            UltraWebGrid.Width = 780;
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            GridDataBind();
        }

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0037_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 0)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", new Font("Arial", 13, FontStyle.Bold), Color.White);
                levelRule.AddFontLevel("1", new Font("Arial", 12), Color.White);
                levelRule.AddFontLevel("2", new Font("Arial", 12), Color.FromArgb(150, 150, 150));
                UltraWebGrid.AddIndicatorRule(levelRule);

                PaddingRule paddingRule = new PaddingRule("Наименование", "Уровень", 10);
                UltraWebGrid.AddIndicatorRule(paddingRule);

                UltraWebGrid.DataTable = gridDt;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(50);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(355);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 2; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(160);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;
            headerLayout.AddCell("");
            headerLayout.AddCell("Наименование");
            headerLayout.AddCell("Сумма задолженности");
            headerLayout.AddCell("в том числе просроченная (нереальная к взысканию)");
            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != String.Empty)
            {
                e.Row.Cells[0].Value = String.Format("<a href='webcommand?showPinchReport={0}'><img style='padding-top: 5px' src='../../../images/Detail.png'/></a>",
                        e.Row.Cells[0].Value);
            }
        }
    }
}