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
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0053
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DataTable chartDt = new DataTable();
        private DateTime currentDate;
        private DateTime lastDate;
        private int firstYear = 2009;

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        #region Параметры запроса

        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;

        // текущий период
        private CustomParam currentPeriod;
        // прошлый период
        private CustomParam lastPeriod;
        // множитель млн/тыс руб.
        private CustomParam rubMutiplier;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            //GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 - 200);
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);

            #endregion

            #region Настройка диаграммы динамики

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.57 - 100);

            DynamicChartBrick.YAxisLabelFormat = "N2";
            DynamicChartBrick.DataFormatString = "N2";
            DynamicChartBrick.DataItemCaption = RubMultiplierCaption;
            DynamicChartBrick.Legend.Visible = true;
            DynamicChartBrick.Legend.Location = LegendLocation.Bottom;
            DynamicChartBrick.Legend.SpanPercentage = 10;
            DynamicChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            DynamicChartBrick.XAxisExtent = 100;
            DynamicChartBrick.YAxisExtent = 90;
            DynamicChartBrick.ZeroAligned = true;
            DynamicChartBrick.SwapRowAndColumns = true;
            DynamicChartBrick.TooltipFormatString = String.Format("<ITEM_LABEL> на <SERIES_LABEL> г.\n<DATA_VALUE_ITEM:N2> {0}", RubMultiplierCaption.ToLower());
            DynamicChartBrick.PeriodMonthSpan = 1;
            DynamicChartBrick.IconSize = SymbolIconSize.Medium;

            #endregion

            #region Инициализация параметров запроса

            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");
            currentPeriod = UserParams.CustomParam("current_period");
            lastPeriod = UserParams.CustomParam("last_period");
            rubMutiplier = UserParams.CustomParam("rub_mutiplier");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(GridBrick.Grid.ClientID);

                DateTime lastMonthDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0053_lastDate");

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastMonthDate.Year));
                ComboYear.SetСheckedState(lastMonthDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastMonthDate.Month)), true);

                selectedGridIndicator.Value = "Консолидированный бюджет субъекта";
                hiddenIndicatorLabel.Text = "Сеть государственных и муниципальных учреждений и органов власти";
            }

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int monthNum = CRHelper.MonthNum(ComboMonth.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            Page.Title = String.Format("Анализ остатков средств бюджетов");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на {0:dd.MM.yyyy} г., {1}", currentDate.AddMonths(1), RubMultiplierCaption.ToLower());

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();

            currentPeriod.Value = String.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", currentDate.Year,
                                                CRHelper.HalfYearNumByMonthNum(currentDate.Month),
                                                CRHelper.QuarterNumByMonthNum(currentDate.Month),
                                                CRHelper.RusMonth(currentDate.Month));

            lastDate = currentDate.AddMonths(-1);
            lastPeriod.Value = String.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", lastDate.Year,
                                    CRHelper.HalfYearNumByMonthNum(lastDate.Month),
                                    CRHelper.QuarterNumByMonthNum(lastDate.Month),
                                    CRHelper.RusMonth(lastDate.Month));

            rubMutiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0053_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 2);
                levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
                GridBrick.AddIndicatorRule(levelRule);

                GridBrick.AddIndicatorRule(new GrowRateRule(10, "Рост остатков средств бюджета относительно прошлого месяца", "Снижение остатков средств бюджета относительно прошлого месяца"));
                GridBrick.AddIndicatorRule(new GrowRateRule(13, "Рост остатков средств бюджета относительно начала года", "Снижение остатков средств бюджета относительно начала года"));

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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            for (int i = 1; i < columnCount - 2; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Наименование");

            AddGroupHeader(String.Format("На 01.01.{0}", currentDate.Year));
            AddGroupHeader(String.Format("На {0:dd.MM.yyyy}", lastDate.AddMonths(1)));
            AddGroupHeader(String.Format("На {0:dd.MM.yyyy}", currentDate.AddMonths(1)));
            AddGroupHeader("Отклонение от предыдущего месяца");
            AddGroupHeader("Отклонение от начала года");

            headerLayout.ApplyHeaderInfo();
        }

        private void AddGroupHeader(string groupName)
        {
            GridHeaderCell groupCell = GridBrick.GridHeaderLayout.AddCell(groupName);
            groupCell.AddCell("Всего", "Общая сумма остатков средств бюджета на отчетную дату");
            groupCell.AddCell("в т.ч. целевые", "Остатки целевых средств бюджета");
            groupCell.AddCell("собственные", "Остатки собственных средств бюджета");
        }

        private void Grid_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(GridBrick.Grid, selectedGridIndicator.Value, 0, 0);
                ActivateGridRow(row);
            }
        }

        private void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }

            string indicatorName = row.Cells[0].Text;

            hiddenIndicatorLabel.Text = indicatorName;
            selectedGridIndicator.Value = hiddenIndicatorLabel.Text;

            DynamicChartCaption.Text = String.Format("Динамика остатков средств бюджета в текущем году ({0})", indicatorName);
            DynamicChartDataBind();
        }

        #endregion

        #region Обработчики диаграммы

        private void DynamicChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0053_chart");
            chartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                DynamicChartBrick.DataTable = chartDt;
                DynamicChartBrick.DataBind();
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Структура");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Динамика");
            ReportExcelExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, sheet2, 3);
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
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, section1);

            ISection section2 = report.AddSection();
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, section2);
        }

        #endregion
    }
}