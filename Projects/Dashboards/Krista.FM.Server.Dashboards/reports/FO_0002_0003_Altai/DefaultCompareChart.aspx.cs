using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003_Altai
{
    public partial class DefaultCompareChart : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2011;
        private int rubMultiplier;

        private GridHeaderLayout headerLayout1;

        private bool UseConsolidateRegionBudget
        {
            get { return useConsolidateRegionBudget.Checked; }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        #region Параметры запроса

        private CustomParam selectedRegion;
        // доходы Итого
        private CustomParam incomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // уровень МО
        private CustomParam regionsLevel;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;
        // Вид дохода
        private CustomParam kindOfIncomes;

        // тип документа СКИФ для местных бюджетов
        private CustomParam localBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для местных бюджетов
        private CustomParam localBudgetSKIFLevel;
        
        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }

            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            if (kindOfIncomes == null)
            {
                kindOfIncomes = UserParams.CustomParam("kind_Of_Incomes");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");

            #endregion

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 160);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 160);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackAreaChart;
            UltraChart.Axis.X.Extent = 100;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.Y.Extent = IsThsRubSelected ? 70 : 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 25;
            //UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value / 3);

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart.TitleLeft.Text = RubMultiplierCaption;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent + (UltraChart.Legend.SpanPercentage * Convert.ToInt32(UltraChart.Height.Value)) / 100;

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE_ITEM:N1> {0}", RubMiltiplierButtonList.SelectedValue);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            UltraChart.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "Сравнение&nbsp;темпа&nbsp;роста&nbsp;фактических&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0003/DefaultCompare.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;

            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 !=
                                                          "null"
                                                              ? string.Format(",{2}.[{1}].[ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ].[{0}],",
                                                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                                                    RegionSettingsHelper.Instance.IncomesKDRootName,
                                                                    RegionSettingsHelper.Instance.IncomesKDAllLevel)
                                                              : ",";
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsLocalBudgetLevel");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 400;
                ComboRegion.ParentSelect = true;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillLocalBudgets(RegionsNamingHelper.LocalBudgetTypes));
                ComboRegion.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboIncomes.Width = 420;
                ComboIncomes.Title = "Вид дохода";
                ComboIncomes.MultiSelect = true;
                ComboIncomes.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboIncomes.ParentSelect = true;
                ComboIncomes.FillDictionaryValues(FillFullKD());
                ComboIncomes.SetСheckedState("Налоговые доходы ", true);
                ComboIncomes.SetСheckedState("Неналоговые доходы ", true);
                ComboIncomes.SetСheckedState("Безвозмездные поступления ", true);
            }

            bool regionSelected = false;
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(ComboRegion.SelectedValue))
            {
                regionSelected = RegionsNamingHelper.LocalBudgetTypes[ComboRegion.SelectedValue] == "МР";
            }
            useConsolidateRegionBudget.Visible = regionSelected;

            string selectedBudget = UseConsolidateRegionBudget && regionSelected
                            ? String.Format("{0} (Консолидированный бюджет МО)", ComboRegion.SelectedValue)
                            : String.Format("{0}", ComboRegion.SelectedValue);

            Page.Title = String.Format("Структурная динамика фактических доходов ({0})", selectedBudget);
            Label1.Text = Page.Title;
            
            switch (ComboRegion.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        selectedRegion.Value = String.Format("{0}.[Консолидированный бюджет субъекта ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Муниципальные районы":
                    {
                        selectedRegion.Value =  String.Format("{0}.[Муниципальные районы ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Городские округа":
                    {
                        selectedRegion.Value = String.Format("{0}.[Городские округа ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                default:
                    {
                        selectedRegion.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue];
                        break;
                    }
            }

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodEndYear.Value = yearNum.ToString();
            UserParams.PeriodYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodFirstYear.Value = (yearNum - 2).ToString();
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            
            string kdList = String.Empty;
            foreach (string kd in ComboIncomes.SelectedValues)
            {
                kdList += String.Format("{0}.[{1}],", UserParams.IncomesKDDimension.Value, kd);
            }
            kindOfIncomes.Value = kdList.TrimEnd(',');
            
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");

            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");

            regionBudgetSKIFLevel.Value = UseConsolidateRegionBudget && regionSelected
                ? RegionSettingsHelper.Instance.GetPropertyValue("ConsMOBudgetSKIFLevel")
                : RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            
            UltraWebGrid.Bands.Clear();
            
            if  (kindOfIncomes.Value != String.Empty)
            {
                UltraChart.DataBind();
                headerLayout1 = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.DataBind();
            }
        }

        public static Dictionary<string, int> FillFullKD()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Налоговые доходы ", 0);
            valuesDictionary.Add("Налог на прибыль ", 1);
            valuesDictionary.Add("НДФЛ ", 1);
            valuesDictionary.Add("Налоги на имущество ", 1);
            valuesDictionary.Add("Налог на имущество физ.лиц ", 2);
            valuesDictionary.Add("Налог на имущество организаций ", 2);
            valuesDictionary.Add("Транспортный налог ", 2);
            valuesDictionary.Add("Транспортный налог с организации ", 3);
            valuesDictionary.Add("Транспортный налог с физ.лиц ", 3);
            valuesDictionary.Add("Налог на игорный бизнес ", 2);
            valuesDictionary.Add("Земельный налог ", 2);
            valuesDictionary.Add("Акцизы ", 1);
            valuesDictionary.Add("Акцизы на нефтепродукты ", 2);
            valuesDictionary.Add("Акцизы на алкоголь ", 2);
            valuesDictionary.Add("Налоги на совокупный доход ", 1);
            valuesDictionary.Add("УСН ", 2);
            valuesDictionary.Add("ЕНВД ", 2);
            valuesDictionary.Add("ЕСХН ", 2);
            valuesDictionary.Add("НДПИ ", 1);
            valuesDictionary.Add("НД общераспространенных ПИ ", 2);
            valuesDictionary.Add("НД прочих ПИ (без природных алмазов) ", 2);
            valuesDictionary.Add("Сборы за пользование объектами жив.мира и за пользование объектами вод.биол.ресурсов ", 1);
            valuesDictionary.Add("Сбор за объекты жив.мира ", 2);
            valuesDictionary.Add("Сбор за объекты водных биол.ресурсов (по вн.водным объектам) ", 2);
            valuesDictionary.Add("Гос.пошлина ", 1);
            valuesDictionary.Add("Задолженность по отмененным налогам ", 1);
            valuesDictionary.Add("Неналоговые доходы ", 0);
            valuesDictionary.Add("Доходы от использования имущества ", 1);
            valuesDictionary.Add("Дивиденды по акциям ", 2);
            valuesDictionary.Add("Арендная плата за земли ", 2);
            valuesDictionary.Add("Доходы от сдачи в аренду имущества ", 2);
            valuesDictionary.Add("Платежи от ГУПов и МУПов ", 2);
            valuesDictionary.Add("Платежи при пользовании природными ресурсами ", 1);
            valuesDictionary.Add("Плата за негативное воздействие на окруж.среду ", 2);
            valuesDictionary.Add("Платежи за пользование лесным фондом ", 2);
            valuesDictionary.Add("Доходы от оказания платных услуг ", 1);
            valuesDictionary.Add("Доходы от продажи активов ", 1);
            valuesDictionary.Add("Доходы от продажи активов (кроме зем.участков) ", 2);
            valuesDictionary.Add("Доходы от продажи зем. участков ", 2);
            valuesDictionary.Add("Штрафы ", 1);
            valuesDictionary.Add("Доходы от приносящей доход деятельности ", 0);
            valuesDictionary.Add("Безвозмездные поступления ", 0);
            return valuesDictionary;
        }


        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_compare_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtChart);

            foreach (DataColumn column in dtChart.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
            }

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0003_compare_chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            List<string> yearList = new List<string>();
            if (dtChart.Rows.Count > 0)
            {
                for (int j = 0; j < dtChart.Rows.Count; j++)
                {
                    DataRow row = dtChart.Rows[j];
                    // если строка первая, либо в первой ячейке Январь, либо предыдущий Декабрь
                    if (row[0] != DBNull.Value && dt.Rows[j][0] != DBNull.Value &&
                            (j == 0 ||
                             row[0].ToString() == "Январь" ||
                             (j != 0 && dtChart.Rows[j - 1][0].ToString() == "Декабрь")))
                    {
                        row[0] = string.Format("{0} - {1}", dt.Rows[j][0], row[0]);
                        yearList.Add(dt.Rows[j][0].ToString());
                    }
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                        }
                    }
                }

                if (yearList.Count == 1)
                {
                    Label2.Text = string.Format("Сравнение структуры помесячного поступления доходов за {0} год",
                        yearList[0]);
                }
                else if (yearList.Count > 1)
                {
                    Label2.Text = string.Format("Сравнение структуры помесячного поступления доходов за {0} - {1} годы",
                        yearList[0], yearList[yearList.Count - 1]);
                }

                UltraChart.DataSource = dtChart;
            }
            else
            {
                UltraChart.DataSource = null;
            }
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0003_compare_addGrid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                DataTable newDt = dtGrid.Clone();

                string curYear = string.Empty;
                foreach (DataRow row in dtGrid.Rows)
                {
                    string year = (row[1] != DBNull.Value) ? row[1].ToString() : string.Empty;
                    if (curYear != year)
                    {
                        DataRow newRow = newDt.NewRow();
                        newRow[0] = RegionsNamingHelper.CheckMultiplyValue(year, 3);
                        newDt.Rows.Add(newRow);
                        curYear = year;
                    }
                    for (int i = 2; i < row.ItemArray.Length; i++)
                    {
                        if (i%2 == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i])/rubMultiplier;
                        }
                        else if (row[i] != DBNull.Value && row[i].ToString() != string.Empty &&
                                 dtGrid.Columns[i].DataType == typeof (string))
                        {
                            row[i] = (Convert.ToDouble(row[i].ToString())).ToString("N4");
                        }
                    }
                    newDt.ImportRow(row);
                }
                newDt.Columns.RemoveAt(1);
                newDt.AcceptChanges();

                UltraWebGrid.DataSource = newDt;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(90);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 25;

                int j = i % 2;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "N1";
                            widthColumn = 90;
                            break;
                        }
                    case 1:
                        {
                            formatString = "N1";
                            widthColumn = 90;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }
           
           headerLayout1.AddCell("Период");
           for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                string headerCaption = (captions.Length > 0) ? captions[0] : string.Empty;

                GridHeaderCell cell = headerLayout1.AddCell(string.Format("{0}", headerCaption));
                cell.AddCell(string.Format("Исполнено, {0}", RubMiltiplierButtonList.SelectedValue), "");
                cell.AddCell("Доля, %", "Удельный вес в общей сумме доходов");
            }

            headerLayout1.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            bool isYear = false;
            if (e.Row.Cells[0] != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                Decimal value;
                isYear = Decimal.TryParse(e.Row.Cells[0].Value.ToString(), out value);
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell c = e.Row.Cells[i];
                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    if (isYear)
                    {
                        c.Style.Font.Bold = true;
                        c.ColSpan = e.Row.Band.Columns.Count;
                    }

                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт 

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            UltraChart.Width = 800;
            ReportPDFExporter1.Export(UltraChart, section1);
            ReportPDFExporter1.Export(headerLayout1, section2);
        }

        #endregion

        #endregion
    }
}
