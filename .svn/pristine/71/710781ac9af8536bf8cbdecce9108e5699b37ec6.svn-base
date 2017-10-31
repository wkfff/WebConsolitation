using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;


namespace Krista.FM.Server.Dashboards.reports.FNS_0007_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear;
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;

        private bool internalCirculatoinExtrude = false;

        private bool GrowRateRanking
        {
            get { return Convert.ToBoolean(growRateRanking.Value); }
        }

        #region Параметры запроса

        private CustomParam year;
        private CustomParam predYear;
        private CustomParam predPredYear;
        private CustomParam regionUniqueName;
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

        // консолидированный элемент районов
        private CustomParam consolidateRegionElement;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (level == null)
            {
                level = UserParams.CustomParam("level");
            }
            if (year == null)
            {
                year = UserParams.CustomParam("year");
            }
            if (predYear == null)
            {
                predYear = UserParams.CustomParam("predYear");
            }
            if (predPredYear == null)
            {
                predPredYear = UserParams.CustomParam("predPredYear_");
            }

            regionUniqueName = UserParams.CustomParam("regionUniqueName");

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

            consolidateRegionElement = UserParams.CustomParam("consolidate_region_element");

            #endregion

            growRateRanking.Value = RegionSettingsHelper.Instance.GetPropertyValue("GrowRateRanking");

            if (GrowRateRanking)
            {
                PopupInformer1.HelpPageUrl = "Default_GrowRateRanking.html";
            }

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 300);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.InitializeLayout +=
                new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

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

            consolidateRegionElement.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            regionUniqueName.Value = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}].[{1}]", RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), RegionSettings.Instance.Name);
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0007_0001_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2008, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);


            }

            Page.Title = "Динамика поступлений налоговых и неналоговых доходов в консолидированный бюджет и ВРП региона";
            Label1.Text = Page.Title;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            DateTime currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue.ToString()), CRHelper.MonthNum("Январь"), 01);
            string date = String.Format("{0:dd.MM.yyyy}", currDateTime);
            year.Value = ComboYear.SelectedValue.ToString();
            predYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
            predPredYear.Value = (Convert.ToInt32(predYear.Value) - 1).ToString();
            Label2.Text = String.Format("Поступления налоговых и неналоговых доходов бюджета и ВРП за {0} год, в сравнении с {1}-{2} годами (млн. руб.)", year.Value, predPredYear.Value, predYear.Value);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
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
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0007_0001_compare_grid1");
            DataTable dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Виды экономической деятельности", dtGrid1);

            query = DataProvider.GetQueryText("FNS_0007_0001_compare_grid2");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Виды", dtGrid2);

            for (int i = 0; i < dtGrid1.Rows.Count; i = i + 1)
            {
                for (int j = 3; j < dtGrid1.Columns.Count; j = j + 2)
                {
                    if (dtGrid2.Rows[i][j] == DBNull.Value)
                    {
                        dtGrid1.Rows[i][j] = DBNull.Value;
                    }
                    else
                        dtGrid1.Rows[i][j] = string.Format("{0:N1}", dtGrid2.Rows[i][j]);
                }
            }

            UltraWebGrid.DataSource = dtGrid1;
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
			e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(100);
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            for (int k = 2; k < e.Layout.Bands[0].Columns.Count; k++)
            {

                string formatString = "N1";
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 106;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
            }
            headerLayout.AddCell("Виды экономической деятельности", "Разделы общероссийского классификатора видов экономической деятельности");
            headerLayout.AddCell("Код раздела", "Код раздела общероссийского классификатора видов экономической деятельности");
            GridHeaderCell cell = headerLayout.AddCell(predPredYear.Value);
            cell.AddCell("Налоговые и неналоговые доходы, млн.руб.", "Поступление налоговых и неналоговых доходов за год");
            cell.AddCell("ВРП отрасли, млн.руб", "Валовый региональный продукт в основных ценах по ОКВЭД за год");
            cell.AddCell("Структура доходов, %", "Удельный вес поступлений по данному виду экономической деятельности к общей сумме поступлений");
            cell.AddCell("Отраслевая структура ВРП, %", "Отраслевая структура валовой добавленной стоимости по ОКВЭД ");

            for (int i = 0; i < 2; i++)
            {
                cell = headerLayout.AddCell((Convert.ToInt32(predYear.Value) + i).ToString());
                cell.AddCell("Налоговые и неналоговые доходы, млн.руб.");
                cell.AddCell("ВРП отрасли, млн.руб");
                cell.AddCell("Структура доходов, %");
                cell.AddCell("Отраслевая структура ВРП, %");
                cell.AddCell("Темп роста доходов, %");
                cell.AddCell("Темп роста ВРП отрасли, %");
            }
            headerLayout.ApplyHeaderInfo();
        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {

            for (int i = 10; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Сокращение по отношению к прошлому году";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост по отношению к прошлому году";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                if (i == 11)
                {
                    i = 15;
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
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
			string label = Label2.Text.Replace("<br/>", "");
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");            
            sheet1.Rows[0].Cells[0].Value = Page.Title;
            sheet1.Rows[1].Cells[0].Value = label;
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
			IText title = section1.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);
            ReportPDFExporter1.Export(headerLayout,Label2.Text.Replace("<br/>", "/n"), section1);
        }
        #endregion
        /*
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
           
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 2; i < columnCounttt; i = i + 1)
            {
              
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "0.0";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
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
            if ((e.CurrentColumnIndex > 1) && (e.CurrentColumnIndex < 6))
            {
                e.HeaderText = predPredYear.Value;
            }
            if ((e.CurrentColumnIndex > 5) && (e.CurrentColumnIndex < 12))
            {
                e.HeaderText = predYear.Value;
            }
            if ((e.CurrentColumnIndex > 11) && (e.CurrentColumnIndex < 18))
            {
                e.HeaderText = year.Value;
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
*/
        public int sts { get; set; }
    }
}
