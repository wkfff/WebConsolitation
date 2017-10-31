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
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.FNS_0006_0006
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid1 = new DataTable();
        private DataTable dtGrid2 = new DataTable();
        private DataTable dtGrid3 = new DataTable();
        private int firstYear;
        private int endYear = 2011;
        private string month = "Январь";
        private string meas = string.Empty;
        private CustomParam mul;
        private bool internalCirculatoinExtrude = false;
        private static MemberAttributesDigest okvedDigest;
        // вычислители рангов
        RankCalculator mrRank = new RankCalculator(RankDirection.Desc);
        RankCalculator goRank = new RankCalculator(RankDirection.Desc);
        private bool GrowRateRanking
        {
            get { return Convert.ToBoolean(growRateRanking.Value); }
        }

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // выбранный регион
        private CustomParam selectedRegion;
        // предыдущий год
        private CustomParam predYear;
        // позапрошлый год
        private CustomParam predpredYear;
        // группа доходов
        private CustomParam fnsKDGroup;

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

        // уровень бюджета
        private CustomParam OKVDGroup;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }

            if (level == null)
            {
                level = UserParams.CustomParam("level");
            }
            if (fnsKDGroup == null)
            {
                fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            }
            if (OKVDGroup == null)
            {
                OKVDGroup = UserParams.CustomParam("selected_OKVD");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (predYear == null)
            {
                predYear = UserParams.CustomParam("predYear");
            }
            if (predpredYear == null)
            {
                predpredYear = UserParams.CustomParam("predpredYear");
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

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.71);
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
            UltraGridExporter1.MultiHeader = false;

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
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            firstYear = 2009;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0006_0006_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState((endYear).ToString(), true);

                ComboMonth.Title = "Месяц";

                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            DateTime currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue.ToString()), CRHelper.MonthNum(ComboMonth.SelectedValue), 01);
            currDateTime = currDateTime.AddMonths(1);
            string incom = string.Empty;
            string date = String.Format("{0:dd.MM.yyyy}", currDateTime);
            if (RadioList.SelectedIndex == 0)
            {
                mul.Value = Convert.ToString(1000);
                meas = "тыс.руб";
            }
            else
            {
                mul.Value = Convert.ToString(1000000);
                meas = "млн.руб";
            }
            Page.Title = "Информация об исполнении консолидированного бюджета субъекта";
            Label1.Text = String.Format("Информация об исполнении консолидированного бюджета субъекта, по состоянию на 01.{0:MM.yyyy}г., {1}", currDateTime, meas);
            Label2.Text = string.Empty;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            predYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1);
            predpredYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2);
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

        private bool IsRankingRow(string rowName)
        {
            return rowName != "Консолидированный бюджет" && rowName != "Бюджет субъекта" &&
            rowName != "Городские округа" && rowName != "Муниципальные районы" &&
            rowName != RegionSettingsHelper.Instance.OwnSubjectBudgetName;
        }


        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FNS_0006_0006_compare_grid1");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtGrid1);
            query = DataProvider.GetQueryText("FNS_0006_0006_compare_grid2");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtGrid2);
            query = DataProvider.GetQueryText("FNS_0006_0006_compare_grid3");
            dtGrid3 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtGrid3);
            dtGrid1.Rows.RemoveAt(0);
            dtGrid1.AcceptChanges();
           if ((dtGrid1.Rows.Count > 0) || (dtGrid2.Rows.Count > 0))
            {
                for (int i = 0; i < dtGrid2.Rows.Count; i++)
                {
                    dtGrid1.ImportRow(dtGrid2.Rows[i]);
                }
            }
            if ((dtGrid1.Rows.Count > 0) || (dtGrid3.Rows.Count > 0))
            {
                for (int i = 0; i < dtGrid3.Rows.Count; i++)
                {
                    dtGrid1.ImportRow(dtGrid3.Rows[i]);
                }
            }
            dtGrid1.AcceptChanges();
            UltraWebGrid.DataSource = dtGrid1;
        }


        private RankCalculator GetRankCalculator(DataRow row)
        {
            if (row[0].ToString().Contains("район") && !row[0].ToString().Contains("районы"))
            {
                return mrRank;
            }
            else
                if (row[0].ToString().Contains("г.") || row[0].ToString().Contains("ЗАТО"))
                {
                    return goRank;
                }
            return null;
        }




        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //if (UltraWebGrid != null && UltraWebGrid.Rows.Count < 30)без прокрутки
            //{
            // UltraWebGrid.Height = Unit.Empty;
            //}
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(260);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 1;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.SpanY = 1;
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {
                string formatString = "N2";
                if (k > 2)
                {
                    formatString = "N2";
                }
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 130;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 0;
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.SpanY = 1;
            }
            e.Layout.Bands[0].Columns[1].Header.Title = "Уточненные годовые назначения на текущую дату";
            e.Layout.Bands[0].Columns[2].Header.Title = "Фактически исполнено на текущую дату";
            e.Layout.Bands[0].Columns[3].Header.Title = "Процент исполнения уточненного годового назначения";
            e.Layout.Bands[0].Columns[4].Header.Title = "Темп роста фактического исполнения на текущую дату к аналогичному периоду прошлого года";

        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 4; (i < UltraWebGrid.Columns.Count); i += 1)
            {
                if (UltraWebGrid.Rows[e.Row.Index].Cells[0].ToString().Contains("ОБРАЗОВАНИЕ") ||
                UltraWebGrid.Rows[e.Row.Index].Cells[0].ToString().Contains("КУЛЬТУРА, КИНЕМАТОГРАФИЯ") ||
                UltraWebGrid.Rows[e.Row.Index].Cells[0].ToString().Contains("ЗДРАВООХРАНЕНИЕ") ||
                UltraWebGrid.Rows[e.Row.Index].Cells[0].ToString().Contains("СОЦИАЛЬНАЯ ПОЛИТИКА") ||
                UltraWebGrid.Rows[e.Row.Index].Cells[0].ToString().Contains("Физическая культура и спорт") ||
                UltraWebGrid.Rows[e.Row.Index].Cells[0].ToString().Contains("СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ"))
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Рост";
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
                else
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Рост";
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }


            }


            int levelColumnIndex = e.Row.Cells.Count - 1;
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0] != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    if ((e.Row.Cells[0].ToString().Contains("ВСЕГО")) || 
                        (e.Row.Cells[0].ToString() == "НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ") ||
                        (e.Row.Cells[0].ToString() == "ДОХОДЫ ОТ ПРИНОСЯЩЕЙ ДОХОД ДЕЯТЕЛЬНОСТИ") ||
                        (e.Row.Cells[0].ToString().Contains("ДЕФИЦИТ/ПРОФИЦИТ") ||
                        (e.Row.Cells[0].ToString().Contains("ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ  БЮДЖЕТОВ"))))
                    {

                        bold = true;
                        italic = false;

                    }

                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    if ((e.Row.Cells[0].ToString() == "ДОХОДЫ ") || (e.Row.Cells[0].ToString() == "РАСХОДЫ") || (e.Row.Cells[0].ToString() == "ИСТОЧНИКИ ФИНАНСИРОВАНИЯ ДЕФИЦИТА"))
                    {
                        e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                    if ((e.Row.Cells[0].ToString().Contains("налог")) || (e.Row.Cells[0].ToString().Contains("Налог")) || (e.Row.Cells[0].ToString().Contains("Дотации")) || (e.Row.Cells[0].ToString().Contains("Субсидии")) || (e.Row.Cells[0].ToString().Contains("Субвенции")) || (e.Row.Cells[0].ToString().Contains("трансферты")) || (e.Row.Cells[0].ToString().Contains("безвозмездные поступления")))  //|| (e.Row.Cells[0].ToString().Contains("Налоги") == false) || (e.Row.Cells[0].ToString().Contains("налоги") == false))
                    {
                        e.Row.Cells[i].Style.Padding.Left = 20;
                    }
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
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
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
            e.CurrentWorksheet.Columns[15].Width = width * 30;
            e.CurrentWorksheet.Columns[16].Width = width * 30;
            e.CurrentWorksheet.Columns[17].Width = width * 30;
            e.CurrentWorksheet.Columns[18].Width = width * 30;
            e.CurrentWorksheet.Columns[19].Width = width * 30;
            int columnCountt = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                if ((i == 3) || (i == 4) || (i == 5) || (i == 6) || (i == 7) || (i == 17) || (i == 18))
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                }
                else
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                }
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0;[Red]-#,##0"; ;
            for (int k = 1; k < columnCounttt; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            for (int i = 3; i < UltraWebGrid.Rows.Count + 7; i = i + 1)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = e.CurrentWorksheet.Rows[i].Height * 3;
            }

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {


        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Доходы");
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

        public int sts { get; set; }
    }


}
