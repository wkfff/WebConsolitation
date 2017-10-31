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

namespace Krista.FM.Server.Dashboards.reports.UFK_0007_0002
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

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (level == null)
            {
                level = UserParams.CustomParam("level");
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

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 200);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.InitializeLayout +=
                new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

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
            //UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("UFK_0007_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

            }

            Page.Title = "Темп роста доходов бюджетов муниципальных образований";
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            DateTime currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue.ToString()), CRHelper.MonthNum(ComboMonth.SelectedValue), 01);
            currDateTime = currDateTime.AddMonths(1);
            string date = String.Format("{0:dd.MM.yyyy}", currDateTime);
            Label2.Text = String.Format("Данные на {0} (тыс. руб.)", date);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

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
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("UFK_0007_0002_compare_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальные образования", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                for (int k = 0; k < dtGrid.Rows.Count; k++)
                {
                    DataRow row = dtGrid.Rows[k];
                    string kd = dtGrid.Rows[k][0].ToString();
                    kd = TrimName(kd);
                    dtGrid.Rows[k][0] = kd;
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;

                        }
                    }
                }
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //if (UltraWebGrid != null && UltraWebGrid.Rows.Count < 30)без прокрутки
            //{
            //    UltraWebGrid.Height = Unit.Empty;
            //}
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {

                string formatString = "N2";
                if ((k == 3) || (k == 4) || (k == 5) || (k == 8) || (k == 9) || (k == 10) || (k == 13) || (k == 14) || (k == 15))
                {
                    formatString = "P2";
                }
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;

                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 106;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
            }

            e.Layout.Bands[0].Columns[1].Header.Caption = "Исполнено, <br/>тыс. руб.";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Исполнено за аналогичный период прошлого года,  <br/>тыс. руб.";
            e.Layout.Bands[0].Columns[3].Header.Caption = "Темп роста к прошлому году, %";
            e.Layout.Bands[0].Columns[4].Header.Caption = "Доля, %";
            e.Layout.Bands[0].Columns[5].Header.Caption = "Доля в прошлом году, %";
            e.Layout.Bands[0].Columns[6].Header.Caption = "Исполнено, <br/>тыс. руб.";
            e.Layout.Bands[0].Columns[7].Header.Caption = "Исполнено за аналогичный период прошлого года,  <br/>тыс. руб.";
            e.Layout.Bands[0].Columns[8].Header.Caption = "Темп роста к прошлому году, %";
            e.Layout.Bands[0].Columns[9].Header.Caption = "Доля, %";
            e.Layout.Bands[0].Columns[10].Header.Caption = "Доля в прошлом году, %";
            e.Layout.Bands[0].Columns[11].Header.Caption = "Исполнено,  <br/>тыс. руб.";
            e.Layout.Bands[0].Columns[12].Header.Caption = "Исполнено за аналогичный период прошлого года,  <br/>тыс. руб.";
            e.Layout.Bands[0].Columns[13].Header.Caption = "Темп роста к прошлому году, %";
            e.Layout.Bands[0].Columns[14].Header.Caption = "Доля, %";
            e.Layout.Bands[0].Columns[15].Header.Caption = "Доля в прошлом году, %";
            e.Layout.Bands[0].Columns[0].Header.Title = "Бюджеты муниципальных образований";
            e.Layout.Bands[0].Columns[1].Header.Title = "Исполнение нарастающим итогом с начала года";
            e.Layout.Bands[0].Columns[2].Header.Title = "Исполнено за аналогичный период прошлого года";
            e.Layout.Bands[0].Columns[3].Header.Title = "Темп роста исполнения к аналогичному периоду прошлого года";
            e.Layout.Bands[0].Columns[4].Header.Title = "Доля бюджета в доходах местных бюджетов";
            e.Layout.Bands[0].Columns[5].Header.Title = "Доля бюджета в доходах местных бюджетов в предыдущем году";
            e.Layout.Bands[0].Columns[6].Header.Title = "Исполнение нарастающим итогом с начала года";
            e.Layout.Bands[0].Columns[7].Header.Title = "Исполнено за аналогичный период прошлого года";
            e.Layout.Bands[0].Columns[8].Header.Title = "Темп роста исполнения к аналогичному периоду прошлого года";
            e.Layout.Bands[0].Columns[9].Header.Title = "Доля бюджета в доходах местных бюджетов";
            e.Layout.Bands[0].Columns[10].Header.Title = "Доля бюджета в доходах местных бюджетов в предыдущем году";
            e.Layout.Bands[0].Columns[11].Header.Title = "Исполнение нарастающим итогом с начала года";
            e.Layout.Bands[0].Columns[12].Header.Title = "Исполнено за аналогичный период прошлого года";
            e.Layout.Bands[0].Columns[13].Header.Title = "Темп роста исполнения к аналогичному периоду прошлого года";
            e.Layout.Bands[0].Columns[14].Header.Title = "Доля бюджета в доходах местных бюджетов";
            e.Layout.Bands[0].Columns[15].Header.Title = "Доля бюджета в доходах местных бюджетов в предыдущем году";

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Налоговые и неналоговые доходы";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 5;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Безвозмездные поступления";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 6;
            ch.RowLayoutColumnInfo.SpanX = 5;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Доходы, всего";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 11;
            ch.RowLayoutColumnInfo.SpanX = 5;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 3; i < e.Row.Cells.Count - 1; i += 5)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Сокращение по отношению к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост по отношению к прошлому году";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null &&
                           (e.Row.Cells[0].Value.ToString().Contains("Все районы")))
                {
                    e.Row.Cells[0].Value = "Местные бюджеты Новосибирской области";
                }
                if (e.Row.Cells[0].Value != null &&
                           (e.Row.Cells[0].Value.ToString().Contains("ДАННЫЕ")))
                {
                    e.Row.Cells[0].Value = "Собственный бюджет муниципального района";
                }
                if (e.Row.Cells[0].Value != null &&
                           ((e.Row.Cells[0].Value.ToString().Contains("район")) || (e.Row.Cells[0].Value.ToString().Contains("Городские округа")) || (e.Row.Cells[0].Value.ToString().Contains("Местные бюджеты Новосибирской области"))) &&
                           (e.Row.Cells[0].Value.ToString().Contains("Собственный бюджет муниципального района") == false))
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
                for (int j = 5; j < 20; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
            }
            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                if ((i == 3) || (i == 4) || (i == 5) || (i == 8) || (i == 9) || (i == 10) || (i == 13) || (i == 14) || (i == 15))
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00%;[Red]-#,##0.00%";
                }
                else
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                }
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
            if ((e.CurrentColumnIndex > 0) && (e.CurrentColumnIndex < 6))
            {
                e.HeaderText = "Налоговые и неналоговые доходы";
            }
            if ((e.CurrentColumnIndex > 5) && (e.CurrentColumnIndex < 11))
            {
                e.HeaderText = "Безвозмездные поступления";
            }
            if ((e.CurrentColumnIndex > 10) && (e.CurrentColumnIndex < 16))
            {
                e.HeaderText = "Доходы, всего";
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Темп роста доходов");
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
        }

        #endregion

        public int sts { get; set; }
    }
}
