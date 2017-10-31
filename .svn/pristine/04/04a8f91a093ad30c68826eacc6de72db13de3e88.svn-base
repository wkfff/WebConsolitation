using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components.ChartBricks;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0010
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dateDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DataTable chart1Dt = new DataTable();
        private DataTable chart2Dt = new DataTable();
        private DateTime currentDate;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion
            
            #region Настройка диаграмм

            SetChartAppearance(chartBrick1);
            SetChartAppearance(chartBrick2);
            
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dateDt = new DataTable();
                string query = DataProvider.GetQueryText("FO_0035_0010_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dateDt);

                if (dateDt.Rows.Count > 0)
                {
                    currentDate = CRHelper.PeriodDayFoDate(dateDt.Rows[0][1].ToString());
                }

                currentDate = currentDate.AddDays(1);

                CustomCalendar.WebCalendar.SelectedDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
            }

            currentDate = CustomCalendar.WebCalendar.SelectedDate;
            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", currentDate.AddDays(-1), 5);

            Page.Title = "Сведения о денежных средствах на лицевом счете областного бюджета";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по состоянию на {0:dd.MM.yyyy}", currentDate);

            chartCaption1.Text = "Остаток средств к распределению (с учетом дотаций из ФБ)";
            chartCaption2.Text = "Поступления без учета невыясненных и средств ФБ";

            GridDataBind();
            ChartsDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0010_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    FontRowLevelRule rule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                    rule.AddFontLevel("0", GridBrick.BoldFont10pt);
                    rule.AddFontLevel("1", GridBrick.BoldFont8pt);
                    GridBrick.AddIndicatorRule(rule);
                }

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(500);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Показатели", "Показатели мониторинга остатков");
            headerLayout.AddCell("Сумма, тыс.руб.", "Сумма на текущую дату");
            headerLayout.ApplyHeaderInfo();
        }

       #endregion
        
        #region Обработчики диаграммы

        private static void SetChartAppearance(PeriodStackAreaChartBrick chartBrick)
        {
            chartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            chartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 - 100);

            chartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            
            chartBrick.SwapRowAndColumns = true;
            chartBrick.DataFormatString = "N2";
            chartBrick.DataItemCaption = "Тыс.руб.";
            chartBrick.YAxisLabelFormat = "N0";
            chartBrick.TooltipFormatString = "по состоянию на <SERIES_LABEL>\n<DATA_VALUE:N2> тыс.руб.";

            chartBrick.Legend.Visible = false;
//            chartBrick.Legend.Location = LegendLocation.Bottom;
//            chartBrick.Legend.SpanPercentage = 23;

            chartBrick.PeriodSpan = new TimeSpan(1, 0, 0, 0);
        }

        private void ChartsDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0010_chart1");
            chart1Dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", chart1Dt);

            if (chart1Dt.Rows.Count > 0)
            {
                chartBrick1.DataTable = chart1Dt;
            }

            query = DataProvider.GetQueryText("FO_0035_0010_chart2");
            chart2Dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", chart2Dt);

            if (chart2Dt.Rows.Count > 0)
            {
                chartBrick2.DataTable = chart2Dt;
            }
        }
        
        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма План");
            ReportExcelExporter1.Export(chartBrick1.Chart, chartCaption1.Text, sheet2, 3);

            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма Факт");
            ReportExcelExporter1.Export(chartBrick2.Chart, chartCaption2.Text, sheet3, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();
            
            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();

            chartBrick1.Width = Unit.Pixel(Convert.ToInt32(chartBrick1.Chart.Width.Value / 3));
            chartBrick2.Width = Unit.Pixel(Convert.ToInt32(chartBrick1.Chart.Width.Value / 3));

//            section1.PageSize = new PageSize(section2.PageSize.Height, Convert.ToInt32(chartBrick1.Chart.Width.Value));
            ReportPDFExporter1.Export(chartBrick1.Chart, chartCaption1.Text, section2);
            ReportPDFExporter1.Export(chartBrick2.Chart, chartCaption2.Text, section2);
        }

        #endregion
    }
}