using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0039 : CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DataTable chartDt = new DataTable();
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            currentDate = CubeInfoHelper.FoMonthReportDebtInfo.LastDate;

            lbDescription.Text = String.Format("Кредиторская задолженность главных распорядителей областного бюджета Новосибирской области<br/>по состоянию на&nbsp;<span class=\"DigitsValue\">{0:dd.MM.yyyy}</span>&nbsp;г., тыс.руб.",
                new DateTime(currentDate.Year + 1, 1, 1));
            
            #region Настройка диаграммы

            UltraChart.BrowserSizeAdapting = false;
            UltraChart.Width = 755;
            UltraChart.Height = 320;

            UltraChart.TooltipFormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL>\n<b><DATA_VALUE:N2></b> тыс.руб.\n<b><PERCENT_VALUE:N2>%</b></span>";
            UltraChart.DataFormatString = "N1";
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 30;
            UltraChart.Legend.Font = new Font("Verdana", 11);
            UltraChart.ColorModel = ChartColorModel.IphoneColors;
            UltraChart.OthersCategoryPercent = 0;
            UltraChart.TemporaryUrlPrefix = "../../../";

            #endregion

            #region Настройка грида

            UltraWebGrid.Grid.EnableAppStyling = DefaultableBoolean.False;
            UltraWebGrid.RedNegativeColoring = false;
            UltraWebGrid.BrowserSizeAdapting = false;
            UltraWebGrid.Width = 780;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            #endregion

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            ChartDataBind();
            GridDataBind();
        }

        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            string queryText = DataProvider.GetQueryText("FO_0035_0039_chart");
            chartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                if (chartDt.Columns.Count > 0)
                {
                    chartDt.Columns.RemoveAt(0);
                }

                UltraChart.DataTable = chartDt;
            }
        }

        #endregion

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0039_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", new Font("Arial", 13, FontStyle.Bold), Color.White);
                UltraWebGrid.AddIndicatorRule(levelRule);

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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(255);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(235);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;
            headerLayout.AddCell("Главные распорядители");
            headerLayout.AddCell("Сумма задолженности");
            headerLayout.AddCell("в том числе просроченная (нереальная к взысканию)");
            headerLayout.ApplyHeaderInfo();
        }

        #endregion
    }
}