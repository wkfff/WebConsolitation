using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Shared.Events;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0015
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear;
        private int endYear = 2011;
        private string month = "Январь";
        private bool internalCirculatoinExtrude = false;

        private bool GrowRateRanking
        {
            get { return Convert.ToBoolean(growRateRanking.Value); }
        }

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // тип документа СКИФ для местных бюджетов
        private CustomParam localBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для местных бюджетов
        private CustomParam localBudgetSKIFLevel;

        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;

        // выводить ранги для темпа роста
        private CustomParam growRateRanking;

        // элемент доходы итого
        private CustomParam incomesTotalItem;
        // элемент безвозмездные поступления
        private CustomParam gratuitousIncomesItem;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        // уровень бюджета
        private CustomParam level;

        //ГРБС
        private CustomParam grbsName;

        //Год в классификатор Бюджет
        private CustomParam numYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (level == null)
            {
                level = UserParams.CustomParam("level");
            }

            if (grbsName == null)
            {
                grbsName = UserParams.CustomParam("grbsName");
            }

            if (numYear == null)
            {
                numYear = UserParams.CustomParam("numYear");
            }

            incomesTotal = UserParams.CustomParam("incomes_total");
            regionsLevel = UserParams.CustomParam("regions_level");
            consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            growRateRanking = UserParams.CustomParam("grow_rate_ranking");
            incomesTotalItem = UserParams.CustomParam("incomes_total_item");
            gratuitousIncomesItem = UserParams.CustomParam("gratuitous_incomes_item");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");

            #endregion

            growRateRanking.Value = RegionSettingsHelper.Instance.GetPropertyValue("GrowRateRanking");

            if (GrowRateRanking)
            {
                PopupInformer1.HelpPageUrl = "Default_GrowRateRanking.html";
            }

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 270);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.InitializeLayout +=
                new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);


            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth/2 - 30);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.45);

            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 160;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Legend.Visible = false;
            UltraChart1.Legend.Location = LegendLocation.Right;
            UltraChart1.Legend.SpanPercentage = 15;
            UltraChart1.Legend.Margins.Bottom = (int)(UltraChart1.Height.Value / 2);
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Margin.Near.Value = 2;
            UltraChart1.Axis.Y.Margin.Near.Value = 2;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.PieChart.StartAngle = 270;
            UltraChart1.PieChart.Labels.Font = new System.Drawing.Font("Verdana", 10);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth/2 - 30);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.45);

            UltraChart2.ChartType = ChartType.PieChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.";
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.Axis.X.Extent = 160;
            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Legend.Visible = false;
            UltraChart2.Legend.Location = LegendLocation.Right;
            UltraChart2.Legend.SpanPercentage = 15;
            UltraChart2.Legend.Margins.Bottom = (int)(UltraChart1.Height.Value / 2);
            UltraChart2.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Margin.Near.Value = 2;
            UltraChart2.Axis.Y.Margin.Near.Value = 2;
            UltraChart2.PieChart.OthersCategoryPercent = 0;
            UltraChart2.PieChart.StartAngle = 270;
            UltraChart2.PieChart.Labels.Font = new System.Drawing.Font("Verdana", 10);

            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 1.5 );

            UltraChart3.ChartType = ChartType.ColumnChart;
            UltraChart3.Border.Thickness = 0;

            UltraChart3.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн. руб.";
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.Axis.X.Extent = 0;
            UltraChart3.Axis.X.Labels.Visible = true;
            UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart3.Axis.X.Labels.ItemFormatString = "";
            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            UltraChart3.Legend.SpanPercentage = 65;
            UltraChart3.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Margin.Near.Value = 2;
            UltraChart3.Axis.Y.Margin.Near.Value = 2;
            UltraChart3.Data.SwapRowsAndColumns = true;
            UltraChart3.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart3.ColumnChart.SeriesSpacing = 0;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport +=
                new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "Темп роста доходов бюджетов муниципальных образований";
            CrossLink1.NavigateUrl = "";
            CrossLink1.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            internalCirculatoinExtrude = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("InternalCirculationExtrude"));
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            base.Page_Load(sender, e);
            CustomCalendar1.WebPanel.Expanded = false;
            if (!Page.IsPostBack)
            {
                Panel1.AddLinkedRequestTrigger(UltraWebGrid);
                Panel1.AddRefreshTarget(UltraChart1);
                Panel1.AddRefreshTarget(ChartCaption1);
                Panel1.AddRefreshTarget(UltraChart2);
                Panel1.AddRefreshTarget(ChartCaption2);
                Panel2.AddRefreshTarget(UltraChart1);
                Panel2.AddRefreshTarget(ChartCaption1);
                Panel2.AddRefreshTarget(UltraChart2);
                Panel2.AddRefreshTarget(ChartCaption2);
                Panel2.LinkedRefreshControlID = Panel1.ID;
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0015_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                       Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                // Инициализируем календарь
                CustomCalendar1.WebCalendar.SelectedDate = date;

            }

            Page.Title = "Анализ ведомственной структуры расходов областного бюджета Новосибирской области";
            Label1.Text = Page.Title;

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, CustomCalendar1.WebCalendar.SelectedDate, 5);
            string datel = CustomCalendar1.WebCalendar.SelectedDate.ToString("dd.MM.yyyy");
            Label2.Text = String.Format("Данные по состоянию на {0} (млн. руб.)", datel);
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            numYear.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Year.ToString());
            CRHelper.SaveToErrorLog(numYear.Value);

            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotalItem.Value = internalCirculatoinExtrude
                  ? "Доходы бюджета без внутренних оборотов "
                  : "Доходы бюджета c внутренними оборотами ";
            gratuitousIncomesItem.Value = internalCirculatoinExtrude
                  ? "Безвозмездные поступления без внутренних оборотов "
                  : "Безвозмездные поступления c внутренними оборотами ";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraWebGrid.Rows[0].Activate();
            grbsName.Value = UltraWebGrid.Rows[0].Cells[0].ToString();
            ChartCaption1.Text = UltraWebGrid.Rows[0].Cells[0].ToString() + ", удельный вес в общем объеме бюджетных назначений по расходам, %";
            ChartCaption2.Text = UltraWebGrid.Rows[0].Cells[0].ToString() + ", удельный вес в общем объеме фактического исполнения по расходам, %";
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0015_compare_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtGrid);

           
                UltraWebGrid.DataSource = dtGrid;
            
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(320);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            for (int k = 2; k < e.Layout.Bands[0].Columns.Count; k++)
            {

                string formatString = "N2";
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 116;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
            }
            e.Layout.Bands[0].Columns[3].Format = "P2";
            e.Layout.Bands[0].Columns[5].Format = "P2";
            e.Layout.Bands[0].Columns[1].Format = "000";
            if (e.Layout.Bands[0].Columns.Count > 0)
            {
                e.Layout.Bands[0].Columns[2].Header.Caption = "Сумма, млн.руб.";
                e.Layout.Bands[0].Columns[3].Header.Caption = "Удельный вес в общих расходах";
                e.Layout.Bands[0].Columns[4].Header.Caption = "Сумма, млн.руб.";
                e.Layout.Bands[0].Columns[5].Header.Caption = "Удельный вес в общих расходах";
                e.Layout.Bands[0].Columns[2].Header.Title = "Уточненные плановые назначения по расходам, млн. руб.";
                e.Layout.Bands[0].Columns[3].Header.Title = "Удельный вес уточненных плановых назначений конкретного ГРБС в общих уточненных плановых расходах, %";
                e.Layout.Bands[0].Columns[4].Header.Title = "Фактическое исполнение по расходам, млн. руб.";
                e.Layout.Bands[0].Columns[5].Header.Title = "Удельный вес расходов конкретного ГРБС в общих расходах, %";
            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "План (уточненный)";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 2;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Факт за текущий период";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 4;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].ToString().Contains("ИТОГО"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }

        private static string TrimName(string name)
        {
            while (Char.IsDigit(name[0]))
            {
                name = name.Remove(0, 1);
            }
            return name;
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        private void ActiveGridRow(UltraGridRow row)
        {

            if (row == null)
                return;
            if (row.Index < 1 &&
                Panel1.IsAsyncPostBack)
            {
                return;
            }
            grbsName.Value = row.Cells[0].Value.ToString();
            ChartCaption1.Text = row.Cells[0].Value.ToString() + ", удельный вес в общем объеме бюджетных назначений по расходам, %";
            ChartCaption2.Text = row.Cells[0].Value.ToString() + ", удельный вес в общем объеме фактического исполнения по расходам, %";
            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }


        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {

            string queryName = "FO_0001_0015_compare_chart1";
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            foreach (DataRow row in dtChart.Rows)
            {
                row[0] = row[0].ToString().Replace("\"", "'");
            }
            UltraChart1.DataSource = dtChart;

        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {

            string queryName = "FO_0001_0015_compare_chart2";
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            foreach (DataRow row in dtChart.Rows)
            {
                row[0] = row[0].ToString().Replace("\"", "'");
            }
            UltraChart2.DataSource = dtChart;

        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {

            string queryName = "FO_0001_0015_compare_chart3_1";
            if (RList.SelectedValue == "Факт")
            { 
                queryName = "FO_0001_0015_compare_chart3_2"; 
            }
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            foreach (DataRow row in dtChart.Rows)
            {
                row[0] = row[0].ToString().Replace("\"", "'");
            }
            UltraChart3.DataSource = dtChart;


        }
        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 65;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = label;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 30 * 3;
            e.CurrentWorksheet.Columns[1].Width = width * 30;
            e.CurrentWorksheet.Columns[2].Width = width * 30;
            e.CurrentWorksheet.Columns[3].Width = width * 30;
            e.CurrentWorksheet.Columns[4].Width = width * 30;
            e.CurrentWorksheet.Columns[5].Width = width * 30;
            e.CurrentWorksheet.Columns[6].Width = width * 30;
            e.CurrentWorksheet.Columns[7].Width = width * 30;
            e.CurrentWorksheet.Columns[8].Width = width * 30;
            e.CurrentWorksheet.Columns[9].Width = width * 30;
            e.CurrentWorksheet.Columns[10].Width = width * 30;
            e.CurrentWorksheet.Columns[11].Width = width * 30;
            e.CurrentWorksheet.Columns[12].Width = width * 30;
            e.CurrentWorksheet.Columns[13].Width = width * 30;
            e.CurrentWorksheet.Columns[14].Width = width * 30;

            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                e.CurrentWorksheet.Rows[4].Cells[i].Value = e.CurrentWorksheet.Rows[4].Cells[i].Value.ToString().Replace("<br/>", "");
                e.CurrentWorksheet.Rows[6].Cells[i].CellFormat.FillPattern = FillPatternStyle.None;
            }
            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 2; i < columnCounttt; i = i + 1)
            {

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            for (int i = 3; i < 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 20 * 35;
            }
            e.CurrentWorksheet.Rows[5].Height = 20 * 15;
            for (int k = 1; k < columnCounttt; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if ((e.CurrentColumnIndex > 1) && (e.CurrentColumnIndex < 4))
            {
                e.HeaderText = "План (уточненный)";
            }
            else if (e.CurrentColumnIndex > 3)
            {
                e.HeaderText = "Факт за текущий период";
            }

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Темп роста доходов");
            Worksheet sheet2 = workbook.Worksheets.Add("Факт");
            Worksheet sheet3 = workbook.Worksheets.Add("План");
            Worksheet sheet4 = workbook.Worksheets.Add("Показатели");
            sheet2.Rows[0].Cells[0].Value = ChartCaption1.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[1].Cells[0], UltraChart1);
            sheet3.Rows[0].Cells[0].Value = ChartCaption2.Text;
            UltraGridExporter.ChartExcelExport(sheet3.Rows[1].Cells[0], UltraChart2);
            sheet4.Rows[0].Cells[0].Value = ChartCaption3.Text;
            UltraGridExporter.ChartExcelExport(sheet4.Rows[1].Cells[0], UltraChart3);
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
            e.Section.AddPageBreak();
            
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption2.Text);
            img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);
            e.Section.AddPageBreak();
            
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption3.Text);
            img = UltraGridExporter.GetImageFromChart(UltraChart3);
            e.Section.AddImage(img);
        }

        #endregion

        public int sts { get; set; }
    }
}
