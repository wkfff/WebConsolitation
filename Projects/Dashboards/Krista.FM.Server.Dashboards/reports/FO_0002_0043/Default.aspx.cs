using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0043
{
    public partial class Default : CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DataTable chartDt = new DataTable();
        private int firstYear = 2000;
        private DateTime currentDate;

        #region Параметры запроса

        // выбранный показатель
        private CustomParam selectedIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedIndicator = UserParams.CustomParam("selected_indicator");

            #endregion

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight*0.5 - 100);
            GridBrick.Width = CustomReportConst.minScreenWidth;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.None;

            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
            GridBrick.Grid.ActiveRowChange +=new ActiveRowChangeEventHandler(grid_ActiveRowChange);

            #region Настройка диаграммы

            ChartBrick.Width = CustomReportConst.minScreenWidth - 25;
            ChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 - 100);

            ChartBrick.DataFormatString = "N0";
            ChartBrick.DataItemCaption = "Тыс.руб.";
            ChartBrick.TooltipFormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE:N1>");
            ChartBrick.XAxisExtent = 100;
            ChartBrick.YAxisExtent = 70;
            ChartBrick.SwapRowAndColumns = true;
            ChartBrick.ZeroAligned = true;
            ChartBrick.Legend.Visible = false;
            ChartBrick.ColorModel = ChartColorModel.PureRandom;
            
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartCaption);
                chartWebAsyncPanel.AddRefreshTarget(ChartBrick.Chart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(GridBrick.Grid);

                DateTime lastDate = CubeInfoHelper.MonthReportIncomesInfo.LastDate;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), true);

                hiddenIndicatorLabel.Text = "ИТОГО ДОХОДОВ ";
                chartCaption.Text = GetChartCaption(hiddenIndicatorLabel.Text.TrimEnd(' '), lastDate.Year);
            }

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, ComboMonth.SelectedIndex + 1, 1);
            DateTime nextMonthDate = currentDate.AddMonths(1);

            Page.Title = "Отдельные показатели исполнения консолидированного бюджета";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на {0:dd.MM.yyyy}", nextMonthDate);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            GridDataBind();

            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            ChartDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0043_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            GridBrick.DataTable = gridDt;

            if (gridDt.Columns.Count > 1)
            {
                FontRowLevelRule rule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                rule.AddFontLevel("0", GridBrick.BoldFont10pt);
                
                GridBrick.AddIndicatorRule(rule);
            }
        }
        
        private void grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (e.Row == null)
                return;

            string indicatorName = e.Row.Cells[0].Text;

            hiddenIndicatorLabel.Text = indicatorName;
            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            chartCaption.Text = GetChartCaption(indicatorName.TrimEnd(' '), currentDate.Year);

            ChartBrick.DataBind();
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(840);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Наименование показателей");
            headerLayout.AddCell("Фактически исполнено, тыс.руб.");
            headerLayout.ApplyHeaderInfo();
        }

        private static string GetChartCaption(string indicatorName, int year)
        {
            return String.Format("Помесячная динамика показателя «{0}» за {1}-{2} годы", indicatorName, year - 2, year);
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0043_chart");
            chartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", chartDt);

            if (chartDt.Rows.Count > 0)
            {
                if (chartDt.Columns.Count > 0)
                {
                    chartDt.Columns.RemoveAt(0);
                }

                ChartBrick.DataTable = chartDt;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 0.8;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
            ReportExcelExporter1.Export(ChartBrick.Chart, chartCaption.Text, sheet2, 3);
       }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
            ChartBrick.Chart.Width = Convert.ToInt32(ChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(ChartBrick.Chart, chartCaption.Text, section2);
        }

        #endregion
    }
}